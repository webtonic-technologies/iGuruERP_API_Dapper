using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Response;
using FeesManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeesManagement_API.Services.Interfaces
{
    public interface IFeeHeadService
    {
        Task<ServiceResponse<int>> AddUpdateFeeHead(AddUpdateFeeHeadRequest request);
        Task<ServiceResponse<IEnumerable<FeeHeadResponse>>> GetAllFeeHead(GetAllFeeHeadRequest request);
        Task<ServiceResponse<IEnumerable<FeeHeadResponse>>> GetAllFeeHeadDDL(GetAllFeeHeadDDLRequest request); 
        Task<ServiceResponse<FeeHeadResponse>> GetFeeHeadById(int feeHeadId);
        Task<ServiceResponse<int>> DeleteFeeHead(int feeHeadId); 
        Task<ServiceResponse<IEnumerable<GetFeeHeadsDDLResponse>>> GetFeeHeadsDDL(GetFeeHeadsDDLRequest request);

    }
}
