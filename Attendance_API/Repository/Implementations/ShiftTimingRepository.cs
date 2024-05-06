using Attendance_API.DTOs;
using Attendance_API.Models;
using Attendance_API.Repository.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Attendance_API.Repository.Implementations
{
    public class ShiftTimingRepository : IShiftTimingRepository
    {
        private readonly IDbConnection _connection;

        public ShiftTimingRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<bool> AddShiftTimingAndDesignations(ShiftTimingRequestDTO request)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    string insertShiftTimingSql = @"
                        INSERT INTO tbl_ShiftTimingMaster (Clock_In, Clock_Out, Late_Coming, Applicable_Date)
                        VALUES (@Clock_In, @Clock_Out, @Late_Coming, @Applicable_Date);
                        SELECT CAST(SCOPE_IDENTITY() as int);";

                    int shiftTimingId = await _connection.QuerySingleAsync<int>(insertShiftTimingSql, new
                    {
                        Clock_In = request.Clock_In,
                        Clock_Out = request.Clock_Out,
                        Late_Coming = request.Late_Coming,
                        Applicable_Date = request.Applicable_Date
                    }, transaction);

                    if (shiftTimingId > 0)
                    {
                        string insertDesignationMappingSql = @"
                            INSERT INTO tbl_ShiftTimingDesignationMapping (Shift_Timing_id, Designation_id)
                            VALUES (@Shift_Timing_id, @Designation_id);";

                        foreach (var designationId in request.Designations)
                        {
                            await _connection.ExecuteAsync(insertDesignationMappingSql, new
                            {
                                Shift_Timing_id = shiftTimingId,
                                Designation_id = designationId
                            }, transaction);
                        }

                        transaction.Commit();
                        _connection.Close();
                        return true;
                    }
                    else
                    {
                        transaction.Rollback();
                        _connection.Close();
                        return false;
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    _connection.Close();
                    throw;
                }
            }
        }

        public async Task<ShiftTimingResponseDTO> GetShiftTimingById(int id)
        {
            var query = @"
                SELECT st.Shift_Timing_id, st.Clock_In, st.Clock_Out, st.Late_Coming, st.Applicable_Date, d.Designation_id, d.DesignationName
                FROM tbl_ShiftTimingMaster st
                JOIN tbl_ShiftTimingDesignationMapping stm ON st.Shift_Timing_id = stm.Shift_Timing_id
                JOIN tbl_Designation d ON stm.Designation_id = d.Designation_id
                WHERE st.Shift_Timing_id = @Id;";

            var shiftTimingDictionary = new Dictionary<int, ShiftTimingResponseDTO>();

            var result = await _connection.QueryAsync<ShiftTimingResponseDTO, ShiftTimingDesignations, ShiftTimingResponseDTO>(
                query,
                (shiftTiming, designation) =>
                {
                    if (!shiftTimingDictionary.TryGetValue(shiftTiming.Shift_Timing_id, out var shiftTimingEntry))
                    {
                        shiftTimingEntry = shiftTiming;
                        shiftTimingEntry.Designations = new List<ShiftTimingDesignations>();
                        shiftTimingDictionary.Add(shiftTimingEntry.Shift_Timing_id, shiftTimingEntry);
                    }

                    shiftTimingEntry.Designations.Add(designation);
                    return shiftTimingEntry;
                },
                new { Id = id },
                splitOn: "Designation_id"
            );

            return shiftTimingDictionary.Values.FirstOrDefault();
        }

        public async Task<bool> EditShiftTimingAndDesignations(ShiftTimingRequestDTO request)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    string updateShiftTimingSql = @"
                        UPDATE tbl_ShiftTimingMaster
                        SET Clock_In = @Clock_In, Clock_Out = @Clock_Out, Late_Coming = @Late_Coming, Applicable_Date = @Applicable_Date
                        WHERE Shift_Timing_id = @Shift_Timing_id;";

                    int rowsAffected = await _connection.ExecuteAsync(updateShiftTimingSql, new
                    {
                        Shift_Timing_id = request.Shift_Timing_id,
                        Clock_In = request.Clock_In,
                        Clock_Out = request.Clock_Out,
                        Late_Coming = request.Late_Coming,
                        Applicable_Date = request.Applicable_Date
                    }, transaction);

                    if (rowsAffected > 0)
                    {
                        string deleteDesignationMappingsSql = "DELETE FROM tbl_ShiftTimingDesignationMapping WHERE Shift_Timing_id = @Shift_Timing_id;";
                        await _connection.ExecuteAsync(deleteDesignationMappingsSql, new { Shift_Timing_id = request.Shift_Timing_id }, transaction);

                        string insertDesignationMappingSql = @"
                            INSERT INTO tbl_ShiftTimingDesignationMapping (Shift_Timing_id, Designation_id)
                            VALUES (@Shift_Timing_id, @Designation_id);";

                        foreach (var designationId in request.Designations)
                        {
                            await _connection.ExecuteAsync(insertDesignationMappingSql, new
                            {
                                Shift_Timing_id = request.Shift_Timing_id,
                                Designation_id = designationId
                            }, transaction);
                        }

                        transaction.Commit();
                        _connection.Close();
                        return true;
                    }
                    else
                    {
                        transaction.Rollback();
                        _connection.Close();
                        return false;
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    _connection.Close();
                    throw;
                }
            }
        }

        public async Task<bool> DeleteShiftTiming(int id)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    string deleteDesignationMappingsSql = "DELETE FROM tbl_ShiftTimingDesignationMapping WHERE Shift_Timing_id = @Id;";
                    await _connection.ExecuteAsync(deleteDesignationMappingsSql, new { Id = id }, transaction);

                    string deleteShiftTimingSql = "DELETE FROM tbl_ShiftTimingMaster WHERE Shift_Timing_id = @Id;";
                    int rowsAffected = await _connection.ExecuteAsync(deleteShiftTimingSql, new { Id = id }, transaction);

                    if (rowsAffected > 0)
                    {
                        transaction.Commit();
                        _connection.Close();
                        return true;
                    }
                    else
                    {
                        transaction.Rollback();
                        _connection.Close();
                        return false;
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    _connection.Close();
                    throw;
                }
            }
        }

        public async Task<List<ShiftTimingResponseDTO>> GetAllShiftTimings()
        {
            var query = @"
                SELECT st.Shift_Timing_id, st.Clock_In, st.Clock_Out, st.Late_Coming, st.Applicable_Date, d.Designation_id, d.DesignationName
                FROM tbl_ShiftTimingMaster st
                JOIN tbl_ShiftTimingDesignationMapping stm ON st.Shift_Timing_id = stm.Shift_Timing_id
                JOIN tbl_Designation d ON stm.Designation_id = d.Designation_id;";

            var shiftTimingDictionary = new Dictionary<int, ShiftTimingResponseDTO>();

            var result = await _connection.QueryAsync<ShiftTimingResponseDTO, ShiftTimingDesignations, ShiftTimingResponseDTO>(
                query,
                (shiftTiming, designation) =>
                {
                    if (!shiftTimingDictionary.TryGetValue(shiftTiming.Shift_Timing_id, out var shiftTimingEntry))
                    {
                        shiftTimingEntry = shiftTiming;
                        shiftTimingEntry.Designations = new List<ShiftTimingDesignations>();
                        shiftTimingDictionary.Add(shiftTimingEntry.Shift_Timing_id, shiftTimingEntry);
                    }

                    shiftTimingEntry.Designations.Add(designation);
                    return shiftTimingEntry;
                },
                splitOn: "Designation_id"
            );

            return shiftTimingDictionary.Values.ToList();
        }
    }
}
