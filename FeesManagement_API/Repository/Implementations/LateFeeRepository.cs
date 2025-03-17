using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using FeesManagement_API.DTOs.ServiceResponse;
using System.Globalization;


namespace FeesManagement_API.Repository.Implementations
{
    public class LateFeeRepository : ILateFeeRepository
    {
        private readonly IDbConnection _connection;

        public LateFeeRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> AddUpdateLateFee(AddUpdateLateFeeRequest request)
        {
            using (var connection = _connection) // Ensure that connection is open and used within a using block
            {
                connection.Open(); // Explicitly open the connection

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {

                        // Convert the DueDate string to DateTime?
                        DateTime? parsedDueDate = null;
                        if (!string.IsNullOrWhiteSpace(request.DueDate))
                        {
                            parsedDueDate = DateTime.ParseExact(
                                request.DueDate,
                                "dd-MM-yyyy",
                                CultureInfo.InvariantCulture);
                        }


                        var lateFeeRuleId = request.LateFeeRuleID;

                        if (lateFeeRuleId == 0)
                        {
                            var query = @"INSERT INTO tblLateFeeRuleSetup (FeeHeadID, FeeTenurityID, DueDate, InstituteID) 
                                  VALUES (@FeeHeadID, @FeeTenurityID, @DueDate, @InstituteID);
                                  SELECT CAST(SCOPE_IDENTITY() as int);";


                            lateFeeRuleId = await connection.ExecuteScalarAsync<int>(
                            query,
                            new
                            {
                                request.FeeHeadID,
                                request.FeeTenurityID,
                                DueDate = parsedDueDate, // use converted date
                                request.InstituteID
                            },
                            transaction);

                            //lateFeeRuleId = await connection.ExecuteScalarAsync<int>(query, request, transaction);
                        }
                        else
                        {
                            var query = @"UPDATE tblLateFeeRuleSetup 
                                  SET FeeHeadID = @FeeHeadID, 
                                      FeeTenurityID = @FeeTenurityID, 
                                      DueDate = @DueDate,
                                      InstituteID = @InstituteID
                                  WHERE LateFeeRuleID = @LateFeeRuleID";


                            await connection.ExecuteAsync(
                            query,
                            new
                            {
                                request.FeeHeadID,
                                request.FeeTenurityID,
                                DueDate = parsedDueDate, // use converted date
                                request.InstituteID,
                                LateFeeRuleID = request.LateFeeRuleID
                            },
                            transaction);

                            //await connection.ExecuteAsync(query, request, transaction);
                        }

                        // Handle Class Sections
                        var deleteClassSectionsQuery = @"DELETE FROM tblLateFeeClassSectionMapping WHERE LateFeeRuleID = @LateFeeRuleID";
                        await connection.ExecuteAsync(deleteClassSectionsQuery, new { LateFeeRuleID = lateFeeRuleId }, transaction);

                        var insertClassSectionQuery = @"INSERT INTO tblLateFeeClassSectionMapping (LateFeeRuleID, ClassID, SectionID)
                                                VALUES (@LateFeeRuleID, @ClassID, @SectionID)";

                        if (request.ClassSections != null)
                        {
                            foreach (var section in request.ClassSections)
                            {
                                await connection.ExecuteAsync(insertClassSectionQuery,
                                    new { LateFeeRuleID = lateFeeRuleId, section.ClassID, section.SectionID },
                                    transaction);
                            }
                        }

                        // Handle Fees Rules
                        var deleteFeesRulesQuery = @"DELETE FROM tblFeesRules WHERE LateFeeRuleID = @LateFeeRuleID";
                        await connection.ExecuteAsync(deleteFeesRulesQuery, new { LateFeeRuleID = lateFeeRuleId }, transaction);

                        if (request.FeesRules != null)
                        {
                            foreach (var rule in request.FeesRules)
                            {
                                var insertFeesRuleQuery = @"INSERT INTO tblFeesRules (LateFeeRuleID, MinDays, MaxDays, LateFee, PerDay, TotalLateFee, ConsolidatedAmount)
                                                    VALUES (@LateFeeRuleID, @MinDays, @MaxDays, @LateFee, @PerDay, @TotalLateFee, @ConsolidatedAmount)";

                                await connection.ExecuteAsync(insertFeesRuleQuery,
                                    new { LateFeeRuleID = lateFeeRuleId, rule.MinDays, rule.MaxDays, rule.LateFee, rule.PerDay, rule.TotalLateFee, rule.ConsolidatedAmount },
                                    transaction);
                            }
                        }

                        transaction.Commit();
                        return lateFeeRuleId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        connection.Close(); // Ensure that the connection is closed after the operation
                    }
                }
            }
        }

