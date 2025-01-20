using Dapper;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Implementations
{
    public class BedInformationRepository : IBedInformationRepository
    {
        private readonly string _connectionString;

        public BedInformationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<GetBedInformationResponse>> GetBedInformation(int instituteID, int hostelID, int roomID)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var sqlQuery = @"
                    SELECT rb.RoomBedID, rb.RoomBedName, rb.BedPosition,
                           CASE
                               WHEN hra.StudentID IS NOT NULL THEN 'Occupied'
                               ELSE 'Available'
                           END AS Availability
                    FROM tblRoomBeds rb
                    LEFT JOIN tblHostelRoomAllocation hra ON rb.RoomBedID = hra.RoomBedID
                    AND hra.IsAllocated = 1
                    LEFT JOIN tblRoom r ON rb.RoomID = r.RoomID
                    WHERE r.InstituteID = @InstituteID
                    AND r.HostelID = @HostelID
                    AND rb.RoomID = @RoomID";

                return await db.QueryAsync<GetBedInformationResponse>(sqlQuery, new { InstituteID = instituteID, HostelID = hostelID, RoomID = roomID });
            }
        }
    }
}
