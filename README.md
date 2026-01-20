# Box Cricket Team Manager

A Windows desktop application for managing box cricket team finances, member tracking, payments, and expense management.

## Features

- **Member Management** - Add, edit, and track team members
- **Payment Tracking** - Record monthly payments with visual grid
- **Expense Management** - Track team expenses by category
- **Reports** - Generate yearly and monthly financial reports
- **Excel Export** - Export data to Excel spreadsheets

## Requirements

- Windows 10/11
- .NET 8.0 Runtime

## Installation

1. Clone the repository
2. Open in Visual Studio 2022 or later
3. Build and run the application

```bash
dotnet build
dotnet run --project BoxCricketTeamManager
```

## User Guide

### 1. Dashboard

The Dashboard provides an overview of your team's finances:

- **Active Members** - Number of currently active members
- **Monthly Collection** - Current month's collected amount vs target
- **Monthly Expenses** - Current month's total expenses
- **Current Balance** - Year-to-date balance (Opening + Collections - Expenses)
- **Pending Payments** - List of members who haven't paid for the current month

Click **Refresh** to update the dashboard data.

---

### 2. Members Tab

Manage your team members here.

#### Adding a New Member
1. Click the **Add Member** button
2. Fill in the member details:
   - **Name** (required)
   - **Phone**
   - **Email**
   - **Join Date**
   - **Notes**
3. Click **Save**

#### Editing a Member
1. Select a member from the list
2. Click **Edit** button
3. Modify the details
4. Click **Save**

#### Deactivating a Member
1. Select a member from the list
2. Click **Deactivate** button
3. Inactive members won't appear in payment tracking

#### Searching Members
- Use the search box to filter members by name, phone, or email
- Check "Show Inactive" to include inactive members in the list

---

### 3. Payments Tab

Track monthly payments for all members using the visual payment grid.

#### Understanding the Grid
- **Rows** = Members
- **Columns** = Months (JAN through DEC)
- **Green cell with ✓** = Payment received
- **Red cell** = Payment pending
- **Gray cell** = Inactive member

#### Recording a Payment
1. Find the member's row
2. Click on the month cell you want to mark as paid
3. The cell turns green with a checkmark
4. Click again to toggle back to unpaid

#### Changing the Year
- Use the **Year** dropdown to view/edit payments for different years

#### Printing a Receipt
1. Select a member row by clicking on it
2. Click **Print Receipt** button
3. A receipt form will open showing the member's payment history

#### Grid Information
- **Monthly Due** - Shows the configured monthly amount
- **Total Collection** - Sum of all payments for the selected year
- **MONTHLY TOTAL row** - Shows collection totals for each month

---

### 4. Expenses Tab

Track team expenses organized by category.

#### Adding an Expense
1. Click **Add Expense** button
2. Fill in:
   - **Description** (required)
   - **Amount** (required)
   - **Category** - Select from dropdown
   - **Month/Year** - When the expense occurred
   - **Notes**
3. Click **Save**

#### Default Categories
- Equipment
- Turf
- Refreshments
- Maintenance
- Miscellaneous

#### Filtering Expenses
- Use the **Year** dropdown to filter by year
- Use the **Category** dropdown to filter by category

#### Editing/Deleting Expenses
1. Select an expense from the list
2. Click **Edit** or **Delete** button

---

### 5. Reports Tab

Generate financial reports for analysis.

#### Available Reports

**Yearly Summary**
- Opening balance
- Total collections
- Total expenses
- Closing balance

**Monthly Breakdown**
- Collections and expenses for each month
- Running balance

**Member Payment Summary**
- Payment status for each member
- Months paid vs pending

**Expense by Category**
- Breakdown of expenses by category
- Percentage distribution

#### Exporting Reports
- Click **Export to Excel** to download the report as an Excel file

---

### 6. Settings Tab

Configure application settings.

#### Monthly Due Amount
1. Enter the monthly due amount
2. Select the year it applies to
3. Click **Save**

#### Opening Balance
1. Enter the opening balance for a year
2. This is typically the closing balance from the previous year
3. Click **Save**

#### Managing Expense Categories
- Add new categories
- Edit existing categories
- Deactivate categories (they won't appear in dropdowns)

#### Data Management
- **Backup Database** - Create a backup of your data
- **Restore Database** - Restore from a previous backup

---

## Data Storage

The application uses SQLite database stored at:
```
%LocalAppData%\BoxCricketTeamManager\BoxCricketTeamManager.db
```

## Tips

1. **Add members first** before trying to record payments
2. **Set the monthly due amount** in Settings for accurate collection targets
3. **Set opening balance** at the start of each year
4. **Regular backups** - Use the backup feature in Settings periodically
5. **Use categories** to organize expenses for better reporting

## Troubleshooting

**Payments grid is empty**
- Make sure you have added members in the Members tab first

**Dashboard shows incorrect balance**
- Check if opening balance is set correctly in Settings
- Verify all payments and expenses are recorded

**Cannot edit inactive member payments**
- Reactivate the member first in the Members tab

## License

MIT License

## Support

For issues and feature requests, please create an issue on GitHub.
