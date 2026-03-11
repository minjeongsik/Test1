using System.Diagnostics;
using System.Text;

namespace Git_Helper.Services;

public class CommandRunner
{
    /// <summary>
    /// Executes a process and captures both stdout and stderr.
    /// </summary>
    public async Task<CommandResult> RunAsync(string fileName, string arguments, string workingDirectory)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
        var output = new StringBuilder();
        var error = new StringBuilder();

        var outputClosed = new TaskCompletionSource<bool>();
        var errorClosed = new TaskCompletionSource<bool>();

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data is null)
            {
                outputClosed.TrySetResult(true);
                return;
            }

            output.AppendLine(e.Data);
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data is null)
            {
                errorClosed.TrySetResult(true);
                return;
            }

            error.AppendLine(e.Data);
        };

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();
            await Task.WhenAll(outputClosed.Task, errorClosed.Task);

            return new CommandResult
            {
                ExitCode = process.ExitCode,
                StandardOutput = output.ToString().TrimEnd(),
                StandardError = error.ToString().TrimEnd()
            };
        }
        catch (Exception ex)
        {
            return new CommandResult
            {
                ExitCode = -1,
                StandardError = ex.Message
            };
        }
    }
}
