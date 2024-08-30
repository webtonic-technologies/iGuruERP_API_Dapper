using Dapper;
using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Helper;
using Institute_API.Repository.Interfaces;
using System.Data;
using System.Data.Common;
using static Institute_API.Models.Enums;

namespace Institute_API.Repository.Implementations
{
    public class HolidayRepository : IHolidayRepository
    {
        private readonly IDbConnection _connection;

        public HolidayRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<ServiceResponse<int>> AddUpdateHoliday(HolidayRequestDTO holidayDTO)
        {
            try
            {
                var StartDate = DateTimeHelper.ConvertToDateTime(holidayDTO.StartDate);
                var EndDate = DateTimeHelper.ConvertToDateTime(holidayDTO.EndDate);
                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    string holidayQuery;
                    int holidayId = holidayDTO.Holiday_id;

                    // Insert or update holiday record
                    if (holidayId > 0)
                    {
                        holidayQuery = @"
                    UPDATE [dbo].[tbl_Holiday]
                    SET 
                        HolidayName = @HolidayName,
                        StartDate = @StartDate,
                        EndDate = @EndDate,
                        Description = @Description,
                        Institute_id = @Institute_id.
                        Academic_year_id =@Academic_year_id
                    WHERE Holiday_id = @Holiday_id;
                ";
                    }
                    else
                    {
                        holidayQuery = @"
                    INSERT INTO [dbo].[tbl_Holiday] (HolidayName, StartDate, EndDate, Description,Institute_id,Academic_year_id,CreatedBy,CreatedTime)
                    VALUES (@HolidayName, @StartDate, @EndDate, @Description,@Institute_id,@Academic_year_id,@CreatedBy,GETDATE());
                    SELECT SCOPE_IDENTITY();
                ";
                    }

                    // Execute holiday query and retrieve the holiday ID
                    holidayId = await _connection.ExecuteScalarAsync<int>(holidayQuery, new
                    {
                        holidayDTO.HolidayName,
                        StartDate,
                        EndDate,
                        holidayDTO.Description,
                        holidayDTO.Institute_id,
                        holidayDTO.Academic_year_id,
                        holidayDTO.Holiday_id,
                        holidayDTO.CreatedBy
                    }, transaction);

                    // Insert or update class session mappings
                    foreach (var mapping in holidayDTO.ClassSessionMappings)
                    {
                        string mappingQuery;
                        int affectedRows;

                        // Construct the query based on whether the mapping already exists
                        if (mapping.HolidayClassSessionMapping_id > 0)
                        {
                            mappingQuery = @"
                        UPDATE [dbo].[tbl_HolidayClassSessionMapping]
                        SET 
                            Class_id = @Class_id,
                            Section_id = @Section_id
                        WHERE HolidayClassSessionMapping_id = @HolidayClassSessionMapping_id;
                    ";
                        }
                        else
                        {
                            mapping.HolidayClassSessionMapping_id = 0;
                            mappingQuery = @"
                        INSERT INTO [dbo].[tbl_HolidayClassSessionMapping] (Holiday_id, Class_id, Section_id)
                        VALUES (@Holiday_id, @Class_id, @Section_id);
                    ";
                        }

                        // Execute the mapping query
                        affectedRows = await _connection.ExecuteAsync(mappingQuery, new { Holiday_id = holidayId, mapping.Class_id, mapping.Section_id, mapping.HolidayClassSessionMapping_id }, transaction);
                        if (affectedRows == 0)
                        {
                            throw new Exception("Failed to save class session mapping");
                        }
                    }

                    // Commit transaction
                    transaction.Commit();

                    return new ServiceResponse<int>(true, "Holiday added/updated successfully", holidayId, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<HolidayDTO>> GetHolidayById(int holidayId)
        {
            try
            {
                string holidayQuery = @"
            SELECT Holiday_id, HolidayName, StartDate, EndDate, Description, IsApproved, ApprovedBy,Institute_id,tbl_Holiday.Academic_year_id,YearName
            LEFT JOIN tbl_AcademicYear on tbl_AcademicYear.Academic_year_id = tbl_Holiday.Academic_year_id
            FROM [dbo].[tbl_Holiday]
            WHERE Holiday_id = @HolidayId ;
        ";

                var holiday = await _connection.QueryFirstOrDefaultAsync<HolidayDTO>(holidayQuery, new { HolidayId = holidayId });

                if (holiday == null)
                {
                    return new ServiceResponse<HolidayDTO>(false, "Holiday not found", null, 404);
                }

                string mappingQuery = @"
            SELECT HolidayClassSessionMapping_id, Holiday_id, tbl_HolidayClassSessionMapping.Class_id, tbl_HolidayClassSessionMapping.Section_id,class_name,section_name
            FROM [dbo].[tbl_HolidayClassSessionMapping]
            INNER JOIN tbl_Class ON tbl_Class.Class_id = tbl_HolidayClassSessionMapping.Class_id
            INNER JOIN tbl_Section ON tbl_Section.Section_id = tbl_HolidayClassSessionMapping.Section_id
            WHERE Holiday_id = @HolidayId;
        ";

                var mappings = await _connection.QueryAsync<HolidayClassSessionMappingDTO>(mappingQuery, new { HolidayId = holidayId });

                holiday.ClassSessionMappings = mappings.ToList();

                return new ServiceResponse<HolidayDTO>(true, "Holiday retrieved successfully", holiday, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HolidayDTO>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<HolidayDTO>>> GetAllHolidays(int Institute_id, int Academic_year_id, string sortColumn, string sortDirection, int? pageSize = null, int? pageNumber = null)
        {
            try
            {
                // List of valid sortable columns
                var validSortColumns = new Dictionary<string, string>
        {
            { "HolidayName", "HolidayName" },
            { "StartDate", "StartDate" },
            { "EndDate", "EndDate" }
        };

                // Ensure the sort column is valid, default to "HolidayName" if not
                if (!validSortColumns.ContainsKey(sortColumn))
                {
                    sortColumn = "HolidayName";
                }
                else
                {
                    sortColumn = validSortColumns[sortColumn];
                }

                // Ensure sort direction is valid, default to "ASC" if not
                sortDirection = sortDirection.ToUpper() == "DESC" ? "DESC" : "ASC";

                // SQL queries
                string queryAll = @"
            SELECT Holiday_id, HolidayName, StartDate, EndDate, Description, IsApproved, ApprovedBy, Institute_id
            FROM [dbo].[tbl_Holiday]
            WHERE Institute_id = @Institute_id AND isDelete = 0 AND (@Academic_year_id = 0 OR Academic_year_id = @Academic_year_id)";

                string queryCount = @"
            SELECT COUNT(*)
            FROM [dbo].[tbl_Holiday]
            WHERE Institute_id = @Institute_id AND isDelete = 0 AND (@Academic_year_id = 0 OR Academic_year_id = @Academic_year_id)";

                List<HolidayDTO> holidays;
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

                    using (var multi = await _connection.QueryMultipleAsync(queryPaginated, new { Offset = offset, PageSize = pageSize, Institute_id, Academic_year_id }))
                    {
                        holidays = multi.Read<HolidayDTO>().ToList();
                        totalRecords = multi.ReadSingle<int>();
                    }
                }
                else
                {
                    // No pagination, return all records with sorting
                    string querySorted = $@"
                {queryAll}
                ORDER BY {sortColumn} {sortDirection}";

                    holidays = (await _connection.QueryAsync<HolidayDTO>(querySorted, new { Institute_id, Academic_year_id })).ToList();
                }

                // Fetch class session mappings for each holiday
                foreach (var holiday in holidays)
                {
                    string mappingQuery = @"
                SELECT HolidayClassSessionMapping_id, Holiday_id, tbl_HolidayClassSessionMapping.Class_id, tbl_HolidayClassSessionMapping.Section_id, class_name, section_name
                FROM [dbo].[tbl_HolidayClassSessionMapping]
                INNER JOIN tbl_Class ON tbl_Class.Class_id = tbl_HolidayClassSessionMapping.Class_id
                INNER JOIN tbl_Section ON tbl_Section.Section_id = tbl_HolidayClassSessionMapping.Section_id
                WHERE Holiday_id = @HolidayId;";

                    var mappings = await _connection.QueryAsync<HolidayClassSessionMappingDTO>(mappingQuery, new { HolidayId = holiday.Holiday_id });

                    holiday.ClassSessionMappings = mappings.ToList();
                }

                return new ServiceResponse<List<HolidayDTO>>(true, "Holidays retrieved successfully", holidays, 200, totalRecords);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HolidayDTO>>(false, ex.Message, null, 500);
            }
        }


        public async Task<ServiceResponse<List<HolidayDTO>>> GetApprovedHolidays(int Institute_id, int Academic_year_id, int Status,string sortColumn, string sortDirection, int? pageSize = null, int? pageNumber = null)
        {
            try
            {
                // List of valid sortable columns
                var validSortColumns = new Dictionary<string, string>
{
    { "HolidayName", "HolidayName" },
    { "StartDate", "StartDate" },
    { "EndDate", "EndDate" }
};

                // Ensure the sort column is valid, default to "HolidayName" if not
                if (!validSortColumns.ContainsKey(sortColumn))
                {
                    sortColumn = "HolidayName";
                }
                else
                {
                    sortColumn = validSortColumns[sortColumn];
                }

                // Ensure sort direction is valid, default to "ASC" if not
                sortDirection = sortDirection.ToUpper() == "DESC" ? "DESC" : "ASC";

                // SQL queries
                string queryAll = @"
    SELECT Holiday_id, HolidayName, StartDate, EndDate, Description, IsApproved, ApprovedBy, Institute_id
    FROM [dbo].[tbl_Holiday]
    WHERE IsApproved = 1 AND Status =@Status AND Institute_id = @Institute_id AND isDelete = 0 AND (@Academic_year_id = 0 OR Academic_year_id = @Academic_year_id)";

                string queryCount = @"
    SELECT COUNT(*)
    FROM [dbo].[tbl_Holiday]
    WHERE IsApproved = 1 AND Status =@Status AND  Institute_id = @Institute_id AND isDelete = 0 AND (@Academic_year_id = 0 OR Academic_year_id = @Academic_year_id)";

                List<HolidayDTO> holidays;
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

                    using (var multi = await _connection.QueryMultipleAsync(queryPaginated, new { Offset = offset, PageSize = pageSize, Institute_id, Academic_year_id }))
                    {
                        holidays = multi.Read<HolidayDTO>().ToList();
                        totalRecords = multi.ReadSingle<int>();
                    }
                }
                else
                {
                    // No pagination, return all records with sorting
                    string querySorted = $@"
        {queryAll}
        ORDER BY {sortColumn} {sortDirection}";

                    holidays = (await _connection.QueryAsync<HolidayDTO>(querySorted, new { Institute_id, Academic_year_id })).ToList();
                }

                // Fetch class session mappings for each holiday
                foreach (var holiday in holidays)
                {
                    string mappingQuery = @"
        SELECT HolidayClassSessionMapping_id, Holiday_id, tbl_HolidayClassSessionMapping.Class_id, tbl_HolidayClassSessionMapping.Section_id, class_name, section_name
        FROM [dbo].[tbl_HolidayClassSessionMapping]
        INNER JOIN tbl_Class ON tbl_Class.Class_id = tbl_HolidayClassSessionMapping.Class_id
        INNER JOIN tbl_Section ON tbl_Section.Section_id = tbl_HolidayClassSessionMapping.Section_id
        WHERE Holiday_id = @HolidayId;";

                    var mappings = await _connection.QueryAsync<HolidayClassSessionMappingDTO>(mappingQuery, new { HolidayId = holiday.Holiday_id });

                    holiday.ClassSessionMappings = mappings.ToList();
                }

                return new ServiceResponse<List<HolidayDTO>>(true, "Approved Holidays retrieved successfully", holidays, 200, totalRecords);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HolidayDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteHoliday(int holidayId)
        {
            try
            {
                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    //        string deleteMappingQuery = @"
                    //    DELETE FROM [dbo].[tbl_HolidayClassSessionMapping] WHERE Holiday_id = @HolidayId;
                    //";

                    //        await _connection.ExecuteAsync(deleteMappingQuery, new { HolidayId = holidayId }, transaction);

                    //        string deleteHolidayQuery = @"
                    //    DELETE FROM [dbo].[tbl_Holiday] WHERE Holiday_id = @HolidayId;
                    //";

                    string deleteHolidayQuery = @"
                UPDATE [dbo].[tbl_Holiday] SET isDelete = 1  WHERE Holiday_id = @HolidayId;
            ";

                    int affectedRows = await _connection.ExecuteAsync(deleteHolidayQuery, new { HolidayId = holidayId }, transaction);

                    transaction.Commit();

                    if (affectedRows > 0)
                    {
                        return new ServiceResponse<bool>(true, "Holiday deleted successfully", true, 200);
                    }
                    else
                    {
                        return new ServiceResponse<bool>(false, "Holiday not found", false, 404);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
        public async Task<ServiceResponse<bool>> UpdateHolidayApprovalStatus(int holidayId, int Status, int approvedBy)
        {
            try
            {

                if (!Enum.IsDefined(typeof(Status_Enum), Status))
                {
                    return new ServiceResponse<bool>(false, "Invalid status value", false, 400);
                }
                string query = @"
            UPDATE [dbo].[tbl_Holiday]
            SET Status = @Status,
                ApprovedBy = @ApprovedBy
            WHERE Holiday_id = @HolidayId;
        ";

                int affectedRows = await _connection.ExecuteAsync(query, new { Status = Status, ApprovedBy = approvedBy, HolidayId = holidayId });

                if (affectedRows > 0)
                {
                    return new ServiceResponse<bool>(true, "Holiday approval status updated successfully", true, 200);
                }
                else
                {
                    return new ServiceResponse<bool>(false, "Holiday not found", false, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

    }
}
