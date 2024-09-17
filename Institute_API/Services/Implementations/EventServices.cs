using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;
using Institute_API.Services.Interfaces;
using Institute_API.Repository.Interfaces;
using OfficeOpenXml;

namespace Institute_API.Services.Implementations
{
    public class EventServices : IEventServices
    {
        private readonly IEventRepository _eventRepository;
        private readonly IImageService _imageService;
        public EventServices(IEventRepository eventRepository, IImageService imageService)
        {
            _eventRepository = eventRepository;
            _imageService = imageService;
        }
        public async Task<ServiceResponse<int>> AddUpdateEvent(EventRequestDTO eventDto)
        {
            try
            {
                  List<string>  strings = new List<string>();
                foreach (var item in eventDto.AttachmentFile)
                {
                    if (item != null && item != "")
                    {
                        if (!_imageService.IsValidFileFormat(item))
                        {
                            return new ServiceResponse<int>(false, "Unsupported file format. Only JPG, PNG, GIF, and PDF are allowed.", 0, 400);
                        }
                        var file = await _imageService.SaveImageAsync(item, "Event");
                        if (eventDto.Event_id != 0)
                        {
                            var data = await _eventRepository.GetEventAttachmentFileById(eventDto.Event_id);
                            if (data.Data != null)
                            {
                                _imageService.DeleteFile(data.Data);
                            }
                        }
                        strings.Add(file.relativePath);

                    }
                }
                //_imageService.SaveImageAsync();
                eventDto.AttachmentFile = strings;  
                return await _eventRepository.AddUpdateEvent(eventDto);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteEvent(int eventId)
        {
            try
            {
                return await _eventRepository.DeleteEvent(eventId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
        public async Task<ServiceResponse<bool>> ToggleEventActiveStatus(int eventId, int Status, int userId)
        {
            try
            {
                return await _eventRepository.ToggleEventActiveStatus(eventId, Status, userId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
        public async Task<ServiceResponse<List<EventDTO>>> GetApprovedEvents(CommonRequestDTO commonRequest)
        {
            try
            {
                var data = await _eventRepository.GetApprovedEvents(commonRequest.Institute_id, commonRequest.Academic_year_id, commonRequest.Status, commonRequest.sortColumn, commonRequest.sortDirection, commonRequest.pageSize, commonRequest.pageNumber);
                foreach (var eventDto in data.Data)
                {
                    if (eventDto != null && eventDto.AttachmentFile != null && eventDto.AttachmentFile != "")
                    {
                        eventDto.AttachmentFile = _imageService.GetImageAsBase64(eventDto.AttachmentFile);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EventDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<EventDTO>>> GetAllEvents(CommonRequestDTO commonRequest)
        {
            try
            {
                var data = await _eventRepository.GetAllEvents(commonRequest.Institute_id, commonRequest.Academic_year_id, commonRequest.sortColumn, commonRequest.sortDirection, commonRequest.pageSize, commonRequest.pageNumber);
                foreach (var eventDto in data.Data)
                {
                    if (eventDto != null && eventDto.AttachmentFile != null && eventDto.AttachmentFile != "")
                    {
                        eventDto.AttachmentFile = _imageService.GetImageAsBase64(eventDto.AttachmentFile);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<EventDTO>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<EventDTO>> GetEventById(int eventId)
        {
            try
            {
                var data = await _eventRepository.GetEventById(eventId);
                if (data.Data != null && data.Data.AttachmentFile != null && data.Data.AttachmentFile != "")
                {
                    data.Data.AttachmentFile = _imageService.GetImageAsBase64(data.Data.AttachmentFile);
                }
                return data;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EventDTO>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<string>> ExportApprovedEventsToExcel(CommonExportRequest commonRequest)
        {
            try
            {
                // Fetch approved events from the repository
                var eventsResponse = await _eventRepository.GetApprovedEvents(commonRequest.Institute_id, commonRequest.Academic_year_id, commonRequest.Status, "", "", int.MaxValue, 1);

                // Check if events were retrieved successfully
                if (!eventsResponse.Success)
                {
                    return new ServiceResponse<string>(false, "No approved events found", null, 404);
                }

                var events = eventsResponse.Data;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Create an Excel package using EPPlus
                using (var package = new ExcelPackage())
                {
                    // Add a worksheet
                    var worksheet = package.Workbook.Worksheets.Add("ApprovedEvents");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Event ID";
                    worksheet.Cells[1, 2].Value = "Event Name";
                    worksheet.Cells[1, 3].Value = "Start Date";
                    worksheet.Cells[1, 4].Value = "End Date";
                    worksheet.Cells[1, 5].Value = "Description";
                    worksheet.Cells[1, 6].Value = "Location";
                    worksheet.Cells[1, 7].Value = "Attachment File";

                    // Add data rows
                    var rowIndex = 2; // Start from row 2 as row 1 contains headers
                    foreach (var eventDto in events)
                    {
                        worksheet.Cells[rowIndex, 1].Value = eventDto.Event_id;
                        worksheet.Cells[rowIndex, 2].Value = eventDto.EventName;
                        worksheet.Cells[rowIndex, 3].Value = eventDto.StartDate;
                        worksheet.Cells[rowIndex, 4].Value = eventDto.EndDate;
                        worksheet.Cells[rowIndex, 5].Value = eventDto.Description;
                        worksheet.Cells[rowIndex, 6].Value = eventDto.Location;
                        worksheet.Cells[rowIndex, 7].Value = eventDto.AttachmentFile;
                        rowIndex++;
                    }

                    // Auto-fit columns for better readability
                    worksheet.Cells.AutoFitColumns();

                    // Generate the Excel file as a byte array
                    var excelFile = package.GetAsByteArray();

                    // Save the file to a specific location or return the file content as a downloadable response
                    var fileName = $"ApprovedEvents_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports", fileName);

                    // Ensure the directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    // Write file to disk
                    await File.WriteAllBytesAsync(filePath, excelFile);

                    // Return the file path as a response
                    return new ServiceResponse<string>(true, "Excel file generated successfully", filePath, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<string>> ExportAllEventsToExcel(CommonExportRequest commonRequest)
        {
            try
            {
                // Fetch all events from the repository
                var eventsResponse = await _eventRepository.GetAllEvents(commonRequest.Institute_id, commonRequest.Academic_year_id, "", "", int.MaxValue, 1);

                // Check if events were retrieved successfully
                if (!eventsResponse.Success)
                {
                    return new ServiceResponse<string>(false, "No events found", null, 404);
                }

                var events = eventsResponse.Data;
                if (events == null || !events.Any())
                {
                    return new ServiceResponse<string>(false, "No events available to export", null, 404);
                }

                // Prepare a unique filename
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string fileName, filePath;

                if (1==1)
                {
                    // Excel file export
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage())
                    {
                        // Add a worksheet
                        var worksheet = package.Workbook.Worksheets.Add("AllEvents");

                        // Add headers
                        worksheet.Cells[1, 1].Value = "Event ID";
                        worksheet.Cells[1, 2].Value = "Event Name";
                        worksheet.Cells[1, 3].Value = "Start Date";
                        worksheet.Cells[1, 4].Value = "End Date";
                        worksheet.Cells[1, 5].Value = "Description";
                        worksheet.Cells[1, 6].Value = "Location";
                        //worksheet.Cells[1, 7].Value = "Attachment File";

                        // Add data rows
                        var rowIndex = 2; // Start from row 2 as row 1 contains headers
                        foreach (var eventDto in events)
                        {
                            worksheet.Cells[rowIndex, 1].Value = eventDto.Event_id;
                            worksheet.Cells[rowIndex, 2].Value = eventDto.EventName;
                            worksheet.Cells[rowIndex, 3].Value = eventDto.StartDate;
                            worksheet.Cells[rowIndex, 4].Value = eventDto.EndDate;
                            worksheet.Cells[rowIndex, 5].Value = eventDto.Description;
                            worksheet.Cells[rowIndex, 6].Value = eventDto.Location;
                            //worksheet.Cells[rowIndex, 7].Value = eventDto.AttachmentFile;
                            rowIndex++;
                        }

                        // Auto-fit columns for better readability
                        worksheet.Cells.AutoFitColumns();

                        // Generate the Excel file as a byte array
                        var excelFile = package.GetAsByteArray();

                        // Define the file name and path
                        fileName = $"AllEvents_{timestamp}.xlsx";
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports", fileName);

                        // Ensure the directory exists
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                        // Write file to disk
                        await File.WriteAllBytesAsync(filePath, excelFile);
                    }
                }
                else
                {
                    // CSV file export
                    fileName = $"AllEvents_{timestamp}.csv";
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports", fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using (var writer = new StreamWriter(filePath))
                    {
                        // Write headers
                        writer.WriteLine("Event ID,Event Name,Start Date,End Date,Description,Location");

                        // Write data rows
                        foreach (var eventDto in events)
                        {
                            writer.WriteLine($"{eventDto.Event_id},{eventDto.EventName},{eventDto.StartDate},{eventDto.EndDate},{eventDto.Description},{eventDto.Location}");
                        }
                    }
                }

                // Return the file path as a response
                return new ServiceResponse<string>(true, $"Excel file generated successfully", filePath, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }


    }
}
