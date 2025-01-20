using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Interfaces
{
    public interface IHostelRepository
    {
        Task<int> AddUpdateHostel(AddUpdateHostelRequest request);
        Task<ServiceResponse<IEnumerable<HostelResponse>>> GetAllHostels(GetAllHostelsRequest request);
        Task<HostelResponse> GetHostelById(int hostelId);
        Task<int> DeleteHostel(int hostelId);
    }
}
