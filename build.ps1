#!/usr/bin/env pwsh
# Script de build pour BlackWoods Compta
# Usage: .\build.ps1

Write-Host "=== Build BlackWoods Compta ===" -ForegroundColor Cyan

# Restauration des packages NuGet
Write-Host "`n[1/2] Restauration des packages NuGet..." -ForegroundColor Yellow
dotnet restore BlackWoodsCompta/src/BlackWoodsCompta.sln

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n[ERREUR] La restauration des packages a echoue" -ForegroundColor Red
    exit 1
}

# Build du projet
Write-Host "`n[2/2] Compilation du projet..." -ForegroundColor Yellow
dotnet build BlackWoodsCompta/src/BlackWoodsCompta.sln --configuration Debug --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n[ERREUR] La compilation a echoue" -ForegroundColor Red
    exit 1
}

Write-Host "`n[OK] Build termine avec succes!" -ForegroundColor Green
Write-Host "Pour lancer l'application: .\run.ps1" -ForegroundColor Cyan
