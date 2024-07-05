using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.SMS;
using Communication_API.Repository.Interfaces.SMS;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Communication_API.Repository.Implementations.SMS
{
    public class SMSRepository : ISMSRepository
    {
        private readonly IDbConnection _connection;

        public SMSRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> SetupSMSConfiguration(SetupSMSConfigurationRequest request)
        {
            var query = "INSERT INTO [tblSMSConfiguration] (APIkey, UserID, SenderID, Status) VALUES (@APIkey, @UserID, @SenderID, @Status)";

            var parameters = new
            {
                request.APIkey,
                request.UserID,
                request.SenderID,
                request.Status
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
        }

        public async Task<ServiceResponse<SMSBalance>> GetSMSBalance(int VendorID)
        {
            var sql = "SELECT * FROM [tblSMSBalance] WHERE VendorID = @VendorID";
            var balance = await _connection.QueryFirstOrDefaultAsync<SMSBalance>(sql, new { VendorID });
            return new ServiceResponse<SMSBalance>(true, "Record Found", balance, 302);
        }

        public async Task<ServiceResponse<string>> CreateSMSTemplate(CreateSMSTemplateRequest request)
        {
            var query = "INSERT INTO [tblSMSTemplate] (TemplateName, TemplateMessage) VALUES (@TemplateName, @TemplateMessage)";

            var parameters = new
            {
                request.TemplateName,
                request.TemplateMessage
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
        }

        public async Task<ServiceResponse<List<SMSTemplate>>> GetAllSMSTemplate(GetAllSMSTemplateRequest request)
        {
            var countSql = "SELECT COUNT(*) FROM [tblSMSTemplate]";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"SELECT * FROM [tblSMSTemplate]
                        ORDER BY TemplateID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var templates = await _connection.QueryAsync<SMSTemplate>(sql, parameters);
            return new ServiceResponse<List<SMSTemplate>>(true, "Records Found", templates.ToList(), 302, totalCount);
        }

        public async Task<ServiceResponse<string>> SendNewSMS(SendNewSMSRequest request)
        {
            //var query = "INSERT INTO [tblSMSMessage] (Message, PhoneNumber, SentDate, Status) VALUES (@Message, @PhoneNumber, @SentDate, @Status)";

            //var parameters = new
            //{
            //    request.Message,
            //    request.PhoneNumber,
            //    SentDate = request.ScheduleNow ? DateTime.Now : request.ScheduleDate + request.ScheduleTime,
            //    Status = request.ScheduleNow
            //};

            //var result = await _connection.ExecuteAsync(query, parameters);
            //return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);


            string sql;
            if (request.SMSID == 0)
            {
                sql = @"INSERT INTO [tblSMSMessage] (PredefinedTemplateID, SMSMessage, UserTypeID, GroupID, Status, ScheduleNow, ScheduleDate, ScheduleTime) VALUES (@PredefinedTemplateID, @SMSMessage, @UserTypeID, @GroupID, @Status, @ScheduleNow, @ScheduleDate, @ScheduleTime)";
            }
            else
            {
                sql = @"UPDATE [tblSMSMessage] SET PredefinedTemplateID = PredefinedTemplateID = @PredefinedTemplateID, SMSMessage = @SMSMessage, UserTypeID = @UserTypeID, GroupID = @GroupID, Status = @Status, ScheduleNow = @ScheduleNow, ScheduleDate = @ScheduleDate, ScheduleTime = @ScheduleTime WHERE SMSID = @SMSID";
            }

            var result = await _connection.ExecuteAsync(sql, request);
            if (result > 0)
            {
                return new ServiceResponse<string>(true, "Operation Successful", "PushNotification added/updated successfully", StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating PushNotification", StatusCodes.Status400BadRequest);
            }
        }

        public async Task<ServiceResponse<List<SMSReport>>> GetSMSReport(GetSMSReportRequest request)
        {
            var countSql = "SELECT COUNT(*) FROM [tblSMSMessage]";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"SELECT * FROM [tblSMSMessage]
                        ORDER BY SMSID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var reports = await _connection.QueryAsync<SMSReport>(sql, parameters);
            return new ServiceResponse<List<SMSReport>>(true, "Records Found", reports.ToList(), 302, totalCount);
        }
    }
}
