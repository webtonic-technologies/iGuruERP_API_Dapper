using Dapper;
using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;
using Transport_API.Repository.Interfaces;
using System.Data;

namespace Transport_API.Repository.Implementations
{
    public class VehicleMaintenanceRepository : IVehicleMaintenanceRepository
    {
        private readonly IDbConnection _dbConnection;

        public VehicleMaintenanceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<string>> AddUpdateVehicleExpense(VehicleExpense vehicleExpense)
        {
            string sql;
            if (vehicleExpense.VehicleExpenseID == 0)
            {
                sql = @"INSERT INTO tblVehicleExpense (ExpenseTypeID, ExpenseDate, Cost, Remarks) 
                        VALUES (@ExpenseTypeID, @ExpenseDate, @Cost, @Remarks)";
            }
            else
            {
                sql = @"UPDATE tblVehicleExpense SET ExpenseTypeID = @ExpenseTypeID, ExpenseDate = @ExpenseDate, 
                        Cost = @Cost, Remarks = @Remarks WHERE VehicleExpenseID = @VehicleExpenseID";
            }

            var result = await _dbConnection.ExecuteAsync(sql, vehicleExpense);
            if (result > 0)
            {
                return new ServiceResponse<string>(true, "Operation Successful", "Vehicle expense added/updated successfully", StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating vehicle expense", StatusCodes.Status400BadRequest);
            }
        }

        public async Task<ServiceResponse<IEnumerable<VehicleExpense>>> GetAllVehicleExpenses(GetAllExpenseRequest request)
        {
            string countSql = @"SELECT COUNT(*) FROM tblVehicleExpense";
            int totalCount = await _dbConnection.ExecuteScalarAsync<int>(countSql);

            string sql = @"SELECT * FROM tblVehicleExpense ORDER BY VehicleExpenseID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            var vehicleExpenses = await _dbConnection.QueryAsync<VehicleExpense>(sql, new { Offset = (request.PageNumber - 1) * request.PageSize, PageSize = request.PageSize });

            if (vehicleExpenses.Any())
            {
                return new ServiceResponse<IEnumerable<VehicleExpense>>(true, "Records Found", vehicleExpenses, StatusCodes.Status200OK, totalCount);
            }
            else
            {
                return new ServiceResponse<IEnumerable<VehicleExpense>>(false, "No Records Found", null, StatusCodes.Status204NoContent);
            }
        }

        public async Task<ServiceResponse<VehicleExpense>> GetVehicleExpenseById(int VehicleExpenseID)
        {
            string sql = @"SELECT * FROM tblVehicleExpense WHERE VehicleExpenseID = @VehicleExpenseID";
            var vehicleExpense = await _dbConnection.QueryFirstOrDefaultAsync<VehicleExpense>(sql, new { VehicleExpenseID = VehicleExpenseID });

            if (vehicleExpense != null)
            {
                return new ServiceResponse<VehicleExpense>(true, "Record Found", vehicleExpense, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<VehicleExpense>(false, "Record Not Found", null, StatusCodes.Status204NoContent);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateVehicleExpenseStatus(int VehicleExpenseID)
        {
            string sql = @"UPDATE tblVehicleExpense SET IsActive = ~IsActive WHERE VehicleExpenseID = @VehicleExpenseID";
            var result = await _dbConnection.ExecuteAsync(sql, new { VehicleExpenseID = VehicleExpenseID });

            if (result > 0)
            {
                return new ServiceResponse<bool>(true, "Status Updated Successfully", true, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<bool>(false, "Status Update Failed", false, StatusCodes.Status400BadRequest);
            }
        }
    }
}
