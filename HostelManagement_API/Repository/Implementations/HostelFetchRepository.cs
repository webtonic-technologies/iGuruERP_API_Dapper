﻿using Dapper;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Implementations
{
    public class HostelFetchRepository : IHostelFetchRepository
    {
        private readonly string _connectionString;

        public HostelFetchRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<HostelFetchResponse>> GetAllHostels(int instituteId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                //string sqlQuery = @"
                //    SELECT 
                //        HostelID, 
                //        HostelName, 
                //        HostelTypeID, 
                //        Address, 
                //        HostelPhoneNo, 
                //        '' AS HostelWarden, 
                //        Attachments, 
                //        InstituteID, 
                //        IsActive 
                //    FROM tblHostel
                //    WHERE InstituteID = @InstituteID";

                string sqlQuery = @"
                    SELECT 
                        HostelID, 
                        HostelName
                    FROM tblHostel
                    WHERE InstituteID = @InstituteID";
                return await db.QueryAsync<HostelFetchResponse>(sqlQuery, new { InstituteID = instituteId });
            }
        }
    }
}
