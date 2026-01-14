# 🎯 PROMPT FINAL POUR APPLICATION DE COMPTABILITÉ BLACKWOODS

> ⚠️ **IMPORTANT** : Ce fichier contient les prompts initiaux du projet.  
> Pour la documentation à jour et organisée, consultez le **[Wiki du projet](wiki/INDEX.md)** 📚

---

## 📚 Navigation Rapide vers le Wiki

- 🏠 **[Accueil du Wiki](wiki/INDEX.md)** - Page d'accueil
- 🎯 **[Philosophie de Développement](wiki/PHILOSOPHIE.md)** - Principes et méthodologie
- 📋 **[Description Générale](wiki/DESCRIPTION.md)** - Architecture et fonctionnalités
- 🔄 **[Journal des Changements](wiki/CHANGEMENTS.md)** - Historique des modifications
- 🐛 **[Problèmes & Résolutions](wiki/PROBLEMES.md)** - Suivi des bugs

---

# 🎯 PROMPT INITIAL (Historique)

## Contexte
Créer une application Windows native (.exe) de comptabilité pour le restaurant "BlackWoods" dans un contexte GTA RP, avec un installateur professionnel.

## Stack Technique
- **Framework** : C# avec WPF (.NET 8)
- **Base de données** : MySQL (connexion via API REST)
- **Architecture** : Client WPF + API REST (à définir)
- **Installateur** : WiX Toolset (.msi)
- **Authentification** : Système de login avec suivi des actions utilisateurs

## Fonctionnalités Complètes

### 1. Authentification & Utilisateurs
- Écran de connexion (username/password)
- Gestion des rôles (Admin, Manager, Employé)
- Logs d'actions par utilisateur
- Session management

### 2. Gestion des Transactions
- Enregistrement des ventes (avec montant en $)
- Enregistrement des dépenses
- Catégorisation (Nourriture, Boissons, Salaires, Fournitures, etc.)
- Recherche et filtres par date, type, montant

### 3. Gestion des Employés
- Fiche employé (nom, poste, salaire)
- Calcul et gestion des paies
- Historique des paiements
- Suivi des heures/présences

### 4. Gestion des Stocks/Inventaire
- Liste des produits (ingrédients, boissons)
- Quantités en stock
- Alertes stock bas
- Historique des mouvements de stock
- Coût des produits

### 5. Factures & Devis
- Création de factures clients
- Génération de devis
- Export PDF
- Numérotation automatique
- Historique des factures

### 6. Rapports & Statistiques
- Tableau de bord avec KPIs
- Graphiques (revenus, dépenses, bénéfices)
- Rapports période (jour, semaine, mois, année)
- Export Excel/PDF
- Analyse de rentabilité

### 7. Caisse
- Interface de caisse rapide
- Calcul automatique de la monnaie
- Impression de tickets
- Clôture de caisse journalière

## Structure API REST à créer

```
POST   /api/auth/login
GET    /api/transactions
POST   /api/transactions
PUT    /api/transactions/{id}
DELETE /api/transactions/{id}
GET    /api/employees
POST   /api/employees
PUT    /api/employees/{id}
DELETE /api/employees/{id}
GET    /api/payrolls
POST   /api/payrolls
GET    /api/inventory
POST   /api/inventory
PUT    /api/inventory/{id}
DELETE /api/inventory/{id}
GET    /api/inventory/movements
POST   /api/inventory/movements
GET    /api/invoices
POST   /api/invoices
PUT    /api/invoices/{id}
DELETE /api/invoices/{id}
GET    /api/reports/dashboard
GET    /api/reports/period
GET    /api/logs
```

## Structure Base de Données MySQL

### Table `users`
```sql
CREATE TABLE users (
    id INT PRIMARY KEY AUTO_INCREMENT,
    username VARCHAR(50) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role ENUM('Admin', 'Manager', 'Employé') NOT NULL,
    full_name VARCHAR(100),
    email VARCHAR(100),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);
```

### Table `transactions`
```sql
CREATE TABLE transactions (
    id INT PRIMARY KEY AUTO_INCREMENT,
    type ENUM('Vente', 'Dépense') NOT NULL,
    category VARCHAR(50) NOT NULL,
    amount DECIMAL(10, 2) NOT NULL,
    description TEXT,
    reference VARCHAR(50),
    user_id INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id)
);
```

### Table `employees`
```sql
CREATE TABLE employees (
    id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(100) NOT NULL,
    position VARCHAR(50) NOT NULL,
    salary DECIMAL(10, 2) NOT NULL,
    hire_date DATE NOT NULL,
    phone VARCHAR(20),
    email VARCHAR(100),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);
```

