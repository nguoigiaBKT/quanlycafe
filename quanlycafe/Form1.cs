using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using quanlycafe.Entities_db;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace quanlycafe
{
    public partial class Form1 : Form
    {
        private QuanLyCafe db = new QuanLyCafe();
        private Button selectedButton = null;
        private bool isAddingDishes = false;

        public Form1()
        {
            InitializeComponent();
            InitializeButtonEvents();
            LoadDanhmuc();
            LoadDoiban();
            cmbDanhmuc.SelectedIndexChanged += CmbDanhmuc_SelectedIndexChanged;
            numGiamgia.ValueChanged += NumGiamgia_ValueChanged; // Add event handler for numGiamgia
            btnDoiban.Click += btnDoiban_Click; // Add event handler for btnDoiban
        }

        private void InitializeButtonEvents()
        {
            // Path to the LyCafe.png image
            string imagePath = @"D:\DataDownloaded\LapTrinhWindows\Projects\quanlycafe\LyCafe.png";

            foreach (Control control in groupBan.Controls)
            {
                if (control is Button button)
                {
                    int tableId = int.Parse(button.Name.Replace("btn", ""));
                    var table = db.BAN.FirstOrDefault(b => b.maBan == tableId);
                    if (table != null && table.trangThai == true)
                    {
                        // Set the background image for occupied tables
                        if (System.IO.File.Exists(imagePath)) // Ensure the image file exists
                        {
                            button.BackgroundImage = Image.FromFile(imagePath);
                            button.BackgroundImageLayout = ImageLayout.Zoom; // Adjust layout to fit the button
                        }
                        else
                        {
                            MessageBox.Show($"Image file not found at {imagePath}");
                        }
                    }
                    else
                    {
                        // Remove the image for empty tables
                        button.BackgroundImage = null;
                        button.BackColor = Color.White; // Optional: Keep the white background for empty tables
                    }
                    button.Click += Btn_Click;
                }
            }
        }



        private void LoadDanhmuc()
        {
            cmbDanhmuc.DataSource = null; // Clear the data source initially
            cmbMon.DataSource = null; // Clear the data source initially

            var danhmucList = db.DANHMUC.ToList();
            cmbDanhmuc.DataSource = danhmucList;
            cmbDanhmuc.DisplayMember = "tenDanhMuc";
            cmbDanhmuc.ValueMember = "maDanhMuc";
            cmbDanhmuc.SelectedIndex = -1; // Ensure no item is selected by default
        }

        private void LoadDoiban()
        {
            cmbDoiban.DataSource = null; // Clear the data source initially

            var banList = db.BAN.ToList();
            cmbDoiban.DataSource = banList;
            cmbDoiban.DisplayMember = "tenBan";
            cmbDoiban.ValueMember = "maBan";
            cmbDoiban.SelectedIndex = -1; // Ensure no item is selected by default
        }

        private void CmbDanhmuc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDanhmuc.SelectedItem != null)
            {
                var selectedDanhmuc = (DANHMUC)cmbDanhmuc.SelectedItem;
                var monList = db.MON.Where(m => m.maDanhMuc == selectedDanhmuc.maDanhMuc).ToList();
                cmbMon.DataSource = monList;
                cmbMon.DisplayMember = "tenMon";
                cmbMon.ValueMember = "maMon";
                if (monList.Count > 0)
                {
                    cmbMon.SelectedIndex = 0; // Select the first item by default
                }
                else
                {
                    cmbMon.SelectedIndex = -1; // Ensure no item is selected if the list is empty
                }
            }
            else
            {
                cmbMon.DataSource = null; // Clear the data source if no category is selected
            }
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                if (selectedButton != null && selectedButton != clickedButton)
                {
                    // Reset the color of the previously selected button
                    int previousTableId = int.Parse(selectedButton.Name.Replace("btn", ""));
                    var previousTable = db.BAN.FirstOrDefault(b => b.maBan == previousTableId);
                    if (previousTable != null && previousTable.trangThai == true)
                    {
                        // Set the background image for occupied tables
                        string imagePath = @"D:\DataDownloaded\LapTrinhWindows\Projects\quanlycafe\LyCafe.png";
                        if (System.IO.File.Exists(imagePath)) // Ensure the image file exists
                        {
                            selectedButton.BackgroundImage = Image.FromFile(imagePath);
                            selectedButton.BackgroundImageLayout = ImageLayout.Zoom; // Adjust layout to fit the button
                            selectedButton.BackColor = Color.Transparent; // Set background color to transparent
                        }
                        else
                        {
                            MessageBox.Show($"Image file not found at {imagePath}");
                        }
                    }
                    else
                    {
                        selectedButton.BackColor = Color.White; // Empty table
                        selectedButton.BackgroundImage = null;
                    }
                }

                if (clickedButton.BackColor == Color.White)
                {
                    selectedButton = clickedButton;
                    selectedButton.BackColor = Color.Blue; // Selected table
                    isAddingDishes = false;
                    dgvChitiet.Rows.Clear(); // Clear DataGridView for empty table
                    txtTongtien.Text = "0đ"; // Reset total amount
                }
                else if (clickedButton.BackgroundImage != null)
                {
                    selectedButton = clickedButton;
                    selectedButton.BackColor = Color.Blue; // Change table color to blue for adding dishes
                    isAddingDishes = true;
                    LoadTableData(int.Parse(selectedButton.Name.Replace("btn", "")));
                }
            }
        }

        private void btnThemmon_Click(object sender, EventArgs e)
        {
            if (selectedButton == null)
            {
                MessageBox.Show("Vui lòng chọn bàn trước.");
                return;
            }

            if (cmbMon.SelectedItem == null || numSoluong.Value <= 0)
            {
                MessageBox.Show("Please select a dish and enter a valid quantity.");
                return;
            }

            // Get selected dish details
            var selectedDish = (MON)cmbMon.SelectedItem;
            int quantity = (int)numSoluong.Value;
            decimal unitPrice = selectedDish.giaBan ?? 0;
            decimal totalPrice = quantity * unitPrice;

            // Add data to the database (example code, adjust as needed)
            var newCTHD = new CTHD
            {
                maBan = int.Parse(selectedButton.Name.Replace("btn", "")),
                maMon = selectedDish.maMon,
                soLuong = quantity,
                tongTien = totalPrice,
                giamGia = numGiamgia.Value
            };
            db.CTHD.Add(newCTHD);

            // Update the table status to 1 (occupied)
            int tableId = int.Parse(selectedButton.Name.Replace("btn", ""));
            var table = db.BAN.FirstOrDefault(b => b.maBan == tableId);
            if (table != null)
            {
                table.trangThai = true;
            }

            db.SaveChanges();

            // Update DataGridView
            dgvChitiet.Rows.Add(selectedDish.tenMon, quantity, unitPrice, totalPrice);

            // Update total amount in txtTongtien
            UpdateTotalAmount();

            // Change table color to transparent with image
            string imagePath = @"D:\DataDownloaded\LapTrinhWindows\Projects\quanlycafe\LyCafe.png";
            if (System.IO.File.Exists(imagePath)) // Ensure the image file exists
            {
                selectedButton.BackgroundImage = Image.FromFile(imagePath);
                selectedButton.BackgroundImageLayout = ImageLayout.Zoom; // Adjust layout to fit the button
                selectedButton.BackColor = Color.Transparent; // Set background color to transparent
            }
            else
            {
                MessageBox.Show($"Image file not found at {imagePath}");
            }
            isAddingDishes = true;
        }


        private void UpdateTotalAmount()
        {
            decimal totalAmount = 0;
            foreach (DataGridViewRow row in dgvChitiet.Rows)
            {
                if (row.Cells["clThanhtien"].Value != null)
                {
                    totalAmount += Convert.ToDecimal(row.Cells["clThanhtien"].Value);
                }
            }

            // Apply percentage discount
            decimal discountPercent = numGiamgia.Value / 100;
            decimal discountAmount = totalAmount * discountPercent;
            totalAmount -= discountAmount;

            txtTongtien.Text = totalAmount.ToString("C"); // Format as currency

            // Update discount in the database
            if (selectedButton != null)
            {
                int tableId = int.Parse(selectedButton.Name.Replace("btn", ""));
                var tableData = db.CTHD.Where(c => c.maBan == tableId).ToList();
                foreach (var item in tableData)
                {
                    item.giamGia = discountPercent * 100; // Store the percentage value
                }
                db.SaveChanges();
            }
        }

        private void LoadTableData(int tableId)
        {
            dgvChitiet.Rows.Clear();
            var tableData = db.CTHD.Where(c => c.maBan == tableId).ToList();
            foreach (var item in tableData)
            {
                var dish = db.MON.FirstOrDefault(m => m.maMon == item.maMon);
                if (dish != null)
                {
                    dgvChitiet.Rows.Add(dish.tenMon, item.soLuong, dish.giaBan, item.tongTien);
                }
            }
            UpdateTotalAmount();
        }

        private void NumGiamgia_ValueChanged(object sender, EventArgs e)
        {
            UpdateTotalAmount();
        }

        private void btnThanhtoan_Click(object sender, EventArgs e)
        {
            if (selectedButton == null || selectedButton.BackColor != Color.Blue)
            {
                MessageBox.Show("Hãy chọn bàn có khách muốn thanh toán.");
                return;
            }

            // Clear all data in DataGridView
            dgvChitiet.Rows.Clear();

            // Reset total amount to 0đ
            txtTongtien.Text = "0đ";

            // Reset the selected button color to white and remove the image
            selectedButton.BackColor = Color.White;
            selectedButton.BackgroundImage = null;

            // Update the table status to 0 (unoccupied) and reset discount
            int tableId = int.Parse(selectedButton.Name.Replace("btn", ""));
            var table = db.BAN.FirstOrDefault(b => b.maBan == tableId);
            if (table != null)
            {
                table.trangThai = false;
            }

            var tableData = db.CTHD.Where(c => c.maBan == tableId).ToList();
            foreach (var item in tableData)
            {
                item.giamGia = 0;
            }

            db.SaveChanges();

            selectedButton = null;
        }



        private void btnDoiban_Click(object sender, EventArgs e)
        {
            if (dgvChitiet.Rows.Count == 0)
            {
                MessageBox.Show("Hãy chọn bàn có khách muốn đổi.");
                return;
            }

            if (cmbDoiban.SelectedItem == null)
            {
                MessageBox.Show("Hãy chọn bàn muốn đổi.");
                return;
            }

            int sourceTableId = int.Parse(selectedButton.Name.Replace("btn", ""));
            int targetTableId = (int)cmbDoiban.SelectedValue;

            if (sourceTableId == targetTableId)
            {
                MessageBox.Show("Không thể đổi cùng bàn.");
                return;
            }

            // Move data from source table to target table
            var sourceTableData = db.CTHD.Where(c => c.maBan == sourceTableId).ToList();
            foreach (var item in sourceTableData)
            {
                var newItem = new CTHD
                {
                    maBan = targetTableId,
                    maMon = item.maMon,
                    soLuong = item.soLuong,
                    tongTien = item.tongTien,
                    giamGia = item.giamGia
                };
                db.CTHD.Add(newItem);
                db.CTHD.Remove(item);
            }

            // Update table statuses
            var sourceTable = db.BAN.FirstOrDefault(b => b.maBan == sourceTableId);
            if (sourceTable != null)
            {
                sourceTable.trangThai = false;
            }

            var targetTable = db.BAN.FirstOrDefault(b => b.maBan == targetTableId);
            if (targetTable != null)
            {
                targetTable.trangThai = true;
            }

            db.SaveChanges();

            // Update button colors
            selectedButton.BackColor = Color.White;
            selectedButton.BackgroundImage = null;

            foreach (Control control in groupBan.Controls)
            {
                if (control is Button button && int.Parse(button.Name.Replace("btn", "")) == targetTableId)
                {
                    string imagePath = @"D:\DataDownloaded\LapTrinhWindows\Projects\quanlycafe\LyCafe.png";
                    if (System.IO.File.Exists(imagePath)) // Ensure the image file exists
                    {
                        button.BackgroundImage = Image.FromFile(imagePath);
                        button.BackgroundImageLayout = ImageLayout.Zoom; // Adjust layout to fit the button
                        button.BackColor = Color.Transparent; // Set background color to transparent
                    }
                    else
                    {
                        MessageBox.Show($"Image file not found at {imagePath}");
                    }
                    break;
                }
            }

            // Clear DataGridView and reset total amount
            dgvChitiet.Rows.Clear();
            txtTongtien.Text = "0đ";

            selectedButton = null;
        }

    }
}
