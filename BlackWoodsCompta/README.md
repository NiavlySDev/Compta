# 🏢 BlackWoods Compta - Application de Comptabilité GTA RP

Application Windows de comptabilité professionnelle pour le restaurant BlackWoods dans un environnement GTA RP.

## 📋 Prérequis

- Windows 10/11
- .NET 8.0 Runtime
- MySQL Server 8.0+ (pour l'API)

## 🚀 Installation

1. Télécharger le fichier `BlackWoodsCompta-Setup.msi`
2. Double-cliquer pour lancer l'installateur
3. Suivre les étapes d'installation
4. Configurer l'URL de l'API lors de la première utilisation

## 🔧 Configuration de l'API

L'application nécessite une API REST connectée à une base de données MySQL. 
Voir `docs/api-spec.md` pour la spécification complète.

## 📖 Documentation

- [Manuel d'utilisation](docs/user-manual.md)
- [Documentation technique](docs/technical-doc.md)
- [Spécification API](docs/api-spec.md)

## 🏗️ Structure du Projet

```
BlackWoodsCompta/
├── src/
│   ├── BlackWoodsCompta.WPF/          # Application WPF
│   ├── BlackWoodsCompta.Models/       # Modèles partagés
│   └── BlackWoodsCompta.sln           # Solution Visual Studio
├── database/
│   ├── schema.sql                     # Schéma MySQL
│   └── seed.sql                       # Données de test
├── docs/
│   ├── user-manual.md
│   ├── technical-doc.md
│   └── api-spec.md
└── README.md
```

## ✨ Fonctionnalités

- ✅ Authentification multi-utilisateurs avec rôles
- 💰 Gestion des transactions (ventes/dépenses)
- 👥 Gestion des employés et paies
- 📦 Gestion de l'inventaire
- 🧾 Facturation et devis
- 💵 Point de vente (caisse)
- 📊 Rapports et statistiques
- 🔍 Audit et traçabilité

## 🛠️ Développement

### Compilation

```powershell
cd src
dotnet restore
dotnet build
```

### Exécution

```powershell
cd src/BlackWoodsCompta.WPF
dotnet run
```

## 📝 License

© 2026 BlackWoods Restaurant - Tous droits réservés

## 👨‍💻 Support

Pour toute question ou problème, contacter l'administrateur système.
