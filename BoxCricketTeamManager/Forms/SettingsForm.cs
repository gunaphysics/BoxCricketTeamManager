using Microsoft.EntityFrameworkCore;
using BoxCricketTeamManager.Services;
using BoxCricketTeamManager.Utilities;

namespace BoxCricketTeamManager.Forms
{
    public partial class SettingsForm : Form
    {
        private readonly ReportService _reportService = new();
        private readonly ExpenseService _expenseService = new();

        private ComboBox cmbYear;
        private NumericUpDown nudMonthlyDue;
        private NumericUpDown nudOpeningBalance;
        private Button btnSaveSettings;
        private DataGridView dgvCategories;
        private TextBox txtNewCategory;
        private Button btnAddCategory;
        private Button btnExportPayments;
        private Button btnBackupDatabase;

        public SettingsForm()
        {
            InitializeComponent();
            LoadSettings();
            LoadCategories();
        }

        private void InitializeComponent()
        {
            this.Text = "Settings";
            this.Size = new Size(800, 600);

            // Year settings group
            var grpYearSettings = new GroupBox
            {
                Text = "Year Settings",
                Location = new Point(20, 20),
                Size = new Size(350, 180),
                Font = new Font("Segoe UI", 10F)
            };

            var lblYear = new Label
            {
                Text = "Select Year:",
                Location = new Point(20, 35),
                AutoSize = true
            };

            cmbYear = new ComboBox
            {
                Location = new Point(140, 32),
                Size = new Size(100, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            int currentYear = DateTime.Now.Year;
            for (int year = currentYear + 2; year >= currentYear - 5; year--)
            {
                cmbYear.Items.Add(year);
            }
            cmbYear.SelectedItem = currentYear;
            cmbYear.SelectedIndexChanged += (s, e) => LoadSettings();

            var lblMonthlyDue = new Label
            {
                Text = "Monthly Due (₹):",
                Location = new Point(20, 75),
                AutoSize = true
            };

            nudMonthlyDue = new NumericUpDown
            {
                Location = new Point(140, 72),
                Size = new Size(100, 25),
                Minimum = 0,
                Maximum = 10000,
                DecimalPlaces = 0,
                Value = 50
            };

            var lblOpeningBalance = new Label
            {
                Text = "Opening Balance (₹):",
                Location = new Point(20, 115),
                AutoSize = true
            };

            nudOpeningBalance = new NumericUpDown
            {
                Location = new Point(160, 112),
                Size = new Size(120, 25),
                Minimum = -999999,
                Maximum = 999999,
                DecimalPlaces = 2,
                Value = 0
            };

            btnSaveSettings = new Button
            {
                Text = "Save Settings",
                Location = new Point(140, 145),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSaveSettings.Click += BtnSaveSettings_Click;

            grpYearSettings.Controls.AddRange(new Control[]
            {
                lblYear, cmbYear,
                lblMonthlyDue, nudMonthlyDue,
                lblOpeningBalance, nudOpeningBalance,
                btnSaveSettings
            });

            // Export group
            var grpExport = new GroupBox
            {
                Text = "Export & Backup",
                Location = new Point(400, 20),
                Size = new Size(350, 180),
                Font = new Font("Segoe UI", 10F)
            };

            btnExportPayments = new Button
            {
                Text = "Export Payments to Excel",
                Location = new Point(20, 40),
                Size = new Size(200, 35),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExportPayments.Click += BtnExportPayments_Click;

            var lblExportHint = new Label
            {
                Text = "Export complete payment grid for the selected year",
                Location = new Point(20, 80),
                Size = new Size(300, 20),
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9F)
            };

            btnBackupDatabase = new Button
            {
                Text = "Backup Database",
                Location = new Point(20, 110),
                Size = new Size(200, 35),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnBackupDatabase.Click += BtnBackupDatabase_Click;

            grpExport.Controls.AddRange(new Control[] { btnExportPayments, lblExportHint, btnBackupDatabase });

            // Categories group
            var grpCategories = new GroupBox
            {
                Text = "Expense Categories",
                Location = new Point(20, 220),
                Size = new Size(730, 300),
                Font = new Font("Segoe UI", 10F)
            };

            dgvCategories = new DataGridView
            {
                Location = new Point(20, 30),
                Size = new Size(400, 220),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            dgvCategories.Columns.Add(new DataGridViewTextBoxColumn { Name = "CategoryId", HeaderText = "ID", Visible = false });
            dgvCategories.Columns.Add(new DataGridViewTextBoxColumn { Name = "CategoryName", HeaderText = "Category Name", FillWeight = 150 });
            dgvCategories.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", FillWeight = 50 });

            var lblNewCategory = new Label
            {
                Text = "New Category:",
                Location = new Point(440, 30),
                AutoSize = true
            };

            txtNewCategory = new TextBox
            {
                Location = new Point(440, 55),
                Size = new Size(200, 25)
            };

            btnAddCategory = new Button
            {
                Text = "Add Category",
                Location = new Point(440, 90),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAddCategory.Click += BtnAddCategory_Click;

            var btnToggleCategory = new Button
            {
                Text = "Toggle Active/Inactive",
                Location = new Point(440, 130),
                Size = new Size(160, 30),
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnToggleCategory.Click += BtnToggleCategory_Click;

            grpCategories.Controls.AddRange(new Control[]
            {
                dgvCategories,
                lblNewCategory, txtNewCategory,
                btnAddCategory, btnToggleCategory
            });

            this.Controls.AddRange(new Control[] { grpYearSettings, grpExport, grpCategories });
        }

        private void LoadSettings()
        {
            if (cmbYear.SelectedItem == null) return;

            int year = (int)cmbYear.SelectedItem;
            nudMonthlyDue.Value = _reportService.GetMonthlyDueAmount(year);

            using var context = Program.CreateDbContext();
            var yearlyBalance = context.YearlyBalances.FirstOrDefault(y => y.Year == year);
            nudOpeningBalance.Value = yearlyBalance?.OpeningBalance ?? 0;
        }

        private void LoadCategories()
        {
            dgvCategories.Rows.Clear();
            var categories = _expenseService.GetAllCategories(includeInactive: true);

            foreach (var category in categories)
            {
                var rowIndex = dgvCategories.Rows.Add(
                    category.CategoryId,
                    category.CategoryName,
                    category.IsActive ? "Active" : "Inactive"
                );

                if (!category.IsActive)
                {
                    dgvCategories.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Gray;
                }
            }
        }

        private void BtnSaveSettings_Click(object? sender, EventArgs e)
        {
            if (cmbYear.SelectedItem == null) return;

            try
            {
                int year = (int)cmbYear.SelectedItem;
                _reportService.SetMonthlyDueAmount(year, nudMonthlyDue.Value);
                _reportService.SetOpeningBalance(year, nudOpeningBalance.Value);

                MessageBox.Show($"Settings saved for year {year}!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddCategory_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewCategory.Text))
            {
                MessageBox.Show("Please enter a category name.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var category = new Models.ExpenseCategory
                {
                    CategoryName = txtNewCategory.Text.Trim(),
                    IsActive = true
                };
                _expenseService.AddCategory(category);
                txtNewCategory.Clear();
                LoadCategories();
                MessageBox.Show("Category added successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding category: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnToggleCategory_Click(object? sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a category.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var categoryId = (int)dgvCategories.SelectedRows[0].Cells["CategoryId"].Value;
            var currentStatus = dgvCategories.SelectedRows[0].Cells["Status"].Value.ToString();

            try
            {
                using var context = Program.CreateDbContext();
                var category = context.ExpenseCategories.Find(categoryId);
                if (category != null)
                {
                    category.IsActive = !category.IsActive;
                    context.SaveChanges();
                    LoadCategories();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating category: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExportPayments_Click(object? sender, EventArgs e)
        {
            if (cmbYear.SelectedItem == null) return;

            int year = (int)cmbYear.SelectedItem;

            using var saveDialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                FileName = $"BoxCricket_Payments_{year}.xlsx"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ExcelExporter.ExportPaymentGridToExcel(saveDialog.FileName, year);
                    MessageBox.Show($"Payments exported successfully!\n\n{saveDialog.FileName}", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting payments: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnBackupDatabase_Click(object? sender, EventArgs e)
        {
            using var saveDialog = new SaveFileDialog
            {
                Filter = "SQL Server Backup (*.bak)|*.bak",
                FileName = $"BoxCricketTeamManager_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using var context = Program.CreateDbContext();
                    var backupPath = saveDialog.FileName.Replace("\\", "\\\\");
                    context.Database.ExecuteSqlRaw($"BACKUP DATABASE [BoxCricketTeamManager] TO DISK = '{backupPath}'");
                    MessageBox.Show($"Database backed up successfully!\n\n{saveDialog.FileName}", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error backing up database: {ex.Message}\n\nNote: LocalDB may have limitations with BACKUP command. " +
                        "Consider copying the .mdf file directly from the LocalDB folder.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}
