using Student_API.DTOs.RequestDTO;
using Student_API.Repository.Interfaces;
using Student_API.DTOs.ServiceResponse;
using System.Data;
using Dapper;
using Student_API.DTOs;

namespace Student_API.Repository.Implementations
{
    public class IMEIRegistrationRepo : IIMEIRegistrationRepo
    {
        private readonly IDbConnection _connection;

        public IMEIRegistrationRepo(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<string>> AddIMEIRegistration(IMEIRegistrationModel imeiRegistrationDto)
        {
            try
            {
                string query = @"
        INSERT INTO tbl_IMEIRegistration (Employee_Id, IMEI_Number, IsReset)
        VALUES (@Employee_Id, @IMEI_Number, @IsReset);
        SELECT CAST(SCOPE_IDENTITY() as int);";

                var newIMEIRegistrationId = await _connection.ExecuteScalarAsync<int>(query, new
                {
                    imeiRegistrationDto.Employee_Id,
                    imeiRegistrationDto.IMEI_Number,
                    imeiRegistrationDto.IsReset
                });

                return new ServiceResponse<string>(true, "IMEI registration added successfully", newIMEIRegistrationId.ToString(), 201);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<List<IMEIRegistrationDto>>> GetAllIMEIRegistrations()
        {
            try
            {
                string query = @"
        SELECT IR.IMEIRegistration_Id, IR.Employee_Id, EPM.First_name + ' ' + EPM.Last_name AS Employee_Name, 
               EPM.Mobile_Number, IR.IMEI_Number, IR.IsReset
        FROM tbl_IMEIRegistration IR
        JOIN tbl_EmployeeProfileMaster EPM ON IR.Employee_Id = EPM.Employee_Id";

                var imeiRegistrations = (await _connection.QueryAsync<IMEIRegistrationDto>(query)).ToList();

                return new ServiceResponse<List<IMEIRegistrationDto>>(true, "IMEI registrations retrieved successfully", imeiRegistrations, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<IMEIRegistrationDto>>(false, ex.Message, null, 500);
            }
        }

    }
}
