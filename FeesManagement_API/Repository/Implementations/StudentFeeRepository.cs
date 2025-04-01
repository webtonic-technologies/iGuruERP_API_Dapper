using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FeesManagement_API.Repository.Implementations
{
    public class StudentFeeRepository : IStudentFeeRepository
    {
        private readonly IConfiguration _configuration;

        public StudentFeeRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        public List<StudentFeeResponse> GetStudentFees(StudentFeeRequest request)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var query = @"
         WITH UniqueLFRS AS (
             SELECT DISTINCT 
                    LateFeeRuleID, 
                    FeeHeadID, 
                    FeeTenurityID, 
                    DueDate, 
                    InstituteID, 
                    IsActive
             FROM tblLateFeeRuleSetup
         ),
         PaymentFromPayment AS (
             SELECT 
                StudentID, 
                ClassID, 
                SectionID, 
                InstituteID, 
                FeeGroupID, 
                FeeHeadID, 
                FeeTenurityID, 
                Amount AS AmountPaid,
                NULL AS PaymentDate
             FROM tblStudentFeePayment
         ),
         PaymentFromTransaction AS (
             SELECT 
                p.StudentID, 
                p.ClassID, 
                p.SectionID, 
                p.InstituteID, 
                p.FeeGroupID, 
                p.FeeHeadID, 
                p.FeeTenurityID, 
                t.PaymentAmount AS AmountPaid,
                t.CashTransactionDate AS PaymentDate
             FROM tblStudentFeePaymentTransaction t
             INNER JOIN tblStudentFeePayment p
                  ON t.PaymentIDs = p.FeesPaymentID
         ),
         AggregatedPayments AS (
             SELECT
               StudentID, ClassID, SectionID, InstituteID, FeeGroupID, FeeHeadID, FeeTenurityID,
               SUM(AmountPaid) AS TotalPaid,
               MIN(PaymentDate) AS PaymentDate
             FROM (
               SELECT * FROM PaymentFromPayment
               UNION ALL
               SELECT * FROM PaymentFromTransaction
             ) AS X
             GROUP BY StudentID, ClassID, SectionID, InstituteID, FeeGroupID, FeeHeadID, FeeTenurityID
         )
         SELECT  
             sm.student_id AS StudentID,
             sm.Admission_Number AS AdmissionNo,
             CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) AS StudentName,
             sm.Roll_Number AS RollNo,
             c.class_name AS ClassName,
             s.section_name AS SectionName,
             CAST(fh.FeeHeadID AS int) AS FeeHeadID,
             fh.FeeHead,
             fg.FeeGroupID AS FeeGroupID,
             fg.FeeTenurityID AS FeeTenurityID,
             CASE 
                 WHEN fg.FeeTenurityID = 1 THEN 'Single'
                 WHEN fg.FeeTenurityID = 2 THEN tt.TermName
                 WHEN fg.FeeTenurityID = 3 THEN tm.Month
                 ELSE 'N/A'
             END AS FeeType,
             COALESCE(ts.Amount, tt.Amount, tm.Amount) AS FeeAmount,
             cg.ConcessionGroupType AS ConcessionGroup,
             CASE 
                  WHEN COALESCE(ap.PaymentDate, GETDATE()) <= lfrs.DueDate THEN 0
                  WHEN COALESCE(ap.TotalPaid, 0) >= COALESCE(ts.Amount, tt.Amount, tm.Amount) THEN 0
                  WHEN DATEDIFF(DAY, lfrs.DueDate, COALESCE(ap.PaymentDate, GETDATE()))
                       BETWEEN fr.MinDays AND fr.MaxDays THEN fr.LateFee
                  ELSE 0
             END AS LateFee
         FROM tbl_StudentMaster sm
         INNER JOIN tbl_Class c 
             ON sm.class_id = c.class_id
         INNER JOIN tbl_Section s 
             ON sm.section_id = s.section_id
         INNER JOIN tblFeeGroupClassSection fgcs 
             ON sm.class_id = fgcs.ClassID 
                AND sm.section_id = fgcs.SectionID
         INNER JOIN tblFeeGroup fg 
             ON fgcs.FeeGroupID = fg.FeeGroupID
         INNER JOIN tblFeeHead fh 
             ON fg.FeeHeadID = fh.FeeHeadID
         LEFT JOIN tblTenuritySingle ts 
             ON fg.FeeTenurityID = 1 
                AND ts.FeeCollectionID = fgcs.FeeGroupID
         LEFT JOIN tblTenurityTerm tt 
             ON fg.FeeTenurityID = 2 
                AND tt.FeeCollectionID = fgcs.FeeGroupID
         LEFT JOIN tblTenurityMonthly tm 
             ON fg.FeeTenurityID = 3 
                AND tm.FeeCollectionID = fgcs.FeeGroupID
         LEFT JOIN tblStudentConcession sc 
             ON sm.student_id = sc.StudentID 
                AND sm.Institute_id = sc.InstituteID 
                AND sc.IsActive = 1
         LEFT JOIN tblConcessionGroup cg 
             ON sc.ConcessionGroupID = cg.ConcessionGroupID 
                AND cg.IsActive = 1
         LEFT JOIN tblLateFeeClassSectionMapping lfm 
             ON sm.class_id = lfm.ClassID 
                AND sm.section_id = lfm.SectionID
         LEFT JOIN UniqueLFRS lfrs 
             ON lfm.LateFeeRuleID = lfrs.LateFeeRuleID 
                AND lfrs.FeeHeadID = fh.FeeHeadID 
                AND lfrs.InstituteID = sm.Institute_id 
                AND lfrs.IsActive = 1
         LEFT JOIN AggregatedPayments ap 
             ON ap.StudentID = sm.student_id 
                AND ap.ClassID = sm.class_id 
                AND ap.SectionID = sm.section_id 
                AND ap.InstituteID = sm.Institute_id 
                AND ap.FeeGroupID = fg.FeeGroupID 
                AND ap.FeeHeadID = fh.FeeHeadID 
                AND ap.FeeTenurityID = fg.FeeTenurityID
         LEFT JOIN tblFeesRules fr 
             ON fr.LateFeeRuleID = lfrs.LateFeeRuleID 
                AND DATEDIFF(DAY, lfrs.DueDate, COALESCE(ap.PaymentDate, GETDATE()))
                    BETWEEN fr.MinDays AND fr.MaxDays
         WHERE sm.class_id = @ClassID 
           AND sm.section_id = @SectionID
           AND sm.Institute_id = @InstituteID
           AND (@Search IS NULL OR 
                sm.Admission_Number LIKE '%' + @Search + '%' OR
                CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) LIKE '%' + @Search + '%')
         ORDER BY sm.Admission_Number;
         ";

                var result = connection.Query<StudentFeeData>(query, new
                {
                    request.ClassID,
                    request.SectionID,
                    request.InstituteID,
                    request.Search
                }).ToList();

                // Group the result by student information and map to StudentFeeResponse,
                // including FeeGroupID and FeeTenurityID as part of the grouping key.
                var response = result.GroupBy(x => new
                {
                    x.StudentID,
                    x.AdmissionNo,
                    x.StudentName,
                    x.RollNo,
                    x.ClassName,
                    x.SectionName,
                    x.ConcessionGroup,
                    x.FeeGroupID,
                    x.FeeTenurityID
                }).Select(group => new StudentFeeResponse
                {
                    StudentID = group.Key.StudentID,
                    AdmissionNo = group.Key.AdmissionNo,
                    StudentName = group.Key.StudentName,
                    RollNo = group.Key.RollNo,
                    ClassName = group.Key.ClassName,
                    SectionName = group.Key.SectionName,
                    ConcessionGroup = group.Key.ConcessionGroup,
                    FeeGroupID = group.Key.FeeGroupID,
                    FeeTenurityID = group.Key.FeeTenurityID,
                    TotalFeeAmount = group.Sum(x => x.FeeAmount),
                    TotalLateFee = group.Sum(x => x.LateFee),
                    FeeDetails = group.Select(x => new StudentFeeDetail
                    {
                        FeeHeadID = x.FeeHeadID,
                        FeeHead = x.FeeHead,
                        TenureType = x.FeeType,
                        Amount = x.FeeAmount,
                        LateFee = x.LateFee
                    }).ToList()
                }).ToList();

                return response;
            }
        }


        //public List<StudentFeeResponse> GetStudentFees(StudentFeeRequest request)
        //{
        //    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        var query = @"
        //        WITH UniqueLFRS AS (
        //            SELECT DISTINCT 
        //                   LateFeeRuleID, 
        //                   FeeHeadID, 
        //                   FeeTenurityID, 
        //                   DueDate, 
        //                   InstituteID, 
        //                   IsActive
        //            FROM tblLateFeeRuleSetup
        //        ),
        //        PaymentFromPayment AS (
        //            SELECT 
        //               StudentID, 
        //               ClassID, 
        //               SectionID, 
        //               InstituteID, 
        //               FeeGroupID, 
        //               FeeHeadID, 
        //               FeeTenurityID, 
        //               Amount AS AmountPaid,
        //               NULL AS PaymentDate
        //            FROM tblStudentFeePayment
        //        ),
        //        PaymentFromTransaction AS (
        //            SELECT 
        //               p.StudentID, 
        //               p.ClassID, 
        //               p.SectionID, 
        //               p.InstituteID, 
        //               p.FeeGroupID, 
        //               p.FeeHeadID, 
        //               p.FeeTenurityID, 
        //               t.PaymentAmount AS AmountPaid,
        //               t.CashTransactionDate AS PaymentDate
        //            FROM tblStudentFeePaymentTransaction t
        //            INNER JOIN tblStudentFeePayment p
        //                 ON t.PaymentIDs = p.FeesPaymentID
        //        ),
        //        AggregatedPayments AS (
        //            SELECT
        //              StudentID, ClassID, SectionID, InstituteID, FeeGroupID, FeeHeadID, FeeTenurityID,
        //              SUM(AmountPaid) AS TotalPaid,
        //              MIN(PaymentDate) AS PaymentDate
        //            FROM (
        //              SELECT * FROM PaymentFromPayment
        //              UNION ALL
        //              SELECT * FROM PaymentFromTransaction
        //            ) AS X
        //            GROUP BY StudentID, ClassID, SectionID, InstituteID, FeeGroupID, FeeHeadID, FeeTenurityID
        //        )
        //        SELECT  
        //            sm.student_id AS StudentID,
        //            sm.Admission_Number AS AdmissionNo,
        //            CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) AS StudentName,
        //            sm.Roll_Number AS RollNo,
        //            c.class_name AS ClassName,
        //            s.section_name AS SectionName,
        //            CAST(fh.FeeHeadID AS int) AS FeeHeadID,
        //            fh.FeeHead,
        //            CASE 
        //                WHEN fg.FeeTenurityID = 1 THEN 'Single'
        //                WHEN fg.FeeTenurityID = 2 THEN tt.TermName
        //                WHEN fg.FeeTenurityID = 3 THEN tm.Month
        //                ELSE 'N/A'
        //            END AS FeeType,
        //            COALESCE(ts.Amount, tt.Amount, tm.Amount) AS FeeAmount,
        //            cg.ConcessionGroupType AS ConcessionGroup,
        //            CASE 
        //                 -- Use PaymentDate from aggregated payments if available; otherwise, GETDATE() for overdue calculation.
        //                 WHEN COALESCE(ap.PaymentDate, GETDATE()) <= lfrs.DueDate THEN 0
        //                 WHEN COALESCE(ap.TotalPaid, 0) >= COALESCE(ts.Amount, tt.Amount, tm.Amount) THEN 0
        //                 WHEN DATEDIFF(DAY, lfrs.DueDate, COALESCE(ap.PaymentDate, GETDATE()))
        //                      BETWEEN fr.MinDays AND fr.MaxDays THEN fr.LateFee
        //                 ELSE 0
        //            END AS LateFee
        //        FROM tbl_StudentMaster sm
        //        INNER JOIN tbl_Class c 
        //            ON sm.class_id = c.class_id
        //        INNER JOIN tbl_Section s 
        //            ON sm.section_id = s.section_id
        //        INNER JOIN tblFeeGroupClassSection fgcs 
        //            ON sm.class_id = fgcs.ClassID 
        //               AND sm.section_id = fgcs.SectionID
        //        INNER JOIN tblFeeGroup fg 
        //            ON fgcs.FeeGroupID = fg.FeeGroupID
        //        INNER JOIN tblFeeHead fh 
        //            ON fg.FeeHeadID = fh.FeeHeadID
        //        LEFT JOIN tblTenuritySingle ts 
        //            ON fg.FeeTenurityID = 1 
        //               AND ts.FeeCollectionID = fgcs.FeeGroupID
        //        LEFT JOIN tblTenurityTerm tt 
        //            ON fg.FeeTenurityID = 2 
        //               AND tt.FeeCollectionID = fgcs.FeeGroupID
        //        LEFT JOIN tblTenurityMonthly tm 
        //            ON fg.FeeTenurityID = 3 
        //               AND tm.FeeCollectionID = fgcs.FeeGroupID
        //        LEFT JOIN tblStudentConcession sc 
        //            ON sm.student_id = sc.StudentID 
        //               AND sm.Institute_id = sc.InstituteID 
        //               AND sc.IsActive = 1
        //        LEFT JOIN tblConcessionGroup cg 
        //            ON sc.ConcessionGroupID = cg.ConcessionGroupID 
        //               AND cg.IsActive = 1
        //        LEFT JOIN tblLateFeeClassSectionMapping lfm 
        //            ON sm.class_id = lfm.ClassID 
        //               AND sm.section_id = lfm.SectionID
        //        LEFT JOIN UniqueLFRS lfrs 
        //            ON lfm.LateFeeRuleID = lfrs.LateFeeRuleID 
        //               AND lfrs.FeeHeadID = fh.FeeHeadID 
        //               AND lfrs.InstituteID = sm.Institute_id 
        //               AND lfrs.IsActive = 1
        //        LEFT JOIN AggregatedPayments ap 
        //            ON ap.StudentID = sm.student_id 
        //               AND ap.ClassID = sm.class_id 
        //               AND ap.SectionID = sm.section_id 
        //               AND ap.InstituteID = sm.Institute_id 
        //               AND ap.FeeGroupID = fg.FeeGroupID 
        //               AND ap.FeeHeadID = fh.FeeHeadID 
        //               AND ap.FeeTenurityID = fg.FeeTenurityID
        //        LEFT JOIN tblFeesRules fr 
        //            ON fr.LateFeeRuleID = lfrs.LateFeeRuleID 
        //               AND DATEDIFF(DAY, lfrs.DueDate, COALESCE(ap.PaymentDate, GETDATE()))
        //                   BETWEEN fr.MinDays AND fr.MaxDays
        //        WHERE sm.class_id = @ClassID 
        //          AND sm.section_id = @SectionID
        //          AND sm.Institute_id = @InstituteID
        //          AND (@Search IS NULL OR 
        //               sm.Admission_Number LIKE '%' + @Search + '%' OR
        //               CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) LIKE '%' + @Search + '%')
        //        ORDER BY sm.Admission_Number;
        //        ";

        //        var result = connection.Query<StudentFeeData>(query, new
        //        {
        //            request.ClassID,
        //            request.SectionID,
        //            request.InstituteID,
        //            request.Search
        //        }).ToList();

        //        // Group the result by student information and map to StudentFeeResponse,
        //        // and sum up the LateFee values as TotalLateFee.
        //        var response = result.GroupBy(x => new
        //        {
        //            x.StudentID,
        //            x.AdmissionNo,
        //            x.StudentName,
        //            x.RollNo,
        //            x.ClassName,
        //            x.SectionName,
        //            x.ConcessionGroup
        //        }).Select(group => new StudentFeeResponse
        //        {
        //            StudentID = group.Key.StudentID,
        //            AdmissionNo = group.Key.AdmissionNo,
        //            StudentName = group.Key.StudentName,
        //            RollNo = group.Key.RollNo,
        //            ClassName = group.Key.ClassName,
        //            SectionName = group.Key.SectionName,
        //            ConcessionGroup = group.Key.ConcessionGroup,
        //            TotalFeeAmount = group.Sum(x => x.FeeAmount),
        //            TotalLateFee = group.Sum(x => x.LateFee), // Sum of late fees for all fee types
        //            FeeDetails = group.Select(x => new StudentFeeDetail
        //            {
        //                FeeHead = x.FeeHead,
        //                TenureType = x.FeeType,
        //                Amount = x.FeeAmount,
        //                LateFee = x.LateFee
        //            }).ToList()
        //        }).ToList();

        //        return response;
        //    }
        //}


        public int DiscountStudentFees(DiscountStudentFeesRequest request)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var query = @"
                INSERT INTO tblFeesDiscount 
                    (StudentID, FeeHeadID, FeeGroupID, FeeTenurityID, DiscountedAmount, InstituteID, AcademicYearCode, DateTime, UserID)
                VALUES
                    (@StudentID, @FeeHeadID, @FeeGroupID, @FeeTenurityID, @DiscountedAmount, @InstituteID, @AcademicYearCode, GETDATE(), @UserID);
                SELECT CAST(SCOPE_IDENTITY() AS INT);
                ";
                // Execute the query and return the generated DiscountID
                var discountID = connection.QuerySingle<int>(query, request);
                return discountID;
            }
        }

        public List<GetFeesChangeLogsResponse> GetFeesChangeLogs(GetFeesChangeLogsRequest request)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var query = @"
                SELECT
                    sm.student_id AS StudentID,
                    sm.Admission_Number AS AdmissionNumber,
                    CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) AS StudentName,
                    sm.Roll_Number AS RollNumber,
                    cg.ConcessionGroupType AS ConcessionGroup,
                    fd.FeeHeadID,
                    fh.FeeHead,
                    fd.FeeGroupID,
                    fd.FeeTenurityID,
                    CASE 
                         WHEN fd.FeeTenurityID = 1 THEN 'Single'
                         WHEN fd.FeeTenurityID = 2 THEN tt.TermName
                         WHEN fd.FeeTenurityID = 3 THEN tm.Month
                         ELSE 'N/A'
                    END AS FeeTenurity,
                    fg.Fee AS TotalFeeAmount,
                    fd.DiscountedAmount,
                    -- Format the date as 'dd-MM-yyyy at hh:mm tt'
                    FORMAT(fd.DateTime, 'dd-MM-yyyy ""at"" hh:mm tt') AS DiscountedDateTime,
                    CONCAT(ep.First_Name, ' ', ep.Middle_Name, ' ', ep.Last_Name) AS UserName
                FROM tblFeesDiscount fd
                INNER JOIN tbl_StudentMaster sm
                    ON fd.StudentID = sm.student_id
                LEFT JOIN tblStudentConcession sc
                    ON sm.student_id = sc.StudentID 
                       AND sm.Institute_id = sc.InstituteID
                LEFT JOIN tblConcessionGroup cg
                    ON sc.ConcessionGroupID = cg.ConcessionGroupID
                INNER JOIN tblFeeHead fh
                    ON fd.FeeHeadID = fh.FeeHeadID
                INNER JOIN tblFeeGroup fg
                    ON fd.FeeGroupID = fg.FeeGroupID
                LEFT JOIN tblTenurityTerm tt
                    ON fd.FeeTenurityID = 2 
                       AND tt.FeeCollectionID = fg.FeeGroupID
                LEFT JOIN tblTenurityMonthly tm
                    ON fd.FeeTenurityID = 3 
                       AND tm.FeeCollectionID = fg.FeeGroupID
                LEFT JOIN tbl_EmployeeProfileMaster ep
                    ON fd.UserID = ep.Employee_id
                WHERE sm.Institute_id = @InstituteID
                  AND sm.class_id = @ClassID
                  AND sm.section_id = @SectionID
                  AND fd.AcademicYearCode = @AcademicYearCode;
                ";
                var result = connection.Query<GetFeesChangeLogsResponse>(query, request).ToList();
                return result;
            }
        }


        public async Task<List<StudentFeeResponse>> GetStudentFeesExport(GetStudentFeesExportRequest request)
        {
            string sqlQuery = @"
WITH UniqueLFRS AS (
    SELECT DISTINCT 
           LateFeeRuleID, 
           FeeHeadID, 
           FeeTenurityID, 
           DueDate, 
           InstituteID, 
           IsActive
    FROM tblLateFeeRuleSetup
),
PaymentFromPayment AS (
    SELECT 
       StudentID, 
       ClassID, 
       SectionID, 
       InstituteID, 
       FeeGroupID, 
       FeeHeadID, 
       FeeTenurityID, 
       Amount AS AmountPaid,
       NULL AS PaymentDate
    FROM tblStudentFeePayment
),
PaymentFromTransaction AS (
    SELECT 
       p.StudentID, 
       p.ClassID, 
       p.SectionID, 
       p.InstituteID, 
       p.FeeGroupID, 
       p.FeeHeadID, 
       p.FeeTenurityID, 
       t.PaymentAmount AS AmountPaid,
       t.CashTransactionDate AS PaymentDate
    FROM tblStudentFeePaymentTransaction t
    INNER JOIN tblStudentFeePayment p
         ON t.PaymentIDs = p.FeesPaymentID
),
AggregatedPayments AS (
    SELECT
      StudentID, ClassID, SectionID, InstituteID, FeeGroupID, FeeHeadID, FeeTenurityID,
      SUM(AmountPaid) AS TotalPaid,
      MIN(PaymentDate) AS PaymentDate
    FROM (
      SELECT * FROM PaymentFromPayment
      UNION ALL
      SELECT * FROM PaymentFromTransaction
    ) AS X
    GROUP BY StudentID, ClassID, SectionID, InstituteID, FeeGroupID, FeeHeadID, FeeTenurityID
)
SELECT  
    sm.student_id AS StudentID,
    sm.Admission_Number AS AdmissionNo,
    CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) AS StudentName,
    sm.Roll_Number AS RollNo,
    c.class_name AS ClassName,
    s.section_name AS SectionName,
    CAST(fh.FeeHeadID AS int) AS FeeHeadID,
    fh.FeeHead,
    fg.FeeGroupID AS FeeGroupID,
    fg.FeeTenurityID AS FeeTenurityID,
    CASE 
        WHEN fg.FeeTenurityID = 1 THEN 'Single'
        WHEN fg.FeeTenurityID = 2 THEN tt.TermName
        WHEN fg.FeeTenurityID = 3 THEN tm.Month
        ELSE 'N/A'
    END AS FeeType,
    COALESCE(ts.Amount, tt.Amount, tm.Amount) AS FeeAmount,
    cg.ConcessionGroupType AS ConcessionGroup,
    CASE 
         WHEN COALESCE(ap.PaymentDate, GETDATE()) <= lfrs.DueDate THEN 0
         WHEN COALESCE(ap.TotalPaid, 0) >= COALESCE(ts.Amount, tt.Amount, tm.Amount) THEN 0
         WHEN DATEDIFF(DAY, lfrs.DueDate, COALESCE(ap.PaymentDate, GETDATE()))
              BETWEEN fr.MinDays AND fr.MaxDays THEN fr.LateFee
         ELSE 0
    END AS LateFee
