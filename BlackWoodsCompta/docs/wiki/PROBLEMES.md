# 🐛 Problèmes & Résolutions

Documentation de tous les problèmes rencontrés et de leurs résolutions.

---

## 📊 Statistiques

**Total des problèmes** : 4  
**Résolus** : 0  
**En cours** : 4  
**Non résolus** : 0

---

## ⚠️ Problèmes Actuels

### ⚠️ #001 - Warning CS8602 : Déréférencement de référence null
**Statut** : 🔶 En attente  
**Sévérité** : Faible (Warning)  
**Date de détection** : 14/01/2026 - 14:10  
**Fichier** : `TransactionsViewModel.cs:203`

**Description** :
```
CS8602: Dereference of a possibly null reference.
```

**Contexte** :
Détecté lors du build initial après transfert GitHub.

**Impact** :
- Non bloquant pour l'exécution
- Risque potentiel de NullReferenceException

**Solution proposée** :
1. Consulter la ligne 203 de TransactionsViewModel.cs
2. Ajouter une vérification de nullité ou utiliser l'opérateur `?.`
3. Documenter la correction

**Logs** :
```
À vérifier dans : %APPDATA%\BlackWoodsCompta\Logs\app20260114.log
```

**Résolution** : En attente

---

### ⚠️ #002 - Warning CS1998 : Méthode async sans await (SalePricesViewModel)
**Statut** : 🔶 En attente  
**Sévérité** : Faible (Warning)  
**Date de détection** : 14/01/2026 - 14:10  
**Fichier** : `SalePricesViewModel.cs:151`

**Description** :
```
CS1998: This async method lacks 'await' operators and will run synchronously.
```

**Contexte** :
Méthode déclarée async mais n'utilisant pas await.

**Impact** :
- Non bloquant
- Méthode s'exécute de manière synchrone malgré la déclaration async
- Peut créer de la confusion

**Solution proposée** :
1. Vérifier si la méthode doit vraiment être async
2. Si oui : ajouter les appels await manquants
3. Si non : retirer le modificateur async

**Résolution** : En attente

---

### ⚠️ #003 - Warning CS1998 : Méthode async sans await (PurchasePricesViewModel)
**Statut** : 🔶 En attente  
**Sévérité** : Faible (Warning)  
**Date de détection** : 14/01/2026 - 14:10  
**Fichier** : `PurchasePricesViewModel.cs:164`

**Description** :
```
CS1998: This async method lacks 'await' operators and will run synchronously.
```

**Contexte** :
Même type de problème que #002 dans un autre ViewModel.

**Solution proposée** :
Identique au problème #002

**Résolution** : En attente

---

### ⚠️ #004 - Warning CS1998 : Méthode async sans await (OrdersViewModel)
**Statut** : 🔶 En attente  
**Sévérité** : Faible (Warning)  
**Date de détection** : 14/01/2026 - 14:10  
**Fichier** : `OrdersViewModel.cs:531`

**Description** :
```
CS1998: This async method lacks 'await' operators and will run synchronously.
```

**Contexte** :
Même type de problème que #002 et #003.

**Solution proposée** :
Identique au problème #002

**Résolution** : En attente

---

## 🔒 Problèmes de Sécurité

### 🔒 #S001 - Vulnérabilité dans RestSharp 111.2.0
**Statut** : 🔶 En attente  
**Sévérité** : Moyenne  
**Date de détection** : 14/01/2026 - 14:05  
**Package** : RestSharp 111.2.0

**Description** :
```
NU1902: Package 'RestSharp' 111.2.0 has a known medium severity vulnerability
```

**Lien** : https://github.com/advisories/GHSA-4rr6-2v9v-wcpc

**Contexte** :
Détecté lors de la restauration des packages NuGet.

**Impact** :
- Vulnérabilité de sévérité moyenne
- Dépend de l'utilisation spécifique dans le projet

**Solution proposée** :
1. Vérifier la dernière version stable de RestSharp
2. Tester la compatibilité avec le projet
3. Mettre à jour le package
4. Vérifier que tous les appels API fonctionnent toujours

**Résolution** : En attente

---

## ✅ Problèmes Résolus

*Aucun problème résolu pour le moment*

---

## 📝 Template pour Nouveaux Problèmes

```markdown
### 🐛/⚠️/🔒 #XXX - [Titre du problème]
**Statut** : 🔴 Non résolu / 🔶 En cours / ✅ Résolu
**Sévérité** : Critique / Haute / Moyenne / Faible
**Date de détection** : JJ/MM/AAAA - HH:MM
**Fichier(s)** : `chemin/fichier.cs:ligne`

**Description** :
[Description détaillée du problème]

**Contexte** :
[Comment le problème a été découvert]

**Impact** :
- [Impact sur l'application]
- [Impact sur les utilisateurs]

**Reproduction** :
1. [Étapes pour reproduire]
2. [...]

**Logs** :
```
[Extraits des logs pertinents]
```

**Solution proposée** :
[Description de la solution envisagée]

**Résolution** :
[Une fois résolu, décrire la solution appliquée]
**Date de résolution** : JJ/MM/AAAA - HH:MM
**Commit** : [hash du commit]

---
```

## 🔍 Comment Utiliser ce Document

### Lors d'un nouveau problème :
1. Consulter d'abord les logs : `%APPDATA%\BlackWoodsCompta\Logs\app[YYYYMMDD].log`
2. Vérifier si le problème existe déjà dans ce fichier
3. Si nouveau : créer une nouvelle entrée avec un numéro unique
4. Documenter le contexte, les logs et l'impact
5. Proposer une solution

### Lors de la résolution :
1. Mettre à jour le statut à 🔶 En cours
2. Appliquer la correction
3. Vérifier le build
4. Tester la correction
5. Mettre à jour le statut à ✅ Résolu
6. Documenter la solution appliquée
7. Déplacer dans la section "Problèmes Résolus"
8. Mettre à jour les statistiques

---

## Légende des Statuts

- 🔴 Non résolu : Problème identifié, aucune action entreprise
- 🔶 En attente / En cours : Travail en cours sur la résolution
- ✅ Résolu : Problème corrigé et testé

## Légende des Sévérités

- **Critique** : Empêche l'application de fonctionner
- **Haute** : Fonctionnalité majeure cassée ou inutilisable
- **Moyenne** : Problème gênant mais contournable
- **Faible** : Problème mineur, warning, amélioration souhaitée

---

*Dernière mise à jour : 14/01/2026*
