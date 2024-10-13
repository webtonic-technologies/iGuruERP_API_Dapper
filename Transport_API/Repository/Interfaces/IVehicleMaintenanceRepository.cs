using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Requests.Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;

namespace Transport_API.Repository.Interfaces
{
    public interface IVehicleMaintenanceRepository
    {
        Task<ServiceResponse<string>> AddUpdateVehicleExpense(VehicleExpenseRequest vehicleExpense);
        Task<ServiceResponse<IEnumerable<GetAllExpenseResponse>>> GetAllVehicleExpenses(GetAllExpenseRequest request);
        Task<ServiceResponse<VehicleExpense>> GetVehicleExpenseById(int VehicleId);
        Task<ServiceResponse<bool>> DeleteVehicleExpense(int vehicleExpenseId);
    }
}
