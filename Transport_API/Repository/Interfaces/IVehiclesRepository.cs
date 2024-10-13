using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;

namespace Transport_API.Repository.Interfaces
{
    public interface IVehiclesRepository
    {
        Task<ServiceResponse<string>> AddUpdateVehicle(VehicleRequest vehicle);
        Task<ServiceResponse<IEnumerable<Vehicle>>> GetAllVehicles(GetAllVehiclesRequest request);
        Task<ServiceResponse<Vehicle>> GetVehicleById(int vehicleId);
        Task<ServiceResponse<bool>> UpdateVehicleStatus(int vehicleId);

        Task<ServiceResponse<byte[]>> ExportExcel(GetAllExportVehiclesRequest request);
        Task<ServiceResponse<byte[]>> ExportCSV(GetAllExportVehiclesRequest request);

    }
}
