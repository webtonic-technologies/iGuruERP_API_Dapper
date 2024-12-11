using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
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

        public string AddWalletAmount(AddWalletAmountRequest request)
        {
            return _walletRepository.AddWalletAmount(request);
        }

        public List<GetWalletResponse> GetWallet(GetWalletRequest request)
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
    }
}