        public async Task<ServiceResponse<IEnumerable<LateFeeResponse>>> GetAllLateFee(GetAllLateFeeRequest request)
        {
            // Query to count the total number of records based on filters
            var countQuery = @"
            SELECT COUNT(*)
            FROM tblLateFeeRuleSetup lf
            LEFT JOIN tblFeeHead fh ON lf.FeeHeadID = fh.FeeHeadID
            WHERE lf.InstituteID = @InstituteID AND lf.IsActive = 1
            ";

                    // Add search filter for FeeHead if search term is provided
                    if (!string.IsNullOrEmpty(request.Search))
                    {
                        countQuery += " AND fh.FeeHead LIKE @Search"; // Use LIKE for partial matching on FeeHead
                    }

                    // Execute the total count query
                    var totalCount = await _connection.ExecuteScalarAsync<int>(countQuery, new
                    {
                        request.InstituteID,
                        Search = "%" + request.Search + "%"  // Use wildcard for LIKE search
                    });

                    // Query to fetch the paginated data
                    var query = @"
            SELECT 
                lf.LateFeeRuleID,
                fh.FeeHeadID,
                fh.FeeHead AS FeeHead,
                ft.FeeTenurityID,
                ft.FeeTenurityType AS FeeTenurity,
                FORMAT(lf.DueDate, 'dd MMM yyyy') as DueDate,
                lf.InstituteID,
                fr.FeeRulesID,
                fr.MinDays,
                fr.MaxDays,
                fr.LateFee,
                fr.PerDay,
                fr.TotalLateFee,
                fr.ConsolidatedAmount
            FROM tblLateFeeRuleSetup lf
            LEFT JOIN tblFeeHead fh ON lf.FeeHeadID = fh.FeeHeadID
            LEFT JOIN tblFeeTenurityMaster ft ON lf.FeeTenurityID = ft.FeeTenurityID
            LEFT JOIN tblFeesRules fr ON lf.LateFeeRuleID = fr.LateFeeRuleID
            WHERE lf.InstituteID = @InstituteID AND lf.IsActive = 1
    ";

            // Add search filter for FeeHead if search term is provided
            if (!string.IsNullOrEmpty(request.Search))
            {
                query += " AND fh.FeeHead LIKE @Search"; // Use LIKE for partial matching on FeeHead
            }

            query += @"
            ORDER BY lf.LateFeeRuleID
            OFFSET @PageSize * (@PageNumber - 1) ROWS
            FETCH NEXT @PageSize ROWS ONLY";

            var lookup = new Dictionary<int, LateFeeResponse>();

            var result = await _connection.QueryAsync<LateFeeResponse, FeesRuleResponse, LateFeeResponse>(
                query,
                (lf, fr) =>
                {
                    if (!lookup.TryGetValue(lf.LateFeeRuleID, out var lateFee))
                    {
                        lateFee = lf;

                        // Convert DueDate to string in the required format after the data is fetched
                        lateFee.DueDate = lateFee.DueDate.ToString();  // Format DateTime to string
                        lateFee.FeesRules = new List<FeesRuleResponse>();
                        lookup.Add(lf.LateFeeRuleID, lateFee);
                    }
                    if (fr != null)
                    {
                        lateFee.FeesRules.Add(fr);
                    }
                    return lateFee;
                },
                new
                {
                    request.InstituteID,
                    request.PageNumber,
                    request.PageSize,
                    Search = "%" + request.Search + "%"  // Use wildcard for LIKE search
                },
                splitOn: "FeeRulesID"); 

            // Return the response with both paginated data and total count
            return new ServiceResponse<IEnumerable<LateFeeResponse>>(
                true,
                "Late Fees retrieved successfully",
                lookup.Values,
                200,
                totalCount);  // Include the total count
        }






        //public async Task<LateFeeResponse> GetLateFeeById(int lateFeeRuleID)
        //{
        //    var query = @"SELECT LateFeeRuleID, FeeHeadID, FeeTenurityID, DueDate, InstituteID 
        //                  FROM tblLateFeeRuleSetup
        //                  WHERE LateFeeRuleID = @LateFeeRuleID";

        //    return await _connection.QueryFirstOrDefaultAsync<LateFeeResponse>(query, new { LateFeeRuleID = lateFeeRuleID });
        //}

