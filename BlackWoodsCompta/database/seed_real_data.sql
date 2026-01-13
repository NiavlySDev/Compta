-- BlackWoods Compta - Real Data from Excel
-- Données réelles de la comptabilité BlackWoods

USE blackwoods_compta;

-- Utilisateurs réels
INSERT INTO users (username, password_hash, role, full_name, email) VALUES
('admin', '$2a$11$5ZQj6C8Kqg4FvC7H3p8IqOYjP7qKQ8fGvfKHzRjvzU9p8fGvfKHzR', 'Admin', 'Administrateur', 'admin@blackwoods.com'),
('jason', '$2a$11$5ZQj6C8Kqg4FvC7H3p8IqOYjP7qKQ8fGvfKHzRjvzU9p8fGvfKHzR', 'Admin', 'Jason Parker', 'jason@blackwoods.com'),
('anne', '$2a$11$5ZQj6C8Kqg4FvC7H3p8IqOYjP7qKQ8fGvfKHzRjvzU9p8fGvfKHzR', 'Admin', 'Anne Holmes', 'anne@blackwoods.com'),
('chris', '$2a$11$5ZQj6C8Kqg4FvC7H3p8IqOYjP7qKQ8fGvfKHzRjvzU9p8fGvfKHzR', 'Manager', 'Chris Janta', 'chris@blackwoods.com'),
('xavier', '$2a$11$5ZQj6C8Kqg4FvC7H3p8IqOYjP7qKQ8fGvfKHzRjvzU9p8fGvfKHzR', 'Employee', 'Xavier Gordon', 'xavier@blackwoods.com');

-- Employés réels BlackWoods  
INSERT INTO employees (name, position, phone, email) VALUES
('Jason Parker', 'PDG', '37833-51492', 'jason@blackwoods.com'),
('Anne Holmes', 'Co-PDG', '59843-70540', 'anne@blackwoods.com'),
('Chris Janta', 'Manager', '58331-69048', 'chris@blackwoods.com'),
('Xavier Gordon', 'Livreur', '54689-66534', 'xavier@blackwoods.com');

-- Fournisseurs réels avec leurs produits et prix
INSERT INTO suppliers (name, contact_person, phone, email, address, notes) VALUES
('Boucherie Jackland', 'Zia Jackland', '', 'jack@boucherie-jackland.com', '123 Meat Street', 'Fournisseur principal de viandes'),
('Molienda Hermandad', '', '555-1002', 'maria@molienda.com', '456 Mill Road', 'Farines et huiles'),
('Woods Farm', '', '555-1003', 'bob@woodsfarm.com', '789 Farm Lane', 'Légumes, lait, œufs locaux'),
('Theronis Harvest', 'Valandor Theronis', '555-1004', 'sarah@theronis.com', '321 Harvest Ave', 'Légumes frais premium'),

-- Articles d'inventaire avec prix réels
INSERT INTO inventory (product_name, category, quantity, unit, unit_cost, min_quantity, supplier, expiry_date) VALUES
-- Boucherie Jackland
('Steak de boeuf', 'Viandes', 497.00, 'unités', 90.00, 50.00, 'Boucherie Jackland', date('now', '+7 days')),

-- Molienda Hermandad  
('Farine de blé', 'Ingrédients', 498.00, 'kg', 80.00, 100.00, 'Molienda Hermandad', date('now', '+6 months')),
('Farine de maïs', 'Ingrédients', 510.00, 'kg', 80.00, 100.00, 'Molienda Hermandad', date('now', '+6 months')),
('Huile', 'Ingrédients', 575.00, 'litres', 80.00, 50.00, 'Molienda Hermandad', date('now', '+1 year')),

-- Woods Farm
('Oignon', 'Légumes', 532.00, 'kg', 10.00, 50.00, 'Woods Farm', date('now', '+2 weeks')),
('Pommes de terre', 'Légumes', 560.00, 'kg', 10.00, 100.00, 'Woods Farm', date('now', '+1 month')),
('Maïs', 'Légumes', 502.00, 'kg', 10.00, 50.00, 'Woods Farm', date('now', '+2 weeks')),
('Lait', 'Produits Laitiers', 512.00, 'litres', 20.00, 100.00, 'Woods Farm', date('now', '+1 week')),
('Oeuf', 'Produits Laitiers', 571.00, 'unités', 10.00, 200.00, 'Woods Farm', date('now', '+2 weeks')),

