using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement_API.Repository.Interfaces
{
    public interface IIssueRepository
    {
        Task<ServiceResponse<List<IssueBookResponse>>> GetAllIssueBooks(GetAllIssueBooksRequest request);
        Task<ServiceResponse<string>> AddUpdateIssue(AddUpdateIssueRequest request);
    }
}
