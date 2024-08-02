using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Interfaces
{
    public interface IOutpassService
    {
        Task<ServiceResponse<int>> AddUpdateOutpass(AddUpdateOutpassRequest request);
        Task<ServiceResponse<PagedResponse<OutpassResponse>>> GetAllOutpass(GetAllOutpassRequest request);
        Task<ServiceResponse<OutpassResponse>> GetOutpassById(int outpassId);
        Task<ServiceResponse<bool>> DeleteOutpass(int outpassId);
    }
}
