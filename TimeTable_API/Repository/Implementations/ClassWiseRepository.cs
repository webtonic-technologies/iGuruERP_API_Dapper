using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.Repository.Interfaces;

namespace TimeTable_API.Repository.Implementations
{
    public class ClassWiseRepository : IClassWiseRepository
    {
        private readonly IDbConnection _connection;

        public ClassWiseRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<ClassWiseResponse>> GetClassWiseTimeTables(ClassWiseRequest request)
        {
            var response = new ServiceResponse<ClassWiseResponse>(
                success: false,
                message: "Initialization failed",
                data: new ClassWiseResponse(),
                statusCode: 500
            );

            try
            {
                var classWiseResponse = new ClassWiseResponse();

                // Fetch all classes for the given InstituteID
                var classes = await _connection.QueryAsync<ClassDetail>(
                    @"SELECT class_id as ClassID, class_name as ClassName
                      FROM tbl_Class
                      WHERE institute_id = @InstituteID",
                    new { request.InstituteID });

                foreach (var classDetail in classes)
                {
                    // Fetch sections for each class
                    var sections = await _connection.QueryAsync<SectionDetail>(
                        @"SELECT section_id as SectionID, section_name as SectionName
                          FROM tbl_Section
                          WHERE class_id = @ClassID",
                        new { ClassID = classDetail.ClassID });

                    foreach (var section in sections)
                    {
                        // Get GroupID by ClassID and SectionID
                        var groupId = await _connection.ExecuteScalarAsync<int?>(
                            @"SELECT GroupID FROM tblTimeTableClassSession
                              WHERE ClassID = @ClassID AND SectionID = @SectionID",
                            new { ClassID = classDetail.ClassID, SectionID = section.SectionID });

                        // If GroupID is found, calculate NumberOfSessions and NumberOfSubjects
                        if (groupId.HasValue)
                        {
                            // Count Number of Sessions for the group
                            section.NumberOfSessions = await _connection.ExecuteScalarAsync<int>(
                                @"SELECT COUNT(*) FROM tblTimeTableSessions
                                  WHERE GroupID = @GroupID",
                                new { GroupID = groupId });

                            // Count Number of Subjects for the group
                            section.NumberOfSubjects = await _connection.ExecuteScalarAsync<int>(
                                @"SELECT COUNT(DISTINCT tse.SubjectID)
                                  FROM tblTimeTableSessionMapping tsm
                                  JOIN tblTimeTableSessionSubjectEmployee tse ON tsm.TTSessionID = tse.TTSessionID
                                  WHERE tsm.GroupID = @GroupID",
                                new { GroupID = groupId });
                        }
                    }

                    classDetail.SectionList.AddRange(sections);
                    classWiseResponse.ClassList.Add(classDetail);
                }

                // Set the response as successful
                response = new ServiceResponse<ClassWiseResponse>(
                    success: true,
                    message: "Class and Section details fetched successfully.",
                    data: classWiseResponse,
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                // Set response in case of failure
                response = new ServiceResponse<ClassWiseResponse>(
                    success: false,
                    message: ex.Message,
                    data: null,
                    statusCode: 500
                );
            }

            return response;
        }
    }
}
