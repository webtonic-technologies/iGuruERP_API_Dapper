using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Requests.Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;

namespace Transport_API.Services.Interfaces
{
    public interface IVehicleMaintenanceService
    {
        Task<ServiceResponse<string>> AddUpdateVehicleExpense(VehicleExpenseRequest vehicleExpense);
        Task<ServiceResponse<IEnumerable<GetAllExpenseResponse>>> GetAllVehicleExpenses(GetAllExpenseRequest request);
        Task<ServiceResponse<GetAllExpenseResponse>> GetVehicleExpenseById(int VehicleId);
        Task<ServiceResponse<bool>> DeleteVehicleExpense(int vehicleExpenseId);
        Task<ServiceResponse<IEnumerable<GetVehicleExpenseTypeResponse>>> GetVehicleExpenseType();
        Task<ServiceResponse<byte[]>> GetAllExpenseExport(GetAllExpenseExportRequest request);

    }
}
