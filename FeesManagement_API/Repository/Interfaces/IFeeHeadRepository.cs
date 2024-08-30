using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface IFeeHeadRepository
    {
        Task<int> AddUpdateFeeHead(AddUpdateFeeHeadRequest request);
        Task<IEnumerable<FeeHeadResponse>> GetAllFeeHead(GetAllFeeHeadRequest request);
        Task<FeeHeadResponse> GetFeeHeadById(int feeHeadId);
        Task<int> DeleteFeeHead(int feeHeadId);
    }
}
