using Dapper;
using Institute_API.DTOs;
using Institute_API.DTOs.ServiceResponse;
using Institute_API.Models;
using Institute_API.Repository.Interfaces;
using System.Data;

namespace Institute_API.Repository.Implementations
{
    public class InstituteHouseRepository : IInstituteHouseRepository
    {

        private readonly IDbConnection _connection;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public InstituteHouseRepository(IDbConnection connection, IWebHostEnvironment hostingEnvironment)
        {
            _connection = connection;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<ServiceResponse<int>> AddUpdateInstituteHouse(InstituteHouseDTO request)
        {
            try
            {
                int addedRecords = 0;
                if (request.InstituteHouses != null)
                {
                    foreach (var data in request.InstituteHouses)
                    {
                        data.Institute_id = request.Institute_id;
                    }
                }
                string query = "SELECT COUNT(*) FROM tbl_InstituteHouse WHERE Institute_id = @Institute_id";
                int count = await _connection.ExecuteScalarAsync<int>(query, new { request.Institute_id });
                if (count > 0)
                {
                    string deleteQuery = "DELETE FROM tbl_InstituteHouse WHERE Institute_id = @Institute_id";
                    int rowsAffected = await _connection.ExecuteAsync(deleteQuery, new { request.Institute_id });
                    if (rowsAffected > 0)
                    {
                        var houses = new List<InstituteHouse>();
                        foreach(var data in request.InstituteHouses ??= ([]))
                        {
                            var newHouse = new InstituteHouse
                            {
                                FileName = ImageUpload(data.FileName),
                                HouseColor = data.HouseColor,
                                HouseName = data.HouseName,
                                Institute_id = request.Institute_id,
                                en_date = data.en_date,
                                IsDeleted = false
                            };
                            houses.Add(newHouse);
                        }
                        string insertQuery = @"INSERT INTO [dbo].[tbl_InstituteHouse] (Institute_id, HouseName, HouseColor, FileName, en_date, IsDeleted)
                       VALUES (@Institute_id, @HouseName, @HouseColor, @FileName, @en_date, @IsDeleted);
                        SELECT SCOPE_IDENTITY();";
                        // Execute the query with multiple parameterized sets of values
                        addedRecords = await _connection.ExecuteAsync(insertQuery, houses);
                    }
                }
                else
                {
                    var houses = new List<InstituteHouse>();
                    foreach (var data in request.InstituteHouses ??= ([]))
                    {
                        var newHouse = new InstituteHouse
                        {
                            FileName = ImageUpload(data.FileName),
                            HouseColor = data.HouseColor,
                            HouseName = data.HouseName,
                            Institute_id = request.Institute_id,
                            en_date = data.en_date,
                            IsDeleted = false
                        };
                        houses.Add(newHouse);
                    }
                    string insertQuery = @"INSERT INTO [dbo].[tbl_InstituteHouse] (Institute_id, HouseName, HouseColor, FileName, en_date, IsDeleted)
                       VALUES (@Institute_id, @HouseName, @HouseColor, @FileName, @en_date, @IsDeleted);
                         SELECT SCOPE_IDENTITY();";
                    // Execute the query with multiple parameterized sets of values
                    addedRecords = await _connection.ExecuteAsync(insertQuery, houses);
                }
                if (addedRecords > 0)
                {
                    return new ServiceResponse<int>(true, "operation successful", addedRecords, 200);
                }
                else
                {
                    return new ServiceResponse<int>(false, "operation failed", 0, 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        public async Task<ServiceResponse<InstituteHouseDTO>> GetInstituteHouseList(GetInstituteHouseList request)
        {
            try
            {
                var response = new InstituteHouseDTO();
                string sql = @"SELECT Institute_house_id, Institute_id, HouseName, HouseColor, en_date, FileName, IsDeleted
                       FROM [dbo].[tbl_InstituteHouse]
                       WHERE Institute_id = @Id AND IsDeleted = 0";

                // Add search conditions if searchText is provided
                if (!string.IsNullOrEmpty(request.SearchText))
                {
                    sql += " AND (HouseName LIKE @SearchText OR HouseColor LIKE @SearchText)";
                }

                var instituteHouses = await _connection.QueryAsync<InstituteHouses>(sql, new { Id = request.InstituteID, SearchText = $"%{request.SearchText}%" });

                if (instituteHouses != null && instituteHouses.Any())
                {
                    foreach (var house in instituteHouses)
                    {
                        house.FileName = GetImage(house.FileName);
                    }
                    response.Institute_id = request.InstituteID;
                    response.InstituteHouses = instituteHouses.AsList();
                    return new ServiceResponse<InstituteHouseDTO>(true, "Records found", response, 200);
                }
                else
                {
                    return new ServiceResponse<InstituteHouseDTO>(false, "No records found", new InstituteHouseDTO(), 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<InstituteHouseDTO>(false, ex.Message, new InstituteHouseDTO(), 500);
            }
        }
        public async Task<ServiceResponse<InstituteHouseDTO>> GetInstituteHouseById(int instituteHouseId)
        {
            try
            {
                var response = new InstituteHouseDTO();
                string sql = @"SELECT Institute_house_id, Institute_id, HouseName, HouseColor, en_date, FileName
                       FROM [dbo].[tbl_InstituteHouse]
                       WHERE Institute_house_id = @InstituteHouseId AND IsDeleted = 0";

                var instituteHouse = await _connection.QueryFirstOrDefaultAsync<InstituteHouses>(sql, new { InstituteHouseId = instituteHouseId });

                if (instituteHouse != null)
                {
                    instituteHouse.FileName = GetImage(instituteHouse.FileName);
                    response.Institute_id = instituteHouse.Institute_id;
                    response.InstituteHouses = new List<InstituteHouses> { instituteHouse };
                    return new ServiceResponse<InstituteHouseDTO>(true, "Record found", response, 200);
                }
                else
                {
                    return new ServiceResponse<InstituteHouseDTO>(false, "Record not found", new InstituteHouseDTO(), 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<InstituteHouseDTO>(false, ex.Message, new InstituteHouseDTO(), 500);
            }
        }
        public async Task<ServiceResponse<bool>> SoftDeleteInstituteHouse(int instituteHouseId)
        {
            try
            {
                string sql = @"UPDATE [dbo].[tbl_InstituteHouse]
                       SET IsDeleted = 1
                       WHERE Institute_house_id = @InstituteHouseId";

                int rowsAffected = await _connection.ExecuteAsync(sql, new { InstituteHouseId = instituteHouseId });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "Record soft deleted successfully", true, 200);
                }
                else
                {
                    return new ServiceResponse<bool>(false, "Record not found or already deleted", false, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
        public async Task<ServiceResponse<bool>> DeleteInstituteHouseImage(int instituteHouseId)
        {
            try
            {
                // Query to get the image path based on the instituteHouseId
                string imagePath = await _connection.QuerySingleOrDefaultAsync<string>(
                    "SELECT FileName FROM tbl_InstituteHouse WHERE Institute_house_id = @InstituteHouseId",
                    new { InstituteHouseId = instituteHouseId });

                if (string.IsNullOrEmpty(imagePath))
                {
                    return new ServiceResponse<bool>(false, "Image not found", false, 404);
                }

                // Update the FileName column to null or empty string
                string updateQuery = "UPDATE tbl_InstituteHouse SET FileName = NULL WHERE Institute_house_id = @InstituteHouseId";
                int rowsAffected = await _connection.ExecuteAsync(updateQuery, new { InstituteHouseId = instituteHouseId });

                if (rowsAffected > 0)
                {
                    // Delete the file from the folder
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                    return new ServiceResponse<bool>(true, "Image deleted successfully", true, 200);
                }
                else
                {
                    return new ServiceResponse<bool>(false, "Image not found or delete failed", false, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
        private string ImageUpload(string image)
        {
            if (string.IsNullOrEmpty(image) || image == "string")
            {
                return string.Empty;
            }
            byte[] imageData = Convert.FromBase64String(image);
            string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "InstituteHouse");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string fileExtension = IsJpeg(imageData) == true ? ".jpg" : IsPng(imageData) == true ? ".png" : IsGif(imageData) == true ? ".gif" : string.Empty;
            string fileName = Guid.NewGuid().ToString() + fileExtension;
            string filePath = Path.Combine(directoryPath, fileName);
            if (string.IsNullOrEmpty(fileExtension))
            {
                throw new InvalidOperationException("Incorrect file uploaded");
            }
            // Write the byte array to the image file
            File.WriteAllBytes(filePath, imageData);
            return filePath;
        }
        private bool IsJpeg(byte[] bytes)
        {
            // JPEG magic number: 0xFF, 0xD8
            return bytes.Length > 1 && bytes[0] == 0xFF && bytes[1] == 0xD8;
        }
        private bool IsPng(byte[] bytes)
        {
            // PNG magic number: 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A
            return bytes.Length > 7 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47
                && bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A;
        }
        private bool IsGif(byte[] bytes)
        {
            // GIF magic number: "GIF"
            return bytes.Length > 2 && bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46;
        }
        private string GetImage(string Filename)
        {
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "InstituteHouse", Filename);

            if (!File.Exists(filePath))
            {
                return string.Empty;
            }
            byte[] fileBytes = File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(fileBytes);
            return base64String;
        }
    }
}
