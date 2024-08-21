using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infirmary_API.Repository.Interfaces
{
    public interface IInfirmaryVisitorTypeRepository
    {
        Task<ServiceResponse<List<InfirmaryVisitorTypeResponse>>> GetAllInfirmaryVisitorTypes();
    }
}
