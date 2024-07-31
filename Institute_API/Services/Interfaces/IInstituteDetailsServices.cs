using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;

namespace Institute_API.Services.Interfaces
{
    public interface IInstituteDetailsServices
    {
        Task<ServiceResponse<int>> AddUpdateInstititeDetails(InstituteDetailsDTO request);
        Task<ServiceResponse<InstituteDetailsResponseDTO>> GetInstituteDetailsById(int Id);
        Task<ServiceResponse<bool>> DeleteImage(DeleteImageRequest request);
        Task<ServiceResponse<List<InstituteDetailsResponseDTO>>> GetAllInstituteDetailsList();
        Task<ServiceResponse<List<Country>>> GetCountriesAsync();
        Task<ServiceResponse<List<State>>> GetStatesByCountryIdAsync(int countryId);
        Task<ServiceResponse<List<City>>> GetCitiesByStateIdAsync(int stateId);
        Task<ServiceResponse<List<District>>> GetDistrictsByCityIdAsync(int cityId);
    }
}
