using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeesManagement_API.Services.Interfaces
{
    public interface INumberSchemeService
    {
        Task<ServiceResponse<int>> AddUpdateNumberScheme(AddUpdateNumberSchemeRequest request);
        Task<ServiceResponse<IEnumerable<NumberSchemeResponse>>> GetAllNumberSchemes(GetAllNumberSchemesRequest request);
        Task<ServiceResponse<NumberSchemeResponse>> GetNumberSchemeById(int numberSchemeID); 
        Task<ServiceResponse<int>> UpdateNumberSchemeStatus(int numberSchemeID);  // Add this method
    }
}
