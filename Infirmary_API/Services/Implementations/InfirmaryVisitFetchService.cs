using InfirmaryVisit_API.DTOs.Response;
using InfirmaryVisit_API.DTOs.ServiceResponse;
using InfirmaryVisit_API.Repositories.Interfaces;
using InfirmaryVisit_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfirmaryVisit_API.Services.Implementations
{
    public class InfirmaryVisitFetchService : IInfirmaryVisitFetchService
    {
        private readonly IInfirmaryVisitFetchRepository _repository;

        public InfirmaryVisitFetchService(IInfirmaryVisitFetchRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponseFetch<List<StudentInfoFetchResponse>>> GetAllStudentInfoFetch(int instituteId)
        {
            return await _repository.GetAllStudentInfoFetch(instituteId);
        }
    }
}
