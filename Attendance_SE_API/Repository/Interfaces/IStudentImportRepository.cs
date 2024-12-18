using Attendance_SE_API.DTOs.Requests;
using System.Collections.Generic;
using Attendance_SE_API.DTOs;
using System.Data;
namespace Attendance_SE_API.Repository.Interfaces
{
   
    public interface IStudentImportRepository
    {
        DataTable GetAttendanceData(StudentImportRequest request);
        DataTable GetStatusData();
        Task DeleteStudentAttendanceData(
            int instituteID,
            string attendanceDate,
            string classID,
            string sectionID,
            string studentID,
            string academicYearCode
        );

        Task InsertStudentAttendanceData(
            int instituteID,
            int attendanceTypeID,
            string classID,
            string sectionID,
            string attendanceDate,
            int subjectID,
            int timeSlotTypeID,
            bool isMarkAsHoliday,
            string studentID,
            string statusID,
            string remarks,
            string academicYearCode
        );
    }
}
