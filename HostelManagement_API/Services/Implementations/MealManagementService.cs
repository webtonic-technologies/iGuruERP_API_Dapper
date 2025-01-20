using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using HostelManagement_API.Repository.Interfaces;
using HostelManagement_API.Services.Interfaces;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Implementations
{
    public class MealManagementService : IMealManagementService
    {
        private readonly IMealManagementRepository _mealManagementRepository;

        public MealManagementService(IMealManagementRepository mealManagementRepository)
        {
            _mealManagementRepository = mealManagementRepository;
        }

        public async Task<ServiceResponse<string>> AddMealType(AddMealTypeRequest request)
        {
            return await _mealManagementRepository.AddMealType(request);
        }

        public async Task<ServiceResponse<GetMealTypeResponse>> GetMealType(GetMealTypeRequest request)
        {
            return await _mealManagementRepository.GetMealType(request);
        }

        public async Task<ServiceResponse<string>> DeleteMealType(DeleteMealTypeRequest request)
        {
            return await _mealManagementRepository.DeleteMealType(request);
        }

        public async Task<ServiceResponse<IEnumerable<GetMealPlannerResponse>>> GetMealPlanner(GetMealPlannerRequest request)
        {
            return await _mealManagementRepository.GetMealPlanner(request);
        }
        public async Task<ServiceResponse<string>> SetMealPlanner(SetMealPlannerRequest request)
        {
            var result = await _mealManagementRepository.SetMealPlanner(request);
            return result;
        }

        public async Task<ServiceResponse<GetDailyMealMenuResponse>> GetDailyMealMenu(GetDailyMealMenuRequest request)
        {
            var mealMenu = await _mealManagementRepository.GetDailyMealMenu(request);
            return new ServiceResponse<GetDailyMealMenuResponse>(
                success: true,
                message: "Daily Meal Menu Retrieved Successfully",
                data: mealMenu,
                statusCode: 200
            );
        }


    }
}
