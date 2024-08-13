
using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Model;
using Lesson_API.Repository.Interfaces;
using Lesson_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson_API.Services.Implementations
{
    public class PlanTypeService : IPlanTypeService
    {
        private readonly IPlanTypeRepository _planTypeRepository;

        public PlanTypeService(IPlanTypeRepository planTypeRepository)
        {
            _planTypeRepository = planTypeRepository;
        }

        public async Task<ServiceResponse<List<PlanType>>> GetAllPlanTypesAsync()
        {
            var planTypes = await _planTypeRepository.GetAllPlanTypesAsync();
            return new ServiceResponse<List<PlanType>>(planTypes, true, "Plan Types retrieved successfully.", 200, planTypes.Count);
        }
    }
}
