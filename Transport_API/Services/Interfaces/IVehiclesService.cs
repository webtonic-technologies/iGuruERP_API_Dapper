using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Responses;
using Transport_API.DTOs.ServiceResponse;

namespace Transport_API.Services.Interfaces
{
    public interface IVehiclesService
    {
        Task<ServiceResponse<string>> AddUpdateVehicle(VehicleRequest vehicle);
        Task<ServiceResponse<IEnumerable<Vehicle>>> GetAllVehicles(GetAllVehiclesRequest request);
        Task<ServiceResponse<Vehicle>> GetVehicleById(int vehicleId);
        Task<ServiceResponse<bool>> UpdateVehicleStatus(int vehicleId, string reason);
        Task<ServiceResponse<byte[]>> ExportExcel(GetAllExportVehiclesRequest request);
        Task<ServiceResponse<byte[]>> ExportCSV(GetAllExportVehiclesRequest request);
        Task<ServiceResponse<IEnumerable<GetVehicleTypeResponse>>> GetVehicleTypes();
        Task<ServiceResponse<IEnumerable<GetFuelTypeResponse>>> GetFuelTypes();

    }
}
