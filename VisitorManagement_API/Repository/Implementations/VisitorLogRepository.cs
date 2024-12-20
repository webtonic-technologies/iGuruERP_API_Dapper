using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.Responses;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Repository.Interfaces;

namespace VisitorManagement_API.Repository.Implementations
{
    public class VisitorLogRepository : IVisitorLogRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public VisitorLogRepository(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<ServiceResponse<string>> AddUpdateVisitorLog(VisitorRequestDTO visitorLog)
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                int affectedRows = 0;
                string insertOrUpdateQuery;

                // Convert CheckInTime and CheckOutTime to DateTime using ParseExact with AM/PM format
                DateTime checkInTime = DateTime.ParseExact(visitorLog.CheckInTime, "dd-MM-yyyy, hh:mm tt", CultureInfo.InvariantCulture);
                DateTime checkOutTime = DateTime.ParseExact(visitorLog.CheckOutTime, "dd-MM-yyyy, hh:mm tt", CultureInfo.InvariantCulture);

                // Format DateTime to the database format (standard format)
                string formattedCheckInTime = checkInTime.ToString("yyyy-MM-dd HH:mm:ss");
                string formattedCheckOutTime = checkOutTime.ToString("yyyy-MM-dd HH:mm:ss");

                // Format DateTime for the response (with AM/PM)
                string formattedCheckInTimeForResponse = checkInTime.ToString("dd-MM-yyyy, hh:mm tt");
                string formattedCheckOutTimeForResponse = checkOutTime.ToString("dd-MM-yyyy, hh:mm tt");

                if (visitorLog.VisitorID == 0)
                {
                    // Insert new visitor log
                    insertOrUpdateQuery = @"
                    INSERT INTO tblVisitorMaster (VisitorCodeID, VisitorName, Photo, SourceID, PurposeID, MobileNo, EmailID, Address, OrganizationName, EmployeeID, NoOfVisitor, AccompaniedBy, CheckInTime, CheckOutTime, Remarks, IDProofDocumentID, Information, ApprovalTypeID, Status, InstituteId)
                    VALUES (@VisitorCodeID, @VisitorName, @Photo, @SourceID, @PurposeID, @MobileNo, @EmailID, @Address, @OrganizationName, @EmployeeID, @NoOfVisitor, @AccompaniedBy, @CheckInTime, @CheckOutTime, @Remarks, @IDProofDocumentID, @Information, @ApprovalTypeID, @Status, @InstituteId);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                    visitorLog.Photo = ImageUpload(visitorLog.Photo);
                    visitorLog.VisitorID = await _dbConnection.QuerySingleAsync<int>(insertOrUpdateQuery,
                        new
                        {
                            visitorLog.VisitorCodeID,
                            visitorLog.VisitorName,
                            visitorLog.Photo,
                            visitorLog.SourceID,
                            visitorLog.PurposeID,
                            visitorLog.MobileNo,
                            visitorLog.EmailID,
                            visitorLog.Address,
                            visitorLog.OrganizationName,
                            visitorLog.EmployeeID,
                            visitorLog.NoOfVisitor,
                            visitorLog.AccompaniedBy,
                            CheckInTime = formattedCheckInTime,
                            CheckOutTime = formattedCheckOutTime,
                            visitorLog.Remarks,
                            visitorLog.IDProofDocumentID,
                            visitorLog.Information,
                            visitorLog.ApprovalTypeID,
                            visitorLog.Status,
                            visitorLog.InstituteId
                        }, transaction);

                    affectedRows = visitorLog.VisitorID > 0 ? 1 : 0;
                }
                else
                {
                    // Update existing visitor log
                    insertOrUpdateQuery = @"
                    UPDATE tblVisitorMaster 
                    SET VisitorCodeID = @VisitorCodeID, VisitorName = @VisitorName, Photo = @Photo, SourceID = @SourceID, PurposeID = @PurposeID, MobileNo = @MobileNo, EmailID = @EmailID, Address = @Address, OrganizationName = @OrganizationName, EmployeeID = @EmployeeID, NoOfVisitor = @NoOfVisitor, AccompaniedBy = @AccompaniedBy, CheckInTime = @CheckInTime, CheckOutTime = @CheckOutTime, Remarks = @Remarks, IDProofDocumentID = @IDProofDocumentID, Information = @Information, ApprovalTypeID = @ApprovalTypeID, Status = @Status, InstituteId = @InstituteId
                    WHERE VisitorID = @VisitorID";
                    visitorLog.Photo = ImageUpload(visitorLog.Photo);
                    affectedRows = await _dbConnection.ExecuteAsync(insertOrUpdateQuery,
                        new
                        {
                            visitorLog.VisitorID,
                            visitorLog.VisitorCodeID,
                            visitorLog.VisitorName,
                            visitorLog.Photo,
                            visitorLog.SourceID,
                            visitorLog.PurposeID,
                            visitorLog.MobileNo,
                            visitorLog.EmailID,
                            visitorLog.Address,
                            visitorLog.OrganizationName,
                            visitorLog.EmployeeID,
                            visitorLog.NoOfVisitor,
                            visitorLog.AccompaniedBy,
                            CheckInTime = formattedCheckInTime,
                            CheckOutTime = formattedCheckOutTime,
                            visitorLog.Remarks,
                            visitorLog.IDProofDocumentID,
                            visitorLog.Information,
                            visitorLog.ApprovalTypeID,
                            visitorLog.Status,
                            visitorLog.InstituteId
                        }, transaction);
                }

                if (affectedRows == 0)
                {
                    transaction.Rollback();
                    return new ServiceResponse<string>(false, "Failed to Add or Update Visitor Log", "Failure", 400);
                }

                // Handling Visitor Documents
                bool documentsHandledSuccessfully = await HandleVisitorDocuments(visitorLog.VisitorID, visitorLog.VisitorDocuments, transaction);

                if (!documentsHandledSuccessfully)
                {
                    transaction.Rollback();
                    return new ServiceResponse<string>(false, "Failed to Handle Visitor Documents", "Failure", 400);
                }

                transaction.Commit();
                return new ServiceResponse<string>(true, "Visitor Log Saved Successfully", "Success", 200);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
            finally { _dbConnection.Close(); }
        }
         
