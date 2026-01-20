using BoxCricketTeamManager.Models;
using BoxCricketTeamManager.Services;

namespace BoxCricketTeamManager.Forms
{
    public partial class PaymentGridForm : Form
    {
        private readonly MemberService _memberService = new();
        private readonly PaymentService _paymentService = new();
        private DataGridView dgvPayments;
        private ComboBox cmbYear;
        private Label lblMonthlyDue;
        private Label lblTotalCollection;
        private Button btnRefresh;
        private Button btnPrintReceipt;

        private static readonly string[] MonthNames = { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
        private int _selectedYear;
        private decimal _monthlyDueAmount;

        public PaymentGridForm()
        {
            _selectedYear = DateTime.Now.Year;
            InitializeComponent();
            LoadPaymentGrid();
        }

        private void InitializeComponent()
        {
            this.Text = "Payment Tracking";
            this.Size = new Size(1200, 700);

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
                Location = new Point(55, 15),
                Size = new Size(100, 25),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Populate years
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear + 1; year >= currentYear - 5; year--)
            {
                cmbYear.Items.Add(year);
            }
            cmbYear.SelectedItem = currentYear;
            cmbYear.SelectedIndexChanged += (s, e) =>
            {
                _selectedYear = (int)cmbYear.SelectedItem!;
                LoadPaymentGrid();
            };

            lblMonthlyDue = new Label
            {
                Text = "Monthly Due: ₹50",
                Location = new Point(180, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            lblTotalCollection = new Label
            {
                Text = "Total Collection: ₹0",
                Location = new Point(350, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113)
            };

            btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(550, 12),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRefresh.Click += (s, e) => LoadPaymentGrid();

            btnPrintReceipt = new Button
            {
                Text = "Print Receipt",
                Location = new Point(660, 12),
                Size = new Size(120, 35),
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnPrintReceipt.Click += BtnPrintReceipt_Click;

            pnlTop.Controls.AddRange(new Control[] { lblYear, cmbYear, lblMonthlyDue, lblTotalCollection, btnRefresh, btnPrintReceipt });

            // Legend panel
            var pnlLegend = new Panel
            {
                Dock = DockStyle.Top,
                Height = 35,
                Padding = new Padding(10)
            };

            var lblLegend = new Label
            {
                Text = "Legend:",
                Location = new Point(10, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F)
            };

            var pnlPaid = new Panel { Location = new Point(70, 5), Size = new Size(20, 20), BackColor = Color.FromArgb(46, 204, 113) };
            var lblPaid = new Label { Text = "Paid", Location = new Point(95, 8), AutoSize = true, Font = new Font("Segoe UI", 9F) };

            var pnlPending = new Panel { Location = new Point(140, 5), Size = new Size(20, 20), BackColor = Color.FromArgb(231, 76, 60) };
            var lblPending = new Label { Text = "Pending", Location = new Point(165, 8), AutoSize = true, Font = new Font("Segoe UI", 9F) };

            var pnlInactive = new Panel { Location = new Point(230, 5), Size = new Size(20, 20), BackColor = Color.FromArgb(189, 195, 199) };
            var lblInactive = new Label { Text = "Inactive", Location = new Point(255, 8), AutoSize = true, Font = new Font("Segoe UI", 9F) };

            var lblInstruction = new Label
            {
                Text = "Click on a cell to toggle payment status",
                Location = new Point(350, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.Gray
            };

            pnlLegend.Controls.AddRange(new Control[] { lblLegend, pnlPaid, lblPaid, pnlPending, lblPending, pnlInactive, lblInactive, lblInstruction });

            // Data grid
            dgvPayments = new DataGridView
            {
                Dock = DockStyle.Fill,
                SelectionMode = DataGridViewSelectionMode.CellSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9F),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 35
            };

            // Setup columns
            dgvPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "MemberId", HeaderText = "ID", Width = 0, Visible = false });
            dgvPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "IsActive", HeaderText = "Active", Width = 0, Visible = false });
            dgvPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Member Name", Width = 180, Frozen = true });

            // Month columns
            foreach (var month in MonthNames)
            {
                var col = new DataGridViewTextBoxColumn
                {
                    Name = month,
                    HeaderText = month,
                    Width = 55,
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
                };
                dgvPayments.Columns.Add(col);
            }

