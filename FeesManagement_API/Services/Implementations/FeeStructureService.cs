using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;

namespace FeesManagement_API.Services.Implementations
{
    public class FeeStructureService : IFeeStructureService
    {
        private readonly IFeeStructureRepository _feeStructureRepository;

        public FeeStructureService(IFeeStructureRepository feeStructureRepository)
        {
            _feeStructureRepository = feeStructureRepository;
        }

        public FeeStructureResponse GetFeeStructure(FeeStructureRequest request)
        {
            return _feeStructureRepository.GetFeeStructure(request);
        }
    }
}
