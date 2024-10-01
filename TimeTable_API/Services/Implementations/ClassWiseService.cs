using System.Threading.Tasks;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.Repository.Interfaces;
using TimeTable_API.Services.Interfaces;

namespace TimeTable_API.Services.Implementations
{
    public class ClassWiseService : IClassWiseService
    {
        private readonly IClassWiseRepository _classWiseRepository;

        public ClassWiseService(IClassWiseRepository classWiseRepository)
        {
            _classWiseRepository = classWiseRepository;
        }

        public async Task<ServiceResponse<ClassWiseResponse>> GetClassWiseTimeTables(ClassWiseRequest request)
        {
            return await _classWiseRepository.GetClassWiseTimeTables(request);
        }
    }
}
