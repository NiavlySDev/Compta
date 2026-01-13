# API REST - BlackWoods Compta

## Base URL
```
http://localhost:5000/api
```

## Authentication
L'API utilise JWT (JSON Web Tokens) pour l'authentification. Tous les endpoints (sauf `/auth/login`) nécessitent un token Bearer dans l'en-tête Authorization.

```
Authorization: Bearer <token>
```

---

## Endpoints

### Authentication

#### POST /auth/login
Authentifie un utilisateur et retourne un token JWT.

**Request Body:**
```json
{
  "username": "admin",
  "password": "admin123"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "message": "Connexion réussie",
  "user": {
    "id": 1,
    "username": "admin",
    "fullName": "Administrateur",
    "role": "Admin",
    "email": "admin@blackwoods.com"
  }
}
```

---

### Transactions

#### GET /transactions
Récupère la liste des transactions.

**Query Parameters:**
- `search` (optional): Recherche par description ou catégorie
- `type` (optional): Filtre par type (Vente, Depense)
- `category` (optional): Filtre par catégorie
- `startDate` (optional): Date de début (format: YYYY-MM-DD)
- `endDate` (optional): Date de fin (format: YYYY-MM-DD)

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "type": "Vente",
      "category": "Nourriture",
      "amount": 1250.50,
      "description": "Ventes du midi",
      "reference": "REF-001",
      "userId": 1,
      "userName": "Administrateur",
      "createdAt": "2024-01-13T10:30:00Z",
      "updatedAt": "2024-01-13T10:30:00Z"
    }
  ]
}
```

#### POST /transactions
Crée une nouvelle transaction.

**Request Body:**
```json
{
  "type": "Vente",
  "category": "Nourriture",
  "amount": 1250.50,
  "description": "Ventes du midi",
  "reference": "REF-001",
  "userId": 1
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "message": "Transaction créée avec succès",
  "data": {
    "id": 10,
    "type": "Vente",
    "category": "Nourriture",
    "amount": 1250.50,
    ...
  }
}
```

#### PUT /transactions/{id}
Met à jour une transaction existante.

**Request Body:** (même structure que POST)

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Transaction mise à jour",
  "data": { ... }
}
```

#### DELETE /transactions/{id}
Supprime une transaction.

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Transaction supprimée"
}
```

---

### Employees

#### GET /employees
Récupère la liste des employés.

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "Marie Martin",
      "position": "Serveuse",
      "salary": 2500.00,
      "hireDate": "2023-01-15",
      "phone": "555-0101",
      "email": "marie@blackwoods.com",
      "isActive": true,
      "createdAt": "2023-01-15T00:00:00Z",
      "updatedAt": "2023-01-15T00:00:00Z"
    }
  ]
}
```

#### POST /employees
Crée un nouvel employé.

**Request Body:**
```json
{
  "name": "Jean Dupont",
  "position": "Cuisinier",
  "salary": 3000.00,
  "hireDate": "2024-01-15",
  "phone": "555-0110",
  "email": "jean@blackwoods.com"
}
```

#### PUT /employees/{id}
Met à jour un employé.

#### DELETE /employees/{id}
Désactive un employé (soft delete).

---

### Payrolls

#### GET /payrolls
Récupère les paies.

**Query Parameters:**
- `employeeId` (optional): Filtre par employé
- `startDate` (optional): Date de début
- `endDate` (optional): Date de fin

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "employeeId": 1,
      "employeeName": "Marie Martin",
      "amount": 2500.00,
      "periodStart": "2024-01-01",
      "periodEnd": "2024-01-31",
      "paidDate": "2024-02-01",
      "notes": "Salaire janvier 2024",
      "createdBy": 1,
      "createdByName": "Administrateur",
      "createdAt": "2024-02-01T00:00:00Z"
    }
  ]
}
```

#### POST /payrolls
Crée une nouvelle paie.

**Request Body:**
```json
{
  "employeeId": 1,
  "amount": 2500.00,
  "periodStart": "2024-01-01",
  "periodEnd": "2024-01-31",
  "paidDate": "2024-02-01",
  "notes": "Salaire janvier",
  "createdBy": 1
}
```

---

### Inventory

#### GET /inventory
Récupère les articles d'inventaire.

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "productName": "Farine",
      "category": "Ingrédients",
      "quantity": 50.00,
      "unit": "kg",
      "unitCost": 1.50,
      "minQuantity": 10.00,
      "supplier": "Fournisseur A",
      "isLowStock": false,
      "createdAt": "2024-01-01T00:00:00Z",
      "updatedAt": "2024-01-13T00:00:00Z"
    }
  ]
}
```

#### POST /inventory
Ajoute un article d'inventaire.

#### PUT /inventory/{id}
Met à jour un article d'inventaire.

#### DELETE /inventory/{id}
Supprime un article d'inventaire.

