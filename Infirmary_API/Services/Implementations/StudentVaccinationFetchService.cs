using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Repositories.Interfaces;
using Infirmary_API.Services.Interfaces;

namespace Infirmary_API.Services.Implementations
{
    public class StudentVaccinationFetchService : IStudentVaccinationFetchService
    {
        private readonly IStudentVaccinationFetchRepository _repository;

        public StudentVaccinationFetchService(IStudentVaccinationFetchRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponseFetch<List<AcademicYearFetchResponse>>> GetAcademicYearFetch()
        {
            var result = await _repository.GetAcademicYearFetch();
            return new ServiceResponseFetch<List<AcademicYearFetchResponse>>(
                result.Success,
                result.Message,
                result.Data,  // Correcting by passing the 'Data' field which is a List<AcademicYearFetchResponse>
                result.StatusCode,
                result.TotalCount
            );
        }

        public async Task<ServiceResponseFetch<List<ClassSectionFetchResponse>>> GetClassSectionFetch()
        {
            var result = await _repository.GetClassSectionFetch();
            return new ServiceResponseFetch<List<ClassSectionFetchResponse>>(
                result.Success,
                result.Message,
                result.Data,  // Correcting by passing the 'Data' field which is a List<ClassSectionFetchResponse>
                result.StatusCode,
                result.TotalCount
            );
        }
    }
}
