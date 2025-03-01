using CsvHelper;
using OfficeOpenXml;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;
using StudentManagement_API.Repository.Interfaces;
using StudentManagement_API.Services.Interfaces;
using System.Globalization;
using System.Text;

namespace StudentManagement_API.Services.Implementations
{
    public class CertificatesService : ICertificatesService
    {
        private readonly ICertificatesRepository _certificatesRepository;

        public CertificatesService(ICertificatesRepository certificatesRepository)
        {
            _certificatesRepository = certificatesRepository;
        }

        public async Task<ServiceResponse<int>> CreateCertificateTemplateAsync(CreateCertificateTemplateRequest request)
        {
            try
            {
                int id = await _certificatesRepository.CreateCertificateTemplateAsync(request);
                return new ServiceResponse<int>(true, "Certificate template created successfully.", id, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetCertificateTemplateResponse>>> GetCertificateTemplateAsync(GetCertificateTemplateRequest request)
        {
            try
            {
                var data = await _certificatesRepository.GetCertificateTemplateAsync(request);
                return new ServiceResponse<IEnumerable<GetCertificateTemplateResponse>>(
                    true,
                    "Certificate template retrieved successfully.",
                    data,
                    200,
                    data.Count()
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetCertificateTemplateResponse>>(
                    false,
                    ex.Message,
                    null,
                    500
                );
            }
        }

        //public async Task<ServiceResponse<int>> GenerateCertificateAsync(GenerateCertificateRequest request)
        //{
        //    try
        //    {
        //        int certificateId = await _certificatesRepository.GenerateCertificateAsync(request);
        //        return new ServiceResponse<int>(true, "Certificate generated successfully.", certificateId, 200);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<int>(false, ex.Message, 0, 500);
        //    }
        //}


        public async Task<ServiceResponse<List<int>>> GenerateCertificatesAsync(GenerateCertificateRequest request)
        {
            try
            {
                var certificateIds = await _certificatesRepository.GenerateCertificatesAsync(request);
                return new ServiceResponse<List<int>>(true, "Certificates generated successfully.", certificateIds, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<int>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetStudentsResponse>>> GetStudentsAsync(GetStudentsRequest request)
        {
            try
            {
                var data = await _certificatesRepository.GetStudentsAsync(request);
                return new ServiceResponse<IEnumerable<GetStudentsResponse>>(
                    true,
                    "Students retrieved successfully.",
                    data,
                    200,
                    data.Count()
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetStudentsResponse>>(
                    false,
                    ex.Message,
                    null,
                    500
                );
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetCertificateReportResponse>>> GetCertificateReportAsync(GetCertificateReportRequest request)
        {
            try
            {
                var data = await _certificatesRepository.GetCertificateReportAsync(request);
                return new ServiceResponse<IEnumerable<GetCertificateReportResponse>>(
                    true,
                    "Certificate report retrieved successfully.",
                    data,
                    200,
                    data.Count()
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetCertificateReportResponse>>(
                    false,
                    ex.Message,
                    null,
                    500
                );
            }
        }

        public async Task<ServiceResponse<string>> GetCertificateReportExportAsync(GetCertificateReportExportRequest request)
        {
            // Retrieve export data from repository.
            var data = await _certificatesRepository.GetCertificateReportExportAsync(request);
            if (data == null || !data.Any())
            {
                return new ServiceResponse<string>(false, "No records found", null, 404);
            }

            string filePath = string.Empty;
            if (request.ExportType == 1)
            {
                // Export to Excel using EPPlus.
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("CertificateReportExport");
                    // Load data into worksheet; headers are generated automatically.
                    worksheet.Cells["A1"].LoadFromCollection(data, true);
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "CertificateReportExport.xlsx");
                    File.WriteAllBytes(filePath, package.GetAsByteArray());
                }
            }
            else if (request.ExportType == 2)
            {
                // Export to CSV using CsvHelper.
                var sb = new StringBuilder();
                using (var writer = new StringWriter(sb))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(data);
                    writer.Flush();
                }
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "CertificateReportExport.csv");
                File.WriteAllText(filePath, sb.ToString());
            }
            else
            {
                return new ServiceResponse<string>(false, "Invalid ExportType.", null, 400);
            }

            return new ServiceResponse<string>(true, "Export file generated", filePath, 200);
        }
    }
}