FROM tbl_StudentMaster sm
INNER JOIN tbl_Class c 
    ON sm.class_id = c.class_id
INNER JOIN tbl_Section s 
    ON sm.section_id = s.section_id
INNER JOIN tblFeeGroupClassSection fgcs 
    ON sm.class_id = fgcs.ClassID 
       AND sm.section_id = fgcs.SectionID
INNER JOIN tblFeeGroup fg 
    ON fgcs.FeeGroupID = fg.FeeGroupID
INNER JOIN tblFeeHead fh 
    ON fg.FeeHeadID = fh.FeeHeadID
LEFT JOIN tblTenuritySingle ts 
    ON fg.FeeTenurityID = 1 
       AND ts.FeeCollectionID = fgcs.FeeGroupID
LEFT JOIN tblTenurityTerm tt 
    ON fg.FeeTenurityID = 2 
       AND tt.FeeCollectionID = fgcs.FeeGroupID
LEFT JOIN tblTenurityMonthly tm 
    ON fg.FeeTenurityID = 3 
       AND tm.FeeCollectionID = fgcs.FeeGroupID
LEFT JOIN tblStudentConcession sc 
    ON sm.student_id = sc.StudentID 
       AND sm.Institute_id = sc.InstituteID 
       AND sc.IsActive = 1
