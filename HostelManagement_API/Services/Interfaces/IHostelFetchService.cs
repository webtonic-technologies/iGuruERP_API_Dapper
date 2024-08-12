using HostelManagement_API.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Interfaces
{
    public interface IHostelFetchService
    {
        Task<IEnumerable<HostelFetchResponse>> GetAllHostels(int instituteId);
    }
}
