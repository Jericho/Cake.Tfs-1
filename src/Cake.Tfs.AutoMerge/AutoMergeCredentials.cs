using System;
using Cake.Core.Diagnostics;
using Cake.Tfs.AutoMerge.Authentication;

namespace Cake.Tfs.AutoMerge
{
    /// <summary>
    /// Configure Auto Merge connection credentials
    /// </summary>
    public class AutoMergeCredentials
    {
        private readonly ICakeLog _log;

        internal AutoMergeCredentials(ICakeLog log)
        {
            _log = log;
        }

        /// <summary>
        /// Configured credentials
        /// </summary>
        public ITfsCredentials Credentials { get; private set; }

        /// <summary>
        /// Returns credentials for integrated / NTLM authentication.
        /// Can only be used for on-premise Team Foundation Server.
        /// </summary>
        /// <returns>Same object instance.</returns>
        public AutoMergeCredentials AuthenticationNtlm()
        {
            _log?.Information("Using NTML authentication.");
            Credentials = new TfsNtlmCredentials();
            return this;
        }

        /// <summary>
        /// Returns credentials for basic authentication.
        /// Can only be used for on-premise Team Foundation Server configured for basic authentication.
        /// See https://www.visualstudio.com/en-us/docs/integrate/get-started/auth/tfs-basic-auth.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        /// <returns>Same object instance.</returns>
        public AutoMergeCredentials AuthenticationBasic(string userName, string password)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }
            if (String.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            _log?.Information("Using Basic authentication.");
            Credentials = new TfsBasicCredentials(userName, password);
            return this;
        }

        /// <summary>
        /// Returns credentials for authentication with a personal access token.
        /// Can be used for Team Foundation Server and Visual Studio Team Services.
        /// </summary>
        /// <param name="personalAccessToken">Personal access token.</param>
        /// <returns>Same object instance.</returns>
        public AutoMergeCredentials AuthenticationPersonalAccessToken(string personalAccessToken)
        {
            if (String.IsNullOrWhiteSpace(personalAccessToken))
            {
                throw new ArgumentNullException(nameof(personalAccessToken));
            }

            _log?.Information("Using Personal Access Token authentication.");
            Credentials = new TfsBasicCredentials(String.Empty, personalAccessToken);
            return this;
        }

        /// <summary>
        /// Returns credentials for OAuth authentication.
        /// Can only be used with Visual Studio Team Services.
        /// </summary>
        /// <param name="accessToken">OAuth access token.</param>
        /// <returns>Same object instance.</returns>
        public AutoMergeCredentials AuthenticationOAuth(string accessToken)
        {
            if (String.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            _log?.Information("Using OAuth authentication.");
            Credentials = new TfsOAuthCredentials(accessToken);
            return this;
        }
    }
}