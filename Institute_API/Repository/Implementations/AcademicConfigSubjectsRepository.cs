using Dapper;
using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Repository.Interfaces;
using OfficeOpenXml;
using System.Data;
using System.Linq;

namespace Institute_API.Repository.Implementations
{
    public class AcademicConfigSubjectsRepository : IAcademicConfigSubjectsRepository
    {
        private readonly IDbConnection _connection;

        public AcademicConfigSubjectsRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<ServiceResponse<string>> AddUpdateAcademicConfigSubject(SubjectRequest request)
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            using (var connection = _connection)
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Check if there are any subjects to process
                        if (request.Subjects == null || request.Subjects.Count == 0)
                        {
                            return new ServiceResponse<string>(false, "No subjects provided.", string.Empty, 400);
                        }

                        // Process each subject
                        foreach (var subject in request.Subjects)
                        {
                            // SQL query to insert or update the subject and retrieve SubjectId
                            string upsertSubjectSql = @"
                    DECLARE @OutputSubjectId INT;

                    IF EXISTS (SELECT 1 FROM tbl_Subjects WHERE SubjectId = @SubjectId)
                    BEGIN
                        UPDATE tbl_Subjects
                        SET SubjectName = @SubjectName,
                            SubjectCode = @SubjectCode,
                            subject_type_id = @subject_type_id,
                            IsDeleted = @IsDeleted,
                            InstituteId = @InstituteId
                        WHERE SubjectId = @SubjectId;

                        -- Get the updated SubjectId
                        SET @OutputSubjectId = @SubjectId;
                    END
                    ELSE
                    BEGIN
                        INSERT INTO tbl_Subjects (InstituteId, SubjectName, SubjectCode, subject_type_id, IsDeleted)
                        VALUES (@InstituteId, @SubjectName, @SubjectCode, @subject_type_id, 0);
                        
                        -- Get the new SubjectId
                        SET @OutputSubjectId = CAST(SCOPE_IDENTITY() AS INT);
                    END

                    SELECT @OutputSubjectId; -- Return the SubjectId
                    ";

                            // Map the subject properties to the parameters
                            var subjectParams = new
                            {
                                InstituteId = request.InstituteId,
                                subject.SubjectId,
                                subject.SubjectName,
                                subject.SubjectCode,
                                subject.subject_type_id,
                                subject.IsDeleted
                            };

                            // Execute the upsert and retrieve the SubjectId
                            var subjectId = await connection.QuerySingleAsync<int>(upsertSubjectSql, subjectParams, transaction);

                            // Update SubjectId for all mappings in the request
                            if (request.SubjectSectionMappingRequests != null && request.SubjectSectionMappingRequests.Count > 0)
                            {
                                foreach (var mapping in request.SubjectSectionMappingRequests)
                                {
                                    mapping.SubjectId = subjectId;
                                }

                                // Handle subject section mappings
                                int rowsInserted = await HandleSubjectMappings(request.SubjectSectionMappingRequests, subjectId, transaction);

                                // If handling mappings failed, rollback the transaction and return an error
                                if (rowsInserted < 0)
                                {
                                    transaction.Rollback();
                                    return new ServiceResponse<string>(false, "Failed to save subject mappings.", string.Empty, 500);
                                }
                            }
                        }

