namespace Git_Helper.Services;

public class GitService
{
    private readonly CommandRunner _commandRunner;

    public GitService(CommandRunner commandRunner)
    {
        _commandRunner = commandRunner;
    }

    public Task<CommandResult> CheckGitInstallationAsync() =>
        _commandRunner.RunAsync("git", "--version", Environment.CurrentDirectory);

    public Task<CommandResult> GetStatusAsync(string repositoryPath) =>
        RunGitCommandAsync("status", repositoryPath);

    public Task<CommandResult> AddAllAsync(string repositoryPath) =>
        RunGitCommandAsync("add .", repositoryPath);

    public Task<CommandResult> CommitAsync(string repositoryPath, string message) =>
        RunGitCommandAsync($"commit -m {Quote(message)}", repositoryPath);

    public Task<CommandResult> PushAsync(string repositoryPath) =>
        RunGitCommandAsync("push", repositoryPath);

    public Task<CommandResult> PushWithUpstreamAsync(string repositoryPath, string branchName) =>
        RunGitCommandAsync($"push -u origin {Quote(branchName)}", repositoryPath);

    public Task<CommandResult> PullAsync(string repositoryPath) =>
        RunGitCommandAsync("pull", repositoryPath);

    public Task<CommandResult> InitRepositoryAsync(string repositoryPath) =>
        RunGitCommandAsync("init", repositoryPath);

    public Task<CommandResult> CloneRepositoryAsync(string remoteUrl, string destinationParentPath) =>
        _commandRunner.RunAsync("git", $"clone {Quote(remoteUrl)}", destinationParentPath);

    public Task<CommandResult> SetRemoteOriginAsync(string repositoryPath, string remoteUrl) =>
        RunGitCommandAsync($"remote set-url origin {Quote(remoteUrl)}", repositoryPath);

    public Task<CommandResult> AddRemoteOriginAsync(string repositoryPath, string remoteUrl) =>
        RunGitCommandAsync($"remote add origin {Quote(remoteUrl)}", repositoryPath);

    public async Task<CommandResult> AddOrUpdateRemoteOriginAsync(string repositoryPath, string remoteUrl)
    {
        var existingRemote = await GetRemoteUrlAsync(repositoryPath);
        return existingRemote == "-"
            ? await AddRemoteOriginAsync(repositoryPath, remoteUrl)
            : await SetRemoteOriginAsync(repositoryPath, remoteUrl);
    }

    public async Task<bool> IsGitRepositoryAsync(string repositoryPath)
    {
        var result = await RunGitCommandAsync("rev-parse --is-inside-work-tree", repositoryPath);
        return result.IsSuccess && result.StandardOutput.Trim().Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<string> GetCurrentBranchAsync(string repositoryPath)
    {
        var result = await RunGitCommandAsync("branch --show-current", repositoryPath);
        return result.IsSuccess && !string.IsNullOrWhiteSpace(result.StandardOutput)
            ? result.StandardOutput.Trim()
            : "-";
    }

    public async Task<string> GetRemoteUrlAsync(string repositoryPath)
    {
        var result = await RunGitCommandAsync("remote get-url origin", repositoryPath);
        return result.IsSuccess && !string.IsNullOrWhiteSpace(result.StandardOutput)
            ? result.StandardOutput.Trim()
            : "-";
    }

    public async Task<GitRepositoryInfo> GetRepositoryInfoAsync(string repositoryPath)
    {
        if (!Directory.Exists(repositoryPath))
        {
            return new GitRepositoryInfo();
        }

        var isGitRepo = await IsGitRepositoryAsync(repositoryPath);
        if (!isGitRepo)
        {
            return new GitRepositoryInfo();
        }

        return new GitRepositoryInfo
        {
            IsGitRepository = true,
            CurrentBranch = await GetCurrentBranchAsync(repositoryPath),
            RemoteUrl = await GetRemoteUrlAsync(repositoryPath)
        };
    }

    private Task<CommandResult> RunGitCommandAsync(string args, string repositoryPath) =>
        _commandRunner.RunAsync("git", args, repositoryPath);

    private static string Quote(string value) =>
        $"\"{value.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";
}
