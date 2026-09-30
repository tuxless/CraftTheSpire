param([string]$GameDataDir)
. (Join-Path $PSScriptRoot 'Common.ps1')
$root = Split-Path -Parent $PSScriptRoot
try {
    Start-Transcript -Path (Join-Path $root 'build-log.txt') -Force | Out-Null
    Write-Utf8 (Join-Path $root 'build-status.json') '{"passed":false,"reason":"Build is incomplete or failed. Do not upload an older workspace as this build."}'
    Write-Host 'Target: public-beta 0.111.0 / RitsuLib 0.6.2. This script does NOT switch Steam branches.'
    $GameDataDir = Resolve-GameData $GameDataDir
    Write-Host "Actual game references: $GameDataDir"
    foreach ($name in @('sts2.dll','0Harmony.dll','SmartFormat.dll')) {
        if (-not (Test-Path -LiteralPath (Join-Path $GameDataDir $name))) { throw "Missing game reference: $name" }
    }
    $apiCheck = Read-Json (Join-Path $root 'docs\game-api-check.json')
    $actualGameHash = (Get-FileHash -LiteralPath (Join-Path $GameDataDir 'sts2.dll') -Algorithm SHA256).Hash
    if ($actualGameHash -ne $apiCheck.sts2Sha256) {
        throw 'The game DLL differs from the one inspected for this source package. Run CollectGameReferences.cmd and send the new ZIP for API review.'
    }
    Write-Host 'Game DLL matches the inspected API references.'
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { throw 'Install .NET 9 SDK. Runtime alone cannot compile the mod.' }
    $sdks = & dotnet --list-sdks
    if ($LASTEXITCODE -ne 0 -or @($sdks | Where-Object { $_ -match '^9\.' }).Count -eq 0) { throw '.NET 9 SDK is required.' }
    Assert-Assets $root
    Push-Location $root
    try {
        & dotnet run --project 'tests\CraftTheSpire.Tests.csproj' --configuration Release
        if ($LASTEXITCODE -ne 0) { throw 'C# rule tests failed. No new workspace was built.' }
        & dotnet build 'CraftTheSpire.csproj' --configuration Release "-p:GameDataDir=$GameDataDir" --output 'build\beta'
        if ($LASTEXITCODE -ne 0) { throw 'C# compilation failed. Send the first complete error and build-log.txt.' }
    } finally { Pop-Location }
    $workspace = Join-Path $root 'dist\CraftTheSpire-beta'
    $content = Join-Path $workspace 'content'
    New-Item -ItemType Directory -Path $content -Force | Out-Null
    Copy-Item -LiteralPath (Join-Path $root 'build\beta\CraftTheSpire.dll') -Destination (Join-Path $content 'CraftTheSpire.dll') -Force
    Copy-Item -LiteralPath (Join-Path $root 'assets\CraftTheSpire.pck') -Destination (Join-Path $content 'CraftTheSpire.pck') -Force
    Copy-Item -LiteralPath (Join-Path $root 'CraftTheSpire.json') -Destination (Join-Path $content 'CraftTheSpire.json') -Force
    if (-not (Test-Path -LiteralPath (Join-Path $workspace 'workshop.json'))) {
        Copy-Item -LiteralPath (Join-Path $root 'workshop\workshop.json') -Destination (Join-Path $workspace 'workshop.json')
    }
    Copy-Item -LiteralPath (Join-Path $root 'workshop\image.png') -Destination (Join-Path $workspace 'image.png') -Force
    $report = [ordered]@{
        targetBranch = 'public-beta'; targetGameVersion = '0.111.0'; ritsuLib = '0.6.2';
        createdUtc = [DateTime]::UtcNow.ToString('o'); actualGameDataDir = $GameDataDir;
        gameSha256 = $actualGameHash;
        dllSha256 = (Get-FileHash -LiteralPath (Join-Path $content 'CraftTheSpire.dll') -Algorithm SHA256).Hash;
        pckSha256 = (Get-FileHash -LiteralPath (Join-Path $content 'CraftTheSpire.pck') -Algorithm SHA256).Hash;
        ruleTests = 'passed'; compilation = 'passed'; actualGameVersion = 'verify in game/log'; gameplay = 'not yet tested'; multiplayer = 'not yet tested'; saveLoad = 'not yet tested'
    }
    Write-Utf8 (Join-Path $workspace 'build-report.json') ($report | ConvertTo-Json -Depth 8)
    & (Join-Path $PSScriptRoot 'ValidateWorkshop.ps1') -Workspace $workspace
    Write-Utf8 (Join-Path $root 'build-status.json') '{"passed":true}'
    Write-Host "BUILD PASSED. Workspace: $workspace"
    Write-Host 'Copy ONLY the 3 files in content/ to the active game mods/CraftTheSpire/ folder. Install RitsuLib separately.'
    Write-Host 'Compilation and workspace validation do not prove gameplay or multiplayer compatibility.'
} catch { Write-Host $_ -ForegroundColor Red; exit 1 }
finally { Stop-Transcript -ErrorAction SilentlyContinue | Out-Null }
