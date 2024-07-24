using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Interfaces
{
    public interface IHostelRepository
    {
        Task<int> AddUpdateHostel(AddUpdateHostelRequest request);
        Task<PagedResponse<HostelResponse>> GetAllHostels(GetAllHostelsRequest request);
        Task<HostelResponse> GetHostelById(int hostelId);
        Task<int> DeleteHostel(int hostelId);
    }
}
