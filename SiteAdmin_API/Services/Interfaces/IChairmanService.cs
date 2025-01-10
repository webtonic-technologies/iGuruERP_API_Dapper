using System.Threading.Tasks;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Responses;
using SiteAdmin_API.DTOs.ServiceResponse;

namespace SiteAdmin_API.Services.Interfaces
{
    public interface IChairmanService
    {
        Task<ServiceResponse<string>> AddUpdateChairman(AddUpdateChairmanRequest request);
        Task<ServiceResponse<IEnumerable<GetAllChairmanResponse>>> GetAllChairman();
        Task<ServiceResponse<string>> DeleteChairman(DeleteChairmanRequest request);
        Task<ServiceResponse<List<GetInstitutesDDLResponse>>> GetInstitutesDDL();

    }
}
