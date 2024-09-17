using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;
using Institute_API.Repository.Interfaces;
using Institute_API.Services.Interfaces;
using OfficeOpenXml;
using System.IO;

namespace Institute_API.Services.Implementations
{
    public class HolidayService : IHolidayService
    {
        private readonly IHolidayRepository _holidayRepository;

        public HolidayService(IHolidayRepository holidayRepository)
        {
            _holidayRepository = holidayRepository;
        }

        public async Task<ServiceResponse<int>> AddUpdateHoliday(HolidayRequestDTO holidayDTO)
        {
            try
            {
                return await _holidayRepository.AddUpdateHoliday(holidayDTO);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<HolidayDTO>> GetHolidayById(int holidayId)
        {
            try
            {
                return await _holidayRepository.GetHolidayById(holidayId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HolidayDTO>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<HolidayDTO>>> GetAllHolidays(CommonRequestDTO commonRequest)
        {
            try
            {
                return await _holidayRepository.GetAllHolidays(commonRequest.Institute_id, commonRequest.Academic_year_id, commonRequest.sortColumn, commonRequest.sortDirection, commonRequest.pageSize, commonRequest.pageNumber);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HolidayDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<HolidayDTO>>> GetApprovedHolidays(CommonRequestDTO commonRequest)
        {
            try
            {
                return await _holidayRepository.GetApprovedHolidays(commonRequest.Institute_id, commonRequest.Academic_year_id, commonRequest.Status, commonRequest.sortColumn, commonRequest.sortDirection, commonRequest.pageSize, commonRequest.pageNumber);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HolidayDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteHoliday(int holidayId)
        {
            try
            {
                return await _holidayRepository.DeleteHoliday(holidayId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateHolidayApprovalStatus(int holidayId, int Status, int approvedBy)
        {
            try
            {
                return await _holidayRepository.UpdateHolidayApprovalStatus(holidayId, Status, approvedBy);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }

        public async Task<ServiceResponse<string>> ExportAllHolidaysToExcel(CommonRequestDTO commonRequest)
        {
            try
            {
                var holidaysResponse = await _holidayRepository.GetAllHolidays(commonRequest.Institute_id, commonRequest.Academic_year_id, commonRequest.sortColumn, commonRequest.sortDirection, commonRequest.pageSize, commonRequest.pageNumber);

                //if (!holidaysResponse.Success)
                //{
                //    return new ServiceResponse<string>(false, "No holidays found", null, 404);
                //}

                var holidays = holidaysResponse.Data;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("AllHolidays");
                    worksheet.Cells[1, 1].Value = "Holiday ID";
                    worksheet.Cells[1, 2].Value = "Holiday Name";
                    worksheet.Cells[1, 3].Value = "Start Date";
                    worksheet.Cells[1, 4].Value = "End Date";
                    worksheet.Cells[1, 5].Value = "Description";

                    var rowIndex = 2;
                    foreach (var holiday in holidays)
                    {
                        worksheet.Cells[rowIndex, 1].Value = holiday.Holiday_id;
                        worksheet.Cells[rowIndex, 2].Value = holiday.HolidayName;
                        worksheet.Cells[rowIndex, 3].Value = holiday.StartDate;
                        worksheet.Cells[rowIndex, 4].Value = holiday.EndDate;
                        worksheet.Cells[rowIndex, 5].Value = holiday.Description;
                        rowIndex++;
                    }

                    worksheet.Cells.AutoFitColumns();
                    var excelFile = package.GetAsByteArray();
                    var fileName = $"AllHolidays_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    await File.WriteAllBytesAsync(filePath, excelFile);

                    return new ServiceResponse<string>(true, "Excel file generated successfully", filePath, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<string>> ExportApprovedHolidaysToExcel(CommonRequestDTO commonRequest)
        {
            try
            {
                var holidaysResponse = await _holidayRepository.GetApprovedHolidays(commonRequest.Institute_id, commonRequest.Academic_year_id, commonRequest.Status, commonRequest.sortColumn, commonRequest.sortDirection, commonRequest.pageSize, commonRequest.pageNumber);

                //if (!holidaysResponse.Success)
                //{
                //    return new ServiceResponse<string>(false, "No approved holidays found", null, 404);
                //}

                var holidays = holidaysResponse.Data;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("ApprovedHolidays");
                    worksheet.Cells[1, 1].Value = "Holiday ID";
                    worksheet.Cells[1, 2].Value = "Holiday Name";
                    worksheet.Cells[1, 3].Value = "Start Date";
                    worksheet.Cells[1, 4].Value = "End Date";
                    worksheet.Cells[1, 5].Value = "Description";

                    var rowIndex = 2;
                    foreach (var holiday in holidays)
                    {
                        worksheet.Cells[rowIndex, 1].Value = holiday.Holiday_id;
                        worksheet.Cells[rowIndex, 2].Value = holiday.HolidayName;
                        worksheet.Cells[rowIndex, 3].Value = holiday.StartDate;
                        worksheet.Cells[rowIndex, 4].Value = holiday.EndDate;
                        worksheet.Cells[rowIndex, 5].Value = holiday.Description;
                        rowIndex++;
                    }

                    worksheet.Cells.AutoFitColumns();
                    var excelFile = package.GetAsByteArray();
                    var fileName = $"ApprovedHolidays_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    await File.WriteAllBytesAsync(filePath, excelFile);

                    return new ServiceResponse<string>(true, "Excel file generated successfully", filePath, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }
    }
}
