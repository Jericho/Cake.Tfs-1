﻿using System;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.OAuth;

namespace Cake.Tfs.AutoMerge.Authentication
{
    /// <summary>
    /// Extensions for the <see cref="ITfsCredentials"/> interface.
    /// </summary>
    internal static class TfsCredentialsExtensions
    {
        /// <summary>
        /// Returns the <see cref="VssCredentials"/> corresponding to a <see cref="ITfsCredentials"/> object.
        /// </summary>
        /// <param name="credentials"><see cref="ITfsCredentials"/> credential instance.</param>
        /// <returns><see cref="VssCredentials"/> instance.</returns>
        public static VssCredentials ToVssCredentials(this ITfsCredentials credentials)
        {
            if (credentials == null)
            {
                throw new ArgumentNullException(nameof(credentials));
            }

            switch (credentials.GetType().Name)
            {
                case nameof(TfsNtlmCredentials):
                    return new VssCredentials();

                case nameof(TfsBasicCredentials):
                    var basicCredentials = (TfsBasicCredentials)credentials;
                    return new VssBasicCredential(basicCredentials.UserName, basicCredentials.Password);

                case nameof(TfsOAuthCredentials):
                    var oAuthCredentials = (TfsOAuthCredentials)credentials;
                    return new VssOAuthAccessTokenCredential(oAuthCredentials.AccessToken);

                default:
                    throw new InvalidOperationException("Not supported credential type.");
            }
        }
    }
}