using Communication_API.DTOs.Requests.DiscussionBoard;
using Communication_API.DTOs.Responses.DiscussionBoard;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.DiscussionBoard;

namespace Communication_API.Repository.Interfaces.DiscussionBoard
{
    public interface IDiscussionBoardRepository
    {
        Task<ServiceResponse<string>> AddUpdateDiscussion(AddUpdateDiscussionRequest request);
        Task<ServiceResponse<List<GetAllDiscussionResponse>>> GetAllDiscussion(GetAllDiscussionRequest request);
        Task<ServiceResponse<GetDiscussionBoardDetailsResponse>> GetDiscussionBoardDetails(GetDiscussionBoardDetailsRequest request); 
        Task<ServiceResponse<string>> DeleteDiscussion(int DiscussionBoardID);
        Task<ServiceResponse<string>> CreateDiscussionThread(CreateDiscussionThreadRequest request);
        Task<ServiceResponse<List<DiscussionThread>>> GetDiscussionThread(int DiscussionBoardID);
        Task<ServiceResponse<string>> AddDiscussionBoardComment(AddDiscussionBoardCommentRequest request);
        Task<ServiceResponse<string>> AddDiscussionBoardReaction(AddDiscussionBoardReactionRequest request);
        Task<ServiceResponse<List<GetDiscussionBoardCommentsResponse>>> GetDiscussionBoardComments(GetDiscussionBoardCommentsRequest request);
        Task<ServiceResponse<GetDiscussionBoardReactionsResponse>> GetDiscussionBoardReactions(GetDiscussionBoardReactionsRequest request);

    }
}
