using EventGallery_API.DTOs.Requests;
using EventGallery_API.DTOs.ServiceResponse;
using EventGallery_API.Services.Interfaces;
using EventGallery_API.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System.IO.Compression;

namespace EventGallery_API.Controllers
{
    [Route("iGuru/Gallery")]
    [ApiController]
    public class GalleryController : ControllerBase
    {
        private readonly IGalleryService _galleryService;

        public GalleryController(IGalleryService galleryService)
        {
            _galleryService = galleryService;
        }

        [HttpPost("UploadGalleryImage")]
        public async Task<IActionResult> UploadGalleryImage([FromBody] GalleryImageRequest request)
        {
            // Convert the base64 string into bytes
            byte[] imageBytes = Convert.FromBase64String(request.FileName);

            // Define the folder path to save the image
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Gallery");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Create a file name using a combination of galleryID and current time
            string fileName = $"gallery_{request.GalleryID}_{DateTime.Now.Ticks}.jpg";
            string filePath = Path.Combine(folderPath, fileName);

            // Write the image file to disk
            await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

            // Update the file name in the request object
            request.FileName = fileName;

            // Call the service to upload the gallery image, passing in the eventID and the request
            var result = await _galleryService.UploadGalleryImage(request.EventID, request);

            // Return the result as a response
            return Ok(result);
        }


        //[HttpPost("UploadGalleryImage")]
        //public async Task<IActionResult> UploadGalleryImage([FromBody] GalleryImageRequest request)
        //{
        //    byte[] imageBytes = Convert.FromBase64String(request.FileName);

        //    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Gallery");
        //    if (!Directory.Exists(folderPath))
        //    {
        //        Directory.CreateDirectory(folderPath);
        //    }

        //    string fileName = $"gallery_{request.GalleryID}_{DateTime.Now.Ticks}.jpg";
        //    string filePath = Path.Combine(folderPath, fileName);
        //    await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

        //    request.FileName = fileName;

        //    // Use the eventID from the request body
        //    var result = await _galleryService.UploadGalleryImage(request.EventID, request);
        //    return Ok(result);
        //}


        [HttpGet("DownloadGalleryImage/{GalleryID}")]
        public async Task<IActionResult> DownloadGalleryImage(int GalleryID)
        {
            var result = await _galleryService.DownloadGalleryImage(GalleryID);
            if (result == null || result.Data == null)
                return NotFound();
            return File(result.Data.ImageData, "image/jpeg", result.Data.ImageName);
        }

        [HttpGet("DownloadAllGalleryImages/{EventID}")]
        public async Task<IActionResult> DownloadAllGalleryImages(int EventID)
        {
            // Fetch images as byte array and file names from the service layer
            var result = await _galleryService.DownloadAllGalleryImages(EventID);

            if (result == null || !result.Success)
            {
                return NotFound("No images found for the given event.");
            }

            // Create a temporary ZIP file in memory
            using (var memoryStream = new MemoryStream())
            {
                using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var image in result.Data)
                    {
                        // Add each image to the ZIP
                        var zipEntry = zip.CreateEntry(image.ImageName);

                        using (var zipEntryStream = zipEntry.Open())
                        {
                            await zipEntryStream.WriteAsync(image.ImageData, 0, image.ImageData.Length);
                        }
                    }
                }

                // Send the ZIP file as a downloadable response
                memoryStream.Seek(0, SeekOrigin.Begin);
                return File(memoryStream.ToArray(), "application/zip", $"Event_{EventID}_GalleryImages.zip");
            }
        }

        [HttpDelete("DeleteGalleryImage/{GalleryID}")]
        public async Task<IActionResult> DeleteGalleryImage(int GalleryID)
        {
            var result = await _galleryService.DeleteGalleryImage(GalleryID);
            return Ok(result);
        }

        [HttpPost("GetAllGalleryImages")]
        public async Task<IActionResult> GetAllGalleryImages([FromBody] GalleryImageRequest_Get request)
        {
            // Fetch images along with event details
            var result = await _galleryService.GetAllGalleryImages(request);

            return Ok(result);
        }

        //[HttpPost("GetAllEvents/{InstituteID}")]
        //public async Task<IActionResult> GetAllEvents(int InstituteID)
        //{
        //    // Fetch events based on InstituteID
        //    var events = await _galleryService.GetAllEvents(InstituteID);

        //    // Map to response body format
        //    var response = events.ConvertAll(e => new
        //    {
        //        e.EventID,
        //        e.EventName
        //    });

        //    return Ok(new
        //    {
        //        success = true,
        //        message = "Events fetched successfully.",
        //        data = response,
        //        statusCode = 200
        //    });
        //}

        [HttpPost("GetAllEvents")]
        public async Task<IActionResult> GetAllEvents([FromBody] GetAllEventsRequestList request)
        {
            // Fetch events based on InstituteID and AcademicYearCode
            var events = await _galleryService.GetAllEvents(request.InstituteID, request.AcademicYearCode);

            // Map to response body format
            var response = events.ConvertAll(e => new
            {
                e.EventID,
                e.EventName
            });

            return Ok(new
            {
                success = true,
                message = "Events fetched successfully.",
                data = response,
                statusCode = 200
            });
        }


    }
}
