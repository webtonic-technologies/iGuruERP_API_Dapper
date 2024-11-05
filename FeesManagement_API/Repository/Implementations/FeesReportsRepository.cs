using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using System.Data;
using System.Data.SqlClient;
using FeesManagement_API.DTOs.ServiceResponse;


namespace FeesManagement_API.Repository.Implementations
{
    public class FeesReportsRepository : IFeesReportsRepository
    {
        private readonly IDbConnection _dbConnection;

        public FeesReportsRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<List<DailyPaymentSummaryResponse>>> GetDailyPaymentSummaryAsync(DailyPaymentSummaryRequest request)
        {
            var response = new List<DailyPaymentSummaryResponse>();

            // SQL to dynamically create columns based on payment modes
            string sql = @"
        DECLARE @sql NVARCHAR(MAX);
        DECLARE @columns NVARCHAR(MAX);

        -- Create the dynamic column list based on payment modes
        SELECT @columns = STRING_AGG('SUM(CASE WHEN pm.PaymentMode = ''' + PaymentMode + ''' THEN spt.PaymentAmount ELSE 0 END) AS [' + PaymentMode + ']', ', ')
        FROM tblPaymentMode;

        -- Build the final SQL query
        SET @sql = N'
        SELECT 
            fh.FeeHead,
            ' + @columns + ',
            SUM(spt.PaymentAmount) AS Total
        FROM 
            tblFeeHead fh
        LEFT JOIN 
            tblStudentFeePayment sp ON fh.FeeHeadID = sp.FeeHeadID
        LEFT JOIN 
            tblStudentFeePaymentTransaction spt ON sp.FeesPaymentID = spt.PaymentIDs
        LEFT JOIN 
            tblPaymentMode pm ON spt.PaymentModeID = pm.PaymentModeID
        WHERE 
            sp.InstituteID = @InstituteID 
            AND CAST(spt.CashTransactionDate AS DATE) BETWEEN @StartDate AND @EndDate
        GROUP BY 
            fh.FeeHead;';

        -- Execute the dynamic SQL
        EXEC sp_executesql @sql, N'@StartDate DATE, @EndDate DATE, @InstituteID INT', @StartDate, @EndDate, @InstituteID;";

            try
            {
                using (var connection = (SqlConnection)_dbConnection)
                {
                    await connection.OpenAsync();  // Ensure the connection is open

                    using (var command = new SqlCommand(sql, connection))
                    {
                        // Parse the string dates into DateTime
                        var startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", null);
                        var endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", null);

                        // Add parameters to the command
                        command.Parameters.AddWithValue("@StartDate", startDate);
                        command.Parameters.AddWithValue("@EndDate", endDate);
                        command.Parameters.AddWithValue("@InstituteID", request.InstituteID);

                        // Execute the command and read results
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var feeHead = reader["FeeHead"].ToString();
                                var totalCollectedAmount = reader.IsDBNull(reader.GetOrdinal("Total")) ? 0 : (decimal)reader["Total"];

                                // Create a dynamic object for payment modes
                                var paymentModes = new Dictionary<string, decimal>();

                                // Loop through dynamic payment mode columns
                                for (int i = 1; i < reader.FieldCount - 1; i++) // Exclude the last Total column
                                {
                                    string paymentMode = reader.GetName(i);
                                    decimal amount = reader.IsDBNull(i) ? 0 : reader.GetDecimal(i);
                                    paymentModes[paymentMode] = amount;
                                }

                                response.Add(new DailyPaymentSummaryResponse
                                {
                                    FeeHead = feeHead,
                                    TotalCollectedAmount = totalCollectedAmount,
                                    PaymentMode = paymentModes  // Add the payment modes to the response
                                });
                            }
                        }
                    }
                }

                return new ServiceResponse<List<DailyPaymentSummaryResponse>>(true, "Data retrieved successfully.", response, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<DailyPaymentSummaryResponse>>(false, ex.Message, response, StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<List<FeePaymentSummaryResponse>> GetFeePaymentSummaryAsync(FeePaymentSummaryRequest request)
        {
            var response = new List<FeePaymentSummaryResponse>();
            string sql = @"
            SELECT 
               c.class_name + ' - ' + s.section_name AS ClassSection,
                sm.Roll_Number AS RollNumber,
                g.Gender_Type AS Gender,
                spi.First_Name + ' ' + spi.Middle_Name + ' ' + spi.Last_Name AS FatherName,
                spi.Contact_Number AS MobileNo,
                sm.First_Name + ' ' + sm.Middle_Name + ' ' + sm.Last_Name AS StudentName,
                fg.Fee AS TotalFee,
                COALESCE(sp_total.Paid, 0) AS Paid,
                (fg.Fee - COALESCE(sp_total.Paid, 0) - COALESCE(d.Amount, 0) - COALESCE(w.Amount, 0)) AS Balance,
                COALESCE(d.Amount, 0) AS Discount,
                COALESCE(w.Amount, 0) AS Waiver
            FROM 
                tbl_Class c
            LEFT JOIN 
                tbl_Section s ON c.class_id = s.class_id
            LEFT JOIN 
                tbl_StudentMaster sm ON sm.class_id = c.class_id AND sm.section_id = s.section_id AND sm.IsActive = @IsActive
            LEFT JOIN 
                tblFeeGroupClassSection fgcs ON sm.class_id = fgcs.ClassID AND sm.section_id = fgcs.SectionID
            LEFT JOIN 
                tblFeeGroup fg ON fgcs.FeeGroupID = fg.FeeGroupID
            LEFT JOIN 
                (SELECT 
                     StudentID, SUM(Amount) AS Paid 
                 FROM 
                     tblStudentFeePayment 
                 GROUP BY 
                     StudentID) sp_total ON sp_total.StudentID = sm.student_id
            LEFT JOIN 
                (SELECT 
                     Student_id, 
                     First_Name, 
                     Middle_Name, 
                     Last_Name, 
                     Contact_Number 
                 FROM 
                     tbl_StudentParentsInfo 
                 WHERE 
                     Parent_Type_id = 1) spi ON sm.student_id = spi.student_id
            LEFT JOIN 
                tbl_Gender g ON sm.gender_id = g.Gender_id
            LEFT JOIN 
                (SELECT 
                     StudentID, SUM(Amount) AS Amount 
                 FROM 
                     tblStudentDiscount 
                 GROUP BY 
                     StudentID) d ON d.StudentID = sm.student_id
            LEFT JOIN 
                (SELECT 
                     StudentID, SUM(Amount) AS Amount 
                 FROM 
                     tblStudentFeeWaiver 
                 GROUP BY 
                     StudentID) w ON w.StudentID = sm.student_id
            WHERE 
                c.institute_id = @InstituteID 
                AND sm.class_id = @ClassID
                AND sm.section_id = @SectionID
            GROUP BY 
                c.class_id, c.class_name, s.section_name, s.section_id,
                sm.Roll_Number, g.Gender_Type,
                spi.First_Name, spi.Middle_Name, spi.Last_Name, spi.Contact_Number,
                fg.Fee, 
                sm.First_Name, sm.Middle_Name, sm.Last_Name, sm.student_id,
                d.Amount, w.Amount,
                sp_total.Paid
            ORDER BY 
                c.class_id, s.section_id;";

            // Adding parameters
            var isActive = request.Active ? 1 : 0;
            var parameters = new { ClassID = request.ClassID, SectionID = request.SectionID, InstituteID = request.InstituteID, IsActive = isActive };

            response = (await _dbConnection.QueryAsync<FeePaymentSummaryResponse>(sql, parameters)).AsList();
            return response;
        }

        public async Task<List<PaidFeeResponse>> GetPaidFeeReportAsync(PaidFeeRequest request)
        {
            var response = new List<PaidFeeResponse>();
            string sql = @"
                SELECT 
                    sm.Admission_Number AS AdmissionNumber,
                    sm.First_Name + ' ' + sm.Middle_Name + ' ' + sm.Last_Name AS StudentName,
                    c.class_name + ' - ' + s.section_name AS ClassSection,
                    sm.Roll_Number AS RollNumber,
                    spi.First_Name + ' ' + spi.Middle_Name + ' ' + spi.Last_Name AS FatherName,
                    spi.Contact_Number AS MobileNo,
                    COALESCE(sp_total.Paid, 0) AS TotalPaid,
                    COALESCE(d.Total_Discount, 0) AS TotalDiscount,
                    COALESCE(w.Total_Waiver, 0) AS TotalWaiver,
                    fg.Fee AS TotalFee,
                    ((fg.Fee - COALESCE(sp_total.Paid, 0)) - COALESCE(d.Total_Discount, 0) - COALESCE(w.Total_Waiver, 0)) AS Balance
                FROM 
                    tbl_Class c
                LEFT JOIN 
                    tbl_Section s ON c.class_id = s.class_id
                LEFT JOIN 
                    tbl_StudentMaster sm ON sm.class_id = c.class_id AND sm.section_id = s.section_id
                LEFT JOIN 
                    tblFeeGroupClassSection fgcs ON sm.class_id = fgcs.ClassID AND sm.section_id = fgcs.SectionID
                LEFT JOIN 
                    tblFeeGroup fg ON fgcs.FeeGroupID = fg.FeeGroupID
                LEFT JOIN 
                    (SELECT 
                         StudentID, SUM(Amount) AS Paid 
                     FROM 
                         tblStudentFeePayment 
                     GROUP BY 
                         StudentID) sp_total ON sp_total.StudentID = sm.student_id
                LEFT JOIN 
                    (SELECT 
                         StudentID, SUM(Amount) AS Total_Discount 
                     FROM 
                         tblStudentDiscount 
                     GROUP BY 
                         StudentID) d ON d.StudentID = sm.student_id
                LEFT JOIN 
                    (SELECT 
                         StudentID, SUM(Amount) AS Total_Waiver 
                     FROM 
                         tblStudentFeeWaiver 
                     GROUP BY 
                         StudentID) w ON w.StudentID = sm.student_id
                LEFT JOIN 
                    (SELECT 
                         Student_id, 
                         First_Name, 
                         Middle_Name, 
                         Last_Name, 
                         Contact_Number 
                     FROM 
                         tbl_StudentParentsInfo 
                     WHERE 
                         Parent_Type_id = 1) spi ON sm.student_id = spi.student_id
                WHERE 
                    c.institute_id = @InstituteID
                    AND sm.class_id = @ClassID
                    AND sm.section_id = @SectionID
                GROUP BY 
                    sm.Admission_Number,
                    sm.First_Name,
                    sm.Middle_Name,
                    sm.Last_Name,
                    c.class_name,
                    s.section_name,
                    sm.Roll_Number,
                    spi.First_Name,
                    spi.Middle_Name,
                    spi.Last_Name,
                    spi.Contact_Number, 
                    fg.Fee,
                    sp_total.Paid,
                    d.Total_Discount,
                    w.Total_Waiver
                ORDER BY 
                    c.class_name, s.section_name, sm.Roll_Number;";

            var parameters = new
            {
                ClassID = request.ClassID,
                SectionID = request.SectionID,
                InstituteID = request.InstituteID
            };

            response = (await _dbConnection.QueryAsync<PaidFeeResponse>(sql, parameters)).AsList();
            return response;
        }

        public async Task<List<ConcessionTypeResponse>> GetConcessionTypeReportAsync(ConcessionTypeRequest request)
        {
            var response = new List<ConcessionTypeResponse>();
            string sql = @"
                SELECT 
                    sm.Admission_Number AS AdmissionNumber,
                    sm.First_Name + ' ' + sm.Middle_Name + ' ' + sm.Last_Name AS StudentName,
                    c.class_name + ' - ' + s.section_name AS ClassSection,
                    sm.Roll_Number AS RollNumber,
                    COALESCE(cg.ConcessionGroupType, 'N/A') AS ConcessionType,
                    fh.FeeHead AS FeeHead,
                    COALESCE(SUM(cr.Amount), 0) AS AmountPercentage,
                    fg.Fee AS TotalFee,
                    SUM(CASE 
                        WHEN cr.DiscountPercentage IS NOT NULL THEN (fg.Fee * (cr.DiscountPercentage / 100.0))
                        ELSE COALESCE(cr.Amount, 0)
                    END) AS TotalConcession,
                    (fg.Fee - COALESCE(SUM(cr.Amount), 0) - 
                        SUM(CASE 
                            WHEN cr.DiscountPercentage IS NOT NULL THEN (fg.Fee * (cr.DiscountPercentage / 100.0))
                            ELSE 0
                        END)) AS FinalBalance
                FROM 
                    tbl_StudentMaster sm
                LEFT JOIN 
                    tbl_Class c ON sm.class_id = c.class_id
                LEFT JOIN 
                    tbl_Section s ON sm.section_id = s.section_id
                LEFT JOIN 
                    tblFeeGroupClassSection fgcs ON sm.class_id = fgcs.ClassID AND sm.section_id = fgcs.SectionID
                LEFT JOIN 
                    tblFeeGroup fg ON fgcs.FeeGroupID = fg.FeeGroupID
                LEFT JOIN 
                    tblFeeHead fh ON fg.FeeHeadID = fh.FeeHeadID
                LEFT JOIN 
                    tblStudentConcession sc ON sc.StudentID = sm.student_id AND sc.IsActive = 1
                LEFT JOIN 
                    tblConcessionRules cr ON cr.ConcessionGroupID = sc.ConcessionGroupID AND cr.InstituteID = c.institute_id
                LEFT JOIN 
                    tblConcessionGroup cg ON cg.ConcessionGroupID = cr.ConcessionGroupID
                WHERE 
                    c.institute_id = @InstituteID
                    AND fh.FeeHead = 'Tuition Fee'
                GROUP BY 
                    sm.Admission_Number,
                    sm.First_Name,
                    sm.Middle_Name,
                    sm.Last_Name,
                    c.class_name,
                    s.section_name,
                    sm.Roll_Number,
                    cg.ConcessionGroupType,
                    fh.FeeHead,
                    fg.Fee
                ORDER BY 
                    c.class_name, s.section_name, sm.Roll_Number;";

            var parameters = new
            {
                InstituteID = request.InstituteID
            };

            response = (await _dbConnection.QueryAsync<ConcessionTypeResponse>(sql, parameters)).AsList();
            return response;
        }

        public async Task<List<ClassWiseConcessionResponse>> GetClassWiseConcessionReportAsync(ClassWiseConcessionRequest request)
        {
            var response = new List<ClassWiseConcessionResponse>();
            string sql = @"
        DECLARE @sql NVARCHAR(MAX);
        DECLARE @concessionTypes NVARCHAR(MAX);

        -- Build the dynamic parts for concession counts based on available concession types
        SELECT @concessionTypes = STRING_AGG(
            'COALESCE(COUNT(CASE WHEN sm.gender_id = 1 AND cg.ConcessionGroupType = ''' + ConcessionGroupType + ''' THEN sm.student_id END), 0) AS ' + QUOTENAME(ConcessionGroupType + '_Boys') + ', ' +
            'COALESCE(COUNT(CASE WHEN sm.gender_id = 2 AND cg.ConcessionGroupType = ''' + ConcessionGroupType + ''' THEN sm.student_id END), 0) AS ' + QUOTENAME(ConcessionGroupType + '_Girls'), 
            ', ')
        FROM 
            tblConcessionGroup
        WHERE 
            IsActive = 1;

        -- Construct the full SQL query
        SET @sql = '
        SELECT 
            CONCAT(c.class_name, '' - '', s.section_name) AS Class_Section,
            COUNT(sm.student_id) AS Total_Strength, ' + @concessionTypes + '
        FROM 
            tbl_StudentMaster sm
        LEFT JOIN 
            tbl_Class c ON sm.class_id = c.class_id
        LEFT JOIN 
            tbl_Section s ON sm.section_id = s.section_id
        LEFT JOIN 
            tblStudentConcession sc ON sc.StudentID = sm.student_id AND sc.IsActive = 1
        LEFT JOIN 
            tblConcessionRules cr ON cr.ConcessionGroupID = sc.ConcessionGroupID
        LEFT JOIN 
            tblConcessionGroup cg ON cg.ConcessionGroupID = cr.ConcessionGroupID
        WHERE 
            c.institute_id = @InstituteID
        GROUP BY 
            c.class_name,
            s.section_name
        ORDER BY 
            c.class_name, 
            s.section_name;';

        -- Execute the dynamic SQL
        EXEC sp_executesql @sql, N'@InstituteID INT', @InstituteID;";

            var parameters = new
            {
                InstituteID = request.InstituteID
            };

            // Execute the query
            var results = await _dbConnection.QueryAsync(sql, parameters);

            // Process results into the desired format
            foreach (var item in results)
            {
                var classSection = item.Class_Section ?? "Unknown"; // Fallback for null
                var totalStrength = item.Total_Strength ?? 0; // Fallback for null

                // Initialize the dynamic structure for concession counts
                var categoryConcessions = new Dictionary<string, Dictionary<string, int>>();

                // Loop through dynamic concession columns
                foreach (var concessionType in item.GetType().GetProperties())
                {
                    // Check if the property name indicates Boys or Girls count
                    if (concessionType.Name.EndsWith("_Boys") || concessionType.Name.EndsWith("_Girls"))
                    {
                        var category = concessionType.Name.Replace("_Boys", "").Replace("_Girls", "");

                        // Initialize the category if it doesn't exist
                        if (!categoryConcessions.ContainsKey(category))
                        {
                            categoryConcessions[category] = new Dictionary<string, int>
                    {
                        { "Boys", 0 },
                        { "Girls", 0 }
                    };
                        }

                        // Get the count safely
                        int count = concessionType.GetValue(item) != null ? Convert.ToInt32(concessionType.GetValue(item)) : 0;

                        // Assign to the respective category
                        if (concessionType.Name.EndsWith("_Boys"))
                        {
                            categoryConcessions[category]["Boys"] = count;
                        }
                        else
                        {
                            categoryConcessions[category]["Girls"] = count;
                        }
                    }
                }

                // Add the response for this item
                response.Add(new ClassWiseConcessionResponse
                {
                    ClassSection = classSection,
                    TotalStrength = totalStrength,
                    CategoryConcessions = categoryConcessions
                });
            }

            return response;
        }

        public async Task<List<DiscountSummaryResponse>> GetDiscountSummaryAsync(DiscountSummaryRequest request)
        {
            string sql = @"
            SELECT 
                sm.Admission_Number AS AdmissionNumber,
                CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) AS StudentName,
                CONCAT(c.class_name, ' - ', s.section_name) AS ClassSection,
                sm.Roll_Number AS RollNumber,
                CONCAT(COALESCE(spi_father.First_Name, ''), ' ', COALESCE(spi_father.Middle_Name, ''), ' ', COALESCE(spi_father.Last_Name, '')) AS FatherName,
                spi_father.Contact_Number AS FatherMobileNo,
                CONCAT(COALESCE(spi_mother.First_Name, ''), ' ', COALESCE(spi_mother.Middle_Name, ''), ' ', COALESCE(spi_mother.Last_Name, '')) AS MotherName,
                spi_mother.Contact_Number AS MotherMobileNo, 
                COALESCE(d.DiscountAmount, 0) AS DiscountAmount
            FROM 
                tbl_StudentMaster sm
            LEFT JOIN 
                tbl_Class c ON sm.class_id = c.class_id
            LEFT JOIN 
                tbl_Section s ON sm.section_id = s.section_id
            LEFT JOIN 
                (SELECT StudentID, SUM(Amount) AS DiscountAmount
                 FROM tblStudentDiscount
                 GROUP BY StudentID) d ON d.StudentID = sm.student_id
            LEFT JOIN 
                (SELECT Student_id, First_Name, Middle_Name, Last_Name, Contact_Number 
                 FROM tbl_StudentParentsInfo 
                 WHERE Parent_Type_id = 1) spi_father ON sm.student_id = spi_father.student_id  
            LEFT JOIN 
                (SELECT Student_id, First_Name, Middle_Name, Last_Name, Contact_Number 
                 FROM tbl_StudentParentsInfo 
                 WHERE Parent_Type_id = 2) spi_mother ON sm.student_id = spi_mother.student_id  
            WHERE 
                c.institute_id = @InstituteID 
                AND sm.class_id = @ClassID 
                AND sm.section_id = @SectionID 
            ORDER BY 
                sm.Admission_Number;";

            var parameters = new
            {
                ClassID = request.ClassID,
                SectionID = request.SectionID,
                InstituteID = request.InstituteID
            };

            var results = await _dbConnection.QueryAsync<DiscountSummaryResponse>(sql, parameters);
            return results.ToList();
        }

        public async Task<List<WaiverSummaryResponse>> GetWaiverSummaryReportAsync(WaiverSummaryRequest request)
        {
            string sql = @"
            SELECT 
                sm.Admission_Number AS AdmissionNumber,
                CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) AS StudentName,
                CONCAT(c.class_name, ' - ', s.section_name) AS ClassSection,
                sm.Roll_Number AS RollNumber,
                CONCAT(
                    COALESCE(spi_father.First_Name, ''), 
                    ' ', 
                    COALESCE(spi_father.Middle_Name, ''), 
                    ' ', 
                    COALESCE(spi_father.Last_Name, '')
                ) AS FatherName,
                spi_father.Contact_Number AS FatherMobileNo,
                CONCAT(
                    COALESCE(spi_mother.First_Name, ''), 
                    ' ', 
                    COALESCE(spi_mother.Middle_Name, ''), 
                    ' ', 
                    COALESCE(spi_mother.Last_Name, '')
                ) AS MotherName,
                spi_mother.Contact_Number AS MotherMobileNo, 
                COALESCE(w.Total_Waiver, 0) AS TotalWaiver
            FROM 
                tbl_StudentMaster sm
            LEFT JOIN 
                tbl_Class c ON sm.class_id = c.class_id
            LEFT JOIN 
                tbl_Section s ON sm.section_id = s.section_id
            LEFT JOIN 
                (SELECT 
                     StudentID, SUM(Amount) AS Total_Waiver
                 FROM 
                     tblStudentFeeWaiver
                 GROUP BY 
                     StudentID) w ON w.StudentID = sm.student_id
            LEFT JOIN 
                (SELECT 
                     Student_id, 
                     First_Name, 
                     Middle_Name, 
                     Last_Name, 
                     Contact_Number 
                 FROM 
                     tbl_StudentParentsInfo 
                 WHERE 
                     Parent_Type_id = 1) spi_father ON sm.student_id = spi_father.student_id  
            LEFT JOIN 
                (SELECT 
                     Student_id, 
                     First_Name, 
                     Middle_Name, 
                     Last_Name, 
                     Contact_Number 
                 FROM 
                     tbl_StudentParentsInfo 
                 WHERE 
                     Parent_Type_id = 2) spi_mother ON sm.student_id = spi_mother.student_id  
            WHERE 
                c.institute_id = @InstituteID 
                AND sm.class_id = @ClassID 
                AND sm.section_id = @SectionID 
            ORDER BY 
                sm.Admission_Number;";

            var parameters = new
            {
                ClassID = request.ClassID,
                SectionID = request.SectionID,
                InstituteID = request.InstituteID
            };

            var results = await _dbConnection.QueryAsync<WaiverSummaryResponse>(sql, parameters);
            return results.ToList();
        }
    }
}
