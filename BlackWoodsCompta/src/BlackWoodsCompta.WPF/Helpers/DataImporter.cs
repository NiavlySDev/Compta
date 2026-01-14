using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BlackWoodsCompta.Models.Entities;
using BlackWoodsCompta.Models.Enums;
using Microsoft.Data.Sqlite;
using Serilog;

namespace BlackWoodsCompta.WPF.Helpers;

/// <summary>
/// Utilitaire pour importer les données depuis les fichiers d'exemple
/// </summary>
public class DataImporter
{
    private readonly string _connectionString;
    private readonly string _examplesPath;

    public DataImporter(string connectionString, string examplesPath)
    {
        _connectionString = connectionString;
        _examplesPath = examplesPath;
        Log.Information("DataImporter initialisé avec chemin: {Path}", examplesPath);
    }

    /// <summary>
    /// Importe toutes les données depuis les fichiers d'exemple
    /// </summary>
    public async Task<bool> ImportAllDataAsync()
    {
        Log.Information("Début de l'import des données d'exemple");
        
        try
        {
            // Import dans l'ordre des dépendances
            await ImportEmployeesAsync();
            await ImportSuppliersAsync();
            await ImportInventoryFromStockAsync();
            await ImportPurchasePricesAsync();
            await ImportSalePricesAsync();
            await ImportPurchaseOrdersAsync();
            await ImportTransactionsAsync();
            
            Log.Information("Import des données terminé avec succès");
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erreur lors de l'import des données");
            return false;
        }
    }

    /// <summary>
    /// Parse un montant au format "$1 000,00" ou "$1000.00"
    /// </summary>
    private decimal ParseAmount(string amountStr)
    {
        if (string.IsNullOrWhiteSpace(amountStr))
            return 0;

        // Enlever le symbole $ et les espaces
        var cleaned = amountStr.Replace("$", "").Replace(" ", "").Trim();
        
        // Remplacer la virgule par un point pour le parsing
        cleaned = cleaned.Replace(",", ".");
        
        if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            return result;
        
        Log.Warning("Impossible de parser le montant: {Amount}", amountStr);
        return 0;
    }

    /// <summary>
    /// Parse une date au format "dd/MM/yyyy"
    /// </summary>
    private DateTime? ParseDate(string dateStr)
    {
        if (string.IsNullOrWhiteSpace(dateStr))
            return null;

        var formats = new[] { "dd/MM/yyyy", "d/M/yyyy", "dd/MM/yy", "d/M/yy" };
        
        if (DateTime.TryParseExact(dateStr, formats, CultureInfo.InvariantCulture, 
            DateTimeStyles.None, out var result))
            return result;
        
        Log.Warning("Impossible de parser la date: {Date}", dateStr);
        return null;
    }

