using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infirmary_API.Services.Interfaces
{
    public interface IVaccinationFetchService
    {
        Task<ServiceResponse<List<VaccinationFetchResponse>>> GetAllVaccinationsFetch(GetAllVaccinationsFetchRequest request);
    }
}
