$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 2.0

function Read-Json([string]$Path) {
    return (Get-Content -LiteralPath $Path -Raw -Encoding UTF8 | ConvertFrom-Json)
}
function Write-Utf8([string]$Path, [string]$Text) {
    [System.IO.File]::WriteAllText($Path, $Text, (New-Object System.Text.UTF8Encoding($false)))
}
function Resolve-GameData([string]$GameDataDir) {
    if ($GameDataDir) {
        $typed = $GameDataDir.Trim().Trim('"')
        if (Test-Path -LiteralPath $typed -PathType Leaf) { $typed = Split-Path -Parent $typed }
        if (-not (Test-Path -LiteralPath (Join-Path $typed 'sts2.dll'))) { throw 'sts2.dll not found in the supplied folder.' }
        return (Resolve-Path -LiteralPath $typed).Path
    }
    $steamRoots = @()
    try { $steamRoots += (Get-ItemProperty 'HKCU:\Software\Valve\Steam').SteamPath } catch {}
    if (${env:ProgramFiles(x86)}) { $steamRoots += Join-Path ${env:ProgramFiles(x86)} 'Steam' }
    $libraries = @($steamRoots)
    foreach ($steam in $steamRoots) {
        $vdf = Join-Path $steam 'steamapps\libraryfolders.vdf'
        if (Test-Path -LiteralPath $vdf) {
            foreach ($match in [regex]::Matches((Get-Content -LiteralPath $vdf -Raw -Encoding UTF8), '"path"\s+"([^"]+)"')) {
                $libraries += $match.Groups[1].Value.Replace('\\','\')
            }
        }
    }
    $candidates = @()
    foreach ($lib in @($libraries | Where-Object { $_ } | Select-Object -Unique)) {
        $game = Join-Path $lib 'steamapps\common\Slay the Spire 2'
        if (Test-Path -LiteralPath $game) {
            $candidates += @(Get-ChildItem -LiteralPath $game -Filter sts2.dll -File -Recurse | ForEach-Object { $_.DirectoryName })
        }
    }
    $candidates = @($candidates | Select-Object -Unique)
    if ($candidates.Count -eq 1) { return $candidates[0] }
    if ($candidates.Count -gt 1) {
        for ($i = 0; $i -lt $candidates.Count; $i++) { Write-Host "[$i] $($candidates[$i])" }
        $index = 0
        $answer = Read-Host 'Choose the ACTIVE Beta installation number'
        if ([int]::TryParse($answer, [ref]$index) -and $index -ge 0 -and $index -lt $candidates.Count) { return $candidates[$index] }
    }
    return Resolve-GameData (Read-Host 'Paste the folder containing sts2.dll (or its full file path)')
}
function Assert-Assets([string]$Root) {
    $record = Read-Json (Join-Path $Root 'assets\asset-build.json')
    $pck = Join-Path $Root 'assets\CraftTheSpire.pck'
    if (-not (Test-Path -LiteralPath $pck)) { throw 'The prepared asset PCK is missing. Run PackAssets.cmd with Godot 4.5.1.' }
    if ((Get-FileHash -LiteralPath $pck -Algorithm SHA256).Hash -ne $record.pckSha256) { throw 'Asset PCK checksum mismatch.' }
    foreach ($source in $record.sources) {
        $path = Join-Path $Root $source.path
        if (-not (Test-Path -LiteralPath $path) -or (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash -ne $source.sha256) {
            throw "Asset changed or missing: $($source.path). Rebuild the PCK with PackAssets.cmd before Build.cmd."
        }
    }
}
