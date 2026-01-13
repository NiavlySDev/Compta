using System.IO;
using BlackWoodsCompta.Models.DTOs;
using BlackWoodsCompta.Models.Entities;
using BlackWoodsCompta.Models.Enums;
using Microsoft.Data.Sqlite;
using Dapper;
using Serilog;

namespace BlackWoodsCompta.WPF.Services;

public class LocalDataService : IDataService
{
    private readonly string _connectionString;
    private readonly string _dbPath;

    public LocalDataService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var dbFolder = Path.Combine(appData, "BlackWoodsCompta", "Data");
        Directory.CreateDirectory(dbFolder);
        
        _dbPath = Path.Combine(dbFolder, "blackwoods.db");
        _connectionString = $"Data Source={_dbPath}";
        
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        // Create tables
        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS users (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                username TEXT UNIQUE NOT NULL,
                password_hash TEXT NOT NULL,
                role TEXT NOT NULL,
                full_name TEXT NOT NULL,
                email TEXT,
                is_active INTEGER DEFAULT 1,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                updated_at TEXT DEFAULT CURRENT_TIMESTAMP
            )");

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS transactions (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                type TEXT NOT NULL,
                category TEXT NOT NULL,
                amount REAL NOT NULL,
                description TEXT,
                reference TEXT,
                user_id INTEGER NOT NULL,
                employee_id INTEGER,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                updated_at TEXT DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (user_id) REFERENCES users(id),
                FOREIGN KEY (employee_id) REFERENCES employees(id)
            )");

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS employees (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL,
                position TEXT NOT NULL,
                salary REAL NOT NULL,
                hire_date TEXT NOT NULL,
                phone TEXT,
                email TEXT,
                is_active INTEGER DEFAULT 1,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                updated_at TEXT DEFAULT CURRENT_TIMESTAMP
            )");

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS inventory (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                product_name TEXT NOT NULL,
                category TEXT NOT NULL,
                quantity REAL NOT NULL,
                unit TEXT NOT NULL,
                unit_cost REAL NOT NULL,
                min_quantity REAL DEFAULT 0,
                supplier TEXT,
                expiry_date TEXT,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                updated_at TEXT DEFAULT CURRENT_TIMESTAMP
            )");

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS orders (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                order_number TEXT UNIQUE NOT NULL,
                supplier TEXT NOT NULL,
                order_date TEXT DEFAULT CURRENT_TIMESTAMP,
                delivery_date TEXT,
                status TEXT DEFAULT 'En attente',
                total_amount REAL NOT NULL,
                notes TEXT,
                user_id INTEGER NOT NULL,
                transaction_id INTEGER,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP,
                updated_at TEXT DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (user_id) REFERENCES users(id),
                FOREIGN KEY (transaction_id) REFERENCES transactions(id)
            )");

