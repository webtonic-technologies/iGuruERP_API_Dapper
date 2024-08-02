using Dapper;
using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Repository.Interfaces;
using System.Data;
using System.Data.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;

namespace Transport_API.Repository.Implementations
{
    public class VehiclesRepository : IVehiclesRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public VehiclesRepository(IDbConnection dbConnection, IWebHostEnvironment hostingEnvironment)
        {
            _dbConnection = dbConnection;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<ServiceResponse<string>> AddUpdateVehicle(VehicleRequest vehicle)
        {
            string sql;
            if (vehicle.VehicleID == 0)
            {
                sql = @"INSERT INTO tblVehicleMaster (VehicleNumber, VehicleModel, RenewalYear, VehicleTypeID, FuelTypeID, SeatingCapacity, ChassieNo, InsurancePolicyNo, RenewalDate, AssignDriverID, GPSIMEINo, TrackingID, InstituteID) 
                        VALUES (@VehicleNumber, @VehicleModel, @RenewalYear, @VehicleTypeID, @FuelTypeID, @SeatingCapacity, @ChassieNo, @InsurancePolicyNo, @RenewalDate, @AssignDriverID, @GPSIMEINo, @TrackingID, @InstituteID)
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                int result = await _dbConnection.QueryFirstOrDefaultAsync<int>(sql, new {vehicle.VehicleNumber, vehicle.VehicleModel, vehicle.RenewalYear, vehicle.VehicleTypeID, vehicle.FuelTypeID, vehicle.SeatingCapacity, vehicle.ChassieNo, vehicle.InsurancePolicyNo, vehicle.RenewalDate, vehicle.AssignDriverID, vehicle.GPSIMEINo, vehicle.TrackingID, vehicle.InstituteID });

                if (result >0)
                {
                   int response = VehicleDocumentMapping(vehicle.VehicleDocuments, result);

                    return new ServiceResponse<string>(true, "Operation Successful", "Vehicle added/updated successfully", StatusCodes.Status200OK);

                }
                else
                {
                    return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating vehicle", StatusCodes.Status400BadRequest);
                }
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
                        TrackingID = @TrackingID,
                        InstituteID = @InstituteID
                        WHERE VehicleID = @VehicleID";


                int result = await _dbConnection.ExecuteAsync(sql, new { vehicle.VehicleID, vehicle.VehicleNumber, vehicle.VehicleModel, vehicle.RenewalYear, vehicle.VehicleTypeID, vehicle.FuelTypeID, vehicle.SeatingCapacity, vehicle.ChassieNo, vehicle.InsurancePolicyNo, vehicle.RenewalDate, vehicle.AssignDriverID, vehicle.GPSIMEINo, vehicle.TrackingID, vehicle.InstituteID });

                if (result > 0)
                {
                    int response = VehicleDocumentMapping(vehicle.VehicleDocuments, vehicle.VehicleID);

                    return new ServiceResponse<string>(true, "Operation Successful", "Vehicle added/updated successfully", StatusCodes.Status200OK);

                }
                else
                {
                    return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating vehicle", StatusCodes.Status400BadRequest);
                }
            }
 
        }


        public async Task<ServiceResponse<IEnumerable<Vehicle>>> GetAllVehicles(GetAllVehiclesRequest request)
        {
            string countSql = @"SELECT COUNT(*) FROM tblVehicleMaster";
            int totalCount = await _dbConnection.ExecuteScalarAsync<int>(countSql);

            string sql = @"SELECT 
                        VM.VehicleID, VM.VehicleNumber, VM.VehicleModel, VM.RenewalYear, VM.VehicleTypeID, VT.Vehicle_type_name as VehicleTypeName, VM.FuelTypeID, FT.Fuel_type_name as FuelTypeName, VM.SeatingCapacity, VM.ChassieNo, VM.InsurancePolicyNo, VM.RenewalDate, VM.AssignDriverID, EP.First_Name + ' ' + EP.Last_Name as AssignDriverName, VM.GPSIMEINo, VM.TrackingID, VM.InstituteID, VM.IsActive
                        FROM tblVehicleMaster VM
                        Left Outer Join tbl_Vehicle_Type VT ON VM.VehicleTypeID = VT.Vehicle_type_id
                        Left Outer Join tbl_Fuel_Type FT ON VM.FuelTypeID = FT.Fuel_type_id 
                        Left Outer Join tbl_EmployeeProfileMaster EP ON EP.Employee_id = VM.AssignDriverID 
                        where VM.InstituteID = @InstituteID 
                        AND (VM.VehicleNumber LIKE @SearchText OR VM.VehicleModel LIKE @SearchText OR @SearchText is NULL)
                        ORDER BY VM.VehicleID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var vehicles = await _dbConnection.QueryAsync<Vehicle>(sql, new { Offset = (request.PageNumber - 1) * request.PageSize, PageSize = request.PageSize, request.InstituteID, request.SearchText });

            if (vehicles.Any())
            {
                foreach (var data in vehicles)
                {
                    data.VehicleDocuments = GetListOfVehiclesDocument(data.VehicleID);
                }

                return new ServiceResponse<IEnumerable<Vehicle>>(true, "Records Found", vehicles, StatusCodes.Status200OK, vehicles.Count());
            }
            else
            {
                return new ServiceResponse<IEnumerable<Vehicle>>(false, "No Records Found", null, StatusCodes.Status204NoContent);
            }
        }

