using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;

namespace FeesManagement_API.Services.Interfaces
{
    public interface IStudentFeeService
    {
        ServiceResponse<List<StudentFeeResponse>> GetStudentFees(StudentFeeRequest request);
        ServiceResponse<DiscountStudentFeesResponse> DiscountStudentFees(DiscountStudentFeesRequest request);
        ServiceResponse<List<GetFeesChangeLogsResponse>> GetFeesChangeLogs(GetFeesChangeLogsRequest request);
        Task<byte[]> GetStudentFeesExportAsync(GetStudentFeesExportRequest request);
        Task<byte[]> GetFeesChangeLogsExportAsync(GetFeesChangeLogsExportRequest request);

    }
}
