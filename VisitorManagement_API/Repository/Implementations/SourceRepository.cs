using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.Models;
using VisitorManagement_API.Repository.Interfaces;
using System.Data.SqlClient;

namespace VisitorManagement_API.Repository.Implementations
{
    public class SourceRepository : ISourceRepository
    {
        private readonly IDbConnection _dbConnection;

        public SourceRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateSource(Sources source)
        {
            try
            {
                if (source.SourceID == 0)
                {
                    // Insert new source
                    string query = @"INSERT INTO tblSources (Source, Description, Status) VALUES (@Source, @Description, @Status)";
                    source.Status = true;
                    int insertedValue = await _dbConnection.ExecuteAsync(query, new { source.Source, source.Description, source.Status });
                    if (insertedValue > 0)
                    {
                        return new ServiceResponse<string>(true, "Source Added Successfully", "Success", 201);
                    }
                    return new ServiceResponse<string>(false, "Failed to Add Source", "Failure", 400);
                }
                else
                {
                    // Update existing source
                    string query = @"UPDATE tblSources SET Source = @Source, Description = @Description, Status = @Status WHERE SourceID = @SourceID";
                    int rowsAffected = await _dbConnection.ExecuteAsync(query, new { source.Source, source.Description, source.SourceID });
                    if (rowsAffected > 0)
                    {
                        return new ServiceResponse<string>(true, "Source Updated Successfully", "Success", 200);
                    }
                    return new ServiceResponse<string>(false, "Failed to Update Source", "Failure", 400);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<Sources>>> GetAllSources(GetAllSourcesRequest request)
        {
            try
            {
                string query = "SELECT * FROM tblSources";
                var sources = await _dbConnection.QueryAsync<Sources>(query);
                var paginatedSources = sources.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
                return new ServiceResponse<IEnumerable<Sources>>(true, "Sources Retrieved Successfully", paginatedSources, 200, sources.Count());
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<Sources>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<Sources>> GetSourceById(int sourceId)
        {
            try
            {
                string query = "SELECT * FROM tblSources WHERE SourceID = @SourceID";
                var source = await _dbConnection.QueryFirstOrDefaultAsync<Sources>(query, new { SourceID = sourceId });
                if (source != null)
                {
                    return new ServiceResponse<Sources>(true, "Source Retrieved Successfully", source, 200);
                }
                return new ServiceResponse<Sources>(false, "Source Not Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Sources>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateSourceStatus(int sourceId)
        {
            try
            {
                // Assuming there is a Status column in tblSources table
                string query = "UPDATE tblSources SET Status = CASE WHEN Status = 1 THEN 0 ELSE 1 END WHERE SourceID = @SourceID";
                int rowsAffected = await _dbConnection.ExecuteAsync(query, new { SourceID = sourceId });
                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "Source Status Updated Successfully", true, 200);
                }
                return new ServiceResponse<bool>(false, "Failed to Update Source Status", false, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
