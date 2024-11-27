using Dapper;
using EventGallery_API.DTOs.Requests.Approvals;
using EventGallery_API.DTOs.Responses;
using EventGallery_API.DTOs.Responses.Approvals;
using EventGallery_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EventGallery_API.Repository.Implementations
{
    public class HolidayApprovalRepository : IHolidayApprovalRepository
    {
        private readonly IDbConnection _dbConnection;

        public HolidayApprovalRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<GetAllHolidaysApprovalsResponse>> GetAllHolidaysApprovals(GetAllHolidaysApprovalsRequest request)
        {
            var query = @"
                SELECT 
                    h.HolidayID,
                    h.HolidayName,
                    h.FromDate,
                    h.ToDate,
                    h.Description,
                    CONCAT(CONVERT(VARCHAR, h.FromDate, 105), ' to ', CONVERT(VARCHAR, h.ToDate, 105)) AS Date,
                    CONCAT(emp.First_Name, ' ', emp.Last_Name) AS ReviewedBy,
                    h.StatusID,
                    c.class_name AS Class,
                    s.section_name AS Section
                FROM tblHolidays h
                LEFT JOIN tbl_EmployeeProfileMaster emp ON emp.Employee_id = h.ReviewerID
                LEFT JOIN tblHolidayClassSectionMapping hcs ON hcs.HolidayID = h.HolidayID
                LEFT JOIN tbl_Class c ON hcs.ClassID = c.class_id
                LEFT JOIN tbl_Section s ON hcs.SectionID = s.section_id
                WHERE h.InstituteID = @InstituteID
                AND (@Search IS NULL OR h.HolidayName LIKE '%' + @Search + '%')
                AND h.AcademicYearCode = @AcademicYearCode";  // Added filter for AcademicYearCode

            var parameters = new
            {
                request.InstituteID,
                Search = string.IsNullOrEmpty(request.Search) ? null : request.Search,
                request.AcademicYearCode  // Added AcademicYearCode to the parameters
            };

            var holidays = await _dbConnection.QueryAsync<GetAllHolidaysApprovalsResponse, ClassSectionResponse1, GetAllHolidaysApprovalsResponse>(
                query,
                (holiday, classSection) =>
                {
                    holiday.ClassSection = holiday.ClassSection ?? new List<ClassSectionResponse1>();
                    holiday.ClassSection.Add(classSection);
                    return holiday;
                },
                parameters,
                splitOn: "Class"
            );

            // Group holidays by HolidayID to avoid duplicate records
            var result = holidays
                .GroupBy(h => h.HolidayID)
                .Select(g =>
                {
                    var groupedHoliday = g.First();
                    groupedHoliday.ClassSection = g.Select(h => h.ClassSection.Single()).ToList();
                    return groupedHoliday;
                }).ToList();

            return result;
        }

        //public async Task<List<GetAllHolidaysApprovalsResponse>> GetAllHolidaysApprovals(GetAllHolidaysApprovalsRequest request)
        //{
        //    var query = @"
        //SELECT 
        //    h.HolidayID,
        //    h.HolidayName,
        //    h.FromDate,
        //    h.ToDate,
        //    h.Description,
        //    CONCAT(CONVERT(VARCHAR, h.FromDate, 105), ' to ', CONVERT(VARCHAR, h.ToDate, 105)) AS Date,
        //    CONCAT(emp.First_Name, ' ', emp.Last_Name) AS ReviewedBy,
        //    h.StatusID,
        //    c.class_name AS Class,
        //    s.section_name AS Section
        //FROM tblHolidays h
        //LEFT JOIN tbl_EmployeeProfileMaster emp ON emp.Employee_id = h.ReviewerID
        //LEFT JOIN tblHolidayClassSectionMapping hcs ON hcs.HolidayID = h.HolidayID
        //LEFT JOIN tbl_Class c ON hcs.ClassID = c.class_id
        //LEFT JOIN tbl_Section s ON hcs.SectionID = s.section_id
        //WHERE h.InstituteID = @InstituteID
        //AND (@Search IS NULL OR h.HolidayName LIKE '%' + @Search + '%')";

        //    var parameters = new
        //    {
        //        request.InstituteID,
        //        Search = string.IsNullOrEmpty(request.Search) ? null : request.Search
        //    };

        //    var holidays = await _dbConnection.QueryAsync<GetAllHolidaysApprovalsResponse, ClassSectionResponse1, GetAllHolidaysApprovalsResponse>(
        //        query,
        //        (holiday, classSection) =>
        //        {
        //            holiday.ClassSection = holiday.ClassSection ?? new List<ClassSectionResponse1>();
        //            holiday.ClassSection.Add(classSection);
        //            return holiday;
        //        },
        //        parameters,
        //        splitOn: "Class"
        //    );

        //    // Group holidays by HolidayID to avoid duplicate records
        //    var result = holidays
        //        .GroupBy(h => h.HolidayID)
        //        .Select(g =>
        //        {
        //            var groupedHoliday = g.First();
        //            groupedHoliday.ClassSection = g.Select(h => h.ClassSection.Single()).ToList();
        //            return groupedHoliday;
        //        }).ToList();

        //    return result;
        //}

        public async Task<bool> UpdateHolidayApprovalStatus(int holidayID, int statusID, int employeeID)
        {
            var query = @"
                UPDATE tblHolidays 
                SET StatusID = @StatusID,
                    ReviewerID = @EmployeeID
                WHERE HolidayID = @HolidayID;";

            var parameters = new
            {
                HolidayID = holidayID,
                StatusID = statusID,
                EmployeeID = employeeID
            };

            var rowsAffected = await _dbConnection.ExecuteAsync(query, parameters);
            return rowsAffected > 0;
        }
    }
}
