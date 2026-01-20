using BoxCricketTeamManager.Models;
using BoxCricketTeamManager.Services;

namespace BoxCricketTeamManager.Forms
{
    public partial class MemberEditForm : Form
    {
        private readonly MemberService _memberService = new();
        private readonly Member? _existingMember;

        private TextBox txtName;
        private TextBox txtPhone;
        private TextBox txtEmail;
        private DateTimePicker dtpJoinDate;
        private CheckBox chkIsActive;
        private TextBox txtNotes;
        private Button btnSave;
        private Button btnCancel;

        public MemberEditForm(Member? member)
        {
            _existingMember = member;
            InitializeComponent();
            LoadMemberData();
        }

        private void InitializeComponent()
        {
            this.Text = _existingMember == null ? "Add New Member" : "Edit Member";
            this.Size = new Size(450, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int labelWidth = 100;
            int controlWidth = 280;
            int startY = 20;
            int rowHeight = 40;
            int leftMargin = 20;

            // Name
            var lblName = new Label { Text = "Name:", Location = new Point(leftMargin, startY + 3), AutoSize = true, Font = new Font("Segoe UI", 10F) };
            txtName = new TextBox { Location = new Point(leftMargin + labelWidth, startY), Size = new Size(controlWidth, 25), Font = new Font("Segoe UI", 10F) };

            // Phone
            var lblPhone = new Label { Text = "Phone:", Location = new Point(leftMargin, startY + rowHeight + 3), AutoSize = true, Font = new Font("Segoe UI", 10F) };
            txtPhone = new TextBox { Location = new Point(leftMargin + labelWidth, startY + rowHeight), Size = new Size(controlWidth, 25), Font = new Font("Segoe UI", 10F) };

            // Email
            var lblEmail = new Label { Text = "Email:", Location = new Point(leftMargin, startY + rowHeight * 2 + 3), AutoSize = true, Font = new Font("Segoe UI", 10F) };
            txtEmail = new TextBox { Location = new Point(leftMargin + labelWidth, startY + rowHeight * 2), Size = new Size(controlWidth, 25), Font = new Font("Segoe UI", 10F) };

            // Join Date
            var lblJoinDate = new Label { Text = "Join Date:", Location = new Point(leftMargin, startY + rowHeight * 3 + 3), AutoSize = true, Font = new Font("Segoe UI", 10F) };
            dtpJoinDate = new DateTimePicker
            {
                Location = new Point(leftMargin + labelWidth, startY + rowHeight * 3),
                Size = new Size(controlWidth, 25),
                Font = new Font("Segoe UI", 10F),
                Format = DateTimePickerFormat.Short
            };

            // Is Active
            chkIsActive = new CheckBox
            {
                Text = "Active Member",
                Location = new Point(leftMargin + labelWidth, startY + rowHeight * 4),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F),
                Checked = true
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
                lblName, txtName,
                lblPhone, txtPhone,
                lblEmail, txtEmail,
                lblJoinDate, dtpJoinDate,
                chkIsActive,
                lblNotes, txtNotes,
                btnSave, btnCancel
            });

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void LoadMemberData()
        {
            if (_existingMember != null)
            {
                txtName.Text = _existingMember.Name;
                txtPhone.Text = _existingMember.Phone ?? "";
                txtEmail.Text = _existingMember.Email ?? "";
                dtpJoinDate.Value = _existingMember.JoinDate;
                chkIsActive.Checked = _existingMember.IsActive;
                txtNotes.Text = _existingMember.Notes ?? "";
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter a name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            try
            {
                if (_existingMember == null)
                {
                    // Add new member
                    var member = new Member
                    {
                        Name = txtName.Text.Trim().ToUpper(),
                        Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim(),
                        Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                        JoinDate = dtpJoinDate.Value.Date,
                        IsActive = chkIsActive.Checked,
                        Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim()
                    };
                    _memberService.AddMember(member);
                    MessageBox.Show("Member added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Update existing member
                    _existingMember.Name = txtName.Text.Trim().ToUpper();
                    _existingMember.Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim();
                    _existingMember.Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim();
                    _existingMember.JoinDate = dtpJoinDate.Value.Date;
                    _existingMember.IsActive = chkIsActive.Checked;
                    _existingMember.Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim();
                    _memberService.UpdateMember(_existingMember);
                    MessageBox.Show("Member updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving member: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
