﻿using Student_API.Repository.Interfaces;
using System.Data;
using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Dapper;
using Student_API.Models;
using System.Collections.Generic;

namespace Student_API.Repository.Implementations
{
    public class StudentInformationRepository : IStudentInformationRepository
    {
        private readonly IDbConnection _connection;

        public StudentInformationRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<StudentInformationDTO>> GetStudentDetailsById(int studentId)
        {
            try
            {
                string sql = @"
                    SELECT * FROM [dbo].[tbl_StudentMaster] WHERE student_id = @studentId;
                    SELECT * FROM [dbo].[tbl_StudentOtherInfo] WHERE student_id = @studentId;
                    SELECT * FROM [dbo].[tbl_StudentParentsInfo] WHERE student_id = @studentId;
                    SELECT * FROM [dbo].[tbl_StudentSiblings] WHERE student_id = @studentId;
                    SELECT * FROM [dbo].[tbl_StudentPreviousSchool] WHERE student_id = @studentId;
                    SELECT * FROM [dbo].[tbl_StudentHealthInfo] WHERE student_id = @studentId;";

                using (var result = await _connection.QueryMultipleAsync(sql, new { studentId }))
                {
                    var studentDetails = await result.ReadFirstOrDefaultAsync<StudentInformationDTO>();
                    var studentOtherInfo = await result.ReadFirstOrDefaultAsync<StudentOtherInfoDTO>();
                    var StudentParentInfo = await result.ReadAsync<StudentParentInfoDTO>();
                    var StudentSiblings = await result.ReadFirstOrDefaultAsync<StudentSiblings>();
                    var StudentPreviousSchool = await result.ReadFirstOrDefaultAsync<StudentPreviousSchool>();
                    var StudentHealthInfo = await result.ReadFirstOrDefaultAsync<StudentHealthInfo>();

                    if (studentDetails != null)
                    {
                        studentDetails.studentOtherInfoDTO = studentOtherInfo;
                        studentDetails.studentParentInfos = StudentParentInfo.ToList();
                        studentDetails.studentSiblings = StudentSiblings;
                        studentDetails.studentPreviousSchool = StudentPreviousSchool;
                        studentDetails.studentHealthInfo = StudentHealthInfo;
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

        public async Task<ServiceResponse<int>> AddUpdateStudentInformation(StudentMasterDTO request)
        {
            try
            {
                var newStudent = new StudentMaster
                {
                    student_id = request.student_id,
                    First_Name = request.First_Name,
                    Middle_Name = request.Middle_Name,
                    Last_Name = request.Last_Name,
                    gender_id = request.gender_id,
                    class_id = request.class_id,
                    section_id = request.section_id,
                    Admission_Number = request.Admission_Number,
                    Roll_Number = request.Roll_Number,
                    Date_of_Joining = request.Date_of_Joining,
                    Academic_Year = request.Academic_Year,
                    Nationality_id = request.Nationality_id,
                    Religion_id = request.Religion_id,
                    Date_of_Birth = request.Date_of_Birth,
                    Mother_Tongue_id = request.Mother_Tongue_id,
                    Caste_id = request.Caste_id,
                    First_Language = request.First_Language,
                    Second_Language = request.Second_Language,
                    Third_Language = request.Third_Language,
                    Medium = request.Medium,
                    Blood_Group_id = request.Blood_Group_id,
                    App_User_id = request.App_User_id,
                    Aadhar_Number = request.Aadhar_Number,
                    NEP = request.NEP,
                    QR_code = request.QR_code
                };

                if (request.student_id == 0)
                {

                    string sql = @"
                    INSERT INTO [dbo].[tbl_StudentMaster] (
                        First_Name, Middle_Name, Last_Name, gender_id, class_id,
                        section_id, Admission_Number, Roll_Number, Date_of_Joining,
                        Academic_Year, Nationality_id, Religion_id, Date_of_Birth,
                        Mother_Tongue_id, Caste_id, First_Language, Second_Language,
                        Third_Language, Medium, Blood_Group_id, App_User_id, Aadhar_Number,
                        NEP, QR_code)
                    VALUES (
                        @First_Name, @Middle_Name, @Last_Name, @gender_id, @class_id,
                        @section_id, @Admission_Number, @Roll_Number, @Date_of_Joining,
                        @Academic_Year, @Nationality_id, @Religion_id, @Date_of_Birth,
                        @Mother_Tongue_id, @Caste_id, @First_Language, @Second_Language,
                        @Third_Language, @Medium, @Blood_Group_id, @App_User_id, @Aadhar_Number,
                        @NEP, @QR_code);
                    SELECT SCOPE_IDENTITY();";

                    int insertedId = await _connection.ExecuteScalarAsync<int>(sql, newStudent);
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
                        QR_code = @QR_code
                    WHERE student_id = @student_id";
                    // Execute the query and retrieve the number of affected rows
                    int affectedRows = await _connection.ExecuteAsync(sql, newStudent);
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

        public async Task<ServiceResponse<int>> AddUpdateStudentOtherInfo(StudentOtherInfoDTO request)
        {
            try
            {
                if (request.Student_Other_Info_id == 0)
                {
                    string addSql = @"
                        INSERT INTO [dbo].[tbl_StudentOtherInfo] (student_id, StudentType_id, email_id, Hall_Ticket_Number, Exam_Board_id, Identification_Mark_1, Identification_Mark_2, Admission_Date, Student_Group_id, Register_Date, Register_Number, samagra_ID, Place_of_Birth, comments, language_known)
                        VALUES (@student_id, @StudentType_id, @email_id, @Hall_Ticket_Number, @Exam_Board_id, @Identification_Mark_1, @Identification_Mark_2, @Admission_Date, @Student_Group_id, @Register_Date, @Register_Number, @samagra_ID, @Place_of_Birth, @comments, @language_known);
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
                            Exam_Board_id = @Exam_Board_id,
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

        public async Task<ServiceResponse<int>> AddUpdateStudentParentInfo(StudentParentInfoDTO request)
        {
            try
            {
                if (request.Student_Parent_Info_id == 0)
                {
                    var addSql = @"
                        INSERT INTO [dbo].[tbl_StudentParentsInfo] ([Student_id],[Parent_Type_id],[First_Name],[Middle_Name],[Last_Name],[Contact_Number],[Bank_Account_no],[Bank_IFSC_Code],[Family_Ration_Card_Type],[Family_Ration_Card_no],[Mobile_Number],[Date_of_Birth],[Aadhar_no],[PAN_card_no],[Residential_Address],[Occupation_id],[Designation],[Name_of_the_Employer],[Office_no],[Email_id],[Annual_Income],[File_Name])
                        VALUES (@Student_id,@Parent_Type_id,@First_Name,@Middle_Name,@Last_Name,@Contact_Number,@Bank_Account_no,@Bank_IFSC_Code,@Family_Ration_Card_Type,@Family_Ration_Card_no,@Mobile_Number,@Date_of_Birth,@Aadhar_no,@PAN_card_no,@Residential_Address,@Occupation_id,@Designation,@Name_of_the_Employer,@Office_no,@Email_id,@Annual_Income,@File_Name); 
                        SELECT CAST(SCOPE_IDENTITY() as int);";
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
                        [Mobile_Number] = @Mobile_Number,
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
                    int affectedRows = await _connection.ExecuteAsync(updateSql, request);
                    if (affectedRows > 0)
                    {
                        return new ServiceResponse<int>(true, "Operation successful", request.Student_Parent_Info_id, 200);
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

        public async Task<ServiceResponse<int>> AddOrUpdateStudentSiblings(StudentSiblings sibling)
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

        public async Task<ServiceResponse<int>> AddOrUpdateStudentPreviousSchool(StudentPreviousSchool previousSchool)
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

        public async Task<ServiceResponse<int>> AddOrUpdateStudentHealthInfo(StudentHealthInfo healthInfo)
        {
            try
            {
                if (healthInfo.Student_Health_Info_id == 0)
                {

                    var query = @"
                    INSERT INTO [dbo].[tbl_StudentHealthInfo] (
                        [Student_id],
                        [Allergies],
                        [Medications],
                        [Doctor_Name],
                        [Doctor_Phone_no],
                        [height],
                        [weight],
                        [Government_ID],
                        [BCG],
                        [MMR_Measles],
                        [Polio]
                    ) VALUES (
                        @Student_id,
                        @Allergies,
                        @Medications,
                        @Doctor_Name,
                        @Doctor_Phone_no,
                        @height,
                        @weight,
                        @Government_ID,
                        @BCG,
                        @MMR_Measles,
                        @Polio
                    );
                    SELECT CAST(SCOPE_IDENTITY() as int);
                ";
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
                        [Polio] = @Polio
                    WHERE [Student_Health_Info_id] = @Student_Health_Info_id;
                ";
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
        public async Task<ServiceResponse<List<StudentDetailsDTO>>> GetAllStudentDetails()
        {
            try
            {
                // Assume that Parent_Type_id = 1 means father
                string sql = @"
                    SELECT tbl_StudentMaster.student_id , tbl_StudentMaster.First_Name , tbl_StudentMaster.Last_Name , class_name , section_name , Admission_Number , Roll_Number ,Date_of_Joining,tbl_StudentMaster.Date_of_Birth,Religion_Type , Gender_Type ,CONCAT(tbl_StudentParentsInfo.First_Name, ' ', tbl_StudentParentsInfo.Last_Name) AS Father_Name FROM [dbo].[tbl_StudentMaster]
                    INNER JOIN tbl_Class ON tbl_StudentMaster.class_id = tbl_Class.class_id 
                    INNER JOIN tbl_Section ON tbl_StudentMaster.section_id = tbl_Section.section_id
                    INNER JOIN tbl_Religion ON tbl_StudentMaster.Religion_id = tbl_Religion.Religion_id
                    INNER JOIN tbl_Gender ON tbl_StudentMaster.gender_id = tbl_Gender.Gender_id
                    INNER JOIN tbl_StudentParentsInfo ON tbl_StudentMaster.student_id = tbl_StudentParentsInfo.Student_id AND tbl_StudentParentsInfo.Parent_Type_id = 1";
                var StudentList = await _connection.QueryAsync<StudentDetailsDTO>(sql);

                if (StudentList != null)
                {
                    return new ServiceResponse<List<StudentDetailsDTO>>(true, "Operation successful", StudentList.ToList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<StudentDetailsDTO>>(false, "Student not found", null, 404);
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

                    var newStudentDocument = new StudentDocumentMaster
                    {
                        Student_Document_id = insertedId,
                        Student_Document_Name = newDocument.Document_Name,
                        en_date = DateTime.Now
                    };

                    string documentSql = @"
                INSERT INTO [dbo].[tbl_StudentDocumentMaster] (Student_Document_id, Student_Document_Name, en_date)
                VALUES (@Student_Document_id , @Student_Document_Name, @en_date);";

                    await _connection.ExecuteAsync(documentSql, newStudentDocument);

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
    }
}

