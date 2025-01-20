using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using HostelManagement_API.Repository.Interfaces;
using HostelManagement_API.Services.Interfaces;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Implementations
{
    public class MealAttendanceService : IMealAttendanceService
    {
        private readonly IMealAttendanceRepository _mealAttendanceRepository;

        public MealAttendanceService(IMealAttendanceRepository mealAttendanceRepository)
        {
            _mealAttendanceRepository = mealAttendanceRepository;
        }

        public async Task<ServiceResponse<string>> SetMealAttendance(SetMealAttendanceRequest request)
        {
            return await _mealAttendanceRepository.SetMealAttendance(request);
        }

        public async Task<ServiceResponse<IEnumerable<GetMealAttendanceResponse>>> GetMealAttendance(GetMealAttendanceRequest request)
        {
            var result = await _mealAttendanceRepository.GetMealAttendance(request);
            return new ServiceResponse<IEnumerable<GetMealAttendanceResponse>>(true, "Meal attendance retrieved successfully", result, 200);
        }
    }
}
