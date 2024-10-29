using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;

namespace FeesManagement_API.Services.Interfaces
{
    public interface IWalletService
    {
        string AddWalletAmount(AddWalletAmountRequest request);
        List<GetWalletResponse> GetWallet(GetWalletRequest request);
    }
}
