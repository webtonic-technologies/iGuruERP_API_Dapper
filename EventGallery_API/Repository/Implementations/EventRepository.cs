using Dapper;
using System.Data;
using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.Response;
using EventGallery_API.DTOs.ServiceResponse; // Ensure this is included
using EventGallery_API.Repository.Interfaces;
using EventGallery_API.DTOs.Responses;
using System.Data.Common;

namespace EventGallery_API.Repository.Implementations
{
    public class EventRepository : IEventRepository
    {
        private readonly IDbConnection _connection;

        public EventRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<int>> AddUpdateEvent(EventRequest request)
        {
            // Parse dates and time
            var fromDate = DateTime.ParseExact(request.FromDate, "dd-MM-yyyy", null);
            var toDate = DateTime.ParseExact(request.ToDate, "dd-MM-yyyy", null);
            var scheduleDate = DateTime.ParseExact(request.ScheduleDate, "dd-MM-yyyy", null);
            var scheduleTime = DateTime.ParseExact(request.ScheduleTime, "hh:mm tt", null).TimeOfDay;

            var query = request.EventID == 0 ?
                @"INSERT INTO tblEvent (EventName, FromDate, ToDate, Description, Location, ScheduleDate, ScheduleTime, AcademicYearCode, CreatedBy, InstituteID, IsActive)
          VALUES (@EventName, @FromDate, @ToDate, @Description, @Location, @ScheduleDate, @ScheduleTime, @AcademicYearCode, @CreatedBy, @InstituteID, 1);
          SELECT CAST(SCOPE_IDENTITY() as int);"
                :
                @"UPDATE tblEvent SET EventName = @EventName, FromDate = @FromDate, ToDate = @ToDate, Description = @Description, Location = @Location, ScheduleDate = @ScheduleDate, ScheduleTime = @ScheduleTime, AcademicYearCode = @AcademicYearCode WHERE EventID = @EventID;
          SELECT @EventID;";

            var parameters = new
            {
                request.EventName,
                FromDate = fromDate,
                ToDate = toDate,
                request.Description,
                request.Location,
                ScheduleDate = scheduleDate,
                ScheduleTime = scheduleTime,
                request.AcademicYearCode,
                request.CreatedBy,
                request.InstituteID,
                request.EventID
            };

            var eventId = await _connection.QuerySingleAsync<int>(query, parameters);

            // Handle Class and Section Mapping using tbl_Class and tbl_Section
            if (request.Students.All)
            {
                // Add all classes and sections based on tbl_Class and tbl_Section with InstituteID filter
                var insertAllClassSectionQuery = @"INSERT INTO tblEventClassSectionMapping (EventID, ClassID, SectionID)
                                           SELECT @EventID, c.class_id, s.section_id 
                                           FROM tbl_Class c 
                                           INNER JOIN tbl_Section s ON c.class_id = s.class_id
                                           WHERE c.IsDeleted = 0 AND s.IsDeleted = 0 AND c.institute_id = @InstituteID";
                await _connection.ExecuteAsync(insertAllClassSectionQuery, new { EventID = eventId, request.InstituteID });

            }
            else if (request.Students.ClassSection != null)
            {
                foreach (var classSection in request.Students.ClassSection)
                {
                    var insertClassSectionQuery = @"INSERT INTO tblEventClassSectionMapping (EventID, ClassID, SectionID)
                                            VALUES (@EventID, @ClassID, @SectionID)";
                    await _connection.ExecuteAsync(insertClassSectionQuery, new
                    {
                        EventID = eventId,
                        ClassID = classSection.ClassID,
                        SectionID = classSection.SectionID
                    });
                }
            }

            // Handle Employee Mapping (same logic as before)
            if (request.Employee.All)
            {
                var insertAllEmployeeQuery = @"INSERT INTO tblEventEmployeeMapping (EventID, EmployeeID)
                                       SELECT @EventID, Employee_id FROM tbl_EmployeeProfileMaster 
                                       WHERE Institute_id = @InstituteID";
                await _connection.ExecuteAsync(insertAllEmployeeQuery, new { EventID = eventId, request.InstituteID });
            }
            else if (request.Employee.EmployeeID != null)
            {
                foreach (var employeeId in request.Employee.EmployeeID)
                {
                    var insertEmployeeQuery = @"INSERT INTO tblEventEmployeeMapping (EventID, EmployeeID)
                                        VALUES (@EventID, @EmployeeID)";
                    await _connection.ExecuteAsync(insertEmployeeQuery, new { EventID = eventId, EmployeeID = employeeId });
                }
            }

            // Handle Attachment (same logic as before)
            if (!string.IsNullOrEmpty(request.Attachment))
            {
                var insertAttachmentQuery = @"INSERT INTO tblEventAttachment (EventID, Attachment)
                                      VALUES (@EventID, @Attachment)";
                await _connection.ExecuteAsync(insertAttachmentQuery, new { EventID = eventId, Attachment = request.Attachment });
            }

            return new ServiceResponse<int>(true, "Event added/updated successfully.", eventId, 200);
        }

