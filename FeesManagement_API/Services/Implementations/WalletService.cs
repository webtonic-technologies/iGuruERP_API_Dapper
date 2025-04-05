using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse; 
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;
using FeesManagement_API.Utilities;

namespace FeesManagement_API.Services.Implementations
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;

        public WalletService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }
          
        public async Task<ServiceResponse<int>> AddWalletAmount(AddWalletAmountRequest request)
        {
            var rowsAffected = await _walletRepository.AddWalletAmount(request);
            if (rowsAffected > 0)
            {
                return new ServiceResponse<int>(true, "Wallet Amount is Credited Successfully", rowsAffected, 200);
            }
            return new ServiceResponse<int>(false, "Failed to credit wallet amount", rowsAffected, 400);
        }


        public ServiceResponse<List<GetWalletResponse>> GetWallet(GetWalletRequest request)
        {
            return _walletRepository.GetWallet(request);
        }

        public byte[] GetWalletExport(GetWalletExportRequest request)
        {
            var dataTable = _walletRepository.GetWalletExportData(request);

            return request.ExportType switch
            {
                1 => FileExportHelper.ExportToExcel(dataTable), // Excel
                2 => FileExportHelper.ExportToCsv(dataTable),   // CSV
                _ => throw new ArgumentException("Invalid ExportType")
            };
        }

        public ServiceResponse<GetWalletHistoryResponse> GetWalletHistory(GetWalletHistoryRequest request)
        {
            return _walletRepository.GetWalletHistory(request);
        }

        public byte[] GetWalletHistoryExport(GetWalletHistoryExportRequest request)
        {
            // Get the DataTable for wallet history export
            var dataTable = _walletRepository.GetWalletHistoryExportData(request);

            // Choose the export format based on ExportType:
            return request.ExportType switch
            {
                1 => FileExportHelper.ExportToExcel(dataTable), // Excel export
                2 => FileExportHelper.ExportToCsv(dataTable),   // CSV export
                _ => throw new ArgumentException("Invalid ExportType")
            };
        }

    }
}
