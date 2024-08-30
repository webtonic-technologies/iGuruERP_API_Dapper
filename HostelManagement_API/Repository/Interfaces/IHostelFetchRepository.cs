using HostelManagement_API.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Interfaces
{
    public interface IHostelFetchRepository
    {
        Task<IEnumerable<HostelFetchResponse>> GetAllHostels(int instituteId);
    }
}
