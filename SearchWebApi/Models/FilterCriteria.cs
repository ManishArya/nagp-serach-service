namespace SearchWebApi.Models
{
    public class FilterCriteria
    {
        public FilterCriteria() { }

        public string? Category { get; set; } = string.Empty;
        public string ? SearchText { get; set; } = string.Empty;

        public RangeFilterCriteria? PriceCriteria { get; set; }

        public AnyFilterCriteria? ColorCriteria { get; set; }

        public AnyFilterCriteria? BrandCriteria { get; set; }

    }

    public class RangeFilterCriteria
    {
        public double? MinVal { get; set; }

        public double? MaxVal { get; set; }
    }

    public class AnyFilterCriteria
    {
        public List<string> Values { get; set; } = [];
    }
}