-- Theronis Harvest
('Salade', 'Légumes', 491.00, 'kg', 20.00, 30.00, 'Theronis Harvest', date('now', '+1 week')),
('Tomate', 'Légumes', 451.00, 'kg', 20.00, 50.00, 'Theronis Harvest', date('now', '+1 week')),
('Piment', 'Légumes', 451.00, 'kg', 20.00, 20.00, 'Theronis Harvest', date('now', '+2 weeks')),
('Pommes', 'Fruits', 256.00, 'kg', 20.00, 50.00, 'Theronis Harvest', date('now', '+2 weeks')),
('Poivron', 'Légumes', 481.00, 'kg', 20.00, 30.00, 'Theronis Harvest', date('now', '+2 weeks')),

-- Production Black Woods
('Tortillas', 'Produits Finis', 95.00, 'unités', 0.00, 50.00, 'Black Woods', date('now', '+1 week')),
('Tacos', 'Plats Préparés', 0.00, 'unités', 0.00, 0.00, 'Black Woods', date('now', '+1 week')),
('Burritos', 'Plats Préparés', 0.00, 'unités', 0.00, 0.00, 'Black Woods', date('now', '+1 week')),
('Frites', 'Plats Préparés', 0.00, 'unités', 0.00, 0.00, 'Black Woods', date('now', '+1 week')),
('Tartes aux pommes', 'Plats Préparés', 0.00, 'unités', 0.00, 0.00, 'Black Woods', date('now', '+1 week'));

-- Transactions réelles de dépenses (du fichier depenses.txt)
INSERT INTO transactions (type, category, amount, description, user_id, transaction_date, reference) VALUES
-- Dépenses 11/01/2026
('Depense', 'Achats Viandes', 27000.00, 'Steak de boeuf - 300 unités à 90.00$ - Boucherie Jackland - Récupéré par Jason Parker', 2, '2026-01-11 19:00:00', 'DEP-2026-001'),

-- Dépenses 12/01/2026 - Molienda Hermandad
('Depense', 'Achats Ingrédients', 4000.00, 'Farine de maïs - 50 kg à 80.00$ - Molienda Hermandad - Récupéré par Jason Parker', 2, '2026-01-12 17:23:33', 'DEP-2026-002'),
('Depense', 'Achats Ingrédients', 16000.00, 'Huile - 200 litres à 80.00$ - Molienda Hermandad - Récupéré par Jason Parker', 2, '2026-01-12 17:23:33', 'DEP-2026-003'),

-- Dépenses 12/01/2026 - Woods Farm (Anne Holmes à rembourser)
('Depense', 'Achats Légumes', 1000.00, 'Oignon - 100 kg à 10.00$ - Woods Farm - Récupéré par Anne Holmes - À REMBOURSER', 3, '2026-01-12 21:00:00', 'DEP-2026-004'),
('Depense', 'Achats Légumes', 3000.00, 'Pommes de terre - 300 kg à 10.00$ - Woods Farm - Récupéré par Anne Holmes - À REMBOURSER', 3, '2026-01-12 21:00:00', 'DEP-2026-005'),
('Depense', 'Achats Légumes', 2000.00, 'Maïs - 200 kg à 10.00$ - Woods Farm - Récupéré par Anne Holmes - À REMBOURSER', 3, '2026-01-12 21:00:00', 'DEP-2026-006'),
('Depense', 'Achats Produits Laitiers', 6000.00, 'Lait - 300 litres à 20.00$ - Woods Farm - Récupéré par Anne Holmes - À REMBOURSER', 3, '2026-01-12 21:00:00', 'DEP-2026-007'),
('Depense', 'Achats Produits Laitiers', 4000.00, 'Oeuf - 400 unités à 10.00$ - Woods Farm - Récupéré par Anne Holmes - À REMBOURSER', 3, '2026-01-12 21:00:00', 'DEP-2026-008'),
('Depense', 'Frais de Livraison', 1000.00, 'Frais Livraison Woods Farm - À REMBOURSER à Anne Holmes', 3, '2026-01-12 21:00:00', 'DEP-2026-009');

