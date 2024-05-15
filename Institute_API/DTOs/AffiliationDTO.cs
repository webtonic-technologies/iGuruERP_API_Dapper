using Institute_API.Models;

namespace Institute_API.DTOs
{
    public class AffiliationDTO
    {
        public int Affiliation_info_id { get; set; }
        public int Institute_id { get; set; }
        public string AffiliationBoardLogo { get; set; } = string.Empty;
        public string AffiliationBoardName { get; set; } = string.Empty;
        public string AffiliationNumber { get; set; } = string.Empty;
        public string AffiliationCertificateNumber { get; set; } = string.Empty;
        public string InstituteCode { get; set; } = string.Empty;
        public DateTime? en_date {  get; set; }
        public List<Accreditation>? Accreditations { get; set; }
    }
}
