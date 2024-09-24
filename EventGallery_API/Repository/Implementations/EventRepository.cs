﻿using Dapper;
using System.Data;
using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.Response;
using EventGallery_API.DTOs.ServiceResponse; // Ensure this is included
using EventGallery_API.Repository.Interfaces;

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
                @"INSERT INTO tblEvent (EventName, FromDate, ToDate, Description, Location, ScheduleDate, ScheduleTime, AcademicYearID, CreatedBy, InstituteID, IsActive)
          VALUES (@EventName, @FromDate, @ToDate, @Description, @Location, @ScheduleDate, @ScheduleTime, @AcademicYearID, @CreatedBy, @InstituteID, 1);
          SELECT CAST(SCOPE_IDENTITY() as int);"
                :
                @"UPDATE tblEvent SET EventName = @EventName, FromDate = @FromDate, ToDate = @ToDate, Description = @Description, Location = @Location, ScheduleDate = @ScheduleDate, ScheduleTime = @ScheduleTime, AcademicYearID = @AcademicYearID WHERE EventID = @EventID;
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
                request.AcademicYearID,
                request.CreatedBy,
                request.InstituteID,
                request.EventID
            };

            var eventId = await _connection.QuerySingleAsync<int>(query, parameters);

            // Handle Class and Section Mapping using tbl_Class and tbl_Section
            if (request.Students.All)
            {
                // Add all classes and sections based on tbl_Class and tbl_Section
                var insertAllClassSectionQuery = @"INSERT INTO tblEventClassSectionMapping (EventID, ClassID, SectionID)
                                           SELECT @EventID, c.class_id, s.section_id 
                                           FROM tbl_Class c 
                                           INNER JOIN tbl_Section s ON c.class_id = s.class_id
                                           WHERE c.IsDeleted = 0 AND s.IsDeleted = 0";
                await _connection.ExecuteAsync(insertAllClassSectionQuery, new { EventID = eventId });
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
                                       SELECT @EventID, EmployeeID FROM tblEmployee";
                await _connection.ExecuteAsync(insertAllEmployeeQuery, new { EventID = eventId });
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

        public async Task<ServiceResponse<List<EventResponse>>> GetAllEvents()
        {
            var query = "SELECT * FROM tblEvent WHERE IsActive = 1 AND IsDelete = 0";
            var events = await _connection.QueryAsync<EventResponse>(query);
            return new ServiceResponse<List<EventResponse>>(true, "Events fetched successfully", events.ToList(), 200);
        }

        public async Task<ServiceResponse<EventResponse>> GetEventById(int eventId)
        {
            var query = "SELECT * FROM tblEvent WHERE EventID = @EventID AND IsActive = 1 AND IsDelete = 0";
            var eventData = await _connection.QueryFirstOrDefaultAsync<EventResponse>(query, new { EventID = eventId });
            return eventData != null
                ? new ServiceResponse<EventResponse>(true, "Event fetched successfully.", eventData, 200)
                : new ServiceResponse<EventResponse>(false, "Event not found.", null, 404);
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
