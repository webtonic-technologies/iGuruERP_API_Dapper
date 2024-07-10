using Dapper;
using System.Data;
using System.Data.SqlClient;
using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.Responses;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;
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

        public async Task<ServiceResponse<string>> AddUpdateVisitorLog(VisitorLog visitorLog)
        {
            try
            {
                if (visitorLog.VisitorID == 0)
                {
                    // Insert new visitor log
                    string query = @"INSERT INTO tblVisitorMaster (VisitorCodeID, VisitorName, Photo, SourceID, PurposeID, MobileNo, EmailID, Address, OrganizationName, EmployeeID, NoOfVisitor, AccompaniedBy, CheckInTime, CheckOutTime, Remarks, IDProofDocumentID, Information, Document, ApprovalTypeID, Status, InstituteId)
                                     VALUES (@VisitorCodeID, @VisitorName, @Photo, @SourceID, @PurposeID, @MobileNo, @EmailID, @Address, @OrganizationName, @EmployeeID, @NoOfVisitor, @AccompaniedBy, @CheckInTime, @CheckOutTime, @Remarks, @IDProofDocumentID, @Information, @Document, @ApprovalTypeID, @Status, @InstituteId)";
                    visitorLog.Photo = ImageUpload(visitorLog.Photo);
                    visitorLog.Document = PDFUpload(visitorLog.Document);
                    int insertedValue = await _dbConnection.ExecuteAsync(query, visitorLog);
                    if (insertedValue > 0)
                    {
                        return new ServiceResponse<string>(true, "Visitor Log Added Successfully", "Success", 201);
                    }
                    return new ServiceResponse<string>(false, "Failed to Add Visitor Log", "Failure", 400);
                }
                else
                {
                    // Update existing visitor log
                    string query = @"UPDATE tblVisitorMaster SET VisitorCodeID = @VisitorCodeID, VisitorName = @VisitorName, Photo = @Photo, SourceID = @SourceID, PurposeID = @PurposeID, MobileNo = @MobileNo, 
                                    EmailID = @EmailID, Address = @Address, OrganizationName = @OrganizationName, EmployeeID = @EmployeeID, NoOfVisitor = @NoOfVisitor, AccompaniedBy = @AccompaniedBy, 
                                    CheckInTime = @CheckInTime, CheckOutTime = @CheckOutTime, Remarks = @Remarks, IDProofDocumentID = @IDProofDocumentID, Information = @Information, Document = @Document,
                                    ApprovalTypeID = @ApprovalTypeID, Status = @Status, InstituteId = @InstituteId WHERE VisitorID = @VisitorID";
                    visitorLog.Photo = ImageUpload(visitorLog.Photo);
                    visitorLog.Document = PDFUpload(visitorLog.Document);
                    int rowsAffected = await _dbConnection.ExecuteAsync(query, visitorLog);
                    if (rowsAffected > 0)
                    {
                        return new ServiceResponse<string>(true, "Visitor Log Updated Successfully", "Success", 200);
                    }
                    return new ServiceResponse<string>(false, "Failed to Update Visitor Log", "Failure", 400);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }
        public async Task<ServiceResponse<IEnumerable<Visitorlogresponse>>> GetAllVisitorLogs(GetAllVisitorLogsRequest request)
        {
            try
            {
                string query = @"SELECT vm.*, s.Source as Sourcename, p.Purpose as Purposename
                         FROM tblVisitorMaster vm
                         LEFT JOIN tblSources s ON vm.SourceID = s.SourceID
                         LEFT JOIN tblPurposeType p ON vm.PurposeID = p.PurposeID
                         WHERE vm.Status = 1 and InstituteId = @InstituteId";

                var visitorLogs = await _dbConnection.QueryAsync<Visitorlogresponse>(query, new { InstituteId = request.InstituteId});
                foreach (var data in visitorLogs)
                {
                    data.Photo = GetImage(data.Photo);
                    data.Document = GetPDF(data.Document);
                }

                var paginatedVisitorLogs = visitorLogs.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
                return new ServiceResponse<IEnumerable<Visitorlogresponse>>(true, "Visitor Logs Retrieved Successfully", paginatedVisitorLogs, 200, visitorLogs.Count());
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
                string query = @"SELECT vm.*, s.Source as Sourcename, p.Purpose as Purposename
                         FROM tblVisitorMaster vm
                         LEFT JOIN tblSources s ON vm.SourceID = s.SourceID
                         LEFT JOIN tblPurposeType p ON vm.PurposeID = p.PurposeID
                         WHERE vm.VisitorID = @VisitorID AND vm.Status = 1";

                var visitorLog = await _dbConnection.QueryFirstOrDefaultAsync<Visitorlogresponse>(query, new { VisitorID = visitorId });

                if (visitorLog != null)
                {
                    visitorLog.Photo = GetImage(visitorLog.Photo);
                    visitorLog.Document = GetPDF(visitorLog.Document);
                    return new ServiceResponse<Visitorlogresponse>(true, "Visitor Log Retrieved Successfully", visitorLog, 200);
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
