# Git_Helper

`Git_Helper`는 **C# .NET 8 WinForms**로 만든 Git 보조 도구입니다.  
터미널 명령이 익숙하지 않은 사용자도, 아래 흐름대로 클릭만으로 Git 작업을 진행할 수 있도록 설계되었습니다.

- 저장소 처음 만들기 (`git init`)
- 원격 저장소 복제 (`git clone`)
- GitHub 원격 연결 (`git remote add/set-url origin`)
- 일상 작업 (`status`, `add`, `commit`, `push`, `pull`)
- 실행 로그 확인 (명령어, stdout, stderr, 종료 코드)

---

## 1. 이 프로그램이 해결하는 문제

처음 Git을 사용할 때는 아래가 가장 어렵습니다.

1. "내 폴더가 Git 저장소인지" 확인하기
2. 로컬 저장소를 GitHub 원격과 연결하기
3. 최초 업로드(`push -u`)와 일반 `push` 차이 이해하기
4. 실패했을 때 원인(오류 메시지) 확인하기

`Git_Helper`는 위 과정을 **탭 순서대로** 나누어 제공해, 처음 쓰는 사람도 자연스럽게 진행할 수 있습니다.

---

## 2. 화면 구성 (TabControl)

앱은 3개 탭으로 구성됩니다.

## 2-1. 초기 설정 탭

처음 저장소를 준비할 때 사용하는 탭입니다.

- **Repository Path**: 작업할 로컬 폴더 경로
- **Browse**: 폴더 선택
- **Init Repo**: 선택 폴더에서 `git init` 실행
- **Clone Repo**: 입력한 Remote URL을 지정한 상위 폴더로 `git clone`
- **Remote URL**: GitHub 저장소 URL 입력 (예: `https://github.com/user/repo.git`)
- **Set Remote**: `origin` 원격 등록/변경
  - origin 없음: `git remote add origin <url>`
  - origin 있음: `git remote set-url origin <url>`
- **Refresh Info**: 현재 저장소 정보를 다시 읽어 표시
- **Current Branch / Remote URL**: 현재 저장소 메타데이터 확인

> 참고: 선택 폴더가 Git 저장소가 아니면 Branch/Remote 값은 `-`로 표시됩니다.

## 2-2. 기본 Git 작업 탭

매일 사용하는 작업을 모아둔 탭입니다.

- **Status**: 변경 상태 확인 (`git status`)
- **Add All**: 전체 변경 스테이징 (`git add .`)
- **Commit**: 커밋 생성 (`git commit -m "..."`)
- **Push**: 원격 반영 (`git push`)
- **Pull**: 원격 변경 가져오기 (`git pull`)
- **First Upload**: 최초 업로드용 자동 흐름
  1) `git add .`
  2) `git commit -m "..."`
  3) `git push -u origin <현재 브랜치>`
- **Commit Message**: 커밋 메시지 입력
- **현재 선택된 저장소**: 실수 방지를 위해 작업 대상 경로를 항상 표시

## 2-3. 로그 탭

모든 실행 결과를 확인하는 탭입니다.

- 실행된 명령어 (`> git ...`)
- 표준 출력(stdout)
- 표준 에러(stderr)
- 성공/실패 및 Exit Code
- **Clear Log** 버튼으로 로그 초기화

---

## 3. 처음 사용하는 사람을 위한 권장 작업 순서

### 시나리오 A: 새 폴더를 Git 저장소로 시작

1. **초기 설정 탭**에서 `Browse`로 폴더 선택
2. `Init Repo` 클릭
3. GitHub에 미리 만든 저장소 URL을 `Remote URL`에 입력
4. `Set Remote` 클릭
5. `Refresh Info`로 Branch/Remote 표시 확인
6. **기본 Git 작업 탭**으로 이동
7. `Add All` → Commit Message 입력 → `Commit`
8. 최초 반영은 `First Upload` 또는 `Push` 사용
9. **로그 탭**에서 결과 확인

