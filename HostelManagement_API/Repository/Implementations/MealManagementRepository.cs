using Dapper;
using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using HostelManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Implementations
{
    public class MealManagementRepository : IMealManagementRepository
    {
        private readonly string _connectionString;

        public MealManagementRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<ServiceResponse<string>> AddMealType(AddMealTypeRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        string sqlQuery = request.MealTypeID == 0
                            ? @"INSERT INTO tblMealType (MealType, DayIDs, StartTime, EndTime, InstituteID, IsActive) 
                                VALUES (@MealType, @DayIDs, @StartTime, @EndTime, @InstituteID, @IsActive); 
                                SELECT CAST(SCOPE_IDENTITY() as int)"
                            : @"UPDATE tblMealType 
                                SET MealType = @MealType, DayIDs = @DayIDs, StartTime = @StartTime, EndTime = @EndTime, 
                                    InstituteID = @InstituteID, IsActive = @IsActive
                                WHERE MealTypeID = @MealTypeID";

                        var mealTypeId = request.MealTypeID == 0
                            ? await db.ExecuteScalarAsync<int>(sqlQuery, new
                            {
                                request.MealType,
                                request.DayIDs,
                                request.StartTime,
                                request.EndTime,
                                request.InstituteID,
                                request.IsActive
                            }, transaction)
                            : await db.ExecuteAsync(sqlQuery, new
                            {
                                request.MealType,
                                request.DayIDs,
                                request.StartTime,
                                request.EndTime,
                                request.InstituteID,
                                request.IsActive,
                                request.MealTypeID
                            }, transaction);

                        // Insert Meal Documents if any
                        if (request.MealDocuments != null && request.MealDocuments.Count > 0)
                        {
                            foreach (var doc in request.MealDocuments)
                            {
                                string docQuery = @"INSERT INTO tblMealTypeDocs (MealTypeID, DocFile) 
                                                    VALUES (@MealTypeID, @DocFile)";
                                await db.ExecuteAsync(docQuery, new { MealTypeID = mealTypeId, doc.DocFile }, transaction);
                            }
                        }

                        transaction.Commit();
                        return new ServiceResponse<string>(true, "Meal Type Added/Updated Successfully", "Success", 200);
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }


        public async Task<ServiceResponse<GetMealTypeResponse>> GetMealType(GetMealTypeRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();

                string sqlQuery = @" 
                SELECT 
                    MealTypeID, 
                    MealType, 
                    DayIDs, 
                    CONVERT(VARCHAR(5), StartTime, 108) AS StartTime, 
                    CONVERT(VARCHAR(5), EndTime, 108) AS EndTime
                FROM tblMealType
                WHERE InstituteID = @InstituteID AND IsActive = 1";

                var mealType = await db.QueryFirstOrDefaultAsync<GetMealTypeResponse>(
                    sqlQuery, new { InstituteID = request.InstituteID });

                if (mealType != null)
                {
                    // Fetch DayTypes based on DayIDs
                    string[] dayIds = mealType.DayIDs.Split(',');
                    string dayTypes = string.Join(", ", dayIds.Select(dayId => GetDayTypeById(int.Parse(dayId))));

                    mealType.DayTypes = dayTypes;

                    return new ServiceResponse<GetMealTypeResponse>(true, "Meal Type Retrieved Successfully", mealType, 200);
                }

                return new ServiceResponse<GetMealTypeResponse>(false, "Meal Type Not Found", null, 404);
            }
        }



        private string GetDayTypeById(int dayId)
        {
            // This could be a switch statement or DB lookup if necessary
            switch (dayId)
            {
                case 1: return "Mon";
                case 2: return "Tue";
                case 3: return "Wed";
                case 4: return "Thu";
                case 5: return "Fri";
                case 6: return "Sat";
                case 7: return "Sun";
                default: return "Unknown";
            }
        }


        public async Task<ServiceResponse<string>> DeleteMealType(DeleteMealTypeRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                var sqlQuery = @"
                    UPDATE tblMealType
                    SET IsActive = 0
                    WHERE MealTypeID = @MealTypeID AND InstituteID = @InstituteID AND IsActive = 1";

                var affectedRows = await db.ExecuteAsync(sqlQuery, new { request.MealTypeID, request.InstituteID });

                if (affectedRows > 0)
                {
                    return new ServiceResponse<string>(true, "Meal Type deleted successfully", "Success", 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Meal Type not found or already inactive", null, 404);
                }
            }
        }

        public async Task<ServiceResponse<IEnumerable<GetMealPlannerResponse>>> GetMealPlanner(GetMealPlannerRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();

                // Fetch meal planner data along with menu items
                string sqlQuery = @"
 SELECT
    md.DayID,
    md.DayType,
    mp.MealTypeID,
    mt.MealType,
    mt.StartTime AS MealTime,
    hm.MealItem
FROM tblMealDayMaster md
LEFT OUTER JOIN tblMealType mt ON CHARINDEX(CAST(md.DayID AS VARCHAR), mt.DayIDs) > 0 
LEFT OUTER JOIN tblMealPlanner mp ON mp.MealTypeID = mt.MealTypeID AND mp.DayID = md.DayID AND mt.InstituteID = @InstituteID AND mt.IsActive = 1
LEFT OUTER JOIN tblHostelMenu hm ON mp.MealPlannerID = hm.MealPlannerID
ORDER BY md.DayID, mp.MealTypeID";

                var mealPlannerData = await db.QueryAsync<dynamic>(sqlQuery, new { InstituteID = request.InstituteID });

                // Group data by DayID, DayType, MealTypeID and MealType
                var groupedData = mealPlannerData
                    .GroupBy(x => new { x.DayID, x.DayType })
                    .Select(g => new GetMealPlannerResponse
                    {
                        DayID = g.Key.DayID,
                        DayType = g.Key.DayType,
                        MealPlanner = g
                            .GroupBy(x => new { x.MealTypeID, x.MealType, x.MealTime })
                            .Select(mpGroup => new MealPlanner
                            {
                                MealTypeID = mpGroup.Key.MealTypeID ?? 0, // Handle null MealTypeID
                                MealType = mpGroup.Key.MealType,
                                MealTime = mpGroup.Key.MealTime?.ToString(), // Handle nullable MealTime
                                MealMenu = mpGroup
                                    .Select(x => new MenuItems { MenuItem = x.MealItem })
                                    .Where(menu => menu.MenuItem != null) // Exclude null menu items
                                    .Distinct() // Ensure no duplicate menu items
                                    .ToList()
                            }).ToList()
                    });

                // Filter out days with no menu items
                var response = groupedData.Select(day => new GetMealPlannerResponse
                {
                    DayID = day.DayID,
                    DayType = day.DayType,
                    MealPlanner = day.MealPlanner
                        .Where(meal => meal.MealMenu.Any()) // Only include meals that have menu items
                        .ToList()
                }).ToList();

                return new ServiceResponse<IEnumerable<GetMealPlannerResponse>>(true, "Meal Planner Retrieved Successfully", response, 200);
            }
        }





        //public async Task<ServiceResponse<IEnumerable<GetMealPlannerResponse>>> GetMealPlanner(GetMealPlannerRequest request)
        //{
        //    using (IDbConnection db = new SqlConnection(_connectionString))
        //    {
        //        db.Open();

        //        // Fetch meal planner data along with menu items
        //        string sqlQuery = @"
        //    SELECT
        //        md.DayID,
        //        md.DayType,
        //        mp.MealTypeID,
        //        mt.MealType,
        //        mt.StartTime AS MealTime,
        //        hm.MealItem
        //    FROM tblMealDayMaster md
        //    LEFT OUTER JOIN tblMealType mt ON CHARINDEX(CAST(md.DayID AS VARCHAR), mt.DayIDs) > 0
        //    LEFT OUTER JOIN tblMealPlanner mp ON mp.MealTypeID = mt.MealTypeID AND mt.InstituteID = @InstituteID AND mt.IsActive = 1
        //    LEFT OUTER JOIN tblHostelMenu hm ON mp.MealPlannerID = hm.MealPlannerID
        //    ORDER BY md.DayID, mp.MealTypeID";

        //        var mealPlannerData = await db.QueryAsync<dynamic>(sqlQuery, new { InstituteID = request.InstituteID });

        //        // Group data by DayID and MealTypeID, and ensure no duplicates
        //        var groupedData = mealPlannerData
        //            .GroupBy(x => new { x.DayID, x.DayType, x.MealTypeID, x.MealType, x.MealTime })
        //            .Select(g => new GetMealPlannerResponse
        //            {
        //                DayID = g.Key.DayID,
        //                DayType = g.Key.DayType,
        //                MealPlanner = new List<MealPlanner>
        //                {
        //            new MealPlanner
        //            {
        //                MealTypeID = g.Key.MealTypeID ?? 0,  // Handle null MealTypeID by defaulting to 0
        //                MealType = g.Key.MealType,
        //                MealTime = g.Key.MealTime?.ToString(), // Handle nullable MealTime
        //                MealMenu = g.Select(x => new MenuItems { MenuItem = x.MealItem }).ToList()
        //            }
        //                }
        //            });

        //        return new ServiceResponse<IEnumerable<GetMealPlannerResponse>>(true, "Meal Planner Retrieved Successfully", groupedData, 200);
        //    }
        //}


        //public async Task<ServiceResponse<IEnumerable<GetMealPlannerResponse>>> GetMealPlanner(GetMealPlannerRequest request)
        //{
        //    using (IDbConnection db = new SqlConnection(_connectionString))
        //    {
        //        db.Open();

        //        // Fetch meal planner data along with menu items
        //        string sqlQuery = @"
        //    SELECT
        //        md.DayID,
        //        md.DayType,
        //        mp.MealTypeID,
        //        mt.MealType,
        //        mt.StartTime AS MealTime,
        //        hm.MealItem
        //    FROM tblMealDayMaster md
        //    LEFT OUTER JOIN tblMealType mt ON CHARINDEX(CAST(md.DayID AS VARCHAR), mt.DayIDs) > 0
        //    LEFT OUTER JOIN tblMealPlanner mp ON mp.MealTypeID = mt.MealTypeID AND mt.InstituteID = 6 AND mt.IsActive = 1
        //    LEFT OUTER JOIN tblHostelMenu hm ON mp.MealPlannerID = hm.MealPlannerID
        //    ORDER BY md.DayID, mp.MealTypeID";

        //        var mealPlannerData = await db.QueryAsync<dynamic>(sqlQuery, new { InstituteID = request.InstituteID });

        //        // Group data by DayID
        //        var groupedData = mealPlannerData
        //            .GroupBy(x => new { x.DayID, x.DayType })
        //            .Select(g => new GetMealPlannerResponse
        //            {
        //                DayID = g.Key.DayID,
        //                DayType = g.Key.DayType,
        //                MealPlanner = g.Select(mp => new MealPlanner
        //                {
        //                    MealTypeID = mp.MealTypeID ?? 0,  // Use null-coalescing operator for nullable MealTypeID
        //                    MealType = mp.MealType,
        //                    MealTime = mp.MealTime?.ToString(),  // Handle nullable MealTime
        //                    MealMenu = g.Where(x => x.MealTypeID == mp.MealTypeID) // Filter meal items for the same MealTypeID
        //                                .Select(x => new MenuItems { MenuItem = x.MealItem }) // Map meal items
        //                                .ToList()
        //                }).ToList()
        //            });

        //        return new ServiceResponse<IEnumerable<GetMealPlannerResponse>>(true, "Meal Planner Retrieved Successfully", groupedData, 200);
        //    }
        //}

        public async Task<ServiceResponse<string>> SetMealPlanner(SetMealPlannerRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();

                // Insert meal planner items into the tblMealPlanner and tblHostelMenu
                var mealPlannerQuery = @"
                    INSERT INTO tblMealPlanner (MealTypeID, DayID)
                    VALUES (@MealTypeID, @DayID);
                    SELECT CAST(SCOPE_IDENTITY() AS INT)";

                var mealPlannerID = await db.ExecuteScalarAsync<int>(mealPlannerQuery, new
                {
                    request.MealTypeID,
                    request.DayID
                });

                // Insert meal items into the tblHostelMenu
                foreach (var item in request.Items)
                {
                    var menuItemQuery = @"
                        INSERT INTO tblHostelMenu (MealPlannerID, MealItem)
                        VALUES (@MealPlannerID, @MealItem)";

                    await db.ExecuteAsync(menuItemQuery, new
                    {
                        MealPlannerID = mealPlannerID,
                        item.MealItem
                    });
                }

                return new ServiceResponse<string>(true, "Meal Planner Set Successfully", "Success", 200);
            }
        }

        //public async Task<GetDailyMealMenuResponse> GetDailyMealMenu(GetDailyMealMenuRequest request)
        //{
        //    using (IDbConnection db = new SqlConnection(_connectionString))
        //    {
        //        // Parse the date string to DateTime using the exact format 'DD-MM-YYYY'
        //        DateTime parsedDate;
        //        if (!DateTime.TryParseExact(request.Date, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out parsedDate))
        //        {
        //            throw new ArgumentException("Invalid Date Format. Expected format is DD-MM-YYYY.");
        //        }

        //        // Get the DayID based on the DayType
        //        var dayTypeQuery = @"
        //    SELECT DayID 
        //    FROM tblMealDayMaster 
        //    WHERE DayType = (SELECT UPPER(SUBSTRING(DATENAME(WEEKDAY, @Date), 1, 3)))";

        //        var dayID = await db.ExecuteScalarAsync<int>(dayTypeQuery, new { Date = parsedDate });

        //        // Updated SQL Query with formatted StartTime and EndTime
        //        var mealQuery = @"
        //    SELECT mt.MealType, 
        //           FORMAT(mt.StartTime, 'hh:mm tt') AS StartTime, 
        //           FORMAT(mt.EndTime, 'hh:mm tt') AS EndTime, 
        //           hm.MealItem
        //    FROM tblMealType mt
        //    INNER JOIN tblMealPlanner mp ON mt.MealTypeID = mp.MealTypeID
        //    INNER JOIN tblMealDayMaster md ON mp.DayID = md.DayID
        //    INNER JOIN tblHostelMenu hm ON mp.MealPlannerID = hm.MealPlannerID
        //    WHERE CHARINDEX(CAST(@DayID AS VARCHAR), mt.DayIDs) > 0 
        //    AND mt.InstituteID = @InstituteID";

        //        // Execute the query and map the result to MealItem class
        //        var mealItems = await db.QueryAsync<MealItem>(mealQuery, new { DayID = dayID, InstituteID = request.InstituteID });

        //        // Build the response
        //        return new GetDailyMealMenuResponse
        //        {
        //            MealType = mealItems.FirstOrDefault()?.MealItemName,  // Adjust as per your requirements
        //            MealTime = $"{mealItems.FirstOrDefault()?.StartTime} to {mealItems.FirstOrDefault()?.EndTime}",
        //            MealItems = mealItems.ToList()
        //        };
        //    }
        //}



        //public async Task<GetDailyMealMenuResponse> GetDailyMealMenu(GetDailyMealMenuRequest request)
        //{
        //    using (IDbConnection db = new SqlConnection(_connectionString))
        //    {
        //        // Parse the date string to DateTime using the exact format 'DD-MM-YYYY'
        //        DateTime parsedDate;
        //        if (!DateTime.TryParseExact(request.Date, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out parsedDate))
        //        {
        //            throw new ArgumentException("Invalid Date Format. Expected format is DD-MM-YYYY.");
        //        }

        //        // Get the DayID based on the DayType
        //        var dayTypeQuery = @"
        //        SELECT DayID 
        //        FROM tblMealDayMaster 
        //        WHERE DayType = (SELECT UPPER(SUBSTRING(DATENAME(WEEKDAY, @Date), 1, 3)))";

        //            var dayID = await db.ExecuteScalarAsync<int>(dayTypeQuery, new { Date = parsedDate });

        //            // Updated SQL Query with formatted StartTime and EndTime
        //            var mealQuery = @"
        //        SELECT mt.MealType, 
        //               FORMAT(mt.StartTime, 'hh:mm tt') AS StartTime, 
        //               FORMAT(mt.EndTime, 'hh:mm tt') AS EndTime, 
        //               hm.MealItem
        //        FROM tblMealType mt
        //        INNER JOIN tblMealPlanner mp ON mt.MealTypeID = mp.MealTypeID
        //        INNER JOIN tblMealDayMaster md ON mp.DayID = md.DayID
        //        INNER JOIN tblHostelMenu hm ON mp.MealPlannerID = hm.MealPlannerID
        //        WHERE CHARINDEX(CAST(@DayID AS VARCHAR), mt.DayIDs) > 0 
        //        AND mt.InstituteID = @InstituteID";

        //        // Execute the query and map the result to MealItem class
        //        var mealItems = await db.QueryAsync<dynamic>(mealQuery, new { DayID = dayID, InstituteID = request.InstituteID });

        //        // Map dynamic result to MealItem list
        //        var mappedMealItems = mealItems.Select(item => new MealItem
        //        {
        //            MealItemName = item.MealItem
        //        }).ToList();

        //        // Build the response
        //        var firstMealItem = mappedMealItems.FirstOrDefault();
        //        return new GetDailyMealMenuResponse
        //        {
        //            MealType = mealItems.FirstOrDefault()?.MealType,  // Now MealType is correctly included
        //            MealTime = $"{mealItems.FirstOrDefault()?.StartTime} to {mealItems.FirstOrDefault()?.EndTime}",
        //            StartTime = mealItems.FirstOrDefault()?.StartTime,
        //            EndTime = mealItems.FirstOrDefault()?.EndTime,
        //            MealItems = mappedMealItems
        //        };
        //    }
        //}

        public async Task<GetDailyMealMenuResponse> GetDailyMealMenu(GetDailyMealMenuRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                // Parse the date string to DateTime using the exact format 'DD-MM-YYYY'
                DateTime parsedDate;
                if (!DateTime.TryParseExact(request.Date, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out parsedDate))
                {
                    throw new ArgumentException("Invalid Date Format. Expected format is DD-MM-YYYY.");
                }

                // Get the DayID based on the DayType
                var dayTypeQuery = @"
            SELECT DayID 
            FROM tblMealDayMaster 
            WHERE DayType = (SELECT UPPER(SUBSTRING(DATENAME(WEEKDAY, @Date), 1, 3)))";

                var dayID = await db.ExecuteScalarAsync<int>(dayTypeQuery, new { Date = parsedDate });

                // Updated SQL Query with StartTime and EndTime as TimeSpan
                var mealQuery = @"
            SELECT mt.MealType, 
                   mt.StartTime, 
                   mt.EndTime, 
                   hm.MealItem
            FROM tblHostelMenu hm
            INNER JOIN tblMealPlanner mp ON hm.MealPlannerID = mp.MealPlannerID
            INNER JOIN tblMealType mt ON mp.MealTypeID = mt.MealTypeID
            INNER JOIN tblMealDayMaster md ON mp.DayID = md.DayID
            WHERE md.DayID = @DayID 
            AND mt.MealTypeID = @MealTypeID
            AND mt.InstituteID = @InstituteID";

                // Execute the query and map the result to MealItem class
                var mealItems = await db.QueryAsync<dynamic>(mealQuery, new { DayID = dayID, MealTypeID = request.MealTypeID, InstituteID = request.InstituteID });

                // Map dynamic result to MealItem list
                var mappedMealItems = mealItems.Select(item => new MealItem
                {
                    MealItemName = item.MealItem
                }).ToList();

                // Build the response
                return new GetDailyMealMenuResponse
                {
                    MealType = mealItems.FirstOrDefault()?.MealType,  // Now MealType is correctly included
                    MealTime = $"{ConvertTimeToString(mealItems.FirstOrDefault()?.StartTime)} to {ConvertTimeToString(mealItems.FirstOrDefault()?.EndTime)}",
                    StartTime = ConvertTimeToString(mealItems.FirstOrDefault()?.StartTime),
                    EndTime = ConvertTimeToString(mealItems.FirstOrDefault()?.EndTime),
                    MealItems = mappedMealItems
                };
            }
        }
         
        private string ConvertTimeToString(object time)
        {
            if (time == null) return string.Empty;

            // If it's a TimeSpan, format it as "hh:mm"
            if (time is TimeSpan timeSpan)
            {
                // If TimeSpan is less than 24 hours, use standard TimeSpan formatting
                if (timeSpan.TotalHours < 24)
                {
                    return timeSpan.ToString(@"hh\:mm");
                }
                // If it's more than 24 hours, use a custom format or handle it based on your requirements
                else
                {
                    return timeSpan.ToString(@"d\.hh\:mm");  // e.g., days.hours:minutes format
                }
            }

            // If it's a DateTime, format it as "hh:mm tt"
            if (time is DateTime dateTime)
            {
                return dateTime.ToString("hh:mm tt");
            }

            // If it's already a string, return it as is
            if (time is string timeString)
            {
                return timeString;
            }

            // Return empty if it's an unsupported type
            return string.Empty;
        }




    }
}