    /// <summary>
    /// Importe les employés depuis effectif.txt
    /// </summary>
    private async Task ImportEmployeesAsync()
    {
        Log.Information("Import des employés...");
        var filePath = Path.Combine(_examplesPath, "effectif.txt");
        
        if (!File.Exists(filePath))
        {
            Log.Warning("Fichier effectif.txt non trouvé");
            return;
        }

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var lines = await File.ReadAllLinesAsync(filePath);
        var imported = 0;

        for (int i = 1; i < lines.Length; i++) // Skip header
        {
            var parts = lines[i].Split('\t');
            
            if (parts.Length < 7 || string.IsNullOrWhiteSpace(parts[0]) || parts[0] == "Woods")
                continue;

            var name = $"{parts[1]} {parts[0]}".Trim(); // Prénom Nom
            var idRp = parts.Length > 2 ? parts[2] : "";
            var phone = parts.Length > 3 ? parts[3] : "";
            var discord = parts.Length > 5 ? parts[5] : "";
            var position = parts.Length > 6 ? parts[6] : "Employé";

            // Déterminer le salaire basé sur la position
            decimal salary = position.ToLower() switch
            {
                "pdg" or "co-pdg" => 5000,
                "manager" => 3000,
                "livreur" => 1500,
                "technicien" => 2000,
                _ => 1200
            };

            var sql = @"
                INSERT OR IGNORE INTO employees (name, position, salary, hire_date, phone, discord, id_rp, is_active)
                VALUES (@name, @position, @salary, @hireDate, @phone, @discord, @idRp, 1)";

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@position", position);
            cmd.Parameters.AddWithValue("@salary", salary);
            cmd.Parameters.AddWithValue("@hireDate", DateTime.Now.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@phone", phone);
            cmd.Parameters.AddWithValue("@discord", discord);
            cmd.Parameters.AddWithValue("@idRp", idRp);

            var rows = await cmd.ExecuteNonQueryAsync();
            if (rows > 0) imported++;
        }

        Log.Information("Import des employés terminé: {Count} employés importés", imported);
    }

    /// <summary>
    /// Importe les fournisseurs depuis prix_achat.txt et depenses.txt
    /// </summary>
    private async Task ImportSuppliersAsync()
    {
        Log.Information("Import des fournisseurs...");
        var filePath = Path.Combine(_examplesPath, "prix_achat.txt");
        
        if (!File.Exists(filePath))
        {
            Log.Warning("Fichier prix_achat.txt non trouvé");
            return;
        }

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var lines = await File.ReadAllLinesAsync(filePath);
        var suppliers = new HashSet<string>();
        var imported = 0;

        for (int i = 1; i < lines.Length; i++) // Skip header
        {
            var parts = lines[i].Split('\t');
            
            if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
                continue;

            var supplierName = parts[1].Trim();
            if (suppliers.Add(supplierName))
            {
                var sql = @"
                    INSERT OR IGNORE INTO suppliers (name, contact_person, phone, email, is_active)
                    VALUES (@name, @contact, @phone, @email, 1)";

                using var cmd = new SqliteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@name", supplierName);
                cmd.Parameters.AddWithValue("@contact", supplierName);
                cmd.Parameters.AddWithValue("@phone", "");
                cmd.Parameters.AddWithValue("@email", "");

                var rows = await cmd.ExecuteNonQueryAsync();
                if (rows > 0) imported++;
            }
        }

        Log.Information("Import des fournisseurs terminé: {Count} fournisseurs importés", imported);
    }

    /// <summary>
    /// Importe l'inventaire depuis stock.txt
    /// </summary>
    private async Task ImportInventoryFromStockAsync()
    {
        Log.Information("Import de l'inventaire...");
        var filePath = Path.Combine(_examplesPath, "stock.txt");
        
        if (!File.Exists(filePath))
        {
            Log.Warning("Fichier stock.txt non trouvé");
            return;
        }

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var lines = await File.ReadAllLinesAsync(filePath);
        var imported = 0;

        for (int i = 1; i < lines.Length; i++) // Skip header
        {
            var parts = lines[i].Split('\t');
            
            if (parts.Length < 5 || string.IsNullOrWhiteSpace(parts[0]))
                continue;

            var productName = parts[0].Trim();
            var supplier = parts.Length > 1 ? parts[1] : "";
            var priceStr = parts.Length > 2 ? parts[2] : "$0";
            var stockStr = parts.Length > 4 ? parts[4] : "0";

            var unitPrice = ParseAmount(priceStr);
            var quantity = decimal.TryParse(stockStr, out var qty) ? qty : 0;

            // Déterminer la catégorie
            var category = productName.ToLower().Contains("livraison") || productName.ToLower().Contains("frais")
                ? "Service"
                : "Matière première";

            var sql = @"
                INSERT OR IGNORE INTO inventory (product_name, category, quantity, unit_price, supplier, min_quantity, unit)
                VALUES (@name, @category, @quantity, @price, @supplier, 50, 'unité')";

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@name", productName);
            cmd.Parameters.AddWithValue("@category", category);
            cmd.Parameters.AddWithValue("@quantity", quantity);
            cmd.Parameters.AddWithValue("@price", unitPrice);
            cmd.Parameters.AddWithValue("@supplier", supplier);

            var rows = await cmd.ExecuteNonQueryAsync();
            if (rows > 0) imported++;
        }

        Log.Information("Import de l'inventaire terminé: {Count} articles importés", imported);
    }

    /// <summary>
    /// Importe les prix d'achat depuis prix_achat.txt
    /// </summary>
    private async Task ImportPurchasePricesAsync()
    {
        Log.Information("Import des prix d'achat...");
        var filePath = Path.Combine(_examplesPath, "prix_achat.txt");
        
        if (!File.Exists(filePath))
        {
            Log.Warning("Fichier prix_achat.txt non trouvé");
            return;
        }

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var lines = await File.ReadAllLinesAsync(filePath);
        var imported = 0;

        for (int i = 1; i < lines.Length; i++) // Skip header
        {
            var parts = lines[i].Split('\t');
            
            if (parts.Length < 3 || string.IsNullOrWhiteSpace(parts[0]))
                continue;

            var productName = parts[0].Trim();
            var supplierName = parts[1].Trim();
            var priceStr = parts[2];
            var price = ParseAmount(priceStr);

            var sql = @"
                INSERT OR IGNORE INTO purchase_prices (product_name, supplier_name, unit_price, effective_date, is_current)
                VALUES (@product, @supplier, @price, @date, 1)";

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@product", productName);
            cmd.Parameters.AddWithValue("@supplier", supplierName);
            cmd.Parameters.AddWithValue("@price", price);
            cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));

            var rows = await cmd.ExecuteNonQueryAsync();
            if (rows > 0) imported++;
        }

