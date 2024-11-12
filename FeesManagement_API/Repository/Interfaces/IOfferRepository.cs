using Configuration.DTOs.Requests;
using Configuration.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.DTOs.Responses;


namespace Configuration.Repository.Interfaces
{
    public interface IOfferRepository
    {
        Task<int> AddUpdateOffer(AddUpdateOfferRequest request);
        Task<ServiceResponse<IEnumerable<OfferResponse>>> GetAllOffers(GetAllOffersRequest request);
        Task<OfferResponse> GetOfferById(int offerID);
        Task<int> DeleteOffer(int offerID);
        Task<IEnumerable<OfferStudentTypeResponse>> GetOfferStudentTypes();

    }
}
