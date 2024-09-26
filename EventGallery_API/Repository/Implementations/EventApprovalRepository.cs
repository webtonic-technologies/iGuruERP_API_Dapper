using Dapper;
using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.Responses;
using EventGallery_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EventGallery_API.Repository.Implementations
{
    public class EventApprovalRepository : IEventApprovalRepository
    {
        private readonly IDbConnection _dbConnection;

        public EventApprovalRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<GetAllEventsApprovalsResponse>> GetAllEventsApprovals(GetAllEventsApprovalsRequest request)
        {
            var query = @"
            SELECT 
                e.EventID,
                e.EventName,
                CONCAT(CONVERT(VARCHAR, e.FromDate, 105), ' to ', CONVERT(VARCHAR, e.ToDate, 105)) AS Date,
                e.Description,
                e.Location,
                CASE 
                    WHEN e.ScheduleTime IS NOT NULL 
                    THEN CONCAT(CONVERT(VARCHAR, e.ScheduleDate, 105), ' at ', FORMAT(e.ScheduleTime, 'hh:mm tt')) 
                    ELSE CONCAT(CONVERT(VARCHAR, e.ScheduleDate, 105), ' at ', 'N/A')
                END AS EventNotification,
                CASE 
                    WHEN emp.First_Name IS NOT NULL AND emp.Last_Name IS NOT NULL
                    THEN CONCAT(emp.First_Name, ' ', emp.Last_Name)
                    ELSE 'N/A'
                END AS CreatedBy,
                e.StatusID
            FROM tblEvent e
            LEFT JOIN tbl_EmployeeProfileMaster emp ON emp.Employee_id = e.CreatedBy
            WHERE e.InstituteID = @InstituteID
                AND (@Search IS NULL OR e.EventName LIKE '%' + @Search + '%')";

            var parameters = new
            {
                request.InstituteID,
                Search = string.IsNullOrEmpty(request.Search) ? null : request.Search
            };

            var events = await _dbConnection.QueryAsync<GetAllEventsApprovalsResponse>(query, parameters);
            return events.ToList();
        }

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
