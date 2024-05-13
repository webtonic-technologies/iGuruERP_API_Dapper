namespace Student_API.Services.Interfaces
{
    public interface IImageService
    {
        Task<(string fileName, string relativePath, string absolutePath)> SaveImageAsync(string image, string targetFolder);
        string GetImageAsBase64(string filename, string subFolder = null);
        void DeleteFile(string filename, string subFolder = null);
    }

}
