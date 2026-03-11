# Git_Helper

`Git_Helper` is a lightweight C# WinForms desktop app for running common Git commands from a simple UI.

## Features

- Select a repository folder with a browse dialog
- View current branch and origin remote URL
- Run common Git commands:
  - `git status`
  - `git add .`
  - `git commit -m "..."`
  - `git push`
  - `git pull`
- Command output and errors are logged in-app
- Commit is blocked when the commit message is empty
- Repository path is validated to ensure it is a Git working tree
- Uses `ProcessStartInfo` for command execution
- Handles command and runtime errors gracefully

## Project Structure

```text
Git_Helper.sln
Git_Helper/
  Git_Helper.csproj           # .NET 8 WinForms project (Windows targeting enabled)
  Program.cs                  # Application entry point
  Forms/
    MainForm.cs               # Main desktop UI and user interactions
  Services/
    CommandResult.cs          # Command result model
    CommandRunner.cs          # ProcessStartInfo wrapper for running commands
    GitService.cs             # Git-focused operations built on CommandRunner
```

## Requirements

- .NET SDK 8.0+
- Windows OS (WinForms target: `net8.0-windows`)
- Git installed and available in `PATH`

## Build and Run

From the repository root:

```bash
dotnet restore
dotnet build Git_Helper.sln
dotnet run --project Git_Helper/Git_Helper.csproj
```

## Notes

- The app does **not** implement force push.
- Branch/remote labels are refreshed after running commands.
- If Git or the repository path is invalid, errors are shown in the log panel.
