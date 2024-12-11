using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface IStudentInformationRepository
    {
        ServiceResponse<List<StudentInformationResponse>> GetStudentInformation(StudentInformationRequest request);
    }
}
