
using Institute_API.Services.Interfaces;
using System.Text;

namespace Institute_API.Services.Implementations
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ImageService(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<(string fileName, string relativePath, string absolutePath)> SaveImageAsync(string image, string targetFolder)
        {
            byte[] imageData = Convert.FromBase64String(image);
            string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", targetFolder);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Determine file extension based on image data
            string fileExtension = GetImageExtension(imageData);

            // Append the file extension to the provided file name
            string fileName = Guid.NewGuid().ToString() + fileExtension;

            // Construct the file path
            string filePath = Path.Combine(directoryPath, fileName);

            // Write the byte array to the image file asynchronously
            await Task.Run(() => File.WriteAllBytes(filePath, imageData));

            // Return both the relative and absolute paths
            string relativePath = Path.Combine("Assets", targetFolder, fileName);
            return (fileName,relativePath, filePath);
        }

        public string GetImageAsBase64(string filename, string subFolder = null)
        {
            string directoryPath;
            if (string.IsNullOrEmpty(subFolder))
            {
                directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath);
            }
            else
            {
                directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", subFolder);
            }

            string filePath = Path.Combine(directoryPath, filename);

            if (!File.Exists(filePath))
            {
                throw new Exception("File not found");
            }

            byte[] fileBytes = File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(fileBytes);
            return base64String;
        }
        public void DeleteFile(string filename, string subFolder = null)
        {
            string directoryPath;
            if (string.IsNullOrEmpty(subFolder))
            {
                directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath);
            }
            else
            {
                directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", subFolder);
            }

            string filePath = Path.Combine(directoryPath, filename);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            else
            {
                throw new FileNotFoundException("File not found", filePath);
            }
        }


        private string GetImageExtension(byte[] imageData)
        {
            if (IsJpeg(imageData))
            {
                return ".jpg";
            }
            else if (IsPng(imageData))
            {
                return ".png";
            }
            else if (IsGif(imageData))
            {
                return ".gif";
            }
            else
            {
                // Default to an empty string or handle unrecognized formats as needed
                return string.Empty;
            }
        }

        private bool IsJpeg(byte[] imageData)
        {
            // JPEG signature: 0xFFD8
            return imageData.Length >= 2 && imageData[0] == 0xFF && imageData[1] == 0xD8;
        }

        private bool IsPng(byte[] imageData)
        {
            // PNG signature: 0x89504E470D0A1A0A
            return imageData.Length >= 8 &&
                   imageData[0] == 0x89 &&
                   imageData[1] == 0x50 &&
                   imageData[2] == 0x4E &&
                   imageData[3] == 0x47 &&
                   imageData[4] == 0x0D &&
                   imageData[5] == 0x0A &&
                   imageData[6] == 0x1A &&
                   imageData[7] == 0x0A;
        }

        private bool IsGif(byte[] imageData)
        {
            // GIF signature: "GIF87a" or "GIF89a"
            return imageData.Length >= 6 &&
                   (Encoding.ASCII.GetString(imageData, 0, 6) == "GIF87a" ||
                    Encoding.ASCII.GetString(imageData, 0, 6) == "GIF89a");
        }
    }



}
