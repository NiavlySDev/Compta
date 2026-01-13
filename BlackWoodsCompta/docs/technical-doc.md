# Documentation Technique - BlackWoods Compta

## Architecture

### Vue d'ensemble

BlackWoods Compta est une application client-serveur composée de:
- **Client WPF** (.NET 8): Application Windows de bureau
- **API REST**: Serveur backend (à développer séparément)
- **Base de données MySQL**: Stockage des données

```
┌─────────────────┐
│   Client WPF    │
│   (.NET 8)      │
└────────┬────────┘
         │ HTTPS
         │ REST API
         │ JWT Auth
┌────────▼────────┐
│   API Server    │
│   (à créer)     │
└────────┬────────┘
         │
         │ MySQL
┌────────▼────────┐
│   MySQL DB      │
│   (8.0+)        │
└─────────────────┘
```

### Pattern Architectural

L'application client utilise le pattern **MVVM (Model-View-ViewModel)**:
- **Models**: Classes de données (dans BlackWoodsCompta.Models)
- **Views**: Interfaces utilisateur XAML
- **ViewModels**: Logique de présentation et binding

---

## Structure du Projet

```
BlackWoodsCompta/
├── src/
│   ├── BlackWoodsCompta.WPF/          # Application principale
│   │   ├── Views/                      # Vues XAML
│   │   │   ├── LoginWindow.xaml
│   │   │   ├── MainWindow.xaml
│   │   │   ├── DashboardView.xaml
│   │   │   ├── TransactionsView.xaml
│   │   │   ├── EmployeesView.xaml
│   │   │   ├── PayrollsView.xaml
│   │   │   ├── InventoryView.xaml
│   │   │   ├── InvoicesView.xaml
│   │   │   ├── CashRegisterView.xaml
│   │   │   ├── ReportsView.xaml
│   │   │   └── SettingsView.xaml
│   │   │
│   │   ├── ViewModels/                 # ViewModels
│   │   │   ├── ViewModelBase.cs
│   │   │   ├── LoginViewModel.cs
│   │   │   ├── MainViewModel.cs
│   │   │   ├── DashboardViewModel.cs
│   │   │   ├── TransactionsViewModel.cs
│   │   │   └── ...
│   │   │
│   │   ├── Services/                   # Services
│   │   │   ├── IApiService.cs
│   │   │   ├── ApiService.cs
│   │   │   ├── IAuthService.cs
│   │   │   └── AuthService.cs
│   │   │
│   │   ├── Helpers/                    # Classes utilitaires
│   │   │   ├── RelayCommand.cs
│   │   │   └── Converters.cs
│   │   │
│   │   ├── Resources/                  # Ressources
│   │   │   ├── blackwoods.ico
│   │   │   └── Images/
│   │   │
│   │   ├── App.xaml                    # Point d'entrée
│   │   └── App.xaml.cs
│   │
│   └── BlackWoodsCompta.Models/        # Modèles partagés
│       ├── Entities/                    # Entités
│       │   ├── User.cs
│       │   ├── Transaction.cs
│       │   ├── Employee.cs
│       │   └── ...
│       │
│       ├── DTOs/                        # Data Transfer Objects
│       │   ├── AuthDtos.cs
│       │   ├── ApiResponse.cs
│       │   └── DashboardDto.cs
│       │
│       └── Enums/                       # Énumérations
│           ├── UserRole.cs
│           ├── TransactionType.cs
│           └── ...
│
├── database/
│   ├── schema.sql                      # Schéma de base de données
│   └── seed.sql                        # Données de test
│
├── docs/
│   ├── user-manual.md                  # Manuel utilisateur
│   ├── technical-doc.md                # Cette documentation
│   └── api-spec.md                     # Spécification API
│
└── README.md
```

---

## Technologies Utilisées

### Frontend (Client WPF)

| Technologie | Version | Usage |
|------------|---------|-------|
| .NET | 8.0 | Framework principal |
| WPF | 8.0 | Interface utilisateur |
| MaterialDesignThemes | 5.0.0 | Design Material |
| RestSharp | 111.2.0 | Client HTTP pour API |
| Newtonsoft.Json | 13.0.3 | Sérialisation JSON |
| BCrypt.Net-Next | 4.0.3 | Hashage de mots de passe |
| LiveChartsCore | 2.0.0 | Graphiques |
| PdfSharp | 6.1.0 | Génération de PDF |
| ClosedXML | 0.102.2 | Export Excel |
| Serilog | 3.1.1 | Logging |
| Microsoft.Extensions.DependencyInjection | 8.0.0 | Injection de dépendances |

### Backend (À implémenter)

Technologies recommandées:
- **Framework**: ASP.NET Core 8.0 / Node.js / Python FastAPI
- **ORM**: Entity Framework Core / Sequelize / SQLAlchemy
- **Auth**: JWT avec bibliothèque adaptée
- **Validation**: FluentValidation / Joi / Pydantic

### Base de données

