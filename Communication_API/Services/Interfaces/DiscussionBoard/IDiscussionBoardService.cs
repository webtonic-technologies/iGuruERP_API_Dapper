using Communication_API.DTOs.Requests.DiscussionBoard;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.DiscussionBoard;

namespace Communication_API.Services.Interfaces.DiscussionBoard
{
    public interface IDiscussionBoardService
    {
        Task<ServiceResponse<string>> AddUpdateDiscussion(AddUpdateDiscussionRequest request);
        Task<ServiceResponse<List<Communication_API.Models.DiscussionBoard.DiscussionBoard>>> GetAllDiscussion(GetAllDiscussionRequest request);
        Task<ServiceResponse<string>> DeleteDiscussion(int DiscussionBoardID);
        Task<ServiceResponse<Communication_API.Models.DiscussionBoard.DiscussionBoard>> GetDiscussionBoard(int DiscussionBoardID);
        Task<ServiceResponse<string>> CreateDiscussionThread(CreateDiscussionThreadRequest request);
        Task<ServiceResponse<List<DiscussionThread>>> GetDiscussionThread(int DiscussionBoardID);
    }
}
