# Code signing policy

CLOCK Screen Saver intends to use free code signing provided by [SignPath.io](https://about.signpath.io/), with a certificate issued by [SignPath Foundation](https://signpath.org/). SignPath Foundation signing is pending. Each [release](https://github.com/geniuskey/clock-screensaver/releases) identifies whether its executable has been signed.

- Committer and reviewer · [geniuskey](https://github.com/geniuskey). The maintainer reviews contributions before merging.
- Signing approver · [geniuskey](https://github.com/geniuskey). Each production signing request requires manual approval in SignPath.
- Anyone with commit or signing access must use multifactor authentication on GitHub and SignPath.
- The workflow builds `Clock.scr` from the official Git tag on a GitHub-hosted Windows runner. The artifact configuration targets `Clock.scr` and checks the product name and release version. The workflow checks the downloaded Authenticode signature and timestamp before uploading the signed artifact. [Windows build](.github/workflows/windows.yml) · [Artifact configuration](.signpath/clock-scr.xml)

See the [privacy policy](PRIVACY.md) for the local data and network requests used by CLOCK and WebView2.

## Maintainer setup after approval

1. Link the official repository to the SignPath GitHub.com trusted build system. Set up a production signing policy with manual approval.
2. Import the [artifact configuration](.signpath/clock-scr.xml) in SignPath with slug `clock-scr`.
3. Set up the GitHub `signpath` environment for official `v*` release tags. Add the environment secret `SIGNPATH_API_TOKEN` for a SignPath signing submitter. Add the environment variables `SIGNPATH_ORGANIZATION_ID`, `SIGNPATH_PROJECT_SLUG`, and `SIGNPATH_SIGNING_POLICY_SLUG` from the approved project.
4. Run the **Windows build** workflow with signing enabled against the matching release tag. With GitHub CLI, use `gh workflow run windows.yml --ref v0.1.1 -f sign=true` for tag `v0.1.1`. After manual approval, use `Clock-screensaver-signed` as the release download. Include the project license and third-party notices with the download.

`Clock.scr` embeds Microsoft-distributed WebView2 SDK binaries. The runtime is installed separately, and the hosted CLOCK web application is maintained separately. Disclose both to SignPath Foundation during onboarding so it can determine whether the build qualifies under the [Foundation conditions](https://signpath.org/terms).
