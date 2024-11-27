using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Response;
using FeesManagement_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks; 


namespace FeesManagement_API.Repository.Implementations
{
    public class FeeGroupRepository : IFeeGroupRepository
    {
        private readonly IDbConnection _connection;

        public FeeGroupRepository(IDbConnection connection)
        {
            _connection = connection;
        }
         
 
        public async Task<int> AddUpdateFeeGroups(AddUpdateFeeGroupsRequest request)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open(); // Ensure the connection is open
            }

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    int totalFeeGroupsAdded = 0;

                    foreach (var feeGroupRequest in request.FeeGroups)
                    {
                        var feeGroupId = feeGroupRequest.FeeGroupID;


                        if (feeGroupId == 0)
                        {
                            var query = @"
                            INSERT INTO tblFeeGroup (GroupName, FeeHeadID, FeeTenurityID, Fee, InstituteID, AcademicYearCode) 
                            VALUES (@GroupName, @FeeHeadID, @FeeTenurityID, @Fee, @InstituteID, @AcademicYearCode);
                            SELECT CAST(SCOPE_IDENTITY() as int);";

                                    feeGroupId = await _connection.ExecuteScalarAsync<int>(query, feeGroupRequest, transaction);
                                }
                                else
                                {
                                    var query = @"
                            UPDATE tblFeeGroup 
                            SET GroupName = @GroupName, 
                                FeeHeadID = @FeeHeadID, 
                                FeeTenurityID = @FeeTenurityID, 
                                Fee = @Fee, 
                                InstituteID = @InstituteID,
                                AcademicYearCode = @AcademicYearCode
                            WHERE FeeGroupID = @FeeGroupID";

                            await _connection.ExecuteAsync(query, feeGroupRequest, transaction);
                        }

                        // Handle tblFeeGroupClassSection
                        var deleteClassSectionsQuery = @"DELETE FROM tblFeeGroupClassSection WHERE FeeGroupID = @FeeGroupID";
                        await _connection.ExecuteAsync(deleteClassSectionsQuery, new { FeeGroupID = feeGroupId }, transaction);

                        var insertClassSectionQuery = @"INSERT INTO tblFeeGroupClassSection (FeeGroupID, ClassID, SectionID)
                                                 VALUES (@FeeGroupID, @ClassID, @SectionID)";

                        if (feeGroupRequest.ClassSections != null)
                        {
                            foreach (var section in feeGroupRequest.ClassSections)
                            {
                                await _connection.ExecuteAsync(insertClassSectionQuery,
                                    new { FeeGroupID = feeGroupId, section.ClassID, section.SectionID },
                                    transaction);
                            }
                        }

                        // Handle tblFeeGroupCollection and related tables based on FeeTenurityID
                        if (feeGroupRequest.FeeCollections != null)
                        {
                            foreach (var collection in feeGroupRequest.FeeCollections)
                            {
                                int feeCollectionId;

                                // Check if the FeeCollectionID already exists for update
                                if (collection.FeeCollectionID > 0)
                                {
                                    // Update existing FeeGroupCollection
                                    var updateFeeCollectionQuery = @"UPDATE tblFeeGroupCollection
                                                             SET FeeTenurityID = @FeeTenurityID
                                                             WHERE FeeCollectionID = @FeeCollectionID";

                                    await _connection.ExecuteAsync(updateFeeCollectionQuery,
                                        new { FeeCollectionID = collection.FeeCollectionID, collection.FeeTenurityID },
                                        transaction);

                                    feeCollectionId = collection.FeeCollectionID;
                                }
                                else
                                {
                                    // Insert new FeeGroupCollection
                                    var insertFeeCollectionQuery = @"INSERT INTO tblFeeGroupCollection (FeeGroupID, FeeTenurityID)
                                                              VALUES (@FeeGroupID, @FeeTenurityID);
                                                              SELECT CAST(SCOPE_IDENTITY() as int);";

                                    feeCollectionId = await _connection.ExecuteScalarAsync<int>(insertFeeCollectionQuery,
                                        new { FeeGroupID = feeGroupId, collection.FeeTenurityID },
                                        transaction);
                                }

                                // Delete existing records in tblTenuritySingle, tblTenurityTerm, and tblTenurityMonthly
                                var deleteSingleQuery = @"DELETE FROM tblTenuritySingle WHERE FeeCollectionID = @FeeCollectionID";
                                await _connection.ExecuteAsync(deleteSingleQuery, new { FeeCollectionID = feeCollectionId }, transaction);

                                var deleteTermQuery = @"DELETE FROM tblTenurityTerm WHERE FeeCollectionID = @FeeCollectionID";
                                await _connection.ExecuteAsync(deleteTermQuery, new { FeeCollectionID = feeCollectionId }, transaction);

                                var deleteMonthQuery = @"DELETE FROM tblTenurityMonthly WHERE FeeCollectionID = @FeeCollectionID";
                                await _connection.ExecuteAsync(deleteMonthQuery, new { FeeCollectionID = feeCollectionId }, transaction);

                                // Insert records based on the FeeTenurityID
                                if (collection.FeeTenurityID == 1 && collection.TenuritySingle != null)
                                {
                                    var insertSingleQuery = @"INSERT INTO tblTenuritySingle (FeeCollectionID, Amount)
                                                      VALUES (@FeeCollectionID, @Amount)";
                                    await _connection.ExecuteAsync(insertSingleQuery,
                                        new { FeeCollectionID = feeCollectionId, collection.TenuritySingle.Amount },
                                        transaction);
                                }
                                else if (collection.FeeTenurityID == 2 && collection.TenurityTerm != null)
                                {
                                    var insertTermQuery = @"INSERT INTO tblTenurityTerm (FeeCollectionID, TermName, Amount, DueDate)
                                                    VALUES (@FeeCollectionID, @TermName, @Amount, @DueDate)";
                                    foreach (var term in collection.TenurityTerm.Terms)
                                    {
                                        await _connection.ExecuteAsync(insertTermQuery,
                                            new { FeeCollectionID = feeCollectionId, term.TermName, term.Amount, term.DueDate },
                                            transaction);
                                    }
                                }
                                else if (collection.FeeTenurityID == 3 && collection.TenurityMonthly != null)
                                {
                                    var insertMonthQuery = @"INSERT INTO tblTenurityMonthly (FeeCollectionID, Month, Amount)
                                                      VALUES (@FeeCollectionID, @Month, @Amount)";
                                    foreach (var month in collection.TenurityMonthly.Months)
                                    {
                                        await _connection.ExecuteAsync(insertMonthQuery,
                                            new { FeeCollectionID = feeCollectionId, month.Month, month.Amount },
                                            transaction);
                                    }
                                }
                            }
                        }

                        totalFeeGroupsAdded++;
                    }

