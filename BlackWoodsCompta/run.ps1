#!/usr/bin/env pwsh
# Script de lancement pour BlackWoods Compta
# Usage: .\run.ps1

Write-Host "=== Lancement BlackWoods Compta ===" -ForegroundColor Cyan

# Vérifier que le projet a été build
$exePath = "src\BlackWoodsCompta.WPF\bin\Debug\net8.0-windows\BlackWoodsCompta.WPF.exe"
if (-not (Test-Path $exePath)) {
    Write-Host "`n[WARNING] L'application n'a pas encore ete compilee" -ForegroundColor Yellow
    Write-Host "Lancement du build automatique..." -ForegroundColor Yellow
    dotnet build src/BlackWoodsCompta.sln --configuration Debug
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "`n[ERREUR] Le build a echoue" -ForegroundColor Red
        exit 1
    }
}

# Lancer l'application
Write-Host "`n[OK] Demarrage de l'application..." -ForegroundColor Green
dotnet run --project src/BlackWoodsCompta.WPF/BlackWoodsCompta.WPF.csproj --no-build
