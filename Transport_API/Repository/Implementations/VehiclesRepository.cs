﻿using Dapper;
using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Repository.Interfaces;
using System.Data;
using System.Data.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using System.Text;
using OfficeOpenXml;


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
                sql = @"INSERT INTO tblVehicleMaster (VehicleNumber, VehicleModel, RegistrationYear, VehicleTypeID, FuelTypeID, SeatingCapacity, ChassieNo, InsurancePolicyNo, RenewalDate, AssignDriverID, GPSIMEINo, TrackingID, InstituteID) 
                        VALUES (@VehicleNumber, @VehicleModel, @RegistrationYear, @VehicleTypeID, @FuelTypeID, @SeatingCapacity, @ChassieNo, @InsurancePolicyNo, @RenewalDate, @AssignDriverID, @GPSIMEINo, @TrackingID, @InstituteID)
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                int result = await _dbConnection.QueryFirstOrDefaultAsync<int>(sql, new {vehicle.VehicleNumber, vehicle.VehicleModel, vehicle.RegistrationYear, vehicle.VehicleTypeID, vehicle.FuelTypeID, vehicle.SeatingCapacity, vehicle.ChassieNo, vehicle.InsurancePolicyNo, vehicle.RenewalDate, vehicle.AssignDriverID, vehicle.GPSIMEINo, vehicle.TrackingID, vehicle.InstituteID });

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
                        RegistrationYear = @RegistrationYear,
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


                int result = await _dbConnection.ExecuteAsync(sql, new { vehicle.VehicleID, vehicle.VehicleNumber, vehicle.VehicleModel, vehicle.RegistrationYear, vehicle.VehicleTypeID, vehicle.FuelTypeID, vehicle.SeatingCapacity, vehicle.ChassieNo, vehicle.InsurancePolicyNo, vehicle.RenewalDate, vehicle.AssignDriverID, vehicle.GPSIMEINo, vehicle.TrackingID, vehicle.InstituteID });

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
            // Step 1: Fetch the total count of records
            string countSql = @"SELECT COUNT(*) FROM tblVehicleMaster WHERE InstituteID = @InstituteID";
            int totalCount = await _dbConnection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

            // Step 2: Fetch the list of active columns from tblVehicleColumnSetting
            string columnListSql = @"SELECT STRING_AGG(DatabaseFieldName, ', ') 
                             FROM tblVehicleColumnSetting
                             WHERE IsActive = 1";
            string columnList = await _dbConnection.ExecuteScalarAsync<string>(columnListSql);

            // Step 3: Build the dynamic SQL query
            string sql = $@"
        SELECT VehicleID, VehicleTypeID, FuelTypeID, AssignDriverID, InstituteID, {columnList}
        FROM (
            SELECT 
                VM.VehicleID, 
                VM.VehicleNumber, 
                VM.VehicleModel, 
                VM.RegistrationYear, 
                VM.VehicleTypeID, 
                VT.Vehicle_type_name as VehicleTypeName, 
                VM.FuelTypeID, 
                FT.Fuel_type_name as FuelTypeName, 
                VM.SeatingCapacity, 
                VM.ChassieNo, 
                VM.InsurancePolicyNo, 
                CONVERT(VARCHAR, VM.RenewalDate, 105) AS RenewalDate,  -- Format as DD-MM-YYYY 
                VM.AssignDriverID, 
                EP.First_Name + ' ' + EP.Last_Name as AssignDriverName, 
                VM.GPSIMEINo, 
                VM.TrackingID, 
                VM.InstituteID, 
                VM.IsActive
            FROM 
                tblVehicleMaster VM
            LEFT OUTER JOIN 
                tbl_Vehicle_Type VT ON VM.VehicleTypeID = VT.Vehicle_type_id
            LEFT OUTER JOIN 
                tbl_Fuel_Type FT ON VM.FuelTypeID = FT.Fuel_type_id 
            LEFT OUTER JOIN 
                tbl_EmployeeProfileMaster EP ON EP.Employee_id = VM.AssignDriverID 
            WHERE 
                VM.InstituteID = @InstituteID 
                AND (VM.VehicleNumber LIKE @SearchText OR VM.VehicleModel LIKE @SearchText OR @SearchText IS NULL)
            ORDER BY 
                VM.VehicleID 
            OFFSET @Offset ROWS 
            FETCH NEXT @PageSize ROWS ONLY
        ) AS db;
    ";

            // Step 4: Execute the dynamic SQL
            var vehicles = await _dbConnection.QueryAsync<Vehicle>(sql, new
            {
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize,
                request.InstituteID,
                SearchText = $"%{request.SearchText}%"
            });

            // Step 5: Check if any vehicles were found
            if (vehicles.Any())
            {
                // Fetch documents for each vehicle
                foreach (var data in vehicles)
                {
                    data.VehicleDocuments = GetListOfVehiclesDocument(data.VehicleID);
                }

                // Return the response with total count and vehicle data
                return new ServiceResponse<IEnumerable<Vehicle>>(true, "Records Found", vehicles, StatusCodes.Status200OK, totalCount);
            }
            else
            {
                return new ServiceResponse<IEnumerable<Vehicle>>(false, "No Records Found", null, StatusCodes.Status204NoContent);
            }
        }

        public async Task<ServiceResponse<Vehicle>> GetVehicleById(int VehicleID)
        {
            string sql = @"SELECT 
                    VM.VehicleID, VM.VehicleNumber, VM.VehicleModel, VM.RegistrationYear, VM.VehicleTypeID, VT.Vehicle_type_name as VehicleTypeName, VM.FuelTypeID, FT.Fuel_type_name as FuelTypeName, VM.SeatingCapacity, VM.ChassieNo, VM.InsurancePolicyNo, CONVERT(VARCHAR, VM.RenewalDate, 105) AS RenewalDate, VM.AssignDriverID, EP.First_Name + ' ' + EP.Last_Name as AssignDriverName, VM.GPSIMEINo, VM.TrackingID, VM.InstituteID, VM.IsActive
                    FROM tblVehicleMaster VM
                    Left Outer Join tbl_Vehicle_Type VT ON VM.VehicleTypeID = VT.Vehicle_type_id
                    Left Outer Join tbl_Fuel_Type FT ON VM.FuelTypeID = FT.Fuel_type_id 
                    Left Outer Join tbl_EmployeeProfileMaster EP ON EP.Employee_id = VM.AssignDriverID 
                    WHERE VM.VehicleID = @VehicleID";
            var vehicle = await _dbConnection.QueryFirstOrDefaultAsync<Vehicle>(sql, new { VehicleId = VehicleID });

            if (vehicle != null)
            {
                vehicle.VehicleDocuments = GetListOfVehiclesDocument(vehicle.VehicleID);

                // Adding TotalCount = 1 since it's fetching one vehicle
                return new ServiceResponse<Vehicle>(true, "Record Found", vehicle, StatusCodes.Status200OK, 1);
            }
            else
            {
                return new ServiceResponse<Vehicle>(false, "Record Not Found", null, StatusCodes.Status204NoContent, 0);
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
         
        public async Task<ServiceResponse<byte[]>> ExportExcel(GetAllExportVehiclesRequest request)
        {
            // Log the input parameters for debugging
            Console.WriteLine($"InstituteID: {request.InstituteID}");

            // Step 1: Fetch the active column names from tblVehicleColumnSetting
            string columnListSql = @"SELECT STRING_AGG(DatabaseFieldName, ', ') 
                             FROM tblVehicleColumnSetting
                             WHERE IsActive = 1";
            string columnList = await _dbConnection.ExecuteScalarAsync<string>(columnListSql);

            if (string.IsNullOrEmpty(columnList))
            {
                Console.WriteLine("No active columns found to export.");
                return new ServiceResponse<byte[]>(false, "No columns found to export", null, StatusCodes.Status204NoContent);
            }

            // Step 2: Build the dynamic SQL query using only active columns
            string sql = $@"
    SELECT {columnList}
    FROM (
        SELECT 
            VM.VehicleID, 
            VM.VehicleNumber, 
            VM.VehicleModel, 
            VM.RegistrationYear, 
            VM.VehicleTypeID, 
            VT.Vehicle_type_name AS VehicleTypeName, 
            VM.FuelTypeID, 
            FT.Fuel_type_name AS FuelTypeName, 
            VM.SeatingCapacity, 
            VM.ChassieNo, 
            VM.InsurancePolicyNo, 
            VM.RenewalDate, 
            VM.AssignDriverID, 
            EP.First_Name + ' ' + EP.Last_Name AS AssignDriverName, 
            VM.GPSIMEINo, 
            VM.TrackingID, 
            VM.InstituteID, 
            VM.IsActive
        FROM 
            tblVehicleMaster VM
        LEFT OUTER JOIN 
            tbl_Vehicle_Type VT ON VM.VehicleTypeID = VT.Vehicle_type_id
        LEFT OUTER JOIN 
            tbl_Fuel_Type FT ON VM.FuelTypeID = FT.Fuel_type_id 
        LEFT OUTER JOIN 
            tbl_EmployeeProfileMaster EP ON EP.Employee_id = VM.AssignDriverID 
        WHERE 
            VM.InstituteID = @InstituteID
    ) AS db
    ORDER BY VehicleID;
    ";

            // Log the final SQL query for debugging
            Console.WriteLine("Generated SQL Query: " + sql);

            // Step 3: Execute the SQL query
            var vehicles = await _dbConnection.QueryAsync<dynamic>(sql, new
            {
                request.InstituteID
            });

            // Step 4: Check if data is returned
            if (vehicles == null || !vehicles.Any())
            {
                Console.WriteLine("No records found for InstituteID: " + request.InstituteID);
                return new ServiceResponse<byte[]>(false, "No records found", null, StatusCodes.Status204NoContent);
            }

            // Step 5: Create Excel using EPPlus
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Vehicles");

                // Step 6: Add headers dynamically
                var firstVehicle = vehicles.FirstOrDefault();
                if (firstVehicle != null)
                {
                    // Dynamically build headers based on the actual column names from the query
                    var properties = ((IDictionary<string, object>)firstVehicle).Keys;
                    int colIndex = 1;
                    foreach (var property in properties)
                    {
                        worksheet.Cells[1, colIndex++].Value = property;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid data format: missing column names.");
                    return new ServiceResponse<byte[]>(false, "Invalid data format: missing column names", null, StatusCodes.Status204NoContent);
                }

                // Step 7: Add the data rows dynamically
                var rowIndex = 2;
                foreach (var vehicle in vehicles)
                {
                    var vehicleData = (IDictionary<string, object>)vehicle;
                    var colIndex = 1;
                    foreach (var value in vehicleData.Values)
                    {
                        worksheet.Cells[rowIndex, colIndex++].Value = value?.ToString() ?? string.Empty;
                    }
                    rowIndex++;
                }

                // Step 8: Auto-fit the columns to adjust their widths
                worksheet.Cells.AutoFitColumns();

                // Step 9: Convert the Excel package to a byte array and return the response
                var excelFile = package.GetAsByteArray();
                return new ServiceResponse<byte[]>(true, "Excel file generated successfully", excelFile, StatusCodes.Status200OK);
            }
        }

        public async Task<ServiceResponse<byte[]>> ExportCSV(GetAllExportVehiclesRequest request)
        {
            // Log the input parameters for debugging
            Console.WriteLine($"InstituteID: {request.InstituteID}");

            // Step 1: Fetch the active column names from tblVehicleColumnSetting
            string columnListSql = @"SELECT STRING_AGG(DatabaseFieldName, ', ') 
                             FROM tblVehicleColumnSetting
                             WHERE IsActive = 1";
            string columnList = await _dbConnection.ExecuteScalarAsync<string>(columnListSql);

            if (string.IsNullOrEmpty(columnList))
            {
                Console.WriteLine("No active columns found to export.");
                return new ServiceResponse<byte[]>(false, "No columns found to export", null, StatusCodes.Status204NoContent);
            }

            // Step 2: Build the dynamic SQL query using only active columns
            string sql = $@"
    SELECT {columnList}
    FROM (
        SELECT 
            VM.VehicleID, 
            VM.VehicleNumber, 
            VM.VehicleModel, 
            VM.RegistrationYear, 
            VM.VehicleTypeID, 
            VT.Vehicle_type_name AS VehicleTypeName, 
            VM.FuelTypeID, 
            FT.Fuel_type_name AS FuelTypeName, 
            VM.SeatingCapacity, 
            VM.ChassieNo, 
            VM.InsurancePolicyNo, 
            VM.RenewalDate, 
            VM.AssignDriverID, 
            EP.First_Name + ' ' + EP.Last_Name AS AssignDriverName, 
            VM.GPSIMEINo, 
            VM.TrackingID, 
            VM.InstituteID, 
            VM.IsActive
        FROM 
            tblVehicleMaster VM
        LEFT OUTER JOIN 
            tbl_Vehicle_Type VT ON VM.VehicleTypeID = VT.Vehicle_type_id
        LEFT OUTER JOIN 
            tbl_Fuel_Type FT ON VM.FuelTypeID = FT.Fuel_type_id 
        LEFT OUTER JOIN 
            tbl_EmployeeProfileMaster EP ON EP.Employee_id = VM.AssignDriverID 
        WHERE 
            VM.InstituteID = @InstituteID
    ) AS db
    ORDER BY VehicleID;
    ";

            // Log the final SQL query for debugging
            Console.WriteLine("Generated SQL Query: " + sql);

            // Step 3: Execute the SQL query
            var vehicles = await _dbConnection.QueryAsync<dynamic>(sql, new
            {
                request.InstituteID
            });

            // Step 4: Check if data is returned
            if (vehicles == null || !vehicles.Any())
            {
                Console.WriteLine("No records found for InstituteID: " + request.InstituteID);
                return new ServiceResponse<byte[]>(false, "No records found", null, StatusCodes.Status204NoContent);
            }

            // Step 5: Convert data to CSV format
            var csvBuilder = new StringBuilder();

            // Step 6: Add headers dynamically
            var firstVehicle = vehicles.FirstOrDefault();
            if (firstVehicle != null)
            {
                // Dynamically build headers based on the actual column names from the query
                var headerColumns = ((IDictionary<string, object>)firstVehicle).Keys;
                csvBuilder.AppendLine(string.Join(",", headerColumns));
            }
            else
            {
                Console.WriteLine("Invalid data format: missing column names.");
                return new ServiceResponse<byte[]>(false, "Invalid data format: missing column names", null, StatusCodes.Status204NoContent);
            }

            // Step 7: Add the data rows dynamically
            foreach (var vehicle in vehicles)
            {
                var vehicleData = (IDictionary<string, object>)vehicle;
                var row = string.Join(",", vehicleData.Values.Select(value => value?.ToString() ?? string.Empty));
                csvBuilder.AppendLine(row);
            }

            // Step 8: Convert the CSV content to a byte array and return the response
            var csvContent = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            return new ServiceResponse<byte[]>(true, "CSV file generated successfully", csvContent, StatusCodes.Status200OK);
        }

    }
}