                    transaction.Commit();
                    return totalFeeGroupsAdded; // Return the number of fee groups added or updated
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    _connection.Close(); // Ensure the connection is closed
                }
            }
        }



        //public async Task<int> AddUpdateFeeGroup(AddUpdateFeeGroupRequest request)
        //{
        //    if (_connection.State == ConnectionState.Closed)
        //    {
        //        _connection.Open(); // Ensure the connection is open
        //    }

        //    using (var transaction = _connection.BeginTransaction())
        //    {
        //        try
        //        {
        //            var feeGroupId = request.FeeGroupID;

        //            if (feeGroupId == 0)
        //            {
        //                var query = @"INSERT INTO tblFeeGroup (GroupName, FeeHeadID, FeeTenurityID, Fee, InstituteID) 
        //                      VALUES (@GroupName, @FeeHeadID, @FeeTenurityID, @Fee, @InstituteID);
        //                      SELECT CAST(SCOPE_IDENTITY() as int);";

        //                feeGroupId = await _connection.ExecuteScalarAsync<int>(query, request, transaction);
        //            }
        //            else
        //            {
        //                var query = @"UPDATE tblFeeGroup 
        //                      SET GroupName = @GroupName, 
        //                          FeeHeadID = @FeeHeadID, 
        //                          FeeTenurityID = @FeeTenurityID, 
        //                          Fee = @Fee, 
        //                          InstituteID = @InstituteID
        //                      WHERE FeeGroupID = @FeeGroupID";

        //                await _connection.ExecuteAsync(query, request, transaction);
        //            }

        //            // Handle tblFeeGroupClassSection
        //            var deleteClassSectionsQuery = @"DELETE FROM tblFeeGroupClassSection WHERE FeeGroupID = @FeeGroupID";
        //            await _connection.ExecuteAsync(deleteClassSectionsQuery, new { FeeGroupID = feeGroupId }, transaction);

        //            var insertClassSectionQuery = @"INSERT INTO tblFeeGroupClassSection (FeeGroupID, ClassID, SectionID)
        //                                    VALUES (@FeeGroupID, @ClassID, @SectionID)";

        //            if (request.ClassSections != null)
        //            {
        //                foreach (var section in request.ClassSections)
        //                {
        //                    await _connection.ExecuteAsync(insertClassSectionQuery,
        //                        new { FeeGroupID = feeGroupId, section.ClassID, section.SectionID },
        //                        transaction);
        //                }
        //            }

        //            // Handle tblFeeGroupCollection and related tables based on FeeTenurityID
        //            if (request.FeeCollections != null)
        //            {
        //                foreach (var collection in request.FeeCollections)
        //                {
        //                    int feeCollectionId;

        //                    // Check if the FeeCollectionID already exists for update
        //                    if (collection.FeeCollectionID > 0)
        //                    {
        //                        // Update existing FeeGroupCollection
        //                        var updateFeeCollectionQuery = @"UPDATE tblFeeGroupCollection
        //                                                 SET FeeTenurityID = @FeeTenurityID
        //                                                 WHERE FeeCollectionID = @FeeCollectionID";

        //                        await _connection.ExecuteAsync(updateFeeCollectionQuery,
        //                            new { FeeCollectionID = collection.FeeCollectionID, collection.FeeTenurityID },
        //                            transaction);

        //                        feeCollectionId = collection.FeeCollectionID;
        //                    }
        //                    else
        //                    {
        //                        // Insert new FeeGroupCollection
        //                        var insertFeeCollectionQuery = @"INSERT INTO tblFeeGroupCollection (FeeGroupID, FeeTenurityID)
        //                                                 VALUES (@FeeGroupID, @FeeTenurityID);
        //                                                 SELECT CAST(SCOPE_IDENTITY() as int);";

        //                        feeCollectionId = await _connection.ExecuteScalarAsync<int>(insertFeeCollectionQuery,
        //                            new { FeeGroupID = feeGroupId, collection.FeeTenurityID },
        //                            transaction);
        //                    }

        //                    // Delete existing records in tblTenuritySingle, tblTenurityTerm, and tblTenurityMonthly
        //                    var deleteSingleQuery = @"DELETE FROM tblTenuritySingle WHERE FeeCollectionID = @FeeCollectionID";
        //                    await _connection.ExecuteAsync(deleteSingleQuery, new { FeeCollectionID = feeCollectionId }, transaction);

        //                    var deleteTermQuery = @"DELETE FROM tblTenurityTerm WHERE FeeCollectionID = @FeeCollectionID";
        //                    await _connection.ExecuteAsync(deleteTermQuery, new { FeeCollectionID = feeCollectionId }, transaction);

        //                    var deleteMonthQuery = @"DELETE FROM tblTenurityMonthly WHERE FeeCollectionID = @FeeCollectionID";
        //                    await _connection.ExecuteAsync(deleteMonthQuery, new { FeeCollectionID = feeCollectionId }, transaction);

        //                    // Insert records based on the FeeTenurityID
        //                    if (collection.FeeTenurityID == 1 && collection.TenuritySingle != null)
        //                    {
        //                        var insertSingleQuery = @"INSERT INTO tblTenuritySingle (FeeCollectionID, Amount)
        //                                          VALUES (@FeeCollectionID, @Amount)";
        //                        await _connection.ExecuteAsync(insertSingleQuery,
        //                            new { FeeCollectionID = feeCollectionId, collection.TenuritySingle.Amount },
        //                            transaction);
        //                    }
        //                    else if (collection.FeeTenurityID == 2 && collection.TenurityTerm != null)
        //                    {
        //                        var insertTermQuery = @"INSERT INTO tblTenurityTerm (FeeCollectionID, TermName, Amount, DueDate)
        //                                        VALUES (@FeeCollectionID, @TermName, @Amount, @DueDate)";
        //                        foreach (var term in collection.TenurityTerm.Terms)
        //                        {
        //                            await _connection.ExecuteAsync(insertTermQuery,
        //                                new { FeeCollectionID = feeCollectionId, term.TermName, term.Amount, term.DueDate },
        //                                transaction);
        //                        }
        //                    }
        //                    else if (collection.FeeTenurityID == 3 && collection.TenurityMonthly != null)
        //                    {
        //                        var insertMonthQuery = @"INSERT INTO tblTenurityMonthly (FeeCollectionID, Month, Amount)
        //                                         VALUES (@FeeCollectionID, @Month, @Amount)";
        //                        foreach (var month in collection.TenurityMonthly.Months)
        //                        {
        //                            await _connection.ExecuteAsync(insertMonthQuery,
        //                                new { FeeCollectionID = feeCollectionId, month.Month, month.Amount },
        //                                transaction);
        //                        }
        //                    }
        //                }
        //            }

        //            transaction.Commit();
        //            return feeGroupId;
        //        }
        //        catch
        //        {
        //            transaction.Rollback();
        //            throw;
        //        }
        //        finally
        //        {
        //            _connection.Close(); // Ensure the connection is closed
        //        }
        //    }
        //}

        //public async Task<int> AddUpdateFeeGroup(AddUpdateFeeGroupRequest request)
        //{
        //    if (_connection.State == ConnectionState.Closed)
        //    {
        //        _connection.Open(); // Ensure the connection is open
        //    }

        //    using (var transaction = _connection.BeginTransaction())
        //    {
        //        try
        //        {
        //            var feeGroupId = request.FeeGroupID;

        //            if (feeGroupId == 0)
        //            {
        //                var query = @"INSERT INTO tblFeeGroup (GroupName, FeeHeadID, FeeTenurityID, Fee, InstituteID) 
        //                      VALUES (@GroupName, @FeeHeadID, @FeeTenurityID, @Fee, @InstituteID);
        //                      SELECT CAST(SCOPE_IDENTITY() as int);";

        //                feeGroupId = await _connection.ExecuteScalarAsync<int>(query, request, transaction);
        //            }
        //            else
        //            {
        //                var query = @"UPDATE tblFeeGroup 
        //                      SET GroupName = @GroupName, 
        //                          FeeHeadID = @FeeHeadID, 
        //                          FeeTenurityID = @FeeTenurityID, 
        //                          Fee = @Fee, 
        //                          InstituteID = @InstituteID
        //                      WHERE FeeGroupID = @FeeGroupID";

        //                await _connection.ExecuteAsync(query, request, transaction);
        //            }

        //            // Handle tblFeeGroupClassSection
        //            var deleteClassSectionsQuery = @"DELETE FROM tblFeeGroupClassSection WHERE FeeGroupID = @FeeGroupID";
        //            await _connection.ExecuteAsync(deleteClassSectionsQuery, new { FeeGroupID = feeGroupId }, transaction);

        //            var insertClassSectionQuery = @"INSERT INTO tblFeeGroupClassSection (FeeGroupID, ClassID, SectionID)
        //                                    VALUES (@FeeGroupID, @ClassID, @SectionID)";

        //            if (request.ClassSections != null)
        //            {
        //                foreach (var section in request.ClassSections)
        //                {
        //                    await _connection.ExecuteAsync(insertClassSectionQuery,
        //                        new { FeeGroupID = feeGroupId, section.ClassID, section.SectionID },
        //                        transaction);
        //                }
        //            }

        //            // Handle tblFeeGroupCollection and related tables based on FeeTenurityID
        //            var deleteFeeCollectionsQuery = @"DELETE FROM tblFeeGroupCollection WHERE FeeGroupID = @FeeGroupID";
        //            await _connection.ExecuteAsync(deleteFeeCollectionsQuery, new { FeeGroupID = feeGroupId }, transaction);

        //            if (request.FeeCollections != null)
        //            {
        //                foreach (var collection in request.FeeCollections)
        //                {
        //                    var insertFeeCollectionQuery = @"INSERT INTO tblFeeGroupCollection (FeeGroupID, FeeTenurityID)
        //                                             VALUES (@FeeGroupID, @FeeTenurityID);
        //                                             SELECT CAST(SCOPE_IDENTITY() as int);";

        //                    var feeCollectionId = await _connection.ExecuteScalarAsync<int>(insertFeeCollectionQuery,
        //                        new { FeeGroupID = feeGroupId, collection.FeeTenurityID },
        //                        transaction);

        //                    if (collection.FeeTenurityID == 1 && collection.TenuritySingle != null)
        //                    {
        //                        var insertSingleQuery = @"INSERT INTO tblTenuritySingle (FeeCollectionID, Amount)
        //                                          VALUES (@FeeCollectionID, @Amount)";
        //                        await _connection.ExecuteAsync(insertSingleQuery,
        //                            new { FeeCollectionID = feeCollectionId, collection.TenuritySingle.Amount },
        //                            transaction);
        //                    }
        //                    else if (collection.FeeTenurityID == 2 && collection.TenurityTerm != null)
        //                    {
        //                        var insertTermQuery = @"INSERT INTO tblTenurityTerm (FeeCollectionID, TermName, Amount, DueDate)
        //                                        VALUES (@FeeCollectionID, @TermName, @Amount, @DueDate)";
        //                        foreach (var term in collection.TenurityTerm.Terms)
        //                        {
        //                            await _connection.ExecuteAsync(insertTermQuery,
        //                                new { FeeCollectionID = feeCollectionId, term.TermName, term.Amount, term.DueDate },
        //                                transaction);
        //                        }
        //                    }
        //                    else if (collection.FeeTenurityID == 3 && collection.TenurityMonthly != null)
        //                    {
        //                        var insertMonthQuery = @"INSERT INTO tblTenurityMonthly (FeeCollectionID, Month, Amount)
        //                                         VALUES (@FeeCollectionID, @Month, @Amount)";
        //                        foreach (var month in collection.TenurityMonthly.Months)
        //                        {
        //                            await _connection.ExecuteAsync(insertMonthQuery,
        //                                new { FeeCollectionID = feeCollectionId, month.Month, month.Amount },
        //                                transaction);
        //                        }
        //                    }
        //                }
        //            }

        //            transaction.Commit();
        //            return feeGroupId;
        //        }
        //        catch
        //        {
        //            transaction.Rollback();
        //            throw;
        //        }
        //        finally
        //        {
        //            _connection.Close(); // Ensure the connection is closed
        //        }
        //    }
        //}


        //    public async Task<IEnumerable<FeeGroupResponse>> GetAllFeeGroups(GetAllFeeGroupRequest request)
        //{
        //    var query = @"
        //        SELECT fg.FeeGroupID, fg.GroupName, fg.FeeHeadID, fh.FeeHead, fg.FeeTenurityID, ft.FeeTenurityType
        //        FROM tblFeeGroup fg
        //        INNER JOIN tblFeeHead fh ON fg.FeeHeadID = fh.FeeHeadID
        //        INNER JOIN tblFeeTenurityMaster ft ON fg.FeeTenurityID = ft.FeeTenurityID
        //        LEFT JOIN tblFeeGroupClassSection fgc ON fg.FeeGroupID = fgc.FeeGroupID
        //        WHERE fg.FeeHeadID = @FeeHeadID AND fg.InstituteID = @InstituteID
        //        GROUP BY fg.FeeGroupID, fg.GroupName, fg.FeeHeadID, fh.FeeHead, fg.FeeTenurityID, ft.FeeTenurityType
        //        ORDER BY fg.FeeGroupID
        //        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        //    var result = await _connection.QueryAsync<FeeGroupResponse>(query, new
        //    {
        //        FeeHeadID = request.FeeHeadID,
        //        InstituteID = request.InstituteID,
        //        Offset = (request.PageNumber - 1) * request.PageSize,
        //        PageSize = request.PageSize
        //    });

        //    return result.ToList();
        //}

        public async Task<IEnumerable<FeeGroupResponse>> GetAllFeeGroups(GetAllFeeGroupRequest request)
        {
            var query = @"
            SELECT 
                fg.FeeGroupID, 
                fg.GroupName, 
                fg.FeeHeadID, 
                fh.FeeHead, 
                fg.FeeTenurityID, 
                ft.FeeTenurityType,
                fg.IsActive,
                c.class_name AS ClassName,
                s.section_name AS SectionName
            FROM tblFeeGroup fg
            INNER JOIN tblFeeHead fh ON fg.FeeHeadID = fh.FeeHeadID
            INNER JOIN tblFeeTenurityMaster ft ON fg.FeeTenurityID = ft.FeeTenurityID
            LEFT JOIN tblFeeGroupClassSection fgc ON fg.FeeGroupID = fgc.FeeGroupID
            LEFT JOIN tbl_Class c ON fgc.ClassID = c.class_id
            LEFT JOIN tbl_Section s ON fgc.SectionID = s.section_id
            WHERE fg.FeeHeadID = @FeeHeadID AND fg.InstituteID = @InstituteID And fg.AcademicYearCode = @AcademicYearCode And fg.IsActive = 1
            ORDER BY fg.FeeGroupID
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            // Declare and initialize feeGroups before using it
            List<FeeGroupResponse> feeGroups = new List<FeeGroupResponse>();

            var result = await _connection.QueryAsync<FeeGroupResponse, ClassSectionResponse, FeeGroupResponse>(
                query,
                (feeGroup, classSection) =>
                {
                    // Check if the feeGroup already exists in feeGroups
                    var existingFeeGroup = feeGroups.FirstOrDefault(fg => fg.FeeGroupID == feeGroup.FeeGroupID);
                    if (existingFeeGroup != null)
                    {
                        // If it exists, add the classSection to the existing feeGroup
                        existingFeeGroup.ClassSections.Add(classSection);
                        return existingFeeGroup;
                    }
                    else
                    {
                        // If it doesn't exist, add the new classSection to the feeGroup and add it to feeGroups
                        feeGroup.ClassSections = new List<ClassSectionResponse> { classSection };
                        feeGroups.Add(feeGroup);
                        return feeGroup;
                    }
                },
                new
                {
                    FeeHeadID = request.FeeHeadID,
                    InstituteID = request.InstituteID,
                    AcademicYearCode = request.AcademicYearCode,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                },
                splitOn: "ClassName"
            );

            return feeGroups.Distinct().ToList();
        }

        public async Task<FeeGroupResponse> GetFeeGroupById(int feeGroupID)
        {
            var query = @"
                SELECT 
                    fg.FeeGroupID, 
                    fg.GroupName, 
                    fg.FeeHeadID, 
                    fh.FeeHead, 
                    fg.FeeTenurityID, 
                    fg.IsActive,
                    ft.FeeTenurityType,
                    c.class_name AS ClassName,
                    s.section_name AS SectionName
                FROM tblFeeGroup fg
                INNER JOIN tblFeeHead fh ON fg.FeeHeadID = fh.FeeHeadID
                INNER JOIN tblFeeTenurityMaster ft ON fg.FeeTenurityID = ft.FeeTenurityID
                LEFT JOIN tblFeeGroupClassSection fgc ON fg.FeeGroupID = fgc.FeeGroupID
                LEFT JOIN tbl_Class c ON fgc.ClassID = c.class_id
                LEFT JOIN tbl_Section s ON fgc.SectionID = s.section_id
                WHERE fg.FeeGroupID = @FeeGroupID";

            var feeGroupDictionary = new Dictionary<int, FeeGroupResponse>();

            var result = await _connection.QueryAsync<FeeGroupResponse, ClassSectionResponse, FeeGroupResponse>(
                query,
                (feeGroup, classSection) =>
                {
                    if (!feeGroupDictionary.TryGetValue(feeGroup.FeeGroupID, out var existingFeeGroup))
                    {
                        existingFeeGroup = feeGroup;
                        existingFeeGroup.ClassSections = new List<ClassSectionResponse>();
                        feeGroupDictionary.Add(existingFeeGroup.FeeGroupID, existingFeeGroup);
                    }

                    if (classSection != null)
                    {
                        existingFeeGroup.ClassSections.Add(classSection);
                    }

                    return existingFeeGroup;
                },
                new { FeeGroupID = feeGroupID },
                splitOn: "ClassName"
            );

            return feeGroupDictionary.Values.FirstOrDefault();
        }


        //public async Task<FeeGroupResponse> GetFeeGroupById(int feeGroupID)
        //{
        //    var query = @"SELECT FeeGroupID, GroupName, FeeHeadID, FeeTenurityID, Fee, InstituteID 
        //                  FROM tblFeeGroup
        //                  WHERE FeeGroupID = @FeeGroupID";

        //    return await _connection.QueryFirstOrDefaultAsync<FeeGroupResponse>(query, new { FeeGroupID = feeGroupID });
        //}

        //public async Task<int> UpdateFeeGroupStatus(int feeGroupID)
        //{
        //    string query = "UPDATE tblFeeGroup SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE FeeGroupID = @FeeGroupID";
        //    return await _connection.ExecuteAsync(query, new { FeeGroupID = feeGroupID });
        //}

        public async Task<int> UpdateFeeGroupStatus(int feeGroupID, string reason)
        {
            // Toggle the status first
            string updateStatusQuery = @"
    UPDATE tblFeeGroup 
    SET IsActive = CASE 
                       WHEN IsActive = 1 THEN 0 
                       ELSE 1 
                   END
    WHERE FeeGroupID = @FeeGroupID";

            // Execute the first query to toggle the IsActive status
            int rowsAffected = await _connection.ExecuteAsync(updateStatusQuery, new { FeeGroupID = feeGroupID });

            // If the status was set to active (1), update the Reason
            if (rowsAffected > 0 && reason != null)
            {
                string updateReasonQuery = @"
        UPDATE tblFeeGroup 
        SET Reason = @Reason 
        WHERE FeeGroupID = @FeeGroupID";

                // Execute the second query to update the Reason
                await _connection.ExecuteAsync(updateReasonQuery, new { FeeGroupID = feeGroupID, Reason = reason });
            }

            return rowsAffected; // Return the number of rows affected by the status update
        }



    }
}
