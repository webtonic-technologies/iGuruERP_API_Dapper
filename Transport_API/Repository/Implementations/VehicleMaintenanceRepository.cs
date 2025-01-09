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
using Transport_API.DTOs.Responses;
using System.Text;

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


        //public async Task<ServiceResponse<string>> AddUpdateVehicleExpense(VehicleExpenseRequest vehicleExpense)
        //{
        //    string sql;

        //    // Convert ExpenseDate to DateTime
        //    DateTime expenseDate;
        //    if (!DateTime.TryParseExact(vehicleExpense.ExpenseDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out expenseDate))
        //    {
        //        return new ServiceResponse<string>(false, "Invalid date format. Please use dd-MM-yyyy.", null, StatusCodes.Status400BadRequest);
        //    }

        //    if (vehicleExpense.VehicleExpenseID == 0)
        //    {
        //        // Insert operation
        //        sql = @"INSERT INTO tblVehicleExpense (VehicleID, VehicleExpenseTypeID, ExpenseDate, Cost, Remarks, InstituteID, IsActive)
        //        VALUES (@VehicleID, @VehicleExpenseTypeID, @ExpenseDate, @Cost, @Remarks, @InstituteID, @IsActive);
        //        SELECT CAST(SCOPE_IDENTITY() AS INT);";

        //        int result = await _dbConnection.QueryFirstOrDefaultAsync<int>(sql, new
        //        {
        //            vehicleExpense.VehicleID,
        //            vehicleExpense.VehicleExpenseTypeID,
        //            ExpenseDate = expenseDate, // use the converted DateTime here
        //            vehicleExpense.Cost,
        //            vehicleExpense.Remarks,
        //            vehicleExpense.InstituteID,
        //            vehicleExpense.IsActive
        //        });

        //        if (result > 0)
        //        {
        //            // Insert attachments if there are any
        //            if (vehicleExpense.Attachments != null && vehicleExpense.Attachments.Any())
        //            {
        //                foreach (var doc in vehicleExpense.Attachments)
        //                {
        //                    await AddVehicleExpenseDocument(result, doc.Attachment);
        //                }
        //            }

        //            return new ServiceResponse<string>(true, "Vehicle expense added successfully", null, StatusCodes.Status200OK);
        //        }
        //        else
        //        {
        //            return new ServiceResponse<string>(false, "Failed to add vehicle expense", null, StatusCodes.Status400BadRequest);
        //        }
        //    }
        //    else
        //    {
        //        // Update operation
        //        sql = @"UPDATE tblVehicleExpense 
        //        SET VehicleID = @VehicleID, 
        //            VehicleExpenseTypeID = @VehicleExpenseTypeID, 
        //            ExpenseDate = @ExpenseDate, 
        //            Cost = @Cost, 
        //            Remarks = @Remarks, 
        //            InstituteID = @InstituteID, 
        //            IsActive = @IsActive
        //        WHERE VehicleExpenseID = @VehicleExpenseID";

        //        int result = await _dbConnection.ExecuteAsync(sql, new
        //        {
        //            vehicleExpense.VehicleID,
        //            vehicleExpense.VehicleExpenseTypeID,
        //            ExpenseDate = expenseDate, // use the converted DateTime here
        //            vehicleExpense.Cost,
        //            vehicleExpense.Remarks,
        //            vehicleExpense.InstituteID,
        //            vehicleExpense.IsActive,
        //            vehicleExpense.VehicleExpenseID
        //        });

        //        if (result > 0)
        //        {
        //            // Update attachments if there are any
        //            if (vehicleExpense.Attachments != null && vehicleExpense.Attachments.Any())
        //            {
        //                foreach (var doc in vehicleExpense.Attachments)
        //                {
        //                    await AddVehicleExpenseDocument(vehicleExpense.VehicleExpenseID, doc.Attachment);
        //                }
        //            }

        //            return new ServiceResponse<string>(true, "Vehicle expense updated successfully", null, StatusCodes.Status200OK);
        //        }
        //        else
        //        {
        //            return new ServiceResponse<string>(false, "Failed to update vehicle expense", null, StatusCodes.Status400BadRequest);
        //        }
        //    }
        //}

        //// Helper method to handle attachments
        //private async Task AddVehicleExpenseDocument(int vehicleExpenseID, string base64Document)
        //{
        //    string filePath = SaveDocumentToDisk(base64Document); // You can implement your method to save the base64 to disk

        //    string sql = @"INSERT INTO tblVehicleExpenseDocument (VehicleExpenseID, VehicleExpenseDocument)
        //           VALUES (@VehicleExpenseID, @VehicleExpenseDocument);";

        //    await _dbConnection.ExecuteAsync(sql, new { VehicleExpenseID = vehicleExpenseID, VehicleExpenseDocument = filePath });
        //}



        public async Task<ServiceResponse<string>> AddUpdateVehicleExpense(VehicleExpenseRequest vehicleExpense)
        {
            try
            {
                // Convert ExpenseDate to DateTime
                DateTime expenseDate;
                if (!DateTime.TryParseExact(vehicleExpense.ExpenseDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out expenseDate))
                {
                    return new ServiceResponse<string>(false, "Invalid date format. Please use DD-MM-YYYY.", null, 400);
                }

                string sql;
                int result;

                if (vehicleExpense.VehicleExpenseID == 0) // Insert operation
                {
                    sql = @"INSERT INTO tblVehicleExpense (VehicleID, VehicleExpenseTypeID, ExpenseDate, Cost, Remarks, InstituteID, IsActive)
                    VALUES (@VehicleID, @VehicleExpenseTypeID, @ExpenseDate, @Cost, @Remarks, @InstituteID, @IsActive);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    result = await _dbConnection.ExecuteScalarAsync<int>(sql, new
                    {
                        vehicleExpense.VehicleID,
                        vehicleExpense.VehicleExpenseTypeID,
                        ExpenseDate = expenseDate, // Convert the date
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

                        return new ServiceResponse<string>(true, "Vehicle expense added successfully", null, 200);
                    }
                    else
                    {
                        return new ServiceResponse<string>(false, "Failed to add vehicle expense", null, 400);
                    }
                }
                else // Update operation
                {
                    sql = @"UPDATE tblVehicleExpense 
                    SET VehicleID = @VehicleID, 
                        VehicleExpenseTypeID = @VehicleExpenseTypeID, 
                        ExpenseDate = @ExpenseDate, 
                        Cost = @Cost, 
                        Remarks = @Remarks, 
                        InstituteID = @InstituteID, 
                        IsActive = @IsActive
                    WHERE VehicleExpenseID = @VehicleExpenseID";

                    result = await _dbConnection.ExecuteAsync(sql, new
                    {
                        vehicleExpense.VehicleID,
                        vehicleExpense.VehicleExpenseTypeID,
                        ExpenseDate = expenseDate, // Convert the date
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

                        return new ServiceResponse<string>(true, "Vehicle expense updated successfully", null, 200);
                    }
                    else
                    {
                        return new ServiceResponse<string>(false, "Failed to update vehicle expense", null, 400);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, $"Error processing request: {ex.Message}", null, 500);
            }
        }

        // Helper method to handle attachments
        private async Task AddVehicleExpenseDocument(int vehicleExpenseID, string base64Document)
        {
            try
            {
                byte[] documentData = Convert.FromBase64String(base64Document); // Convert base64 string to byte array
                string sql = @"INSERT INTO tblVehicleExpenseDocument (VehicleExpenseID, VehicleExpenseDocument)
                       VALUES (@VehicleExpenseID, @VehicleExpenseDocument)";
                await _dbConnection.ExecuteAsync(sql, new
                {
                    VehicleExpenseID = vehicleExpenseID,
                    VehicleExpenseDocument = documentData // Insert the byte array into the VARBINARY column
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving document: {ex.Message}");
            }
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



        //    public async Task<ServiceResponse<IEnumerable<GetAllExpenseResponse>>> GetAllVehicleExpenses(GetAllExpenseRequest request)
        //    {
        //        // Parse StartDate and EndDate from DD-MM-YYYY string format to DateTime
        //        DateTime startDate;
        //        DateTime endDate;

        //        if (!DateTime.TryParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate) ||
        //            !DateTime.TryParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
        //        {
        //            return new ServiceResponse<IEnumerable<GetAllExpenseResponse>>(false, "Invalid date format. Please use DD-MM-YYYY.", null, StatusCodes.Status400BadRequest);
        //        }

        //        // SQL to retrieve expense records with filters and pagination, including the IsActive condition
        //        string sql = @"
        //    SELECT 
        //        ve.VehicleExpenseID, -- Include VehicleExpenseID to fetch documents correctly
        //        ve.VehicleID, 
        //        vm.VehicleNumber, 
        //        vet.VehicleExpenseType AS ExpenseType, 
        //        ve.ExpenseDate, 
        //        ve.Remarks, 
        //        ve.Cost AS Amount
        //    FROM 
        //        tblVehicleExpense ve
        //    JOIN 
        //        tblVehicleMaster vm ON ve.VehicleID = vm.VehicleID
        //    JOIN 
        //        tblVehicleExpenseType vet ON ve.VehicleExpenseTypeID = vet.VehicleExpenseTypeID
        //    WHERE 
        //        ve.InstituteID = @InstituteID
        //        AND ve.ExpenseDate BETWEEN @StartDate AND @EndDate
        //        AND ve.IsActive = 1 -- Ensure only active records are returned
        //        AND (@VehicleID IS NULL OR ve.VehicleID = @VehicleID)
        //        AND (@ExpenseTypeID IS NULL OR ve.VehicleExpenseTypeID = @ExpenseTypeID)
        //    ORDER BY 
        //        ve.ExpenseDate
        //    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        //";

        //        var expenses = await _dbConnection.QueryAsync<GetAllExpenseResponse>(sql, new
        //        {
        //            InstituteID = request.InstituteID,
        //            StartDate = startDate, // Parsed DateTime
        //            EndDate = endDate,     // Parsed DateTime
        //            VehicleID = request.VehicleID,
        //            ExpenseTypeID = request.ExpenseTypeID,
        //            Offset = (request.PageNumber - 1) * request.PageSize,
        //            PageSize = request.PageSize
        //        });

        //        // Process each expense to retrieve associated documents and format the date
        //        foreach (var expense in expenses)
        //        {
        //            // Fetch associated documents for each expense using the VehicleExpenseID
        //            string documentSql = @"SELECT VehicleExpenseDocument FROM tblVehicleExpenseDocument WHERE VehicleExpenseID = @VehicleExpenseID";
        //            var documents = await _dbConnection.QueryAsync<string>(documentSql, new { VehicleExpenseID = expense.VehicleExpenseID });
        //            expense.Documents = documents.ToList();

        //            // Convert ExpenseDate to string format 'DD-MM-YYYY'
        //            expense.ExpenseDate = DateTime.Parse(expense.ExpenseDate).ToString("dd-MM-yyyy");
        //        }

        //        // Check if any expenses were retrieved and return the appropriate response
        //        if (expenses.Any())
        //        {
        //            return new ServiceResponse<IEnumerable<GetAllExpenseResponse>>(true, "Records Found", expenses, StatusCodes.Status200OK, expenses.Count());
        //        }
        //        else
        //        {
        //            // Custom message when no records are found
        //            return new ServiceResponse<IEnumerable<GetAllExpenseResponse>>(false, "No vehicle expenses found for the given filters.", null, StatusCodes.Status404NotFound);
        //        }
        //    }


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

            // SQL to retrieve expense records with filters and pagination, including the IsActive condition
            string sql = @"
    SELECT 
        ve.VehicleExpenseID, -- Include VehicleExpenseID to fetch documents correctly
        ve.VehicleID, 
        vm.VehicleNumber, 
        vet.VehicleExpenseType AS ExpenseType, 
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
        AND ve.IsActive = 1 -- Ensure only active records are returned
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

            // Process each expense to retrieve associated documents and format the date
            foreach (var expense in expenses)
            {
                // Fetch associated documents for each expense using the VehicleExpenseID
                string documentSql = @"SELECT VehicleExpenseDocument FROM tblVehicleExpenseDocument WHERE VehicleExpenseID = @VehicleExpenseID";
                var documents = await _dbConnection.QueryAsync<byte[]>(documentSql, new { VehicleExpenseID = expense.VehicleExpenseID });

                // Convert the binary data to base64 string and add it to Documents
                expense.Documents = documents.Select(doc => Convert.ToBase64String(doc)).ToList();

                // Convert ExpenseDate to string format 'DD-MM-YYYY'
                expense.ExpenseDate = DateTime.Parse(expense.ExpenseDate).ToString("dd-MM-yyyy");
            }

            // Check if any expenses were retrieved and return the appropriate response
            if (expenses.Any())
            {
                return new ServiceResponse<IEnumerable<GetAllExpenseResponse>>(true, "Records Found", expenses, StatusCodes.Status200OK, expenses.Count());
            }
            else
            {
                // Custom message when no records are found
                return new ServiceResponse<IEnumerable<GetAllExpenseResponse>>(false, "No vehicle expenses found for the given filters.", null, StatusCodes.Status404NotFound);
            }
        }



        public async Task<ServiceResponse<GetAllExpenseResponse>> GetVehicleExpenseById(int VehicleID)
        {
            // SQL query to fetch the vehicle expense data for a specific VehicleID
            string sql = @"
        SELECT  
            ve.VehicleExpenseID, 
            ve.VehicleID, 
            vm.VehicleNumber, 
            vet.VehicleExpenseType AS ExpenseType, 
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
            ve.VehicleID = @VehicleID
            AND ve.IsActive = 1
        ORDER BY 
            ve.ExpenseDate";

            // Fetch the vehicle expense data using the provided query and VehicleID parameter
            var vehicleExpense = await _dbConnection.QueryFirstOrDefaultAsync<GetAllExpenseResponse>(sql, new { VehicleID });

            if (vehicleExpense != null)
            {
                // Map VehicleExpense data to GetAllExpenseResponse
                var expenseResponse = new GetAllExpenseResponse
                {
                    VehicleExpenseID = vehicleExpense.VehicleExpenseID,
                    VehicleID = vehicleExpense.VehicleID,
                    VehicleNumber = vehicleExpense.VehicleNumber,
                    ExpenseType = vehicleExpense.ExpenseType,
                    // Parse the date in the format of 'MM/dd/yyyy HH:mm:ss' and convert to 'dd-MM-yyyy'
                    ExpenseDate = DateTime.ParseExact(vehicleExpense.ExpenseDate, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy"),
                    Remarks = vehicleExpense.Remarks,
                    Amount = vehicleExpense.Amount
                    //Documents = GetListOfVehiclesExpenseDocument(vehicleExpense.VehicleExpenseID)  // Un-comment when implemented
                };

                // Return success response with the mapped response data
                return new ServiceResponse<GetAllExpenseResponse>(true, "Record Found", expenseResponse, StatusCodes.Status200OK);
            }
            else
            {
                // Return not found response
                return new ServiceResponse<GetAllExpenseResponse>(false, "Record Not Found", null, StatusCodes.Status204NoContent);
            }
        }



        //public async Task<ServiceResponse<VehicleExpense>> GetVehicleExpenseById(int VehicleID)
        //{
        //    //string sql = @"SELECT * FROM tblVehicleExpense WHERE VehicleExpenseID = @VehicleExpenseID";
        //    string sql = @"SELECT  
        //                VM.VehicleID, VM.VehicleModel, FT.Fuel_type_name, SUM(VE.Cost) AS TotalCost
        //                from tblVehicleMaster VM
        //                Left Outer Join tblVehicleExpense VE ON VE.VehicleID = VM.VehicleID
        //                Left Outer Join tbl_Fuel_Type FT ON FT.Fuel_type_id = VE.VehicleExpenseTypeID
        //                where VM.VehicleID = @VehicleExpenseID
        //                GROUP BY VM.VehicleModel, FT.Fuel_type_name, VM.VehicleID";

        //    var vehicleExpense = await _dbConnection.QueryFirstOrDefaultAsync<VehicleExpense>(sql, new { VehicleExpenseID = VehicleID });

        //    if (vehicleExpense != null)
        //    {
        //        vehicleExpense.VehicleExpenseDocuments = GetListOfVehiclesExpenseDocument(vehicleExpense.VehicleExpenseID);

        //        return new ServiceResponse<VehicleExpense>(true, "Record Found", vehicleExpense, StatusCodes.Status200OK);
        //    }
        //    else
        //    {
        //        return new ServiceResponse<VehicleExpense>(false, "Record Not Found", null, StatusCodes.Status204NoContent);
        //    }
        //}

        public async Task<ServiceResponse<bool>> DeleteVehicleExpense(int VehicleExpenseID)
        {
            // SQL to update IsActive field to 0 (soft delete)
            string sql = @"UPDATE tblVehicleExpense 
                   SET IsActive = 0 
                   WHERE VehicleExpenseID = @VehicleExpenseID";

            var result = await _dbConnection.ExecuteAsync(sql, new { VehicleExpenseID = VehicleExpenseID });

            if (result > 0)
            {
                return new ServiceResponse<bool>(true, "Record Soft Deleted Successfully", true, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<bool>(false, "Soft Delete Failed", false, StatusCodes.Status400BadRequest);
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

        //private List<VehicleExpenseDocumentRequest> GetListOfVehiclesExpenseDocument(int VehicleExpenseID)
        //{
        //    string boardQuery = @"
        //    select * From tblVehicleExpenseDocument
        //    WHERE VehicleExpenseID = @VehicleExpenseID";

        //    // Execute the SQL query with the SOTDID parameter
        //    var data = _dbConnection.Query<VehicleExpenseDocumentRequest>(boardQuery, new { VehicleExpenseID });
        //    foreach (var item in data)
        //    {
        //        item.Attachment = GetPDF(item.Attachment);
        //    }
        //    return data != null ? data.AsList() : [];
        //}


        private List<VehicleExpenseDocumentRequest> GetListOfVehiclesExpenseDocument(int VehicleExpenseID)
        {
            string boardQuery = @"
            select * From tblVehicleExpenseDocument
            WHERE VehicleExpenseID = @VehicleExpenseID";

            // Execute the SQL query with the VehicleExpenseID parameter
            var data = _dbConnection.Query<VehicleExpenseDocumentRequest>(boardQuery, new { VehicleExpenseID }).ToList();

            // Check if data is null or empty
            if (data == null || !data.Any())
            {
                return new List<VehicleExpenseDocumentRequest>(); // Return an empty list if no documents found
            }

            // Loop through each document and validate the attachment before calling GetPDF
            foreach (var item in data)
            {
                // Check if the Attachment is null or empty
                if (string.IsNullOrEmpty(item.Attachment))
                {
                    // Handle invalid attachment path if necessary, e.g., set a default or log an error
                    item.Attachment = "No attachment found"; // You can set a placeholder or log a warning
                }
                else
                {
                    // If the attachment exists, call GetPDF
                    item.Attachment = GetPDF(item.Attachment);
                }
            }

            return data;
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

        public async Task<IEnumerable<GetVehicleExpenseTypeResponse>> GetVehicleExpenseTypes()
        {
            string sql = @"SELECT VehicleExpenseTypeID, VehicleExpenseType 
                           FROM tblVehicleExpenseType";

            var result = await _dbConnection.QueryAsync<GetVehicleExpenseTypeResponse>(sql);
            return result;
        }

        //public async Task<List<GetAllExpenseExportResponse>> GetAllExpenseExport(GetAllExpenseExportRequest request)
        //{
        //    // Parse StartDate and EndDate from string to DateTime
        //    DateTime startDate;
        //    DateTime endDate;

        //    // Attempt to parse the dates in the required format
        //    if (!DateTime.TryParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate) ||
        //        !DateTime.TryParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
        //    {
        //        throw new ArgumentException("Invalid date format. Please use 'DD-MM-YYYY'.");
        //    }

        //    // SQL query with DateTime parameters
        //    string sql = @"
        //        SELECT 
        //            ve.VehicleID, 
        //            vm.VehicleNumber, 
        //            vet.VehicleExpenseType AS ExpenseType, 
        //            ve.ExpenseDate, 
        //            ve.Remarks, 
        //            ve.Cost AS Amount
        //        FROM 
        //            tblVehicleExpense ve
        //        JOIN 
        //            tblVehicleMaster vm ON ve.VehicleID = vm.VehicleID
        //        JOIN 
        //            tblVehicleExpenseType vet ON ve.VehicleExpenseTypeID = vet.VehicleExpenseTypeID
        //        WHERE 
        //            ve.InstituteID = @InstituteID
        //            AND ve.ExpenseDate BETWEEN @StartDate AND @EndDate
        //            AND ve.IsActive = 1
        //            AND (@VehicleID = 0 OR ve.VehicleID = @VehicleID)
        //            AND (@ExpenseTypeID = 0 OR ve.VehicleExpenseTypeID = @ExpenseTypeID)
        //        ORDER BY 
        //            ve.ExpenseDate";

        //    // Execute the query with the proper DateTime parameters
        //    return (await _dbConnection.QueryAsync<GetAllExpenseExportResponse>(sql, new
        //    {
        //        InstituteID = request.InstituteID,
        //        StartDate = startDate,
        //        EndDate = endDate,
        //        VehicleID = request.VehicleID,
        //        ExpenseTypeID = request.ExpenseTypeID
        //    })).AsList();
        //}

        public async Task<List<GetAllExpenseExportResponse>> GetAllExpenseExport(GetAllExpenseExportRequest request)
        {
            // Parse StartDate and EndDate from string to DateTime
            DateTime startDate;
            DateTime endDate;

            // Attempt to parse the dates in the required format
            if (!DateTime.TryParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate) ||
                !DateTime.TryParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                throw new ArgumentException("Invalid date format. Please use 'DD-MM-YYYY'.");
            }

            // SQL query with DateTime parameters
            string sql = @"
        SELECT 
            ve.VehicleID, 
            vm.VehicleNumber, 
            vet.VehicleExpenseType AS ExpenseType, 
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
            AND ve.IsActive = 1
            AND (@VehicleID = 0 OR ve.VehicleID = @VehicleID)
            AND (@ExpenseTypeID = 0 OR ve.VehicleExpenseTypeID = @ExpenseTypeID)
        ORDER BY 
            ve.ExpenseDate";

            // Execute the query with the proper DateTime parameters
            return (await _dbConnection.QueryAsync<GetAllExpenseExportResponse>(sql, new
            {
                InstituteID = request.InstituteID,
                StartDate = startDate,
                EndDate = endDate,
                VehicleID = request.VehicleID,
                ExpenseTypeID = request.ExpenseTypeID
            })).AsList();
        }



        public async Task<ServiceResponse<string>> AddFuelExpense(AddFuelExpenseRequest request)
        {
            try
            {
                // Convert the ExpenseDate string to DateTime
                DateTime expenseDate;
                if (!DateTime.TryParseExact(request.ExpenseDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out expenseDate))
                {
                    return new ServiceResponse<string>(false, "Invalid date format. Please use DD-MM-YYYY.", null, 400);
                }

                string sql = @"INSERT INTO tblVehicleFuelExpense
                        (VehicleID, CurrentReadingInKM, PreviousReading, DistanceTravelledInKM, 
                         PreviousFuelAddedInLitre, FuelAddedInLitre, UnitPrice, TotalAmount, 
                         ExpenseDate, Remarks, InstituteID, IsActive)
                       VALUES 
                        (@VehicleID, @CurrentReadingInKM, @PreviousReading, @DistanceTravelledInKM,
                         @PreviousFuelAddedInLitre, @FuelAddedInLitre, @UnitPrice, @TotalAmount, 
                         @ExpenseDate, @Remarks, @InstituteID, @IsActive);
                       SELECT CAST(SCOPE_IDENTITY() as int);";

                int result = await _dbConnection.ExecuteScalarAsync<int>(sql, new
                {
                    request.VehicleID,
                    request.CurrentReadingInKM,
                    request.PreviousReading,
                    request.DistanceTravelledInKM,
                    request.PreviousFuelAddedInLitre,
                    request.FuelAddedInLitre,
                    request.UnitPrice,
                    request.TotalAmount,
                    ExpenseDate = expenseDate, // Use the DateTime object here
                    request.Remarks,
                    request.InstituteID,
                    request.IsActive
                });

                // Insert attachments if present
                if (request.Attachments != null && request.Attachments.Count > 0)
                {
                    foreach (var attachment in request.Attachments)
                    {
                        await AddAttachment(result, attachment);
                    }
                }

                return new ServiceResponse<string>(true, "Fuel expense added successfully", "Success", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, $"Error adding fuel expense: {ex.Message}", null, 500);
            }
        }


        private async Task AddAttachment(int vehicleFuelExpenseID, AttachmentRequest attachment)
        {
            // Convert the base64 string to a byte array
            byte[] attachmentData = Convert.FromBase64String(attachment.Attachment);

            string sql = @"INSERT INTO tblVehicleExpenseDocument (VehicleExpenseID, VehicleExpenseDocument)
                   VALUES (@VehicleExpenseID, @VehicleExpenseDocument)";

            await _dbConnection.ExecuteAsync(sql, new
            {
                VehicleExpenseID = vehicleFuelExpenseID,
                VehicleExpenseDocument = attachmentData // Insert the byte array into the VARBINARY column
            });
        }

        public async Task<ServiceResponse<IEnumerable<GetFuelExpenseResponse>>> GetFuelExpense(GetFuelExpenseRequest request)
        {
            try
            {
                // Convert the StartDate and EndDate to DateTime
                DateTime startDate;
                DateTime endDate;

                if (!DateTime.TryParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate) ||
                    !DateTime.TryParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                {
                    return new ServiceResponse<IEnumerable<GetFuelExpenseResponse>>(false, "Invalid date format. Please use DD-MM-YYYY.", null, StatusCodes.Status400BadRequest);
                }

                // SQL to retrieve fuel expenses with pagination and filters
                string sql = @"
            SELECT 
                vfe.VehicleFuelExpenseID,
                vfe.VehicleID,
                vfe.CurrentReadingInKM,
                vfe.PreviousReading,
                vfe.DistanceTravelledInKM,
                vfe.PreviousFuelAddedInLitre,
                vfe.FuelAddedInLitre,
                vfe.UnitPrice,
                vfe.TotalAmount,
                vfe.ExpenseDate,
                vfe.Remarks,
                vfe.InstituteID
            FROM 
                tblVehicleFuelExpense vfe
            WHERE 
                vfe.InstituteID = @InstituteID
                AND vfe.VehicleID = @VehicleID
                AND vfe.ExpenseDate BETWEEN @StartDate AND @EndDate
                AND vfe.IsActive = 1
            ORDER BY 
                vfe.ExpenseDate
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

                var expenses = await _dbConnection.QueryAsync<GetFuelExpenseResponse>(sql, new
                {
                    InstituteID = request.InstituteID,
                    VehicleID = request.VehicleID,
                    StartDate = startDate,
                    EndDate = endDate,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                foreach (var expense in expenses)
                {
                    // Fetch associated documents for each expense using the VehicleFuelExpenseID
                    string documentSql = @"SELECT VehicleExpenseDocument FROM tblVehicleExpenseDocument WHERE VehicleExpenseID = @VehicleFuelExpenseID";
                    var documents = await _dbConnection.QueryAsync<byte[]>(documentSql, new { VehicleFuelExpenseID = expense.VehicleFuelExpenseID });

                    // Convert the binary data to base64 string and add it to Documents
                    expense.Documents = documents.Select(doc => Convert.ToBase64String(doc)).ToList();

                    // Convert ExpenseDate to string format 'DD-MM-YYYY'
                    expense.ExpenseDate = DateTime.Parse(expense.ExpenseDate).ToString("dd-MM-yyyy");
                }

                if (expenses.Any())
                {
                    return new ServiceResponse<IEnumerable<GetFuelExpenseResponse>>(true, "Fuel expenses retrieved successfully", expenses, StatusCodes.Status200OK, expenses.Count());
                }
                else
                {
                    return new ServiceResponse<IEnumerable<GetFuelExpenseResponse>>(false, "No fuel expenses found for the given filters", null, StatusCodes.Status404NotFound);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetFuelExpenseResponse>>(false, $"Error retrieving fuel expenses: {ex.Message}", null, StatusCodes.Status500InternalServerError);
            }
        }


        public async Task<ServiceResponse<byte[]>> GetFuelExpenseExport(GetFuelExpenseExportRequest request)
        {
            try
            {
                // Parse StartDate and EndDate
                DateTime startDate;
                DateTime endDate;

                if (!DateTime.TryParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate) ||
                    !DateTime.TryParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                {
                    return new ServiceResponse<byte[]>(false, "Invalid date format. Please use DD-MM-YYYY.", null, 400);
                }

                // SQL query to fetch fuel expenses along with VehicleNumber
                string sql = @"
            SELECT 
                vfe.VehicleFuelExpenseID,
                vfe.VehicleID,
                vm.VehicleNumber, -- Include VehicleNumber
                vfe.CurrentReadingInKM,
                vfe.PreviousReading,
                vfe.DistanceTravelledInKM,
                vfe.PreviousFuelAddedInLitre,
                vfe.FuelAddedInLitre,
                vfe.UnitPrice,
                vfe.TotalAmount,
                vfe.ExpenseDate,
                vfe.Remarks
            FROM 
                tblVehicleFuelExpense vfe
            JOIN
                tblVehicleMaster vm ON vfe.VehicleID = vm.VehicleID -- Join with VehicleMaster table to get VehicleNumber
            WHERE 
                vfe.InstituteID = @InstituteID
                AND vfe.VehicleID = @VehicleID
                AND vfe.ExpenseDate BETWEEN @StartDate AND @EndDate
                AND vfe.IsActive = 1
            ORDER BY 
                vfe.ExpenseDate
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

                var expenses = await _dbConnection.QueryAsync<GetFuelExpenseExportResponse>(sql, new
                {
                    InstituteID = request.InstituteID,
                    VehicleID = request.VehicleID,
                    StartDate = startDate,
                    EndDate = endDate,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                if (!expenses.Any())
                {
                    return new ServiceResponse<byte[]>(false, "No fuel expenses found.", null, 404);
                }

                // Process the data to export to Excel or CSV
                if (request.ExportType == 1) // Excel Export
                {
                    using (var package = new OfficeOpenXml.ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Fuel Expenses");
                        worksheet.Cells[1, 1].Value = "Sr. No.";  // Add Sr. No. column as the first column
                        worksheet.Cells[1, 2].Value = "Vehicle Number";
                        worksheet.Cells[1, 3].Value = "Current Reading (KM)";
                        worksheet.Cells[1, 4].Value = "Previous Reading (KM)";
                        worksheet.Cells[1, 5].Value = "Distance Travelled (KM)";
                        worksheet.Cells[1, 6].Value = "Previous Fuel Added (Litre)";
                        worksheet.Cells[1, 7].Value = "Fuel Added (Litre)";
                        worksheet.Cells[1, 8].Value = "Unit Price";
                        worksheet.Cells[1, 9].Value = "Total Amount";
                        worksheet.Cells[1, 10].Value = "Remarks";

                        var row = 2;
                        int srNo = 1;
                        foreach (var expense in expenses)
                        {
                            worksheet.Cells[row, 1].Value = srNo++; // Fill Sr. No.
                            worksheet.Cells[row, 2].Value = expense.VehicleNumber;  // Fill Vehicle Number
                            worksheet.Cells[row, 3].Value = expense.CurrentReadingInKM;
                            worksheet.Cells[row, 4].Value = expense.PreviousReading;
                            worksheet.Cells[row, 5].Value = expense.DistanceTravelledInKM;
                            worksheet.Cells[row, 6].Value = expense.PreviousFuelAddedInLitre;
                            worksheet.Cells[row, 7].Value = expense.FuelAddedInLitre;
                            worksheet.Cells[row, 8].Value = expense.UnitPrice;
                            worksheet.Cells[row, 9].Value = expense.TotalAmount;
                            worksheet.Cells[row, 10].Value = expense.Remarks;
                            row++;
                        }

                        return new ServiceResponse<byte[]>(true, "Excel file generated successfully", package.GetAsByteArray(), 200);
                    }
                }
                else if (request.ExportType == 2) // CSV Export
                {
                    var csv = new StringBuilder();
                    csv.AppendLine("Sr. No.,Vehicle Number,Current Reading (KM),Previous Reading (KM),Distance Travelled (KM),Previous Fuel Added (Litre),Fuel Added (Litre),Unit Price,Total Amount,Remarks");

                    int srNo = 1;
                    foreach (var expense in expenses)
                    {
                        csv.AppendLine($"{srNo++},{expense.VehicleNumber},{expense.CurrentReadingInKM},{expense.PreviousReading},{expense.DistanceTravelledInKM},{expense.PreviousFuelAddedInLitre},{expense.FuelAddedInLitre},{expense.UnitPrice},{expense.TotalAmount},{expense.Remarks}");
                    }

                    return new ServiceResponse<byte[]>(true, "CSV file generated successfully", Encoding.UTF8.GetBytes(csv.ToString()), 200);
                }
                else
                {
                    return new ServiceResponse<byte[]>(false, "Invalid export type", null, 400);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, $"Error exporting data: {ex.Message}", null, 500);
            }
        }


    }
}
