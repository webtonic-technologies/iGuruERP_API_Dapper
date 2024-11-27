using Dapper;
using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.Responses;
using EventGallery_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using EventGallery_API.DTOs.ServiceResponse;

namespace EventGallery_API.Repository.Implementations
{
    public class EventApprovalRepository : IEventApprovalRepository
    {
        private readonly IDbConnection _dbConnection;

        public EventApprovalRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<List<GetAllEventsApprovalsResponse>>> GetAllEventsApprovals(GetAllEventsApprovalsRequest request)
        {
            var countQuery = @"
                SELECT COUNT(*)
                FROM tblEvent e
                WHERE e.InstituteID = @InstituteID
                AND (@Search IS NULL OR e.EventName LIKE '%' + @Search + '%')
                AND e.AcademicYearCode = @AcademicYearCode";  // Added filter for AcademicYearCode

            var countParameters = new
            {
                request.InstituteID,
                Search = string.IsNullOrEmpty(request.Search) ? null : request.Search,
                request.AcademicYearCode  // Pass AcademicYearCode
            };

            // Get the total count of events
            var totalCount = await _dbConnection.ExecuteScalarAsync<int?>(countQuery, countParameters) ?? 0;

            var query = @"
                SELECT 
                    e.EventID,
                    e.EventName,
                    CONCAT(CONVERT(VARCHAR, e.FromDate, 105), ' to ', CONVERT(VARCHAR, e.ToDate, 105)) AS Date,
                    e.Description,
                    e.Location,
                    CASE 
                        WHEN e.ScheduleTime IS NOT NULL 
                        THEN CONCAT(CONVERT(VARCHAR, e.ScheduleDate, 105), ' at ', RIGHT(CONVERT(VARCHAR(20), e.ScheduleTime, 100), 7)) 
                        ELSE CONCAT(CONVERT(VARCHAR, e.ScheduleDate, 105), ' at ', 'N/A')
                    END AS EventNotification,
                    CASE 
                        WHEN reviewer.First_Name IS NOT NULL AND reviewer.Last_Name IS NOT NULL
                        THEN CONCAT(reviewer.First_Name, ' ', reviewer.Last_Name)
                        ELSE 'N/A'
                    END AS ReviewedBy,
                    e.StatusID,
                    ISNULL(tea.Attachment, '') AS Document
                FROM tblEvent e
                LEFT JOIN tbl_EmployeeProfileMaster reviewer ON reviewer.Employee_id = e.ReviewerID
                LEFT JOIN tblEventAttachment tea ON e.EventID = tea.EventID
                WHERE e.InstituteID = @InstituteID
                AND (@Search IS NULL OR e.EventName LIKE '%' + @Search + '%')
                AND e.AcademicYearCode = @AcademicYearCode";  // Added filter for AcademicYearCode

            // Fetch the event data
            var events = await _dbConnection.QueryAsync<GetAllEventsApprovalsResponse>(query, countParameters);

            // Return the events with the total count wrapped in a ServiceResponse
            return new ServiceResponse<List<GetAllEventsApprovalsResponse>>(
                true,
                events.Any() ? "Events fetched successfully." : "No events found.",
                events.ToList(),
                200,
                totalCount // Include the total count here
            );
        }


        //public async Task<ServiceResponse<List<GetAllEventsApprovalsResponse>>> GetAllEventsApprovals(GetAllEventsApprovalsRequest request)
        //{
        //    // Query to get the total count of events before applying pagination
        //    var countQuery = @"
        //                SELECT COUNT(*)
        //                FROM tblEvent e
        //                WHERE e.InstituteID = @InstituteID
        //                AND (@Search IS NULL OR e.EventName LIKE '%' + @Search + '%')";

        //    // Calculate the parameters for the count query
        //    var countParameters = new
        //    {
        //        request.InstituteID,
        //        Search = string.IsNullOrEmpty(request.Search) ? null : request.Search
        //    };

        //    // Execute the count query and ensure it has a value
        //    var totalCount = await _dbConnection.ExecuteScalarAsync<int?>(countQuery, countParameters) ?? 0;

        //    // Query to fetch the actual event data with details
        //    var query = @"
        //        SELECT 
        //            e.EventID,
        //            e.EventName,
        //            CONCAT(CONVERT(VARCHAR, e.FromDate, 105), ' to ', CONVERT(VARCHAR, e.ToDate, 105)) AS Date,
        //            e.Description,
        //            e.Location,
        //            CASE 
        //                WHEN e.ScheduleTime IS NOT NULL 
        //                THEN CONCAT(CONVERT(VARCHAR, e.ScheduleDate, 105), ' at ', RIGHT(CONVERT(VARCHAR(20), e.ScheduleTime, 100), 7)) 
        //                ELSE CONCAT(CONVERT(VARCHAR, e.ScheduleDate, 105), ' at ', 'N/A')
        //            END AS EventNotification,
        //            CASE 
        //                WHEN reviewer.First_Name IS NOT NULL AND reviewer.Last_Name IS NOT NULL
        //                THEN CONCAT(reviewer.First_Name, ' ', reviewer.Last_Name)
        //                ELSE 'N/A'
        //            END AS ReviewedBy,
        //            e.StatusID,
        //            ISNULL(tea.Attachment, '') AS Document
        //        FROM tblEvent e
        //        LEFT JOIN tbl_EmployeeProfileMaster reviewer ON reviewer.Employee_id = e.ReviewerID
        //        LEFT JOIN tblEventAttachment tea ON e.EventID = tea.EventID
        //        WHERE e.InstituteID = @InstituteID
        //        AND (@Search IS NULL OR e.EventName LIKE '%' + @Search + '%')";

        //    // Fetch the event data
        //    var events = await _dbConnection.QueryAsync<GetAllEventsApprovalsResponse>(query, countParameters);

        //    // Return the events along with the total count in the service response
        //    return new ServiceResponse<List<GetAllEventsApprovalsResponse>>(
        //        true,
        //        events != null && events.Count() > 0 ? "Events fetched successfully." : "No events found.",
        //        events.ToList(),
        //        200,
        //        totalCount // Include the total count here
        //    );
        //}

        public async Task<bool> UpdateEventApprovalStatus(UpdateEventApprovalStatusRequest request)
        {
            var query = @"UPDATE tblEvent 
                  SET StatusID = @StatusID, 
                      ReviewerID = @EmployeeID 
                  WHERE EventID = @EventID";

            var result = await _dbConnection.ExecuteAsync(query, new
            {
                request.StatusID,
                request.EmployeeID,
                request.EventID
            });

            return result > 0; // Return true if at least one row is affected
        }

    }
}
