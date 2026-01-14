# 🎯 Philosophie de Développement

## Principes Fondamentaux

### 1. 📝 Logging Exhaustif
**Objectif** : Faciliter le débogage et le diagnostic

**Règles** :
- Ajouter des logs à chaque étape critique du code
- Logger les entrées/sorties des méthodes importantes
- Logger les exceptions avec stack trace complète
- Utiliser des niveaux de log appropriés (Debug, Info, Warning, Error)

**Implémentation** :
```csharp
// Exemple de logging dans un service
Log.Information("Démarrage de GetTransactions pour l'utilisateur {UserId}", userId);
try 
{
    var result = await _repository.GetTransactionsAsync(userId);
    Log.Information("GetTransactions: {Count} transactions récupérées", result.Count);
    return result;
}
catch (Exception ex)
{
    Log.Error(ex, "Erreur lors de GetTransactions pour {UserId}", userId);
    throw;
}
```

**Localisation des logs** :
```
%APPDATA%\BlackWoodsCompta\Logs\app[YYYYMMDD].log
```

---

### 2. 🔍 Vérification Systématique des Logs
**Objectif** : Diagnostiquer avant de corriger

**Processus** :
1. Lors d'un problème signalé → **TOUJOURS** consulter les logs en premier
2. Identifier la séquence d'événements qui a mené à l'erreur
3. Repérer les messages d'erreur, warnings et exceptions
4. Analyser le contexte (données, utilisateur, timing)
5. Formuler une hypothèse basée sur les logs
6. Appliquer la correction ciblée

**Ne JAMAIS** :
- Corriger "à l'aveugle" sans avoir consulté les logs
- Supposer la cause sans preuve dans les logs
- Ignorer les warnings qui pourraient être liés

---

### 3. 📚 Documentation Continue
**Objectif** : Maintenir un historique complet et traçable

**Ce qui doit être documenté** :
- ✅ Chaque modification de code significative
- ✅ Chaque problème rencontré et sa résolution
- ✅ Les décisions d'architecture importantes
- ✅ Les changements de configuration
- ✅ Les mises à jour de dépendances

**Où documenter** :
- **Problèmes** : [PROBLEMES.md](PROBLEMES.md)
- **Changements** : [CHANGEMENTS.md](CHANGEMENTS.md)
- **Architecture** : [DESCRIPTION.md](DESCRIPTION.md)

**Format de documentation** :
```markdown
#### 🐛/✨/⚙️ [Titre du changement]
**Date** : JJ/MM/AAAA - HH:MM
**Type** : Bug Fix / Feature / Refactoring / Configuration
**Fichiers modifiés** : 
- chemin/fichier1.cs
- chemin/fichier2.xaml

**Contexte** : Description du problème ou de la fonctionnalité

**Solution** : Ce qui a été fait

**Résultat** : État après modification

**Build** : ✅ Réussi / ❌ Échoué
```

---

### 4. 🔨 Build Incrémental
**Objectif** : Détecter les régressions immédiatement

**Règles** :
- Vérifier que le projet compile **APRÈS CHAQUE** modification significative
- Ne jamais accumuler plusieurs changements sans vérifier le build
- Si le build échoue, corriger avant de continuer
- Documenter tout échec de build et sa résolution

**Commande de build** :
```powershell
cd "C:\Users\NiavlyS\Documents\Coding\Windows\Compta\BlackWoodsCompta\src"
dotnet build --no-restore
```

**Commande de build + exécution** :
```powershell
cd "C:\Users\NiavlyS\Documents\Coding\Windows\Compta\BlackWoodsCompta\src\BlackWoodsCompta.WPF"
dotnet run
```

---

## 🔄 Workflow de Développement

### Processus Standard

1. **Avant toute modification** :
   ```
   - Consulter les logs si un problème est signalé
   - Lire la documentation existante
   - Comprendre le contexte du code à modifier
   ```

2. **Pendant la modification** :
   ```
   - Ajouter des logs aux points critiques
   - Commenter le code complexe
   - Respecter les conventions du projet
   ```

3. **Après la modification** :
   ```
   - Vérifier le build (dotnet build)
   - Tester la fonctionnalité modifiée
   - Documenter dans le wiki
   - Commit avec message descriptif
   ```

### En cas d'erreur

```mermaid
Erreur Signalée
    ↓
Consulter les Logs
    ↓
Identifier la Cause
    ↓
Documenter dans PROBLEMES.md
    ↓
Appliquer la Correction
    ↓
Vérifier le Build
    ↓
Tester la Correction
    ↓
Documenter la Résolution
    ↓
Build Final
```

---

## 🎨 Standards de Code

### Conventions de Nommage
- **Classes** : PascalCase (`TransactionService`)
- **Méthodes** : PascalCase (`GetTransactionsAsync`)
- **Variables** : camelCase (`userId`, `transactionList`)
- **Constantes** : PascalCase ou UPPER_CASE (`MaxRetries` ou `MAX_RETRIES`)
- **Propriétés** : PascalCase (`TotalAmount`)
- **Champs privés** : _camelCase (`_apiService`)

### Organisation des Fichiers
```
Services/
    IServiceName.cs          # Interface
    ServiceName.cs           # Implémentation
    
ViewModels/
    ViewModelBase.cs         # Classe de base
    [View]ViewModel.cs       # ViewModels spécifiques
    
Views/
    [View].xaml             # Interface
    [View].xaml.cs          # Code-behind (minimal)
```

### Gestion des Exceptions
```csharp
// ✅ Bon
try 
{
    Log.Debug("Tentative de connexion à l'API");
    var result = await _apiService.CallAsync();
    Log.Information("Connexion réussie");
    return result;
}
catch (HttpRequestException ex)
{
    Log.Error(ex, "Erreur réseau lors de l'appel API");
    throw new ApplicationException("Impossible de se connecter au serveur", ex);
}

// ❌ Mauvais
try 
{
    var result = await _apiService.CallAsync();
    return result;
}
catch (Exception ex)
{
    // Exception avalée sans log
    return null;
}
```

---

## 📊 Métriques de Qualité

### Objectifs
- ✅ 0 erreur de compilation
- ⚠️ Minimiser les warnings (< 5)
- 📝 Tous les services critiques loggés
- 📚 Toutes les modifications documentées
- 🧪 Fonctionnalités testées manuellement avant commit

---

## 🚀 Évolution de la Philosophie

Cette philosophie est un document vivant qui peut évoluer selon les besoins du projet.

**Historique des révisions** :
- **14/01/2026** : Première version établie

---

*"Un code bien documenté et bien loggé est un code maintenable"*
