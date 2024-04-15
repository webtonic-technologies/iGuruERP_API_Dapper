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
                                FileName = string.Empty,
                                HouseColor = data.HouseColor,
                                HouseName = data.HouseName,
                                Institute_id = request.Institute_id
                            };
                            houses.Add(newHouse);
                        }
                        string insertQuery = @"INSERT INTO [dbo].[tbl_InstituteHouse] (Institute_id, HouseName, HouseColor, FileName)
                       VALUES (@Institute_id, @HouseName, @HouseColor, @FileName);
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
                            FileName = string.Empty,
                            HouseColor = data.HouseColor,
                            HouseName = data.HouseName,
                            Institute_id = request.Institute_id
                        };
                        houses.Add(newHouse);
                    }
                    string insertQuery = @"INSERT INTO [dbo].[tbl_InstituteHouse] (Institute_id, HouseName, HouseColor, FileName)
                       VALUES (@Institute_id, @HouseName, @HouseColor, @FileName);
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

        public async Task<ServiceResponse<InstituteHouseDTO>> GetInstituteHouseList(int Id)
        {
            try
            {
                var response = new InstituteHouseDTO();
                string sql = @"SELECT Institute_house_id, Institute_id, HouseName, HouseColor
                       FROM [dbo].[tbl_InstituteHouse]
                       WHERE Institute_id = @Id";
                var instituteHouse = await _connection.QueryAsync<InstituteHouses>(sql, new { Id });
                if (instituteHouse != null)
                {
                    response.Institute_id = Id;
                    response.InstituteHouses = instituteHouse.AsList();
                    return new ServiceResponse<InstituteHouseDTO>(true, "record found", response, 500);
                }
                else
                {
                    return new ServiceResponse<InstituteHouseDTO>(false, "record not found", new InstituteHouseDTO(), 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<InstituteHouseDTO>(false, ex.Message, new InstituteHouseDTO(), 500);
            }
        }
        public async Task<ServiceResponse<string>> AddUpdateHouseFile(HoueseFile request)
        {
            try
            {
                string sql = @"UPDATE [dbo].[tbl_InstituteHouse]
                       SET FileName = @FileName
                       WHERE Institute_house_id = @Institute_house_id";
                string FileName = request.FileName != null ? await HandleImageUpload(request.FileName) : string.Empty;
                int rowsAffected = await _connection.ExecuteAsync(sql, new { FileName,request.Institute_house_id });
                if (rowsAffected > 0)
                {
                    return new ServiceResponse<string>(true, "Operation successsful", "File added successfully", 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Operation failed", string.Empty, 500);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }
        public async Task<ServiceResponse<byte[]>> GetInstituteHouseLogoById(int Id)
        {

            try
            {
                var data = await _connection.QueryFirstOrDefaultAsync<InstituteHouse>(
                   "SELECT FileName FROM tbl_InstituteHouse WHERE Institute_house_id = @Institute_house_id",
                   new { Institute_house_id = Id }) ?? throw new Exception("Data not found");
                var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "Institution", data.FileName);

                if (!File.Exists(filePath))
                    throw new Exception("File not found");
                var fileBytes = await File.ReadAllBytesAsync(filePath);

                return new ServiceResponse<byte[]>(true, "Record Found", fileBytes, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, ex.Message, [], 500);
            }
        }
        private async Task<string> HandleImageUpload(IFormFile request)
        {
            if (request != null)
            {
                var uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "Institution");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                var fileName = Path.GetFileNameWithoutExtension(request.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(request.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await request.CopyToAsync(fileStream);
                }
                return fileName;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
