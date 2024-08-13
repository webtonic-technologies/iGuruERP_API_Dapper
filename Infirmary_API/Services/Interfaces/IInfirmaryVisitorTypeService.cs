using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infirmary_API.Services.Interfaces
{
    public interface IInfirmaryVisitorTypeService
    {
        Task<ServiceResponse<List<InfirmaryVisitorTypeResponse>>> GetAllInfirmaryVisitorTypes();
    }
}
