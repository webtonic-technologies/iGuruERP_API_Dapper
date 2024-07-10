using Student_API.Repository.Interfaces;
using System.Data;
using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Dapper;

using System.Collections.Generic;
using Student_API.DTOs.RequestDTO;
using Student_API.Models;

namespace Student_API.Repository.Implementations
{
    public class StudentInformationRepository : IStudentInformationRepository
    {
        private readonly IDbConnection _connection;

        public StudentInformationRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<string>> GetStudentInfoImageById(int studentId)
        {
            var stringQuery = "";
            try
            {

                stringQuery = @"SELECT File_Name FROM [dbo].[tbl_StudentMaster] WHERE student_id = @studentId;";

                var holiday = await _connection.QueryFirstOrDefaultAsync<string>(stringQuery, new { studentId = studentId });

                if (holiday == null)
                {
                    return new ServiceResponse<string>(false, "Student InfoImage not found", null, 404);
                }
                return new ServiceResponse<string>(true, "Operation successful", holiday, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<string>> GetStudentparentImageById(int Student_Parent_Info_id)
        {
            var stringQuery = "";
            try
            {

                stringQuery = @"SELECT File_Name FROM [dbo].[tbl_StudentParentsInfo] WHERE Student_Parent_Info_id = @Student_Parent_Info_id;";

                var holiday = await _connection.QueryFirstOrDefaultAsync<string>(stringQuery, new { Student_Parent_Info_id = Student_Parent_Info_id });

                if (holiday == null)
                {
                    return new ServiceResponse<string>(false, "Student parent InfoImage not found", null, 404);
                }
                return new ServiceResponse<string>(true, "Operation successful", holiday, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<StudentInformationDTO>> GetStudentDetailsById(int studentId)
        {
            try
            {
                string sql = @"
                    SELECT  tbl_StudentMaster.student_id, tbl_StudentMaster.First_Name, tbl_StudentMaster.Middle_Name, tbl_StudentMaster.Last_Name, tbl_StudentMaster.gender_id, Gender_Type,tbl_Class.[class_id], class_name AS class_course,tbl_section.[section_id], section_name AS Section,[Admission_Number], [Roll_Number],
                    [Date_of_Joining], Academic_year_id, tbl_AcademicYear.YearName,tbl_StudentMaster.Nationality_id, Nationality_Type,tbl_Religion.Religion_id,Religion_Type, tbl_StudentMaster.Date_of_Birth, tbl_StudentMaster.Mother_Tongue_id, Mother_Tongue_Name,tbl_StudentMaster.Caste_id,caste_type,
                    [Medium], tbl_StudentMaster.Blood_Group_id,Blood_Group_Type, [App_User_id], [Aadhar_Number], [NEP], [QR_code], [IsPhysicallyChallenged],
                    [IsSports], [IsAided], [IsNCC], [IsNSS], [IsScout], tbl_StudentMaster.File_Name, [isActive] 
                    ,tbl_Gender.Gender_Type,Religion_Type , Gender_Type,tbl_StudentMaster.Institute_id,Institute_name ,tbl_InstituteHouse.Institute_House_id AS Student_House_id,tbl_InstituteHouse.HouseName AS Student_House_Name
                    FROM tbl_StudentMaster 
                    LEFT JOIN tbl_Class ON tbl_StudentMaster.class_id = tbl_Class.class_id
                    LEFT JOIN tbl_Section on tbl_StudentMaster.section_id =  tbl_Section.section_id
                    LEFT JOIN tbl_Gender ON tbl_StudentMaster.gender_id = tbl_Gender.Gender_id
                    LEFT JOIN tbl_Religion ON tbl_StudentMaster.Religion_id = tbl_Religion.Religion_id
                    LEFT JOIN tbl_Nationality ON tbl_Nationality.Nationality_id = tbl_StudentMaster.Nationality_id 
                    LEFt JOIN tbl_MotherTongue ON tbl_StudentMaster.Mother_Tongue_id = tbl_MotherTongue.Mother_Tongue_id
                    LEFT JOIN tbl_BloodGroup ON tbl_BloodGroup.Blood_Group_id = tbl_StudentMaster.Blood_Group_id
                    LEFT JOIN tbl_CasteMaster ON tbl_CasteMaster.caste_id = tbl_StudentMaster.Caste_id
                    LEFT JOIN tbl_InstituteDetails ON tbl_InstituteDetails.Institute_id = tbl_StudentMaster.Institute_id
                    LEFT JOIN tbl_AcademicYear ON tbl_AcademicYear.Id = tbl_StudentMaster.Academic_year_id
                    LEFT JOIN tbl_InstituteHouse ON tbl_InstituteHouse.Institute_House_id = tbl_StudentMaster.Student_House_id
                    WHERE tbl_StudentMaster.student_id = @studentId;

                    SELECT [Student_Other_Info_id], [student_id], [StudentType_id], [email_id],  [Identification_Mark_1],
                    [Identification_Mark_2], [Admission_Date], tbl_StudentOtherInfo.Student_House_id, [Register_Date], [Register_Number], [samagra_ID], [Place_of_Birth], [comments], 
                    [language_known]  ,tbl_StudentOtherInfo.StudentType_id , Student_Type_Name
                    FROM [dbo].[tbl_StudentOtherInfo] 
                    INNER JOIN tbl_StudentType ON tbl_StudentType.Student_Type_id = tbl_StudentOtherInfo.StudentType_id
                    WHERE student_id = @studentId;

                    SELECT [Student_Parent_Info_id], [Student_id], tbl_StudentParentsInfo.Parent_Type_id, [First_Name], [Middle_Name], [Last_Name], [Contact_Number],
                    [Bank_Account_no], [Bank_IFSC_Code], [Family_Ration_Card_Type], [Family_Ration_Card_no], [Mobile_Number], [Date_of_Birth], [Aadhar_no], 
                    [PAN_card_no], [Residential_Address], tbl_StudentParentsInfo.Occupation_id, [Designation], [Name_of_the_Employer], [Office_no], [Email_id], [Annual_Income], 
                    [File_Name],tbl_Occupation.Occupation_Type,tbl_ParentType.parent_type
                    FROM [dbo].[tbl_StudentParentsInfo]
                    INNER JOIN tbl_Occupation ON tbl_Occupation.Occupation_id = tbl_StudentParentsInfo.Occupation_id
                    INNER JOIN tbl_ParentType ON tbl_ParentType.Parent_Type_id = tbl_StudentParentsInfo.Parent_Type_id
                    WHERE student_id = @studentId;

                   

                    SELECT ss.[Student_Siblings_id],ss.Name,ss.Last_Name,ss.[Student_id],ss.[Admission_Number],ss.[Date_of_Birth],ss.[Class_id],ss.[Selection_id],
                    ss.[Institute_Name],ss.[Aadhar_no],cc.class_Name AS [class_course],ccs.section_name AS [Section]
                    FROM [tbl_StudentSiblings] ss
                    INNER JOIN [tbl_Class] cc ON ss.[Class_id] = cc.class_id
                    INNER JOIN [tbl_Section] ccs ON ss.[Selection_id] = ccs.[section_id]
                    WHERE ss.[Student_id] = @studentId;


                    SELECT * FROM [dbo].[tbl_StudentPreviousSchool] WHERE student_id = @studentId;
                    SELECT * FROM [dbo].[tbl_StudentHealthInfo] WHERE student_id = @studentId;
                    select Student_Parent_Office_Info_id , Student_id , Parents_Type_id  , Office_Building_no , Street , Area , Pincode,tbl_StudentParentsOfficeInfo.City_id , city_name,tbl_StudentParentsOfficeInfo.State_id , state_name from tbl_StudentParentsOfficeInfo
INNER JOIN tbl_State ON  tbl_State.state_id = tbl_StudentParentsOfficeInfo.state_id
INNER JOIN tbl_City ON tbl_City.city_id = tbl_StudentParentsOfficeInfo.city_id WHERE tbl_StudentParentsOfficeInfo.student_id = @studentId;


                    SELECT * FROM [dbo].[tbl_StudentDocuments] WHERE student_id = @studentId AND isDelete = 0;";

                using (var result = await _connection.QueryMultipleAsync(sql, new { studentId }))
                {
                    var studentDetails = await result.ReadFirstOrDefaultAsync<StudentInformationDTO>();
                    var studentOtherInfo = await result.ReadFirstOrDefaultAsync<StudentOtherInfoDTO>();
                    var StudentParentInfo = await result.ReadAsync<StudentParentInfoDTO>();
                    var StudentSiblings = await result.ReadFirstOrDefaultAsync<StudentSiblings>();
                    var StudentPreviousSchool = await result.ReadFirstOrDefaultAsync<StudentPreviousSchool>();
                    var StudentHealthInfo = await result.ReadFirstOrDefaultAsync<StudentHealthInfo>();
                    var StudentParentOfficeInfo = await result.ReadAsync<StudentParentOfficeInfo>();
                    var StudentDocuments = await result.ReadAsync<StudentDocumentListDTO>();

                    if (studentDetails != null)
                    {
                        studentDetails.studentOtherInfoDTO = studentOtherInfo;
                        studentDetails.studentParentInfos = StudentParentInfo.ToList();
                        studentDetails.studentSiblings = StudentSiblings;
                        studentDetails.studentPreviousSchool = StudentPreviousSchool;
                        studentDetails.studentHealthInfo = StudentHealthInfo;
                        foreach (var parentInfo in StudentParentInfo)
                        {
                            parentInfo.studentParentOfficeInfo = StudentParentOfficeInfo
                                .Where(officeInfo => officeInfo.Parents_Type_id == parentInfo.Parent_Type_id && officeInfo.Student_id == studentId)
                                .FirstOrDefault();
                        }
                        studentDetails.studentDocumentListDTOs = StudentDocuments.ToList();
                        return new ServiceResponse<StudentInformationDTO>(true, "Operation successful", studentDetails, 200);
                    }
                    else
                    {
                        return new ServiceResponse<StudentInformationDTO>(false, "Student not found", null, 404);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<StudentInformationDTO>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<int>> AddUpdateStudent(StudentDTO request, List<StudentDocumentListDTO> studentDocuments)
        {
            try
            {
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        int studentId = request.student_id;
                        if (studentId == 0)
                        {
                            // Insert student data
                            string insertStudentSql = @"
                        INSERT INTO [dbo].[tbl_StudentMaster] (
                            First_Name, Middle_Name, Last_Name, gender_id, class_id,
                            section_id, Admission_Number, Roll_Number, Date_of_Joining,
                            Academic_year_id, Nationality_id, Religion_id, Date_of_Birth,
                            Mother_Tongue_id, Caste_id, Medium, Blood_Group_id, App_User_id, Aadhar_Number,
                            NEP, QR_code, IsPhysicallyChallenged, IsSports, IsAided,
                            IsNCC, IsNSS, IsScout, File_Name, Institute_id,Institute_house_id)
                        VALUES (
                            @First_Name, @Middle_Name, @Last_Name, @gender_id, @class_id,
                            @section_id, @Admission_Number, @Roll_Number, @Date_of_Joining,
                            @Academic_year_id, @Nationality_id, @Religion_id, @Date_of_Birth,
                            @Mother_Tongue_id, @Caste_id,  @Medium, @Blood_Group_id, @App_User_id, @Aadhar_Number,
                            @NEP, @QR_code, @IsPhysicallyChallenged, @IsSports, @IsAided,
                            @IsNCC, @IsNSS, @IsScout, @File_Name, @Institute_id,@Student_House_id);
                        SELECT CAST(SCOPE_IDENTITY() as int);";

                            studentId = await _connection.ExecuteScalarAsync<int>(insertStudentSql, request, transaction);

                            if (studentId <= 0)
                            {
                                transaction.Rollback();
                                return new ServiceResponse<int>(false, "Failed to insert student data.", 0, 500);
                            }
                        }
                        else
                        {
                            // Update student data
                            string updateStudentSql = @"
                        UPDATE [dbo].[tbl_StudentMaster]
                        SET 
                            First_Name = @First_Name,
                            Middle_Name = @Middle_Name,
                            Last_Name = @Last_Name,
                            gender_id = @gender_id,
                            class_id = @class_id,
                            section_id = @section_id,
                            Admission_Number = @Admission_Number,
                            Roll_Number = @Roll_Number,
                            Date_of_Joining = @Date_of_Joining,
                            Academic_year_id = @Academic_year_id,
                            Nationality_id = @Nationality_id,
                            Religion_id = @Religion_id,
                            Date_of_Birth = @Date_of_Birth,
                            Mother_Tongue_id = @Mother_Tongue_id,
                            Caste_id = @Caste_id,
                            Medium = @Medium,
                            Blood_Group_id = @Blood_Group_id,
                            App_User_id = @App_User_id,
                            Aadhar_Number = @Aadhar_Number,
                            NEP = @NEP,
                            QR_code = @QR_code,
                            IsPhysicallyChallenged = @IsPhysicallyChallenged,
                            IsSports = @IsSports,
                            IsAided = @IsAided,
                            IsNCC = @IsNCC,
                            IsNSS = @IsNSS,
                            IsScout = @IsScout,
                            File_Name = @File_Name,
                            Institute_id = @Institute_id,
                            Institute_house_id = @Student_House_id
                        WHERE student_id = @student_id";

                            int affectedRows = await _connection.ExecuteAsync(updateStudentSql, request, transaction);
                            if (affectedRows <= 0)
                            {
                                transaction.Rollback();
                                return new ServiceResponse<int>(false, "Failed to update student data.", 0, 500);
                            }
                        }

                        // Insert or Update StudentOtherInfos
                        if (request.StudentOtherInfos != null)
                        {
                            request.StudentOtherInfos.student_id = studentId;
                            if (request.StudentOtherInfos.Student_Other_Info_id == 0)
                            {
                                string insertOtherInfoSql = @"
                               INSERT INTO [dbo].[tbl_StudentOtherInfo] (student_id, StudentType_id, email_id,  Identification_Mark_1, Identification_Mark_2, Admission_Date,  Register_Date, Register_Number, samagra_ID, Place_of_Birth, comments, language_known)
                               VALUES (@student_id, @StudentType_id, @email_id, @Identification_Mark_1, @Identification_Mark_2, @Admission_Date,  @Register_Date, @Register_Number, @samagra_ID, @Place_of_Birth, @comments, @language_known);
                               SELECT CAST(SCOPE_IDENTITY() as int)";

                                int otherInfoId = await _connection.ExecuteScalarAsync<int>(insertOtherInfoSql, request.StudentOtherInfos, transaction);
                                if (otherInfoId <= 0)
                                {
                                    transaction.Rollback();
                                    return new ServiceResponse<int>(false, "Failed to insert student other info.", 0, 500);
                                }
                            }
                            else
                            {
                                string updateOtherInfoSql = @"
                           UPDATE [dbo].[tbl_StudentOtherInfo]
                            SET 
                                StudentType_id = @StudentType_id,
                                email_id = @email_id,
                                Identification_Mark_1 = @Identification_Mark_1,
                                Identification_Mark_2 = @Identification_Mark_2,
                                Admission_Date = @Admission_Date,
                                Student_Group_id = @Student_Group_id,
                                Register_Date = @Register_Date,
                                Register_Number = @Register_Number,
                                samagra_ID = @samagra_ID,
                                Place_of_Birth = @Place_of_Birth,
                                comments = @comments,
                                language_known = @language_known
                            WHERE Student_Other_Info_id = @Student_Other_Info_id;
                            SELECT @Student_Other_Info_id";

                                int affectedRows = await _connection.ExecuteAsync(updateOtherInfoSql, request.StudentOtherInfos, transaction);
                                if (affectedRows <= 0)
                                {
                                    transaction.Rollback();
                                    return new ServiceResponse<int>(false, "Failed to update student other info.", 0, 500);
                                }
                            }
                        }

                        // Insert or Update StudentParentInfos
                        foreach (var parentInfo in request.studentParentInfos)
                        {
                            parentInfo.Student_id = studentId;
                            parentInfo.studentParentOfficeInfo.Student_id = studentId;
                            parentInfo.studentParentOfficeInfo.Parents_Type_id = parentInfo.Parent_Type_id;

                            if (parentInfo.Student_Parent_Info_id == 0)
                            {
                                // Insert Student Parent Info
                                var addSql = @"
                    INSERT INTO [dbo].[tbl_StudentParentsInfo] ([Student_id],[Parent_Type_id],[First_Name],[Middle_Name],[Last_Name],[Contact_Number],[Bank_Account_no],[Bank_IFSC_Code],[Family_Ration_Card_Type],[Family_Ration_Card_no],[Date_of_Birth],[Aadhar_no],[PAN_card_no],[Residential_Address],[Occupation_id],[Designation],[Name_of_the_Employer],[Office_no],[Email_id],[Annual_Income],[File_Name])
                    VALUES (@Student_id,@Parent_Type_id,@First_Name,@Middle_Name,@Last_Name,@Contact_Number,@Bank_Account_no,@Bank_IFSC_Code,@Family_Ration_Card_Type,@Family_Ration_Card_no,@Date_of_Birth,@Aadhar_no,@PAN_card_no,@Residential_Address,@Occupation_id,@Designation,@Name_of_the_Employer,@Office_no,@Email_id,@Annual_Income,@File_Name); 
                    SELECT CAST(SCOPE_IDENTITY() as int);";
                                int insertedId = await _connection.ExecuteScalarAsync<int>(addSql, parentInfo, transaction);

                                // Insert Student Parent Office Info
                                var addOfficeSql = @"
                    INSERT INTO [dbo].[tbl_StudentParentsOfficeInfo] ([Student_id],[Parents_Type_id],[Office_Building_no],[Street],[Area],[City_id],[State_id],[Pincode])
                    VALUES (@Student_id,@Parents_Type_id,@Office_Building_no,@Street,@Area,@City_id,@State_id,@Pincode);";
                                await _connection.ExecuteAsync(addOfficeSql, parentInfo.studentParentOfficeInfo, transaction);

                                if (insertedId <= 0)
                                {
                                    transaction.Rollback();
                                    return new ServiceResponse<int>(false, "Failed to Add student parent info.", 0, 500);
                                }
                            }
                            else
                            {
                                // Update Student Parent Info
                                var updateSql = @"
                    UPDATE [dbo].[tbl_StudentParentsInfo] SET
                        [Student_id] = @Student_id,
                        [Parent_Type_id] = @Parent_Type_id,
                        [First_Name] = @First_Name,
                        [Middle_Name] = @Middle_Name,
                        [Last_Name] = @Last_Name,
                        [Contact_Number] = @Contact_Number,
                        [Bank_Account_no] = @Bank_Account_no,
                        [Bank_IFSC_Code] = @Bank_IFSC_Code,
                        [Family_Ration_Card_Type] = @Family_Ration_Card_Type,
                        [Family_Ration_Card_no] = @Family_Ration_Card_no,
                        [Date_of_Birth] = @Date_of_Birth,
                        [Aadhar_no] = @Aadhar_no,
                        [PAN_card_no] = @PAN_card_no,
                        [Residential_Address] = @Residential_Address,
                        [Occupation_id] = @Occupation_id,
                        [Designation] = @Designation,
                        [Name_of_the_Employer] = @Name_of_the_Employer,
                        [Office_no] = @Office_no,
                        [Email_id] = @Email_id,
                        [Annual_Income] = @Annual_Income,
                        [File_Name] = @File_Name
                    WHERE [Student_Parent_Info_id] = @Student_Parent_Info_id;";
                                int affectedRows = await _connection.ExecuteAsync(updateSql, parentInfo, transaction);

                                // Update Student Parent Office Info
                                var updateOfficeSql = @"
                    UPDATE [dbo].[tbl_StudentParentsOfficeInfo] SET
                        [Student_id] = @Student_id,
                        [Parents_Type_id] = @Parents_Type_id,
                        [Office_Building_no] = @Office_Building_no,
                        [Street] = @Street,
                        [Area] = @Area,
                        [City_id] = @City_id,
                        [State_id] = @State_id,
                        [Pincode] = @Pincode
                    WHERE [Student_Parent_Office_Info_id] = @Student_Parent_Office_Info_id;";
                                int affectedRowsOffice = await _connection.ExecuteAsync(updateOfficeSql, parentInfo.studentParentOfficeInfo, transaction);


                                if (affectedRows <= 0 || affectedRowsOffice <= 0)
                                {
                                    transaction.Rollback();
                                    return new ServiceResponse<int>(false, "Failed to update student parent info.", 0, 500);
                                }
                            }


                        }

                        // Insert or Update StudentSiblingDetails
                        foreach (var siblingInfo in request.studentSiblings)
                        {
                            siblingInfo.Student_id = studentId;
                            if (siblingInfo.Student_Siblings_id == 0)
                            {
                                string insertSiblingInfoSql = @"
                            INSERT INTO [dbo].[tbl_StudentSiblings] (
                                  [Student_id],
                                  [Name],
                                  [Last_Name],
                                  [Admission_Number],
                                  [Date_of_Birth],
                                  [Class_id],
                                  [Selection_id],
                                  [Institute_Name],
                                  [Aadhar_no]
                              ) VALUES (
                                  @Student_id,
                                  @Name,
                                  @Last_Name,
                                  @Admission_Number,
                                  @Date_of_Birth,
                                  @Class_id,
                                  @Selection_id,
                                  @Institute_Name,
                                  @Aadhar_no
                              );
                              SELECT CAST(SCOPE_IDENTITY() as int);";

                                int siblingId = await _connection.ExecuteScalarAsync<int>(insertSiblingInfoSql, siblingInfo, transaction);
                                if (siblingId <= 0)
                                {
                                    transaction.Rollback();
                                    return new ServiceResponse<int>(false, "Failed to insert student sibling info.", 0, 500);
                                }
                            }
                            else
                            {
                                string updateSiblingInfoSql = @"
                              UPDATE [dbo].[tbl_StudentSiblings] SET
                        [Student_id] = @Student_id,
                        [Name] = @Name,
                        [Last_Name] = @Last_Name,
                        [Admission_Number] = @Admission_Number,
                        [Date_of_Birth] = @Date_of_Birth,
                        [Class_id] = @Class_id,
                        [Selection_id] = @Selection_id,
                        [Institute_Name] = @Institute_Name,
                        [Aadhar_no] = @Aadhar_no
                    WHERE [Student_Siblings_id] = @Student_Siblings_id;";

                                int affectedRows = await _connection.ExecuteAsync(updateSiblingInfoSql, siblingInfo, transaction);
                                if (affectedRows <= 0)
                                {
                                    transaction.Rollback();
                                    return new ServiceResponse<int>(false, "Failed to update student sibling info.", 0, 500);
                                }
                            }
                        }

                        // Insert or Update PreviousSchoolDetails
                        if (request.studentPreviousSchools != null)
                        {
                            request.studentPreviousSchools.student_id = studentId;
                            if (request.studentPreviousSchools.Student_Prev_School_id == 0)
                            {
                                string insertPreviousSchoolSql = @"
                           INSERT INTO [dbo].[tbl_StudentPreviousSchool] (
                            [student_id],
                            [Previous_School_Name],
                            [Previous_Board],
                            [Previous_Medium],
                            [Previous_School_Address],
                            [Previous_School_Group],
                            [Previous_Class],
                            [TC_number],
                            [TC_date],
                            [isTC_Submitted]
                        ) VALUES (
                            @student_id,
                            @Previous_School_Name,
                            @Previous_Board,
                            @Previous_Medium,
                            @Previous_School_Address,
                            @Previous_School_Group,
                            @Previous_Class,
                            @TC_number,
                            @TC_date,
                            @isTC_Submitted
                        );
                        SELECT CAST(SCOPE_IDENTITY() as int);";

                                int previousSchoolId = await _connection.ExecuteScalarAsync<int>(insertPreviousSchoolSql, request.studentPreviousSchools, transaction);
                                if (previousSchoolId <= 0)
                                {
                                    return new ServiceResponse<int>(false, "Failed to insert previous school details.", 0, 500);
                                }
                            }
                            else
                            {
                                string updatePreviousSchoolSql = @"
                              UPDATE [dbo].[tbl_StudentPreviousSchool] SET
                            [student_id] = @student_id,
                            [Previous_School_Name] = @Previous_School_Name,
                            [Previous_Board] = @Previous_Board,
                            [Previous_Medium] = @Previous_Medium,
                            [Previous_School_Address] = @Previous_School_Address,
                            [Previous_School_Group] = @Previous_School_Group,
                            [Previous_Class] = @Previous_Class,
                            [TC_number] = @TC_number,
                            [TC_date] = @TC_date,
                            [isTC_Submitted] = @isTC_Submitted
                        WHERE [Student_Prev_School_id] = @Student_Prev_School_id;";

                                int affectedRows = await _connection.ExecuteAsync(updatePreviousSchoolSql, request.studentPreviousSchools, transaction);
                                if (affectedRows <= 0)
                                {
                                    transaction.Rollback();
                                    return new ServiceResponse<int>(false, "Failed to update previous school details.", 0, 500);
                                }
                            }
                        }

                        // Insert or Update StudentHealthInfos
                        if (request.studentHealthInfos != null)
                        {
                            request.studentHealthInfos.Student_id = studentId;
                            if (request.studentHealthInfos.Student_Health_Info_id == 0)
                            {
                                string insertHealthInfoSql = @"
                            INSERT INTO [dbo].[tbl_StudentHealthInfo] (
                                 [Student_id], [Allergies], [Medications], [Doctor_Name], [Doctor_Phone_no], 
                                 [height], [weight], [Government_ID], [BCG], [MMR_Measles], [Polio], 
                                 [Hepatitis], [Triple_Antigen], [Others], [General_Health], [Head_Eye_ENT], 
                                 [Chest], [CVS], [Abdomen], [Genitalia], [Congenital_Disease], [Physical_Deformity], 
                                 [History_Majorillness], [History_Accident], [Vision], [Hearing], [Speech], 
                                 [Behavioral_Problem], [Remarks_Weakness], [Student_Name], [Student_Age]
                             ) VALUES (
                                 @Student_id, @Allergies, @Medications, @Doctor_Name, @Doctor_Phone_no, 
                                 @height, @weight, @Government_ID, @BCG, @MMR_Measles, @Polio, 
                                 @Hepatitis, @Triple_Antigen, @Others, @General_Health, @Head_Eye_ENT, 
                                 @Chest, @CVS, @Abdomen, @Genitalia, @Congenital_Disease, @Physical_Deformity, 
                                 @History_Majorillness, @History_Accident, @Vision, @Hearing, @Speech, 
                                 @Behavioral_Problem, @Remarks_Weakness, @Student_Name, @Student_Age
                             );
                             SELECT CAST(SCOPE_IDENTITY() as int);";

                                int healthInfoId = await _connection.ExecuteScalarAsync<int>(insertHealthInfoSql, request.studentHealthInfos, transaction);
                                if (healthInfoId <= 0)
                                {
                                    transaction.Rollback();
                                    return new ServiceResponse<int>(false, "Failed to insert student health info.", 0, 500);
                                }
                            }
                            else
                            {
                                string updateHealthInfoSql = @"
                           UPDATE [dbo].[tbl_StudentHealthInfo] SET
                                [Student_id] = @Student_id,
                                [Allergies] = @Allergies,
                                [Medications] = @Medications,
                                [Doctor_Name] = @Doctor_Name,
                                [Doctor_Phone_no] = @Doctor_Phone_no,
                                [height] = @height,
                                [weight] = @weight,
                                [Government_ID] = @Government_ID,
                                [BCG] = @BCG,
                                [MMR_Measles] = @MMR_Measles,
                                [Polio] = @Polio,
                                [Hepatitis] = @Hepatitis,
                                [Triple_Antigen] = @Triple_Antigen,
                                [Others] = @Others,
                                [General_Health] = @General_Health,
                                [Head_Eye_ENT] = @Head_Eye_ENT,
                                [Chest] = @Chest,
                                [CVS] = @CVS,
                                [Abdomen] = @Abdomen,
                                [Genitalia] = @Genitalia,
                                [Congenital_Disease] = @Congenital_Disease,
                                [Physical_Deformity] = @Physical_Deformity,
                                [History_Majorillness] = @History_Majorillness,
                                [History_Accident] = @History_Accident,
                                [Vision] = @Vision,
                                [Hearing] = @Hearing,
                                [Speech] = @Speech,
                                [Behavioral_Problem] = @Behavioral_Problem,
                                [Remarks_Weakness] = @Remarks_Weakness,
                                [Student_Name] = @Student_Name,
                                [Student_Age] = @Student_Age
                            WHERE [Student_Health_Info_id] = @Student_Health_Info_id;";

                                int affectedRows = await _connection.ExecuteAsync(updateHealthInfoSql, request.studentHealthInfos, transaction);
                                if (affectedRows <= 0)
                                {
                                    return new ServiceResponse<int>(false, "Failed to update student health info.", 0, 500);
                                }
                            }
                        }

                        foreach (var item in studentDocuments)
                        {
                            if (item.Student_Documents_id == 0)
                            {

                                var newDocument = new StudentDocuments
                                {
                                    Student_id = studentId,
                                    Document_Name = item.Document_Name,
                                    File_Path = item.File_Path,
                                    File_Name = item.File_Name
                                };

                                string insertSql = @"
                INSERT INTO [dbo].[tbl_StudentDocuments] (student_id, Document_Name, File_Name , File_Path)
                VALUES (@Student_id, @Document_Name, @File_Name, @File_Path);
                SELECT CAST(SCOPE_IDENTITY() as int);";

                                int insertedId = await _connection.ExecuteScalarAsync<int>(insertSql, newDocument, transaction);
                                if (insertedId <= 0)
                                {
                                    transaction.Rollback();
                                    return new ServiceResponse<int>(false, "Failed to Add student Documents.", 0, 500);
                                }
                            }

                        }

                        transaction.Commit();
                        return new ServiceResponse<int>(true, "Student data saved successfully.", studentId, 200);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new ServiceResponse<int>(false, $"Error saving student data: {ex.Message}", 0, 500);
                    }
                    finally
                    {
                        _connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, $"Error opening connection: {ex.Message}", 0, 500);
            }
        }

        public async Task<ServiceResponse<int>> AddUpdateStudentInformation(StudentMasters request)
        {
            try
            {

                if (request.student_id == 0)
                {

                    string sql = @"
                    INSERT INTO [dbo].[tbl_StudentMaster] (
                        First_Name, Middle_Name, Last_Name, gender_id, class_id,
                        section_id, Admission_Number, Roll_Number, Date_of_Joining,
                        Academic_Year, Nationality_id, Religion_id, Date_of_Birth,
                        Mother_Tongue_id, Caste_id, First_Language, Second_Language,
                        Third_Language, Medium, Blood_Group_id, App_User_id, Aadhar_Number,
                        NEP, QR_code, IsPhysicallyChallenged, IsSports, IsAided,
                        IsNCC, IsNSS, IsScout, File_Name,Institute_id)
                    VALUES (
                        @First_Name, @Middle_Name, @Last_Name, @gender_id, @class_id,
                        @section_id, @Admission_Number, @Roll_Number, @Date_of_Joining,
                        @Academic_Year, @Nationality_id, @Religion_id, @Date_of_Birth,
                        @Mother_Tongue_id, @Caste_id, @First_Language, @Second_Language,
                        @Third_Language, @Medium, @Blood_Group_id, @App_User_id, @Aadhar_Number,
                        @NEP, @QR_code, @IsPhysicallyChallenged, @IsSports, @IsAided,
                        @IsNCC, @IsNSS, @IsScout, @File_Name,@Institute_id);
                    SELECT SCOPE_IDENTITY();";

                    int insertedId = await _connection.ExecuteScalarAsync<int>(sql, request);
                    if (insertedId > 0)
                    {
                        return new ServiceResponse<int>(true, "Operation successful", insertedId, 200);
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "Some error occured", 0, 500);
                    }
                }
                else
                {
                    string sql = @"
                    UPDATE [dbo].[tbl_StudentMaster]
                    SET 
                        First_Name = @First_Name,
                        Middle_Name = @Middle_Name,
                        Last_Name = @Last_Name,
                        gender_id = @gender_id,
                        class_id = @class_id,
                        section_id = @section_id,
                        Admission_Number = @Admission_Number,
                        Roll_Number = @Roll_Number,
                        Date_of_Joining = @Date_of_Joining,
                        Academic_Year = @Academic_Year,
                        Nationality_id = @Nationality_id,
                        Religion_id = @Religion_id,
                        Date_of_Birth = @Date_of_Birth,
                        Mother_Tongue_id = @Mother_Tongue_id,
                        Caste_id = @Caste_id,
                        First_Language = @First_Language,
                        Second_Language = @Second_Language,
                        Third_Language = @Third_Language,
                        Medium = @Medium,
                        Blood_Group_id = @Blood_Group_id,
                        App_User_id = @App_User_id,
                        Aadhar_Number = @Aadhar_Number,
                        NEP = @NEP,
                        IsPhysicallyChallenged = @IsPhysicallyChallenged,
                        IsSports = @IsSports,
                        IsAided = @IsAided,
                        IsNCC = @IsNCC,
                        IsNSS = @IsNSS,
                        IsScout = @IsScout,
                        File_Name = @File_Name,
                        Institute_id = @Institute_id
                        WHERE student_id = @student_id";

                    // Execute the query and retrieve the number of affected rows
                    int affectedRows = await _connection.ExecuteAsync(sql, request);
                    if (affectedRows > 0)
                    {
                        return new ServiceResponse<int>(true, "Operation successful", request.student_id, 200);
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "Some error occured", 0, 500);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<int>> AddUpdateStudentOtherInfo(StudentOtherInfos request)
        {
            try
            {
                if (request.Student_Other_Info_id == 0)
                {
                    string addSql = @"
                        INSERT INTO [dbo].[tbl_StudentOtherInfo] (student_id, StudentType_id, email_id, Hall_Ticket_Number, Identification_Mark_1, Identification_Mark_2, Admission_Date, Student_House_id, Register_Date, Register_Number, samagra_ID, Place_of_Birth, comments, language_known)
                        VALUES (@student_id, @StudentType_id, @email_id, @Hall_Ticket_Number, @Identification_Mark_1, @Identification_Mark_2, @Admission_Date, @Student_House_id, @Register_Date, @Register_Number, @samagra_ID, @Place_of_Birth, @comments, @language_known);
                        SELECT CAST(SCOPE_IDENTITY() as int)";
                    int insertedId = await _connection.ExecuteScalarAsync<int>(addSql, request);
                    if (insertedId > 0)
                    {
                        return new ServiceResponse<int>(true, "Operation successful", insertedId, 200);
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "Some error occured", 0, 500);
                    }
                }
                else
                {
                    string updateSql = @"
                        UPDATE [dbo].[tbl_StudentOtherInfo]
                        SET 
                            StudentType_id = @StudentType_id,
                            email_id = @email_id,
                            Hall_Ticket_Number = @Hall_Ticket_Number,
                            Identification_Mark_1 = @Identification_Mark_1,
                            Identification_Mark_2 = @Identification_Mark_2,
                            Admission_Date = @Admission_Date,
                            Student_Group_id = @Student_Group_id,
                            Register_Date = @Register_Date,
                            Register_Number = @Register_Number,
                            samagra_ID = @samagra_ID,
                            Place_of_Birth = @Place_of_Birth,
                            comments = @comments,
                            language_known = @language_known
                        WHERE Student_Other_Info_id = @Student_Other_Info_id;
                        SELECT @Student_Other_Info_id";
                    int affectedRows = await _connection.ExecuteAsync(updateSql, request);
                    if (affectedRows > 0)
                    {
                        return new ServiceResponse<int>(true, "Operation successful", request.Student_Other_Info_id, 200);
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "Some error occured", 0, 500);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<int>> AddUpdateStudentParentInfo(StudentParentInfo request)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (request.Student_Parent_Info_id == 0)
                    {
                        // Insert Student Parent Info
                        var addSql = @"
                    INSERT INTO [dbo].[tbl_StudentParentsInfo] ([Student_id],[Parent_Type_id],[First_Name],[Middle_Name],[Last_Name],[Contact_Number],[Bank_Account_no],[Bank_IFSC_Code],[Family_Ration_Card_Type],[Family_Ration_Card_no],[Date_of_Birth],[Aadhar_no],[PAN_card_no],[Residential_Address],[Occupation_id],[Designation],[Name_of_the_Employer],[Office_no],[Email_id],[Annual_Income],[File_Name])
                    VALUES (@Student_id,@Parent_Type_id,@First_Name,@Middle_Name,@Last_Name,@Contact_Number,@Bank_Account_no,@Bank_IFSC_Code,@Family_Ration_Card_Type,@Family_Ration_Card_no,@Date_of_Birth,@Aadhar_no,@PAN_card_no,@Residential_Address,@Occupation_id,@Designation,@Name_of_the_Employer,@Office_no,@Email_id,@Annual_Income,@File_Name); 
                    SELECT CAST(SCOPE_IDENTITY() as int);";
                        int insertedId = await _connection.ExecuteScalarAsync<int>(addSql, request, transaction);

                        // Insert Student Parent Office Info
                        var addOfficeSql = @"
                    INSERT INTO [dbo].[tbl_StudentParentsOfficeInfo] ([Student_id],[Parents_Type_id],[Office_Building_no],[Street],[Area],[City_id],[State_id],[Pincode])
                    VALUES (@Student_id,@Parents_Type_id,@Office_Building_no,@Street,@Area,@City_id,@State_id,@Pincode);";
                        await _connection.ExecuteAsync(addOfficeSql, request.studentParentOfficeInfo, transaction);

                        transaction.Commit();

                        if (insertedId > 0)
                        {
                            return new ServiceResponse<int>(true, "Operation successful", insertedId, 200);
                        }
                        else
                        {
                            return new ServiceResponse<int>(false, "Some error occurred", 0, 500);
                        }
                    }
                    else
                    {
                        // Update Student Parent Info
                        var updateSql = @"
                    UPDATE [dbo].[tbl_StudentParentsInfo] SET
                        [Student_id] = @Student_id,
                        [Parent_Type_id] = @Parent_Type_id,
                        [First_Name] = @First_Name,
                        [Middle_Name] = @Middle_Name,
                        [Last_Name] = @Last_Name,
                        [Contact_Number] = @Contact_Number,
                        [Bank_Account_no] = @Bank_Account_no,
                        [Bank_IFSC_Code] = @Bank_IFSC_Code,
                        [Family_Ration_Card_Type] = @Family_Ration_Card_Type,
                        [Family_Ration_Card_no] = @Family_Ration_Card_no,
                        [Date_of_Birth] = @Date_of_Birth,
                        [Aadhar_no] = @Aadhar_no,
                        [PAN_card_no] = @PAN_card_no,
                        [Residential_Address] = @Residential_Address,
                        [Occupation_id] = @Occupation_id,
                        [Designation] = @Designation,
                        [Name_of_the_Employer] = @Name_of_the_Employer,
                        [Office_no] = @Office_no,
                        [Email_id] = @Email_id,
                        [Annual_Income] = @Annual_Income,
                        [File_Name] = @File_Name
                    WHERE [Student_Parent_Info_id] = @Student_Parent_Info_id;";
                        int affectedRows = await _connection.ExecuteAsync(updateSql, request, transaction);

                        // Update Student Parent Office Info
                        var updateOfficeSql = @"
                    UPDATE [dbo].[tbl_StudentParentsOfficeInfo] SET
                        [Student_id] = @Student_id,
                        [Parents_Type_id] = @Parents_Type_id,
                        [Office_Building_no] = @Office_Building_no,
                        [Street] = @Street,
                        [Area] = @Area,
                        [City_id] = @City_id,
                        [State_id] = @State_id,
                        [Pincode] = @Pincode
                    WHERE [Student_Parent_Office_Info_id] = @Student_Parent_Office_Info_id;";
                        int affectedRowsOffice = await _connection.ExecuteAsync(updateOfficeSql, request.studentParentOfficeInfo, transaction);

                        transaction.Commit();

                        if (affectedRows > 0 && affectedRowsOffice > 0)
                        {
                            return new ServiceResponse<int>(true, "Operation successful", request.Student_Parent_Info_id, 200);
                        }
                        else
                        {
                            return new ServiceResponse<int>(false, "Some error occurred", 0, 500);
                        }
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ServiceResponse<int>(false, ex.Message, 0, 500);
                }
            }
        }

        public async Task<ServiceResponse<int>> AddOrUpdateStudentSiblings(StudentSibling sibling)
        {

            try
            {
                if (sibling.Student_Siblings_id == 0)
                {

                    var query = @"
                    INSERT INTO [dbo].[tbl_StudentSiblings] (
                        [Student_id],
                        [Name],
                        [Last_Name],
                        [Admission_Number],
                        [Date_of_Birth],
                        [Class_id],
                        [Selection_id],
                        [Institute_Name],
                        [Aadhar_no]
                    ) VALUES (
                        @Student_id,
                        @Name,
                        @Last_Name,
                        @Admission_Number,
                        @Date_of_Birth,
                        @Class_id,
                        @Selection_id,
                        @Institute_Name,
                        @Aadhar_no
                    );
                    SELECT CAST(SCOPE_IDENTITY() as int);
                ";

                    int insertedId = await _connection.ExecuteScalarAsync<int>(query, sibling);
                    if (insertedId > 0)
                    {
                        return new ServiceResponse<int>(true, "Operation successful", insertedId, 200);
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "Some error occured", 0, 500);
                    }
                }
                else
                {
                    try
                    {
                        var query = @"
                    UPDATE [dbo].[tbl_StudentSiblings] SET
                        [Student_id] = @Student_id,
                        [Name] = @Name,
                        [Last_Name] = @Last_Name,
                        [Admission_Number] = @Admission_Number,
                        [Date_of_Birth] = @Date_of_Birth,
                        [Class_id] = @Class_id,
                        [Selection_id] = @Selection_id,
                        [Institute_Name] = @Institute_Name,
                        [Aadhar_no] = @Aadhar_no
                    WHERE [Student_Siblings_id] = @Student_Siblings_id;
                ";
                        int affectedRows = await _connection.ExecuteAsync(query, sibling);
                        if (affectedRows > 0)
                        {
                            return new ServiceResponse<int>(true, "Operation successful", sibling.Student_Siblings_id, 200);
                        }
                        else
                        {
                            return new ServiceResponse<int>(false, "Some error occured", 0, 500);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new ServiceResponse<int>(false, "Some error occured", 0, 500);
                    }
                }

            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, "Some error occured", 0, 500);
            }
        }

        public async Task<ServiceResponse<int>> AddOrUpdateStudentPreviousSchool(StudentPreviousSchools previousSchool)
        {
            try
            {
                if (previousSchool.Student_Prev_School_id == 0)
                {

                    var query = @"
                        INSERT INTO [dbo].[tbl_StudentPreviousSchool] (
                            [student_id],
                            [Previous_School_Name],
                            [Previous_Board],
                            [Previous_Medium],
                            [Previous_School_Address],
                            [Previous_School_Group],
                            [Previous_Class],
                            [TC_number],
                            [TC_date],
                            [isTC_Submitted]
                        ) VALUES (
                            @student_id,
                            @Previous_School_Name,
                            @Previous_Board,
                            @Previous_Medium,
                            @Previous_School_Address,
                            @Previous_School_Group,
                            @Previous_Class,
                            @TC_number,
                            @TC_date,
                            @isTC_Submitted
                        );
                        SELECT CAST(SCOPE_IDENTITY() as int);
                        ";
                    int insertedId = await _connection.ExecuteScalarAsync<int>(query, previousSchool);

                    if (insertedId > 0)
                    {
                        return new ServiceResponse<int>(true, "Operation successful", insertedId, 200);
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "Some error occured", 0, 500);
                    }
                }
                else
                {
                    var query = @"
                        UPDATE [dbo].[tbl_StudentPreviousSchool] SET
                            [student_id] = @student_id,
                            [Previous_School_Name] = @Previous_School_Name,
                            [Previous_Board] = @Previous_Board,
                            [Previous_Medium] = @Previous_Medium,
                            [Previous_School_Address] = @Previous_School_Address,
                            [Previous_School_Group] = @Previous_School_Group,
                            [Previous_Class] = @Previous_Class,
                            [TC_number] = @TC_number,
                            [TC_date] = @TC_date,
                            [isTC_Submitted] = @isTC_Submitted
                        WHERE [Student_Prev_School_id] = @Student_Prev_School_id;
                        ";
                    int affectedRows = await _connection.ExecuteAsync(query, previousSchool);
                    if (affectedRows > 0)
                    {
                        return new ServiceResponse<int>(true, "Operation successful", previousSchool.Student_Prev_School_id, 200);
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "Some error occured", 0, 500);
                    }

                }
              ;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, "Some error occured", 0, 500);
            }
        }

        public async Task<ServiceResponse<int>> AddOrUpdateStudentHealthInfo(StudentHealthInfos healthInfo)
        {
            try
            {
                if (healthInfo.Student_Health_Info_id == 0)
                {

                    var query = @"
                    INSERT INTO [dbo].[tbl_StudentHealthInfo] (
                        [Student_id], [Allergies], [Medications], [Doctor_Name], [Doctor_Phone_no], 
                        [height], [weight], [Government_ID], [BCG], [MMR_Measles], [Polio], 
                        [Hepatitis], [Triple_Antigen], [Others], [General_Health], [Head_Eye_ENT], 
                        [Chest], [CVS], [Abdomen], [Genitalia], [Congenital_Disease], [Physical_Deformity], 
                        [History_Majorillness], [History_Accident], [Vision], [Hearing], [Speech], 
                        [Behavioral_Problem], [Remarks_Weakness], [Student_Name], [Student_Age]
                    ) VALUES (
                        @Student_id, @Allergies, @Medications, @Doctor_Name, @Doctor_Phone_no, 
                        @height, @weight, @Government_ID, @BCG, @MMR_Measles, @Polio, 
                        @Hepatitis, @Triple_Antigen, @Others, @General_Health, @Head_Eye_ENT, 
                        @Chest, @CVS, @Abdomen, @Genitalia, @Congenital_Disease, @Physical_Deformity, 
                        @History_Majorillness, @History_Accident, @Vision, @Hearing, @Speech, 
                        @Behavioral_Problem, @Remarks_Weakness, @Student_Name, @Student_Age
                    );
                    SELECT CAST(SCOPE_IDENTITY() as int);";
                    int insertedId = await _connection.ExecuteScalarAsync<int>(query, healthInfo);
                    if (insertedId > 0)
                    {
                        return new ServiceResponse<int>(true, "Operation successful", insertedId, 200);
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "Some error occured", 0, 500);
                    }
                }
                else
                {
                    var query = @"
                    UPDATE [dbo].[tbl_StudentHealthInfo] SET
                        [Student_id] = @Student_id,
                        [Allergies] = @Allergies,
                        [Medications] = @Medications,
                        [Doctor_Name] = @Doctor_Name,
                        [Doctor_Phone_no] = @Doctor_Phone_no,
                        [height] = @height,
                        [weight] = @weight,
                        [Government_ID] = @Government_ID,
                        [BCG] = @BCG,
                        [MMR_Measles] = @MMR_Measles,
                        [Polio] = @Polio,
                        [Hepatitis] = @Hepatitis,
                        [Triple_Antigen] = @Triple_Antigen,
                        [Others] = @Others,
                        [General_Health] = @General_Health,
                        [Head_Eye_ENT] = @Head_Eye_ENT,
                        [Chest] = @Chest,
                        [CVS] = @CVS,
                        [Abdomen] = @Abdomen,
                        [Genitalia] = @Genitalia,
                        [Congenital_Disease] = @Congenital_Disease,
                        [Physical_Deformity] = @Physical_Deformity,
                        [History_Majorillness] = @History_Majorillness,
                        [History_Accident] = @History_Accident,
                        [Vision] = @Vision,
                        [Hearing] = @Hearing,
                        [Speech] = @Speech,
                        [Behavioral_Problem] = @Behavioral_Problem,
                        [Remarks_Weakness] = @Remarks_Weakness,
                        [Student_Name] = @Student_Name,
                        [Student_Age] = @Student_Age
                    WHERE [Student_Health_Info_id] = @Student_Health_Info_id;";
                    int affectedRows = await _connection.ExecuteAsync(query, healthInfo);
                    if (affectedRows > 0)
                    {
                        return new ServiceResponse<int>(true, "Operation successful", healthInfo.Student_Health_Info_id, 200);
                    }
                    else
                    {
                        return new ServiceResponse<int>(false, "Some error occured", 0, 500);
                    }

                }

            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, "Some error occured", 0, 500);
            }
        }
        public async Task<ServiceResponse<List<StudentDetailsDTO>>> GetAllStudentDetails(GetStudentRequestModel obj)
        {
            try
            {
                const int MaxPageSize = int.MaxValue;
                int actualPageSize = obj.pageSize ?? MaxPageSize;
                int actualPageNumber = obj.pageNumber ?? 1;
                int offset = (actualPageNumber - 1) * actualPageSize;
                var allowedSortFields = new List<string>
        {
            "Student_Name", "Admission_Number", "Date_of_Joining", "Roll_Number"
        };
                var allowedSortDirections = new List<string> { "ASC", "DESC" };

                // Validate sort field
                if (!allowedSortFields.Contains(obj.sortField))
                {
                    obj.sortField = "Student_Name";  // Default to Student_Name if invalid
                }

                // Validate sort direction
                obj.sortDirection = obj.sortDirection.ToUpper();
                if (!allowedSortDirections.Contains(obj.sortDirection))
                {
                    obj.sortDirection = "ASC";  // Default to ASC if invalid
                }
                // Assume that Parent_Type_id = 1 means father
                string sql = $@"
            -- Insert data into the temporary table using SELECT INTO
            IF OBJECT_ID('tempdb..#TempStudentDetails') IS NOT NULL DROP TABLE #TempStudentDetails;

            SELECT 
                tbl_StudentMaster.student_id, 
                CONCAT(tbl_StudentMaster.First_Name, ' ', tbl_StudentMaster.Last_Name) AS Student_Name, 
                class_name AS class_course, 
                Section_name AS Section, 
                Admission_Number, 
                Roll_Number,
                Date_of_Joining,
                tbl_StudentMaster.Date_of_Birth,
                Religion_Type, 
                Gender_Type,
                CONCAT(tbl_StudentParentsInfo.First_Name, ' ', tbl_StudentParentsInfo.Last_Name) AS Father_Name 
            INTO 
                #TempStudentDetails
            FROM 
                [dbo].[tbl_StudentMaster]
            LEFT JOIN 
                tbl_StudentOtherInfo ON tbl_StudentOtherInfo.Student_id = tbl_StudentMaster.Student_id
            LEFT JOIN 
                tbl_Class ON tbl_StudentMaster.class_id = tbl_Class.Class_id
            LEFT JOIN 
                tbl_Section ON tbl_StudentMaster.section_id = tbl_Section.section_id
            LEFT JOIN 
                tbl_Religion ON tbl_StudentMaster.Religion_id = tbl_Religion.Religion_id
            LEFT JOIN 
                tbl_Gender ON tbl_StudentMaster.gender_id = tbl_Gender.gender_id
            LEFT JOIN 
                tbl_StudentParentsInfo ON tbl_StudentMaster.student_id = tbl_StudentParentsInfo.Student_id 
                AND tbl_StudentParentsInfo.Parent_Type_id = 1 
            WHERE 
                tbl_StudentMaster.Institute_id = @InstituteId AND (tbl_StudentMaster.Class_id = @class_id OR @class_id = 0)
                AND (tbl_StudentMaster.Section_id = @section_id OR @section_id = 0) AND (tbl_StudentMaster.Academic_year_id = @Academic_year_id OR @Academic_year_id = 0)
                AND (tbl_StudentOtherInfo.StudentType_id = @StudentType_id OR @StudentType_id = 0)
                AND tbl_StudentMaster.isActive = @isActive;

            -- Query the temporary table with sorting and pagination
            SELECT 
                *
            FROM 
                #TempStudentDetails
            ORDER BY 
                {obj.sortField} {obj.sortDirection}, 
                student_id
            OFFSET 
                @Offset ROWS
            FETCH NEXT 
                @PageSize ROWS ONLY;

            -- Get the total count of records
            SELECT 
                COUNT(*) 
            FROM 
                #TempStudentDetails;";

                using (var multi = await _connection.QueryMultipleAsync(sql, new { InstituteId = obj.Institute_id, Offset = offset, PageSize = actualPageSize, class_id = obj.class_id, section_id = obj.section_id , Academic_year_id  =obj.Academic_year_id, isActive=obj.isActive, StudentType_id = obj.StudentType_id }))
                {
                    var studentList = multi.Read<StudentDetailsDTO>().ToList();
                    int? totalRecords = (obj.pageSize.HasValue && obj.pageNumber.HasValue) == true ? multi.ReadSingle<int>() : null;

                    if (studentList.Any())
                    {
                        return new ServiceResponse<List<StudentDetailsDTO>>(true, "Operation successful", studentList, 200, totalRecords);
                    }
                    else
                    {
                        return new ServiceResponse<List<StudentDetailsDTO>>(false, "Student not found", null, 404);
                    }
                }
            }

            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentDetailsDTO>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<int>> ChangeStudentStatus(StudentStatusDTO statusDTO)
        {
            try
            {
                string sql = @"
                    update tbl_StudentMaster
                    SET IsActive = @IsActive
                    where student_id = @StudentId;";

                var updatedId = await _connection.ExecuteScalarAsync<int>(sql, statusDTO);
                return new ServiceResponse<int>(true, "Operation successful", updatedId, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, "Some error occured", 0, 500);
            }
        }

        public async Task<ServiceResponse<int>> AddUpdateStudentDocuments(StudentDocumentListDTO request, int Student_id)
        {
            try
            {

                if (request.Student_Documents_id == 0)
                {

                    var newDocument = new StudentDocuments
                    {
                        Student_id = Student_id,
                        Document_Name = request.Document_Name,
                        File_Path = request.File_Path,
                        File_Name = request.File_Name
                    };

                    string insertSql = @"
                INSERT INTO [dbo].[tbl_StudentDocuments] (student_id, Document_Name, File_Name , File_Path)
                VALUES (@Student_id, @Document_Name, @File_Name, @File_Path);
                SELECT CAST(SCOPE_IDENTITY() as int);";

                    int insertedId = await _connection.ExecuteScalarAsync<int>(insertSql, newDocument);
                    if (insertedId == 0)
                    {
                        return new ServiceResponse<int>(false, "Some error occurred during insertion", 0, 500);
                    }

                    //    var newStudentDocument = new StudentDocumentMaster
                    //    {
                    //        Student_Document_id = insertedId,
                    //        Student_Document_Name = newDocument.Document_Name,
                    //        en_date = DateTime.Now
                    //    };

                    //    string documentSql = @"
                    //INSERT INTO [dbo].[tbl_StudentDocumentMaster] (Student_Document_id, Student_Document_Name, en_date)
                    //VALUES (@Student_Document_id , @Student_Document_Name, @en_date);";

                    //    await _connection.ExecuteAsync(documentSql, newStudentDocument);

                    return new ServiceResponse<int>(true, "Insertion successful", 1, 200);
                }
                else
                {

                    string updateSql = @"
                UPDATE [dbo].[tbl_StudentDocuments] 
                SET 
                    Document_Name = @Document_Name, 
                    File_Name = @File_Name, 
                    File_Path = @File_Path
                WHERE 
                    Student_Documents_id = @Student_Documents_id";

                    await _connection.ExecuteAsync(updateSql, request);


                    return new ServiceResponse<int>(true, "Update successful", 1, 200);
                }



            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }

        public async Task<ServiceResponse<int>> DeleteStudentDocument(int Student_Documents_id)
        {
            try
            {
                string sql = @"
                    update tbl_StudentDocuments
                    SET isDelete = 1
                    where Student_Documents_id = @Student_Documents_id;";

                var updatedId = await _connection.ExecuteScalarAsync<int>(sql, new { Student_Documents_id });
                return new ServiceResponse<int>(true, "Operation successful", updatedId, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, "Some error occured", 0, 500);
            }
        }
    }
}


