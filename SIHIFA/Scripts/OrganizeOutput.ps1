param(
    [Parameter(Mandatory = $true)]
    [string]$TargetDir
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path $TargetDir)) {
    exit 0
}

$dllDir = Join-Path $TargetDir "components\dll"
if (-not (Test-Path $dllDir)) {
    New-Item -ItemType Directory -Path $dllDir -Force | Out-Null
}

# 1. Pindahkan semua file .dll, .pdb, .json, dan .xml yang berada di root ke components\dll
Get-ChildItem -Path $TargetDir -File | Where-Object { 
    $_.Extension -in '.dll', '.pdb', '.json', '.xml' 
} | ForEach-Object {
    $dest = Join-Path $dllDir $_.Name
    Move-Item -Path $_.FullName -Destination $dest -Force
}

# 2. Pindahkan semua folder satelit kultur (cs, ru, de, dll) serta folder runtimes ke components\dll
Get-ChildItem -Path $TargetDir -Directory | Where-Object { 
    $_.Name -notin @('components', 'publish') 
} | ForEach-Object {
    $destSub = Join-Path $dllDir $_.Name
    if (Test-Path $destSub) {
        Remove-Item -Path $destSub -Recurse -Force
    }
    Move-Item -Path $_.FullName -Destination $destSub -Force
}

Write-Host "[SIHIFA] Output tree successfully organized: /SIHIFA.exe and /components/dll/"
