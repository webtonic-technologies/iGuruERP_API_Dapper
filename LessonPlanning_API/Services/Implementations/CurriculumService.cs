using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.Responses;
using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models;
using Lesson_API.Repository.Interfaces;
using Lesson_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson_API.Services.Implementations
{
    public class CurriculumService : ICurriculumService
    {
        private readonly ICurriculumRepository _curriculumRepository;

        public CurriculumService(ICurriculumRepository curriculumRepository)
        {
            _curriculumRepository = curriculumRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateCurriculum(CurriculumRequest request)
        {
            return await _curriculumRepository.AddUpdateCurriculum(request);
        }

        public async Task<ServiceResponse<List<GetAllCurriculumResponse>>> GetAllCurriculum(GetAllCurriculumRequest request)
        {
            return await _curriculumRepository.GetAllCurriculum(request);
        }

        public async Task<ServiceResponse<Curriculum>> GetCurriculumById(int id)
        {
            return await _curriculumRepository.GetCurriculumById(id);
        }

        public async Task<ServiceResponse<bool>> DeleteCurriculum(int id)
        {
            return await _curriculumRepository.DeleteCurriculum(id);
        }
    }
}
