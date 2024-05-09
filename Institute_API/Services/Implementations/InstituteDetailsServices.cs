using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Repository.Interfaces;
using Institute_API.Services.Interfaces;

namespace Institute_API.Services.Implementations
{
    public class InstituteDetailsServices : IInstituteDetailsServices
    {
        private readonly IInstituteDetailsRepository _instituteDetailsRepository;

        public InstituteDetailsServices(IInstituteDetailsRepository instituteDetailsRepository)
        {
            _instituteDetailsRepository = instituteDetailsRepository;
        }
        public async Task<ServiceResponse<int>> AddUpdateInstititeDetails(InstituteDetailsDTO request)
        {
            try
            {
                return await _instituteDetailsRepository.AddUpdateInstititeDetails(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<InstituteDetailsDTO>> GetInstituteDetailsById(int Id)
        {
            try
            {
                return await _instituteDetailsRepository.GetInstituteDetailsById(Id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<InstituteDetailsDTO>(false, ex.Message, new InstituteDetailsDTO(), 500);
            }
        }
    }
}
