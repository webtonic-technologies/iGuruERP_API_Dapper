using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;

namespace Transport_API.Services.Interfaces
{
    public interface IVehiclesService
    {
        Task<ServiceResponse<string>> AddUpdateVehicle(VehicleRequest vehicle);
        Task<ServiceResponse<IEnumerable<Vehicle>>> GetAllVehicles(GetAllVehiclesRequest request);
        Task<ServiceResponse<Vehicle>> GetVehicleById(int vehicleId);
        Task<ServiceResponse<bool>> UpdateVehicleStatus(int vehicleId);
    }
}
