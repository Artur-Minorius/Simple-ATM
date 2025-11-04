namespace Simple_ATM.DomainLayer.Helpers
{
    public class PinVerificationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool CardBlocked { get; set; } = false;
    }
}
