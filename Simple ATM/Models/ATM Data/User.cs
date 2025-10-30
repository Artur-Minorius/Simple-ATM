using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Simple_ATM.Models.ATM_Data
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
        public decimal CardAmount { get; set; }
        public List<Operation> Operations { get; set; } = new();

        public int FailedAttempts { get; set; } = 0;
    }
}
