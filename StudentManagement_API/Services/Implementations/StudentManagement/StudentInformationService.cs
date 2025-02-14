using System.Threading.Tasks;
using OfficeOpenXml;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;
using StudentManagement_API.Repository.Interfaces;
using StudentManagement_API.Services.Interfaces;

namespace StudentManagement_API.Services.Implementations
{
    public class StudentInformationService : IStudentInformationService
    {
        private readonly IStudentInformationRepository _studentInformationRepository;

        public StudentInformationService(IStudentInformationRepository studentInformationRepository)
        {
            _studentInformationRepository = studentInformationRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateStudent(AddUpdateStudentRequest request)
        {
            // You can add any additional business logic or validations here.
            return await _studentInformationRepository.AddUpdateStudent(request);
        }

        public async Task<ServiceResponse<IEnumerable<GetStudentInformationResponse>>> GetStudentInformation(GetStudentInformationRequest request)
        {
            return await _studentInformationRepository.GetStudentInformation(request);
        }

        public async Task<ServiceResponse<string>> SetStudentStatusActivity(SetStudentStatusActivityRequest request)
        {
            return await _studentInformationRepository.SetStudentStatusActivity(request);
        }

        public async Task<ServiceResponse<IEnumerable<GetStudentStatusActivityResponse>>> GetStudentStatusActivity(GetStudentStatusActivityRequest request)
        {
            return await _studentInformationRepository.GetStudentStatusActivity(request);
        }

        public async Task<ServiceResponse<byte[]>> DownloadStudentImportTemplate(int instituteID)
        {
            try
            {
                var masterData = await _studentInformationRepository.GetMasterTablesData(instituteID);

                using (var package = new ExcelPackage())
                {
                    // ✅ 1️⃣ Create "Student Data Entry" Sheet
                    var studentSheet = package.Workbook.Worksheets.Add("Student Data Entry");
                    studentSheet.Cells["A1"].Value = "Student Import Template";
                    studentSheet.Cells["A1"].Style.Font.Bold = true;
                    studentSheet.Cells["A1"].Style.Font.Size = 14;
                    studentSheet.Cells["A3"].Value = "Institute ID:";
                    studentSheet.Cells["B3"].Value = instituteID;

                    // ✅ 2️⃣ Define columns based on the JSON Request Body
                    var headers = new List<string>
                    {
                        "Student ID",
                        "First Name", "Middle Name", "Last Name", "Gender ID", "Class ID", "Section ID",
                        "Admission Number", "Roll Number", "Date of Joining", "Academic Year", "Nationality ID",
                        "Religion ID", "Date of Birth", "Mother Tongue ID", "Caste ID", "Blood Group ID", "Aadhar No",
                        "PEN", "Student Type ID", "Student House ID",

                        "Registration Date", "Registration No", "Admission Date", "Samagra ID",
                        "Place of Birth", "Email ID", "Language Known", "Comments",
                        "Identification Mark 1", "Identification Mark 2",

                        "Parent First Name", "Parent Middle Name", "Parent Last Name",
                        "Primary Contact No", "Bank Account No", "Bank IFSC Code",
                        "Family Ration Card Type", "Family Ration Card No", "Parent Date of Birth",
                        "Parent Aadhar No", "Parent PAN Card No", "Occupation",
                        "Designation", "Name of Employer", "Office No", "Parent Email ID",
                        "Annual Income", "Residential Address",

                        "Sibling First Name", "Sibling Middle Name", "Sibling Last Name",
                        "Sibling Admission No", "Sibling Date of Birth", "Sibling Class",
                        "Sibling Section", "Sibling Institute Name", "Sibling Aadhar No",

                        "Previous School Name", "Previous Board", "Medium", "Previous School Address",
                        "Previous Course", "Previous Class", "TC Number", "TC Date", "Is TC Submitted",

                        "Allergies", "Medications", "Doctor Name", "Doctor Contact",
                        "Height", "Weight", "Government Health ID", "Vision", "Hearing", "Speech",
                        "Behavioral Problems", "Chest Condition", "History of Accidents",
                        "Physical Deformities", "Major Illness History", "Other Remarks/Weakness"
                    };

                    // ✅ 3️⃣ Write Column Headers
                    for (int i = 0; i < headers.Count; i++)
                    {
                        studentSheet.Cells[5, i + 1].Value = headers[i];
                        studentSheet.Cells[5, i + 1].Style.Font.Bold = true;
                        studentSheet.Cells[5, i + 1].AutoFitColumns();
                    }

                    // ✅ 4️⃣ Add Master Data Sheets (Start from Sheet 2)
                    AddMasterSheet(package, "Classes", masterData.Classes);
                    AddMasterSheet(package, "Sections", masterData.Sections);
                    AddMasterSheet(package, "Genders", masterData.Genders);
                    AddMasterSheet(package, "Religions", masterData.Religions);
                    AddMasterSheet(package, "Nationalities", masterData.Nationalities);
                    AddMasterSheet(package, "MotherTongues", masterData.MotherTongues);
                    AddMasterSheet(package, "BloodGroups", masterData.BloodGroups);
                    AddMasterSheet(package, "Castes", masterData.Castes);
                    AddMasterSheet(package, "InstituteHouses", masterData.InstituteHouses);
                    AddMasterSheet(package, "StudentTypes", masterData.StudentTypes);
                    AddMasterSheet(package, "ParentTypes", masterData.ParentTypes);

                    // ✅ 5️⃣ Convert to byte array and return as response
                    var fileBytes = package.GetAsByteArray();
                    return new ServiceResponse<byte[]>(true, "Template Generated Successfully", fileBytes, 200);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<byte[]>(false, $"Error generating template: {ex.Message}", null, 500);
            }
        }

        /// <summary>
        /// ✅ Helper function to create a master data sheet in Excel
        /// </summary>
        private void AddMasterSheet<T>(ExcelPackage package, string sheetName, List<T> data)
        {
            if (data == null || !data.Any()) return;

            var worksheet = package.Workbook.Worksheets.Add(sheetName);
            worksheet.Cells["A1"].Value = sheetName;
            worksheet.Cells["A1"].Style.Font.Bold = true;

            // Convert list to table
            worksheet.Cells["A2"].LoadFromCollection(data, true);
        }


        public async Task<ServiceResponse<string>> ImportStudentInformation(Stream fileStream)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(fileStream))
                {
                    var worksheet = package.Workbook.Worksheets["Student Data Entry"];
                    if (worksheet == null)
                        return new ServiceResponse<string>(false, "Invalid Excel format. Missing 'Student Data Entry' sheet.", null, 400);

                    var students = new List<StudentInformationImportRequest>();

                    int row = 2;
                    while (worksheet.Cells[row, 1].Value != null)
                    {
                        var student = new StudentInformationImportRequest
                        {
                            StudentDetails = new StudentDetails_IM
                            {
                                FirstName = worksheet.Cells[row, 1].Text,
                                MiddleName = worksheet.Cells[row, 2].Text,
                                LastName = worksheet.Cells[row, 3].Text,
                                GenderID = Convert.ToInt32(worksheet.Cells[row, 4].Text),
                                ClassID = Convert.ToInt32(worksheet.Cells[row, 5].Text),
                                SectionID = Convert.ToInt32(worksheet.Cells[row, 6].Text),
                                AdmissionNumber = worksheet.Cells[row, 7].Text,
                                RollNumber = worksheet.Cells[row, 8].Text,
                                DateOfJoining = worksheet.Cells[row, 9].Text,
                                AcademicYear = worksheet.Cells[row, 10].Text,
                                NationalityID = Convert.ToInt32(worksheet.Cells[row, 11].Text),
                                ReligionID = Convert.ToInt32(worksheet.Cells[row, 12].Text),
                                DateOfBirth = worksheet.Cells[row, 13].Text,
                                MotherTongueID = Convert.ToInt32(worksheet.Cells[row, 14].Text),
                                CasteID = Convert.ToInt32(worksheet.Cells[row, 15].Text),
                                BloodGroupID = Convert.ToInt32(worksheet.Cells[row, 16].Text),
                                AadharNo = worksheet.Cells[row, 17].Text,
                                PEN = worksheet.Cells[row, 18].Text,
                                StudentTypeID = Convert.ToInt32(worksheet.Cells[row, 19].Text),
                                StudentHouseID = Convert.ToInt32(worksheet.Cells[row, 20].Text)
                            }
                        };

                        students.Add(student);
                        row++;
                    }

                    return await _studentInformationRepository.InsertStudents(students);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, "Error processing file: " + ex.Message, null, 500);
            }
        }
    }
}
