using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;

namespace Infirmary_API.Repositories.Interfaces
{
    public interface IStudentVaccinationFetchRepository
    {
        Task<ServiceResponseFetch<List<AcademicYearFetchResponse>>> GetAcademicYearFetch();
        Task<ServiceResponseFetch<List<ClassSectionFetchResponse>>> GetClassSectionFetch();
    }
}
