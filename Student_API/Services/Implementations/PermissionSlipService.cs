using QRCoder;
using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using Student_API.Services.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection.Metadata;

namespace Student_API.Services.Implementations
{
    public class PermissionSlipService : IPermissionSlipService
    {
        private readonly IPermissionSlipRepository _repository;
        private readonly IImageService _imageService;

        public PermissionSlipService(IPermissionSlipRepository repository, IImageService imageService)
        {
            _repository = repository;
            _imageService = imageService;
        }

        public async Task<ServiceResponse<List<PermissionSlipDTO>>> GetAllPermissionSlips(int Institute_id, int classId, int sectionId, int? pageNumber = null, int? pageSize = null)
        {
            return await _repository.GetAllPermissionSlips(Institute_id, classId, sectionId, pageNumber, pageSize);
        }
        public async Task<ServiceResponse<List<PermissionSlipDTO>>> GetPermissionSlips(int Institute_id, int classId, int sectionId, string startDate, string endDate, bool isApproved, int? pageNumber = null, int? pageSize = null)
        {
            return await _repository.GetPermissionSlips(Institute_id, classId, sectionId, startDate, endDate, isApproved, pageNumber, pageSize);
        }
        public async Task<ServiceResponse<string>> UpdatePermissionSlipStatus(int permissionSlipId, bool isApproved)
        {
            return await _repository.UpdatePermissionSlipStatus(permissionSlipId, isApproved);
        }

        public async Task<ServiceResponse<string>> AddPermissionSlip(PermissionSlip permissionSlipDto)
        {

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(permissionSlipDto.Student_Id + "  " + permissionSlipDto.Student_Parent_Info_id, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCodeImage = new QRCode(qrCodeData);

            using (Bitmap bitmap = qrCodeImage.GetGraphic(60))
            {
                string base64String;
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    byte[] byteImage = ms.ToArray();
                    base64String = Convert.ToBase64String(byteImage);
                }

                var result = await _imageService.SaveImageAsync(base64String, "Insititute_" + permissionSlipDto.Institute_id + "/PermissionQrCodes");
                permissionSlipDto.Qr_Code = result.relativePath;
            }
            return await _repository.AddPermissionSlip(permissionSlipDto);
        }

        public async Task<ServiceResponse<SinglePermissionSlipDTO>> GetPermissionSlipById(int permissionSlipId)
        {
            var data = await _repository.GetPermissionSlipById(permissionSlipId);
            if (data != null)
            {
                if (!string.IsNullOrEmpty(data.Data.Qr_Code) && File.Exists(data.Data.Qr_Code))
                {
                    data.Data.Qr_Code = _imageService.GetImageAsBase64(data.Data.Qr_Code);
                }
            }
            return data;
        }
    }

}
