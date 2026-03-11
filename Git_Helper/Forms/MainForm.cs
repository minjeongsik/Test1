using Git_Helper.Services;

namespace Git_Helper.Forms;

public class MainForm : Form
{
    private readonly GitService _gitService = new(new CommandRunner());

    private readonly TextBox _repositoryPathTextBox = new() { Width = 540 };
    private readonly TextBox _remoteUrlTextBox = new() { Width = 540 };
    private readonly TextBox _commitMessageTextBox = new() { Width = 640 };

    private readonly Label _branchValueLabel = new() { AutoSize = true, Text = "-" };
    private readonly Label _remoteValueLabel = new() { AutoSize = true, Text = "-" };
    private readonly Label _selectedPathValueLabel = new() { AutoSize = true, Text = "-" };

    private readonly RichTextBox _logRichTextBox = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        Font = new Font("Consolas", 10)
    };

    private readonly Button _browseButton = new() { Text = "Browse", Width = 110, Height = 34 };
    private readonly Button _initRepoButton = new() { Text = "Init Repo", Width = 110, Height = 34 };
    private readonly Button _cloneRepoButton = new() { Text = "Clone Repo", Width = 110, Height = 34 };
    private readonly Button _setRemoteButton = new() { Text = "Set Remote", Width = 110, Height = 34 };
    private readonly Button _refreshInfoButton = new() { Text = "Refresh Info", Width = 110, Height = 34 };

    private readonly Button _statusButton = new() { Text = "Status", Width = 120, Height = 50 };
    private readonly Button _addAllButton = new() { Text = "Add All", Width = 120, Height = 50 };
    private readonly Button _commitButton = new() { Text = "Commit", Width = 120, Height = 50 };
    private readonly Button _pushButton = new() { Text = "Push", Width = 120, Height = 50 };
    private readonly Button _pullButton = new() { Text = "Pull", Width = 120, Height = 50 };
    private readonly Button _firstPushButton = new() { Text = "First Upload", Width = 120, Height = 50 };
    private readonly Button _clearLogButton = new() { Text = "Clear Log", Width = 110, Height = 34 };

    public MainForm()
    {
        Text = "Git Helper";
        Width = 980;
        Height = 760;
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
            RowCount = 1
        };

        var tabControl = new TabControl { Dock = DockStyle.Fill };
        tabControl.TabPages.Add(CreateSetupTab());
        tabControl.TabPages.Add(CreateOperationsTab());
        tabControl.TabPages.Add(CreateLogTab());

        root.Controls.Add(tabControl, 0, 0);
        Controls.Add(root);
    }

    private TabPage CreateSetupTab()
    {
        var tab = new TabPage("초기 설정");
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            ColumnCount = 1,
            RowCount = 8,
            AutoScroll = true
        };

        layout.Controls.Add(new Label
        {
            Text = "1) 저장소 폴더를 고르고, 2) 새 저장소 초기화 또는 Clone, 3) Remote를 연결하세요.",
            AutoSize = true,
            Font = new Font(Font, FontStyle.Bold),
            Margin = new Padding(0, 0, 0, 10)
        });

        var pathPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = new Padding(0, 0, 0, 8) };
        pathPanel.Controls.Add(new Label { Text = "Repository Path:", AutoSize = true, Padding = new Padding(0, 8, 0, 0), Width = 130 });
        pathPanel.Controls.Add(_repositoryPathTextBox);
        pathPanel.Controls.Add(_browseButton);
        layout.Controls.Add(pathPanel);

        var initClonePanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = new Padding(0, 0, 0, 8) };
        initClonePanel.Controls.Add(_initRepoButton);
        initClonePanel.Controls.Add(_cloneRepoButton);
        initClonePanel.Controls.Add(_refreshInfoButton);
        layout.Controls.Add(initClonePanel);

        var remotePanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = new Padding(0, 0, 0, 8) };
        remotePanel.Controls.Add(new Label { Text = "Remote URL:", AutoSize = true, Padding = new Padding(0, 8, 0, 0), Width = 130 });
        remotePanel.Controls.Add(_remoteUrlTextBox);
        remotePanel.Controls.Add(_setRemoteButton);
        layout.Controls.Add(remotePanel);

        layout.Controls.Add(new Label
        {
            Text = "초기 push 전에는 Remote URL을 먼저 설정하세요. Git 저장소가 아니면 정보는 '-' 로 표시됩니다.",
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 12)
        });

        var infoPanel = new TableLayoutPanel { ColumnCount = 2, AutoSize = true };
        infoPanel.Controls.Add(new Label { Text = "Current Branch:", AutoSize = true }, 0, 0);
        infoPanel.Controls.Add(_branchValueLabel, 1, 0);
        infoPanel.Controls.Add(new Label { Text = "Remote URL:", AutoSize = true }, 0, 1);
        infoPanel.Controls.Add(_remoteValueLabel, 1, 1);
        layout.Controls.Add(infoPanel);

        tab.Controls.Add(layout);
        return tab;
    }

    private TabPage CreateOperationsTab()
    {
        var tab = new TabPage("기본 Git 작업");
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            ColumnCount = 1,
            RowCount = 5,
            AutoScroll = true
        };

        layout.Controls.Add(new Label
        {
            Text = "일상 작업: Status 확인 → Add All → Commit → Push 순서로 진행하세요.",
            AutoSize = true,
            Font = new Font(Font, FontStyle.Bold),
            Margin = new Padding(0, 0, 0, 12)
        });

        var selectedPathPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = new Padding(0, 0, 0, 12) };
        selectedPathPanel.Controls.Add(new Label { Text = "현재 선택된 저장소:", AutoSize = true, Padding = new Padding(0, 4, 0, 0) });
        selectedPathPanel.Controls.Add(_selectedPathValueLabel);
        layout.Controls.Add(selectedPathPanel);

        var buttonPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = new Padding(0, 0, 0, 12) };
        buttonPanel.Controls.AddRange([_statusButton, _addAllButton, _commitButton, _pushButton, _pullButton, _firstPushButton]);
        layout.Controls.Add(buttonPanel);

        var commitPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
        commitPanel.Controls.Add(new Label { Text = "Commit Message:", AutoSize = true, Padding = new Padding(0, 8, 0, 0), Width = 130 });
        commitPanel.Controls.Add(_commitMessageTextBox);
        layout.Controls.Add(commitPanel);

        tab.Controls.Add(layout);
        return tab;
    }

    private TabPage CreateLogTab()
    {
        var tab = new TabPage("로그");
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            ColumnCount = 1,
            RowCount = 2
        };

        var header = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40 };
        header.Controls.Add(new Label { Text = "실행 명령, stdout, stderr가 모두 기록됩니다.", AutoSize = true, Padding = new Padding(0, 8, 10, 0) });
        header.Controls.Add(_clearLogButton);

        layout.Controls.Add(header, 0, 0);
        layout.Controls.Add(_logRichTextBox, 0, 1);

        tab.Controls.Add(layout);
        return tab;
    }

    private void WireEvents()
    {
        Load += async (_, _) => await EnsureGitAvailableAndRefreshAsync();

        _browseButton.Click += (_, _) => SelectRepositoryFolder();
        _refreshInfoButton.Click += async (_, _) => await RefreshRepositoryInfoAsync();
        _initRepoButton.Click += async (_, _) => await InitializeRepositoryAsync();
        _cloneRepoButton.Click += async (_, _) => await CloneRepositoryAsync();
        _setRemoteButton.Click += async (_, _) => await SetRemoteAsync();

        _statusButton.Click += async (_, _) => await ExecuteGitCommandAsync("git status", _gitService.GetStatusAsync);
        _addAllButton.Click += async (_, _) => await ExecuteGitCommandAsync("git add .", _gitService.AddAllAsync);
        _pullButton.Click += async (_, _) => await ExecuteGitCommandAsync("git pull", _gitService.PullAsync);
        _pushButton.Click += async (_, _) => await ExecuteGitCommandAsync("git push", _gitService.PushAsync);
        _commitButton.Click += async (_, _) => await CommitAsync();
        _firstPushButton.Click += async (_, _) => await FirstUploadAsync();

        _clearLogButton.Click += (_, _) => _logRichTextBox.Clear();
        _repositoryPathTextBox.TextChanged += (_, _) => SyncSelectedPathLabel();
    }

    private async Task EnsureGitAvailableAndRefreshAsync()
    {
        if (!await IsGitInstalledAsync())
        {
            MessageBox.Show("Git이 설치되어 있지 않거나 PATH에 등록되지 않았습니다.", "Git Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        await RefreshRepositoryInfoAsync();
    }

    private async void SelectRepositoryFolder()
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "저장소 폴더를 선택하세요.",
            UseDescriptionForTitle = true
        };

        if (dialog.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        _repositoryPathTextBox.Text = dialog.SelectedPath;
        await RefreshRepositoryInfoAsync();
    }

    private async Task InitializeRepositoryAsync()
    {
        var path = _repositoryPathTextBox.Text.Trim();
        if (!await ValidateDirectoryPathAsync(path))
        {
            return;
        }

        await RunAndLogCommandAsync("git init", () => _gitService.InitRepositoryAsync(path), refreshInfo: true);
    }

    private async Task CloneRepositoryAsync()
    {
        if (!await IsGitInstalledAsync())
        {
            return;
        }

        var remoteUrl = _remoteUrlTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(remoteUrl))
        {
            MessageBox.Show("Clone할 원격 저장소 URL을 입력하세요.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var dialog = new FolderBrowserDialog
        {
            Description = "Clone 결과를 저장할 상위 폴더를 선택하세요.",
            UseDescriptionForTitle = true
        };

        if (dialog.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        var destinationParentPath = dialog.SelectedPath;
        var result = await RunAndLogCommandAsync($"git clone {remoteUrl}", () => _gitService.CloneRepositoryAsync(remoteUrl, destinationParentPath));
        if (!result.IsSuccess)
        {
            return;
        }

        var repositoryName = GetRepositoryFolderName(remoteUrl);
        var clonedPath = Path.Combine(destinationParentPath, repositoryName);

        if (!Directory.Exists(clonedPath))
        {
            LogWarning($"Clone은 성공했지만, 폴더를 자동 탐지하지 못했습니다: {clonedPath}");
            return;
        }

        _repositoryPathTextBox.Text = clonedPath;
        await RefreshRepositoryInfoAsync();
    }

    private async Task SetRemoteAsync()
    {
        var path = _repositoryPathTextBox.Text.Trim();
        if (!await ValidateRepositoryPathAsync(path))
        {
            return;
        }

        var remoteUrl = _remoteUrlTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(remoteUrl))
        {
            MessageBox.Show("원격 저장소 URL을 입력하세요.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        await RunAndLogCommandAsync($"git remote add/set-url origin {remoteUrl}", () => _gitService.AddOrUpdateRemoteOriginAsync(path, remoteUrl), refreshInfo: true);
    }

    private async Task CommitAsync()
    {
        var path = _repositoryPathTextBox.Text.Trim();
        if (!await ValidateRepositoryPathAsync(path))
        {
            return;
        }

        var message = _commitMessageTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(message))
        {
            MessageBox.Show("Commit message를 입력하세요.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        await RunAndLogCommandAsync($"git commit -m \"{message}\"", () => _gitService.CommitAsync(path, message), refreshInfo: true);
    }

    private async Task FirstUploadAsync()
    {
        var path = _repositoryPathTextBox.Text.Trim();
        if (!await ValidateRepositoryPathAsync(path))
        {
            return;
        }

        var message = _commitMessageTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(message))
        {
            MessageBox.Show("최초 업로드를 위해 Commit message를 입력하세요.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var branch = await _gitService.GetCurrentBranchAsync(path);
        if (branch == "-")
        {
            MessageBox.Show("현재 브랜치를 확인할 수 없습니다. 초기화 또는 clone 후 다시 시도하세요.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var remote = await _gitService.GetRemoteUrlAsync(path);
        if (remote == "-")
        {
            MessageBox.Show("origin remote가 설정되어 있지 않습니다. 초기 설정 탭에서 Set Remote를 먼저 실행하세요.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var addResult = await RunAndLogCommandAsync("git add .", () => _gitService.AddAllAsync(path));
        if (!addResult.IsSuccess)
        {
            return;
        }

        var commitResult = await RunAndLogCommandAsync($"git commit -m \"{message}\"", () => _gitService.CommitAsync(path, message));
        if (!commitResult.IsSuccess)
        {
            return;
        }

        await RunAndLogCommandAsync($"git push -u origin {branch}", () => _gitService.PushWithUpstreamAsync(path, branch), refreshInfo: true);
    }

    private async Task ExecuteGitCommandAsync(string displayCommand, Func<string, Task<CommandResult>> action)
    {
        var path = _repositoryPathTextBox.Text.Trim();
        if (!await ValidateRepositoryPathAsync(path))
        {
            return;
        }

        await RunAndLogCommandAsync(displayCommand, () => action(path), refreshInfo: true);
    }

    private async Task<CommandResult> RunAndLogCommandAsync(string displayCommand, Func<Task<CommandResult>> action, bool refreshInfo = false)
    {
        ToggleButtons(false);
        LogCommand(displayCommand);

        try
        {
            var result = await action();
            LogCommandResult(result);

            if (refreshInfo)
            {
                await RefreshRepositoryInfoAsync();
            }

            return result;
        }
        catch (Exception ex)
        {
            LogError($"Unexpected error: {ex.Message}");
            return new CommandResult { ExitCode = -1, StandardError = ex.Message };
        }
        finally
        {
            ToggleButtons(true);
        }
    }

    private async Task RefreshRepositoryInfoAsync()
    {
        SyncSelectedPathLabel();

        var path = _repositoryPathTextBox.Text.Trim();
        var info = await _gitService.GetRepositoryInfoAsync(path);

        _branchValueLabel.Text = info.CurrentBranch;
        _remoteValueLabel.Text = info.RemoteUrl;

        if (!string.IsNullOrWhiteSpace(_remoteUrlTextBox.Text) || info.RemoteUrl == "-")
        {
            return;
        }

        _remoteUrlTextBox.Text = info.RemoteUrl;
    }

    private async Task<bool> ValidateDirectoryPathAsync(string path)
    {
        if (!await IsGitInstalledAsync())
        {
            return false;
        }

        if (!Directory.Exists(path))
        {
            MessageBox.Show("유효한 폴더 경로를 선택하세요.", "Invalid Path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }

    private async Task<bool> ValidateRepositoryPathAsync(string path)
    {
        if (!await ValidateDirectoryPathAsync(path))
        {
            return false;
        }

        var isRepository = await _gitService.IsGitRepositoryAsync(path);
        if (isRepository)
        {
            return true;
        }

        _branchValueLabel.Text = "-";
        _remoteValueLabel.Text = "-";
        LogWarning("선택한 경로는 Git 저장소가 아닙니다. 초기 설정 탭에서 Init Repo 또는 Clone Repo를 실행하세요.");
        MessageBox.Show("선택한 폴더는 Git 저장소가 아닙니다. 초기 설정 탭에서 Init Repo 또는 Clone Repo를 실행하세요.", "Invalid Repository", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return false;
    }

    private async Task<bool> IsGitInstalledAsync()
    {
        var result = await _gitService.CheckGitInstallationAsync();
        if (result.IsSuccess)
        {
            return true;
        }

        LogError("Git 실행 파일을 찾지 못했습니다. Git을 설치하고 PATH를 확인하세요.");
        return false;
    }

    private static string GetRepositoryFolderName(string remoteUrl)
    {
        var fileName = Path.GetFileName(remoteUrl.TrimEnd('/'));
        return fileName.EndsWith(".git", StringComparison.OrdinalIgnoreCase)
            ? fileName[..^4]
            : fileName;
    }

    private void SyncSelectedPathLabel()
    {
        var path = _repositoryPathTextBox.Text.Trim();
        _selectedPathValueLabel.Text = string.IsNullOrWhiteSpace(path) ? "-" : path;
    }

    private void ToggleButtons(bool enabled)
    {
        _browseButton.Enabled = enabled;
        _initRepoButton.Enabled = enabled;
        _cloneRepoButton.Enabled = enabled;
        _setRemoteButton.Enabled = enabled;
        _refreshInfoButton.Enabled = enabled;

        _statusButton.Enabled = enabled;
        _addAllButton.Enabled = enabled;
        _commitButton.Enabled = enabled;
        _pushButton.Enabled = enabled;
        _pullButton.Enabled = enabled;
        _firstPushButton.Enabled = enabled;
    }

    private void LogCommand(string command) => Log($"> {command}");

    private void LogCommandResult(CommandResult result)
    {
        if (!string.IsNullOrWhiteSpace(result.StandardOutput))
        {
            Log($"[stdout]{Environment.NewLine}{result.StandardOutput}");
        }

        if (!string.IsNullOrWhiteSpace(result.StandardError))
        {
            LogError($"[stderr]{Environment.NewLine}{result.StandardError}");
        }

        if (result.IsSuccess)
        {
            Log("[success] Exit Code: 0");
        }
        else
        {
            LogError($"[failed] Exit Code: {result.ExitCode}");
        }

        Log(new string('-', 80));
    }

    private void LogWarning(string message) => Log($"[warning] {message}");

    private void LogError(string message) => Log($"[error] {message}");

    private void Log(string message)
    {
        _logRichTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        _logRichTextBox.ScrollToCaret();
    }
}
