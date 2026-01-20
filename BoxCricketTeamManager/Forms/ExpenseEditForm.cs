using BoxCricketTeamManager.Models;
using BoxCricketTeamManager.Services;

namespace BoxCricketTeamManager.Forms
{
    public partial class ExpenseEditForm : Form
    {
        private readonly ExpenseService _expenseService = new();
        private readonly Expense? _existingExpense;

        private ComboBox cmbCategory;
        private TextBox txtDescription;
        private NumericUpDown nudAmount;
        private ComboBox cmbMonth;
        private ComboBox cmbYear;
        private DateTimePicker dtpExpenseDate;
        private TextBox txtNotes;
        private Button btnSave;
        private Button btnCancel;
        private Button btnAddCategory;

        private static readonly string[] MonthNames = { "January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December" };

        public ExpenseEditForm(Expense? expense)
        {
            _existingExpense = expense;
            InitializeComponent();
            LoadExpenseData();
        }

        private void InitializeComponent()
        {
            this.Text = _existingExpense == null ? "Add New Expense" : "Edit Expense";
            this.Size = new Size(450, 480);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int labelWidth = 100;
            int controlWidth = 280;
            int startY = 20;
            int rowHeight = 45;
            int leftMargin = 20;

            // Category
            var lblCategory = new Label { Text = "Category:", Location = new Point(leftMargin, startY + 3), AutoSize = true, Font = new Font("Segoe UI", 10F) };
            cmbCategory = new ComboBox
            {
                Location = new Point(leftMargin + labelWidth, startY),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            LoadCategories();

            btnAddCategory = new Button
            {
                Text = "+",
                Location = new Point(leftMargin + labelWidth + 210, startY - 2),
                Size = new Size(30, 28),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnAddCategory.Click += BtnAddCategory_Click;

            // Description
            var lblDescription = new Label { Text = "Description:", Location = new Point(leftMargin, startY + rowHeight + 3), AutoSize = true, Font = new Font("Segoe UI", 10F) };
            txtDescription = new TextBox
            {
                Location = new Point(leftMargin + labelWidth, startY + rowHeight),
                Size = new Size(controlWidth, 25),
                Font = new Font("Segoe UI", 10F)
            };

            // Amount
            var lblAmount = new Label { Text = "Amount (₹):", Location = new Point(leftMargin, startY + rowHeight * 2 + 3), AutoSize = true, Font = new Font("Segoe UI", 10F) };
            nudAmount = new NumericUpDown
            {
                Location = new Point(leftMargin + labelWidth, startY + rowHeight * 2),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10F),
                Minimum = 0,
                Maximum = 999999,
                DecimalPlaces = 2
            };

            // Month
            var lblMonth = new Label { Text = "Month:", Location = new Point(leftMargin, startY + rowHeight * 3 + 3), AutoSize = true, Font = new Font("Segoe UI", 10F) };
            cmbMonth = new ComboBox
            {
                Location = new Point(leftMargin + labelWidth, startY + rowHeight * 3),
                Size = new Size(130, 25),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            for (int i = 0; i < 12; i++)
            {
                cmbMonth.Items.Add(new { Month = i + 1, Name = MonthNames[i] });
            }
            cmbMonth.DisplayMember = "Name";
            cmbMonth.ValueMember = "Month";
            cmbMonth.SelectedIndex = DateTime.Now.Month - 1;

            // Year
            var lblYear = new Label { Text = "Year:", Location = new Point(leftMargin + labelWidth + 150, startY + rowHeight * 3 + 3), AutoSize = true, Font = new Font("Segoe UI", 10F) };
            cmbYear = new ComboBox
            {
                Location = new Point(leftMargin + labelWidth + 195, startY + rowHeight * 3),
                Size = new Size(85, 25),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear + 1; year >= currentYear - 5; year--)
            {
                cmbYear.Items.Add(year);
            }
            cmbYear.SelectedItem = currentYear;

            // Expense Date
            var lblDate = new Label { Text = "Date:", Location = new Point(leftMargin, startY + rowHeight * 4 + 3), AutoSize = true, Font = new Font("Segoe UI", 10F) };
            dtpExpenseDate = new DateTimePicker
            {
                Location = new Point(leftMargin + labelWidth, startY + rowHeight * 4),
                Size = new Size(controlWidth, 25),
                Font = new Font("Segoe UI", 10F),
                Format = DateTimePickerFormat.Short
            };

            // Notes
            var lblNotes = new Label { Text = "Notes:", Location = new Point(leftMargin, startY + rowHeight * 5 + 3), AutoSize = true, Font = new Font("Segoe UI", 10F) };
            txtNotes = new TextBox
            {
                Location = new Point(leftMargin + labelWidth, startY + rowHeight * 5),
                Size = new Size(controlWidth, 80),
                Font = new Font("Segoe UI", 10F),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            // Buttons
            btnSave = new Button
            {
                Text = "Save",
                Location = new Point(leftMargin + labelWidth, startY + rowHeight * 5 + 100),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.None
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(leftMargin + labelWidth + 120, startY + rowHeight * 5 + 100),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 10F),
                DialogResult = DialogResult.Cancel
            };

            this.Controls.AddRange(new Control[]
            {
                lblCategory, cmbCategory, btnAddCategory,
                lblDescription, txtDescription,
                lblAmount, nudAmount,
                lblMonth, cmbMonth,
                lblYear, cmbYear,
                lblDate, dtpExpenseDate,
                lblNotes, txtNotes,
                btnSave, btnCancel
            });

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void LoadCategories()
        {
            cmbCategory.Items.Clear();
            var categories = _expenseService.GetAllCategories();
            foreach (var category in categories)
            {
                cmbCategory.Items.Add(new { category.CategoryId, category.CategoryName });
            }
            cmbCategory.DisplayMember = "CategoryName";
            cmbCategory.ValueMember = "CategoryId";
            if (cmbCategory.Items.Count > 0)
                cmbCategory.SelectedIndex = 0;
        }

        private void LoadExpenseData()
        {
            if (_existingExpense != null)
            {
                // Select category
                for (int i = 0; i < cmbCategory.Items.Count; i++)
                {
                    dynamic item = cmbCategory.Items[i]!;
                    if (item.CategoryId == _existingExpense.CategoryId)
                    {
                        cmbCategory.SelectedIndex = i;
                        break;
                    }
                }

                txtDescription.Text = _existingExpense.Description;
                nudAmount.Value = _existingExpense.Amount;
                cmbMonth.SelectedIndex = _existingExpense.ExpenseMonth - 1;
                cmbYear.SelectedItem = _existingExpense.ExpenseYear;
                dtpExpenseDate.Value = _existingExpense.ExpenseDate;
                txtNotes.Text = _existingExpense.Notes ?? "";
            }
        }

        private void BtnAddCategory_Click(object? sender, EventArgs e)
        {
            var categoryName = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter new category name:", "Add Category", "");

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                try
                {
                    var category = new ExpenseCategory { CategoryName = categoryName.Trim() };
                    _expenseService.AddCategory(category);
                    LoadCategories();

                    // Select the newly added category
                    for (int i = 0; i < cmbCategory.Items.Count; i++)
                    {
                        dynamic item = cmbCategory.Items[i]!;
                        if (item.CategoryName == categoryName.Trim())
                        {
                            cmbCategory.SelectedIndex = i;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding category: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Please enter a description.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDescription.Focus();
                return;
            }

            if (nudAmount.Value <= 0)
            {
                MessageBox.Show("Please enter an amount greater than zero.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudAmount.Focus();
                return;
            }

            try
            {
                dynamic? selectedCategory = cmbCategory.SelectedItem;
                dynamic selectedMonth = cmbMonth.SelectedItem!;

                if (_existingExpense == null)
                {
                    // Add new expense
                    var expense = new Expense
                    {
                        CategoryId = selectedCategory?.CategoryId,
                        Description = txtDescription.Text.Trim(),
                        Amount = nudAmount.Value,
                        ExpenseMonth = selectedMonth.Month,
                        ExpenseYear = (int)cmbYear.SelectedItem!,
                        ExpenseDate = dtpExpenseDate.Value.Date,
                        Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim()
                    };
                    _expenseService.AddExpense(expense);
                    MessageBox.Show("Expense added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Update existing expense
                    _existingExpense.CategoryId = selectedCategory?.CategoryId;
                    _existingExpense.Description = txtDescription.Text.Trim();
                    _existingExpense.Amount = nudAmount.Value;
                    _existingExpense.ExpenseMonth = selectedMonth.Month;
                    _existingExpense.ExpenseYear = (int)cmbYear.SelectedItem!;
                    _existingExpense.ExpenseDate = dtpExpenseDate.Value.Date;
                    _existingExpense.Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim();
                    _expenseService.UpdateExpense(_existingExpense);
                    MessageBox.Show("Expense updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving expense: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
