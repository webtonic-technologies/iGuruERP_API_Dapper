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
using System.Globalization;
using System.Data.Common;

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
                // Check if the SearchTerm can be parsed as an integer (for BookID)
                int? parsedBookID = null;
                if (int.TryParse(request.SearchTerm, out int bookID))
                {
                    parsedBookID = bookID;
                }

                // SQL to get the total count of catalogues based on the search
                string countSql = @"
                SELECT COUNT(*) 
                FROM tblCatalogue c
                LEFT JOIN tblLibrary l ON c.LibraryID = l.LibraryID  
                LEFT JOIN tblPublisher p ON c.PublisherID = p.PublisherID
                LEFT JOIN tblLibraryLanguage lang ON c.LanguageID = lang.LanguageID
                LEFT JOIN tblLibraryCategory cat ON c.LibraryCategoryID = cat.LibraryCategoryID
                LEFT JOIN tblGenre g ON c.GenreID = g.GenreID
                WHERE c.InstituteID = @InstituteID AND c.IsActive = 1
                AND (
                    (@SearchTermInt IS NULL AND c.BookName LIKE '%' + @SearchTerm + '%') OR
                    (@SearchTermInt IS NULL AND c.ISBNNo10 LIKE '%' + @SearchTerm + '%') OR
                    (@SearchTermInt IS NULL AND c.AccessionNumber LIKE '%' + @SearchTerm + '%') OR
                    (c.CatalogueID = @SearchTermInt)
                )";

                // Get total count of records
                int totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new
                {
                    request.InstituteID,
                    SearchTerm = request.SearchTerm,
                  SearchTermInt = parsedBookID // Pass null for SearchTermInt if it's not a valid integer
                });




                string columnListSql = @"SELECT STRING_AGG(VC.DatabaseFieldName, ', ') 
                                    FROM tblCatalogueColumnSetting VC
                                    INNER JOIN tblCatalogueSettingMapping VSM ON VC.CatalogueColumnID = VSM.CatalogueColumnID 
                                        AND VSM.InstituteID = @InstituteID
                                    WHERE VC.IsActive = 1 ";
                string columnList = await _connection.ExecuteScalarAsync<string>(columnListSql, new { request.InstituteID });


                string sql = $@"
                SELECT CatalogueID, {columnList}, AuthorID, AuthorName 
                FROM (
                    SELECT 
                        c.CatalogueID,
                        c.BookID,
	                    c.LibraryID,
                        l.LibraryName,
                        c.BookName,
                        c.AccessionNumber,
                        c.ISBNNo10 AS ISBN10,
                        c.ISBNNo13 AS ISBN13,
	                    c.PublisherID AS PublisherID,
                        p.PublisherName AS Publisher,
	                    c.LanguageID AS LanguageID,
                        lang.LanguageName AS Language,
	                    c.LibraryCategoryID AS LibraryCategoryID,
                        cat.LibraryCategoryName AS Category,    
	                    C.GenreID AS GenreID, 
                        g.GenreName AS Genre,
                        c.Price,
                        c.NoOfPages AS NumberOfPages, 
	                    c.DateOfPurchase,
	                    c.LocationID,
	                    c.PublishedDate,
	                    c.Edition,
	                    c.Volume,
                        c.FundingSource AS Funding,
	                    c.Comments,
	                    c.IsActive,
                        a.AuthorID,
                        a.AuthorName
                    FROM tblCatalogue c
                    LEFT JOIN tblLibrary l ON c.LibraryID = l.LibraryID
                    LEFT JOIN tblCatalogueAuthorMapping cam ON c.CatalogueID = cam.CatalogueID
                    LEFT JOIN tblAuthor a ON cam.AuthorID = a.AuthorID
                    LEFT JOIN tblPublisher p ON c.PublisherID = p.PublisherID
                    LEFT JOIN tblLibraryLanguage lang ON c.LanguageID = lang.LanguageID
                    LEFT JOIN tblLibraryCategory cat ON c.LibraryCategoryID = cat.LibraryCategoryID
                    LEFT JOIN tblGenre g ON c.GenreID = g.GenreID
                    WHERE c.InstituteID = @InstituteID AND c.IsActive = 1
                    AND (
                        (@SearchTermInt IS NULL AND c.BookName LIKE '%' + @SearchTerm + '%') OR
                        (@SearchTermInt IS NULL AND c.ISBNNo10 LIKE '%' + @SearchTerm + '%') OR
                        (@SearchTermInt IS NULL AND c.AccessionNumber LIKE '%' + @SearchTerm + '%') OR
                        (c.CatalogueID = @SearchTermInt)
                    )
                ) AS db;
                ";


                // SQL to fetch catalogues with the search applied
                //string sql = @"
                //SELECT 
                //    c.CatalogueID,
                //    c.BookID,
                //    l.LibraryName,
                //    c.BookName,
                //    c.ISBNNo10 AS ISBN10,
                //    c.ISBNNo13 AS ISBN13,
                //    c.AccessionNumber,
                //    p.PublisherName AS Publisher,
                //    lang.LanguageName AS Language,
                //    cat.LibraryCategoryName AS Category,
                //    g.GenreName AS Genre,
                //    c.Price,
                //    c.FundingSource AS Funding,
                //    c.NoOfPages AS NumberOfPages, 
                //    a.AuthorID,
                //    a.AuthorName
                //FROM tblCatalogue c
                //LEFT JOIN tblLibrary l ON c.LibraryID = l.LibraryID
                //LEFT JOIN tblCatalogueAuthorMapping cam ON c.CatalogueID = cam.CatalogueID
                //LEFT JOIN tblAuthor a ON cam.AuthorID = a.AuthorID
                //LEFT JOIN tblPublisher p ON c.PublisherID = p.PublisherID
                //LEFT JOIN tblLibraryLanguage lang ON c.LanguageID = lang.LanguageID
                //LEFT JOIN tblLibraryCategory cat ON c.LibraryCategoryID = cat.LibraryCategoryID
                //LEFT JOIN tblGenre g ON c.GenreID = g.GenreID
                //WHERE c.InstituteID = @InstituteID AND c.IsActive = 1
                //AND (
                //    (@SearchTermInt IS NULL AND c.BookName LIKE '%' + @SearchTerm + '%') OR
                //    (@SearchTermInt IS NULL AND c.ISBNNo10 LIKE '%' + @SearchTerm + '%') OR
                //    (@SearchTermInt IS NULL AND c.AccessionNumber LIKE '%' + @SearchTerm + '%') OR
                //    (c.CatalogueID = @SearchTermInt)
                //)";

                var catalogues = await _connection.QueryAsync<CatalogueResponse, AuthorResponse1, CatalogueResponse>(
                    sql,
                    (catalogue, author) =>
                    {
                        // Ensure all properties are populated correctly
                        var catalogueWithAuthors = catalogue;

                        // Initialize authors list if it's null
                        if (author != null)
                        {
                            if (catalogueWithAuthors.Authors == null)
                            {
                                catalogueWithAuthors.Authors = new List<AuthorResponse1>();
                            }
                            catalogueWithAuthors.Authors.Add(author);
                        }

                        return catalogueWithAuthors;
                    },
                    new { request.InstituteID, SearchTerm = request.SearchTerm, SearchTermInt = parsedBookID },
                    splitOn: "AuthorID");

                // Group by BookID to merge author data correctly
                var groupedCatalogues = catalogues
                    .GroupBy(c => c.BookID)
                    .Select(group =>
                    {
                        var firstItem = group.First();
                        firstItem.Authors = group.Select(g => new AuthorResponse1
                        {
                            AuthorID = g.Authors.First().AuthorID,
                            AuthorName = g.Authors.First().AuthorName
                        }).ToList();

                        return firstItem;
                    })
                    .ToList();

                var paginatedList = groupedCatalogues.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();

                return new ServiceResponse<List<CatalogueResponse>>(true, "Records Found", paginatedList, 200, totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<CatalogueResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<CatalogueResponse>> GetCatalogueById(int catalogueId)
        {
            try
            {
                string sql = @"SELECT 
                c.CatalogueID,
                c.BookID,
                c.LibraryID,
                l.LibraryName,
                c.BookName,
                c.AccessionNumber,
                c.ISBNNo10 AS ISBN10,
                c.ISBNNo13 AS ISBN13,
                c.PublisherID AS PublisherID,
                p.PublisherName AS Publisher,
                c.LanguageID AS LanguageID,
                lang.LanguageName AS Language,
                c.LibraryCategoryID AS LibraryCategoryID,
                cat.LibraryCategoryName AS Category,    
                C.GenreID AS GenreID, 
                g.GenreName AS Genre,
                c.Price,
                c.NoOfPages AS NumberOfPages, 
                c.DateOfPurchase,
                c.LocationID,
                c.PublishedDate,
                c.Edition,
                c.Volume,
                c.FundingSource AS Funding,
                c.Comments,
                c.IsActive,
                a.AuthorID,
                a.AuthorName
            FROM tblCatalogue c
            LEFT JOIN tblLibrary l ON c.LibraryID = l.LibraryID
            LEFT JOIN tblCatalogueAuthorMapping cam ON c.CatalogueID = cam.CatalogueID
            LEFT JOIN tblAuthor a ON cam.AuthorID = a.AuthorID
            LEFT JOIN tblPublisher p ON c.PublisherID = p.PublisherID
            LEFT JOIN tblLibraryLanguage lang ON c.LanguageID = lang.LanguageID
            LEFT JOIN tblLibraryCategory cat ON c.LibraryCategoryID = cat.LibraryCategoryID
            LEFT JOIN tblGenre g ON c.GenreID = g.GenreID
            WHERE c.CatalogueID = @CatalogueID";

                // Fetch catalogues and authors
                var catalogues = await _connection.QueryAsync<CatalogueResponse, AuthorResponse1, CatalogueResponse>(
                    sql,
                    (catalogue, author) =>
                    {
                        // Ensure the catalogue has authors assigned correctly
                        if (author != null)
                        {
                            // Initialize authors list if it's null
                            if (catalogue.Authors == null)
                            {
                                catalogue.Authors = new List<AuthorResponse1>();
                            }
                            catalogue.Authors.Add(author);
                        }
                        return catalogue;
                    },
                    new { CatalogueID = catalogueId },
                    splitOn: "AuthorID");

                // Ensure the authors are grouped correctly under a single catalogue
                var catalogueWithAuthors = catalogues
                    .GroupBy(c => c.CatalogueID)
                    .Select(group =>
                    {
                        var firstItem = group.First();
                        firstItem.Authors = group.Select(g => new AuthorResponse1
                        {
                            AuthorID = g.Authors.First().AuthorID,
                            AuthorName = g.Authors.First().AuthorName
                        }).ToList();
                        return firstItem;
                    })
                    .FirstOrDefault();

                // Return response if a catalogue was found
                if (catalogueWithAuthors != null)
                {
                    return new ServiceResponse<CatalogueResponse>(true, "Record Found", catalogueWithAuthors, 200);
                }

                return new ServiceResponse<CatalogueResponse>(false, "Record Not Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<CatalogueResponse>(false, ex.Message, null, 500);
            }
        }


        public async Task<ServiceResponse<string>> AddUpdateCatalogue(Catalogue request)
        {
            try
            {
                // Ensure that DateOfPurchase and PublishedDate are in DD-MM-YYYY format and within a valid range
                DateTime dateOfPurchase;
                DateTime publishedDate;

                // Try to parse the DateOfPurchase and PublishedDate to DateTime
                bool isDateOfPurchaseValid = DateTime.TryParseExact(request.DateOfPurchase, "dd-MM-yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out dateOfPurchase);

                bool isPublishedDateValid = DateTime.TryParseExact(request.PublishedDate, "dd-MM-yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out publishedDate);

                if (!isDateOfPurchaseValid || !isPublishedDateValid)
                {
                    return new ServiceResponse<string>(false, "Invalid date format. Please use DD-MM-YYYY.", null, 400);
                }

                string sql = request.CatalogueID == 0 ?
                    @"INSERT INTO tblCatalogue (InstituteID, BookName, ISBNNo10, ISBNNo13, AccessionNumber, Price, FundingSource, NoOfPages, BookID, LibraryID, DateOfPurchase, LocationID, PublishedDate, Edition, Volume, Comments, PublisherID, LanguageID, LibraryCategoryID, GenreID, IsActive) 
VALUES (@InstituteID, @BookName, @ISBN10, @ISBN13, @AccessionNumber, @Price, @FundingSource, @NumberOfPages, @BookID, @LibraryID, @DateOfPurchase, @LocationID, @PublishedDate, @Edition, @Volume, @Comments, @PublisherID, @LanguageID, @LibraryCategoryID, @GenreID, @IsActive);

SELECT CAST(SCOPE_IDENTITY() AS INT);" :
                    @"UPDATE tblCatalogue SET InstituteID = @InstituteID, BookName = @BookName, ISBNNo10 = @ISBN10, ISBNNo13 = @ISBN13, AccessionNumber = @AccessionNumber, Price = @Price, FundingSource = @FundingSource, NoOfPages = @NumberOfPages, BookID = @BookID, LibraryID = @LibraryID, DateOfPurchase = @DateOfPurchase, LocationID = @LocationID, PublishedDate = @PublishedDate, Edition = @Edition, Volume = @Volume, Comments = @Comments, PublisherID = @PublisherID, LanguageID = @LanguageID, LibraryCategoryID = @LibraryCategoryID, GenreID = @GenreID, IsActive = @IsActive WHERE CatalogueID = @CatalogueID;

SELECT @CatalogueID;";  // Return the CatalogueID for update scenario

                // Execute SQL to insert or update the catalogue and get the CatalogueID
                int catalogueID = await _connection.QueryFirstOrDefaultAsync<int>(sql, new
                {
                    request.InstituteID,
                    request.BookName,
                    request.ISBN10,            // Correctly passed ISBN10
                    request.ISBN13,            // Correctly passed ISBN13
                    request.AccessionNumber,
                    request.Price,
                    request.FundingSource,
                    request.NumberOfPages,
                    request.BookID,
                    request.LibraryID,
                    DateOfPurchase = dateOfPurchase,  // Pass DateTime directly
                    request.LocationID,
                    PublishedDate = publishedDate,  // Pass DateTime directly
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

                // Check if catalogueID is valid
                if (catalogueID == 0)
                {
                    return new ServiceResponse<string>(false, "Failed to save catalogue. No rows affected.", null, 400);
                }

                // Delete existing authors for the catalogue if there are any
                string deleteAuthorsSql = @"DELETE FROM tblCatalogueAuthorMapping WHERE CatalogueID = @CatalogueID";
                await _connection.ExecuteAsync(deleteAuthorsSql, new { CatalogueID = catalogueID });

                // Now, insert the authors for this catalogue if there are any
                if (request.AuthorIDs != null && request.AuthorIDs.Count > 0)
                {
                    foreach (var authorID in request.AuthorIDs)
                    {
                        string authorSql = @"INSERT INTO tblCatalogueAuthorMapping (CatalogueID, AuthorID) 
                                     VALUES (@CatalogueID, @AuthorID)";
                        await _connection.ExecuteAsync(authorSql, new
                        {
                            CatalogueID = catalogueID,  // Use the inserted CatalogueID
                            AuthorID = authorID
                        });
                    }
                }

                return new ServiceResponse<string>(true, "Success", "Catalogue saved successfully", 200);
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

        public async Task<ServiceResponse<IEnumerable<GetCatalogueSettingResponse>>> GetCatalogueSetting(GetCatalogueSettingRequest request)
        {
            string sqlQuery = @"
            SELECT VC.CatalogueColumnID, VC.ScreenFieldName, 
                CASE WHEN VM.CatalogueColumnID IS NOT NULL THEN '1' ELSE '0' END AS Status
            FROM tblCatalogueColumnSetting VC
            LEFT OUTER JOIN tblCatalogueSettingMapping VM ON VM.CatalogueColumnID = VC.CatalogueColumnID
            AND VM.InstituteID = @InstituteID"
            ;

            var result = await _connection.QueryAsync<GetCatalogueSettingResponse>(sqlQuery, new { request.InstituteID });

            return new ServiceResponse<IEnumerable<GetCatalogueSettingResponse>>(
                true, "Catalogue settings fetched successfully", result, 200);
        }

        public async Task<ServiceResponse<string>> AddRemoveCatalogueSetting(AddRemoveCatalogueSettingRequest request)
        {
            // Check if the entry already exists
            string checkSql = @"SELECT COUNT(*) 
                            FROM tblCatalogueSettingMapping 
                            WHERE InstituteID = @InstituteID AND CatalogueColumnID = @CatalogueColumnID";
            int count = await _connection.ExecuteScalarAsync<int>(checkSql, new { request.InstituteID, request.CatalogueColumnID });

            if (count > 0)
            {
                // Delete if exists
                string deleteSql = @"DELETE FROM tblCatalogueSettingMapping 
                                 WHERE InstituteID = @InstituteID AND CatalogueColumnID = @CatalogueColumnID";
                await _connection.ExecuteAsync(deleteSql, new { request.InstituteID, request.CatalogueColumnID });
                return new ServiceResponse<string>(true, "Catalogue setting removed successfully", "Success", 200);
            }
            else
            {
                // Add if does not exist
                string insertSql = @"INSERT INTO tblCatalogueSettingMapping (InstituteID, CatalogueColumnID)
                                 VALUES (@InstituteID, @CatalogueColumnID)";
                await _connection.ExecuteAsync(insertSql, new { request.InstituteID, request.CatalogueColumnID });
                return new ServiceResponse<string>(true, "Catalogue setting added successfully", "Success", 200);
            }
        }
    }
}
