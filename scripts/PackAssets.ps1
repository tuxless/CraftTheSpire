param([string]$Godot)
. (Join-Path $PSScriptRoot 'Common.ps1')
$root = Split-Path -Parent $PSScriptRoot
try {
    if (-not $Godot) { $Godot = (Read-Host 'Paste Godot 4.5.1 console executable full path').Trim().Trim('"') }
    if (-not (Test-Path -LiteralPath $Godot -PathType Leaf)) { throw 'Godot executable not found.' }
    $version = & $Godot --headless --version
    if ($LASTEXITCODE -ne 0 -or ($version -join ' ') -notmatch '4\.5\.1') { throw 'Use Godot 4.5.1 for this frozen asset build.' }
    & $Godot --headless --path $root --editor --import
    if ($LASTEXITCODE -ne 0) { throw 'Godot asset import failed.' }
    & $Godot --headless --path $root --script (Join-Path $root 'scripts\pack_assets.gd')
    if ($LASTEXITCODE -ne 0) { throw 'Godot asset packing failed.' }
    $sources = @()
    foreach ($file in @(Get-ChildItem -LiteralPath (Join-Path $root 'CraftTheSpire') -File -Recurse | Where-Object { $_.Extension -in @('.png','.json') } | Sort-Object FullName)) {
        $sources += [ordered]@{path=$file.FullName.Substring($root.Length+1).Replace('\','/');sha256=(Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash}
    }
    $record = [ordered]@{engine='4.5.1';pckSha256=(Get-FileHash -LiteralPath (Join-Path $root 'assets\CraftTheSpire.pck') -Algorithm SHA256).Hash;sources=$sources}
    Write-Utf8 (Join-Path $root 'assets\asset-build.json') ($record | ConvertTo-Json -Depth 8)
    Assert-Assets $root
    Write-Host 'Asset PCK rebuilt. Run Build.cmd to copy it into the workspace. Verify assets in game.'
} catch { Write-Host $_ -ForegroundColor Red; exit 1 }
