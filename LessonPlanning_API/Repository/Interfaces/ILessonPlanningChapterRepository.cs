using Lesson_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson_API.Repository.Interfaces
{
    public interface ILessonPlanningChapterRepository
    {
        Task<List<LessonPlanningChapterModel>> GetChaptersByInstituteId(int instituteID);
    }
}
