param([string]$ScreenSaver = (Join-Path $PSScriptRoot 'bin/Release/net48/Clock.scr'))

$ErrorActionPreference = 'Stop'
$source = (Resolve-Path -LiteralPath $ScreenSaver).Path
$test = New-Item -ItemType Directory -Path (Join-Path ([IO.Path]::GetTempPath()) ("clock-screensaver-test-{0:N}" -f [Guid]::NewGuid()))
$copy = Join-Path $test.FullName 'Clock.scr'
$data = Join-Path $test.FullName 'data'
$startInfo = [Diagnostics.ProcessStartInfo]::new($copy)
$startInfo.UseShellExecute = $false
$startInfo.EnvironmentVariables['CLOCK_SAVER_DATA_DIR'] = $data

function Test-EntryPoint([string]$argument) {
    $startInfo.Arguments = $argument
    $process = [Diagnostics.Process]::Start($startInfo)
    try {
        if (-not $process.WaitForExit(45000)) {
            $process.Kill()
            throw "Clock.scr $argument timed out."
        }
        if ($process.ExitCode -ne 0) {
            $report = Join-Path $data 'selftest.txt'
            if ($argument -eq '/selftest' -and (Test-Path -LiteralPath $report)) {
                Get-Content -LiteralPath $report | Write-Host
            }
            throw "Clock.scr $argument exited with code $($process.ExitCode)."
        }
    } finally {
        $process.Dispose()
    }
}

try {
    Copy-Item -LiteralPath $source -Destination $copy
    Test-EntryPoint '/selftest'
    Get-Content -LiteralPath (Join-Path $data 'selftest.txt')
    Test-EntryPoint '/p:0'

    powershell.exe -NoProfile -STA -File (Join-Path $PSScriptRoot 'Test-Preview.ps1') -ScreenSaver $copy
    if ($LASTEXITCODE -ne 0) { throw 'Windows preview test failed.' }
} finally {
    $path = [IO.Path]::GetFullPath($test.FullName)
    $tempRoot = [IO.Path]::GetFullPath([IO.Path]::GetTempPath()).TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar
    if ($path.StartsWith($tempRoot, [StringComparison]::OrdinalIgnoreCase)) {
        Remove-Item -LiteralPath $path -Recurse -Force -ErrorAction SilentlyContinue
    }
}
