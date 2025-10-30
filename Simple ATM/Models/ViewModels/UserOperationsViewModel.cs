using Simple_ATM.Models.ATM_Data;
namespace Simple_ATM.Models.ViewModels
{
    public class UserOperationsViewModel
    {
        public required int UserId { get; set; }
        public required string CardNumber { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateOnly CurrentDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public List<Operation> Operations { get; set; } = new();
    }
}
