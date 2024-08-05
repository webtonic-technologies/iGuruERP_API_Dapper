using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infirmary_API.Services.Interfaces
{
    public interface IInfirmaryVisitService
    {
        Task<ServiceResponse<string>> AddUpdateInfirmaryVisit(AddUpdateInfirmaryVisitRequest request);
        Task<ServiceResponse<List<InfirmaryVisitResponse>>> GetAllInfirmaryVisits(GetAllInfirmaryVisitsRequest request);
        Task<ServiceResponse<InfirmaryVisit>> GetInfirmaryVisitById(int id);
        Task<ServiceResponse<bool>> DeleteInfirmaryVisit(int id);
    }
}
