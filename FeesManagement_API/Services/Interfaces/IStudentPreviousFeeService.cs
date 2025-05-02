using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Response;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;

namespace FeesManagement_API.Services.Interfaces
{
    public interface IStudentPreviousFeeService
    {
        ServiceResponse<List<StudentPreviousFeeResponse>> GetStudentPreviousFees(StudentPreviousFeeRequest request);
        ServiceResponse<IList<DiscountStudentPreviousFeesResponse>> DiscountStudentPreviousFees(IEnumerable<DiscountStudentPreviousFeesRequest> requests);
        ServiceResponse<List<GetPreviousFeesChangeLogsResponse>> GetPreviousFeesChangeLogs(GetPreviousFeesChangeLogsRequest request);
        Task<byte[]> GetStudentPreviousFeesExportAsync(GetStudentPreviousFeesExportRequest request);
        Task<byte[]> GetPreviousFeesChangeLogsExportAsync(GetPreviousFeesChangeLogsExportRequest request);

    }
}
