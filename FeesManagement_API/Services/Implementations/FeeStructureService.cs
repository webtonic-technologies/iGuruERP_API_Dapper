using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;
using FeesManagement_API.DTOs.ServiceResponse;


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

        public async Task<ServiceResponse<byte[]>> GetFeeStructureExcel(FeeStructureRequest request)
        {
            var fileBytes = await _feeStructureRepository.GetFeeStructureExcel(request);  // Get the Excel byte array from the repository
            return new ServiceResponse<byte[]>(true, "Excel file created successfully", fileBytes, 200);
        }

    }
}
