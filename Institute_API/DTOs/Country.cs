namespace Institute_API.DTOs
{
    public class Country
    {
        public int Country_id { get; set; }
        public string Country_name { get; set; } = string.Empty;
    }

    public class State
    {
        public int State_id { get; set; }
        public int Country_id { get; set; }
        public string State_name { get; set; } = string.Empty;
    }

    public class City
    {
        public int City_id { get; set; }
        public int District_id { get; set; }
        public string City_name { get; set; } = string.Empty;
    }

    public class District
    {
        public int District_id { get; set; }
        public int State_id { get; set; }
        public string District_name { get; set; } = string.Empty;
    }

}
