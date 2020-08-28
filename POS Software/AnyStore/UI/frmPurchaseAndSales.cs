using AnyStore.BLL;
using AnyStore.DAL;
using DGVPrinterHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;

namespace AnyStore.UI
{
    public partial class frmPurchaseAndSales : Form
    {
        public frmPurchaseAndSales()
        {
            InitializeComponent();
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
        DeaCustDAL dcDAL = new DeaCustDAL();
        productsDAL pDAL = new productsDAL();
        userDAL uDAL = new userDAL();
        transactionDAL tDAL = new transactionDAL();
        transactionDetailDAL tdDAL = new transactionDetailDAL();

        DataTable transactionDT = new DataTable();

        private void lblSearch_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void lblContact_Click(object sender, EventArgs e)
        {

        }

        private void lblName_Click(object sender, EventArgs e)
        {

        }

        private void lblEmail_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void pnlDeaCust_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void frmPurchaseAndSales_Load(object sender, EventArgs e)
        {
            string type = frmUserDashboard.transactionType;
            lblTop.Text = type;

            transactionDT.Columns.Add("Product Name");
            transactionDT.Columns.Add("Rate");
            transactionDT.Columns.Add("Quantity");
            transactionDT.Columns.Add("Total");
        }

        private void label2_Click(object sender, EventArgs e)
        {
                    }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text;
            if (keyword == "")
            {
                txtName.Text = "";
                txtEmail.Text = "";
                txtContact.Text = "";
                txtAddress.Text = "";
                return;
            }

            DeaCustBLL dc = dcDAL.SearchDealerCustomerForTransaction(keyword);
                txtName.Text = dc.name;
                txtEmail.Text = dc.email;
                txtContact.Text = dc.contact;
                txtAddress.Text = dc.address;
            
        }

        private void txtProductSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtProductSearch.Text;

            if (keyword == "")
            {
                txtProductName.Text = "";
                txtInventory.Text = "";
                txtRate.Text = "";
                txtQty.Text = "";
                return;
            }
            productsBLL p = pDAL.GetProductsForTransaction(keyword);

            txtProductName.Text = p.name;
            txtInventory.Text = p.qty.ToString();
            txtRate.Text = p.rate.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string ProductName = txtProductName.Text;
            decimal Rate = decimal.Parse(txtRate.Text);
            decimal Qty = decimal.Parse(txtQty.Text);

            decimal Total = Rate * Qty;

            decimal SubTotal = decimal.Parse(txtSubTotal.Text);
            SubTotal = SubTotal + Total;

            if (ProductName == "")
            {
                MessageBox.Show("Select the product first. Try Again. ");
            }
            else
            {
                transactionDT.Rows.Add(ProductName, Rate, Qty,Total);

                pnldataGridView.DataSource = transactionDT;
                txtSubTotal.Text = SubTotal.ToString();

                txtProductSearch.Text = "";
                txtProductName.Text = "";
                txtInventory.Text = "0.00";
                txtRate.Text = "0.00";
                txtQty.Text = "0.00";
            }

        }

        private void txtDiscount_TextChanged(object sender, EventArgs e)
        {
            string value = txtDiscount.Text;
            if (value == "")
            {
                MessageBox.Show("Please Add Discount First");
            }
            else
            {
                decimal subTotal = decimal.Parse(txtSubTotal.Text);
                decimal discount = decimal.Parse(txtDiscount.Text);

                decimal GrandTotal = ((100 - discount) / 100) * subTotal;

                txtGrandTotal.Text = GrandTotal.ToString();
            }
        }

        private void txtVat_TextChanged(object sender, EventArgs e)
        {
            string check = txtGrandTotal.Text;
            if (check == "")
            {
                MessageBox.Show("Calculate The Discount and set the Grand Total First");
            }
            else
            {
                decimal previousGT = decimal.Parse(txtGrandTotal.Text);
                decimal vat = decimal.Parse(txtVat.Text);
                decimal grandTotalWithVat = ((100 + vat) / 100) * previousGT;

                txtGrandTotal.Text = grandTotalWithVat.ToString();
            }
        }

