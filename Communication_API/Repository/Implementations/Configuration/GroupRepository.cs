using Communication_API.DTOs.Requests.Configuration;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.Configuration;
using Communication_API.Repository.Interfaces.Configuration;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Communication_API.Repository.Implementations.Configuration
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IDbConnection _connection;

        public GroupRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateGroup(AddUpdateGroupRequest request)
        {
            var query = request.GroupID == 0
                ? "INSERT INTO [tblCommunicationGroup] (GroupName, AcadamicYearID, TypeID) VALUES (@GroupName, @AcadamicYearID, @TypeID)"
                : "UPDATE [tblCommunicationGroup] SET GroupName = @GroupName, AcadamicYearID = @AcadamicYearID, TypeID = @TypeID WHERE GroupID = @GroupID";

            var parameters = new
            {
                request.GroupID,
                request.GroupName,
                request.AcadamicYearID,
                request.TypeID
            };

            var result = await _connection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 201 : 400);
        }

        public async Task<ServiceResponse<List<Group>>> GetAllGroup(GetAllGroupRequest request)
        {
            var countSql = "SELECT COUNT(*) FROM [tblCommunicationGroup]";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countSql);

            var sql = @"SELECT * FROM [tblCommunicationGroup]
                        ORDER BY GroupID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            var groups = await _connection.QueryAsync<Group>(sql, parameters);
            return new ServiceResponse<List<Group>>(true, "Records Found", groups.ToList(), 302, totalCount);
        }

        public async Task<ServiceResponse<Group>> GetGroup(int GroupID)
        {
            var sql = "SELECT * FROM [tblCommunicationGroup] WHERE GroupID = @GroupID";
            var group = await _connection.QueryFirstOrDefaultAsync<Group>(sql, new { GroupID });
            return new ServiceResponse<Group>(true, "Record Found", group, 302);
        }

        public async Task<ServiceResponse<string>> DeleteGroup(int GroupID)
        {
            var sql = "DELETE FROM [tblCommunicationGroup] WHERE GroupID = @GroupID";
            var result = await _connection.ExecuteAsync(sql, new { GroupID });
            return new ServiceResponse<string>(true, "Operation Successful", result > 0 ? "Success" : "Failure", result > 0 ? 200 : 400);
        }
    }
}
