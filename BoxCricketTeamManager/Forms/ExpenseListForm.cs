using BoxCricketTeamManager.Models;
using BoxCricketTeamManager.Services;

namespace BoxCricketTeamManager.Forms
{
    public partial class ExpenseListForm : Form
    {
        private readonly ExpenseService _expenseService = new();
        private DataGridView dgvExpenses;
        private ComboBox cmbYear;
        private ComboBox cmbCategory;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Label lblTotalExpenses;

        public ExpenseListForm()
        {
            InitializeComponent();
            LoadExpenses();
        }

        private void InitializeComponent()
        {
            this.Text = "Expense Management";
            this.Size = new Size(1000, 600);

            // Top panel
            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(10)
            };

            var lblYear = new Label
            {
                Text = "Year:",
                Location = new Point(10, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F)
            };

            cmbYear = new ComboBox
            {
                Location = new Point(50, 15),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Populate years
            int currentYear = DateTime.Now.Year;
            cmbYear.Items.Add("All");
            for (int year = currentYear + 1; year >= currentYear - 5; year--)
            {
                cmbYear.Items.Add(year);
            }
            cmbYear.SelectedItem = currentYear;
            cmbYear.SelectedIndexChanged += (s, e) => LoadExpenses();

            var lblCategory = new Label
            {
                Text = "Category:",
                Location = new Point(150, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F)
            };

            cmbCategory = new ComboBox
            {
                Location = new Point(220, 15),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            LoadCategories();
            cmbCategory.SelectedIndexChanged += (s, e) => LoadExpenses();

            lblTotalExpenses = new Label
            {
                Text = "Total: ₹0",
                Location = new Point(400, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 76, 60)
            };

            pnlTop.Controls.AddRange(new Control[] { lblYear, cmbYear, lblCategory, cmbCategory, lblTotalExpenses });

            // Button panel
            var pnlButtons = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(10)
            };

            btnAdd = new Button
            {
                Text = "Add Expense",
                Location = new Point(10, 8),
                Size = new Size(120, 35),
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button
            {
                Text = "Edit",
                Location = new Point(140, 8),
                Size = new Size(80, 35),
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button
            {
                Text = "Delete",
                Location = new Point(230, 8),
                Size = new Size(80, 35),
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnDelete.Click += BtnDelete_Click;

            pnlButtons.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete });

            // Data grid
            dgvExpenses = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 10F),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            dgvExpenses.CellDoubleClick += (s, e) => BtnEdit_Click(s, e);

            // Add columns
            dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn { Name = "ExpenseId", HeaderText = "ID", Width = 50, Visible = false });
            dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn { Name = "Date", HeaderText = "Date", FillWeight = 80 });
            dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn { Name = "Month", HeaderText = "Month", FillWeight = 80 });
            dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Category", FillWeight = 100 });
            dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description", FillWeight = 200 });
            dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn { Name = "Amount", HeaderText = "Amount", FillWeight = 80 });
            dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn { Name = "Notes", HeaderText = "Notes", FillWeight = 150 });

            // Add controls
            this.Controls.Add(dgvExpenses);
            this.Controls.Add(pnlButtons);
            this.Controls.Add(pnlTop);
        }

        private void LoadCategories()
        {
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add(new { CategoryId = 0, CategoryName = "All Categories" });

            var categories = _expenseService.GetAllCategories();
            foreach (var category in categories)
            {
                cmbCategory.Items.Add(new { category.CategoryId, category.CategoryName });
            }

            cmbCategory.DisplayMember = "CategoryName";
            cmbCategory.ValueMember = "CategoryId";
            cmbCategory.SelectedIndex = 0;
        }

        public void LoadExpenses()
        {
            try
            {
                dgvExpenses.Rows.Clear();

                int? year = cmbYear.SelectedItem is int selectedYear ? selectedYear : null;

                var expenses = _expenseService.GetAllExpenses(year);

                // Filter by category if selected
                dynamic? selectedCategory = cmbCategory.SelectedItem;
                if (selectedCategory != null && selectedCategory.CategoryId != 0)
                {
                    expenses = expenses.Where(e => e.CategoryId == selectedCategory.CategoryId).ToList();
                }

                decimal total = 0;
                foreach (var expense in expenses)
                {
                    dgvExpenses.Rows.Add(
                        expense.ExpenseId,
                        expense.ExpenseDate.ToString("dd-MMM-yyyy"),
                        new DateTime(2000, expense.ExpenseMonth, 1).ToString("MMM") + " " + expense.ExpenseYear,
                        expense.Category?.CategoryName ?? "Uncategorized",
                        expense.Description,
                        $"₹{expense.Amount:N0}",
                        expense.Notes ?? ""
                    );
                    total += expense.Amount;
                }

                lblTotalExpenses.Text = $"Total: ₹{total:N0}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading expenses: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            using var form = new ExpenseEditForm(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadCategories();
                LoadExpenses();
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvExpenses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an expense to edit.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var expenseId = (int)dgvExpenses.SelectedRows[0].Cells["ExpenseId"].Value;
            var expense = _expenseService.GetExpenseById(expenseId);

            if (expense != null)
            {
                using var form = new ExpenseEditForm(expense);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadCategories();
                    LoadExpenses();
                }
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvExpenses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an expense to delete.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var expenseId = (int)dgvExpenses.SelectedRows[0].Cells["ExpenseId"].Value;
            var description = dgvExpenses.SelectedRows[0].Cells["Description"].Value.ToString();

            var result = MessageBox.Show($"Are you sure you want to delete this expense?\n\n{description}",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                _expenseService.DeleteExpense(expenseId);
                LoadExpenses();
            }
        }
    }
}
