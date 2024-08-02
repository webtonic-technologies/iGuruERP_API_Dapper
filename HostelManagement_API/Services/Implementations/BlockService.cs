using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using HostelManagement_API.Repository.Interfaces;
using HostelManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Implementations
{
    public class BlockService : IBlockService
    {
        private readonly IBlockRepository _blockRepository;

        public BlockService(IBlockRepository blockRepository)
        {
            _blockRepository = blockRepository;
        }

        public async Task<ServiceResponse<int>> AddUpdateBlock(AddUpdateBlockRequest request)
        {
            var blockId = await _blockRepository.AddUpdateBlock(request);
            return new ServiceResponse<int>(true, "Block added/updated successfully", blockId, 200);
        }

        public async Task<ServiceResponse<PagedResponse<BlockResponse>>> GetAllBlocks(GetAllBlocksRequest request)
        {
            var blocks = await _blockRepository.GetAllBlocks(request);
            return new ServiceResponse<PagedResponse<BlockResponse>>(true, "Blocks retrieved successfully", blocks, 200);
        }

        public async Task<ServiceResponse<BlockResponse>> GetBlockById(int blockId)
        {
            var block = await _blockRepository.GetBlockById(blockId);
            return new ServiceResponse<BlockResponse>(true, "Block retrieved successfully", block, 200);
        }

        public async Task<ServiceResponse<int>> DeleteBlock(int blockId)
        {
            var result = await _blockRepository.DeleteBlock(blockId);
            return new ServiceResponse<int>(true, "Block deleted successfully", result, 200);
        }
    }
}
