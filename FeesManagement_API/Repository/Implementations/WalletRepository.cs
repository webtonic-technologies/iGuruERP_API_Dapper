using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using Dapper;
using System.Data;

namespace FeesManagement_API.Repository.Implementations
{
    public class WalletRepository : IWalletRepository
    {
        private readonly IDbConnection _connection;

        public WalletRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public string AddWalletAmount(AddWalletAmountRequest request)
        {
            var query = @"
                INSERT INTO tblStudentWallet (StudentID, Amount, PaymentModeID, Comment, InstituteID)
                VALUES (@StudentID, @Amount, @PaymentModeID, @Comment, @InstituteID)";
            _connection.Execute(query, request);
            return "Wallet amount added successfully.";
        }

        public List<GetWalletResponse> GetWallet(GetWalletRequest request)
        {
            var query = @"
                SELECT 
                    sm.student_id AS StudentID,
                    CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName,
                    sm.Admission_Number AS AdmissionNo,
                    c.class_name AS Class,
                    s.section_name AS Section,
                    CONCAT(sp.First_Name, ' ', sp.Last_Name) AS FatherName,
                    sp.Contact_Number AS PhoneNumber,
                    SUM(sw.Amount) AS WalletBalance
                FROM tbl_StudentMaster sm
                LEFT JOIN tbl_Class c ON sm.class_id = c.class_id
                LEFT JOIN tbl_Section s ON sm.section_id = s.section_id
                LEFT JOIN tbl_StudentParentsInfo sp ON sm.student_id = sp.Student_id AND sp.Parent_Type_id = 1
                LEFT JOIN tblStudentWallet sw ON sm.student_id = sw.StudentID
                WHERE sm.isActive = 1 AND sm.class_id = @ClassID AND sm.section_id = @SectionID
                GROUP BY sm.student_id, sm.First_Name, sm.Last_Name, sm.Admission_Number, c.class_name, s.section_name, sp.First_Name, sp.Last_Name, sp.Contact_Number";

            return _connection.Query<GetWalletResponse>(query, request).ToList();
        }
    }
}
