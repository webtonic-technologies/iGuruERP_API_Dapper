using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson_API.Services.Interfaces
{
    public interface ILessonPlanningChapterService
    {
        Task<ServiceResponse<List<LessonPlanningChapterModel>>> GetChaptersByInstituteId(int instituteID);
    }
}
