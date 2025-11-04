using Simple_ATM.DomainLayer.Enums;
namespace Simple_ATM.Models.ViewModels
{
    public class OperationViewModel
    {
        public required string Amount { get; set; }
        public OperationType OperationType { get; set; } = OperationType.Withdrawal;
    }
}
