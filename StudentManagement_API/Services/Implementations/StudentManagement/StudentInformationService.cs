using System.Globalization;
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
                    //var headers = new List<string>
                    //{
                    //    "First Name", "Middle Name", "Last Name", "Gender", "Class Name", "Section Name",
                    //    "Admission Number", "Roll Number", "Date of Joining", "Academic Year", "Nationality",
                    //    "Religion", "Date of Birth", "Mother Tongue", "Caste", "Blood Group", "Aadhar No",
                    //    "PEN", "Student Type", "Student House",

                    //    "Registration Date", "Registration No", "Admission Date", "Samagra ID",
                    //    "Place of Birth", "Email ID", "Language Known", "Comments",
                    //    "Identification Mark 1", "Identification Mark 2",

                    //    "Parent First Name", "Parent Middle Name", "Parent Last Name",
                    //    "Primary Contact No", "Bank Account No", "Bank IFSC Code",
                    //    "Family Ration Card Type", "Family Ration Card No", "Parent Date of Birth",
                    //    "Parent Aadhar No", "Parent PAN Card No", "Occupation",
                    //    "Designation", "Name of Employer", "Office No", "Parent Email ID",
                    //    "Annual Income", "Residential Address",

                    //    "Sibling First Name", "Sibling Middle Name", "Sibling Last Name",
                    //    "Sibling Admission No", "Sibling Date of Birth", "Sibling Class",
                    //    "Sibling Section", "Sibling Institute Name", "Sibling Aadhar No",

                    //    "Previous School Name", "Previous Board", "Medium", "Previous School Address",
                    //    "Previous Course", "Previous Class", "TC Number", "TC Date", "Is TC Submitted",

                    //    "Allergies", "Medications", "Doctor Name", "Doctor Contact",
                    //    "Height", "Weight", "Government Health ID", "Vision", "Hearing", "Speech",
                    //    "Behavioral Problems", "Chest Condition", "History of Accidents",
                    //    "Physical Deformities", "Major Illness History", "Other Remarks/Weakness"
                    //};


                    var headers = new List<string>
                    {
                        "Admission No",
                        "First Name",
                        "Middle Name",
                        "Last Name",
                        "Gender",
                        "Class",
                        "Section",
                        "Roll No",
                        "Date of Joining",
                        "Academic Year",
                        "Nationality",
                        "Religion",
                        "Date of Birth",
                        "Mother Tongue",
                        "Caste",
                        "Blood Group",
                        "Aadhar No",
                        "PEN",
                        "Student Type",
                        "Student House",
                        "Registration Date",
                        "Registration No",
                        "Admission Date",
                        "Samagra ID",
                        "Place of Birth",
                        "Email ID",
                        "Languages Known",
                        "Identification Mark 1",
                        "Identification Mark 2",
                        "Father First Name",
                        "Father Middle Name",
                        "Father Last Name",
                        "Primary Contact No",
                        "Bank Account No",
                        "Bank IFSC Code",
                        "Family Ration Card Type",
                        "Family Ration Card No",
                        "Date of Birth",
                        "Aadhar No",
                        "PAN Card No",
                        "Occupation",
                        "Designation",
                        "Name of the Employer",
                        "Office No",
                        "Email ID",
                        "Annual Income",
                        "Residential Address",
                        "Office Building No",
                        "Street/Road/Lane",
                        "Area/Locality/Sector",
                        "State",
                        "City",
                        "Pincode",
                        "Mother First Name",
                        "Mother Middle Name",
                        "Mother Last Name",
                        "Primary Contact No",
                        "Bank Account No",
                        "Bank IFSC Code",
                        "Family Ration Card Type",
                        "Family Ration Card No",
                        "Date of Birth",
                        "Aadhar No",
                        "PAN Card No",
                        "Occupation",
                        "Designation",
                        "Name of the Employer",
                        "Office No",
                        "Email ID",
                        "Annual Income",
                        "Residential Address",
                        "Office Building No",
                        "Street/Road/Lane",
                        "Area/Locality/Sector",
                        "State",
                        "City",
                        "Pincode",
                        "Guardian First Name",
                        "Guardian Middle Name",
                        "Guardian Last Name",
                        "Primary Contact No",
                        "Bank Account No",
                        "Bank IFSC Code",
                        "Family Ration Card Type",
                        "Family Ration Card No",
                        "Date of Birth",
                        "Aadhar No",
                        "PAN Card No",
                        "Occupation",
                        "Designation",
                        "Name of the Employer",
                        "Office No",
                        "Email ID",
                        "Annual Income",
                        "Residential Address",
                        "Office Building No",
                        "Street/Road/Lane",
                        "Area/Locality/Sector",
                        "State",
                        "City",
                        "Pincode",
                        "Sibling First Name",
                        "Sibling Middle Name",
                        "Sibling Last Name",
                        "Admission No",
                        "Date of Birth",
                        "Class",
                        "Section",
                        "Institute Name",
                        "Aadhar No",
                        "Institute Name",
                        "Board",
                        "Medium",
                        "Institute Address",
                        "Course",
                        "Class",
                        "TC Number",
                        "TC Date",
                        "Allergies",
                        "Medications",
                        "Consulting Doctor's Name",
                        "Consulting Doctor's Phone No",
                        "Height",
                        "Weight",
                        "Government Health ID",
                        "Vision",
                        "Hearing",
                        "Speech",
                        "Behaviourial Problems If Any",
                        "Chest",
                        "History of Any Accident",
                        "Any Physical Deformity",
                        "History of Major Illness",
                        "Any other Remarks or Special Weakness"
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

                    //// ✅ 3️⃣ Write Column Headers (Starting from row 1)
                    //for (int i = 0; i < headers.Count; i++)
                    //{
                    //    var cell = studentSheet.Cells[1, i + 1];
                    //    cell.Value = headers[i];
                    //    cell.Style.Font.Bold = true;

                    //    // Set background color to red for "Admission No" column
                    //    if (headers[i] == "Admission No")
                    //    {
                    //        cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    //        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                    //    }

                    //    cell.AutoFitColumns();
                    //}




                    var headersColor = new List<string>
                    {
                        "Admission No",
                        "First Name",
                        "Last Name",
                        "Gender",
                        "Class",
                        "Section",
                        "Roll No",
                        "Date of Joining",
                        "Academic Year",
                        "Nationality",
                        "Religion",
                        "Date of Birth",
                        "Caste",
                        "Student Type",
                        "Father First Name",
                        "Father Last Name",
                        "Primary Contact No", // Father's Primary Contact No
                        "Mother First Name",
                        "Mother Last Name",
                        "Primary Contact No", // Mother's Primary Contact No
                        "Guardian First Name",
                        "Guardian Last Name",
                        "Primary Contact No"  // Guardian's Primary Contact No
                    };



                    for (int i = 0; i < headers.Count; i++)
                    {
                        var cell = studentSheet.Cells[1, i + 1];
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;

                        // Set the background color to red for each header cell in the list
                        if (headers[i] == "Admission No" ||
                            headers[i] == "First Name" ||
                            headers[i] == "Last Name" ||
                            headers[i] == "Gender" ||
                            headers[i] == "Class" ||
                            headers[i] == "Section" ||
                            headers[i] == "Roll No" ||
                            headers[i] == "Date of Joining" ||
                            headers[i] == "Academic Year" ||
                            headers[i] == "Nationality" ||
                            headers[i] == "Religion" ||
                            headers[i] == "Date of Birth" ||
                            headers[i] == "Caste" ||
                            headers[i] == "Student Type" ||
                            headers[i] == "Father First Name" ||
                            headers[i] == "Father Last Name" ||
                            headers[i] == "Primary Contact No" || // Will match each instance of "Primary Contact No"
                            headers[i] == "Mother First Name" ||
                            headers[i] == "Mother Last Name" ||
                            headers[i] == "Guardian First Name" ||
                            headers[i] == "Guardian Last Name")
                        {
                            cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                        }

                        cell.AutoFitColumns();
                    }


                    // Set date format "dd-mm-yyyy" for each date column:
                    studentSheet.Column(9).Style.Numberformat.Format = "dd-mm-yyyy";    // Date of Joining
                    studentSheet.Column(13).Style.Numberformat.Format = "dd-mm-yyyy";   // Date of Birth (Student)
                    studentSheet.Column(21).Style.Numberformat.Format = "dd-mm-yyyy";   // Registration Date
                    studentSheet.Column(23).Style.Numberformat.Format = "dd-mm-yyyy";   // Admission Date
                    studentSheet.Column(38).Style.Numberformat.Format = "dd-mm-yyyy";   // Date of Birth (Father)
                    studentSheet.Column(62).Style.Numberformat.Format = "dd-mm-yyyy";   // Date of Birth (Mother)
                    studentSheet.Column(86).Style.Numberformat.Format = "dd-mm-yyyy";   // Date of Birth (Guardian)
                    studentSheet.Column(106).Style.Numberformat.Format = "dd-mm-yyyy";  // Date of Birth (Sibling)
                    studentSheet.Column(118).Style.Numberformat.Format = "dd-mm-yyyy";  // TC Date


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
                    //--------------------------- Section -------------------------//

                    //--------------------------- Nationalities -------------------------//
                    // Build a lookup dictionary from the "Classes" sheet. 
                    var NationalitySheet = package.Workbook.Worksheets["Nationalities"];
                    var NationalityLookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    if (NationalitySheet != null)
                    {
                        int gRow = 2; // Assuming header is in row 1.
                        while (NationalitySheet.Cells[gRow, 1].Value != null)
                        {
                            var idText = NationalitySheet.Cells[gRow, 1].Text;
                            var NationalityName = NationalitySheet.Cells[gRow, 3].Text;
                            if (int.TryParse(idText, out int gID))
                            {
                                if (!NationalityLookup.ContainsKey(NationalityName))
                                    NationalityLookup.Add(NationalityName, gID);
                            }
                            gRow++;
                        }
                    }
                    //--------------------------- Nationalities -------------------------//

                    //--------------------------- Religions -------------------------//
                    // Build a lookup dictionary from the "Classes" sheet. 
                    var ReligionsSheet = package.Workbook.Worksheets["Religions"];
                    var ReligionsLookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    if (ReligionsSheet != null)
                    {
                        int gRow = 2; // Assuming header is in row 1.
                        while (ReligionsSheet.Cells[gRow, 1].Value != null)
                        {
                            var idText = ReligionsSheet.Cells[gRow, 1].Text;
                            var ReligionsName = ReligionsSheet.Cells[gRow, 3].Text;
                            if (int.TryParse(idText, out int gID))
                            {
                                if (!ReligionsLookup.ContainsKey(ReligionsName))
                                    ReligionsLookup.Add(ReligionsName, gID);
                            }
                            gRow++;
                        }
                    }
                    //--------------------------- Religions -------------------------//


                    //--------------------------- MotherTongues -------------------------//
                    // Build a lookup dictionary from the "MotherTongues" sheet. 
                    var MotherTonguesSheet = package.Workbook.Worksheets["MotherTongues"];
                    var MotherTonguesLookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    if (MotherTonguesSheet != null)
                    {
                        int gRow = 2; // Assuming header is in row 1.
                        while (MotherTonguesSheet.Cells[gRow, 1].Value != null)
                        {
                            var idText = MotherTonguesSheet.Cells[gRow, 1].Text;
                            var MotherTonguesName = MotherTonguesSheet.Cells[gRow, 3].Text;
                            if (int.TryParse(idText, out int gID))
                            {
                                if (!MotherTonguesLookup.ContainsKey(MotherTonguesName))
                                    MotherTonguesLookup.Add(MotherTonguesName, gID);
                            }
                            gRow++;
                        }
                    }
                    //--------------------------- MotherTongues -------------------------//


                    //--------------------------- Castes -------------------------//
                    // Build a lookup dictionary from the "MotherTongues" sheet. 
                    var CastesSheet = package.Workbook.Worksheets["Castes"];
                    var CastesLookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    if (CastesSheet != null)
                    {
                        int gRow = 2; // Assuming header is in row 1.
                        while (CastesSheet.Cells[gRow, 1].Value != null)
                        {
                            var idText = CastesSheet.Cells[gRow, 1].Text;
                            var CastesName = CastesSheet.Cells[gRow, 3].Text;
                            if (int.TryParse(idText, out int gID))
                            {
                                if (!CastesLookup.ContainsKey(CastesName))
                                    CastesLookup.Add(CastesName, gID);
                            }
                            gRow++;
                        }
                    }
                    //--------------------------- Castes -------------------------//

                    //--------------------------- BloodGroups -------------------------//
                    // Build a lookup dictionary from the "MotherTongues" sheet. 
                    var BloodGroupsSheet = package.Workbook.Worksheets["BloodGroups"];
                    var BloodGroupsLookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    if (BloodGroupsSheet != null)
                    {
                        int gRow = 2; // Assuming header is in row 1.
                        while (BloodGroupsSheet.Cells[gRow, 1].Value != null)
                        {
                            var idText = BloodGroupsSheet.Cells[gRow, 1].Text;
                            var BloodGroupsName = BloodGroupsSheet.Cells[gRow, 3].Text;
                            if (int.TryParse(idText, out int gID))
                            {
                                if (!BloodGroupsLookup.ContainsKey(BloodGroupsName))
                                    BloodGroupsLookup.Add(BloodGroupsName, gID);
                            }
                            gRow++;
                        }
                    }
                    //--------------------------- BloodGroups -------------------------//


                    //--------------------------- StudentTypes -------------------------//
                    // Build a lookup dictionary from the "StudentTypes" sheet. 
                    var StudentTypesSheet = package.Workbook.Worksheets["StudentTypes"];
                    var StudentTypesLookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    if (StudentTypesSheet != null)
                    {
                        int gRow = 2; // Assuming header is in row 1.
                        while (StudentTypesSheet.Cells[gRow, 1].Value != null)
                        {
                            var idText = StudentTypesSheet.Cells[gRow, 1].Text;
                            var StudentTypesName = StudentTypesSheet.Cells[gRow, 3].Text;
                            if (int.TryParse(idText, out int gID))
                            {
                                if (!StudentTypesLookup.ContainsKey(StudentTypesName))
                                    StudentTypesLookup.Add(StudentTypesName, gID);
                            }
                            gRow++;
                        }
                    }
                    //--------------------------- StudentTypes -------------------------//

                    //--------------------------- InstituteHouses -------------------------//
                    // Build a lookup dictionary from the "InstituteHouses" sheet. 
                    var InstituteHousesSheet = package.Workbook.Worksheets["InstituteHouses"];
                    var InstituteHousesLookup = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    if (InstituteHousesSheet != null)
                    {
                        int gRow = 2; // Assuming header is in row 1.
                        while (InstituteHousesSheet.Cells[gRow, 1].Value != null)
                        {
                            var idText = InstituteHousesSheet.Cells[gRow, 1].Text;
                            var InstituteHousesName = InstituteHousesSheet.Cells[gRow, 3].Text;
                            if (int.TryParse(idText, out int gID))
                            {
                                if (!InstituteHousesLookup.ContainsKey(InstituteHousesName))
                                    InstituteHousesLookup.Add(InstituteHousesName, gID);
                            }
                            gRow++;
                        }
                    }
                    //--------------------------- InstituteHouses -------------------------//

                    var students = new List<StudentInformationImportRequest>();
                    var errorMessages = new List<string>();
                    int row = 2;
                    while (worksheet.Cells[row, 1].Value != null)
                    {
                        //string AdmissionNumber = worksheet.Cells[row, 1].Text; //"Admission No", 1
                        //string firstName = worksheet.Cells[row, 2].Text;  //"First Name", 2
                        //string middleName = worksheet.Cells[row, 3].Text;//"Middle Name", 3
                        //string lastName = worksheet.Cells[row, 4].Text;  //"Last Name", 4
                        //var genderText = worksheet.Cells[row, 5].Text;  //"Gender", 5
                        //var ClassText = worksheet.Cells[row, 6].Text;  //"Class", 6
                        //var SectionText = worksheet.Cells[row, 7].Text;  //"Section", 7
                        //string RollNumber = worksheet.Cells[row, 8].Text;  //"Roll No", 8
                        ////var dateOfJoiningText = worksheet.Cells[row, 9].Text;  //"Date of Joining", 9
                        ////"Academic Year", 10
                        //var NationalityText = worksheet.Cells[row, 11].Text;  //"Nationality", 11
                        //var ReligionsText = worksheet.Cells[row, 12].Text;  //"Religion", 12
                        ////var dateOfBirthText = worksheet.Cells[row, 13].Text;   //"Date of Birth", 13
                        //var MotherTonguesText = worksheet.Cells[row, 14].Text;  //"Mother Tongue", 14
                        //var CastesText = worksheet.Cells[row, 15].Text;  //"Caste", 15
                        //var BloodGroupsText = worksheet.Cells[row, 16].Text;  //"Blood Group", 16
                        //var AadharNoText = worksheet.Cells[row, 17].Text;  //"Aadhar No", 17
                        //var PENText = worksheet.Cells[row, 18].Text;  //"PEN", 18
                        //var StudentTypesText = worksheet.Cells[row, 19].Text;  //"Student Type", 19



                        // Read all values from the row based on the new column order:
                        string AdmissionNumber = worksheet.Cells[row, 1].Text;   // "Admission No"
                        string firstName = worksheet.Cells[row, 2].Text;   // "First Name"
                        string middleName = worksheet.Cells[row, 3].Text;   // "Middle Name"
                        string lastName = worksheet.Cells[row, 4].Text;   // "Last Name"
                        string genderText = worksheet.Cells[row, 5].Text;   // "Gender"
                        string classText = worksheet.Cells[row, 6].Text;   // "Class"
                        string sectionText = worksheet.Cells[row, 7].Text;   // "Section"
                        string RollNumber = worksheet.Cells[row, 8].Text;   // "Roll No"
                        string dateOfJoiningText = worksheet.Cells[row, 9].Text;   // "Date of Joining"
                        string academicYear = worksheet.Cells[row, 10].Text;  // "Academic Year"
                        string NationalityText = worksheet.Cells[row, 11].Text;  // "Nationality"
                        string ReligionsText = worksheet.Cells[row, 12].Text;  // "Religion"
                        string dateOfBirthText = worksheet.Cells[row, 13].Text;  // "Date of Birth"
                        string MotherTonguesText = worksheet.Cells[row, 14].Text;  // "Mother Tongue"
                        string CastesText = worksheet.Cells[row, 15].Text;  // "Caste"
                        string BloodGroupsText = worksheet.Cells[row, 16].Text;  // "Blood Group"
                        string AadharNoText = worksheet.Cells[row, 17].Text;  // "Aadhar No"
                        string PENText = worksheet.Cells[row, 18].Text;  // "PEN"
                        string StudentTypesText = worksheet.Cells[row, 19].Text;  // "Student Type"
                        string StudentHouseText = worksheet.Cells[row, 20].Text;  // "Student House"

                        string registrationDateText = worksheet.Cells[row, 21].Text;  // "Registration Date"
                        string registrationNo = worksheet.Cells[row, 22].Text;  // "Registration No"
                        string admissionDateText = worksheet.Cells[row, 23].Text;  // "Admission Date"
                        string samagraID = worksheet.Cells[row, 24].Text;  // "Samagra ID"
                        string placeOfBirth = worksheet.Cells[row, 25].Text;  // "Place of Birth"
                        string emailID = worksheet.Cells[row, 26].Text;  // "Email ID"
                        string languagesKnown = worksheet.Cells[row, 27].Text;  // "Languages Known"
                        string identificationMark1 = worksheet.Cells[row, 28].Text;  // "Identification Mark 1"
                        string identificationMark2 = worksheet.Cells[row, 29].Text;  // "Identification Mark 2"

                        // ----- Father Details -----
                        string fatherFirstName = worksheet.Cells[row, 30].Text;  // "Father First Name"
                        string fatherMiddleName = worksheet.Cells[row, 31].Text;  // "Father Middle Name"
                        string fatherLastName = worksheet.Cells[row, 32].Text;  // "Father Last Name"
                        string fatherPrimaryContactNo = worksheet.Cells[row, 33].Text;  // "Father Primary Contact No"
                        string fatherBankAccountNo = worksheet.Cells[row, 34].Text;  // "Father Bank Account No"
                        string fatherBankIFSCCode = worksheet.Cells[row, 35].Text;  // "Father Bank IFSC Code"
                        string fatherFamilyRationCardType = worksheet.Cells[row, 36].Text; // "Father Family Ration Card Type"
                        string fatherFamilyRationCardNo = worksheet.Cells[row, 37].Text; // "Father Family Ration Card No"
                        string fatherDOBText = worksheet.Cells[row, 38].Text;  // "Father Date of Birth"
                        string fatherAadharNo = worksheet.Cells[row, 39].Text;  // "Father Aadhar No"
                        string fatherPANCardNo = worksheet.Cells[row, 40].Text;  // "Father PAN Card No"
                        string fatherOccupation = worksheet.Cells[row, 41].Text;  // "Father Occupation"
                        string fatherDesignation = worksheet.Cells[row, 42].Text;  // "Father Designation"
                        string fatherEmployerName = worksheet.Cells[row, 43].Text;  // "Father Name of the Employer"
                        string fatherOfficeNo = worksheet.Cells[row, 44].Text;  // "Father Office No"
                        string fatherEmailID = worksheet.Cells[row, 45].Text;  // "Father Email ID"
                        string fatherAnnualIncome = worksheet.Cells[row, 46].Text;  // "Father Annual Income"
                        string fatherResidentialAddress = worksheet.Cells[row, 47].Text;  // "Father Residential Address"
                        string fatherOfficeBuildingNo = worksheet.Cells[row, 48].Text;  // "Father Office Building No"
                        string fatherStreetRoadLane = worksheet.Cells[row, 49].Text;  // "Father Street/Road/Lane"
                        string fatherAreaLocalitySector = worksheet.Cells[row, 50].Text;  // "Father Area/Locality/Sector"
                        string fatherState = worksheet.Cells[row, 51].Text;  // "Father State"
                        string fatherCity = worksheet.Cells[row, 52].Text;  // "Father City"
                        string fatherPincode = worksheet.Cells[row, 53].Text;  // "Father Pincode"

                        // ----- Mother Details -----
                        string motherFirstName = worksheet.Cells[row, 54].Text;  // "Mother First Name"
                        string motherMiddleName = worksheet.Cells[row, 55].Text;  // "Mother Middle Name"
                        string motherLastName = worksheet.Cells[row, 56].Text;  // "Mother Last Name"
                        string motherPrimaryContactNo = worksheet.Cells[row, 57].Text;  // "Mother Primary Contact No"
                        string motherBankAccountNo = worksheet.Cells[row, 58].Text;  // "Mother Bank Account No"
                        string motherBankIFSCCode = worksheet.Cells[row, 59].Text;  // "Mother Bank IFSC Code"
                        string motherFamilyRationCardType = worksheet.Cells[row, 60].Text; // "Mother Family Ration Card Type"
                        string motherFamilyRationCardNo = worksheet.Cells[row, 61].Text; // "Mother Family Ration Card No"
                        string motherDOBText = worksheet.Cells[row, 62].Text;  // "Mother Date of Birth"
                        string motherAadharNo = worksheet.Cells[row, 63].Text;  // "Mother Aadhar No"
                        string motherPANCardNo = worksheet.Cells[row, 64].Text;  // "Mother PAN Card No"
                        string motherOccupation = worksheet.Cells[row, 65].Text;  // "Mother Occupation"
                        string motherDesignation = worksheet.Cells[row, 66].Text;  // "Mother Designation"
                        string motherEmployerName = worksheet.Cells[row, 67].Text;  // "Mother Name of the Employer"
                        string motherOfficeNo = worksheet.Cells[row, 68].Text;  // "Mother Office No"
                        string motherEmailID = worksheet.Cells[row, 69].Text;  // "Mother Email ID"
                        string motherAnnualIncome = worksheet.Cells[row, 70].Text;  // "Mother Annual Income"
                        string motherResidentialAddress = worksheet.Cells[row, 71].Text;  // "Mother Residential Address"
                        string motherOfficeBuildingNo = worksheet.Cells[row, 72].Text;  // "Mother Office Building No"
                        string motherStreetRoadLane = worksheet.Cells[row, 73].Text;  // "Mother Street/Road/Lane"
                        string motherAreaLocalitySector = worksheet.Cells[row, 74].Text;  // "Mother Area/Locality/Sector"
                        string motherState = worksheet.Cells[row, 75].Text;  // "Mother State"
                        string motherCity = worksheet.Cells[row, 76].Text;  // "Mother City"
                        string motherPincode = worksheet.Cells[row, 77].Text;  // "Mother Pincode"

                        // ----- Guardian Details -----
                        string guardianFirstName = worksheet.Cells[row, 78].Text;  // "Guardian First Name"
                        string guardianMiddleName = worksheet.Cells[row, 79].Text;  // "Guardian Middle Name"
                        string guardianLastName = worksheet.Cells[row, 80].Text;  // "Guardian Last Name"
                        string guardianPrimaryContactNo = worksheet.Cells[row, 81].Text;  // "Guardian Primary Contact No"
                        string guardianBankAccountNo = worksheet.Cells[row, 82].Text;  // "Guardian Bank Account No"
                        string guardianBankIFSCCode = worksheet.Cells[row, 83].Text;  // "Guardian Bank IFSC Code"
                        string guardianFamilyRationCardType = worksheet.Cells[row, 84].Text;  // "Guardian Family Ration Card Type"
                        string guardianFamilyRationCardNo = worksheet.Cells[row, 85].Text;  // "Guardian Family Ration Card No"
                        string guardianDOBText = worksheet.Cells[row, 86].Text;  // "Guardian Date of Birth"
                        string guardianAadharNo = worksheet.Cells[row, 87].Text;  // "Guardian Aadhar No"
                        string guardianPANCardNo = worksheet.Cells[row, 88].Text;  // "Guardian PAN Card No"
                        string guardianOccupation = worksheet.Cells[row, 89].Text;  // "Guardian Occupation"
                        string guardianDesignation = worksheet.Cells[row, 90].Text;  // "Guardian Designation"
                        string guardianEmployerName = worksheet.Cells[row, 91].Text;  // "Guardian Name of the Employer"
                        string guardianOfficeNo = worksheet.Cells[row, 92].Text;  // "Guardian Office No"
                        string guardianEmailID = worksheet.Cells[row, 93].Text;  // "Guardian Email ID"
                        string guardianAnnualIncome = worksheet.Cells[row, 94].Text;  // "Guardian Annual Income"
                        string guardianResidentialAddress = worksheet.Cells[row, 95].Text;  // "Guardian Residential Address"
                        string guardianOfficeBuildingNo = worksheet.Cells[row, 96].Text;  // "Guardian Office Building No"
                        string guardianStreetRoadLane = worksheet.Cells[row, 97].Text;  // "Guardian Street/Road/Lane"
                        string guardianAreaLocalitySector = worksheet.Cells[row, 98].Text;  // "Guardian Area/Locality/Sector"
                        string guardianState = worksheet.Cells[row, 99].Text;  // "Guardian State"
                        string guardianCity = worksheet.Cells[row, 100].Text;  // "Guardian City"
                        string guardianPincode = worksheet.Cells[row, 101].Text; // "Guardian Pincode"

                        // ----- Sibling Details -----
                        string siblingFirstName = worksheet.Cells[row, 102].Text; // "Sibling First Name"
                        string siblingMiddleName = worksheet.Cells[row, 103].Text; // "Sibling Middle Name"
                        string siblingLastName = worksheet.Cells[row, 104].Text; // "Sibling Last Name"
                        string siblingAdmissionNumber = worksheet.Cells[row, 105].Text; // "Sibling Admission No"
                        string siblingDOBText = worksheet.Cells[row, 106].Text; // "Sibling Date of Birth"
                        string siblingClass = worksheet.Cells[row, 107].Text; // "Sibling Class"
                        string siblingSection = worksheet.Cells[row, 108].Text; // "Sibling Section"
                        string siblingInstituteName = worksheet.Cells[row, 109].Text; // "Sibling Institute Name"
                        string siblingAadharNo = worksheet.Cells[row, 110].Text; // "Sibling Aadhar No"
                        string siblingInstituteName2 = worksheet.Cells[row, 111].Text; // Possibly duplicate "Institute Name"
                        string siblingBoard = worksheet.Cells[row, 112].Text; // "Sibling Board"
                        string siblingMedium = worksheet.Cells[row, 113].Text; // "Sibling Medium"
                        string siblingInstituteAddress = worksheet.Cells[row, 114].Text; // "Sibling Institute Address"
                        string siblingCourse = worksheet.Cells[row, 115].Text; // "Sibling Course"
                        string siblingClass2 = worksheet.Cells[row, 116].Text; // "Sibling Class (again)"
                        string siblingTCNumber = worksheet.Cells[row, 117].Text; // "Sibling TC Number"
                        string siblingTCDateText = worksheet.Cells[row, 118].Text; // "Sibling TC Date"

                        // ----- Health & Other Details -----
                        string allergies = worksheet.Cells[row, 119].Text; // "Allergies"
                        string medications = worksheet.Cells[row, 120].Text; // "Medications"
                        string consultingDoctorName = worksheet.Cells[row, 121].Text; // "Consulting Doctor's Name"
                        string consultingDoctorPhoneNo = worksheet.Cells[row, 122].Text; // "Consulting Doctor's Phone No"
                        string height = worksheet.Cells[row, 123].Text; // "Height"
                        string weight = worksheet.Cells[row, 124].Text; // "Weight"
                        string governmentHealthID = worksheet.Cells[row, 125].Text; // "Government Health ID"
                        string vision = worksheet.Cells[row, 126].Text; // "Vision"
                        string hearing = worksheet.Cells[row, 127].Text; // "Hearing"
                        string speech = worksheet.Cells[row, 128].Text; // "Speech"
                        string behavioralProblems = worksheet.Cells[row, 129].Text; // "Behaviourial Problems If Any"
                        string chestCondition = worksheet.Cells[row, 130].Text; // "Chest"
                        string historyOfAccidents = worksheet.Cells[row, 131].Text; // "History of Any Accident"
                        string physicalDeformities = worksheet.Cells[row, 132].Text; // "Any Physical Deformity"
                        string majorIllnessHistory = worksheet.Cells[row, 133].Text; // "History of Major Illness"
                        string otherRemarks = worksheet.Cells[row, 134].Text; // "Any other Remarks or Special Weakness"



                        //string registrationDateText = worksheet.Cells[row, 20].Text;  // "Registration Date"
                        //string registrationNo = worksheet.Cells[row, 21].Text;  // "Registration No"
                        //string admissionDateText = worksheet.Cells[row, 22].Text;  // "Admission Date"
                        //string samagraID = worksheet.Cells[row, 23].Text;  // "Samagra ID"
                        //string placeOfBirth = worksheet.Cells[row, 24].Text;  // "Place of Birth"
                        //string emailID = worksheet.Cells[row, 25].Text;  // "Email ID"
                        //string languagesKnown = worksheet.Cells[row, 26].Text;  // "Languages Known"
                        //string identificationMark1 = worksheet.Cells[row, 27].Text;  // "Identification Mark 1"
                        //string identificationMark2 = worksheet.Cells[row, 28].Text;  // "Identification Mark 2"

                        //// ----- Father Details -----
                        //string fatherFirstName = worksheet.Cells[row, 29].Text;  // "Father First Name"
                        //string fatherMiddleName = worksheet.Cells[row, 30].Text;  // "Father Middle Name"
                        //string fatherLastName = worksheet.Cells[row, 31].Text;  // "Father Last Name"
                        //string fatherPrimaryContactNo = worksheet.Cells[row, 32].Text;  // "Father Primary Contact No"
                        //string fatherBankAccountNo = worksheet.Cells[row, 33].Text;  // "Father Bank Account No"
                        //string fatherBankIFSCCode = worksheet.Cells[row, 34].Text;  // "Father Bank IFSC Code"
                        //string fatherFamilyRationCardType = worksheet.Cells[row, 35].Text; // "Father Family Ration Card Type"
                        //string fatherFamilyRationCardNo = worksheet.Cells[row, 36].Text; // "Father Family Ration Card No"
                        //string fatherDOBText = worksheet.Cells[row, 37].Text;  // "Father Date of Birth"
                        //string fatherAadharNo = worksheet.Cells[row, 38].Text;  // "Father Aadhar No"
                        //string fatherPANCardNo = worksheet.Cells[row, 39].Text;  // "Father PAN Card No"
                        //string fatherOccupation = worksheet.Cells[row, 40].Text;  // "Father Occupation"
                        //string fatherDesignation = worksheet.Cells[row, 41].Text;  // "Father Designation"
                        //string fatherEmployerName = worksheet.Cells[row, 42].Text;  // "Father Name of the Employer"
                        //string fatherOfficeNo = worksheet.Cells[row, 43].Text;  // "Father Office No"
                        //string fatherEmailID = worksheet.Cells[row, 44].Text;  // "Father Email ID"
                        //string fatherAnnualIncome = worksheet.Cells[row, 45].Text;  // "Father Annual Income"
                        //string fatherResidentialAddress = worksheet.Cells[row, 46].Text;  // "Father Residential Address"
                        //string fatherOfficeBuildingNo = worksheet.Cells[row, 47].Text;  // "Father Office Building No"
                        //string fatherStreetRoadLane = worksheet.Cells[row, 48].Text;  // "Father Street/Road/Lane"
                        //string fatherAreaLocalitySector = worksheet.Cells[row, 49].Text;  // "Father Area/Locality/Sector"
                        //string fatherState = worksheet.Cells[row, 50].Text;  // "Father State"
                        //string fatherCity = worksheet.Cells[row, 51].Text;  // "Father City"
                        //string fatherPincode = worksheet.Cells[row, 52].Text;  // "Father Pincode"

                        //// ----- Mother Details (if needed) -----
                        //string motherFirstName = worksheet.Cells[row, 53].Text;  // "Mother First Name"
                        //string motherMiddleName = worksheet.Cells[row, 54].Text;  // "Mother Middle Name"
                        //string motherLastName = worksheet.Cells[row, 55].Text;  // "Mother Last Name"
                        //string motherPrimaryContactNo = worksheet.Cells[row, 56].Text;  // "Mother Primary Contact No"
                        //string motherBankAccountNo = worksheet.Cells[row, 57].Text;  // "Mother Bank Account No"
                        //string motherBankIFSCCode = worksheet.Cells[row, 58].Text;  // "Mother Bank IFSC Code"
                        //string motherFamilyRationCardType = worksheet.Cells[row, 59].Text; // "Mother Family Ration Card Type"
                        //string motherFamilyRationCardNo = worksheet.Cells[row, 60].Text; // "Mother Family Ration Card No"
                        //string motherDOBText = worksheet.Cells[row, 61].Text;  // "Mother Date of Birth"
                        //string motherAadharNo = worksheet.Cells[row, 62].Text;  // "Mother Aadhar No"
                        //string motherPANCardNo = worksheet.Cells[row, 63].Text;  // "Mother PAN Card No"
                        //string motherOccupation = worksheet.Cells[row, 64].Text;  // "Mother Occupation"
                        //string motherDesignation = worksheet.Cells[row, 65].Text;  // "Mother Designation"
                        //string motherEmployerName = worksheet.Cells[row, 66].Text;  // "Mother Name of the Employer"
                        //string motherOfficeNo = worksheet.Cells[row, 67].Text;  // "Mother Office No"
                        //string motherEmailID = worksheet.Cells[row, 68].Text;  // "Mother Email ID"
                        //string motherAnnualIncome = worksheet.Cells[row, 69].Text;  // "Mother Annual Income"
                        //string motherResidentialAddress = worksheet.Cells[row, 70].Text;  // "Mother Residential Address"
                        //string motherOfficeBuildingNo = worksheet.Cells[row, 71].Text;  // "Mother Office Building No"
                        //string motherStreetRoadLane = worksheet.Cells[row, 72].Text;  // "Mother Street/Road/Lane"
                        //string motherAreaLocalitySector = worksheet.Cells[row, 73].Text;  // "Mother Area/Locality/Sector"
                        //string motherState = worksheet.Cells[row, 74].Text;  // "Mother State"
                        //string motherCity = worksheet.Cells[row, 75].Text;  // "Mother City"
                        //string motherPincode = worksheet.Cells[row, 76].Text;  // "Mother Pincode"

                        //// ----- Guardian Details (if needed) -----
                        //string guardianFirstName = worksheet.Cells[row, 77].Text;  // "Guardian First Name"
                        //string guardianMiddleName = worksheet.Cells[row, 78].Text;  // "Guardian Middle Name"
                        //string guardianLastName = worksheet.Cells[row, 79].Text;  // "Guardian Last Name"
                        //string guardianPrimaryContactNo = worksheet.Cells[row, 80].Text;  // "Guardian Primary Contact No"
                        //string guardianBankAccountNo = worksheet.Cells[row, 81].Text;  // "Guardian Bank Account No"
                        //string guardianBankIFSCCode = worksheet.Cells[row, 82].Text;  // "Guardian Bank IFSC Code"
                        //string guardianFamilyRationCardType = worksheet.Cells[row, 83].Text;  // "Guardian Family Ration Card Type"
                        //string guardianFamilyRationCardNo = worksheet.Cells[row, 84].Text;  // "Guardian Family Ration Card No"
                        //string guardianDOBText = worksheet.Cells[row, 85].Text;  // "Guardian Date of Birth"
                        //string guardianAadharNo = worksheet.Cells[row, 86].Text;  // "Guardian Aadhar No"
                        //string guardianPANCardNo = worksheet.Cells[row, 87].Text;  // "Guardian PAN Card No"
                        //string guardianOccupation = worksheet.Cells[row, 88].Text;  // "Guardian Occupation"
                        //string guardianDesignation = worksheet.Cells[row, 89].Text;  // "Guardian Designation"
                        //string guardianEmployerName = worksheet.Cells[row, 90].Text;  // "Guardian Name of the Employer"
                        //string guardianOfficeNo = worksheet.Cells[row, 91].Text;  // "Guardian Office No"
                        //string guardianEmailID = worksheet.Cells[row, 92].Text;  // "Guardian Email ID"
                        //string guardianAnnualIncome = worksheet.Cells[row, 93].Text;  // "Guardian Annual Income"
                        //string guardianResidentialAddress = worksheet.Cells[row, 94].Text;  // "Guardian Residential Address"
                        //string guardianOfficeBuildingNo = worksheet.Cells[row, 95].Text;  // "Guardian Office Building No"
                        //string guardianStreetRoadLane = worksheet.Cells[row, 96].Text;  // "Guardian Street/Road/Lane"
                        //string guardianAreaLocalitySector = worksheet.Cells[row, 97].Text;  // "Guardian Area/Locality/Sector"
                        //string guardianState = worksheet.Cells[row, 98].Text;  // "Guardian State"
                        //string guardianCity = worksheet.Cells[row, 99].Text;  // "Guardian City"
                        //string guardianPincode = worksheet.Cells[row, 100].Text; // "Guardian Pincode"

                        //// ----- Sibling Details -----
                        //string siblingFirstName = worksheet.Cells[row, 101].Text; // "Sibling First Name"
                        //string siblingMiddleName = worksheet.Cells[row, 102].Text; // "Sibling Middle Name"
                        //string siblingLastName = worksheet.Cells[row, 103].Text; // "Sibling Last Name"
                        //string siblingAdmissionNumber = worksheet.Cells[row, 104].Text; // "Sibling Admission No"
                        //string siblingDOBText = worksheet.Cells[row, 105].Text; // "Sibling Date of Birth"
                        //string siblingClass = worksheet.Cells[row, 106].Text; // "Sibling Class"
                        //string siblingSection = worksheet.Cells[row, 107].Text; // "Sibling Section"
                        //string siblingInstituteName = worksheet.Cells[row, 108].Text; // "Sibling Institute Name"
                        //string siblingAadharNo = worksheet.Cells[row, 109].Text; // "Sibling Aadhar No"
                        //string siblingInstituteName2 = worksheet.Cells[row, 110].Text; // Possibly duplicate "Institute Name"
                        //string siblingBoard = worksheet.Cells[row, 111].Text; // "Sibling Board"
                        //string siblingMedium = worksheet.Cells[row, 112].Text; // "Sibling Medium"
                        //string siblingInstituteAddress = worksheet.Cells[row, 113].Text; // "Sibling Institute Address"
                        //string siblingCourse = worksheet.Cells[row, 114].Text; // "Sibling Course"
                        //string siblingClass2 = worksheet.Cells[row, 115].Text; // "Sibling Class (again)"
                        //string siblingTCNumber = worksheet.Cells[row, 116].Text; // "Sibling TC Number"
                        //string siblingTCDateText = worksheet.Cells[row, 117].Text; // "Sibling TC Date"

                        //// ----- Health & Other Details -----
                        //string allergies = worksheet.Cells[row, 118].Text; // "Allergies"
                        //string medications = worksheet.Cells[row, 119].Text; // "Medications"
                        //string consultingDoctorName = worksheet.Cells[row, 120].Text; // "Consulting Doctor's Name"
                        //string consultingDoctorPhoneNo = worksheet.Cells[row, 121].Text; // "Consulting Doctor's Phone No"
                        //string height = worksheet.Cells[row, 122].Text; // "Height"
                        //string weight = worksheet.Cells[row, 123].Text; // "Weight"
                        //string governmentHealthID = worksheet.Cells[row, 124].Text; // "Government Health ID"
                        //string vision = worksheet.Cells[row, 125].Text; // "Vision"
                        //string hearing = worksheet.Cells[row, 126].Text; // "Hearing"
                        //string speech = worksheet.Cells[row, 127].Text; // "Speech"
                        //string behavioralProblems = worksheet.Cells[row, 128].Text; // "Behaviourial Problems If Any"
                        //string chestCondition = worksheet.Cells[row, 129].Text; // "Chest"
                        //string historyOfAccidents = worksheet.Cells[row, 130].Text; // "History of Any Accident"
                        //string physicalDeformities = worksheet.Cells[row, 131].Text; // "Any Physical Deformity"
                        //string majorIllnessHistory = worksheet.Cells[row, 132].Text; // "History of Major Illness"
                        //string otherRemarks = worksheet.Cells[row, 133].Text; // "Any other Remarks or Special Weakness"






                        var compositeKey = $"{classText.Trim()}|{sectionText.Trim()}";

                        //// Validate DateOfJoining format 'DD-MM-YYYY'
                        //if (!DateTime.TryParseExact(dateOfJoiningText, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfJoining))
                        //{
                        //    errorMessages.Add($"{firstName} {lastName} has an invalid Date of Joining format ('{dateOfJoiningText}'). Expected format is 'DD-MM-YYYY'.");
                        //}

                        //// Validate DateOfBirth format 'DD-MM-YYYY'
                        //else if (!DateTime.TryParseExact(dateOfBirthText, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth))
                        //{
                        //    errorMessages.Add($"{firstName} {lastName} has an invalid Date of Birth format ('{dateOfBirthText}'). Expected format is 'DD-MM-YYYY'.");
                        //}


                        // Inside your while loop, after reading the cell values

                        // Validate the date fields. If a date value is provided, it must follow "dd-MM-yyyy".
                        if (!string.IsNullOrWhiteSpace(dateOfJoiningText) &&
                            !DateTime.TryParseExact(dateOfJoiningText, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfJoining))
                        {
                            errorMessages.Add($"{firstName} {lastName} has an invalid DateOfJoining format ('{dateOfJoiningText}'). Expected format is 'DD-MM-YYYY'.");
                        }

                        if (!string.IsNullOrWhiteSpace(dateOfBirthText) &&
                            !DateTime.TryParseExact(dateOfBirthText, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth))
                        {
                            errorMessages.Add($"{firstName} {lastName} has an invalid DateOfBirth format ('{dateOfBirthText}'). Expected format is 'DD-MM-YYYY'.");
                        }

                        if (!string.IsNullOrWhiteSpace(registrationDateText) &&
                            !DateTime.TryParseExact(registrationDateText, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime registrationDate))
                        {
                            errorMessages.Add($"{firstName} {lastName} has an invalid RegistrationDate format ('{registrationDateText}'). Expected format is 'DD-MM-YYYY'.");
                        }

                        if (!string.IsNullOrWhiteSpace(admissionDateText) &&
                            !DateTime.TryParseExact(admissionDateText, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime admissionDate))
                        {
                            errorMessages.Add($"{firstName} {lastName} has an invalid AdmissionDate format ('{admissionDateText}'). Expected format is 'DD-MM-YYYY'.");
                        }

                        if (!string.IsNullOrWhiteSpace(fatherDOBText) &&
                            !DateTime.TryParseExact(fatherDOBText, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fatherDOB))
                        {
                            errorMessages.Add($"{firstName} {lastName} has an invalid FatherDateOfBirth format ('{fatherDOBText}'). Expected format is 'DD-MM-YYYY'.");
                        }

                        if (!string.IsNullOrWhiteSpace(motherDOBText) &&
                            !DateTime.TryParseExact(motherDOBText, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime motherDOB))
                        {
                            errorMessages.Add($"{firstName} {lastName} has an invalid MotherDateOfBirth format ('{motherDOBText}'). Expected format is 'DD-MM-YYYY'.");
                        }

                        if (!string.IsNullOrWhiteSpace(guardianDOBText) &&
                            !DateTime.TryParseExact(guardianDOBText, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime guardianDOB))
                        {
                            errorMessages.Add($"{firstName} {lastName} has an invalid GuardianDateOfBirth format ('{guardianDOBText}'). Expected format is 'DD-MM-YYYY'.");
                        }

                        if (!string.IsNullOrWhiteSpace(siblingTCDateText) &&
                            !DateTime.TryParseExact(siblingTCDateText, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime tcDate))
                        {
                            errorMessages.Add($"{firstName} {lastName} has an invalid TCDate format ('{siblingTCDateText}'). Expected format is 'DD-MM-YYYY'.");
                        }



                        // Validate the gender.
                        if (string.IsNullOrWhiteSpace(genderText) || !genderLookup.TryGetValue(genderText, out int foundGenderID))
                        {
                            errorMessages.Add($"{firstName} {lastName} mentioned GenderType '{genderText}' is not available");
                        }
                        // Validate the Class.
                        else if (string.IsNullOrWhiteSpace(classText) || !ClassesLookup.TryGetValue(classText, out int foundClassID))
                        {
                            errorMessages.Add($"{firstName} {lastName} mentioned Class Name '{classText}' is not available");
                        }
                        // Validate the Sections.
                        //else if (string.IsNullOrWhiteSpace(SectionText) || !SectionsLookup.TryGetValue(SectionText, out int foundSectionID))
                        //{
                        //    errorMessages.Add($"{firstName} {lastName} mentioned Section Name '{SectionText}' is not available");
                        //} 
                        else if (string.IsNullOrWhiteSpace(sectionText) || !SectionsLookup.TryGetValue(compositeKey, out int foundSectionID))
                        {
                            errorMessages.Add($"{firstName} {lastName} mentioned Section Name '{sectionText}' for Class '{classText}' is not available");
                        }
                        // Validate the Nationalities.
                        else if (string.IsNullOrWhiteSpace(NationalityText) || !NationalityLookup.TryGetValue(NationalityText, out int foundNationalityID))
                        {
                            errorMessages.Add($"{firstName} {lastName} mentioned Nationality '{NationalityText}' is not available");
                        } 
                        // Validate the Religions.
                        else if (string.IsNullOrWhiteSpace(ReligionsText) || !ReligionsLookup.TryGetValue(ReligionsText, out int foundReligionsID))
                        {
                            errorMessages.Add($"{firstName} {lastName} mentioned Religion '{ReligionsText}' is not available");
                        }
                        // Validate the MotherTongues.
                        else if (string.IsNullOrWhiteSpace(MotherTonguesText) || !MotherTonguesLookup.TryGetValue(MotherTonguesText, out int foundMotherTonguesID))
                        {
                            errorMessages.Add($"{firstName} {lastName} mentioned Mother Tongue '{MotherTonguesText}' is not available");
                        }
                        // Validate the Castes.
                        else if (string.IsNullOrWhiteSpace(CastesText) || !CastesLookup.TryGetValue(CastesText, out int foundCastesID))
                        {
                            errorMessages.Add($"{firstName} {lastName} mentioned Caste '{CastesText}' is not available");
                        }
                        // Validate the BloodGroups.
                        else if (string.IsNullOrWhiteSpace(BloodGroupsText) || !BloodGroupsLookup.TryGetValue(BloodGroupsText, out int foundBloodGroupsID))
                        {
                            errorMessages.Add($"{firstName} {lastName} mentioned Blood Group '{BloodGroupsText}' is not available");
                        }
                        // Validate the StudentTypes.
                        else if (string.IsNullOrWhiteSpace(StudentTypesText) || !StudentTypesLookup.TryGetValue(StudentTypesText, out int foundStudentTypesID))
                        {
                            errorMessages.Add($"{firstName} {lastName} mentioned Student Type '{StudentTypesText}' is not available");
                        }
                        //Validate the InstituteHouses.
                        else if (string.IsNullOrWhiteSpace(StudentHouseText) || !InstituteHousesLookup.TryGetValue(StudentHouseText, out int foundStudentHouseID))
                        {
                            errorMessages.Add($"{firstName} {lastName} mentioned Student House '{StudentHouseText}' is not available");
                        } 
                        else
                        {
                            // If valid, create the student object.
                            var student = new StudentInformationImportRequest
                            {
                                StudentDetails = new StudentDetails_IM
                                {
                                    AdmissionNumber = AdmissionNumber,
                                    FirstName = firstName,
                                    MiddleName = middleName,
                                    LastName = lastName,
                                    GenderID = foundGenderID,         // from gender lookup
                                    ClassID = foundClassID,           // from ClassesLookup
                                    SectionID = foundSectionID,       // from SectionsLookup (using compositeKey)
                                    RollNumber = RollNumber,
                                    DateOfJoining = dateOfJoiningText,  // Consider parsing/formatting as needed
                                    AcademicYear = academicYear,
                                    NationalityID = foundNationalityID, // from NationalityLookup
                                    ReligionID = foundReligionsID,      // from ReligionsLookup
                                    DateOfBirth = dateOfBirthText,
                                    MotherTongueID = foundMotherTonguesID, // from MotherTonguesLookup
                                    CasteID = foundCastesID,               // from CastesLookup
                                    BloodGroupID = foundBloodGroupsID,     // from BloodGroupsLookup
                                    AadharNo = AadharNoText,
                                    PEN = PENText,
                                    StudentTypeID = foundStudentTypesID,   // from StudentTypesLookup
                                    StudentHouseID = foundStudentHouseID,   // StudentHouseID could be mapped similarly if available (e.g., from InstituteHousesLookup)
                                },
                                OtherInformation = new OtherInformation_IM
                                {
                                    RegistrationDate = registrationDateText,
                                    RegistrationNo = registrationNo,
                                    AdmissionDate = admissionDateText,
                                    SamagraID = samagraID,
                                    PlaceofBirth = placeOfBirth,
                                    EmailID = emailID,
                                    LanguageKnown = languagesKnown,
                                    IdentificationMark1 = identificationMark1,
                                    IdentificationMark2 = identificationMark2,
                                    Comments = "" // Optional additional comments
                                },
                                // For this example, we map the Father's details into ParentsInfo.
                                // (You may choose to extend the DTO to include separate Mother/Guardian objects.)
                                //ParentsInfo = new ParentsInfo_IM
                                //{
                                //    FirstName = fatherFirstName,
                                //    MiddleName = fatherMiddleName,
                                //    LastName = fatherLastName,
                                //    PrimaryContactNo = fatherPrimaryContactNo,
                                //    BankAccountNo = fatherBankAccountNo,
                                //    BankIFSCCode = fatherBankIFSCCode,
                                //    FamilyRationCardType = fatherFamilyRationCardType,
                                //    FamilyRationCardNo = fatherFamilyRationCardNo,
                                //    DateOfBirth = fatherDOBText,
                                //    AadharNo = fatherAadharNo,
                                //    PANCardNo = fatherPANCardNo,
                                //    Occupation = fatherOccupation,
                                //    Designation = fatherDesignation,
                                //    NameoftheEmployer = fatherEmployerName,
                                //    OfficeNo = fatherOfficeNo,
                                //    EmailID = fatherEmailID,
                                //    AnnualIncome = decimal.TryParse(fatherAnnualIncome, out decimal dadIncome) ? dadIncome : 0,
                                //    ResidentialAddress = fatherResidentialAddress
                                //},

                                ParentsInfo = new ParentsInfo_IM
                                {
                                    // Father Info
                                    FatherFirstName = fatherFirstName,
                                    FatherMiddleName = fatherMiddleName,
                                    FatherLastName = fatherLastName,
                                    FatherPrimaryContactNo = fatherPrimaryContactNo,
                                    FatherBankAccountNo = fatherBankAccountNo,
                                    FatherBankIFSCCode = fatherBankIFSCCode,
                                    FatherFamilyRationCardType = fatherFamilyRationCardType,
                                    FatherFamilyRationCardNo = fatherFamilyRationCardNo,
                                    FatherDateOfBirth = fatherDOBText,
                                    FatherAadharNo = fatherAadharNo,
                                    FatherPANCardNo = fatherPANCardNo,
                                    FatherOccupation = fatherOccupation,
                                    FatherDesignation = fatherDesignation,
                                    FatherNameoftheEmployer = fatherEmployerName,
                                    FatherOfficeNo = fatherOfficeNo,
                                    FatherEmailID = fatherEmailID,
                                    FatherAnnualIncome = decimal.TryParse(fatherAnnualIncome, out decimal dadIncome) ? dadIncome : 0,
                                    FatherResidentialAddress = fatherResidentialAddress,

                                    // Mother Info
                                    MotherFirstName = motherFirstName,
                                    MotherMiddleName = motherMiddleName,
                                    MotherLastName = motherLastName,
                                    MotherPrimaryContactNo = motherPrimaryContactNo,
                                    MotherBankAccountNo = motherBankAccountNo,
                                    MotherBankIFSCCode = motherBankIFSCCode,
                                    MotherFamilyRationCardType = motherFamilyRationCardType,
                                    MotherFamilyRationCardNo = motherFamilyRationCardNo,
                                    MotherDateOfBirth = motherDOBText,
                                    MotherAadharNo = motherAadharNo,
                                    MotherPANCardNo = motherPANCardNo,
                                    MotherOccupation = motherOccupation,
                                    MotherDesignation = motherDesignation,
                                    MotherNameoftheEmployer = motherEmployerName,
                                    MotherOfficeNo = motherOfficeNo,
                                    MotherEmailID = motherEmailID,
                                    MotherAnnualIncome = decimal.TryParse(motherAnnualIncome, out decimal momIncome) ? momIncome : 0,
                                    MotherResidentialAddress = motherResidentialAddress,

                                    // Guardian Info
                                    GuardianFirstName = guardianFirstName,
                                    GuardianMiddleName = guardianMiddleName,
                                    GuardianLastName = guardianLastName,
                                    GuardianPrimaryContactNo = guardianPrimaryContactNo,
                                    GuardianBankAccountNo = guardianBankAccountNo,
                                    GuardianBankIFSCCode = guardianBankIFSCCode,
                                    GuardianFamilyRationCardType = guardianFamilyRationCardType,
                                    GuardianFamilyRationCardNo = guardianFamilyRationCardNo,
                                    GuardianDateOfBirth = guardianDOBText,
                                    GuardianAadharNo = guardianAadharNo,
                                    GuardianPANCardNo = guardianPANCardNo,
                                    GuardianOccupation = guardianOccupation,
                                    GuardianDesignation = guardianDesignation,
                                    GuardianNameoftheEmployer = guardianEmployerName,
                                    GuardianOfficeNo = guardianOfficeNo,
                                    GuardianEmailID = guardianEmailID,
                                    GuardianAnnualIncome = decimal.TryParse(guardianAnnualIncome, out decimal guardianIncome) ? guardianIncome : 0,
                                    GuardianResidentialAddress = guardianResidentialAddress
                                },


                            SiblingsDetails = new SiblingsDetails_IM
                                {
                                    FirstName = siblingFirstName,
                                    MiddleName = siblingMiddleName,
                                    LastName = siblingLastName,
                                    AdmissionNo = siblingAdmissionNumber,
                                    DateOfBirth = siblingDOBText,
                                    Class = siblingClass,
                                    Section = siblingSection,
                                    InstituteName = siblingInstituteName,
                                    AadharNo = siblingAadharNo
                                },
                                PreviousSchoolDetails = new PreviousSchoolDetails_IM
                                {
                                    InstituteName = siblingInstituteName2,  // Using duplicate field as previous school name
                                    Board = siblingBoard,
                                    Medium = siblingMedium,
                                    InstituteAddress = siblingInstituteAddress,
                                    Course = siblingCourse,
                                    Class = siblingClass2,
                                    TCNumber = siblingTCNumber,
                                    TCDate = siblingTCDateText,
                                    IsTCSubmitted = false  // Set as needed
                                },
                                HealthInformation = new HealthInformation_IM
                                {
                                    Allergies = allergies,
                                    Medications = medications,
                                    ConsultingDoctorsName = consultingDoctorName,
                                    ConsultingDoctorPhoneNumber = consultingDoctorPhoneNo,
                                    Height = height,
                                    Weight = weight,
                                    GovermentHealthID = governmentHealthID,
                                    // Convert Vision to int or default to 0 if invalid
                                    Vision = int.TryParse(vision, out int visionVal) ? visionVal : 0,
                                    Hearing = hearing,
                                    Speech = speech,
                                    BehavioralProblems = behavioralProblems,
                                    Chest = chestCondition,
                                    HistoryofanyAccident = historyOfAccidents,
                                    AnyPhysicalDeformiity = physicalDeformities,
                                    HistoryofMajorIllness = majorIllnessHistory,
                                    AnyOtherRemarksOrWeakness = otherRemarks
                                },
                                Documents = new Documents_IM
                                {
                                    DocumentFile = "" // Populate if applicable
                                }
                            };


                            //    StudentDetails = new StudentDetails_IM
                            //    {
                            //        FirstName = firstName,
                            //        MiddleName = middleName,
                            //        LastName = lastName,
                            //        GenderID = foundGenderID, 
                            //        ClassID = foundClassID,
                            //        SectionID = foundSectionID,
                            //        AdmissionNumber = AdmissionNumber,
                            //        RollNumber = RollNumber,
                            //        NationalityID = foundNationalityID,
                            //        ReligionID = foundReligionsID,
                            //        MotherTongueID = foundMotherTonguesID,
                            //        CasteID = foundCastesID, 
                            //        BloodGroupID = foundBloodGroupsID,
                            //        StudentTypeID = foundStudentTypesID,
                            //        //StudentHouseID = foundInstituteHousesID,
                            //        AadharNo = AadharNoText,
                            //        PEN = PENText,
                            //        //DateOfBirth = dateOfBirthText,
                            //        //DateOfJoining = dateOfJoiningText

                            //        // Additional fields can be parsed here.
                            //    }
                            //};
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
         
        public async Task<ServiceResponse<IEnumerable<GetStudentSettingResponse>>> GetStudentSetting(GetStudentSettingRequest request)
        {
            return await _studentInformationRepository.GetStudentSetting(request);
        }

        public async Task<ServiceResponse<string>> AddRemoveStudentSetting(AddRemoveStudentSettingRequest request)
        {
            return await _studentInformationRepository.AddRemoveStudentSetting(request);
        }
    }
}