### Table `payrolls`
```sql
CREATE TABLE payrolls (
    id INT PRIMARY KEY AUTO_INCREMENT,
    employee_id INT NOT NULL,
    amount DECIMAL(10, 2) NOT NULL,
    period_start DATE NOT NULL,
    period_end DATE NOT NULL,
    paid_date DATE NOT NULL,
    notes TEXT,
    created_by INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (employee_id) REFERENCES employees(id),
    FOREIGN KEY (created_by) REFERENCES users(id)
);
```

### Table `inventory`
```sql
CREATE TABLE inventory (
    id INT PRIMARY KEY AUTO_INCREMENT,
    product_name VARCHAR(100) NOT NULL,
    category VARCHAR(50) NOT NULL,
    quantity DECIMAL(10, 2) NOT NULL,
    unit VARCHAR(20) NOT NULL,
    unit_cost DECIMAL(10, 2) NOT NULL,
    min_quantity DECIMAL(10, 2) DEFAULT 0,
    supplier VARCHAR(100),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);
```

### Table `inventory_movements`
```sql
CREATE TABLE inventory_movements (
    id INT PRIMARY KEY AUTO_INCREMENT,
    product_id INT NOT NULL,
    quantity DECIMAL(10, 2) NOT NULL,
    type ENUM('Entrée', 'Sortie', 'Ajustement') NOT NULL,
    reason VARCHAR(255),
    user_id INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (product_id) REFERENCES inventory(id),
    FOREIGN KEY (user_id) REFERENCES users(id)
);
```

### Table `invoices`
```sql
CREATE TABLE invoices (
    id INT PRIMARY KEY AUTO_INCREMENT,
    invoice_number VARCHAR(50) UNIQUE NOT NULL,
    client_name VARCHAR(100) NOT NULL,
    client_phone VARCHAR(20),
    client_email VARCHAR(100),
    total_amount DECIMAL(10, 2) NOT NULL,
    status ENUM('Brouillon', 'Envoyée', 'Payée', 'Annulée') NOT NULL,
    issue_date DATE NOT NULL,
    due_date DATE,
    notes TEXT,
    created_by INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (created_by) REFERENCES users(id)
);
```

### Table `invoice_items`
```sql
CREATE TABLE invoice_items (
    id INT PRIMARY KEY AUTO_INCREMENT,
    invoice_id INT NOT NULL,
    description VARCHAR(255) NOT NULL,
    quantity DECIMAL(10, 2) NOT NULL,
    unit_price DECIMAL(10, 2) NOT NULL,
    total_price DECIMAL(10, 2) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (invoice_id) REFERENCES invoices(id) ON DELETE CASCADE
);
```

### Table `audit_logs`
```sql
CREATE TABLE audit_logs (
    id INT PRIMARY KEY AUTO_INCREMENT,
    user_id INT NOT NULL,
    action VARCHAR(50) NOT NULL,
    entity VARCHAR(50) NOT NULL,
    entity_id INT,
    details TEXT,
    ip_address VARCHAR(45),
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id)
);
```

## Interface Utilisateur

### Design
- Design moderne et professionnel (Material Design)
- Navigation par menu latéral
- Thème sombre/clair
- Responsive et intuitive
- Icônes Material Design Icons
- Animations fluides

### Écrans principaux
1. **Écran de connexion** : Login/Password avec logo BlackWoods
2. **Dashboard** : Vue d'ensemble avec KPIs et graphiques
3. **Transactions** : Liste avec filtres et bouton d'ajout
4. **Employés** : Gestion des fiches employés
5. **Paies** : Gestion des salaires et paiements
6. **Inventaire** : Gestion des stocks avec alertes
7. **Factures** : Création et suivi des factures
8. **Caisse** : Interface de point de vente
9. **Rapports** : Génération de rapports personnalisés
10. **Paramètres** : Configuration de l'application et gestion des utilisateurs

## Installateur (.msi)

