namespace FeesManagement_API.DTOs.Responses
{
    public class ClassWiseConcessionResponse
    {
        public string ClassSection { get; set; }
        public int TotalStrength { get; set; }
        public Dictionary<string, Dictionary<string, int>> CategoryConcessions { get; set; } // Dynamic concession counts
    }
}