-- Transactions réelles de recettes (du fichier recettes.txt)
INSERT INTO transactions (type, category, amount, description, user_id, transaction_date, reference) VALUES
-- Recettes 12/01/2026 par Jason Parker
('Vente', 'Plats Principaux', 3475.00, 'Burritos - 5 unités vendues à 695.00$ - Servies par Jason Parker', 2, '2026-01-12 19:50:00', 'VTE-2026-001'),
('Vente', 'Plats Principaux', 8206.00, 'Tacos - 11 unités vendues à 746.00$ - Servies par Jason Parker', 2, '2026-01-12 19:50:00', 'VTE-2026-002');

-- Commandes en cours (du fichier stock.txt)
INSERT INTO orders (supplier_name, order_date, expected_delivery_date, status, total_amount, notes) VALUES
-- Commandes chez Theronis Harvest (En Production)
('Theronis Harvest', '2026-01-10', '2026-01-16', 'En_Production', 15500.00, 'Salade 100kg, Tomate 100kg, Piment 100kg, Pommes 300kg, Poivron 100kg + Frais livraison'),

-- Commandes récentes livrées
('Boucherie Jackland', '2026-01-11', '2026-01-11', 'Livree', 27000.00, 'Steak de bœuf 300 unités'),
('Molienda Hermandad', '2026-01-12', '2026-01-12', 'Livree', 20000.00, 'Farine de maïs 50kg, Huile 200L'),
('Woods Farm', '2026-01-12', '2026-01-12', 'Livree', 17000.00, 'Oignon 100kg, Pommes de terre 300kg, Maïs 200kg, Lait 300L, Œufs 400 unités + Frais');

