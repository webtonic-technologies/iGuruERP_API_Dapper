using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.Responses;
using Lesson_API.DTOs.ServiceResponse;

namespace Lesson_API.Services.Interfaces
{
    public interface IAssignmentReportService
    {
        Task<ServiceResponse<List<GetAssignmentsReportsResponse>>> GetAssignmentsReports(GetAssignmentsReportsRequest request);
        Task<ServiceResponse<byte[]>> GetAssignmentsReportExport(GetAssignmentsReportsExportRequest request);

    }
}