        private async Task<bool> HandleVisitorDocuments(int visitorId, List<VisitorDocument>? documents, IDbTransaction transaction)
        {
            // Delete existing documents for the visitor
            string deleteQuery = "DELETE FROM tblVisitorDocuments WHERE VisitorId = @VisitorId";
            int deleteResult = await _dbConnection.ExecuteAsync(deleteQuery, new { VisitorId = visitorId }, transaction);

            // Insert new documents
            if (documents != null && documents.Count > 0)
            {
                foreach (var document in documents)
                {
                    document.VisitorId = visitorId;
                    document.PdfDoc = PDFUpload(document.PdfDoc);

                    string insertDocumentQuery = @"
                INSERT INTO tblVisitorDocuments (VisitorId, PdfDoc) 
                VALUES (@VisitorId, @PdfDoc)";
                    int insertResult = await _dbConnection.ExecuteAsync(insertDocumentQuery, document, transaction);
                    if (insertResult == 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public async Task<ServiceResponse<IEnumerable<Visitorlogresponse>>> GetAllVisitorLogs(GetAllVisitorLogsRequest request)
        {
            try
            {
                string query = @"SELECT vm.*, s.Source as Sourcename, p.Purpose as Purposename, e.First_Name, e.Middle_Name, e.Last_Name,  i.IDProofDocument as IDProofDocumentName, 
                         a.ApprovalType as ApprovalTypeName
                 FROM tblVisitorMaster vm
                 LEFT JOIN tblSources s ON vm.SourceID = s.SourceID
                 LEFT JOIN tblPurposeType p ON vm.PurposeID = p.PurposeID
                 LEFT JOIN tbl_EmployeeProfileMaster e ON vm.EmployeeID = e.Employee_id
                 LEFT JOIN tblIDProofDocument i ON vm.IDProofDocumentID = i.IDProofDocumentID
                 LEFT JOIN tblVisitorApprovalMaster a ON vm.ApprovalTypeID = a.ApprovalTypeID
                 WHERE vm.Status = 1 AND vm.InstituteId = @InstituteId";

                var visitorLogs = await _dbConnection.QueryAsync(query, new { InstituteId = request.InstituteId });

                // Filter by date range if provided
                IEnumerable<dynamic> filteredVisitorLogs;

                if (!string.IsNullOrEmpty(request.StartDate) && !string.IsNullOrEmpty(request.EndDate))
                {
                    // Convert StartDate and EndDate strings to DateTime
                    DateTime startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    DateTime endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    filteredVisitorLogs = visitorLogs
                        .Where(v => v.CheckInTime >= startDate && v.CheckInTime <= endDate);
                }
                else
                {
                    filteredVisitorLogs = visitorLogs; // Return all logs if no date range is provided
                }

                var visitorLogResponses = filteredVisitorLogs
                    .Select(v => new Visitorlogresponse
                    {
                        VisitorID = v.VisitorID,
                        VisitorCodeID = v.VisitorCodeID,
                        VisitorName = v.VisitorName,
                        Photo = GetImage(v.Photo),
                        SourceID = v.SourceID,
                        Sourcename = v.Sourcename,
                        PurposeID = v.PurposeID,
                        Purposename = v.Purposename,
                        MobileNo = v.MobileNo,
                        EmailID = v.EmailID,
                        Address = v.Address,
                        OrganizationName = v.OrganizationName,
                        EmployeeID = v.EmployeeID,
                        EmployeeFullName = $"{v.First_Name} {v.Middle_Name} {v.Last_Name}".Trim(),
                        NoOfVisitor = v.NoOfVisitor,
                        AccompaniedBy = v.AccompaniedBy,
                        CheckInTime = v.CheckInTime,
                        CheckOutTime = v.CheckOutTime,
                        Remarks = v.Remarks,
                        IDProofDocumentID = v.IDProofDocumentID,
                        IDProofDocumentName = v.IDProofDocumentName,
                        Information = v.Information,
                        ApprovalTypeID = v.ApprovalTypeID,
                        ApprovalTypeName = v.ApprovalTypeName,
                        Status = v.Status,
                        InstituteId = v.InstituteId
                    })
                    .ToList();

                foreach (var data in visitorLogResponses)
                {
                    // Fetch documents related to the visitor
                    string documentQuery = @"SELECT DocumentId, VisitorId, PdfDoc
                             FROM tblVisitorDocuments
                             WHERE VisitorId = @VisitorId";
                    var documents = await _dbConnection.QueryAsync<VisitorDocumentResponse>(documentQuery, new { VisitorId = data.VisitorID });
                    foreach (var item in documents)
                    {
                        item.PdfDoc = GetPDF(item.PdfDoc);
                    }
                    data.VisitorDocuments = documents.ToList();
                }

                // Filter by SearchText if provided
                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    filteredVisitorLogs = filteredVisitorLogs
                        .Where(v => v.VisitorName.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) ||
                                   v.VisitorCodeID.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) ||
                                   v.MobileNo.Contains(request.SearchText));
                }

                var paginatedVisitorLogs = visitorLogResponses
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);

                return new ServiceResponse<IEnumerable<Visitorlogresponse>>(true, "Visitor Logs Retrieved Successfully", paginatedVisitorLogs, 200, visitorLogResponses.Count());
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<Visitorlogresponse>>(false, ex.Message, null, 500);
            }
        }


