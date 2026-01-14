# -*- coding: utf-8 -*-
"""
Script d'import des données d'exemple dans la base SQLite BlackWoods Compta
"""

import sqlite3
import os
import re
from pathlib import Path
from datetime import datetime

def parse_amount(amount_str):
    """Parse un montant au format '$1 000,00' ou '$1000.00'"""
    if not amount_str:
        return 0.0
    # Enlever le symbole $ et les espaces
    cleaned = amount_str.replace('$', '').replace(' ', '').strip()
    # Remplacer la virgule par un point
    cleaned = cleaned.replace(',', '.')
    try:
        return float(cleaned)
    except:
        return 0.0

def parse_date(date_str):
    """Parse une date au format 'dd/MM/yyyy'"""
    if not date_str or not date_str.strip():
        return None
    try:
        return datetime.strptime(date_str, '%d/%m/%Y').strftime('%Y-%m-%d')
    except:
        return None

def import_data():
    print("=== Import des donnees BlackWoods Compta ===\n")
    
    # Chemins
    appdata = os.getenv('APPDATA')
    db_path = Path(appdata) / 'BlackWoodsCompta' / 'Data' / 'blackwoods.db'
    project_root = Path(__file__).parent
    examples_path = project_root / 'docs' / 'Exemple'
    
    print(f"Base de donnees: {db_path}")
    print(f"Exemples: {examples_path}\n")
    
    if not db_path.exists():
        print("ERREUR: La base de donnees n'existe pas encore!")
        print("Veuillez lancer l'application une fois avant d'importer les donnees\n")
        return False
    
    if not examples_path.exists():
        print(f"ERREUR: Dossier d'exemples non trouve: {examples_path}\n")
        return False
    
    # Connexion à la base
    conn = sqlite3.connect(str(db_path))
    cursor = conn.cursor()
    
    try:
        # Import des employés
        print("Import des employes...")
        effectif_file = examples_path / 'effectif.txt'
        if effectif_file.exists():
            with open(effectif_file, 'r', encoding='utf-8') as f:
                lines = f.readlines()[1:]  # Skip header
                imported = 0
                for line in lines:
                    parts = line.strip().split('\t')
                    if len(parts) < 7 or not parts[0] or parts[0] == 'Woods':
                        continue
                    
                    name = f"{parts[1]} {parts[0]}".strip()
                    id_rp = parts[2] if len(parts) > 2 else ''
                    phone = parts[3] if len(parts) > 3 else ''
                    discord = parts[5] if len(parts) > 5 else ''
                    position = parts[6] if len(parts) > 6 else 'Employe'
                    
                    # Déterminer le salaire
                    salary = {
                        'PDG': 5000, 'Co-PDG': 5000,
                        'Manager': 3000, 'Livreur': 1500,
                        'Technicien': 2000
                    }.get(position, 1200)
                    
                    cursor.execute("""
                        INSERT OR IGNORE INTO employees (name, position, salary, hire_date, phone, discord, id_rp, is_active)
                        VALUES (?, ?, ?, ?, ?, ?, ?, 1)
                    """, (name, position, salary, datetime.now().strftime('%Y-%m-%d'), phone, discord, id_rp))
                    
                    if cursor.rowcount > 0:
                        imported += 1
                
                print(f"  -> {imported} employes importes")
        
        # Import des fournisseurs
        print("Import des fournisseurs...")
        prix_achat_file = examples_path / 'prix_achat.txt'
        if prix_achat_file.exists():
            suppliers = set()
            with open(prix_achat_file, 'r', encoding='utf-8') as f:
                lines = f.readlines()[1:]  # Skip header
                for line in lines:
                    parts = line.strip().split('\t')
                    if len(parts) >= 2 and parts[1]:
                        suppliers.add(parts[1].strip())
            
            imported = 0
            for supplier in suppliers:
                cursor.execute("""
                    INSERT OR IGNORE INTO suppliers (name, contact_person, phone, email, is_active)
                    VALUES (?, ?, '', '', 1)
                """, (supplier, supplier))
                if cursor.rowcount > 0:
                    imported += 1
            
            print(f"  -> {imported} fournisseurs importes")
        
        # Import de l'inventaire
        print("Import de l'inventaire...")
        stock_file = examples_path / 'stock.txt'
        if stock_file.exists():
            with open(stock_file, 'r', encoding='utf-8') as f:
                lines = f.readlines()[1:]  # Skip header
                imported = 0
                for line in lines:
                    parts = line.strip().split('\t')
                    if len(parts) < 5 or not parts[0]:
                        continue
                    
                    product_name = parts[0].strip()
                    supplier = parts[1] if len(parts) > 1 else ''
                    unit_price = parse_amount(parts[2] if len(parts) > 2 else '0')
                    quantity = float(parts[4]) if len(parts) > 4 and parts[4].strip() else 0
                    
                    category = 'Service' if 'livraison' in product_name.lower() or 'frais' in product_name.lower() else 'Matiere premiere'
                    
                    cursor.execute("""
                        INSERT OR IGNORE INTO inventory (product_name, category, quantity, unit, unit_price, low_stock_threshold, supplier)
                        VALUES (?, ?, ?, 'unite', ?, 50, ?)
                    """, (product_name, category, quantity, unit_price, supplier))
                    
                    if cursor.rowcount > 0:
                        imported += 1
                
                print(f"  -> {imported} articles importes")
        
        # Import des prix d'achat
        print("Import des prix d'achat...")
        if prix_achat_file.exists():
            with open(prix_achat_file, 'r', encoding='utf-8') as f:
                lines = f.readlines()[1:]
                imported = 0
                for line in lines:
                    parts = line.strip().split('\t')
                    if len(parts) >= 3 and parts[0]:
                        cursor.execute("""
                            INSERT OR IGNORE INTO purchase_prices (product_name, supplier_name, unit_price, effective_date, is_current)
                            VALUES (?, ?, ?, ?, 1)
                        """, (parts[0].strip(), parts[1].strip(), parse_amount(parts[2]), datetime.now().strftime('%Y-%m-%d')))
                        if cursor.rowcount > 0:
                            imported += 1
                print(f"  -> {imported} prix d'achat importes")
        
        # Import des prix de vente
        print("Import des prix de vente...")
        prix_vente_file = examples_path / 'prix_vente.txt'
        if prix_vente_file.exists():
            with open(prix_vente_file, 'r', encoding='utf-8') as f:
                lines = f.readlines()[1:]
                imported = 0
                for line in lines:
                    parts = line.strip().split('\t')
                    if len(parts) >= 2 and parts[0] and parts[0] != 'Aucun':
                        price = parse_amount(parts[1])
                        if price > 0:
                            cursor.execute("""
                                INSERT OR IGNORE INTO sale_prices (product_name, unit_price, effective_date, is_current)
                                VALUES (?, ?, ?, 1)
                            """, (parts[0].strip(), price, datetime.now().strftime('%Y-%m-%d')))
                            if cursor.rowcount > 0:
                                imported += 1
                print(f"  -> {imported} prix de vente importes")
        
        # Import des transactions (recettes)
        print("Import des transactions (recettes)...")
        recettes_file = examples_path / 'recettes.txt'
        if recettes_file.exists():
            with open(recettes_file, 'r', encoding='utf-8') as f:
                lines = f.readlines()[1:]
                imported = 0
                for i, line in enumerate(lines):
                    parts = line.strip().split('\t')
                    if len(parts) >= 7 and parts[0] and parts[2] != 'Aucun':
                        date = parse_date(parts[0]) or datetime.now().strftime('%Y-%m-%d')
                        product = parts[2].strip()
                        quantity = int(parts[4]) if len(parts) > 4 and parts[4].strip() else 0
                        amount = parse_amount(parts[6] if len(parts) > 6 else '0')
                        
                        if amount > 0 and quantity > 0:
                            cursor.execute("""
                                INSERT INTO transactions (type, category, amount, description, reference, user_id, created_at)
                                VALUES ('Vente', 'Vente Restaurant', ?, ?, ?, 1, ?)
                            """, (amount, f"{quantity}x {product}", f"V-{date.replace('-', '')}-{i:04d}", f"{date} 12:00:00"))
                            if cursor.rowcount > 0:
                                imported += 1
                print(f"  -> {imported} recettes importees")
        
        # Import des transactions (dépenses)
        print("Import des transactions (depenses)...")
        depenses_file = examples_path / 'depenses.txt'
        if depenses_file.exists():
            with open(depenses_file, 'r', encoding='utf-8') as f:
                lines = f.readlines()[1:]
                imported = 0
                for i, line in enumerate(lines):
                    parts = line.strip().split('\t')
                    if len(parts) >= 7 and parts[0] and parts[2] != 'Aucun':
                        date = parse_date(parts[0]) or datetime.now().strftime('%Y-%m-%d')
                        product = parts[2].strip()
                        supplier = parts[3].strip()
                        quantity_str = parts[4] if len(parts) > 4 else '0'
                        # Nettoyer la quantité (peut contenir des caractères non numériques)
                        quantity = 0
                        try:
                            quantity = int(float(quantity_str.replace('$', '').replace(',', '.').replace(' ', '')))
                        except:
                            quantity = 0
                        amount = parse_amount(parts[6] if len(parts) > 6 else '0')
                        
                        if amount > 0:
                            cursor.execute("""
                                INSERT INTO transactions (type, category, amount, description, reference, user_id, created_at)
                                VALUES ('Depense', 'Achats Fournisseurs', ?, ?, ?, 1, ?)
                            """, (amount, f"{quantity}x {product} - {supplier}", f"D-{date.replace('-', '')}-{i:04d}", f"{date} 12:00:00"))
                            if cursor.rowcount > 0:
                                imported += 1
                print(f"  -> {imported} depenses importees")
        
        conn.commit()
        print("\nImport termine avec succes!")
        return True
        
    except Exception as e:
        print(f"\nERREUR lors de l'import: {e}")
        conn.rollback()
        return False
    finally:
        conn.close()

if __name__ == '__main__':
    import_data()
    input("\nAppuyez sur Entree pour quitter...")
