using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using System.Threading.Tasks;

namespace Infirmary_API.Services.Interfaces
{
    public interface IStudentVaccinationFetchService
    {
        Task<ServiceResponseFetch<List<AcademicYearFetchResponse>>> GetAcademicYearFetch();
        Task<ServiceResponseFetch<List<ClassSectionFetchResponse>>> GetClassSectionFetch();
    }
}
