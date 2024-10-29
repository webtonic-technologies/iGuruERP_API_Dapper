using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;

namespace FeesManagement_API.Services.Implementations
{
    public class ChequeTrackingService : IChequeTrackingService
    {
        private readonly IChequeTrackingRepository _chequeTrackingRepository;

        public ChequeTrackingService(IChequeTrackingRepository chequeTrackingRepository)
        {
            _chequeTrackingRepository = chequeTrackingRepository;
        }

        public ServiceResponse<List<ChequeTrackingResponse>> GetChequeTracking(GetChequeTrackingRequest request)
        {
            return _chequeTrackingRepository.GetChequeTracking(request);
        }
    }
}
