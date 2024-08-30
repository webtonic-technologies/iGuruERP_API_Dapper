using Dapper;
using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Helper;
using Institute_API.Repository.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.Common;
using static Institute_API.Models.Enums;

namespace Institute_API.Repository.Implementations
{
    public class EventRepository : IEventRepository
    {
        private readonly IDbConnection _connection;

        public EventRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        //public async Task<ServiceResponse<int>> AddUpdateEvent(EventRequestDTO eventDto)
        //{
        //    try
        //    {
        //        _connection.Open();
        //        using (var transaction = _connection.BeginTransaction())
        //        {
        //            try
        //            {
        //                // Save or update the event
        //                string eventQuery;
        //                if (eventDto.Event_id > 0)
        //                {
        //                    eventQuery = @"
        //                UPDATE [dbo].[tbl_CreateEvent]
        //                SET EventName = @EventName,
        //                    StartDate = @StartDate,
        //                    EndDate = @EndDate,
        //                    Description = @Description,
        //                    Location = @Location,
        //                    ScheduleTime = @ScheduleDate,
        //                    Time = @ScheduleTime,
        //                    Institute_id=@Institute_id,
        //                    AttachmentFile = @AttachmentFile,
        //                    Academic_year_id = @Academic_year_id
        //                WHERE Event_id = @Event_id";
        //                }
        //                else
        //                {
        //                    eventQuery = @"
        //                INSERT INTO [dbo].[tbl_CreateEvent] (EventName, StartDate, EndDate, Description, Location, ScheduleTime, Time, AttachmentFile,Institute_id,Academic_year_id)
        //                VALUES (@EventName, @StartDate, @EndDate, @Description, @Location, @ScheduleDate, @ScheduleTime, @AttachmentFile,@Institute_id,@Academic_year_id);
        //                SELECT SCOPE_IDENTITY();"
        //                    ; // Retrieve the inserted id
        //                }

        //                int insertedEventId = await _connection.ExecuteScalarAsync<int>(eventQuery, eventDto, transaction);

        //                // Save or update EventEmployeeMappings
        //                foreach (var mapping in eventDto.EmployeeMappings)
        //                {
        //                    string employeeMappingQuery;
        //                    if (mapping.EventEmployeeMapping_id > 0)
        //                    {
        //                        employeeMappingQuery = @"
        //                    UPDATE [dbo].[tbl_EventEmployeeMapping]
        //                    SET Event_id = @Event_id,
        //                        Employee_id = @Employee_id
        //                    WHERE EventEmployeeMapping_id = @EventEmployeeMapping_id";
        //                    }
        //                    else
        //                    {
        //                        employeeMappingQuery = @"
        //                    INSERT INTO [dbo].[tbl_EventEmployeeMapping] (Event_id, Employee_id)
        //                    VALUES (@Event_id, @Employee_id)"
        //                        ;
        //                    }

        //                    int affectedRows = await _connection.ExecuteAsync(employeeMappingQuery, new { Event_id = insertedEventId, mapping.Employee_id, mapping.EventEmployeeMapping_id }, transaction);
        //                    if (affectedRows == 0)
        //                    {
        //                        throw new Exception("Failed to save Employee mapping");
        //                    }
        //                }

        //                // Save or update EventClassSessionMappings
        //                foreach (var mapping in eventDto.ClassSessionMappings)
        //                {

        //                    string classSessionMappingQuery;
        //                    if (mapping.EventClassSessionMapping_id > 0)
        //                    {
        //                        classSessionMappingQuery = @"
        //                    UPDATE [dbo].[tbl_EventClassSessionMapping]
        //                    SET Event_id = @Event_id,
        //                        Class_id = @Class_id,
        //                        Section_id = @Section_id
        //                    WHERE EventClassSessionMapping_id = @EventClassSessionMapping_id";
        //                    }
        //                    else
        //                    {
        //                        mapping.EventClassSessionMapping_id = 0;
        //                        classSessionMappingQuery = @"
        //                    INSERT INTO [dbo].[tbl_EventClassSessionMapping] (Event_id, Class_id, Section_id)
        //                    VALUES (@Event_id, @Class_id, @Section_id)"
        //                        ;
        //                    }