- **MySQL 8.0+**
- **Storage Engine**: InnoDB
- **Character Set**: utf8mb4
- **Collation**: utf8mb4_unicode_ci

---

## Services

### ApiService

Service de communication avec l'API REST.

**Interface**: `IApiService`

```csharp
public interface IApiService
{
    string BaseUrl { get; set; }
    string? Token { get; set; }
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, object data);
    Task<T?> PutAsync<T>(string endpoint, object data);
    Task<bool> DeleteAsync(string endpoint);
}
```

**Implémentation**: `ApiService`
- Utilise RestSharp pour les appels HTTP
- Gère automatiquement l'ajout du token d'authentification
- Log les erreurs via Serilog

### AuthService

Service de gestion de l'authentification.

**Interface**: `IAuthService`

```csharp
public interface IAuthService
{
    User? CurrentUser { get; }
    bool IsAuthenticated { get; }
    string? Token { get; }
    Task<LoginResponse> LoginAsync(string username, string password);
    void Logout();
    bool HasRole(string role);
}
```

**Implémentation**: `AuthService`
- Stocke l'utilisateur courant en mémoire
- Gère le token JWT
- Vérifie les rôles pour les autorisations

---

## ViewModels

Tous les ViewModels héritent de `ViewModelBase` qui implémente `INotifyPropertyChanged`.

### Pattern de base

```csharp
public class ExampleViewModel : ViewModelBase
{
    private string _myProperty;
    
    public string MyProperty
    {
        get => _myProperty;
        set => SetProperty(ref _myProperty, value);
    }
    
    public ICommand MyCommand { get; }
    
    public ExampleViewModel()
    {
        MyCommand = new RelayCommand(ExecuteMyCommand);
    }
    
    private void ExecuteMyCommand(object? parameter)
    {
        // Logic here
    }
}
```

### Commandes

Deux types de commandes sont disponibles:
- **RelayCommand**: Pour les opérations synchrones
- **AsyncRelayCommand**: Pour les opérations asynchrones (appels API)

---

## Communication avec l'API

### Format des requêtes

Toutes les requêtes incluent:
- **Content-Type**: `application/json`
- **Authorization**: `Bearer {token}` (sauf pour login)

### Format des réponses

Réponse standard:
```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}
```

### Gestion des erreurs

1. Les erreurs HTTP sont loggées via Serilog
2. Les erreurs sont affichées à l'utilisateur via des messages
3. Les exceptions sont catchées dans les ViewModels

---

## Logging

### Configuration Serilog

```csharp
var logPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    "BlackWoodsCompta",
    "Logs",
    "app.log"
);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

### Emplacement des logs

```
%AppData%\BlackWoodsCompta\Logs\app-YYYYMMDD.log
```

### Niveaux de log

- **Debug**: Informations détaillées de développement
- **Information**: Événements généraux (connexion, actions)
- **Warning**: Événements inhabituels mais non critiques
- **Error**: Erreurs nécessitant attention
- **Fatal**: Erreurs critiques causant l'arrêt

---

## Injection de Dépendances

L'application utilise Microsoft.Extensions.DependencyInjection.

### Configuration dans App.xaml.cs

```csharp
private void ConfigureServices(IServiceCollection services)
{
    // Services
    services.AddSingleton<IApiService, ApiService>();
    services.AddSingleton<IAuthService, AuthService>();

    // ViewModels
    services.AddTransient<LoginViewModel>();
    services.AddTransient<MainViewModel>();
    // ...

    // Views
    services.AddTransient<LoginWindow>();
    services.AddTransient<MainWindow>();
    // ...
}
```

### Résolution des dépendances

```csharp
var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
loginWindow.Show();
```

---

## Base de Données

### Schéma

Voir `database/schema.sql` pour le schéma complet.

### Tables principales

- **users**: Utilisateurs de l'application
- **transactions**: Transactions financières
- **employees**: Employés du restaurant
- **payrolls**: Paies versées
- **inventory**: Articles en stock
- **inventory_movements**: Mouvements de stock
- **invoices**: Factures émises
- **invoice_items**: Lignes de facture
- **audit_logs**: Journaux d'audit

### Relations

```
users ←──── transactions
users ←──── payrolls
users ←──── inventory_movements
users ←──── invoices
users ←──── audit_logs

employees ←──── payrolls

inventory ←──── inventory_movements

