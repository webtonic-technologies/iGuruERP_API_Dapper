using Lesson_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson_API.Repository.Interfaces
{
    public interface IChapterSubtopicListRepository
    {
        Task<List<ChapterSubtopicList>> GetChapterSubtopics(int curriculumID, int instituteID);
    }
}
