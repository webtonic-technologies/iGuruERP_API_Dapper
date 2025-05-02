using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Response;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Implementations;
using System.Collections.Generic;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface IStudentPreviousFeeRepository
    {
        List<StudentPreviousFeeResponse> GetStudentPreviousFees(StudentPreviousFeeRequest request); 
        public IEnumerable<int> DiscountStudentPreviousFees(IEnumerable<DiscountStudentPreviousFeesRequest> requests);

        List<GetPreviousFeesChangeLogsResponse> GetPreviousFeesChangeLogs(GetPreviousFeesChangeLogsRequest request);
        Task<IEnumerable<StudentPreviousFeeRawData>> GetStudentPreviousFeeRawDataAsync(GetStudentPreviousFeesExportRequest request);
        Task<IEnumerable<GetPreviousFeesChangeLogsExportResponse>> GetPreviousFeesChangeLogsExportAsync(GetPreviousFeesChangeLogsExportRequest request);

    }
}
