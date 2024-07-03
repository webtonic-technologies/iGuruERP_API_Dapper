using Communication_API.DTOs.Requests.DigitalDiary;
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
                ? "INSERT INTO [tblDiary] (ClassID, SectionID, SubjectID, StudentID, Remarks) VALUES (@ClassID, @SectionID, @SubjectID, @StudentID, @Remarks)"
                : "UPDATE [tblDiary] SET ClassID = @ClassID, SectionID = @SectionID, SubjectID = @SubjectID, StudentID = @StudentID, Remarks = @Remarks WHERE DiaryID = @DiaryID";

            var parameters = new
            {
                request.DiaryID,
                request.ClassID,
                request.SectionID,
                request.SubjectID,
                request.StudentID,
                request.Remarks
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
        }

        public async Task<ServiceResponse<List<Communication_API.Models.DigitalDiary.DigitalDiary>>> GetAllDiary(GetAllDiaryRequest request)
        {
            var countSql = "SELECT COUNT(*) FROM [tblDiary]";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"SELECT * FROM [tblDiary]
                        ORDER BY DiaryID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var diaries = await _connection.QueryAsync<Communication_API.Models.DigitalDiary.DigitalDiary>(sql, parameters);
            return new ServiceResponse<List<Communication_API.Models.DigitalDiary.DigitalDiary>>(true, "Records Found", diaries.ToList(), 302, totalCount);
        }

        public async Task<ServiceResponse<string>> DeleteDiary(int DiaryID)
        {
            var query = "DELETE FROM [tblDiary] WHERE DiaryID = @DiaryID";
            var result = await _connection.ExecuteAsync(query, new { DiaryID });
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 200 : 400);
        }
    }
}
