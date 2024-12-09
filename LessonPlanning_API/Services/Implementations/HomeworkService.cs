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
    public class HomeworkService : IHomeworkService
    {
        private readonly IHomeworkRepository _homeworkRepository;

        public HomeworkService(IHomeworkRepository homeworkRepository)
        {
            _homeworkRepository = homeworkRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateHomework(HomeworkRequest request)
        {
            return await _homeworkRepository.AddUpdateHomework(request);
        }

        public async Task<ServiceResponse<List<GetAllHomeworkResponse>>> GetAllHomework(GetAllHomeworkRequest request)
        {
            return await _homeworkRepository.GetAllHomework(request);
        }

        public async Task<ServiceResponse<Homework>> GetHomeworkById(int id)
        {
            return await _homeworkRepository.GetHomeworkById(id);
        }

        public async Task<ServiceResponse<bool>> DeleteHomework(int id)
        {
            return await _homeworkRepository.DeleteHomework(id);
        }
        public async Task<ServiceResponse<GetHomeworkHistoryResponse>> GetHomeworkHistory(GetHomeworkHistoryRequest request)
        {
            return await _homeworkRepository.GetHomeworkHistory(request);
        }
    }
}
