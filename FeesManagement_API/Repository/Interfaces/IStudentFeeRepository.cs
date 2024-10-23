using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using System.Collections.Generic;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface IStudentFeeRepository
    {
        List<StudentFeeResponse> GetStudentFees(StudentFeeRequest request);
    }
}
