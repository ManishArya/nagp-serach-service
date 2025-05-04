namespace SearchWebApi.Models
{
    public class SearchCriteriaRequest
    {
        public  PagingCriteria PagingCriteria {  get; set; } = new();

        public  SortingCriteria SortingCriteria { get; set; } = new();

        public FilterCriteria FilterCriteria { get; set; } = new();

    }
}
