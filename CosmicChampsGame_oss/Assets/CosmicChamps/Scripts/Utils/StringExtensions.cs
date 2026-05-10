namespace CosmicChamps.Utils
{
    public static class StringExtensions
    {
        public static string FormatWalletId (this string walletId) =>
            string.IsNullOrEmpty (walletId)
                ? string.Empty
                : $"{walletId[..4]}...{walletId[^4..]}";

        public static string FormatNickname (this string nickname, int length = 6) =>
            nickname.Length > length ? $"{nickname[..(length / 2)]}...{nickname[^(length / 2)..]}" : nickname;
    }
}