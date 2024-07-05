using Communication_API.DTOs.Requests.Email;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.Email;
using Communication_API.Repository.Interfaces.Email;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Communication_API.Repository.Implementations.Email
{
    public class EmailRepository : IEmailRepository
    {
        private readonly IDbConnection _connection;

        public EmailRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> ConfigureEmail(ConfigureEmailRequest request)
        {
            var query = "INSERT INTO [tblEmailConfiguration] (SMTPUserName, SMTPPassword, SMTPServer, SMTPPort, Security) VALUES (@SMTPUserName, @SMTPPassword, @SMTPServer, @SMTPPort, @Security)";

            var parameters = new
            {
                request.SMTPUserName,
                request.SMTPPassword,
                request.SMTPServer,
                request.SMTPPort,
                request.Security
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
        }

        public async Task<ServiceResponse<string>> SendNewEmail(SendNewEmailRequest request)
        {
            var query = "INSERT INTO [tblEmailMaster] (EmailSubject, EmailBody, UserTypeID, GroupID, Status, ScheduleNow, ScheduleDate, ScheduleTime) VALUES (@EmailSubject, @EmailBody, @UserTypeID, @GroupID, @Status, @ScheduleNow, @ScheduleDate, @ScheduleTime)";

            var parameters = new
            {
                request.EmailSubject,
                request.EmailBody,
                request.UserTypeID,
                request.GroupID,
                Status = request.ScheduleNow,
                request.ScheduleNow,
                request.ScheduleDate,
                request.ScheduleTime
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
        }

        public async Task<ServiceResponse<List<EmailReport>>> GetEmailReports(GetEmailReportsRequest request)
        {
            var countSql = "SELECT COUNT(*) FROM [tblEmailMaster]";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"SELECT * FROM [tblEmailMaster]
                        ORDER BY EmailSendID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var reports = await _connection.QueryAsync<EmailReport>(sql, parameters);
            return new ServiceResponse<List<EmailReport>>(true, "Records Found", reports.ToList(), 302, totalCount);
        }
    }
}
