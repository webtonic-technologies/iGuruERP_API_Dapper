using OfficeOpenXml;
using System.Text;
using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Requests.Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.Responses;
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

        public async Task<ServiceResponse<IEnumerable<GetAllExpenseResponse>>> GetAllVehicleExpenses(GetAllExpenseRequest request)
        {
            return await _vehicleMaintenanceRepository.GetAllVehicleExpenses(request);
        }


        public async Task<ServiceResponse<GetAllExpenseResponse>> GetVehicleExpenseById(int vehicleExpenseId)
        {
            return await _vehicleMaintenanceRepository.GetVehicleExpenseById(vehicleExpenseId);
        }

        public async Task<ServiceResponse<bool>> DeleteVehicleExpense(int vehicleExpenseId)
        {
            return await _vehicleMaintenanceRepository.DeleteVehicleExpense(vehicleExpenseId);
        }

        public async Task<ServiceResponse<IEnumerable<GetVehicleExpenseTypeResponse>>> GetVehicleExpenseType()
        {
            var vehicleExpenseTypes = await _vehicleMaintenanceRepository.GetVehicleExpenseTypes();

            if (vehicleExpenseTypes == null || !vehicleExpenseTypes.Any())
            {
                return new ServiceResponse<IEnumerable<GetVehicleExpenseTypeResponse>>(false, "No expense types found", new List<GetVehicleExpenseTypeResponse>(), 404);
            }

            return new ServiceResponse<IEnumerable<GetVehicleExpenseTypeResponse>>(true, "Expense types found", vehicleExpenseTypes, 200);
        }

        //public async Task<ServiceResponse<byte[]>> GetAllExpenseExport(GetAllExpenseExportRequest request)
        //{
        //    try
        //    {
        //        // Fetch the expenses from the repository
        //        var expenses = await _vehicleMaintenanceRepository.GetAllExpenseExport(request);

        //        if (expenses == null || expenses.Count == 0)
        //        {
        //            return new ServiceResponse<byte[]>(false, "No data found to export", null, 404);
        //        }

        //        // Create Excel file using EPPlus
        //        using (var package = new ExcelPackage())
        //        {
        //            var worksheet = package.Workbook.Worksheets.Add("Vehicle Expenses");

        //            // Add headers to the Excel sheet
        //            worksheet.Cells[1, 1].Value = "Vehicle Number";
        //            worksheet.Cells[1, 2].Value = "Expense Type";
        //            worksheet.Cells[1, 3].Value = "Expense Date";
        //            worksheet.Cells[1, 4].Value = "Remarks";
        //            worksheet.Cells[1, 5].Value = "Amount";

        //            // Fill data rows
        //            for (int i = 0; i < expenses.Count; i++)
        //            {
        //                var expense = expenses[i];
        //                worksheet.Cells[i + 2, 1].Value = expense.VehicleNumber;
        //                worksheet.Cells[i + 2, 2].Value = expense.ExpenseType;
        //                worksheet.Cells[i + 2, 3].Value = expense.ExpenseDate;
        //                worksheet.Cells[i + 2, 4].Value = expense.Remarks;
        //                worksheet.Cells[i + 2, 5].Value = expense.Amount;
        //            }

        //            // Format columns (optional)
        //            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        //            // Convert to byte array for download
        //            var fileContents = package.GetAsByteArray();

        //            return new ServiceResponse<byte[]>(true, "Excel file generated successfully", fileContents, 200);
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return new ServiceResponse<byte[]>(false, ex.Message, null, 500);
        //    }
        //}

        public async Task<ServiceResponse<byte[]>> GetAllExpenseExport(GetAllExpenseExportRequest request)
        {
            try
            {
                // Fetch the expenses from the repository
                var expenses = await _vehicleMaintenanceRepository.GetAllExpenseExport(request);

                if (expenses == null || expenses.Count == 0)
                {
                    return new ServiceResponse<byte[]>(false, "No data found to export", null, 404);
                }

                if (request.ExportType == 1) // Excel Export
                {
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Vehicle Expenses");

                        // Add headers to the Excel sheet
                        worksheet.Cells[1, 1].Value = "Vehicle Number";
                        worksheet.Cells[1, 2].Value = "Expense Type";
                        worksheet.Cells[1, 3].Value = "Expense Date";
                        worksheet.Cells[1, 4].Value = "Remarks";
                        worksheet.Cells[1, 5].Value = "Amount";

                        // Fill data rows
                        for (int i = 0; i < expenses.Count; i++)
                        {
                            var expense = expenses[i];
                            worksheet.Cells[i + 2, 1].Value = expense.VehicleNumber;
                            worksheet.Cells[i + 2, 2].Value = expense.ExpenseType;
                            worksheet.Cells[i + 2, 3].Value = expense.ExpenseDate;
                            worksheet.Cells[i + 2, 4].Value = expense.Remarks;
                            worksheet.Cells[i + 2, 5].Value = expense.Amount;
                        }

                        // Format columns (optional)
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                        // Convert to byte array for download
                        var fileContents = package.GetAsByteArray();
                        return new ServiceResponse<byte[]>(true, "Excel file generated successfully", fileContents, 200);
                    }
                }
                else if (request.ExportType == 2) // CSV Export
                {
                    var csvData = new StringBuilder();
                    csvData.AppendLine("Vehicle Number,Expense Type,Expense Date,Remarks,Amount");

                    // Create CSV content
                    foreach (var expense in expenses)
                    {
                        csvData.AppendLine($"{expense.VehicleNumber},{expense.ExpenseType},{expense.ExpenseDate},{expense.Remarks},{expense.Amount}");
                    }

                    // Convert the StringBuilder to byte array for download
                    return new ServiceResponse<byte[]>(true, "CSV file generated successfully", Encoding.UTF8.GetBytes(csvData.ToString()), 200);
                }
                else
                {
                    return new ServiceResponse<byte[]>(false, "Invalid Export Type", null, 400);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, null, 500);
            }
        }


        public async Task<ServiceResponse<string>> AddFuelExpense(AddFuelExpenseRequest request)
        {
            return await _vehicleMaintenanceRepository.AddFuelExpense(request);
        }

        public async Task<ServiceResponse<IEnumerable<GetFuelExpenseResponse>>> GetFuelExpense(GetFuelExpenseRequest request)
        {
            try
            {
                // Call the repository method to fetch the fuel expenses
                var response = await _vehicleMaintenanceRepository.GetFuelExpense(request);

                // Return the repository response back to the controller
                return response;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetFuelExpenseResponse>>(false, $"Error retrieving fuel expenses: {ex.Message}", null, 500);
            }
        }

        public async Task<ServiceResponse<byte[]>> GetFuelExpenseExport(GetFuelExpenseExportRequest request)
        { 
            return await _vehicleMaintenanceRepository.GetFuelExpenseExport(request);
        }
    }
}