        //public async Task<ServiceResponse<List<GetAllEventsResponse>>> GetAllEvents(GetAllEventsRequest request)
        //{
        //    var query = @"
        //        SELECT 
        //        e.EventID,
        //        e.EventName,
        //        CONCAT(CONVERT(VARCHAR, e.FromDate, 105), ' to ', CONVERT(VARCHAR, e.ToDate, 105)) AS Date,
        //        e.Description AS Document,
        //        e.Location,
        //        CASE 
        //            WHEN e.ScheduleTime IS NOT NULL 
        //            THEN CONCAT(CONVERT(VARCHAR, e.ScheduleDate, 105), ' at ', FORMAT(e.ScheduleTime, 'hh:mm tt')) 
        //            ELSE CONCAT(CONVERT(VARCHAR, e.ScheduleDate, 105), ' at ', 'N/A')
        //        END AS EventNotification,
        //        CASE 
        //            WHEN emp.First_Name IS NOT NULL AND emp.Last_Name IS NOT NULL
        //            THEN CONCAT(emp.First_Name, ' ', emp.Last_Name)
        //            ELSE 'N/A'
        //        END AS CreatedBy
        //    FROM tblEvent e
        //    LEFT JOIN tbl_EmployeeProfileMaster emp ON emp.Employee_id = e.CreatedBy
        //    WHERE e.AcademicYearID = @AcademicYearID AND e.InstituteID = @InstituteID;";

        //    var parameters = new
        //    {
        //        request.AcademicYearID,
        //        request.InstituteID
        //    };

        //    var events = await _connection.QueryAsync<GetAllEventsResponse>(query, parameters);
        //    return new ServiceResponse<List<GetAllEventsResponse>>(true, "Events fetched successfully.", events.ToList(), 200);
        //}

        public async Task<ServiceResponse<List<GetAllEventsResponse>>> GetAllEvents(GetAllEventsRequest request)
        {
            // Query to get the total count of events (before applying paging, if any), with optional search criteria
            var countQuery = @"
    SELECT COUNT(*)
    FROM tblEvent e
    WHERE e.AcademicYearCode = @AcademicYearCode AND e.InstituteID = @InstituteID
    AND (@Search IS NULL OR e.EventName LIKE '%' + @Search + '%');";

            // Query to get the actual event data, with optional search and paging
            var query = @"
    SELECT 
        e.EventID,
        e.EventName,
        CONCAT(CONVERT(VARCHAR, e.FromDate, 105), ' to ', CONVERT(VARCHAR, e.ToDate, 105)) AS Date,
        e.Description AS Description,
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
        ISNULL(tea.Attachment, '') AS Document
    FROM tblEvent e
    LEFT JOIN tbl_EmployeeProfileMaster emp ON emp.Employee_id = e.CreatedBy
    LEFT JOIN tblEventAttachment tea ON e.EventID = tea.EventID
    WHERE e.AcademicYearCode = @AcademicYearCode AND e.InstituteID = @InstituteID
    AND (@Search IS NULL OR e.EventName LIKE '%' + @Search + '%')
    ORDER BY e.EventName
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;"; // Paging logic using OFFSET and FETCH

            var parameters = new
            {
                AcademicYearCode = request.AcademicYearCode, // Update parameter name
                request.InstituteID,
                Search = string.IsNullOrEmpty(request.Search) ? null : request.Search, // Handle null search
                Offset = (request.PageNumber - 1) * request.PageSize, // Calculate offset
                PageSize = request.PageSize
            };

            // Get the total count of events (without paging)
            var totalCount = await _connection.ExecuteScalarAsync<int>(countQuery, parameters);

            // Get the paginated list of events
            var events = await _connection.QueryAsync<GetAllEventsResponse>(query, parameters);

            // Return the events along with the total count
            return new ServiceResponse<List<GetAllEventsResponse>>(true, "Events fetched successfully.", events.ToList(), 200, totalCount);
        }


