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
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _assignmentRepository;

        public AssignmentService(IAssignmentRepository assignmentRepository)
        {
            _assignmentRepository = assignmentRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateAssignment(AssignmentRequest request)
        {
            return await _assignmentRepository.AddUpdateAssignment(request);
        }

        public async Task<ServiceResponse<List<GetAllAssignmentsResponse>>> GetAllAssignments(GetAllAssignmentsRequest request)
        {
            return await _assignmentRepository.GetAllAssignments(request);
        }

        public async Task<ServiceResponse<Assignment>> GetAssignmentById(int id)
        {
            return await _assignmentRepository.GetAssignmentById(id);
        }

        public async Task<ServiceResponse<bool>> DeleteAssignment(int id)
        {
            return await _assignmentRepository.DeleteAssignment(id);
        }

        public async Task<ServiceResponse<byte[]>> DownloadDocument(int documentId)
        {
            return await _assignmentRepository.DownloadDocument(documentId);
        }
    }
}
