using System.Threading.Tasks;
using ProductUpdates_API.DTOs.Requests;
using ProductUpdates_API.DTOs.Response;
using ProductUpdates_API.DTOs.ServiceResponse;
using ProductUpdates_API.Repository.Interfaces;
using ProductUpdates_API.Services.Interfaces;

namespace ProductUpdates_API.Services.Implementations
{
    public class ProductUpdatePostService : IProductUpdatePostService
    {
        private readonly IProductUpdatePostRepository _repository;

        public ProductUpdatePostService(IProductUpdatePostRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<int>> CreatePost(CreatePostRequest request)
        {
            return await _repository.CreatePost(request);
        }

        public async Task<ServiceResponse<List<PostResponse>>> GetAllPosts(int moduleId)
        {
            return await _repository.GetAllPosts(moduleId);
        }
    }
}
