namespace Infrastructure.Cache
{
    public class Park4YouCacheKeys
    {
        /// <summary>
        /// Solvision token expiry time recieved from login and will be used for request purposes.
        /// </summary>
        public static string SolvisionTokenExpiryTime { get { return "SolvisionTokenExpiryTime"; } }

        /// <summary>
        /// Solvision hash recieved from login and will be used for request purposes.
        /// </summary>
        public static string SolvisionTokenHash { get { return "SolvisionTokenHash"; } }
    }
}