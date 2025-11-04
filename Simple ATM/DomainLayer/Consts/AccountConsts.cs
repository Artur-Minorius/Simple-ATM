
namespace Simple_ATM.DomainLayer.Consts
{
    public static class AccountConsts
    {
        public static string CardNotFound = "Card not found";
        public static string CardIsBlocked = "Card is blocked";
        public static string CardNowBlocked = "Card blocked after 4 failed attempts.";
        public static string SomethingWentWrong = "Something went wrong, try again later";
        public static string InsufficientFunds = "Insufficient funds to withdraw";
        public static string CardWillBeBlockedAfter(int attempts) => $"Incorrect PIN. {attempts} attempts left.";
    }
}
