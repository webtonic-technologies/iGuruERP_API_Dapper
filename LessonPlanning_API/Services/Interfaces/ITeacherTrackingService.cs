using Lesson_API.DTOs.Responses;
using Lesson_API.DTOs.ServiceResponse;

namespace Lesson_API.Services.Interfaces
{
    public interface ITeacherTrackingService
    {
        Task<ServiceResponse<IEnumerable<GetTeacherTrackingResponse>>> GetTeacherTrackingAsync(int instituteID, int employeeID);
        Task<ServiceResponse<GetTeacherClassSectionSubjectResponse>> GetTeacherClassSectionSubjectAsync(int employeeID);
        Task<ServiceResponse<IEnumerable<GetChaptersResponse>>> GetChaptersAsync(int classID, int subjectID, int instituteID);
        Task<ServiceResponse<GetTeacherInfoResponse>> GetTeacherInfoAsync(int employeeID);

    }
}
