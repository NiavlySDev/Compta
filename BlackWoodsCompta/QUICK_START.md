# 🚀 Guide de Démarrage Rapide - BlackWoods Compta

## ⚡ Démarrage en 5 minutes

### 1. Installer les prérequis (10 min)

#### A. .NET 8.0 SDK
```bash
# Télécharger depuis:
https://dotnet.microsoft.com/download/dotnet/8.0

# Vérifier l'installation:
dotnet --version
# Devrait afficher: 8.0.x
```

#### B. MySQL Server 8.0+
```bash
# Télécharger depuis:
https://dev.mysql.com/downloads/mysql/

# Ou avec Chocolatey (Windows):
choco install mysql

# Ou avec Docker:
docker run --name blackwoods-mysql -e MYSQL_ROOT_PASSWORD=root -p 3306:3306 -d mysql:8.0
```

#### C. Visual Studio 2022 (Optionnel mais recommandé)
```bash
# Télécharger Community Edition:
https://visualstudio.microsoft.com/downloads/

# Ou utiliser VS Code + C# Extension
```

---

### 2. Configurer la base de données (5 min)

```bash
# Se connecter à MySQL
mysql -u root -p

# Créer la base de données
mysql> source database/schema.sql

# Insérer les données de test
mysql> source database/seed.sql

# Vérifier
mysql> USE blackwoods_compta;
mysql> SELECT COUNT(*) FROM users;
# Devrait retourner: 3

mysql> exit
```

---

### 3. Créer l'API Backend (Option Express.js) (15 min)

#### Installation
```bash
# Créer le dossier de l'API
mkdir api
cd api

# Initialiser le projet Node.js
npm init -y

# Installer les dépendances
npm install express mysql2 bcryptjs jsonwebtoken cors dotenv body-parser
npm install --save-dev nodemon
```

