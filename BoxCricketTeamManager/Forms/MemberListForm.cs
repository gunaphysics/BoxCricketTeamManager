using BoxCricketTeamManager.Models;
using BoxCricketTeamManager.Services;

namespace BoxCricketTeamManager.Forms
{
    public partial class MemberListForm : Form
    {
        private readonly MemberService _memberService = new();
        private DataGridView dgvMembers;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnToggleActive;
        private Button btnDelete;
        private CheckBox chkShowInactive;
        private Label lblMemberCount;

        public MemberListForm()
        {
            InitializeComponent();
            LoadMembers();
        }

        private void InitializeComponent()
        {
            this.Text = "Member Management";
            this.Size = new Size(1000, 600);

            // Search panel
            var pnlSearch = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(10)
            };

            var lblSearch = new Label
            {
                Text = "Search:",
                Location = new Point(10, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F)
            };

            txtSearch = new TextBox
            {
                Location = new Point(70, 15),
                Size = new Size(250, 25),
                Font = new Font("Segoe UI", 10F)
            };
            txtSearch.KeyPress += (s, e) => { if (e.KeyChar == (char)Keys.Enter) LoadMembers(); };

            btnSearch = new Button
            {
                Text = "Search",
                Location = new Point(330, 13),
                Size = new Size(80, 30),
                Font = new Font("Segoe UI", 9F)
            };
            btnSearch.Click += (s, e) => LoadMembers();

            chkShowInactive = new CheckBox
            {
                Text = "Show Inactive",
                Location = new Point(430, 17),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F)
            };
            chkShowInactive.CheckedChanged += (s, e) => LoadMembers();

            lblMemberCount = new Label
            {
                Text = "Members: 0",
                Location = new Point(580, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            pnlSearch.Controls.AddRange(new Control[] { lblSearch, txtSearch, btnSearch, chkShowInactive, lblMemberCount });

            // Button panel
            var pnlButtons = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(10)
            };

            btnAdd = new Button
            {
                Text = "Add Member",
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

            btnToggleActive = new Button
            {
                Text = "Toggle Active",
                Location = new Point(230, 8),
                Size = new Size(120, 35),
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnToggleActive.Click += BtnToggleActive_Click;

            btnDelete = new Button
            {
                Text = "Delete",
                Location = new Point(360, 8),
                Size = new Size(80, 35),
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnDelete.Click += BtnDelete_Click;

            pnlButtons.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnToggleActive, btnDelete });

            // Data grid
            dgvMembers = new DataGridView
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
            dgvMembers.CellDoubleClick += (s, e) => BtnEdit_Click(s, e);

            // Add columns
            dgvMembers.Columns.Add(new DataGridViewTextBoxColumn { Name = "MemberId", HeaderText = "ID", Width = 50, Visible = false });
            dgvMembers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Name", FillWeight = 150 });
            dgvMembers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Phone", HeaderText = "Phone", FillWeight = 100 });
            dgvMembers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", FillWeight = 150 });
            dgvMembers.Columns.Add(new DataGridViewTextBoxColumn { Name = "JoinDate", HeaderText = "Join Date", FillWeight = 80 });
            dgvMembers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Status", FillWeight = 60 });
            dgvMembers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Notes", HeaderText = "Notes", FillWeight = 100 });

            // Add controls in reverse order (bottom to top for docking)
            this.Controls.Add(dgvMembers);
            this.Controls.Add(pnlButtons);
            this.Controls.Add(pnlSearch);
        }

        public void LoadMembers()
        {
            try
            {
                dgvMembers.Rows.Clear();
                var searchTerm = txtSearch.Text.Trim();
                var members = string.IsNullOrEmpty(searchTerm)
                    ? _memberService.GetAllMembers(chkShowInactive.Checked)
                    : _memberService.SearchMembers(searchTerm, chkShowInactive.Checked);

                foreach (var member in members)
                {
                    dgvMembers.Rows.Add(
                        member.MemberId,
                        member.Name,
                        member.Phone ?? "",
                        member.Email ?? "",
                        member.JoinDate.ToString("dd-MMM-yyyy"),
                        member.IsActive ? "Active" : "Inactive",
                        member.Notes ?? ""
                    );

                    // Color inactive rows
                    if (!member.IsActive)
                    {
                        dgvMembers.Rows[dgvMembers.Rows.Count - 1].DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
                        dgvMembers.Rows[dgvMembers.Rows.Count - 1].DefaultCellStyle.ForeColor = Color.Gray;
                    }
                }

                lblMemberCount.Text = $"Members: {members.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading members: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            using var form = new MemberEditForm(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadMembers();
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a member to edit.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var memberId = (int)dgvMembers.SelectedRows[0].Cells["MemberId"].Value;
            var member = _memberService.GetMemberById(memberId);

            if (member != null)
            {
                using var form = new MemberEditForm(member);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadMembers();
                }
            }
        }

        private void BtnToggleActive_Click(object? sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a member.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var memberId = (int)dgvMembers.SelectedRows[0].Cells["MemberId"].Value;
            var memberName = dgvMembers.SelectedRows[0].Cells["Name"].Value.ToString();
            var currentStatus = dgvMembers.SelectedRows[0].Cells["Status"].Value.ToString();
            var isActive = currentStatus == "Active";

            var action = isActive ? "deactivate" : "activate";
            var result = MessageBox.Show($"Are you sure you want to {action} {memberName}?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (isActive)
                    _memberService.DeactivateMember(memberId);
                else
                    _memberService.ActivateMember(memberId);
                LoadMembers();
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvMembers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a member to delete.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var memberId = (int)dgvMembers.SelectedRows[0].Cells["MemberId"].Value;
            var memberName = dgvMembers.SelectedRows[0].Cells["Name"].Value.ToString();

            var result = MessageBox.Show(
                $"Are you sure you want to permanently delete {memberName}?\n\nThis will also delete all payment records for this member.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                _memberService.DeleteMember(memberId);
                LoadMembers();
            }
        }
    }
}
