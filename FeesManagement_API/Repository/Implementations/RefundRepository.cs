using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse; 
using FeesManagement_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace FeesManagement_API.Repository.Implementations
{
    public class RefundRepository : IRefundRepository
    {
        private readonly IDbConnection _connection;

        public RefundRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public string AddRefund(AddRefundRequest request)
        {
            // Convert dates from string to DateTime
            DateTime refundDate = DateTime.ParseExact(request.RefundDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            // SQL query to insert a new refund
            var query = @"
        INSERT INTO tblRefund (AcademiceYearCode, ClassID, SectionID, StudentStatus, StudentID, 
            RefundAmount, RefundDate, PaymentModeID, Remarks, InstituteID, EmployeeID, IsActive)
        VALUES (@AcademiceYearCode, @ClassID, @SectionID, @StudentStatus, @StudentID, 
            @RefundAmount, @RefundDate, @PaymentModeID, @Remarks, @InstituteID, @EmployeeID, @IsActive);
        
        SELECT SCOPE_IDENTITY();
    ";

            // Execute the query and get the generated RefundID
            int refundID = _connection.ExecuteScalar<int>(query, new
            {
                request.AcademiceYearCode,
                request.ClassID,
                request.SectionID,
                request.StudentStatus,
                request.StudentID,
                request.RefundAmount,
                RefundDate = refundDate,
                request.PaymentModeID,
                request.Remarks,
                request.InstituteID,
                request.EmployeeID,
                request.IsActive
            });

            // Process RefundTransfer if PaymentModeID is 1
            if (request.PaymentModeID == 1 && request.RefundTransfer != null)
            {
                DateTime transactionDate = DateTime.ParseExact(request.RefundTransfer.TransactionDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                var refundTransferQuery = @"
            INSERT INTO tblRefundTransfer (RefundID, BankName, AccountNo, IFSCCode, TransactionDate)
            VALUES (@RefundID, @BankName, @AccountNo, @IFSCCode, @TransactionDate);
        ";

                _connection.Execute(refundTransferQuery, new
                {
                    RefundID = refundID,
                    request.RefundTransfer.BankName,
                    request.RefundTransfer.AccountNo,
                    request.RefundTransfer.IFSCCode,
                    TransactionDate = transactionDate
                });
            }

            // Process RefundCheque if PaymentModeID is 2
            if (request.PaymentModeID == 2 && request.RefundCheque != null)
            {
                DateTime issueDate = DateTime.ParseExact(request.RefundCheque.IssueDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                var refundChequeQuery = @"
            INSERT INTO tblRefundCheque (RefundID, ChequeNo, BankName, AccountNo, IFSCCode, IssueDate)
            VALUES (@RefundID, @ChequeNo, @BankName, @AccountNo, @IFSCCode, @IssueDate);
        ";

                _connection.Execute(refundChequeQuery, new
                {
                    RefundID = refundID,
                    request.RefundCheque.ChequeNo,
                    request.RefundCheque.BankName,
                    request.RefundCheque.AccountNo,
                    request.RefundCheque.IFSCCode,
                    IssueDate = issueDate
                });
            }

            // Process RefundCard if PaymentModeID is 3
            if (request.PaymentModeID == 3 && request.RefundCard != null)
            {
                DateTime cardIssueDate = DateTime.ParseExact(request.RefundCard.IssueDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                var refundCardQuery = @"
                INSERT INTO tblRefundCard (RefundID, TransactionID, IssueDate)
                VALUES (@RefundID, @TransactionID, @IssueDate);
        ";

                _connection.Execute(refundCardQuery, new
                {
                    RefundID = refundID,
                    request.RefundCard.TransactionID,
                    IssueDate = cardIssueDate
                });
            }

            // Process Student Wallet if PaymentModeID is 4
            if (request.PaymentModeID == 4 && request.RefundWallet != null)
            {
                var refundWalletQuery = @"
                INSERT INTO tblRefundWallet (RefundID, WalletBalance)
                VALUES (@RefundID, @WalletBalance);
        ";

                _connection.Execute(refundWalletQuery, new
                {
                    RefundID = refundID,
                    WalletBalance = request.RefundWallet.WalletBalance
                });
            }

            return "Success";
        }


        public async Task<ServiceResponse<IEnumerable<GetRefundResponse>>> GetRefund(GetRefundRequest request)
        {
            // Count query to get the total number of matching refunds
            var countQuery = @"
        SELECT COUNT(*)
        FROM tblRefund r
        LEFT JOIN tbl_StudentMaster sm ON r.StudentID = sm.student_id
        WHERE 
            r.InstituteID = @InstituteID 
            AND r.ClassID = @ClassID 
            AND r.SectionID = @SectionID 
            AND (sm.First_Name + ' ' + sm.Last_Name LIKE '%' + @Search + '%' OR 
                 sm.Admission_Number LIKE '%' + @Search + '%')";

            var totalCount = await _connection.ExecuteScalarAsync<int>(countQuery, new
            {
                request.InstituteID,
                request.ClassID,
                request.SectionID,
                Search = request.Search
            });

            // Main query with paging using OFFSET and FETCH NEXT
            var query = @"
        SELECT 
            r.RefundID,
            r.AcademiceYearCode,
            r.ClassID,
            r.SectionID,
            r.StudentStatus,
            r.StudentID,
            CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName,
            sm.Admission_Number AS AdmissionNumber,
            r.RefundAmount,
            FORMAT(r.RefundDate, 'dd-MM-yyyy') AS RefundDate,
            r.PaymentModeID,
            m.PaymentModeType AS PaymentMode, 
            r.Remarks,
            r.InstituteID,
            r.EmployeeID,
            CONCAT(emp.First_Name, ' ', emp.Last_Name) AS EmployeeName,
            c.class_name AS ClassName,
            s.section_name AS SectionName,
            r.StudentStatus AS Status,  -- Additional alias for Status
            (
                SELECT CONCAT(sp.First_Name, ' ', sp.Middle_Name, ' ', sp.Last_Name)
                FROM tbl_StudentParentsInfo sp 
                WHERE sp.Student_id = sm.student_id AND sp.Parent_Type_id = 1
            ) AS FatherName,
            (
                SELECT sp.Mobile_Number
                FROM tbl_StudentParentsInfo sp 
                WHERE sp.Student_id = sm.student_id AND sp.Parent_Type_id = 1
            ) AS MobileNumber,
            r.IsActive
        FROM 
            tblRefund r
        LEFT JOIN tbl_Class c ON r.ClassID = c.class_id
        LEFT JOIN tbl_Section s ON r.SectionID = s.section_id
        LEFT JOIN tblFeesRefundPaymentMode m ON r.PaymentModeID = m.PaymentModeID
        LEFT JOIN tbl_StudentMaster sm ON r.StudentID = sm.student_id
        LEFT JOIN tbl_EmployeeProfileMaster emp ON r.EmployeeID = emp.Employee_id
        WHERE 
            r.InstituteID = @InstituteID 
            AND r.ClassID = @ClassID 
            AND r.SectionID = @SectionID 
            AND (sm.First_Name + ' ' + sm.Last_Name LIKE '%' + @Search + '%' OR 
                 sm.Admission_Number LIKE '%' + @Search + '%')
        ORDER BY r.RefundID
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var parameters = new
            {
                Offset = (request.PageNumber - 1) * request.PageSize,
                request.PageSize,
                request.InstituteID,
                request.ClassID,
                request.SectionID,
                Search = request.Search
            };

            var refundList = await _connection.QueryAsync<GetRefundResponse>(query, parameters);

            return new ServiceResponse<IEnumerable<GetRefundResponse>>(
                success: true,
                message: "Refunds retrieved successfully",
                data: refundList,
                statusCode: 200,
                totalCount: totalCount
            );
        }


        //public async Task<ServiceResponse<IEnumerable<GetRefundResponse>>> GetRefund(GetRefundRequest request)
        //{
        //    // Count query to get the total number of matching refunds
        //    var countQuery = @"
        //SELECT COUNT(*)
        //FROM tblRefund r
        //LEFT JOIN tbl_StudentMaster sm ON r.StudentID = sm.student_id
        //WHERE 
        //    r.InstituteID = @InstituteID 
        //    AND r.ClassID = @ClassID 
        //    AND r.SectionID = @SectionID 
        //    AND (sm.First_Name + ' ' + sm.Last_Name LIKE '%' + @Search + '%' OR 
        //         sm.Admission_Number LIKE '%' + @Search + '%')";

        //    var totalCount = await _connection.ExecuteScalarAsync<int>(countQuery, new
        //    {
        //        request.InstituteID,
        //        request.ClassID,
        //        request.SectionID,
        //        Search = request.Search
        //    });

        //    // Main query with paging using OFFSET and FETCH NEXT
        //    var query = @"
        //SELECT 
        //    r.RefundID,
        //    r.AcademiceYearCode,
        //    r.ClassID,
        //    r.SectionID,
        //    r.StudentStatus,
        //    r.StudentID,
        //    CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName,
        //    r.RefundAmount,
        //    FORMAT(r.RefundDate, 'dd-MM-yyyy') AS RefundDate,
        //    r.PaymentModeID,
        //    m.PaymentMode,
        //    r.Remarks,
        //    r.InstituteID,
        //    r.EmployeeID,
        //    CONCAT(emp.First_Name, ' ', emp.Last_Name) AS EmployeeName,
        //    c.class_name AS ClassName,
        //    s.section_name AS SectionName,
        //    r.IsActive
        //FROM 
        //    tblRefund r
        //LEFT JOIN 
        //    tbl_Class c ON r.ClassID = c.class_id
        //LEFT JOIN 
        //    tbl_Section s ON r.SectionID = s.section_id
        //LEFT JOIN 
        //    tblPaymentMode m ON r.PaymentModeID = m.PaymentModeID
        //LEFT JOIN 
        //    tbl_StudentMaster sm ON r.StudentID = sm.student_id
        //LEFT JOIN 
        //    tbl_EmployeeProfileMaster emp ON r.EmployeeID = emp.Employee_id
        //WHERE 
        //    r.InstituteID = @InstituteID 
        //    AND r.ClassID = @ClassID 
        //    AND r.SectionID = @SectionID 
        //    AND (sm.First_Name + ' ' + sm.Last_Name LIKE '%' + @Search + '%' OR 
        //         sm.Admission_Number LIKE '%' + @Search + '%')
        //ORDER BY r.RefundID
        //OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        //    var parameters = new
        //    {
        //        Offset = (request.PageNumber - 1) * request.PageSize,
        //        request.PageSize,
        //        request.InstituteID,
        //        request.ClassID,
        //        request.SectionID,
        //        Search = request.Search
        //    };

        //    var refundList = await _connection.QueryAsync<GetRefundResponse>(query, parameters);

        //    return new ServiceResponse<IEnumerable<GetRefundResponse>>(
        //        success: true,
        //        message: "Refunds retrieved successfully",
        //        data: refundList,
        //        statusCode: 200,
        //        totalCount: totalCount
        //    );
        //}

        public string DeleteRefund(int refundID)
        {
            var query = @"UPDATE tblRefund SET IsActive = 0 WHERE RefundID = @RefundID";
            _connection.Execute(query, new { RefundID = refundID });
            return "Success";
        }

        public DataTable GetRefundExportData(GetRefundExportRequest request)
        {
            var query = @"
        SELECT 
            r.AcademiceYearCode,
            c.class_name AS ClassName,
            s.section_name AS SectionName,
            r.StudentStatus,
            CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName,
            sm.Admission_Number AS AdmissionNumber,
            r.RefundAmount,
            FORMAT(r.RefundDate, 'dd-MM-yyyy') AS RefundDate,
            m.PaymentModeType AS PaymentMode, 
            r.Remarks,
            CONCAT(emp.First_Name, ' ', emp.Last_Name) AS EmployeeName,
            (
                SELECT CONCAT(sp.First_Name, ' ', sp.Middle_Name, ' ', sp.Last_Name)
                FROM tbl_StudentParentsInfo sp 
                WHERE sp.Student_id = sm.student_id AND sp.Parent_Type_id = 1
            ) AS FatherName,
            (
                SELECT sp.Mobile_Number
                FROM tbl_StudentParentsInfo sp 
                WHERE sp.Student_id = sm.student_id AND sp.Parent_Type_id = 1
            ) AS MobileNumber
        FROM 
            tblRefund r
        LEFT JOIN tbl_Class c ON r.ClassID = c.class_id
        LEFT JOIN tbl_Section s ON r.SectionID = s.section_id
        LEFT JOIN tblFeesRefundPaymentMode m ON r.PaymentModeID = m.PaymentModeID
        LEFT JOIN tbl_StudentMaster sm ON r.StudentID = sm.student_id
        LEFT JOIN tbl_EmployeeProfileMaster emp ON r.EmployeeID = emp.Employee_id
        WHERE 
            r.InstituteID = @InstituteID 
            AND r.ClassID = @ClassID 
            AND r.SectionID = @SectionID 
            AND (sm.First_Name + ' ' + sm.Last_Name LIKE '%' + @Search + '%' OR 
                 sm.Admission_Number LIKE '%' + @Search + '%')
        ORDER BY r.RefundID";

            var parameters = new
            {
                request.InstituteID,
                request.ClassID,
                request.SectionID,
                Search = request.Search
            };

            var dataTable = new DataTable();
            using (var reader = _connection.ExecuteReader(query, parameters))
            {
                dataTable.Load(reader);
            }
            return dataTable;
        }


        public IEnumerable<GetStudentListResponse> GetStudentList(GetStudentListRequest request)
        {
            var query = @"
                SELECT 
                    sm.student_id AS StudentID,
                    CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName,
                    sm.Admission_Number AS AdmissionNumber,
                    sm.Roll_Number AS RollNumber,
                    c.class_name AS Class,
                    s.section_name AS Section
                FROM tbl_StudentMaster sm
                LEFT JOIN tbl_Class c ON sm.class_id = c.class_id
                LEFT JOIN tbl_Section s ON sm.section_id = s.section_id
                WHERE 
                    sm.Institute_id = @InstituteID 
                    AND sm.class_id = @ClassID 
                    AND sm.section_id = @SectionID
                    AND sm.isActive = 1";

            return _connection.Query<GetStudentListResponse>(query, new
            {
                request.InstituteID,
                request.ClassID,
                request.SectionID
            });
        }

        public IEnumerable<GetRefundPaymentModeResponse> GetRefundPaymentMode()
        {
            var query = "SELECT PaymentModeID, PaymentModeType FROM tblFeesRefundPaymentMode ORDER BY PaymentModeID";
            return _connection.Query<GetRefundPaymentModeResponse>(query);
        }
    }
}
