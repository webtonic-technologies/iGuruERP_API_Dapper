using InfirmaryVisit_API.DTOs.Response;
using InfirmaryVisit_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfirmaryVisit_API.Repositories.Interfaces
{
    public interface IInfirmaryVisitFetchRepository
    {
        Task<ServiceResponseFetch<List<StudentInfoFetchResponse>>> GetAllStudentInfoFetch(int instituteId);
    }
}
