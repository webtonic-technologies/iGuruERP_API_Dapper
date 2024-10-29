using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;

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
    }
}
