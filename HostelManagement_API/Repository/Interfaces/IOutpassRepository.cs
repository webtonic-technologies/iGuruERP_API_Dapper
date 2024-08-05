using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Interfaces
{
    public interface IOutpassRepository
    {
        Task<int> AddUpdateOutpass(AddUpdateOutpassRequest request);
        Task<PagedResponse<OutpassResponse>> GetAllOutpass(GetAllOutpassRequest request);
        Task<OutpassResponse> GetOutpassById(int outpassId);
        Task<int> DeleteOutpass(int outpassId);
    }
}
