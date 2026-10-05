# Serves the interactive reveal.js deck on http://localhost:8083
param([int]$Port = 8083, [switch]$NoBrowser)

$deck = Join-Path $PSScriptRoot 'deck'

if (-not (Test-Path (Join-Path $deck 'node_modules/reveal.js'))) {
    Write-Host 'Installing deck dependencies (first run only)...' -ForegroundColor Yellow
    Push-Location $deck; npm install; Pop-Location
}

Write-Host "Serving the deck on http://localhost:$Port/index.html" -ForegroundColor Cyan
Write-Host '  ?  keyboard help     /  jump to a slide     B  run demo     S  speaker notes' -ForegroundColor DarkGray

Push-Location $deck
try {
    if ($NoBrowser) { npx http-server . -p $Port -c-1 }
    else { npx http-server . -p $Port -c-1 -o /index.html }
} finally { Pop-Location }
