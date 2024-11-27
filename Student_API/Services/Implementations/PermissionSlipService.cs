using OfficeOpenXml;
using QRCoder;
using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using Student_API.Services.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection.Metadata;

namespace Student_API.Services.Implementations
{
    public class PermissionSlipService : IPermissionSlipService
    {
        private readonly IPermissionSlipRepository _repository;
        private readonly IImageService _imageService;
        private readonly ICommonService _commonService;

        public PermissionSlipService(IPermissionSlipRepository repository, IImageService imageService, ICommonService commonService)
        {
            _repository = repository;
            _imageService = imageService;
            _commonService = commonService;
        }

        public async Task<ServiceResponse<List<PermissionSlipDTO>>> GetAllPermissionSlips(int Institute_id, int classId, int sectionId, int? pageNumber = null, int? pageSize = null)
        {
            return await _repository.GetAllPermissionSlips(Institute_id, classId, sectionId, pageNumber, pageSize);
        }
        public async Task<ServiceResponse<List<PermissionSlipDTO>>> GetPermissionSlips(int Institute_id, int classId, int sectionId, string startDate, string endDate, bool isApproved, int? pageNumber = null, int? pageSize = null)
        {
            return await _repository.GetPermissionSlips(Institute_id, classId, sectionId, startDate, endDate, isApproved, pageNumber, pageSize);
        }
        public async Task<ServiceResponse<string>> UpdatePermissionSlipStatus(int permissionSlipId, bool isApproved)
        {
            return await _repository.UpdatePermissionSlipStatus(permissionSlipId, isApproved);
        }

        public async Task<ServiceResponse<string>> AddPermissionSlip(PermissionSlip permissionSlipDto)
        {

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(permissionSlipDto.Student_Id + "  " + permissionSlipDto.Student_Parent_Info_id, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCodeImage = new QRCode(qrCodeData);

            using (Bitmap bitmap = qrCodeImage.GetGraphic(60))
            {
                string base64String;
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    base64String = Convert.ToBase64String(byteImage);
                }

                var result = await _imageService.SaveImageAsync(base64String, "Insititute_" + permissionSlipDto.Institute_id + "/PermissionQrCodes");
                permissionSlipDto.Qr_Code = result.relativePath;
            }
            return await _repository.AddPermissionSlip(permissionSlipDto);
        }

        public async Task<ServiceResponse<SinglePermissionSlipDTO>> GetPermissionSlipById(int permissionSlipId, int Institute_id)
        {
            var data = await _repository.GetPermissionSlipById(permissionSlipId, Institute_id);
            if (data.Data != null)
            {
                if (!string.IsNullOrEmpty(data.Data.Qr_Code) && File.Exists(data.Data.Qr_Code))
                {
                    data.Data.Qr_Code = _imageService.GetImageAsBase64(data.Data.Qr_Code);
                }

                if (!string.IsNullOrEmpty(data.Data.Parent_File) && File.Exists(data.Data.Parent_File))
                {
                    data.Data.Parent_File = _imageService.GetImageAsBase64(data.Data.Parent_File);
                }
            }
            return data;
        }

