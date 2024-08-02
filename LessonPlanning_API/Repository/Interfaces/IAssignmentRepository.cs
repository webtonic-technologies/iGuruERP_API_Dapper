using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.Responses;
using Lesson_API.DTOs.ServiceResponse;
using Lesson_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lesson_API.Repository.Interfaces
{
    public interface IAssignmentRepository
    {
        Task<ServiceResponse<string>> AddUpdateAssignment(AssignmentRequest request);
        Task<ServiceResponse<List<GetAllAssignmentsResponse>>> GetAllAssignments(GetAllAssignmentsRequest request);
        Task<ServiceResponse<Assignment>> GetAssignmentById(int id);
        Task<ServiceResponse<bool>> DeleteAssignment(int id);
    }
}
