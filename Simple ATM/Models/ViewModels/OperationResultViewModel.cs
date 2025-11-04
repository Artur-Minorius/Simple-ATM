using Simple_ATM.DomainLayer.Enums;

namespace Simple_ATM.Models.ViewModels
{
    public class OperationResultViewModel
    {
        public required string CardNumber { get; set; }
        public decimal Amount { get; set; }
        public decimal RemainingAmount { get; set; }
        public OperationType OperationType { get; set; }
        public DateTime OperationTime { get; set; } = DateTime.UtcNow;
    }
}
