using BoxCricketTeamManager.Services;
using BoxCricketTeamManager.Utilities;

namespace BoxCricketTeamManager.Forms
{
    public partial class ReportsForm : Form
    {
        private readonly ReportService _reportService = new();
        private ComboBox cmbYear;
        private ComboBox cmbReportType;
        private Button btnGenerateReport;
        private Button btnExportExcel;
        private DataGridView dgvReport;
        private Panel pnlSummary;
        private Label lblSummary;

        public ReportsForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Reports";
            this.Size = new Size(1100, 700);

            // Top panel
            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                Padding = new Padding(10)
            };

            var lblYear = new Label
            {
                Text = "Year:",
                Location = new Point(10, 23),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F)
            };

            cmbYear = new ComboBox
            {
                Location = new Point(55, 20),
                Size = new Size(80, 25),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            int currentYear = DateTime.Now.Year;
            for (int year = currentYear + 1; year >= currentYear - 5; year--)
            {
                cmbYear.Items.Add(year);
            }
            cmbYear.SelectedItem = currentYear;

            var lblReport = new Label
            {
                Text = "Report Type:",
                Location = new Point(160, 23),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F)
            };

            cmbReportType = new ComboBox
            {
                Location = new Point(255, 20),
                Size = new Size(250, 25),
                Font = new Font("Segoe UI", 10F),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbReportType.Items.AddRange(new object[]
            {
                "Yearly Summary",
                "Monthly Collections & Expenses",
                "Member Payment Status",
                "Expense Report by Category",
                "Pending Payments"
            });
            cmbReportType.SelectedIndex = 0;

            btnGenerateReport = new Button
            {
                Text = "Generate Report",
                Location = new Point(530, 17),
                Size = new Size(140, 35),
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnGenerateReport.Click += BtnGenerateReport_Click;

            btnExportExcel = new Button
            {
                Text = "Export to Excel",
                Location = new Point(680, 17),
                Size = new Size(140, 35),
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExportExcel.Click += BtnExportExcel_Click;

            pnlTop.Controls.AddRange(new Control[] { lblYear, cmbYear, lblReport, cmbReportType, btnGenerateReport, btnExportExcel });

            // Summary panel
            pnlSummary = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(236, 240, 241),
                Visible = false
            };

            lblSummary = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F),
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlSummary.Controls.Add(lblSummary);

            // Data grid
            dgvReport = new DataGridView
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

            // Add controls
            this.Controls.Add(dgvReport);
            this.Controls.Add(pnlSummary);
            this.Controls.Add(pnlTop);
        }

        private void BtnGenerateReport_Click(object? sender, EventArgs e)
        {
            if (cmbYear.SelectedItem == null || cmbReportType.SelectedItem == null)
                return;

            int year = (int)cmbYear.SelectedItem;
            string reportType = cmbReportType.SelectedItem.ToString()!;

            try
            {
                dgvReport.Columns.Clear();
                dgvReport.Rows.Clear();

                switch (reportType)
                {
                    case "Yearly Summary":
                        GenerateYearlySummary(year);
                        break;
                    case "Monthly Collections & Expenses":
                        GenerateMonthlySummary(year);
                        break;
                    case "Member Payment Status":
                        GenerateMemberPaymentStatus(year);
                        break;
                    case "Expense Report by Category":
                        GenerateExpenseByCategory(year);
                        break;
                    case "Pending Payments":
                        GeneratePendingPayments(year);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateYearlySummary(int year)
        {
            var summary = _reportService.GetYearlySummary(year);

            dgvReport.Columns.Add("Item", "Item");
            dgvReport.Columns.Add("Amount", "Amount");
            dgvReport.Columns["Amount"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvReport.Rows.Add("Opening Balance", $"₹{summary.OpeningBalance:N2}");
            dgvReport.Rows.Add("Total Collections", $"₹{summary.TotalCollections:N2}");
            dgvReport.Rows.Add("Total Expenses", $"₹{summary.TotalExpenses:N2}");
            dgvReport.Rows.Add("Closing Balance", $"₹{summary.ClosingBalance:N2}");

            // Style closing balance row
            var lastRow = dgvReport.Rows[dgvReport.Rows.Count - 1];
            lastRow.DefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lastRow.DefaultCellStyle.BackColor = summary.ClosingBalance >= 0
                ? Color.FromArgb(212, 237, 218)
                : Color.FromArgb(248, 215, 218);

            pnlSummary.Visible = true;
            lblSummary.Text = $"Year: {year}\n" +
                $"Opening Balance: ₹{summary.OpeningBalance:N2} | " +
                $"Collections: ₹{summary.TotalCollections:N2} | " +
                $"Expenses: ₹{summary.TotalExpenses:N2} | " +
                $"Closing Balance: ₹{summary.ClosingBalance:N2}";
        }

        private void GenerateMonthlySummary(int year)
        {
            var summaries = _reportService.GetMonthlySummaries(year);

            dgvReport.Columns.Add("Month", "Month");
            dgvReport.Columns.Add("Collections", "Collections");
            dgvReport.Columns.Add("Expenses", "Expenses");
            dgvReport.Columns.Add("NetBalance", "Net Balance");

            dgvReport.Columns["Collections"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvReport.Columns["Expenses"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvReport.Columns["NetBalance"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            decimal totalCollections = 0;
            decimal totalExpenses = 0;

            foreach (var summary in summaries)
            {
                var row = dgvReport.Rows.Add(
                    summary.MonthName,
                    $"₹{summary.Collections:N2}",
                    $"₹{summary.Expenses:N2}",
                    $"₹{summary.NetBalance:N2}"
                );

                if (summary.NetBalance < 0)
                    dgvReport.Rows[row].Cells["NetBalance"].Style.ForeColor = Color.Red;

                totalCollections += summary.Collections;
                totalExpenses += summary.Expenses;
            }

            // Add totals row
            var totalsRow = dgvReport.Rows.Add("TOTAL", $"₹{totalCollections:N2}", $"₹{totalExpenses:N2}", $"₹{totalCollections - totalExpenses:N2}");
            dgvReport.Rows[totalsRow].DefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvReport.Rows[totalsRow].DefaultCellStyle.BackColor = Color.FromArgb(236, 240, 241);

            pnlSummary.Visible = true;
            lblSummary.Text = $"Year: {year}\nTotal Collections: ₹{totalCollections:N2} | Total Expenses: ₹{totalExpenses:N2} | Net: ₹{totalCollections - totalExpenses:N2}";
        }

        private void GenerateMemberPaymentStatus(int year)
        {
            var summaries = _reportService.GetMemberPaymentSummaries(year);
            var monthlyDue = _reportService.GetMonthlyDueAmount(year);

            dgvReport.Columns.Add("Name", "Member Name");
            for (int i = 1; i <= 12; i++)
            {
                var col = new DataGridViewTextBoxColumn
                {
                    Name = $"M{i}",
                    HeaderText = new DateTime(2000, i, 1).ToString("MMM"),
                    Width = 50
                };
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvReport.Columns.Add(col);
            }
            dgvReport.Columns.Add("Total", "Total");
            dgvReport.Columns.Add("Pending", "Pending");

            dgvReport.Columns["Total"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvReport.Columns["Pending"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            foreach (var summary in summaries)
            {
                var rowData = new object[15];
                rowData[0] = summary.MemberName;

                for (int i = 0; i < 12; i++)
                {
                    rowData[i + 1] = summary.MonthsPaid[i] ? "✓" : "";
                }

                rowData[13] = $"₹{summary.TotalPaid:N0}";
                rowData[14] = $"₹{(12 - summary.MonthsPaidCount) * monthlyDue:N0}";

                var rowIndex = dgvReport.Rows.Add(rowData);

                // Color month cells
                for (int i = 0; i < 12; i++)
                {
                    var cell = dgvReport.Rows[rowIndex].Cells[$"M{i + 1}"];
                    if (summary.MonthsPaid[i])
                    {
                        cell.Style.BackColor = Color.FromArgb(46, 204, 113);
                        cell.Style.ForeColor = Color.White;
                    }
                    else
                    {
                        cell.Style.BackColor = Color.FromArgb(231, 76, 60);
                        cell.Style.ForeColor = Color.White;
                    }
                }
            }

            pnlSummary.Visible = true;
            decimal totalCollected = summaries.Sum(s => s.TotalPaid);
            decimal totalExpected = summaries.Count * 12 * monthlyDue;
            lblSummary.Text = $"Year: {year} | Members: {summaries.Count} | Monthly Due: ₹{monthlyDue:N0}\n" +
                $"Total Collected: ₹{totalCollected:N0} | Expected (Full Year): ₹{totalExpected:N0}";
        }

        private void GenerateExpenseByCategory(int year)
        {
            var expensesByCategory = new ExpenseService().GetExpensesByCategory(year);

            dgvReport.Columns.Add("Category", "Category");
            dgvReport.Columns.Add("Amount", "Total Amount");
            dgvReport.Columns.Add("Percentage", "Percentage");

            dgvReport.Columns["Amount"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvReport.Columns["Percentage"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            decimal total = expensesByCategory.Values.Sum();

            foreach (var kvp in expensesByCategory.OrderByDescending(k => k.Value))
            {
                decimal percentage = total > 0 ? (kvp.Value / total) * 100 : 0;
                dgvReport.Rows.Add(kvp.Key, $"₹{kvp.Value:N2}", $"{percentage:N1}%");
            }

            // Add totals row
            var totalsRow = dgvReport.Rows.Add("TOTAL", $"₹{total:N2}", "100%");
            dgvReport.Rows[totalsRow].DefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvReport.Rows[totalsRow].DefaultCellStyle.BackColor = Color.FromArgb(236, 240, 241);

            pnlSummary.Visible = true;
            lblSummary.Text = $"Year: {year}\nTotal Expenses: ₹{total:N2} | Categories: {expensesByCategory.Count}";
        }

        private void GeneratePendingPayments(int year)
        {
            var memberService = new MemberService();
            var monthlyDue = _reportService.GetMonthlyDueAmount(year);
            int currentMonth = DateTime.Now.Year == year ? DateTime.Now.Month : 12;

            dgvReport.Columns.Add("Name", "Member Name");
            dgvReport.Columns.Add("PendingMonths", "Pending Months");
            dgvReport.Columns.Add("PendingAmount", "Pending Amount");

            dgvReport.Columns["PendingAmount"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            var memberSummaries = _reportService.GetMemberPaymentSummaries(year);
            decimal totalPending = 0;

            foreach (var summary in memberSummaries)
            {
                var pendingMonths = new List<string>();
                for (int i = 0; i < currentMonth; i++)
                {
                    if (!summary.MonthsPaid[i])
                    {
                        pendingMonths.Add(new DateTime(2000, i + 1, 1).ToString("MMM"));
                    }
                }

                if (pendingMonths.Any())
                {
                    decimal pendingAmount = pendingMonths.Count * monthlyDue;
                    totalPending += pendingAmount;
                    dgvReport.Rows.Add(summary.MemberName, string.Join(", ", pendingMonths), $"₹{pendingAmount:N0}");
                }
            }

            // Add totals row
            var totalsRow = dgvReport.Rows.Add("TOTAL PENDING", "", $"₹{totalPending:N0}");
            dgvReport.Rows[totalsRow].DefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvReport.Rows[totalsRow].DefaultCellStyle.BackColor = Color.FromArgb(248, 215, 218);

            pnlSummary.Visible = true;
            lblSummary.Text = $"Year: {year} (Up to {new DateTime(2000, currentMonth, 1):MMMM})\n" +
                $"Members with Pending Payments: {dgvReport.Rows.Count - 1} | Total Pending: ₹{totalPending:N0}";
        }

        private void BtnExportExcel_Click(object? sender, EventArgs e)
        {
            if (dgvReport.Rows.Count == 0)
            {
                MessageBox.Show("Please generate a report first.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var saveDialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                FileName = $"Report_{cmbReportType.SelectedItem}_{cmbYear.SelectedItem}_{DateTime.Now:yyyyMMdd}.xlsx"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ExcelExporter.ExportDataGridViewToExcel(dgvReport, saveDialog.FileName,
                        $"{cmbReportType.SelectedItem} - {cmbYear.SelectedItem}");
                    MessageBox.Show($"Report exported successfully!\n\n{saveDialog.FileName}", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting report: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
