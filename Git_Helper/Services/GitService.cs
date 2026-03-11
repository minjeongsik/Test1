namespace Git_Helper.Services;

public class GitService
{
    private readonly CommandRunner _commandRunner;

    public GitService(CommandRunner commandRunner)
    {
        _commandRunner = commandRunner;
    }

    public Task<CommandResult> GetStatusAsync(string repositoryPath) =>
        RunGitCommandAsync("status", repositoryPath);

    public Task<CommandResult> AddAllAsync(string repositoryPath) =>
        RunGitCommandAsync("add .", repositoryPath);

    public Task<CommandResult> CommitAsync(string repositoryPath, string message) =>
        RunGitCommandAsync($"commit -m {Quote(message)}", repositoryPath);

    public Task<CommandResult> PushAsync(string repositoryPath) =>
        RunGitCommandAsync("push", repositoryPath);

    public Task<CommandResult> PullAsync(string repositoryPath) =>
        RunGitCommandAsync("pull", repositoryPath);

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
            : "Unknown";
    }

    public async Task<string> GetRemoteUrlAsync(string repositoryPath)
    {
        var result = await RunGitCommandAsync("remote get-url origin", repositoryPath);
        return result.IsSuccess && !string.IsNullOrWhiteSpace(result.StandardOutput)
            ? result.StandardOutput.Trim()
            : "Not configured";
    }

    /// <summary>
    /// Runs a git command in the selected repository and returns the command result.
    /// </summary>
    private Task<CommandResult> RunGitCommandAsync(string args, string repositoryPath) =>
        _commandRunner.RunAsync("git", args, repositoryPath);

    private static string Quote(string value) =>
        $"\"{value.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";
}