        //                    int affectedRows = await _connection.ExecuteAsync(classSessionMappingQuery, new { Event_id = insertedEventId, mapping.Class_id, mapping.Section_id, mapping.EventClassSessionMapping_id }, transaction);
        //                    if (affectedRows == 0)
        //                    {
        //                        throw new Exception("Failed to save Class Session mapping");
        //                    }
        //                }

        //                // Commit the transaction if all operations succeed
        //                transaction.Commit();

        //                return new ServiceResponse<int>(true, "Event and mappings saved successfully", insertedEventId, 200);
        //            }
        //            catch (Exception ex)
        //            {
        //                // Rollback the transaction if any operation fails
        //                transaction.Rollback();
        //                return new ServiceResponse<int>(false, ex.Message, 0, 500);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<int>(false, ex.Message, 0, 500);
        //    }
        //}

        public async Task<ServiceResponse<int>> AddUpdateEvent(EventRequestDTO eventDto)
        {
            try
            {
                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {

                        var StartDate = DateTimeHelper.ConvertToDateTime(eventDto.StartDate);
                        var EndDate = DateTimeHelper.ConvertToDateTime(eventDto.EndDate);
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
                        ScheduleTime = @ScheduleDate,
                        Time = @ScheduleTime,
                        Institute_id = @Institute_id,
                        AttachmentFile = @AttachmentFile,
                        Academic_year_id = @Academic_year_id
                    WHERE Event_id = @Event_id";
                        }
                        else
                        {
                            eventQuery = @"
                    INSERT INTO [dbo].[tbl_CreateEvent] (EventName, StartDate, EndDate, Description, Location, ScheduleTime, Time, AttachmentFile, Institute_id, Academic_year_id,CreatedBy,CreatedTime)
                    VALUES (@EventName, @StartDate, @EndDate, @Description, @Location, @ScheduleDate, @ScheduleTime, @AttachmentFile, @Institute_id, @Academic_year_id,@CreatedBy,GETDATE());
                    SELECT SCOPE_IDENTITY();";
                        }
                        int insertedEventId = await _connection.ExecuteScalarAsync<int>(eventQuery, new
                        {
                            eventDto.Event_id,
                            eventDto.EventName,
                            StartDate,
                            EndDate,
                            eventDto.Description,
                            eventDto.Location,
                            eventDto.ScheduleDate,
                            eventDto.ScheduleTime,
                            eventDto.AttachmentFile,
                            eventDto.Institute_id,
                            eventDto.Academic_year_id,
                            eventDto.CreatedBy
                        }, transaction);

                        if (eventDto.Event_id > 0)
                        {
                            insertedEventId = eventDto.Event_id;
                        }

                        // Retrieve existing Employee Mappings
                        var existingEmployeeMappings = await _connection.QueryAsync<int>(
                        "SELECT Employee_id FROM [dbo].[tbl_EventEmployeeMapping] WHERE Event_id = @Event_id",
                        new { Event_id = insertedEventId }, transaction);

                        // Delete removed Employee Mappings
                        var employeeIdsToDelete = existingEmployeeMappings.Except(eventDto.EmployeeMappings.Select(x => x.Employee_id)).ToList();
                        if (employeeIdsToDelete.Any())
                        {
                            string deleteEmployeeMappingsQuery = @"
                    DELETE FROM [dbo].[tbl_EventEmployeeMapping]
                    WHERE Event_id = @Event_id AND Employee_id IN @EmployeeIds";
                            await _connection.ExecuteAsync(deleteEmployeeMappingsQuery, new { Event_id = insertedEventId, EmployeeIds = employeeIdsToDelete }, transaction);
                        }

                        // Insert new Employee Mappings
                        var employeeIdsToInsert = eventDto.EmployeeMappings.Select(x => x.Employee_id).Except(existingEmployeeMappings).ToList();
                        if (employeeIdsToInsert.Any())
                        {
                            string insertEmployeeMappingsQuery = @"
                    INSERT INTO [dbo].[tbl_EventEmployeeMapping] (Event_id, Employee_id)
                    VALUES (@Event_id, @Employee_id)";
                            foreach (var employeeId in employeeIdsToInsert)
                            {
                                await _connection.ExecuteAsync(insertEmployeeMappingsQuery, new { Event_id = insertedEventId, Employee_id = employeeId }, transaction);
                            }
                        }

                        // Retrieve existing Class Session Mappings
                        var existingClassSessionMappings = await _connection.QueryAsync<(int Class_id, int Section_id)>(
                            "SELECT Class_id, Section_id FROM [dbo].[tbl_EventClassSessionMapping] WHERE Event_id = @Event_id",
                            new { Event_id = insertedEventId }, transaction);

                        // Delete removed Class Session Mappings
                        var classSessionMappingsToDelete = existingClassSessionMappings
                            .Except(eventDto.ClassSessionMappings.Select(x => (x.Class_id, x.Section_id))).ToList();
                        if (classSessionMappingsToDelete.Any())
                        {
                            string deleteClassSessionMappingsQuery = @"
                    DELETE FROM [dbo].[tbl_EventClassSessionMapping]
                    WHERE Event_id = @Event_id AND (Class_id, Section_id) IN @ClassSessionMappings";
                            await _connection.ExecuteAsync(deleteClassSessionMappingsQuery, new { Event_id = insertedEventId, ClassSessionMappings = classSessionMappingsToDelete }, transaction);
                        }

                        // Insert new Class Session Mappings
                        var classSessionMappingsToInsert = eventDto.ClassSessionMappings
                            .Select(x => (x.Class_id, x.Section_id))
                            .Except(existingClassSessionMappings).ToList();
                        if (classSessionMappingsToInsert.Any())
                        {
                            string insertClassSessionMappingsQuery = @"
                    INSERT INTO [dbo].[tbl_EventClassSessionMapping] (Event_id, Class_id, Section_id)
                    VALUES (@Event_id, @Class_id, @Section_id)";
                            foreach (var mapping in classSessionMappingsToInsert)
                            {
                                await _connection.ExecuteAsync(insertClassSessionMappingsQuery, new { Event_id = insertedEventId, mapping.Class_id, mapping.Section_id }, transaction);
                            }
                        }

                        if (eventDto.AttachmentFile != null && eventDto.AttachmentFile.Any())
                        {
                            // Delete existing attachments
                            string deleteAttachmentsQuery = @"
                    DELETE FROM [dbo].[tbl_EventFileMapping]
                    WHERE Event_id = @Event_id";
                            await _connection.ExecuteAsync(deleteAttachmentsQuery, new { Event_id = insertedEventId }, transaction);

                            // Insert new attachments
                            string insertAttachmentsQuery = @"
                    INSERT INTO [dbo].[tbl_EventFileMapping] (Event_id, attachment)
                    VALUES (@Event_id, @attachment)";

                            foreach (var file in eventDto.AttachmentFile)
                            {
                                await _connection.ExecuteAsync(insertAttachmentsQuery, new { Event_id = insertedEventId, attachment = file }, transaction);
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
                        //    string deleteEmployeeMappingsQuery = @"
                        //DELETE FROM [dbo].[tbl_EventEmployeeMapping]
                        //WHERE Event_id = @eventId";
                        //    await _connection.ExecuteAsync(deleteEmployeeMappingsQuery, new { eventId }, transaction);

                        //    // Delete EventClassSessionMappings
                        //    string deleteClassSessionMappingsQuery = @"
                        //DELETE FROM [dbo].[tbl_EventClassSessionMapping]
                        //WHERE Event_id = @eventId";
                        //    await _connection.ExecuteAsync(deleteClassSessionMappingsQuery, new { eventId }, transaction);

                        //    // Delete the event
                        //    string deleteEventQuery = @"
                        //DELETE FROM [dbo].[tbl_CreateEvent]
                        //WHERE Event_id = @eventId";

                        string query1 = @"
                         SELECT COUNT(0)
                         FROM [dbo].[tbl_Gallery]
                         WHERE Event_id = @eventId";

                        int count = await _connection.ExecuteScalarAsync<int>(query1, new { eventId }, transaction);

                        if (count > 0)
                        {
                            transaction.Rollback();
                            return new ServiceResponse<bool>(false, "There is a dependency in gallery documents, so it cannot be deleted.", false, 400);
                        }

                        string deleteEventQuery = @"UPDATE tbl_CreateEvent SET isDelete = 1 WHERE Event_id = @eventId";
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
            SELECT Event_id,EventName,StartDate,EndDate,Description,Location,ScheduleTime AS ScheduleDate,Time AS ScheduleTime,AttachmentFile,isApproved,approvedBy,Academic_year_id
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
                   tbl_EventEmployeeMapping.Employee_id,
                   CONCAT(tbl_EmployeeProfileMaster.First_Name, ' ', tbl_EmployeeProfileMaster.Last_Name) AS Employee_Name 
            FROM tbl_EventEmployeeMapping
            INNER JOIN tbl_EmployeeProfileMaster ON tbl_EmployeeProfileMaster.Employee_id = tbl_EventEmployeeMapping.Employee_id
            WHERE Event_id = @EventId";

                var employeeMappings = await _connection.QueryAsync<EventEmployeeMapping>(employeeMappingsQuery, new { EventId = eventId });

                // Get EventClassSessionMappings
                string classSessionMappingsQuery = @"
            SELECT EventClassSessionMapping_id,Event_id,tbl_Class.Class_id,tbl_Section.Section_id,class_name,section_name
            FROM tbl_EventClassSessionMapping
            INNER JOIN tbl_Class ON tbl_Class.class_id = tbl_EventClassSessionMapping.class_id
            INNER JOIN tbl_Section ON tbl_Section.section_id = tbl_EventClassSessionMapping.section_id
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
        public async Task<ServiceResponse<bool>> ToggleEventActiveStatus(int eventId, int Status, int UserId)
        {
            try
            {
                if (!Enum.IsDefined(typeof(Status_Enum), Status))
                {
                    return new ServiceResponse<bool>(false, "Invalid status value", false, 400);
                }

                string query = @"
            UPDATE tbl_CreateEvent
            SET Status = @Status , approvedBy = @UserId
            WHERE Event_id = @EventId";

                int rowsAffected = await _connection.ExecuteAsync(query, new { Status = Status, EventId = eventId, UserId = UserId });

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
        public async Task<ServiceResponse<List<EventDTO>>> GetApprovedEvents(int Institute_id, int Academic_year_id, int Status, string sortColumn, string sortDirection, int? pageSize = null, int? pageNumber = null)
        {
            try
            {
                // List of valid sortable columns
                var validSortColumns = new Dictionary<string, string>
{
    { "EventName", "EventName" },
    { "StartDate", "StartDate" },
    { "EndDate", "EndDate" }
};

                // Ensure the sort column is valid, default to "EventName" if not
                if (!validSortColumns.ContainsKey(sortColumn))
                {
                    sortColumn = "EventName";
                }
                else
                {
                    sortColumn = validSortColumns[sortColumn];
                }

                // Ensure sort direction is valid, default to "ASC" if not
                sortDirection = sortDirection.ToUpper() == "DESC" ? "DESC" : "ASC";

                // SQL queries
                string queryAll = @"
    SELECT Event_id,
           EventName,
           StartDate,
           EndDate,
           Description,
           Location,
           AttachmentFile
    FROM tbl_CreateEvent 
    WHERE  isApproved = 1 AND  Status = @Status AND isDelete = 0 AND Institute_id = @Institute_id AND (@Academic_year_id = 0 OR Academic_year_id=@Academic_year_id)";

                string queryCount = @"
    SELECT COUNT(*)
    FROM tbl_CreateEvent 
    WHERE  isApproved = 1 AND Status = @Status AND isDelete = 0 AND Institute_id = @Institute_id AND (@Academic_year_id = 0 OR Academic_year_id=@Academic_year_id)";

                List<EventDTO> events;
                int totalRecords = 0;

                if (pageSize.HasValue && pageNumber.HasValue)
                {
                    int offset = (pageNumber.Value - 1) * pageSize.Value;

                    // Build the paginated query with dynamic sorting
                    string queryPaginated = $@"
        {queryAll}
        ORDER BY {sortColumn} {sortDirection}
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY;

        {queryCount}";

                    using (var multi = await _connection.QueryMultipleAsync(queryPaginated, new { Offset = offset, PageSize = pageSize, Institute_id = Institute_id, Academic_year_id = Academic_year_id, Status = Status }))
                    {
                        events = multi.Read<EventDTO>().ToList();
                        totalRecords = multi.ReadSingle<int>();
                    }

                    return new ServiceResponse<List<EventDTO>>(true, "Approved events retrieved successfully", events, 200, totalRecords);
                }
                else
                {
                    // No pagination, return all records with sorting
                    string querySorted = $@"
        {queryAll}
        ORDER BY {sortColumn} {sortDirection}";

                    events = (await _connection.QueryAsync<EventDTO>(querySorted, new { Institute_id })).ToList();
                    return new ServiceResponse<List<EventDTO>>(true, "All approved events retrieved successfully", events, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EventDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<EventDTO>>> GetAllEvents(int Institute_id, int Academic_year_id, string sortColumn, string sortDirection, int? pageSize = null, int? pageNumber = null)
        {
            try
            {
                // List of valid sortable columns
                var validSortColumns = new Dictionary<string, string>
        {
            { "EventName", "EventName" },
            { "StartDate", "StartDate" },
            { "EndDate", "EndDate" }
        };

                // Ensure the sort column is valid, default to "EventName" if not
                if (!validSortColumns.ContainsKey(sortColumn))
                {
                    sortColumn = "EventName";
                }
                else
                {
                    sortColumn = validSortColumns[sortColumn];
                }

                // Ensure sort direction is valid, default to "ASC" if not
                sortDirection = sortDirection.ToUpper() == "DESC" ? "DESC" : "ASC";

                // SQL queries
                string queryAll = @"
            SELECT Event_id,
                   EventName,
                   StartDate,
                   EndDate,
                   Description,
                   Location,
                   AttachmentFile
            FROM tbl_CreateEvent 
            WHERE isDelete = 0 AND Institute_id = @Institute_id AND (@Academic_year_id = 0 OR Academic_year_id=@Academic_year_id)";

                string queryCount = @"
            SELECT COUNT(0)
            FROM tbl_CreateEvent 
            WHERE isDelete = 0 AND Institute_id = @Institute_id AND (@Academic_year_id = 0 OR Academic_year_id=@Academic_year_id)";

                List<EventDTO> events;
                int totalRecords = 0;

                if (pageSize.HasValue && pageNumber.HasValue)
                {
                    int offset = (pageNumber.Value - 1) * pageSize.Value;

                    // Build the paginated query with dynamic sorting
                    string queryPaginated = $@"
                {queryAll}
                ORDER BY {sortColumn} {sortDirection}
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                {queryCount}";

                    using (var multi = await _connection.QueryMultipleAsync(queryPaginated, new { Offset = offset, PageSize = pageSize, Institute_id = Institute_id, Academic_year_id = Academic_year_id }))
                    {
                        events = multi.Read<EventDTO>().ToList();
                        totalRecords = multi.ReadSingle<int>();
                    }

                    return new ServiceResponse<List<EventDTO>>(true, "Approved events retrieved successfully", events, 200, totalRecords);
                }
                else
                {
                    // No pagination, return all records with sorting
                    string querySorted = $@"
                {queryAll}
                ORDER BY {sortColumn} {sortDirection}";

                    events = (await _connection.QueryAsync<EventDTO>(querySorted, new { Institute_id })).ToList();
                    return new ServiceResponse<List<EventDTO>>(true, "All approved events retrieved successfully", events, 200);
                }
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