        private void txtPaidAmount_TextChanged(object sender, EventArgs e)
        {
            decimal grandTotal = decimal.Parse(txtGrandTotal.Text);
            decimal paidAmount = decimal.Parse(txtPaidAmount.Text);

            decimal returnAmount = paidAmount - grandTotal;
            txtReturnAmount.Text = returnAmount.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            transactionsBLL transaction = new transactionsBLL();
            transaction.type = lblTop.Text;

            string deaCustName = txtName.Text;
            DeaCustBLL dc = dcDAL.GetDeaCustIDFromName(deaCustName);

            transaction.dea_cust_id = dc.id;
            transaction.grandTotal =Math.Round(decimal.Parse(txtGrandTotal.Text),2);
            transaction.transaction_date = DateTime.Now;
            transaction.tax = decimal.Parse(txtVat.Text);
            transaction.discount = decimal.Parse(txtDiscount.Text);

            string username = frmLogin.loggedIn;
            userBLL u = uDAL.GetIdFromUsername(username);

            transaction.added_by = u.id;
            transaction.transactionDetails = transactionDT;

            bool success = false;

            using (TransactionScope scope = new TransactionScope())
            {
                int transactionID = -1;

                bool w = tDAL.Insert_Transaction(transaction, out transactionID);

                for(int i = 0; i <transactionDT.Rows.Count; i++)
                {
                    transactionDetailBLL transactionDetail = new transactionDetailBLL();

                    string ProductName = transactionDT.Rows[i][0].ToString();
                    productsBLL p = pDAL.GetProductIDFromName(ProductName);

                    transactionDetail.product_id = p.id;
                    transactionDetail.rate = decimal.Parse(transactionDT.Rows[i][1].ToString());
                    transactionDetail.qty = decimal.Parse(transactionDT.Rows[i][2].ToString());
                    transactionDetail.total = Math.Round(decimal.Parse(transactionDT.Rows[i][3].ToString()),2);
                    transactionDetail.dea_cust_id = dc.id;
                    transactionDetail.added_date = DateTime.Now;
                    transactionDetail.added_by = u.id;

                    string transactionType = lblTop.Text;
                    bool x=false;
                    if (transactionType == "Purchase")
                    {
                         x = pDAL.IncreaseProduct(transactionDetail.product_id, transactionDetail.qty);
                    }
                    else if (transactionType == "Sales")
                    {
                         x = pDAL.DecreaseProduct(transactionDetail.product_id, transactionDetail.qty);
                    }


                    bool y = tdDAL.InsertTransactionDetail(transactionDetail);
                    success = w && x && y;
                }
                
                if (success == true)
                {
                    scope.Complete();

                    //Code to Print Bill
                    DGVPrinter printer = new DGVPrinter();

                    printer.Title = "\r\n\r\n\r\n TEAM ANTIVIRUS PVT. LTD. \r\n\r\n";
                    printer.SubTitle = "Dhaka, Bangladesh \r\n Phone: 017xxxxxxxx \r\n\r\n";
                    printer.SubTitleFormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
                    printer.PageNumbers = true;
                    printer.PageNumberInHeader = false;
                    printer.PorportionalColumns = true;
                    printer.HeaderCellAlignment = StringAlignment.Near;
                    printer.Footer = "Discount: " + txtDiscount.Text + "% \r\n" + "VAT:" + txtVat.Text + "% \r\n" + "Grand Total:" + txtGrandTotal.Text + "\r\n\r\n" + "Thank You";
                    printer.FooterSpacing = 15;
                    printer.PrintDataGridView(pnldataGridView);

                    MessageBox.Show("Transaction Completed Successfully");

                    pnldataGridView.DataSource = null;
                    pnldataGridView.Rows.Clear();

                    txtSearch.Text = "";
                    txtName.Text = "";
                    txtEmail.Text = "";
                    txtContact.Text = "";
                    txtAddress.Text = "";
                    txtProductSearch.Text = "";
                    txtProductName.Text = "";
                    txtInventory.Text = "0";
                    txtRate.Text = "0";
                    txtVat.Text = "0";
                    txtQty.Text = "0";
                    txtSubTotal.Text = "0";
                    txtGrandTotal.Text = "0";
                    txtDiscount.Text = "0";
                    txtPaidAmount.Text = "0";
                    txtReturnAmount.Text = "0";

                }
                else
                {
                    MessageBox.Show("Transaction Failed");
                }
            }

        }
    }
}
