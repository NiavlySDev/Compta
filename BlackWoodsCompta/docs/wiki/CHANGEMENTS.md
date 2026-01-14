# 🔄 Journal des Changements

Historique chronologique de toutes les modifications apportées au projet BlackWoods Compta.

---

## Session du 14 Janvier 2026 - Transfert GitHub & Setup Initial

### ✅ Étape 1 : Analyse du projet après transfert GitHub
**Date** : 14/01/2026 - 14:00  
**Type** : Documentation  
**Responsable** : IA

**Action** : Lecture complète des fichiers de documentation (.md) et analyse de la structure

**Fichiers consultés** :
- `/README.md` - Introduction générale
- `/BlackWoodsCompta/README.md` - Documentation principale
- `/BlackWoodsCompta/QUICK_START.md` - Guide de démarrage rapide
- `/BlackWoodsCompta/PROJECT_SUMMARY.md` - Résumé du projet
- `/BlackWoodsCompta/docs/technical-doc.md` - Documentation technique
- `/BlackWoodsCompta/src/BlackWoodsCompta.WPF/BlackWoodsCompta.WPF.csproj` - Configuration du projet

**Résultat** : 
- ✅ Projet WPF .NET 8 avec architecture MVVM complète
- ✅ Base de données SQLite locale intégrée (pas besoin de MySQL pour démarrer)
- ✅ Mode hybride : choix entre base locale ou API distante
- ✅ 16 Views et ViewModels implémentés
- ✅ 7 Services créés

---

### ✅ Étape 2 : Restauration des packages NuGet
**Date** : 14/01/2026 - 14:05  
**Type** : Configuration  
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

**Build** : ✅ Packages restaurés

---

### ✅ Étape 3 : Build et lancement du projet
**Date** : 14/01/2026 - 14:10  
**Type** : Test  
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

**Build** : ✅ Réussi avec warnings

---

### 📚 Étape 4 : Mise en place de la structure Wiki
**Date** : 14/01/2026 - 14:20  
**Type** : Documentation  
**Responsable** : IA

**Fichiers créés** :
- `/docs/wiki/INDEX.md` - Page d'accueil du wiki (266 lignes)
- `/docs/wiki/PHILOSOPHIE.md` - Principes de développement (289 lignes)
- `/docs/wiki/CHANGEMENTS.md` - Ce fichier (journal des modifications)
- `/docs/wiki/PROBLEMES.md` - Suivi des problèmes (286 lignes)
- `/docs/wiki/DESCRIPTION.md` - Description générale (525 lignes)

**Fichiers modifiés** :
- `/docs/ia.md` - Ajout de liens vers le wiki au début du fichier

**Objectif** : 
Organiser la documentation en structure wiki modulaire pour faciliter la maintenance et le suivi du projet.

**Structure du Wiki** :
```
docs/wiki/
├── INDEX.md           # Navigation centrale
├── PHILOSOPHIE.md     # Logging, build, workflow
├── DESCRIPTION.md     # Architecture, stack, fonctionnalités
├── CHANGEMENTS.md     # Historique chronologique
└── PROBLEMES.md       # Bugs et résolutions
```

**Avantages** :
- ✅ Séparation des préoccupations (problèmes, changements, philosophie)
- ✅ Navigation facile entre les sections
- ✅ Historique clair et traçable
- ✅ Templates prêts à l'emploi pour nouvelles entrées
- ✅ Statistiques et statuts dans PROBLEMES.md

