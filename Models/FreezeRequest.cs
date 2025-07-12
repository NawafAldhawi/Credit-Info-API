namespace Credit_Info_API.Models
{
    public class FreezeRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public int Reason { get; set; }
    }
}
