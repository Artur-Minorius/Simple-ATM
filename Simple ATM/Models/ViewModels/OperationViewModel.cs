using Simple_ATM.Common.Enums;
namespace Simple_ATM.Models.ViewModels
{
    public class OperationViewModel
    {
        public decimal Amount { get; set; }
        public OperationType OperationType { get; set; } = OperationType.Withdrawal;
    }
}
