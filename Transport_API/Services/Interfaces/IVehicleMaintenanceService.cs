using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;

namespace Transport_API.Services.Interfaces
{
    public interface IVehicleMaintenanceService
    {
        Task<ServiceResponse<string>> AddUpdateVehicleExpense(VehicleExpense vehicleExpense);
        Task<ServiceResponse<IEnumerable<VehicleExpense>>> GetAllVehicleExpenses(GetAllExpenseRequest request);
        Task<ServiceResponse<VehicleExpense>> GetVehicleExpenseById(int vehicleExpenseId);
        Task<ServiceResponse<bool>> UpdateVehicleExpenseStatus(int vehicleExpenseId);
    }
}
