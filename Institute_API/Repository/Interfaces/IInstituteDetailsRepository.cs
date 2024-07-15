using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;

namespace Institute_API.Repository.Interfaces
{
    public interface IInstituteDetailsRepository
    {
        Task<ServiceResponse<int>> AddUpdateInstititeDetails(InstituteDetailsDTO request);
        Task<ServiceResponse<InstituteDetailsResponseDTO>> GetInstituteDetailsById(int Id);
        Task<ServiceResponse<bool>> DeleteImage(DeleteImageRequest request);
    }
}
