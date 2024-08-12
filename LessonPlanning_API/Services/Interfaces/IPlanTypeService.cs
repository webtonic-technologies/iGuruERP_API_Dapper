using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson_API.Services.Interfaces
{
    public interface IPlanTypeService
    {
        Task<ServiceResponse<List<PlanType>>> GetAllPlanTypesAsync();
    }
}
