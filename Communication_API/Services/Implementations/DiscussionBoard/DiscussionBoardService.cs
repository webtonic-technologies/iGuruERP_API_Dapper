using Communication_API.DTOs.Requests.DiscussionBoard;
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

        public async Task<ServiceResponse<List<Communication_API.Models.DiscussionBoard.DiscussionBoard>>> GetAllDiscussion(GetAllDiscussionRequest request)
        {
            return await _discussionBoardRepository.GetAllDiscussion(request);
        }

        public async Task<ServiceResponse<string>> DeleteDiscussion(int DiscussionBoardID)
        {
            return await _discussionBoardRepository.DeleteDiscussion(DiscussionBoardID);
        }

        public async Task<ServiceResponse<Communication_API.Models.DiscussionBoard.DiscussionBoard>> GetDiscussionBoard(int DiscussionBoardID)
        {
            return await _discussionBoardRepository.GetDiscussionBoard(DiscussionBoardID);
        }

        public async Task<ServiceResponse<string>> CreateDiscussionThread(CreateDiscussionThreadRequest request)
        {
            return await _discussionBoardRepository.CreateDiscussionThread(request);
        }

        public async Task<ServiceResponse<List<DiscussionThread>>> GetDiscussionThread(int DiscussionBoardID)
        {
            return await _discussionBoardRepository.GetDiscussionThread(DiscussionBoardID);
        }
    }
}
