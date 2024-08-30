using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Repository.Interfaces;
using Infirmary_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infirmary_API.Services.Implementations
{
    public class VaccinationFetchService : IVaccinationFetchService
    {
        private readonly IVaccinationFetchRepository _vaccinationFetchRepository;

        public VaccinationFetchService(IVaccinationFetchRepository vaccinationFetchRepository)
        {
            _vaccinationFetchRepository = vaccinationFetchRepository;
        }

        public async Task<ServiceResponse<List<VaccinationFetchResponse>>> GetAllVaccinationsFetch(GetAllVaccinationsFetchRequest request)
        {
            return await _vaccinationFetchRepository.GetAllVaccinationsFetch(request);
        }
    }
}
