using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.ServiceResponse;
using System.Data;
using Attendance_SE_API.Services.Interfaces;
using Attendance_SE_API.Repository.Interfaces;
using OfficeOpenXml;
using System;

namespace Attendance_SE_API.Services.Implementations
{
    public class StudentImportService : IStudentImportService
    {
        private readonly IStudentImportRepository _studentImportRepository;

        public StudentImportService(IStudentImportRepository studentImportRepository)
        {
            _studentImportRepository = studentImportRepository;
        }

        public ServiceResponse<DataTable> DownloadAttendanceTemplate(StudentImportRequest request)
        {
            // Fetch the attendance data directly as a DataTable
            DataTable attendanceData = _studentImportRepository.GetAttendanceData(request);

            // Return the DataTable as part of the service response
            return new ServiceResponse<DataTable>(true, "Attendance Template Retrieved", attendanceData, 200);
        } 
        public byte[] GenerateExcelFile(DataTable dataTable, DateTime startDate, DateTime endDate)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Attendance");

                // Adding fixed columns headers
                worksheet.Cells[1, 1].Value = "StudentID";
                worksheet.Cells[1, 2].Value = "ClassID";
                worksheet.Cells[1, 3].Value = "SectionID";
                worksheet.Cells[1, 4].Value = "AdmissionNumber";
                worksheet.Cells[1, 5].Value = "RollNumber";
                worksheet.Cells[1, 6].Value = "StudentName";
                worksheet.Cells[1, 7].Value = "MobileNumber";

                // Add dynamic columns for each day between startDate and endDate
                int column = 8;  // Start from column H (8th column)
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    worksheet.Cells[1, column].Value = date.ToString("MMM dd, ddd");  // Format: Dec 17, Tue
                    column++;
                }

                // Hide columns A, B, C
                worksheet.Column(1).Hidden = true;
                worksheet.Column(2).Hidden = true;
                worksheet.Column(3).Hidden = true;

                // Make columns D, E, F, G non-editable
                for (int i = 4; i <= 7; i++)  // Columns D (4), E (5), F (6), G (7)
                {
                    worksheet.Column(i).Style.Locked = true;
                }

                // Populate the student data in the Excel sheet
                int rowIndex = 2; // Start from the second row
                foreach (DataRow studentRow in dataTable.Rows)
                {
                    // Handle DBNull for each value
                    worksheet.Cells[rowIndex, 1].Value = studentRow != null && studentRow["StudentID"] != DBNull.Value ? studentRow["StudentID"] : "";
                    worksheet.Cells[rowIndex, 2].Value = studentRow != null && studentRow["ClassID"] != DBNull.Value ? studentRow["ClassID"] : "";
                    worksheet.Cells[rowIndex, 3].Value = studentRow != null && studentRow["SectionID"] != DBNull.Value ? studentRow["SectionID"] : "";
                    worksheet.Cells[rowIndex, 4].Value = studentRow != null && studentRow["AdmissionNumber"] != DBNull.Value ? studentRow["AdmissionNumber"] : "";
                    worksheet.Cells[rowIndex, 5].Value = studentRow != null && studentRow["RollNumber"] != DBNull.Value ? studentRow["RollNumber"] : "";
                    worksheet.Cells[rowIndex, 6].Value = studentRow != null && studentRow["StudentName"] != DBNull.Value ? studentRow["StudentName"] : "";
                    worksheet.Cells[rowIndex, 7].Value = studentRow != null && studentRow["MobileNumber"] != DBNull.Value ? studentRow["MobileNumber"] : "";

                    rowIndex++;
                }

                // Protect the worksheet to prevent changes in locked cells (if needed)
                //worksheet.Protection.IsProtected = true;
                //worksheet.Protection.AllowSelectLockedCells = false;

                // Return the Excel file as a byte array
                return package.GetAsByteArray();
            }
        }


        // New method to get the status data for the Status sheet
        public DataTable GetStatusData()
        {
            // Call the repository method to get the status data
            return _studentImportRepository.GetStatusData();
        }

        // Method to add Status sheet to the Excel file
        public byte[] AddStatusSheetToExcel(byte[] excelFile, DataTable statusData)
        {
            using (var package = new ExcelPackage(new System.IO.MemoryStream(excelFile)))
            {
                var worksheet = package.Workbook.Worksheets.Add("Status");

                // Adding headers for Status sheet
                worksheet.Cells[1, 1].Value = "StatusID";
                worksheet.Cells[1, 2].Value = "StatusName";
                worksheet.Cells[1, 3].Value = "ShortName";

                worksheet.Column(1).Hidden = true;

                // Populate status data
                int rowIndex = 2;
                foreach (DataRow statusRow in statusData.Rows)
                {
                    worksheet.Cells[rowIndex, 1].Value = statusRow["StatusID"];
                    worksheet.Cells[rowIndex, 2].Value = statusRow["StatusName"];
                    worksheet.Cells[rowIndex, 3].Value = statusRow["ShortName"];
                    rowIndex++;
                }

                // Return the updated Excel file as a byte array
                return package.GetAsByteArray();
            }
        }

        public async Task<ServiceResponse<bool>> ImportAttendanceData(
            int instituteID,
            string classID,
            string sectionID,
            string attendanceDate,
            string studentID,
            string statusID,
            string academicYearCode)
        {
            try
            {
                // First, delete existing records to avoid duplicates
                await _studentImportRepository.DeleteStudentAttendanceData(
                    instituteID,
                    attendanceDate,
                    classID,
                    sectionID,
                    studentID,
                    academicYearCode
                );

                // Then insert the new data
                await _studentImportRepository.InsertStudentAttendanceData(
                    instituteID,
                    1, // AttendanceTypeID (You can adjust this value as per your requirements)
                    classID,
                    sectionID,
                    attendanceDate,
                    0, // SubjectID
                    3, // TimeSlotTypeID (You can adjust this value as per your requirements)
                    false, // IsMarkAsHoliday (Adjust as needed)
                    studentID,
                    statusID,
                    string.Empty, // Remarks (Adjust as needed)
                    academicYearCode
                );

                return new ServiceResponse<bool>(true, "Attendance data imported successfully", true, 200);
            }
            catch (System.Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
