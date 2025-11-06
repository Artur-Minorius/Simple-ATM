using Simple_ATM.DomainLayer.Enums;

namespace Simple_ATM.DomainLayer.Entities
{
    public class Operation
    {
        public OperationType OperationType { get; set; } = OperationType.Withdrawal;
        public int OperationId { get; set; }
        public decimal Amount { get; set; }
        public int UserId { get; set; }
        public required User User { get; set; }
        public DateTime OperationTime { get; set; } = DateTime.UtcNow;
    }
}
