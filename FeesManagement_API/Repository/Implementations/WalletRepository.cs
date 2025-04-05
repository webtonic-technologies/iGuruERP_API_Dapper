using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse; 
using FeesManagement_API.Repository.Interfaces;
using Dapper;
using System.Data;

namespace FeesManagement_API.Repository.Implementations
{
    public class WalletRepository : IWalletRepository
    {
        private readonly IDbConnection _connection;

        public WalletRepository(IDbConnection connection)
        {
            _connection = connection;
        }
         
        public async Task<int> AddWalletAmount(AddWalletAmountRequest request)
        {
            var query = @"
            INSERT INTO tblStudentWallet (StudentID, Amount, PaymentModeID, Comment, InstituteID, JournalEntriesTypeID)
            VALUES (@StudentID, @Amount, @PaymentModeID, @Comment, @InstituteID, 2)";
            // ExecuteAsync returns the number of rows affected.
            var rowsAffected = await _connection.ExecuteAsync(query, request);
            return rowsAffected;
        }

        public ServiceResponse<List<GetWalletResponse>> GetWallet(GetWalletRequest request)
        {
            var response = new ServiceResponse<List<GetWalletResponse>>(
                true,
                "Wallet records retrieved successfully",
                new List<GetWalletResponse>(),
                200
            );

            // Count Query: Retrieve total matching records (before paging)
            string countQuery = @"
            SELECT COUNT(*) FROM (
                SELECT sm.student_id
                FROM tbl_StudentMaster sm
                LEFT JOIN tbl_Class c ON sm.class_id = c.class_id
                LEFT JOIN tbl_Section s ON sm.section_id = s.section_id
                LEFT JOIN tbl_StudentParentsInfo sp ON sm.student_id = sp.Student_id AND sp.Parent_Type_id = 1
                LEFT JOIN tblStudentWallet sw ON sm.student_id = sw.StudentID
                WHERE 
                    sm.isActive = 1 AND 
                    sm.class_id = @ClassID AND 
                    sm.section_id = @SectionID AND
                    (@Search IS NULL OR 
                     sm.Admission_Number LIKE '%' + @Search + '%' OR 
                     CONCAT(sm.First_Name, ' ', sm.Last_Name) LIKE '%' + @Search + '%')
                GROUP BY sm.student_id
            ) AS WalletCount
            ";

            var countParameters = new
            {
                request.ClassID,
                request.SectionID,
                Search = string.IsNullOrEmpty(request.Search) ? null : request.Search
            };

            int totalCount = _connection.ExecuteScalar<int>(countQuery, countParameters);

            // Calculate the offset based on the current page and page size
            int offset = (request.PageNumber - 1) * request.PageSize;

            // Main Query: Retrieve paged wallet records with net balance calculation (credit - debit)
            string query = @"
            SELECT 
                sm.student_id AS StudentID,
                CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName,
                sm.Admission_Number AS AdmissionNo,
                c.class_name AS Class,
                s.section_name AS Section,
                CONCAT(sp.First_Name, ' ', sp.Last_Name) AS FatherName,
                sp.Mobile_Number AS PhoneNumber,
                ISNULL(SUM(
                    CASE 
                        WHEN sw.JournalEntriesTypeID = 2 THEN sw.Amount   -- Credit: add amount
                        WHEN sw.JournalEntriesTypeID = 1 THEN -sw.Amount  -- Debit: subtract amount
                        ELSE 0
                    END
                ), 0) AS WalletBalance
            FROM tbl_StudentMaster sm
            LEFT JOIN tbl_Class c ON sm.class_id = c.class_id
            LEFT JOIN tbl_Section s ON sm.section_id = s.section_id
            LEFT JOIN tbl_StudentParentsInfo sp ON sm.student_id = sp.Student_id AND sp.Parent_Type_id = 1
            LEFT JOIN tblStudentWallet sw ON sm.student_id = sw.StudentID
            WHERE 
                sm.isActive = 1 AND 
                sm.class_id = @ClassID AND 
                sm.section_id = @SectionID AND
                (@Search IS NULL OR 
                 sm.Admission_Number LIKE '%' + @Search + '%' OR 
                 CONCAT(sm.First_Name, ' ', sm.Last_Name) LIKE '%' + @Search + '%')
            GROUP BY 
                sm.student_id, sm.First_Name, sm.Last_Name, sm.Admission_Number, c.class_name, s.section_name, 
                sp.First_Name, sp.Last_Name, sp.Mobile_Number
            ORDER BY sm.student_id
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            ";

            var queryParameters = new
            {
                request.ClassID,
                request.SectionID,
                Search = string.IsNullOrEmpty(request.Search) ? null : request.Search,
                Offset = offset,
                PageSize = request.PageSize
            };

            var walletList = _connection.Query<GetWalletResponse>(query, queryParameters).ToList();

            // Set data and total count in the response
            response.Data = walletList;
            response.TotalCount = totalCount;

            if (walletList.Count == 0)
            {
                response.Success = false;
                response.Message = "No wallet records found.";
            }

            return response;
        }
         

