using Dapper;
using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Repository.Interfaces;
using System.Data;
using System.Data.Common;

namespace Institute_API.Repository.Implementations
{
    public class HolidayRepository : IHolidayRepository
    {
        private readonly IDbConnection _connection;

        public HolidayRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<ServiceResponse<int>> AddUpdateHoliday(HolidayDTO holidayDTO)
        {
            try
            {
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
                        Description = @Description
                    WHERE Holiday_id = @Holiday_id;
                ";
                    }
                    else
                    {
                        holidayQuery = @"
                    INSERT INTO [dbo].[tbl_Holiday] (HolidayName, StartDate, EndDate, Description)
                    VALUES (@HolidayName, @StartDate, @EndDate, @Description);
                    SELECT SCOPE_IDENTITY();
                ";
                    }

                    // Execute holiday query and retrieve the holiday ID
                    holidayId = await _connection.ExecuteScalarAsync<int>(holidayQuery, holidayDTO, transaction);

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
            SELECT Holiday_id, HolidayName, StartDate, EndDate, Description, IsApproved, ApprovedBy
            FROM [dbo].[tbl_Holiday]
            WHERE Holiday_id = @HolidayId;
        ";

                var holiday = await _connection.QueryFirstOrDefaultAsync<HolidayDTO>(holidayQuery, new { HolidayId = holidayId });

                if (holiday == null)
                {
                    return new ServiceResponse<HolidayDTO>(false, "Holiday not found", null, 404);
                }

                string mappingQuery = @"
            SELECT HolidayClassSessionMapping_id, Holiday_id, Class_id, Section_id
            FROM [dbo].[tbl_HolidayClassSessionMapping]
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

        public async Task<ServiceResponse<List<HolidayDTO>>> GetAllHolidays()
        {
            try
            {
                string query = @"
            SELECT Holiday_id, HolidayName, StartDate, EndDate, Description, IsApproved, ApprovedBy
            FROM [dbo].[tbl_Holiday];
        ";

                var holidays = await _connection.QueryAsync<HolidayDTO>(query);

                foreach (var holiday in holidays)
                {
                    string mappingQuery = @"
                SELECT HolidayClassSessionMapping_id, Holiday_id, Class_id, Section_id
                FROM [dbo].[tbl_HolidayClassSessionMapping]
                WHERE Holiday_id = @HolidayId;
            ";

                    var mappings = await _connection.QueryAsync<HolidayClassSessionMappingDTO>(mappingQuery, new { HolidayId = holiday.Holiday_id });

                    holiday.ClassSessionMappings = mappings.ToList();
                }

                return new ServiceResponse<List<HolidayDTO>>(true, "Holidays retrieved successfully", holidays.ToList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HolidayDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<HolidayDTO>>> GetApprovedHolidays()
        {
            try
            {
                string query = @"
            SELECT Holiday_id, HolidayName, StartDate, EndDate, Description, IsApproved, ApprovedBy
            FROM [dbo].[tbl_Holiday]
            WHERE IsApproved = 1;
        ";

                var holidays = await _connection.QueryAsync<HolidayDTO>(query);

                foreach (var holiday in holidays)
                {
                    string mappingQuery = @"
                SELECT HolidayClassSessionMapping_id, Holiday_id, Class_id, Section_id
                FROM [dbo].[tbl_HolidayClassSessionMapping]
                WHERE Holiday_id = @HolidayId;
            ";

                    var mappings = await _connection.QueryAsync<HolidayClassSessionMappingDTO>(mappingQuery, new { HolidayId = holiday.Holiday_id });

                    holiday.ClassSessionMappings = mappings.ToList();
                }

                return new ServiceResponse<List<HolidayDTO>>(true, "Approved holidays retrieved successfully", holidays.ToList(), 200);
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
                    string deleteMappingQuery = @"
                DELETE FROM [dbo].[tbl_HolidayClassSessionMapping] WHERE Holiday_id = @HolidayId;
            ";

                    await _connection.ExecuteAsync(deleteMappingQuery, new { HolidayId = holidayId }, transaction);

                    string deleteHolidayQuery = @"
                DELETE FROM [dbo].[tbl_Holiday] WHERE Holiday_id = @HolidayId;
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
        public async Task<ServiceResponse<bool>> UpdateHolidayApprovalStatus(int holidayId, bool isApproved, int approvedBy)
        {
            try
            {
                string query = @"
            UPDATE [dbo].[tbl_Holiday]
            SET IsApproved = @IsApproved,
                ApprovedBy = @ApprovedBy
            WHERE Holiday_id = @HolidayId;
        ";

                int affectedRows = await _connection.ExecuteAsync(query, new { IsApproved = isApproved, ApprovedBy = approvedBy, HolidayId = holidayId });

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
