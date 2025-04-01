using Configuration.DTOs.Requests;
using Configuration.DTOs.Responses;
using Configuration.Repository.Interfaces;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeesManagement_API.Services.Implementations
{
    public class OfferService : IOfferService
    {
        private readonly IOfferRepository _offerRepository;

        public OfferService(IOfferRepository offerRepository)
        {
            _offerRepository = offerRepository;
        }

        public async Task<ServiceResponse<int>> AddUpdateOffer(AddUpdateOfferRequest request)
        {
            var result = await _offerRepository.AddUpdateOffer(request);
            return new ServiceResponse<int>(true, "Offer saved successfully", result, 200);
        }

        public async Task<ServiceResponse<IEnumerable<OfferResponse>>> GetAllOffers(GetAllOffersRequest request)
        {
            var result = await _offerRepository.GetAllOffers(request);
            return result; // Directly return the result from the repository
        }

        public async Task<ServiceResponse<OfferResponse>> GetOfferById(int offerID)
        {
            var result = await _offerRepository.GetOfferById(offerID);
            if (result != null)
            {
                return new ServiceResponse<OfferResponse>(true, "Offer retrieved successfully", result, 200);
            }
            return new ServiceResponse<OfferResponse>(false, "Offer not found", null, 404);
        }

        public async Task<ServiceResponse<int>> DeleteOffer(int offerID)
        {
            var result = await _offerRepository.DeleteOffer(offerID);
            if (result > 0)
            {
                return new ServiceResponse<int>(true, "Offer deleted successfully", result, 200);
            }
            return new ServiceResponse<int>(false, "Failed to update offer status", 0, 400);
        }
        public async Task<IEnumerable<OfferStudentTypeResponse>> GetOfferStudentTypes()
        {
            return await _offerRepository.GetOfferStudentTypes();
        }

        
    }
}