        public DataTable GetWalletExportData(GetWalletExportRequest request)
        {

            var query = @"
            SELECT  
                CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName,
                sm.Admission_Number AS AdmissionNo,
                c.class_name AS Class,
                s.section_name AS Section,
                CONCAT(sp.First_Name, ' ', sp.Last_Name) AS FatherName,
                sp.Mobile_Number AS PhoneNumber,
                ISNULL(SUM(
                    CASE 
                        WHEN sw.JournalEntriesTypeID = 2 THEN sw.Amount   -- Credit: add amount
                        WHEN sw.JournalEntriesTypeID = 1 THEN -sw.Amount  -- Debit: subtract amount
                        ELSE 0
                    END
                ), 0) AS WalletBalance
            FROM tbl_StudentMaster sm
            LEFT JOIN tbl_Class c ON sm.class_id = c.class_id
            LEFT JOIN tbl_Section s ON sm.section_id = s.section_id
            LEFT JOIN tbl_StudentParentsInfo sp ON sm.student_id = sp.Student_id AND sp.Parent_Type_id = 1
            LEFT JOIN tblStudentWallet sw ON sm.student_id = sw.StudentID
            WHERE 
                sm.isActive = 1 AND 
                sm.class_id = @ClassID AND 
                sm.section_id = @SectionID AND
                (@Search IS NULL OR 
                 sm.Admission_Number LIKE '%' + @Search + '%' OR 
                 CONCAT(sm.First_Name, ' ', sm.Last_Name) LIKE '%' + @Search + '%')
            GROUP BY 
                sm.student_id, sm.First_Name, sm.Last_Name, sm.Admission_Number, c.class_name, s.section_name, 
                sp.First_Name, sp.Last_Name, sp.Mobile_Number
            ORDER BY sm.student_id";
             

            var parameters = new
            {
                request.InstituteID,
                request.ClassID,
                request.SectionID,
                Search = string.IsNullOrEmpty(request.Search) ? null : request.Search
            };

            var dataTable = new DataTable();
            using (var reader = _connection.ExecuteReader(query, parameters))
            {
                dataTable.Load(reader);
            }

            return dataTable;
        }

        public ServiceResponse<GetWalletHistoryResponse> GetWalletHistory(GetWalletHistoryRequest request)
        {
            var responseData = new GetWalletHistoryResponse
            {
                WalletHistory = new List<GetWalletHistoryItem>(),
                Total = new GetWalletHistoryTotal()
            };

            // 1) Query for the detailed wallet history (transactions)
            //    NOTE: We assume there's a datetime column in tblStudentWallet named CreatedOn or PaymentDate.
            //    If not, replace it with your actual date column or remove if not available.
            //    Similarly, we map PaymentModeID to a text value (Cash, Online, etc.).
            string historyQuery = @"
                SELECT
                    -- Example of how to convert a DateTime to a string (if you have a CreatedOn column):
                    --CONVERT(VARCHAR(20), sw.CreatedOn, 120) AS PaymentDate, 
                    '' AS PaymentDate, 

                    CASE 
                        WHEN sw.PaymentModeID = 1 THEN 'Cash'
                        WHEN sw.PaymentModeID = 2 THEN 'Online'
                        ELSE 'Other'
                    END AS PaymentMode,

                    CASE WHEN sw.JournalEntriesTypeID = 1 THEN sw.Amount ELSE 0 END AS Debit,
                    CASE WHEN sw.JournalEntriesTypeID = 2 THEN sw.Amount ELSE 0 END AS Credit,
                    sw.Comment
                FROM tblStudentWallet sw
                WHERE sw.StudentID = @StudentID
                  AND sw.InstituteID = @InstituteID
                ORDER BY sw.WalletID;  -- or by CreatedOn DESC/ASC
            ";

            var historyParams = new
            {
                request.StudentID,
                request.InstituteID
            };

            var walletHistoryItems = _connection.Query<GetWalletHistoryItem>(historyQuery, historyParams).ToList();

            // 2) Query for total debits, credits, and available balance
            string totalQuery = @"
                SELECT
                    SUM(CASE WHEN JournalEntriesTypeID = 1 THEN Amount ELSE 0 END) AS TotalDebits,
                    SUM(CASE WHEN JournalEntriesTypeID = 2 THEN Amount ELSE 0 END) AS TotalCredits,
                    (
                        SUM(CASE WHEN JournalEntriesTypeID = 2 THEN Amount ELSE 0 END)
                        - 
                        SUM(CASE WHEN JournalEntriesTypeID = 1 THEN Amount ELSE 0 END)
                    ) AS TotalAvailableBalance
                FROM tblStudentWallet
                WHERE StudentID = @StudentID
                  AND InstituteID = @InstituteID;
            ";

            var totalResult = _connection.QuerySingleOrDefault<GetWalletHistoryTotal>(totalQuery, historyParams);

            // Fill the response data
            responseData.WalletHistory = walletHistoryItems;
            responseData.Total = totalResult ?? new GetWalletHistoryTotal();

            // Build the final service response
            var response = new ServiceResponse<GetWalletHistoryResponse>(
                success: true,
                message: "Wallet history retrieved successfully",
                data: responseData,
                statusCode: 200
            );

            if (walletHistoryItems.Count == 0)
            {
                response.Success = false;
                response.Message = "No wallet history found.";
            }

            return response;
        }


