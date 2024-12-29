using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.Responses;
using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models;
using Lesson_API.Repository.Interfaces;
using Lesson_API.Services.Interfaces;
using Lesson_API.DTOs.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson_API.Services.Implementations
{
    public class LessonPlanningService : ILessonPlanningService
    {
        private readonly ILessonPlanningRepository _lessonPlanningRepository;

        public LessonPlanningService(ILessonPlanningRepository lessonPlanningRepository)
        {
            _lessonPlanningRepository = lessonPlanningRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateLessonPlanning(LessonPlanningRequest request)
        {
            return await _lessonPlanningRepository.AddUpdateLessonPlanning(request);
        }

        public async Task<ServiceResponse<List<GetAllLessonPlanningResponse>>> GetAllLessonPlanning(GetAllLessonPlanningRequest request)
        {
            return await _lessonPlanningRepository.GetAllLessonPlanning(request);
        }
        public async Task<ServiceResponse<GetLessonPlanningResponse1>> GetLessonPlanning(GetLessonPlanningRequest request)
        {
            return await _lessonPlanningRepository.GetLessonPlanning(request);
        }

        public async Task<ServiceResponse<LessonPlanning>> GetLessonPlanningById(int id)
        {
            return await _lessonPlanningRepository.GetLessonPlanningById(id);
        }

        public async Task<ServiceResponse<bool>> DeleteLessonPlanning(int id)
        {
            return await _lessonPlanningRepository.DeleteLessonPlanning(id);
        }

        public async Task<ServiceResponse<List<GetLessonStatusResponse>>> GetLessonStatus()
        {
            return await _lessonPlanningRepository.GetLessonStatus();
        }
    }
}
