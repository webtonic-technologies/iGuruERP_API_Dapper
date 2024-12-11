using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;

namespace FeesManagement_API.Services.Interfaces
{
    public interface IStudentInformationService
    {
        ServiceResponse<List<StudentInformationResponse>> GetStudentInformation(StudentInformationRequest request);
    }
}
