using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;

namespace FeesManagement_API.Services.Interfaces
{
    public interface IStudentFeeService
    {
        ServiceResponse<List<StudentFeeResponse>> GetStudentFees(StudentFeeRequest request);
    }
}
