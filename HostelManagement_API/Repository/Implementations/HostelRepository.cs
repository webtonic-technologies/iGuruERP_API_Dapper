using Dapper;
using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.Repository.Interfaces;
using System.Data;
using System.Data.SqlClient;
using HostelManagement_API.DTOs.ServiceResponse;

namespace HostelManagement_API.Repository.Implementations
{
    public class HostelRepository : IHostelRepository
    {
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HostelRepository(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _hostingEnvironment = hostingEnvironment;
        }


        public async Task<int> AddUpdateHostel(AddUpdateHostelRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        string sqlQuery = request.HostelID == 0
                            ? @"INSERT INTO tblHostel (HostelName, HostelTypeID, HostelPhoneNo, HostelWardenID, Address, InstituteID, IsActive) 
                                VALUES (@HostelName, @HostelTypeID, @HostelPhoneNo, @HostelWardenID, @Address, @InstituteID, @IsActive); 
                                SELECT CAST(SCOPE_IDENTITY() as int)"
                            : @"UPDATE tblHostel 
                                SET HostelName = @HostelName, HostelTypeID = @HostelTypeID, HostelPhoneNo = @HostelPhoneNo, 
                                    HostelWardenID = @HostelWardenID, Address = @Address, InstituteID = @InstituteID, IsActive = @IsActive WHERE HostelID = @HostelID";

                        var hostelId = request.HostelID == 0
                            ? await db.ExecuteScalarAsync<int>(sqlQuery, new { request.HostelName, request.HostelTypeID, request.HostelPhoneNo, request.HostelWardenID, request.Address, request.InstituteID, request.IsActive }, transaction)
                            : await db.ExecuteAsync(sqlQuery, new { request.HostelName, request.HostelTypeID, request.HostelPhoneNo, request.HostelWardenID, request.Address, request.InstituteID, request.IsActive, request.HostelID }, transaction);

                        if (request.HostelID == 0)
                        {
                            hostelId = hostelId;
                        }
                        else
                        {
                            hostelId = (int)request.HostelID;
                        }

                        // Handle Block Mappings
                        await db.ExecuteAsync(@"DELETE FROM tblHostelBlockMapping WHERE HostelID = @HostelID", new { HostelID = hostelId }, transaction);
                        if (request.BlockIDs != null)
                        {
                            foreach (var blockId in request.BlockIDs)
                            {
                                await db.ExecuteAsync(@"INSERT INTO tblHostelBlockMapping (HostelID, BlockID) VALUES (@HostelID, @BlockID)", new { HostelID = hostelId, BlockID = blockId }, transaction);
                            }
                        }

                        // Handle Building Mappings
                        await db.ExecuteAsync(@"DELETE FROM tblHostelBuildingMapping WHERE HostelID = @HostelID", new { HostelID = hostelId }, transaction);
                        if (request.BuildingIDs != null)
                        {
                            foreach (var buildingId in request.BuildingIDs)
                            {
                                await db.ExecuteAsync(@"INSERT INTO tblHostelBuildingMapping (HostelID, BuildingID) VALUES (@HostelID, @BuildingID)", new { HostelID = hostelId, BuildingID = buildingId }, transaction);
                            }
                        }

                        // Handle Floor Mappings
                        await db.ExecuteAsync(@"DELETE FROM tblHostelFloorMapping WHERE HostelID = @HostelID", new { HostelID = hostelId }, transaction);
                        if (request.FloorIDs != null)
                        {
                            foreach (var floorId in request.FloorIDs)
                            {
                                await db.ExecuteAsync(@"INSERT INTO tblHostelFloorMapping (HostelID, FloorID) VALUES (@HostelID, @FloorID)", new { HostelID = hostelId, FloorID = floorId }, transaction);
                            }
                        }
                        var doc = await AddUpdateHostelDocuments(request.HostelDocs, hostelId);
                        transaction.Commit();

                        return hostelId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public async Task<ServiceResponse<IEnumerable<HostelResponse>>> GetAllHostels(GetAllHostelsRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                // Count the total number of records
                string countQuery = @"SELECT COUNT(*) FROM tblHostel WHERE InstituteID = @InstituteID";
                int totalCount = await db.ExecuteScalarAsync<int>(countQuery, new { request.InstituteID });

                // Query to retrieve hostel details
                string sqlQuery = @"
                SELECT h.HostelID, 
                       h.HostelName, 
                       h.HostelTypeID, 
                       ht.HostelType, 
                       h.Address, 
                       h.HostelPhoneNo AS PhoneNo, 
                       e.First_Name AS HostelWarden, 
                       Blocks.Block AS Block, 
                       Building.Building AS Building, 
                       Floors.Floor AS Floors
                FROM tblHostel h
                LEFT JOIN tblHostelType ht ON h.HostelTypeID = ht.HostelTypeID
                LEFT JOIN tbl_EmployeeProfileMaster e ON h.HostelWardenID = e.Employee_id
                LEFT JOIN (
                    SELECT hbm.HostelID, STRING_AGG(b.BlockName, ', ') AS Block
                    FROM tblHostelBlockMapping hbm
                    JOIN tblBlock b ON hbm.BlockID = b.BlockID
                    GROUP BY hbm.HostelID
                ) Blocks ON h.HostelID = Blocks.HostelID
                LEFT JOIN (
                    SELECT hbm.HostelID, STRING_AGG(bl.BuildingName, ', ') AS Building
                    FROM tblHostelBuildingMapping hbm
                    JOIN tblBuilding bl ON hbm.BuildingID = bl.BuildingID
                    GROUP BY hbm.HostelID
                ) Building ON h.HostelID = Building.HostelID
                LEFT JOIN (
                    SELECT hfm.HostelID, STRING_AGG(f.FloorName + ' – ' + bl.BuildingName, ', ') AS Floor
                    FROM tblHostelFloorMapping hfm
                    JOIN tblBuildingFloors f ON hfm.FloorID = f.FloorID
                    JOIN tblBuilding bl ON f.BuildingID = bl.BuildingID
                    GROUP BY hfm.HostelID
                ) Floors ON h.HostelID = Floors.HostelID
                WHERE h.InstituteID = @InstituteID
                ORDER BY h.HostelName
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                // Fetch the list of hostels
                var hostels = await db.QueryAsync<HostelResponse>(sqlQuery, new
                {
                    request.InstituteID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                // For each hostel, fetch its documents
                foreach (var data in hostels)
                {
                    data.HostelDocs = await GetHostelDocuments(data.HostelID);
                }

                // Return the paginated result wrapped in ServiceResponse
                return new ServiceResponse<IEnumerable<HostelResponse>>(true, "Hostels Retrieved Successfully", hostels, 200, totalCount);
            }
        }

        //public async Task<HostelResponse> GetHostelById(int hostelId)
        //{
        //    using (IDbConnection db = new SqlConnection(_connectionString))
        //    {
        //        string sqlQuery = @"SELECT * FROM tblHostel WHERE HostelID = @HostelID";
        //        var data = await db.QueryFirstOrDefaultAsync<HostelResponse>(sqlQuery, new { HostelID = hostelId });
        //        if (data != null)
        //        {
        //            data.HostelDocs = await GetHostelDocuments(hostelId);
        //            return data;
        //        }
        //        else
        //        {
        //            return data;
        //        }
        //    }
        //}


        public async Task<HostelResponse> GetHostelById(int hostelId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                // SQL query to get hostel details along with blocks, buildings, and floors
                string sqlQuery = @"
                SELECT h.HostelID, 
                       h.HostelName, 
                       h.HostelTypeID, 
                       ht.HostelType, 
                       h.Address, 
                       h.HostelPhoneNo AS PhoneNo, 
                       e.First_Name AS HostelWarden, 
                       Blocks.Block AS Block, 
                       Building.Building AS Building, 
                       Floors.Floor AS Floors
                FROM tblHostel h
                LEFT JOIN tblHostelType ht ON h.HostelTypeID = ht.HostelTypeID
                LEFT JOIN tbl_EmployeeProfileMaster e ON h.HostelWardenID = e.Employee_id
                LEFT JOIN (
                    SELECT hbm.HostelID, STRING_AGG(b.BlockName, ', ') AS Block
                    FROM tblHostelBlockMapping hbm
                    JOIN tblBlock b ON hbm.BlockID = b.BlockID
                    GROUP BY hbm.HostelID
                ) Blocks ON h.HostelID = Blocks.HostelID
                LEFT JOIN (
                    SELECT hbm.HostelID, STRING_AGG(bl.BuildingName, ', ') AS Building
                    FROM tblHostelBuildingMapping hbm
                    JOIN tblBuilding bl ON hbm.BuildingID = bl.BuildingID
                    GROUP BY hbm.HostelID
                ) Building ON h.HostelID = Building.HostelID
                LEFT JOIN (
                    SELECT hfm.HostelID, STRING_AGG(f.FloorName + ' – ' + bl.BuildingName, ', ') AS Floor
                    FROM tblHostelFloorMapping hfm
                    JOIN tblBuildingFloors f ON hfm.FloorID = f.FloorID
                    JOIN tblBuilding bl ON f.BuildingID = bl.BuildingID
                    GROUP BY hfm.HostelID
                ) Floors ON h.HostelID = Floors.HostelID
                WHERE h.HostelID = @HostelID";

                // Query the database for hostel details by HostelID
                var data = await db.QueryFirstOrDefaultAsync<HostelResponse>(sqlQuery, new { HostelID = hostelId });

                // If data is found, fetch the documents associated with the hostel
                if (data != null)
                {
                    data.HostelDocs = await GetHostelDocuments(hostelId);
                    return data;
                }
                else
                {
                    return null; // Return null if no data is found
                }
            }
        }


        public async Task<int> DeleteHostel(int hostelId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"UPDATE tblHostel SET IsActive = 0 WHERE HostelID = @HostelID";
                return await db.ExecuteAsync(sqlQuery, new { HostelID = hostelId });
            }
        }
        public async Task<ServiceResponse<string>> DeleteHostelDocuments(int documentIds)
        {
            if (documentIds == 0)
            {
                return new ServiceResponse<string>(false, "No document IDs provided.", string.Empty, 400);
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string deleteSql = @"
                DELETE FROM tblHostelDocuments
                WHERE DocumentsId = @DocumentIds";

                        var parameters = new { DocumentIds = documentIds };

                        var affectedRows = await connection.ExecuteAsync(deleteSql, parameters, transaction);

                        transaction.Commit();

                        if (affectedRows > 0)
                        {
                            return new ServiceResponse<string>(true, "Documents deleted successfully.", string.Empty, 200);
                        }
                        else
                        {
                            return new ServiceResponse<string>(false, "No documents found to delete.", string.Empty, 404);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
                    }
                }
            }
        }
        private async Task<int> AddUpdateHostelDocuments(List<HostelDocs>? request, int hostelId)
        {
            if (request == null || !request.Any())
            {
                return 0; // No documents to process
            }

            using (var connection = new SqlConnection(_connectionString)) 
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Delete existing documents for the specified hostel
                        string deleteSql = @"
                DELETE FROM tblHostelDocuments
                WHERE HostelId = @HostelId";

                        await connection.ExecuteAsync(deleteSql, new { HostelId = hostelId }, transaction);

                        // Insert new documents from the request
                        string insertSql = @"
                INSERT INTO tblHostelDocuments (HostelId, DocFile)
                VALUES (@HostelId, @DocFile)";

                        foreach (var doc in request)
                        {
                            await connection.ExecuteAsync(insertSql, new { HostelId = hostelId, DocFile = ImageUpload(doc.DocFile) }, transaction);
                        }

                        transaction.Commit();
                        return request.Count; // Return the number of documents added
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw; // Rethrow the exception to handle it upstream
                    }
                }
            }
        }
        private async Task<List<HostelDocs>> GetHostelDocuments(int HostelId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = @"
        SELECT DocumentID, HostelId, DocFile
        FROM tblHostelDocuments
        WHERE HostelId = @HostelId";

                var parameters = new { HostelId };

                var documents = await connection.QueryAsync<HostelDocs>(sql, parameters);
                foreach(var data in documents)
                {
                    data.DocFile = GetImage(data.DocFile);
                }
                return documents.ToList();
            }
        }
        private string ImageUpload(string image)
        {
            if (string.IsNullOrEmpty(image) || image == "string")
            {
                return string.Empty;
            }
            byte[] imageData = Convert.FromBase64String(image);
            string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "HostelDocs");

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
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "HostelDocs", Filename);

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
