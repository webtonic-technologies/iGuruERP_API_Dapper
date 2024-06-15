using Dapper;
using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;

namespace Institute_API.Repository.Implementations
{
    public class EventRepository : IEventRepository
    {
        private readonly IDbConnection _connection;

        public EventRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<ServiceResponse<int>> AddUpdateEvent(EventDTO eventDto)
        {
            try
            {
                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        // Save or update the event
                        string eventQuery;
                        if (eventDto.Event_id > 0)
                        {
                            eventQuery = @"
                        UPDATE [dbo].[tbl_CreateEvent]
                        SET EventName = @EventName,
                            StartDate = @StartDate,
                            EndDate = @EndDate,
                            Description = @Description,
                            Location = @Location,
                            ScheduleTime = @ScheduleTime,
                            Time = @Time,
                            AttachmentFile = @AttachmentFile
                        WHERE Event_id = @Event_id";
                        }
                        else
                        {
                            eventQuery = @"
                        INSERT INTO [dbo].[tbl_CreateEvent] (EventName, StartDate, EndDate, Description, Location, ScheduleTime, Time, AttachmentFile)
                        VALUES (@EventName, @StartDate, @EndDate, @Description, @Location, @ScheduleTime, @Time, @AttachmentFile);
                        SELECT SCOPE_IDENTITY();"
                            ; // Retrieve the inserted id
                        }

                        int insertedEventId = await _connection.ExecuteScalarAsync<int>(eventQuery, eventDto, transaction);

                        // Save or update EventEmployeeMappings
                        foreach (var mapping in eventDto.EmployeeMappings)
                        {
                            string employeeMappingQuery;
                            if (mapping.EventEmployeeMapping_id > 0)
                            {
                                employeeMappingQuery = @"
                            UPDATE [dbo].[tbl_EventEmployeeMapping]
                            SET Event_id = @Event_id,
                                Employee_id = @Employee_id
                            WHERE EventEmployeeMapping_id = @EventEmployeeMapping_id";
                            }
                            else
                            {
                                employeeMappingQuery = @"
                            INSERT INTO [dbo].[tbl_EventEmployeeMapping] (Event_id, Employee_id)
                            VALUES (@Event_id, @Employee_id)"
                                ;
                            }

                            int affectedRows = await _connection.ExecuteAsync(employeeMappingQuery, new { Event_id = insertedEventId, mapping.Employee_id, mapping.EventEmployeeMapping_id }, transaction);
                            if (affectedRows == 0)
                            {
                                throw new Exception("Failed to save Employee mapping");
                            }
                        }

                        // Save or update EventClassSessionMappings
                        foreach (var mapping in eventDto.ClassSessionMappings)
                        {

                            string classSessionMappingQuery;
                            if (mapping.EventClassSessionMapping_id > 0)
                            {
                                classSessionMappingQuery = @"
                            UPDATE [dbo].[tbl_EventClassSessionMapping]
                            SET Event_id = @Event_id,
                                Class_id = @Class_id,
                                Section_id = @Section_id
                            WHERE EventClassSessionMapping_id = @EventClassSessionMapping_id";
                            }
                            else
                            {
                                mapping.EventClassSessionMapping_id = 0;
                                classSessionMappingQuery = @"
                            INSERT INTO [dbo].[tbl_EventClassSessionMapping] (Event_id, Class_id, Section_id)
                            VALUES (@Event_id, @Class_id, @Section_id)"
                                ;
                            }

                            int affectedRows = await _connection.ExecuteAsync(classSessionMappingQuery, new { Event_id = insertedEventId, mapping.Class_id, mapping.Section_id, mapping.EventClassSessionMapping_id }, transaction);
                            if (affectedRows == 0)
                            {
                                throw new Exception("Failed to save Class Session mapping");
                            }
                        }

                        // Commit the transaction if all operations succeed
                        transaction.Commit();

                        return new ServiceResponse<int>(true, "Event and mappings saved successfully", insertedEventId, 200);
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if any operation fails
                        transaction.Rollback();
                        return new ServiceResponse<int>(false, ex.Message, 0, 500);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteEvent(int eventId)
        {
            try
            {
                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        // Delete EventEmployeeMappings
                        string deleteEmployeeMappingsQuery = @"
                    DELETE FROM [dbo].[tbl_EventEmployeeMapping]
                    WHERE Event_id = @eventId";
                        await _connection.ExecuteAsync(deleteEmployeeMappingsQuery, new { eventId }, transaction);

                        // Delete EventClassSessionMappings
                        string deleteClassSessionMappingsQuery = @"
                    DELETE FROM [dbo].[tbl_EventClassSessionMapping]
                    WHERE Event_id = @eventId";
                        await _connection.ExecuteAsync(deleteClassSessionMappingsQuery, new { eventId }, transaction);

                        // Delete the event
                        string deleteEventQuery = @"
                    DELETE FROM [dbo].[tbl_CreateEvent]
                    WHERE Event_id = @eventId";
                        await _connection.ExecuteAsync(deleteEventQuery, new { eventId }, transaction);

                        // Commit the transaction if all delete operations succeed
                        transaction.Commit();

                        return new ServiceResponse<bool>(true, "Event and associated mappings deleted successfully", true, 200);
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if any delete operation fails
                        transaction.Rollback();
                        return new ServiceResponse<bool>(false, ex.Message, false, 500);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<ServiceResponse<EventDTO>> GetEventById(int eventId)
        {
            try
            {
                // Get event details from tbl_CreateEvent
                string eventQuery = @"
            SELECT Event_id,EventName,StartDate,EndDate,Description,Location,ScheduleTime,Time,AttachmentFile,isApproved,approvedBy
            FROM tbl_CreateEvent
            WHERE Event_id = @EventId";

                var eventDto = await _connection.QuerySingleOrDefaultAsync<EventDTO>(eventQuery, new { EventId = eventId });

                if (eventDto == null)
                {
                    return new ServiceResponse<EventDTO>(false, "Event not found", null, 404);
                }

                // Get EventEmployeeMappings
                string employeeMappingsQuery = @"
            SELECT EventEmployeeMapping_id,
                   Event_id,
                   Employee_id,
                   CONCAT(tbl_EmployeeProfileMaster.First_Name, ' ', tbl_EmployeeProfileMaster.Last_Name) AS Employee_Name 
            FROM tbl_EventEmployeeMapping
            INNER JOIN tbl_EmployeeProfileMaster ON tbl_EmployeeProfileMaster.Employee_id = tbl_EventEmployeeMapping.Employee_id
            WHERE Event_id = @EventId";

                var employeeMappings = await _connection.QueryAsync<EventEmployeeMapping>(employeeMappingsQuery, new { EventId = eventId });

                // Get EventClassSessionMappings
                string classSessionMappingsQuery = @"
            SELECT EventClassSessionMapping_id,Event_id,Class_id,Section_id,class_course,Section
            FROM tbl_EventClassSessionMapping
            INNER JOIN tbl_CourseClass ON tbl_CourseClass.CourseClass_id = Class_id
            INNER JOIN tbl_CourseClassSection ON tbl_CourseClassSection.CourseClassSection_id = Section_id
            WHERE Event_id = @EventId";

                var classSessionMappings = await _connection.QueryAsync<EventClassSessionMapping>(classSessionMappingsQuery, new { EventId = eventId });

                eventDto.EmployeeMappings = employeeMappings.ToList();
                eventDto.ClassSessionMappings = classSessionMappings.ToList();

                return new ServiceResponse<EventDTO>(true, "Event retrieved successfully", eventDto, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EventDTO>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<bool>> ToggleEventActiveStatus(int eventId, bool isActive, int UserId)
        {
            try
            {
                string query = @"
            UPDATE tbl_CreateEvent
            SET isApproved = @IsActive , approvedBy = @UserId
            WHERE Event_id = @EventId";

                int rowsAffected = await _connection.ExecuteAsync(query, new { IsActive = isActive, EventId = eventId, UserId = UserId });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "Event status updated successfully", true, 200);
                }
                else
                {
                    return new ServiceResponse<bool>(false, "Event not found", false, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
        public async Task<ServiceResponse<List<EventDTO>>> GetApprovedEvents()
        {
            try
            {
                string query = @"
            SELECT Event_id,
                   EventName,
                   StartDate,
                   EndDate,
                   Description,
                   Location,
                   AttachmentFile
            FROM tbl_CreateEvent
            WHERE isApproved = 1";

                var events = await _connection.QueryAsync<EventDTO>(query);

                return new ServiceResponse<List<EventDTO>>(true, "Approved events retrieved successfully", events.ToList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EventDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<string>> GetEventAttachmentFileById(int eventId)
        {
            try
            {
                // Get event details from tbl_CreateEvent
                string eventQuery = @"
            SELECT  AttachmentFile,
            FROM tbl_CreateEvent
            WHERE Event_id = @EventId";

                var eventDto = await _connection.QuerySingleOrDefaultAsync<string>(eventQuery, new { EventId = eventId });

                if (eventDto == null)
                {
                    return new ServiceResponse<string>(false, "Event not found", null, 404);
                }
                return new ServiceResponse<string>(true, "Event retrieved successfully", eventDto, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }
    }
}
