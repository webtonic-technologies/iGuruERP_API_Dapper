using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FeesManagement_API.Repository.Implementations
{
    public class FeesDashboardRepository : IFeesDashboardRepository
    {
        private readonly string _connectionString;

        public FeesDashboardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<(decimal totalAmountCollected, decimal totalPendingAmount, decimal totalFineCollected)> GetFeeStatisticsAsync(int instituteId)
        {
            // The SQL query uses a parameter for InstituteID.
            var query = @"
            DECLARE @InstituteID INT = @InstituteIDParam;

            WITH StudentFeeCTE AS
            (
                SELECT 
                    s.student_id,
                    ISNULL(SUM(fg.Fee), 0) AS TotalFee,
                    (
                        SELECT ISNULL(SUM(sd.Amount), 0)
                        FROM tblStudentDiscount sd
                        WHERE sd.StudentID = s.student_id
                            AND sd.InstituteID = @InstituteID
                    ) AS TotalDiscount,
                    (
                        SELECT ISNULL(SUM(sw.Amount), 0)
                        FROM tblStudentFeeWaiver sw
                        WHERE sw.StudentID = s.student_id
                            AND sw.InstituteID = @InstituteID
                    ) AS TotalWaiver,
                    (
                        SELECT ISNULL(SUM(sp.Amount), 0)
                        FROM tblStudentFeePayment sp
                        WHERE sp.StudentID = s.student_id
                            AND sp.InstituteID = @InstituteID
                    ) AS TotalPaid
                FROM tbl_StudentMaster s
                INNER JOIN tblFeeGroupClassSection fgcs 
                    ON fgcs.ClassID = s.class_id 
                        AND fgcs.SectionID = s.section_id
                INNER JOIN tblFeeGroup fg
                    ON fg.FeeGroupID = fgcs.FeeGroupID
                WHERE s.Institute_id = @InstituteID
                    AND fg.InstituteID = @InstituteID
                GROUP BY s.student_id
            )
            SELECT
                (SELECT ISNULL(SUM(t.TotalAmount), 0)
                    FROM tblStudentFeePaymentTransaction t
                    WHERE t.InstituteID = @InstituteID) AS TotalAmountCollected,
                (SELECT ISNULL(SUM(t.LateFeesAmount), 0)
                    FROM tblStudentFeePaymentTransaction t
                    WHERE t.InstituteID = @InstituteID) AS TotalFineCollected,
                SUM((A.TotalFee - A.TotalDiscount - A.TotalWaiver) - A.TotalPaid) AS TotalPendingAmount
            FROM StudentFeeCTE A;
            ";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InstituteIDParam", instituteId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            decimal totalAmountCollected = reader["TotalAmountCollected"] != DBNull.Value ? Convert.ToDecimal(reader["TotalAmountCollected"]) : 0;
                            decimal totalFineCollected = reader["TotalFineCollected"] != DBNull.Value ? Convert.ToDecimal(reader["TotalFineCollected"]) : 0;
                            decimal totalPendingAmount = reader["TotalPendingAmount"] != DBNull.Value ? Convert.ToDecimal(reader["TotalPendingAmount"]) : 0;
                            return (totalAmountCollected, totalPendingAmount, totalFineCollected);
                        }
                    }
                }
            }
            return (0, 0, 0);
        }


        public async Task<GetHeadWiseCollectedAmountResponse> GetHeadWiseCollectedAmountAsync(int instituteId)
        {
            // This query uses a CTE to compute per-head collection 
            // and then computes total amount and head-wise percentages.
            var query = @"
        DECLARE @InstituteID INT = @InstituteIDParam;

        WITH FeeHeadData AS
        (
            SELECT 
               fh.FeeHead,
               SUM(sfp.Amount) AS CollectedAmount
            FROM tblStudentFeePayment AS sfp
            INNER JOIN tblFeeHead AS fh 
                ON sfp.FeeHeadID = fh.FeeHeadID
            INNER JOIN tblStudentFeePaymentTransaction AS txn
                ON sfp.TransactionCode = txn.TransactionCode
            WHERE 
               sfp.InstituteID = @InstituteID
               AND fh.InstituteID = @InstituteID
               AND txn.InstituteID = @InstituteID
            GROUP BY fh.FeeHead
        )
        SELECT 
           (SELECT SUM(CollectedAmount) FROM FeeHeadData) AS TotalAmount,
           FeeHead,
           (CollectedAmount * 100.0) / (SELECT SUM(CollectedAmount) FROM FeeHeadData) AS Percentage
        FROM FeeHeadData
        ORDER BY CollectedAmount DESC;
        ";

            var response = new GetHeadWiseCollectedAmountResponse();
            var headWiseList = new List<HeadWiseCollected>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InstituteIDParam", instituteId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        decimal totalAmount = 0;
                        while (await reader.ReadAsync())
                        {
                            // Read the total amount from the first row (it is the same for every row).
                            if (totalAmount == 0)
                            {
                                totalAmount = reader["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(reader["TotalAmount"]) : 0;
                            }
                            headWiseList.Add(new HeadWiseCollected
                            {
                                FeeHead = reader["FeeHead"].ToString(),
                                Percentage = reader["Percentage"] != DBNull.Value ? Convert.ToDecimal(reader["Percentage"]) : 0
                            });
                        }
                        response.TotalAmount = totalAmount;
                        response.HeadWise = headWiseList;
                    }
                }
            }
            return response;
        }

        public async Task<GetDayWiseFeesResponse> GetDayWiseFeesAsync(int instituteId)
        {
            var query = @"
            DECLARE @InstituteID INT = @InstituteIDParam;

            ;WITH Last7Days AS
            (
                SELECT CONVERT(DATE, DATEADD(DAY, i, CAST(GETDATE() AS DATE))) AS [Day]
                FROM 
                (
                    SELECT 0 AS i 
                    UNION ALL SELECT -1 
                    UNION ALL SELECT -2 
                    UNION ALL SELECT -3 
                    UNION ALL SELECT -4 
                    UNION ALL SELECT -5 
                    UNION ALL SELECT -6
                ) AS OffsetValues
            )
            SELECT 
                L.[Day] AS TransactionDate,
                ISNULL(SUM(txn.TotalAmount), 0) AS CollectedAmount
            FROM Last7Days AS L
            LEFT JOIN tblStudentFeePaymentTransaction txn
                ON CAST(txn.TransactionDate AS DATE) = L.[Day]
                AND txn.InstituteID = @InstituteID
            GROUP BY L.[Day]
            ORDER BY L.[Day];
            ";

            var dayWiseList = new List<DayWiseFee>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InstituteIDParam", instituteId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Get the date from the reader and format it as "MMM dd"
                            DateTime transactionDate = reader["TransactionDate"] != DBNull.Value
                                ? Convert.ToDateTime(reader["TransactionDate"])
                                : DateTime.MinValue;
                            string formattedDay = transactionDate.ToString("MMM dd");

                            decimal amountCollected = reader["CollectedAmount"] != DBNull.Value
                                ? Convert.ToDecimal(reader["CollectedAmount"])
                                : 0;

                            dayWiseList.Add(new DayWiseFee
                            {
                                Day = formattedDay,
                                Amount = amountCollected
                            });
                        }
                    }
                }
            }

            var response = new GetDayWiseFeesResponse
            {
                DayWiseFees = dayWiseList
            };

            return response;
        }
         
        public async Task<GetClassSectionWiseResponse> GetClassSectionWiseAsync(int instituteId)
        {
            var query = @"
            DECLARE @InstituteID INT = @InstituteIDParam;

            SELECT 
                c.class_id,
                c.class_name,
                s.section_id,
                s.section_name,
                ISNULL(SUM(CASE WHEN txn.TransactionCode IS NOT NULL 
                                THEN sfp.Amount 
                                ELSE 0 
                           END), 0) AS TotalCollected
            FROM tbl_Class c
            INNER JOIN tbl_Section s
                ON c.class_id = s.class_id
            LEFT JOIN tbl_StudentMaster sm
                ON sm.class_id = c.class_id 
                AND sm.section_id = s.section_id
                AND sm.Institute_id = @InstituteID
            LEFT JOIN tblStudentFeePayment sfp
                ON sfp.StudentID = sm.student_id
                AND sfp.InstituteID = @InstituteID
            LEFT JOIN tblStudentFeePaymentTransaction txn
                ON txn.TransactionCode = sfp.TransactionCode
                AND txn.InstituteID = @InstituteID
            WHERE 
                c.Institute_id = @InstituteID
            GROUP BY 
                c.class_id,
                c.class_name,
                s.section_id,
                s.section_name
            ORDER BY 
                c.class_id,
                s.section_id;
            ";

            var rows = new List<(int ClassId, string ClassName, int SectionId, string SectionName, decimal TotalCollected)>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InstituteIDParam", instituteId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int classId = reader["class_id"] != DBNull.Value ? Convert.ToInt32(reader["class_id"]) : 0;
                            string className = reader["class_name"].ToString();
                            int sectionId = reader["section_id"] != DBNull.Value ? Convert.ToInt32(reader["section_id"]) : 0;
                            string sectionName = reader["section_name"].ToString();
                            decimal totalCollected = reader["TotalCollected"] != DBNull.Value ? Convert.ToDecimal(reader["TotalCollected"]) : 0;

                            rows.Add((classId, className, sectionId, sectionName, totalCollected));
                        }
                    }
                }
            }

            // Group the results by class.
            var classGroups = rows.GroupBy(r => new { r.ClassId, r.ClassName })
                                  .Select(g => new ClassWiseFee
                                  {
                                      ClassName = g.Key.ClassName,
                                      Amount = g.Sum(x => x.TotalCollected),
                                      Section = g.Select(x => new SectionWiseFee
                                      {
                                          SectionName = x.SectionName,
                                          Amount = x.TotalCollected
                                      }).ToList()
                                  }).ToList();

            return new GetClassSectionWiseResponse
            {
                CollectedAmount = classGroups
            };
        }

        public async Task<GetTypeWiseCollectionResponse> GetTypeWiseCollectionAsync(int instituteId)
        {
            var query = @"
            DECLARE @InstituteID INT = @InstituteIDParam;

            WITH FeeTypePayments AS (
                SELECT 
                    fh.RegTypeID,
                    SUM(sfp.Amount) AS CollectedAmount
                FROM tblStudentFeePayment AS sfp
                INNER JOIN tblStudentFeePaymentTransaction AS txn 
                    ON sfp.TransactionCode = txn.TransactionCode
                INNER JOIN tblFeeHead AS fh
                    ON sfp.FeeHeadID = fh.FeeHeadID
                WHERE 
                    sfp.InstituteID = @InstituteID
                    AND txn.InstituteID = @InstituteID
                    AND fh.InstituteID = @InstituteID
                GROUP BY fh.RegTypeID
            )
            SELECT 
                r.RegTypeID,
                r.RegType AS FeeType,
                ISNULL(fp.CollectedAmount, 0) AS CollectedAmount
            FROM tblFeeHeadingRegType AS r
            LEFT JOIN FeeTypePayments fp
                ON r.RegTypeID = fp.RegTypeID
            ORDER BY r.RegTypeID;
            ";
            var collections = new List<TypeWiseCollection>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InstituteIDParam", instituteId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var feeType = reader["FeeType"].ToString();
                            var collectedAmount = reader["CollectedAmount"] != DBNull.Value
                                                    ? Convert.ToDecimal(reader["CollectedAmount"])
                                                    : 0;
                            collections.Add(new TypeWiseCollection
                            {
                                Type = feeType,
                                Amount = collectedAmount
                            });
                        }
                    }
                }
            }

            return new GetTypeWiseCollectionResponse { Collections = collections };
        }

        public async Task<GetModeWiseCollectionResponse> GetModeWiseCollectionAsync(int instituteId, int month, int year)
        {
            var query = @"
DECLARE @InstituteID INT = @InstituteIDParam;
DECLARE @FilterMonth INT = @FilterMonthParam;
DECLARE @FilterYear INT = @FilterYearParam;

WITH TransactionAggregates AS
(
    SELECT 
        PaymentModeID,
        SUM(TotalAmount) AS CollectedAmount
    FROM tblStudentFeePaymentTransaction
    WHERE 
        InstituteID = @InstituteID
        AND MONTH(TransactionDate) = @FilterMonth
        AND YEAR(TransactionDate) = @FilterYear
    GROUP BY PaymentModeID
)
SELECT 
    pm.PaymentModeID,
    pm.PaymentMode,
    ISNULL(txnAgg.CollectedAmount, 0) AS CollectedAmount
FROM tblPaymentMode pm
LEFT JOIN TransactionAggregates txnAgg
    ON pm.PaymentModeID = txnAgg.PaymentModeID
ORDER BY pm.PaymentModeID;
";
            var collections = new List<ModeWiseCollection>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InstituteIDParam", instituteId);
                    command.Parameters.AddWithValue("@FilterMonthParam", month);
                    command.Parameters.AddWithValue("@FilterYearParam", year);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string type = reader["PaymentMode"] != DBNull.Value ? reader["PaymentMode"].ToString() : string.Empty;
                            decimal amount = reader["CollectedAmount"] != DBNull.Value ? Convert.ToDecimal(reader["CollectedAmount"]) : 0;

                            collections.Add(new ModeWiseCollection { Type = type, Amount = amount });
                        }
                    }
                }
            }

            return new GetModeWiseCollectionResponse { Collections = collections };
        }

        public async Task<GetCollectionAnalysisResponse> GetCollectionAnalysisAsync(int instituteId)
        {
            var query = @"
DECLARE @InstituteID INT = @InstituteIDParam;

;WITH Last10Days AS
(
    -- Generate a list of 10 dates (today and the previous 9 days)
    SELECT CONVERT(DATE, DATEADD(DAY, -v.n, GETDATE())) AS [Day]
    FROM (VALUES (0),(1),(2),(3),(4),(5),(6),(7),(8),(9)) AS v(n)
),
DailyCollections AS
(
    -- Aggregate the total fee collection per day from fee payment transactions
    SELECT 
         CONVERT(DATE, txn.TransactionDate) AS TransDate,
         SUM(txn.TotalAmount) AS AmountCollected
    FROM tblStudentFeePaymentTransaction txn
    WHERE txn.InstituteID = @InstituteID
    GROUP BY CONVERT(DATE, txn.TransactionDate)
)
SELECT
    L.[Day] AS CollectionDate,
    ISNULL(DC.AmountCollected, 0) AS AmountCollected
FROM Last10Days L
LEFT JOIN DailyCollections DC
    ON L.[Day] = DC.TransDate
ORDER BY L.[Day];
";
            var dayCollections = new List<DayWiseCollection>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@InstituteIDParam", instituteId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Read the date and format it as "MMM dd" (e.g., "Apr 13")
                            DateTime collectionDate = reader["CollectionDate"] != DBNull.Value
                                ? Convert.ToDateTime(reader["CollectionDate"])
                                : DateTime.MinValue;
                            string formattedDate = collectionDate.ToString("MMM dd");

                            decimal amountCollected = reader["AmountCollected"] != DBNull.Value
                                ? Convert.ToDecimal(reader["AmountCollected"])
                                : 0;

                            dayCollections.Add(new DayWiseCollection
                            {
                                Date = formattedDate,
                                Amount = amountCollected
                            });
                        }
                    }
                }
            }

            return new GetCollectionAnalysisResponse
            {
                AmountCollected = dayCollections
            };
        }
    }
} 