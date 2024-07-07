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

        public async Task<ServiceResponse<string>> AddUpdateSource(Source source)
        {
            try
            {
                if (source.SourceID == 0)
                {
                    // Insert new source
                    string query = @"INSERT INTO tblSources (Source, Description) VALUES (@SourceName, @Description)";
                    int insertedValue = await _dbConnection.ExecuteAsync(query, new { source.SourceName, source.Description });
                    if (insertedValue > 0)
                    {
                        return new ServiceResponse<string>(true, "Source Added Successfully", "Success", 201);
                    }
                    return new ServiceResponse<string>(false, "Failed to Add Source", "Failure", 400);
                }
                else
                {
                    // Update existing source
                    string query = @"UPDATE tblSources SET SourceName = @Source, Description = @Description WHERE SourceID = @SourceID";
                    int rowsAffected = await _dbConnection.ExecuteAsync(query, new { source.SourceName, source.Description, source.SourceID });
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

        public async Task<ServiceResponse<IEnumerable<Source>>> GetAllSources(GetAllSourcesRequest request)
        {
            try
            {
                string query = "SELECT * FROM tblSources";
                var sources = await _dbConnection.QueryAsync<Source>(query);
                var paginatedSources = sources.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
                return new ServiceResponse<IEnumerable<Source>>(true, "Sources Retrieved Successfully", paginatedSources, 200, sources.Count());
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<Source>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<Source>> GetSourceById(int sourceId)
        {
            try
            {
                string query = "SELECT * FROM tblSources WHERE SourceID = @SourceID";
                var source = await _dbConnection.QueryFirstOrDefaultAsync<Source>(query, new { SourceID = sourceId });
                if (source != null)
                {
                    return new ServiceResponse<Source>(true, "Source Retrieved Successfully", source, 200);
                }
                return new ServiceResponse<Source>(false, "Source Not Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Source>(false, ex.Message, null, 500);
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
