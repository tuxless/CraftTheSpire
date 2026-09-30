param([string]$Author)
. (Join-Path $PSScriptRoot 'Common.ps1')
$root = Split-Path -Parent $PSScriptRoot
try {
    if (-not $Author) { $Author = Read-Host 'Enter the author name shown in the mod list' }
    if ([string]::IsNullOrWhiteSpace($Author)) { throw 'Author cannot be empty.' }
    foreach ($path in @((Join-Path $root 'CraftTheSpire.json'),(Join-Path $root 'workshop\content\CraftTheSpire.json'))) {
        $manifest = Read-Json $path
        $manifest.author = $Author.Trim()
        Write-Utf8 $path ($manifest | ConvertTo-Json -Depth 8)
    }
    Write-Host 'Author saved. Run Build.cmd to apply it to the upload workspace.'
} catch { Write-Host $_ -ForegroundColor Red; exit 1 }
