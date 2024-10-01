using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses;
using TimeTable_API.Repository.Interfaces;
using TimeTable_API.Services.Interfaces; 

namespace TimeTable_API.Services.Implementations
{
    public class DaySetupService : IDaySetupService
    {
        private readonly IDaySetupRepository _daySetupRepository;

        public DaySetupService(IDaySetupRepository daySetupRepository)
        {
            _daySetupRepository = daySetupRepository;
        }

        // Get all DaySetups

        public async Task<ServiceResponse<List<DaySetupResponse>>> GetAllDaySetups(GetAllDaySetupsRequest request)
        {
            return await _daySetupRepository.GetAllDaySetups(request);
        }

        // Get DaySetup by ID
        public async Task<ServiceResponse<DaySetupResponse>> GetDaySetupById(int planId)
        {
            return await _daySetupRepository.GetDaySetupById(planId);
        }

        // Add or update DaySetup
        public async Task<ServiceResponse<int>> AddUpdatePlan(AddUpdatePlanRequest request)
        {
            return await _daySetupRepository.AddUpdatePlan(request);
        }

        // Delete DaySetup
        public async Task<ServiceResponse<bool>> DeleteDaySetup(int planId)
        {
            return await _daySetupRepository.DeleteDaySetup(planId);
        }
    }
}