invoices ←──── invoice_items
```

### Indexes

Des index sont créés sur:
- Clés étrangères
- Champs de recherche fréquents (username, product_name, etc.)
- Champs de tri (created_at, dates, etc.)

---

## Sécurité

### Authentification

- JWT (JSON Web Tokens)
- Expiration: 8 heures
- Stockage en mémoire uniquement (pas de persistance locale)

### Mots de passe

- Hashage avec BCrypt (cost factor: 11)
- Jamais stockés en clair
- Validation côté serveur

### Autorisations

Vérification des rôles avant chaque opération sensible:

```csharp
if (!_authService.HasRole("Admin"))
{
    // Refuser l'accès
}
```

### Communication

- HTTPS obligatoire en production
- Validation de tous les inputs
- Protection contre les injections SQL (via paramètres)

---

## Build et Déploiement

### Prérequis de développement

- Visual Studio 2022 ou supérieur
- .NET 8.0 SDK
- WiX Toolset v4 (pour l'installateur)

### Build en mode Debug

```powershell
cd src
dotnet restore
dotnet build
```

### Build en mode Release

```powershell
dotnet build --configuration Release
```

### Exécution

```powershell
cd src/BlackWoodsCompta.WPF
dotnet run
```

### Création de l'installateur

TODO: Configuration WiX à compléter

---

## Tests

### Tests unitaires (À implémenter)

Structure recommandée:
```
tests/
├── BlackWoodsCompta.Tests/
│   ├── ViewModels/
│   ├── Services/
│   └── Helpers/
```

Framework recommandé: xUnit ou NUnit

### Tests d'intégration (À implémenter)

- Tests des appels API
- Tests de l'authentification
- Tests des ViewModels avec services mockés

---

## Performance

### Optimisations

1. **Pagination**: Les listes sont limitées à 50 éléments par page
2. **Lazy Loading**: Les données sont chargées à la demande
3. **Cache**: Les données fréquemment utilisées sont cachées
4. **Async/Await**: Toutes les opérations longues sont asynchrones

### Métriques

Temps de chargement cibles:
- Écran de connexion: < 1s
- Dashboard: < 2s
- Listes: < 1.5s
- Création d'enregistrement: < 1s

---

## Maintenance

### Logs à surveiller

- Erreurs de connexion API
- Échecs d'authentification répétés
- Erreurs lors des sauvegardes
- Exceptions non gérées

### Mises à jour

1. Vérifier les mises à jour des packages NuGet
2. Tester en environnement de développement
3. Créer un nouveau build
4. Générer un nouvel installateur
5. Déployer auprès des utilisateurs

### Sauvegarde

Les données sont sur le serveur MySQL.
Plan de sauvegarde recommandé:
- Quotidien: Sauvegarde complète
- Horaire: Sauvegarde incrémentale
- Conservation: 30 jours

---

## API Backend (À développer)

### Technologies recommandées

#### Option 1: ASP.NET Core
```csharp
// Program.cs
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { ... });
builder.Services.AddControllers();

// Controller example
[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<Transaction>>>> GetTransactions()
    {
        // Implementation
    }
}
```

#### Option 2: Node.js + Express
```javascript
// server.js
const express = require('express');
const jwt = require('jsonwebtoken');
const mysql = require('mysql2/promise');

app.post('/api/auth/login', async (req, res) => {
    // Implementation
});

app.get('/api/transactions', authenticate, async (req, res) => {
    // Implementation
});
```

#### Option 3: Python + FastAPI
```python
# main.py
from fastapi import FastAPI, Depends
from sqlalchemy.orm import Session

@app.post("/api/auth/login")
async def login(credentials: LoginRequest, db: Session = Depends(get_db)):
    # Implementation
    pass

@app.get("/api/transactions")
async def get_transactions(
    current_user: User = Depends(get_current_user),
    db: Session = Depends(get_db)
):
    # Implementation
    pass
```

### Endpoints à implémenter

Voir `docs/api-spec.md` pour la spécification complète de tous les endpoints.

---

## Troubleshooting

### Problème: L'application ne démarre pas

**Cause possible**: .NET 8 Runtime non installé

**Solution**:
1. Télécharger .NET 8 Desktop Runtime
2. Réinstaller l'application

### Problème: Erreur "Cannot connect to API"

**Causes possibles**:
- Serveur API non démarré
- URL incorrecte
- Firewall bloquant la connexion

**Solutions**:
1. Vérifier que l'API est accessible
2. Tester l'URL dans un navigateur
3. Vérifier les paramètres réseau

### Problème: Données ne se chargent pas

**Causes possibles**:
- Token expiré
- Problème réseau
- Erreur serveur

**Solutions**:
1. Se déconnecter et se reconnecter
2. Vérifier les logs
3. Contacter l'administrateur

---

## Roadmap

### Version 1.1 (Future)
- [ ] Mode hors ligne avec synchronisation
- [ ] Export PDF personnalisé
- [ ] Graphiques avancés
- [ ] Module de réservations

### Version 1.2 (Future)
- [ ] Application mobile (iOS/Android)
- [ ] Intégration avec systèmes de paiement
- [ ] Module de fidélité client
- [ ] BI et analytics avancés

---

## Contact et Support

**Développeur Principal**: [Votre nom]  
**Email**: dev@blackwoods.com  
**Repository**: [URL du repo Git si applicable]  

**Documentation en ligne**: https://docs.blackwoods.com  
**Issues**: [URL du système de tickets]

---

**BlackWoods Compta - Documentation Technique v1.0.0**  
Dernière mise à jour: 13 janvier 2026
