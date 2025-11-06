using Microsoft.EntityFrameworkCore;
using Simple_ATM.DomainLayer.Enums;
using System.ComponentModel.DataAnnotations;

namespace Simple_ATM.DomainLayer.Entities
{
    [Index(nameof(CardNumber), IsUnique = true)]
    public class User
    {
        public int UserId { get; set; }
        public bool IsBlocked { get; set; } = false;
        [MaxLength(16)]
        public required string CardNumber { get; set; }
        [MaxLength(4)]
        public required string CardPin { get; set; }
        public decimal CardAmount =>
             Operations?.Sum(o => o.OperationType == OperationType.Deposit ? o.Amount : -o.Amount) ?? 0m; // If no operations was done, set it to 0
        public List<Operation> Operations { get; set; } = new();
        public int FailedAttempts { get; set; } = 0;
    }
}
