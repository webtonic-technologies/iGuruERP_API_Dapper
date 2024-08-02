namespace Transport_API.DTOs.Requests
{
    public class RouteFeeStructure
    {
        public int RouteFeeStructureId { get; set; }
        public int StopId { get; set; }
        public int FrequencyId { get; set; }
        public List<RouteFeeFrequency>? RouteFeeFrequencies { get; set; }
        public List<RouteTermFeeFrequency>? RouteTermFeeFrequencies { get; set; }
    }
    public class RouteFeeFrequency
    {
        public int FeeFrequencyId { get; set; }
        public int RouteFeeStructureId { get; set; }
        public int FrequencyId { get; set; }
        public decimal Fees { get; set; }
        public int MonthId { get; set; }
        public DateTime DueDate {  get; set; }
    }
    public class RouteTermFeeFrequency
    {
        public int TermFeeFrequencyId { get; set; }
        public int RouteFeeStructureId { get; set; }
        public int FrequencyId { get; set; }
        public decimal Fees { get; set; }
        public DateTime DueDate { get; set; }
        public string TermName { get; set; } = string.Empty;
    }
}
