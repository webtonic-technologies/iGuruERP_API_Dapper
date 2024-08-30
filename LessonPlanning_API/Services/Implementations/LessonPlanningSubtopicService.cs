using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models;
using Lesson_API.Repository.Interfaces;
using Lesson_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson_API.Services.Implementations
{
    public class LessonPlanningSubtopicService : ILessonPlanningSubtopicService
    {
        private readonly ILessonPlanningSubtopicRepository _repository;

        public LessonPlanningSubtopicService(ILessonPlanningSubtopicRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<List<LessonPlanningSubtopicModel>>> GetSubtopicsByInstituteId(int instituteID)
        {
            var subtopics = await _repository.GetSubtopicsByInstituteId(instituteID);
            if (subtopics != null && subtopics.Count > 0)
            {
                return new ServiceResponse<List<LessonPlanningSubtopicModel>>(
                    subtopics,
                    true,
                    "Curriculum Subtopics retrieved successfully.",
                    200,
                    subtopics.Count
                );
            }
            return new ServiceResponse<List<LessonPlanningSubtopicModel>>(
                null,
                false,
                "No Curriculum Subtopics found.",
                404,
                null
            );
        }
    }
}
