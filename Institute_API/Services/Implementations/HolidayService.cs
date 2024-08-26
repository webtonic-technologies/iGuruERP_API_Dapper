using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;
using Institute_API.Repository.Interfaces;
using Institute_API.Services.Interfaces;

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
    }
}
