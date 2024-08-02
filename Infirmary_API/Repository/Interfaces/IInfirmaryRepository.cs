using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infirmary_API.Repository.Interfaces
{
    public interface IInfirmaryRepository
    {
        Task<ServiceResponse<string>> AddUpdateInfirmary(AddUpdateInfirmaryRequest request);
        Task<ServiceResponse<List<InfirmaryResponse>>> GetAllInfirmary(GetAllInfirmaryRequest request);
        Task<ServiceResponse<Infirmary>> GetInfirmaryById(int id);
        Task<ServiceResponse<bool>> DeleteInfirmary(int id);
    }
}