        public async Task<ServiceResponse<Visitorlogresponse>> GetVisitorLogById(int visitorId)
        {
            try
            {
                string query = @"SELECT vm.*, s.Source as Sourcename, p.Purpose as Purposename, 
                                e.First_Name, e.Middle_Name, e.Last_Name, 
                                i.IDProofDocument as IDProofDocumentName, 
                                a.ApprovalType as ApprovalTypeName
                         FROM tblVisitorMaster vm
                         LEFT JOIN tblSources s ON vm.SourceID = s.SourceID
                         LEFT JOIN tblPurposeType p ON vm.PurposeID = p.PurposeID
                         LEFT JOIN tbl_EmployeeProfileMaster e ON vm.EmployeeID = e.Employee_id
                         LEFT JOIN tblIDProofDocument i ON vm.IDProofDocumentID = i.IDProofDocumentID
                         LEFT JOIN tblVisitorApprovalMaster a ON vm.ApprovalTypeID = a.ApprovalTypeID
                         WHERE vm.VisitorID = @VisitorID AND vm.Status = 1";

                var visitorLog = await _dbConnection.QueryFirstOrDefaultAsync(query, new { VisitorID = visitorId });

                if (visitorLog != null)
                {
                    var visitorLogResponse = new Visitorlogresponse
                    {
                        VisitorID = visitorLog.VisitorID,
                        VisitorCodeID = visitorLog.VisitorCodeID,
                        VisitorName = visitorLog.VisitorName,
                        Photo = GetImage(visitorLog.Photo),
                        SourceID = visitorLog.SourceID,
                        Sourcename = visitorLog.Sourcename,
                        PurposeID = visitorLog.PurposeID,
                        Purposename = visitorLog.Purposename,
                        MobileNo = visitorLog.MobileNo,
                        EmailID = visitorLog.EmailID,
                        Address = visitorLog.Address,
                        OrganizationName = visitorLog.OrganizationName,
                        EmployeeID = visitorLog.EmployeeID,
                        EmployeeFullName = $"{visitorLog.First_Name} {visitorLog.Middle_Name} {visitorLog.Last_Name}".Trim(),
                        NoOfVisitor = visitorLog.NoOfVisitor,
                        AccompaniedBy = visitorLog.AccompaniedBy,
                        CheckInTime = visitorLog.CheckInTime,
                        CheckOutTime = visitorLog.CheckOutTime,
                        Remarks = visitorLog.Remarks,
                        IDProofDocumentID = visitorLog.IDProofDocumentID,
                        IDProofDocumentName = visitorLog.IDProofDocumentName,
                        Information = visitorLog.Information,
                        ApprovalTypeID = visitorLog.ApprovalTypeID,
                        ApprovalTypeName = visitorLog.ApprovalTypeName,
                        Status = visitorLog.Status,
                        InstituteId = visitorLog.InstituteId
                    };

                    // Fetch documents related to the visitor
                    string documentQuery = @"SELECT DocumentId, VisitorId, PdfDoc
                                     FROM tblVisitorDocuments
                                     WHERE VisitorId = @VisitorId";
                    var documents = await _dbConnection.QueryAsync<VisitorDocumentResponse>(documentQuery, new { VisitorId = visitorLog.VisitorID });
                    foreach(var item in documents)
                    {
                        item.PdfDoc = GetPDF(item.PdfDoc);
                    }
                    visitorLogResponse.VisitorDocuments = documents.ToList();

                    return new ServiceResponse<Visitorlogresponse>(true, "Visitor Log Retrieved Successfully", visitorLogResponse, 200);
                }

                return new ServiceResponse<Visitorlogresponse>(false, "Visitor Log Not Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Visitorlogresponse>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<bool>> UpdateVisitorLogStatus(int visitorId)
        {
            try
            {
                // Update the Status column to false to mark the record as deleted
                string query = "UPDATE tblVisitorMaster SET Status = 0 WHERE VisitorID = @VisitorID";
                int rowsAffected = await _dbConnection.ExecuteAsync(query, new { VisitorID = visitorId });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "Visitor Log Soft Deleted Successfully", true, 200);
                }

                return new ServiceResponse<bool>(false, "Failed to Soft Delete Visitor Log", false, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
        private string ImageUpload(string image)
        {
            if (string.IsNullOrEmpty(image) || image == "string")
            {
                return string.Empty;
            }
            byte[] imageData = Convert.FromBase64String(image);
            string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "Visitorlog");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string fileExtension = IsJpeg(imageData) == true ? ".jpg" : IsPng(imageData) == true ? ".png" : IsGif(imageData) == true ? ".gif" : string.Empty;
            string fileName = Guid.NewGuid().ToString() + fileExtension;
            string filePath = Path.Combine(directoryPath, fileName);
            if (string.IsNullOrEmpty(fileExtension))
            {
                throw new InvalidOperationException("Incorrect file uploaded");
            }
            // Write the byte array to the image file
            File.WriteAllBytes(filePath, imageData);
            return filePath;
        }
        private string GetImage(string Filename)
        {
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "Visitorlog", Filename);

            if (!File.Exists(filePath))
            {
                return string.Empty;
            }
            byte[] fileBytes = File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(fileBytes);
            return base64String;
        }
        private bool IsJpeg(byte[] bytes)
        {
            // JPEG magic number: 0xFF, 0xD8
            return bytes.Length > 1 && bytes[0] == 0xFF && bytes[1] == 0xD8;
        }
        private bool IsPng(byte[] bytes)
        {
            // PNG magic number: 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A
            return bytes.Length > 7 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47
                && bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A;
        }
        private bool IsGif(byte[] bytes)
        {
            // GIF magic number: "GIF"
            return bytes.Length > 2 && bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46;
        }
        private string PDFUpload(string pdf)
        {
            if (string.IsNullOrEmpty(pdf) || pdf == "string")
            {
                return string.Empty;
            }
            byte[] imageData = Convert.FromBase64String(pdf);
            string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "VisitorlogDocuments");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string fileExtension = IsPdf(imageData) == true ? ".pdf" : string.Empty;
            if (string.IsNullOrEmpty(fileExtension))
            {
                throw new InvalidOperationException("Incorrect file uploaded");
            }
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
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "VisitorlogDocuments", Filename);
            if (!File.Exists(filePath))
            {
                return string.Empty;
            }
            byte[] fileBytes = File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(fileBytes);
            return base64String;
        }


