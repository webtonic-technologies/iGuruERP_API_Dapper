using Dapper;
using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;
using Transport_API.Repository.Interfaces;
using System.Data;
using System.Text.RegularExpressions;
using Transport_API.DTOs.Response;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;

namespace Transport_API.Repository.Implementations
{
    public class VehicleMaintenanceRepository : IVehicleMaintenanceRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public VehicleMaintenanceRepository(IDbConnection dbConnection, IWebHostEnvironment hostingEnvironment)
        {
            _dbConnection = dbConnection;
            _hostingEnvironment = hostingEnvironment;

        }

        public async Task<ServiceResponse<string>> AddUpdateVehicleExpense(VehicleExpenseRequest vehicleExpense)
        {
            string sql;
            if (vehicleExpense.VehicleExpenseID == 0)
            {
                sql = @"INSERT INTO tblVehicleExpense (ExpenseTypeID, ExpenseDate, Cost, Remarks, VehicleID, InstituteID) 
                        VALUES (@ExpenseTypeID, @ExpenseDate, @Cost, @Remarks, @VehicleID, @InstituteID)
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";


                int result = await _dbConnection.QueryFirstOrDefaultAsync<int>(sql, new { vehicleExpense.ExpenseTypeID, vehicleExpense.ExpenseDate, vehicleExpense.Cost, vehicleExpense.Remarks, vehicleExpense.VehicleID, vehicleExpense.InstituteID });

                if (result > 0)
                {
                    int response = VehicleDocumentMapping(vehicleExpense.VehicleExpenseDocuments, result);

                    return new ServiceResponse<string>(true, "Operation Successful", "Vehicle added/updated successfully", StatusCodes.Status200OK);

                }
                else
                {
                    return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating vehicle", StatusCodes.Status400BadRequest);
                }

            }
            else
            {
                sql = @"UPDATE tblVehicleExpense SET ExpenseTypeID = @ExpenseTypeID, ExpenseDate = @ExpenseDate, 
                        Cost = @Cost, Remarks = @Remarks, VehicleID = @VehicleID , InstituteID = @InstituteID WHERE VehicleExpenseID = @VehicleExpenseID";

                int result = await _dbConnection.ExecuteAsync(sql, new { vehicleExpense.ExpenseTypeID, vehicleExpense.ExpenseDate, vehicleExpense.Cost, vehicleExpense.Remarks, vehicleExpense.VehicleID, vehicleExpense.InstituteID });

                if (result > 0)
                {
                    int response = VehicleDocumentMapping(vehicleExpense.VehicleExpenseDocuments, vehicleExpense.VehicleID);

                    return new ServiceResponse<string>(true, "Operation Successful", "Vehicle added/updated successfully", StatusCodes.Status200OK);

                }
                else
                {
                    return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating vehicle", StatusCodes.Status400BadRequest);
                }
            }

            
        }

        public async Task<ServiceResponse<IEnumerable<VehicleExpenseResponse>>> GetAllVehicleExpenses(GetAllExpenseRequest request)
        {
            //string countSql = @"SELECT COUNT(*) FROM tblVehicleExpense";
 

            //string sql = @"SELECT * FROM tblVehicleExpense ORDER BY VehicleExpenseID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            string sql = @"Select
                        VE.VehicleExpenseID, VE.VehicleID, VM.VehicleNumber, ET.VehicleExpenseType, SUM(VE.Cost) AS TotalCost
                        from tblVehicleExpense VE
                        Left Outer Join tblVehicleMaster VM ON VM.VehicleID = VE.VehicleID
                        Left Outer Join tblVehicleExpenseType ET ON ET.VehicleExpenseTypeID = VE.ExpenseTypeID
                        WHERE VE.InstituteID = @InstituteID
                        GROUP BY VE.VehicleExpenseID, VM.VehicleNumber, ET.VehicleExpenseType, VE.VehicleID 
                        Order by VE.VehicleExpenseID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
              

            var vehicleExpenses = await _dbConnection.QueryAsync<VehicleExpenseResponse>(sql, new { Offset = (request.PageNumber - 1) * request.PageSize, PageSize = request.PageSize, request.InstituteID });

            if (vehicleExpenses.Any())
            {
                return new ServiceResponse<IEnumerable<VehicleExpenseResponse>>(true, "Records Found", vehicleExpenses, StatusCodes.Status200OK, vehicleExpenses.Count());
            }
            else
            {
                return new ServiceResponse<IEnumerable<VehicleExpenseResponse>>(false, "No Records Found", null, StatusCodes.Status204NoContent);
            }
        }

