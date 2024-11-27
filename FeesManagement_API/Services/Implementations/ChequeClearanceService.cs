using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;

namespace FeesManagement_API.Services.Implementations
{
    public class ChequeClearanceService : IChequeClearanceService
    {
        private readonly IChequeClearanceRepository _chequeClearanceRepository;

        public ChequeClearanceService(IChequeClearanceRepository chequeClearanceRepository)
        {
            _chequeClearanceRepository = chequeClearanceRepository;
        }

        public ServiceResponse<bool> AddChequeClearance(SubmitChequeClearanceRequest request)
        {
            return _chequeClearanceRepository.AddChequeClearance(request);
        }
    }
}
