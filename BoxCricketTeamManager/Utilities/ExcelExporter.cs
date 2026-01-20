using ClosedXML.Excel;

namespace BoxCricketTeamManager.Utilities
{
    public static class ExcelExporter
    {
        public static void ExportDataGridViewToExcel(DataGridView dgv, string filePath, string sheetTitle)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetTitle.Length > 31 ? sheetTitle[..31] : sheetTitle);

            // Add title
            worksheet.Cell(1, 1).Value = sheetTitle;
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 14;
            worksheet.Range(1, 1, 1, dgv.Columns.Count).Merge();

            // Add timestamp
            worksheet.Cell(2, 1).Value = $"Generated: {DateTime.Now:dd-MMM-yyyy HH:mm}";
            worksheet.Cell(2, 1).Style.Font.Italic = true;
            worksheet.Range(2, 1, 2, dgv.Columns.Count).Merge();

            int startRow = 4;

            // Add headers
            for (int col = 0; col < dgv.Columns.Count; col++)
            {
                if (!dgv.Columns[col].Visible) continue;

                worksheet.Cell(startRow, col + 1).Value = dgv.Columns[col].HeaderText;
                worksheet.Cell(startRow, col + 1).Style.Font.Bold = true;
                worksheet.Cell(startRow, col + 1).Style.Fill.BackgroundColor = XLColor.FromArgb(52, 73, 94);
                worksheet.Cell(startRow, col + 1).Style.Font.FontColor = XLColor.White;
                worksheet.Cell(startRow, col + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // Add data
            for (int row = 0; row < dgv.Rows.Count; row++)
            {
                for (int col = 0; col < dgv.Columns.Count; col++)
                {
                    if (!dgv.Columns[col].Visible) continue;

                    var cellValue = dgv.Rows[row].Cells[col].Value?.ToString() ?? "";
                    var cell = worksheet.Cell(startRow + row + 1, col + 1);

                    // Try to parse as number for proper Excel formatting
                    if (cellValue.StartsWith("₹"))
                    {
                        var numStr = cellValue.Replace("₹", "").Replace(",", "").Trim();
                        if (decimal.TryParse(numStr, out decimal num))
                        {
                            cell.Value = num;
                            cell.Style.NumberFormat.Format = "₹#,##0.00";
                        }
                        else
                        {
                            cell.Value = cellValue;
                        }
                    }
                    else if (cellValue.EndsWith("%"))
                    {
                        var numStr = cellValue.Replace("%", "").Trim();
                        if (decimal.TryParse(numStr, out decimal num))
                        {
                            cell.Value = num / 100;
                            cell.Style.NumberFormat.Format = "0.0%";
                        }
                        else
                        {
                            cell.Value = cellValue;
                        }
                    }
                    else
                    {
                        cell.Value = cellValue;
                    }

                    // Apply cell color if set
                    var dgvCell = dgv.Rows[row].Cells[col];
                    if (dgvCell.Style.BackColor != Color.Empty && dgvCell.Style.BackColor != Color.White)
                    {
                        cell.Style.Fill.BackgroundColor = XLColor.FromColor(dgvCell.Style.BackColor);
                        if (dgvCell.Style.ForeColor != Color.Empty)
                        {
                            cell.Style.Font.FontColor = XLColor.FromColor(dgvCell.Style.ForeColor);
                        }
                    }

                    // Check row default style
                    if (dgv.Rows[row].DefaultCellStyle.BackColor != Color.Empty &&
                        dgv.Rows[row].DefaultCellStyle.BackColor != Color.White)
                    {
                        cell.Style.Fill.BackgroundColor = XLColor.FromColor(dgv.Rows[row].DefaultCellStyle.BackColor);
                        if (dgv.Rows[row].DefaultCellStyle.ForeColor != Color.Empty)
                        {
                            cell.Style.Font.FontColor = XLColor.FromColor(dgv.Rows[row].DefaultCellStyle.ForeColor);
                        }
                    }

                    if (dgv.Rows[row].DefaultCellStyle.Font?.Bold == true ||
                        dgvCell.Style.Font?.Bold == true)
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Add border
            var dataRange = worksheet.Range(startRow, 1, startRow + dgv.Rows.Count, dgv.Columns.Count);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            workbook.SaveAs(filePath);
        }

        public static void ExportPaymentGridToExcel(string filePath, int year)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add($"Payments {year}");

            var memberService = new Services.MemberService();
            var paymentService = new Services.PaymentService();

            var members = memberService.GetAllMembers(true);
            var payments = paymentService.GetPaymentsByYear(year);
            var monthlyDue = paymentService.GetMonthlyDueAmount(year);

            string[] months = { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };

            // Title
            worksheet.Cell(1, 1).Value = $"Box Cricket Team - Payment Record {year}";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 14;
            worksheet.Range(1, 1, 1, 14).Merge();

            // Monthly Due info
            worksheet.Cell(2, 1).Value = $"Monthly Due: ₹{monthlyDue:N0}";
            worksheet.Range(2, 1, 2, 14).Merge();

            int startRow = 4;

            // Headers
            worksheet.Cell(startRow, 1).Value = "S.No";
            worksheet.Cell(startRow, 2).Value = "Member Name";
            for (int i = 0; i < 12; i++)
            {
                worksheet.Cell(startRow, i + 3).Value = months[i];
            }
            worksheet.Cell(startRow, 15).Value = "TOTAL";

            // Style header row
            var headerRange = worksheet.Range(startRow, 1, startRow, 15);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromArgb(52, 73, 94);
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Data
            int row = startRow + 1;
            int serial = 1;
            decimal[] monthlyTotals = new decimal[12];

            foreach (var member in members)
            {
                worksheet.Cell(row, 1).Value = serial++;
                worksheet.Cell(row, 2).Value = member.Name;

                var memberPayments = payments.Where(p => p.MemberId == member.MemberId).ToList();
                decimal memberTotal = 0;

                for (int month = 1; month <= 12; month++)
                {
                    var payment = memberPayments.FirstOrDefault(p => p.PaymentMonth == month);
                    var cell = worksheet.Cell(row, month + 2);

                    if (payment != null)
                    {
                        cell.Value = payment.Amount;
                        cell.Style.NumberFormat.Format = "₹#,##0";
                        cell.Style.Fill.BackgroundColor = XLColor.FromArgb(46, 204, 113);
                        cell.Style.Font.FontColor = XLColor.White;
                        memberTotal += payment.Amount;
                        monthlyTotals[month - 1] += payment.Amount;
                    }
                    else if (!member.IsActive)
                    {
                        cell.Style.Fill.BackgroundColor = XLColor.FromArgb(189, 195, 199);
                    }
                    else
                    {
                        cell.Style.Fill.BackgroundColor = XLColor.FromArgb(231, 76, 60);
                    }
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                worksheet.Cell(row, 15).Value = memberTotal;
                worksheet.Cell(row, 15).Style.NumberFormat.Format = "₹#,##0";
                worksheet.Cell(row, 15).Style.Font.Bold = true;

                if (!member.IsActive)
                {
                    worksheet.Range(row, 1, row, 15).Style.Font.FontColor = XLColor.Gray;
                }

                row++;
            }

            // Monthly totals row
            worksheet.Cell(row, 2).Value = "MONTHLY TOTAL";
            worksheet.Cell(row, 2).Style.Font.Bold = true;
            decimal grandTotal = 0;
            for (int i = 0; i < 12; i++)
            {
                worksheet.Cell(row, i + 3).Value = monthlyTotals[i];
                worksheet.Cell(row, i + 3).Style.NumberFormat.Format = "₹#,##0";
                grandTotal += monthlyTotals[i];
            }
            worksheet.Cell(row, 15).Value = grandTotal;
            worksheet.Cell(row, 15).Style.NumberFormat.Format = "₹#,##0";

            var totalsRange = worksheet.Range(row, 1, row, 15);
            totalsRange.Style.Font.Bold = true;
            totalsRange.Style.Fill.BackgroundColor = XLColor.FromArgb(52, 73, 94);
            totalsRange.Style.Font.FontColor = XLColor.White;

            // Auto-fit and borders
            worksheet.Columns().AdjustToContents();
            worksheet.Column(2).Width = 25;

            var dataRange = worksheet.Range(startRow, 1, row, 15);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            workbook.SaveAs(filePath);
        }
    }
}
