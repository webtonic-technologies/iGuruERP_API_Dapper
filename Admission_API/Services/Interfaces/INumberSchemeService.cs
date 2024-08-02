using Admission_API.DTOs.Requests;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Services.Interfaces
{
    public interface INumberSchemeService
    {
        Task<ServiceResponse<string>> AddUpdateNumberScheme(NumberScheme request);
        Task<ServiceResponse<List<NumberScheme>>> GetAllNumberSchemes(GetAllRequest request);
        Task<ServiceResponse<NumberScheme>> GetNumberSchemeById(int schemeID);
        Task<ServiceResponse<bool>> DeleteNumberScheme(int schemeID);
    }
}
