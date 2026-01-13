# Manuel d'Utilisation - BlackWoods Compta

## Table des Matières
1. [Introduction](#introduction)
2. [Installation](#installation)
3. [Connexion](#connexion)
4. [Tableau de Bord](#tableau-de-bord)
5. [Transactions](#transactions)
6. [Employés](#employés)
7. [Paies](#paies)
8. [Inventaire](#inventaire)
9. [Factures](#factures)
10. [Caisse](#caisse)
11. [Rapports](#rapports)
12. [Paramètres](#paramètres)

---

## Introduction

BlackWoods Compta est une application de gestion comptable conçue spécifiquement pour le restaurant BlackWoods dans l'environnement GTA RP. Elle permet de gérer toutes les opérations financières, les employés, l'inventaire et la facturation de manière simple et efficace.

### Fonctionnalités principales
- ✅ Gestion des transactions (ventes et dépenses)
- 👥 Gestion des employés et des paies
- 📦 Suivi de l'inventaire en temps réel
- 🧾 Création et gestion de factures
- 💵 Point de vente intégré
- 📊 Rapports et statistiques détaillés
- 🔒 Système d'authentification sécurisé
- 📝 Traçabilité complète des opérations

---

## Installation

### Prérequis
- Windows 10 ou supérieur
- 200 MB d'espace disque disponible
- Connexion Internet pour accéder à l'API

### Procédure d'installation

1. **Téléchargement**
   - Téléchargez le fichier `BlackWoodsCompta-Setup.msi`
   - Vérifiez que le fichier est bien téléchargé

2. **Exécution de l'installateur**
   - Double-cliquez sur `BlackWoodsCompta-Setup.msi`
   - Cliquez sur "Suivant" à l'écran de bienvenue
   - Acceptez les termes de la licence
   - Choisissez le dossier d'installation (par défaut: `C:\Program Files\BlackWoods`)
   - Cliquez sur "Installer"

3. **Configuration initiale**
   - Au premier lancement, l'application vous demandera l'URL de l'API
   - Entrez l'URL fournie par votre administrateur système
   - Exemple: `http://api.blackwoods.local:5000`

4. **Vérification**
   - L'application devrait maintenant s'ouvrir sur l'écran de connexion
   - Un raccourci a été créé sur le bureau

---

## Connexion

### Première connexion

1. Lancez l'application depuis le raccourci bureau ou le menu démarrer
2. Sur l'écran de connexion, entrez vos identifiants:
   - **Nom d'utilisateur**: Fourni par votre administrateur
   - **Mot de passe**: Fourni par votre administrateur
3. Cliquez sur "SE CONNECTER"

### Rôles utilisateurs

L'application distingue trois types de rôles:

- **Admin**: Accès complet à toutes les fonctionnalités
- **Manager**: Peut gérer les transactions, employés, inventaire et factures
- **Employé**: Accès limité à la caisse et consultation des données

### Déconnexion

Pour vous déconnecter:
1. Cliquez sur le bouton "Déconnexion" en bas du menu latéral
2. Vous serez redirigé vers l'écran de connexion

---

## Tableau de Bord

Le tableau de bord est la page d'accueil après connexion. Il affiche une vue d'ensemble de l'activité du restaurant.

### Indicateurs Clés (KPIs)

- **Revenus**: Somme totale des ventes
- **Dépenses**: Somme totale des dépenses
- **Bénéfice Net**: Différence entre revenus et dépenses
- **Transactions**: Nombre total de transactions

### Informations Rapides

- **Employés**: Nombre d'employés actifs
- **Stock Bas**: Articles nécessitant un réapprovisionnement
- **Factures en attente**: Factures non payées

### Actualisation

Cliquez sur l'icône ⟳ en haut à droite pour rafraîchir les données.

---

## Transactions

### Consulter les transactions

1. Cliquez sur "Transactions" dans le menu latéral
2. La liste de toutes les transactions s'affiche
3. Utilisez la barre de recherche pour filtrer les résultats

### Ajouter une transaction

1. Cliquez sur le bouton "+ Nouvelle Transaction"
2. Remplissez le formulaire:
   - **Type**: Vente ou Dépense
   - **Catégorie**: Choisissez ou créez une catégorie
   - **Montant**: Entrez le montant en $
   - **Description**: Détails de la transaction (optionnel)
3. Cliquez sur "Enregistrer"

### Catégories courantes

- **Ventes**: Nourriture, Boissons
- **Dépenses**: Salaires, Fournitures, Loyer, Électricité, Entretien, Marketing

### Supprimer une transaction

1. Trouvez la transaction dans la liste
2. Cliquez sur l'icône 🗑️ dans la colonne "Actions"
3. Confirmez la suppression

---

## Employés

### Liste des employés

1. Cliquez sur "Employés" dans le menu
2. Consultez la liste de tous les employés
3. Voyez le statut (Actif/Inactif) de chaque employé

### Ajouter un employé

1. Cliquez sur "+ Nouvel Employé"
2. Remplissez les informations:
   - **Nom complet**
   - **Poste** (Serveur, Cuisinier, etc.)
   - **Salaire mensuel** en $
   - **Date d'embauche**
   - **Téléphone** (optionnel)
   - **Email** (optionnel)
3. Cliquez sur "Enregistrer"

### Modifier un employé

1. Cliquez sur l'employé dans la liste
2. Modifiez les informations
3. Cliquez sur "Enregistrer"

### Désactiver un employé

Pour les employés qui quittent l'entreprise:
1. Ouvrez la fiche de l'employé
2. Changez le statut à "Inactif"
3. Enregistrez

---

## Paies

### Consulter les paies

1. Cliquez sur "Paies" dans le menu
2. Voyez l'historique de toutes les paies versées
3. Filtrez par employé ou période

### Enregistrer une paie

1. Cliquez sur "+ Nouvelle Paie"
2. Sélectionnez l'employé
3. Le montant est pré-rempli avec le salaire de l'employé
4. Choisissez la période:
   - **Début de période**
   - **Fin de période**
   - **Date de paiement**
5. Ajoutez des notes si nécessaire
6. Cliquez sur "Enregistrer"

### Rapports de paie

- Consultez le total des salaires versés par mois
- Exportez les données pour la comptabilité

---

## Inventaire

### Vue de l'inventaire

1. Cliquez sur "Inventaire" dans le menu
2. Consultez tous les articles en stock
3. Les articles en **stock bas** sont mis en évidence en orange

### Ajouter un article

1. Cliquez sur "+ Nouvel Article"
2. Remplissez les informations:
   - **Nom du produit**
   - **Catégorie** (Ingrédients, Boissons, etc.)
   - **Quantité initiale**
   - **Unité** (kg, litres, unités, etc.)
   - **Coût unitaire** en $
   - **Quantité minimale** (pour les alertes de stock bas)
   - **Fournisseur** (optionnel)
3. Cliquez sur "Enregistrer"

### Mouvements d'inventaire

Pour enregistrer une entrée ou sortie de stock:

1. Cliquez sur l'article concerné
2. Cliquez sur "Ajouter un mouvement"
3. Choisissez le type:
   - **Entrée**: Réapprovisionnement
   - **Sortie**: Utilisation ou vente
   - **Ajustement**: Correction d'inventaire
4. Entrez la quantité
5. Ajoutez une raison
6. Cliquez sur "Enregistrer"

### Historique des mouvements

- Consultez tous les mouvements d'un article
- Tracez qui a effectué chaque mouvement
- Analysez les tendances de consommation

---

## Factures

### Liste des factures

1. Cliquez sur "Factures" dans le menu
2. Consultez toutes les factures
3. Filtrez par statut:
   - **Brouillon**: En cours de création
   - **Envoyée**: Envoyée au client
   - **Payée**: Réglée par le client
   - **Annulée**: Annulée

### Créer une facture

1. Cliquez sur "+ Nouvelle Facture"
2. Remplissez les informations client:
   - **Nom du client**
   - **Téléphone** (optionnel)
   - **Email** (optionnel)
3. Définissez les dates:
   - **Date d'émission**
   - **Date d'échéance**
4. Ajoutez des articles:
   - **Description**
   - **Quantité**
   - **Prix unitaire**
   - Le total est calculé automatiquement
5. Ajoutez des notes si nécessaire
6. Choisissez le statut
7. Cliquez sur "Enregistrer"

### Exporter une facture en PDF

1. Ouvrez la facture
2. Cliquez sur "Exporter PDF"
3. Choisissez l'emplacement de sauvegarde
4. Le PDF est généré avec le logo BlackWoods

### Modifier le statut d'une facture

1. Ouvrez la facture
2. Changez le statut (ex: de "Envoyée" à "Payée")
3. Enregistrez

---

## Caisse

Le module Caisse est conçu pour les opérations de vente rapides.

### Utiliser la caisse

1. Cliquez sur "Caisse" dans le menu
2. Interface simplifiée pour la prise de commande
3. Ajoutez des articles au panier:
   - Recherchez un article
   - Entrez la quantité
   - Le total s'affiche automatiquement
4. Choisissez le mode de paiement:
   - **Espèces**
   - **Carte bancaire**
5. Pour les espèces, entrez le montant reçu
   - La monnaie à rendre est calculée automatiquement
6. Cliquez sur "Valider la vente"
7. Un ticket peut être imprimé

### Clôture de caisse

À la fin du service:
1. Cliquez sur "Clôture de caisse"
2. Consultez le récapitulatif:
   - Total des ventes
   - Nombre de transactions
   - Répartition par mode de paiement
3. Imprimez ou exportez le rapport
4. Cliquez sur "Clôturer"

---

## Rapports

### Tableau de bord des rapports

1. Cliquez sur "Rapports" dans le menu
2. Consultez les graphiques et statistiques

### Générer un rapport personnalisé

1. Cliquez sur "Nouveau Rapport"
2. Choisissez la période:
   - Aujourd'hui
   - Cette semaine
   - Ce mois
   - Période personnalisée
3. Sélectionnez les données à inclure:
   - Transactions
   - Ventes par catégorie
   - Dépenses par catégorie
   - Évolution temporelle
4. Cliquez sur "Générer"

### Exporter un rapport

Les rapports peuvent être exportés dans plusieurs formats:
- **PDF**: Pour impression ou archivage
- **Excel**: Pour analyse approfondie
- **CSV**: Pour import dans d'autres outils

---

## Paramètres

### Configuration de l'application

1. Cliquez sur "Paramètres" dans le menu
2. Sections disponibles:

#### URL de l'API
- Modifiez l'URL si le serveur change
- Testez la connexion

#### Gestion des utilisateurs (Admin seulement)
- Ajoutez de nouveaux utilisateurs
- Modifiez les rôles
- Désactivez des comptes

#### Préférences
- Langue de l'interface
- Format de date
- Devise

#### À propos
- Version de l'application
- Informations de support

---

## Résolution de Problèmes

### Impossible de se connecter

**Symptôme**: Message d'erreur "Erreur de connexion au serveur"

**Solutions**:
1. Vérifiez votre connexion Internet
2. Vérifiez l'URL de l'API dans les paramètres
3. Contactez votre administrateur système

### Les données ne se chargent pas

**Solutions**:
1. Cliquez sur le bouton d'actualisation
2. Déconnectez-vous et reconnectez-vous
3. Vérifiez que le serveur est en ligne

### Erreur lors de l'enregistrement

**Solutions**:
1. Vérifiez que tous les champs obligatoires sont remplis
2. Vérifiez le format des données (montants, dates, etc.)
3. Consultez les logs d'erreur

---

## Support

Pour toute question ou problème:
- **Email**: support@blackwoods.com
- **Téléphone**: 555-BLACKWOODS
- **Documentation en ligne**: https://docs.blackwoods.com

---

## Conseils d'utilisation

### Bonnes pratiques

1. **Sauvegardes**: Les données sont automatiquement sauvegardées sur le serveur
2. **Déconnexion**: Déconnectez-vous toujours après utilisation
3. **Mots de passe**: Changez votre mot de passe régulièrement
4. **Vérification**: Vérifiez toujours vos saisies avant d'enregistrer
5. **Rapports**: Consultez les rapports régulièrement pour suivre l'activité

### Raccourcis clavier

- `Ctrl + N`: Nouvelle transaction (sur l'écran Transactions)
- `Ctrl + S`: Sauvegarder
- `Ctrl + F`: Rechercher
- `F5`: Actualiser
- `Esc`: Annuler/Fermer

---

**BlackWoods Compta v1.0.0**  
© 2026 BlackWoods Restaurant - Tous droits réservés
