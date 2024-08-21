using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models;
using Lesson_API.Repository.Interfaces;
using Lesson_API.Services.Interfaces;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Lesson_API.Services.Implementations
{
    public class ChapterSubtopicListService : IChapterSubtopicListService
    {
        private readonly IChapterSubtopicListRepository _chapterSubtopicListRepository;

        public ChapterSubtopicListService(IChapterSubtopicListRepository chapterSubtopicListRepository)
        {
            _chapterSubtopicListRepository = chapterSubtopicListRepository;
        }

        public async Task<ServiceResponse<List<ChapterSubtopicList>>> GetChapterSubtopics(int curriculumID, int instituteID)
        {
            var chapterSubtopicList = await _chapterSubtopicListRepository.GetChapterSubtopics(curriculumID, instituteID);

            if (chapterSubtopicList == null)
            {
                return new ServiceResponse<List<ChapterSubtopicList>>(
                    data: null,
                    success: false,
                    message: "Data not found",
                    statusCode: (int)HttpStatusCode.NotFound,
                    totalCount: null
                );
            }

            return new ServiceResponse<List<ChapterSubtopicList>>(
                data: chapterSubtopicList,
                success: true,
                message: "Data retrieved successfully",
                statusCode: (int)HttpStatusCode.OK,
                totalCount: chapterSubtopicList.Count
            );
        }
    }
}
