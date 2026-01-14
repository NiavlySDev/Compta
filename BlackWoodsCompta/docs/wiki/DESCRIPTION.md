# 📋 Description Générale du Projet

## 🏢 BlackWoods Compta

Application Windows de comptabilité professionnelle pour le restaurant BlackWoods dans un environnement GTA RP.

---

## 🎯 Objectif du Projet

Créer une application Windows native (.exe) complète de gestion comptable permettant de :
- Gérer les finances du restaurant (ventes, dépenses)
- Suivre les employés et leurs salaires
- Gérer l'inventaire et les stocks
- Générer des factures et rapports
- Assurer un suivi et une traçabilité complète

---

## 🏗️ Architecture Technique

### Stack Technologique

| Composant | Technologie | Version |
|-----------|-------------|---------|
| Framework | .NET | 8.0 |
| Interface | WPF | 8.0 |
| Base de données locale | SQLite | 3.x |
| Base de données externe | MySQL | 8.0+ |
| Design | Material Design | 5.0.0 |
| Logging | Serilog | 3.1.1 |

### Pattern Architectural : MVVM

```
┌─────────────────────────────────────────────┐
│               Application WPF                │
├─────────────────────────────────────────────┤
│  Views (XAML)                               │
│    ↕ Data Binding                           │
│  ViewModels                                 │
│    ↕ Commands/Properties                    │
│  Services                                   │
│    ├─ IDataService (Interface)             │
│    │   ├─ LocalDataService (SQLite)        │
│    │   └─ ApiDataService (REST API)        │
│    ├─ IAuthService                         │
│    └─ IApiService                          │
│    ↕                                        │
│  Models (Entities, DTOs, Enums)            │
└─────────────────────────────────────────────┘
         ↕                      ↕
    ┌──────────┐          ┌──────────┐
    │  SQLite  │          │  MySQL   │
    │  (Local) │          │  (API)   │
    └──────────┘          └──────────┘
```

---

## 📦 Structure du Projet

```
BlackWoodsCompta/
├── src/
│   ├── BlackWoodsCompta.WPF/              # Application principale
│   │   ├── Views/                         # Interfaces XAML (16 vues)
│   │   │   ├── LoginWindow.xaml
│   │   │   ├── MainWindow.xaml
│   │   │   ├── DatabaseSelectionWindow.xaml
│   │   │   ├── DashboardView.xaml
│   │   │   ├── TransactionsView.xaml
│   │   │   ├── EmployeesView.xaml
│   │   │   ├── PayrollsView.xaml
│   │   │   ├── InventoryView.xaml
│   │   │   ├── OrdersView.xaml
│   │   │   ├── SuppliersView.xaml
│   │   │   ├── PurchasePricesView.xaml
│   │   │   ├── SalePricesView.xaml
│   │   │   ├── ReimbursementsView.xaml
│   │   │   ├── InvoicesView.xaml
│   │   │   ├── CashRegisterView.xaml
│   │   │   ├── ReportsView.xaml
│   │   │   └── SettingsView.xaml
│   │   │
│   │   ├── ViewModels/                    # Logique de présentation
│   │   │   ├── ViewModelBase.cs
│   │   │   ├── LoginViewModel.cs
│   │   │   ├── MainViewModel.cs
│   │   │   ├── DashboardViewModel.cs
│   │   │   ├── TransactionsViewModel.cs
│   │   │   ├── EmployeesViewModel.cs
│   │   │   ├── InventoryViewModel.cs
│   │   │   ├── OrdersViewModel.cs
│   │   │   ├── SuppliersViewModel.cs
│   │   │   ├── PurchasePricesViewModel.cs
│   │   │   ├── SalePricesViewModel.cs
│   │   │   ├── ReimbursementsViewModel.cs
│   │   │   └── SettingsViewModel.cs
│   │   │
│   │   ├── Services/                      # Services métier
│   │   │   ├── IDataService.cs           # Interface principale
│   │   │   ├── LocalDataService.cs       # Implémentation SQLite
│   │   │   ├── ApiDataService.cs         # Implémentation REST API
│   │   │   ├── IApiService.cs
│   │   │   ├── ApiService.cs
│   │   │   ├── IAuthService.cs
│   │   │   └── AuthService.cs
│   │   │
│   │   ├── Helpers/                       # Utilitaires
│   │   │   ├── RelayCommand.cs
│   │   │   └── Converters.cs
│   │   │
│   │   ├── App.xaml                       # Point d'entrée
│   │   └── App.xaml.cs
│   │
│   └── BlackWoodsCompta.Models/           # Modèles partagés
│       ├── Entities/                      # Entités métier (15)
│       │   ├── User.cs
│       │   ├── Transaction.cs
│       │   ├── Employee.cs
│       │   ├── EmployeeReimbursement.cs
│       │   ├── Payroll.cs
│       │   ├── InventoryItem.cs
│       │   ├── InventoryMovement.cs
│       │   ├── Order.cs
│       │   ├── OrderItem.cs
│       │   ├── Supplier.cs
│       │   ├── PurchasePrice.cs
│       │   ├── SalePrice.cs
│       │   ├── Invoice.cs
│       │   ├── InvoiceItem.cs
│       │   └── AuditLog.cs
│       │
│       ├── DTOs/                          # Data Transfer Objects
│       │   ├── ApiResponse.cs
│       │   ├── AuthDtos.cs
│       │   └── DashboardDto.cs
│       │
│       └── Enums/                         # Énumérations
│           ├── UserRole.cs
│           ├── TransactionType.cs
│           ├── InventoryMovementType.cs
│           └── InvoiceStatus.cs
│
├── database/                               # Scripts SQL
│   ├── schema.sql                         # Schéma MySQL complet
│   ├── seed.sql                           # Données de test
│   └── seed_real_data.sql                 # Données réalistes
│
├── docs/                                   # Documentation
│   ├── wiki/                              # Wiki du projet
│   │   ├── INDEX.md                       # Page d'accueil
│   │   ├── PHILOSOPHIE.md                 # Principes de dev
│   │   ├── DESCRIPTION.md                 # Ce fichier
│   │   ├── CHANGEMENTS.md                 # Journal des modifs
│   │   └── PROBLEMES.md                   # Suivi des bugs
│   │
│   ├── user-manual.md                     # Manuel utilisateur
│   ├── technical-doc.md                   # Doc technique
│   ├── api-spec.md                        # Spécifications API
│   ├── ia.md                              # Prompts IA originaux
│   │
│   └── Exemple/                           # Exemples de données
│       ├── depenses.txt
│       ├── effectif.txt
│       ├── ingredients.txt
│       └── ...
│
├── README.md                               # Introduction
├── QUICK_START.md                         # Guide démarrage rapide
├── PROJECT_SUMMARY.md                     # Résumé du projet
├── LICENSE                                # Licence
├── CODE_OF_CONDUCT.md                     # Code de conduite
├── CONTRIBUTING.md                        # Guide de contribution
└── SECURITY.md                            # Politique de sécurité
```

