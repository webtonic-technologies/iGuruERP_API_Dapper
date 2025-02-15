using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Response.StudentManagement;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;
using StudentManagement_API.Repository.Interfaces;

namespace StudentManagement_API.Repository.Implementations
{
    public class StudentInformationRepository : IStudentInformationRepository
    {
        private readonly IDbConnection _dbConnection;

        public StudentInformationRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateStudent(AddUpdateStudentRequest request)
        {
            try
            {
                // Ensure the connection is open
                if (_dbConnection.State == ConnectionState.Closed)
                    _dbConnection.Open();

                using (var transaction = _dbConnection.BeginTransaction())
                {
                    // Convert date strings for StudentDetails using the "dd-MM-yyyy" format.
                    if (!DateTime.TryParseExact(request.StudentDetails.DateOfJoining, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfJoining))
                    {
                        return new ServiceResponse<string>(false, "Invalid DateOfJoining format", "Failure", 400);
                    }
                    if (!DateTime.TryParseExact(request.StudentDetails.DateOfBirth, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth))
                    {
                        return new ServiceResponse<string>(false, "Invalid DateOfBirth format", "Failure", 400);
                    }

                    // Convert ParentsInfo date
                    if (!DateTime.TryParseExact(request.ParentsInfo.DateOfBirth, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parentDateOfBirth))
                    {
                        return new ServiceResponse<string>(false, "Invalid Parent DateOfBirth format", "Failure", 400);
                    }

                    // Convert SiblingsDetails date if provided.
                    DateTime siblingDOB = DateTime.MinValue;
                    if (request.SiblingsDetails != null && !string.IsNullOrWhiteSpace(request.SiblingsDetails.DateOfBirth))
                    {
                        if (!DateTime.TryParseExact(request.SiblingsDetails.DateOfBirth, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out siblingDOB))
                        {
                            return new ServiceResponse<string>(false, "Invalid Sibling DateOfBirth format", "Failure", 400);
                        }
                    }

                    // For PreviousSchoolDetails, convert TCDate if provided.
                    DateTime tcDate = DateTime.MinValue;
                    bool tcDateProvided = request.PreviousSchoolDetails != null &&
                                            !string.IsNullOrWhiteSpace(request.PreviousSchoolDetails.TCDate);
                    if (tcDateProvided)
                    {
                        if (!DateTime.TryParseExact(request.PreviousSchoolDetails.TCDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tcDate))
                        {
                            return new ServiceResponse<string>(false, "Invalid TCDate format", "Failure", 400);
                        }
                    }

                    if (request.StudentID == 0)
                    {
                        // Insert into tbl_StudentMaster
                        string insertMasterQuery = @"
                    INSERT INTO tbl_StudentMaster
                        (First_Name, Middle_Name, Last_Name, gender_id, class_id, section_id, Admission_Number, Roll_Number, Date_of_Joining, AcademicYearCode, Nationality_id, Religion_id, Date_of_Birth, Mother_Tongue_id, Caste_id, Blood_Group_id, Aadhar_Number, PEN, StudentType_id, Institute_house_id)
                    VALUES
                        (@FirstName, @MiddleName, @LastName, @GenderID, @ClassID, @SectionID, @AdmissionNumber, @RollNumber, @DateOfJoining, @AcademicYear, @NationalityID, @ReligionID, @Date_of_Birth, @MotherTongueID, @CasteID, @BloodGroupID, @AadharNo, @PEN, @StudentTypeID, @StudentHouseID);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                        int newStudentId = await _dbConnection.QuerySingleAsync<int>(
                            insertMasterQuery,
                            new
                            {
                                request.StudentDetails.FirstName,
                                request.StudentDetails.MiddleName,
                                request.StudentDetails.LastName,
                                request.StudentDetails.GenderID,
                                request.StudentDetails.ClassID,
                                request.StudentDetails.SectionID,
                                request.StudentDetails.AdmissionNumber,
                                request.StudentDetails.RollNumber,
                                DateOfJoining = dateOfJoining,
                                AcademicYear = request.StudentDetails.AcademicYear,
                                request.StudentDetails.NationalityID,
                                request.StudentDetails.ReligionID,
                                Date_of_Birth = dateOfBirth,
                                request.StudentDetails.MotherTongueID,
                                request.StudentDetails.CasteID,
                                request.StudentDetails.BloodGroupID,
                                request.StudentDetails.AadharNo,
                                request.StudentDetails.PEN,
                                request.StudentDetails.StudentTypeID,
                                request.StudentDetails.StudentHouseID
                            },
                            transaction
                        );
                        request.StudentID = newStudentId;

                        // Insert into tbl_StudentOtherInfo using OtherInformation
                        if (!DateTime.TryParseExact(request.OtherInformation.RegistrationDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime registrationDate))
                        {
                            return new ServiceResponse<string>(false, "Invalid RegistrationDate format", "Failure", 400);
                        }
                        if (!DateTime.TryParseExact(request.OtherInformation.AdmissionDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime admissionDate))
                        {
                            return new ServiceResponse<string>(false, "Invalid AdmissionDate format", "Failure", 400);
                        }

                        string insertOtherInfoQuery = @"
                    INSERT INTO tbl_StudentOtherInfo
                        (student_id, Register_Date, Register_Number, Admission_Date, samagra_ID, Place_of_Birth, email_id, language_known, comments, Identification_Mark_1, Identification_Mark_2)
                    VALUES
                        (@StudentID, @Register_Date, @Register_Number, @Admission_Date, @samagra_ID, @Place_of_Birth, @email_id, @language_known, @comments, @Identification_Mark_1, @Identification_Mark_2);";

                        await _dbConnection.ExecuteAsync(
                            insertOtherInfoQuery,
                            new
                            {
                                StudentID = request.StudentID,
                                Register_Date = registrationDate,
                                Register_Number = request.OtherInformation.RegistrationNo,
                                Admission_Date = admissionDate,
                                samagra_ID = request.OtherInformation.SamagraID,
                                Place_of_Birth = request.OtherInformation.PlaceofBirth,
                                email_id = request.OtherInformation.EmailID,
                                language_known = request.OtherInformation.LanguageKnown,
                                comments = request.OtherInformation.Comments,
                                Identification_Mark_1 = request.OtherInformation.IdentificationMark1,
                                Identification_Mark_2 = request.OtherInformation.IdentificationMark2
                            },
                            transaction
                        );

                        // Insert into tbl_StudentParentsInfo using ParentsInfo
                        string insertParentsInfoQuery = @"
                    INSERT INTO tbl_StudentParentsInfo
                        (student_id, Parent_Type_id, First_Name, Middle_Name, Last_Name, Mobile_Number, Bank_Account_no, Bank_IFSC_Code, Family_Ration_Card_Type, Family_Ration_Card_no, Date_of_Birth, Aadhar_no, PAN_card_no, Residential_Address, Designation, Name_of_the_Employer, Office_no, Email_id, Annual_Income, File_Name, Occupation)
                    VALUES
                        (@StudentID, @Parent_Type_id, @First_Name, @Middle_Name, @Last_Name, @Mobile_Number, @Bank_Account_no, @Bank_IFSC_Code, @Family_Ration_Card_Type, @Family_Ration_Card_no, @Date_of_Birth, @Aadhar_no, @PAN_card_no, @Residential_Address, @Designation, @Name_of_the_Employer, @Office_no, @Email_id, @Annual_Income, @File_Name, @Occupation);";

                        await _dbConnection.ExecuteAsync(
                            insertParentsInfoQuery,
                            new
                            {
                                StudentID = request.StudentID,
                                Parent_Type_id = 1, // Default value (adjust as needed)
                                First_Name = request.ParentsInfo.FirstName,
                                Middle_Name = request.ParentsInfo.MiddleName,
                                Last_Name = request.ParentsInfo.LastName,
                                Mobile_Number = request.ParentsInfo.PrimaryContactNo,
                                Bank_Account_no = request.ParentsInfo.BankAccountNo,
                                Bank_IFSC_Code = request.ParentsInfo.BankIFSCCode,
                                Family_Ration_Card_Type = request.ParentsInfo.FamilyRationCardType,
                                Family_Ration_Card_no = request.ParentsInfo.FamilyRationCardNo,
                                Date_of_Birth = parentDateOfBirth,
                                Aadhar_no = request.ParentsInfo.AadharNo,
                                PAN_card_no = request.ParentsInfo.PANCardNo,
                                Residential_Address = request.ParentsInfo.ResidentialAddress,
                                Designation = request.ParentsInfo.Designation,
                                Name_of_the_Employer = request.ParentsInfo.NameoftheEmployer,
                                Office_no = request.ParentsInfo.OfficeNo,
                                Email_id = request.ParentsInfo.EmailID,
                                Annual_Income = request.ParentsInfo.AnnualIncome,
                                File_Name = (string)null,
                                Occupation = request.ParentsInfo.Occupation
                            },
                            transaction
                        );

                        // Insert into tbl_StudentSiblings using SiblingsDetails (if provided)
                        if (request.SiblingsDetails != null)
                        {
                            string insertSiblingsQuery = @"
                        INSERT INTO tbl_StudentSiblings
                            (student_id, Name, Middle_Name, Last_Name, Admission_Number, Date_of_Birth, Institute_Name, Aadhar_no, Class, section)
                        VALUES
                            (@StudentID, @FirstName, @MiddleName, @LastName, @AdmissionNo, @Date_of_Birth, @InstituteName, @AadharNo, @Class, @Section);";

                            await _dbConnection.ExecuteAsync(
                                insertSiblingsQuery,
                                new
                                {
                                    StudentID = request.StudentID,
                                    FirstName = request.SiblingsDetails.FirstName,
                                    MiddleName = request.SiblingsDetails.MiddleName,
                                    LastName = request.SiblingsDetails.LastName,
                                    AdmissionNo = request.SiblingsDetails.AdmissionNo,
                                    Date_of_Birth = siblingDOB,  // Already converted above
                                    InstituteName = request.SiblingsDetails.InstituteName,
                                    AadharNo = request.SiblingsDetails.AadharNo,
                                    Class = request.SiblingsDetails.Class,
                                    Section = request.SiblingsDetails.Section
                                },
                                transaction
                            );
                        }

                        // Insert into tbl_StudentPreviousSchool using PreviousSchoolDetails (if provided)
                        if (request.PreviousSchoolDetails != null)
                        {
                            // Ensure TCDate is provided and valid
                            if (!tcDateProvided)
                            {
                                return new ServiceResponse<string>(false, "TCDate is required in PreviousSchoolDetails", "Failure", 400);
                            }

                            string insertPreviousSchoolQuery = @"
                        INSERT INTO tbl_StudentPreviousSchool
                            (student_id, Previous_School_Name, Previous_Board, Previous_Medium, Previous_School_Address, previous_School_Course, Previous_Class, TC_number, TC_date, isTC_Submitted)
                        VALUES
                            (@StudentID, @Previous_School_Name, @Previous_Board, @Previous_Medium, @Previous_School_Address, @previous_School_Course, @Previous_Class, @TC_number, @TC_date, @isTC_Submitted);";

                            await _dbConnection.ExecuteAsync(
                                insertPreviousSchoolQuery,
                                new
                                {
                                    StudentID = request.StudentID,
                                    Previous_School_Name = request.PreviousSchoolDetails.InstituteName,
                                    Previous_Board = request.PreviousSchoolDetails.Board,
                                    Previous_Medium = request.PreviousSchoolDetails.Medium,
                                    Previous_School_Address = request.PreviousSchoolDetails.InstituteAddress,
                                    previous_School_Course = request.PreviousSchoolDetails.Course,
                                    Previous_Class = request.PreviousSchoolDetails.Class,
                                    TC_number = request.PreviousSchoolDetails.TCNumber,
                                    TC_date = tcDate,
                                    isTC_Submitted = request.PreviousSchoolDetails.IsTCSubmitted
                                },
                                transaction
                            );
                        }

                        // Insert into tbl_StudentHealthInfo using HealthInformation (if provided)
                        if (request.HealthInformation != null)
                        {
                            string insertHealthInfoQuery = @"
                        INSERT INTO tbl_StudentHealthInfo
                            (student_id, Allergies, Medications, Doctor_Name, Doctor_Phone_no, height, weight, Government_ID, Chest, Physical_Deformity, History_Majorillness, History_Accident, Vision, Hearing, Speech, Behavioral_Problem, Remarks_Weakness)
                        VALUES
                            (@StudentID, @Allergies, @Medications, @Doctor_Name, @Doctor_Phone_no, @height, @weight, @Government_ID, @Chest, @Physical_Deformity, @History_Majorillness, @History_Accident, @Vision, @Hearing, @Speech, @Behavioral_Problem, @Remarks_Weakness);";

                            await _dbConnection.ExecuteAsync(
                                insertHealthInfoQuery,
                                new
                                {
                                    StudentID = request.StudentID,
                                    Allergies = request.HealthInformation.Allergies,
                                    Medications = request.HealthInformation.Medications,
                                    Doctor_Name = request.HealthInformation.ConsultingDoctorsName,
                                    Doctor_Phone_no = request.HealthInformation.ConsultingDoctorPhoneNumber,
                                    height = request.HealthInformation.Height.ToString(),
                                    weight = request.HealthInformation.Weight.ToString(),
                                    Government_ID = request.HealthInformation.GovermentHealthID,
                                    Chest = request.HealthInformation.Chest,
                                    Physical_Deformity = request.HealthInformation.AnyPhysicalDeformiity,
                                    History_Majorillness = request.HealthInformation.HistoryofMajorIllness,
                                    History_Accident = request.HealthInformation.HistoryofanyAccident,
                                    Vision = request.HealthInformation.Vision,
                                    Hearing = request.HealthInformation.Hearing,
                                    Speech = request.HealthInformation.Speech,
                                    Behavioral_Problem = request.HealthInformation.BehavioralProblems,
                                    Remarks_Weakness = request.HealthInformation.AnyOtherRemarksOrWeakness
                                },
                                transaction
                            );
                        }
                    }
                    else
                    {
                        // Update tbl_StudentMaster
                        string updateMasterQuery = @"
                    UPDATE tbl_StudentMaster SET 
                        First_Name = @FirstName,
                        Middle_Name = @MiddleName,
                        Last_Name = @LastName,
                        gender_id = @GenderID,
                        class_id = @ClassID,
                        section_id = @SectionID,
                        Admission_Number = @AdmissionNumber,
                        Roll_Number = @RollNumber,
                        Date_of_Joining = @DateOfJoining,
                        AcademicYearCode = @AcademicYear,
                        Nationality_id = @NationalityID,
                        Religion_id = @ReligionID,
                        Date_of_Birth = @DateOfBirth,
                        Mother_Tongue_id = @MotherTongueID,
                        Caste_id = @CasteID,
                        Blood_Group_id = @BloodGroupID,
                        Aadhar_Number = @AadharNo,
                        PEN = @PEN,
                        StudentType_id = @StudentTypeID,
                        Institute_house_id = @StudentHouseID
                    WHERE student_id = @StudentID";

                        await _dbConnection.ExecuteAsync(
                            updateMasterQuery,
                            new
                            {
                                request.StudentDetails.FirstName,
                                request.StudentDetails.MiddleName,
                                request.StudentDetails.LastName,
                                request.StudentDetails.GenderID,
                                request.StudentDetails.ClassID,
                                request.StudentDetails.SectionID,
                                request.StudentDetails.AdmissionNumber,
                                request.StudentDetails.RollNumber,
                                DateOfJoining = dateOfJoining,
                                AcademicYear = request.StudentDetails.AcademicYear,
                                request.StudentDetails.NationalityID,
                                request.StudentDetails.ReligionID,
                                Date_of_Birth = dateOfBirth,
                                request.StudentDetails.MotherTongueID,
                                request.StudentDetails.CasteID,
                                request.StudentDetails.BloodGroupID,
                                request.StudentDetails.AadharNo,
                                request.StudentDetails.PEN,
                                request.StudentDetails.StudentTypeID,
                                request.StudentDetails.StudentHouseID,
                                StudentID = request.StudentID
                            },
                            transaction
                        );

                        // Update tbl_StudentOtherInfo using OtherInformation
                        if (!DateTime.TryParseExact(request.OtherInformation.RegistrationDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime registrationDate))
                        {
                            return new ServiceResponse<string>(false, "Invalid RegistrationDate format", "Failure", 400);
                        }
                        if (!DateTime.TryParseExact(request.OtherInformation.AdmissionDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime admissionDate))
                        {
                            return new ServiceResponse<string>(false, "Invalid AdmissionDate format", "Failure", 400);
                        }

                        string updateOtherInfoQuery = @"
                    UPDATE tbl_StudentOtherInfo SET
                        Register_Date = @Register_Date,
                        Register_Number = @Register_Number,
                        Admission_Date = @Admission_Date,
                        samagra_ID = @samagra_ID,
                        Place_of_Birth = @Place_of_Birth,
                        email_id = @email_id,
                        language_known = @language_known,
                        comments = @comments,
                        Identification_Mark_1 = @Identification_Mark_1,
                        Identification_Mark_2 = @Identification_Mark_2
                    WHERE student_id = @StudentID";

                        await _dbConnection.ExecuteAsync(
                            updateOtherInfoQuery,
                            new
                            {
                                Register_Date = registrationDate,
                                Register_Number = request.OtherInformation.RegistrationNo,
                                Admission_Date = admissionDate,
                                samagra_ID = request.OtherInformation.SamagraID,
                                Place_of_Birth = request.OtherInformation.PlaceofBirth,
                                email_id = request.OtherInformation.EmailID,
                                language_known = request.OtherInformation.LanguageKnown,
                                comments = request.OtherInformation.Comments,
                                Identification_Mark_1 = request.OtherInformation.IdentificationMark1,
                                Identification_Mark_2 = request.OtherInformation.IdentificationMark2,
                                StudentID = request.StudentID
                            },
                            transaction
                        );

                        // Update tbl_StudentParentsInfo using ParentsInfo
                        string updateParentsInfoQuery = @"
                    UPDATE tbl_StudentParentsInfo SET
                        Parent_Type_id = @Parent_Type_id,
                        First_Name = @First_Name,
                        Middle_Name = @Middle_Name,
                        Last_Name = @Last_Name,
                        Mobile_Number = @Mobile_Number,
                        Bank_Account_no = @Bank_Account_no,
                        Bank_IFSC_Code = @Bank_IFSC_Code,
                        Family_Ration_Card_Type = @Family_Ration_Card_Type,
                        Family_Ration_Card_no = @Family_Ration_Card_no,
                        Date_of_Birth = @Date_of_Birth,
                        Aadhar_no = @Aadhar_no,
                        PAN_card_no = @PAN_card_no,
                        Residential_Address = @Residential_Address,
                        Designation = @Designation,
                        Name_of_the_Employer = @Name_of_the_Employer,
                        Office_no = @Office_no,
                        Email_id = @Email_id,
                        Annual_Income = @Annual_Income,
                        File_Name = @File_Name,
                        Occupation = @Occupation
                    WHERE student_id = @StudentID";

                        await _dbConnection.ExecuteAsync(
                            updateParentsInfoQuery,
                            new
                            {
                                Parent_Type_id = 1, // Default value (adjust as needed)
                                First_Name = request.ParentsInfo.FirstName,
                                Middle_Name = request.ParentsInfo.MiddleName,
                                Last_Name = request.ParentsInfo.LastName,
                                Mobile_Number = request.ParentsInfo.PrimaryContactNo,
                                Bank_Account_no = request.ParentsInfo.BankAccountNo,
                                Bank_IFSC_Code = request.ParentsInfo.BankIFSCCode,
                                Family_Ration_Card_Type = request.ParentsInfo.FamilyRationCardType,
                                Family_Ration_Card_no = request.ParentsInfo.FamilyRationCardNo,
                                Date_of_Birth = parentDateOfBirth,
                                Aadhar_no = request.ParentsInfo.AadharNo,
                                PAN_card_no = request.ParentsInfo.PANCardNo,
                                Residential_Address = request.ParentsInfo.ResidentialAddress,
                                Designation = request.ParentsInfo.Designation,
                                Name_of_the_Employer = request.ParentsInfo.NameoftheEmployer,
                                Office_no = request.ParentsInfo.OfficeNo,
                                Email_id = request.ParentsInfo.EmailID,
                                Annual_Income = request.ParentsInfo.AnnualIncome,
                                File_Name = (string)null,
                                Occupation = request.ParentsInfo.Occupation,
                                StudentID = request.StudentID
                            },
                            transaction
                        );

                        // Update tbl_StudentSiblings using SiblingsDetails (if provided)
                        if (request.SiblingsDetails != null)
                        {
                            string updateSiblingsQuery = @"
                        UPDATE tbl_StudentSiblings SET
                            Name = @FirstName,
                            Middle_Name = @MiddleName,
                            Last_Name = @LastName,
                            Admission_Number = @AdmissionNo,
                            Date_of_Birth = @Date_of_Birth,
                            Institute_Name = @InstituteName,
                            Aadhar_no = @AadharNo,
                            Class = @Class,
                            section = @Section
                        WHERE student_id = @StudentID";

                            await _dbConnection.ExecuteAsync(
                                updateSiblingsQuery,
                                new
                                {
                                    StudentID = request.StudentID,
                                    FirstName = request.SiblingsDetails.FirstName,
                                    Middle_Name = request.SiblingsDetails.MiddleName,
                                    LastName = request.SiblingsDetails.LastName,
                                    AdmissionNo = request.SiblingsDetails.AdmissionNo,
                                    Date_of_Birth = siblingDOB, // Already converted above
                                    Institute_Name = request.SiblingsDetails.InstituteName,
                                    Aadhar_no = request.SiblingsDetails.AadharNo,
                                    Class = request.SiblingsDetails.Class,
                                    Section = request.SiblingsDetails.Section
                                },
                                transaction
                            );
                        }

                        // Update tbl_StudentPreviousSchool using PreviousSchoolDetails (if provided)
                        if (request.PreviousSchoolDetails != null)
                        {
                            // Ensure TCDate is provided and valid
                            if (!tcDateProvided)
                            {
                                return new ServiceResponse<string>(false, "TCDate is required in PreviousSchoolDetails", "Failure", 400);
                            }

                            string updatePreviousSchoolQuery = @"
                        UPDATE tbl_StudentPreviousSchool SET
                            Previous_School_Name = @Previous_School_Name,
                            Previous_Board = @Previous_Board,
                            Previous_Medium = @Previous_Medium,
                            Previous_School_Address = @Previous_School_Address,
                            previous_School_Course = @previous_School_Course,
                            Previous_Class = @Previous_Class,
                            TC_number = @TC_number,
                            TC_date = @TC_date,
                            isTC_Submitted = @isTC_Submitted
                        WHERE student_id = @StudentID";

                            await _dbConnection.ExecuteAsync(
                                updatePreviousSchoolQuery,
                                new
                                {
                                    Previous_School_Name = request.PreviousSchoolDetails.InstituteName,
                                    Previous_Board = request.PreviousSchoolDetails.Board,
                                    Previous_Medium = request.PreviousSchoolDetails.Medium,
                                    Previous_School_Address = request.PreviousSchoolDetails.InstituteAddress,
                                    previous_School_Course = request.PreviousSchoolDetails.Course,
                                    Previous_Class = request.PreviousSchoolDetails.Class,
                                    TC_number = request.PreviousSchoolDetails.TCNumber,
                                    TC_date = tcDate,
                                    isTC_Submitted = request.PreviousSchoolDetails.IsTCSubmitted,
                                    StudentID = request.StudentID
                                },
                                transaction
                            );
                        }

                        // Insert or update tbl_StudentHealthInfo using HealthInformation (if provided)
                        if (request.HealthInformation != null)
                        {
                            // In this example, we assume a new record is inserted if the student is new,
                            // or the existing record is updated for an existing student.
                            if (request.StudentID != 0 && /* you may want to check if a record exists */ false)
                            {
                                // This branch could be used if you check existence; otherwise, we assume update in the else block.
                            }

                            if (request.StudentID != 0 && /* record exists */ true) // Update branch
                            {
                                string updateHealthInfoQuery = @"
                            UPDATE tbl_StudentHealthInfo SET
                                Allergies = @Allergies,
                                Medications = @Medications,
                                Doctor_Name = @Doctor_Name,
                                Doctor_Phone_no = @Doctor_Phone_no,
                                height = @height,
                                weight = @weight,
                                Government_ID = @Government_ID,
                                Chest = @Chest,
                                Physical_Deformity = @Physical_Deformity,
                                History_Majorillness = @History_Majorillness,
                                History_Accident = @History_Accident,
                                Vision = @Vision,
                                Hearing = @Hearing,
                                Speech = @Speech,
                                Behavioral_Problem = @Behavioral_Problem,
                                Remarks_Weakness = @Remarks_Weakness
                            WHERE student_id = @StudentID";

                                await _dbConnection.ExecuteAsync(
                                    updateHealthInfoQuery,
                                    new
                                    {
                                        Allergies = request.HealthInformation.Allergies,
                                        Medications = request.HealthInformation.Medications,
                                        Doctor_Name = request.HealthInformation.ConsultingDoctorsName,
                                        Doctor_Phone_no = request.HealthInformation.ConsultingDoctorPhoneNumber,
                                        height = request.HealthInformation.Height,
                                        weight = request.HealthInformation.Weight,
                                        Government_ID = request.HealthInformation.GovermentHealthID,
                                        Chest = request.HealthInformation.Chest,
                                        Physical_Deformity = request.HealthInformation.AnyPhysicalDeformiity,
                                        History_Majorillness = request.HealthInformation.HistoryofMajorIllness,
                                        History_Accident = request.HealthInformation.HistoryofanyAccident,
                                        Vision = request.HealthInformation.Vision,
                                        Hearing = request.HealthInformation.Hearing,
                                        Speech = request.HealthInformation.Speech,
                                        Behavioral_Problem = request.HealthInformation.BehavioralProblems,
                                        Remarks_Weakness = request.HealthInformation.AnyOtherRemarksOrWeakness,
                                        StudentID = request.StudentID
                                    },
                                    transaction
                                );
                            }
                            else // For new student, insert new HealthInformation record.
                            {
                                string insertHealthInfoQuery = @"
                            INSERT INTO tbl_StudentHealthInfo
                                (student_id, Allergies, Medications, Doctor_Name, Doctor_Phone_no, height, weight, Government_ID, Chest, Physical_Deformity, History_Majorillness, History_Accident, Vision, Hearing, Speech, Behavioral_Problem, Remarks_Weakness)
                            VALUES
                                (@StudentID, @Allergies, @Medications, @Doctor_Name, @Doctor_Phone_no, @height, @weight, @Government_ID, @Chest, @Physical_Deformity, @History_Majorillness, @History_Accident, @Vision, @Hearing, @Speech, @Behavioral_Problem, @Remarks_Weakness);";

                                await _dbConnection.ExecuteAsync(
                                    insertHealthInfoQuery,
                                    new
                                    {
                                        StudentID = request.StudentID,
                                        Allergies = request.HealthInformation.Allergies,
                                        Medications = request.HealthInformation.Medications,
                                        Doctor_Name = request.HealthInformation.ConsultingDoctorsName,
                                        Doctor_Phone_no = request.HealthInformation.ConsultingDoctorPhoneNumber,
                                        height = request.HealthInformation.Height,
                                        weight = request.HealthInformation.Weight,
                                        Government_ID = request.HealthInformation.GovermentHealthID,
                                        Chest = request.HealthInformation.Chest,
                                        Physical_Deformity = request.HealthInformation.AnyPhysicalDeformiity,
                                        History_Majorillness = request.HealthInformation.HistoryofMajorIllness,
                                        History_Accident = request.HealthInformation.HistoryofanyAccident,
                                        Vision = request.HealthInformation.Vision,
                                        Hearing = request.HealthInformation.Hearing,
                                        Speech = request.HealthInformation.Speech,
                                        Behavioral_Problem = request.HealthInformation.BehavioralProblems,
                                        Remarks_Weakness = request.HealthInformation.AnyOtherRemarksOrWeakness
                                    },
                                    transaction
                                );
                            }
                        }
                    }

                    transaction.Commit();
                    return new ServiceResponse<string>(true, "Student information saved successfully", "Success", 200);
                }
            }
            catch (Exception ex)
            {
                // Consider logging the error details in a real-world scenario.
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
            finally
            {
                // Ensure the connection is closed
                if (_dbConnection.State == ConnectionState.Open)
                    _dbConnection.Close();
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetStudentInformationResponse>>> GetStudentInformation(GetStudentInformationRequest request)
        {
            try
            {
                // Build base query
                string query = @"
                SELECT sm.student_id AS StudentID,
                sm.Admission_Number AS AdmissionNumber,
                (sm.First_Name + ' ' + ISNULL(sm.Middle_Name,'') + ' ' + sm.Last_Name) AS StudentName,
                c.class_name AS [Class],
                s.section_name AS Section,
                sm.Roll_Number AS RollNumber,
                g.Gender_Type AS Gender,
                CONVERT(VARCHAR(10), sm.Date_of_Birth, 105) AS DateOfBirth, -- dd-MM-yyyy
                r.Religion_Type AS Religion,
                CONVERT(VARCHAR(10), sm.Date_of_Joining, 105) AS DateOfJoining, -- dd-MM-yyyy
                -- father name from ParentsInfo table with Parent_Type_id = 1
                (SELECT TOP 1 (p.First_Name + ' ' + ISNULL(p.Middle_Name,'') + ' ' + p.Last_Name)
                 FROM tbl_StudentParentsInfo p
                 WHERE p.student_id = sm.student_id 
                   AND p.Parent_Type_id = 1) AS FatherName,

                -- sibling details (assuming single sibling for example)
                sib.Name AS SiblingFirstName,
                sib.Last_Name AS SiblingLastName,
                sib.Admission_Number AS SiblingAdmissionNumber,
                sib.Class AS SiblingClass,
                sib.section AS SiblingSection,
                sib.Institute_Name AS SiblingInstituteName,
                sib.Aadhar_no AS SiblingAadharNumber

            FROM tbl_StudentMaster sm
                LEFT JOIN tbl_Class c ON sm.class_id = c.class_id
                LEFT JOIN tbl_Section s ON sm.section_id = s.section_id
                LEFT JOIN tbl_Gender g ON sm.gender_id = g.Gender_id
                LEFT JOIN tbl_Religion r ON sm.Religion_id = r.Religion_id
                LEFT JOIN tbl_StudentSiblings sib ON sm.student_id = sib.Student_id
            WHERE sm.Institute_id = @InstituteID
              AND sm.AcademicYearCode = @AcademicYearCode
              AND sm.class_id = @ClassID
              AND sm.section_id = @SectionID
              AND sm.StudentType_id = @StudentTypeID
        ";

                // For isActive filter (Status)
                // 1 => isActive=1
                // 2 => isActive=0
                // 3 => isActive in (1,0)
                // We'll append the condition to the WHERE clause
                if (request.Status == 1)
                {
                    query += " AND sm.isActive = 1 ";
                }
                else if (request.Status == 2)
                {
                    query += " AND sm.isActive = 0 ";
                }
                else if (request.Status == 3)
                {
                    // isActive in (1,0) => no additional filter needed, or you can omit it entirely
                    // but if there's a third possible value (like 2?), handle accordingly
                }

                // Prepare parameters
                var parameters = new DynamicParameters();
                parameters.Add("@InstituteID", request.InstituteID);
                parameters.Add("@AcademicYearCode", request.AcademicYearCode);
                parameters.Add("@ClassID", request.ClassID);
                parameters.Add("@SectionID", request.SectionID);
                parameters.Add("@StudentTypeID", request.StudentTypeID);

                var result = await _dbConnection.QueryAsync<GetStudentInformationResponse>(query, parameters);

                // Return data
                return new ServiceResponse<IEnumerable<GetStudentInformationResponse>>(
                    true,
                    "Student Information Retrieved Successfully",
                    result,
                    200,
                    result.Count()
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetStudentInformationResponse>>(
                    false,
                    ex.Message,
                    null,
                    500
                );
            }
        }

        public async Task<ServiceResponse<string>> SetStudentStatusActivity(SetStudentStatusActivityRequest request)
        {
            try
            {
                if (!DateTime.TryParseExact(request.ActivityDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime activityDate))
                {
                    return new ServiceResponse<string>(false, "Invalid ActivityDate format. Expected format: DD-MM-YYYY", "Failure", 400);
                }

                string query = @"
                INSERT INTO tblStudentACActivity
                    (StudentID, ActivityStatusID, ActivityDate, UserID, Reason, InstituteID)
                VALUES
                    (@StudentID, @ActivityStatusID, @ActivityDate, @UserID, @InactiveReason, @InstituteID)";

                int rowsAffected = await _dbConnection.ExecuteAsync(query, new
                {
                    request.StudentID,
                    request.ActivityStatusID,
                    ActivityDate = activityDate,
                    request.UserID,
                    request.InactiveReason,
                    request.InstituteID
                });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<string>(true, "Student Activity Status Updated Successfully", "Success", 200);
                }

                return new ServiceResponse<string>(false, "Failed to Update Student Activity Status", "Failure", 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetStudentStatusActivityResponse>>> GetStudentStatusActivity(GetStudentStatusActivityRequest request)
        {
            try
            {
                string query = @"
        SELECT 
            s.ActivityStatus AS Status,
            FORMAT(sa.ActivityDate, 'dd-MM-yyyy') AS Date, -- Format Date Here
            e.First_Name + ' ' + e.Middle_Name + ' ' + e.Last_Name AS UserName,
            sa.Reason 
        FROM tblStudentACActivity sa
        INNER JOIN tblActivityStatus s ON sa.ActivityStatusID = s.ActivityStatusID
        LEFT JOIN tbl_EmployeeProfileMaster e ON sa.UserID = e.Employee_id
        WHERE sa.StudentID = @StudentID AND sa.InstituteID = @InstituteID
        ORDER BY sa.ActivityDate DESC";

                var studentStatusActivities = await _dbConnection.QueryAsync<GetStudentStatusActivityResponse>(query, new
                {
                    request.StudentID,
                    request.InstituteID
                });

                return new ServiceResponse<IEnumerable<GetStudentStatusActivityResponse>>(true, "Student Activity Status Retrieved Successfully", studentStatusActivities, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<GetStudentStatusActivityResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<DownloadStudentImportTemplateResponse> GetMasterTablesData(int instituteID)
        {
            var response = new DownloadStudentImportTemplateResponse();

            response.Classes = (await _dbConnection.QueryAsync<ClassResponse>("SELECT class_id AS Id, class_name AS Name FROM tbl_Class WHERE institute_id = @InstituteID AND IsDeleted = 0", new { InstituteID = instituteID })).ToList();
            response.Sections = (await _dbConnection.QueryAsync<SectionResponse>("SELECT section_id AS Id, section_name AS Name, class_id FROM tbl_Section WHERE IsDeleted = 0")).ToList();
            response.Genders = (await _dbConnection.QueryAsync<GenericResponse>("SELECT Gender_id AS Id, Gender_Type AS Name FROM tbl_Gender")).ToList();
            response.Religions = (await _dbConnection.QueryAsync<GenericResponse>("SELECT Religion_id AS Id, Religion_Type AS Name FROM tbl_Religion")).ToList();
            response.Nationalities = (await _dbConnection.QueryAsync<GenericResponse>("SELECT Nationality_id AS Id, Nationality_Type AS Name FROM tbl_Nationality")).ToList();
            response.MotherTongues = (await _dbConnection.QueryAsync<GenericResponse>("SELECT Mother_Tongue_id AS Id, Mother_Tongue_Name AS Name FROM tbl_MotherTongue")).ToList();
            response.BloodGroups = (await _dbConnection.QueryAsync<GenericResponse>("SELECT Blood_Group_id AS Id, Blood_Group_Type AS Name FROM tbl_BloodGroup")).ToList();
            response.Castes = (await _dbConnection.QueryAsync<GenericResponse>("SELECT caste_id AS Id, caste_type AS Name FROM tbl_CasteMaster")).ToList();
            response.InstituteHouses = (await _dbConnection.QueryAsync<GenericResponse>("SELECT Institute_house_id AS Id, HouseName AS Name FROM tbl_InstituteHouse WHERE Institute_id = @InstituteID AND IsDeleted = 0", new { InstituteID = instituteID })).ToList();
            response.StudentTypes = (await _dbConnection.QueryAsync<GenericResponse>("SELECT Student_Type_id AS Id, Student_Type_Name AS Name FROM tbl_StudentType")).ToList();
            response.ParentTypes = (await _dbConnection.QueryAsync<GenericResponse>("SELECT parent_type_id AS Id, parent_type AS Name FROM tbl_ParentType")).ToList();

            return response;
        }

        public async Task<List<SectionJoinedResponse>> GetSectionsWithClassNames(int instituteID)
        {
            const string sql = @"
            SELECT 
                s.section_id     AS SectionID,
                c.class_name     AS ClassName,
                s.section_name   AS SectionName
            FROM tbl_Section s
            JOIN tbl_Class c ON s.class_id = c.class_id
            WHERE s.IsDeleted = 0
              AND c.IsDeleted = 0
              AND c.institute_id = @InstituteID
            ORDER BY c.class_id, s.section_id;
    ";

            var result = await _dbConnection.QueryAsync<SectionJoinedResponse>(
                sql, new { InstituteID = instituteID }
            );

            return result.ToList();
        }

        public async Task<ServiceResponse<string>> InsertStudents(int instituteID, List<StudentInformationImportRequest> students)
        {

            // Ensure the connection is open before beginning the transaction.
            if (_dbConnection.State == System.Data.ConnectionState.Closed)
            {
                _dbConnection.Open();
            }

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    foreach (var student in students)
                    {
                        //string insertStudentQuery = @"
                        //INSERT INTO tbl_StudentMaster 
                        //(First_Name, Middle_Name, Last_Name, gender_id, class_id, section_id, Admission_Number, Roll_Number, Date_of_Joining, AcademicYearCode, 
                        // Nationality_id, Religion_id, Date_of_Birth, Mother_Tongue_id, Caste_id, Blood_Group_id, Aadhar_Number, PEN, StudentType_id, Institute_house_id) 
                        //VALUES 
                        //(@FirstName, @MiddleName, @LastName, @GenderID, @ClassID, @SectionID, @AdmissionNumber, @RollNumber, @DateOfJoining, @AcademicYear, 
                        // @NationalityID, @ReligionID, @DateOfBirth, @MotherTongueID, @CasteID, @BloodGroupID, @AadharNo, @PEN, @StudentTypeID, @StudentHouseID)";


                        string insertStudentQuery = @"
                        INSERT INTO tbl_StudentMaster 
                        (First_Name, Middle_Name, Last_Name, gender_id, class_id, section_id, Admission_Number, Roll_Number, Institute_id) 
                        VALUES 
                        (@FirstName, @MiddleName, @LastName, @GenderID, @ClassID, @SectionID, @AdmissionNumber, @RollNumber, @InstituteID)";

                        await _dbConnection.ExecuteAsync(insertStudentQuery, new
                        {
                            student.StudentDetails.FirstName,
                            student.StudentDetails.MiddleName,
                            student.StudentDetails.LastName,
                            student.StudentDetails.GenderID,
                            student.StudentDetails.ClassID,
                            student.StudentDetails.SectionID,
                            student.StudentDetails.AdmissionNumber,
                            student.StudentDetails.RollNumber,
                            //student.StudentDetails.DateOfJoining,
                            //student.StudentDetails.AcademicYear,
                            //student.StudentDetails.NationalityID,
                            //student.StudentDetails.ReligionID,
                            //student.StudentDetails.DateOfBirth,
                            //student.StudentDetails.MotherTongueID,
                            //student.StudentDetails.CasteID,
                            //student.StudentDetails.BloodGroupID,
                            //student.StudentDetails.AadharNo,
                            //student.StudentDetails.PEN,
                            //student.StudentDetails.StudentTypeID,
                            //student.StudentDetails.StudentHouseID
                            InstituteID = instituteID
                        }, transaction);
                    }

                    transaction.Commit();
                    return new ServiceResponse<string>(true, "Students imported successfully", null, 200);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ServiceResponse<string>(false, "Error inserting students: " + ex.Message, null, 500);
                }
            }
        }
    }
}
