using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;

namespace StudentManagement_API.Repository.Interfaces
{
    public interface IApprovalsRepository
    {
        Task<int> CreatePermissionSlipAsync(CreatePermissionSlipRequest request);
        Task<IEnumerable<GetPermissionSlipResponse>> GetPermissionSlipAsync(GetPermissionSlipRequest request);
        Task<bool> ChangePermissionSlipStatusAsync(ChangePermissionSlipStatusRequest request);
        Task<IEnumerable<GetApprovedHistoryResponse>> GetApprovedHistoryAsync(GetApprovedHistoryRequest request);
        Task<IEnumerable<GetRejectedHistoryResponse>> GetRejectedHistoryAsync(GetRejectedHistoryRequest request);
        Task<IEnumerable<GetPermissionSlipExportResponse>> GetPermissionSlipExportAsync(GetPermissionSlipExportRequest request);
        Task<IEnumerable<GetApprovedHistoryExportResponse>> GetApprovedHistoryExportAsync(GetApprovedHistoryExportRequest request);
        Task<IEnumerable<GetRejectedHistoryExportResponse>> GetRejectedHistoryExportAsync(GetRejectedHistoryExportRequest request);

    }
}
