using Communication_API.DTOs.Requests.DiscussionBoard;
using Communication_API.DTOs.Responses.DiscussionBoard;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.DiscussionBoard;
using Communication_API.Repository.Interfaces.DiscussionBoard;
using Communication_API.Services.Interfaces.DiscussionBoard;

namespace Communication_API.Services.Implementations.DiscussionBoard
{
    public class DiscussionBoardService : IDiscussionBoardService
    {
        private readonly IDiscussionBoardRepository _discussionBoardRepository;

        public DiscussionBoardService(IDiscussionBoardRepository discussionBoardRepository)
        {
            _discussionBoardRepository = discussionBoardRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateDiscussion(AddUpdateDiscussionRequest request)
        {
            return await _discussionBoardRepository.AddUpdateDiscussion(request);
        }

        public async Task<ServiceResponse<List<GetAllDiscussionResponse>>> GetAllDiscussion(GetAllDiscussionRequest request)
        {
            return await _discussionBoardRepository.GetAllDiscussion(request);
        }
         
        public async Task<ServiceResponse<GetDiscussionBoardDetailsResponse>> GetDiscussionBoardDetails(GetDiscussionBoardDetailsRequest request)
        {
            return await _discussionBoardRepository.GetDiscussionBoardDetails(request);
        }

        public async Task<ServiceResponse<string>> DeleteDiscussion(int DiscussionBoardID)
        {
            return await _discussionBoardRepository.DeleteDiscussion(DiscussionBoardID);
        }
         

        public async Task<ServiceResponse<string>> CreateDiscussionThread(CreateDiscussionThreadRequest request)
        {
            return await _discussionBoardRepository.CreateDiscussionThread(request);
        }

        public async Task<ServiceResponse<List<DiscussionThread>>> GetDiscussionThread(int DiscussionBoardID)
        {
            return await _discussionBoardRepository.GetDiscussionThread(DiscussionBoardID);
        }

        public async Task<ServiceResponse<string>> AddDiscussionBoardComment(AddDiscussionBoardCommentRequest request)
        {
            return await _discussionBoardRepository.AddDiscussionBoardComment(request);
        }

        public async Task<ServiceResponse<string>> AddDiscussionBoardReaction(AddDiscussionBoardReactionRequest request)
        {
            return await _discussionBoardRepository.AddDiscussionBoardReaction(request);
        }

        public async Task<ServiceResponse<List<GetDiscussionBoardCommentsResponse>>> GetDiscussionBoardComments(GetDiscussionBoardCommentsRequest request)
        {
            return await _discussionBoardRepository.GetDiscussionBoardComments(request);
        }

        public async Task<ServiceResponse<GetDiscussionBoardReactionsResponse>> GetDiscussionBoardReactions(GetDiscussionBoardReactionsRequest request)
        {
            return await _discussionBoardRepository.GetDiscussionBoardReactions(request);
        }
    }
}
