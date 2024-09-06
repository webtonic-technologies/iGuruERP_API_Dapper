using Attendance_API.DTOs;
using Attendance_API.Models;
using Attendance_API.Repository.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Attendance_API.DTOs.ServiceResponse;


namespace Attendance_API.Repository.Implementations
{
    public class ShiftTimingRepository : IShiftTimingRepository
    {
        private readonly IDbConnection _connection;

        public ShiftTimingRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<string>> AddOrEditShiftTimingsAndDesignations(List<ShiftTimingRequestDTO> requests)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    foreach (var request in requests)
                    {
                        if (request.Shift_Timing_id > 0)
                        {
                            // Edit existing shift timing
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
                            }
                        }
                        else
                        {
                            // Add new shift timing
                            string insertShiftTimingSql = @"
                                INSERT INTO tbl_ShiftTimingMaster (Clock_In, Clock_Out, Late_Coming, Applicable_Date, InstituteId)
                                VALUES (@Clock_In, @Clock_Out, @Late_Coming, @Applicable_Date, @InstituteId);
                                SELECT CAST(SCOPE_IDENTITY() as int);";

                            int shiftTimingId = await _connection.QuerySingleAsync<int>(insertShiftTimingSql, new
                            {
                                Clock_In = request.Clock_In,
                                Clock_Out = request.Clock_Out,
                                Late_Coming = request.Late_Coming,
                                Applicable_Date = request.Applicable_Date,
                                InstituteId = request.InstituteId
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
                            }
                        }
                    }

                    transaction.Commit();
                    _connection.Close();
                    return new ServiceResponse<string>(true, "Operation successful", "Data processed successfully", 200);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _connection.Close();
                    return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
                }
            }
        }

        public async Task<ServiceResponse<ShiftTimingResponse>> GetShiftTimingById(int id)
        {
            try
            {
                var query = @"
                    SELECT st.Shift_Timing_id, st.Clock_In, st.Clock_Out, st.Late_Coming, st.Applicable_Date, d.Designation_id, d.DesignationName
                    FROM tbl_ShiftTimingMaster st
                    JOIN tbl_ShiftTimingDesignationMapping stm ON st.Shift_Timing_id = stm.Shift_Timing_id
                    JOIN tbl_Designation d ON stm.Designation_id = d.Designation_id
                    WHERE st.Shift_Timing_id = @Id;";

                var shiftTimingDictionary = new Dictionary<int, ShiftTimingResponse>();

                var result = await _connection.QueryAsync<ShiftTimingResponse, ShiftTimingDesignations, ShiftTimingResponse>(
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

                var shiftTiming = shiftTimingDictionary.Values.FirstOrDefault();
                if (shiftTiming != null)
                {
                    return new ServiceResponse<ShiftTimingResponse>(true, "Record found", shiftTiming, 200);
                }
                else
                {
                    return new ServiceResponse<ShiftTimingResponse>(false, "Record not found", null, 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ShiftTimingResponse>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<string>> DeleteShiftTiming(int id)
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
                        return new ServiceResponse<string>(true, "Operation successful", "Data deleted successfully", 200);
                    }
                    else
                    {
                        transaction.Rollback();
                        _connection.Close();
                        return new ServiceResponse<string>(false, "Operation failed", string.Empty, 500);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _connection.Close();
                    return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
                }
            }
        }

        public async Task<ServiceResponse<ShiftTimingResponseDTO>> GetAllShiftTimings(ShiftTimingFilterDTO request)
        {
            try
            {
                var query = @"
                    SELECT st.Shift_Timing_id, st.Clock_In, st.Clock_Out, st.Late_Coming, st.Applicable_Date, d.Designation_id, d.DesignationName
                    FROM tbl_ShiftTimingMaster st
                   LEFT JOIN tbl_ShiftTimingDesignationMapping stm ON st.Shift_Timing_id = stm.Shift_Timing_id
                    LEFT JOIN tbl_Designation d ON stm.Designation_id = d.Designation_id";

                if (request.pageNumber != null && request.pageSize != null)
                {
                    query += $" Order by 1 OFFSET {(request.pageNumber - 1) * request.pageSize} ROWS FETCH NEXT {request.pageSize} ROWS ONLY;";
                }

                var shiftTimingDictionary = new Dictionary<int, ShiftTimingResponse>();

                var result = await _connection.QueryAsync<ShiftTimingResponse, ShiftTimingDesignations, ShiftTimingResponse>(
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

                query = @"
                    SELECT COUNT(*)
                    FROM tbl_ShiftTimingMaster st
                    LEFT JOIN tbl_ShiftTimingDesignationMapping stm ON st.Shift_Timing_id = stm.Shift_Timing_id
                     LEFT JOIN tbl_Designation d ON stm.Designation_id = d.Designation_id;";

                var countRes = await _connection.QueryAsync<long>(query);
                var count = countRes.FirstOrDefault();
                return new ServiceResponse<ShiftTimingResponseDTO>(true, "Operation successful", new ShiftTimingResponseDTO { Data = shiftTimingDictionary.Values.ToList(), Total = count }, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ShiftTimingResponseDTO>(false, ex.Message, null, 500);
            }
        }
    }
}
