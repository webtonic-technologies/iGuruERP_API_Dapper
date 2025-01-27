using LibraryManagement_API.DTOs.Requests; // Ensure this using directive is present
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.Models;
using LibraryManagement_API.Repository.Interfaces;
using LibraryManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement_API.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateCategories(List<Category> requests)
        {
            return await _categoryRepository.AddUpdateCategories(requests);
        }

        public async Task<ServiceResponse<List<CategoryResponse>>> GetAllCategories(GetAllCategoriesRequest request)
        {
            return await _categoryRepository.GetAllCategories(request);
        }

        public async Task<ServiceResponse<Category>> GetCategoryById(int categoryId)
        {
            return await _categoryRepository.GetCategoryById(categoryId);
        }

        public async Task<ServiceResponse<bool>> DeleteCategory(int categoryId)
        {
            return await _categoryRepository.DeleteCategory(categoryId);
        }
    }
}