        public async Task<ServiceResponse<string>> ExportPermissionSlipsToExcel(int instituteId, int classId, int sectionId, int exportFormat)
        {
            try
            {
                // Fetch permission slips from the repository
                var permissionSlipsResponse = await GetAllPermissionSlips(instituteId, classId, sectionId, 1, int.MaxValue);

                // Check if permission slips were retrieved successfully
                //if (!permissionSlipsResponse.Success || permissionSlipsResponse.Data == null || !permissionSlipsResponse.Data.Any())
                //{
                //    return new ServiceResponse<string>(false, "No permission slips found", null, 404);
                //}
                var permissionSlips = permissionSlipsResponse.Data;

                // Define the headers for permission slips
                var headers = new List<string>
    {
        "Permission Slip ID", "Student ID", "Student Name", "Admission Number", "Class Name",
        "Section Name", "Gender", "Parent Name", "Requested Date", "Reason", "Status", "Modified Date"
    };

                // Call the generic export function
                return await _commonService.ExportDataToFile(permissionSlips, headers, exportFormat, $"PermissionSlips_{DateTime.Now:yyyyMMddHHmmss}");


                //var permissionSlips = permissionSlipsResponse.Data;
                //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                //// Create an Excel package using EPPlus
                //using (var package = new ExcelPackage())
                //{
                //    // Add a worksheet
                //    var worksheet = package.Workbook.Worksheets.Add("PermissionSlips");

                //    // Add headers
                //    worksheet.Cells[1, 1].Value = "Permission Slip ID";
                //    worksheet.Cells[1, 2].Value = "Student ID";
                //    worksheet.Cells[1, 3].Value = "Student Name";
                //    worksheet.Cells[1, 4].Value = "Admission Number";
                //    worksheet.Cells[1, 5].Value = "Class Name";
                //    worksheet.Cells[1, 6].Value = "Section Name";
                //    worksheet.Cells[1, 7].Value = "Gender";
                //    worksheet.Cells[1, 8].Value = "Parent Name";
                //    worksheet.Cells[1, 9].Value = "Requested Date";
                //    worksheet.Cells[1, 10].Value = "Reason";
                //    worksheet.Cells[1, 11].Value = "Status";
                //    worksheet.Cells[1, 12].Value = "Modified Date";

                //    // Add data rows
                //    var rowIndex = 2; // Start from row 2 as row 1 contains headers
                //    foreach (var slip in permissionSlips)
                //    {
                //        worksheet.Cells[rowIndex, 1].Value = slip.PermissionSlip_Id;
                //        worksheet.Cells[rowIndex, 2].Value = slip.Student_Id;
                //        worksheet.Cells[rowIndex, 3].Value = slip.StudentName;
                //        worksheet.Cells[rowIndex, 4].Value = slip.Admission_Number;
                //        worksheet.Cells[rowIndex, 5].Value = slip.ClassName;
                //        worksheet.Cells[rowIndex, 6].Value = slip.SectionName;
                //        worksheet.Cells[rowIndex, 7].Value = slip.GenderName;
                //        worksheet.Cells[rowIndex, 8].Value = slip.ParentName;
                //        worksheet.Cells[rowIndex, 9].Value = slip.RequestedDateTime;
                //        worksheet.Cells[rowIndex, 10].Value = slip.Reason;
                //        worksheet.Cells[rowIndex, 11].Value = slip.Status;
                //        worksheet.Cells[rowIndex, 12].Value = slip.ModifiedDate;

                //        rowIndex++;
                //    }

                //    // Auto-fit columns for better readability
                //    worksheet.Cells.AutoFitColumns();

                //    // Generate the Excel file as a byte array
                //    var excelFile = package.GetAsByteArray();

                //    // Save the file to a specific location or return the file content as a downloadable response
                //    var fileName = $"PermissionSlips_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports", fileName);

                //    // Ensure the directory exists
                //    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                //    // Write file to disk
                //    await File.WriteAllBytesAsync(filePath, excelFile);

                //    // Return the file path as a response
                //    return new ServiceResponse<string>(true, "Excel file generated successfully", filePath, 200);
                //}
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<string>> ExportPermissionSlipsToExcel(int instituteId, int classId, int sectionId, string startDate, string endDate, bool isApproved, int exportFormat)
        {
            try
            {
                // Fetch the filtered permission slips from the repository
                var permissionSlipsResponse = await GetPermissionSlips(instituteId, classId, sectionId, startDate, endDate, isApproved, 1, int.MaxValue);

                // Check if permission slips were retrieved successfully
                //if (!permissionSlipsResponse.Success || permissionSlipsResponse.Data == null || !permissionSlipsResponse.Data.Any())
                //{
                //    return new ServiceResponse<string>(false, "No permission slips found", null, 404);
                //}

                var permissionSlips = permissionSlipsResponse.Data;
                var latestPermissionSlips = permissionSlips
    .Select(x => new PermissionSlipExport
    {
        StudentName = x.StudentName,
        Admission_Number = x.Admission_Number,
        ClassName = x.ClassName,
        SectionName = x.SectionName,
        GenderName = x.GenderName,
        ParentName = x.ParentName,
        RequestedDateTime = x.RequestedDateTime,
        Reason = x.Reason,
        ModifiedDate = x.ModifiedDate
    })
    .ToList();
                var headers = new List<string>
    {
        "Student Name", "Admission Number", "Class Name",
        "Section Name", "Gender", "Parent Name", "Requested Date", "Reason", "Modified Date"
    };

                // Call the generic export function
                return await _commonService.ExportDataToFile(latestPermissionSlips, headers, exportFormat, isApproved ? "Approved_PermissionSlips" : "Rejected_PermissionSlips");


                //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                //// Create an Excel package using EPPlus
                //using (var package = new ExcelPackage())
                //{
                //    // Add a worksheet
                //    var worksheet = package.Workbook.Worksheets.Add("PermissionSlips");

                //    // Add headers
                //    worksheet.Cells[1, 1].Value = "Permission Slip ID";
                //    worksheet.Cells[1, 2].Value = "Student ID";
                //    worksheet.Cells[1, 3].Value = "Student Name";
                //    worksheet.Cells[1, 4].Value = "Admission Number";
                //    worksheet.Cells[1, 5].Value = "Class Name";
                //    worksheet.Cells[1, 6].Value = "Section Name";
                //    worksheet.Cells[1, 7].Value = "Gender";
                //    worksheet.Cells[1, 8].Value = "Parent Name";
                //    worksheet.Cells[1, 9].Value = "Requested Date";
                //    worksheet.Cells[1, 10].Value = "Reason";
                //    worksheet.Cells[1, 11].Value = "Status";
                //    worksheet.Cells[1, 12].Value = "Modified Date";

                //    // Add data rows
                //    var rowIndex = 2; // Start from row 2 as row 1 contains headers
                //    foreach (var slip in permissionSlips)
                //    {
                //        worksheet.Cells[rowIndex, 1].Value = slip.PermissionSlip_Id;
                //        worksheet.Cells[rowIndex, 2].Value = slip.Student_Id;
                //        worksheet.Cells[rowIndex, 3].Value = slip.StudentName;
                //        worksheet.Cells[rowIndex, 4].Value = slip.Admission_Number;
                //        worksheet.Cells[rowIndex, 5].Value = slip.ClassName;
                //        worksheet.Cells[rowIndex, 6].Value = slip.SectionName;
                //        worksheet.Cells[rowIndex, 7].Value = slip.GenderName;
                //        worksheet.Cells[rowIndex, 8].Value = slip.ParentName;
                //        worksheet.Cells[rowIndex, 9].Value = slip.RequestedDateTime;
                //        worksheet.Cells[rowIndex, 10].Value = slip.Reason;
                //        worksheet.Cells[rowIndex, 11].Value = slip.Status;
                //        worksheet.Cells[rowIndex, 12].Value = slip.ModifiedDate;

                //        rowIndex++;
                //    }

                //    // Auto-fit columns for better readability
                //    worksheet.Cells.AutoFitColumns();

                //    // Generate the Excel file as a byte array
                //    var excelFile = package.GetAsByteArray();
                //    var fileName = "";

                //    if (isApproved)
                //    {
                //         fileName = $"Approved_PermissionSlips_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                //    }
                //    else
                //    {
                //        fileName = $"Rejected_PermissionSlips_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                //    }
                //    // Save the file to a specific location or return the file content as a downloadable response

                //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports", fileName);

                //    // Ensure the directory exists
                //    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                //    // Write file to disk
                //    await File.WriteAllBytesAsync(filePath, excelFile);

                //    // Return the file path as a response
                //    return new ServiceResponse<string>(true, "Excel file generated successfully", filePath, 200);
                //}
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

    }

}