### Caractéristiques
- Installation dans Program Files\BlackWoods
- Raccourci bureau + menu démarrer
- Configuration initiale (URL de l'API)
- Vérification des prérequis (.NET 8 Runtime)
- Création d'un dossier de données utilisateur
- Désinstallation propre

### Processus d'installation
1. Écran de bienvenue
2. Acceptation de la licence
3. Choix du répertoire d'installation
4. Configuration de l'URL de l'API
5. Installation des fichiers
6. Création des raccourcis
7. Écran de fin

## Livrables attendus

1. **Solution Visual Studio complète**
   - Projet WPF client
   - Projet de modèles partagés
   - Projet WiX pour l'installateur

2. **Code source du client WPF**
   - Architecture MVVM
   - Services (API, Authentication, etc.)
   - ViewModels
   - Views (XAML)
   - Helpers et Utilities

3. **Structure de l'API REST**
   - Documentation des endpoints
   - Modèles de requêtes/réponses
   - Codes d'erreur

4. **Scripts SQL**
   - Création de base de données
   - Tables et relations
   - Données de test (seed)
   - Procédures stockées si nécessaire

5. **Projet WiX**
   - Configuration de l'installateur
   - Scripts de build

6. **Documentation utilisateur**
   - Guide d'installation
   - Manuel d'utilisation
   - FAQ

7. **Documentation technique**
   - Architecture de l'application
   - Guide de déploiement
   - Configuration de l'API

## Contraintes et Bonnes Pratiques

### Sécurité
- Hashage des mots de passe (BCrypt)
- HTTPS uniquement pour l'API
- Tokens JWT pour l'authentification
- Validation des données côté client et serveur
- Protection contre les injections SQL
- Gestion des permissions par rôle

### Performance
- Lazy loading des données
- Cache local pour réduire les appels API
- Pagination des listes
- Compression des réponses API

### Gestion des erreurs
- Try-catch robuste
- Messages d'erreur clairs pour l'utilisateur
- Logs détaillés pour le débogage
- Retry automatique pour les appels API échoués

### Fonctionnalités optionnelles
- Mode hors ligne (cache local avec synchronisation)
- Sauvegarde automatique des données
- Import/Export de données (CSV, Excel)
- Multi-langue (FR/EN)
- Notifications push

## Structure du Projet

```
BlackWoodsCompta/
├── src/
│   ├── BlackWoodsCompta.WPF/          # Application WPF principale
│   │   ├── Views/                      # Vues XAML
│   │   ├── ViewModels/                 # ViewModels MVVM
│   │   ├── Services/                   # Services (API, Auth, etc.)
│   │   ├── Models/                     # Modèles locaux
│   │   ├── Helpers/                    # Classes utilitaires
│   │   ├── Resources/                  # Images, styles, etc.
│   │   └── App.xaml                    # Point d'entrée
│   │
│   ├── BlackWoodsCompta.Models/        # Modèles partagés
│   │   ├── DTOs/                       # Data Transfer Objects
│   │   ├── Entities/                   # Entités de domaine
│   │   └── Enums/                      # Énumérations
│   │
│   └── BlackWoodsCompta.Installer/     # Projet WiX
│       ├── Product.wxs                 # Configuration installateur
│       └── Assets/                     # Icônes, bannières
│
├── docs/
│   ├── user-manual.md                  # Manuel utilisateur
│   ├── technical-doc.md                # Documentation technique
│   └── api-spec.md                     # Spécification API
│
├── database/
│   ├── schema.sql                      # Schéma de base de données
│   └── seed.sql                        # Données de test
│
└── README.md
```

## Technologies et NuGet Packages recommandés

### Client WPF
- **MaterialDesignThemes** : Interface Material Design
- **MaterialDesignColors** : Palettes de couleurs
- **Newtonsoft.Json** : Sérialisation JSON
- **RestSharp** ou **Refit** : Client HTTP pour API
- **BCrypt.Net-Next** : Hashage des mots de passe
- **LiveCharts** : Graphiques et visualisations
- **PdfSharp** ou **iTextSharp** : Génération de PDF
- **ClosedXML** : Export Excel
- **Serilog** : Logging
- **Microsoft.Extensions.DependencyInjection** : Injection de dépendances

### Installateur
- **WiX Toolset v4** : Création de l'installateur .msi

## Prochaines Étapes

1. ✅ Définition des spécifications (FAIT)
2. 🚀 Création de la structure du projet
3. 🔨 Développement du client WPF
4. 📡 Documentation de l'API REST
5. 🗄️ Scripts SQL de base de données
6. 📦 Configuration de l'installateur
7. 📝 Rédaction de la documentation
8. ✅ Tests et validation

---

## 📋 PHILOSOPHIE DE DÉVELOPPEMENT (14 Janvier 2026)

### Principes adoptés :
1. **Logging exhaustif** : Ajouter des logs à chaque étape critique pour faciliter le debug
2. **Vérification des logs** : Consulter systématiquement les logs avant toute correction
3. **Documentation continue** : Tout changement doit être documenté ici étape par étape
4. **Build incrémental** : Vérifier que le projet compile après chaque modification

---

## 🔄 JOURNAL DES MODIFICATIONS

### Session du 14 Janvier 2026 - Transfert GitHub & Setup Initial

#### ✅ Étape 1 : Analyse du projet après transfert GitHub
**Date** : 14/01/2026 - 14:00
**Action** : Lecture complète des fichiers de documentation (.md) et analyse de la structure
**Fichiers consultés** :
- `/README.md` - Introduction générale
- `/BlackWoodsCompta/README.md` - Documentation principale
- `/BlackWoodsCompta/QUICK_START.md` - Guide de démarrage rapide
- `/BlackWoodsCompta/PROJECT_SUMMARY.md` - Résumé du projet
- `/BlackWoodsCompta/docs/technical-doc.md` - Documentation technique
- `/BlackWoodsCompta/src/BlackWoodsCompta.WPF/BlackWoodsCompta.WPF.csproj` - Configuration du projet

**Résultat** : 
- Projet WPF .NET 8 avec architecture MVVM complète
- Base de données SQLite locale intégrée (pas besoin de MySQL pour démarrer)
- Mode hybride : choix entre base locale ou API distante
- 16 Views et ViewModels implémentés
- 7 Services créés

#### ✅ Étape 2 : Restauration des packages NuGet
**Date** : 14/01/2026 - 14:05
**Commande** : `dotnet restore`
**Répertoire** : `c:\Users\NiavlyS\Documents\Coding\Windows\Compta\BlackWoodsCompta\src`

**Résultat** :
```
✅ Restauration effectuée avec succès
⚠️  Warning NU1902: RestSharp 111.2.0 a une vulnérabilité de sécurité moyenne
   → À considérer pour une mise à jour future
```

**Packages installés** :
- MaterialDesignThemes 5.0.0
- MaterialDesignColors 3.0.0
- Newtonsoft.Json 13.0.3
- RestSharp 111.2.0 (⚠️ vulnérabilité)
- BCrypt.Net-Next 4.0.3
- LiveChartsCore.SkiaSharpView.WPF 2.0.0-rc2
- PdfSharp 6.1.0
- ClosedXML 0.102.2
- Serilog 3.1.1 + Serilog.Sinks.File 5.0.0
- Microsoft.Extensions.DependencyInjection 8.0.0
- Microsoft.Data.Sqlite 8.0.0
- Dapper 2.1.28

#### ✅ Étape 3 : Build et lancement du projet
**Date** : 14/01/2026 - 14:10
**Commande** : `dotnet run`
**Répertoire** : `c:\Users\NiavlyS\Documents\Coding\Windows\Compta\BlackWoodsCompta\src\BlackWoodsCompta.WPF`

**Résultat de la compilation** :
```
✅ Build réussi - Application lancée
⚠️  3 Warnings CS1998: Méthodes async sans await
   - SalePricesViewModel.cs ligne 151
   - PurchasePricesViewModel.cs ligne 164
   - OrdersViewModel.cs ligne 531
⚠️  1 Warning CS8602: Déréférencement possible d'une référence null
   - TransactionsViewModel.cs ligne 203
```

**État** : 
- ✅ Application opérationnelle
- ✅ Fenêtre de sélection de base de données s'affiche
- ✅ Mode local (SQLite) fonctionnel
- ⚠️ Warnings mineurs à corriger (non bloquants)

#### 📝 Étape 4 : Adoption de la nouvelle philosophie
**Date** : 14/01/2026 - 14:15
**Action** : Documentation de la philosophie de développement et du journal des modifications

**Prochaines actions recommandées** :
1. ✅ Tester le login avec la base locale
2. ⚠️ Corriger les warnings de nullabilité
3. ⚠️ Ajouter des logs dans les services critiques
4. ⚠️ Considérer mise à jour de RestSharp pour la sécurité
5. 📊 Vérifier le fonctionnement de toutes les vues

**Localisation des logs applicatifs** :
```
%APPDATA%\BlackWoodsCompta\Logs\app20260114.log
```
*Note : Le fichier de log contient la date du jour (format YYYYMMDD) et change quotidiennement*

**Procédure de débogage** :
1. Consulter les logs en priorité lors d'un problème
2. Documenter l'erreur dans ce fichier ia.md
3. Appliquer la correction
4. Vérifier que le build passe
5. Documenter la résolution

---

**Projet BlackWoods - Application de Comptabilité GTA RP**
*Version 1.0 - Janvier 2026*
