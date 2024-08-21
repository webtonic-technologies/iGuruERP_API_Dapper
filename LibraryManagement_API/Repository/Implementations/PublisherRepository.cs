using Dapper;
using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Models;
using LibraryManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LibraryManagement_API.Repository.Implementations
{
    public class PublisherRepository : IPublisherRepository
    {
        private readonly IDbConnection _connection;

        public PublisherRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<List<PublisherResponse>>> GetAllPublishers(GetAllPublishersRequest request)
        {
            try
            {
                string countSql = "SELECT COUNT(*) FROM tblPublisher WHERE InstituteID = @InstituteID AND IsActive = 1";
                int totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

                string sql = @"SELECT p.PublisherID, p.InstituteID, p.PublisherName, p.MobileNumber, c.CountryName, p.IsActive 
                               FROM tblPublisher p
                               LEFT JOIN tblCountry c ON p.CountryID = c.CountryID
                               WHERE p.InstituteID = @InstituteID AND p.IsActive = 1";

                var publishers = await _connection.QueryAsync<PublisherResponse>(sql, new { request.InstituteID });

                var paginatedList = publishers.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

                return new ServiceResponse<List<PublisherResponse>>(true, "Records Found", paginatedList, 200, totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<PublisherResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<PublisherFetchResponse>>> GetAllPublishersFetch(GetAllPublishersFetchRequest request)
        {
            try
            {
                string sql = @"SELECT PublisherID, InstituteID, PublisherName, MobileNumber, CountryID, IsActive 
                               FROM tblPublisher 
                               WHERE InstituteID = @InstituteID AND IsActive = 1";

                var publishers = await _connection.QueryAsync<PublisherFetchResponse>(sql, new { request.InstituteID });

                return new ServiceResponse<List<PublisherFetchResponse>>(true, "Records Found", publishers.AsList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<PublisherFetchResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<Publisher>> GetPublisherById(int publisherId)
        {
            try
            {
                string sql = @"SELECT p.PublisherID, p.InstituteID, p.PublisherName, p.MobileNumber, p.CountryID, p.IsActive, c.CountryName
                               FROM tblPublisher p
                               LEFT JOIN tblCountry c ON p.CountryID = c.CountryID
                               WHERE p.PublisherID = @PublisherID AND p.IsActive = 1";
                var publisher = await _connection.QueryFirstOrDefaultAsync<Publisher>(sql, new { PublisherID = publisherId });

                return publisher != null ?
                    new ServiceResponse<Publisher>(true, "Record Found", publisher, 200) :
                    new ServiceResponse<Publisher>(false, "Record Not Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Publisher>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<string>> AddUpdatePublisher(Publisher request)
        {
            try
            {
                string sql = request.PublisherID == 0 ?
                    @"INSERT INTO tblPublisher (InstituteID, PublisherName, MobileNumber, CountryID, IsActive) 
                      VALUES (@InstituteID, @PublisherName, @MobileNumber, @CountryID, @IsActive)" :
                    @"UPDATE tblPublisher SET InstituteID = @InstituteID, PublisherName = @PublisherName, 
                      MobileNumber = @MobileNumber, CountryID = @CountryID, IsActive = @IsActive 
                      WHERE PublisherID = @PublisherID";

                int rowsAffected = await _connection.ExecuteAsync(sql, new
                {
                    request.InstituteID,
                    request.PublisherName,
                    request.MobileNumber,
                    request.CountryID,
                    request.IsActive,
                    request.PublisherID
                });

                return new ServiceResponse<string>(rowsAffected > 0, rowsAffected > 0 ? "Success" : "Failure",
                    rowsAffected > 0 ? "Publisher saved successfully" : "Failed to save publisher", rowsAffected > 0 ? 200 : 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeletePublisher(int publisherId)
        {
            try
            {
                string sql = "UPDATE tblPublisher SET IsActive = 0 WHERE PublisherID = @PublisherID";
                int rowsAffected = await _connection.ExecuteAsync(sql, new { PublisherID = publisherId });

                return new ServiceResponse<bool>(rowsAffected > 0, rowsAffected > 0 ? "Deleted Successfully" : "Delete Failed",
                    rowsAffected > 0, rowsAffected > 0 ? 200 : 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
