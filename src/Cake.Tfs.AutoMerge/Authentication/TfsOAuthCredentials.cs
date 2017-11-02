namespace Cake.Tfs.AutoMerge.Authentication
{
    /// <summary>
    /// Credentials for OAuth authentication.
    /// </summary>
    internal class TfsOAuthCredentials : ITfsCredentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TfsOAuthCredentials"/> class.
        /// </summary>
        /// <param name="accessToken">OAuth access token.</param>
        public TfsOAuthCredentials(string accessToken)
        {
            AccessToken = accessToken;
        }

        /// <summary>
        /// Gets the OAuth access token.
        /// </summary>
        public string AccessToken { get; private set; }
    }
}