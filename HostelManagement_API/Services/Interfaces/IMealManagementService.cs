using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Interfaces
{
    public interface IMealManagementService
    {
        Task<ServiceResponse<string>> AddMealType(AddMealTypeRequest request);
        Task<ServiceResponse<GetMealTypeResponse>> GetMealType(GetMealTypeRequest request);
        Task<ServiceResponse<string>> DeleteMealType(DeleteMealTypeRequest request);
        Task<ServiceResponse<IEnumerable<GetMealPlannerResponse>>> GetMealPlanner(GetMealPlannerRequest request);
        Task<ServiceResponse<string>> SetMealPlanner(SetMealPlannerRequest request);
        Task<ServiceResponse<GetDailyMealMenuResponse>> GetDailyMealMenu(GetDailyMealMenuRequest request);


    }

}


 
