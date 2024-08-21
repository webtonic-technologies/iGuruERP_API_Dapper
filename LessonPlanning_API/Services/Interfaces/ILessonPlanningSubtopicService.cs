using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson_API.Services.Interfaces
{
    public interface ILessonPlanningSubtopicService
    {
        Task<ServiceResponse<List<LessonPlanningSubtopicModel>>> GetSubtopicsByInstituteId(int instituteID);
    }
}
