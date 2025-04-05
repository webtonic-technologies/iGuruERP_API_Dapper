using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;


namespace FeesManagement_API.Services.Interfaces
{
    public interface IWalletService
    { 
        Task<ServiceResponse<int>> AddWalletAmount(AddWalletAmountRequest request);
        ServiceResponse<List<GetWalletResponse>> GetWallet(GetWalletRequest request);
        byte[] GetWalletExport(GetWalletExportRequest request); // Add this
        ServiceResponse<GetWalletHistoryResponse> GetWalletHistory(GetWalletHistoryRequest request);
        byte[] GetWalletHistoryExport(GetWalletHistoryExportRequest request);

    }
}
