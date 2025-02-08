namespace SearchWebApi.Models
{
    public class FilterCriteria
    {
        public FilterCriteria() { }

        public string ? SearchText { get; set; } = string.Empty;

        public RangeFilterCriteria? Price { get; set; }

        public BooleanFilterCriteria? Colors { get; set; }

        public BooleanFilterCriteria? Brands { get; set; }

    }

    public class RangeFilterCriteria
    {
        public int MinVal { get; set; }

        public int MaxVal { get; set; }
    }

    public class BooleanFilterCriteria
    {

    }


}
