using Admission_API.DTOs.Requests;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admission_API.Repository.Interfaces
{
    public interface INumberSchemeRepository
    {
        Task<ServiceResponse<string>> AddUpdateNumberScheme(NumberScheme request);
        Task<ServiceResponse<List<NumberSchemeResponse>>> GetAllNumberSchemes(GetAllRequest request);
        Task<ServiceResponse<NumberSchemeResponse>> GetNumberSchemeById(int schemeID);
        Task<ServiceResponse<bool>> DeleteNumberScheme(int schemeID);
    }
}
