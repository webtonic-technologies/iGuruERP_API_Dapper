using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models;
using Lesson_API.Repository.Interfaces;
using Lesson_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson_API.Services.Implementations
{
    public class AssignmentTypeService : IAssignmentTypeService
    {
        private readonly IAssignmentTypeRepository _repository;

        public AssignmentTypeService(IAssignmentTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<List<AssignmentTypeModel>>> GetAllAssignmentTypes()
        {
            var assignmentTypes = await _repository.GetAllAssignmentTypes();
            if (assignmentTypes != null && assignmentTypes.Count > 0)
            {
                return new ServiceResponse<List<AssignmentTypeModel>>(
                    assignmentTypes,
                    true,
                    "Assignment Types retrieved successfully.",
                    200,
                    assignmentTypes.Count
                );
            }
            return new ServiceResponse<List<AssignmentTypeModel>>(
                null,
                false,
                "No Assignment Types found.",
                404,
                null
            );
        }
    }
}