        public async Task<ServiceResponse<Vehicle>> GetVehicleById(int VehicleID)
        {
            string sql = @"SELECT 
                        VM.VehicleID, VM.VehicleNumber, VM.VehicleModel, VM.RenewalYear, VM.VehicleTypeID, VT.Vehicle_type_name as VehicleTypeName, VM.FuelTypeID, FT.Fuel_type_name as FuelTypeName, VM.SeatingCapacity, VM.ChassieNo, VM.InsurancePolicyNo, VM.RenewalDate, VM.AssignDriverID, EP.First_Name + ' ' + EP.Last_Name as AssignDriverName, VM.GPSIMEINo, VM.TrackingID, VM.InstituteID, VM.IsActive
                        FROM tblVehicleMaster VM
                        Left Outer Join tbl_Vehicle_Type VT ON VM.VehicleTypeID = VT.Vehicle_type_id
                        Left Outer Join tbl_Fuel_Type FT ON VM.FuelTypeID = FT.Fuel_type_id 
                        Left Outer Join tbl_EmployeeProfileMaster EP ON EP.Employee_id = VM.AssignDriverID WHERE VM.VehicleID = @VehicleID";
            var vehicle = await _dbConnection.QueryFirstOrDefaultAsync<Vehicle>(sql, new { VehicleId = VehicleID });

            if (vehicle != null)
            {
                vehicle.VehicleDocuments = GetListOfVehiclesDocument(vehicle.VehicleID);

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

        private List<VehicleDocumentRequest> GetListOfVehiclesDocument(int VehicleID)
        {
            string boardQuery = @"
            select * From tblVehicleDocument
            WHERE VehicleID = @VehicleID";

            // Execute the SQL query with the SOTDID parameter
            var data = _dbConnection.Query<VehicleDocumentRequest>(boardQuery, new { VehicleID });
            foreach (var item in data)
            {
                item.VehicleDocument = GetPDF(item.VehicleDocument);
            }
            return data != null ? data.AsList() : [];
        }


        private int VehicleDocumentMapping(List<VehicleDocumentRequest> requests, int VehicleID)
        {
            foreach (var data in requests)
            {
                data.VehicleDocument = PDFUpload(data.VehicleDocument);
                data.VehicleID = VehicleID;
            }

            string query = "SELECT COUNT(*) FROM [tblVehicleDocument] WHERE [VehicleID] = @VehicleID";
            int count = _dbConnection.QueryFirstOrDefault<int>(query, new { VehicleID });
            if (count > 0)
            {
                var deleteDuery = @"DELETE FROM [tblVehicleDocument]
                    WHERE [VehicleID] = @VehicleID;";
                var rowsAffected = _dbConnection.Execute(deleteDuery, new { VehicleID });
                if (rowsAffected > 0)
                {
                    var insertquery = @"INSERT INTO [tblVehicleDocument] ([VehicleID], [VehicleDocument])
                    VALUES (@VehicleID, @VehicleDocument);";
                    var valuesInserted = _dbConnection.Execute(insertquery, requests);
                    return valuesInserted;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                var insertquery = @"INSERT INTO [tblVehicleDocument] ([VehicleID], [VehicleDocument])
                    VALUES (@VehicleID, @VehicleDocument);";
                var valuesInserted = _dbConnection.Execute(insertquery, requests);
                return valuesInserted;
            }
        }

        private string PDFUpload(string pdf)
        {
            if (string.IsNullOrEmpty(pdf) || pdf == "string")
            {
                return string.Empty;
            }
            byte[] imageData = Convert.FromBase64String(pdf);
            string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "VehicleDcoumentsPDF");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string fileExtension = IsPdf(imageData) == true ? ".pdf" : string.Empty;
            string fileName = Guid.NewGuid().ToString() + fileExtension;
            string filePath = Path.Combine(directoryPath, fileName);

            // Write the byte array to the image file
            File.WriteAllBytes(filePath, imageData);
            return filePath;
        }

        private bool IsPdf(byte[] fileData)
        {
            return fileData.Length > 4 &&
                   fileData[0] == 0x25 && fileData[1] == 0x50 && fileData[2] == 0x44 && fileData[3] == 0x46;
        }

        private string GetPDF(string Filename)
        {
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "VehicleDcoumentsPDF", Filename);
            if (!File.Exists(filePath))
            {
                return string.Empty;
            }
            byte[] fileBytes = File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(fileBytes);
            return base64String;
        }



    }
}
