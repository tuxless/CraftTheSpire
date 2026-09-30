param([string]$Uploader)
. (Join-Path $PSScriptRoot 'Common.ps1')
$root = Split-Path -Parent $PSScriptRoot
try {
    $lastBuild = Read-Json (Join-Path $root 'build-status.json')
    if ($lastBuild.passed -ne $true) { throw 'The most recent Build.cmd did not pass. Resolve the build failure before uploading.' }
    $workspace = Join-Path $root 'dist\CraftTheSpire-beta'
    & (Join-Path $PSScriptRoot 'ValidateWorkshop.ps1') -Workspace $workspace -ForUpload
    $idPath = Join-Path $workspace 'mod_id.txt'
    $identityPath = Join-Path $workspace 'workshop-identity.json'
    $hadId = Test-Path -LiteralPath $idPath
    if ($hadId) {
        if (-not (Test-Path -LiteralPath $identityPath)) {
            throw 'An existing mod_id.txt has no CraftTheSpire ownership record. Do not reuse the SpireDraft ID or any other mod ID. Keep a backup and use a fresh workspace for the first upload.'
        }
        $identity = Read-Json $identityPath
        $existingId = (Get-Content -LiteralPath $idPath -Raw -Encoding UTF8).Trim()
        if ($identity.modId -ne 'CraftTheSpire' -or [string]$identity.workshopId -ne $existingId) {
            throw 'Workshop identity mismatch. This ID is not the one recorded for this CraftTheSpire workspace.'
        }
    } elseif (Test-Path -LiteralPath $identityPath) {
        throw 'mod_id.txt is missing from an existing CraftTheSpire workspace. Restore it from backup before updating.'
    }
    $config = Read-Json (Join-Path $workspace 'workshop.json')
    if ($config.visibility -eq 'public') {
        $manifest = Read-Json (Join-Path $workspace 'content\CraftTheSpire.json')
        if ($manifest.version -ne '1.0.0') { throw 'The document requires 1.0.0 for first public release. Follow docs/RELEASE.md.' }
        $evidence = Read-Json (Join-Path $workspace 'release-checks.json')
        foreach ($gate in @('startup','allContent','localization','singlePlayer','twoPlayers','fourPlayers','saveLoad','fullHand','upgrades','missingDependency','cleanSubscription')) {
            if ($evidence.$gate -ne $true) { throw "Required document release gate is not recorded as passed: $gate" }
        }
        if ($evidence.gameVersion -ne '0.111.0' -or $evidence.ritsuLib -ne '0.6.2') { throw 'Recorded test versions do not match the target.' }
        foreach ($pair in @(@('dllSha256','CraftTheSpire.dll'),@('pckSha256','CraftTheSpire.pck'))) {
            if ($evidence.($pair[0]) -ne (Get-FileHash -LiteralPath (Join-Path $workspace ('content\' + $pair[1])) -Algorithm SHA256).Hash) { throw 'Tests were recorded for different binaries. Retest the current workspace.' }
        }
    }
    if (-not $Uploader) { $Uploader = (Read-Host 'Paste the official ModUploader.exe full path').Trim().Trim('"') }
    if (-not (Test-Path -LiteralPath $Uploader -PathType Leaf)) { throw 'Official ModUploader.exe not found. Get it from megacrit/sts2-mod-uploader Releases.' }
    Write-Host "Uploading $workspace / visibility $($config.visibility)"
    Push-Location (Split-Path -Parent $Uploader)
    try {
        & $Uploader upload -w $workspace
        $uploaderExit = $LASTEXITCODE
        if (-not $hadId -and (Test-Path -LiteralPath $idPath)) {
            $createdId = (Get-Content -LiteralPath $idPath -Raw -Encoding UTF8).Trim()
            $parsedId = [uint64]0
            if (-not [uint64]::TryParse($createdId,[ref]$parsedId) -or $parsedId -eq 0) { throw 'The uploader wrote an invalid Workshop ID.' }
            $identity = [ordered]@{modId='CraftTheSpire';workshopId=$createdId;createdBy='official-uploader-first-upload'}
            Write-Utf8 $identityPath ($identity | ConvertTo-Json)
            Write-Host "New CraftTheSpire Workshop ID: $createdId"
        }
        if ($uploaderExit -ne 0) { throw 'Upload failed. Send mod-uploader.log from the uploader folder. Keep any newly created ID and its ownership record for retry.' }
    } finally { Pop-Location }
    Write-Host 'Keep this whole workspace, mod_id.txt and workshop-identity.json for updates. Check Steam branch labels on the Workshop page.'
} catch { Write-Host $_ -ForegroundColor Red; exit 1 }
