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
    public class CatalogueRepository : ICatalogueRepository
    {
        private readonly IDbConnection _connection;

        public CatalogueRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<List<CatalogueResponse>>> GetAllCatalogues(GetAllCataloguesRequest request)
        {
            try
            {
                string countSql = "SELECT COUNT(*) FROM tblCatalogue WHERE InstituteID = @InstituteID AND IsActive = 1";
                int totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

                string sql = @"
        SELECT 
            c.CatalogueID AS BookID,
            l.LibraryName,
            c.BookName,
            c.ISBNNo10 AS ISBN10,
            c.ISBNNo13 AS ISBN13,
            c.AccessionNumber,
            a.AuthorName,
            p.PublisherName AS Publisher,
            lang.LanguageName AS Language,
            cat.LibraryCategoryName AS Category,
            g.GenreName AS Genre,
            c.Price,
            c.FundingSource AS Funding,
            c.NoOfPages AS NumberOfPages
        FROM tblCatalogue c
        INNER JOIN tblLibrary l ON c.LibraryID = l.LibraryID
        LEFT JOIN tblCatalogueAuthorMapping cam ON c.CatalogueID = cam.CatalogueID
        LEFT JOIN tblAuthor a ON cam.AuthorID = a.AuthorID
        LEFT JOIN tblPublisher p ON c.PublisherID = p.PublisherID
        LEFT JOIN tblLibraryLanguage lang ON c.LanguageID = lang.LanguageID
        LEFT JOIN tblLibraryCategory cat ON c.LibraryCategoryID = cat.LibraryCategoryID
        LEFT JOIN tblGenre g ON c.GenreID = g.GenreID
        WHERE c.InstituteID = @InstituteID AND c.IsActive = 1";

                var catalogues = await _connection.QueryAsync<CatalogueResponse>(sql, new { request.InstituteID });

                var paginatedList = catalogues.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

                return new ServiceResponse<List<CatalogueResponse>>(true, "Records Found", paginatedList, 200, totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<CatalogueResponse>>(false, ex.Message, null, 500);
            }
        }


        public async Task<ServiceResponse<Catalogue>> GetCatalogueById(int catalogueId)
        {
            try
            {
                string sql = @"SELECT * FROM tblCatalogue WHERE CatalogueID = @CatalogueID AND IsActive = 1";
                var catalogue = await _connection.QueryFirstOrDefaultAsync<Catalogue>(sql, new { CatalogueID = catalogueId });

                return catalogue != null ?
                    new ServiceResponse<Catalogue>(true, "Record Found", catalogue, 200) :
                    new ServiceResponse<Catalogue>(false, "Record Not Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Catalogue>(false, ex.Message, null, 500);
            }
        }


        public async Task<ServiceResponse<string>> AddUpdateCatalogue(Catalogue request)
        {
            try
            {
                string sql = request.CatalogueID == 0 ?
                    @"INSERT INTO tblCatalogue (InstituteID, BookName, ISBNNo10, ISBNNo13, AccessionNumber, Price, FundingSource, NoOfPages, BookID, LibraryID, DateOfPurchase, LocationID, PublishedDate, Edition, Volume, Comments, PublisherID, LanguageID, LibraryCategoryID, GenreID, IsActive) 
              VALUES (@InstituteID, @BookName, @ISBN10, @ISBN13, @AccessionNumber, @Price, @FundingSource, @NumberOfPages, @BookID, @LibraryID, @DateOfPurchase, @LocationID, @PublishedDate, @Edition, @Volume, @Comments, @PublisherID, @LanguageID, @LibraryCategoryID, @GenreID, @IsActive)" :
                    @"UPDATE tblCatalogue SET InstituteID = @InstituteID, BookName = @BookName, ISBNNo10 = @ISBN10, ISBNNo13 = @ISBN13, AccessionNumber = @AccessionNumber, Price = @Price, FundingSource = @FundingSource, NoOfPages = @NumberOfPages, BookID = @BookID, LibraryID = @LibraryID, DateOfPurchase = @DateOfPurchase, LocationID = @LocationID, PublishedDate = @PublishedDate, Edition = @Edition, Volume = @Volume, Comments = @Comments, PublisherID = @PublisherID, LanguageID = @LanguageID, LibraryCategoryID = @LibraryCategoryID, GenreID = @GenreID, IsActive = @IsActive WHERE CatalogueID = @CatalogueID";

                int rowsAffected = await _connection.ExecuteAsync(sql, new
                {
                    request.InstituteID,
                    request.BookName,
                    request.ISBN10,
                    request.ISBN13,
                    request.AccessionNumber,
                    request.Price,
                    request.FundingSource,
                    request.NumberOfPages,
                    request.BookID,
                    request.LibraryID,
                    request.DateOfPurchase,
                    request.LocationID,
                    request.PublishedDate,
                    request.Edition,
                    request.Volume,
                    request.Comments,
                    request.PublisherID,
                    request.LanguageID,
                    request.LibraryCategoryID,
                    request.GenreID,
                    request.IsActive,
                    request.CatalogueID
                });

                return new ServiceResponse<string>(rowsAffected > 0, rowsAffected > 0 ? "Success" : "Failure",
                    rowsAffected > 0 ? "Catalogue saved successfully" : "Failed to save catalogue", rowsAffected > 0 ? 200 : 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteCatalogue(int catalogueId)
        {
            try
            {
                string sql = "UPDATE tblCatalogue SET IsActive = 0 WHERE CatalogueID = @CatalogueID";
                int rowsAffected = await _connection.ExecuteAsync(sql, new { CatalogueID = catalogueId });

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
