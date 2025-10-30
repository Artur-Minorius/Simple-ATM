namespace Simple_ATM.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string BackController { get; set; } = String.Empty;
        public string BackAction { get; set; } = String.Empty;
    }
}