LEFT JOIN tblConcessionGroup cg 
    ON sc.ConcessionGroupID = cg.ConcessionGroupID 
       AND cg.IsActive = 1
LEFT JOIN tblLateFeeClassSectionMapping lfm 
    ON sm.class_id = lfm.ClassID 
       AND sm.section_id = lfm.SectionID
LEFT JOIN UniqueLFRS lfrs 
    ON lfm.LateFeeRuleID = lfrs.LateFeeRuleID 
       AND lfrs.FeeHeadID = fh.FeeHeadID 
       AND lfrs.InstituteID = sm.Institute_id 
       AND lfrs.IsActive = 1
LEFT JOIN AggregatedPayments ap 
    ON ap.StudentID = sm.student_id 
       AND ap.ClassID = sm.class_id 
       AND ap.SectionID = sm.section_id 
       AND ap.InstituteID = sm.Institute_id 
       AND ap.FeeGroupID = fg.FeeGroupID 
       AND ap.FeeHeadID = fh.FeeHeadID 
       AND ap.FeeTenurityID = fg.FeeTenurityID
LEFT JOIN tblFeesRules fr 
    ON fr.LateFeeRuleID = lfrs.LateFeeRuleID 
       AND DATEDIFF(DAY, lfrs.DueDate, COALESCE(ap.PaymentDate, GETDATE()))
           BETWEEN fr.MinDays AND fr.MaxDays
