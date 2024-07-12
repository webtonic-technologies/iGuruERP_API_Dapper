using Dapper;
using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Repository.Interfaces;
using System.Data;

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
                        // Insert or update the subject
                        string upsertSubjectSql = @"
                    IF EXISTS (SELECT 1 FROM tbl_Subjects WHERE SubjectId = @SubjectId)
                    BEGIN
                        UPDATE tbl_Subjects
                        SET SubjectName = @SubjectName,
                            SubjectCode = @SubjectCode,
                            subject_type_id = @subject_type_id,
                            IsDeleted = @IsDeleted
                        WHERE SubjectId = @SubjectId
                    END
                    ELSE
                    BEGIN
                        INSERT INTO tbl_Subjects (InstituteId, SubjectName, SubjectCode, subject_type_id, IsDeleted)
                        VALUES (@InstituteId, @SubjectName, @SubjectCode, @subject_type_id, @IsDeleted)
                        SELECT CAST(SCOPE_IDENTITY() as int)
                    END
                ";
                        request.IsDeleted = false;
                        var subjectId = request.SubjectId > 0
                            ? await connection.ExecuteAsync(upsertSubjectSql, request, transaction)
                            : await connection.QuerySingleAsync<int>(upsertSubjectSql, request, transaction);

                        // Handle subject section mappings
                        int rowsInserted = await HandleSubjectMappings(request.SubjectSectionMappingRequests, subjectId, transaction);

                        // Commit transaction if all operations were successful
                        if (rowsInserted >= 0)
                        {
                            transaction.Commit();
                            return new ServiceResponse<string>(true, "Subject and mappings saved successfully.", string.Empty, 200);
                        }
                        else
                        {
                            transaction.Rollback();
                            return new ServiceResponse<string>(false, "Failed to save subject mappings.", string.Empty, 500);
                        }
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
            SELECT s.SubjectId, s.InstituteId, s.SubjectName, s.SubjectCode, s.subject_type_id, st.subject_type
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
            SELECT csm.CSSMappingId, csm.SubjectId, s.SubjectName, csm.class_id, c.class_name, sec.section_id, sec.section_name
            FROM tbl_ClassSectionSubjectMapping csm
            INNER JOIN tbl_Class c ON csm.class_id = c.class_id
            LEFT JOIN tbl_Section sec ON csm.section_id = sec.section_id
            WHERE csm.SubjectId = @SubjectId AND csm.IsDeleted = 0 AND c.IsDeleted = 0 AND sec.IsDeleted = 0";

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
            SELECT s.SubjectId, s.InstituteId, s.SubjectName, s.SubjectCode, s.subject_type_id, st.subject_type
            FROM tbl_Subjects s
            LEFT JOIN tbl_SubjectTypeMaster st ON s.subject_type_id = st.subject_type_id
            WHERE s.InstituteId = @Institute_id AND s.IsDeleted = 0
            AND (@SearchText = '' OR s.SubjectName LIKE '%' + @SearchText + '%')
            AND (@ClassId = 0 OR EXISTS (
                SELECT * FROM tbl_ClassSectionSubjectMapping csm
                WHERE csm.SubjectId = s.SubjectId AND csm.class_id = @ClassId AND csm.IsDeleted = 0
            ))
            AND (@SectionId = 0 OR EXISTS (
                SELECT * FROM tbl_ClassSectionSubjectMapping csm
                WHERE csm.SubjectId = s.SubjectId AND csm.section_id = @SectionId AND csm.IsDeleted = 0
            ))
            ORDER BY s.SubjectName
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                // Calculate offset for pagination
                int offset = (request.PageNumber - 1) * request.PageSize;

                // Execute the query and retrieve the subjects
                var subjects = await _connection.QueryAsync<SubjectResponse>(sql, new
                {
                    Institute_id = request.Institute_id,
                    SearchText = request.SearchText,
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
            SELECT csm.CSSMappingId, csm.SubjectId, csm.class_id, c.class_name, sec.section_id, sec.section_name
            FROM tbl_ClassSectionSubjectMapping csm
            INNER JOIN tbl_Class c ON csm.class_id = c.class_id
            LEFT JOIN tbl_Section sec ON csm.section_id = sec.section_id
            WHERE csm.SubjectId IN @SubjectIds AND csm.IsDeleted = 0 AND c.IsDeleted = 0 AND sec.IsDeleted = 0";

                // Get the subject IDs to retrieve mappings
                var subjectIds = subjects.Select(s => s.SubjectId).ToList();

                // Execute the query and retrieve the mappings
                var mappings = await _connection.QueryAsync<dynamic>(mappingsSql, new { SubjectIds = subjectIds });

                // Group the mappings by subject ID and populate the response
                var groupedMappings = mappings.GroupBy(m => m.SubjectId).ToDictionary(g => g.Key, g => g.ToList());

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

                return new ServiceResponse<List<SubjectResponse>>(true, "Subjects retrieved successfully", subjects.ToList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<SubjectResponse>>(false, ex.Message, [], 500);
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
        INSERT INTO tbl_ClassSectionSubjectMapping (SubjectId, class_id, section_id)
        VALUES (@SubjectId, @class_id, @section_id);
    ";

            var mappingParams = mappings.Select(mapping => new
            {
                SubjectId = subjectId,
                mapping.class_id,
                mapping.section_id
            }).ToList();

            int rowsInserted = await _connection.ExecuteAsync(insertMappingSql, mappingParams, transaction);

            return rowsInserted;
        }
    }
}
