using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Response.StudentManagement;
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
                    // ✅ 1️⃣ Create "Student Data Entry" Sheet (Without Template Header)
                    var studentSheet = package.Workbook.Worksheets.Add("Student Import Info");

                    var sectionsWithClass = await _studentInformationRepository.GetSectionsWithClassNames(instituteID);


                    // ✅ 2️⃣ Define column headers
                    var headers = new List<string>
                    {
                        "First Name", "Middle Name", "Last Name", "Gender", "Class Name", "Section Name",
                        "Admission Number", "Roll Number", "Date of Joining", "Academic Year", "Nationality",
                        "Religion", "Date of Birth", "Mother Tongue", "Caste", "Blood Group", "Aadhar No",
                        "PEN", "Student Type", "Student House",

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

                    // ✅ 3️⃣ Write Column Headers (Starting from row 1)
                    for (int i = 0; i < headers.Count; i++)
                    {
                        studentSheet.Cells[1, i + 1].Value = headers[i];
                        studentSheet.Cells[1, i + 1].Style.Font.Bold = true;
                        studentSheet.Cells[1, i + 1].AutoFitColumns();
                    }

                    // ✅ 4️⃣ Add Master Data Sheets (Start from Sheet 2)
                    AddMasterSheet(package, "Classes", masterData.Classes);
                    //AddMasterSheet(package, "Sections", masterData.Sections);
                    AddSectionsSheet(package, "Sections", sectionsWithClass);  
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

        private void AddMasterSheet<T>(ExcelPackage package, string sheetName, List<T> data)
        {
            if (data == null || !data.Any())
                return;

            var worksheet = package.Workbook.Worksheets.Add(sheetName);

            // ==============================================
            // 1) Custom logic for "Classes" sheet
            // ==============================================
            if (sheetName == "Classes")
            {
                // Write custom headers in row 1
                worksheet.Cells["A1"].Value = "ClassID";
                worksheet.Cells["B1"].Value = "Sr. No.";
                worksheet.Cells["C1"].Value = "Class Name";
                worksheet.Cells["A1:C1"].Style.Font.Bold = true;

                int row = 2;
                int srNo = 1;
                foreach (var item in data)
                {
                    // Assuming your class object has properties "Id" and "Name"
                    var idProp = item.GetType().GetProperty("Id");
                    var nameProp = item.GetType().GetProperty("Name");

                    var classID = idProp != null ? idProp.GetValue(item) : null;
                    var className = nameProp != null ? nameProp.GetValue(item) : null;

                    worksheet.Cells[row, 1].Value = classID;
                    worksheet.Cells[row, 2].Value = srNo;
                    worksheet.Cells[row, 3].Value = className;

                    row++;
                    srNo++;
                }

                // Hide the "ClassID" column (Column A)
                worksheet.Column(1).Hidden = true;

                // Left-align the "Sr. No." column (Column B)
                worksheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                // Auto-fit columns first
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set "Class Name" (Column C) to ~100px
                worksheet.Column(3).Width = 14; // ~100px
            }

            // ==============================================
            // 2) Custom logic for "Genders" sheet
            // ==============================================
            else if (sheetName == "Genders")
            {
                // Write custom headers in row 1
                worksheet.Cells["A1"].Value = "GenderID";
                worksheet.Cells["B1"].Value = "Sr. No.";
                worksheet.Cells["C1"].Value = "Gender Type";
                worksheet.Cells["A1:C1"].Style.Font.Bold = true;

                int row = 2;
                int srNo = 1;
                foreach (var item in data)
                {
                    // Assuming your class object has properties "Id" and "Name"
                    var idProp = item.GetType().GetProperty("Id");
                    var nameProp = item.GetType().GetProperty("Name");

                    var GenderID = idProp != null ? idProp.GetValue(item) : null;
                    var GenderType = nameProp != null ? nameProp.GetValue(item) : null;

                    worksheet.Cells[row, 1].Value = GenderID;
                    worksheet.Cells[row, 2].Value = srNo;
                    worksheet.Cells[row, 3].Value = GenderType;

                    row++;
                    srNo++;
                }

                // Hide the "ClassID" column (Column A)
                worksheet.Column(1).Hidden = true;

                // Left-align the "Sr. No." column (Column B)
                worksheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                // Auto-fit columns first
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set "Class Name" (Column C) to ~100px
                worksheet.Column(3).Width = 14; // ~100px
            }

            // ==============================================
            // 3) Custom logic for "Religions" sheet
            // ==============================================
            else if (sheetName == "Religions")
            {
                // Write custom headers in row 1
                worksheet.Cells["A1"].Value = "ReligionID";
                worksheet.Cells["B1"].Value = "Sr. No.";
                worksheet.Cells["C1"].Value = "Religion";
                worksheet.Cells["A1:C1"].Style.Font.Bold = true;

                int row = 2;
                int srNo = 1;
                foreach (var item in data)
                {
                    // Assuming your class object has properties "Id" and "Name"
                    var idProp = item.GetType().GetProperty("Id");
                    var nameProp = item.GetType().GetProperty("Name");

                    var ReligionID = idProp != null ? idProp.GetValue(item) : null;
                    var Religion = nameProp != null ? nameProp.GetValue(item) : null;

                    worksheet.Cells[row, 1].Value = ReligionID;
                    worksheet.Cells[row, 2].Value = srNo;
                    worksheet.Cells[row, 3].Value = Religion;

                    row++;
                    srNo++;
                }

                // Hide the "ClassID" column (Column A)
                worksheet.Column(1).Hidden = true;

                // Left-align the "Sr. No." column (Column B)
                worksheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                // Auto-fit columns first
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set "Class Name" (Column C) to ~100px
                worksheet.Column(3).Width = 14; // ~100px
            }

            // ==============================================
            // 3) Custom logic for "Nationalities" sheet
            // ==============================================
            else if (sheetName == "Nationalities")
            {
                // Write custom headers in row 1
                worksheet.Cells["A1"].Value = "NationalityID";
                worksheet.Cells["B1"].Value = "Sr. No.";
                worksheet.Cells["C1"].Value = "NationalityType";
                worksheet.Cells["A1:C1"].Style.Font.Bold = true;

                int row = 2;
                int srNo = 1;
                foreach (var item in data)
                {
                    // Assuming your class object has properties "Id" and "Name"
                    var idProp = item.GetType().GetProperty("Id");
                    var nameProp = item.GetType().GetProperty("Name");

                    var NationalityID = idProp != null ? idProp.GetValue(item) : null;
                    var NationalityType = nameProp != null ? nameProp.GetValue(item) : null;

                    worksheet.Cells[row, 1].Value = NationalityID;
                    worksheet.Cells[row, 2].Value = srNo;
                    worksheet.Cells[row, 3].Value = NationalityType;

                    row++;
                    srNo++;
                }

                // Hide the "ClassID" column (Column A)
                worksheet.Column(1).Hidden = true;

                // Left-align the "Sr. No." column (Column B)
                worksheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                // Auto-fit columns first
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set "Class Name" (Column C) to ~100px
                worksheet.Column(3).Width = 14; // ~100px
            }

            // ==============================================
            // 3) Custom logic for "Nationalities" sheet
            // ==============================================
            else if (sheetName == "MotherTongues")
            {
                // Write custom headers in row 1
                worksheet.Cells["A1"].Value = "MotherTongueID";
                worksheet.Cells["B1"].Value = "Sr. No.";
                worksheet.Cells["C1"].Value = "Mother Tongue";
                worksheet.Cells["A1:C1"].Style.Font.Bold = true;

                int row = 2;
                int srNo = 1;
                foreach (var item in data)
                {
                    // Assuming your class object has properties "Id" and "Name"
                    var idProp = item.GetType().GetProperty("Id");
                    var nameProp = item.GetType().GetProperty("Name");

                    var MotherTongueID = idProp != null ? idProp.GetValue(item) : null;
                    var MotherTongue = nameProp != null ? nameProp.GetValue(item) : null;

                    worksheet.Cells[row, 1].Value = MotherTongueID;
                    worksheet.Cells[row, 2].Value = srNo;
                    worksheet.Cells[row, 3].Value = MotherTongue;

                    row++;
                    srNo++;
                }

                // Hide the "ClassID" column (Column A)
                worksheet.Column(1).Hidden = true;

                // Left-align the "Sr. No." column (Column B)
                worksheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                // Auto-fit columns first
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set "Class Name" (Column C) to ~100px
                worksheet.Column(3).Width = 14; // ~100px
            }

            else if (sheetName == "BloodGroups")
            {
                // Write custom headers in row 1
                worksheet.Cells["A1"].Value = "BloodGroupID";
                worksheet.Cells["B1"].Value = "Sr. No.";
                worksheet.Cells["C1"].Value = "Blood Group";
                worksheet.Cells["A1:C1"].Style.Font.Bold = true;

                int row = 2;
                int srNo = 1;
                foreach (var item in data)
                {
                    // Assuming your class object has properties "Id" and "Name"
                    var idProp = item.GetType().GetProperty("Id");
                    var nameProp = item.GetType().GetProperty("Name");

                    var BloodGroupID = idProp != null ? idProp.GetValue(item) : null;
                    var BloodGroups = nameProp != null ? nameProp.GetValue(item) : null;

                    worksheet.Cells[row, 1].Value = BloodGroupID;
                    worksheet.Cells[row, 2].Value = srNo;
                    worksheet.Cells[row, 3].Value = BloodGroups;

                    row++;
                    srNo++;
                }

                // Hide the "ClassID" column (Column A)
                worksheet.Column(1).Hidden = true;

                // Left-align the "Sr. No." column (Column B)
                worksheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                // Auto-fit columns first
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set "Class Name" (Column C) to ~100px
                worksheet.Column(3).Width = 14; // ~100px
            }

            else if (sheetName == "Castes")
            {
                // Write custom headers in row 1
                worksheet.Cells["A1"].Value = "CasteID";
                worksheet.Cells["B1"].Value = "Sr. No.";
                worksheet.Cells["C1"].Value = "Caste";
                worksheet.Cells["A1:C1"].Style.Font.Bold = true;

                int row = 2;
                int srNo = 1;
                foreach (var item in data)
                {
                    // Assuming your class object has properties "Id" and "Name"
                    var idProp = item.GetType().GetProperty("Id");
                    var nameProp = item.GetType().GetProperty("Name");

                    var CasteID = idProp != null ? idProp.GetValue(item) : null;
                    var Castes = nameProp != null ? nameProp.GetValue(item) : null;

                    worksheet.Cells[row, 1].Value = CasteID;
                    worksheet.Cells[row, 2].Value = srNo;
                    worksheet.Cells[row, 3].Value = Castes;

                    row++;
                    srNo++;
                }

                // Hide the "ClassID" column (Column A)
                worksheet.Column(1).Hidden = true;

                // Left-align the "Sr. No." column (Column B)
                worksheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                // Auto-fit columns first
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set "Class Name" (Column C) to ~100px
                worksheet.Column(3).Width = 14; // ~100px
            }

            else if (sheetName == "InstituteHouses")
            {
                // Write custom headers in row 1
                worksheet.Cells["A1"].Value = "InstituteHouseID";
                worksheet.Cells["B1"].Value = "Sr. No.";
                worksheet.Cells["C1"].Value = "Institute Houses";
                worksheet.Cells["A1:C1"].Style.Font.Bold = true;

                int row = 2;
                int srNo = 1;
                foreach (var item in data)
                {
                    // Assuming your class object has properties "Id" and "Name"
                    var idProp = item.GetType().GetProperty("Id");
                    var nameProp = item.GetType().GetProperty("Name");

                    var InstituteHouseID = idProp != null ? idProp.GetValue(item) : null;
                    var InstituteHouses = nameProp != null ? nameProp.GetValue(item) : null;

                    worksheet.Cells[row, 1].Value = InstituteHouseID;
                    worksheet.Cells[row, 2].Value = srNo;
                    worksheet.Cells[row, 3].Value = InstituteHouses;

                    row++;
                    srNo++;
                }

                // Hide the "ClassID" column (Column A)
                worksheet.Column(1).Hidden = true;

                // Left-align the "Sr. No." column (Column B)
                worksheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                // Auto-fit columns first
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set "Class Name" (Column C) to ~100px
                worksheet.Column(3).Width = 14; // ~100px
            }

            else if (sheetName == "StudentTypes")
            {
                // Write custom headers in row 1
                worksheet.Cells["A1"].Value = "StudentTypeID";
                worksheet.Cells["B1"].Value = "Sr. No.";
                worksheet.Cells["C1"].Value = "Student Type";
                worksheet.Cells["A1:C1"].Style.Font.Bold = true;

                int row = 2;
                int srNo = 1;
                foreach (var item in data)
                {
                    // Assuming your class object has properties "Id" and "Name"
                    var idProp = item.GetType().GetProperty("Id");
                    var nameProp = item.GetType().GetProperty("Name");

                    var StudentTypeID = idProp != null ? idProp.GetValue(item) : null;
                    var StudentTypes = nameProp != null ? nameProp.GetValue(item) : null;

                    worksheet.Cells[row, 1].Value = StudentTypeID;
                    worksheet.Cells[row, 2].Value = srNo;
                    worksheet.Cells[row, 3].Value = StudentTypes;

                    row++;
                    srNo++;
                }

                // Hide the "ClassID" column (Column A)
                worksheet.Column(1).Hidden = true;

                // Left-align the "Sr. No." column (Column B)
                worksheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                // Auto-fit columns first
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set "Class Name" (Column C) to ~100px
                worksheet.Column(3).Width = 14; // ~100px
            }

            else if (sheetName == "ParentTypes")
            {
                // Write custom headers in row 1
                worksheet.Cells["A1"].Value = "ParentTypeID";
                worksheet.Cells["B1"].Value = "Sr. No.";
                worksheet.Cells["C1"].Value = "Parent Type";
                worksheet.Cells["A1:C1"].Style.Font.Bold = true;

                int row = 2;
                int srNo = 1;
                foreach (var item in data)
                {
                    // Assuming your class object has properties "Id" and "Name"
                    var idProp = item.GetType().GetProperty("Id");
                    var nameProp = item.GetType().GetProperty("Name");

                    var ParentTypeID = idProp != null ? idProp.GetValue(item) : null;
                    var ParentTypes = nameProp != null ? nameProp.GetValue(item) : null;

                    worksheet.Cells[row, 1].Value = ParentTypeID;
                    worksheet.Cells[row, 2].Value = srNo;
                    worksheet.Cells[row, 3].Value = ParentTypes;

                    row++;
                    srNo++;
                }

                // Hide the "ClassID" column (Column A)
                worksheet.Column(1).Hidden = true;

                // Left-align the "Sr. No." column (Column B)
                worksheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                // Auto-fit columns first
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Set "Class Name" (Column C) to ~100px
                worksheet.Column(3).Width = 14; // ~100px
            }

            // ==============================================
            // 3) Default logic for other sheets
            // ==============================================
            else
            {
                // For other master sheets, use the default format
                worksheet.Cells["A1"].Value = sheetName;
                worksheet.Cells["A2"].LoadFromCollection(data, true);
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            }
        }

        public async Task<ServiceResponse<StudentImportResponse>> ImportStudentInformation(int instituteID, Stream fileStream)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(fileStream))
                {
                    // Get the main import sheet.
                    var worksheet = package.Workbook.Worksheets["Student Import Info"];
                    if (worksheet == null)
                        return new ServiceResponse<StudentImportResponse>(
                            false,
                            "Invalid Excel format. Missing 'Student Import Info' sheet.",
                            null,
                            400
                        );


                    //--------------------------- Gender -------------------------//
                    // Build a lookup dictionary from the "Genders" sheet. 
                    var genderSheet = package.Workbook.Worksheets["Genders"];
                    var genderLookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    if (genderSheet != null)
                    {
                        int gRow = 2; // Assuming header is in row 1.
                        while (genderSheet.Cells[gRow, 1].Value != null)
                        {
                            var idText = genderSheet.Cells[gRow, 1].Text;
                            var genderName = genderSheet.Cells[gRow, 3].Text;
                            if (int.TryParse(idText, out int gID))
                            {
                                if (!genderLookup.ContainsKey(genderName))
                                    genderLookup.Add(genderName, gID);
                            }
                            gRow++;
                        }
                    }
                    //--------------------------- Gender -------------------------//


                    //--------------------------- Class -------------------------//
                    // Build a lookup dictionary from the "Classes" sheet. 
                    var ClassesSheet = package.Workbook.Worksheets["Classes"];
                    var ClassesLookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    if (ClassesSheet != null)
                    {
                        int gRow = 2; // Assuming header is in row 1.
                        while (ClassesSheet.Cells[gRow, 1].Value != null)
                        {
                            var idText = ClassesSheet.Cells[gRow, 1].Text;
                            var ClassName = ClassesSheet.Cells[gRow, 3].Text;
                            if (int.TryParse(idText, out int gID))
                            {
                                if (!ClassesLookup.ContainsKey(ClassName))
                                    ClassesLookup.Add(ClassName, gID);
                            }
                            gRow++;
                        }
                    }
                    //--------------------------- Class -------------------------//

                    //--------------------------- Section -------------------------//

                    var SectionsSheet = package.Workbook.Worksheets["Sections"];
                    var SectionsLookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    if (SectionsSheet != null)
                    {
                        int gRow = 2; // Assuming header is in row 1.
                        while (SectionsSheet.Cells[gRow, 1].Value != null)
                        {
                            // Read the SectionID from column 1.
                            var idText = SectionsSheet.Cells[gRow, 1].Text;
                            // Read the Class Name from column 3.
                            var className = SectionsSheet.Cells[gRow, 3].Text;
                            // Read the Section Name from column 4.
                            var sectionName = SectionsSheet.Cells[gRow, 4].Text;
                            if (int.TryParse(idText, out int sectionID))
                            {
                                // Create a composite key combining class name and section name.
                                var key = $"{className.Trim()}|{sectionName.Trim()}";
                                if (!SectionsLookup.ContainsKey(key))
                                    SectionsLookup.Add(key, sectionID);
                            }
                            gRow++;
                        }
                    } 

                    //// Build a lookup dictionary from the "Sections" sheet. 
                    //var SectionsSheet = package.Workbook.Worksheets["Sections"];
                    //var SectionsLookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    //if (SectionsSheet != null)
                    //{
                    //    int gRow = 2; // Assuming header is in row 1.
                    //    while (SectionsSheet.Cells[gRow, 1].Value != null)
                    //    {
                    //        var idText = SectionsSheet.Cells[gRow, 1].Text;  
                    //        var SectionName = SectionsSheet.Cells[gRow, 4].Text;
                    //        if (int.TryParse(idText, out int gID))
                    //        {
                    //            if (!SectionsLookup.ContainsKey(SectionName))
                    //                SectionsLookup.Add(SectionName, gID);
                    //        }
                    //        gRow++;
                    //    }
                    //}
                    //--------------------------- Section -------------------------//

                    var students = new List<StudentInformationImportRequest>();
                    var errorMessages = new List<string>();
                    int row = 2;
                    while (worksheet.Cells[row, 1].Value != null)
                    {
                        // Read student basic details.
                        string firstName = worksheet.Cells[row, 1].Text;
                        string middleName = worksheet.Cells[row, 2].Text;
                        string lastName = worksheet.Cells[row, 3].Text; 
                        var genderText = worksheet.Cells[row, 4].Text;
                        var ClassText = worksheet.Cells[row, 5].Text; 
                        var SectionText = worksheet.Cells[row, 6].Text; 
                        string AdmissionNumber = worksheet.Cells[row, 7].Text;
                        string RollNumber = worksheet.Cells[row, 8].Text;

                        var compositeKey = $"{ClassText.Trim()}|{SectionText.Trim()}";

                        // Validate the gender.
                        if (string.IsNullOrWhiteSpace(genderText) || !genderLookup.TryGetValue(genderText, out int foundGenderID))
                        {
                            errorMessages.Add($"{firstName} {lastName} mentioned GenderType '{genderText}' is not available");
                        }
                        // Validate the Class.
                        else if (string.IsNullOrWhiteSpace(ClassText) || !ClassesLookup.TryGetValue(ClassText, out int foundClassID))
                        {
                            errorMessages.Add($"{firstName} {lastName} mentioned Class Name '{ClassText}' is not available");
                        }
                        // Validate the Sections.
                        //else if (string.IsNullOrWhiteSpace(SectionText) || !SectionsLookup.TryGetValue(SectionText, out int foundSectionID))
                        //{
                        //    errorMessages.Add($"{firstName} {lastName} mentioned Section Name '{SectionText}' is not available");
                        //} 
                        else if (string.IsNullOrWhiteSpace(SectionText) || !SectionsLookup.TryGetValue(compositeKey, out int foundSectionID))
                        {
                            errorMessages.Add($"{firstName} {lastName} mentioned Section Name '{SectionText}' for Class '{ClassText}' is not available");
                        }
                        else
                        {
                            // If valid, create the student object.
                            var student = new StudentInformationImportRequest
                            {
                                StudentDetails = new StudentDetails_IM
                                {
                                    FirstName = firstName,
                                    MiddleName = middleName,
                                    LastName = lastName,
                                    GenderID = foundGenderID, 
                                    ClassID = foundClassID,
                                    SectionID = foundSectionID,
                                    AdmissionNumber = AdmissionNumber,
                                    RollNumber = RollNumber
                                    // Additional fields can be parsed here.
                                }
                            };
                            students.Add(student);
                        }
                        row++;
                    }

                    // If any errors were found, return them in the response class.
                    if (errorMessages.Any())
                    {
                        var errorResponse = new StudentImportResponse
                        {
                            Success = false,
                            Message = "One or more errors occurred during import.",
                            ErrorMessages = errorMessages
                        };

                        return new ServiceResponse<StudentImportResponse>(
                            false,
                            errorResponse.Message,
                            errorResponse,
                            400
                        );
                    }

                    // If no errors, proceed with inserting the students.
                    var insertResult = await _studentInformationRepository.InsertStudents(instituteID, students);
                    // Assuming insertResult returns a ServiceResponse<string> upon success.

                    var successResponse = new StudentImportResponse
                    {
                        Success = true,
                        Message = "Students imported successfully",
                        ErrorMessages = new List<string>()
                    };

                    return new ServiceResponse<StudentImportResponse>(
                        true,
                        successResponse.Message,
                        successResponse,
                        200
                    );
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<StudentImportResponse>(
                    false,
                    "Error processing file: " + ex.Message,
                    null,
                    500
                );
            }
        }



        //public async Task<ServiceResponse<string>> ImportStudentInformation(int instituteID, Stream fileStream)
        //{
        //    try
        //    {
        //        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //        using (var package = new ExcelPackage(fileStream))
        //        {
        //            // Get the main import sheet.
        //            var worksheet = package.Workbook.Worksheets["Student Import Info"];
        //            if (worksheet == null)
        //                return new ServiceResponse<string>(false, "Invalid Excel format. Missing 'Student Import Info' sheet.", null, 400);

        //            // Build a lookup dictionary from the "Genders" sheet.
        //            // Assume that column 1 (hidden) contains GenderID and column 3 contains the Gender Name.
        //            var genderSheet = package.Workbook.Worksheets["Genders"];
        //            var genderLookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        //            if (genderSheet != null)
        //            {
        //                int gRow = 2; // Assuming header is in row 1.
        //                while (genderSheet.Cells[gRow, 1].Value != null)
        //                {
        //                    var idText = genderSheet.Cells[gRow, 1].Text;
        //                    var genderName = genderSheet.Cells[gRow, 3].Text;
        //                    if (int.TryParse(idText, out int gID))
        //                    {
        //                        if (!genderLookup.ContainsKey(genderName))
        //                            genderLookup.Add(genderName, gID);
        //                    }
        //                    gRow++;
        //                }
        //            }

        //            var students = new List<StudentInformationImportRequest>();
        //            var errorMessages = new List<string>();
        //            int row = 2;
        //            while (worksheet.Cells[row, 1].Value != null)
        //            {
        //                // Read student basic details.
        //                string firstName = worksheet.Cells[row, 1].Text;
        //                string middleName = worksheet.Cells[row, 2].Text;
        //                string lastName = worksheet.Cells[row, 3].Text;
        //                var genderText = worksheet.Cells[row, 4].Text;

        //                // Check if the gender is present in the lookup.
        //                if (string.IsNullOrWhiteSpace(genderText) || !genderLookup.TryGetValue(genderText, out int foundGenderID))
        //                {
        //                    errorMessages.Add($"{firstName} {lastName} mentioned GenderType '{genderText}' is not available");
        //                }
        //                else
        //                {
        //                    var student = new StudentInformationImportRequest
        //                    {
        //                        StudentDetails = new StudentDetails_IM
        //                        {
        //                            FirstName = firstName,
        //                            MiddleName = middleName,
        //                            LastName = lastName,
        //                            GenderID = foundGenderID
        //                            // Additional fields can be parsed here.
        //                        }
        //                    };
        //                    students.Add(student);
        //                }
        //                row++;
        //            }

        //            // If there are any errors, return them together.
        //            if (errorMessages.Any())
        //            {
        //                var combinedErrors = string.Join(Environment.NewLine, errorMessages);
        //                return new ServiceResponse<string>(false, combinedErrors, null, 400);
        //            }

        //            // Optionally, use instituteID for further processing.
        //            return await _studentInformationRepository.InsertStudents(instituteID, students);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<string>(false, "Error processing file: " + ex.Message, null, 500);
        //    }
        //}


        //public async Task<ServiceResponse<string>> ImportStudentInformation(int instituteID, Stream fileStream)
        //{
        //    try
        //    {
        //        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //        using (var package = new ExcelPackage(fileStream))
        //        {
        //            // Get the main import sheet.
        //            var worksheet = package.Workbook.Worksheets["Student Import Info"];
        //            if (worksheet == null)
        //                return new ServiceResponse<string>(false, "Invalid Excel format. Missing 'Student Import Info' sheet.", null, 400);

        //            // Build a lookup dictionary from the "Genders" sheet.
        //            // Assume that column 1 (hidden) contains GenderID and column 3 contains the Gender Name.
        //            var genderSheet = package.Workbook.Worksheets["Genders"];
        //            var genderLookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        //            if (genderSheet != null)
        //            {
        //                int gRow = 2; // Assuming header is in row 1.
        //                while (genderSheet.Cells[gRow, 1].Value != null)
        //                {
        //                    var idText = genderSheet.Cells[gRow, 1].Text;
        //                    var genderName = genderSheet.Cells[gRow, 3].Text;
        //                    if (int.TryParse(idText, out int gID))
        //                    {
        //                        if (!genderLookup.ContainsKey(genderName))
        //                            genderLookup.Add(genderName, gID);
        //                    }
        //                    gRow++;
        //                }
        //            }

        //            var students = new List<StudentInformationImportRequest>();
        //            int row = 2;
        //            while (worksheet.Cells[row, 1].Value != null)
        //            {
        //                // Read student basic details.
        //                string firstName = worksheet.Cells[row, 1].Text;
        //                string middleName = worksheet.Cells[row, 2].Text;
        //                string lastName = worksheet.Cells[row, 3].Text;
        //                var genderText = worksheet.Cells[row, 4].Text;

        //                // Check if the gender is present in the lookup.
        //                if (string.IsNullOrWhiteSpace(genderText) || !genderLookup.TryGetValue(genderText, out int foundGenderID))
        //                {
        //                    // Return error message and stop processing.
        //                    return new ServiceResponse<string>(
        //                        false,
        //                        $"{firstName} {lastName} mentioned GenderType '{genderText}' is not available",
        //                        null,
        //                        400
        //                    );
        //                }

        //                var student = new StudentInformationImportRequest
        //                {
        //                    StudentDetails = new StudentDetails_IM
        //                    {
        //                        FirstName = firstName,
        //                        MiddleName = middleName,
        //                        LastName = lastName,
        //                        GenderID = foundGenderID
        //                        // Additional fields can be parsed here.
        //                    }
        //                };

        //                students.Add(student);
        //                row++;
        //            }

        //            // Optionally, use instituteID for further processing.
        //            return await _studentInformationRepository.InsertStudents(instituteID, students);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<string>(false, "Error processing file: " + ex.Message, null, 500);
        //    }
        //}


        private void AddSectionsSheet(ExcelPackage package, string sheetName, List<SectionJoinedResponse> sections)
        {
            if (sections == null || !sections.Any()) return;

            var worksheet = package.Workbook.Worksheets.Add(sheetName);

            // 1) Headers
            worksheet.Cells["A1"].Value = "SectionID";
            worksheet.Cells["B1"].Value = "Sr. No.";
            worksheet.Cells["C1"].Value = "Class Name";
            worksheet.Cells["D1"].Value = "Section Name";
            worksheet.Cells["A1:D1"].Style.Font.Bold = true;

            // 2) Write Rows
            int row = 2;
            int srNo = 1;
            foreach (var item in sections)
            {
                worksheet.Cells[row, 1].Value = item.SectionID;   // SectionID
                worksheet.Cells[row, 2].Value = srNo;             // Sr. No.
                worksheet.Cells[row, 3].Value = item.ClassName;   // Class Name
                worksheet.Cells[row, 4].Value = item.SectionName; // Section Name

                row++;
                srNo++;
            }

            worksheet.Column(1).Hidden = true;  

            // 3) Formatting
            // Left-align "Sr. No." column
            worksheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            // Auto-fit columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

    }
}
