using Dapper;
using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using Infirmary_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Infirmary_API.Repository.Implementations
{
    public class InfirmaryVisitRepository : IInfirmaryVisitRepository
    {
        private readonly IDbConnection _connection;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public InfirmaryVisitRepository(IDbConnection connection, IWebHostEnvironment hostingEnvironment)
        {
            _connection = connection;
            _hostingEnvironment = hostingEnvironment;
        }


        //public async Task<ServiceResponse<string>> AddUpdateInfirmaryVisit(AddUpdateInfirmaryVisitRequest request)
        //{
        //    try
        //    {
        //        _connection.Open();
        //        using (var transaction = _connection.BeginTransaction())
        //        {
        //            try
        //            {
        //                // Convert EntryDate and ExitDate from string to DateTime
        //                DateTime entryDate = DateTime.ParseExact(request.EntryDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        //                DateTime? exitDate = string.IsNullOrEmpty(request.ExitDate) ? (DateTime?)null : DateTime.ParseExact(request.ExitDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

        //                // Convert EntryTime and ExitTime from string to TimeSpan
        //                TimeSpan entryTime = DateTime.ParseExact(request.EntryTime, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
        //                TimeSpan? exitTime = string.IsNullOrEmpty(request.ExitTime) ? (TimeSpan?)null : DateTime.ParseExact(request.ExitTime, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;

        //                string query = request.VisitID == 0
        //                    ? @"INSERT INTO tblInfirmaryVisit (VisitorTypeID, VisitorID, EntryDate, EntryTime, ExitDate, ExitTime, ReasonToVisitInfirmary, Diagnosis, Treatment, Prescription, DiagnosisBy, PrescriptionFile, InstituteID, IsActive) 
        //               VALUES (@VisitorTypeID, @VisitorID, @EntryDate, @EntryTime, @ExitDate, @ExitTime, @ReasonToVisitInfirmary, @Diagnosis, @Treatment, @Prescription, @DiagnosisBy, @PrescriptionFile, @InstituteID, @IsActive);
        //               SELECT CAST(SCOPE_IDENTITY() as int);"
        //                    : @"UPDATE tblInfirmaryVisit SET VisitorTypeID = @VisitorTypeID, VisitorID = @VisitorID, EntryDate = @EntryDate, EntryTime = @EntryTime, ExitDate = @ExitDate, ExitTime = @ExitTime, 
        //               ReasonToVisitInfirmary = @ReasonToVisitInfirmary, Diagnosis = @Diagnosis, Treatment = @Treatment, Prescription = @Prescription, DiagnosisBy = @DiagnosisBy, PrescriptionFile = @PrescriptionFile, 
        //               InstituteID = @InstituteID, IsActive = @IsActive 
        //               WHERE VisitID = @VisitID;
        //               SELECT @VisitID;";

        //                var visitID = await _connection.ExecuteScalarAsync<int>(query, new
        //                {
        //                    request.VisitorTypeID,
        //                    request.VisitorID,
        //                    EntryDate = entryDate,
        //                    EntryTime = entryTime,
        //                    ExitDate = exitDate,
        //                    ExitTime = exitTime,
        //                    request.ReasonToVisitInfirmary,
        //                    request.Diagnosis,
        //                    request.Treatment,
        //                    request.Prescription,
        //                    request.DiagnosisBy,
        //                    request.PrescriptionFile,
        //                    request.InstituteID,
        //                    request.IsActive,
        //                    request.VisitID
        //                }, transaction);

        //                // Insert/Update Medicines
        //                if (request.Medicines != null && request.Medicines.Count > 0)
        //                {
        //                    foreach (var medicine in request.Medicines)
        //                    {
        //                        medicine.VisitID = visitID;
        //                        string medicineQuery = medicine.MedicineID == 0
        //                            ? @"INSERT INTO tblMedicine (VisitID, ItemTypeID, PrescribedMedicineID, NoOfDose, Quantity, Remarks, IsActive) 
        //                       VALUES (@VisitID, @ItemTypeID, @PrescribedMedicineID, @NoOfDose, @Quantity, @Remarks, @IsActive)"
        //                            : @"UPDATE tblMedicine SET VisitID = @VisitID, ItemTypeID = @ItemTypeID, PrescribedMedicineID = @PrescribedMedicineID, NoOfDose = @NoOfDose, Quantity = @Quantity, Remarks = @Remarks, IsActive = @IsActive 
        //                       WHERE MedicineID = @MedicineID";

        //                        await _connection.ExecuteAsync(medicineQuery, medicine, transaction);
        //                    }
        //                }

        //                // Insert/Update Documents
        //                var docs = await AddUpdateInfirmaryVisitDocs(request.InfirmaryVisitDocs, visitID);
        //                transaction.Commit();
        //                return new ServiceResponse<string>(true, "Operation Successful", "Infirmary visit updated successfully", 200);
        //            }
        //            catch (Exception ex)
        //            {
        //                transaction.Rollback();
        //                return new ServiceResponse<string>(false, ex.Message, null, 500);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<string>(false, ex.Message, null, 500);
        //    }
        //    finally
        //    {
        //        _connection.Close();
        //    }
        //}

        public async Task<ServiceResponse<string>> AddUpdateInfirmaryVisit(AddUpdateInfirmaryVisitRequest request)
        {
            try
            {
                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        // Convert EntryDate and ExitDate from string to DateTime
                        DateTime entryDate = DateTime.ParseExact(request.EntryDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        DateTime? exitDate = string.IsNullOrEmpty(request.ExitDate) ? (DateTime?)null : DateTime.ParseExact(request.ExitDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                        // Convert EntryTime and ExitTime from string to TimeSpan
                        TimeSpan entryTime = DateTime.ParseExact(request.EntryTime, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
                        TimeSpan? exitTime = string.IsNullOrEmpty(request.ExitTime) ? (TimeSpan?)null : DateTime.ParseExact(request.ExitTime, "hh:mm tt", CultureInfo.InvariantCulture).TimeOfDay;

                        string query = request.VisitID == 0
                            ? @"INSERT INTO tblInfirmaryVisit (VisitorTypeID, VisitorID, EntryDate, EntryTime, ExitDate, ExitTime, ReasonToVisitInfirmary, Diagnosis, Treatment, Prescription, DiagnosisBy, PrescriptionFile, InstituteID, IsActive) 
                        VALUES (@VisitorTypeID, @VisitorID, @EntryDate, @EntryTime, @ExitDate, @ExitTime, @ReasonToVisitInfirmary, @Diagnosis, @Treatment, @Prescription, @DiagnosisBy, @PrescriptionFile, @InstituteID, @IsActive);
                        SELECT CAST(SCOPE_IDENTITY() as int);"
                            : @"UPDATE tblInfirmaryVisit SET VisitorTypeID = @VisitorTypeID, VisitorID = @VisitorID, EntryDate = @EntryDate, EntryTime = @EntryTime, ExitDate = @ExitDate, ExitTime = @ExitTime, 
                        ReasonToVisitInfirmary = @ReasonToVisitInfirmary, Diagnosis = @Diagnosis, Treatment = @Treatment, Prescription = @Prescription, DiagnosisBy = @DiagnosisBy, PrescriptionFile = @PrescriptionFile, 
                        InstituteID = @InstituteID, IsActive = @IsActive 
                        WHERE VisitID = @VisitID;
                        SELECT @VisitID;";

                        var visitID = await _connection.ExecuteScalarAsync<int>(query, new
                        {
                            request.VisitorTypeID,
                            request.VisitorID,
                            EntryDate = entryDate,
                            EntryTime = entryTime,
                            ExitDate = exitDate,
                            ExitTime = exitTime,
                            request.ReasonToVisitInfirmary,
                            request.Diagnosis,
                            request.Treatment,
                            request.Prescription,
                            request.DiagnosisBy,
                            request.PrescriptionFile,
                            request.InstituteID,
                            request.IsActive,
                            request.VisitID
                        }, transaction);

                        // Remove previous medicines before inserting new ones
                        string deleteMedicinesQuery = @"DELETE FROM tblMedicine WHERE VisitID = @VisitID";
                        await _connection.ExecuteAsync(deleteMedicinesQuery, new { VisitID = visitID }, transaction);

                        // Insert new medicines
                        if (request.Medicines != null && request.Medicines.Count > 0)
                        {
                            foreach (var medicine in request.Medicines)
                            {
                                medicine.VisitID = visitID;
                                string medicineInsertQuery = @"INSERT INTO tblMedicine (VisitID, ItemTypeID, PrescribedMedicineID, NoOfDose, Quantity, Remarks, IsActive) 
                                      VALUES (@VisitID, @ItemTypeID, @PrescribedMedicineID, @NoOfDose, @Quantity, @Remarks, @IsActive)";
                                await _connection.ExecuteAsync(medicineInsertQuery, medicine, transaction);
                            }
                        }


                        // Insert/Update Documents
                        var docs = await AddUpdateInfirmaryVisitDocs(request.InfirmaryVisitDocs, visitID);
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
                foreach (var data in groupedResult)
                {
                    data.InfirmaryVisitDocs = await GetInfirmaryVisitDocs(data.VisitID);
                }
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
                groupedResult.InfirmaryVisitDocs = await GetInfirmaryVisitDocs(id);
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
        public async Task<ServiceResponse<int>> DeleteInfirmaryVisitDoc(int documentId)
        {
            try
            {
                // SQL query to delete the document by ID
                string deleteSql = @"
        DELETE FROM tblInfirmaryVisitDocuments 
        WHERE DocumentsId = @DocumentsId";

                // Execute the delete command
                int rowsAffected = await _connection.ExecuteAsync(deleteSql, new { DocumentsId = documentId });

                // Check if any rows were affected
                if (rowsAffected > 0)
                {
                    return new ServiceResponse<int>(true, "Document deleted successfully", rowsAffected, 200);
                }
                else
                {
                    return new ServiceResponse<int>(false, "No document found with the given ID", 0, 404);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
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
        private async Task<ServiceResponse<int>> AddUpdateInfirmaryVisitDocs(List<InfirmaryVisitDocs> request, int visitId)
        {
            if (request == null || !request.Any())
            {
                return new ServiceResponse<int>(false, "No documents provided to update", 0, 400);
            }

            try
            {

                // Step 1: Hard delete existing documents for the given VisitID
                string deleteSql = @"
        DELETE FROM tblInfirmaryVisitDocuments 
        WHERE VisitID = @VisitID";

                await _connection.ExecuteAsync(deleteSql, new { VisitID = visitId });

                // Step 2: Insert new documents
                string insertSql = @"
        INSERT INTO tblInfirmaryVisitDocuments (VisitID, DocFile)
        VALUES (@VisitID, @DocFile)";

                foreach (var doc in request)
                {
                    doc.VisitID = visitId; // Ensure the VisitID is set for each document
                    doc.DocFile = ImageUpload(doc.DocFile);
                    await _connection.ExecuteAsync(insertSql, doc);
                }

                return new ServiceResponse<int>(true, "Documents added/updated successfully", request.Count, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }
        private string ImageUpload(string image)
        {
            if (string.IsNullOrEmpty(image) || image == "string")
            {
                return string.Empty;
            }
            byte[] imageData = Convert.FromBase64String(image);
            string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "InfirmaryVisit");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string fileExtension = IsJpeg(imageData) == true ? ".jpg" : IsPng(imageData) == true ? ".png" : IsGif(imageData) == true ? ".gif" : IsPdf(imageData) == true ? ".pdf" : string.Empty;
            string fileName = Guid.NewGuid().ToString() + fileExtension;
            string filePath = Path.Combine(directoryPath, fileName);
            if (string.IsNullOrEmpty(fileExtension))
            {
                throw new InvalidOperationException("Incorrect file uploaded");
            }
            // Write the byte array to the image file
            File.WriteAllBytes(filePath, imageData);
            return filePath;
        }
        private bool IsJpeg(byte[] bytes)
        {
            // JPEG magic number: 0xFF, 0xD8
            return bytes.Length > 1 && bytes[0] == 0xFF && bytes[1] == 0xD8;
        }
        private bool IsPng(byte[] bytes)
        {
            // PNG magic number: 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A
            return bytes.Length > 7 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47
                && bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A;
        }
        private bool IsGif(byte[] bytes)
        {
            // GIF magic number: "GIF"
            return bytes.Length > 2 && bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46;
        }
        private bool IsPdf(byte[] fileData)
        {
            return fileData.Length > 4 &&
                   fileData[0] == 0x25 && fileData[1] == 0x50 && fileData[2] == 0x44 && fileData[3] == 0x46;
        }
        private string GetImage(string Filename)
        {
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Assets", "InfirmaryVisit", Filename);

            if (!File.Exists(filePath))
            {
                return string.Empty;
            }
            byte[] fileBytes = File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(fileBytes);
            return base64String;
        }
        private async Task<List<InfirmaryVisitDocs>> GetInfirmaryVisitDocs(int visitId)
        {
            try
            {

                // SQL query to select documents for the given VisitID
                string sql = @"
        SELECT DocumentsId, VisitID, DocFile
        FROM tblInfirmaryVisitDocuments
        WHERE VisitID = @VisitID";

                // Execute the query and map the results to the InfirmaryVisitDocs model
                var documents = await _connection.QueryAsync<InfirmaryVisitDocs>(sql, new { VisitID = visitId });
                foreach(var data in documents)
                {
                    data.DocFile = GetImage(data.DocFile);
                }
                return documents.ToList(); // Convert to a List and return
            }
            catch (Exception ex)
            {
                // Handle exceptions (logging can be added here)
                throw new Exception("An error occurred while retrieving infirmary visit documents: " + ex.Message);
            }
        }
    }
}
