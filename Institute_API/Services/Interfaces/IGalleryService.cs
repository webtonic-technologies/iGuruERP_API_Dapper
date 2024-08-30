using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;

namespace Institute_API.Services.Interfaces
{
    public interface IGalleryService
    {
        Task<ServiceResponse<int>> AddGalleryImage(GalleryDTO galleryDTO);
        Task<ServiceResponse<List<GalleryEventDTO>>> GetApprovedImagesByEvent(GetGalleryRequestModel model);
        Task<ServiceResponse<bool>> UpdateGalleryImageApprovalStatus(int galleryId, int Status, int userId);
        Task<ServiceResponse<List<GalleryEventDTO>>> GetAllGalleryImagesByEvent(GetGalleryRequestModel model);
        Task<ServiceResponse<bool>> DeleteGalleryImage(int Gallery_id);
    }
}
