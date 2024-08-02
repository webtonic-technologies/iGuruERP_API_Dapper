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
    public class IssueRepository : IIssueRepository
    {
        private readonly IConfiguration _configuration;

        public IssueRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateIssue(AddUpdateIssueRequest request)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string sql;

                        if (request.IssueID == 0)
                        {
                            sql = @"INSERT INTO tblIssuedBook (InstituteID, CatalogueID, BorrowerID, BorrowerTypeID, GuestName, GuestType, Address, Contact, IssueDate, DueDate) 
                                    VALUES (@InstituteID, @CatalogueID, @BorrowerID, @BorrowerTypeID, @GuestName, @GuestType, @Address, @Contact, @IssueDate, @DueDate)";

                            await connection.ExecuteAsync(sql, new
                            {
                                request.InstituteID,
                                request.CatalogueID,
                                request.BorrowerID,
                                request.BorrowerTypeID,
                                request.GuestName,
                                request.GuestType,
                                request.Address,
                                request.Contact,
                                request.IssueDate,
                                request.DueDate
                            }, transaction);
                        }
                        else
                        {
                            sql = @"UPDATE tblIssuedBook 
                                    SET InstituteID = @InstituteID, CatalogueID = @CatalogueID, BorrowerID = @BorrowerID, 
                                        BorrowerTypeID = @BorrowerTypeID, GuestName = @GuestName, GuestType = @GuestType, 
                                        Address = @Address, Contact = @Contact, IssueDate = @IssueDate, DueDate = @DueDate
                                    WHERE IssueBookID = @IssueID";

                            await connection.ExecuteAsync(sql, new
                            {
                                request.InstituteID,
                                request.CatalogueID,
                                request.BorrowerID,
                                request.BorrowerTypeID,
                                request.GuestName,
                                request.GuestType,
                                request.Address,
                                request.Contact,
                                request.IssueDate,
                                request.DueDate,
                                request.IssueID
                            }, transaction);
                        }

                        transaction.Commit();
                        return new ServiceResponse<string>(true, "Success", "Issue saved successfully", 200);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new ServiceResponse<string>(false, ex.Message, null, 500);
                    }
                }
            }
        }

        public async Task<ServiceResponse<List<IssueBookResponse>>> GetAllIssueBooks(GetAllIssueBooksRequest request)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                try
                {
                    string countSql = @"
                        SELECT COUNT(*) 
                        FROM tblIssuedBook 
                        WHERE InstituteID = @InstituteID";
                    int totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

                    string sql = @"
                        SELECT 
                            c.CatalogueID AS BookID,
                            l.LibraryName AS Library,
                            c.BookName,
                            c.ISBNNo10 AS ISBN10,
                            c.ISBNNo13 AS ISBN13,
                            c.AccessionNumber,
                            a.AuthorName AS Author,
                            p.PublisherName AS Publisher
                        FROM tblIssuedBook i
                        INNER JOIN tblCatalogue c ON i.CatalogueID = c.CatalogueID
                        INNER JOIN tblLibrary l ON c.LibraryID = l.LibraryID
                        LEFT JOIN tblCatalogueAuthorMapping cam ON c.CatalogueID = cam.CatalogueID
                        LEFT JOIN tblAuthor a ON cam.AuthorID = a.AuthorID
                        LEFT JOIN tblPublisher p ON c.PublisherID = p.PublisherID
                        WHERE i.InstituteID = @InstituteID";

                    var issues = await connection.QueryAsync<IssueBookResponse>(sql, new { request.InstituteID });

                    var paginatedList = issues.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

                    return new ServiceResponse<List<IssueBookResponse>>(true, "Records Found", paginatedList, 200, totalCount);
                }
                catch (Exception ex)
                {
                    return new ServiceResponse<List<IssueBookResponse>>(false, ex.Message, null, 500);
                }
            }
        }
    }
}
