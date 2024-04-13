using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Repository.Interfaces;
using Institute_API.Services.Interfaces;

namespace Institute_API.Services.Implementations
{
    public class InstituteAffiliationServices : IInstituteAffiliationServices
    {
        private readonly IInstituteAffiliationRepository _instituteAffiliationRepository;

        public InstituteAffiliationServices(IInstituteAffiliationRepository instituteAffiliationRepository)
        {
            _instituteAffiliationRepository = instituteAffiliationRepository;
        }
        public async Task<ServiceResponse<int>> AddUpdateInstituteAffiliation(AffiliationDTO request)
        {
            try
            {
                return await _instituteAffiliationRepository.AddUpdateInstituteAffiliation(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<string>> AddUpdateLogo(AffiliationLogoDTO request)
        {
            try
            {
                return await _instituteAffiliationRepository.AddUpdateLogo(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        public async Task<ServiceResponse<AffiliationDTO>> GetAffiliationInfoById(int Id)
        {
            try
            {
                return await _instituteAffiliationRepository.GetAffiliationInfoById(Id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AffiliationDTO>(false, ex.Message, new AffiliationDTO(), 500);
            }
        }

        public async Task<ServiceResponse<byte[]>> GetAffiliationLogoById(int Id)
        {
            try
            {
                return await _instituteAffiliationRepository.GetAffiliationLogoById(Id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, [], 500);
            }
        }
    }
}
