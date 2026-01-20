using BoxCricketTeamManager.Data;

namespace BoxCricketTeamManager.Forms
{
    public partial class MainForm : Form
    {
        private TabControl tabControl;
        private TabPage tabDashboard;
        private TabPage tabMembers;
        private TabPage tabPayments;
        private TabPage tabExpenses;
        private TabPage tabReports;
        private TabPage tabSettings;

        // Dashboard controls
        private Panel pnlDashboard;
        private Label lblActiveMembers;
        private Label lblActiveMembersValue;
        private Label lblMonthlyCollection;
        private Label lblMonthlyCollectionValue;
        private Label lblMonthlyExpenses;
        private Label lblMonthlyExpensesValue;
        private Label lblCurrentBalance;
        private Label lblCurrentBalanceValue;
        private Label lblPendingPayments;
        private ListBox lstPendingPayments;
        private Button btnRefreshDashboard;

        public MainForm()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        private void InitializeComponent()
        {
            this.Text = "Box Cricket Team Manager";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1000, 700);

            // Create TabControl
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                Padding = new Point(15, 6)
            };

            // Create tabs
            tabDashboard = new TabPage("Dashboard") { Tag = "Dashboard" };
            tabMembers = new TabPage("Members") { Tag = "Members" };
            tabPayments = new TabPage("Payments") { Tag = "Payments" };
            tabExpenses = new TabPage("Expenses") { Tag = "Expenses" };
            tabReports = new TabPage("Reports") { Tag = "Reports" };
            tabSettings = new TabPage("Settings") { Tag = "Settings" };

            tabControl.TabPages.Add(tabDashboard);
            tabControl.TabPages.Add(tabMembers);
            tabControl.TabPages.Add(tabPayments);
            tabControl.TabPages.Add(tabExpenses);
            tabControl.TabPages.Add(tabReports);
            tabControl.TabPages.Add(tabSettings);

            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;

            this.Controls.Add(tabControl);

