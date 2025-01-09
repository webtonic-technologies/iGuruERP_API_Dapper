using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Requests.Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.Responses;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;

namespace Transport_API.Repository.Interfaces
{
    public interface IVehicleMaintenanceRepository
    {
        Task<ServiceResponse<string>> AddUpdateVehicleExpense(VehicleExpenseRequest vehicleExpense);
        Task<ServiceResponse<IEnumerable<GetAllExpenseResponse>>> GetAllVehicleExpenses(GetAllExpenseRequest request);
        Task<ServiceResponse<GetAllExpenseResponse>> GetVehicleExpenseById(int VehicleId);
        Task<ServiceResponse<bool>> DeleteVehicleExpense(int vehicleExpenseId);
        Task<IEnumerable<GetVehicleExpenseTypeResponse>> GetVehicleExpenseTypes();
        Task<List<GetAllExpenseExportResponse>> GetAllExpenseExport(GetAllExpenseExportRequest request);
        Task<ServiceResponse<string>> AddFuelExpense(AddFuelExpenseRequest request);
        Task<ServiceResponse<IEnumerable<GetFuelExpenseResponse>>> GetFuelExpense(GetFuelExpenseRequest request);
        Task<ServiceResponse<byte[]>> GetFuelExpenseExport(GetFuelExpenseExportRequest request);
    }
}
