using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;
using System.Collections.Generic;

namespace FeesManagement_API.Services.Implementations
{
    public class StudentFeeService : IStudentFeeService
    {
        private readonly IStudentFeeRepository _studentFeeRepository;

        public StudentFeeService(IStudentFeeRepository studentFeeRepository)
        {
            _studentFeeRepository = studentFeeRepository;
        }

        public ServiceResponse<List<StudentFeeResponse>> GetStudentFees(StudentFeeRequest request)
        {
            var studentFees = _studentFeeRepository.GetStudentFees(request);
            var response = new ServiceResponse<List<StudentFeeResponse>>(
                success: true,
                message: "Student fees retrieved successfully",
                data: studentFees,
                statusCode: 200,
                totalCount: studentFees?.Count
            );

            return response;
        }
    }
}
