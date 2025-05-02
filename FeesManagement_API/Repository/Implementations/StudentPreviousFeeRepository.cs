using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Response;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FeesManagement_API.Repository.Implementations
{
    public class StudentPreviousFeeRepository : IStudentPreviousFeeRepository
    {
        private readonly IConfiguration _configuration;

        public StudentPreviousFeeRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<StudentPreviousFeeResponse> GetStudentPreviousFees(StudentPreviousFeeRequest request)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var query = @"
WITH UniqueLFRS AS (
    SELECT DISTINCT LateFeeRuleID, FeeHeadID, FeeTenurityID, DueDate, InstituteID, IsActive
    FROM tblLateFeeRuleSetup
),
PaymentFromPayment AS (
    SELECT StudentID, ClassID, SectionID, InstituteID,
           FeeGroupID, FeeHeadID, FeeTenurityID,
           Amount     AS AmountPaid,
           NULL       AS PaymentDate
    FROM tblStudentFeePayment
),
PaymentFromTransaction AS (
    SELECT p.StudentID, p.ClassID, p.SectionID, p.InstituteID,
           p.FeeGroupID, p.FeeHeadID, p.FeeTenurityID,
           t.PayableAmount   AS AmountPaid,
           t.TransactionDate AS PaymentDate
    FROM tblStudentFeePaymentTransaction t
    JOIN tblStudentFeePayment p
      ON t.TransactionCode = p.TransactionCode
),
AggregatedPayments AS (
    SELECT StudentID, ClassID, SectionID, InstituteID,
           FeeGroupID, FeeHeadID, FeeTenurityID,
           SUM(AmountPaid) AS TotalPaid,
           MIN(PaymentDate) AS PaymentDate
    FROM (
      SELECT * FROM PaymentFromPayment
      UNION ALL
      SELECT * FROM PaymentFromTransaction
    ) AS X
    GROUP BY StudentID, ClassID, SectionID, InstituteID,
             FeeGroupID, FeeHeadID, FeeTenurityID
)
SELECT  
    sm.student_id        AS StudentID,
    sm.class_id          AS ClassID,
    sm.section_id        AS SectionID,
    sm.Admission_Number  AS AdmissionNo,
    CONCAT(sm.First_Name,' ',sm.Middle_Name,' ',sm.Last_Name) AS StudentName,
    sm.Roll_Number       AS RollNo,
    c.class_name         AS ClassName,
    s.section_name       AS SectionName,

    fh.FeeHeadID         AS FeeHeadID,
    fh.FeeHead,

    fg.FeeGroupID        AS FeeGroupID,
    fg.FeeTenurityID     AS FeeTenurityID,

    CASE 
      WHEN fg.FeeTenurityID = 1 THEN 'Single'
      WHEN fg.FeeTenurityID = 2 THEN tt.TermName
      WHEN fg.FeeTenurityID = 3 THEN tm.Month
      ELSE 'N/A'
    END                   AS FeeType,

    -- base fee overridden by any discount
    COALESCE(sd.Amount, COALESCE(ts.Amount,tt.Amount,tm.Amount)) AS FeeAmount,

    cg.ConcessionGroupType AS ConcessionGroup,

    -- late fee
    CASE 
      WHEN COALESCE(ap.PaymentDate,GETDATE()) <= lfrs.DueDate THEN 0
      WHEN COALESCE(ap.TotalPaid,0) >= COALESCE(ts.Amount,tt.Amount,tm.Amount) THEN 0
      WHEN DATEDIFF(DAY,lfrs.DueDate,COALESCE(ap.PaymentDate,GETDATE()))
           BETWEEN fr.MinDays AND fr.MaxDays THEN fr.LateFee
      ELSE 0
    END                  AS LateFee,

    -- waiver
    COALESCE(
      CASE 
        WHEN fg.FeeTenurityID=1 THEN sw.Amount
        WHEN fg.FeeTenurityID=2 THEN (
          SELECT w.Amount FROM tblStudentFeeWaiver w
           WHERE w.StudentID=sm.student_id
             AND w.FeeHeadID=fh.FeeHeadID
             AND w.FeeGroupID=fg.FeeGroupID
             AND w.TenuritySTMID=tt.TenurityTermID
        )
        WHEN fg.FeeTenurityID=3 THEN (
          SELECT w.Amount FROM tblStudentFeeWaiver w
           WHERE w.StudentID=sm.student_id
             AND w.FeeHeadID=fh.FeeHeadID
             AND w.FeeGroupID=fg.FeeGroupID
             AND w.TenuritySTMID=tm.TenurityMonthID
        )
        ELSE 0
      END
    ,0)                   AS WaiverAmount,

    -- discount
    COALESCE(
      CASE 
        WHEN fg.FeeTenurityID=1 THEN sd.Amount
        WHEN fg.FeeTenurityID=2 THEN (
          SELECT d.Amount FROM tblStudentDiscount d
           WHERE d.StudentID=sm.student_id
             AND d.FeeHeadID=fh.FeeHeadID
             AND d.FeeGroupID=fg.FeeGroupID
             AND d.TenuritySTMID=tt.TenurityTermID
        )
        WHEN fg.FeeTenurityID=3 THEN (
          SELECT d.Amount FROM tblStudentDiscount d
           WHERE d.StudentID=sm.student_id
             AND d.FeeHeadID=fh.FeeHeadID
             AND d.FeeGroupID=fg.FeeGroupID
             AND d.TenuritySTMID=tm.TenurityMonthID
        )
        ELSE 0
      END
    ,0)                   AS DiscountAmount,

    -- paid
    COALESCE(
      CASE 
        WHEN fg.FeeTenurityID=1 THEN sp.Amount
        WHEN fg.FeeTenurityID=2 THEN (
          SELECT p2.Amount FROM tblStudentFeePayment p2
           WHERE p2.StudentID=sm.student_id
             AND p2.FeeHeadID=fh.FeeHeadID
             AND p2.FeeGroupID=fg.FeeGroupID
             AND p2.TenuritySTMID=tt.TenurityTermID
        )
        WHEN fg.FeeTenurityID=3 THEN (
          SELECT p3.Amount FROM tblStudentFeePayment p3
           WHERE p3.StudentID=sm.student_id
             AND p3.FeeHeadID=fh.FeeHeadID
             AND p3.FeeGroupID=fg.FeeGroupID
             AND p3.TenuritySTMID=tm.TenurityMonthID
        )
        ELSE 0
      END
    ,0)                   AS PaidAmount,

    -- balance = base_fee – paid – waived – discount
    COALESCE(ts.Amount,tt.Amount,tm.Amount,0)
      - COALESCE(sp.Amount,0)
      - COALESCE(sw.Amount,0)
      - COALESCE(sd.Amount,0) AS Balance,

    -- tenure & collection IDs
    CASE WHEN fg.FeeTenurityID=1 THEN ts.TenuritySingleID
         WHEN fg.FeeTenurityID=2 THEN tt.TenurityTermID
         WHEN fg.FeeTenurityID=3 THEN tm.TenurityMonthID
         ELSE NULL END AS TenuritySTMID,

    CASE WHEN fg.FeeTenurityID=1 THEN ts.FeeCollectionID
         WHEN fg.FeeTenurityID=2 THEN tt.FeeCollectionID
         WHEN fg.FeeTenurityID=3 THEN tm.FeeCollectionID
         ELSE NULL END AS FeeCollectionSTMID

FROM tbl_StudentMaster sm
INNER JOIN tblStudentStandards ss
  ON ss.StudentID=sm.student_id
 AND ss.ClassID=sm.class_id
 AND ss.SectionID=sm.section_id
 AND ss.InstituteID=sm.Institute_id
 AND ss.AcademicYearCode=@AcademicYearCode

INNER JOIN tbl_Class c ON sm.class_id=c.class_id
INNER JOIN tbl_Section s ON sm.section_id=s.section_id
INNER JOIN tblFeeGroupClassSection fgcs
  ON sm.class_id=fgcs.ClassID
 AND sm.section_id=fgcs.SectionID
INNER JOIN tblFeeGroup fg
  ON fgcs.FeeGroupID=fg.FeeGroupID
 AND fg.AcademicYearCode=@AcademicYearCode
 AND fg.InstituteID=sm.Institute_id
 AND fg.IsActive=1
INNER JOIN tblFeeHead fh
  ON fg.FeeHeadID=fh.FeeHeadID

LEFT JOIN tblTenuritySingle ts
  ON fg.FeeTenurityID=1
 AND ts.FeeCollectionID=fgcs.FeeGroupID
LEFT JOIN tblTenurityTerm tt
  ON fg.FeeTenurityID=2
 AND tt.FeeCollectionID=fgcs.FeeGroupID
LEFT JOIN tblTenurityMonthly tm
  ON fg.FeeTenurityID=3
 AND tm.FeeCollectionID=fgcs.FeeGroupID

LEFT JOIN tblStudentFeeWaiver sw
  ON sw.StudentID=sm.student_id
 AND sw.FeeHeadID=fh.FeeHeadID
 AND sw.FeeGroupID=fg.FeeGroupID

LEFT JOIN tblStudentDiscount sd
  ON sd.StudentID=sm.student_id
 AND sd.FeeHeadID=fh.FeeHeadID
 AND sd.FeeGroupID=fg.FeeGroupID

LEFT JOIN tblStudentFeePayment sp
  ON sp.StudentID=sm.student_id
 AND sp.FeeHeadID=fh.FeeHeadID
 AND sp.FeeGroupID=fg.FeeGroupID

LEFT JOIN tblStudentConcession sc
  ON sc.StudentID=sm.student_id
 AND sc.InstituteID=sm.Institute_id
 AND sc.IsActive=1
LEFT JOIN tblConcessionGroup cg
  ON cg.ConcessionGroupID=sc.ConcessionGroupID
 AND cg.IsActive=1

LEFT JOIN tblLateFeeClassSectionMapping lfm
  ON lfm.ClassID=sm.class_id
 AND lfm.SectionID=sm.section_id
LEFT JOIN UniqueLFRS lfrs
  ON lfm.LateFeeRuleID=lfrs.LateFeeRuleID
 AND lfrs.FeeHeadID=fh.FeeHeadID
 AND lfrs.InstituteID=sm.Institute_id
 AND lfrs.IsActive=1
LEFT JOIN AggregatedPayments ap
  ON ap.StudentID=sm.student_id
 AND ap.ClassID=sm.class_id
 AND ap.SectionID=sm.section_id
 AND ap.InstituteID=sm.Institute_id
 AND ap.FeeGroupID=fg.FeeGroupID
 AND ap.FeeHeadID=fh.FeeHeadID
 AND ap.FeeTenurityID=fg.FeeTenurityID
LEFT JOIN tblFeesRules fr
  ON fr.LateFeeRuleID=lfrs.LateFeeRuleID
 AND DATEDIFF(
       DAY,
       lfrs.DueDate,
       COALESCE(ap.PaymentDate,GETDATE())
     ) BETWEEN fr.MinDays AND fr.MaxDays

WHERE sm.class_id     = @ClassID
  AND sm.section_id   = @SectionID
  AND sm.Institute_id = @InstituteID
  AND (
       @Search IS NULL
    OR sm.Admission_Number LIKE '%' + @Search + '%'
    OR CONCAT(
         sm.First_Name,' ',
         sm.Middle_Name,' ',
         sm.Last_Name
       ) LIKE '%' + @Search + '%'
  )
ORDER BY sm.Admission_Number;
";

            var data = connection.Query<StudentPreviousFeeData>(query, new
            {
                request.InstituteID,
                request.ClassID,
                request.SectionID,
                request.Search,
                request.AcademicYearCode
            }).ToList();

            var response = data
              .GroupBy(x => new {
                  x.StudentID,
                  x.AdmissionNo,
                  x.StudentName,
                  x.RollNo,
                  x.ClassID,
                  x.SectionID,
                  x.ClassName,
                  x.SectionName,
                  x.ConcessionGroup,
                  x.FeeGroupID,
                  x.FeeTenurityID
              })
              .Select(g => new StudentPreviousFeeResponse
              {
                  StudentID = g.Key.StudentID,
                  AdmissionNo = g.Key.AdmissionNo,
                  StudentName = g.Key.StudentName,
                  RollNo = g.Key.RollNo,
                  ClassID = g.Key.ClassID,
                  SectionID = g.Key.SectionID,
                  ClassName = g.Key.ClassName,
                  SectionName = g.Key.SectionName,
                  ConcessionGroup = g.Key.ConcessionGroup,
                  FeeGroupID = g.Key.FeeGroupID,
                  FeeTenurityID = g.Key.FeeTenurityID,
                  TotalFeeAmount = g.Sum(x => x.FeeAmount),
                  TotalLateFee = g.Sum(x => x.LateFee),
                  TotalWaiverAmount = g.Sum(x => x.WaiverAmount),
                  TotalDiscountAmount = g.Sum(x => x.DiscountAmount),
                  TotalPaidAmount = g.Sum(x => x.PaidAmount),
                  TotalBalance = g.Sum(x => x.Balance),
                  FeeDetails = g.Select(x => new StudentPreviousFeeDetail
                  {
                      FeeHeadID = x.FeeHeadID,
                      FeeHead = x.FeeHead,
                      TenureType = x.FeeType,
                      Amount = x.FeeAmount,
                      LateFee = x.LateFee,
                      WaiverAmount = x.WaiverAmount,
                      DiscountAmount = x.DiscountAmount,
                      PaidAmount = x.PaidAmount,
                      Balance = x.Balance,
                      TenuritySTMID = x.TenuritySTMID,
                      FeeCollectionSTMID = x.FeeCollectionSTMID
                  }).ToList()
              })
              .ToList();

            return response;
        }


        //public List<StudentPreviousFeeResponse> GetStudentPreviousFees(StudentPreviousFeeRequest request)
        //{
        //    using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    var query = @"
        //    WITH UniqueLFRS AS (
        //        SELECT DISTINCT LateFeeRuleID, FeeHeadID, FeeTenurityID, DueDate, InstituteID, IsActive
        //        FROM tblLateFeeRuleSetup
        //    ),
        //    PaymentFromPayment AS (
        //        SELECT StudentID, ClassID, SectionID, InstituteID,
        //               FeeGroupID, FeeHeadID, FeeTenurityID,
        //               Amount     AS AmountPaid,
        //               NULL       AS PaymentDate
        //        FROM tblStudentFeePayment
        //    ),
        //    PaymentFromTransaction AS (
        //        SELECT p.StudentID, p.ClassID, p.SectionID, p.InstituteID,
        //               p.FeeGroupID, p.FeeHeadID, p.FeeTenurityID,
        //               t.PayableAmount   AS AmountPaid,
        //               t.TransactionDate AS PaymentDate
        //        FROM tblStudentFeePaymentTransaction t
        //        JOIN tblStudentFeePayment p
        //          ON t.TransactionCode = p.TransactionCode
        //    ),
        //    AggregatedPayments AS (
        //        SELECT StudentID, ClassID, SectionID, InstituteID,
        //               FeeGroupID, FeeHeadID, FeeTenurityID,
        //               SUM(AmountPaid) AS TotalPaid,
        //               MIN(PaymentDate) AS PaymentDate
        //        FROM (
        //          SELECT * FROM PaymentFromPayment
        //          UNION ALL
        //          SELECT * FROM PaymentFromTransaction
        //        ) AS X
        //        GROUP BY StudentID, ClassID, SectionID, InstituteID,
        //                 FeeGroupID, FeeHeadID, FeeTenurityID
        //    )
        //    SELECT  
        //        sm.student_id        AS StudentID,
        //        sm.class_id          AS ClassID,
        //        sm.section_id        AS SectionID,
        //        sm.Admission_Number  AS AdmissionNo,
        //        CONCAT(sm.First_Name,' ',sm.Middle_Name,' ',sm.Last_Name) AS StudentName,
        //        sm.Roll_Number       AS RollNo,
        //        c.class_name         AS ClassName,
        //        s.section_name       AS SectionName,
        //        fh.FeeHeadID         AS FeeHeadID,
        //        fh.FeeHead,
        //        fg.FeeGroupID        AS FeeGroupID,
        //        fg.FeeTenurityID     AS FeeTenurityID,
        //        CASE 
        //          WHEN fg.FeeTenurityID = 1 THEN 'Single'
        //          WHEN fg.FeeTenurityID = 2 THEN tt.TermName
        //          WHEN fg.FeeTenurityID = 3 THEN tm.Month
        //          ELSE 'N/A'
        //        END                   AS FeeType,
        //        COALESCE(
        //            sd.Amount,                            -- tblStudentDiscount.Amount
        //            COALESCE(ts.Amount, tt.Amount, tm.Amount)
        //        )                     AS FeeAmount,
        //        cg.ConcessionGroupType AS ConcessionGroup,
        //        CASE 
        //          WHEN COALESCE(ap.PaymentDate,GETDATE()) <= lfrs.DueDate THEN 0
        //          WHEN COALESCE(ap.TotalPaid,0) >= COALESCE(ts.Amount, tt.Amount, tm.Amount) THEN 0
        //          WHEN DATEDIFF(
        //                DAY,
        //                lfrs.DueDate,
        //                COALESCE(ap.PaymentDate,GETDATE())
        //               ) BETWEEN fr.MinDays AND fr.MaxDays THEN fr.LateFee
        //          ELSE 0
        //        END                  AS LateFee,
        //        CASE 
        //          WHEN fg.FeeTenurityID = 1 THEN ts.TenuritySingleID
        //          WHEN fg.FeeTenurityID = 2 THEN tt.TenurityTermID
        //          WHEN fg.FeeTenurityID = 3 THEN tm.TenurityMonthID
        //          ELSE NULL
        //        END                  AS TenuritySTMID,
        //        CASE 
        //          WHEN fg.FeeTenurityID = 1 THEN ts.FeeCollectionID
        //          WHEN fg.FeeTenurityID = 2 THEN tt.FeeCollectionID
        //          WHEN fg.FeeTenurityID = 3 THEN tm.FeeCollectionID
        //          ELSE NULL
        //        END                  AS FeeCollectionSTMID
        //    FROM tbl_StudentMaster sm

        //    -- 1) only students in the selected academic year
        //    INNER JOIN tblStudentStandards ss
        //      ON ss.StudentID        = sm.student_id
        //     AND ss.ClassID          = sm.class_id
        //     AND ss.SectionID        = sm.section_id
        //     AND ss.InstituteID      = sm.Institute_id
        //     AND ss.AcademicYearCode = @AcademicYearCode

        //    INNER JOIN tbl_Class c
        //      ON sm.class_id   = c.class_id
        //    INNER JOIN tbl_Section s
        //      ON sm.section_id = s.section_id

        //    -- 2) map to all fee‐groups for that class & section
        //    INNER JOIN tblFeeGroupClassSection fgcs
        //      ON sm.class_id   = fgcs.ClassID
        //     AND sm.section_id = fgcs.SectionID

        //    -- 3) then limit fee‐group to the same academic year
        //    INNER JOIN tblFeeGroup fg
        //      ON fgcs.FeeGroupID    = fg.FeeGroupID
        //     AND fg.AcademicYearCode = @AcademicYearCode
        //     AND fg.InstituteID     = sm.Institute_id
        //     AND fg.IsActive        = 1

        //    INNER JOIN tblFeeHead fh 
        //      ON fg.FeeHeadID    = fh.FeeHeadID

        //    LEFT JOIN tblTenuritySingle ts 
        //      ON fg.FeeTenurityID   = 1 
        //     AND ts.FeeCollectionID = fgcs.FeeGroupID
        //    LEFT JOIN tblTenurityTerm tt 
        //      ON fg.FeeTenurityID   = 2 
        //     AND tt.FeeCollectionID = fgcs.FeeGroupID
        //    LEFT JOIN tblTenurityMonthly tm 
        //      ON fg.FeeTenurityID   = 3 
        //     AND tm.FeeCollectionID = fgcs.FeeGroupID

        //    -- bring in any student‐specific discount
        //    LEFT JOIN tblStudentDiscount sd
        //      ON sd.StudentID          = sm.student_id
        //     AND sd.ClassID            = sm.class_id
        //     AND sd.SectionID          = sm.section_id
        //     AND sd.InstituteID        = sm.Institute_id
        //     AND sd.FeeGroupID         = fg.FeeGroupID
        //     AND sd.FeeHeadID          = fh.FeeHeadID
        //     AND sd.FeeTenurityID      = fg.FeeTenurityID
        //     AND sd.TenuritySTMID      = CASE 
        //                                   WHEN fg.FeeTenurityID = 1 THEN ts.TenuritySingleID
        //                                   WHEN fg.FeeTenurityID = 2 THEN tt.TenurityTermID
        //                                   WHEN fg.FeeTenurityID = 3 THEN tm.TenurityMonthID
        //                                 END
        //     AND sd.FeeCollectionSTMID = CASE 
        //                                   WHEN fg.FeeTenurityID = 1 THEN ts.FeeCollectionID
        //                                   WHEN fg.FeeTenurityID = 2 THEN tt.FeeCollectionID
        //                                   WHEN fg.FeeTenurityID = 3 THEN tm.FeeCollectionID
        //                                 END

        //    LEFT JOIN tblStudentConcession sc 
        //      ON sm.student_id  = sc.StudentID
        //     AND sm.Institute_id = sc.InstituteID
        //     AND sc.IsActive    = 1
        //    LEFT JOIN tblConcessionGroup cg 
        //      ON sc.ConcessionGroupID = cg.ConcessionGroupID
        //     AND cg.IsActive         = 1
        //    LEFT JOIN tblLateFeeClassSectionMapping lfm 
        //      ON sm.class_id   = lfm.ClassID
        //     AND sm.section_id = lfm.SectionID
        //    LEFT JOIN UniqueLFRS lfrs 
        //      ON lfm.LateFeeRuleID = lfrs.LateFeeRuleID
        //     AND lfrs.FeeHeadID    = fh.FeeHeadID
        //     AND lfrs.InstituteID  = sm.Institute_id
        //     AND lfrs.IsActive     = 1
        //    LEFT JOIN AggregatedPayments ap 
        //      ON ap.StudentID     = sm.student_id
        //     AND ap.ClassID       = sm.class_id
        //     AND ap.SectionID     = sm.section_id
        //     AND ap.InstituteID   = sm.Institute_id
        //     AND ap.FeeGroupID    = fg.FeeGroupID
        //     AND ap.FeeHeadID     = fh.FeeHeadID
        //     AND ap.FeeTenurityID = fg.FeeTenurityID
        //    LEFT JOIN tblFeesRules fr 
        //      ON fr.LateFeeRuleID = lfrs.LateFeeRuleID
        //     AND DATEDIFF(
        //           DAY,
        //           lfrs.DueDate,
        //           COALESCE(ap.PaymentDate,GETDATE())
        //         ) BETWEEN fr.MinDays AND fr.MaxDays

        //    WHERE sm.class_id     = @ClassID 
        //      AND sm.section_id   = @SectionID
        //      AND sm.Institute_id = @InstituteID
        //      AND (
        //           @Search IS NULL
        //        OR sm.Admission_Number LIKE '%' + @Search + '%'
        //        OR CONCAT(sm.First_Name,' ',sm.Middle_Name,' ',sm.Last_Name)
        //              LIKE '%' + @Search + '%'
        //      )
        //    ORDER BY sm.Admission_Number;
        //    ";

        //    var result = connection
        //        .Query<StudentPreviousFeeData>(query, new
        //        {
        //            request.InstituteID,
        //            request.ClassID,
        //            request.SectionID,
        //            request.Search,
        //            request.AcademicYearCode    // pass it in
        //        })
        //        .ToList();

        //    // ... group & map to StudentPreviousFeeResponse as before ...
        //    var response = result
        //        .GroupBy(x => new {
        //            x.StudentID,
        //            x.AdmissionNo,
        //            x.StudentName,
        //            x.RollNo,
        //            x.ClassID,
        //            x.SectionID,
        //            x.ClassName,
        //            x.SectionName,
        //            x.ConcessionGroup,
        //            x.FeeGroupID,
        //            x.FeeTenurityID
        //        })
        //        .Select(g => new StudentPreviousFeeResponse
        //        {
        //            StudentID = g.Key.StudentID,
        //            AdmissionNo = g.Key.AdmissionNo,
        //            StudentName = g.Key.StudentName,
        //            RollNo = g.Key.RollNo,
        //            ClassID = g.Key.ClassID,
        //            SectionID = g.Key.SectionID,
        //            ClassName = g.Key.ClassName,
        //            SectionName = g.Key.SectionName,
        //            ConcessionGroup = g.Key.ConcessionGroup,
        //            FeeGroupID = g.Key.FeeGroupID,
        //            FeeTenurityID = g.Key.FeeTenurityID,
        //            TotalFeeAmount = g.Sum(x => x.FeeAmount),
        //            TotalLateFee = g.Sum(x => x.LateFee),
        //            FeeDetails = g.Select(x => new StudentPreviousFeeDetail
        //            {
        //                FeeHeadID = x.FeeHeadID,
        //                FeeHead = x.FeeHead,
        //                TenureType = x.FeeType,
        //                Amount = x.FeeAmount,
        //                LateFee = x.LateFee,
        //                TenuritySTMID = x.TenuritySTMID,
        //                FeeCollectionSTMID = x.FeeCollectionSTMID
        //            }).ToList()
        //        })
        //        .ToList();

        //    return response;
        //}

        public IEnumerable<int> DiscountStudentPreviousFees(IEnumerable<DiscountStudentPreviousFeesRequest> requests)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            connection.Open();
            using var tx = connection.BeginTransaction();

            var ids = new List<int>();

            foreach (var req in requests)
            {
                // 1) Look for an existing discount
                const string findSql = @"
            SELECT FeesDiscountID
            FROM tblStudentDiscount
            WHERE StudentID        = @StudentID
              AND ClassID          = @ClassID
              AND SectionID        = @SectionID
              AND InstituteID      = @InstituteID
              AND FeeGroupID       = @FeeGroupID
              AND FeeHeadID        = @FeeHeadID
              AND FeeTenurityID    = @FeeTenurityID
              AND TenuritySTMID    = @TenuritySTMID
              AND FeeCollectionSTMID = @FeeCollectionSTMID;
        ";
                var existingId = connection
                    .QuerySingleOrDefault<int?>(findSql, req, tx);

                if (existingId.HasValue)
                {
                    // 2a) Update the one existing row
                    const string updSql = @"
                UPDATE tblStudentDiscount
                SET Amount          = @DiscountedAmount,
                    FeeDiscountDate = GETDATE(),
                    DiscountGivenBy = @DiscountGivenBy
                WHERE FeesDiscountID = @FeesDiscountID;
            ";
                    connection.Execute(updSql, new
                    {
                        req.DiscountGivenBy,
                        req.DiscountedAmount,
                        FeesDiscountID = existingId.Value
                    }, tx);

                    ids.Add(existingId.Value);
                }
                else
                {
                    // 2b) Insert & get new ID
                    const string insSql = @"
                INSERT INTO tblStudentDiscount
                (
                    StudentID, ClassID, SectionID, InstituteID,
                    FeeGroupID, FeeHeadID, FeeTenurityID, TenuritySTMID, FeeCollectionSTMID,
                    Amount, FeeDiscountDate, DiscountGivenBy
                )
                VALUES
                (
                    @StudentID, @ClassID, @SectionID, @InstituteID,
                    @FeeGroupID, @FeeHeadID, @FeeTenurityID, @TenuritySTMID, @FeeCollectionSTMID,
                    @DiscountedAmount, GETDATE(), @DiscountGivenBy
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";
                    var newId = connection.QuerySingle<int>(insSql, req, tx);
                    ids.Add(newId);
                }
            }

            tx.Commit();
            return ids;
        }
         
        public List<GetPreviousFeesChangeLogsResponse> GetPreviousFeesChangeLogs(GetPreviousFeesChangeLogsRequest request)
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
                var result = connection.Query<GetPreviousFeesChangeLogsResponse>(query, request).ToList();
                return result;
            }
        }

        public async Task<IEnumerable<StudentPreviousFeeRawData>> GetStudentPreviousFeeRawDataAsync(GetStudentPreviousFeesExportRequest request)
        {
            string sql = @"
            WITH UniqueLFRS AS (
                SELECT DISTINCT LateFeeRuleID, FeeHeadID, FeeTenurityID, DueDate, InstituteID, IsActive
                FROM tblLateFeeRuleSetup
            ),
            PaymentFromPayment AS (
                SELECT StudentID, ClassID, SectionID, InstituteID,
                       FeeGroupID, FeeHeadID, FeeTenurityID,
                       Amount     AS AmountPaid,
                       NULL       AS PaymentDate
                FROM tblStudentFeePayment
            ),
            PaymentFromTransaction AS (
                SELECT p.StudentID, p.ClassID, p.SectionID, p.InstituteID,
                       p.FeeGroupID, p.FeeHeadID, p.FeeTenurityID,
                       t.PayableAmount   AS AmountPaid,
                       t.TransactionDate AS PaymentDate
                FROM tblStudentFeePaymentTransaction t
                JOIN tblStudentFeePayment p
                  ON t.TransactionCode = p.TransactionCode
            ),
            AggregatedPayments AS (
                SELECT StudentID, ClassID, SectionID, InstituteID,
                       FeeGroupID, FeeHeadID, FeeTenurityID,
                       SUM(AmountPaid) AS TotalPaid,
                       MIN(PaymentDate) AS PaymentDate
                FROM (
                  SELECT * FROM PaymentFromPayment
                  UNION ALL
                  SELECT * FROM PaymentFromTransaction
                ) AS X
                GROUP BY StudentID, ClassID, SectionID, InstituteID,
                         FeeGroupID, FeeHeadID, FeeTenurityID
            )
            SELECT  
                sm.student_id        AS StudentID,
                sm.class_id          AS ClassID,
                sm.section_id        AS SectionID,
                sm.Admission_Number  AS AdmissionNo,
                CONCAT(sm.First_Name,' ',sm.Middle_Name,' ',sm.Last_Name) AS StudentName,
                sm.Roll_Number       AS RollNo,
                c.class_name         AS ClassName,
                s.section_name       AS SectionName,

                fh.FeeHeadID         AS FeeHeadID,
                fh.FeeHead,

                fg.FeeGroupID        AS FeeGroupID,
                fg.FeeTenurityID     AS FeeTenurityID,

                CASE 
                  WHEN fg.FeeTenurityID = 1 THEN 'Single'
                  WHEN fg.FeeTenurityID = 2 THEN tt.TermName
                  WHEN fg.FeeTenurityID = 3 THEN tm.Month
                  ELSE 'N/A'
                END                   AS FeeType,

                -- base fee overridden by any discount
                COALESCE(sd.Amount, COALESCE(ts.Amount,tt.Amount,tm.Amount)) AS FeeAmount,

                cg.ConcessionGroupType AS ConcessionGroup,

                -- late fee
                CASE 
                  WHEN COALESCE(ap.PaymentDate,GETDATE()) <= lfrs.DueDate THEN 0
                  WHEN COALESCE(ap.TotalPaid,0) >= COALESCE(ts.Amount,tt.Amount,tm.Amount) THEN 0
                  WHEN DATEDIFF(DAY,lfrs.DueDate,COALESCE(ap.PaymentDate,GETDATE()))
                       BETWEEN fr.MinDays AND fr.MaxDays THEN fr.LateFee
                  ELSE 0
                END                  AS LateFee,

                -- waiver
                COALESCE(
                  CASE 
                    WHEN fg.FeeTenurityID=1 THEN sw.Amount
                    WHEN fg.FeeTenurityID=2 THEN (
                      SELECT w.Amount FROM tblStudentFeeWaiver w
                       WHERE w.StudentID=sm.student_id
                         AND w.FeeHeadID=fh.FeeHeadID
                         AND w.FeeGroupID=fg.FeeGroupID
                         AND w.TenuritySTMID=tt.TenurityTermID
                    )
                    WHEN fg.FeeTenurityID=3 THEN (
                      SELECT w.Amount FROM tblStudentFeeWaiver w
                       WHERE w.StudentID=sm.student_id
                         AND w.FeeHeadID=fh.FeeHeadID
                         AND w.FeeGroupID=fg.FeeGroupID
                         AND w.TenuritySTMID=tm.TenurityMonthID
                    )
                    ELSE 0
                  END
                ,0)                   AS WaiverAmount,

                -- discount
                COALESCE(
                  CASE 
                    WHEN fg.FeeTenurityID=1 THEN sd.Amount
                    WHEN fg.FeeTenurityID=2 THEN (
                      SELECT d.Amount FROM tblStudentDiscount d
                       WHERE d.StudentID=sm.student_id
                         AND d.FeeHeadID=fh.FeeHeadID
                         AND d.FeeGroupID=fg.FeeGroupID
                         AND d.TenuritySTMID=tt.TenurityTermID
                    )
                    WHEN fg.FeeTenurityID=3 THEN (
                      SELECT d.Amount FROM tblStudentDiscount d
                       WHERE d.StudentID=sm.student_id
                         AND d.FeeHeadID=fh.FeeHeadID
                         AND d.FeeGroupID=fg.FeeGroupID
                         AND d.TenuritySTMID=tm.TenurityMonthID
                    )
                    ELSE 0
                  END
                ,0)                   AS DiscountAmount,

                -- paid
                COALESCE(
                  CASE 
                    WHEN fg.FeeTenurityID=1 THEN sp.Amount
                    WHEN fg.FeeTenurityID=2 THEN (
                      SELECT p2.Amount FROM tblStudentFeePayment p2
                       WHERE p2.StudentID=sm.student_id
                         AND p2.FeeHeadID=fh.FeeHeadID
                         AND p2.FeeGroupID=fg.FeeGroupID
                         AND p2.TenuritySTMID=tt.TenurityTermID
                    )
                    WHEN fg.FeeTenurityID=3 THEN (
                      SELECT p3.Amount FROM tblStudentFeePayment p3
                       WHERE p3.StudentID=sm.student_id
                         AND p3.FeeHeadID=fh.FeeHeadID
                         AND p3.FeeGroupID=fg.FeeGroupID
                         AND p3.TenuritySTMID=tm.TenurityMonthID
                    )
                    ELSE 0
                  END
                ,0)                   AS PaidAmount,

                -- balance = base_fee – paid – waived – discount
                COALESCE(ts.Amount,tt.Amount,tm.Amount,0)
                  - COALESCE(sp.Amount,0)
                  - COALESCE(sw.Amount,0)
                  - COALESCE(sd.Amount,0) AS Balance,

                -- tenure & collection IDs
                CASE WHEN fg.FeeTenurityID=1 THEN ts.TenuritySingleID
                     WHEN fg.FeeTenurityID=2 THEN tt.TenurityTermID
                     WHEN fg.FeeTenurityID=3 THEN tm.TenurityMonthID
                     ELSE NULL END AS TenuritySTMID,

                CASE WHEN fg.FeeTenurityID=1 THEN ts.FeeCollectionID
                     WHEN fg.FeeTenurityID=2 THEN tt.FeeCollectionID
                     WHEN fg.FeeTenurityID=3 THEN tm.FeeCollectionID
                     ELSE NULL END AS FeeCollectionSTMID

            FROM tbl_StudentMaster sm
            INNER JOIN tblStudentStandards ss
              ON ss.StudentID=sm.student_id
             AND ss.ClassID=sm.class_id
             AND ss.SectionID=sm.section_id
             AND ss.InstituteID=sm.Institute_id
             AND ss.AcademicYearCode=@AcademicYearCode

            INNER JOIN tbl_Class c ON sm.class_id=c.class_id
            INNER JOIN tbl_Section s ON sm.section_id=s.section_id
            INNER JOIN tblFeeGroupClassSection fgcs
              ON sm.class_id=fgcs.ClassID
             AND sm.section_id=fgcs.SectionID
            INNER JOIN tblFeeGroup fg
              ON fgcs.FeeGroupID=fg.FeeGroupID
             AND fg.AcademicYearCode=@AcademicYearCode
             AND fg.InstituteID=sm.Institute_id
             AND fg.IsActive=1
            INNER JOIN tblFeeHead fh
              ON fg.FeeHeadID=fh.FeeHeadID

            LEFT JOIN tblTenuritySingle ts
              ON fg.FeeTenurityID=1
             AND ts.FeeCollectionID=fgcs.FeeGroupID
            LEFT JOIN tblTenurityTerm tt
              ON fg.FeeTenurityID=2
             AND tt.FeeCollectionID=fgcs.FeeGroupID
            LEFT JOIN tblTenurityMonthly tm
              ON fg.FeeTenurityID=3
             AND tm.FeeCollectionID=fgcs.FeeGroupID

            LEFT JOIN tblStudentFeeWaiver sw
              ON sw.StudentID=sm.student_id
             AND sw.FeeHeadID=fh.FeeHeadID
             AND sw.FeeGroupID=fg.FeeGroupID

            LEFT JOIN tblStudentDiscount sd
              ON sd.StudentID=sm.student_id
             AND sd.FeeHeadID=fh.FeeHeadID
             AND sd.FeeGroupID=fg.FeeGroupID

            LEFT JOIN tblStudentFeePayment sp
              ON sp.StudentID=sm.student_id
             AND sp.FeeHeadID=fh.FeeHeadID
             AND sp.FeeGroupID=fg.FeeGroupID

            LEFT JOIN tblStudentConcession sc
              ON sc.StudentID=sm.student_id
             AND sc.InstituteID=sm.Institute_id
             AND sc.IsActive=1
            LEFT JOIN tblConcessionGroup cg
              ON cg.ConcessionGroupID=sc.ConcessionGroupID
             AND cg.IsActive=1

            LEFT JOIN tblLateFeeClassSectionMapping lfm
              ON lfm.ClassID=sm.class_id
             AND lfm.SectionID=sm.section_id
            LEFT JOIN UniqueLFRS lfrs
              ON lfm.LateFeeRuleID=lfrs.LateFeeRuleID
             AND lfrs.FeeHeadID=fh.FeeHeadID
             AND lfrs.InstituteID=sm.Institute_id
             AND lfrs.IsActive=1
            LEFT JOIN AggregatedPayments ap
              ON ap.StudentID=sm.student_id
             AND ap.ClassID=sm.class_id
             AND ap.SectionID=sm.section_id
             AND ap.InstituteID=sm.Institute_id
             AND ap.FeeGroupID=fg.FeeGroupID
             AND ap.FeeHeadID=fh.FeeHeadID
             AND ap.FeeTenurityID=fg.FeeTenurityID
            LEFT JOIN tblFeesRules fr
              ON fr.LateFeeRuleID=lfrs.LateFeeRuleID
             AND DATEDIFF(
                   DAY,
                   lfrs.DueDate,
                   COALESCE(ap.PaymentDate,GETDATE())
                 ) BETWEEN fr.MinDays AND fr.MaxDays

            WHERE sm.class_id     = @ClassID
              AND sm.section_id   = @SectionID
              AND sm.Institute_id = @InstituteID
              AND (
                   @Search IS NULL
                OR sm.Admission_Number LIKE '%' + @Search + '%'
                OR CONCAT(
                     sm.First_Name,' ',
                     sm.Middle_Name,' ',
                     sm.Last_Name
                   ) LIKE '%' + @Search + '%'
              )
            ORDER BY sm.Admission_Number;
            ";

            var parameters = new
            {
                InstituteID = request.InstituteID,
                ClassID = request.ClassID,
                SectionID = request.SectionID,
                AcademicYearCode = request.AcademicYearCode, 
                Search = string.IsNullOrWhiteSpace(request.Search) ? null : request.Search
            };

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var result = await connection.QueryAsync<StudentPreviousFeeRawData>(sql, parameters);
                return result;
            }
        }

        //public async Task<IEnumerable<StudentPreviousFeeRawData>> GetStudentPreviousFeeRawDataAsync(GetStudentPreviousFeesExportRequest request)
        //{
        //    string sql = @"
        //    WITH UniqueLFRS AS (
        //        SELECT DISTINCT 
        //               LateFeeRuleID, 
        //               FeeHeadID, 
        //               FeeTenurityID, 
        //               DueDate, 
        //               InstituteID, 
        //               IsActive
        //        FROM tblLateFeeRuleSetup
        //    ),
        //    PaymentFromPayment AS (
        //        SELECT 
        //           StudentID, 
        //           ClassID, 
        //           SectionID, 
        //           InstituteID, 
        //           FeeGroupID, 
        //           FeeHeadID, 
        //           FeeTenurityID, 
        //           Amount AS AmountPaid,
        //           NULL AS PaymentDate
        //        FROM tblStudentFeePayment
        //    ),
        //    PaymentFromTransaction AS (
        //        SELECT 
        //           p.StudentID, 
        //           p.ClassID, 
        //           p.SectionID, 
        //           p.InstituteID, 
        //           p.FeeGroupID, 
        //           p.FeeHeadID, 
        //           p.FeeTenurityID, 
        //           t.PaymentAmount AS AmountPaid,
        //           t.CashTransactionDate AS PaymentDate
        //        FROM tblStudentFeePaymentTransaction t
        //        INNER JOIN tblStudentFeePayment p
        //             ON t.PaymentIDs = p.FeesPaymentID
        //    ),
        //    AggregatedPayments AS (
        //        SELECT
        //          StudentID, ClassID, SectionID, InstituteID, FeeGroupID, FeeHeadID, FeeTenurityID,
        //          SUM(AmountPaid) AS TotalPaid,
        //          MIN(PaymentDate) AS PaymentDate
        //        FROM (
        //          SELECT * FROM PaymentFromPayment
        //          UNION ALL
        //          SELECT * FROM PaymentFromTransaction
        //        ) AS X
        //        GROUP BY StudentID, ClassID, SectionID, InstituteID, FeeGroupID, FeeHeadID, FeeTenurityID
        //    )
        //    SELECT  
        //        sm.student_id AS StudentID,
        //        sm.Admission_Number AS AdmissionNo,
        //        CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) AS StudentName,
        //        sm.Roll_Number AS RollNo,
        //        c.class_name AS ClassName,
        //        s.section_name AS SectionName,
        //        cg.ConcessionGroupType AS ConcessionGroup,
        //        CAST(fh.FeeHeadID AS int) AS FeeHeadID,
        //        fh.FeeHead,
        //        fg.FeeGroupID AS FeeGroupID,
        //        fg.FeeTenurityID AS FeeTenurityID,
        //        CASE 
        //            WHEN fg.FeeTenurityID = 1 THEN 'Single'
        //            WHEN fg.FeeTenurityID = 2 THEN tt.TermName
        //            WHEN fg.FeeTenurityID = 3 THEN tm.Month
        //            ELSE 'N/A'
        //        END AS FeeType,
        //        COALESCE(ts.Amount, tt.Amount, tm.Amount) AS FeeAmount,
        //        CASE 
        //             WHEN COALESCE(ap.PaymentDate, GETDATE()) <= lfrs.DueDate THEN 0
        //             WHEN COALESCE(ap.TotalPaid, 0) >= COALESCE(ts.Amount, tt.Amount, tm.Amount) THEN 0
        //             WHEN DATEDIFF(DAY, lfrs.DueDate, COALESCE(ap.PaymentDate, GETDATE()))
        //                  BETWEEN fr.MinDays AND fr.MaxDays THEN fr.LateFee
        //             ELSE 0
        //        END AS LateFee
        //    FROM tbl_StudentMaster sm
        //    INNER JOIN tbl_Class c 
        //        ON sm.class_id = c.class_id
        //    INNER JOIN tbl_Section s 
        //        ON sm.section_id = s.section_id
        //    INNER JOIN tblFeeGroupClassSection fgcs 
        //        ON sm.class_id = fgcs.ClassID AND sm.section_id = fgcs.SectionID
        //    INNER JOIN tblFeeGroup fg 
        //        ON fgcs.FeeGroupID = fg.FeeGroupID
        //    INNER JOIN tblFeeHead fh 
        //        ON fg.FeeHeadID = fh.FeeHeadID
        //    LEFT JOIN tblTenuritySingle ts 
        //        ON fg.FeeTenurityID = 1 AND ts.FeeCollectionID = fgcs.FeeGroupID
        //    LEFT JOIN tblTenurityTerm tt 
        //        ON fg.FeeTenurityID = 2 AND tt.FeeCollectionID = fgcs.FeeGroupID
        //    LEFT JOIN tblTenurityMonthly tm 
        //        ON fg.FeeTenurityID = 3 AND tm.FeeCollectionID = fgcs.FeeGroupID
        //    LEFT JOIN tblStudentConcession sc 
        //        ON sm.student_id = sc.StudentID AND sm.Institute_id = sc.InstituteID AND sc.IsActive = 1
        //    LEFT JOIN tblConcessionGroup cg 
        //        ON sc.ConcessionGroupID = cg.ConcessionGroupID AND cg.IsActive = 1
        //    LEFT JOIN tblLateFeeClassSectionMapping lfm 
        //        ON sm.class_id = lfm.ClassID AND sm.section_id = lfm.SectionID
        //    LEFT JOIN UniqueLFRS lfrs 
        //        ON lfm.LateFeeRuleID = lfrs.LateFeeRuleID 
        //           AND lfrs.FeeHeadID = fh.FeeHeadID 
        //           AND lfrs.InstituteID = sm.Institute_id 
        //           AND lfrs.IsActive = 1
        //    LEFT JOIN AggregatedPayments ap 
        //        ON ap.StudentID = sm.student_id 
        //           AND ap.ClassID = sm.class_id 
        //           AND ap.SectionID = sm.section_id 
        //           AND ap.InstituteID = sm.Institute_id 
        //           AND ap.FeeGroupID = fg.FeeGroupID 
        //           AND ap.FeeHeadID = fh.FeeHeadID 
        //           AND ap.FeeTenurityID = fg.FeeTenurityID
        //    LEFT JOIN tblFeesRules fr 
        //        ON fr.LateFeeRuleID = lfrs.LateFeeRuleID 
        //           AND DATEDIFF(DAY, lfrs.DueDate, COALESCE(ap.PaymentDate, GETDATE()))
        //               BETWEEN fr.MinDays AND fr.MaxDays
        //    WHERE sm.class_id = @ClassID 
        //      AND sm.section_id = @SectionID
        //      AND sm.Institute_id = @InstituteID
        //      AND (@Search IS NULL OR sm.Admission_Number LIKE '%' + @Search + '%' OR CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) LIKE '%' + @Search + '%')
        //    ORDER BY sm.Admission_Number;
        //    ";

        //    var parameters = new
        //    {
        //        InstituteID = request.InstituteID,
        //        ClassID = request.ClassID,
        //        SectionID = request.SectionID,
        //        Search = string.IsNullOrWhiteSpace(request.Search) ? null : request.Search
        //    };

        //    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        var result = await connection.QueryAsync<StudentPreviousFeeRawData>(sql, parameters);
        //        return result;
        //    }
        //}


        public async Task<IEnumerable<GetPreviousFeesChangeLogsExportResponse>> GetPreviousFeesChangeLogsExportAsync(GetPreviousFeesChangeLogsExportRequest request)
        {
            string sql = @"
                SELECT
                    sm.Admission_Number AS AdmissionNumber,
                    CONCAT(sm.First_Name, ' ', sm.Middle_Name, ' ', sm.Last_Name) AS StudentName,
                    sm.Roll_Number AS RollNumber,
                    cg.ConcessionGroupType AS ConcessionGroup,
                    fh.FeeHead,
                    CASE 
                         WHEN fd.FeeTenurityID = 1 THEN 'Single'
                         WHEN fd.FeeTenurityID = 2 THEN tt.TermName
                         WHEN fd.FeeTenurityID = 3 THEN tm.Month
                         ELSE 'N/A'
                    END AS FeeTenurity,
                    fg.Fee AS TotalFeeAmount,
                    fd.DiscountedAmount,
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

            var parameters = new
            {
                request.InstituteID,
                request.ClassID,
                request.SectionID,
                request.AcademicYearCode
            };

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var result = await connection.QueryAsync<GetPreviousFeesChangeLogsExportResponse>(sql, parameters);
                return result;
            }
        }

    }
}
