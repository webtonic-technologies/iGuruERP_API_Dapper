using Communication_API.DTOs.Requests.PushNotification;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.PushNotification;
using Communication_API.Repository.Interfaces.PushNotification;
using Dapper;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Communication_API.Repository.Implementations.PushNotification
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IDbConnection _connection;

        public NotificationRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> TriggerNotification(TriggerNotificationRequest request)
        {
            //var query = request.NotificationID == 0
            //    ? "INSERT INTO [tblPushNotificationMaster] (PredefinedTemplateID, NotificationMessage, UserTypeID, GroupID, Status, ScheduleNow, ScheduleDate, ScheduleTime) " +
            //    "VALUES (@PredefinedTemplateID, @NotificationMessage, @UserTypeID, @GroupID, @Status, @ScheduleNow, @ScheduleDate, @ScheduleTime)"
            //    : "UPDATE [tblPushNotificationMaster] SET Message = @Message, SentDate = @SentDate, Status = @Status WHERE NotificationID = @NotificationID";

            //var parameters = new
            //{
            //    request.NotificationID,
            //    request.Message,
            //    SentDate = request.ScheduleNow ? DateTime.Now : request.ScheduleDate + request.ScheduleTime,
            //    Status = request.ScheduleNow
            //};

            //var result = await _connection.ExecuteAsync(query, parameters);
            //return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);


            string sql;
            if (request.PushNotificationID == 0)
            {
                sql = @"INSERT INTO [tblPushNotificationMaster] (PredefinedTemplateID, NotificationMessage, UserTypeID, GroupID, Status, ScheduleNow, ScheduleDate, ScheduleTime) VALUES (@PredefinedTemplateID, @NotificationMessage, @UserTypeID, @GroupID, @Status, @ScheduleNow, @ScheduleDate, @ScheduleTime)";
            }
            else
            {
                sql = @"UPDATE [tblPushNotificationMaster] SET PredefinedTemplateID = @PredefinedTemplateID, NotificationMessage = @NotificationMessage, UserTypeID = @UserTypeID, GroupID = @GroupID, Status = @Status, ScheduleNow = @ScheduleNow, ScheduleDate = @ScheduleDate, ScheduleTime = @ScheduleTime WHERE PushNotificationID = @PushNotificationID";
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

        public async Task<ServiceResponse<List<Notification>>> GetNotificationReport(GetNotificationReportRequest request)
        {
            var countSql = "SELECT COUNT(*) FROM [tblPushNotificationMaster]";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"SELECT * FROM [tblPushNotificationMaster]
                        ORDER BY PushNotificationID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var notifications = await _connection.QueryAsync<Notification>(sql, parameters);
            return new ServiceResponse<List<Notification>>(true, "Records Found", notifications.ToList(), 302, totalCount);
        }
    }
}
