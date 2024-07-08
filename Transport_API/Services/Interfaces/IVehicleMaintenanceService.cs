using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;

namespace Transport_API.Services.Interfaces
{
    public interface IVehicleMaintenanceService
    {
        Task<ServiceResponse<string>> AddUpdateVehicleExpense(VehicleExpenseRequest vehicleExpense);
        Task<ServiceResponse<IEnumerable<VehicleExpenseResponse>>> GetAllVehicleExpenses(GetAllExpenseRequest request);
        Task<ServiceResponse<VehicleExpense>> GetVehicleExpenseById(int VehicleId);
        Task<ServiceResponse<bool>> DeleteVehicleExpense(int vehicleExpenseId);
    }
}