        Log.Information("Import des prix d'achat terminé: {Count} prix importés", imported);
    }

    /// <summary>
    /// Importe les prix de vente depuis prix_vente.txt
    /// </summary>
    private async Task ImportSalePricesAsync()
    {
        Log.Information("Import des prix de vente...");
        var filePath = Path.Combine(_examplesPath, "prix_vente.txt");
        
        if (!File.Exists(filePath))
        {
            Log.Warning("Fichier prix_vente.txt non trouvé");
            return;
        }

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var lines = await File.ReadAllLinesAsync(filePath);
        var imported = 0;

        for (int i = 1; i < lines.Length; i++) // Skip header
        {
            var parts = lines[i].Split('\t');
            
            if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[0]) || parts[0] == "Aucun")
                continue;

            var productName = parts[0].Trim();
            var priceStr = parts[1];
            var price = ParseAmount(priceStr);

            if (price == 0) continue;

            var sql = @"
                INSERT OR IGNORE INTO sale_prices (product_name, unit_price, effective_date, is_current)
                VALUES (@product, @price, @date, 1)";

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@product", productName);
            cmd.Parameters.AddWithValue("@price", price);
            cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));

            var rows = await cmd.ExecuteNonQueryAsync();
            if (rows > 0) imported++;
        }

        Log.Information("Import des prix de vente terminé: {Count} prix importés", imported);
    }

    /// <summary>
    /// Importe les commandes d'achat depuis depenses.txt
    /// </summary>
    private async Task ImportPurchaseOrdersAsync()
    {
        Log.Information("Import des commandes d'achat...");
        var filePath = Path.Combine(_examplesPath, "depenses.txt");
        
        if (!File.Exists(filePath))
        {
            Log.Warning("Fichier depenses.txt non trouvé");
            return;
        }

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var lines = await File.ReadAllLinesAsync(filePath);
        var imported = 0;

        // Grouper par fournisseur et date pour créer des commandes
        var orders = new Dictionary<string, List<string>>();

        for (int i = 1; i < lines.Length; i++) // Skip header
        {
            var parts = lines[i].Split('\t');
            
            if (parts.Length < 7 || string.IsNullOrWhiteSpace(parts[0]) || parts[0] == "Aucun")
                continue;

            var dateStr = parts[0];
            var supplier = parts[3].Trim();
            var key = $"{dateStr}_{supplier}";

            if (!orders.ContainsKey(key))
                orders[key] = new List<string>();
            
            orders[key].Add(lines[i]);
        }

        // Créer les commandes
        foreach (var orderGroup in orders)
        {
            var parts = orderGroup.Value[0].Split('\t');
            var date = ParseDate(parts[0]) ?? DateTime.Now;
            var supplier = parts[3].Trim();
            var deliveryDate = ParseDate(parts.Length > 7 ? parts[7] : "");
            
            var totalAmount = orderGroup.Value.Sum(line =>
            {
                var lineParts = line.Split('\t');
                return ParseAmount(lineParts.Length > 6 ? lineParts[6] : "$0");
            });

            var orderNumber = $"CMD-{date:yyyyMMdd}-{imported + 1:D4}";

            var sql = @"
                INSERT INTO orders (order_number, supplier, order_date, delivery_date, status, total_amount, notes, user_id)
                VALUES (@orderNum, @supplier, @orderDate, @deliveryDate, @status, @total, @notes, 1)";

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@orderNum", orderNumber);
            cmd.Parameters.AddWithValue("@supplier", supplier);
            cmd.Parameters.AddWithValue("@orderDate", date.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@deliveryDate", deliveryDate?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@status", "Livrée");
            cmd.Parameters.AddWithValue("@total", totalAmount);
            cmd.Parameters.AddWithValue("@notes", $"Import automatique - {orderGroup.Value.Count} articles");

            var rows = await cmd.ExecuteNonQueryAsync();
            if (rows > 0) imported++;
        }

        Log.Information("Import des commandes terminé: {Count} commandes importées", imported);
    }

    /// <summary>
    /// Importe les transactions de ventes depuis recettes.txt
    /// </summary>
    private async Task ImportTransactionsAsync()
    {
        Log.Information("Import des transactions...");
        var filePath = Path.Combine(_examplesPath, "recettes.txt");
        
        if (!File.Exists(filePath))
        {
            Log.Warning("Fichier recettes.txt non trouvé");
            return;
        }

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var lines = await File.ReadAllLinesAsync(filePath);
        var imported = 0;

        for (int i = 1; i < lines.Length; i++) // Skip header
        {
            var parts = lines[i].Split('\t');
            
            if (parts.Length < 7 || string.IsNullOrWhiteSpace(parts[0]) || parts[2] == "Aucun")
                continue;

            var dateStr = parts[0];
            var timeStr = parts.Length > 1 ? parts[1] : "12:00:00";
            var product = parts[2].Trim();
            var employeeName = parts.Length > 3 ? parts[3].Trim() : "";
            var quantityStr = parts.Length > 4 ? parts[4] : "0";
            var amountStr = parts.Length > 6 ? parts[6] : "$0";

            var date = ParseDate(dateStr) ?? DateTime.Now;
            var amount = ParseAmount(amountStr);
            var quantity = int.TryParse(quantityStr, out var qty) ? qty : 0;

            if (amount == 0 || quantity == 0) continue;

            var sql = @"
                INSERT INTO transactions (type, category, amount, description, reference, user_id, employee_id, created_at)
                VALUES (@type, @category, @amount, @description, @reference, 1, NULL, @createdAt)";

            using var cmd = new SqliteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@type", "Vente");
            cmd.Parameters.AddWithValue("@category", "Vente Restaurant");
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@description", $"{quantity}x {product}");
            cmd.Parameters.AddWithValue("@reference", $"V-{date:yyyyMMdd}-{i:D4}");
            cmd.Parameters.AddWithValue("@createdAt", $"{date:yyyy-MM-dd} {timeStr}");

            var rows = await cmd.ExecuteNonQueryAsync();
            if (rows > 0) imported++;
        }

        // Import des dépenses
        filePath = Path.Combine(_examplesPath, "depenses.txt");
        if (File.Exists(filePath))
        {
            lines = await File.ReadAllLinesAsync(filePath);

            for (int i = 1; i < lines.Length; i++) // Skip header
            {
                var parts = lines[i].Split('\t');
                
                if (parts.Length < 7 || string.IsNullOrWhiteSpace(parts[0]) || parts[2] == "Aucun")
                    continue;

                var dateStr = parts[0];
                var product = parts[2].Trim();
                var supplier = parts[3].Trim();
                var quantityStr = parts.Length > 4 ? parts[4] : "0";
                var amountStr = parts.Length > 6 ? parts[6] : "$0";

                var date = ParseDate(dateStr) ?? DateTime.Now;
                var amount = ParseAmount(amountStr);
                var quantity = int.TryParse(quantityStr, out var qty) ? qty : 0;

                if (amount == 0) continue;

                var sql = @"
                    INSERT INTO transactions (type, category, amount, description, reference, user_id, employee_id, created_at)
                    VALUES (@type, @category, @amount, @description, @reference, 1, NULL, @createdAt)";

                using var cmd = new SqliteCommand(sql, connection);
                cmd.Parameters.AddWithValue("@type", "Depense");
                cmd.Parameters.AddWithValue("@category", "Achats Fournisseurs");
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@description", $"{quantity}x {product} - {supplier}");
                cmd.Parameters.AddWithValue("@reference", $"D-{date:yyyyMMdd}-{i:D4}");
                cmd.Parameters.AddWithValue("@createdAt", date.ToString("yyyy-MM-dd HH:mm:ss"));

                var rows = await cmd.ExecuteNonQueryAsync();
                if (rows > 0) imported++;
            }
        }

        Log.Information("Import des transactions terminé: {Count} transactions importées", imported);
    }
}