### 시나리오 B: 기존 GitHub 저장소를 로컬로 가져오기

1. **초기 설정 탭**에서 `Remote URL` 입력
2. `Clone Repo` 클릭 후, 복제할 **상위 폴더** 선택
3. Clone 성공 시 Repository Path가 자동 반영됨
4. `Refresh Info`로 Branch/Remote 확인
5. 이후 **기본 Git 작업 탭**에서 일반 작업 진행

---

## 4. 유효성 검사 / 안전장치

앱은 아래 상황에서 동작을 막고 안내 메시지를 표시합니다.

- Git 미설치 또는 PATH 미등록
- Repository Path가 비어 있거나 잘못된 경로
- Git 저장소가 아닌 폴더에서 Git 작업 실행 시도
- Commit Message 없이 `Commit` 또는 `First Upload` 실행
- origin remote가 없는데 최초 업로드 시도

또한 위험한 명령은 의도적으로 제공하지 않습니다.

- `push --force` 미구현
- `reset --hard` 미구현

---

## 5. 내부 동작 요약 (구조)

```text
Git_Helper.sln
Git_Helper/
  Git_Helper.csproj           # .NET 8 WinForms
  Program.cs                  # 앱 시작점
  Forms/
    MainForm.cs               # 3개 탭 UI + 이벤트 + 검증 + 로그
  Services/
    CommandResult.cs          # 명령 실행 결과 모델 (stdout/stderr/exit code)
    CommandRunner.cs          # Process 기반 커맨드 실행기
    GitRepositoryInfo.cs      # 저장소 메타데이터 DTO (isRepo/branch/remote)
    GitService.cs             # Git 명령 래핑 서비스
```

- `CommandRunner`: 실제 프로세스 실행, stdout/stderr 수집
- `GitService`: Git 명령 기능 분리 (`init`, `clone`, `status`, `push` 등)
- `MainForm`: 사용자 입력/검증/로그 출력, 탭 기반 흐름 제어

---

## 6. 요구 사항

- **Windows OS** (`net8.0-windows` WinForms)
- **.NET SDK 8.0+**
- **Git 설치 + PATH 등록**

Git 확인 예시:

```bash
git --version
```

---

## 7. 빌드 / 실행

저장소 루트에서 실행:

```bash
dotnet restore
dotnet build Git_Helper.sln
dotnet run --project Git_Helper/Git_Helper.csproj
```

---

## 8. 자주 묻는 문제 (Troubleshooting)

### Q1. Branch/Remote가 계속 `-`로 보입니다.
선택한 폴더가 Git 저장소가 아닐 가능성이 큽니다.  
`Init Repo`를 먼저 실행하거나, `Clone Repo`로 저장소를 가져오세요.

### Q2. Push가 실패합니다.
주요 원인은 아래 중 하나입니다.

- origin remote 미설정 (`Set Remote` 필요)
- 인증 문제 (GitHub 로그인/토큰)
- 권한 없는 저장소에 push 시도

로그 탭의 stderr 메시지를 먼저 확인하세요.

### Q3. Commit이 안 됩니다.
- Commit Message가 비어 있으면 차단됩니다.
- 변경 파일이 없으면 Git이 커밋할 내용이 없다고 안내할 수 있습니다.

### Q4. Clone 후 경로가 예상과 다릅니다.
`Clone Repo`는 선택한 폴더의 하위에 저장소명을 기준으로 새 폴더를 생성합니다.  
예: 상위 폴더 `D:\Work`, URL이 `.../sample.git`이면 `D:\Work\sample`에 생성됩니다.

---

## 9. 사용 팁

- 작업 전 `Status`로 현재 상태를 먼저 확인하세요.
- 초반에는 `First Upload`를 사용하면 upstream 설정 실수를 줄일 수 있습니다.
- 실패 시 로그 탭의 **stderr**를 먼저 확인하면 원인 파악이 가장 빠릅니다.