        public async Task<LateFeeResponse> GetLateFeeById(int lateFeeRuleID)
        {
            var query = @"
        SELECT 
            lf.LateFeeRuleID,
            fh.FeeHeadID,
            fh.FeeHead AS FeeHead,
            ft.FeeTenurityID,
            ft.FeeTenurityType AS FeeTenurity,
            FORMAT(lf.DueDate, 'dd MMM yyyy') as DueDate,
            --lf.DueDate,
            lf.InstituteID,
            fr.FeeRulesID,
            fr.MinDays,
            fr.MaxDays,
            fr.LateFee,
            fr.PerDay,
            fr.TotalLateFee,
            fr.ConsolidatedAmount,
            c.class_name AS ClassName,
            s.section_name AS SectionName
        FROM tblLateFeeRuleSetup lf
        LEFT JOIN tblFeeHead fh ON lf.FeeHeadID = fh.FeeHeadID
        LEFT JOIN tblFeeTenurityMaster ft ON lf.FeeTenurityID = ft.FeeTenurityID
        LEFT JOIN tblFeesRules fr ON lf.LateFeeRuleID = fr.LateFeeRuleID
        LEFT JOIN tblLateFeeClassSectionMapping fcs ON lf.LateFeeRuleID = fcs.LateFeeRuleID
        LEFT JOIN tbl_Class c ON fcs.ClassID = c.class_id
        LEFT JOIN tbl_Section s ON fcs.SectionID = s.section_id
        WHERE lf.LateFeeRuleID = @LateFeeRuleID";

            var lookup = new Dictionary<int, LateFeeResponse>();

            var result = await _connection.QueryAsync<LateFeeResponse, FeesRuleResponse, ClassSectionResponse, LateFeeResponse>(
                query,
                (lf, fr, classSection) =>
                {
                    if (!lookup.TryGetValue(lf.LateFeeRuleID, out var lateFee))
                    {
                        lateFee = lf;
                        lateFee.FeesRules = new List<FeesRuleResponse>();
                        lateFee.ClassSections = new List<ClassSectionResponse>();
                        lookup.Add(lf.LateFeeRuleID, lateFee);
                    }
                    if (fr != null)
                    {
                        lateFee.FeesRules.Add(fr);
                    }
                    if (classSection != null)
                    {
                        lateFee.ClassSections.Add(classSection);
                    }
                    return lateFee;
                },
                new { LateFeeRuleID = lateFeeRuleID },
                splitOn: "FeeRulesID, ClassName"
            );

            return lookup.Values.FirstOrDefault();
        }


        public async Task<int> UpdateLateFeeStatus(int lateFeeRuleID)
        {
            var query = @"UPDATE tblLateFeeRuleSetup 
                          SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END 
                          WHERE LateFeeRuleID = @LateFeeRuleID";

            return await _connection.ExecuteAsync(query, new { LateFeeRuleID = lateFeeRuleID });
        }

        public async Task<IEnumerable<FeeTenureResponse>> GetFeeTenureDDL(GetFeeTenureDDLRequest request)
        {
            var query = @"
        SELECT 
            fg.FeeTenurityID,
            fgc.FeeCollectionID AS TenuritySTMID,
            CASE 
                WHEN ts.TenuritySingleID IS NOT NULL THEN ts.TenuritySingleID 
                WHEN tt.TenurityTermID IS NOT NULL THEN tt.TenurityTermID 
                WHEN tm.TenurityMonthID IS NOT NULL THEN tm.TenurityMonthID 
            END AS FeeCollectionSTMID,
            CASE 
                WHEN ts.TenuritySingleID IS NOT NULL THEN 'Single' 
                WHEN tt.TenurityTermID IS NOT NULL THEN tt.TermName 
                WHEN tm.TenurityMonthID IS NOT NULL THEN tm.Month 
            END AS TenurityType
        FROM tblFeeGroup fg
        JOIN tblFeeGroupCollection fgc ON fg.FeeGroupID = fgc.FeeGroupID
        LEFT JOIN tblTenuritySingle ts ON fgc.FeeCollectionID = ts.FeeCollectionID
        LEFT JOIN tblTenurityTerm tt ON fgc.FeeCollectionID = tt.FeeCollectionID
        LEFT JOIN tblTenurityMonthly tm ON fgc.FeeCollectionID = tm.FeeCollectionID
        WHERE fg.FeeHeadID = @FeeHeadID AND fg.InstituteID = @InstituteID";

            var result = await _connection.QueryAsync<FeeTenureResponse>(query, request);
            return result;
        }


        public async Task<int> DeleteLateFee(int lateFeeRuleID)
        {
            // Soft delete: update IsActive to 0
            var query = @"UPDATE tblLateFeeRuleSetup 
                  SET IsActive = 0 
                  WHERE LateFeeRuleID = @lateFeeRuleID";
            return await _connection.ExecuteAsync(query, new { lateFeeRuleID });
        }

    }
}
