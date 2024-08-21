using Dapper;
using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Repository.Interfaces;
using System.Data;

namespace Institute_API.Repository.Implementations
{
    public class AcaConfigSubStudentRepository : IAcaConfigSubStudentRepository
    {
        private readonly IDbConnection _connection;

        public AcaConfigSubStudentRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<ServiceResponse<string>> AddUpdateSubjectStudentMapping(AcaConfigSubStudentRequest request)
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Check for valid request
                    if (request.SubStudentMappingReqs == null || !request.SubStudentMappingReqs.Any())
                    {
                        return new ServiceResponse<string>(false, "No mappings provided", "Failed", 400);
                    }

                    foreach (var mapping in request.SubStudentMappingReqs)
                    {
                        mapping.InstituteId = request.InstituteId;
                        string insertOrUpdateMappingSql = @"
                    IF EXISTS (SELECT * FROM tbl_StudentSubjectMapping WHERE SSMappingId = @SSMappingId)
                    BEGIN
                        UPDATE tbl_StudentSubjectMapping
                        SET InstituteId = @InstituteId, StudentId = @StudentId, SubjectId = @SubjectId
                        WHERE SSMappingId = @SSMappingId
                    END
                    ELSE
                    BEGIN
                        INSERT INTO tbl_StudentSubjectMapping (InstituteId, StudentId, SubjectId)
                        VALUES (@InstituteId, @StudentId, @SubjectId)
                    END
                ";

                        await _connection.ExecuteAsync(insertOrUpdateMappingSql, new
                        {
                            mapping.SSMappingId,
                            mapping.InstituteId,
                            mapping.StudentId,
                            mapping.SubjectId
                        }, transaction);
                    }

                    transaction.Commit();
                    return new ServiceResponse<string>(true, "Mappings added/updated successfully", "Success", 200);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ServiceResponse<string>(false, ex.Message, "Failed", 500);
                }
                finally
                {
                    _connection.Close();
                }
            }
        }
        public async Task<ServiceResponse<List<StudentListResponse>>> GetInstituteStudentsList(StudentListRequest request)
        {
            try
            {
                var sql = @"
            SELECT student_id AS StudentId,
                   CONCAT(First_Name, ' ', Middle_Name, ' ', Last_Name) AS StudentFullName,
                   Admission_Number AS AdmissionNumber
            FROM tbl_StudentMaster
            WHERE (class_id = @ClassId OR @ClassId = 0)
              AND (Institute_id = @Institute_id)
              AND (section_id = @SectionId OR @SectionId = 0)
              AND (First_Name + ' ' + Middle_Name + ' ' + Last_Name LIKE '%' + @SearchText + '%' OR @SearchText = '')
              AND isActive = 1
        ";

                var students = await _connection.QueryAsync<StudentListResponse>(sql, new
                {
                    request.ClassId,
                    request.SectionId,
                    request.SearchText,
                    request.Institute_id
                });

                if (students != null && students.Any())
                {
                    return new ServiceResponse<List<StudentListResponse>>(true, "Records found", students.ToList(), 200, students.Count());
                }
                else
                {
                    return new ServiceResponse<List<StudentListResponse>>(false, "No records found", new List<StudentListResponse>(), 204);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentListResponse>>(false, ex.Message, new List<StudentListResponse>(), 500);
            }
        }
        public async Task<ServiceResponse<List<SubjectList>>> GetInstituteSubjectsList(int SubjectTypeId)
        {
            try
            {
                var sql = @"
            SELECT s.SubjectId,
                   s.InstituteId,
                   s.SubjectName,
                   s.SubjectCode,
                   s.subject_type_id,
                   stm.subject_type AS SubjectTypeName
            FROM tbl_Subjects s
            INNER JOIN tbl_SubjectTypeMaster stm ON s.subject_type_id = stm.subject_type_id
            WHERE s.subject_type_id = @SubjectTypeId AND s.IsDeleted = 0
        ";

                var subjects = await _connection.QueryAsync<SubjectList>(sql, new { SubjectTypeId });

                if (subjects != null && subjects.Any())
                {
                    return new ServiceResponse<List<SubjectList>>(true, "Records found", subjects.ToList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<SubjectList>>(false, "No records found", new List<SubjectList>(), 204);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<SubjectList>>(false, ex.Message, new List<SubjectList>(), 500);
            }
        }
        public async Task<ServiceResponse<AcaConfigSubStudentRequest>> GetSubjectStudentMappingList(MappingListRequest request)
        {
            try
            {
                // Base SQL query to get mappings
                var sql = @"
        SELECT ssm.SSMappingId,
               ssm.InstituteId,
               ssm.StudentId,
               ssm.SubjectId
        FROM tbl_StudentSubjectMapping ssm
        INNER JOIN tbl_ClassSectionSubjectMapping cssm ON ssm.SubjectId = cssm.SubjectId
        INNER JOIN tbl_Subjects s ON s.SubjectId = ssm.SubjectId
        WHERE ssm.InstituteId = @InstituteId
          AND s.IsDeleted = 0
          AND cssm.IsDeleted = 0";

                // Initialize parameters
                var parameters = new DynamicParameters();
                parameters.Add("InstituteId", request.InstituteId);

                // Conditionally apply filters
                if (request.SubjectTypeId > 0)
                {
                    sql += " AND s.subject_type_id = @SubjectTypeId";
                    parameters.Add("SubjectTypeId", request.SubjectTypeId);
                }

                // Execute the query to get mappings
                var mappings = await _connection.QueryAsync<SubStudentMappingReq>(sql, parameters);

                // Check if mappings are found
                if (mappings != null && mappings.Any())
                {
                    // Get unique student IDs from the mappings
                    var studentIds = mappings.Select(m => m.StudentId).Distinct().ToList();

                    // Fetch class and section IDs for the students
                    var studentClassSectionSql = @"
            SELECT student_id, class_id, section_id
            FROM tbl_StudentMaster
            WHERE student_id IN @StudentIds";

                    var studentClassSections = await _connection.QueryAsync<dynamic>(studentClassSectionSql, new { StudentIds = studentIds });

                    // Create a dictionary for quick lookup of class and section by student ID
                    var studentClassSectionDict = studentClassSections.ToDictionary(s => s.student_id, s => new
                    {
                        ClassId = s.class_id,
                        SectionId = s.section_id
                    });

                    // Filter mappings based on request ClassId and SectionId
                    var filteredMappings = mappings
                        .Where(mapping =>
                            studentClassSectionDict.TryGetValue(mapping.StudentId, out var classSection) &&
                            classSection.ClassId == request.ClassId &&
                            classSection.SectionId == request.SectionId
                        ).ToList();

                    var response = new AcaConfigSubStudentRequest
                    {
                        InstituteId = request.InstituteId,
                        SubStudentMappingReqs = filteredMappings
                    };

                    if (filteredMappings.Any())
                    {
                        return new ServiceResponse<AcaConfigSubStudentRequest>(true, "Records found", response, 200);
                    }
                    else
                    {
                        return new ServiceResponse<AcaConfigSubStudentRequest>(false, "No records found", response, 204);
                    }
                }
                else
                {
                    return new ServiceResponse<AcaConfigSubStudentRequest>(false, "No records found", new AcaConfigSubStudentRequest(), 204);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AcaConfigSubStudentRequest>(false, ex.Message, new AcaConfigSubStudentRequest(), 500);
            }
        }
    }
}
