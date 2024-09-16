using Communication_API.DTOs.Requests.DigitalDiary;
using Communication_API.DTOs.Responses.DigitalDiary;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.DigitalDiary;
using Communication_API.Repository.Interfaces.DigitalDiary;
using Dapper;
using System.Data;
using System.Data.SqlClient;

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
            var query = request.DiaryID == 0
                ? @"INSERT INTO [tblDiary] (InstituteID, ClassID, SectionID, SubjectID, StudentID, Remarks, EnDate) 
           VALUES (@InstituteID, @ClassID, @SectionID, @SubjectID, @StudentID, @Remarks, @EnDate)"
                : @"UPDATE [tblDiary] 
           SET InstituteID = @InstituteID, ClassID = @ClassID, SectionID = @SectionID, 
               SubjectID = @SubjectID, StudentID = @StudentID, Remarks = @Remarks, EnDate = @EnDate 
           WHERE DiaryID = @DiaryID";

            var parameters = new
            {
                request.InstituteID,
                request.DiaryID,
                request.ClassID,
                request.SectionID,
                request.SubjectID,
                request.StudentID,
                request.Remarks,
                request.EnDate
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
        }


        public async Task<ServiceResponse<List<DiaryResponse>>> GetAllDiary(GetAllDiaryRequest request)
        {
            var countSql = "SELECT COUNT(*) FROM [tblDiary]";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);

            // SQL query to fetch the required data
            var sql = @"
        SELECT 
            d.DiaryID,
            s.student_id AS StudentID,
            CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName,
            CONCAT(c.class_name, '-', sec.section_name) AS ClassSection,
            subj.subject_name AS Subject,
            d.Remarks AS DiaryRemarks,
            d.EnDate AS ShareOn
        FROM tblDiary d
        INNER JOIN tbl_StudentMaster s ON d.StudentID = s.student_id
        INNER JOIN tbl_Class c ON d.ClassID = c.class_id
        INNER JOIN tbl_Section sec ON d.SectionID = sec.section_id
        INNER JOIN tbl_InstituteSubjects subj ON d.SubjectID = subj.institute_subject_id
        WHERE d.InstituteID = @InstituteID
          AND (@StartDate IS NULL OR d.EnDate >= @StartDate)
          AND (@EndDate IS NULL OR d.EnDate <= @EndDate)
        ORDER BY d.DiaryID 
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var parameters = new
            {
                request.InstituteID,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var diaries = await _connection.QueryAsync<DiaryResponse>(sql, parameters);

            return new ServiceResponse<List<DiaryResponse>>(true, "Records Found", diaries.ToList(), 302, totalCount);
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

    }
}