            dgvPayments.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Total",
                HeaderText = "TOTAL",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                }
            });

            dgvPayments.CellClick += DgvPayments_CellClick;
            dgvPayments.CellFormatting += DgvPayments_CellFormatting;

            // Add controls
            this.Controls.Add(dgvPayments);
            this.Controls.Add(pnlLegend);
            this.Controls.Add(pnlTop);
        }

        public void LoadPaymentGrid()
        {
            try
            {
                dgvPayments.Rows.Clear();

                _monthlyDueAmount = _paymentService.GetMonthlyDueAmount(_selectedYear);
                lblMonthlyDue.Text = $"Monthly Due: ₹{_monthlyDueAmount:N0}";

                var members = _memberService.GetAllMembers(includeInactive: true);
                var payments = _paymentService.GetPaymentsByYear(_selectedYear);

                decimal totalCollection = 0;

                foreach (var member in members)
                {
                    var memberPayments = payments.Where(p => p.MemberId == member.MemberId).ToList();
                    var row = new object[16];
                    row[0] = member.MemberId;
                    row[1] = member.IsActive;
                    row[2] = member.Name;

                    decimal memberTotal = 0;
                    for (int month = 1; month <= 12; month++)
                    {
                        var payment = memberPayments.FirstOrDefault(p => p.PaymentMonth == month);
                        if (payment != null)
                        {
                            row[2 + month] = "✓";
                            memberTotal += payment.Amount;
                        }
                        else
                        {
                            row[2 + month] = "";
                        }
                    }

                    row[15] = $"₹{memberTotal:N0}";
                    totalCollection += memberTotal;

                    dgvPayments.Rows.Add(row);
                }

                // Add monthly totals row
                var totalsRow = new object[16];
                totalsRow[0] = 0;
                totalsRow[1] = true;
                totalsRow[2] = "MONTHLY TOTAL";

                decimal grandTotal = 0;
                for (int month = 1; month <= 12; month++)
                {
                    var monthTotal = _paymentService.GetMonthlyCollection(month, _selectedYear);
                    totalsRow[2 + month] = monthTotal > 0 ? $"₹{monthTotal:N0}" : "";
                    grandTotal += monthTotal;
                }

                totalsRow[15] = $"₹{grandTotal:N0}";
                dgvPayments.Rows.Add(totalsRow);

                // Style the totals row
                var lastRow = dgvPayments.Rows[dgvPayments.Rows.Count - 1];
                lastRow.DefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
                lastRow.DefaultCellStyle.ForeColor = Color.White;
                lastRow.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

                lblTotalCollection.Text = $"Total Collection: ₹{totalCollection:N0}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading payment grid: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvPayments_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 3 || e.ColumnIndex > 14)
                return;

            // Don't allow editing totals row
            if (e.RowIndex == dgvPayments.Rows.Count - 1)
                return;

            var row = dgvPayments.Rows[e.RowIndex];
            var memberId = (int)row.Cells["MemberId"].Value;
            var isActive = (bool)row.Cells["IsActive"].Value;

            if (!isActive)
            {
                MessageBox.Show("Cannot record payment for inactive member.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int month = e.ColumnIndex - 2; // Columns 3-14 = months 1-12

            try
            {
                _paymentService.TogglePayment(memberId, month, _selectedYear, _monthlyDueAmount);
                LoadPaymentGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating payment: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvPayments_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvPayments.Rows.Count - 1)
                return;

            if (e.ColumnIndex >= 3 && e.ColumnIndex <= 14) // Month columns
            {
                var row = dgvPayments.Rows[e.RowIndex];
                var isActive = (bool)row.Cells["IsActive"].Value;
                var cellValue = e.Value?.ToString();

                if (!isActive)
                {
                    e.CellStyle!.BackColor = Color.FromArgb(189, 195, 199); // Gray for inactive
                    e.CellStyle.ForeColor = Color.Gray;
                }
                else if (cellValue == "✓")
                {
                    e.CellStyle!.BackColor = Color.FromArgb(46, 204, 113); // Green for paid
                    e.CellStyle.ForeColor = Color.White;
                    e.CellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                }
                else
                {
                    e.CellStyle!.BackColor = Color.FromArgb(231, 76, 60); // Red for pending
                    e.CellStyle.ForeColor = Color.White;
                }
            }
        }

        private void BtnPrintReceipt_Click(object? sender, EventArgs e)
        {
            if (dgvPayments.SelectedCells.Count == 0)
            {
                MessageBox.Show("Please select a member row to print receipt.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var rowIndex = dgvPayments.SelectedCells[0].RowIndex;
            if (rowIndex == dgvPayments.Rows.Count - 1) // Totals row
            {
                MessageBox.Show("Please select a member row.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var row = dgvPayments.Rows[rowIndex];
            var memberId = (int)row.Cells["MemberId"].Value;
            var memberName = row.Cells["Name"].Value.ToString();

            using var receiptForm = new ReceiptForm(memberId, memberName!, _selectedYear);
            receiptForm.ShowDialog();
        }
    }
}
