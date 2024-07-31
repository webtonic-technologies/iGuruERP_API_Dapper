using Dapper;
using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using Infirmary_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Infirmary_API.Repository.Implementations
{
    public class InfirmaryVisitRepository : IInfirmaryVisitRepository
    {
        private readonly IDbConnection _connection;

        public InfirmaryVisitRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<string>> AddUpdateInfirmaryVisit(AddUpdateInfirmaryVisitRequest request)
        {
            try
            {
                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        string query = request.VisitID == 0
                            ? @"INSERT INTO tblInfirmaryVisit (VisitorTypeID, VisitorID, EntryDate, EntryTime, ExitDate, ExitTime, ReasonToVisitInfirmary, Diagnosis, Treatment, Prescription, DiagnosisBy, PrescriptionFile, InstituteID, IsActive) 
                               VALUES (@VisitorTypeID, @VisitorID, @EntryDate, @EntryTime, @ExitDate, @ExitTime, @ReasonToVisitInfirmary, @Diagnosis, @Treatment, @Prescription, @DiagnosisBy, @PrescriptionFile, @InstituteID, @IsActive);
                               SELECT CAST(SCOPE_IDENTITY() as int);"
                            : @"UPDATE tblInfirmaryVisit SET VisitorTypeID = @VisitorTypeID, VisitorID = @VisitorID, EntryDate = @EntryDate, EntryTime = @EntryTime, ExitDate = @ExitDate, ExitTime = @ExitTime, 
                               ReasonToVisitInfirmary = @ReasonToVisitInfirmary, Diagnosis = @Diagnosis, Treatment = @Treatment, Prescription = @Prescription, DiagnosisBy = @DiagnosisBy, PrescriptionFile = @PrescriptionFile, 
                               InstituteID = @InstituteID, IsActive = @IsActive 
                               WHERE VisitID = @VisitID;
                               SELECT @VisitID;";

                        var visitID = await _connection.ExecuteScalarAsync<int>(query, request, transaction);

                        if (request.Medicines != null && request.Medicines.Count > 0)
                        {
                            foreach (var medicine in request.Medicines)
                            {
                                medicine.VisitID = visitID;
                                string medicineQuery = medicine.MedicineID == 0
                                    ? @"INSERT INTO tblMedicine (VisitID, ItemTypeID, PrescribedMedicineID, NoOfDose, Quantity, Remarks, IsActive) 
                                       VALUES (@VisitID, @ItemTypeID, @PrescribedMedicineID, @NoOfDose, @Quantity, @Remarks, @IsActive)"
                                    : @"UPDATE tblMedicine SET VisitID = @VisitID, ItemTypeID = @ItemTypeID, PrescribedMedicineID = @PrescribedMedicineID, NoOfDose = @NoOfDose, Quantity = @Quantity, Remarks = @Remarks, IsActive = @IsActive 
                                       WHERE MedicineID = @MedicineID";

                                await _connection.ExecuteAsync(medicineQuery, medicine, transaction);
                            }
                        }

                        transaction.Commit();
                        return new ServiceResponse<string>(true, "Operation Successful", "Infirmary visit updated successfully", 200);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new ServiceResponse<string>(false, ex.Message, null, 500);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, null, 500);
            }
            finally
            {
                _connection.Close();
            }
        }

        public async Task<ServiceResponse<List<InfirmaryVisitResponse>>> GetAllInfirmaryVisits(GetAllInfirmaryVisitsRequest request)
        {
            try
            {
                string countSql = @"SELECT COUNT(*) FROM tblInfirmaryVisit WHERE IsActive = 1 AND InstituteID = @InstituteID";
                int totalCount = await _connection.ExecuteScalarAsync<int>(countSql, new { request.InstituteID });

                string sql = @"
                SELECT 
                    iv.VisitID,
                    iv.VisitorTypeID,
                    iv.VisitorID,
                    iv.EntryDate,
                    iv.EntryTime,
                    iv.ExitDate,
                    iv.ExitTime,
                    iv.ReasonToVisitInfirmary,
                    iv.Diagnosis,
                    iv.Treatment,
                    iv.Prescription,
                    iv.DiagnosisBy,
                    iv.PrescriptionFile,
                    iv.InstituteID,
                    iv.IsActive,
                    m.MedicineID,
                    m.ItemTypeID,
                    m.PrescribedMedicineID,
                    m.NoOfDose,
                    m.Quantity,
                    m.Remarks,
                    m.IsActive as MedicineIsActive
                FROM 
                    tblInfirmaryVisit iv
                LEFT JOIN 
                    tblMedicine m ON iv.VisitID = m.VisitID
                WHERE
                    iv.IsActive = 1 AND iv.InstituteID = @InstituteID
                ORDER BY 
                    iv.VisitID
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var result = await _connection.QueryAsync<InfirmaryVisitResponse, MedicineResponse, InfirmaryVisitResponse>(
                    sql,
                    (visit, medicine) =>
                    {
                        visit.Medicines = visit.Medicines ?? new List<MedicineResponse>();
                        if (medicine != null && medicine.MedicineID != 0)
                        {
                            visit.Medicines.Add(medicine);
                        }
                        return visit;
                    },
                    new
                    {
                        request.InstituteID,
                        Offset = (request.PageNumber - 1) * request.PageSize,
                        PageSize = request.PageSize
                    },
                    splitOn: "MedicineID"
                );

                var groupedResult = result.GroupBy(v => v.VisitID).Select(g =>
                {
                    var groupedVisit = g.First();
                    groupedVisit.Medicines = g.SelectMany(v => v.Medicines).ToList();
                    return groupedVisit;
                }).ToList();

                return new ServiceResponse<List<InfirmaryVisitResponse>>(true, "Records found", groupedResult, 200, totalCount);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<InfirmaryVisitResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<InfirmaryVisit>> GetInfirmaryVisitById(int id)
        {
            try
            {
                string query = @"
        SELECT 
            iv.VisitID,
            iv.VisitorTypeID,
            iv.VisitorID,
            iv.EntryDate,
            iv.EntryTime,
            iv.ExitDate,
            iv.ExitTime,
            iv.ReasonToVisitInfirmary,
            iv.Diagnosis,
            iv.Treatment,
            iv.Prescription,
            iv.DiagnosisBy,
            iv.PrescriptionFile,
            iv.InstituteID,
            iv.IsActive,
            m.MedicineID,
            m.ItemTypeID,
            m.PrescribedMedicineID,
            m.NoOfDose,
            m.Quantity,
            m.Remarks,
            m.IsActive as MedicineIsActive
        FROM 
            tblInfirmaryVisit iv
        LEFT JOIN 
            tblMedicine m ON iv.VisitID = m.VisitID
        WHERE
            iv.VisitID = @Id AND iv.IsActive = 1";

                var result = await _connection.QueryAsync<InfirmaryVisit, Medicine, InfirmaryVisit>(
                    query,
                    (visit, medicine) =>
                    {
                        visit.Medicines = visit.Medicines ?? new List<Medicine>();
                        if (medicine != null && medicine.MedicineID != 0)
                        {
                            visit.Medicines.Add(medicine);
                        }
                        return visit;
                    },
                    new { Id = id },
                    splitOn: "MedicineID"
                );

                var groupedResult = result.GroupBy(v => v.VisitID).Select(g =>
                {
                    var groupedVisit = g.First();
                    groupedVisit.Medicines = g.SelectMany(v => v.Medicines).ToList();
                    return groupedVisit;
                }).FirstOrDefault();

                if (groupedResult != null)
                    return new ServiceResponse<InfirmaryVisit>(true, "Record found", groupedResult, 200);
                else
                    return new ServiceResponse<InfirmaryVisit>(false, "Record not found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<InfirmaryVisit>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> DeleteInfirmaryVisit(int id)
        {
            try
            {
                string query = "UPDATE tblInfirmaryVisit SET IsActive = 0 WHERE VisitID = @Id";
                int result = await _connection.ExecuteAsync(query, new { Id = id });

                if (result > 0)
                    return new ServiceResponse<bool>(true, "Infirmary visit deleted successfully", true, 200);
                else
                    return new ServiceResponse<bool>(false, "Operation failed", false, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
