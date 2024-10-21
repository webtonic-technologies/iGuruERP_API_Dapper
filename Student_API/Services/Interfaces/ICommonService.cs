using Student_API.DTOs.ServiceResponse;

namespace Student_API.Services.Interfaces
{
    public interface ICommonService
    {
        Task<ServiceResponse<string>> ExportDataToFile<T>(List<T> data, List<string> headers, int exportFormat, string fileName);
    }
}
