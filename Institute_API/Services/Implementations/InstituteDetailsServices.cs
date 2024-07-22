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

        public async Task<ServiceResponse<bool>> DeleteImage(DeleteImageRequest request)
        {
            try
            {
                return await _instituteDetailsRepository.DeleteImage(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<ServiceResponse<List<InstituteDetailsResponseDTO>>> GetAllInstituteDetailsList()
        {
            try
            {
                return await _instituteDetailsRepository.GetAllInstituteDetailsList();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<InstituteDetailsResponseDTO>>(false, ex.Message, [], 500);
            }
        }

        public async Task<ServiceResponse<InstituteDetailsResponseDTO>> GetInstituteDetailsById(int Id)
        {
            try
            {
                return await _instituteDetailsRepository.GetInstituteDetailsById(Id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<InstituteDetailsResponseDTO>(false, ex.Message, new InstituteDetailsResponseDTO(), 500);
            }
        }
    }
}
