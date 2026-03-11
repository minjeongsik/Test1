# Git_Helper

`Git_Helper`는 .NET 8 WinForms 기반의 Git 보조 데스크톱 도구입니다.
초보자도 **저장소 초기 설정(Init/Clone/Remote 연결)**부터 **일상 Git 작업(Status/Add/Commit/Push/Pull)**까지 한 화면에서 순서대로 진행할 수 있도록 구성했습니다.

## 주요 기능

- TabControl 기반 3개 탭 UI
  - **초기 설정 탭**: 저장소 경로 선택, `git init`, `git clone`, `origin` 설정, 저장소 정보 새로고침
  - **기본 Git 작업 탭**: `status`, `add .`, `commit`, `push`, `pull`, 최초 업로드(추적 브랜치 설정 포함)
  - **로그 탭**: 실행 명령어 + stdout/stderr + 성공/실패 코드 표시
- 로컬 경로 검증 + Git 저장소 여부 검증
  - `git rev-parse --is-inside-work-tree` 기반
  - Git 저장소가 아니면 branch/remote를 `-`로 표시
- 원격 저장소 연결 자동 처리
  - `origin`이 없으면 `git remote add origin <url>`
  - 있으면 `git remote set-url origin <url>`
- 최초 업로드 지원
  - `git add .` → `git commit -m "..."` → `git push -u origin <branch>`
- Git 미설치 시 안내 메시지 및 로그 출력
- 위험한 명령(예: force push, hard reset) 미구현

## 탭별 사용 목적

### 1) 초기 설정 탭
처음 프로젝트를 연결할 때 사용하는 탭입니다.

- Repository Path 선택 (`Browse`)
- 새 로컬 저장소 생성 (`Init Repo`)
- 원격 저장소 복제 (`Clone Repo`)
- Remote URL 입력 후 `Set Remote`
- `Refresh Info`로 Current Branch / Remote URL 즉시 확인

### 2) 기본 Git 작업 탭
매일 사용하는 일반 작업을 모아둔 탭입니다.

- `Status`, `Add All`, `Commit`, `Push`, `Pull`
- Commit Message 입력창
- 현재 선택된 저장소 경로 상시 표시
- **First Upload** 버튼으로 최초 업로드 흐름 자동 실행

### 3) 로그 탭
명령 실행 결과를 상세하게 확인하는 탭입니다.

- 실행된 명령어 표시
- stdout/stderr 전체 표시
- 성공/실패(Exit Code) 표시
- `Clear Log`로 로그 초기화

## 사용 순서 예시

1. 초기 설정 탭에서 저장소 폴더 선택
2. `Init Repo` 또는 `Clone Repo` 실행
3. Remote URL 입력 후 `Set Remote`
4. 기본 Git 작업 탭에서 `Add All` / `Commit` / `Push`
5. 로그 탭에서 실행 결과 확인

## 프로젝트 구조

```text
Git_Helper.sln
Git_Helper/
  Git_Helper.csproj           # .NET 8 WinForms project
  Program.cs                  # Application entry point
  Forms/
    MainForm.cs               # Tab UI + 사용자 상호작용 + 검증/로그
  Services/
    CommandResult.cs          # Command result model
    CommandRunner.cs          # Process wrapper for stdout/stderr capture
    GitRepositoryInfo.cs      # Branch/remote/repo 여부 메타데이터 DTO
    GitService.cs             # Git 기능(Init/Clone/Remote/Push/Pull 등)
```

## 요구 사항

- .NET SDK 8.0+
- Windows OS (`net8.0-windows` WinForms)
- Git 설치 및 `PATH` 등록

## 빌드 및 실행

```bash
dotnet restore
dotnet build Git_Helper.sln
dotnet run --project Git_Helper/Git_Helper.csproj
```
