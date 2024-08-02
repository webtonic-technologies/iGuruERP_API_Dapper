using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.Repository.Interfaces;
using LibraryManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement_API.Services.Implementations
{
    public class IssueService : IIssueService
    {
        private readonly IIssueRepository _issueRepository;

        public IssueService(IIssueRepository issueRepository)
        {
            _issueRepository = issueRepository;
        }

        public async Task<ServiceResponse<List<IssueBookResponse>>> GetAllIssueBooks(GetAllIssueBooksRequest request)
        {
            return await _issueRepository.GetAllIssueBooks(request);
        }

        public async Task<ServiceResponse<string>> AddUpdateIssue(AddUpdateIssueRequest request)
        {
            return await _issueRepository.AddUpdateIssue(request);
        }
    }
}
