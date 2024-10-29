using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;

namespace FeesManagement_API.Services.Interfaces
{
    public interface IStudentInformationService
    {
        ServiceResponse<StudentInformationResponse> GetStudentInformation(StudentInformationRequest request);
    }
}
