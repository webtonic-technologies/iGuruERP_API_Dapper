using OfficeOpenXml;
using System.Text;
using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.Responses;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;
using VisitorManagement_API.Repository.Interfaces;
using VisitorManagement_API.Services.Interfaces;

namespace VisitorManagement_API.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateAppointment(Appointment appointment)
        {
            return await _appointmentRepository.AddUpdateAppointment(appointment);
        }

        public async Task<ServiceResponse<IEnumerable<AppointmentResponse>>> GetAllAppointments(GetAllAppointmentsRequest request)
        {
            return await _appointmentRepository.GetAllAppointments(request);
        }

        public async Task<ServiceResponse<AppointmentResponse>> GetAppointmentById(int appointmentId)
        {
            return await _appointmentRepository.GetAppointmentById(appointmentId);
        }

        public async Task<ServiceResponse<bool>> UpdateAppointmentStatus(int appointmentId)
        {
            return await _appointmentRepository.UpdateAppointmentStatus(appointmentId);
        }

        public async Task<ServiceResponse<byte[]>> GetAppointmentsExport(GetAppointmentsExportRequest request)
        {
            try
            {
                var appointments = await _appointmentRepository.GetAppointments(request);

                byte[] exportData = null;

                if (request.ExportType == 1) // Excel export
                {
                    exportData = GenerateExcel(appointments); // Generate Excel byte array
                }
                else if (request.ExportType == 2) // CSV export
                {
                    exportData = GenerateCsv(appointments); // Generate CSV byte array
                }

                return new ServiceResponse<byte[]>(
                    true,
                    "Export completed successfully",
                    exportData,
                    200
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(
                    false,
                    ex.Message,
                    null,
                    500
                );
            }
        }

        // Helper method to generate CSV
        private byte[] GenerateCsv(IEnumerable<GetAppointmentsExportResponse> appointments)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            using (var csv = new CsvHelper.CsvWriter(streamWriter, System.Globalization.CultureInfo.InvariantCulture))
            {
                // Write header
                csv.WriteHeader<GetAppointmentsExportResponse>();
                csv.NextRecord();

                // Write data
                foreach (var appointment in appointments)
                {
                    csv.WriteRecord(appointment);
                    csv.NextRecord();
                }

                // Flush the data to memory stream
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }

        // Helper method to generate Excel
        private byte[] GenerateExcel(IEnumerable<GetAppointmentsExportResponse> appointments)
        {
            using (var memoryStream = new MemoryStream())
            using (var package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Appointments");

                // Write header
                worksheet.Cells[1, 1].Value = "Appointee";
                worksheet.Cells[1, 2].Value = "OrganizationName";
                worksheet.Cells[1, 3].Value = "MobileNo";
                worksheet.Cells[1, 4].Value = "EmailID";
                worksheet.Cells[1, 5].Value = "PurposeName";
                worksheet.Cells[1, 6].Value = "EmployeeFullName";
                worksheet.Cells[1, 7].Value = "CheckInTime";
                worksheet.Cells[1, 8].Value = "CheckOutTime";
                worksheet.Cells[1, 9].Value = "Description";
                worksheet.Cells[1, 10].Value = "NoOfVisitors";
                worksheet.Cells[1, 11].Value = "ApprovalStatusName";

                int row = 2;
                foreach (var appointment in appointments)
                {
                    worksheet.Cells[row, 1].Value = appointment.Appointee;
                    worksheet.Cells[row, 2].Value = appointment.OrganizationName;
                    worksheet.Cells[row, 3].Value = appointment.MobileNo;
                    worksheet.Cells[row, 4].Value = appointment.EmailID;
                    worksheet.Cells[row, 5].Value = appointment.PurposeName;
                    worksheet.Cells[row, 6].Value = appointment.EmployeeFullName;
                    worksheet.Cells[row, 7].Value = appointment.CheckInTime;
                    worksheet.Cells[row, 8].Value = appointment.CheckOutTime;
                    worksheet.Cells[row, 9].Value = appointment.Description;
                    worksheet.Cells[row, 10].Value = appointment.NoOfVisitors;
                    worksheet.Cells[row, 11].Value = appointment.ApprovalStatusName;

                    row++;
                }

                // Save to memory stream
                package.Save();
                return memoryStream.ToArray();
            }
        }
    }
}