WHERE sm.class_id = @ClassID 
  AND sm.section_id = @SectionID
  AND sm.Institute_id = @InstituteID
  AND (@Search IS NULL OR 
       sm.Admission_Number LIKE '%' + @Search + '%' OR
       CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) LIKE '%' + @Search + '%')
ORDER BY sm.Admission_Number;
";

            var param = new
            {
                request.ClassID,
                request.SectionID,
                request.InstituteID,
                Search = request.Search
            };

            // Create a new connection instance
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                // Step 1: Query raw rows
                var rawRows = await connection.QueryAsync<StudentFeeRaw>(sqlQuery, param);

                // Step 2: Group rows by Student and map to StudentFeeResponse
                var groupedData = rawRows
                    .GroupBy(r => new
                    {
                        r.StudentID,
                        r.AdmissionNo,
                        r.StudentName,
                        r.RollNo,
                        r.ClassName,
                        r.SectionName,
                        r.ConcessionGroup,
                        r.FeeGroupID,
                        r.FeeTenurityID
                    })
                    .Select(grp =>
                    {
                        var first = grp.First();

                        return new StudentFeeResponse
                        {
                            StudentID = first.StudentID,
                            AdmissionNo = first.AdmissionNo,
                            StudentName = first.StudentName,
                            RollNo = first.RollNo,
                            ClassName = first.ClassName,
                            SectionName = first.SectionName,
                            ConcessionGroup = first.ConcessionGroup,
                            FeeGroupID = first.FeeGroupID,
                            FeeTenurityID = first.FeeTenurityID,
                            TotalLateFee = grp.Sum(x => x.LateFee),
                            TotalFeeAmount = grp.Sum(x => x.FeeAmount),
                            FeeDetails = grp.Select(x => new StudentFeeDetail
                            {
                                FeeHeadID = x.FeeHeadID,
                                FeeHead = x.FeeHead,
                                TenureType = x.FeeType,
                                Amount = x.FeeAmount,
                                LateFee = x.LateFee
                            }).ToList()
                        };
                    })
                    .ToList();

                return groupedData;
            }
        }



        internal class StudentFeeRaw
        {
            public int StudentID { get; set; }
            public string AdmissionNo { get; set; }
            public string StudentName { get; set; }
            public string RollNo { get; set; }
            public string ClassName { get; set; }
            public string SectionName { get; set; }
            public string ConcessionGroup { get; set; }
            public int FeeHeadID { get; set; }
            public string FeeHead { get; set; }
            public int FeeGroupID { get; set; }
            public int FeeTenurityID { get; set; }
            public string FeeType { get; set; }
            public decimal FeeAmount { get; set; }
            public decimal LateFee { get; set; }
        }



    }
}
