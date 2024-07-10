using Dapper;
using System.Data;
using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Repository.Interfaces;

namespace Transport_API.Repository.Implementations
{
    public class RouteFeesRepository : IRouteFeesRepository
    {
        private readonly IDbConnection _connection;

        public RouteFeesRepository(IDbConnection dbConnection)
        {
            _connection = dbConnection;
        }
        public async Task<ServiceResponse<string>> AddUpdateRouteFeeStructure(RouteFeeStructure request)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Insert or update tblRouteFeeStructure
                    int routeFeeStructureId;
                    if (request.RouteFeeStructureId == 0)
                    {
                        string insertSql = @"INSERT INTO tblRouteFeeStructure (StopId, FrequencyId)
                                     VALUES (@StopId, @FrequencyId);
                                     SELECT SCOPE_IDENTITY();";
                        routeFeeStructureId = await _connection.ExecuteScalarAsync<int>(insertSql, new
                        {
                            StopId = request.StopId,
                            FrequencyId = request.FrequencyId
                        }, transaction);
                    }
                    else
                    {
                        string updateSql = @"UPDATE tblRouteFeeStructure
                                     SET StopId = @StopId, FrequencyId = @FrequencyId
                                     WHERE RouteFeeStructureId = @RouteFeeStructureId";
                        await _connection.ExecuteAsync(updateSql, new
                        {
                            StopId = request.StopId,
                            FrequencyId = request.FrequencyId,
                            RouteFeeStructureId = request.RouteFeeStructureId
                        }, transaction);
                        routeFeeStructureId = request.RouteFeeStructureId;
                    }

                    // Delete existing frequencies if updating
                    if (request.RouteFeeStructureId != 0)
                    {
                        await _connection.ExecuteAsync("DELETE FROM tblTransportFeeFrequency WHERE RouteFeeStructureId = @RouteFeeStructureId", new { RouteFeeStructureId = routeFeeStructureId }, transaction);
                        await _connection.ExecuteAsync("DELETE FROM tblTransportFeeFrequencyTerm WHERE RouteFeeStructureId = @RouteFeeStructureId", new { RouteFeeStructureId = routeFeeStructureId }, transaction);
                    }

                    // Insert into tblTransportFeeFrequency or tblTransportFeeFrequencyTerm based on FrequencyId
                    if (request.FrequencyId == 1 || request.FrequencyId == 3)
                    {
                        await InsertTransportFeeFrequency(request.RouteFeeFrequencies ??= ([]), routeFeeStructureId, transaction);
                    }
                    else if (request.FrequencyId == 2)
                    {
                        await InsertTransportFeeFrequencyTerm(request.RouteTermFeeFrequencies ??= ([]), routeFeeStructureId, transaction);
                    }

                    transaction.Commit();
                    return new ServiceResponse<string>(true, "Operation successful", "Data added or updated successfully", 200);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
                }
            }
        }
        public async Task<ServiceResponse<RouteFeeStructure>> GetRouteFeeStructureById(int routeFeeStructureId)
        {
            try
            {
                string sql = @"SELECT * FROM tblRouteFeeStructure WHERE RouteFeeStructureId = @RouteFeeStructureId";
                var routeFeeStructure = await _connection.QueryFirstOrDefaultAsync<RouteFeeStructure>(sql, new { RouteFeeStructureId = routeFeeStructureId });

                if (routeFeeStructure != null)
                {
                    if (routeFeeStructure.FrequencyId == 1 || routeFeeStructure.FrequencyId == 3)
                    {
                        routeFeeStructure.RouteFeeFrequencies = await FetchRouteFeeFrequencies(routeFeeStructureId);
                    }
                    else if (routeFeeStructure.FrequencyId == 2)
                    {
                        routeFeeStructure.RouteTermFeeFrequencies = await FetchRouteTermFeeFrequencies(routeFeeStructureId);
                    }

                    return new ServiceResponse<RouteFeeStructure>(true, "Record found", routeFeeStructure, 200);
                }
                else
                {
                    return new ServiceResponse<RouteFeeStructure>(false, "Record not found", new RouteFeeStructure(), 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<RouteFeeStructure>(false, ex.Message, new RouteFeeStructure(), 500);
            }
        }
        private async Task<List<RouteFeeFrequency>> FetchRouteFeeFrequencies(int routeFeeStructureId)
        {
            string feeFrequencySql = @"SELECT * FROM tblTransportFeeFrequency WHERE RouteFeeStructureId = @RouteFeeStructureId";
            var feeFrequencies = await _connection.QueryAsync<RouteFeeFrequency>(feeFrequencySql, new { RouteFeeStructureId = routeFeeStructureId });
            return feeFrequencies.ToList();
        }
        private async Task<List<RouteTermFeeFrequency>> FetchRouteTermFeeFrequencies(int routeFeeStructureId)
        {
            string termFrequencySql = @"SELECT * FROM tblTransportFeeFrequencyTerm WHERE RouteFeeStructureId = @RouteFeeStructureId";
            var termFrequencies = await _connection.QueryAsync<RouteTermFeeFrequency>(termFrequencySql, new { RouteFeeStructureId = routeFeeStructureId });
            return termFrequencies.ToList();
        }
        private async Task InsertTransportFeeFrequency(IEnumerable<RouteFeeFrequency> frequencies, int routeFeeStructureId, IDbTransaction transaction)
        {
            string insertFeeSql = @"INSERT INTO tblTransportFeeFrequency (RouteFeeStructureId, FrequencyId, Fees, MonthId, DueDate)
                            VALUES (@RouteFeeStructureId, @FrequencyId, @Fees, @MonthId, @DueDate)";
            foreach (var frequency in frequencies)
            {
                await _connection.ExecuteAsync(insertFeeSql, new
                {
                    RouteFeeStructureId = routeFeeStructureId,
                    FrequencyId = frequency.FrequencyId,
                    Fees = frequency.Fees,
                    MonthId = frequency.MonthId,
                    DueDate = frequency.DueDate
                }, transaction);
            }
        }
        private async Task InsertTransportFeeFrequencyTerm(IEnumerable<RouteTermFeeFrequency> termFrequencies, int routeFeeStructureId, IDbTransaction transaction)
        {
            string insertTermFeeSql = @"INSERT INTO tblTransportFeeFrequencyTerm (RouteFeeStructureId, FrequencyId, Fees, DueDate, TermName)
                                VALUES (@RouteFeeStructureId, @FrequencyId, @Fees, @DueDate, @TermName)";
            foreach (var termFrequency in termFrequencies)
            {
                await _connection.ExecuteAsync(insertTermFeeSql, new
                {
                    RouteFeeStructureId = routeFeeStructureId,
                    FrequencyId = termFrequency.FrequencyId,
                    Fees = termFrequency.Fees,
                    DueDate = termFrequency.DueDate,
                    TermName = termFrequency.TermName
                }, transaction);
            }
        }
    }
}
