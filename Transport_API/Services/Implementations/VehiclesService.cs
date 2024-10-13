using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Repository.Interfaces;
using Transport_API.Services.Interfaces;

namespace Transport_API.Services.Implementations
{
    public class VehiclesService : IVehiclesService
    {
        private readonly IVehiclesRepository _vehiclesRepository;

        public VehiclesService(IVehiclesRepository vehiclesRepository)
        {
            _vehiclesRepository = vehiclesRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateVehicle(VehicleRequest vehicle)
        {
            return await _vehiclesRepository.AddUpdateVehicle(vehicle);
        }

        public async Task<ServiceResponse<IEnumerable<Vehicle>>> GetAllVehicles(GetAllVehiclesRequest request)
        {
            return await _vehiclesRepository.GetAllVehicles(request);
        }

        public async Task<ServiceResponse<Vehicle>> GetVehicleById(int vehicleId)
        {
            return await _vehiclesRepository.GetVehicleById(vehicleId);
        }

        public async Task<ServiceResponse<bool>> UpdateVehicleStatus(int vehicleId)
        {
            return await _vehiclesRepository.UpdateVehicleStatus(vehicleId);
        }
        public async Task<ServiceResponse<byte[]>> ExportExcel(GetAllExportVehiclesRequest request)
        {
            return await _vehiclesRepository.ExportExcel(request);
        }

        public async Task<ServiceResponse<byte[]>> ExportCSV(GetAllExportVehiclesRequest request)
        {
            return await _vehiclesRepository.ExportCSV(request);
        }

    }
}
