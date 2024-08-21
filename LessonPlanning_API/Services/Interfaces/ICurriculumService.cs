using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.Responses;
using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson_API.Services.Interfaces
{
    public interface ICurriculumService
    {
        Task<ServiceResponse<string>> AddUpdateCurriculum(CurriculumRequest request);
        Task<ServiceResponse<List<GetAllCurriculumResponse>>> GetAllCurriculum(GetAllCurriculumRequest request);
        Task<ServiceResponse<Curriculum>> GetCurriculumById(int id);
        Task<ServiceResponse<bool>> DeleteCurriculum(int id);
    }
}
