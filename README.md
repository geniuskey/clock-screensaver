# CLOCK Screen Saver

A Windows screen saver that turns your display into a customizable clock.

`Clock.scr`은 Windows 10·11용 화면 보호기입니다. 기본 페이스는 **지금의 하늘(Ciel)**입니다. CLOCK 사이트에서 페이스를 불러오며, 연결이 끊기면 PC 시각을 표시하고 1분마다 다시 연결을 시도합니다. 온라인 페이스를 표시하려면 Microsoft Edge WebView2 Runtime과 인터넷 연결이 필요합니다.

이 저장소는 Windows 화면 보호기와 미리 보기 이미지를 관리합니다. 시계 페이스와 서버 시간은 [CLOCK 웹사이트](https://clock.euiyun.com)에서 불러옵니다.

## 설치와 설정

1. [GitHub Releases](https://github.com/geniuskey/clock-screensaver/releases)에서 받은 `Clock.scr`을 계속 보관할 폴더에 둡니다.
2. 파일을 우클릭하고 **설치**를 선택합니다. Windows 11에서는 **더 많은 옵션 표시**에 있을 수 있습니다.
3. Windows **화면 보호기 설정**에서 `Clock`을 선택하고 대기 시간을 정합니다.
4. **설정**에서 대표 페이스, 시간대, 12/24시간제, 언어, 모니터를 선택합니다. **전체 화면 미리 보기**나 **브라우저에서 보기**로 확인한 뒤 **확인**을 눌러 저장합니다.

CLOCK 웹사이트에서 다른 페이스나 다이얼·색상 등 세부 옵션을 골랐다면, 시계 공유 링크를 복사해 **공유 링크 가져오기**에 붙여넣습니다.

기본적으로 주 모니터에 시계를 표시하고 나머지 모니터는 검게 채웁니다. **모든 모니터에 걸쳐 표시**를 고르면 전체 디스플레이에 걸쳐 페이스를 표시합니다. 마우스를 움직이거나 클릭하거나 키를 누르면 화면 보호기가 닫힙니다. Windows의 디스플레이 끄기 설정을 따릅니다.

화면 보호기를 제거하려면 Windows 화면 보호기 설정에서 **없음** 또는 다른 화면 보호기를 선택한 뒤 Clock.scr을 삭제합니다.

## 빌드와 확인

```powershell
dotnet build ClockSaver.csproj -c Release
./Test-Release.ps1
# bin/Release/net48/Clock.scr
```

실행에는 `Clock.scr` 파일 하나가 필요합니다. 배포 시 [LICENSE](LICENSE)와 [THIRD-PARTY-NOTICES.txt](THIRD-PARTY-NOTICES.txt)를 함께 제공합니다. [Windows CI](.github/workflows/windows.yml)에서 같은 빌드와 테스트를 실행합니다.

| 인수 | 동작 |
| --- | --- |
| `/s` | 전체 화면 실행 |
| `/c` 또는 인수 생략 | 설정 창 |
| `/p <HWND>` | Windows 설정 창 안의 미리 보기 이미지 |
| `/selftest` | 내장 WebView2 로더와 설치된 런타임 확인 |

```powershell
powershell.exe -NoProfile -STA -File Test-Preview.ps1 -ScreenSaver bin/Release/net48/Clock.scr
```

실행 로그와 자체 검사 결과는 `%LOCALAPPDATA%\Euiyun\Clock Screen Saver`에 기록됩니다.

## Code signing policy

SignPath Foundation 코드 서명을 준비 중입니다. 서명 상태는 각 릴리스 안내에 표시합니다. [Code signing policy](CODE-SIGNING.md) · [Privacy policy](PRIVACY.md)

## 라이선스

이 저장소의 화면 보호기 소스와 포함된 정적 미리 보기 이미지는 [MIT 라이선스](LICENSE)의 적용을 받습니다. 화면 보호기가 불러오는 CLOCK 웹사이트는 별도로 관리됩니다. 내장 WebView2 SDK의 라이선스와 고지는 [THIRD-PARTY-NOTICES.txt](THIRD-PARTY-NOTICES.txt)를 참조하세요.
