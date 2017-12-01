using System;
using System.Linq;
using Cake.Core.Diagnostics;
using Cake.Tfs.AutoMerge.Authentication;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;

namespace Cake.Tfs.AutoMerge.Services
{
  internal class PullRequestRepository
  {
    private readonly ICakeLog _log;
    private readonly AutoMergeSettings _settings;
    private readonly AutoMergeCredentials _credential;

    internal PullRequestRepository(ICakeLog log, AutoMergeSettings settings, AutoMergeCredentials credential)
    {
      _log = log;
      _settings = settings;
      _credential = credential;

      _log.Verbose(
          "Repository information:\n  CollectionUrl: {0}\n  ProjectName: {1}\n  RepositoryName: {2}",
          settings.CollectionUri,
          settings.ProjectName,
          settings.RepositoryName);
    }

    public int CreatePullRequest()
    {
      using (var client = CreateGitClient(out var authorizedIdenity))
      {
        var criteria = new GitPullRequestSearchCriteria
        {
          Status = PullRequestStatus.Active,
          SourceRefName = _settings.SourceBranch,
          TargetRefName = _settings.TargetBranch
        };

        var query = client.GetPullRequestsAsync(_settings.ProjectName, _settings.RepositoryName, criteria).Result;
        if (query.Count > 0)
        {
          var item = query.First();
          _log.Information("Pull Request already exists: {0}", item.Url);
          return item.PullRequestId;
        }

        var pr = client.CreatePullRequestAsync(
            new GitPullRequest
            {
              SourceRefName = _settings.SourceBranch,
              TargetRefName = _settings.TargetBranch,
              Title = _settings.Title,
              Description = _settings.Description,
              CompletionOptions = new GitPullRequestCompletionOptions
              {
                DeleteSourceBranch = _settings.DeleteSourceBranch,
                SquashMerge = _settings.SquashMerge,
                BypassPolicy = true,
                BypassReason = "Automatic Merge"
              },
              AutoCompleteSetBy = new IdentityRef
              {
                Id = authorizedIdenity.Id.ToString()
              }
            },
            _settings.ProjectName,
            _settings.RepositoryName).Result;

        _log.Information("New Pull Request was created: {0}", pr.Url);
        return pr.PullRequestId;
      }
    }

    public int VotePullRequest(int pullRequestId, AutoMergeVote vote)
    {
      using (var client = CreateGitClient(out var authorizedIdenity))
      {
        var reviewer = new IdentityRefWithVote
        {
          Id = authorizedIdenity.Id.ToString(),
          Vote = (short)vote
        };

        var result = client.CreatePullRequestReviewerAsync(
            reviewer, _settings.ProjectName, _settings.RepositoryName, pullRequestId, authorizedIdenity.Id.ToString()).Result;

        _log.Verbose("Voted for pull request with '{0}'.", result.Vote);
        return result.Vote;
      }
    }

    public string AddReviewer(int pullRequestId, AutoMergeReviewer reviewer)
    {
      using (var client = CreateGitClient(out var authorizedIdenity))
      {
        var pr = client.GetPullRequestByIdAsync(pullRequestId).Result;
        _log.Verbose("Pull Request Details\nId: {0}\nStatus: {1}\nMerge Status: {2}", pr.PullRequestId, pr.Status, pr.MergeStatus);

        if (pr.Reviewers.Any(t => t.Id == reviewer.Id))
        {
          return reviewer.Id;
        }

        _log.Verbose("Attempt to add reviewer to Pull Request");
        var reviewerIdentity = new IdentityRefWithVote(
          new IdentityRef{Id = reviewer.Id })
        { 
          Vote = (short)reviewer.Vote.GetValueOrDefault()
        };

        var result = client.CreatePullRequestReviewerAsync(
          reviewerIdentity, _settings.ProjectName, _settings.RepositoryName, pullRequestId, authorizedIdenity.Id.ToString()).Result;

        return result.Id;
      }
    }

    public int CompletePullRequest(int pullRequestId, bool overridePolicies)
    {
      using (var client = CreateGitClient(out var authorizedIdenity))
      {
        var pr = client.GetPullRequestByIdAsync(pullRequestId).Result;
        _log.Verbose("Pull Request Details\nId: {0}\nStatus: {1}\nMerge Status: {2}", pr.PullRequestId, pr.Status, pr.MergeStatus);

        if (pr.MergeStatus == PullRequestAsyncStatus.Conflicts || pr.MergeStatus == PullRequestAsyncStatus.Failure)
        {
          return pullRequestId;
        }

        _log.Verbose("Attempt to complete Pull Request");
        pr = client.UpdatePullRequestAsync(
          new GitPullRequest
          {
            Status = PullRequestStatus.Completed,
            LastMergeSourceCommit = pr.LastMergeSourceCommit,
            CompletionOptions = new GitPullRequestCompletionOptions
            {
              BypassPolicy = true,
              BypassReason = "Automatic Merge"
            }
          },
          _settings.ProjectName, _settings.RepositoryName, pullRequestId).Result;
        return pr.PullRequestId;
      }
    }
    
    private GitHttpClient CreateGitClient(out Identity authorizedIdenity)
    {
      var connection = new VssConnection(_settings.CollectionUri, _credential.Credentials.ToVssCredentials());

      authorizedIdenity = connection.AuthorizedIdentity;
      _log.Verbose(
          "Authorized Identity:\n  Id: {0}\n  DisplayName: {1}",
          authorizedIdenity.Id,
          authorizedIdenity.DisplayName);

      var gitClient = connection.GetClient<GitHttpClient>();
      if (gitClient == null)
      {
        throw new InvalidOperationException("Could not retrieve the GitHttpClient object");
      }

      return gitClient;
    }
  }
}
