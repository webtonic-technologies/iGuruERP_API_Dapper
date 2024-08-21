using Dapper;
using System.Data;
using System.Threading.Tasks;
using ProductUpdates_API.DTOs.Requests;
using ProductUpdates_API.DTOs.Response;
using ProductUpdates_API.DTOs.ServiceResponse;
using ProductUpdates_API.Repository.Interfaces;

namespace ProductUpdates_API.Repository.Implementations
{
    public class ProductUpdatePostRepository : IProductUpdatePostRepository
    {
        private readonly IDbConnection _connection;

        public ProductUpdatePostRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<int>> CreatePost(CreatePostRequest request)
        {
            var query = @"INSERT INTO tblProductUpdatesPost (Heading, CreatedBy, PostDate, ModuleID, Description, IsActive) 
                          VALUES (@Heading, @CreatedBy, @PostDate, @ModuleID, @Description, '1');
                          SELECT CAST(SCOPE_IDENTITY() as int)";

            var postId = await _connection.ExecuteScalarAsync<int>(query, new
            {
                request.Heading,
                request.CreatedBy,
                request.PostDate,
                request.ModuleID,
                request.Description
            });

            return new ServiceResponse<int>(true, "Post created successfully", postId, 201);
        }

        public async Task<ServiceResponse<List<PostResponse>>> GetAllPosts(int moduleId)
        {
            var query = @"SELECT * FROM tblProductUpdatesPost WHERE ModuleID = @ModuleID AND IsActive = '1'";

            var posts = (await _connection.QueryAsync<PostResponse>(query, new { ModuleID = moduleId })).ToList();

            return new ServiceResponse<List<PostResponse>>(true, "Posts retrieved successfully", posts, 200);
        }
    }
}
