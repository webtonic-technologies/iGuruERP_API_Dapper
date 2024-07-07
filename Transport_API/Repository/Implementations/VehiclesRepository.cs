using Dapper;
using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Repository.Interfaces;
using System.Data;

namespace Transport_API.Repository.Implementations
{
    public class VehiclesRepository : IVehiclesRepository
    {
        private readonly IDbConnection _dbConnection;

        public VehiclesRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<string>> AddUpdateVehicle(Vehicle vehicle)
        {
            string sql;
            if (vehicle.VehicleID == 0)
            {
                sql = @"INSERT INTO tblVehicleMaster (VehicleNumber, VehicleModel, RenewalYear, VehicleTypeID, FuelTypeID, SeatingCapacity, ChassieNo, InsurancePolicyNo, RenewalDate, AssignDriverID, GPSIMEINo, TrackingID) 
                        VALUES (@VehicleNumber, @VehicleModel, @RenewalYear, @VehicleTypeID, @FuelTypeID, @SeatingCapacity, @ChassieNo, @InsurancePolicyNo, @RenewalDate, @AssignDriverID, @GPSIMEINo, @TrackingID)";
            }
            else
            {
                sql = @"UPDATE tblVehicleMaster SET 
                        VehicleNumber = @VehicleNumber,
                        VehicleModel = @VehicleModel,
                        RenewalYear = @RenewalYear,
                        VehicleTypeID = @VehicleTypeID,
                        FuelTypeID = @FuelTypeID,
                        SeatingCapacity = @SeatingCapacity,
                        ChassieNo = @ChassieNo, 
                        InsurancePolicyNo = @InsurancePolicyNo, 
                        RenewalDate = @RenewalDate, 
                        AssignDriverID = @AssignDriverID, 
                        GPSIMEINo = @GPSIMEINo, 
                        TrackingID = @TrackingID
                        WHERE VehicleID = @VehicleID";
            }

            var result = await _dbConnection.ExecuteAsync(sql, vehicle);
            if (result > 0)
            {
                return new ServiceResponse<string>(true, "Operation Successful", "Vehicle added/updated successfully", StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating vehicle", StatusCodes.Status400BadRequest);
            }
        }

        public async Task<ServiceResponse<IEnumerable<Vehicle>>> GetAllVehicles(GetAllVehiclesRequest request)
        {
            string countSql = @"SELECT COUNT(*) FROM tblVehicleMaster";
            int totalCount = await _dbConnection.ExecuteScalarAsync<int>(countSql);

            string sql = @"SELECT * FROM tblVehicleMaster ORDER BY VehicleID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            var vehicles = await _dbConnection.QueryAsync<Vehicle>(sql, new { Offset = (request.PageNumber - 1) * request.PageSize, PageSize = request.PageSize });

            if (vehicles.Any())
            {
                return new ServiceResponse<IEnumerable<Vehicle>>(true, "Records Found", vehicles, StatusCodes.Status200OK, totalCount);
            }
            else
            {
                return new ServiceResponse<IEnumerable<Vehicle>>(false, "No Records Found", null, StatusCodes.Status204NoContent);
            }
        }

        public async Task<ServiceResponse<Vehicle>> GetVehicleById(int VehicleID)
        {
            string sql = @"SELECT * FROM tblVehicleMaster WHERE VehicleID = @VehicleID";
            var vehicle = await _dbConnection.QueryFirstOrDefaultAsync<Vehicle>(sql, new { VehicleId = VehicleID });

            if (vehicle != null)
            {
                return new ServiceResponse<Vehicle>(true, "Record Found", vehicle, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<Vehicle>(false, "Record Not Found", null, StatusCodes.Status204NoContent);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateVehicleStatus(int VehicleID)
        {
            string sql = @"UPDATE tblVehicleMaster SET IsActive = ~IsActive WHERE VehicleID = @VehicleID";
            var result = await _dbConnection.ExecuteAsync(sql, new { VehicleID = VehicleID });

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
