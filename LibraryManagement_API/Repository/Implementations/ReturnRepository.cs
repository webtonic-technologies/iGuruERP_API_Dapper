using Dapper;
using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LibraryManagement_API.Repository.Implementations
{
    public class ReturnRepository : IReturnRepository
    {
        private readonly IDbConnection _connection;

        public ReturnRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<List<GetReturnStudentBookResponse>>> GetReturnStudentBook(GetReturnStudentBookRequest request)
        {
            try
            {
                // Updated SQL query to include date conversion from DateTime to string
                string sql = @"
                SELECT 
                    b.CatalogueID, 
                    b.BookID, 
                    l.LibraryName, 
                    b.BookName, 
                    b.AccessionNumber, 
                    s.First_Name AS StudentName, 
                    s.Admission_Number AS AdmissionNumber, 
                    s.Roll_Number AS RollNumber, 
                    CONCAT(c.class_name, ' - ', sec.section_name) AS ClassSection, 
                    CONVERT(VARCHAR, ib.IssueDate, 105) AS IssueDate,  -- IssueDate as string in DD-MM-YYYY format
                    CONVERT(VARCHAR, ib.DueDate, 105) AS DueDate      -- DueDate as string in DD-MM-YYYY format
                FROM tblIssuedBook ib
                INNER JOIN tblCatalogue b ON ib.CatalogueID = b.CatalogueID
                INNER JOIN tbl_StudentMaster s ON ib.BorrowerID = s.student_id
                INNER JOIN tblLibrary l ON b.LibraryID = l.LibraryID
                INNER JOIN tbl_Class c ON s.class_id = c.class_id
                INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
                WHERE ib.InstituteID = @InstituteID
                    AND ib.BorrowerTypeID = 1
                    AND ib.IssueDate BETWEEN CONVERT(DATE, @StartDate, 105) AND CONVERT(DATE, @EndDate, 105)";  // Convert string dates to DateTime

                var books = await _connection.QueryAsync(
                    sql,
                    new
                    {
                        request.InstituteID, 
                        StartDate = request.StartDate,  // Pass the string values to SQL query
                        EndDate = request.EndDate       // Pass the string values to SQL query
                    });

                // Map the results into the GetReturnStudentBookResponse
                var response = books.Select(book => new GetReturnStudentBookResponse
                {
                    BookDetails = new BookDetails
                    {
                        CatalogueID = book.CatalogueID,
                        BookID = book.BookID,
                        LibraryName = book.LibraryName,
                        BookName = book.BookName,
                        AccessionNumber = book.AccessionNumber
                    },
                    BorrowerDetails = new BorrowerDetails
                    {
                        StudentName = book.StudentName,
                        AdmissionNumber = book.AdmissionNumber,
                        RollNumber = book.RollNumber,
                        ClassSection = book.ClassSection // ClassSection is now populated correctly
                    },
                    IssueDate = book.IssueDate,  // IssueDate is now a string
                    DueDate = book.DueDate       // DueDate is now a string
                }).ToList();

                return new ServiceResponse<List<GetReturnStudentBookResponse>>(true, "Records found", response, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GetReturnStudentBookResponse>>(false, ex.Message, null, 500);
            }
        }


        public async Task<ServiceResponse<List<GetReturnEmployeeBookResponse>>> GetReturnEmployeeBook(GetReturnEmployeeBookRequest request)
        {
            try
            {
                // Updated SQL query to include date conversion from DateTime to string
                string sql = @"
                SELECT 
                    b.CatalogueID, 
                    b.BookID, 
                    l.LibraryName, 
                    b.BookName, 
                    b.AccessionNumber, 
                    e.First_Name + ' ' + e.Last_Name AS EmployeeName, 
                    e.Employee_code_id AS EmployeeID,  
	                dp.DepartmentName,
	                de.DesignationName,
                    CONVERT(VARCHAR, ib.IssueDate, 105) AS IssueDate,
                    CONVERT(VARCHAR, ib.DueDate, 105) AS DueDate
                FROM tblIssuedBook ib
                INNER JOIN tblCatalogue b ON ib.CatalogueID = b.CatalogueID
                INNER JOIN tbl_EmployeeProfileMaster e ON ib.BorrowerID = e.Employee_id
                INNER JOIN tblLibrary l ON b.LibraryID = l.LibraryID
                INNER JOIN tbl_Department dp ON e.Department_id = dp.Department_id
                INNER JOIN tbl_Designation de ON e.Designation_id = de.Designation_id
                WHERE ib.InstituteID = @InstituteID
                    AND ib.BorrowerTypeID = 2
                    AND ib.IssueDate BETWEEN CONVERT(DATE, @StartDate, 105) AND CONVERT(DATE, @EndDate, 105)";  // Convert string dates to DateTime

                var books = await _connection.QueryAsync(
                    sql,
                    new
                    {
                        request.InstituteID,
                        StartDate = request.StartDate,  // Pass the string values to SQL query
                        EndDate = request.EndDate       // Pass the string values to SQL query
                    });

                // Map the results into the GetReturnStudentBookResponse
                var response = books.Select(book => new GetReturnEmployeeBookResponse
                {
                    BookDetails = new EBookDetails
                    {
                        CatalogueID = book.CatalogueID,
                        BookID = book.BookID,
                        LibraryName = book.LibraryName,
                        BookName = book.BookName,
                        AccessionNumber = book.AccessionNumber
                    },
                    BorrowerDetails = new EBorrowerDetails
                    {
                        EmployeeName = book.EmployeeName,
                        EmployeeID = book.EmployeeID,
                        DepartmentName = book.DepartmentName,
                        DesignationName = book.DesignationName // ClassSection is now populated correctly
                    },
                    IssueDate = book.IssueDate,  // IssueDate is now a string
                    DueDate = book.DueDate       // DueDate is now a string
                }).ToList();

                return new ServiceResponse<List<GetReturnEmployeeBookResponse>>(true, "Records found", response, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GetReturnEmployeeBookResponse>>(false, ex.Message, null, 500);
            }
        }



        public async Task<ServiceResponse<List<GetReturnGuestBookResponse>>> GetReturnGuestBook(GetReturnGuestBookRequest request)
        {
            try
            {
                // Updated SQL query to include date conversion from DateTime to string
                string sql = @"
                SELECT 
                    b.CatalogueID, 
                    b.BookID, 
                    l.LibraryName, 
                    b.BookName, 
                    b.AccessionNumber, 
                    ib.GuestName, 
	                ib.GuestType, 	
	                ib.Address,
	                ib.Contact,
                    CONVERT(VARCHAR, ib.IssueDate, 105) AS IssueDate,
                    CONVERT(VARCHAR, ib.DueDate, 105) AS DueDate
                FROM tblIssuedBook ib
                INNER JOIN tblCatalogue b ON ib.CatalogueID = b.CatalogueID
                INNER JOIN tblLibrary l ON b.LibraryID = l.LibraryID 
                WHERE ib.InstituteID = @InstituteID
                    AND ib.BorrowerTypeID = 3
                    AND ib.IssueDate BETWEEN CONVERT(DATE, @StartDate, 105) AND CONVERT(DATE, @EndDate, 105)";  // Convert string dates to DateTime

                var books = await _connection.QueryAsync(
                    sql,
                    new
                    {
                        request.InstituteID,
                        StartDate = request.StartDate,  // Pass the string values to SQL query
                        EndDate = request.EndDate       // Pass the string values to SQL query
                    });

                // Map the results into the GetReturnStudentBookResponse
                var response = books.Select(book => new GetReturnGuestBookResponse
                {
                    BookDetails = new GBookDetails
                    {
                        CatalogueID = book.CatalogueID,
                        BookID = book.BookID,
                        LibraryName = book.LibraryName,
                        BookName = book.BookName,
                        AccessionNumber = book.AccessionNumber
                    },
                    BorrowerDetails = new GBorrowerDetails
                    {
                        GuestName = book.GuestName,
                        GuestType = book.GuestType,
                        Address = book.Address,
                        Contact = book.Contact // ClassSection is now populated correctly
                    },
                    IssueDate = book.IssueDate,  // IssueDate is now a string
                    DueDate = book.DueDate       // DueDate is now a string
                }).ToList();

                return new ServiceResponse<List<GetReturnGuestBookResponse>>(true, "Records found", response, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GetReturnGuestBookResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<string>> AddBookReturnCollection(CollectBookRequest request)
        {
            try
            {
                string sql = @"
                    INSERT INTO tblBookReturnCollection 
                    (ReturnActionTypeID, InstituteID, ReturnDate, DueDate, LateFeeAmount, Reason, RevisedDueDate, 
                     LostBookPenalty, TotalAmount, DamageBookPenalty, BookCondition)
                    VALUES
                    (@ReturnActionTypeID, @InstituteID, CONVERT(DATETIME, @ReturnDate, 105), 
                     CONVERT(DATETIME, @DueDate, 105), @LateFeeAmount, @Reason, CONVERT(DATETIME, @RevisedDueDate, 105),
                     @LostBookPenalty, @TotalAmount, @DamageBookPenalty, @BookCondition)";

                var result = await _connection.ExecuteAsync(sql, new
                {
                    request.ReturnActionTypeID,
                    request.InstituteID,
                    request.ReturnDate,
                    request.DueDate,
                    request.LateFeeAmount,
                    request.Reason,
                    request.RevisedDueDate,
                    request.LostBookPenalty,
                    request.TotalAmount,
                    request.DamageBookPenalty,
                    request.BookCondition
                });

                return result > 0
                    ? new ServiceResponse<string>(true, "Book return collection recorded successfully", "Success", 200)
                    : new ServiceResponse<string>(false, "Failed to collect book return", null, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

    }
}
