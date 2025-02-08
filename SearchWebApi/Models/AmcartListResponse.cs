namespace SearchWebApi.Models
{
    public class AmcartListResponse<T>: AmcartResponseState
    {
        public AmcartListContent<T> ? Content { get; set; }
    }

    public class AmcartListContent<T> 
    {
        public int Total { get; set; }

        public  List<T> Records { get; set; } = [];
    }
}