#### Créer server.js
```javascript
const express = require('express');
const mysql = require('mysql2/promise');
const bcrypt = require('bcryptjs');
const jwt = require('jsonwebtoken');
const cors = require('cors');
require('dotenv').config();

const app = express();
const PORT = process.env.PORT || 5000;

// Middleware
app.use(cors());
app.use(express.json());

// Database connection pool
const pool = mysql.createPool({
  host: process.env.DB_HOST || 'localhost',
  user: process.env.DB_USER || 'root',
  password: process.env.DB_PASSWORD || 'root',
  database: process.env.DB_NAME || 'blackwoods_compta',
  waitForConnections: true,
  connectionLimit: 10,
  queueLimit: 0
});

// JWT Secret
const JWT_SECRET = process.env.JWT_SECRET || 'your-secret-key-change-in-production';

// Middleware d'authentification
const authenticate = (req, res, next) => {
  const authHeader = req.headers.authorization;
  
  if (!authHeader || !authHeader.startsWith('Bearer ')) {
    return res.status(401).json({ success: false, message: 'Token manquant' });
  }
  
  const token = authHeader.substring(7);
  
  try {
    const decoded = jwt.verify(token, JWT_SECRET);
    req.user = decoded;
    next();
  } catch (error) {
    return res.status(401).json({ success: false, message: 'Token invalide' });
  }
};

// Routes

// POST /api/auth/login
app.post('/api/auth/login', async (req, res) => {
  try {
    const { username, password } = req.body;
    
    const [users] = await pool.query(
      'SELECT * FROM users WHERE username = ? AND is_active = TRUE',
      [username]
    );
    
    if (users.length === 0) {
      return res.json({ success: false, message: 'Utilisateur non trouvé' });
    }
    
    const user = users[0];
    const passwordMatch = await bcrypt.compare(password, user.password_hash);
    
    if (!passwordMatch) {
      return res.json({ success: false, message: 'Mot de passe incorrect' });
    }
    
    const token = jwt.sign(
      { id: user.id, username: user.username, role: user.role },
      JWT_SECRET,
      { expiresIn: '8h' }
    );
    
    res.json({
      success: true,
      token,
      message: 'Connexion réussie',
      user: {
        id: user.id,
        username: user.username,
        fullName: user.full_name,
        role: user.role,
        email: user.email
      }
    });
  } catch (error) {
    console.error('Login error:', error);
    res.status(500).json({ success: false, message: 'Erreur serveur' });
  }
});

// GET /api/transactions
app.get('/api/transactions', authenticate, async (req, res) => {
  try {
    const { search, type, category } = req.query;
    let query = `
      SELECT t.*, u.full_name as userName 
      FROM transactions t
      JOIN users u ON t.user_id = u.id
      WHERE 1=1
    `;
    const params = [];
    
    if (search) {
      query += ' AND (t.description LIKE ? OR t.category LIKE ?)';
      params.push(`%${search}%`, `%${search}%`);
    }
    
    if (type) {
      query += ' AND t.type = ?';
      params.push(type);
    }
    
    if (category) {
      query += ' AND t.category = ?';
      params.push(category);
    }
    
    query += ' ORDER BY t.created_at DESC LIMIT 100';
    
    const [transactions] = await pool.query(query, params);
    
    res.json({
      success: true,
      data: transactions
    });
  } catch (error) {
    console.error('Get transactions error:', error);
    res.status(500).json({ success: false, message: 'Erreur serveur' });
  }
});

// POST /api/transactions
app.post('/api/transactions', authenticate, async (req, res) => {
  try {
    const { type, category, amount, description, userId } = req.body;
    
    if (!type || !category || !amount) {
      return res.status(400).json({
        success: false,
        message: 'Données manquantes'
      });
    }
    
    const [result] = await pool.query(
      'INSERT INTO transactions (type, category, amount, description, user_id) VALUES (?, ?, ?, ?, ?)',
      [type, category, amount, description || null, userId || req.user.id]
    );
    
    const [newTransaction] = await pool.query(
      'SELECT t.*, u.full_name as userName FROM transactions t JOIN users u ON t.user_id = u.id WHERE t.id = ?',
      [result.insertId]
    );
    
    res.status(201).json({
      success: true,
      message: 'Transaction créée',
      data: newTransaction[0]
    });
  } catch (error) {
    console.error('Create transaction error:', error);
    res.status(500).json({ success: false, message: 'Erreur serveur' });
  }
});

// DELETE /api/transactions/:id
app.delete('/api/transactions/:id', authenticate, async (req, res) => {
  try {
    const { id } = req.params;
    
    await pool.query('DELETE FROM transactions WHERE id = ?', [id]);
    
    res.json({ success: true, message: 'Transaction supprimée' });
  } catch (error) {
    console.error('Delete transaction error:', error);
    res.status(500).json({ success: false, message: 'Erreur serveur' });
  }
});

// GET /api/reports/dashboard
app.get('/api/reports/dashboard', authenticate, async (req, res) => {
  try {
    const [stats] = await pool.query('SELECT * FROM dashboard_stats');
    
    res.json({
      success: true,
      data: {
        totalRevenue: parseFloat(stats[0].total_revenue) || 0,
        totalExpenses: parseFloat(stats[0].total_expenses) || 0,
        netProfit: parseFloat(stats[0].net_profit) || 0,
        transactionCount: stats[0].transaction_count || 0,
        employeeCount: stats[0].employee_count || 0,
        lowStockItemsCount: stats[0].low_stock_items_count || 0,
        pendingInvoicesCount: stats[0].pending_invoices_count || 0,
        revenueChart: [],
        expensesChart: [],
        expensesByCategory: []
      }
    });
  } catch (error) {
    console.error('Dashboard error:', error);
    res.status(500).json({ success: false, message: 'Erreur serveur' });
  }
});

// Health check
app.get('/health', (req, res) => {
  res.json({ status: 'OK', timestamp: new Date().toISOString() });
});

// Démarrer le serveur
app.listen(PORT, () => {
  console.log(`🚀 API Server running on http://localhost:${PORT}`);
  console.log(`📊 Dashboard: http://localhost:${PORT}/health`);
});
```

#### Créer .env
```bash
PORT=5000
DB_HOST=localhost
DB_USER=root
DB_PASSWORD=root
DB_NAME=blackwoods_compta
JWT_SECRET=change-this-to-a-random-secret-key-in-production
```

#### Modifier package.json
```json
{
  "scripts": {
    "start": "node server.js",
    "dev": "nodemon server.js"
  }
}
```

#### Démarrer l'API
```bash
npm run dev

# L'API devrait démarrer sur http://localhost:5000
```

#### Tester l'API
```bash
# Test de santé
curl http://localhost:5000/health

# Test de login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'
```

---

### 4. Lancer l'application WPF (5 min)

```bash
# Ouvrir un nouveau terminal

# Se placer dans le dossier du projet
cd src/BlackWoodsCompta.WPF

