using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses; 

namespace TimeTable_API.Services.Interfaces
{
    public interface IDaySetupService
    {
        Task<ServiceResponse<List<DaySetupResponse>>> GetAllDaySetups(GetAllDaySetupsRequest request);
        Task<ServiceResponse<DaySetupResponse>> GetDaySetupById(int planId);
        Task<ServiceResponse<int>> AddUpdatePlan(AddUpdatePlanRequest request);
        Task<ServiceResponse<bool>> DeleteDaySetup(int planId);
    }
}
