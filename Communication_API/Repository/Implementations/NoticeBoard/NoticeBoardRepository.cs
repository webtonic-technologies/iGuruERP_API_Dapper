using Communication_API.DTOs.Requests.NoticeBoard;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.NoticeBoard;
using Communication_API.Repository.Interfaces.NoticeBoard;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Communication_API.Repository.Implementations.NoticeBoard
{
    public class NoticeBoardRepository : INoticeBoardRepository
    {
        private readonly IDbConnection _connection;

        public NoticeBoardRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateNotice(AddUpdateNoticeRequest request)
        {
            var query = request.NoticeID == 0
                ? "INSERT INTO [tblNotice] (Title, Description, Attachments, StartDate, EndDate, IsStudent, IsEmployee, ScheduleNow, ScheduleDate, ScheduleTime) VALUES (@Title, @Description, @Attachments, @StartDate, @EndDate, @IsStudent, @IsEmployee, @ScheduleNow, @ScheduleDate, @ScheduleTime)"
                : "UPDATE [tblNotice] SET Title = @Title, Description = @Description, Attachments = @Attachments, StartDate = @StartDate, EndDate = @EndDate, IsStudent = @IsStudent, IsEmployee = @IsEmployee, ScheduleNow = @ScheduleNow, ScheduleDate = @ScheduleDate, ScheduleTime = @ScheduleTime WHERE NoticeID = @NoticeID";

            var parameters = new
            {
                request.NoticeID,
                request.Title,
                request.Description,
                request.Attachments,
                request.StartDate,
                request.EndDate,
                request.IsStudent,
                request.IsEmployee,
                request.ScheduleNow,
                request.ScheduleDate,
                request.ScheduleTime
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
        }

        public async Task<ServiceResponse<List<Notice>>> GetAllNotice(GetAllNoticeRequest request)
        {
            var countSql = "SELECT COUNT(*) FROM [tblNotice]";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"SELECT * FROM [tblNotice]
                        ORDER BY NoticeID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var notices = await _connection.QueryAsync<Notice>(sql, parameters);
            return new ServiceResponse<List<Notice>>(true, "Records Found", notices.ToList(), 302, totalCount);
        }

        public async Task<ServiceResponse<string>> AddUpdateCircular(AddUpdateCircularRequest request)
        {
            var query = request.CircularID == 0
                ? "INSERT INTO [tblCircular] (Title, Message, Attachment, CircularNo, CircularDate, PublishedDate, IsStudent, IsEmployee, ScheduleNow, ScheduleDate, ScheduleTime) VALUES (@Title, @Message, @Attachment, @CircularNo, @CircularDate, @PublishedDate, @IsStudent, @IsEmployee, @ScheduleNow, @ScheduleDate, @ScheduleTime)"
                : "UPDATE [tblCircular] SET Title = @Title, Message = @Message, Attachment = @Attachment, CircularNo = @CircularNo, CircularDate = @CircularDate, PublishedDate = @PublishedDate, IsStudent = @IsStudent, IsEmployee = @IsEmployee, ScheduleNow = @ScheduleNow, ScheduleDate = @ScheduleDate, ScheduleTime = @ScheduleTime WHERE CircularID = @CircularID";

            var parameters = new
            {
                request.CircularID,
                request.Title,
                request.Message,
                request.Attachment,
                request.CircularNo,
                request.CircularDate,
                request.PublishedDate,
                request.IsStudent,
                request.IsEmployee,
                request.ScheduleNow,
                request.ScheduleDate,
                request.ScheduleTime
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
        }

        public async Task<ServiceResponse<List<Circular>>> GetAllCircular(GetAllCircularRequest request)
        {
            var countSql = "SELECT COUNT(*) FROM [tblCircular]";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"SELECT * FROM [tblCircular]
                        ORDER BY CircularID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var circulars = await _connection.QueryAsync<Circular>(sql, parameters);
            return new ServiceResponse<List<Circular>>(true, "Records Found", circulars.ToList(), 302, totalCount);
        }
    }
}
