using Lesson_API.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson_API.Repository.Interfaces
{
    public interface IPlanTypeRepository
    {
        Task<List<PlanType>> GetAllPlanTypesAsync();
    }
}