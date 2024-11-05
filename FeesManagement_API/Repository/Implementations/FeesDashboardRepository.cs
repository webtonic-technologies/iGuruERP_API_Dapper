using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using System.Data;

namespace FeesManagement_API.Repository.Implementations
{
    public class FeesDashboardRepository : IFeesDashboardRepository
    {
        private readonly IDbConnection _dbConnection;

        public FeesDashboardRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<decimal> GetTotalAmountCollectedAsync(int instituteId)
        {
            string sql = @"
            SELECT 
                SUM(PaymentAmount) AS Total_Amount_Collected
            FROM 
                tblStudentFeePaymentTransaction 
            WHERE 
                InstituteID = @InstituteID";

            var parameters = new { InstituteID = instituteId };
            var result = await _dbConnection.ExecuteScalarAsync<decimal?>(sql, parameters);
            return result ?? 0; // Return 0 if null
        }

        public async Task<decimal> GetTotalPendingAmountAsync(int instituteId)
        {
            var query = @"
            WITH StudentFees AS (
                SELECT 
                    fg.Fee AS Total_Fee, 
                    COALESCE(sp_total.Paid, 0) AS Total_Paid, 
                    COALESCE(d.Total_Discount, 0) AS Total_Discount,  
                    COALESCE(w.Total_Waiver, 0) AS Total_Waiver
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
                    (SELECT StudentID, SUM(Amount) AS Paid FROM tblStudentFeePayment GROUP BY StudentID) sp_total ON sp_total.StudentID = sm.student_id
                LEFT JOIN 
                    (SELECT StudentID, SUM(Amount) AS Total_Discount FROM tblStudentDiscount GROUP BY StudentID) d ON d.StudentID = sm.student_id 
                LEFT JOIN 
                    (SELECT StudentID, SUM(Amount) AS Total_Waiver FROM tblStudentFeeWaiver GROUP BY StudentID) w ON w.StudentID = sm.student_id 
                WHERE 
                    c.institute_id = @InstituteID
            )

            SELECT 
                SUM(Total_Fee - Total_Paid - Total_Discount - Total_Waiver) AS Total_Pending_Amount
            FROM 
                StudentFees;";

            return await _dbConnection.QuerySingleOrDefaultAsync<decimal>(query, new { InstituteID = instituteId });
        }

        public async Task<List<HeadWiseCollectedAmountResponse>> GetHeadWiseCollectedAmountAsync(HeadWiseCollectedAmountRequest request)
        {
            var query = @"
            WITH FeeCollection AS (
                SELECT 
                    fh.FeeHead,
                    SUM(COALESCE(ts.Amount, tt.Amount, tm.Amount)) AS TotalFeeAmount
                FROM 
                    tbl_StudentMaster sm
                INNER JOIN 
                    tbl_Class c ON sm.class_id = c.class_id
                INNER JOIN 
                    tbl_Section s ON sm.section_id = s.section_id
                INNER JOIN 
                    tblFeeGroupClassSection fgcs ON sm.class_id = fgcs.ClassID AND sm.section_id = fgcs.SectionID
                INNER JOIN 
                    tblFeeGroup fg ON fgcs.FeeGroupID = fg.FeeGroupID
                INNER JOIN 
                    tblFeeHead fh ON fg.FeeHeadID = fh.FeeHeadID
                LEFT JOIN 
                    tblTenuritySingle ts ON fg.FeeTenurityID = 1 AND ts.FeeCollectionID = fgcs.FeeGroupID
                LEFT JOIN 
                    tblTenurityTerm tt ON fg.FeeTenurityID = 2 AND tt.FeeCollectionID = fgcs.FeeGroupID
                LEFT JOIN 
                    tblTenurityMonthly tm ON fg.FeeTenurityID = 3 AND tm.FeeCollectionID = fgcs.FeeGroupID
                WHERE 
                    sm.class_id = @ClassID 
                    AND sm.section_id = @SectionID
                    AND c.institute_id = @InstituteID
                GROUP BY 
                    fh.FeeHead
            ),
            PaidFees AS (
                SELECT 
                    fh.FeeHead,
                    SUM(spt.PaymentAmount) AS TotalPaid
                FROM 
                    tblFeeHead fh
                LEFT JOIN 
                    tblStudentFeePayment sp ON fh.FeeHeadID = sp.FeeHeadID
                LEFT JOIN 
                    tblStudentFeePaymentTransaction spt ON sp.FeesPaymentID = spt.PaymentIDs
                WHERE 
                    sp.InstituteID = @InstituteID
                GROUP BY 
                    fh.FeeHead
            ),
            TotalSummary AS (
                SELECT 
                    SUM(COALESCE(fc.TotalFeeAmount, 0)) AS GrandTotalFeeAmount,
                    SUM(COALESCE(pf.TotalPaid, 0)) AS GrandTotalPaid
                FROM 
                    FeeCollection fc
                LEFT JOIN 
                    PaidFees pf ON fc.FeeHead = pf.FeeHead
            )

            SELECT 
                fc.FeeHead,
                COALESCE(fc.TotalFeeAmount, 0) AS TotalFeeAmount,
                COALESCE(pf.TotalPaid, 0) AS TotalPaid,
                (COALESCE(fc.TotalFeeAmount, 0) - COALESCE(pf.TotalPaid, 0)) AS Balance,
                CASE 
                    WHEN ts.GrandTotalFeeAmount > 0 THEN 
                        (COALESCE(pf.TotalPaid, 0) * 100.0 / ts.GrandTotalFeeAmount)
                    ELSE 0
                END AS PercentageCollected
            FROM 
                FeeCollection fc
            LEFT JOIN 
                PaidFees pf ON fc.FeeHead = pf.FeeHead
            CROSS JOIN 
                TotalSummary ts  
            ORDER BY 
                fc.FeeHead;";

            return (await _dbConnection.QueryAsync<HeadWiseCollectedAmountResponse>(query, request)).AsList();
        }

