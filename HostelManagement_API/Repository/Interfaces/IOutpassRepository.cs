using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Interfaces
{
    public interface IOutpassRepository
    {
        Task<int> AddUpdateOutpass(AddUpdateOutpassRequest request);
        Task<ServiceResponse<IEnumerable<OutpassResponse>>> GetAllOutpass(GetAllOutpassRequest request);
        Task<OutpassResponse> GetOutpassById(int outpassId);
        Task<int> DeleteOutpass(int outpassId);
    }
}
