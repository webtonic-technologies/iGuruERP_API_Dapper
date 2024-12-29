using Lesson_API.DTOs.Responses;

namespace Lesson_API.Repository.Interfaces
{
    public interface ITeacherTrackingRepository
    {
        Task<IEnumerable<GetTeacherTrackingResponse>> GetTeacherTrackingAsync(int instituteID, int employeeID);
        Task<GetTeacherClassSectionSubjectResponse> GetTeacherClassSectionSubjectAsync(int employeeID);
        Task<IEnumerable<GetChaptersResponse>> GetChaptersAsync(int classID, int subjectID, int instituteID);
        Task<GetTeacherInfoResponse> GetTeacherInfoAsync(int employeeID);

    }
}
