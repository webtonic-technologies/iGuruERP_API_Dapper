using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;

namespace Institute_API.Repository.Interfaces
{
    public interface IGalleryRepository
    {
        Task<ServiceResponse<int>> AddGalleryImage(GalleryDTO galleryDTO);
        Task<ServiceResponse<bool>> UpdateGalleryImageApprovalStatus(int galleryId, int Status, int userId);
        Task<ServiceResponse<List<GalleryEventDTO>>> GetApprovedImagesByEvent(int Institute_id, int Status,int? pageSize = null, int? pageNumber = null);
        Task<ServiceResponse<List<GalleryEventDTO>>> GetAllGalleryImagesByEvent(int Institute_id, int Event_id,int? pageSize = null, int? pageNumber = null);
        Task<ServiceResponse<bool>> DeleteGalleryImage(int Gallery_id);
    }
}
