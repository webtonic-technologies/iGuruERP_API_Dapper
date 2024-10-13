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
using Transport_API.DTOs.Requests.Transport_API.DTOs.Requests;
using System.Globalization;

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
                // Insert operation
                sql = @"INSERT INTO tblVehicleExpense (VehicleID, VehicleExpenseTypeID, ExpenseDate, Cost, Remarks, InstituteID, IsActive)
                VALUES (@VehicleID, @VehicleExpenseTypeID, @ExpenseDate, @Cost, @Remarks, @InstituteID, @IsActive);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                int result = await _dbConnection.QueryFirstOrDefaultAsync<int>(sql, new
                {
                    vehicleExpense.VehicleID,
                    vehicleExpense.VehicleExpenseTypeID,
                    vehicleExpense.ExpenseDate,
                    vehicleExpense.Cost,
                    vehicleExpense.Remarks,
                    vehicleExpense.InstituteID,
                    vehicleExpense.IsActive
                });

                if (result > 0)
                {
                    // Insert attachments if there are any
                    if (vehicleExpense.Attachments != null && vehicleExpense.Attachments.Any())
                    {
                        foreach (var doc in vehicleExpense.Attachments)
                        {
                            await AddVehicleExpenseDocument(result, doc.Attachment);
                        }
                    }

                    return new ServiceResponse<string>(true, "Vehicle expense added successfully", null, StatusCodes.Status200OK);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Failed to add vehicle expense", null, StatusCodes.Status400BadRequest);
                }
            }
            else
            {
                // Update operation
                sql = @"UPDATE tblVehicleExpense 
                SET VehicleID = @VehicleID, 
                    VehicleExpenseTypeID = @VehicleExpenseTypeID, 
                    ExpenseDate = @ExpenseDate, 
                    Cost = @Cost, 
                    Remarks = @Remarks, 
                    InstituteID = @InstituteID, 
                    IsActive = @IsActive
                WHERE VehicleExpenseID = @VehicleExpenseID";

                int result = await _dbConnection.ExecuteAsync(sql, new
                {
                    vehicleExpense.VehicleID,
                    vehicleExpense.VehicleExpenseTypeID,
                    vehicleExpense.ExpenseDate,
                    vehicleExpense.Cost,
                    vehicleExpense.Remarks,
                    vehicleExpense.InstituteID,
                    vehicleExpense.IsActive,
                    vehicleExpense.VehicleExpenseID
                });

                if (result > 0)
                {
                    // Update attachments if there are any
                    if (vehicleExpense.Attachments != null && vehicleExpense.Attachments.Any())
                    {
                        foreach (var doc in vehicleExpense.Attachments)
                        {
                            await AddVehicleExpenseDocument(vehicleExpense.VehicleExpenseID, doc.Attachment);
                        }
                    }

                    return new ServiceResponse<string>(true, "Vehicle expense updated successfully", null, StatusCodes.Status200OK);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Failed to update vehicle expense", null, StatusCodes.Status400BadRequest);
                }
            }
        }

        // Helper method to handle attachments
        private async Task AddVehicleExpenseDocument(int vehicleExpenseID, string base64Document)
        {
            string filePath = SaveDocumentToDisk(base64Document); // You can implement your method to save the base64 to disk

            string sql = @"INSERT INTO tblVehicleExpenseDocument (VehicleExpenseID, VehicleExpenseDocument)
                   VALUES (@VehicleExpenseID, @VehicleExpenseDocument);";

            await _dbConnection.ExecuteAsync(sql, new { VehicleExpenseID = vehicleExpenseID, VehicleExpenseDocument = filePath });
        }

        // Simulate saving document to disk using ContentRootPath
        private string SaveDocumentToDisk(string base64Document)
        {
            // Use ContentRootPath instead of WebRootPath
            string rootPath = _hostingEnvironment.ContentRootPath;

            // Define the path where files will be stored inside the Assets folder
            string directoryPath = Path.Combine(rootPath, "Assets", "VehicleDocuments");

            // Ensure the directory exists
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Create a unique file name
            string fileName = Guid.NewGuid().ToString() + ".pdf";  // Assuming document is in PDF format
            string filePath = Path.Combine(directoryPath, fileName);

            // Convert the base64 string to bytes and write the file to disk
            byte[] documentBytes = Convert.FromBase64String(base64Document);
            File.WriteAllBytes(filePath, documentBytes);

            return filePath; // Return the file path for saving to the database
        }



        public async Task<ServiceResponse<IEnumerable<GetAllExpenseResponse>>> GetAllVehicleExpenses(GetAllExpenseRequest request)
        {
            // Parse StartDate and EndDate from DD-MM-YYYY string format to DateTime
            DateTime startDate;
            DateTime endDate;

            if (!DateTime.TryParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate) ||
                !DateTime.TryParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                return new ServiceResponse<IEnumerable<GetAllExpenseResponse>>(false, "Invalid date format. Please use DD-MM-YYYY.", null, StatusCodes.Status400BadRequest);
            }

            // SQL to retrieve expense records with filters and pagination
            string sql = @"
    SELECT 
        ve.VehicleExpenseID, -- Include VehicleExpenseID to fetch documents correctly
        ve.VehicleID, 
        vm.VehicleNumber, 
        vet.VehicleExpenseType, 
        ve.ExpenseDate, 
        ve.Remarks, 
        ve.Cost AS Amount
    FROM 
        tblVehicleExpense ve
    JOIN 
        tblVehicleMaster vm ON ve.VehicleID = vm.VehicleID
    JOIN 
        tblVehicleExpenseType vet ON ve.VehicleExpenseTypeID = vet.VehicleExpenseTypeID
    WHERE 
        ve.InstituteID = @InstituteID
        AND ve.ExpenseDate BETWEEN @StartDate AND @EndDate
        AND (@VehicleID IS NULL OR ve.VehicleID = @VehicleID)
        AND (@ExpenseTypeID IS NULL OR ve.VehicleExpenseTypeID = @ExpenseTypeID)
    ORDER BY 
        ve.ExpenseDate
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    ";

            var expenses = await _dbConnection.QueryAsync<GetAllExpenseResponse>(sql, new
            {
                InstituteID = request.InstituteID,
                StartDate = startDate, // Parsed DateTime
                EndDate = endDate,     // Parsed DateTime
                VehicleID = request.VehicleID,
                ExpenseTypeID = request.ExpenseTypeID,
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            });

            foreach (var expense in expenses)
            {
                // Fetch associated documents for each expense using the VehicleExpenseID
                string documentSql = @"SELECT VehicleExpenseDocument FROM tblVehicleExpenseDocument WHERE VehicleExpenseID = @VehicleExpenseID";
                var documents = await _dbConnection.QueryAsync<string>(documentSql, new { VehicleExpenseID = expense.VehicleExpenseID });
                expense.Documents = documents.ToList();
            }

            if (expenses.Any())
            {
                return new ServiceResponse<IEnumerable<GetAllExpenseResponse>>(true, "Records Found", expenses, StatusCodes.Status200OK, expenses.Count());
            }
            else
            {
                return new ServiceResponse<IEnumerable<GetAllExpenseResponse>>(false, "No Records Found", null, StatusCodes.Status204NoContent);
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
            // Ensure the documents have valid VehicleExpenseID set

            foreach (var data in requests)
            {
                var uploadedFilePath = PDFUpload(data.Attachment);

                // Ensure that the upload was successful
                if (string.IsNullOrEmpty(uploadedFilePath))
                {
                    throw new Exception("Document upload failed. Please check the base64 string or file saving process.");
                }

                data.Attachment = uploadedFilePath;  // Store the file path returned by PDFUpload
                data.VehicleExpenseID = VehicleExpenseID;
            }

            // Check if there are already documents for this VehicleExpenseID
            string query = "SELECT COUNT(*) FROM [tblVehicleExpenseDocument] WHERE [VehicleExpenseID] = @VehicleExpenseID";
            int count = _dbConnection.QueryFirstOrDefault<int>(query, new { VehicleExpenseID });

            if (count > 0)
            {
                // Delete existing documents before adding new ones
                var deleteQuery = @"DELETE FROM [tblVehicleExpenseDocument] WHERE [VehicleExpenseID] = @VehicleExpenseID;";
                var rowsAffected = _dbConnection.Execute(deleteQuery, new { VehicleExpenseID });
            }

            // Insert new documents for this VehicleExpenseID
            var insertQuery = @"INSERT INTO [tblVehicleExpenseDocument] ([VehicleExpenseID], [VehicleExpenseDocument]) 
                        VALUES (@VehicleExpenseID, @Attachment);";

            // Using batch insertion for all requests
            var valuesInserted = 0;
            foreach (var document in requests)
            {
                valuesInserted += _dbConnection.Execute(insertQuery, new
                {
                    VehicleExpenseID = document.VehicleExpenseID,
                    Attachment = document.Attachment // Assuming base64 string converted and returned by PDFUpload
                });
            }

            return valuesInserted;
        }


        private string PDFUpload(string base64String)
        {
            // Check if the input is null or invalid
            if (string.IsNullOrEmpty(base64String) || base64String == "string")
            {
                throw new ArgumentException("Invalid base64 string for document upload.");
            }

            try
            {
                // Convert base64 string to byte array
                byte[] fileData = Convert.FromBase64String(base64String);

                // Generate a unique file name
                string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "VehicleExpenseDocumentsPDF");

                // Ensure directory exists
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Generate a unique file name with extension
                string fileName = $"{Guid.NewGuid()}.pdf";
                string filePath = Path.Combine(directoryPath, fileName);

                // Save the file to the directory
                File.WriteAllBytes(filePath, fileData);

                // Return the saved file path (or a relative URL if needed)
                return filePath; // Or return a URL if needed: new Uri(filePath).AbsoluteUri;
            }
            catch (Exception ex)
            {
                // Log the error and return null (or rethrow the exception)
                // Logging ex.Message for further debugging
                return null; // Return null to indicate failure
            }
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
                item.Attachment = GetPDF(item.Attachment);
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
