using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Repository.Interfaces;
using Infirmary_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infirmary_API.Services.Implementations
{
    public class InfirmaryVisitorTypeService : IInfirmaryVisitorTypeService
    {
        private readonly IInfirmaryVisitorTypeRepository _infirmaryVisitorTypeRepository;

        public InfirmaryVisitorTypeService(IInfirmaryVisitorTypeRepository infirmaryVisitorTypeRepository)
        {
            _infirmaryVisitorTypeRepository = infirmaryVisitorTypeRepository;
        }

        public async Task<ServiceResponse<List<InfirmaryVisitorTypeResponse>>> GetAllInfirmaryVisitorTypes()
        {
            return await _infirmaryVisitorTypeRepository.GetAllInfirmaryVisitorTypes();
        }
    }
}
