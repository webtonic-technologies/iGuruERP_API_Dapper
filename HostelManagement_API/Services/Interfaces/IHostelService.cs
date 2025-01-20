using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Interfaces
{
    public interface IHostelService
    {
        Task<ServiceResponse<int>> AddUpdateHostel(AddUpdateHostelRequest request);
        Task<ServiceResponse<IEnumerable<HostelResponse>>> GetAllHostels(GetAllHostelsRequest request);
        Task<ServiceResponse<HostelResponse>> GetHostelById(int hostelId);
        Task<ServiceResponse<bool>> DeleteHostel(int hostelId);
    }
}