            // Initialize Dashboard
            InitializeDashboard();
        }

        private void InitializeDashboard()
        {
            pnlDashboard = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };
            tabDashboard.Controls.Add(pnlDashboard);

            int cardWidth = 250;
            int cardHeight = 100;
            int spacing = 20;
            int startY = 20;

            // Card 1 - Active Members
            var cardMembers = CreateCard("Active Members", "0", 20, startY, cardWidth, cardHeight, Color.FromArgb(52, 152, 219));
            lblActiveMembersValue = (Label)cardMembers.Controls[1];
            pnlDashboard.Controls.Add(cardMembers);

            // Card 2 - Monthly Collection
            var cardCollection = CreateCard($"Collection ({DateTime.Now:MMM yyyy})", "₹0 / ₹0", 20 + cardWidth + spacing, startY, cardWidth, cardHeight, Color.FromArgb(46, 204, 113));
            lblMonthlyCollectionValue = (Label)cardCollection.Controls[1];
            pnlDashboard.Controls.Add(cardCollection);

            // Card 3 - Monthly Expenses
            var cardExpenses = CreateCard($"Expenses ({DateTime.Now:MMM yyyy})", "₹0", 20 + (cardWidth + spacing) * 2, startY, cardWidth, cardHeight, Color.FromArgb(231, 76, 60));
            lblMonthlyExpensesValue = (Label)cardExpenses.Controls[1];
            pnlDashboard.Controls.Add(cardExpenses);

            // Card 4 - Current Balance
            var cardBalance = CreateCard("Current Balance", "₹0", 20 + (cardWidth + spacing) * 3, startY, cardWidth, cardHeight, Color.FromArgb(155, 89, 182));
            lblCurrentBalanceValue = (Label)cardBalance.Controls[1];
            pnlDashboard.Controls.Add(cardBalance);

            // Pending Payments Section
            lblPendingPayments = new Label
            {
                Text = $"Pending Payments - {DateTime.Now:MMMM yyyy}",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(20, startY + cardHeight + 40),
                AutoSize = true
            };
            pnlDashboard.Controls.Add(lblPendingPayments);

            lstPendingPayments = new ListBox
            {
                Location = new Point(20, startY + cardHeight + 70),
                Size = new Size(600, 350),
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlDashboard.Controls.Add(lstPendingPayments);

            // Refresh Button
            btnRefreshDashboard = new Button
            {
                Text = "Refresh",
                Location = new Point(640, startY + cardHeight + 70),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRefreshDashboard.Click += (s, e) => LoadDashboardData();
            pnlDashboard.Controls.Add(btnRefreshDashboard);
        }

        private Panel CreateCard(string title, string value, int x, int y, int width, int height, Color color)
        {
            var card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = color,
                BorderStyle = BorderStyle.None
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.White,
                Location = new Point(15, 15),
                AutoSize = true
            };

            var lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 45),
                AutoSize = true
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);

            return card;
        }

        private void LoadDashboardData()
        {
            try
            {
                using var context = Program.CreateDbContext();

                // Active members count
                int activeMembers = context.Members.Count(m => m.IsActive);
                lblActiveMembersValue.Text = activeMembers.ToString();

                // Current month collection
                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;

                decimal collected = context.Payments
                    .Where(p => p.PaymentMonth == currentMonth && p.PaymentYear == currentYear)
                    .AsEnumerable()
                    .Sum(p => p.Amount);

                var monthlyDue = context.MonthlyDues.FirstOrDefault(m => m.Year == currentYear);
                decimal monthlyAmount = monthlyDue?.MonthlyAmount ?? 50;
                decimal target = activeMembers * monthlyAmount;

                lblMonthlyCollectionValue.Text = $"₹{collected:N0} / ₹{target:N0}";

                // Current month expenses
                decimal expenses = context.Expenses
                    .Where(e => e.ExpenseMonth == currentMonth && e.ExpenseYear == currentYear)
                    .AsEnumerable()
                    .Sum(e => e.Amount);
                lblMonthlyExpensesValue.Text = $"₹{expenses:N0}";

                // Calculate current balance
                var yearlyBalance = context.YearlyBalances.FirstOrDefault(y => y.Year == currentYear);
                decimal openingBalance = yearlyBalance?.OpeningBalance ?? 0;

                decimal totalPayments = context.Payments
                    .Where(p => p.PaymentYear == currentYear)
                    .AsEnumerable()
                    .Sum(p => p.Amount);

                decimal totalExpenses = context.Expenses
                    .Where(e => e.ExpenseYear == currentYear)
                    .AsEnumerable()
                    .Sum(e => e.Amount);

                decimal currentBalance = openingBalance + totalPayments - totalExpenses;
                lblCurrentBalanceValue.Text = $"₹{currentBalance:N0}";

                // Pending payments for current month
                lstPendingPayments.Items.Clear();
                var paidMemberIds = context.Payments
                    .Where(p => p.PaymentMonth == currentMonth && p.PaymentYear == currentYear)
                    .Select(p => p.MemberId)
                    .ToList();

                var pendingMembers = context.Members
                    .Where(m => m.IsActive && !paidMemberIds.Contains(m.MemberId))
                    .OrderBy(m => m.Name)
                    .ToList();

                if (pendingMembers.Any())
                {
                    foreach (var member in pendingMembers)
                    {
                        lstPendingPayments.Items.Add($"{member.Name} - ₹{monthlyAmount:N0} pending");
                    }
                }
                else
                {
                    lstPendingPayments.Items.Add("All payments collected for this month!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TabControl_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var selectedTab = tabControl.SelectedTab;
            if (selectedTab == null) return;

            // Lazy load tab contents
            switch (selectedTab.Tag?.ToString())
            {
                case "Dashboard":
                    LoadDashboardData();
                    break;
                case "Members":
                    if (selectedTab.Controls.Count == 0)
                        LoadMembersTab();
                    break;
                case "Payments":
                    if (selectedTab.Controls.Count == 0)
                        LoadPaymentsTab();
                    break;
                case "Expenses":
                    if (selectedTab.Controls.Count == 0)
                        LoadExpensesTab();
                    break;
                case "Reports":
                    if (selectedTab.Controls.Count == 0)
                        LoadReportsTab();
                    break;
                case "Settings":
                    if (selectedTab.Controls.Count == 0)
                        LoadSettingsTab();
                    break;
            }
        }

        private void LoadMembersTab()
        {
            var memberListForm = new MemberListForm { TopLevel = false, Dock = DockStyle.Fill, FormBorderStyle = FormBorderStyle.None };
            tabMembers.Controls.Add(memberListForm);
            memberListForm.Show();
        }

        private void LoadPaymentsTab()
        {
            var paymentGridForm = new PaymentGridForm { TopLevel = false, Dock = DockStyle.Fill, FormBorderStyle = FormBorderStyle.None };
            tabPayments.Controls.Add(paymentGridForm);
            paymentGridForm.Show();
        }

        private void LoadExpensesTab()
        {
            var expenseListForm = new ExpenseListForm { TopLevel = false, Dock = DockStyle.Fill, FormBorderStyle = FormBorderStyle.None };
            tabExpenses.Controls.Add(expenseListForm);
            expenseListForm.Show();
        }

        private void LoadReportsTab()
        {
            var reportsForm = new ReportsForm { TopLevel = false, Dock = DockStyle.Fill, FormBorderStyle = FormBorderStyle.None };
            tabReports.Controls.Add(reportsForm);
            reportsForm.Show();
        }

        private void LoadSettingsTab()
        {
            var settingsForm = new SettingsForm { TopLevel = false, Dock = DockStyle.Fill, FormBorderStyle = FormBorderStyle.None };
            tabSettings.Controls.Add(settingsForm);
            settingsForm.Show();
        }
    }
}
