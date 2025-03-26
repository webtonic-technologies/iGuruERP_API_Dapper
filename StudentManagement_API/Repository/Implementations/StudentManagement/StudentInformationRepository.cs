using System;
using System.Data;
using System.Data.Common;
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
                            (First_Name, Middle_Name, Last_Name, gender_id, class_id, section_id, Admission_Number, Roll_Number, Date_of_Joining, AcademicYearCode, Nationality_id, Religion_id, Date_of_Birth, Mother_Tongue_id, Caste_id, Blood_Group_id, Aadhar_Number, PEN, StudentType_id, Institute_id, Institute_house_id)
                        VALUES
                            (@FirstName, @MiddleName, @LastName, @GenderID, @ClassID, @SectionID, @AdmissionNumber, @RollNumber, @DateOfJoining, @AcademicYear, @NationalityID, @ReligionID, @Date_of_Birth, @MotherTongueID, @CasteID, @BloodGroupID, @AadharNo, @PEN, @StudentTypeID, @InstituteID, @StudentHouseID);
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
                                InstituteID = request.InstituteID,  // <-- New parameter 
                                request.StudentDetails.StudentHouseID
                            },
                            transaction
                        );
                        request.StudentID = newStudentId;


                        //---------------- Add Information in tblStudentStandards

                         request.StudentID = newStudentId;

                         string insertStudentStandardsQuery = @"
                            INSERT INTO tblStudentStandards
                                (StudentID, ClassID, SectionID, AcademicYearCode, PromotionDate, InstituteID)
                            VALUES
                                (@StudentID, @ClassID, @SectionID, @AcademicYearCode, @PromotionDate, @InstituteID);
                        ";

                        await _dbConnection.ExecuteAsync(
                            insertStudentStandardsQuery,
                            new
                            {
                                StudentID = request.StudentID,
                                ClassID = request.StudentDetails.ClassID,
                                SectionID = request.StudentDetails.SectionID,
                                AcademicYearCode = request.StudentDetails.AcademicYear,
                                PromotionDate = (DateTime?)null, // Set PromotionDate here if you have a value; otherwise, null
                                InstituteID = request.InstituteID
                            },
                            transaction
                        );

                        //---------------- Add Information in tblStudentStandards



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
                        Institute_id = @InstituteID, 
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
                                DateOfBirth = dateOfBirth,  // Changed here to match the query (@DateOfBirth)
                                request.StudentDetails.MotherTongueID,
                                request.StudentDetails.CasteID,
                                request.StudentDetails.BloodGroupID,
                                request.StudentDetails.AadharNo,
                                request.StudentDetails.PEN,
                                request.StudentDetails.StudentTypeID,
                                InstituteID = request.InstituteID,  // <-- New parameter 
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
                                    MiddleName = request.SiblingsDetails.MiddleName,
                                    LastName = request.SiblingsDetails.LastName,
                                    AdmissionNo = request.SiblingsDetails.AdmissionNo,
                                    Date_of_Birth = siblingDOB, // Already converted above
                                    InstituteName = request.SiblingsDetails.InstituteName,
                                    AadharNo = request.SiblingsDetails.AadharNo,
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
                // 1. Count total records matching the filter criteria
                string countSql = @"
                SELECT COUNT(*) 
                FROM tbl_StudentMaster 
                WHERE Institute_id = @InstituteID 
                  AND AcademicYearCode = @AcademicYearCode 
                  AND class_id = @ClassID 
                  AND section_id = @SectionID 
                  AND StudentType_id = @StudentTypeID";

                int totalCount = await _dbConnection.ExecuteScalarAsync<int>(countSql, new
                {
                    request.InstituteID,
                    request.AcademicYearCode,
                    request.ClassID,
                    request.SectionID,
                    request.StudentTypeID
                });

                // 2. Build the dynamic column list from tblStudentColumnSetting.
                string columnListSql = @"
                SELECT STRING_AGG(SCS.DatabaseFieldName, ', ')
                FROM tblStudentColumnSetting SCS
                INNER JOIN tblStudentSettingMapping SSM 
                    ON SCS.StudentColumnID = SSM.StudentColumnID 
                    AND SSM.InstituteID = @InstituteID
                WHERE SCS.IsActive = 1";
                string columnList = await _dbConnection.ExecuteScalarAsync<string>(columnListSql, new { request.InstituteID });

                // 3. Build the dynamic WHERE clause starting with the required filters.
                    string whereClause = @"
                WHERE sm.Institute_id = @InstituteID
                  AND sm.AcademicYearCode = @AcademicYearCode
                  AND sm.class_id = @ClassID
                  AND sm.section_id = @SectionID
                  AND sm.StudentType_id = @StudentTypeID";

                // 4. Add the search condition if the Search parameter is provided.
                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    whereClause += @"
                AND (
                    (sm.First_Name + ' ' + sm.Middle_Name + ' ' + sm.Last_Name) LIKE '%' + @Search + '%' OR 
                    sm.Admission_Number LIKE '%' + @Search + '%' OR 
                    sm.Roll_Number LIKE '%' + @Search + '%' OR 
                    spi_f.Mobile_Number LIKE '%' + @Search + '%'
                )";
                }

                // 5. Build the inner query ensuring every column in the dynamic list is present.
                string innerQuery = $@"
                SELECT
                    -- Student Master
                    sm.student_id AS StudentID,
                    sm.First_Name AS FirstName,
                    sm.Middle_Name AS MiddleName,
                    sm.Last_Name AS LastName,
                    sm.gender_id AS Gender,
                    sm.class_id AS Class,
                    sm.section_id AS Section,
                    sm.Admission_Number AS AdmissionNo,
                    sm.Roll_Number AS RollNumber,
                    sm.Date_of_Joining AS DateOfJoining,
                    sm.Nationality_id AS Nationality,
                    sm.Religion_id AS Religion,
                    sm.Date_of_Birth AS DateOfBirth,
                    sm.Mother_Tongue_id AS MotherTongue,
                    sm.Caste_id AS Caste,
                    sm.Blood_Group_id AS BloodGroup,
                    sm.Aadhar_Number AS AadharNo,
                    sm.PEN AS PEN,
                    sm.QR_code AS QRCode,
                    sm.IsPhysicallyChallenged AS PhysicallyChallenged,
                    sm.IsSports AS Sports,
                    sm.IsAided AS Aided,
                    sm.IsNCC AS NCC,
                    sm.IsNSS AS NSS,
                    sm.IsScout AS Scout,
                    sm.File_Name AS FileName,
                    sm.isActive AS IsActive,
                    sm.Institute_id AS InstituteID,
                    sm.Institute_house_id AS InstituteHouseID,
                    sm.StudentType_id AS StudentType,
                    sm.AcademicYearCode AS AcademicYearCode,
    
                    -- Student Other Info
                    soi.Student_Other_Info_id AS StudentOtherInfoID,
                    soi.email_id AS EmailID,
                    soi.Hall_Ticket_Number AS HallTicketNumber,
                    soi.Identification_Mark_1 AS IdentificationMark1,
                    soi.Identification_Mark_2 AS IdentificationMark2,
                    soi.Admission_Date AS AdmissionDate,
                    soi.Register_Date AS RegisterDate,
                    soi.Register_Number AS RegisterNumber,
                    soi.samagra_ID AS SamagraID,
                    soi.Place_of_Birth AS PlaceOfBirth,
                    soi.comments AS Comments,
                    soi.language_known AS LanguageKnown,
                    soi.Mobile_Number AS MobileNumber,
    
                    -- Father Info (Parent_Type_id = 1)
                    spi_f.First_Name AS FatherFirstName,
                    spi_f.Middle_Name AS FatherMiddleName,
                    spi_f.Last_Name AS FatherLastName,
                    spi_f.Mobile_Number AS FatherMobileNumber,
                    spi_f.Bank_Account_no AS FatherBankAccountNo,
                    spi_f.Bank_IFSC_Code AS FatherBankIFSCCode,
                    spi_f.Family_Ration_Card_Type AS FatherRationCardType,
                    spi_f.Family_Ration_Card_no AS FatherRationCardNo,
                    spi_f.Date_of_Birth AS FatherDateOfBirth,
                    spi_f.Aadhar_no AS FatherAadharNo,
                    spi_f.PAN_card_no AS FatherPANCardNo,
                    spi_f.Residential_Address AS FatherResidentialAddress,
                    spi_f.Designation AS FatherDesignation,
                    spi_f.Name_of_the_Employer AS FatherEmployerName,
                    spi_f.Office_no AS FatherOfficeNo,
                    spi_f.Email_id AS FatherEmailID,
                    spi_f.Annual_Income AS FatherAnnualIncome,
                    spi_f.Occupation AS FatherOccupation,
    
                    -- Additional columns from other joins as needed...
    
                    -- Sibling Info (example)
                    ss.Student_Siblings_id AS StudentSiblingsID,
                    ss.Student_id AS SiblingStudentID,
                    ss.Name AS SiblingName,
                    ss.Last_Name AS SiblingLastName,
                    ss.Admission_Number AS SiblingAdmissionNo,
                    ss.Date_of_Birth AS SiblingDateOfBirth,
                    ss.Institute_Name AS SiblingInstituteName,
                    ss.Aadhar_no AS SiblingAadharNo,
                    ss.Class AS SiblingClass,
                    ss.section AS SiblingSection,
                    ss.Middle_Name AS SiblingMiddleName
                FROM tbl_StudentMaster sm
                LEFT JOIN tbl_StudentOtherInfo soi ON sm.student_id = soi.student_id
                LEFT JOIN tbl_StudentParentsInfo spi_f ON sm.student_id = spi_f.Student_id AND spi_f.Parent_Type_id = 1
                LEFT JOIN tbl_StudentParentsInfo spi_m ON sm.student_id = spi_m.Student_id AND spi_m.Parent_Type_id = 2
                LEFT JOIN tbl_StudentParentsInfo spi_g ON sm.student_id = spi_g.Student_id AND spi_g.Parent_Type_id = 3
                LEFT JOIN tbl_StudentSiblings ss ON sm.student_id = ss.Student_id
                LEFT JOIN tbl_StudentDocuments sd ON sm.student_id = sd.Student_id
                LEFT JOIN tbl_StudentPreviousSchool sps ON sm.student_id = sps.student_id
                LEFT JOIN tbl_StudentParentsOfficeInfo spoi ON sm.student_id = spoi.Student_id
                LEFT JOIN tbl_StudentHealthInfo sh ON sm.student_id = sh.Student_id
                {whereClause}";

                // 6. Build the outer query that uses the dynamic column list.
                string sql = $@"
                SELECT {columnList}
                FROM (
                    {innerQuery}
                ) AS db";

                // 7. Prepare parameters for the query.
                var parameters = new DynamicParameters();
                parameters.Add("@InstituteID", request.InstituteID);
                parameters.Add("@AcademicYearCode", request.AcademicYearCode);
                parameters.Add("@ClassID", request.ClassID);
                parameters.Add("@SectionID", request.SectionID);
                parameters.Add("@StudentTypeID", request.StudentTypeID);
                parameters.Add("@Search", request.Search);

                // 8. Execute the query and return the result.
                var result = await _dbConnection.QueryAsync<GetStudentInformationResponse>(sql, parameters);

                return new ServiceResponse<IEnumerable<GetStudentInformationResponse>>(
                    true,
                    "Student Information Retrieved Successfully",
                    result,
                    200,
                    totalCount
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

        
        public async Task<ServiceResponse<string>> InsertStudents(int instituteID, string AcademicYearCode, string IPAddress, int UserID, List<StudentInformationImportRequest> students)
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
                        // 1. Insert basic student details into tbl_StudentMaster and get the new StudentID.
                        string insertMasterQuery = @"
                        INSERT INTO tbl_StudentMaster 
                        (First_Name, Middle_Name, Last_Name, gender_id, class_id, section_id, Admission_Number, Roll_Number, Date_of_Joining,
                         Nationality_id, Religion_id, Date_of_Birth, Mother_Tongue_id, Caste_id, Blood_Group_id, StudentType_id, 
                         Institute_house_id, Aadhar_Number, PEN, Institute_id)
                        VALUES 
                        (@FirstName, @MiddleName, @LastName, @GenderID, @ClassID, @SectionID, @AdmissionNumber, @RollNumber, @DateOfJoining,
                         @NationalityID, @ReligionID, @DateOfBirth, @MotherTongueID, @CasteID, @BloodGroupID, @StudentTypeID, 
                         @StudentHouseID, @AadharNo, @PEN, @InstituteID);
                        SELECT CAST(SCOPE_IDENTITY() AS int);";


                        string dojString = student.StudentDetails.DateOfJoining; // e.g. "15-06-2023"
                        if (!DateTime.TryParseExact(dojString, "dd-MM-yyyy",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfJoining))
                        {
                            throw new Exception($"Invalid date format for Date of Joining: {dojString}");
                        }
                        //DateOfJoining = dateOfJoining.ToString("dd-MM-yyyy");


                        string dobString = student.StudentDetails.DateOfBirth; // e.g. "15-05-2010"
                        if (!DateTime.TryParseExact(dobString, "dd-MM-yyyy",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateOfBirth))
                        {
                            throw new Exception($"Invalid date format for Date of Birth: {dobString}");
                        }
                        //DateOfBirth = dateOfBirth.ToString("dd-MM-yyyy");





                        int studentId = await _dbConnection.QuerySingleAsync<int>(insertMasterQuery, new
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
                            dateOfJoining,
                            student.StudentDetails.NationalityID,
                            student.StudentDetails.ReligionID,
                            //student.StudentDetails.DateOfBirth,  
                            dateOfBirth,
                            student.StudentDetails.MotherTongueID,
                            student.StudentDetails.CasteID,
                            student.StudentDetails.BloodGroupID,
                            student.StudentDetails.StudentTypeID,
                            student.StudentDetails.StudentHouseID,
                            student.StudentDetails.AadharNo,
                            student.StudentDetails.PEN,
                            InstituteID = instituteID
                        }, transaction);




                        //---------------- Add Information in tblStudentStandards

 
                        string insertStudentStandardsQuery = @"
                            INSERT INTO tblStudentStandards
                                (StudentID, ClassID, SectionID, AcademicYearCode, PromotionDate, InstituteID)
                            VALUES
                                (@StudentID, @ClassID, @SectionID, @AcademicYearCode, @PromotionDate, @InstituteID);
                        ";

                        await _dbConnection.ExecuteAsync(
                            insertStudentStandardsQuery,
                            new
                            {
                                StudentID = studentId,
                                ClassID = student.StudentDetails.ClassID,
                                SectionID = student.StudentDetails.SectionID,
                                AcademicYearCode = student.StudentDetails.AcademicYear,
                                PromotionDate = (DateTime?)null, // Set PromotionDate here if you have a value; otherwise, null
                                InstituteID = instituteID
                            },
                            transaction
                        );

                        //---------------- Add Information in tblStudentStandards


                        // 2. Insert "other information" into tbl_StudentOtherInfo.
                        string insertOtherInfoQuery = @"
                        INSERT INTO tbl_StudentOtherInfo
                        (student_id, Register_Date, Register_Number, Admission_Date, samagra_ID, Place_of_Birth, email_id, language_known, Identification_Mark_1, Identification_Mark_2)
                        VALUES
                        (@StudentID, @RegistrationDate, @RegistrationNo, @AdmissionDate, @SamagraID, @PlaceOfBirth, @EmailID, @LanguageKnown, @IdentificationMark1, @IdentificationMark2)";



                        string regDateString = student.OtherInformation.RegistrationDate; // e.g. "15-05-2023"
                        if (!DateTime.TryParseExact(regDateString, "dd-MM-yyyy",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime registrationDate))
                        {
                            throw new Exception($"Invalid date format for Registration Date: {regDateString}");
                        }

                        string admissionDateString = student.OtherInformation.AdmissionDate; // e.g. "15-05-2023"
                        if (!DateTime.TryParseExact(admissionDateString, "dd-MM-yyyy",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime admissionDate))
                        {
                            throw new Exception($"Invalid date format for Admission Date: {admissionDateString}");
                        }


                        await _dbConnection.ExecuteAsync(insertOtherInfoQuery, new
                        {
                            StudentID = studentId,
                            RegistrationDate = registrationDate,
                            //RegistrationDate = student.OtherInformation.RegistrationDate, // maps to Register_Date
                            RegistrationNo = student.OtherInformation.RegistrationNo,         // maps to Register_Number
                            //AdmissionDate = student.OtherInformation.AdmissionDate,
                            AdmissionDate = admissionDate,
                            SamagraID = student.OtherInformation.SamagraID,
                            PlaceOfBirth = student.OtherInformation.PlaceofBirth,
                            EmailID = student.OtherInformation.EmailID,
                            LanguageKnown = student.OtherInformation.LanguageKnown,
                            IdentificationMark1 = student.OtherInformation.IdentificationMark1,
                            IdentificationMark2 = student.OtherInformation.IdentificationMark2
                        }, transaction) ;

                        // 3. Insert parent's details into tbl_StudentParentsInfo.
                        string insertParentQuery = @"
                    INSERT INTO tbl_StudentParentsInfo
                    (Student_id, Parent_Type_id, First_Name, Middle_Name, Last_Name, Mobile_Number, Bank_Account_no, 
                     Bank_IFSC_Code, Family_Ration_Card_Type, Family_Ration_Card_no, Date_of_Birth, Aadhar_no, PAN_card_no, 
                     Residential_Address, Designation, Name_of_the_Employer, Office_no, Email_id, Annual_Income, Occupation)
                    VALUES
                    (@Student_id, @Parent_Type_id, @First_Name, @Middle_Name, @Last_Name, @Mobile_Number, @Bank_Account_no, 
                     @Bank_IFSC_Code, @Family_Ration_Card_Type, @Family_Ration_Card_no, @Date_of_Birth, @Aadhar_no, @PAN_card_no, 
                     @Residential_Address, @Designation, @Name_of_the_Employer, @Office_no, @Email_id, @Annual_Income, @Occupation)";


                        string fatherDobString = student.ParentsInfo.FatherDateOfBirth; // e.g. "15-05-1975"
                        if (!DateTime.TryParseExact(fatherDobString, "dd-MM-yyyy",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fatherDOB))
                        {
                            throw new Exception($"Invalid date format for Father Date of Birth: {fatherDobString}");
                        } 


                        // Insert Father Info (Parent_Type_id = 1)
                        await _dbConnection.ExecuteAsync(insertParentQuery, new
                        {
                            Student_id = studentId,
                            Parent_Type_id = 1,
                            First_Name = student.ParentsInfo.FatherFirstName,
                            Middle_Name = student.ParentsInfo.FatherMiddleName,
                            Last_Name = student.ParentsInfo.FatherLastName,
                            Mobile_Number = student.ParentsInfo.FatherPrimaryContactNo,
                            Bank_Account_no = student.ParentsInfo.FatherBankAccountNo,
                            Bank_IFSC_Code = student.ParentsInfo.FatherBankIFSCCode,
                            Family_Ration_Card_Type = student.ParentsInfo.FatherFamilyRationCardType,
                            Family_Ration_Card_no = student.ParentsInfo.FatherFamilyRationCardNo,
                            //Date_of_Birth = student.ParentsInfo.FatherDateOfBirth,
                            Date_of_Birth = fatherDOB,
                            Aadhar_no = student.ParentsInfo.FatherAadharNo,
                            PAN_card_no = student.ParentsInfo.FatherPANCardNo,
                            Residential_Address = student.ParentsInfo.FatherResidentialAddress,
                            Designation = student.ParentsInfo.FatherDesignation,
                            Name_of_the_Employer = student.ParentsInfo.FatherNameoftheEmployer,
                            Office_no = student.ParentsInfo.FatherOfficeNo,
                            Email_id = student.ParentsInfo.FatherEmailID,
                            Annual_Income = student.ParentsInfo.FatherAnnualIncome,
                            Occupation = student.ParentsInfo.FatherOccupation
                        }, transaction);


                        string motherDobString = student.ParentsInfo.MotherDateOfBirth; // e.g. "15-05-1978"
                        if (!DateTime.TryParseExact(motherDobString, "dd-MM-yyyy",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime motherDOB))
                        {
                            throw new Exception($"Invalid date format for Mother Date of Birth: {motherDobString}");
                        }
                        


                        // Insert Mother Info (Parent_Type_id = 2)
                        await _dbConnection.ExecuteAsync(insertParentQuery, new
                        {
                            Student_id = studentId,
                            Parent_Type_id = 2,
                            First_Name = student.ParentsInfo.MotherFirstName,
                            Middle_Name = student.ParentsInfo.MotherMiddleName,
                            Last_Name = student.ParentsInfo.MotherLastName,
                            Mobile_Number = student.ParentsInfo.MotherPrimaryContactNo,
                            Bank_Account_no = student.ParentsInfo.MotherBankAccountNo,
                            Bank_IFSC_Code = student.ParentsInfo.MotherBankIFSCCode,
                            Family_Ration_Card_Type = student.ParentsInfo.MotherFamilyRationCardType,
                            Family_Ration_Card_no = student.ParentsInfo.MotherFamilyRationCardNo,
                            //Date_of_Birth = student.ParentsInfo.MotherDateOfBirth,
                            Date_of_Birth = motherDOB,
                            Aadhar_no = student.ParentsInfo.MotherAadharNo,
                            PAN_card_no = student.ParentsInfo.MotherPANCardNo,
                            Residential_Address = student.ParentsInfo.MotherResidentialAddress,
                            Designation = student.ParentsInfo.MotherDesignation,
                            Name_of_the_Employer = student.ParentsInfo.MotherNameoftheEmployer,
                            Office_no = student.ParentsInfo.MotherOfficeNo,
                            Email_id = student.ParentsInfo.MotherEmailID,
                            Annual_Income = student.ParentsInfo.MotherAnnualIncome,
                            Occupation = student.ParentsInfo.MotherOccupation
                        }, transaction);

                        //string dobString = student.ParentsInfo.GuardianDateOfBirth; // e.g. "20/01/1980"
                        //if (!DateTime.TryParseExact(dobString, "dd/MM/yyyy",
                        //    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime guardianDOB))
                        //{
                        //    // Handle invalid date format, e.g., add an error message or set a default
                        //    throw new Exception($"Invalid date format for Guardian Date of Birth: {dobString}");
                        //}

                        string guardianDateString = student.ParentsInfo.GuardianDateOfBirth; // e.g. "15-05-2023"
                        if (!DateTime.TryParseExact(guardianDateString, "dd-MM-yyyy",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime guardianDOB))
                        {
                            throw new Exception($"Invalid date format for Guardian Date of Birth: {guardianDateString}");
                        }



                        // Insert Guardian Info (Parent_Type_id = 3)
                        await _dbConnection.ExecuteAsync(insertParentQuery, new
                        {
                            Student_id = studentId,
                            Parent_Type_id = 3,
                            First_Name = student.ParentsInfo.GuardianFirstName,
                            Middle_Name = student.ParentsInfo.GuardianMiddleName,
                            Last_Name = student.ParentsInfo.GuardianLastName,
                            Mobile_Number = student.ParentsInfo.GuardianPrimaryContactNo,
                            Bank_Account_no = student.ParentsInfo.GuardianBankAccountNo,
                            Bank_IFSC_Code = student.ParentsInfo.GuardianBankIFSCCode,
                            Family_Ration_Card_Type = student.ParentsInfo.GuardianFamilyRationCardType,
                            Family_Ration_Card_no = student.ParentsInfo.GuardianFamilyRationCardNo,
                            Date_of_Birth = guardianDOB,  // Now a valid DateTime
                            Aadhar_no = student.ParentsInfo.GuardianAadharNo,
                            PAN_card_no = student.ParentsInfo.GuardianPANCardNo,
                            Residential_Address = student.ParentsInfo.GuardianResidentialAddress,
                            Designation = student.ParentsInfo.GuardianDesignation,
                            Name_of_the_Employer = student.ParentsInfo.GuardianNameoftheEmployer,
                            Office_no = student.ParentsInfo.GuardianOfficeNo,
                            Email_id = student.ParentsInfo.GuardianEmailID,
                            Annual_Income = student.ParentsInfo.GuardianAnnualIncome,
                            Occupation = student.ParentsInfo.GuardianOccupation
                        }, transaction);
                         

                        // 4. Insert Health & Other Details into tbl_StudentHealthInfo
                        string insertHealthInfoQuery = @"
                        INSERT INTO tbl_StudentHealthInfo
                        (student_id, Allergies, Medications, Doctor_Name, Doctor_Phone_no, height, weight, Government_ID, Vision, Hearing, Speech, Behavioral_Problem, 
                         Chest, History_Accident, Physical_Deformity, History_Majorillness, Remarks_Weakness)
                        VALUES
                        (@Student_id, @Allergies, @Medications, @Doctor_Name, @Doctor_Phone_no, @height, @weight, @Government_ID, @Vision, @Hearing, @Speech, @Behavioral_Problem, 
                         @Chest, @History_Accident, @Physical_Deformity, @History_Majorillness, @Remarks_Weakness)";

                        await _dbConnection.ExecuteAsync(insertHealthInfoQuery, new
                        {
                            Student_id = studentId,
                            Allergies = student.HealthInformation.Allergies,
                            Medications = student.HealthInformation.Medications,
                            Doctor_Name = student.HealthInformation.ConsultingDoctorsName,
                            Doctor_Phone_no = student.HealthInformation.ConsultingDoctorPhoneNumber,
                            height = student.HealthInformation.Height,
                            weight = student.HealthInformation.Weight,
                            Government_ID = student.HealthInformation.GovermentHealthID,
                            Vision = student.HealthInformation.Vision,
                            Hearing = student.HealthInformation.Hearing,
                            Speech = student.HealthInformation.Speech,
                            Behavioral_Problem = student.HealthInformation.BehavioralProblems,
                            Chest = student.HealthInformation.Chest,
                            History_Accident = student.HealthInformation.HistoryofanyAccident,
                            Physical_Deformity = student.HealthInformation.AnyPhysicalDeformiity,
                            History_Majorillness = student.HealthInformation.HistoryofMajorIllness,
                            Remarks_Weakness = student.HealthInformation.AnyOtherRemarksOrWeakness
                        }, transaction);
                    }


                    transaction.Commit();




                    // Log history: count of students inserted, etc.
                    var historyResponse = await InsertStudentDataHistory(
                        instituteID,
                        students.Count,
                        AcademicYearCode,
                        IPAddress,
                        UserID,
                        1
                    );

                    if (!historyResponse.Success)
                    {
                        // Handle history insert error: log the error.
                        // Replace with your logging mechanism (e.g., _logger.LogError).
                        Console.WriteLine($"Error inserting student data history: {historyResponse.Message}");

                        return new ServiceResponse<string>(false, "Error inserting students", null, 500);

                    }

                   



                    return new ServiceResponse<string>(true, "Students imported successfully", null, 200);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ServiceResponse<string>(false, "Error inserting students: " + ex.Message, null, 500);
                }
            }
        }


        public async Task<ServiceResponse<string>> InsertStudentDataHistory(
        int instituteID,
        int studentCount,
        string academicYearCode,
        string ipAddress,
        int userID,
        int exportHistoryTypeID)
        {
            // Ensure the connection is open.
            if (_dbConnection.State != System.Data.ConnectionState.Open)
            {
                if (_dbConnection is System.Data.Common.DbConnection dbConn)
                {
                    await dbConn.OpenAsync();
                }
                else
                {
                    _dbConnection.Open();
                }
            }

            try
            {
                string sql = @"
                INSERT INTO tblStudentDataHistory 
                (StudentCount, AcademicYearCode, DateTime, IPAddress, InstituteID, UserID, ExportHistoryTypeID)
                VALUES 
                (@StudentCount, @AcademicYearCode, GETDATE(), @IPAddress, @InstituteID, @UserID, @ExportHistoryTypeID)";

                await _dbConnection.ExecuteAsync(sql, new
                {
                    StudentCount = studentCount,
                    AcademicYearCode = academicYearCode,
                    IPAddress = ipAddress,
                    InstituteID = instituteID,
                    UserID = userID,
                    ExportHistoryTypeID = exportHistoryTypeID
                });

                return new ServiceResponse<string>(true, "Student data history inserted successfully", "Success", 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }



        public async Task<ServiceResponse<IEnumerable<GetStudentSettingResponse>>> GetStudentSetting(GetStudentSettingRequest request)
        {
            string sqlQuery = @"
                SELECT SCS.StudentColumnID, 
                       SCS.ScreenFieldName, 
                       CASE WHEN SSM.StudentColumnID IS NOT NULL THEN '1' ELSE '0' END AS Status
                FROM tblStudentColumnSetting SCS
                LEFT JOIN tblStudentSettingMapping SSM 
                  ON SSM.StudentColumnID = SCS.StudentColumnID
                  AND SSM.InstituteID = @InstituteID";

            var result = await _dbConnection.QueryAsync<GetStudentSettingResponse>(sqlQuery, new { request.InstituteID });

            return new ServiceResponse<IEnumerable<GetStudentSettingResponse>>(
                true, "Student settings fetched successfully", result, 200);
        }

        //public async Task<ServiceResponse<string>> AddRemoveStudentSetting(AddRemoveStudentSettingRequest request)
        //{
        //    // Check if the mapping already exists
        //    string checkSql = @"
        //        SELECT COUNT(*) 
        //        FROM tblStudentSettingMapping 
        //        WHERE InstituteID = @InstituteID 
        //          AND StudentColumnID = @StudentColumnID";
        //    int count = await _dbConnection.ExecuteScalarAsync<int>(checkSql, new { request.InstituteID, request.StudentColumnID });

        //    if (count > 0)
        //    {
        //        // Mapping exists, so remove it
        //        string deleteSql = @"
        //            DELETE FROM tblStudentSettingMapping 
        //            WHERE InstituteID = @InstituteID 
        //              AND StudentColumnID = @StudentColumnID";
        //        await _dbConnection.ExecuteAsync(deleteSql, new { request.InstituteID, request.StudentColumnID });
        //        return new ServiceResponse<string>(true, "Student setting removed successfully", "Success", 200);
        //    }
        //    else
        //    {
        //        // Mapping does not exist, so add it
        //        string insertSql = @"
        //            INSERT INTO tblStudentSettingMapping (InstituteID, StudentColumnID)
        //            VALUES (@InstituteID, @StudentColumnID)";
        //        await _dbConnection.ExecuteAsync(insertSql, new { request.InstituteID, request.StudentColumnID });
        //        return new ServiceResponse<string>(true, "Student setting added successfully", "Success", 200);
        //    }
        //}

        public async Task<ServiceResponse<string>> AddRemoveStudentSetting(List<AddRemoveStudentSettingRequest> requests)
        {
            // If there are no records, nothing to do.
            if (requests == null || !requests.Any())
            {
                return new ServiceResponse<string>(true, "No settings to process", "Success", 200);
            }

            // Ensure the connection is open.
            if (_dbConnection.State != System.Data.ConnectionState.Open)
            {
                if (_dbConnection is System.Data.Common.DbConnection dbConn)
                {
                    await dbConn.OpenAsync();
                }
                else
                {
                    _dbConnection.Open();
                }
            }

            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                // Get distinct InstituteIDs from the request.
                var instituteIds = requests.Select(r => r.InstituteID).Distinct();

                // For each institute, remove all existing records.
                foreach (var instituteID in instituteIds)
                {
                    string deleteSql = @"
                DELETE FROM tblStudentSettingMapping 
                WHERE InstituteID = @InstituteID";
                    await _dbConnection.ExecuteAsync(
                        deleteSql,
                        new { InstituteID = instituteID },
                        transaction: transaction
                    );
                }

                // Insert all new records.
                string insertSql = @"
            INSERT INTO tblStudentSettingMapping (InstituteID, StudentColumnID)
            VALUES (@InstituteID, @StudentColumnID)";
                foreach (var request in requests)
                {
                    await _dbConnection.ExecuteAsync(
                        insertSql,
                        new { request.InstituteID, request.StudentColumnID },
                        transaction: transaction
                    );
                }

                transaction.Commit();
                return new ServiceResponse<string>(true, "Student settings processed successfully", "Success", 200);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }



        
        //public async Task<List<GetStudentInformationResponse>> GetStudentInformationExport( GetStudentInformationExportRequest request)
        public async Task<List<GetStudentInformationResponse>> GetStudentInformationExport(int instituteID, string AcademicYearCode, string IPAddress, int UserID, GetStudentInformationExportRequest request)
        {
            // Build the dynamic WHERE clause
            string whereClause = @"
                WHERE sm.Institute_id = @InstituteID
                  AND sm.AcademicYearCode = @AcademicYearCode
                  AND sm.class_id = @ClassID
                  AND sm.section_id = @SectionID
                  AND sm.StudentType_id = @StudentTypeID";

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                whereClause += @"
                  AND (
                      (sm.First_Name + ' ' + sm.Middle_Name + ' ' + sm.Last_Name) LIKE '%' + @Search + '%' OR 
                      sm.Admission_Number LIKE '%' + @Search + '%' OR 
                      sm.Roll_Number LIKE '%' + @Search + '%' OR 
                      spi_f.Mobile_Number LIKE '%' + @Search + '%'
                  )";
            }

            // Get the dynamic column list
            string columnListSql = @"
                SELECT STRING_AGG(SCS.DatabaseFieldName, ', ')
                FROM tblStudentColumnSetting SCS
                INNER JOIN tblStudentSettingMapping SSM 
                    ON SCS.StudentColumnID = SSM.StudentColumnID 
                    AND SSM.InstituteID = @InstituteID
                WHERE SCS.IsActive = 1";
            string columnList = await _dbConnection.ExecuteScalarAsync<string>(columnListSql, new { request.InstituteID });

            // Build the inner query (include required joins and select columns)
            string innerQuery = $@"
                SELECT
                    -- Student Master
                    sm.student_id AS StudentID,
                    sm.First_Name AS FirstName,
                    sm.Middle_Name AS MiddleName,
                    sm.Last_Name AS LastName,
                    sm.gender_id AS Gender,
                    sm.class_id AS Class,
                    sm.section_id AS Section,
                    sm.Admission_Number AS AdmissionNo,
                    sm.Roll_Number AS RollNumber,
                    sm.Date_of_Joining AS DateOfJoining,
                    sm.Nationality_id AS Nationality,
                    sm.Religion_id AS Religion,
                    sm.Date_of_Birth AS DateOfBirth,
                    sm.Mother_Tongue_id AS MotherTongue,
                    sm.Caste_id AS Caste,
                    sm.Blood_Group_id AS BloodGroup,
                    sm.Aadhar_Number AS AadharNo,
                    sm.PEN AS PEN,
                    sm.QR_code AS QRCode,
                    sm.IsPhysicallyChallenged AS PhysicallyChallenged,
                    sm.IsSports AS Sports,
                    sm.IsAided AS Aided,
                    sm.IsNCC AS NCC,
                    sm.IsNSS AS NSS,
                    sm.IsScout AS Scout,
                    sm.File_Name AS FileName,
                    sm.isActive AS IsActive,
                    sm.Institute_id AS InstituteID,
                    sm.Institute_house_id AS InstituteHouseID,
                    sm.StudentType_id AS StudentType,
                    sm.AcademicYearCode AS AcademicYearCode,
    
                    -- Student Other Info
                    soi.Student_Other_Info_id AS StudentOtherInfoID,
                    soi.email_id AS EmailID,
                    soi.Hall_Ticket_Number AS HallTicketNumber,
                    soi.Identification_Mark_1 AS IdentificationMark1,
                    soi.Identification_Mark_2 AS IdentificationMark2,
                    soi.Admission_Date AS AdmissionDate,
                    soi.Register_Date AS RegisterDate,
                    soi.Register_Number AS RegisterNumber,
                    soi.samagra_ID AS SamagraID,
                    soi.Place_of_Birth AS PlaceOfBirth,
                    soi.comments AS Comments,
                    soi.language_known AS LanguageKnown,
                    soi.Mobile_Number AS MobileNumber,
    
                    -- Father Info (Parent_Type_id = 1)
                    spi_f.First_Name AS FatherFirstName,
                    spi_f.Middle_Name AS FatherMiddleName,
                    spi_f.Last_Name AS FatherLastName,
                    spi_f.Mobile_Number AS FatherMobileNumber,
                    spi_f.Bank_Account_no AS FatherBankAccountNo,
                    spi_f.Bank_IFSC_Code AS FatherBankIFSCCode,
                    spi_f.Family_Ration_Card_Type AS FatherRationCardType,
                    spi_f.Family_Ration_Card_no AS FatherRationCardNo,
                    spi_f.Date_of_Birth AS FatherDateOfBirth,
                    spi_f.Aadhar_no AS FatherAadharNo,
                    spi_f.PAN_card_no AS FatherPANCardNo,
                    spi_f.Residential_Address AS FatherResidentialAddress,
                    spi_f.Designation AS FatherDesignation,
                    spi_f.Name_of_the_Employer AS FatherEmployerName,
                    spi_f.Office_no AS FatherOfficeNo,
                    spi_f.Email_id AS FatherEmailID,
                    spi_f.Annual_Income AS FatherAnnualIncome,
                    spi_f.Occupation AS FatherOccupation,
    
                    -- Additional columns from other joins as needed...
    
                    -- Sibling Info (example)
                    ss.Student_Siblings_id AS StudentSiblingsID,
                    ss.Student_id AS SiblingStudentID,
                    ss.Name AS SiblingName,
                    ss.Last_Name AS SiblingLastName,
                    ss.Admission_Number AS SiblingAdmissionNo,
                    ss.Date_of_Birth AS SiblingDateOfBirth,
                    ss.Institute_Name AS SiblingInstituteName,
                    ss.Aadhar_no AS SiblingAadharNo,
                    ss.Class AS SiblingClass,
                    ss.section AS SiblingSection,
                    ss.Middle_Name AS SiblingMiddleName
                FROM tbl_StudentMaster sm
                LEFT JOIN tbl_StudentOtherInfo soi ON sm.student_id = soi.student_id
                LEFT JOIN tbl_StudentParentsInfo spi_f ON sm.student_id = spi_f.Student_id AND spi_f.Parent_Type_id = 1
                LEFT JOIN tbl_StudentParentsInfo spi_m ON sm.student_id = spi_m.Student_id AND spi_m.Parent_Type_id = 2
                LEFT JOIN tbl_StudentParentsInfo spi_g ON sm.student_id = spi_g.Student_id AND spi_g.Parent_Type_id = 3
                LEFT JOIN tbl_StudentSiblings ss ON sm.student_id = ss.Student_id
                LEFT JOIN tbl_StudentDocuments sd ON sm.student_id = sd.Student_id
                LEFT JOIN tbl_StudentPreviousSchool sps ON sm.student_id = sps.student_id
                LEFT JOIN tbl_StudentParentsOfficeInfo spoi ON sm.student_id = spoi.Student_id
                LEFT JOIN tbl_StudentHealthInfo sh ON sm.student_id = sh.Student_id
                {whereClause}";

            // Build the final query using the dynamic column list
            string sql = $@"
                SELECT {columnList}
                FROM (
                    {innerQuery}
                ) AS db";

            var parameters = new DynamicParameters();
            parameters.Add("@InstituteID", request.InstituteID);
            parameters.Add("@AcademicYearCode", request.AcademicYearCode);
            parameters.Add("@ClassID", request.ClassID);
            parameters.Add("@SectionID", request.SectionID);
            parameters.Add("@StudentTypeID", request.StudentTypeID);
            parameters.Add("@Search", request.Search);

            var result = await _dbConnection.QueryAsync<GetStudentInformationResponse>(sql, parameters);
           


            var historyResponse = await InsertStudentDataHistory(
                instituteID,
                result.Count(),
                AcademicYearCode,
                IPAddress,
                UserID,
                2   
            );

            if (!historyResponse.Success)
            {
                // Handle history insert error: log the error.
                // Replace with your logging mechanism (e.g., _logger.LogError).
                Console.WriteLine($"Error inserting student data history: {historyResponse.Message}");

            }

            return result.AsList();
        }


        public async Task<IEnumerable<GetStudentImportHistoryResponse>> GetStudentImportHistoryAsync(GetStudentImportHistoryRequest request)
        {
            string sql = @"
            SELECT 
                sdh.StudentCount,
                sdh.AcademicYearCode,
                FORMAT(sdh.DateTime, 'dd-MM-yyyy ''at'' hh:mm tt') AS DateTime,
                sdh.IPAddress,
                CONCAT(ep.First_Name, ' ', ISNULL(ep.Last_Name, '')) AS UserName
            FROM tblStudentDataHistory sdh
            LEFT JOIN tbl_EmployeeProfileMaster ep ON sdh.UserID = ep.Employee_id
            WHERE sdh.InstituteID = @InstituteID
              AND sdh.ExportHistoryTypeID = 1
            ORDER BY sdh.DateTime DESC";

            // Use the existing _dbConnection instead of creating a new connection using _connectionString.
            if (_dbConnection.State == ConnectionState.Closed)
            {
                _dbConnection.Open();
            }

            var result = await _dbConnection.QueryAsync<GetStudentImportHistoryResponse>(
                sql,
                new { InstituteID = request.InstituteID }
            );
            return result;
        }


        public async Task<IEnumerable<GetStudentExportHistoryResponse>> GetStudentExportHistoryAsync(GetStudentExportHistoryRequest request)
        {
            string sql = @"
                SELECT 
                    sdh.StudentCount,
                    sdh.AcademicYearCode,
                    FORMAT(sdh.DateTime, 'dd-MM-yyyy ''at'' hh:mm tt') AS DateTime,
                    sdh.IPAddress,
                    CONCAT(ep.First_Name, ' ', ISNULL(ep.Last_Name, '')) AS UserName
                FROM tblStudentDataHistory sdh
                LEFT JOIN tbl_EmployeeProfileMaster ep ON sdh.UserID = ep.Employee_id
                WHERE sdh.InstituteID = @InstituteID
                  AND sdh.ExportHistoryTypeID = 2
                ORDER BY sdh.DateTime DESC";

            if (_dbConnection.State == ConnectionState.Closed)
            {
                _dbConnection.Open();
            }

            var result = await _dbConnection.QueryAsync<GetStudentExportHistoryResponse>(
                sql,
                new { InstituteID = request.InstituteID }
            );

            return result;
        }
    }
}
