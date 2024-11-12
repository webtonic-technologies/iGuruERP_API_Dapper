using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeesManagement_API.Services.Interfaces
{
    public interface IConcessionService
    {
        Task<ServiceResponse<int>> AddUpdateConcession(AddUpdateConcessionRequest request);
        //Task<ServiceResponse<IEnumerable<ConcessionResponse>>> GetAllConcessions(GetAllConcessionRequest request);
        Task<ServiceResponse<IEnumerable<ConcessionResponse>>> GetAllConcessions(GetAllConcessionRequest request);

        Task<ServiceResponse<ConcessionResponse>> GetConcessionById(int concessionGroupID);
        Task<int> UpdateConcessionGroupStatus(int concessionGroupID);
    }
}
