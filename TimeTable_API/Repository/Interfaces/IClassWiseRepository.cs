using System.Threading.Tasks;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses;
using TimeTable_API.DTOs.ServiceResponse;

namespace TimeTable_API.Repository.Interfaces
{
    public interface IClassWiseRepository
    {
        Task<ServiceResponse<ClassWiseResponse>> GetClassWiseTimeTables(ClassWiseRequest request);

        Task<ServiceResponse<ClassWiseTimeTableResponse>> GetClassWiseTimeTables(GetClassWiseTimeTablesRequest request);

    }
}
