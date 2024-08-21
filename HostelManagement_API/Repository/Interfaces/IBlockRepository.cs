using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Interfaces
{
    public interface IBlockRepository
    {
        Task<int> AddUpdateBlocks(AddUpdateBlocksRequest request);
        Task<PagedResponse<BlockResponse>> GetAllBlocks(GetAllBlocksRequest request);
        Task<IEnumerable<BlockResponse>> GetAllBlocksFetch();

        Task<BlockResponse> GetBlockById(int blockId);
        Task<int> DeleteBlock(int blockId);
    }
}
