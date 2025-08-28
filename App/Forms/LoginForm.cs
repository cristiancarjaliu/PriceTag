using Microsoft.Data.SqlClient;
using PriceTag.App.Interface;
using PriceTag.App.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PriceTag.App.Forms
{
    public partial class LoginForm : Form
    {
        private PictureBox pbLogo = null!;
        private Label lblEmail = null!;
        private TextBox txtEmail = null!;
        private Label lblParola = null!;
        private TextBox txtParola = null!;
        private Label lblCodUnic = null!;
        private TextBox txtCodUnic = null!;
        private Button btnLogin = null!;
        private Button btnGenCod = null!;
        private Button btnRecuperareParola = null!;
        private Label lblNewParola = null!;
        private TextBox txtNewParola = null!;
        private Label lblConfirmaParola = null!;
        private TextBox txtConfirmaParola = null!;
        public string UserEmail { get; private set; } = "";
        public int CollectionId { get; private set; }
        public LoginForm()
        {
            InitializeComponents();
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            this.SetTitleBarColor(AppColors.BgColor);
        }
        private void InitializeComponents()
        {
            this.Size = new Size(350, 450);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = AppColors.BgColor;
            this.Icon = new Icon("Properties\\Resources\\icon.ico");
            this.Text = "Price Tag";
            this.MaximizeBox = false;
            pbLogo = new PictureBox
            {
                Image = Image.FromFile("Properties\\Resources\\logo.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(200, 60),
                Location = new Point((this.ClientSize.Width - 200) / 2, 10),
                BackColor = Color.Transparent
            };
            this.Controls.Add(pbLogo);
            lblEmail = new Label { Text = "Email:", Font = new Font("Century Gothic", 10, FontStyle.Bold), ForeColor = AppColors.AccentColor, AutoSize = true };
            txtEmail = new TextBox { Font = new Font("Century Gothic", 10, FontStyle.Bold), Width = 250 };
            lblParola = new Label { Text = "Parola:", Font = new Font("Century Gothic", 10, FontStyle.Bold), ForeColor = AppColors.AccentColor, AutoSize = true };
            txtParola = new TextBox { Font = new Font("Century Gothic", 10, FontStyle.Bold), Width = 250, UseSystemPasswordChar = true };
            lblCodUnic = new Label { Text = "Cod unic:", Font = new Font("Century Gothic", 10, FontStyle.Bold), ForeColor = AppColors.AccentColor, AutoSize = true };
            txtCodUnic = new TextBox { Font = new Font("Century Gothic", 10, FontStyle.Bold), Width = 250 };
            lblNewParola = new Label { Text = "Parola noua:", Font = new Font("Century Gothic", 10, FontStyle.Bold), ForeColor = AppColors.AccentColor, AutoSize = true, Visible = false };
            txtNewParola = new TextBox { Font = new Font("Century Gothic", 10, FontStyle.Bold), Width = 250, UseSystemPasswordChar = true, Visible = false };
            lblConfirmaParola = new Label { Text = "Confirma parola:", Font = new Font("Century Gothic", 10, FontStyle.Bold), ForeColor = AppColors.AccentColor, AutoSize = true, Visible = false };
            txtConfirmaParola = new TextBox { Font = new Font("Century Gothic", 10, FontStyle.Bold), Width = 250, UseSystemPasswordChar = true, Visible = false };
            btnLogin = new Button { Text = "Inrolare", Font = new Font("Century Gothic", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat, BackColor = AppColors.BgColor, ForeColor = AppColors.AccentColor, Height = 27, Width = 250 };
            btnLogin.FlatAppearance.BorderColor = AppColors.AccentColor;
            btnLogin.FlatAppearance.BorderSize = 1;
            btnLogin.MouseEnter += (s, e) => { btnLogin.BackColor = AppColors.AccentColor; btnLogin.ForeColor = AppColors.BgColor; };
            btnLogin.MouseLeave += (s, e) => { btnLogin.BackColor = AppColors.BgColor; btnLogin.ForeColor = AppColors.AccentColor; };
            btnLogin.Click += BtnLogin_Click;
            btnGenCod = new Button { Text = "Genereaza cod unic", Font = new Font("Century Gothic", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat, BackColor = AppColors.BgColor, ForeColor = AppColors.AccentColor, Width = 250, Height = 27 };
            btnGenCod.FlatAppearance.BorderColor = AppColors.AccentColor;
            btnGenCod.FlatAppearance.BorderSize = 1;
            btnGenCod.MouseEnter += (s, e) => { btnGenCod.BackColor = AppColors.AccentColor; btnGenCod.ForeColor = AppColors.BgColor; };
            btnGenCod.MouseLeave += (s, e) => { btnGenCod.BackColor = AppColors.BgColor; btnGenCod.ForeColor = AppColors.AccentColor; };
            btnGenCod.Click += async (s, e) => { await GenerateUniqueCodeAsync(); };
            btnRecuperareParola = new Button { Text = "Recuperare parola", Font = new Font("Century Gothic", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat, BackColor = AppColors.BgColor, ForeColor = AppColors.AccentColor, Width = 250, Height = 27 };
            btnRecuperareParola.FlatAppearance.BorderColor = AppColors.AccentColor;
            btnRecuperareParola.FlatAppearance.BorderSize = 1;
            btnRecuperareParola.MouseEnter += (s, e) => { btnRecuperareParola.BackColor = AppColors.AccentColor; btnRecuperareParola.ForeColor = AppColors.BgColor; };
            btnRecuperareParola.MouseLeave += (s, e) => { btnRecuperareParola.BackColor = AppColors.BgColor; btnRecuperareParola.ForeColor = AppColors.AccentColor; };
            btnRecuperareParola.Click += (s, e) => { RecoverCollectionPassword(); };
            FlowLayoutPanel panelFields = new()
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            panelFields.Controls.Add(lblEmail);
            panelFields.Controls.Add(txtEmail);
            panelFields.Controls.Add(lblParola);
            panelFields.Controls.Add(txtParola);
            panelFields.Controls.Add(lblCodUnic);
            panelFields.Controls.Add(txtCodUnic);
            panelFields.Controls.Add(lblNewParola);
            panelFields.Controls.Add(txtNewParola);
            panelFields.Controls.Add(lblConfirmaParola);
            panelFields.Controls.Add(txtConfirmaParola);
            FlowLayoutPanel panelButtons = new()
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 20, 0, 0)
            };
            panelFields.Padding = new Padding(0, 30, 0, 0);
            pbLogo.Margin = new Padding(25, 10, 0, 0);
            panelButtons.Controls.Add(btnRecuperareParola);
            panelButtons.Controls.Add(btnGenCod);
            panelButtons.Controls.Add(btnLogin);
            FlowLayoutPanel panelMain = new()
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            panelMain.Controls.Add(pbLogo);
            panelMain.Controls.Add(panelFields);
            panelMain.Controls.Add(panelButtons);
            panelMain.Location = new Point((this.ClientSize.Width - panelMain.PreferredSize.Width) / 2, 10);
            this.Controls.Add(panelMain);
        }
        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string parola = txtParola.Text;
            string codUnic = txtCodUnic.Text.Trim();
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(parola) || string.IsNullOrEmpty(codUnic))
            {
                CustomMessageBox.Show("Email, Parola si Cod Unic sunt obligatorii!", "Atentie!", MessageBoxButtons.OK);
                return;
            }
            string connectionString = Program.ConnectionString;
            int id = 0;
            string storedHash = "";
            string storedCodUnic = "";
            DateTime uniqueCodeExpiry = DateTime.MinValue;
            using (SqlConnection conn = new(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new("select Id, CollectionPassword, UniqueCode, UniqueCodeExpiry from Collection where Email=@Email", conn);
                cmd.Parameters.AddWithValue("@Email", email);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        CustomMessageBox.Show("Email sau Parola sunt invalide!", "Atentie!", MessageBoxButtons.OK);
                        return;
                    }
                    id = reader.GetInt32(0);
                    storedHash = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    storedCodUnic = reader.IsDBNull(2) ? "" : reader.GetString(2);
                    if (!reader.IsDBNull(3))
                        uniqueCodeExpiry = reader.GetDateTime(3);
                }
                if (string.IsNullOrEmpty(storedHash))
                {
                    if (!lblNewParola.Visible)
                    {
                        lblNewParola.Visible = true;
                        txtNewParola.Visible = true;
                        lblConfirmaParola.Visible = true;
                        txtConfirmaParola.Visible = true;
                        CustomMessageBox.Show("Introduceti Parola Noua si Confirmati Parola.", "Atentie!", MessageBoxButtons.OK);
                        return;
                    }
                    else
                    {
                        if (txtNewParola.Text != txtConfirmaParola.Text)
                        {
                            CustomMessageBox.Show("Parolele nu corespund!", "Atentie!", MessageBoxButtons.OK);
                            return;
                        }
                        string newHash = BCrypt.Net.BCrypt.HashPassword(txtNewParola.Text);
                        SqlCommand cmdUpdate = new("update Collection set CollectionPassword=@hash where Id=@Id", conn);
                        cmdUpdate.Parameters.AddWithValue("@hash", newHash);
                        cmdUpdate.Parameters.AddWithValue("@Id", id);
                        cmdUpdate.ExecuteNonQuery();
                    }
                }
                else
                {
                    if (!BCrypt.Net.BCrypt.Verify(parola, storedHash))
                    {
                        CustomMessageBox.Show("Email sau Parola sunt invalide!", "Atentie!", MessageBoxButtons.OK);
                        return;
                    }
                }
                if (uniqueCodeExpiry < DateTime.Now || storedCodUnic != codUnic)
                {
                    CustomMessageBox.Show("Codul unic este invalid!", "Atentie!", MessageBoxButtons.OK);
                    return;
                }
                SqlCommand cmdCount = new("select count(*) from Licenses where CollectionId=@CollectionId and IsValid=1 and EndDate>GETDATE()", conn);
                cmdCount.Parameters.AddWithValue("@CollectionId", id);
                object? result = cmdCount.ExecuteScalar();
                int count = result != null ? Convert.ToInt32(result) : 0;
                if (count <= 0)
                {
                    CustomMessageBox.Show("Nu s-a gasit nicio licenta activa pentru acest cont!", "Atentie!", MessageBoxButtons.OK);
                    return;
                }
            }
            UserEmail = email;
            CollectionId = id;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void RecoverCollectionPassword()
        {
            string email = txtEmail.Text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                CustomMessageBox.Show("Introduceti email-ul colectiei!", "Atentie!", MessageBoxButtons.OK);
                return;
            }
            string connectionString = Program.ConnectionString;
            using (SqlConnection conn = new(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new("select CollectionPassword from Collection where Email=@Email", conn);
                cmd.Parameters.AddWithValue("@Email", email);
                var result = cmd.ExecuteScalar();
                if (result == null)
                {
                    CustomMessageBox.Show("Email-ul colectiei nu a fost gasit!", "Atentie!", MessageBoxButtons.OK);
                    return;
                }
                string newPassword = SecurityPassGenerator.GenerateRandomPassword(12);
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                SqlCommand cmdUpdate = new("update Collection set CollectionPassword=@pass where Email=@Email", conn);
                cmdUpdate.Parameters.AddWithValue("@pass", hashedPassword);
                cmdUpdate.Parameters.AddWithValue("@Email", email);
                cmdUpdate.ExecuteNonQuery();
                EmailSender.SendEmail(email, "Recuperare Parola", "Noua parola a colectiei: " + newPassword);
                CustomMessageBox.Show("Noua parola a fost trimisa pe email.", "Atentie!", MessageBoxButtons.OK);
            }
        }
        private async Task GenerateUniqueCodeAsync()
        {
            string email = txtEmail.Text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                CustomMessageBox.Show("Introduceti email-ul colectiei!", "Atentie!", MessageBoxButtons.OK);
                return;
            }
            string connectionString = Program.ConnectionString;
            using (SqlConnection conn = new(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new("select CollectionPassword from Collection where Email=@Email", conn);
                cmd.Parameters.AddWithValue("@Email", email);
                var result = cmd.ExecuteScalar();
                if (result == null)
                {
                    CustomMessageBox.Show("Email-ul colectiei nu a fost gasit!", "Atentie!", MessageBoxButtons.OK);
                    return;
                }
                string uniqueCode = SecurityPassGenerator.GenerateUniqueCode(8);
                DateTime expiry = DateTime.Now.AddHours(24);
                SqlCommand cmdUpdate = new("update Collection set UniqueCode=@code, UniqueCodeExpiry=@expiry where Email=@Email", conn);
                cmdUpdate.Parameters.AddWithValue("@code", uniqueCode);
                cmdUpdate.Parameters.AddWithValue("@expiry", expiry);
                cmdUpdate.Parameters.AddWithValue("@Email", email);
                cmdUpdate.ExecuteNonQuery();
                EmailSender.SendEmail(email, "Cod Unic", "Codul tau unic: " + uniqueCode);
                CustomMessageBox.Show("Codul unic a fost trimis pe email.", "Atentie!", MessageBoxButtons.OK);
            }
            await Task.CompletedTask;
        }
    }
}
