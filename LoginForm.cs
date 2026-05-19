using MyLib;
using System;
using System.Windows.Forms;

namespace Sudoky
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, EventArgs e)
        {
            var username = usernameTextBox.Text.Trim();
            var password = passwordTextBox.Text;

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Ім'я користувача не може бути пустим.", "Помилка");
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пароль не може бути пустим.", "Помилка");
                return;
            }

            try
            {
                using (var db = new DB())
                {
                    db.Open();
            
                   string sql = "SELECT id, password FROM users WHERE userName = ?";
                    using (var reader = db.ExecuteReader(sql))
                    {
                        if (!reader.Read())
                        {
                            MessageBox.Show("Користувача не знайдено.", "Помилка");
                            return;
                        }

                        int userId = reader.GetInt32(0);
                        string storedPassword = reader.GetString(1);

                        if (storedPassword != password)
                        {
                            MessageBox.Show("Невірний пароль.", "Помилка");
                            return;
                        }

                        GlobalUser.Username = username;
                        GlobalUser.UserID = userId;
                        MessageBox.Show($"Вітаємо, {username}!", "Авторизація успішна");
                        this.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Виникла помилка: " + ex.Message, "Помилка");
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}
