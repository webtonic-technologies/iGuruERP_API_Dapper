using Communication_API.DTOs.ServiceResponse;

using Communication_API.DTOs.Requests.DiscussionBoard;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.DiscussionBoard;
using Communication_API.Repository.Interfaces.DiscussionBoard;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Communication_API.Repository.Implementations.DiscussionBoard
{
    public class DiscussionBoardRepository : IDiscussionBoardRepository
    {
        private readonly IDbConnection _connection;

        public DiscussionBoardRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateDiscussion(AddUpdateDiscussionRequest request)
        {
            var query = request.DiscussionBoardID == 0
                ? "INSERT INTO [tblDiscussionBoard] (DiscussionHeading, Description, Attachments, IsStudent, IsEmployee) VALUES (@DiscussionHeading, @Description, @Attachments, @IsStudent, @IsEmployee)"
                : "UPDATE [tblDiscussionBoard] SET DiscussionHeading = @DiscussionHeading, Description = @Description, Attachments = @Attachments, IsStudent = @IsStudent, IsEmployee = @IsEmployee WHERE DiscussionBoardID = @DiscussionBoardID";

            var parameters = new
            {
                request.DiscussionBoardID,
                request.DiscussionHeading,
                request.Description,
                request.Attachments,
                request.IsStudent,
                request.IsEmployee
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
        }

        public async Task<ServiceResponse<List<Communication_API.Models.DiscussionBoard.DiscussionBoard>>> GetAllDiscussion(GetAllDiscussionRequest request)
        {
            var countSql = "SELECT COUNT(*) FROM [tblDiscussionBoard]";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"SELECT * FROM [tblDiscussionBoard]
                        ORDER BY DiscussionBoardID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var discussions = await _connection.QueryAsync<Communication_API.Models.DiscussionBoard.DiscussionBoard>(sql, parameters);
            return new ServiceResponse<List<Communication_API.Models.DiscussionBoard.DiscussionBoard>>(true, "Records Found", discussions.ToList(), 302, totalCount);
        }

        public async Task<ServiceResponse<string>> DeleteDiscussion(int DiscussionBoardID)
        {
            var query = "DELETE FROM [tblDiscussionBoard] WHERE DiscussionBoardID = @DiscussionBoardID";
            var result = await _connection.ExecuteAsync(query, new { DiscussionBoardID });
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 200 : 400);
        }

        public async Task<ServiceResponse<Communication_API.Models.DiscussionBoard.DiscussionBoard>> GetDiscussionBoard(int DiscussionBoardID)
        {
            var query = "SELECT * FROM [tblDiscussionBoard] WHERE DiscussionBoardID = @DiscussionBoardID";
            var discussion = await _connection.QueryFirstOrDefaultAsync<Communication_API.Models.DiscussionBoard.DiscussionBoard>(query, new { DiscussionBoardID });
            return new ServiceResponse<Communication_API.Models.DiscussionBoard.DiscussionBoard>(discussion != null, discussion != null ? "Operation Successful" : "No Record Found", discussion, discussion != null ? 200 : 404);
        }

        public async Task<ServiceResponse<string>> CreateDiscussionThread(CreateDiscussionThreadRequest request)
        {
            var query = "INSERT INTO [tblDiscussionBoardComment] (DiscussionBoardID, UserID, Comments, CommentDate) VALUES (@DiscussionBoardID, @UserID, @Comments, @CommentDate)";

            var parameters = new
            {
                request.DiscussionBoardID,
                request.UserID,
                request.Comments,
                CommentDate = DateTime.Now
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
        }

        public async Task<ServiceResponse<List<DiscussionThread>>> GetDiscussionThread(int DiscussionBoardID)
        {
            var query = "SELECT * FROM [tblDiscussionBoardComment] WHERE DiscussionBoardID = @DiscussionBoardID";
            var comments = await _connection.QueryAsync<DiscussionThread>(query, new { DiscussionBoardID });
            return new ServiceResponse<List<DiscussionThread>>(true, "Records Found", comments.ToList(), 302);
        }
    }
}
