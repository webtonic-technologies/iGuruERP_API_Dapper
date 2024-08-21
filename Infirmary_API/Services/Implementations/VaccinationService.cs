using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using Infirmary_API.Repository.Interfaces;
using Infirmary_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infirmary_API.Services.Implementations
{
    public class VaccinationService : IVaccinationService
    {
        private readonly IVaccinationRepository _vaccinationRepository;

        public VaccinationService(IVaccinationRepository vaccinationRepository)
        {
            _vaccinationRepository = vaccinationRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateVaccination(AddUpdateVaccinationRequest request)
        {
            return await _vaccinationRepository.AddUpdateVaccination(request);
        }

        public async Task<ServiceResponse<List<VaccinationResponse>>> GetAllVaccinations(GetAllVaccinationsRequest request)
        {
            return await _vaccinationRepository.GetAllVaccinations(request);
        }

        public async Task<ServiceResponse<Vaccination>> GetVaccinationById(int id)
        {
            return await _vaccinationRepository.GetVaccinationById(id);
        }

        public async Task<ServiceResponse<bool>> DeleteVaccination(int id)
        {
            return await _vaccinationRepository.DeleteVaccination(id);
        }
    }
}
