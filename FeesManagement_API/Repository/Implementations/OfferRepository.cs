using Configuration.DTOs.Requests;
using Configuration.DTOs.Responses;
using Configuration.Repository.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.DTOs.Responses;


namespace Configuration.Repository.Implementations
{
    public class OfferRepository : IOfferRepository
    {
        private readonly IDbConnection _connection;

        public OfferRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> AddUpdateOffer(AddUpdateOfferRequest request)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    var offerID = request.OfferID;

                    // Parse dates from 'DD-MM-YYYY' format
                    var openingDate = DateTime.ParseExact(request.OpeningDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    var closingDate = DateTime.ParseExact(request.ClosingDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    if (offerID == 0)
                    {
                        var query = @"INSERT INTO tblOffer (OfferName, AcademicYear, OpeningDate, ClosingDate, StudentTypeID, isAmount, isPercentage, Amount, IsActive, InstituteID) 
                              VALUES (@OfferName, @AcademicYear, @OpeningDate, @ClosingDate, @StudentTypeID, @isAmount, @isPercentage, @Amount, @IsActive, @InstituteID);
                              SELECT CAST(SCOPE_IDENTITY() as int);";

                        offerID = await _connection.ExecuteScalarAsync<int>(query, new { request.OfferName, request.AcademicYear, OpeningDate = openingDate, ClosingDate = closingDate, request.StudentTypeID, request.isAmount, request.isPercentage, request.Amount, request.IsActive, request.InstituteID }, transaction);
                    }
                    else
                    {
                        var query = @"UPDATE tblOffer 
                              SET OfferName = @OfferName, 
                                  AcademicYear = @AcademicYear, 
                                  OpeningDate = @OpeningDate, 
                                  ClosingDate = @ClosingDate,
                                  StudentTypeID = @StudentTypeID,
                                  isAmount = @isAmount,
                                  isPercentage = @isPercentage,
                                  Amount = @Amount,
                                  IsActive = @IsActive,
                                  InstituteID = @InstituteID
                              WHERE OfferID = @OfferID";

                        await _connection.ExecuteAsync(query, new { request.OfferName, request.AcademicYear, OpeningDate = openingDate, ClosingDate = closingDate, request.StudentTypeID, request.isAmount, request.isPercentage, request.Amount, request.IsActive, request.InstituteID, request.OfferID }, transaction);
                    }

                    // Handle Fee Head Mapping
                    var deleteFeeHeadMappingQuery = @"DELETE FROM tblOfferFeeHeadMapping WHERE OfferID = @OfferID";
                    await _connection.ExecuteAsync(deleteFeeHeadMappingQuery, new { OfferID = offerID }, transaction);

                    var insertFeeHeadMappingQuery = @"INSERT INTO tblOfferFeeHeadMapping (OfferID, FeeHeadID)
                                              VALUES (@OfferID, @FeeHeadID)";

                    if (request.OfferFeeHeadMappings != null)
                    {
                        foreach (var feeHeadMapping in request.OfferFeeHeadMappings)
                        {
                            await _connection.ExecuteAsync(insertFeeHeadMappingQuery,
                                new { OfferID = offerID, feeHeadMapping.FeeHeadID },
                                transaction);
                        }
                    }

                    // Handle Fee Tenure Mapping
                    var deleteFeeTenureMappingQuery = @"DELETE FROM tblOfferFeeTenureMapping WHERE OfferID = @OfferID";
                    await _connection.ExecuteAsync(deleteFeeTenureMappingQuery, new { OfferID = offerID }, transaction);

                    var insertFeeTenureMappingQuery = @"INSERT INTO tblOfferFeeTenureMapping (OfferID, FeeTenurityID, STMTenurityID, FeeCollectionID)
                                                VALUES (@OfferID, @FeeTenurityID, @STMTenurityID, @FeeCollectionID)";

                    if (request.OfferFeeTenureMappings != null)
                    {
                        foreach (var feeTenureMapping in request.OfferFeeTenureMappings)
                        {
                            await _connection.ExecuteAsync(insertFeeTenureMappingQuery,
                                new
                                {
                                    OfferID = offerID,
                                    feeTenureMapping.FeeTenurityID,
                                    feeTenureMapping.STMTenurityID,
                                    feeTenureMapping.FeeCollectionID
                                },
                                transaction);
                        }
                    }

                    // Handle Class Section Mapping
                    var deleteClassSectionMappingQuery = @"DELETE FROM tblOfferClassSectionMapping WHERE OfferID = @OfferID";
                    await _connection.ExecuteAsync(deleteClassSectionMappingQuery, new { OfferID = offerID }, transaction);

                    var insertClassSectionMappingQuery = @"INSERT INTO tblOfferClassSectionMapping (OfferID, ClassID, SectionID)
                                                  VALUES (@OfferID, @ClassID, @SectionID)";

                    if (request.OfferClassSectionMappings != null)
                    {
                        foreach (var classSectionMapping in request.OfferClassSectionMappings)
                        {
                            await _connection.ExecuteAsync(insertClassSectionMappingQuery,
                                new { OfferID = offerID, classSectionMapping.ClassID, classSectionMapping.SectionID },
                                transaction);
                        }
                    }

                    transaction.Commit();
                    return offerID;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }


        //public async Task<int> AddUpdateOffer(AddUpdateOfferRequest request)
        //{
        //    if (_connection.State == ConnectionState.Closed)
        //    {
        //        _connection.Open();
        //    }

        //    using (var transaction = _connection.BeginTransaction())
        //    {
        //        try
        //        {
        //            var offerID = request.OfferID;

        //            if (offerID == 0)
        //            {
        //                var query = @"INSERT INTO tblOffer (OfferName, AcademicYear, OpeningDate, ClosingDate, StudentTypeID, isAmount, isPercentage, Amount, IsActive, InstituteID) 
        //                              VALUES (@OfferName, @AcademicYear, @OpeningDate, @ClosingDate, @StudentTypeID, @isAmount, @isPercentage, @Amount, @IsActive, @InstituteID);
        //                              SELECT CAST(SCOPE_IDENTITY() as int);";

        //                offerID = await _connection.ExecuteScalarAsync<int>(query, request, transaction);
        //            }
        //            else
        //            {
        //                var query = @"UPDATE tblOffer 
        //                              SET OfferName = @OfferName, 
        //                                  AcademicYear = @AcademicYear, 
        //                                  OpeningDate = @OpeningDate, 
        //                                  ClosingDate = @ClosingDate,
        //                                  StudentTypeID = @StudentTypeID,
        //                                  isAmount = @isAmount,
        //                                  isPercentage = @isPercentage,
        //                                  Amount = @Amount,
        //                                  IsActive = @IsActive,
        //                                  InstituteID = @InstituteID
        //                              WHERE OfferID = @OfferID";

        //                await _connection.ExecuteAsync(query, request, transaction);
        //            }

        //            // Handle Fee Head Mapping
        //            var deleteFeeHeadMappingQuery = @"DELETE FROM tblOfferFeeHeadMapping WHERE OfferID = @OfferID";
        //            await _connection.ExecuteAsync(deleteFeeHeadMappingQuery, new { OfferID = offerID }, transaction);

        //            var insertFeeHeadMappingQuery = @"INSERT INTO tblOfferFeeHeadMapping (OfferID, FeeHeadID)
        //                                              VALUES (@OfferID, @FeeHeadID)";

        //            if (request.OfferFeeHeadMappings != null)
        //            {
        //                foreach (var feeHeadMapping in request.OfferFeeHeadMappings)
        //                {
        //                    await _connection.ExecuteAsync(insertFeeHeadMappingQuery,
        //                        new { OfferID = offerID, feeHeadMapping.FeeHeadID },
        //                        transaction);
        //                }
        //            }

        //            // Handle Fee Tenure Mapping
        //            var deleteFeeTenureMappingQuery = @"DELETE FROM tblOfferFeeTenureMapping WHERE OfferID = @OfferID";
        //            await _connection.ExecuteAsync(deleteFeeTenureMappingQuery, new { OfferID = offerID }, transaction);

        //            var insertFeeTenureMappingQuery = @"INSERT INTO tblOfferFeeTenureMapping (OfferID, FeeTenurityID)
        //                                                VALUES (@OfferID, @FeeTenurityID)";

        //            if (request.OfferFeeTenureMappings != null)
        //            {
        //                foreach (var feeTenureMapping in request.OfferFeeTenureMappings)
        //                {
        //                    await _connection.ExecuteAsync(insertFeeTenureMappingQuery,
        //                        new { OfferID = offerID, feeTenureMapping.FeeTenurityID },
        //                        transaction);
        //                }
        //            }

        //            // Handle Class Section Mapping
        //            var deleteClassSectionMappingQuery = @"DELETE FROM tblOfferClassSectionMapping WHERE OfferID = @OfferID";
        //            await _connection.ExecuteAsync(deleteClassSectionMappingQuery, new { OfferID = offerID }, transaction);

        //            var insertClassSectionMappingQuery = @"INSERT INTO tblOfferClassSectionMapping (OfferID, ClassID, SectionID)
        //                                                  VALUES (@OfferID, @ClassID, @SectionID)";

        //            if (request.OfferClassSectionMappings != null)
        //            {
        //                foreach (var classSectionMapping in request.OfferClassSectionMappings)
        //                {
        //                    await _connection.ExecuteAsync(insertClassSectionMappingQuery,
        //                        new { OfferID = offerID, classSectionMapping.ClassID, classSectionMapping.SectionID },
        //                        transaction);
        //                }
        //            }

        //            transaction.Commit();
        //            return offerID;
        //        }
        //        catch
        //        {
        //            transaction.Rollback();
        //            throw;
        //        }
        //    }
        //}

        //public async Task<IEnumerable<OfferResponse>> GetAllOffers(GetAllOffersRequest request)
        //{
        //    var query = @"
        //                SELECT 
        //            o.OfferID, 
        //            o.OfferName, 
        //            o.AcademicYear,
        //            CONVERT(VARCHAR, o.OpeningDate, 105) AS OpeningDateFormatted,
        //            CONVERT(VARCHAR, o.ClosingDate, 105) AS ClosingDateFormatted,
        //            o.OpeningDate,
        //            o.ClosingDate,
        //            o.isAmount, 
        //            o.isPercentage, 
        //            o.Amount, 
        //            o.IsActive,
        //            fh.FeeHeadID,
        //            fh.FeeHead,
        //            cr.FeeTenurityID,
        //            cr.STMTenurityID,
        //            cr.FeeCollectionID,
        //            CASE 
        //                WHEN cr.FeeTenurityID = 1 THEN 'Single'
        //                WHEN cr.FeeTenurityID = 2 THEN tt.TermName
        //                WHEN cr.FeeTenurityID = 3 THEN (
        //                    SELECT STRING_AGG(tm.Month, ', ') 
        //                    FROM tblTenurityMonthly tm 
        //                    WHERE tm.FeeCollectionID = cr.FeeCollectionID
        //                )
        //                ELSE ''
        //            END AS FeeTenure,
        //            c.class_name AS ClassName,
        //            s.section_name AS SectionName
        //        FROM tblOffer o
        //        LEFT JOIN tblOfferFeeHeadMapping fhMap ON o.OfferID = fhMap.OfferID
        //        LEFT JOIN tblFeeHead fh ON fhMap.FeeHeadID = fh.FeeHeadID
        //        LEFT JOIN tblOfferFeeTenureMapping cr ON o.OfferID = cr.OfferID
        //        LEFT JOIN tblTenurityTerm tt ON cr.FeeCollectionID = tt.FeeCollectionID AND cr.FeeTenurityID = 2
        //        LEFT JOIN tblTenurityMonthly tm ON cr.FeeCollectionID = tm.FeeCollectionID AND cr.FeeTenurityID = 3
        //        LEFT JOIN tblOfferClassSectionMapping cs ON o.OfferID = cs.OfferID
        //        LEFT JOIN tbl_Class c ON cs.ClassID = c.class_id
        //        LEFT JOIN tbl_Section s ON cs.SectionID = s.section_id
        //        WHERE o.InstituteID = @InstituteID And o.IsActive = 1
        //        ORDER BY o.OfferID
        //        OFFSET @PageSize * (@PageNumber - 1) ROWS
        //        FETCH NEXT @PageSize ROWS ONLY";

        //    var offerLookup = new Dictionary<int, OfferResponse>();

        //    var result = await _connection.QueryAsync<OfferResponse, FeeHeadFeeTenureResponse, ClassSectionResponse, OfferResponse>(
        //        query,
        //        (offer, feeHeadTenure, classSection) =>
        //        {
        //            if (!offerLookup.TryGetValue(offer.OfferID, out var existingOffer))
        //            {
        //                existingOffer = offer;
        //                existingOffer.FeeHeadFeeTenures = new List<FeeHeadFeeTenureResponse>();
        //                existingOffer.ClassSections = new List<ClassSectionResponse>();
        //                offerLookup.Add(offer.OfferID, existingOffer);
        //            }

        //            // Add FeeHeadFeeTenure information
        //            if (feeHeadTenure != null && !existingOffer.FeeHeadFeeTenures.Any(ft => ft.FeeHeadID == feeHeadTenure.FeeHeadID))
        //            {
        //                existingOffer.FeeHeadFeeTenures.Add(feeHeadTenure);
        //            }

        //            // Add ClassSection information
        //            if (classSection != null && !existingOffer.ClassSections.Any(cs => cs.ClassName == classSection.ClassName && cs.SectionName == classSection.SectionName))
        //            {
        //                existingOffer.ClassSections.Add(classSection);
        //            }

        //            return existingOffer;
        //        },
        //        new { request.InstituteID, request.PageNumber, request.PageSize },
        //        splitOn: "FeeHeadID,ClassName");

        //    return offerLookup.Values;
        //}

        public async Task<ServiceResponse<IEnumerable<OfferResponse>>> GetAllOffers(GetAllOffersRequest request)
        {
            // Query to get the offers
            var query = @"
                    SELECT 
                        o.OfferID, 
                        o.OfferName, 
                        o.AcademicYear,
                        CONVERT(VARCHAR, o.OpeningDate, 105) AS OpeningDateFormatted,
                        CONVERT(VARCHAR, o.ClosingDate, 105) AS ClosingDateFormatted,
                        o.OpeningDate,
                        o.ClosingDate,
                        o.isAmount, 
                        o.isPercentage, 
                        o.Amount, 
                        o.IsActive,
                        fh.FeeHeadID,
                        fh.FeeHead,
                        cr.FeeTenurityID,
                        cr.STMTenurityID,
                        cr.FeeCollectionID,
                        CASE 
                            WHEN cr.FeeTenurityID = 1 THEN 'Single'
                            WHEN cr.FeeTenurityID = 2 THEN tt.TermName
                            WHEN cr.FeeTenurityID = 3 THEN (
                                SELECT STRING_AGG(tm.Month, ', ') 
                                FROM tblTenurityMonthly tm 
                                WHERE tm.FeeCollectionID = cr.FeeCollectionID
                            )
                            ELSE ''
                        END AS FeeTenure,
                        c.class_name AS ClassName,
                        s.section_name AS SectionName
                    FROM tblOffer o
                    LEFT JOIN tblOfferFeeHeadMapping fhMap ON o.OfferID = fhMap.OfferID
                    LEFT JOIN tblFeeHead fh ON fhMap.FeeHeadID = fh.FeeHeadID
                    LEFT JOIN tblOfferFeeTenureMapping cr ON o.OfferID = cr.OfferID
                    LEFT JOIN tblTenurityTerm tt ON cr.FeeCollectionID = tt.FeeCollectionID AND cr.FeeTenurityID = 2
                    LEFT JOIN tblTenurityMonthly tm ON cr.FeeCollectionID = tm.FeeCollectionID AND cr.FeeTenurityID = 3
                    LEFT JOIN tblOfferClassSectionMapping cs ON o.OfferID = cs.OfferID
                    LEFT JOIN tbl_Class c ON cs.ClassID = c.class_id
                    LEFT JOIN tbl_Section s ON cs.SectionID = s.section_id
                    WHERE o.InstituteID = @InstituteID And o.IsActive = 1 AND o.AcademicYear = @AcademicYear
                    ORDER BY o.OfferID
                    OFFSET @PageSize * (@PageNumber - 1) ROWS
                    FETCH NEXT @PageSize ROWS ONLY";

            var offerLookup = new Dictionary<int, OfferResponse>();

            var result = await _connection.QueryAsync<OfferResponse, FeeHeadFeeTenureResponse, DTOs.Responses.ClassSectionResponse, OfferResponse>(
                query,
                (offer, feeHeadTenure, classSection) =>
                {
                    if (!offerLookup.TryGetValue(offer.OfferID, out var existingOffer))
                    {
                        existingOffer = offer;
                        existingOffer.FeeHeadFeeTenures = new List<FeeHeadFeeTenureResponse>();
                        existingOffer.ClassSections = new List<DTOs.Responses.ClassSectionResponse>();
                        offerLookup.Add(offer.OfferID, existingOffer);
                    }

                    // Add FeeHeadFeeTenure information
                    if (feeHeadTenure != null && !existingOffer.FeeHeadFeeTenures.Any(ft => ft.FeeHeadID == feeHeadTenure.FeeHeadID))
                    {
                        existingOffer.FeeHeadFeeTenures.Add(feeHeadTenure);
                    }

                    // Add ClassSection information
                    if (classSection != null && !existingOffer.ClassSections.Any(cs => cs.ClassName == classSection.ClassName && cs.SectionName == classSection.SectionName))
                    {
                        existingOffer.ClassSections.Add(classSection);
                    }

                    return existingOffer;
                },
                new
                {
                    request.InstituteID,
                    request.PageNumber,
                    request.PageSize,
                    AcademicYear = request.AcademicYear // Pass the new parameter to the query
                },
                splitOn: "FeeHeadID, ClassName"
            );

            // Query to get the total count of offers
            var countQuery = @"
        SELECT COUNT(*) 
        FROM tblOffer o
        WHERE o.InstituteID = @InstituteID AND o.IsActive = 1 AND o.AcademicYear = @AcademicYear";

            var totalCount = await _connection.ExecuteScalarAsync<int>(countQuery, new
            {
                request.InstituteID,
                AcademicYear = request.AcademicYear
            });

            return new ServiceResponse<IEnumerable<OfferResponse>>(true, "Offers retrieved successfully", offerLookup.Values, 200, totalCount);
        }



        public async Task<OfferResponse> GetOfferById(int offerID)
        {
            var query = @"SELECT OfferID, OfferName, AcademicYear, OpeningDate, ClosingDate, isAmount, isPercentage, Amount, IsActive 
                          FROM tblOffer 
                          WHERE OfferID = @OfferID";

            return await _connection.QueryFirstOrDefaultAsync<OfferResponse>(query, new { OfferID = offerID });
        }

        public async Task<int> DeleteOffer(int offerID)
        {
            var query = @"UPDATE tblOffer 
                          SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END 
                          WHERE OfferID = @OfferID";

            return await _connection.ExecuteAsync(query, new { OfferID = offerID });
        }
        public async Task<IEnumerable<OfferStudentTypeResponse>> GetOfferStudentTypes()
        {
            var query = "SELECT StudentTypeID, StudentType FROM tblOfferStudentype";
            return await _connection.QueryAsync<OfferStudentTypeResponse>(query);
        }
    }
}