-- Articles des commandes détaillées
INSERT INTO order_items (order_id, product_name, quantity, unit_price, total_price) VALUES
-- Theronis Harvest (commande #1)
(1, 'Salade', 100.00, 20.00, 2000.00),
(1, 'Tomate', 100.00, 20.00, 2000.00),
(1, 'Piment', 100.00, 20.00, 2000.00),
(1, 'Pommes', 300.00, 20.00, 6000.00),
(1, 'Poivron', 100.00, 20.00, 2000.00),
(1, 'Frais Livraison', 1.00, 1500.00, 1500.00),

-- Boucherie Jackland (commande #2 - livrée)
(2, 'Steak de boeuf', 300.00, 90.00, 27000.00),

-- Molienda Hermandad (commande #3 - livrée)  
(3, 'Farine de maïs', 50.00, 80.00, 4000.00),
(3, 'Huile', 200.00, 80.00, 16000.00),

-- Woods Farm (commande #4 - livrée)
(4, 'Oignon', 100.00, 10.00, 1000.00),
(4, 'Pommes de terre', 300.00, 10.00, 3000.00),
(4, 'Maïs', 200.00, 10.00, 2000.00),
(4, 'Lait', 300.00, 20.00, 6000.00),
(4, 'Oeuf', 400.00, 10.00, 4000.00),
(4, 'Frais Livraison', 1.00, 1000.00, 1000.00);

-- Mouvements d'inventaire basés sur les vraies données
INSERT INTO inventory_movements (product_id, quantity, type, reason, user_id, movement_date) VALUES
-- Entrées suite aux livraisons
(1, 300.00, 'Entree', 'Livraison Boucherie Jackland - 11/01/2026', 3, '2026-01-11 19:00:00'),
(3, 50.00, 'Entree', 'Livraison Molienda Hermandad - 12/01/2026', 2, '2026-01-12 17:23:33'),
(4, 200.00, 'Entree', 'Livraison Molienda Hermandad - 12/01/2026', 2, '2026-01-12 17:23:33'),
(5, 100.00, 'Entree', 'Livraison Woods Farm - 12/01/2026', 3, '2026-01-12 21:00:00'),
(6, 300.00, 'Entree', 'Livraison Woods Farm - 12/01/2026', 3, '2026-01-12 21:00:00'),
(7, 200.00, 'Entree', 'Livraison Woods Farm - 12/01/2026', 3, '2026-01-12 21:00:00'),
(8, 300.00, 'Entree', 'Livraison Woods Farm - 12/01/2026', 3, '2026-01-12 21:00:00'),
(9, 400.00, 'Entree', 'Livraison Woods Farm - 12/01/2026', 3, '2026-01-12 21:00:00'),

-- Sorties pour production de plats
(1, 16.00, 'Sortie', 'Production Burritos et Tacos - 12/01/2026', 2, '2026-01-12 19:50:00'),
(16, 16.00, 'Sortie', 'Utilisation Tortillas pour Burritos et Tacos', 2, '2026-01-12 19:50:00');

-- Remboursements dus (basé sur remboursements.txt)
INSERT INTO employee_reimbursements (employee_id, amount, description, request_date, status, transaction_ids) VALUES
(2, 17000.00, 'Remboursement achats personnels Woods Farm du 12/01/2026: Oignon 1000$, Pommes de terre 3000$, Maïs 2000$, Lait 6000$, Œufs 4000$, Frais livraison 1000$', '2026-01-12', 'En_Attente', 'DEP-2026-004,DEP-2026-005,DEP-2026-006,DEP-2026-007,DEP-2026-008,DEP-2026-009');

-- Recettes avec ingrédients (basé sur ingredients.txt)
INSERT INTO recipes (name, category, selling_price, cost_per_unit, preparation_time, servings) VALUES
('Tacos', 'Plats Principaux', 746.00, 0.00, 15, 1),
('Burritos', 'Plats Principaux', 695.00, 0.00, 20, 1),
('Frites', 'Accompagnements', 162.00, 0.00, 10, 1),
('Tartes aux pommes', 'Desserts', 270.00, 0.00, 45, 1);

-- Ingrédients des recettes
INSERT INTO recipe_ingredients (recipe_id, ingredient_name, quantity, unit) VALUES
-- Tacos: Steak de bœuf, Tortillas, Salade, Tomate, Oignon, Piment
(1, 'Steak de boeuf', 1.00, 'unité'),
(1, 'Tortillas', 2.00, 'unités'),
(1, 'Salade', 0.05, 'kg'),
(1, 'Tomate', 0.05, 'kg'),
(1, 'Oignon', 0.02, 'kg'),
(1, 'Piment', 0.01, 'kg'),

-- Burritos: Steak de bœuf, Tortillas, Poivron, Oignons, Maïs  
(2, 'Steak de boeuf', 1.00, 'unité'),
(2, 'Tortillas', 2.00, 'unités'),
(2, 'Poivron', 0.05, 'kg'),
(2, 'Oignon', 0.03, 'kg'),
(2, 'Maïs', 0.05, 'kg'),

-- Frites: Pommes de terre, Huile
(3, 'Pommes de terre', 0.25, 'kg'),
(3, 'Huile', 0.05, 'litres'),

-- Tartes aux pommes: Lait, Pommes, Farine, Œufs
(4, 'Lait', 0.2, 'litres'),
(4, 'Pommes', 0.3, 'kg'),
(4, 'Farine de blé', 0.15, 'kg'),
(4, 'Oeuf', 2.00, 'unités');

-- Audit logs des opérations réelles
INSERT INTO audit_logs (user_id, action, entity, entity_id, details) VALUES
(3, 'PURCHASE', 'Transaction', 1, 'Achat Steak de boeuf - 27000$ - Boucherie Jackland'),
(2, 'PURCHASE', 'Transaction', 2, 'Achat Farine de maïs - 4000$ - Molienda Hermandad'),  
(2, 'PURCHASE', 'Transaction', 3, 'Achat Huile - 16000$ - Molienda Hermandad'),
(3, 'PURCHASE', 'Transaction', 4, 'Achat Oignon - 1000$ - Woods Farm - À rembourser Anne'),
(3, 'PURCHASE', 'Transaction', 5, 'Achat Pommes de terre - 3000$ - Woods Farm - À rembourser Anne'),
(2, 'SALE', 'Transaction', 10, 'Vente Burritos - 3475$ - Jason Parker'),
(2, 'SALE', 'Transaction', 11, 'Vente Tacos - 8206$ - Jason Parker');

-- Prix de vente des produits finis
INSERT INTO product_prices (product_name, category, selling_price, cost_price, margin_percent, last_updated) VALUES
('Tacos', 'Plats Principaux', 746.00, 0.00, 0.00, datetime('now')),
('Burritos', 'Plats Principaux', 695.00, 0.00, 0.00, datetime('now')),
('Frites', 'Accompagnements', 162.00, 0.00, 0.00, datetime('now')),
('Tartes aux pommes', 'Desserts', 270.00, 0.00, 0.00, datetime('now'));