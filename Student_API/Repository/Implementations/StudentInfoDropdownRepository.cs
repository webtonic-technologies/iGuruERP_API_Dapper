﻿using Dapper;
using Student_API.DTOs.ServiceResponse;
using Student_API.Models;
using Student_API.Repository.Interfaces;
using System.Data;


namespace Student_API.Repository.Implementations
{
    public class StudentInfoDropdownRepository : IStudentInfoDropdownRepository
    {
        private readonly IDbConnection _connection;

        public StudentInfoDropdownRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<ServiceResponse<List<Gender>>> GetAllGenders()
        {
            try
            {
                string sql = @"
            SELECT [Gender_id], [Gender_Type]
            FROM [dbo].[tbl_Gender]";

                var genders = await _connection.QueryAsync<Gender>(sql);

                if (genders != null && genders.Any())
                {
                    return new ServiceResponse<List<Gender>>(true, "Operation successful", genders.ToList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<Gender>>(false, "Genders not found", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Gender>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<List<Sections>>> GetAllSections(int Class_Id)
        {
            try
            {
                string sql = @"
            SELECT Section_id AS section_id, section_name AS [section_name]
            FROM [dbo].[tbl_Section] where class_id = @Class_Id";

                var sections = await _connection.QueryAsync<Sections>(sql, new { Class_Id = Class_Id });

                if (sections != null && sections.Any())
                {
                    return new ServiceResponse<List<Sections>>(true, "Operation successful", sections.ToList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<Sections>>(false, "Sections not found", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Sections>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<List<Class>>> GetAllClass(int institute_id)
        {
            try
            {
                string sql = @"
            SELECT class_id, class_name 
            FROM [dbo].[tbl_class] where institute_id = @institute_id";

                var classes = await _connection.QueryAsync<Class>(sql, new { institute_id = institute_id });

                if (classes != null && classes.Any())
                {
                    return new ServiceResponse<List<Class>>(true, "Operation successful", classes.ToList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<Class>>(false, "Sections not found", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Class>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<ClassWithSections>>> GetAllClassesWithSections(int institute_id)
        {
            try
            {
                string sql = @"
                SELECT c.class_id, c.class_name, s.section_id AS section_id, s.section_name AS section_name
                FROM [dbo].[tbl_class] c
                LEFT JOIN [dbo].[tbl_Section] s ON c.class_id = s.class_id
                WHERE c.institute_id = @institute_id";

                var classWithSections = await _connection.QueryAsync<dynamic>(sql, new { institute_id = institute_id });

                if (classWithSections != null && classWithSections.Any())
                {
                    var groupedClasses = classWithSections
                        .GroupBy(c => new { class_id = (int)c.class_id, class_name = (string)c.class_name })
                        .Select(g => new ClassWithSections
                        {
                            class_id = g.Key.class_id,
                            class_name = g.Key.class_name,
                            sections = g.Select(s => new Sections
                            {
                                section_id = (int)s.section_id,
                                section_name = (string)s.section_name
                            }).ToList()
                        }).ToList();

                    return new ServiceResponse<List<ClassWithSections>>(true, "Operation successful", groupedClasses, 200);
                }
                else
                {
                    return new ServiceResponse<List<ClassWithSections>>(false, "Classes and sections not found", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ClassWithSections>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<BloodGroup>>> GetAllBloodGroups()
        {
            try
            {
                string sql = @"
            SELECT [Blood_Group_id], [Blood_Group_Type]
            FROM [dbo].[tbl_BloodGroup]";

                var bloodGroups = await _connection.QueryAsync<BloodGroup>(sql);

                if (bloodGroups != null && bloodGroups.Any())
                {
                    return new ServiceResponse<List<BloodGroup>>(true, "Operation successful", bloodGroups.ToList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<BloodGroup>>(false, "Blood groups not found", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<BloodGroup>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<StudentType>>> GetAllStudentTypes()
        {
            try
            {
                string sql = @"
            SELECT [Student_Type_id], [Student_Type_Name]
            FROM [dbo].[tbl_StudentType]";

                var studentTypes = await _connection.QueryAsync<StudentType>(sql);

                if (studentTypes != null && studentTypes.Any())
                {
                    return new ServiceResponse<List<StudentType>>(true, "Operation successful", studentTypes.ToList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<StudentType>>(false, "Student types not found", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentType>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<StudentGroup>>> GetAllStudentGroups()
        {
            try
            {
                string sql = @"
            SELECT [Student_Group_id], [Student_Group_Type]
            FROM [dbo].[tbl_StudentGroup]";

                var studentGroups = await _connection.QueryAsync<StudentGroup>(sql);

                if (studentGroups != null && studentGroups.Any())
                {
                    return new ServiceResponse<List<StudentGroup>>(true, "Operation successful", studentGroups.ToList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<StudentGroup>>(false, "Student groups not found", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentGroup>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<Occupation>>> GetAllOccupations()
        {
            try
            {
                string sql = @"
            SELECT [Occupation_id], [Occupation_Type]
            FROM [dbo].[tbl_Occupation]";

                var occupations = await _connection.QueryAsync<Occupation>(sql);

                if (occupations != null && occupations.Any())
                {
                    return new ServiceResponse<List<Occupation>>(true, "Operation successful", occupations.ToList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<Occupation>>(false, "Occupations not found", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Occupation>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<ParentType>>> GetAllParentTypes()
        {
            try
            {
                string sql = @"
            SELECT [parent_type_id], [parent_type]
            FROM [dbo].[tbl_ParentType]";

                var parentTypes = await _connection.QueryAsync<ParentType>(sql);

                if (parentTypes != null && parentTypes.Any())
                {
                    return new ServiceResponse<List<ParentType>>(true, "Operation successful", parentTypes.ToList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<ParentType>>(false, "Parent types not found", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ParentType>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<List<State>>> GetAllStates()
        {
            try
            {
                string sql = @"
            SELECT [State_id], [State_Name]
            FROM [dbo].[tbl_State]";

                var states = await _connection.QueryAsync<State>(sql);

                if (states != null && states.Any())
                {
                    return new ServiceResponse<List<State>>(true, "Operation successful", states.ToList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<State>>(false, "States not found", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<State>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<City>>> GetAllCities(int stateId)
        {
            try
            {
                string sql = @"
            SELECT [City_id], [City_Name]
            FROM [dbo].[tbl_City]
            WHERE [State_id] = @StateId";

                var cities = await _connection.QueryAsync<City>(sql, new { StateId = stateId });

                if (cities != null && cities.Any())
                {
                    return new ServiceResponse<List<City>>(true, "Operation successful", cities.ToList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<City>>(false, "Cities not found", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<City>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<Academic>>> GetAllAcademic()
        {
            try
            {
                string sql = @"
            SELECT *
            FROM [dbo].[tbl_AcademicYear]";

                var states = await _connection.QueryAsync<Academic>(sql);

                if (states != null && states.Any())
                {
                    return new ServiceResponse<List<Academic>>(true, "Operation successful", states.ToList(), 200);
                }
                else
                {
                    return new ServiceResponse<List<Academic>>(false, "Academic Year not found", null, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Academic>>(false, ex.Message, null, 500);
            }
        }
    }
}
