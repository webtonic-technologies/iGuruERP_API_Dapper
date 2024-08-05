using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.Responses;
using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson_API.Repository.Interfaces
{
    public interface ILessonPlanningRepository
    {
        Task<ServiceResponse<string>> AddUpdateLessonPlanning(LessonPlanningRequest request);
        Task<ServiceResponse<List<GetAllLessonPlanningResponse>>> GetAllLessonPlanning(GetAllLessonPlanningRequest request);
        Task<ServiceResponse<LessonPlanning>> GetLessonPlanningById(int id);
        Task<ServiceResponse<bool>> DeleteLessonPlanning(int id);
    }
}