        public async Task<List<DayWiseResponse>> GetDayWiseCollectedAmountAsync(DayWiseRequest request)
        {
            var query = @"
                WITH Last7Days AS (
                    SELECT CAST(GETDATE() - n AS DATE) AS Transaction_Date
                    FROM (VALUES (0), (1), (2), (3), (4), (5), (6)) AS Numbers(n)
                )

                SELECT 
                    l.Transaction_Date,
                    COALESCE(SUM(spt.PaymentAmount), 0) AS TotalCollectedAmount
                FROM 
                    Last7Days l
                LEFT JOIN 
                    tblStudentFeePaymentTransaction spt ON CAST(spt.CashTransactionDate AS DATE) = l.Transaction_Date
                    AND spt.InstituteID = @InstituteID
                GROUP BY 
                    l.Transaction_Date
                ORDER BY 
                    l.Transaction_Date;";

            var parameters = new { InstituteID = request.InstituteID };
            var result = await _dbConnection.QueryAsync(query, parameters);

            return result.Select(r => new DayWiseResponse
            {
                TransactionDate = ((DateTime)r.Transaction_Date).ToString("MMM dd"), // Format the date here
                TotalCollectedAmount = r.TotalCollectedAmount
            }).ToList();
        }

        public async Task<List<FeeCollectionAnalysisResponse>> GetFeeCollectionAnalysisAsync(FeeCollectionAnalysisRequest request)
        {
            var query = @"
            WITH Last12Months AS (
                SELECT DATEADD(MONTH, -n, EOMONTH(GETDATE())) AS Month_Start
                FROM (VALUES (0), (1), (2), (3), (4), (5), (6), (7), (8), (9), (10), (11)) AS Numbers(n)
            )
            SELECT 
                FORMAT(l.Month_Start, 'MMMM yyyy') AS Month,  -- Format the month for better readability
                COALESCE(SUM(spt.PaymentAmount), 0) AS TotalCollectedAmount  -- Ensure to show 0 if no payments
            FROM 
                Last12Months l
            LEFT JOIN 
                tblStudentFeePaymentTransaction spt ON 
                    MONTH(spt.CashTransactionDate) = MONTH(l.Month_Start) AND 
                    YEAR(spt.CashTransactionDate) = YEAR(l.Month_Start) AND 
                    spt.InstituteID = @InstituteID
            GROUP BY 
                l.Month_Start
            ORDER BY 
                l.Month_Start DESC;";  // Order by month, newest first

            var parameters = new { InstituteID = request.InstituteID };
            return (await _dbConnection.QueryAsync<FeeCollectionAnalysisResponse>(query, parameters)).ToList();
        }

    }
}