---

## ✨ Fonctionnalités Principales

### 🔐 Authentification
- Écran de connexion sécurisé
- Gestion des rôles (Admin, Manager, Employé)
- Sessions utilisateur avec JWT
- Logs d'audit des actions

### 🗄️ Base de Données Hybride
- **Mode Local** : SQLite intégré, aucune configuration requise
- **Mode API** : Connexion à MySQL via API REST
- Sélection au démarrage via interface dédiée

### 💰 Gestion Financière
- **Transactions** : Enregistrement des ventes et dépenses
- **Catégorisation** : Classification automatique
- **Recherche et Filtres** : Par date, type, montant, catégorie
- **Dashboard** : Vue d'ensemble avec KPIs et graphiques

### 👥 Gestion des Ressources Humaines
- **Employés** : Fiches complètes avec informations RP
- **Paies** : Calcul et historique des salaires
- **Remboursements** : Suivi des remboursements employés

### 📦 Gestion des Stocks
- **Inventaire** : Matières premières et plats préparés
- **Mouvements** : Entrées, sorties, ajustements
- **Alertes** : Stock bas, dates de péremption
- **Fournisseurs** : Gestion des fournisseurs et commandes

### 💵 Tarification
- **Prix d'Achat** : Coûts des matières premières
- **Prix de Vente** : Tarifs des produits finis
- **Historique** : Évolution des prix dans le temps

### 🧾 Facturation
- Création et gestion de factures
- Génération de PDF
- Numérotation automatique
- Suivi des statuts (Brouillon, Envoyée, Payée, Annulée)

### 📊 Rapports et Analyses
- Tableau de bord avec métriques en temps réel
- Graphiques de tendances
- Rapports personnalisables
- Export Excel/PDF

### 💵 Point de Vente (Caisse)
- Interface rapide pour les ventes
- Calcul automatique de la monnaie
- Clôture de caisse

### ⚙️ Configuration
- Paramètres de l'application
- Gestion des utilisateurs
- Configuration de l'API

---

## 🔒 Sécurité

### Authentification
- Hashage des mots de passe avec BCrypt
- Tokens JWT pour les sessions
- Vérification des rôles et permissions

### Protection des Données
- Connexions HTTPS uniquement (mode API)
- Base locale chiffrée (SQLite)
- Validation des entrées côté client et serveur

### Audit
- Logging de toutes les actions critiques
- Traçabilité complète des modifications
- Table audit_logs dans la base de données

---

## 📊 Base de Données

### Tables Principales (15)

