using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Tfs.AutoMerge.Services;

namespace Cake.Tfs.AutoMerge
{
  /// <summary>
  /// Cake aliases for TFS auto merge operations.
  /// </summary>
  [CakeAliasCategory("Tfs")]
  public static class AutoMergeAliases
  {


    /// <summary>
    /// If an active Pull Request is opened from sourceBranch to  targetBranch return this instance. Create a new instance in other case.
    /// </summary>
    /// <param name="ctx">Cake context</param>
    /// <param name="settings">Settings to connect to TFS server</param>
    /// <param name="credentialConfigurator">Credentials to connect to TFS server</param>
    /// <returns>The existing Pull Request if exists otherwise the newly created.</returns>
    [CakeMethodAlias]
    public static int CreateAutoMergePullRequest(this ICakeContext ctx, AutoMergeSettings settings, Action<AutoMergeCredentials> credentialConfigurator = null)
    {
      if (settings == null)
      {
        throw new ArgumentNullException(nameof(settings));
      }

      var credentials = new AutoMergeCredentials(ctx.Log);
      if (credentialConfigurator == null)
      {
        credentials.AuthenticationNtlm();
      }
      else
      {
        credentialConfigurator(credentials);
      }

      var repository = new PullRequestRepository(ctx.Log, settings, credentials);
      var pullRequestId = repository.CreatePullRequest();

      if (settings.AutoApprove)
      {
        repository.VotePullRequest(pullRequestId, AutoMergeVote.Approved);
      }

      if (settings.AutoComplete)
      {
        repository.CompletePullRequest(pullRequestId, settings.OverridePolicies);
      }

      foreach (var reviewer in settings.Reviewers)
      {
        repository.AddReviewer(pullRequestId, reviewer);
      }

      return pullRequestId;
    }
  }
}