        public async Task<ServiceResponse<VehicleExpense>> GetVehicleExpenseById(int VehicleID)
        {
            //string sql = @"SELECT * FROM tblVehicleExpense WHERE VehicleExpenseID = @VehicleExpenseID";
            string sql = @"SELECT  
                        VM.VehicleID, VM.VehicleModel, FT.Fuel_type_name, SUM(VE.Cost) AS TotalCost
                        from tblVehicleMaster VM
                        Left Outer Join tblVehicleExpense VE ON VE.VehicleID = VM.VehicleID
                        Left Outer Join tbl_Fuel_Type FT ON FT.Fuel_type_id = VE.ExpenseTypeID
                        where VM.VehicleID = @VehicleExpenseID
                        GROUP BY VM.VehicleModel, FT.Fuel_type_name, VM.VehicleID";

            var vehicleExpense = await _dbConnection.QueryFirstOrDefaultAsync<VehicleExpense>(sql, new { VehicleExpenseID = VehicleID });

            if (vehicleExpense != null)
            {
                vehicleExpense.VehicleExpenseDocuments = GetListOfVehiclesExpenseDocument(vehicleExpense.VehicleExpenseID);

                return new ServiceResponse<VehicleExpense>(true, "Record Found", vehicleExpense, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<VehicleExpense>(false, "Record Not Found", null, StatusCodes.Status204NoContent);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteVehicleExpense(int VehicleExpenseID)
        {
            string sql = @"Delete tblVehicleExpense WHERE VehicleExpenseID = @VehicleExpenseID";
            var result = await _dbConnection.ExecuteAsync(sql, new { VehicleExpenseID = VehicleExpenseID });

            if (result > 0)
            {
                return new ServiceResponse<bool>(true, "Record Deleted Successfully", true, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<bool>(false, "Delete Failed", false, StatusCodes.Status400BadRequest);
            }
        }

        private int VehicleDocumentMapping(List<VehicleExpenseDocumentRequest> requests, int VehicleExpenseID)
        {
            foreach (var data in requests)
            {
                data.VehicleExpenseDocument = PDFUpload(data.VehicleExpenseDocument);
                data.VehicleExpenseID = VehicleExpenseID;
            }

            string query = "SELECT COUNT(*) FROM [tblVehicleExpenseDocument] WHERE [VehicleExpenseID] = @VehicleExpenseID";
            int count = _dbConnection.QueryFirstOrDefault<int>(query, new { VehicleExpenseID });
            if (count > 0)
            {
                var deleteDuery = @"DELETE FROM [tblVehicleExpenseDocument]
                    WHERE [VehicleID] = @VehicleID;";
                var rowsAffected = _dbConnection.Execute(deleteDuery, new { VehicleExpenseID });
                if (rowsAffected > 0)
                {
                    var insertquery = @"INSERT INTO [tblVehicleExpenseDocument] ([VehicleExpenseID], [VehicleExpenseDocument])
                    VALUES (@VehicleExpenseID, @VehicleExpenseDocument);";
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
                var insertquery = @"INSERT INTO [tblVehicleExpenseDocument] ([VehicleExpenseID], [VehicleExpenseDocument])
                    VALUES (@VehicleExpenseID, @VehicleExpenseDocument);";
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
            string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "VehicleExpenseDcoumentsPDF");

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

        private List<VehicleExpenseDocumentRequest> GetListOfVehiclesExpenseDocument(int VehicleExpenseID)
        {
            string boardQuery = @"
            select * From tblVehicleExpenseDocument
            WHERE VehicleExpenseID = @VehicleExpenseID";

            // Execute the SQL query with the SOTDID parameter
            var data = _dbConnection.Query<VehicleExpenseDocumentRequest>(boardQuery, new { VehicleExpenseID });
            foreach (var item in data)
            {
                item.VehicleExpenseDocument = GetPDF(item.VehicleExpenseDocument);
            }
            return data != null ? data.AsList() : [];
        }

        private string GetPDF(string Filename)
        {
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "VehicleExpenseDcoumentsPDF", Filename);
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
