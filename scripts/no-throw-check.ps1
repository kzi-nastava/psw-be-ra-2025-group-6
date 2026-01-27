param(
    [string]$RepoRoot = (Resolve-Path ".").Path
)

$ErrorActionPreference = "Stop"

function Require-Tool {
    param([string]$Name)
    if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
        Write-Error "$Name not found. Please install it or update the script."
        exit 2
    }
}

Require-Tool -Name "rg"

Set-Location $RepoRoot

$backendPath = Join-Path $RepoRoot "src"
$frontendPath = Join-Path $RepoRoot "src\\app"

Write-Host "Checking for 'throw' statements in backend: $backendPath"
$backendMatches = rg "\bthrow\b" $backendPath
if ($backendMatches) {
    Write-Error "Found 'throw' statements in backend code. Remove them before building."
    exit 1
}

if (Test-Path $frontendPath) {
    Write-Host "Checking for 'throw' statements in frontend: $frontendPath"
    $frontendMatches = rg "\bthrow\b" $frontendPath
    if ($frontendMatches) {
        Write-Error "Found 'throw' statements in frontend code. Remove them before building."
        exit 1
    }
} else {
    Write-Host "Frontend path not found; skipping frontend check."
}

Write-Host "No 'throw' statements found."
