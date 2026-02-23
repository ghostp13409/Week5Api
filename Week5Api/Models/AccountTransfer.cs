namespace Week5Api.Models
{
    public class AccountTransfer
    {
        public int FromId { get; set; }
        public int ToId { get; set; }
        public decimal Amount { get; set; }
    }
}
