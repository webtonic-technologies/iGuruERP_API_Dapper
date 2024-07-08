using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;
using Transport_API.Repository.Interfaces;
using Transport_API.Services.Interfaces;

namespace Transport_API.Services.Implementations
{
    public class VehicleMaintenanceService : IVehicleMaintenanceService
    {
        private readonly IVehicleMaintenanceRepository _vehicleMaintenanceRepository;

        public VehicleMaintenanceService(IVehicleMaintenanceRepository vehicleMaintenanceRepository)
        {
            _vehicleMaintenanceRepository = vehicleMaintenanceRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateVehicleExpense(VehicleExpenseRequest vehicleExpense)
        {
            return await _vehicleMaintenanceRepository.AddUpdateVehicleExpense(vehicleExpense);
        }

        public async Task<ServiceResponse<IEnumerable<VehicleExpenseResponse>>> GetAllVehicleExpenses(GetAllExpenseRequest request)
        {
            return await _vehicleMaintenanceRepository.GetAllVehicleExpenses(request);
        }

        public async Task<ServiceResponse<VehicleExpense>> GetVehicleExpenseById(int vehicleExpenseId)
        {
            return await _vehicleMaintenanceRepository.GetVehicleExpenseById(vehicleExpenseId);
        }

        public async Task<ServiceResponse<bool>> DeleteVehicleExpense(int vehicleExpenseId)
        {
            return await _vehicleMaintenanceRepository.DeleteVehicleExpense(vehicleExpenseId);
        }
    }
}
