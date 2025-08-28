using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using OfficeOpenXml;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;
using QRCoder;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using BCrypt.Net;
using System.Net.Mail;
using System.Xml.Linq;
using static PriceTag.Globals;
using PriceTag.App.Forms;
using PriceTag.App.Interface;
using PriceTag.App.Services;

namespace PriceTag
{
    
    public class MainForm : Form
    {
        private Panel configPanel = null!;
        private Label lblConfig = null!;
        private CustomCheckBox chkDiscount = null!;
        private ComboBox cmbDiscountType = null!;
        private Label lblDiscountPercentage = null!;
        private NumericUpDown nudDiscountPercentage = null!;
        private CustomCheckBox chkBarcode = null!;
        private ComboBox cmbBarcodeType = null!;
        private CustomCheckBox chkProducer = null!;
        private Panel previewPanel = null!;
        private Label lblPreview = null!;
        private PictureBox previewBox = null!;
        private Button btnProcessFile = null!;
        private Button btnGenerateModel = null!;
        private string selectedFilePath = "";
        private PictureBox logo = null!;
        private int collectionId;
        public MainForm(int collId)
        {
            collectionId = collId;
            InitializeComponents();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            CenterForm();
            this.SetTitleBarColor(AppColors.BgColor);
            this.BackColor = AppColors.BgColor;
            this.Icon = new Icon("Properties\\Resources\\icon.ico");
            this.Text = "Price Tag v" + appVersion;
            this.ForeColor = Color.White;
            this.Load += (s, e) =>
            {
                LayoutControls();
                LoadAppSettings();
                if (!IsLicenseValid())
                {
                    CustomMessageBox.Show("Licenta pentru acest cont nu mai este valida!", "Atentie!", MessageBoxButtons.OK);
                    Application.Exit();
                }
            };
            this.Resize += (s, e) => LayoutControls();
        }
        private void InitializeComponents()
        {
            configPanel = new Panel { Size = new Size(300, this.ClientSize.Height), BackColor = AppColors.BgColor };
            this.Controls.Add(configPanel);
            lblConfig = new Label { Text = "Configurare etichete:", Font = new Font("Century Gothic", 14, FontStyle.Bold), ForeColor = AppColors.AccentColor, Size = new Size(280, 30) };
            configPanel.Controls.Add(lblConfig);
            chkDiscount = new CustomCheckBox { Text = "Foloseste pret cu discount", Font = new Font("Century Gothic", 10, FontStyle.Bold), ForeColor = AppColors.AccentColor, Size = new Size(280, 25) };
            chkDiscount.CheckedChanged += (s, e) => { lblDiscountPercentage.Visible = chkDiscount.Checked; nudDiscountPercentage.Visible = chkDiscount.Checked; UpdatePreview(); SaveAppSettings(); };
            configPanel.Controls.Add(chkDiscount);
            cmbDiscountType = new ComboBox { Font = new Font("Century Gothic", 10, FontStyle.Bold), Size = new Size(280, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbDiscountType.Items.Add("Card de fidelitate");
            cmbDiscountType.Items.Add("Reducere promotionala");
            cmbDiscountType.SelectedIndex = 0;
            cmbDiscountType.SelectedIndexChanged += (s, e) => { UpdatePreview(); SaveAppSettings(); };
            configPanel.Controls.Add(cmbDiscountType);
            lblDiscountPercentage = new Label { Text = "Procent discount:", Font = new Font("Century Gothic", 10, FontStyle.Bold), ForeColor = AppColors.AccentColor, Size = new Size(130, 25), Visible = false };
            configPanel.Controls.Add(lblDiscountPercentage);
            nudDiscountPercentage = new NumericUpDown { Font = new Font("Century Gothic", 10, FontStyle.Bold), Size = new Size(100, 25), Minimum = 1, Maximum = 100, Value = 5, Visible = false };
            nudDiscountPercentage.ValueChanged += (s, e) => { UpdatePreview(); SaveAppSettings(); };
            configPanel.Controls.Add(nudDiscountPercentage);
            chkBarcode = new CustomCheckBox { Text = "Foloseste cod bare", Font = new Font("Century Gothic", 10, FontStyle.Bold), ForeColor = AppColors.AccentColor, Size = new Size(280, 25), Checked = true };
            chkBarcode.CheckedChanged += (s, e) => { UpdatePreview(); SaveAppSettings(); };
            configPanel.Controls.Add(chkBarcode);
            cmbBarcodeType = new ComboBox { Font = new Font("Century Gothic", 10, FontStyle.Bold), Size = new Size(280, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbBarcodeType.Items.Add("Cod bare QR");
            cmbBarcodeType.Items.Add("Cod bare clasic");
            cmbBarcodeType.SelectedIndex = 0;
            cmbBarcodeType.SelectedIndexChanged += (s, e) => { UpdatePreview(); SaveAppSettings(); };
            configPanel.Controls.Add(cmbBarcodeType);
            chkProducer = new CustomCheckBox { Text = "Foloseste nume producator", Checked = true, Font = new Font("Century Gothic", 10, FontStyle.Bold), ForeColor = AppColors.AccentColor, Size = new Size(280, 25) };
            chkProducer.CheckedChanged += (s, e) => { UpdatePreview(); SaveAppSettings(); };
            configPanel.Controls.Add(chkProducer);
            previewPanel = new Panel { Size = new Size(300, this.ClientSize.Height), BackColor = AppColors.BgColor };
            this.Controls.Add(previewPanel);
            lblPreview = new Label { Text = "Previzualizare:", Font = new Font("Century Gothic", 14, FontStyle.Bold), ForeColor = AppColors.AccentColor, Size = new Size(280, 30) };
            previewPanel.Controls.Add(lblPreview);
            previewBox = new PictureBox { Size = new Size(230, 115), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White };
            previewPanel.Controls.Add(previewBox);
            btnGenerateModel = new Button { Text = "Model fisier", Font = new Font("Century Gothic", 10, FontStyle.Bold), Size = new Size(150, 30), BackColor = AppColors.BgColor, ForeColor = AppColors.AccentColor, FlatStyle = FlatStyle.Flat };
            btnGenerateModel.FlatAppearance.BorderColor = AppColors.AccentColor;
            btnGenerateModel.FlatAppearance.BorderSize = 1;
            btnGenerateModel.Click += (s, e) => GenerateExcelTemplate();
            btnGenerateModel.MouseEnter += (s, e) => { btnGenerateModel.BackColor = AppColors.AccentColor; btnGenerateModel.ForeColor = AppColors.BgColor; };
            btnGenerateModel.MouseLeave += (s, e) => { btnGenerateModel.BackColor = AppColors.BgColor; btnGenerateModel.ForeColor = AppColors.AccentColor; };
            previewPanel.Controls.Add(btnGenerateModel);
            btnProcessFile = new Button { Text = "Prelucrare fisier", Font = new Font("Century Gothic", 10, FontStyle.Bold), Size = new Size(150, 30), BackColor = AppColors.BgColor, ForeColor = AppColors.AccentColor, FlatStyle = FlatStyle.Flat };
            btnProcessFile.FlatAppearance.BorderColor = AppColors.AccentColor;
            btnProcessFile.FlatAppearance.BorderSize = 1;
            btnProcessFile.Click += async (s, e) => await btnProcessFile_Click(s, e);
            btnProcessFile.MouseEnter += (s, e) => { btnProcessFile.BackColor = AppColors.AccentColor; btnProcessFile.ForeColor = AppColors.BgColor; };
            btnProcessFile.MouseLeave += (s, e) => { btnProcessFile.BackColor = AppColors.BgColor; btnProcessFile.ForeColor = AppColors.AccentColor; };
            this.Controls.Add(btnProcessFile);
            btnProcessFile.BringToFront();
            logo = new PictureBox { Image = Image.FromFile("Properties\\Resources\\logo.png"), SizeMode = PictureBoxSizeMode.Zoom, Size = new Size(250, 60), BackColor = AppColors.BgColor };
            this.Controls.Add(logo);
            logo.BringToFront();
        }
        private void LayoutControls()
        {
            int gap = 80;
            int totalWidth = configPanel.Width + previewPanel.Width + gap;
            int startX = (this.ClientSize.Width - totalWidth) / 2;
            configPanel.Location = new Point(startX, 0);
            previewPanel.Location = new Point(configPanel.Right + gap, 0);
            configPanel.Height = this.ClientSize.Height;
            previewPanel.Height = this.ClientSize.Height;
            int groupHeight = 240;
            int newBase = (configPanel.Height - groupHeight) / 2;
            lblConfig.Location = new Point(10, newBase);
            chkDiscount.Location = new Point(10, newBase + 40);
            cmbDiscountType.Location = new Point(10, newBase + 70);
            lblDiscountPercentage.Location = new Point(10, newBase + 105);
            nudDiscountPercentage.Location = new Point(150, newBase + 105);
            chkBarcode.Location = new Point(10, newBase + 140);
            cmbBarcodeType.Location = new Point(10, newBase + 170);
            chkProducer.Location = new Point(10, newBase + 200);
            int previewBoxX = (previewPanel.Width - previewBox.Width) / 2;
            lblPreview.Location = new Point(previewBoxX, newBase);
            previewBox.Location = new Point(previewBoxX, newBase + 50);
            btnGenerateModel.Location = new Point((previewPanel.Width - btnGenerateModel.Width) / 2, previewBox.Bottom + 20);
            btnProcessFile.Location = new Point((this.ClientSize.Width - btnProcessFile.Width) / 2, this.ClientSize.Height - btnProcessFile.Height - 40);
            logo.Location = new Point((this.ClientSize.Width - logo.Width) / 2, 0);
        }
        private string GetDiscountText() =>
            (cmbDiscountType.SelectedItem?.ToString() ?? "") == "Card de fidelitate" ? "Pret cu card de fidelitate:" : "Pret promotional:";
        private void DrawPriceBlockPreview(Graphics g, double originalPrice, double discountedPrice, string barcodeData, int width, int height, string producer)
        {
            bool noChecks = (!chkDiscount.Checked && !chkBarcode.Checked);
            Font priceFont = new("Century Gothic", 10, FontStyle.Bold);
            Font biggerPriceFont = new("Century Gothic", 12, FontStyle.Bold);
            Font smallFont = new("Century Gothic", 7, FontStyle.Bold);
            int marginRight = 10;
            if (noChecks)
            {
                g.DrawString("Pret:", priceFont, Brushes.Black, new PointF(5, 65));
                g.DrawString($"{originalPrice:F2} Lei", biggerPriceFont, Brushes.Black, new PointF(5, 80));
            }
            else if (cmbBarcodeType.SelectedItem?.ToString() == "Cod bare clasic")
            {
                if (chkDiscount.Checked)
                {
                    string discountText = GetDiscountText();
                    g.DrawString($"Pret: {originalPrice:F2} Lei", priceFont, Brushes.Black, new PointF(5, 50));
                    g.DrawString(discountText, priceFont, Brushes.Black, new PointF(5, 65));
                    g.DrawString($"{discountedPrice:F2} Lei", biggerPriceFont, Brushes.Black, new PointF(5, 80));
                }
                else
                {
                    g.DrawString("Pret:", priceFont, Brushes.Black, new PointF(5, 65));
                    g.DrawString($"{originalPrice:F2} Lei", biggerPriceFont, Brushes.Black, new PointF(5, 80));
                }
                if (chkBarcode.Checked && !string.IsNullOrEmpty(barcodeData))
                {
                    int barcodeWidth = (int)(width * 0.9 / 2) - 30;
                    int barcodeHeight = 10;
                    int barcodeX = width - barcodeWidth - marginRight;
                    using (Bitmap classicBarcode = BarcodeGenerator.GenerateCode39Barcode(barcodeData, 25))
                    {
                        g.DrawImage(classicBarcode, barcodeX, 85, barcodeWidth, barcodeHeight);
                    }
                    SizeF textSize = g.MeasureString(barcodeData, smallFont);
                    float textX = barcodeX + (barcodeWidth - textSize.Width) / 2;
                    float textY = 85 + barcodeHeight;
                    g.DrawString(barcodeData, smallFont, Brushes.Black, new PointF(textX, textY));
                }
            }
            else
            {
                if (chkDiscount.Checked)
                {
                    string discountText = GetDiscountText();
                    g.DrawString($"Pret: {originalPrice:F2} Lei", priceFont, Brushes.Black, new PointF(5, 50));
                    g.DrawString(discountText, priceFont, Brushes.Black, new PointF(5, 65));
                    g.DrawString($"{discountedPrice:F2} Lei", biggerPriceFont, Brushes.Black, new PointF(5, 80));
                }
                else
                {
                    g.DrawString("Pret:", priceFont, Brushes.Black, new PointF(5, 65));
                    g.DrawString($"{originalPrice:F2} Lei", biggerPriceFont, Brushes.Black, new PointF(5, 80));
                }
                if (chkBarcode.Checked && !string.IsNullOrEmpty(barcodeData))
                    g.DrawImage(GenerateQRCodeHelper.GenerateQRCodeBitmap(barcodeData), width - 60, height - 60, 50, 50);
            }
            if (chkProducer.Checked && !string.IsNullOrEmpty(producer))
            {
                Font producerFont = new("Century Gothic", 6, FontStyle.Bold);
                float availableWidth;
                if (chkBarcode.Checked)
                {
                    if (cmbBarcodeType.SelectedItem?.ToString() == "Cod bare clasic")
                    {
                        int barcodeWidth = (int)(width * 0.9 / 2) - 30;
                        int barcodeX = width - barcodeWidth - marginRight;
                        availableWidth = barcodeX - 8;
                    }
                    else
                        availableWidth = width - 60 - 8;
                }
                else
                    availableWidth = width - 16;
                string truncatedProducer = TextHelper.TruncateText(g, producer, producerFont, availableWidth);
                g.DrawString(truncatedProducer, producerFont, Brushes.Black, new PointF(8, height - 18));
            }
        }
        private void DrawPriceBlockPdf(XGraphics gfx, double originalPrice, double discountedPrice, string barcodeData, int x, int y, int labelWidth, int labelHeight, bool discountChecked, bool barcodeChecked, string barcodeType, string discountText, bool currentIncludeBarcode, bool producerChecked, bool currentIncludeProducer, string producer)
        {
            int marginRight = 10;
            bool noChecks = (!discountChecked && !barcodeChecked);
            XFont originalPriceFont = new("Century Gothic", 12, PdfSharpCore.Drawing.XFontStyle.Bold);
            XFont biggerPriceFont = new("Century Gothic", 18, PdfSharpCore.Drawing.XFontStyle.Bold);
            XFont xSmallFont = new("Century Gothic", 7, PdfSharpCore.Drawing.XFontStyle.Bold);
            if (noChecks)
            {
                gfx.DrawString("Pret:", originalPriceFont, XBrushes.Black, new XRect(x + 10, y + 65, labelWidth - 20, 20), XStringFormats.TopLeft);
                gfx.DrawString($"{originalPrice:F2} Lei", biggerPriceFont, XBrushes.Black, new XRect(x + 10, y + 80, labelWidth - 20, 20), XStringFormats.TopLeft);
            }
            else if (barcodeType == "Cod bare clasic")
            {
                if (discountChecked)
                {
                    gfx.DrawString($"Pret: {originalPrice:F2} Lei", originalPriceFont, XBrushes.Black, new XRect(x + 10, y + 50, labelWidth - 20, 20), XStringFormats.TopLeft);
                    gfx.DrawString(discountText, originalPriceFont, XBrushes.Black, new XRect(x + 10, y + 65, labelWidth - 20, 20), XStringFormats.TopLeft);
                    gfx.DrawString($"{discountedPrice:F2} Lei", biggerPriceFont, XBrushes.Black, new XRect(x + 10, y + 80, labelWidth - 20, 20), XStringFormats.TopLeft);
                }
                else
                {
                    gfx.DrawString("Pret:", originalPriceFont, XBrushes.Black, new XRect(x + 10, y + 65, labelWidth - 20, 20), XStringFormats.TopLeft);
                    gfx.DrawString($"{originalPrice:F2} Lei", biggerPriceFont, XBrushes.Black, new XRect(x + 10, y + 80, labelWidth - 20, 20), XStringFormats.TopLeft);
                }
                if (barcodeChecked && currentIncludeBarcode && !string.IsNullOrEmpty(barcodeData))
                {
                    int barcodeWidth = (int)(labelWidth * 0.9 / 2) - 30;
                    int barcodeHeight = 10;
                    int barcodeX = x + labelWidth - barcodeWidth - marginRight;
                    XImage barcodeImage = BarcodeGenerator.GenerateCode39XImage(barcodeData);
                    gfx.DrawImage(barcodeImage, barcodeX, y + 85, barcodeWidth, barcodeHeight);
                    XSize textSize = gfx.MeasureString(barcodeData, xSmallFont);
                    double textX = barcodeX + (barcodeWidth - textSize.Width) / 2;
                    double textY = y + 85 + barcodeHeight;
                    gfx.DrawString(barcodeData, xSmallFont, XBrushes.Black, new XRect(textX, textY, textSize.Width, textSize.Height), XStringFormats.TopLeft);
                }
            }
            else
            {
                if (discountChecked)
                {
                    gfx.DrawString($"Pret: {originalPrice:F2} Lei", originalPriceFont, XBrushes.Black, new XRect(x + 10, y + 50, labelWidth - 20, 20), XStringFormats.TopLeft);
                    gfx.DrawString(discountText, originalPriceFont, XBrushes.Black, new XRect(x + 10, y + 65, labelWidth - 20, 20), XStringFormats.TopLeft);
                    gfx.DrawString($"{discountedPrice:F2} Lei", biggerPriceFont, XBrushes.Black, new XRect(x + 10, y + 80, labelWidth - 20, 20), XStringFormats.TopLeft);
                }
                else
                {
                    gfx.DrawString("Pret:", originalPriceFont, XBrushes.Black, new XRect(x + 10, y + 65, labelWidth - 20, 20), XStringFormats.TopLeft);
                    gfx.DrawString($"{originalPrice:F2} Lei", biggerPriceFont, XBrushes.Black, new XRect(x + 10, y + 80, labelWidth - 20, 20), XStringFormats.TopLeft);
                }
                if (barcodeChecked && currentIncludeBarcode && !string.IsNullOrEmpty(barcodeData))
                {
                    XImage qrImage = GenerateQRCodeHelper.GenerateQRCode(barcodeData);
                    gfx.DrawImage(qrImage, x + labelWidth - 70, y + 45, 70, 70);
                }
            }
            if (producerChecked && currentIncludeProducer && !string.IsNullOrEmpty(producer))
            {
                XFont producerFont = new("Century Gothic", 7, PdfSharpCore.Drawing.XFontStyle.Bold);
                double availableWidth;
                if (barcodeChecked)
                {
                    if (barcodeType == "Cod bare clasic")
                    {
                        int marginR = 10;
                        int barcodeW = (int)(labelWidth * 0.9 / 2) - 30;
                        double barcodeX = x + labelWidth - barcodeW - marginR;
                        availableWidth = barcodeX - (x + 10);
                    }
                    else
                        availableWidth = (x + labelWidth - 60) - (x + 10);
                }
                else
                    availableWidth = labelWidth - 20;
                string truncatedProducer = TextHelper.TruncateText(gfx, producer, producerFont, availableWidth);
                gfx.DrawString(truncatedProducer, producerFont, XBrushes.Black, new XRect(x + 10, y + labelHeight - 12, availableWidth, 10), XStringFormats.TopLeft);
            }
        }
        private void UpdatePreview()
        {
            int width = 230, height = 115;
            Bitmap bmp = new(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                using (Pen borderPen = new(Color.Black, 2))
                    g.DrawRectangle(borderPen, 0, 0, width - 1, height - 1);
                g.DrawString("Produs Exemplu", new Font("Century Gothic", 10, FontStyle.Bold), Brushes.Black, new PointF(5, 10));
                double originalPrice = 100.00;
                double discountPerc = chkDiscount.Checked ? (double)nudDiscountPercentage.Value / 100.0 : 0;
                double discountedPrice = originalPrice - (originalPrice * discountPerc);
                string barcodeData = "1234567890123";
                string producer = chkProducer.Checked ? "Nume producator exemplu" : "";
                DrawPriceBlockPreview(g, originalPrice, discountedPrice, barcodeData, width, height, producer);
            }
            previewBox.Image = bmp;
        }
        private void GenerateExcelTemplate()
        {
            SaveFileDialog saveFileDialog = new() { Filter = "Excel Files|*.xlsx", FileName = "Model.xlsx" };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Model");
                    worksheet.Cells[1, 1].Value = "Nume produs";
                    worksheet.Cells[1, 2].Value = "Pret produs";
                    worksheet.Cells[1, 3].Value = "Cod bare";
                    worksheet.Cells[1, 4].Value = "Nume producator";
                    worksheet.Cells["A1:D1000"].Style.Numberformat.Format = "@";
                    package.SaveAs(new FileInfo(saveFileDialog.FileName));
                }
                Process.Start(new ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true });
            }
        }
        private async Task btnProcessFile_Click(object? sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new() { Filter = "Excel Files|*.xlsx;*.xls" };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string ext = Path.GetExtension(openFileDialog.FileName).ToLower();
                if (ext != ".xlsx" && ext != ".xls")
                {
                    CustomMessageBox.Show("Formatul fisierului nu este suportat!", "Atentie!", MessageBoxButtons.OK);
                    return;
                }
                btnProcessFile.Enabled = false;
                Label overlayLabel = new()
                {
                    Text = "Fisierul este in curs de prelucrare...",
                    BackColor = this.BackColor,
                    ForeColor = btnProcessFile.ForeColor,
                    Font = btnProcessFile.Font,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                using (Graphics g = this.CreateGraphics())
                {
                    SizeF textSize = g.MeasureString(overlayLabel.Text, overlayLabel.Font);
                    int overlayWidth = (int)textSize.Width + 20;
                    int overlayHeight = btnProcessFile.Height;
                    overlayLabel.Size = new Size(overlayWidth, overlayHeight);
                }
                overlayLabel.Location = new Point(btnProcessFile.Left + (btnProcessFile.Width - overlayLabel.Width) / 2, btnProcessFile.Top);
                this.Controls.Add(overlayLabel);
                overlayLabel.BringToFront();
                selectedFilePath = openFileDialog.FileName;
                bool currentIncludeBarcode = true;
                bool currentIncludeProducer = true;
                var productList = LoadDataFromExcel(selectedFilePath, out currentIncludeBarcode, out currentIncludeProducer);
                if (productList == null)
                {
                    this.Controls.Remove(overlayLabel);
                    btnProcessFile.Enabled = true;
                    return;
                }
                bool discountChecked = chkDiscount.Checked;
                double discountPerc = discountChecked ? (double)nudDiscountPercentage.Value / 100.0 : 0;
                bool barcodeChecked = chkBarcode.Checked;
                bool producerChecked = chkProducer.Checked;
                string barcodeType = cmbBarcodeType.SelectedItem?.ToString() ?? "";
                string discountText = GetDiscountText();
                string pdfPath = await Task.Run(() =>
                {
                    return GenerateLabelsPdf(productList, discountChecked, discountPerc, barcodeChecked, barcodeType, discountText, currentIncludeBarcode, producerChecked, currentIncludeProducer);
                });
                if (File.Exists(pdfPath))
                    Process.Start(new ProcessStartInfo(pdfPath) { UseShellExecute = true });
                this.Controls.Remove(overlayLabel);
                btnProcessFile.Enabled = true;
            }
        }
        private List<Product>? LoadDataFromExcel(string filePath, out bool currentIncludeBarcode, out bool currentIncludeProducer)
        {
            List<Product> products = new();
            currentIncludeBarcode = true;
            currentIncludeProducer = true;
            bool missingName = false;
            bool missingPrice = false;
            bool missingBarcode = false;
            bool missingProducer = false;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int colCount = worksheet.Dimension.Columns;
                int rowCount = worksheet.Dimension.Rows;
                int nameCol = 0, priceCol = 0, barcodeCol = 0, producerCol = 0;
                List<string> nameFilters = new() { "produs", "nume produs" };
                List<string> priceFilters = new() { "pret", "pret vanzare", "pret amanunt" };
                List<string> barcodeFilters = new() { "cod bare", "cod de bare" };
                List<string> producerFilters = new() { "producator", "nume producator" };
                for (int col = 1; col <= colCount; col++)
                {
                    string header = (worksheet.Cells[1, col].Text ?? "").ToLower();
                    if (nameCol == 0)
                        foreach (string filter in nameFilters)
                            if (header.Contains(filter)) { nameCol = col; break; }
                    if (priceCol == 0)
                        foreach (string filter in priceFilters)
                            if (header.Contains(filter)) { priceCol = col; break; }
                    if (chkBarcode.Checked && currentIncludeBarcode && barcodeCol == 0)
                        foreach (string filter in barcodeFilters)
                            if (header.Contains(filter)) { barcodeCol = col; break; }
                    if (producerCol == 0)
                        foreach (string filter in producerFilters)
                            if (header.Contains(filter)) { producerCol = col; break; }
                }
                if (nameCol == 0 && priceCol == 0)
                {
                    CustomMessageBox.Show("Niciun parametru nu a fost gasit!\nParametrii obligatorii: 'Nume produs' si 'Pret produs'.", "Atentie!", MessageBoxButtons.OK);
                    return null;
                }
                if (nameCol == 0)
                {
                    CustomMessageBox.Show("Parametrul 'Nume produs' nu a fost gasit!", "Atentie!", MessageBoxButtons.OK);
                    return null;
                }
                if (priceCol == 0)
                {
                    CustomMessageBox.Show("Parametrul 'Pret produs' nu a fost gasit!", "Atentie!", MessageBoxButtons.OK);
                    return null;
                }
                if (chkBarcode.Checked && currentIncludeBarcode && barcodeCol == 0)
                {
                    DialogResult res = CustomMessageBox.Show("Parametrul 'Cod bare' nu a fost gasit!\r\nDoriti sa continuati fara?", "Atentie!", MessageBoxButtons.YesNo);
                    if (res != DialogResult.Yes)
                        return null;
                    currentIncludeBarcode = false;
                }
                if (chkProducer.Checked)
                {
                    if (producerCol == 0)
                    {
                        DialogResult res = CustomMessageBox.Show("Parametrul 'Nume producator' nu a fost gasit!\r\nDoriti sa continuati fara?", "Atentie!", MessageBoxButtons.YesNo);
                        if (res != DialogResult.Yes)
                            return null;
                        currentIncludeProducer = false;
                    }
                }
                for (int row = 2; row <= rowCount; row++)
                {
                    string name = worksheet.Cells[row, nameCol].Text;
                    string price = worksheet.Cells[row, priceCol].Text;
                    string barcode = (chkBarcode.Checked && currentIncludeBarcode) ? worksheet.Cells[row, barcodeCol].Text : "";
                    string producer = (chkProducer.Checked && currentIncludeProducer) ? worksheet.Cells[row, producerCol].Text : "";
                    if (string.IsNullOrWhiteSpace(name))
                        missingName = true;
                    if (string.IsNullOrWhiteSpace(price))
                        missingPrice = true;
                    if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(price))
                    {
                        if (chkBarcode.Checked && currentIncludeBarcode && string.IsNullOrWhiteSpace(barcode))
                            missingBarcode = true;
                        if (chkProducer.Checked && currentIncludeProducer && string.IsNullOrWhiteSpace(producer))
                            missingProducer = true;
                        products.Add(new Product { Name = name, Price = price, Barcode = barcode, Producer = producer });
                    }
                }
            }
            if (missingName && missingPrice)
                CustomMessageBox.Show("Anumite produse nu au parametrii:\n'Nume produs' si 'Pret produs'!", "Atentie!", MessageBoxButtons.OK);
            else if (missingName)
                CustomMessageBox.Show("Anumite produse nu au parametrul 'Nume produs'!", "Atentie!", MessageBoxButtons.OK);
            else if (missingPrice)
                CustomMessageBox.Show("Anumite produse nu au parametrul 'Pret produs'!", "Atentie!", MessageBoxButtons.OK);
            if (chkBarcode.Checked && currentIncludeBarcode && missingBarcode)
            {
                DialogResult res = CustomMessageBox.Show("Anumite produse nu au parametrul 'Cod bare'.\r\nDoriti sa continuati fara?", "Atentie!", MessageBoxButtons.YesNo);
                if (res != DialogResult.Yes)
                    return null;
            }
            if (chkProducer.Checked && currentIncludeProducer && missingProducer)
            {
                DialogResult res = CustomMessageBox.Show("Anumite produse nu au parametrul 'Nume producator'!\r\nDoriti sa continuati fara?", "Atentie!", MessageBoxButtons.YesNo);
                if (res != DialogResult.Yes)
                    return null;
            }
            return products;
        }
        private void CenterForm()
        {
            if (Screen.PrimaryScreen == null)
            {
                return;
            }
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            this.Width = screen.Width / 2 + 10;
            this.Height = screen.Height / 2 + 10;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(screen.Width / 2 - this.Width / 2, screen.Height / 2 - this.Height / 2);
        }
        private void LoadAppSettings()
        {
            using (SqlConnection conn = new(Program.ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new("SELECT UseDiscount, DiscountType, UseBarcode, BarcodeType, UseProducerName, DiscountValue FROM AppSettings WHERE CollectionId = @CollectionId", conn);
                cmd.Parameters.AddWithValue("@CollectionId", collectionId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        chkDiscount.Checked = reader.GetBoolean(0);
                        string discType = reader.GetString(1);
                        if (discType == "Card de fidelitate")
                            cmbDiscountType.SelectedItem = "Card de fidelitate";
                        else if (discType == "Reducere promotionala")
                            cmbDiscountType.SelectedItem = "Reducere promotionala";
                        else
                            cmbDiscountType.SelectedIndex = 0;
                        nudDiscountPercentage.Value = reader.GetInt32(5);
                        chkBarcode.Checked = reader.GetBoolean(2);
                        string barType = reader.GetString(3);
                        if (barType == "Cod bare QR")
                            cmbBarcodeType.SelectedItem = "Cod bare QR";
                        else if (barType == "Cod bare clasic")
                            cmbBarcodeType.SelectedItem = "Cod bare clasic";
                        else
                            cmbBarcodeType.SelectedIndex = 0;
                        chkProducer.Checked = reader.GetBoolean(4);
                    }
                    else
                    {
                        reader.Close();
                        SqlCommand insertCmd = new("INSERT INTO AppSettings (CollectionId, UseDiscount, DiscountType, DiscountValue, UseBarcode, BarcodeType, UseProducerName) VALUES (@CollectionId, @UseDiscount, @DiscountType, @DiscountValue, @UseBarcode, @BarcodeType, @UseProducerName)", conn);
                        insertCmd.Parameters.AddWithValue("@CollectionId", collectionId);
                        insertCmd.Parameters.AddWithValue("@UseDiscount", 0);
                        insertCmd.Parameters.AddWithValue("@DiscountType", "none");
                        insertCmd.Parameters.AddWithValue("@DiscountValue", 5);
                        insertCmd.Parameters.AddWithValue("@UseBarcode", 1);
                        insertCmd.Parameters.AddWithValue("@BarcodeType", "Cod bare QR");
                        insertCmd.Parameters.AddWithValue("@UseProducerName", 1);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
            UpdatePreview();
        }
        private void SaveAppSettings()
        {
            using (SqlConnection conn = new(Program.ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new("UPDATE AppSettings SET UseDiscount = @UseDiscount, DiscountType = @DiscountType, DiscountValue = @DiscountValue, UseBarcode = @UseBarcode, BarcodeType = @BarcodeType, UseProducerName = @UseProducerName WHERE CollectionId = @CollectionId", conn);
                cmd.Parameters.AddWithValue("@UseDiscount", chkDiscount.Checked);
                cmd.Parameters.AddWithValue("@DiscountType", chkDiscount.Checked ? cmbDiscountType.SelectedItem?.ToString() ?? "none" : "none");
                cmd.Parameters.AddWithValue("@DiscountValue", (int)nudDiscountPercentage.Value);
                cmd.Parameters.AddWithValue("@UseBarcode", chkBarcode.Checked);
                cmd.Parameters.AddWithValue("@BarcodeType", chkBarcode.Checked ? cmbBarcodeType.SelectedItem?.ToString() ?? "none" : "none");
                cmd.Parameters.AddWithValue("@UseProducerName", chkProducer.Checked);
                cmd.Parameters.AddWithValue("@CollectionId", collectionId);
                cmd.ExecuteNonQuery();
            }
        }
        private bool IsLicenseValid()
        {
            using (SqlConnection conn = new(Program.ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new("select count(*) from Licenses where CollectionId=@CollectionId and IsValid=1 and EndDate>GETDATE()", conn);
                cmd.Parameters.AddWithValue("@CollectionId", collectionId);
                object? result = cmd.ExecuteScalar();
                int count = result != null ? Convert.ToInt32(result) : 0;
                return count > 0;
            }
        }
        private string GenerateLabelsPdf(List<Product> products, bool discountChecked, double discountPerc, bool barcodeChecked, string barcodeType, string discountText, bool currentIncludeBarcode, bool producerChecked, bool currentIncludeProducer)
        {
            PdfDocument pdf = new();
            PdfPage page = pdf.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont originalPriceFont = new("Century Gothic", 12, PdfSharpCore.Drawing.XFontStyle.Bold);
            XFont biggerPriceFont = new("Century Gothic", 18, PdfSharpCore.Drawing.XFontStyle.Bold);
            XFont xSmallFont = new("Century Gothic", 7, PdfSharpCore.Drawing.XFontStyle.Bold);
            int pageWidth = (int)page.Width;
            int pageHeight = (int)page.Height;
            int labelWidth = 230, labelHeight = 115;
            int padding = 20;
            int labelsPerPage = 12;
            int totalHeight = (6 * labelHeight) + (5 * padding);
            int yStart = (pageHeight - totalHeight) / 2;
            int col1X = (pageWidth / 4) - (labelWidth / 2);
            int col2X = (pageWidth * 3 / 4) - (labelWidth / 2);
            int x = col1X, y = yStart;
            int labelCount = 0;
            int marginRight = 10;
            foreach (var product in products)
            {
                if (labelCount > 0 && labelCount % labelsPerPage == 0)
                {
                    page = pdf.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    x = col1X;
                    y = yStart;
                }
                gfx.DrawRectangle(XPens.Black, XBrushes.White, x, y, labelWidth, labelHeight);
                XTextFormatter tf = new(gfx);
                tf.DrawString(product.Name, new XFont("Century Gothic", 10, PdfSharpCore.Drawing.XFontStyle.Bold), XBrushes.Black, new XRect(x + 10, y + 10, labelWidth - 20, 40));
                double originalPrice = double.TryParse(product.Price, out double p) ? p : 0;
                double finalDiscount = discountChecked ? discountPerc : 0;
                double discountedPrice = originalPrice - (originalPrice * finalDiscount);
                string barcodeData = product.Barcode;
                if (!discountChecked && !barcodeChecked)
                {
                    gfx.DrawString("Pret:", originalPriceFont, XBrushes.Black, new XRect(x + 10, y + 65, labelWidth - 20, 20), XStringFormats.TopLeft);
                    gfx.DrawString($"{originalPrice:F2} Lei", biggerPriceFont, XBrushes.Black, new XRect(x + 10, y + 80, labelWidth - 20, 20), XStringFormats.TopLeft);
                }
                else if (barcodeType == "Cod bare clasic")
                {
                    if (discountChecked)
                    {
                        gfx.DrawString($"Pret: {originalPrice:F2} Lei", originalPriceFont, XBrushes.Black, new XRect(x + 10, y + 50, labelWidth - 20, 20), XStringFormats.TopLeft);
                        gfx.DrawString(discountText, originalPriceFont, XBrushes.Black, new XRect(x + 10, y + 65, labelWidth - 20, 20), XStringFormats.TopLeft);
                        gfx.DrawString($"{discountedPrice:F2} Lei", biggerPriceFont, XBrushes.Black, new XRect(x + 10, y + 80, labelWidth - 20, 20), XStringFormats.TopLeft);
                    }
                    else
                    {
                        gfx.DrawString("Pret:", originalPriceFont, XBrushes.Black, new XRect(x + 10, y + 65, labelWidth - 20, 20), XStringFormats.TopLeft);
                        gfx.DrawString($"{originalPrice:F2} Lei", biggerPriceFont, XBrushes.Black, new XRect(x + 10, y + 80, labelWidth - 20, 20), XStringFormats.TopLeft);
                    }
                    if (barcodeChecked && currentIncludeBarcode && !string.IsNullOrEmpty(barcodeData))
                    {
                        int barcodeWidth = (int)(labelWidth * 0.9 / 2) - 30;
                        int barcodeHeight = 10;
                        int barcodeX = x + labelWidth - barcodeWidth - marginRight;
                        XImage barcodeImage = BarcodeGenerator.GenerateCode39XImage(barcodeData);
                        gfx.DrawImage(barcodeImage, barcodeX, y + 85, barcodeWidth, barcodeHeight);
                        XSize textSize = gfx.MeasureString(barcodeData, xSmallFont);
                        double textX = barcodeX + (barcodeWidth - textSize.Width) / 2;
                        double textY = y + 85 + barcodeHeight;
                        gfx.DrawString(barcodeData, xSmallFont, XBrushes.Black, new XRect(textX, textY, textSize.Width, textSize.Height), XStringFormats.TopLeft);
                    }
                }
                else
                {
                    if (discountChecked)
                    {
                        gfx.DrawString($"Pret: {originalPrice:F2} Lei", originalPriceFont, XBrushes.Black, new XRect(x + 10, y + 50, labelWidth - 20, 20), XStringFormats.TopLeft);
                        gfx.DrawString(discountText, originalPriceFont, XBrushes.Black, new XRect(x + 10, y + 65, labelWidth - 20, 20), XStringFormats.TopLeft);
                        gfx.DrawString($"{discountedPrice:F2} Lei", biggerPriceFont, XBrushes.Black, new XRect(x + 10, y + 80, labelWidth - 20, 20), XStringFormats.TopLeft);
                    }
                    else
                    {
                        gfx.DrawString("Pret:", originalPriceFont, XBrushes.Black, new XRect(x + 10, y + 65, labelWidth - 20, 20), XStringFormats.TopLeft);
                        gfx.DrawString($"{originalPrice:F2} Lei", biggerPriceFont, XBrushes.Black, new XRect(x + 10, y + 80, labelWidth - 20, 20), XStringFormats.TopLeft);
                    }
                    if (barcodeChecked && currentIncludeBarcode && !string.IsNullOrEmpty(barcodeData))
                    {
                        XImage qrImage = GenerateQRCodeHelper.GenerateQRCode(barcodeData);
                        gfx.DrawImage(qrImage, x + labelWidth - 70, y + 45, 70, 70);
                    }
                }
                if (!string.IsNullOrEmpty(product.Producer) && producerChecked && currentIncludeProducer)
                {
                    XFont producerFont = new("Century Gothic", 7, PdfSharpCore.Drawing.XFontStyle.Bold);
                    double availableWidth;
                    if (barcodeChecked)
                    {
                        if (barcodeType == "Cod bare clasic")
                        {
                            int marginR = 10;
                            int barcodeW = (int)(labelWidth * 0.9 / 2) - 30;
                            double barcodeX = x + labelWidth - barcodeW - marginR;
                            availableWidth = barcodeX - (x + 10);
                        }
                        else
                            availableWidth = (x + labelWidth - 60) - (x + 10);
                    }
                    else
                        availableWidth = labelWidth - 20;
                    string truncatedProducer = TextHelper.TruncateText(gfx, product.Producer, producerFont, availableWidth);
                    gfx.DrawString(truncatedProducer, producerFont, XBrushes.Black, new XRect(x + 10, y + labelHeight - 12, availableWidth, 10), XStringFormats.TopLeft);
                }
                labelCount++;
                if (labelCount % 2 == 0)
                {
                    x = col1X;
                    y += labelHeight + padding;
                }
                else
                    x = col2X;
            }
            string pdfPath = "Etichete.pdf";
            pdf.Save(pdfPath);
            return pdfPath;
        }
    }

    static class Program
    {
        public static string? ConnectionString;



        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string savedToken = Authentication.LoadAuthToken();

            SplashScreenForm splash = new();
            splash.Show();
            Application.DoEvents();
            Thread.Sleep(2000);
            splash.Close();

            try
            {
                var creds = SecureConfig.Load();
                var builder = new SqlConnectionStringBuilder
                {
                    DataSource = creds?.Server,
                    InitialCatalog = creds?.Database,
                    UserID = creds?.User,
                    Password = creds?.Password,
                    Encrypt = true,
                    TrustServerCertificate = false,
                    PersistSecurityInfo = false,
                    MultipleActiveResultSets = false,
                    ConnectTimeout = 30
                };
                ConnectionString = builder.ConnectionString;
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Nu am putut citi utilities.enc: " + ex.Message, "Eroare", MessageBoxButtons.OK);
                return;
            }


            if (!string.IsNullOrEmpty(savedToken) && Authentication.ValidateAuthToken(savedToken, ConnectionString))
            {
                Application.Run(new MainForm(1));
            }
            else
            {
                if (!string.IsNullOrEmpty(savedToken))
                {
                    File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PriceTag.xml"));
                }
                using (LoginForm login = new())
                {
                    if (login.ShowDialog() == DialogResult.OK)
                    {
                        Enrollment.Enroll(login.UserEmail, ConnectionString);
                        Application.Run(new MainForm(login.CollectionId));
                    }
                    else
                    {
                        Application.Exit();
                    }
                }
            }
        }
    }

}