        public DataTable GetWalletHistoryExportData(GetWalletHistoryExportRequest request)
        {
            // Query to get wallet history items
            string historyQuery = @"
        SELECT 
            --CONVERT(VARCHAR(20), sw.CreatedOn, 120) AS PaymentDate, 
            '' AS PaymentDate, 
            CASE 
                WHEN sw.PaymentModeID = 1 THEN 'Cash'
                WHEN sw.PaymentModeID = 2 THEN 'Online'
                ELSE 'Other'
            END AS PaymentMode,
            CASE WHEN sw.JournalEntriesTypeID = 1 THEN sw.Amount ELSE 0 END AS Debit,
            CASE WHEN sw.JournalEntriesTypeID = 2 THEN sw.Amount ELSE 0 END AS Credit,
            sw.Comment
        FROM tblStudentWallet sw
        WHERE sw.StudentID = @StudentID AND sw.InstituteID = @InstituteID
        ORDER BY sw.WalletID;
    ";

            var parameters = new { request.StudentID, request.InstituteID };

            var historyData = _connection.Query(historyQuery, parameters).ToList();

            // Query to calculate totals
            string totalQuery = @"
        SELECT
            ISNULL(SUM(CASE WHEN JournalEntriesTypeID = 1 THEN Amount ELSE 0 END), 0) AS TotalDebits,
            ISNULL(SUM(CASE WHEN JournalEntriesTypeID = 2 THEN Amount ELSE 0 END), 0) AS TotalCredits,
            ISNULL(SUM(CASE WHEN JournalEntriesTypeID = 2 THEN Amount ELSE 0 END), 0) - 
            ISNULL(SUM(CASE WHEN JournalEntriesTypeID = 1 THEN Amount ELSE 0 END), 0) AS TotalAvailableBalance
        FROM tblStudentWallet
        WHERE StudentID = @StudentID AND InstituteID = @InstituteID;
    ";

            var totals = _connection.QuerySingleOrDefault(totalQuery, parameters);

            // Create a DataTable for export with the desired columns.
            DataTable dt = new DataTable();
            dt.Columns.Add("PaymentDate", typeof(string));
            dt.Columns.Add("PaymentMode", typeof(string));
            dt.Columns.Add("Debit", typeof(decimal));
            dt.Columns.Add("Credit", typeof(decimal));
            dt.Columns.Add("Comment", typeof(string));

            // Fill DataTable with wallet history rows
            foreach (var row in historyData)
            {
                DataRow dr = dt.NewRow();
                dr["PaymentDate"] = row.PaymentDate;
                dr["PaymentMode"] = row.PaymentMode;
                dr["Debit"] = row.Debit;
                dr["Credit"] = row.Credit;
                dr["Comment"] = row.Comment;
                dt.Rows.Add(dr);
            }

            // Append a totals row (you can adjust the layout as needed)
            DataRow totalRow = dt.NewRow();
            totalRow["PaymentDate"] = "Total";
            totalRow["PaymentMode"] = "";
            totalRow["Debit"] = totals?.TotalDebits ?? 0;
            totalRow["Credit"] = totals?.TotalCredits ?? 0;
            totalRow["Comment"] = "Available Balance: " + (totals?.TotalAvailableBalance ?? 0);
            dt.Rows.Add(totalRow);

            return dt;
        }

    }
}