#### GET /inventory/movements
Récupère l'historique des mouvements.

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "productId": 1,
      "productName": "Farine",
      "quantity": 20.00,
      "type": "Entree",
      "reason": "Réapprovisionnement",
      "userId": 1,
      "userName": "Administrateur",
      "createdAt": "2024-01-10T00:00:00Z"
    }
  ]
}
```

#### POST /inventory/movements
Enregistre un mouvement d'inventaire.

**Request Body:**
```json
{
  "productId": 1,
  "quantity": 20.00,
  "type": "Entree",
  "reason": "Réapprovisionnement",
  "userId": 1
}
```

---

### Invoices

#### GET /invoices
Récupère les factures.

**Query Parameters:**
- `status` (optional): Filtre par statut
- `search` (optional): Recherche par numéro ou client

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "invoiceNumber": "INV-2024-001",
      "clientName": "Restaurant Le Gourmet",
      "clientPhone": "555-1001",
      "clientEmail": "contact@legourmet.com",
      "totalAmount": 2500.00,
      "status": "Payee",
      "issueDate": "2024-01-10",
      "dueDate": "2024-02-10",
      "notes": "",
      "createdBy": 1,
      "createdByName": "Administrateur",
      "createdAt": "2024-01-10T00:00:00Z",
      "updatedAt": "2024-01-10T00:00:00Z",
      "items": [
        {
          "id": 1,
          "invoiceId": 1,
          "description": "Menu 3 services pour 50 personnes",
          "quantity": 50.00,
          "unitPrice": 45.00,
          "totalPrice": 2250.00,
          "createdAt": "2024-01-10T00:00:00Z"
        }
      ]
    }
  ]
}
```

#### POST /invoices
Crée une nouvelle facture.

**Request Body:**
```json
{
  "clientName": "Restaurant Le Gourmet",
  "clientPhone": "555-1001",
  "clientEmail": "contact@legourmet.com",
  "status": "Brouillon",
  "issueDate": "2024-01-10",
  "dueDate": "2024-02-10",
  "notes": "Événement spécial",
  "createdBy": 1,
  "items": [
    {
      "description": "Menu 3 services",
      "quantity": 50,
      "unitPrice": 45.00
    }
  ]
}
```

#### PUT /invoices/{id}
Met à jour une facture.

#### DELETE /invoices/{id}
Supprime une facture.

---

### Reports

#### GET /reports/dashboard
Récupère les données du tableau de bord.

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "totalRevenue": 15250.50,
    "totalExpenses": 5420.75,
    "netProfit": 9829.75,
    "transactionCount": 145,
    "employeeCount": 8,
    "lowStockItemsCount": 3,
    "pendingInvoicesCount": 5,
    "revenueChart": [
      { "label": "Jan", "value": 5200.00 },
      { "label": "Fév", "value": 6100.00 },
      { "label": "Mar", "value": 3950.50 }
    ],
    "expensesChart": [
      { "label": "Jan", "value": 1800.00 },
      { "label": "Fév", "value": 2020.75 },
      { "label": "Mar", "value": 1600.00 }
    ],
    "expensesByCategory": [
      { "category": "Fournitures", "amount": 2500.00, "percentage": 46.1 },
      { "category": "Salaires", "amount": 1800.00, "percentage": 33.2 },
      { "category": "Loyer", "amount": 1120.75, "percentage": 20.7 }
    ]
  }
}
```

#### GET /reports/period
Génère un rapport pour une période donnée.

**Query Parameters:**
- `startDate` (required): Date de début (YYYY-MM-DD)
- `endDate` (required): Date de fin (YYYY-MM-DD)
- `format` (optional): Format de sortie (json, pdf, excel)

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "period": {
      "start": "2024-01-01",
      "end": "2024-01-31"
    },
    "summary": {
      "totalRevenue": 5200.00,
      "totalExpenses": 1800.00,
      "netProfit": 3400.00,
      "transactionCount": 45
    },
    "transactions": [ ... ],
    "topCategories": [ ... ]
  }
}
```

---

### Audit Logs

#### GET /logs
Récupère les logs d'audit.

**Query Parameters:**
- `userId` (optional): Filtre par utilisateur
- `action` (optional): Filtre par action
- `entity` (optional): Filtre par entité
- `startDate` (optional): Date de début
- `endDate` (optional): Date de fin

**Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "userId": 1,
      "userName": "Administrateur",
      "action": "CREATE",
      "entity": "Transaction",
      "entityId": 1,
      "details": "Created new transaction",
      "ipAddress": "192.168.1.100",
      "timestamp": "2024-01-13T10:30:00Z"
    }
  ]
}
```

---

## Error Responses

### 400 Bad Request
```json
{
  "success": false,
  "message": "Données invalides",
  "errors": [
    "Le montant doit être supérieur à 0",
    "La catégorie est requise"
  ]
}
```

### 401 Unauthorized
```json
{
  "success": false,
  "message": "Token invalide ou expiré"
}
```

### 403 Forbidden
```json
{
  "success": false,
  "message": "Vous n'avez pas les permissions nécessaires"
}
```

### 404 Not Found
```json
{
  "success": false,
  "message": "Ressource introuvable"
}
```

### 500 Internal Server Error
```json
{
  "success": false,
  "message": "Erreur serveur interne"
}
```

---

## Notes d'implémentation

1. **Sécurité**
   - Tous les mots de passe doivent être hashés avec BCrypt (cost factor: 11)
   - Les tokens JWT expirent après 8 heures
   - Rate limiting: 100 requêtes par minute par IP

2. **Pagination**
   - Les endpoints retournant des listes supportent la pagination
   - Paramètres: `page` (default: 1), `pageSize` (default: 50, max: 100)

3. **Formats de date**
   - Les dates sont au format ISO 8601 (YYYY-MM-DDTHH:mm:ssZ)
   - Timezone: UTC

4. **Validation**
   - Toutes les entrées doivent être validées côté serveur
   - Les montants ne peuvent pas être négatifs
   - Les emails doivent être au format valide

5. **CORS**
   - L'API doit accepter les requêtes depuis l'application cliente
   - Headers autorisés: Authorization, Content-Type
