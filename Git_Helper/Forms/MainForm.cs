using Git_Helper.Services;

namespace Git_Helper.Forms;

public class MainForm : Form
{
    private readonly GitService _gitService = new(new CommandRunner());

    private readonly TextBox _repositoryPathTextBox = new() { Width = 520 };
    private readonly Label _branchValueLabel = new() { AutoSize = true, Text = "-" };
    private readonly Label _remoteValueLabel = new() { AutoSize = true, Text = "-" };
    private readonly TextBox _commitMessageTextBox = new() { Width = 600 };
    private readonly RichTextBox _logRichTextBox = new()
    {
        ReadOnly = true,
        Width = 780,
        Height = 300,
        Font = new Font("Consolas", 10)
    };

    private readonly Button _statusButton = new() { Text = "Status", Width = 90, Height = 50 };
    private readonly Button _addAllButton = new() { Text = "Add All", Width = 90, Height = 50 };
    private readonly Button _commitButton = new() { Text = "Commit", Width = 90, Height = 50 };
    private readonly Button _pushButton = new() { Text = "Push", Width = 90, Height = 50 };
    private readonly Button _pullButton = new() { Text = "Pull", Width = 90, Height = 50 };

    public MainForm()
    {
        Text = "Git Helper";
        Width = 840;
        Height = 640;
        StartPosition = FormStartPosition.CenterScreen;

        InitializeLayout();
        WireEvents();
    }

    private void InitializeLayout()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            ColumnCount = 1,
            RowCount = 6,
            AutoSize = true
        };

        var pathPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
        var browseButton = new Button { Text = "Browse", Width = 90, Height = 30 };

        pathPanel.Controls.Add(new Label { Text = "Repository Path:", AutoSize = true, Padding = new Padding(0, 7, 0, 0) });
        pathPanel.Controls.Add(_repositoryPathTextBox);
        pathPanel.Controls.Add(browseButton);

        var infoPanel = new TableLayoutPanel { ColumnCount = 2, AutoSize = true };
        infoPanel.Controls.Add(new Label { Text = "Current Branch:", AutoSize = true }, 0, 0);
        infoPanel.Controls.Add(_branchValueLabel, 1, 0);
        infoPanel.Controls.Add(new Label { Text = "Remote URL:", AutoSize = true }, 0, 1);
        infoPanel.Controls.Add(_remoteValueLabel, 1, 1);

        var commandPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
        commandPanel.Controls.AddRange([_statusButton, _addAllButton, _commitButton, _pushButton, _pullButton]);

        var commitPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
        commitPanel.Controls.Add(new Label { Text = "Commit Message:", AutoSize = true, Padding = new Padding(0, 7, 0, 0) });
        commitPanel.Controls.Add(_commitMessageTextBox);

        root.Controls.Add(pathPanel);
        root.Controls.Add(infoPanel);
        root.Controls.Add(commandPanel);
        root.Controls.Add(commitPanel);
        root.Controls.Add(new Label { Text = "Logs:", AutoSize = true, Padding = new Padding(0, 10, 0, 0) });
        root.Controls.Add(_logRichTextBox);

        Controls.Add(root);

        browseButton.Click += (_, _) => SelectRepositoryFolder();
    }

    private void WireEvents()
    {
        _statusButton.Click += async (_, _) => await ExecuteAsync("git status", _gitService.GetStatusAsync);
        _addAllButton.Click += async (_, _) => await ExecuteAsync("git add .", _gitService.AddAllAsync);
        _pushButton.Click += async (_, _) => await ExecuteAsync("git push", _gitService.PushAsync);
        _pullButton.Click += async (_, _) => await ExecuteAsync("git pull", _gitService.PullAsync);
        _commitButton.Click += async (_, _) => await CommitAsync();
    }

    /// <summary>
    /// Opens a folder picker and updates branch/remote metadata for the selected repository.
    /// </summary>
    private async void SelectRepositoryFolder()
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Select a Git repository folder",
            UseDescriptionForTitle = true
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            _repositoryPathTextBox.Text = dialog.SelectedPath;
            await RefreshRepositoryInfoAsync();
        }
    }

    private async Task CommitAsync()
    {
        if (string.IsNullOrWhiteSpace(_commitMessageTextBox.Text))
        {
            MessageBox.Show("Commit message cannot be empty.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        await ExecuteAsync($"git commit -m \"{_commitMessageTextBox.Text}\"",
            path => _gitService.CommitAsync(path, _commitMessageTextBox.Text));
    }

    /// <summary>
    /// Runs a Git action, updates logs, and refreshes repository metadata.
    /// </summary>
    private async Task ExecuteAsync(string displayCommand, Func<string, Task<CommandResult>> action)
    {
        var path = _repositoryPathTextBox.Text.Trim();
        if (!await ValidateRepositoryPathAsync(path))
        {
            return;
        }

        ToggleButtons(false);
        Log($"> {displayCommand}");

        try
        {
            var result = await action(path);
            if (!string.IsNullOrWhiteSpace(result.StandardOutput))
            {
                Log(result.StandardOutput);
            }

            if (!string.IsNullOrWhiteSpace(result.StandardError))
            {
                Log($"ERROR: {result.StandardError}");
            }

            Log($"Exit Code: {result.ExitCode}");
            Log(new string('-', 60));

            await RefreshRepositoryInfoAsync();
        }
        catch (Exception ex)
        {
            Log($"Unexpected error: {ex.Message}");
        }
        finally
        {
            ToggleButtons(true);
        }
    }

    private async Task RefreshRepositoryInfoAsync()
    {
        var path = _repositoryPathTextBox.Text.Trim();
        if (!Directory.Exists(path) || !await _gitService.IsGitRepositoryAsync(path))
        {
            _branchValueLabel.Text = "-";
            _remoteValueLabel.Text = "-";
            return;
        }

        _branchValueLabel.Text = await _gitService.GetCurrentBranchAsync(path);
        _remoteValueLabel.Text = await _gitService.GetRemoteUrlAsync(path);
    }

    /// <summary>
    /// Validates that a directory exists and is a Git repository before executing commands.
    /// </summary>
    private async Task<bool> ValidateRepositoryPathAsync(string path)
    {
        if (!Directory.Exists(path))
        {
            MessageBox.Show("Please choose a valid repository path.", "Invalid Path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (!await _gitService.IsGitRepositoryAsync(path))
        {
            MessageBox.Show("Selected folder is not a Git repository.", "Invalid Repository", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }

    private void ToggleButtons(bool enabled)
    {
        _statusButton.Enabled = enabled;
        _addAllButton.Enabled = enabled;
        _commitButton.Enabled = enabled;
        _pushButton.Enabled = enabled;
        _pullButton.Enabled = enabled;
    }

    private void Log(string message)
    {
        _logRichTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        _logRichTextBox.ScrollToCaret();
    }
}
