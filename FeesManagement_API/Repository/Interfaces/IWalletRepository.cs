using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;

using System.Data;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface IWalletRepository
    { 
        Task<int> AddWalletAmount(AddWalletAmountRequest request); 
        ServiceResponse<List<GetWalletResponse>> GetWallet(GetWalletRequest request);
        DataTable GetWalletExportData(GetWalletExportRequest request); // Add this
        ServiceResponse<GetWalletHistoryResponse> GetWalletHistory(GetWalletHistoryRequest request);
        DataTable GetWalletHistoryExportData(GetWalletHistoryExportRequest request);

    }
}
