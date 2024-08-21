using Dapper;
using System.Data;
using System.Data.SqlClient;
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
                if (visitorLog.VisitorID == 0)
                {
                    // Insert new visitor log
                    insertOrUpdateQuery = @"
                INSERT INTO tblVisitorMaster (VisitorCodeID, VisitorName, Photo, SourceID, PurposeID, MobileNo, EmailID, Address, OrganizationName, EmployeeID, NoOfVisitor, AccompaniedBy, CheckInTime, CheckOutTime, Remarks, IDProofDocumentID, Information, ApprovalTypeID, Status, InstituteId)
                VALUES (@VisitorCodeID, @VisitorName, @Photo, @SourceID, @PurposeID, @MobileNo, @EmailID, @Address, @OrganizationName, @EmployeeID, @NoOfVisitor, @AccompaniedBy, @CheckInTime, @CheckOutTime, @Remarks, @IDProofDocumentID, @Information, @ApprovalTypeID, @Status, @InstituteId);
                SELECT CAST(SCOPE_IDENTITY() as int)";
                    visitorLog.Photo = ImageUpload(visitorLog.Photo);
                    visitorLog.VisitorID = await _dbConnection.QuerySingleAsync<int>(insertOrUpdateQuery, visitorLog, transaction);
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
                    affectedRows = await _dbConnection.ExecuteAsync(insertOrUpdateQuery, visitorLog, transaction);
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
                return new ServiceResponse<string>(true, "Visitor Log Saved Successfully", "Success", 201);
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

                if (request.StartDate.HasValue && request.EndDate.HasValue)
                {
                    filteredVisitorLogs = visitorLogs
                        .Where(v => v.CheckInTime >= request.StartDate && v.CheckInTime <= request.EndDate);
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
                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    filteredVisitorLogs = filteredVisitorLogs
                        .Where(v => v.VisitorName.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) ||
                                     v.MobileNo.Contains(request.SearchText) ||
                                     v.EmailID.Contains(request.SearchText));
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
    }
}