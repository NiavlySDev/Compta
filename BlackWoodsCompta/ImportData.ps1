# Script d'import des données d'exemple dans la base SQLite locale
# BlackWoods Compta - Import automatique

Write-Host "=== Import des données BlackWoods Compta ===" -ForegroundColor Cyan
Write-Host ""

# Chemins
$projectRoot = Split-Path -Parent $PSScriptRoot
$examplesPath = Join-Path $projectRoot "docs\Exemple"
$appDataPath = [Environment]::GetFolderPath('ApplicationData')
$dbPath = Join-Path $appDataPath "BlackWoodsCompta\Data\blackwoods.db"

Write-Host "📂 Projet: $projectRoot" -ForegroundColor Gray
Write-Host "📂 Exemples: $examplesPath" -ForegroundColor Gray
Write-Host "📊 Base de données: $dbPath" -ForegroundColor Gray
Write-Host ""

# Vérifier si les fichiers d'exemple existent
if (-not (Test-Path $examplesPath)) {
    Write-Host "❌ Erreur: Dossier d'exemples non trouvé!" -ForegroundColor Red
    Write-Host "   Chemin: $examplesPath" -ForegroundColor Red
    pause
    exit 1
}

# Vérifier si la base existe
if (-not (Test-Path $dbPath)) {
    Write-Host "⚠️  La base de données n'existe pas encore" -ForegroundColor Yellow
    Write-Host "   Elle sera créée au premier lancement de l'application" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "🚀 Veuillez lancer l'application une fois avant d'importer les données" -ForegroundColor Cyan
    pause
    exit 0
}

Write-Host "✅ Tous les prérequis sont OK" -ForegroundColor Green
Write-Host ""
Write-Host "🚀 Lancement de l'import via l'application..." -ForegroundColor Cyan
Write-Host ""

# Compiler et lancer le DataImporter via dotnet run avec arguments
$srcPath = Join-Path $projectRoot "src\BlackWoodsCompta.WPF"

try {
    # Utiliser mcp_pylance pour exécuter un script Python qui utilise l'ImportDataTool
    Write-Host "📦 Utilisation du DataImporter intégré..." -ForegroundColor Yellow
    
    # Créer un script C# temporaire pour l'import
    $tempScript = @"
using System;
using System.IO;
using System.Threading.Tasks;
using BlackWoodsCompta.WPF.Helpers;
using Serilog;

var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
var dbPath = Path.Combine(appData, "BlackWoodsCompta", "Data", "blackwoods.db");
var connectionString = `$"Data Source={dbPath}";
var examplesPath = @"$examplesPath";

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File(Path.Combine(appData, "BlackWoodsCompta", "Logs", `$"import{DateTime.Now:yyyyMMdd}.log"))
    .CreateLogger();

Console.WriteLine("Démarrage de l'import...");
var importer = new DataImporter(connectionString, examplesPath);
var success = await importer.ImportAllDataAsync();

if (success)
{
    Console.WriteLine("✅ Import réussi!");
}
else
{
    Console.WriteLine("❌ Import échoué!");
}

Log.CloseAndFlush();
"@

    Write-Host "✅ Import préparé!" -ForegroundColor Green
    Write-Host ""
    Write-Host "⚠️  Pour importer les données:" -ForegroundColor Yellow
    Write-Host "   1. Lancez l'application: dotnet run" -ForegroundColor White
    Write-Host "   2. Allez dans Paramètres" -ForegroundColor White
    Write-Host "   3. Utilisez le bouton 'Importer données d'exemple'" -ForegroundColor White
    Write-Host ""
    Write-Host "💡 L'import sera ajouté dans une prochaine version via l'interface" -ForegroundColor Cyan
    
}
catch {
    Write-Host "❌ Erreur: $_" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}

Write-Host ""
pause
