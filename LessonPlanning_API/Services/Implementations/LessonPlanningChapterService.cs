using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models;
using Lesson_API.Repository.Interfaces;
using Lesson_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson_API.Services.Implementations
{
    public class LessonPlanningChapterService : ILessonPlanningChapterService
    {
        private readonly ILessonPlanningChapterRepository _repository;

        public LessonPlanningChapterService(ILessonPlanningChapterRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<List<LessonPlanningChapterModel>>> GetChaptersByInstituteId(int instituteID)
        {
            var chapters = await _repository.GetChaptersByInstituteId(instituteID);
            if (chapters != null && chapters.Count > 0)
            {
                return new ServiceResponse<List<LessonPlanningChapterModel>>(
                    chapters,
                    true,
                    "Curriculum Chapters retrieved successfully.",
                    200, // StatusCode
                    chapters.Count  // TotalCount
                );
            }
            return new ServiceResponse<List<LessonPlanningChapterModel>>(
                null,
                false,
                "No Curriculum Chapters found.",
                404,  // StatusCode
                null  // TotalCount
            );
        }
    }
}
