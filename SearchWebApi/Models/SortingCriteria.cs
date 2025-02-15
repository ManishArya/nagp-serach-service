namespace SearchWebApi.Models
{
    public class SortingCriteria
    {
        public SortingCriteria() { }

        public bool IsDescending { get; set; } = false;

        public SortKeyOption SortKey { get; set; } = SortKeyOption.Price;
    }
}
