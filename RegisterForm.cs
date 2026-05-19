using MyLib;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Sudoky
{
    public partial class RegisterForm : Form
    {
        private Label userLabel;
        private TextBox userNameTextBox;

        private Label passwordLabel;
        private TextBox passwordTextBox;

        private Button registerButton;

        public RegisterForm()
        {
            this.Text = "Реєстрація користувача";
            this.Size = new System.Drawing.Size(300, 250);
            this.StartPosition = FormStartPosition.CenterParent;

            userLabel = new Label()
            {
                Text = "Ім'я користувача:",
                Location = new System.Drawing.Point(20, 20),
                AutoSize = true
            };

            userNameTextBox = new TextBox()
            {
                Location = new System.Drawing.Point(20, 50),
                Width = 240
            };

            passwordLabel = new Label()
            {
                Text = "Пароль:",
                Location = new System.Drawing.Point(20, 90),
                AutoSize = true
            };

            passwordTextBox = new TextBox()
            {
                Location = new System.Drawing.Point(20, 120),
                Width = 240,
                UseSystemPasswordChar = true
            };

            registerButton = new Button()
            {
                Text = "Зареєструватись",
                Location = new System.Drawing.Point(20, 160),
                Width = 240
            };
            registerButton.Click += RegisterButton_Click;

            this.Controls.Add(userLabel);
            this.Controls.Add(userNameTextBox);
            this.Controls.Add(passwordLabel);
            this.Controls.Add(passwordTextBox);
            this.Controls.Add(registerButton);
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            string userName = userNameTextBox.Text.Trim();
            string password = passwordTextBox.Text;

            if (userName == null || userName.Trim() == "")
            {
                MessageBox.Show("Ім’я користувача не може бути порожнім.", "Помилка");
                return;
            }

            if (userName.Length < 3 || userName.Length > 20)
            {
                MessageBox.Show("Ім’я користувача має містити від 3 до 20 символів.", "Помилка");
                return;
            }

            if (!Regex.IsMatch(userName, @"^[a-zA-Z0-9_]+$"))
            {
                MessageBox.Show("Ім’я користувача може містити лише літери, цифри та підкреслення.", "Помилка");
                return;
            }

            if (userName == null || password.Trim() == "")
            {
                MessageBox.Show("Пароль не може бути порожнім.", "Помилка");
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Пароль має містити щонайменше 6 символів.", "Помилка");
                return;
            }

            if (password.Contains(" "))
            {
                MessageBox.Show("Пароль не повинен містити пробілів.", "Помилка");
                return;
            }

            if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$"))
            {
                MessageBox.Show("Пароль має містити хоча б одну велику літеру, одну малу літеру та одну цифру.", "Помилка");
                return;
            }

            try
            {
                using (var db = new DB())
                {
                    var error = db.Open();
                    if (error != null)
                    {
                        MessageBox.Show("Помилка підключення: " + error.Message);
                        return;
                    }

                    string checkSql = $"SELECT COUNT(*) FROM users WHERE userName = '{userName.Replace("'", "''")}'";
                    var exists = Convert.ToInt32(db.ExecuteScalar(checkSql));
                    if (exists > 0)
                    {
                        MessageBox.Show("Користувач з таким ім'ям вже існує.", "Помилка");
                        return;
                    }

                    string insertSql = $"INSERT INTO users (userName, password) VALUES ('{userName.Replace("'", "''")}', '{password.Replace("'", "''")}')";
                    int rowsAffected = db.ExecuteNonQuery(insertSql);

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Користувача успішно зареєстровано!", "Успіх");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Помилка при реєстрації користувача.", "Помилка");
                    }

                    db.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Виникла помилка: " + ex.Message, "Помилка");
            }
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {
        }
    }
}