        public async Task<ServiceResponse<IEnumerable<GetSourcesResponse>>> GetSources(GetSourcesRequest request)
        {
            try
            {
                // Query to fetch SourceID and Source based on InstituteID
                string query = "SELECT SourceID, Source FROM tblSources WHERE Status = 1 AND InstituteID = @InstituteID";

                // Fetch the sources from the database
                var sources = await _dbConnection.QueryAsync<GetSourcesResponse>(query, new { InstituteID = request.InstituteID });

                return new ServiceResponse<IEnumerable<GetSourcesResponse>>(true, "Sources Retrieved Successfully", sources, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetSourcesResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetPurposeResponse>>> GetPurpose(GetPurposeRequest request)
        {
            try
            {
                string query = @"SELECT PurposeID, Purpose FROM tblPurposeType WHERE Status = 1 AND InstituteID = @InstituteID";
                var purposes = await _dbConnection.QueryAsync<GetPurposeResponse>(query, new { InstituteID = request.InstituteID });

                return new ServiceResponse<IEnumerable<GetPurposeResponse>>(true, "Purpose List Retrieved Successfully", purposes, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetPurposeResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetIDProofResponse>>> GetIDProof()
        {
            try
            {
                string query = "SELECT IDProofID, IDProof FROM tblVisitorIDProof";
                var idProofs = await _dbConnection.QueryAsync<GetIDProofResponse>(query);

                if (idProofs.Any())
                {
                    return new ServiceResponse<IEnumerable<GetIDProofResponse>>(true, "ID Proofs Retrieved Successfully", idProofs, 200);
                }
                return new ServiceResponse<IEnumerable<GetIDProofResponse>>(false, "No ID Proofs Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetIDProofResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetApprovalTypeResponse>>> GetApprovalType()
        {
            try
            {
                string query = "SELECT ApprovalTypeID, ApprovalType FROM tblVisitorApprovalMaster";
                var approvalTypes = await _dbConnection.QueryAsync<GetApprovalTypeResponse>(query);

                if (approvalTypes.Any())
                {
                    return new ServiceResponse<IEnumerable<GetApprovalTypeResponse>>(true, "Approval Types Retrieved Successfully", approvalTypes, 200);
                }
                return new ServiceResponse<IEnumerable<GetApprovalTypeResponse>>(false, "No Approval Types Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetApprovalTypeResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetEmployeeResponse>>> GetEmployee(GetEmployeeRequest request)
        {
            try
            {
                string query = @"
                    SELECT e.Employee_id AS EmployeeID, e.Employee_code_id AS EmployeeCode, 
                           CONCAT(e.First_Name, ' ', e.Middle_Name, ' ', e.Last_Name) AS EmployeeName,
                           d.DepartmentName AS Department,
                           des.DesignationName AS Designation
                    FROM tbl_EmployeeProfileMaster e
                    INNER JOIN tbl_Department d ON e.Department_id = d.Department_id
                    INNER JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
                    WHERE e.Institute_id = @InstituteID AND e.Status = 1";

                var employees = await _dbConnection.QueryAsync<GetEmployeeResponse>(query, new { InstituteID = request.InstituteID });

                if (employees.Any())
                {
                    return new ServiceResponse<IEnumerable<GetEmployeeResponse>>(true, "Employees Retrieved Successfully", employees, 200);
                }

                return new ServiceResponse<IEnumerable<GetEmployeeResponse>>(false, "No Employees Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetEmployeeResponse>>(false, ex.Message, null, 500);
            }
        }


        public async Task<ServiceResponse<GetVisitorSlipResponse>> GetVisitorSlip(GetVisitorSlipRequest request)
        {
            try
            {
                // Query to get Institute details
                string instituteQuery = @"
            SELECT 
                i.Institute_name AS InstituteName, 
                CONCAT(ia.house, ', ', ia.Locality, ', ', ia.Landmark, ', ', ia.pincode) AS Address, 
                ia.Mobile_number AS Mobile, 
                ia.Email
            FROM 
                tbl_InstituteDetails i
            JOIN 
                tbl_InstituteAddress ia ON i.Institute_id = ia.Institute_id
            WHERE 
                i.Institute_id = @InstituteID";
                var instituteInfo = await _dbConnection.QueryFirstOrDefaultAsync<InstituteInfo>(instituteQuery, new { InstituteID = request.InstituteID });

                if (instituteInfo == null)
                {
                    return new ServiceResponse<GetVisitorSlipResponse>(false, "Institute not found", null, 404);
                }

                // Query to get Visitor details, including the purpose directly from tblPurposeType
                string visitorQuery = @"
            SELECT 
                VisitorCodeID AS VisitorCode, 
                VisitorName, 
                OrganizationName, 
                MobileNo AS MobileNumber, 
                Address, 
                vm.PurposeID, 
                CONCAT(e.First_Name, ' ', e.Middle_Name, ' ', e.Last_Name) AS MeetingWith, 
                Remarks, 
                FORMAT(CheckInTime, 'dd-MM-yyyy, hh:mm tt') AS CheckInTime,  -- Formatting CheckInTime with AM/PM
                FORMAT(CheckOutTime, 'dd-MM-yyyy, hh:mm tt') AS CheckOutTime, -- Formatting CheckOutTime with AM/PM
                p.Purpose AS Purpose  -- Directly get the purpose from tblPurposeType
            FROM 
                tblVisitorMaster vm
            LEFT OUTER JOIN  
                tbl_EmployeeProfileMaster e ON vm.EmployeeID = e.Employee_id
            LEFT OUTER JOIN  
                tblPurposeType p ON vm.PurposeID = p.PurposeID  -- Join with tblPurposeType to get the purpose name
            WHERE 
                vm.VisitorID = @VisitorID AND vm.InstituteID = @InstituteID";

                var visitorInfo = await _dbConnection.QueryFirstOrDefaultAsync<VisitorSlip>(visitorQuery, new { VisitorID = request.VisitorID, InstituteID = request.InstituteID });

                if (visitorInfo == null)
                {
                    return new ServiceResponse<GetVisitorSlipResponse>(false, "Visitor not found", null, 404);
                }

                // Populate the response object
                var response = new GetVisitorSlipResponse
                {
                    InstituteInfo = instituteInfo,
                    VisitorSlip = visitorInfo
                };

                return new ServiceResponse<GetVisitorSlipResponse>(true, "Visitor Slip Retrieved Successfully", response, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetVisitorSlipResponse>(false, ex.Message, null, 500);
            }
        }

        public async Task<bool> UpdateApprovalStatus(int visitorID, int approvalTypeID, int instituteID)
        {
            string query = @"
                UPDATE tblVisitorMaster 
                SET ApprovalTypeID = @ApprovalTypeID
                WHERE VisitorID = @VisitorID AND InstituteID = @InstituteID";

            var rowsAffected = await _dbConnection.ExecuteAsync(query, new
            {
                VisitorID = visitorID,
                ApprovalTypeID = approvalTypeID,
                InstituteID = instituteID
            });

            return rowsAffected > 0; // Returns true if update was successful
        }


    }
}