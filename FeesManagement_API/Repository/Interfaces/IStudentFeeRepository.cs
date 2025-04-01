using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Implementations;
using System.Collections.Generic;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface IStudentFeeRepository
    {
        List<StudentFeeResponse> GetStudentFees(StudentFeeRequest request);
        int DiscountStudentFees(DiscountStudentFeesRequest request);
        List<GetFeesChangeLogsResponse> GetFeesChangeLogs(GetFeesChangeLogsRequest request);
        Task<IEnumerable<StudentFeeRawData>> GetStudentFeeRawDataAsync(GetStudentFeesExportRequest request);
        Task<IEnumerable<GetFeesChangeLogsExportResponse>> GetFeesChangeLogsExportAsync(GetFeesChangeLogsExportRequest request);

    }
}
