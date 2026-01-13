-- BlackWoods Compta - Seed Data
-- Test data for development

USE blackwoods_compta;

-- Insert admin user (password: admin123)
-- Note: Password hash should be generated using BCrypt in production
INSERT INTO users (username, password_hash, role, full_name, email) VALUES
('admin', '$2a$11$5ZQj6C8Kqg4FvC7H3p8IqOYjP7qKQ8fGvfKHzRjvzU9p8fGvfKHzR', 'Admin', 'Administrateur', 'admin@blackwoods.com'),
('manager', '$2a$11$5ZQj6C8Kqg4FvC7H3p8IqOYjP7qKQ8fGvfKHzRjvzU9p8fGvfKHzR', 'Manager', 'Chef Manager', 'manager@blackwoods.com'),
('employe1', '$2a$11$5ZQj6C8Kqg4FvC7H3p8IqOYjP7qKQ8fGvfKHzRjvzU9p8fGvfKHzR', 'Employe', 'Jean Dupont', 'jean@blackwoods.com');

-- Insert sample employees
INSERT INTO employees (name, position, salary, hire_date, phone, email) VALUES
('Marie Martin', 'Serveuse', 2500.00, '2023-01-15', '555-0101', 'marie@blackwoods.com'),
('Pierre Bernard', 'Cuisinier', 3200.00, '2023-02-01', '555-0102', 'pierre@blackwoods.com'),
('Sophie Laurent', 'Bartender', 2800.00, '2023-03-10', '555-0103', 'sophie@blackwoods.com'),
('Thomas Dubois', 'Serveur', 2500.00, '2023-04-20', '555-0104', 'thomas@blackwoods.com'),
('Julie Moreau', 'Chef Cuisinier', 4000.00, '2023-01-05', '555-0105', 'julie@blackwoods.com');

-- Insert sample transactions
INSERT INTO transactions (type, category, amount, description, user_id) VALUES
('Vente', 'Nourriture', 1250.50, 'Ventes du midi - 15 tables', 1),
('Vente', 'Boissons', 580.00, 'Ventes bar', 1),
('Depense', 'Fournitures', 350.75, 'Achat de produits frais', 1),
('Vente', 'Nourriture', 890.00, 'Ventes du soir', 2),
('Depense', 'Électricité', 220.00, 'Facture électricité', 1),
('Vente', 'Boissons', 420.50, 'Ventes bar soirée', 2),
('Depense', 'Entretien', 150.00, 'Nettoyage professionnel', 1),
('Vente', 'Nourriture', 1580.00, 'Événement privé', 1);

-- Insert sample inventory items
INSERT INTO inventory (product_name, category, quantity, unit, unit_cost, min_quantity, supplier) VALUES
('Farine', 'Ingrédients', 50.00, 'kg', 1.50, 10.00, 'Fournisseur A'),
('Tomates', 'Légumes', 25.00, 'kg', 2.50, 5.00, 'Marché Local'),
('Poulet', 'Viandes', 30.00, 'kg', 8.00, 10.00, 'Fournisseur B'),
('Vin Rouge', 'Boissons', 48.00, 'bouteilles', 12.00, 12.00, 'Cave du Roi'),
('Bière', 'Boissons', 120.00, 'bouteilles', 3.50, 24.00, 'Brasserie Locale'),
('Fromage', 'Produits Laitiers', 15.00, 'kg', 15.00, 5.00, 'Fromagerie'),
('Pain', 'Boulangerie', 40.00, 'unités', 2.00, 10.00, 'Boulangerie du Coin'),
('Salade', 'Légumes', 20.00, 'kg', 3.00, 5.00, 'Marché Local');

-- Insert sample inventory movements
INSERT INTO inventory_movements (product_id, quantity, type, reason, user_id) VALUES
(1, 20.00, 'Entree', 'Réapprovisionnement', 1),
(2, 5.00, 'Sortie', 'Utilisation cuisine', 1),
(3, 10.00, 'Entree', 'Livraison hebdomadaire', 1),
(4, 12.00, 'Sortie', 'Ventes', 2),
(5, 24.00, 'Sortie', 'Ventes bar', 2);

-- Insert sample invoices
INSERT INTO invoices (invoice_number, client_name, client_phone, client_email, total_amount, status, issue_date, due_date, created_by) VALUES
('INV-2024-001', 'Restaurant Le Gourmet', '555-1001', 'contact@legourmet.com', 2500.00, 'Payee', '2024-01-10', '2024-02-10', 1),
('INV-2024-002', 'Hôtel Luxe', '555-1002', 'events@hotelluxe.com', 5000.00, 'Envoyee', '2024-01-15', '2024-02-15', 1),
('INV-2024-003', 'Entreprise Tech', '555-1003', 'admin@tech.com', 1800.00, 'Brouillon', '2024-01-20', '2024-02-20', 2);

-- Insert sample invoice items
INSERT INTO invoice_items (invoice_id, description, quantity, unit_price, total_price) VALUES
(1, 'Menu 3 services pour 50 personnes', 50.00, 45.00, 2250.00),
(1, 'Boissons premium', 1.00, 250.00, 250.00),
(2, 'Buffet gastronomique pour 100 personnes', 100.00, 48.00, 4800.00),
(2, 'Service traiteur', 1.00, 200.00, 200.00),
(3, 'Déjeuner d\'affaires pour 30 personnes', 30.00, 60.00, 1800.00);

-- Insert sample audit logs
INSERT INTO audit_logs (user_id, action, entity, entity_id, details) VALUES
(1, 'CREATE', 'Transaction', 1, 'Created new transaction'),
(1, 'CREATE', 'Employee', 1, 'Added new employee'),
(2, 'UPDATE', 'Inventory', 1, 'Updated inventory quantity'),
(1, 'CREATE', 'Invoice', 1, 'Created new invoice'),
(2, 'DELETE', 'Transaction', 5, 'Deleted transaction');

-- Insert sample payrolls
INSERT INTO payrolls (employee_id, amount, period_start, period_end, paid_date, notes, created_by) VALUES
(1, 2500.00, '2024-01-01', '2024-01-31', '2024-02-01', 'Salaire janvier 2024', 1),
(2, 3200.00, '2024-01-01', '2024-01-31', '2024-02-01', 'Salaire janvier 2024', 1),
(3, 2800.00, '2024-01-01', '2024-01-31', '2024-02-01', 'Salaire janvier 2024', 1),
(4, 2500.00, '2024-01-01', '2024-01-31', '2024-02-01', 'Salaire janvier 2024', 1),
(5, 4000.00, '2024-01-01', '2024-01-31', '2024-02-01', 'Salaire janvier 2024', 1);