        connection.Execute(@"
            CREATE TABLE IF NOT EXISTS order_items (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                order_id INTEGER NOT NULL,
                product_name TEXT NOT NULL,
                category TEXT NOT NULL,
                quantity REAL NOT NULL,
                unit TEXT NOT NULL,
                unit_price REAL NOT NULL,
                total_price REAL NOT NULL,
                expiry_date TEXT,
                FOREIGN KEY (order_id) REFERENCES orders(id)
            )");

        // Migration: Add employee_id column if it doesn't exist
        var columns = connection.Query<string>("PRAGMA table_info(transactions)").Select(c => c).ToList();
        var hasEmployeeId = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM pragma_table_info('transactions') WHERE name='employee_id'") > 0;
        if (!hasEmployeeId)
        {
            connection.Execute("ALTER TABLE transactions ADD COLUMN employee_id INTEGER");
            Log.Information("Added employee_id column to transactions table");
        }

        // Check if default user exists
        var userCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM users");
        if (userCount == 0)
        {
            // Create default admin user (password: admin123)
            connection.Execute(@"
                INSERT INTO users (username, password_hash, role, full_name, email)
                VALUES (@Username, @PasswordHash, @Role, @FullName, @Email)",
                new
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "Admin",
                    FullName = "Administrateur Local",
                    Email = "admin@blackwoods.local"
                });

            Log.Information("Default admin user created for local database");
        }
    }

    public async Task<LoginResponse> LoginAsync(string username, string password)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var user = await connection.QueryFirstOrDefaultAsync<dynamic>(@"
                SELECT * FROM users 
                WHERE username = @Username AND is_active = 1",
                new { Username = username });

            if (user == null)
            {
                return new LoginResponse { Success = false, Message = "Utilisateur non trouvé" };
            }

            var passwordMatch = BCrypt.Net.BCrypt.Verify(password, (string)user.password_hash);
            if (!passwordMatch)
            {
                return new LoginResponse { Success = false, Message = "Mot de passe incorrect" };
            }

            return new LoginResponse
            {
                Success = true,
                Token = "local-token",
                Message = "Connexion réussie",
                User = new UserDto
                {
                    Id = Convert.ToInt32(user.id),
                    Username = user.username,
                    FullName = user.full_name,
                    Role = user.role,
                    Email = user.email
                }
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Local login error");
            return new LoginResponse { Success = false, Message = "Erreur de connexion locale" };
        }
    }

    public async Task<List<Transaction>> GetTransactionsAsync(string? search = null, string? type = null, string? category = null)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                SELECT t.*, u.full_name as UserName, e.name as EmployeeName 
                FROM transactions t
                LEFT JOIN users u ON t.user_id = u.id
                LEFT JOIN employees e ON t.employee_id = e.id
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(search))
            {
                sql += " AND (t.description LIKE @Search OR t.category LIKE @Search)";
                parameters.Add("Search", $"%{search}%");
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                sql += " AND t.type = @Type";
                parameters.Add("Type", type);
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                sql += " AND t.category = @Category";
                parameters.Add("Category", category);
            }

            sql += " ORDER BY t.created_at DESC";

            var transactions = await connection.QueryAsync<Transaction>(sql, parameters);
            Log.Information($"Loaded {transactions.Count()} transactions from database");
            return transactions.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting transactions from local DB");
            return new List<Transaction>();
        }
    }

    public async Task<Transaction?> CreateTransactionAsync(Transaction transaction)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var id = await connection.ExecuteScalarAsync<int>(@"
                INSERT INTO transactions (type, category, amount, description, reference, user_id, employee_id, created_at)
                VALUES (@Type, @Category, @Amount, @Description, @Reference, @UserId, @EmployeeId, @CreatedAt);
                SELECT last_insert_rowid();",
                new
                {
                    Type = transaction.Type.ToString(),
                    transaction.Category,
                    transaction.Amount,
                    transaction.Description,
                    transaction.Reference,
                    transaction.UserId,
                    transaction.EmployeeId,
                    CreatedAt = transaction.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                });

            transaction.Id = id;
            return transaction;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating transaction in local DB");
            return null;
        }
    }

    public async Task<bool> DeleteTransactionAsync(int id)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            await connection.ExecuteAsync("DELETE FROM transactions WHERE id = @Id", new { Id = id });
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting transaction from local DB");
            return false;
        }
    }

    public async Task<DashboardDto?> GetDashboardDataAsync()
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var totalRevenue = await connection.ExecuteScalarAsync<decimal>(
                "SELECT COALESCE(SUM(amount), 0) FROM transactions WHERE type = 'Vente'");

            var totalExpenses = await connection.ExecuteScalarAsync<decimal>(
                "SELECT COALESCE(SUM(amount), 0) FROM transactions WHERE type = 'Depense'");

            var transactionCount = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM transactions");

            var employeeCount = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM employees WHERE is_active = 1");

            return new DashboardDto
            {
                TotalRevenue = totalRevenue,
                TotalExpenses = totalExpenses,
                NetProfit = totalRevenue - totalExpenses,
                TransactionCount = transactionCount,
                EmployeeCount = employeeCount,
                LowStockItemsCount = 0,
                PendingInvoicesCount = 0
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting dashboard data from local DB");
            return null;
        }
    }

    public async Task<List<Employee>> GetEmployeesAsync(string? search = null, string? position = null, bool? isActive = null)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM employees WHERE 1=1";
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query += " AND name LIKE @Search";
                parameters.Add("Search", $"%{search}%");
            }

            if (!string.IsNullOrWhiteSpace(position))
            {
                query += " AND position = @Position";
                parameters.Add("Position", position);
            }

            if (isActive.HasValue)
            {
                query += " AND is_active = @IsActive";
                parameters.Add("IsActive", isActive.Value ? 1 : 0);
            }

            query += " ORDER BY name";

            var employees = await connection.QueryAsync<Employee>(query, parameters);
            return employees.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting employees from local DB");
            return new List<Employee>();
        }
    }

    public async Task<Employee?> CreateEmployeeAsync(Employee employee)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var id = await connection.ExecuteScalarAsync<int>(@"
                INSERT INTO employees (name, position, salary, hire_date, phone, email)
                VALUES (@Name, @Position, @Salary, @HireDate, @Phone, @Email);
                SELECT last_insert_rowid();",
                employee);

            employee.Id = id;
            return employee;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating employee in local DB");
            return null;
        }
    }

    public async Task<bool> UpdateEmployeeAsync(Employee employee)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            await connection.ExecuteAsync(@"
                UPDATE employees 
                SET name = @Name, position = @Position, salary = @Salary, 
                    hire_date = @HireDate, phone = @Phone, email = @Email,
                    is_active = @IsActive, updated_at = CURRENT_TIMESTAMP
                WHERE id = @Id",
                employee);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating employee in local DB");
            return false;
        }
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            await connection.ExecuteAsync(
                "UPDATE employees SET is_active = 0 WHERE id = @Id", 
                new { Id = id });
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting employee from local DB");
            return false;
        }
    }

    public string GetConnectionInfo()
    {
        return $"Base de données locale: {_dbPath}";
    }

    // Payrolls
    public async Task<List<Payroll>> GetPayrollsAsync(int? employeeId = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"SELECT p.*, e.name as EmployeeName 
                         FROM payrolls p 
                         LEFT JOIN employees e ON p.employee_id = e.id 
                         WHERE 1=1";
            var parameters = new DynamicParameters();

            if (employeeId.HasValue)
            {
                query += " AND p.employee_id = @EmployeeId";
                parameters.Add("EmployeeId", employeeId.Value);
            }

            if (startDate.HasValue)
            {
                query += " AND p.paid_date >= @StartDate";
                parameters.Add("StartDate", startDate.Value.ToString("yyyy-MM-dd"));
            }

            if (endDate.HasValue)
            {
                query += " AND p.paid_date <= @EndDate";
                parameters.Add("EndDate", endDate.Value.ToString("yyyy-MM-dd"));
            }

            query += " ORDER BY p.paid_date DESC";

            var payrolls = await connection.QueryAsync<Payroll>(query, parameters);
            return payrolls.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting payrolls from local DB");
            return new List<Payroll>();
        }
    }

    public async Task<Payroll?> CreatePayrollAsync(Payroll payroll)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var id = await connection.ExecuteScalarAsync<long>(@"
                INSERT INTO payrolls (employee_id, amount, period_start, period_end, paid_date, notes, created_by)
                VALUES (@EmployeeId, @Amount, @PeriodStart, @PeriodEnd, @PaidDate, @Notes, @CreatedBy);
                SELECT last_insert_rowid();",
                new
                {
                    payroll.EmployeeId,
                    payroll.Amount,
                    PeriodStart = payroll.PeriodStart.ToString("yyyy-MM-dd"),
                    PeriodEnd = payroll.PeriodEnd.ToString("yyyy-MM-dd"),
                    PaidDate = payroll.PaidDate.ToString("yyyy-MM-dd"),
                    payroll.Notes,
                    payroll.CreatedBy
                });

            payroll.Id = Convert.ToInt32(id);
            return payroll;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating payroll in local DB");
            return null;
        }
    }

    public async Task<bool> DeletePayrollAsync(int id)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            await connection.ExecuteAsync("DELETE FROM payrolls WHERE id = @Id", new { Id = id });
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting payroll from local DB");
            return false;
        }
    }

    // Inventory
    public async Task<List<InventoryItem>> GetInventoryAsync(string? search = null, string? category = null, bool? lowStock = null)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM inventory WHERE 1=1";
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query += " AND product_name LIKE @Search";
                parameters.Add("Search", $"%{search}%");
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query += " AND category = @Category";
                parameters.Add("Category", category);
            }

            if (lowStock == true)
            {
                query += " AND quantity <= min_quantity";
            }

            query += " ORDER BY product_name";

            var items = await connection.QueryAsync<InventoryItem>(query, parameters);
            return items.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting inventory from local DB");
            return new List<InventoryItem>();
        }
    }

    public async Task<InventoryItem?> CreateInventoryItemAsync(InventoryItem item)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var id = await connection.ExecuteScalarAsync<long>(@"
                INSERT INTO inventory (product_name, category, quantity, unit, unit_cost, min_quantity, supplier)
                VALUES (@ProductName, @Category, @Quantity, @Unit, @UnitCost, @MinQuantity, @Supplier);
                SELECT last_insert_rowid();",
                item);

            item.Id = Convert.ToInt32(id);
            return item;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating inventory item in local DB");
            return null;
        }
    }

    public async Task<bool> UpdateInventoryItemAsync(InventoryItem item)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            await connection.ExecuteAsync(@"
                UPDATE inventory 
                SET product_name = @ProductName, category = @Category, quantity = @Quantity,
                    unit = @Unit, unit_cost = @UnitCost, min_quantity = @MinQuantity,
                    supplier = @Supplier, updated_at = CURRENT_TIMESTAMP
                WHERE id = @Id",
                item);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating inventory item in local DB");
            return false;
        }
    }

    public async Task<bool> DeleteInventoryItemAsync(int id)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            await connection.ExecuteAsync("DELETE FROM inventory WHERE id = @Id", new { Id = id });
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting inventory item from local DB");
            return false;
        }
    }

    public async Task<List<InventoryMovement>> GetInventoryMovementsAsync(int? productId = null)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"SELECT m.*, i.product_name as ProductName, u.username as UserName
                         FROM inventory_movements m
                         LEFT JOIN inventory i ON m.product_id = i.id
                         LEFT JOIN users u ON m.user_id = u.id
                         WHERE 1=1";
            var parameters = new DynamicParameters();

            if (productId.HasValue)
            {
                query += " AND m.product_id = @ProductId";
                parameters.Add("ProductId", productId.Value);
            }

            query += " ORDER BY m.created_at DESC LIMIT 100";

            var movements = await connection.QueryAsync<InventoryMovement>(query, parameters);
            return movements.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting inventory movements from local DB");
            return new List<InventoryMovement>();
        }
    }

    public async Task<InventoryMovement?> CreateInventoryMovementAsync(InventoryMovement movement)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            // Start transaction
            using var transaction = connection.BeginTransaction();

            try
            {
                // Insert movement
                var id = await connection.ExecuteScalarAsync<long>(@"
                    INSERT INTO inventory_movements (product_id, quantity, type, reason, user_id)
                    VALUES (@ProductId, @Quantity, @Type, @Reason, @UserId);
                    SELECT last_insert_rowid();",
                    movement, transaction);

                movement.Id = Convert.ToInt32(id);

                // Update inventory quantity
                var quantityChange = movement.Type == InventoryMovementType.Entree ? movement.Quantity : -movement.Quantity;
                await connection.ExecuteAsync(@"
                    UPDATE inventory 
                    SET quantity = quantity + @Change, updated_at = CURRENT_TIMESTAMP
                    WHERE id = @ProductId",
                    new { Change = quantityChange, movement.ProductId }, transaction);

                transaction.Commit();
                return movement;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating inventory movement in local DB");
            return null;
        }
    }

    // Invoices
    public async Task<List<Invoice>> GetInvoicesAsync(string? search = null, string? status = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM invoices WHERE 1=1";
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query += " AND (invoice_number LIKE @Search OR client_name LIKE @Search)";
                parameters.Add("Search", $"%{search}%");
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query += " AND status = @Status";
                parameters.Add("Status", status);
            }

            if (startDate.HasValue)
            {
                query += " AND issue_date >= @StartDate";
                parameters.Add("StartDate", startDate.Value.ToString("yyyy-MM-dd"));
            }

            if (endDate.HasValue)
            {
                query += " AND issue_date <= @EndDate";
                parameters.Add("EndDate", endDate.Value.ToString("yyyy-MM-dd"));
            }

            query += " ORDER BY issue_date DESC";

            var invoices = await connection.QueryAsync<Invoice>(query, parameters);
            return invoices.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting invoices from local DB");
            return new List<Invoice>();
        }
    }

    public async Task<Invoice?> CreateInvoiceAsync(Invoice invoice)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var id = await connection.ExecuteScalarAsync<long>(@"
                INSERT INTO invoices (invoice_number, client_name, client_phone, client_email, 
                                     total_amount, status, issue_date, due_date, notes, created_by)
                VALUES (@InvoiceNumber, @ClientName, @ClientPhone, @ClientEmail,
                       @TotalAmount, @Status, @IssueDate, @DueDate, @Notes, @CreatedBy);
                SELECT last_insert_rowid();",
                new
                {
                    invoice.InvoiceNumber,
                    invoice.ClientName,
                    invoice.ClientPhone,
                    invoice.ClientEmail,
                    invoice.TotalAmount,
                    Status = invoice.Status.ToString(),
                    IssueDate = invoice.IssueDate.ToString("yyyy-MM-dd"),
                    DueDate = invoice.DueDate?.ToString("yyyy-MM-dd"),
                    invoice.Notes,
                    invoice.CreatedBy
                });

            invoice.Id = Convert.ToInt32(id);

            // Insert invoice items
            if (invoice.Items != null && invoice.Items.Any())
            {
                foreach (var item in invoice.Items)
                {
                    item.InvoiceId = invoice.Id;
                    await connection.ExecuteAsync(@"
                        INSERT INTO invoice_items (invoice_id, description, quantity, unit_price, total_price)
                        VALUES (@InvoiceId, @Description, @Quantity, @UnitPrice, @TotalPrice)",
                        item);
                }
            }

            return invoice;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating invoice in local DB");
            return null;
        }
    }

    public async Task<bool> UpdateInvoiceAsync(Invoice invoice)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            await connection.ExecuteAsync(@"
                UPDATE invoices 
                SET client_name = @ClientName, client_phone = @ClientPhone, client_email = @ClientEmail,
                    total_amount = @TotalAmount, status = @Status, issue_date = @IssueDate,
                    due_date = @DueDate, notes = @Notes, updated_at = CURRENT_TIMESTAMP
                WHERE id = @Id",
                new
                {
                    invoice.Id,
                    invoice.ClientName,
                    invoice.ClientPhone,
                    invoice.ClientEmail,
                    invoice.TotalAmount,
                    Status = invoice.Status.ToString(),
                    IssueDate = invoice.IssueDate.ToString("yyyy-MM-dd"),
                    DueDate = invoice.DueDate?.ToString("yyyy-MM-dd"),
                    invoice.Notes
                });

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating invoice in local DB");
            return false;
        }
    }

    public async Task<bool> DeleteInvoiceAsync(int id)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            // Delete invoice items first (cascade)
            await connection.ExecuteAsync("DELETE FROM invoice_items WHERE invoice_id = @Id", new { Id = id });
            await connection.ExecuteAsync("DELETE FROM invoices WHERE id = @Id", new { Id = id });
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting invoice from local DB");
            return false;
        }
    }

    public async Task<List<InvoiceItem>> GetInvoiceItemsAsync(int invoiceId)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var items = await connection.QueryAsync<InvoiceItem>(
                "SELECT * FROM invoice_items WHERE invoice_id = @InvoiceId ORDER BY id",
                new { InvoiceId = invoiceId });
            return items.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting invoice items from local DB");
            return new List<InvoiceItem>();
        }
    }

    // Inventory operations
    public async Task<List<InventoryItem>> GetInventoryItemsAsync(string? search = null, string? category = null)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM inventory WHERE 1=1";
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(search))
            {
                query += " AND product_name LIKE @Search";
                parameters.Add("Search", $"%{search}%");
            }

            if (!string.IsNullOrEmpty(category))
            {
                query += " AND category = @Category";
                parameters.Add("Category", category);
            }

            query += " ORDER BY product_name";

            var items = await connection.QueryAsync<InventoryItem>(query, parameters);
            return items.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting inventory items from local DB");
            return new List<InventoryItem>();
        }
    }

    // Orders operations
    public async Task<List<Order>> GetOrdersAsync(string? search = null, string? status = null)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM orders WHERE 1=1";
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(search))
            {
                query += " AND (order_number LIKE @Search OR supplier LIKE @Search)";
                parameters.Add("Search", $"%{search}%");
            }

            if (!string.IsNullOrEmpty(status))
            {
                query += " AND status = @Status";
                parameters.Add("Status", status);
            }

            query += " ORDER BY order_date DESC";

            var orders = await connection.QueryAsync<Order>(query, parameters);
            
            // Load order items for each order
            foreach (var order in orders)
            {
                order.Items = await GetOrderItemsAsync(order.Id);
            }
            
            return orders.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting orders from local DB");
            return new List<Order>();
        }
    }

    public async Task<Order?> CreateOrderAsync(Order order)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var id = await connection.QuerySingleAsync<int>(@"
                    INSERT INTO orders (order_number, supplier, order_date, delivery_date, status, total_amount, notes, transaction_id, created_at, updated_at)
                    VALUES (@OrderNumber, @Supplier, @OrderDate, @DeliveryDate, @Status, @TotalAmount, @Notes, @TransactionId, @CreatedAt, @UpdatedAt)
                    RETURNING id", order, transaction);

                order.Id = id;

                // Insert order items
                foreach (var item in order.Items)
                {
                    item.OrderId = id;
                    await connection.ExecuteAsync(@"
                        INSERT INTO order_items (order_id, product_name, category, quantity, unit, unit_price, total_price, expiry_date)
                        VALUES (@OrderId, @ProductName, @Category, @Quantity, @Unit, @UnitPrice, @TotalPrice, @ExpiryDate)", 
                        item, transaction);
                }

                await transaction.CommitAsync();
                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating order in local DB");
            return null;
        }
    }

    public async Task<bool> UpdateOrderAsync(Order order)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var updated = await connection.ExecuteAsync(@"
                    UPDATE orders 
                    SET order_number = @OrderNumber, supplier = @Supplier, order_date = @OrderDate, 
                        delivery_date = @DeliveryDate, status = @Status, total_amount = @TotalAmount, 
                        notes = @Notes, transaction_id = @TransactionId, updated_at = @UpdatedAt
                    WHERE id = @Id", order, transaction);

                if (updated > 0)
                {
                    // Delete existing order items and re-insert
                    await connection.ExecuteAsync("DELETE FROM order_items WHERE order_id = @Id", new { order.Id }, transaction);

                    foreach (var item in order.Items)
                    {
                        item.OrderId = order.Id;
                        await connection.ExecuteAsync(@"
                            INSERT INTO order_items (order_id, product_name, category, quantity, unit, unit_price, total_price, expiry_date)
                            VALUES (@OrderId, @ProductName, @Category, @Quantity, @Unit, @UnitPrice, @TotalPrice, @ExpiryDate)", 
                            item, transaction);
                    }
                }

                await transaction.CommitAsync();
                return updated > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating order in local DB");
            return false;
        }
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Delete order items first
                await connection.ExecuteAsync("DELETE FROM order_items WHERE order_id = @Id", new { Id = id }, transaction);
                
                // Delete the order
                await connection.ExecuteAsync("DELETE FROM orders WHERE id = @Id", new { Id = id }, transaction);

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting order from local DB");
            return false;
        }
    }

    public async Task<List<OrderItem>> GetOrderItemsAsync(int orderId)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var items = await connection.QueryAsync<OrderItem>(
                "SELECT * FROM order_items WHERE order_id = @OrderId ORDER BY id",
                new { OrderId = orderId });
            return items.ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting order items from local DB");
            return new List<OrderItem>();
        }
    }
}
