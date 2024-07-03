using Communication_API.DTOs.Requests.WhatsApp;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.WhatsApp;
using Communication_API.Repository.Interfaces.WhatsApp;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Communication_API.Repository.Implementations.WhatsApp
{
    public class WhatsAppRepository : IWhatsAppRepository
    {
        private readonly IDbConnection _connection;

        public WhatsAppRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> Setup(SetupWhatsAppRequest request)
        {
            var query = "INSERT INTO [tblWhatsAppConfiguration] (EmailID, PhoneNumber, Address) VALUES (@EmailID, @PhoneNumber, @Address)";

            var parameters = new
            {
                request.EmailID,
                request.PhoneNumber,
                request.Address
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
        }

        public async Task<ServiceResponse<WhatsAppConfiguration>> GetBalance(int VendorID)
        {
            var query = "SELECT * FROM [tblWhatsAppConfiguration] WHERE ConfigurationID = @VendorID";
            var config = await _connection.QueryFirstOrDefaultAsync<WhatsAppConfiguration>(query, new { VendorID });
            return new ServiceResponse<WhatsAppConfiguration>(config != null, config != null ? "Operation Successful" : "No Record Found", config, config != null ? 200 : 404);
        }

        public async Task<ServiceResponse<string>> AddUpdateTemplate(AddUpdateTemplateRequest request)
        {
            var query = request.TemplateID == 0
                ? "INSERT INTO [tblWhatsAppTemplate] (TemplateTypeID, TemplateName, TemplateMessage) VALUES (@TemplateTypeID, @TemplateName, @TemplateMessage)"
                : "UPDATE [tblWhatsAppTemplate] SET TemplateTypeID = @TemplateTypeID, TemplateName = @TemplateName, TemplateMessage = @TemplateMessage WHERE TemplateID = @TemplateID";

            var parameters = new
            {
                request.TemplateID,
                request.TemplateTypeID,
                request.TemplateName,
                request.TemplateMessage
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
        }

        public async Task<ServiceResponse<List<WhatsAppTemplate>>> GetWhatsAppTemplate(GetWhatsAppTemplateRequest request)
        {
            var countSql = "SELECT COUNT(*) FROM [tblWhatsAppTemplate]";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"SELECT * FROM [tblWhatsAppTemplate]
                        ORDER BY WhatsAppTemplateID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var templates = await _connection.QueryAsync<WhatsAppTemplate>(sql, parameters);
            return new ServiceResponse<List<WhatsAppTemplate>>(true, "Records Found", templates.ToList(), 302, totalCount);
        }

        public async Task<ServiceResponse<string>> Send(SendWhatsAppRequest request)
        {
            var query = "INSERT INTO [tblWhatsAppMessage] (PredefinedTemplateID, WhatsAppMessage, UserTypeID, GroupID, ScheduleNow, ScheduleDate, ScheduleTime) VALUES (@PredefinedTemplateID, @WhatsAppMessage, @UserTypeID, @GroupID, @ScheduleNow, @ScheduleDate, @ScheduleTime)";

            var parameters = new
            {
                request.PredefinedTemplateID,
                request.WhatsAppMessage,
                request.UserTypeID,
                request.GroupID,
                request.ScheduleNow,
                request.ScheduleDate,
                request.ScheduleTime
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
        }

        public async Task<ServiceResponse<List<WhatsAppReport>>> GetWhatsAppReport(GetWhatsAppReportRequest request)
        {
            var countSql = "SELECT COUNT(*) FROM [tblWhatsAppMessage]";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"SELECT * FROM [tblWhatsAppMessage]
                        ORDER BY WhatsAppMessageID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var reports = await _connection.QueryAsync<WhatsAppReport>(sql, parameters);
            return new ServiceResponse<List<WhatsAppReport>>(true, "Records Found", reports.ToList(), 302, totalCount);
        }
    }
}
