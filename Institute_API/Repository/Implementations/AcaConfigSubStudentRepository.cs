using Dapper;
using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Repository.Interfaces;
using OfficeOpenXml;
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
                            request.InstituteId,
                            request.StudentId,
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
        //public async Task<ServiceResponse<AcaConfigSubStudentResponse>> GetSubjectStudentMappingList(MappingListRequest request)
        //{
        //    try
        //    {
        //        // Base SQL query to get mappings
        //        var sql = @"
        //SELECT ssm.SSMappingId,
        //       ssm.InstituteId,
        //       ssm.StudentId,
        //       ssm.SubjectId
        //FROM tbl_StudentSubjectMapping ssm
        //INNER JOIN tbl_ClassSectionSubjectMapping cssm ON ssm.SubjectId = cssm.SubjectId
        //INNER JOIN tbl_Subjects s ON s.SubjectId = ssm.SubjectId
        //WHERE ssm.InstituteId = @InstituteId
        //  AND s.IsDeleted = 0
        //  AND cssm.IsDeleted = 0";

        //        // Initialize parameters
        //        var parameters = new DynamicParameters();
        //        parameters.Add("InstituteId", request.InstituteId);

        //        // Conditionally apply filters
        //        if (request.SubjectTypeId > 0)
        //        {
        //            sql += " AND s.subject_type_id = @SubjectTypeId";
        //            parameters.Add("SubjectTypeId", request.SubjectTypeId);
        //        }

        //        // Execute the query to get mappings
        //        var mappings = await _connection.QueryAsync<SubStudentMappingReq>(sql, parameters);

        //        // Check if mappings are found
        //        if (mappings != null && mappings.Any())
        //        {
        //            // Get unique student IDs from the mappings
        //            var studentIds = mappings.Select(m => m.StudentId).Distinct().ToList();

        //            // Fetch class and section IDs for the students
        //            var studentClassSectionSql = @"
        //    SELECT student_id, class_id, section_id
        //    FROM tbl_StudentMaster
        //    WHERE student_id IN @StudentIds";

        //            var studentClassSections = await _connection.QueryAsync<dynamic>(studentClassSectionSql, new { StudentIds = studentIds });

        //            // Create a dictionary for quick lookup of class and section by student ID
        //            var studentClassSectionDict = studentClassSections.ToDictionary(s => s.student_id, s => new
        //            {
        //                ClassId = s.class_id,
        //                SectionId = s.section_id
        //            });

        //            // Filter mappings based on request ClassId and SectionId
        //            var filteredMappings = mappings
        //                .Where(mapping =>
        //                    studentClassSectionDict.TryGetValue(mapping.StudentId, out var classSection) &&
        //                    classSection.ClassId == request.ClassId &&
        //                    classSection.SectionId == request.SectionId
        //                ).ToList();

        //            var response = new AcaConfigSubStudentRequest
        //            {
        //                InstituteId = request.InstituteId,
        //                SubStudentMappingReqs = filteredMappings
        //            };

        //            if (filteredMappings.Any())
        //            {
        //                return new ServiceResponse<AcaConfigSubStudentRequest>(true, "Records found", response, 200);
        //            }
        //            else
        //            {
        //                return new ServiceResponse<AcaConfigSubStudentRequest>(false, "No records found", response, 204);
        //            }
        //        }
        //        else
        //        {
        //            return new ServiceResponse<AcaConfigSubStudentRequest>(false, "No records found", new AcaConfigSubStudentRequest(), 204);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<AcaConfigSubStudentRequest>(false, ex.Message, new AcaConfigSubStudentRequest(), 500);
        //    }
        //}
        public async Task<ServiceResponse<AcaConfigSubStudentResponse>> GetSubjectStudentMappingList(MappingListRequest request)
        {
            try
            {
                // Base SQL query to get mappings
                var sql = @"
        SELECT ssm.SSMappingId,
               ssm.InstituteId,
               ssm.StudentId,
               ssm.SubjectId,
               s.SubjectName
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
                var mappings = await _connection.QueryAsync<dynamic>(sql, parameters);

                // Check if mappings are found
                if (mappings != null && mappings.Any())
                {
                    // Get unique student IDs from the mappings
                    var studentIds = mappings.Select(m => (int)m.StudentId).Distinct().ToList();

                    // Fetch class and section IDs for the students
                    var studentClassSectionSql = @"
            SELECT student_id, class_id, section_id
            FROM tbl_StudentMaster
            WHERE student_id IN @StudentIds";

                    var studentClassSections = await _connection.QueryAsync<dynamic>(studentClassSectionSql, new { StudentIds = studentIds });

                    // Create a list to store filtered mappings
                    var filteredMappings = new List<dynamic>();

                    // Iterate through each mapping to filter based on ClassId and SectionId
                    foreach (var mapping in mappings)
                    {
                        var studentDetail = studentClassSections.FirstOrDefault(s => s.student_id == mapping.StudentId);

                        if (studentDetail != null &&
                            studentDetail.class_id == request.ClassId &&
                            studentDetail.section_id == request.SectionId)
                        {
                            filteredMappings.Add(mapping);
                        }
                    }

                    // Prepare response
                    var response = new AcaConfigSubStudentResponse
                    {
                        InstituteId = request.InstituteId,
                        StudentSubjectResponses = filteredMappings
                            .GroupBy(m => m.SubjectId)
                            .Select(g => new StudentSubjectResponse
                            {
                                subjectId = g.Key,
                                SubjectName = g.First().SubjectName,
                                Students = g.Select(m => new Students
                                {
                                    StudentId = m.StudentId,
                                    StudentName = $"{m.First_Name} {m.Middle_Name} {m.Last_Name}".Trim()
                                }).ToList()
                            }).ToList()
                    };

                    // Return appropriate response
                    if (filteredMappings.Any())
                    {
                        return new ServiceResponse<AcaConfigSubStudentResponse>(true, "Records found", response, 200);
                    }
                    else
                    {
                        return new ServiceResponse<AcaConfigSubStudentResponse>(false, "No records found", response, 204);
                    }
                }
                else
                {
                    return new ServiceResponse<AcaConfigSubStudentResponse>(false, "No records found", new AcaConfigSubStudentResponse(), 204);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AcaConfigSubStudentResponse>(false, ex.Message, new AcaConfigSubStudentResponse(), 500);
            }
        }
        public async Task<ServiceResponse<byte[]>> DownloadExcelSheet(int InstituteId)
        {
            try
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                // SQL to fetch all students for the given InstituteId
                string studentSql = @"
            SELECT student_id, Admission_Number, First_Name, Middle_Name, Last_Name 
            FROM tbl_StudentMaster 
            WHERE Institute_id = @InstituteId AND isActive = 1";

                // SQL to fetch all subjects for the given InstituteId
                string subjectSql = @"
            SELECT SubjectId, SubjectName 
            FROM tbl_Subjects 
            WHERE InstituteId = @InstituteId AND IsDeleted = 0";

                // SQL to fetch the student-subject mappings
                string studentSubjectMappingSql = @"
            SELECT StudentId, SubjectId 
            FROM tbl_StudentSubjectMapping 
            WHERE InstituteId = @InstituteId";

                // Execute queries
                var students = await _connection.QueryAsync<dynamic>(studentSql, new { InstituteId });
                var subjects = await _connection.QueryAsync<dynamic>(subjectSql, new { InstituteId });
                var studentSubjectMappings = await _connection.QueryAsync<dynamic>(studentSubjectMappingSql, new { InstituteId });

                // Group subject mappings by student
                var studentToSubjectsMap = studentSubjectMappings
                    .GroupBy(mapping => mapping.StudentId)
                    .ToDictionary(group => group.Key, group => group.Select(mapping => (dynamic)mapping.SubjectId).ToList());

                // Initialize EPPlus Excel package
                using (var package = new ExcelPackage())
                {
                    // Add a worksheet
                    var worksheet = package.Workbook.Worksheets.Add("Student Subject Mapping");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Sr No";
                    worksheet.Cells[1, 2].Value = "Admission Number";
                    worksheet.Cells[1, 3].Value = "Student Name";

                    // Dynamically add headers for each subject
                    int subjectColumnStart = 4;
                    var subjectIds = subjects.Select(s => (int)s.SubjectId).ToList(); // Cast dynamic to int
                    int currentColumn = subjectColumnStart;

                    foreach (var subject in subjects)
                    {
                        worksheet.Cells[1, currentColumn].Value = subject.SubjectName;
                        currentColumn++;
                    }

                    // Populate student rows
                    int rowNumber = 2;
                    int serialNumber = 1;

                    foreach (var student in students)
                    {
                        // Set basic student info
                        worksheet.Cells[rowNumber, 1].Value = serialNumber++;
                        worksheet.Cells[rowNumber, 2].Value = student.Admission_Number;
                        worksheet.Cells[rowNumber, 3].Value = $"{student.First_Name} {student.Middle_Name} {student.Last_Name}";

                        // Get the subjects for this student, if any
                        if (studentToSubjectsMap.TryGetValue(student.student_id, out List<dynamic> assignedSubjectsDynamic))
                        {
                            // Convert dynamic subjects to int
                            var assignedSubjects = assignedSubjectsDynamic.Select(s => (int)s).ToList();

                            for (int i = 0; i < subjectIds.Count; i++)
                            {
                                var subjectId = subjectIds[i];
                                var cell = worksheet.Cells[rowNumber, subjectColumnStart + i];

                                if (assignedSubjects.Contains(subjectId))
                                {
                                    cell.Value = "Yes";
                                    // Apply color if the subject is assigned
                                    cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen); // Set the cell background color
                                }
                                else
                                {
                                    cell.Value = "No";
                                }
                            }
                        }
                        else
                        {
                            // If no subjects are assigned, mark all as "No"
                            for (int i = 0; i < subjectIds.Count; i++)
                            {
                                worksheet.Cells[rowNumber, subjectColumnStart + i].Value = "No";
                            }
                        }

                        rowNumber++;
                    }

                    // Auto-fit the columns for better visibility
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Convert the package to a byte array and return it
                    var excelData = package.GetAsByteArray();
                    return new ServiceResponse<byte[]>(true, "Excel file generated successfully", excelData, 200);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return new ServiceResponse<byte[]>(false, $"Error generating Excel file: {ex.Message}", null, 500);
            }
        }
    }
}
