namespace Simple_ATM.DomainLayer.Helpers
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public decimal Amount { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsInsufficientFunds { get; set; } = false;
    }
}
