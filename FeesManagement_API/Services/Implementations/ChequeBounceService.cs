using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;

namespace FeesManagement_API.Services.Implementations
{
    public class ChequeBounceService : IChequeBounceService
    {
        private readonly IChequeBounceRepository _chequeBounceRepository;

        public ChequeBounceService(IChequeBounceRepository chequeBounceRepository)
        {
            _chequeBounceRepository = chequeBounceRepository;
        }

        public ServiceResponse<bool> AddChequeBounce(SubmitChequeBounceRequest request)
        {
            return _chequeBounceRepository.AddChequeBounce(request);
        }
    }
}
