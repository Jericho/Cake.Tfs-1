using System;
using System.Collections.Generic;

namespace Cake.Tfs.AutoMerge
{
  /// <summary>
  /// Auto Merge TFS settings
  /// </summary>
  public class AutoMergeSettings
  {
    /// <summary>
    /// TFS Collection url.
    /// </summary>
    public Uri CollectionUri { get; set; }

    /// <summary>
    /// TFS Project name.
    /// </summary>
    public string ProjectName { get; set; }

    /// <summary>
    /// TFS Repository name.
    /// </summary>
    public string RepositoryName { get; set; }

    /// <summary>
    /// Pull Request source branch.
    /// </summary>
    public string SourceBranch { get; set; }

    /// <summary>
    /// Pull Request target branch.
    /// </summary>
    public string TargetBranch { get; set; }

    /// <summary>
    /// Pull Request title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Pull Request description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Pull Request delete source branch after merge.
    /// </summary>
    public bool DeleteSourceBranch { get; set; }

    /// <summary>
    /// Pull Request uses Squash merge strategy.
    /// </summary>
    public bool SquashMerge { get; set; }
    
    /// <summary>
    /// Pull Request auto complete after creation.
    /// </summary>
    public bool AutoComplete { get; set; }

    /// <summary>
    /// Pull Request auto approve after creation.
    /// </summary>
    public bool AutoApprove { get; set; }

    /// <summary>
    /// Pull Request override existing policies.
    /// </summary>
    public bool OverridePolicies { get; set; }


    /// <summary>
    /// Pull Request reviewers.
    /// </summary>
    public ICollection<AutoMergeReviewer> Reviewers { get; set; }

    /// <summary>
    /// Creates default settings instance
    /// </summary>
    public AutoMergeSettings()
    {
      AutoComplete = true;
      AutoApprove = true;

      Reviewers = new List<AutoMergeReviewer>();
    }
  }
}