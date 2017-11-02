namespace Cake.Tfs.AutoMerge
{
  /// <summary>
  /// Represents Auto Merge Reviewer
  /// </summary>
  public class AutoMergeReviewer
  {
    /// <summary>
    /// Pull Request reviewer Id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Pull Request vote
    /// </summary>
    public AutoMergeVote Vote { get; set; }
  }
}