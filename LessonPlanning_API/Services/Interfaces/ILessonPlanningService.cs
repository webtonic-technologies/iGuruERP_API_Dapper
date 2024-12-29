using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.Responses;
using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models;
using Lesson_API.DTOs.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson_API.Services.Interfaces
{
    public interface ILessonPlanningService
    {
        Task<ServiceResponse<string>> AddUpdateLessonPlanning(LessonPlanningRequest request);
        Task<ServiceResponse<List<GetAllLessonPlanningResponse>>> GetAllLessonPlanning(GetAllLessonPlanningRequest request);
        Task<ServiceResponse<GetLessonPlanningResponse1>> GetLessonPlanning(GetLessonPlanningRequest request);

        Task<ServiceResponse<LessonPlanning>> GetLessonPlanningById(int id);
        Task<ServiceResponse<bool>> DeleteLessonPlanning(int id);
        Task<ServiceResponse<List<GetLessonStatusResponse>>> GetLessonStatus();

    }
}
