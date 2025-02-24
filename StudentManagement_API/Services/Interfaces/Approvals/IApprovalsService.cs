using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;

namespace StudentManagement_API.Services.Interfaces
{
    public interface IApprovalsService
    {
        Task<ServiceResponse<int>> CreatePermissionSlipAsync(CreatePermissionSlipRequest request);
        Task<ServiceResponse<IEnumerable<GetPermissionSlipResponse>>> GetPermissionSlipAsync(GetPermissionSlipRequest request);
        Task<ServiceResponse<bool>> ChangePermissionSlipStatusAsync(ChangePermissionSlipStatusRequest request);
        Task<ServiceResponse<IEnumerable<GetApprovedHistoryResponse>>> GetApprovedHistoryAsync(GetApprovedHistoryRequest request);
        Task<ServiceResponse<IEnumerable<GetRejectedHistoryResponse>>> GetRejectedHistoryAsync(GetRejectedHistoryRequest request);
        Task<ServiceResponse<string>> GetPermissionSlipExportAsync(GetPermissionSlipExportRequest request);
        Task<ServiceResponse<string>> GetApprovedHistoryExportAsync(GetApprovedHistoryExportRequest request);
        Task<ServiceResponse<string>> GetRejectedHistoryExportAsync(GetRejectedHistoryExportRequest request);

    }
}
