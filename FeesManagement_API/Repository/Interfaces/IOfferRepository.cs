using Configuration.DTOs.Requests;
using Configuration.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Configuration.Repository.Interfaces
{
    public interface IOfferRepository
    {
        Task<int> AddUpdateOffer(AddUpdateOfferRequest request);
        Task<IEnumerable<OfferResponse>> GetAllOffers(GetAllOffersRequest request);
        Task<OfferResponse> GetOfferById(int offerID);
        Task<int> DeleteOffer(int offerID);
    }
}
