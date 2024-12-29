using Lesson_API.DTOs.Requests;
using Lesson_API.DTOs.Responses;

namespace Lesson_API.Repository.Interfaces
{
    public interface IAssignmentReportRepository
    {
        Task<List<GetAssignmentsReportsResponse>> GetAssignmentsReports(GetAssignmentsReportsRequest request);
        Task<IEnumerable<GetAssignmentsReportsExportResponse>> GetAssignmentsReportsExport(GetAssignmentsReportsExportRequest request);

    }
}