                        // Commit transaction if all operations were successful
                        transaction.Commit();
                        return new ServiceResponse<string>(true, "Subjects and mappings saved successfully.", "Operation successful", 200);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
        public async Task<ServiceResponse<List<SubjectType>>> GetSubjectTypeList()
        {
            try
            {
                string sql = "SELECT subject_type_id, subject_type FROM tbl_SubjectTypeMaster";

                // Execute the query and retrieve the subject types
                var subjectTypes = await _connection.QueryAsync<SubjectType>(sql);

                // Check if any subject types were found
                if (!subjectTypes.Any())
                {
                    return new ServiceResponse<List<SubjectType>>(false, "No subject types found", new List<SubjectType>(), 204);
                }

                return new ServiceResponse<List<SubjectType>>(true, "Subject types retrieved successfully", subjectTypes.ToList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<SubjectType>>(false, ex.Message, new List<SubjectType>(), 500);
            }
        }
        public async Task<ServiceResponse<SubjectType>> GetSubjectTypeById(int subjectTypeId)
        {
            try
            {
                string sql = "SELECT subject_type_id, subject_type FROM tbl_SubjectTypeMaster WHERE subject_type_id = @SubjectTypeId";

                // Execute the query and retrieve the subject type
                var subjectType = await _connection.QuerySingleOrDefaultAsync<SubjectType>(sql, new { SubjectTypeId = subjectTypeId });

                // Check if the subject type was found
                if (subjectType == null)
                {
                    return new ServiceResponse<SubjectType>(false, "Subject type not found", null, 404);
                }

                return new ServiceResponse<SubjectType>(true, "Subject type retrieved successfully", subjectType, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<SubjectType>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<string>> AddUpdateSubjectType(SubjectType request)
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
            using (var connection = _connection)
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Check if the subject type already exists
                        string upsertSubjectTypeSql = @"
                IF EXISTS (SELECT 1 FROM tbl_SubjectTypeMaster WHERE subject_type_id = @SubjectTypeId)
                BEGIN
                    UPDATE tbl_SubjectTypeMaster
                    SET subject_type = @subject_type
                    WHERE subject_type_id = @SubjectTypeId
                END
                ELSE
                BEGIN
                    INSERT INTO tbl_SubjectTypeMaster (subject_type)
                    VALUES (@subject_type);
                END";

                        // Prepare parameters
                        var parameters = new
                        {
                            SubjectTypeId = request.subject_type_id > 0 ? request.subject_type_id : (int?)null,
                            subject_type = request.subject_type
                        };

                        // Execute the upsert query
                        await connection.ExecuteAsync(upsertSubjectTypeSql, parameters, transaction);

                        // Commit transaction
                        transaction.Commit();
                        return new ServiceResponse<string>(true, "Subject type added/updated successfully.", "Operation successful", 200);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
        //public async Task<ServiceResponse<string>> AddUpdateAcademicConfigSubject(SubjectRequest request)
        //{
        //    if (_connection.State != ConnectionState.Open)
        //    {
        //        _connection.Open();
        //    }
        //    using (var connection = _connection)
        //    {
        //        using (var transaction = connection.BeginTransaction())
        //        {
        //            try
        //            {
        //                // Insert or update the subject
        //                string upsertSubjectSql = @"
        //            IF EXISTS (SELECT 1 FROM tbl_Subjects WHERE SubjectId = @SubjectId)
        //            BEGIN
        //                UPDATE tbl_Subjects
        //                SET SubjectName = @SubjectName,
        //                    SubjectCode = @SubjectCode,
        //                    subject_type_id = @subject_type_id,
        //                    IsDeleted = @IsDeleted
        //                WHERE SubjectId = @SubjectId
        //            END
        //            ELSE
        //            BEGIN
        //                INSERT INTO tbl_Subjects (InstituteId, SubjectName, SubjectCode, subject_type_id, IsDeleted)
        //                VALUES (@InstituteId, @SubjectName, @SubjectCode, @subject_type_id, @IsDeleted)
        //                SELECT CAST(SCOPE_IDENTITY() as int)
        //            END
        //        ";
        //                request.IsDeleted = false;
        //                if (request.SubjectId > 0)
        //                {
        //                    foreach (var data in request.SubjectSectionMappingRequests)
        //                    {
        //                        data.SubjectId = request.SubjectId;
        //                    }
        //                }
        //                var subjectId = request.SubjectId > 0
        //                    ? await connection.ExecuteAsync(upsertSubjectSql, request, transaction)
        //                    : await connection.QuerySingleAsync<int>(upsertSubjectSql, request, transaction);

        //                // Handle subject section mappings
        //                int rowsInserted = await HandleSubjectMappings(request.SubjectSectionMappingRequests, subjectId, transaction);

        //                // Commit transaction if all operations were successful
        //                if (rowsInserted >= 0)
        //                {
        //                    transaction.Commit();
        //                    return new ServiceResponse<string>(true, "Subject and mappings saved successfully.", "operation successful", 200);
        //                }
        //                else
        //                {
        //                    transaction.Rollback();
        //                    return new ServiceResponse<string>(false, "Failed to save subject mappings.", string.Empty, 500);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                transaction.Rollback();
        //                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
        //            }
        //            finally
        //            {
        //                connection.Close();
        //            }
        //        }
        //    }
        //}
        public async Task<ServiceResponse<string>> DeleteAcademicConfigSubject(int SubjectId)
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
            using (var connection = _connection)
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Soft delete the subject
                        string deleteSubjectSql = @"
                    UPDATE tbl_Subjects
                    SET IsDeleted = 1
                    WHERE SubjectId = @SubjectId
                ";
                        int rowsAffected = await connection.ExecuteAsync(deleteSubjectSql, new { SubjectId }, transaction);

                        if (rowsAffected > 0)
                        {
                            // Soft delete the subject mappings only if the subject update is successful
                            string deleteMappingsSql = @"
                        UPDATE tbl_ClassSectionSubjectMapping
                        SET IsDeleted = 1
                        WHERE SubjectId = @SubjectId
                    ";
                            await connection.ExecuteAsync(deleteMappingsSql, new { SubjectId }, transaction);

                            // Commit transaction
                            transaction.Commit();
                            return new ServiceResponse<string>(true, "Subject and mappings deleted successfully.", null, 200);
                        }
                        else
                        {
                            // Rollback transaction if no rows were affected in the subject update
                            transaction.Rollback();
                            return new ServiceResponse<string>(false, "Subject not found or already deleted.", null, 404);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new ServiceResponse<string>(false, ex.Message, null, 500);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
        public async Task<ServiceResponse<SubjectResponse>> GetAcademicConfigSubjectById(int SubjectId)
        {
            try
            {
                // SQL to retrieve subject details
                string subjectSql = @"
        SELECT s.SubjectId, s.InstituteId, s.SubjectName, s.SubjectCode, s.subject_type_id, st.subject_type AS subject_type_name
        FROM tbl_Subjects s
        LEFT JOIN tbl_SubjectTypeMaster st ON s.subject_type_id = st.subject_type_id
        WHERE s.SubjectId = @SubjectId AND s.IsDeleted = 0";

                var subject = await _connection.QueryFirstOrDefaultAsync<SubjectResponse>(subjectSql, new { SubjectId });

                if (subject == null)
                {
                    return new ServiceResponse<SubjectResponse>(false, "Subject not found or has been deleted", null, 404);
                }

                // SQL to retrieve class and section mappings for the subject
                string mappingsSql = @"
        SELECT csm.CSSMappingId, csm.SubjectId, sub.SubjectName, c.class_id, c.class_name, sec.section_id, sec.section_name
        FROM tbl_ClassSectionSubjectMapping csm
        INNER JOIN tbl_Class c ON csm.class_id = c.class_id
        LEFT JOIN (
            SELECT section_id, section_name FROM tbl_Section 
            WHERE IsDeleted = 0
        ) sec ON CHARINDEX(CONVERT(varchar, sec.section_id), csm.section_id) > 0
        INNER JOIN tbl_Subjects sub ON csm.SubjectId = sub.SubjectId
        WHERE csm.SubjectId = @SubjectId AND csm.IsDeleted = 0";

                var mappings = await _connection.QueryAsync<dynamic>(mappingsSql, new { SubjectId });

                // Group the mappings by class_id
                var groupedMappings = mappings
                    .GroupBy(m => m.class_id)
                    .Select(g => new SubjectSectionMappingResponse
                    {
                        CSSMappingId = g.First().CSSMappingId,
                        SubjectId = g.First().SubjectId,
                        SubjectName = g.First().SubjectName,
                        class_id = g.First().class_id,
                        className = g.First().class_name,
                        Section = g.Select(s => new Sections
                        {
                            section_id = s.section_id,
                            sectionName = s.section_name
                        }).ToList()
                    }).ToList();

                subject.SubjectSectionMappings = groupedMappings;

                return new ServiceResponse<SubjectResponse>(true, "Record found", subject, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<SubjectResponse>(false, ex.Message, new SubjectResponse(), 500);
            }
        }
        public async Task<ServiceResponse<List<SubjectResponse>>> GetAcademicConfigSubjectList(GetAllSubjectRequest request)
        {
            try
            {
                // Base SQL query to retrieve subjects
                string sql = @"
            SELECT s.SubjectId, s.InstituteId, s.SubjectName, s.SubjectCode, s.subject_type_id, 
                   st.subject_type AS subject_type_name
            FROM tbl_Subjects s
            LEFT JOIN tbl_SubjectTypeMaster st ON s.subject_type_id = st.subject_type_id
            WHERE s.IsDeleted = 0";

                // Add filters based on request
                if (request.Institute_id > 0)
                {
                    sql += " AND s.InstituteId = @Institute_id";
                }

                if (!string.IsNullOrEmpty(request.SearchText))
                {
                    sql += " AND s.SubjectName LIKE '%' + @SearchText + '%'";
                }

                if (request.ClassId > 0)
                {
                    sql += @" AND EXISTS (
                SELECT 1 FROM tbl_ClassSectionSubjectMapping csm
                WHERE csm.SubjectId = s.SubjectId AND csm.class_id = @ClassId AND csm.IsDeleted = 0
            )";
                }

                // Check if SectionId is provided
                if (request.SectionId > 0)
                {
                    sql += @" AND EXISTS (
                SELECT 1 FROM tbl_ClassSectionSubjectMapping csm
                WHERE csm.SubjectId = s.SubjectId
                AND CHARINDEX(CONVERT(varchar, @SectionId), csm.section_id) > 0
                AND csm.IsDeleted = 0
            )";
                }

                // Calculate offset for pagination
                int offset = (request.PageNumber - 1) * request.PageSize;
                sql += " ORDER BY s.SubjectName OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                // Execute the query and retrieve the subjects
                var subjects = await _connection.QueryAsync<SubjectResponse>(sql, new
                {
                    Institute_id = request.Institute_id,
                    SearchText = request.SearchText ?? string.Empty,
                    ClassId = request.ClassId,
                    SectionId = request.SectionId,
                    Offset = offset,
                    PageSize = request.PageSize
                });

                // If no subjects found, return empty response
                if (!subjects.Any())
                {
                    return new ServiceResponse<List<SubjectResponse>>(false, "No subjects found", new List<SubjectResponse>(), 204);
                }

                // SQL query to retrieve class and section mappings for the subjects
                string mappingsSql = @"
            SELECT csm.CSSMappingId, csm.SubjectId, c.class_id, c.class_name, sec.section_id, sec.section_name
            FROM tbl_ClassSectionSubjectMapping csm
            INNER JOIN tbl_Class c ON csm.class_id = c.class_id
            LEFT JOIN tbl_Section sec ON CHARINDEX(CONVERT(varchar, sec.section_id), csm.section_id) > 0
            WHERE csm.SubjectId IN @SubjectIds AND csm.IsDeleted = 0 AND c.IsDeleted = 0 
            AND (sec.IsDeleted = 0 OR sec.IsDeleted IS NULL)";

                // Add filters for class and section IDs
                if (request.ClassId > 0)
                {
                    mappingsSql += " AND c.class_id = @ClassId";
                }

                if (request.SectionId > 0)
                {
                    mappingsSql += " AND CHARINDEX(CONVERT(varchar, @SectionId), csm.section_id) > 0";
                }

                // Get the subject IDs to retrieve mappings
                var subjectIds = subjects.Select(s => s.SubjectId).ToList();

                // Execute the query and retrieve the mappings
                var mappings = await _connection.QueryAsync<dynamic>(mappingsSql, new
                {
                    SubjectIds = subjectIds,
                    ClassId = request.ClassId,
                    SectionId = request.SectionId
                });

                // Group the mappings by subject ID
                var groupedMappings = mappings.GroupBy(m => m.SubjectId).ToDictionary(g => g.Key, g => g.ToList());

                // Populate the subject mappings
                foreach (var subject in subjects)
                {
                    if (groupedMappings.ContainsKey(subject.SubjectId))
                    {
                        subject.SubjectSectionMappings = groupedMappings[subject.SubjectId]
                            .GroupBy(m => m.class_id)
                            .Select(g => new SubjectSectionMappingResponse
                            {
                                CSSMappingId = g.First().CSSMappingId,
                                SubjectId = g.First().SubjectId,
                                SubjectName = subject.SubjectName,
                                class_id = g.First().class_id,
                                className = g.First().class_name,
                                Section = g.Select(s => new Sections
                                {
                                    section_id = s.section_id,
                                    sectionName = s.section_name
                                }).ToList()
                            }).ToList();
                    }
                    else
                    {
                        subject.SubjectSectionMappings = new List<SubjectSectionMappingResponse>();
                    }
                }

                return new ServiceResponse<List<SubjectResponse>>(true, "Subjects retrieved successfully", subjects.ToList(), 200, subjectIds.Count());
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<SubjectResponse>>(false, ex.Message, new List<SubjectResponse>(), 500);
            }
        }
        private async Task<int> HandleSubjectMappings(List<SubjectSectionMappingRequest>? mappings, int subjectId, IDbTransaction transaction)
        {
            if (mappings == null || mappings.Count == 0) return 0;

            // Delete existing mappings
            string deleteMappingSql = "DELETE FROM tbl_ClassSectionSubjectMapping WHERE SubjectId = @SubjectId";
            await _connection.ExecuteAsync(deleteMappingSql, new { SubjectId = subjectId }, transaction);

            // Insert new mappings
            string insertMappingSql = @"
            INSERT INTO tbl_ClassSectionSubjectMapping (SubjectId, class_id, section_id, IsDeleted)
            VALUES (@SubjectId, @class_id, @section_id, @IsDeleted);
        ";

            var mappingParams = mappings.Select(mapping => new
            {
                SubjectId = subjectId,
                mapping.class_id,
                mapping.section_id,
                IsDeleted = false
            }).ToList();

            int rowsInserted = await _connection.ExecuteAsync(insertMappingSql, mappingParams, transaction);

            return rowsInserted;
        }
        public async Task<ServiceResponse<byte[]>> DownloadExcelSheet(ExcelDownloadRequest request)
        {
            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                // SQL query to fetch subjects, class-section mappings, and subject types based on filters
                string sql = @"
        SELECT s.SubjectName, s.SubjectCode, st.subject_type, csm.class_id, csm.section_id
        FROM tbl_Subjects s
        LEFT JOIN tbl_ClassSectionSubjectMapping csm ON s.SubjectId = csm.SubjectId
        LEFT JOIN tbl_SubjectTypeMaster st ON s.subject_type_id = st.subject_type_id
        WHERE s.InstituteId = @InstituteId AND s.IsDeleted = 0
        AND (@ClassId = 0 OR csm.class_id = @ClassId)
        AND (@SectionId = 0 OR csm.section_id LIKE '%' + CAST(@SectionId AS VARCHAR) + '%')
        AND csm.IsDeleted = 0";

                var subjectResult = await _connection.QueryAsync(sql, new { request.InstituteId, request.ClassId, request.SectionId });

                // If no data is present, return only headers
                if (!subjectResult.Any())
                {
                    return await GenerateEmptyExcelSheet();
                }

                // Prepare to gather all unique section IDs across results
                List<int> allSectionIds = new List<int>();

                foreach (var row in subjectResult)
                {
                    string sectionIdCsv = row.section_id;
                    if (!string.IsNullOrEmpty(sectionIdCsv))
                    {
                        string[] sectionIdStrings = sectionIdCsv.Split(',');
                        foreach (var sectionIdStr in sectionIdStrings)
                        {
                            if (int.TryParse(sectionIdStr, out int sectionId))
                            {
                                if (!allSectionIds.Contains(sectionId))
                                {
                                    allSectionIds.Add(sectionId);
                                }
                            }
                        }
                    }
                }

                // Fetch corresponding section names
                var sectionSql = @"
        SELECT section_id, section_name 
        FROM tbl_Section 
        WHERE section_id IN @SectionIds";

                var sectionData = await _connection.QueryAsync(sectionSql, new { SectionIds = allSectionIds.ToArray() });

                // Create a dictionary for fast lookup of section names by their ID
                Dictionary<int, string> sectionIdToNameMap = sectionData.ToDictionary(row => (int)row.section_id, row => (string)row.section_name);

                // Prepare the final result set
                List<dynamic> finalResult = new List<dynamic>();

                foreach (var row in subjectResult)
                {
                    string[] sectionIdStrings = row.section_id.Split(',');
                    List<string> sectionNames = new List<string>();

                    foreach (var sectionIdStr in sectionIdStrings)
                    {
                        if (int.TryParse(sectionIdStr, out int sectionId) && sectionIdToNameMap.ContainsKey(sectionId))
                        {
                            sectionNames.Add(sectionIdToNameMap[sectionId]);
                        }
                    }

                    finalResult.Add(new
                    {
                        SubjectName = row.SubjectName,
                        SubjectCode = row.SubjectCode,
                        SubjectType = row.subject_type,
                        ClassId = row.class_id,
                        Sections = string.Join(", ", sectionNames) // Join all the section names
                    });
                }

                // Create and return the Excel file with EPPlus
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Subject Details");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Sr No";
                    worksheet.Cells[1, 2].Value = "Subject Name";
                    worksheet.Cells[1, 3].Value = "Subject Code";
                    worksheet.Cells[1, 4].Value = "Class";
                    worksheet.Cells[1, 5].Value = "Sections";
                    worksheet.Cells[1, 6].Value = "Subject Type";

                    // Fill the worksheet with data
                    int rowNumber = 2;
                    int serialNumber = 1;

                    foreach (var entry in finalResult)
                    {
                        worksheet.Cells[rowNumber, 1].Value = serialNumber++;
                        worksheet.Cells[rowNumber, 2].Value = entry.SubjectName;
                        worksheet.Cells[rowNumber, 3].Value = entry.SubjectCode;
                        worksheet.Cells[rowNumber, 4].Value = entry.ClassId;
                        worksheet.Cells[rowNumber, 5].Value = entry.Sections;
                        worksheet.Cells[rowNumber, 6].Value = entry.SubjectType;

                        rowNumber++;
                    }

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Convert the Excel package to byte array and return it
                    var excelData = package.GetAsByteArray();
                    return new ServiceResponse<byte[]>(true, "Excel file generated successfully", excelData, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, $"Error generating Excel file: {ex.Message}", null, 500);
            }
        }

        // Helper method to generate an empty Excel sheet with just headers
        private async Task<ServiceResponse<byte[]>> GenerateEmptyExcelSheet()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Subject Details");

                // Add headers
                worksheet.Cells[1, 1].Value = "Sr No";
                worksheet.Cells[1, 2].Value = "Subject Name";
                worksheet.Cells[1, 3].Value = "Subject Code";
                worksheet.Cells[1, 4].Value = "Class";
                worksheet.Cells[1, 5].Value = "Sections";
                worksheet.Cells[1, 6].Value = "Subject Type";

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var excelData = package.GetAsByteArray();
                return new ServiceResponse<byte[]>(true, "No data found. Returning an empty Excel sheet.", excelData, 200);
            }
        }
    }
}