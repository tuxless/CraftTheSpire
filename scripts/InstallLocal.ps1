param([string]$GameDataDir)
. (Join-Path $PSScriptRoot 'Common.ps1')
$root = Split-Path -Parent $PSScriptRoot
try {
    $workspace = Join-Path $root 'dist\CraftTheSpire-beta'
    & (Join-Path $PSScriptRoot 'ValidateWorkshop.ps1') -Workspace $workspace
    $GameDataDir = Resolve-GameData $GameDataDir
    $gameRoot = Split-Path -Parent $GameDataDir
    if (-not (Test-Path -LiteralPath (Join-Path $gameRoot 'steam_appid.txt')) -and (Split-Path -Leaf $GameDataDir) -ne 'data_sts2_windows_x86_64') {
        throw 'Could not verify the game root. Copy the 3 content files manually as described in README.zh-CN.md.'
    }
    $destination = Join-Path $gameRoot 'mods\CraftTheSpire'
    New-Item -ItemType Directory -Path $destination -Force | Out-Null
    foreach ($name in @('CraftTheSpire.dll','CraftTheSpire.pck','CraftTheSpire.json')) {
        Copy-Item -LiteralPath (Join-Path $workspace ('content\' + $name)) -Destination (Join-Path $destination $name) -Force
    }
    Write-Host "Installed: $destination. Start the matching Beta game WITH MODS. RitsuLib must be installed separately."
    Write-Host 'Keep only one copy: local or subscribed Workshop. Do not load duplicate mod IDs.'
} catch { Write-Host $_ -ForegroundColor Red; exit 1 }