1. **users** - Utilisateurs du système
2. **transactions** - Transactions financières
3. **employees** - Employés du restaurant
4. **employee_reimbursements** - Remboursements employés
5. **payrolls** - Paies
6. **inventory** - Articles en stock
7. **inventory_movements** - Mouvements de stock
8. **orders** - Commandes fournisseurs
9. **order_items** - Détails des commandes
10. **suppliers** - Fournisseurs
11. **purchase_prices** - Prix d'achat des matières
12. **sale_prices** - Prix de vente des produits
13. **invoices** - Factures clients
14. **invoice_items** - Lignes de factures
15. **audit_logs** - Logs d'audit

### Relations
- Foreign Keys avec contraintes d'intégrité
- Index optimisés pour les performances
- Vue matérialisée pour le dashboard

---

## 🎨 Interface Utilisateur

### Design System
- **Material Design** avec MaterialDesignThemes
- Palette de couleurs cohérente
- Icônes Material Design Icons
- Animations fluides

### Navigation
- Menu latéral avec 10 sections principales
- Breadcrumb pour la navigation
- Raccourcis clavier

### Responsive
- Adaptation aux différentes résolutions
- Tailles de fenêtre minimales respectées

---

## 📦 Dépendances Principales

### NuGet Packages

| Package | Version | Usage |
|---------|---------|-------|
| MaterialDesignThemes | 5.0.0 | Interface Material Design |
| MaterialDesignColors | 3.0.0 | Palette de couleurs |
| Newtonsoft.Json | 13.0.3 | Sérialisation JSON |
| RestSharp | 111.2.0 | Client HTTP REST |
| BCrypt.Net-Next | 4.0.3 | Hashage mots de passe |
| LiveChartsCore | 2.0.0-rc2 | Graphiques |
| PdfSharp | 6.1.0 | Génération PDF |
| ClosedXML | 0.102.2 | Export Excel |
| Serilog | 3.1.1 | Logging |
| Microsoft.Extensions.DI | 8.0.0 | Injection dépendances |
| Microsoft.Data.Sqlite | 8.0.0 | Base de données locale |
| Dapper | 2.1.28 | Micro-ORM |

---

## 🚀 Installation & Démarrage

### Prérequis
- Windows 10/11
- .NET 8.0 SDK/Runtime

### Commandes de Base

**Restaurer les packages** :
```powershell
cd "C:\Users\NiavlyS\Documents\Coding\Windows\Compta\BlackWoodsCompta\src"
dotnet restore
```

**Compiler** :
```powershell
dotnet build --no-restore
```

**Lancer l'application** :
```powershell
cd "C:\Users\NiavlyS\Documents\Coding\Windows\Compta\BlackWoodsCompta\src\BlackWoodsCompta.WPF"
dotnet run
```

### Première Utilisation
1. Choisir le mode de base de données (Local/API)
2. Se connecter avec les identifiants par défaut
3. Explorer les différentes fonctionnalités

---

## 📈 État Actuel du Projet

### ✅ Complété
- Architecture MVVM complète
- 16 Views et ViewModels
- 7 Services fonctionnels
- Base de données SQLite locale
- Authentification et autorisation
- Dashboard avec KPIs
- Gestion des transactions
- Gestion des employés
- Gestion de l'inventaire
- Système de commandes
- Gestion des fournisseurs
- Tarification (achat/vente)
- Remboursements employés

### ⚠️ En Cours / À Améliorer
- Correction des warnings de compilation
- Ajout de logs exhaustifs
- Tests unitaires
- API REST backend (à développer séparément)
- Export PDF des rapports
- Notifications système

### 🔮 Fonctionnalités Futures
- Mode hors ligne avec synchronisation
- Multi-langue (FR/EN)
- Thème sombre/clair personnalisable
- Sauvegarde automatique cloud
- Application mobile companion

---

## 📝 Documentation Complémentaire

### Pour les Utilisateurs
- [Manuel d'utilisation](../user-manual.md)
- [Guide de démarrage rapide](../QUICK_START.md)

### Pour les Développeurs
- [Documentation technique](../technical-doc.md)
- [Spécifications API](../api-spec.md)
- [Philosophie de développement](PHILOSOPHIE.md)
- [Journal des changements](CHANGEMENTS.md)
- [Suivi des problèmes](PROBLEMES.md)

---

## 👥 Contribution

Ce projet suit les principes définis dans [PHILOSOPHIE.md](PHILOSOPHIE.md).

Pour contribuer :
1. Consulter le [Guide de contribution](../../CONTRIBUTING.md)
2. Lire la philosophie de développement
3. Respecter le workflow documenté
4. Documenter tous les changements

---

## 📄 Licence

© 2026 BlackWoods Restaurant - Tous droits réservés

---

*Dernière mise à jour : 14/01/2026*
*Version : 1.0*
