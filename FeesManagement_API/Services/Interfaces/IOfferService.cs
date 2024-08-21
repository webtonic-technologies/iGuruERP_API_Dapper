using Configuration.DTOs.Requests;
using Configuration.DTOs.Responses;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeesManagement_API.Services.Interfaces
{
    public interface IOfferService
    {
        Task<ServiceResponse<int>> AddUpdateOffer(AddUpdateOfferRequest request);
        Task<ServiceResponse<IEnumerable<OfferResponse>>> GetAllOffers(GetAllOffersRequest request);
        Task<ServiceResponse<OfferResponse>> GetOfferById(int offerID);
        Task<ServiceResponse<int>> DeleteOffer(int offerID);
    }
}
