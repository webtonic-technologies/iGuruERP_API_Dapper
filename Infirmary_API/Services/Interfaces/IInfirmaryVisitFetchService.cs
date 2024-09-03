using InfirmaryVisit_API.DTOs.Response;
using InfirmaryVisit_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfirmaryVisit_API.Services.Interfaces
{
    public interface IInfirmaryVisitFetchService
    {
        Task<ServiceResponseFetch<List<StudentInfoFetchResponse>>> GetAllStudentInfoFetch(int instituteId);
    }
}
