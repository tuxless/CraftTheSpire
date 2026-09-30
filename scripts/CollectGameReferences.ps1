param([string]$GameDataDir)
. (Join-Path $PSScriptRoot 'Common.ps1')
$root = Split-Path -Parent $PSScriptRoot
try {
    $GameDataDir = Resolve-GameData $GameDataDir
    Write-Host "References from: $GameDataDir. Ensure Steam is on public-beta 0.111.0."
    $temp = Join-Path $root ('reference-export-' + [Guid]::NewGuid().ToString('N'))
    New-Item -ItemType Directory -Path $temp | Out-Null
    try {
        foreach ($name in @('sts2.dll','0Harmony.dll','GodotSharp.dll','SmartFormat.dll')) {
            $source = Join-Path $GameDataDir $name
            if (-not (Test-Path -LiteralPath $source)) { throw "Missing $name in this installation." }
            Copy-Item -LiteralPath $source -Destination (Join-Path $temp $name)
        }
        $info = [ordered]@{target='public-beta 0.111.0';gameDataDir=$GameDataDir;gameSha256=(Get-FileHash -LiteralPath (Join-Path $temp 'sts2.dll') -Algorithm SHA256).Hash}
        Write-Utf8 (Join-Path $temp 'reference-info.json') ($info | ConvertTo-Json)
        $zip = Join-Path $root ('CraftTheSpire-GameReferences-beta-' + [DateTime]::Now.ToString('yyyyMMdd-HHmmss') + '.zip')
        Compress-Archive -Path (Join-Path $temp '*') -DestinationPath $zip
        Write-Host "Attach this ZIP for API inspection: $zip"
        Write-Host 'Only compile references are collected. Saves, logs, credentials and account data are not collected. Never upload this ZIP to Workshop.'
    } finally { Remove-Item -LiteralPath $temp -Recurse -Force }
} catch { Write-Host $_ -ForegroundColor Red; exit 1 }
