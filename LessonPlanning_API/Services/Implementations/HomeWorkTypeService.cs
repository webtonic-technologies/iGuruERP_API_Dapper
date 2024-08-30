using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models;
using Lesson_API.Repository.Interfaces;
using Lesson_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson_API.Services.Implementations
{
    public class HomeWorkTypeService : IHomeWorkTypeService
    {
        private readonly IHomeWorkTypeRepository _repository;

        public HomeWorkTypeService(IHomeWorkTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<List<HomeWorkTypeModel>>> GetAllHomeWorkTypes()
        {
            var homeworkTypes = await _repository.GetAllHomeWorkTypes();
            if (homeworkTypes != null && homeworkTypes.Count > 0)
            {
                return new ServiceResponse<List<HomeWorkTypeModel>>(
                    homeworkTypes,
                    true,
                    "Homework Types retrieved successfully.",
                    200,
                    homeworkTypes.Count
                );
            }
            return new ServiceResponse<List<HomeWorkTypeModel>>(
                null,
                false,
                "No Homework Types found.",
                404,
                null
            );
        }
    }
}