        //public async Task<ServiceResponse<List<GetAllEventsResponse>>> GetAllEvents(GetAllEventsRequest request)
        //{
        //    var query = @"
        //SELECT e.EventID, e.EventName, 
        //       CONVERT(varchar, e.FromDate, 105) + ' to ' + CONVERT(varchar, e.ToDate, 105) AS Date,
        //       e.Description, e.Location, 
        //       CONVERT(varchar, e.ScheduleDate, 105) + ' at ' + FORMAT(e.ScheduleTime, 'hh:mm tt') AS EventNotification,
        //       ep.First_Name + ' ' + ep.Last_Name AS CreatedBy, 
        //       e.StatusID
        //FROM tblEvent e
        //LEFT JOIN tbl_EmployeeProfileMaster ep ON e.CreatedBy = ep.Employee_id
        //WHERE e.InstituteID = @InstituteID 
        //AND (@Search IS NULL OR e.EventName LIKE '%' + @Search + '%')
        //ORDER BY e.EventID DESC
        //OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        //    var events = await _connection.QueryAsync<GetAllEventsResponse>(query, new
        //    {
        //        request.InstituteID,
        //        request.Search,
        //        Offset = (request.PageNumber - 1) * request.PageSize,
        //        request.PageSize
        //    });

        //    var totalCount = await GetTotalEventCount(request); // Total number of events

        //    return new ServiceResponse<List<GetAllEventsResponse>>(true, "Events fetched successfully.", events.ToList(), 200, totalCount);
        //}



        public async Task<int> GetTotalEventCount(GetAllEventsRequest request)
        {
            var query = @"
        SELECT COUNT(*)
        FROM tblEvent e
        WHERE e.InstituteID = @InstituteID
        AND (@Search IS NULL OR e.EventName LIKE '%' + @Search + '%')";

            return await _connection.ExecuteScalarAsync<int>(query, new
            {
                request.InstituteID,
                request.Search
            });
        }

        //public async Task<ServiceResponse<EventResponse>> GetEventById(int eventId)
        //{
        //    var query = "SELECT * FROM tblEvent WHERE EventID = @EventID AND IsActive = 1 AND IsDelete = 0";
        //    var eventData = await _connection.QueryFirstOrDefaultAsync<EventResponse>(query, new { EventID = eventId });
        //    return eventData != null
        //        ? new ServiceResponse<EventResponse>(true, "Event fetched successfully.", eventData, 200)
        //        : new ServiceResponse<EventResponse>(false, "Event not found.", null, 404);
        //}

        public async Task<ServiceResponse<GetAllEventsResponse>> GetEventById(int eventId)
        {
            // Query to get the event details along with additional information for response
            var query = @"
                        SELECT 
                            e.EventID,
                            e.EventName,
                            CONCAT(CONVERT(VARCHAR, e.FromDate, 105), ' to ', CONVERT(VARCHAR, e.ToDate, 105)) AS Date,
                            e.Description AS Description,
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
                            ISNULL(tea.Attachment, '') AS Document
                        FROM tblEvent e
                        LEFT JOIN tbl_EmployeeProfileMaster emp ON emp.Employee_id = e.CreatedBy
                        LEFT JOIN tblEventAttachment tea ON e.EventID = tea.EventID
                        WHERE e.EventID = @EventID AND e.IsActive = 1 AND e.IsDelete = 0;";

            // Fetch the event details using the query
            var eventData = await _connection.QueryFirstOrDefaultAsync<GetAllEventsResponse>(query, new { EventID = eventId });

            // Return the response in the new format
            return eventData != null
                ? new ServiceResponse<GetAllEventsResponse>(true, "Event fetched successfully.", eventData, 200)
                : new ServiceResponse<GetAllEventsResponse>(false, "Event not found.", null, 404);
        }


        public async Task<ServiceResponse<bool>> DeleteEvent(int eventId)
        {
            var query = "UPDATE tblEvent SET IsActive = 0 WHERE EventID = @EventID";
            await _connection.ExecuteAsync(query, new { EventID = eventId });
            return new ServiceResponse<bool>(true, "Event deleted successfully.", true, 200);
        }

        public async Task<ServiceResponse<bool>> ExportAllEvents() // Ensure return type matches the interface
        {
            // Export logic would go here
            return new ServiceResponse<bool>(true, "Exported all events.", true, 200);
        }
    }
}
