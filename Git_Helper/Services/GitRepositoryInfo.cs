namespace Git_Helper.Services;

public class GitRepositoryInfo
{
    public bool IsGitRepository { get; init; }
    public string CurrentBranch { get; init; } = "-";
    public string RemoteUrl { get; init; } = "-";
}