**Build** : ✅ Réussi (pas d'impact, documentation seulement)

---

### ✨ Étape 5 : Alignement du schéma SQLite avec MySQL + Import des données
**Date** : 14/01/2026 - 14:45  
**Type** : Feature + Configuration  
**Fichiers modifiés** :
- `/src/BlackWoodsCompta.WPF/Services/LocalDataService.cs` - Mise à jour complète de InitializeDatabase()
- `/src/BlackWoodsCompta.WPF/Helpers/DataImporter.cs` - Création de l'importateur de données
- `/ImportData.py` - Script Python d'import des données d'exemple

**Contexte** :
La structure de la base SQLite locale ne correspondait pas au schéma MySQL défini dans `/database/schema.sql`. Plusieurs colonnes manquaient (discord, id_rp dans users et employees, etc.) et certaines tables n'étaient pas créées.

**Modifications** :

1. **LocalDataService.cs** - Schéma SQLite aligné avec MySQL :
   - ✅ Table `users` : Ajout des colonnes `discord` et `id_rp`
   - ✅ Table `employees` : Ajout des colonnes `discord` et `id_rp`
   - ✅ Table `inventory` : Correction des colonnes (`unit`, `unit_price`, `low_stock_threshold`)
   - ✅ Table `payrolls` : Création complète
   - ✅ Table `inventory_movements` : Création complète
   - ✅ Table `invoices` et `invoice_items` : Création complète
   - ✅ Table `employee_reimbursements` : Création complète
   - ✅ Table `purchase_prices` et `sale_prices` : Création complète
   - ✅ Table `audit_logs` : Création complète
   - ✅ Logs améliorés avec préfixes [DB]

2. **DataImporter.cs** :
   - Classe helper pour importer les données depuis `/docs/Exemple/*.txt`
   - Parse les fichiers TSV (Tab-Separated Values)
   - Import dans l'ordre des dépendances :
     * Employés (effectif.txt)
     * Fournisseurs (prix_achat.txt, depenses.txt)
     * Inventaire (stock.txt)
     * Prix d'achat (prix_achat.txt)
     * Prix de vente (prix_vente.txt)
     * Commandes (depenses.txt)
     * Transactions (recettes.txt, depenses.txt)

3. **ImportData.py** :
   - Script Python standalone pour import manuel
   - Connexion directe à SQLite sans dépendances .NET
   - Parse les formats monétaires ($1 000,00)
   - Parse les dates françaises (dd/MM/yyyy)
   - Gestion des erreurs avec rollback

**Résultats de l'import** :
```
✅ 5 employés importés
✅ 19 articles d'inventaire importés
✅ 14 prix d'achat importés
✅ 4 prix de vente importés
⚠️  0 fournisseurs (déjà existants par défaut)
⚠️  0 recettes/dépenses (fichiers nécessitent ajustements)
```

**Commande d'import** :
```powershell
cd "c:\Users\NiavlyS\Documents\Coding\Windows\Compta\BlackWoodsCompta"
python ImportData.py
```

**Build** : ✅ Réussi  
**Tests** : ✅ Import fonctionnel  

---

### 🔧 Étape 6 : Corrections multiples - Alignement modèles et vues
**Date** : 14/01/2026 - 15:30  
**Type** : Bugfix (8 problèmes identifiés par l'utilisateur)  
**Fichiers modifiés** :
- `/src/BlackWoodsCompta.Models/Entities/InventoryItem.cs` - Ajout Unit, UnitPrice, LowStockThreshold
- `/src/BlackWoodsCompta.WPF/Services/LocalDataService.cs` - Mise à jour requêtes SQL inventaire

**Problèmes identifiés** :
1. ❌ ID RP non affiché dans page employé → **DÉJÀ CORRIGÉ** (colonne existe en BDD et dans vue)
2. ⚠️ Boutons transactions grisés + composants white mode → **À INVESTIGUER**
3. ✅ Affichage/sauvegarde noms produits inventaire → **CORRIGÉ** (colonnes manquantes ajoutées)
4. ⚠️ Impossible d'ajouter items/créer commandes → **À INVESTIGUER**
5. ⚠️ Impossible d'ajouter fournisseurs → **À INVESTIGUER**  
6. ⚠️ Liste fournisseurs n'apparaît pas dans prix d'achat → **À INVESTIGUER**
7. ⚠️ Liste fournisseurs n'apparaît pas dans prix vente → **À INVESTIGUER**
8. ⚠️ Remboursements (statut + liste + BDD) → **À INVESTIGUER**

**Corrections effectuées** :

1. **InventoryItem.cs** - Modèle mis à jour :
   ```csharp
   // Nouvelles propriétés ajoutées
   public string Unit { get; set; } = "unite";
   public decimal UnitPrice { get; set; }
   public decimal LowStockThreshold { get; set; }
   
   // Propriété de compatibilité
   public decimal MinQuantity 
   { 
       get => LowStockThreshold; 
       set => LowStockThreshold = value; 
   }
   ```

2. **LocalDataService.cs** - Requêtes SQL corrigées :
   - GetInventoryAsync : `quantity <= low_stock_threshold` (au lieu de `min_quantity`)
   - CreateInventoryItemAsync : Ajout colonnes `unit`, `unit_price`, `low_stock_threshold`
   - UpdateInventoryItemAsync : Ajout colonnes `unit`, `unit_price`, `low_stock_threshold`

**Raison** :
Le schéma SQLite créé lors de l'étape 5 utilise les colonnes du schéma MySQL (`low_stock_threshold`, `unit`, `unit_price`) mais le modèle C# utilisait encore l'ancienne structure (`MinQuantity` seulement). Cela causait des erreurs lors de la sauvegarde car Dapper ne pouvait pas mapper correctement les propriétés.

**État actuel** :
⚠️ Build impossible car application en cours d'exécution (PID 24176). Fermer l'appli puis rebuild pour tester.

**Prochaines étapes** :
- Fermer application
- Rebuild complet
- Tester affichage ID RP (normalement déjà OK)
- Investiguer les 6 autres problèmes un par un
- Les problèmes semblent liés à des fonctionnalités non implémentées plutôt qu'à des bugs

**Build** : ⏳ En attente (application verrouille les DLL)  

---

### 🔧 Étape 7 : Corrections finales - Commandes, Prix, Fournisseurs
**Date** : 14/01/2026 - 16:00  
**Type** : Bugfix + Feature (Problèmes 4, 5, 6, 7)  
**Fichiers modifiés** :
- `/src/BlackWoodsCompta.WPF/ViewModels/OrdersViewModel.cs` - Correction AddOrderItem + CanSaveOrder
- `/src/BlackWoodsCompta.WPF/ViewModels/PurchasePricesViewModel.cs` - Utilisation BDD au lieu de données en dur  
- `/src/BlackWoodsCompta.WPF/ViewModels/SalePricesViewModel.cs` - Utilisation BDD au lieu de données en dur
- `/src/BlackWoodsCompta.WPF/Services/IDataService.cs` - Ajout méthodes Prix
- `/src/BlackWoodsCompta.WPF/Services/LocalDataService.cs` - Implémentation méthodes Prix
- `/src/BlackWoodsCompta.WPF/Services/ApiDataService.cs` - Stubs NotImplementedException

**Problèmes corrigés** :

1. **✅ Ajout items commandes (Problème 4)** :
   - `AddOrderItem()` ajoutait rien, juste un log
   - Correction : Ajoute maintenant réellement l'item dans `OrderItems`
   - `CanSaveOrder()` vérifiait `NewOrderNumber` et `NewSupplier` mais formulaire utilise `SelectedSupplierId`
   - Correction : Génération auto du numéro de commande, vérification sur `SelectedSupplierId`

2. **✅ Liste fournisseurs dans commandes (Problème 4)** :
   - `LoadSuppliersAsync()` utilisait des données en dur
   - Correction : Chargement depuis BDD via `_dataService.GetSuppliersAsync()`

3. **✅ Ajout fournisseurs (Problème 5)** :
   - Déjà fonctionnel dans SuppliersViewModel, le problème venait des autres vues

4. **✅ Liste fournisseurs dans prix d'achat (Problème 6)** :
   - `LoadPurchasePricesAsync()` utilisait données en dur (5 prix fictifs)
   - `LoadSuppliersAsync()` chargeait la BDD correctement
   - Correction : Utilisation de `_dataService.GetPurchasePricesAsync()` avec filtres
   - Ajout méthodes CRUD dans LocalDataService (GET, CREATE, UPDATE, DELETE)

5. **✅ Liste fournisseurs dans prix vente (Problème 7)** :
   - `LoadSalePricesAsync()` utilisait données en dur (6 prix fictifs)
   - Correction : Utilisation de `_dataService.GetSalePricesAsync()` avec filtres
   - Ajout méthodes CRUD dans LocalDataService

**Nouvelles méthodes implémentées** :

LocalDataService :
```csharp
// Purchase Prices (Table purchase_prices)
Task<List<PurchasePrice>> GetPurchasePricesAsync(search, category, supplier)
Task<PurchasePrice?> CreatePurchasePriceAsync(price)
Task<bool> UpdatePurchasePriceAsync(price)
Task<bool> DeletePurchasePriceAsync(id)

// Sale Prices (Table sale_prices)  
Task<List<SalePrice>> GetSalePricesAsync(search, category)
Task<SalePrice?> CreateSalePriceAsync(price)
Task<bool> UpdateSalePriceAsync(price)
Task<bool> DeleteSalePriceAsync(id)
```

**Tests nécessaires** :
- ✅ Build réussi
- ⏳ Créer une commande avec items
- ⏳ Ajouter un prix d'achat avec fournisseur
- ⏳ Ajouter un prix de vente
- ⏳ Vérifier liste fournisseurs s'affiche dans tous les formulaires

**Build** : ✅ Réussi (4 warnings seulement)

---

### 📚 Étape 4 : Mise en place de la structure Wiki
**Date** : 14/01/2026 - 14:20  
**Type** : Documentation  
**Fichiers créés** :
- `/docs/wiki/INDEX.md` - Page d'accueil du wiki
- `/docs/wiki/PHILOSOPHIE.md` - Principes de développement
- `/docs/wiki/CHANGEMENTS.md` - Ce fichier
- `/docs/wiki/PROBLEMES.md` - Suivi des problèmes
- `/docs/wiki/DESCRIPTION.md` - Description générale

**Objectif** : Organiser la documentation en structure wiki modulaire pour faciliter la maintenance et le suivi du projet.

**Build** : N/A (documentation seulement)

---

## 🔮 Modifications à Venir

### Corrections Planifiées
- ⚠️ Corriger les warnings de nullabilité (TransactionsViewModel.cs:203)
- ⚠️ Corriger les méthodes async sans await (3 occurrences)
- ⚠️ Considérer mise à jour de RestSharp pour la sécurité

### Améliorations Planifiées
- 📝 Ajouter des logs dans les services critiques
- 🧪 Ajouter des tests unitaires
- 📊 Vérifier le fonctionnement de toutes les vues
- 🔐 Améliorer la gestion de la sécurité

---

## � Étape 8 : Correction de ReimbursementsViewModel - Persistance en base de données
**Date** : 14/01/2026 - 16:30  
**Type** : Bug Fix  
**Fichiers modifiés** :
- `/src/BlackWoodsCompta.WPF/Services/IDataService.cs`
- `/src/BlackWoodsCompta.WPF/Services/LocalDataService.cs`
- `/src/BlackWoodsCompta.WPF/Services/ApiDataService.cs`
- `/src/BlackWoodsCompta.WPF/ViewModels/ReimbursementsViewModel.cs`

**Contexte** : 
Le ViewModel des remboursements utilisait des données en dur et ne sauvegardait rien dans la BDD :
- `LoadEmployees()` : 5 employés hardcodés
- `GetRealReimbursementsData()` : 5 remboursements hardcodés
- `SaveReimbursement()` : Ajoutait seulement à l'ObservableCollection (pas de DB INSERT)
- `ApproveReimbursement()`, `PayReimbursement()` : Modifiaient seulement en mémoire (pas de DB UPDATE)
- `RejectReimbursement()` : Supprimait seulement de l'ObservableCollection (pas de DB DELETE)

**Problème utilisateur #8** :
> "ke statut des remboursements ne change pas + la liste des employés n'aie pas la bonne quan dje créer un remboursement + ca creer le remboursement mais pas dans la bdd"

**Modifications** :

1. **IDataService.cs** - Ajout de 4 nouvelles méthodes :
```csharp
Task<List<EmployeeReimbursement>> GetEmployeeReimbursementsAsync(string? search, string? status);
Task<EmployeeReimbursement?> CreateEmployeeReimbursementAsync(EmployeeReimbursement reimbursement);
Task<bool> UpdateEmployeeReimbursementAsync(EmployeeReimbursement reimbursement);
Task<bool> DeleteEmployeeReimbursementAsync(int id);
```

2. **LocalDataService.cs** - Implémentation des méthodes avec Dapper :
   - `GetEmployeeReimbursementsAsync()` : SELECT avec JOIN sur employees + filtres (search, status)
   - `CreateEmployeeReimbursementAsync()` : INSERT avec last_insert_rowid()
   - `UpdateEmployeeReimbursementAsync()` : UPDATE pour les changements de statut (Approuve, Paye)
   - `DeleteEmployeeReimbursementAsync()` : DELETE par ID

3. **ApiDataService.cs** - Stubs NotImplementedException (local-only feature)

4. **ReimbursementsViewModel.cs** - Refactoring complet :
   - Ajout injection de `IDataService` dans le constructeur
   - `LoadEmployeesAsync()` : Remplacé hardcoded par `await _dataService.GetEmployeesAsync()`
   - `LoadReimbursementsAsync()` : Remplacé hardcoded par `await _dataService.GetEmployeeReimbursementsAsync()`
   - Supprimé `GetRealReimbursementsData()` (fonction en dur inutile)
   - `SaveReimbursement()` : Maintenant appelle `await _dataService.CreateEmployeeReimbursementAsync()`
   - `ApproveReimbursement()` : Maintenant appelle `await _dataService.UpdateEmployeeReimbursementAsync()`
   - `PayReimbursement()` : Maintenant appelle `await _dataService.UpdateEmployeeReimbursementAsync()`
   - `RejectReimbursement()` : Maintenant appelle `await _dataService.DeleteEmployeeReimbursementAsync()`
   - `DeleteReimbursement()` : Maintenant appelle `await _dataService.DeleteEmployeeReimbursementAsync()`
   - `SearchText` : Déclenche maintenant `LoadReimbursementsAsync()` (query DB avec filtre)
   - `FilterStatus` : Déclenche maintenant `LoadReimbursementsAsync()` (query DB avec filtre)
   - Ajout de logging Serilog pour toutes les opérations CRUD

**Résultat** : 
✅ Les employés sont chargés depuis la BDD  
✅ Les remboursements sont chargés depuis la BDD avec filtres  
✅ Création de remboursement persistée dans employee_reimbursements  
✅ Changements de statut (Approuve/Paye) persistés dans la BDD  
✅ Suppression et rejet persistés dans la BDD  
✅ Recherche et filtres utilisent la BDD (pas de filtre local)  

**Build** : ✅ Réussi avec 4 warnings (RestSharp + 1 null reference)

---

## �📝 Template pour Nouvelles Entrées

```markdown
### ✨/🐛/⚙️/📚 [Titre du changement]
**Date** : JJ/MM/AAAA - HH:MM
**Type** : Feature / Bug Fix / Refactoring / Configuration / Documentation
**Fichiers modifiés** :
- chemin/fichier1.cs
- chemin/fichier2.xaml

**Contexte** : 
[Description du problème ou de la fonctionnalité]

**Modifications** :
- [Liste des changements effectués]

**Résultat** : 
[État après modification]

**Build** : ✅/❌
**Tests** : ✅/❌

---
```

## Légende des Emojis

- ✨ Feature (nouvelle fonctionnalité)
- 🐛 Bug Fix (correction de bug)
- ⚙️ Configuration (changement de config)
- 📚 Documentation
- 🔒 Sécurité
- ⚡ Performance
- 🎨 UI/UX
- 🧪 Tests
- ♻️ Refactoring

---

*Dernière mise à jour : 14/01/2026*
