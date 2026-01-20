using System.Drawing.Printing;
using BoxCricketTeamManager.Services;

namespace BoxCricketTeamManager.Forms
{
    public partial class ReceiptForm : Form
    {
        private readonly PaymentService _paymentService = new();
        private readonly int _memberId;
        private readonly string _memberName;
        private readonly int _year;

        private Panel pnlReceipt;
        private PrintDocument printDocument;
        private PrintPreviewDialog printPreviewDialog;

        public ReceiptForm(int memberId, string memberName, int year)
        {
            _memberId = memberId;
            _memberName = memberName;
            _year = year;
            InitializeComponent();
            CreateReceiptContent();
        }

        private void InitializeComponent()
        {
            this.Text = $"Payment Receipt - {_memberName}";
            this.Size = new Size(500, 650);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;

            // Receipt panel
            pnlReceipt = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(440, 500),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Buttons
            var btnPrint = new Button
            {
                Text = "Print",
                Location = new Point(120, 540),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnPrint.Click += BtnPrint_Click;

            var btnClose = new Button
            {
                Text = "Close",
                Location = new Point(240, 540),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 10F),
                DialogResult = DialogResult.Cancel
            };

            this.Controls.Add(pnlReceipt);
            this.Controls.Add(btnPrint);
            this.Controls.Add(btnClose);

            // Setup printing
            printDocument = new PrintDocument();
            printDocument.PrintPage += PrintDocument_PrintPage;

            printPreviewDialog = new PrintPreviewDialog
            {
                Document = printDocument
            };
        }

