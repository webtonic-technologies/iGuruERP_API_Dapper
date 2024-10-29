using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface IWalletRepository
    {
        string AddWalletAmount(AddWalletAmountRequest request);
        List<GetWalletResponse> GetWallet(GetWalletRequest request);
    }
}
