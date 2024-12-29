using Lesson_API.DTOs.Responses;
using Lesson_API.Repository.Interfaces;
using Lesson_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lesson_API.Services.Interfaces;

namespace Lesson_API.Services.Implementations
{
    public class TeacherTrackingService : ITeacherTrackingService
    {
        private readonly ITeacherTrackingRepository _repository;

        public TeacherTrackingService(ITeacherTrackingRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<IEnumerable<GetTeacherTrackingResponse>>> GetTeacherTrackingAsync(int instituteID, int employeeID)
        {
            var teacherTrackingData = await _repository.GetTeacherTrackingAsync(instituteID, employeeID);
            var response = new ServiceResponse<IEnumerable<GetTeacherTrackingResponse>>(teacherTrackingData, true, "Teacher tracking data fetched successfully", 200);
            return response;
        }

        public async Task<ServiceResponse<GetTeacherClassSectionSubjectResponse>> GetTeacherClassSectionSubjectAsync(int employeeID)
        {
            var teacherClassSectionSubjectData = await _repository.GetTeacherClassSectionSubjectAsync(employeeID);

            var response = new ServiceResponse<GetTeacherClassSectionSubjectResponse>(
                teacherClassSectionSubjectData,
                true,
                "Teacher class section and subject data fetched successfully",
                200
            );

            return response;
        }

        public async Task<ServiceResponse<IEnumerable<GetChaptersResponse>>> GetChaptersAsync(int classID, int subjectID, int instituteID)
        {
            var chaptersData = await _repository.GetChaptersAsync(classID, subjectID, instituteID);

            var response = new ServiceResponse<IEnumerable<GetChaptersResponse>>(
                chaptersData,
                true,
                "Chapters data fetched successfully",
                200
            );

            return response;
        }

        public async Task<ServiceResponse<GetTeacherInfoResponse>> GetTeacherInfoAsync(int employeeID)
        {
            var teacherInfo = await _repository.GetTeacherInfoAsync(employeeID);

            if (teacherInfo == null)
            {
                return new ServiceResponse<GetTeacherInfoResponse>(null, false, "Teacher not found", 404);
            }

            return new ServiceResponse<GetTeacherInfoResponse>(teacherInfo, true, "Teacher information fetched successfully", 200);
        }
    }
}