# Restaurer les packages
dotnet restore

# Compiler et lancer
dotnet run

# Ou ouvrir la solution dans Visual Studio
# et appuyer sur F5
```

#### Première utilisation
1. L'application s'ouvre sur l'écran de login
2. Dans la configuration (si demandé), entrer l'URL de l'API:
   ```
   http://localhost:5000
   ```
3. Se connecter avec:
   - **Username**: `admin`
   - **Password**: `admin123`

---

## 📋 Checklist de Vérification

### ✅ Base de données
- [ ] MySQL est installé et démarré
- [ ] Base de données `blackwoods_compta` créée
- [ ] Tables créées (9 tables)
- [ ] Données de test insérées (3 users, 5 employees, etc.)

### ✅ API Backend
- [ ] Node.js installé
- [ ] Dépendances installées (`node_modules/`)
- [ ] Fichier `.env` configuré
- [ ] Serveur démarre sans erreur
- [ ] Endpoint `/health` répond
- [ ] Login fonctionne

### ✅ Application WPF
- [ ] .NET 8 SDK installé
- [ ] Solution compile sans erreur
- [ ] Application se lance
- [ ] Écran de login s'affiche
- [ ] Connexion réussie
- [ ] Dashboard s'affiche

---

## 🔍 Tests Rapides

### Test 1: Login
```bash
# Dans Postman ou curl
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}

# Devrait retourner:
# - success: true
# - token: "eyJ..."
# - user: { id, username, role, ... }
```

### Test 2: Get Transactions
```bash
# Copier le token du login ci-dessus
GET http://localhost:5000/api/transactions
Authorization: Bearer {ton_token}

# Devrait retourner:
# - success: true
# - data: [ {...}, {...}, ... ]
```

### Test 3: Create Transaction
```bash
POST http://localhost:5000/api/transactions
Authorization: Bearer {ton_token}
Content-Type: application/json

{
  "type": "Vente",
  "category": "Nourriture",
  "amount": 99.99,
  "description": "Test transaction"
}

# Devrait retourner:
# - success: true
# - data: { id: X, ... }
```

---

## 🐛 Résolution de Problèmes

### Problème: "dotnet command not found"
**Solution**: Installer .NET 8 SDK et redémarrer le terminal

### Problème: "MySQL connection refused"
**Solution**: 
```bash
# Windows
net start MySQL80

# Mac/Linux
sudo service mysql start

# Docker
docker start blackwoods-mysql
```

### Problème: "Port 5000 already in use"
**Solution**: Changer le port dans `.env`:
```bash
PORT=5001
```

### Problème: "Cannot connect to API"
**Solution**: Vérifier que l'API tourne et est accessible
```bash
curl http://localhost:5000/health
```

---

## 📚 Prochaines Étapes

Une fois que tout fonctionne:

1. **Compléter l'API**
   - Ajouter les endpoints pour employees, payrolls, inventory, etc.
   - Voir `docs/api-spec.md` pour la liste complète

2. **Compléter l'Interface**
   - Créer les vues manquantes (Employees, Inventory, etc.)
   - Voir `PROJECT_SUMMARY.md` pour la liste

3. **Tester**
   - Tester toutes les fonctionnalités
   - Ajouter des tests unitaires

4. **Déployer**
   - Créer l'installateur .msi
   - Déployer l'API sur un serveur
   - Configurer la base de données de production

---

## 🎓 Ressources

- **Documentation projet**: Dossier `docs/`
- **Spécification API**: `docs/api-spec.md`
- **Manuel utilisateur**: `docs/user-manual.md`
- **Doc technique**: `docs/technical-doc.md`
- **Résumé projet**: `PROJECT_SUMMARY.md`

---

## 💡 Conseils

1. **Développement**:
   - Utilise `nodemon` pour l'API (rechargement auto)
   - Utilise Hot Reload dans Visual Studio
   - Consulte les logs: `%AppData%\BlackWoodsCompta\Logs\`

2. **Base de données**:
   - Utilise MySQL Workbench pour visualiser les données
   - Fais des sauvegardes régulières
   - Utilise les données de test pour commencer

3. **Debug**:
   - Vérifie les logs de l'API dans la console
   - Vérifie les logs WPF dans le dossier logs
   - Utilise Postman pour tester l'API isolément

---

## ✨ C'est parti !

Tu es maintenant prêt à développer l'application complète ! 🚀

**Bon développement !** 💻
