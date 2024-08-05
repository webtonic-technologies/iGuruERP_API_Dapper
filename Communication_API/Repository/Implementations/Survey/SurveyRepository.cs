using Communication_API.DTOs.Requests.Survey;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.Survey;
using Communication_API.Repository.Interfaces.Survey;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Communication_API.Repository.Implementations.Survey
{
    public class SurveyRepository : ISurveyRepository
    {
        private readonly IDbConnection _connection;

        public SurveyRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> CreateSurvey(CreateSurveyRequest request)
        {
            var query = request.SurveyID == 0
                ? "INSERT INTO [tblSurvey] (SurveyName, Description, StartDate, EndDate, IsStudent, IsEmployee) VALUES (@SurveyName, @Description, @StartDate, @EndDate, @IsStudent, @IsEmployee)"
                : "UPDATE [tblSurvey] SET SurveyName = @SurveyName, Description = @Description, StartDate = @StartDate, StartTime = @StartTime, EndDate = @EndDate, EndTime = @EndTime, IsStudent = @IsStudent, IsEmployee = @IsEmployee WHERE SurveyID = @SurveyID";

            var parameters = new
            {
                request.SurveyID,
                request.SurveyName,
                request.Description,
                request.StartDate,
                request.EndDate,
                request.IsStudent,
                request.IsEmployee
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
        }

        public async Task<ServiceResponse<int>> GetTotalResponseCount()
        {
            var query = "SELECT COUNT(*) FROM [tblSurvey]";
            var totalCount = await _connection.ExecuteScalarAsync<int>(query);
            return new ServiceResponse<int>(true, "Total Response Count Retrieved", totalCount, 200);
        }

        public async Task<ServiceResponse<int>> GetInProgressSurveysCount()
        {
            var query = "SELECT COUNT(*) FROM [tblSurvey] WHERE EndDate >= GETDATE()";
            var inProgressCount = await _connection.ExecuteScalarAsync<int>(query);
            return new ServiceResponse<int>(true, "In Progress Surveys Count Retrieved", inProgressCount, 200);
        }

        public async Task<ServiceResponse<int>> GetPastSurveysCount()
        {
            var query = "SELECT COUNT(*) FROM [tblSurvey] WHERE EndDate < GETDATE()";
            var pastCount = await _connection.ExecuteScalarAsync<int>(query);
            return new ServiceResponse<int>(true, "Past Surveys Count Retrieved", pastCount, 200);
        }

        public async Task<ServiceResponse<List<Communication_API.Models.Survey.Survey>>> GetAllActiveSurveys(GetAllSurveysRequest request)
        {
            var countSql = "SELECT COUNT(*) FROM [tblSurvey] WHERE StartDate <= GETDATE() AND EndDate >= GETDATE()";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"SELECT * FROM [tblSurvey]
                        WHERE StartDate <= GETDATE() AND EndDate >= GETDATE()
                        ORDER BY SurveyID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var surveys = await _connection.QueryAsync<Communication_API.Models.Survey.Survey>(sql, parameters);
            return new ServiceResponse<List<Communication_API.Models.Survey.Survey>>(true, "Records Found", surveys.ToList(), 302, totalCount);
        }

        public async Task<ServiceResponse<List<Communication_API.Models.Survey.Survey>>> GetAllScheduledSurveys(GetAllSurveysRequest request)
        {
            var countSql = "SELECT COUNT(*) FROM [tblSurvey] WHERE StartDate > GETDATE()";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"SELECT * FROM [tblSurvey]
                        WHERE StartDate > GETDATE()
                        ORDER BY SurveyID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var surveys = await _connection.QueryAsync<Communication_API.Models.Survey.Survey>(sql, parameters);
            return new ServiceResponse<List<Communication_API.Models.Survey.Survey>>(true, "Records Found", surveys.ToList(), 302, totalCount);
        }

        public async Task<ServiceResponse<List<Communication_API.Models.Survey.Survey>>> GetAllPastSurveys(GetAllSurveysRequest request)
        {
            var countSql = "SELECT COUNT(*) FROM [tblSurvey] WHERE EndDate < GETDATE()";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"SELECT * FROM [tblSurvey]
                        WHERE EndDate < GETDATE()
                        ORDER BY SurveyID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var surveys = await _connection.QueryAsync<Communication_API.Models.Survey.Survey>(sql, parameters);
            return new ServiceResponse<List<Communication_API.Models.Survey.Survey>>(true, "Records Found", surveys.ToList(), 302, totalCount);
        }

        public async Task<ServiceResponse<List<SurveyResponse>>> GetSurveysResponse(int SurveyID)
        {
            var sql = "SELECT * FROM [tblSurvey] WHERE SurveyID = @SurveyID";
            var responses = await _connection.QueryAsync<SurveyResponse>(sql, new { SurveyID });
            return new ServiceResponse<List<SurveyResponse>>(true, "Records Found", responses.ToList(), 302);
        }
    }
}
