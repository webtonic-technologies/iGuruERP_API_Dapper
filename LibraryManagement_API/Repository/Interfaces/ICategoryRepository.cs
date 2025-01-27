using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement_API.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        Task<ServiceResponse<List<CategoryResponse>>> GetAllCategories(GetAllCategoriesRequest request);
        Task<ServiceResponse<string>> AddUpdateCategories(List<Category> requests);
        Task<ServiceResponse<Category>> GetCategoryById(int categoryId);
        Task<ServiceResponse<bool>> DeleteCategory(int categoryId);
    }
}
 