namespace SearchWebApi.Models
{
    public class AmcartResponseState
    {
        public AmcartRequestStatus Status { get; set; } = AmcartRequestStatus.Success;

        public string ? ErrorMessage { get; set; }
    }
}
