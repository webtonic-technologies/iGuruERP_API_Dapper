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
using System.Data.Common;
using System.Globalization;

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
                        // Convert string dates (DD-MM-YYYY) to DateTime
                        DateTime issueDate = DateTime.ParseExact(request.IssueDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        DateTime dueDate = DateTime.ParseExact(request.DueDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

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
                                IssueDate = issueDate,  // Use the converted DateTime
                                DueDate = dueDate       // Use the converted DateTime
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
                                IssueDate = issueDate,  // Use the converted DateTime
                                DueDate = dueDate,      // Use the converted DateTime
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
                FROM tblIssuedBook i
                INNER JOIN tblCatalogue c ON i.CatalogueID = c.CatalogueID
                INNER JOIN tblLibrary l ON c.LibraryID = l.LibraryID 
                LEFT JOIN tblPublisher p ON c.PublisherID = p.PublisherID
                INNER JOIN tblBorrowerTypeMaster bt ON i.BorrowerTypeID = bt.BorrowerTypeID
                WHERE i.InstituteID = @InstituteID
                AND i.IssueDate BETWEEN @StartDate AND @EndDate";

                    int totalCount = await connection.ExecuteScalarAsync<int>(countSql, new
                    {
                        request.InstituteID,
                        StartDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                        EndDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)
                    });

                    string sql = @"
                SELECT 
                    i.IssueBookID,
                    FORMAT(i.IssueDate, 'dd-MM-yyyy') AS IssuedOn,  -- Format IssueDate to DD-MM-YYYY
                    bt.BorrowerType,
                    c.CatalogueID AS BookID,
                    c.BookName,
                    c.ISBNNo10 AS ISBN10,
                    c.ISBNNo13 AS ISBN13,
                    c.AccessionNumber,
                    p.PublisherName AS Publisher,
                    l.LibraryName,
                    a.AuthorID,
                    a.AuthorName
                FROM tblIssuedBook i
                INNER JOIN tblCatalogue c ON i.CatalogueID = c.CatalogueID
                INNER JOIN tblLibrary l ON c.LibraryID = l.LibraryID
                LEFT JOIN tblCatalogueAuthorMapping cam ON c.CatalogueID = cam.CatalogueID
                LEFT JOIN tblAuthor a ON cam.AuthorID = a.AuthorID
                LEFT JOIN tblPublisher p ON c.PublisherID = p.PublisherID
                INNER JOIN tblBorrowerTypeMaster bt ON i.BorrowerTypeID = bt.BorrowerTypeID
                WHERE i.InstituteID = @InstituteID
                AND i.IssueDate BETWEEN @StartDate AND @EndDate";

                    var issueBooks = await connection.QueryAsync<IssueBookResponse, AuthorResponse2, IssueBookResponse>(
                        sql,
                        (issueBook, author) =>
                        {
                            var issueBookWithAuthors = issueBook;

                            if (author != null)
                            {
                                if (issueBookWithAuthors.Authors == null)
                                {
                                    issueBookWithAuthors.Authors = new List<AuthorResponse2>();
                                }
                                issueBookWithAuthors.Authors.Add(author);
                            }

                            return issueBookWithAuthors;
                        },
                        new
                        {
                            request.InstituteID,
                            StartDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                            EndDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)
                        },
                        splitOn: "AuthorID");

                    // Grouping books by IssueBookID
                    var groupedBooks = issueBooks
                        .GroupBy(b => b.IssueBookID)
                        .Select(group =>
                        {
                            var firstItem = group.First();
                            firstItem.Authors = group.Select(g => new AuthorResponse2
                            {
                                AuthorID = g.Authors.First().AuthorID,
                                AuthorName = g.Authors.First().AuthorName
                            }).ToList();

                            return firstItem;
                        })
                        .ToList();

                    var paginatedList = groupedBooks.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

                    return new ServiceResponse<List<IssueBookResponse>>(true, "Records Found", paginatedList, 200, totalCount);
                }
                catch (Exception ex)
                {
                    return new ServiceResponse<List<IssueBookResponse>>(false, ex.Message, null, 500);
                }
            }
        }


        //public async Task<ServiceResponse<List<IssueBookResponse>>> GetAllIssueBooks(GetAllIssueBooksRequest request)
        //{
        //    using (var connection = CreateConnection())
        //    {
        //        connection.Open();
        //        try
        //        {
        //            string countSql = @"
        //        SELECT COUNT(*) 
        //        FROM tblIssuedBook i
        //        INNER JOIN tblCatalogue c ON i.CatalogueID = c.CatalogueID
        //        INNER JOIN tblLibrary l ON c.LibraryID = l.LibraryID 
        //        LEFT JOIN tblPublisher p ON c.PublisherID = p.PublisherID
        //        INNER JOIN tblBorrowerTypeMaster bt ON i.BorrowerTypeID = bt.BorrowerTypeID
        //        WHERE i.InstituteID = @InstituteID
        //        AND i.IssueDate BETWEEN @StartDate AND @EndDate";

        //            int totalCount = await connection.ExecuteScalarAsync<int>(countSql, new
        //            {
        //                request.InstituteID,
        //                StartDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
        //                EndDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)
        //            });

        //            string sql = @"
        //        SELECT 
        //            i.IssueBookID,
        //            FORMAT(i.IssueDate, 'dd-MM-yyyy') AS IssuedOn,  -- Format IssueDate to DD-MM-YYYY
        //            bt.BorrowerType,
        //            c.CatalogueID AS BookID,
        //            c.BookName,
        //            c.ISBNNo10 AS ISBN10,
        //            c.ISBNNo13 AS ISBN13,
        //            c.AccessionNumber,
        //            p.PublisherName AS Publisher,
        //            l.LibraryName,
        //            a.AuthorID,
        //            a.AuthorName
        //        FROM tblIssuedBook i
        //        INNER JOIN tblCatalogue c ON i.CatalogueID = c.CatalogueID
        //        INNER JOIN tblLibrary l ON c.LibraryID = l.LibraryID
        //        LEFT JOIN tblCatalogueAuthorMapping cam ON c.CatalogueID = cam.CatalogueID
        //        LEFT JOIN tblAuthor a ON cam.AuthorID = a.AuthorID
        //        LEFT JOIN tblPublisher p ON c.PublisherID = p.PublisherID
        //        INNER JOIN tblBorrowerTypeMaster bt ON i.BorrowerTypeID = bt.BorrowerTypeID
        //        WHERE i.InstituteID = @InstituteID
        //        AND i.IssueDate BETWEEN @StartDate AND @EndDate";

        //            var catalogues = await connection.QueryAsync<IssueBookResponse, AuthorResponse2, IssueBookResponse>(
        //                sql,
        //                (catalogue, author) =>
        //                {
        //                    var catalogueWithAuthors = catalogue;

        //                    if (author != null)
        //                    {
        //                        if (catalogueWithAuthors.Authors == null)
        //                        {
        //                            catalogueWithAuthors.Authors = new List<AuthorResponse2>();
        //                        }
        //                        catalogueWithAuthors.Authors.Add(author);
        //                    }

        //                    return catalogueWithAuthors;
        //                },
        //                new
        //                {
        //                    request.InstituteID,
        //                    StartDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
        //                    EndDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)
        //                },
        //                splitOn: "AuthorID");

        //            var groupedCatalogues = catalogues
        //                .GroupBy(c => c.BookID)
        //                .Select(group =>
        //                {
        //                    var firstItem = group.First();
        //                    firstItem.Authors = group.Select(g => new AuthorResponse2
        //                    {
        //                        AuthorID = g.Authors.First().AuthorID,
        //                        AuthorName = g.Authors.First().AuthorName
        //                    }).ToList();

        //                    return firstItem;
        //                })
        //                .ToList();

        //            var paginatedList = groupedCatalogues.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

        //            return new ServiceResponse<List<IssueBookResponse>>(true, "Records Found", paginatedList, 200, totalCount);
        //        }
        //        catch (Exception ex)
        //        {
        //            return new ServiceResponse<List<IssueBookResponse>>(false, ex.Message, null, 500);
        //        }
        //    }
        //}

    }
}