        private void CreateReceiptContent()
        {
            int y = 20;
            int leftMargin = 20;

            // Header
            var lblTitle = new Label
            {
                Text = "BOX CRICKET TEAM",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(leftMargin, y),
                AutoSize = true
            };
            pnlReceipt.Controls.Add(lblTitle);
            y += 35;

            var lblSubtitle = new Label
            {
                Text = "Payment Receipt",
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(leftMargin, y),
                AutoSize = true
            };
            pnlReceipt.Controls.Add(lblSubtitle);
            y += 40;

            // Divider
            var divider1 = new Panel
            {
                Location = new Point(leftMargin, y),
                Size = new Size(400, 2),
                BackColor = Color.FromArgb(52, 73, 94)
            };
            pnlReceipt.Controls.Add(divider1);
            y += 20;

            // Receipt details
            var lblReceiptNo = new Label
            {
                Text = $"Receipt No: {DateTime.Now:yyyyMMddHHmmss}",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(leftMargin, y),
                AutoSize = true
            };
            pnlReceipt.Controls.Add(lblReceiptNo);

            var lblDate = new Label
            {
                Text = $"Date: {DateTime.Now:dd-MMM-yyyy}",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(250, y),
                AutoSize = true
            };
            pnlReceipt.Controls.Add(lblDate);
            y += 35;

            // Member name
            var lblMemberLabel = new Label
            {
                Text = "Received From:",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(leftMargin, y),
                AutoSize = true
            };
            pnlReceipt.Controls.Add(lblMemberLabel);
            y += 25;

            var lblMemberName = new Label
            {
                Text = _memberName,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(leftMargin, y),
                AutoSize = true
            };
            pnlReceipt.Controls.Add(lblMemberName);
            y += 40;

            // Year
            var lblYearLabel = new Label
            {
                Text = $"For Year: {_year}",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(leftMargin, y),
                AutoSize = true
            };
            pnlReceipt.Controls.Add(lblYearLabel);
            y += 30;

            // Divider
            var divider2 = new Panel
            {
                Location = new Point(leftMargin, y),
                Size = new Size(400, 1),
                BackColor = Color.FromArgb(189, 195, 199)
            };
            pnlReceipt.Controls.Add(divider2);
            y += 15;

            // Payment details header
            var lblDetailsHeader = new Label
            {
                Text = "Payment Details:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(leftMargin, y),
                AutoSize = true
            };
            pnlReceipt.Controls.Add(lblDetailsHeader);
            y += 25;

            // Get payments for the year
            var monthlyDue = _paymentService.GetMonthlyDueAmount(_year);
            string[] months = { "January", "February", "March", "April", "May", "June",
                               "July", "August", "September", "October", "November", "December" };

            decimal totalPaid = 0;
            int paidMonths = 0;

            for (int month = 1; month <= 12; month++)
            {
                var payment = _paymentService.GetPayment(_memberId, month, _year);
                if (payment != null)
                {
                    var lblPayment = new Label
                    {
                        Text = $"{months[month - 1]}",
                        Font = new Font("Segoe UI", 9F),
                        Location = new Point(leftMargin + 20, y),
                        Size = new Size(100, 20)
                    };
                    pnlReceipt.Controls.Add(lblPayment);

                    var lblAmount = new Label
                    {
                        Text = $"₹{payment.Amount:N0}",
                        Font = new Font("Segoe UI", 9F),
                        Location = new Point(150, y),
                        Size = new Size(80, 20),
                        TextAlign = ContentAlignment.MiddleRight
                    };
                    pnlReceipt.Controls.Add(lblAmount);

                    var lblPaymentDate = new Label
                    {
                        Text = $"({payment.PaymentDate:dd-MMM})",
                        Font = new Font("Segoe UI", 8F),
                        ForeColor = Color.Gray,
                        Location = new Point(240, y),
                        AutoSize = true
                    };
                    pnlReceipt.Controls.Add(lblPaymentDate);

                    totalPaid += payment.Amount;
                    paidMonths++;
                    y += 20;
                }
            }

            if (paidMonths == 0)
            {
                var lblNoPayments = new Label
                {
                    Text = "No payments recorded for this year",
                    Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    Location = new Point(leftMargin + 20, y),
                    AutoSize = true
                };
                pnlReceipt.Controls.Add(lblNoPayments);
                y += 25;
            }

            y += 10;

            // Divider
            var divider3 = new Panel
            {
                Location = new Point(leftMargin, y),
                Size = new Size(400, 1),
                BackColor = Color.FromArgb(189, 195, 199)
            };
            pnlReceipt.Controls.Add(divider3);
            y += 15;

            // Total
            var lblTotalLabel = new Label
            {
                Text = "Total Paid:",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(leftMargin, y),
                AutoSize = true
            };
            pnlReceipt.Controls.Add(lblTotalLabel);

            var lblTotal = new Label
            {
                Text = $"₹{totalPaid:N0}",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                Location = new Point(150, y - 2),
                AutoSize = true
            };
            pnlReceipt.Controls.Add(lblTotal);
            y += 35;

            // Pending
            decimal pendingAmount = (12 * monthlyDue) - totalPaid;
            if (pendingAmount > 0)
            {
                var lblPendingLabel = new Label
                {
                    Text = "Pending:",
                    Font = new Font("Segoe UI", 10F),
                    Location = new Point(leftMargin, y),
                    AutoSize = true
                };
                pnlReceipt.Controls.Add(lblPendingLabel);

                var lblPending = new Label
                {
                    Text = $"₹{pendingAmount:N0}",
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(231, 76, 60),
                    Location = new Point(150, y),
                    AutoSize = true
                };
                pnlReceipt.Controls.Add(lblPending);
                y += 30;
            }

            // Footer
            y = 450;
            var divider4 = new Panel
            {
                Location = new Point(leftMargin, y),
                Size = new Size(400, 1),
                BackColor = Color.FromArgb(189, 195, 199)
            };
            pnlReceipt.Controls.Add(divider4);
            y += 10;

            var lblFooter = new Label
            {
                Text = "Thank you for your payment!",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(leftMargin, y),
                AutoSize = true
            };
            pnlReceipt.Controls.Add(lblFooter);
        }

        private void BtnPrint_Click(object? sender, EventArgs e)
        {
            try
            {
                printPreviewDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing: {ex.Message}", "Print Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintDocument_PrintPage(object? sender, PrintPageEventArgs e)
        {
            if (e.Graphics == null) return;

            // Create bitmap from panel
            var bmp = new Bitmap(pnlReceipt.Width, pnlReceipt.Height);
            pnlReceipt.DrawToBitmap(bmp, new Rectangle(0, 0, pnlReceipt.Width, pnlReceipt.Height));

            // Center on page
            int x = (e.PageBounds.Width - pnlReceipt.Width) / 2;
            int y = 50;

            e.Graphics.DrawImage(bmp, x, y);
            bmp.Dispose();
        }
    }
}
