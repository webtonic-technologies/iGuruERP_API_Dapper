using Communication_API.DTOs.Requests.DigitalDiary;
using Communication_API.DTOs.Responses;
using Communication_API.DTOs.Responses.DigitalDiary;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.DigitalDiary;
using Communication_API.Repository.Interfaces.DigitalDiary;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Communication_API.Repository.Implementations.DigitalDiary
{
    public class DigitalDiaryRepository : IDigitalDiaryRepository
    {
        private readonly IDbConnection _connection;

        public DigitalDiaryRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateDiary(AddUpdateDiaryRequest request)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    int insertedDiaryId = request.DiaryID;

                    // Insert or Update the Diary
                    if (request.DiaryID == 0)
                    {
                        // Insert new diary and get DiaryID using SCOPE_IDENTITY()
                        var query = @"INSERT INTO [tblDiary] (InstituteID, ClassID, SectionID, SubjectID, Remarks, CreatedBy) 
                              VALUES (@InstituteID, @ClassID, @SectionID, @SubjectID, @Remarks, @CreatedBy);
                              SELECT CAST(SCOPE_IDENTITY() as int);";

                        insertedDiaryId = await _connection.ExecuteScalarAsync<int>(query, request, transaction);
                    }
                    else
                    {
                        // Update existing diary
                        var query = @"UPDATE [tblDiary] 
                              SET InstituteID = @InstituteID, ClassID = @ClassID, SectionID = @SectionID, 
                                  SubjectID = @SubjectID, Remarks = @Remarks
                              WHERE DiaryID = @DiaryID";

                        await _connection.ExecuteAsync(query, request, transaction);
                    }

                    // Delete previous student mappings (if updating)
                    string deleteExistingMappingsQuery = @"DELETE FROM tblDiaryStudentMapping WHERE DiaryID = @DiaryID";
                    await _connection.ExecuteAsync(deleteExistingMappingsQuery, new { DiaryID = insertedDiaryId }, transaction);

                    // Insert new student mappings
                    foreach (var studentId in request.StudentIDs)
                    {
                        string insertMappingQuery = @"INSERT INTO tblDiaryStudentMapping (DiaryID, StudentID) 
                                              VALUES (@DiaryID, @StudentID)";
                        await _connection.ExecuteAsync(insertMappingQuery, new { DiaryID = insertedDiaryId, StudentID = studentId }, transaction);
                    }

                    transaction.Commit();
                    return new ServiceResponse<string>(true, "Operation Successful", "Success", 200);
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    _connection.Close();
                }
            }
        }


        public async Task<ServiceResponse<List<DiaryResponse>>> GetAllDiary(GetAllDiaryRequest request)
        {
            // Parse the StartDate and EndDate into DateTime format
            DateTime? parsedStartDate = string.IsNullOrEmpty(request.StartDate) ? (DateTime?)null : DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime? parsedEndDate = string.IsNullOrEmpty(request.EndDate) ? (DateTime?)null : DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            // Updated SQL query to handle string dates, format EnDate, and include search by StudentName
            var sql = @"
                SELECT 
                    d.DiaryID,
                    s.student_id AS StudentID,
                    s.Admission_Number AS AdmissionNumber,
                    CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName,
                    CONCAT(c.class_name, '-', sec.section_name) AS ClassSection,
                    subj.subject_name AS Subject,
                    d.Remarks AS DiaryRemarks,
                    FORMAT(d.EnDate, 'dd MMMM yyyy, hh:mm tt', 'en-US') AS ShareOn,   -- Format EnDate to required string format
                    e.First_Name + ' ' + e.Last_Name AS GivenBy
                FROM tblDiary d
                INNER JOIN tblDiaryStudentMapping dm ON d.DiaryID = dm.DiaryID   -- Join tblDiaryStudentMapping for StudentID
                INNER JOIN tbl_StudentMaster s ON dm.StudentID = s.student_id     -- Join tbl_StudentMaster for Student details
                INNER JOIN tbl_Class c ON d.ClassID = c.class_id
                INNER JOIN tbl_Section sec ON d.SectionID = sec.section_id
                INNER JOIN tbl_InstituteSubjects subj ON d.SubjectID = subj.institute_subject_id
                LEFT JOIN tbl_EmployeeProfileMaster e ON e.Employee_id = d.CreatedBy
                WHERE d.InstituteID = @InstituteID AND d.IsActive = 1
                  AND (@StartDate IS NULL OR d.EnDate >= @StartDate)   -- Use parsed StartDate parameter
                  AND (@EndDate IS NULL OR d.EnDate <= @EndDate)       -- Use parsed EndDate parameter
                  AND s.class_id = @ClassID AND s.section_id = @SectionID 
                  AND d.CreatedBy = @EmployeeID
                  AND (@Search IS NULL OR CONCAT(s.First_Name, ' ', s.Last_Name) LIKE '%' + @Search + '%') -- Search by StudentName
                ORDER BY d.DiaryID 
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var parameters = new
            {
                request.InstituteID,
                request.EmployeeID,
                request.ClassID,
                request.SectionID,
                StartDate = parsedStartDate,
                EndDate = parsedEndDate,
                Search = request.Search,
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var diaries = await _connection.QueryAsync<DiaryResponse>(sql, parameters);

            return new ServiceResponse<List<DiaryResponse>>(true, "Records Found", diaries.ToList(), 200, diaries.Count());
        }



        public async Task<ServiceResponse<string>> DeleteDiary(int DiaryID)
        {
            // Update the diary to set IsActive to 0 for soft delete
            var query = "UPDATE [tblDiary] SET IsActive = 0 WHERE DiaryID = @DiaryID";
            var result = await _connection.ExecuteAsync(query, new { DiaryID });

            if (result > 0)
            {
                return new ServiceResponse<string>(true, "Diary has been deactivated successfully.", "Success", 200);
            }
            else
            {
                return new ServiceResponse<string>(false, "Diary deactivation failed.", "Failure", 400);
            }
        }


        public async Task<IEnumerable<GetAllDiaryExportResponse>> GetAllDiaryExport(GetAllDiaryExportRequest request)
        {
            using (IDbConnection db = _connection) // Use _connection here instead of creating a new connection
            {
                db.Open();

                string query = @"
                SELECT 
                    CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName,
                    s.Admission_Number AS AdmissionNumber,
                    CONCAT(c.class_name, '-', sec.section_name) AS ClassSection,
                    subj.subject_name AS Subject,
                    d.Remarks AS DiaryRemarks,
                    FORMAT(d.EnDate, 'dd MMMM yyyy, hh:mm tt', 'en-US') AS ShareOn,
                    e.First_Name + ' ' + e.Last_Name AS GivenBy
                FROM tblDiary d
                INNER JOIN tblDiaryStudentMapping dm ON d.DiaryID = dm.DiaryID
                INNER JOIN tbl_StudentMaster s ON dm.StudentID = s.student_id
                INNER JOIN tbl_Class c ON d.ClassID = c.class_id
                INNER JOIN tbl_Section sec ON d.SectionID = sec.section_id
                INNER JOIN tbl_InstituteSubjects subj ON d.SubjectID = subj.institute_subject_id
                LEFT JOIN tbl_EmployeeProfileMaster e ON e.Employee_id = d.CreatedBy
                WHERE d.InstituteID = @InstituteID AND d.IsActive = 1
                  AND (@StartDate IS NULL OR d.EnDate >= @StartDate)
                  AND (@EndDate IS NULL OR d.EnDate <= @EndDate)
                  AND s.class_id = @ClassID 
                  AND s.section_id = @SectionID 
                  AND d.CreatedBy = @EmployeeID
                  AND (@Search IS NULL OR CONCAT(s.First_Name, ' ', s.Last_Name) LIKE '%' + @Search + '%')
                ORDER BY d.DiaryID";

                var parameters = new
                {
                    request.InstituteID,
                    request.EmployeeID,
                    StartDate = request.StartDate != null ? DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", null) : (DateTime?)null,
                    EndDate = request.EndDate != null ? DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", null) : (DateTime?)null,
                    request.ClassID,
                    request.SectionID,
                    request.Search
                };

                return await db.QueryAsync<GetAllDiaryExportResponse>(query, parameters);
            }
        }


    }
}
