using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infirmary_API.Services.Interfaces
{
    public interface IVaccinationService
    {
        Task<ServiceResponse<string>> AddUpdateVaccination(AddUpdateVaccinationRequest request);
        Task<ServiceResponse<List<VaccinationResponse>>> GetAllVaccinations(GetAllVaccinationsRequest request);
        Task<ServiceResponse<Vaccination>> GetVaccinationById(int id);
        Task<ServiceResponse<bool>> DeleteVaccination(int id);
    }
}
