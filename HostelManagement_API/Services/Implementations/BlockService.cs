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

        
        public async Task<ServiceResponse<string>>  AddUpdateBlocks(AddUpdateBlocksRequest request)
        {
            return await _blockRepository.AddUpdateBlocks(request);
        }
 
        public async Task<ServiceResponse<IEnumerable<BlockResponse>>> GetAllBlocks(GetAllBlocksRequest request)
        {
            return await _blockRepository.GetAllBlocks(request);  

        }
        public async Task<IEnumerable<BlockResponse>> GetAllBlocksFetch()
        {
            return await _blockRepository.GetAllBlocksFetch();
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
