using MyLib;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sudoky
{
    public partial class Menu : Form
    {
        private Label titleLabel;
        private Button startGameBtn;
        private Button loginBtn;
        private Label userLabel;
        private Button registerBtn;


        public Menu()
        {
            InitializeComponent();
            SetupUI();
        }
        private void Menu_Load(object sender, EventArgs e)
        { 
        }
            private void SetupUI()
        {

            this.Text = "Sudoku - Меню";

            this.Size = new Size(400, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            titleLabel = new Label()
            {
                Text = "Sudoku",
                Font = new Font("Arial", 32, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(120, 30)
            };

            startGameBtn = new Button()
            {
                Text = "Почати гру",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Size = new Size(200, 50),
                Location = new Point(100, 100)
            };
            startGameBtn.Click += StartGameBtn_Click;

            loginBtn = new Button()
            {
                Text = "Авторизуватись",
                Size = new Size(200, 50),
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(100, 170)
            };
            loginBtn.Click += LoginBtn_Click;

            userLabel = new Label()
            {
                Text = "Користувач: Не авторизований",
                AutoSize = true,
                Location = new Point(10, 300),
                Font = new Font("Arial", 10, FontStyle.Italic)
            };
            
            registerBtn = new Button()
            {
                Text = "Реєстрація",
                Size = new Size(200, 50),
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(100, 225)
            };
            registerBtn.Click += RegisterBtn_Click;


            this.Controls.Add(registerBtn);
            this.Controls.Add(titleLabel);
            this.Controls.Add(startGameBtn);
            this.Controls.Add(loginBtn);
            this.Controls.Add(userLabel);

            this.Activated += MenuForm_FormActivated;
        }


        private void RegisterBtn_Click(object sender, EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.ShowDialog();
            this.Activate();
        }

        private void MenuForm_FormActivated(object sender, EventArgs e)
        {
            bool isAuthenticated = GlobalUser.IsAuthenticated;

            userLabel.Text = isAuthenticated ? $"Користувач: {GlobalUser.Username}" : "Користувач: Не авторизований";
            loginBtn.Text = isAuthenticated ? "Вийти" : "Авторизуватись";

            if (isAuthenticated && registerBtn.Text != "Таблиця рекордів")
            {
                registerBtn.Click -= RegisterBtn_Click;
                registerBtn.Click += AttemtsBtn_Click;
                registerBtn.Text = "Таблиця рекордів";
                registerBtn.TextAlign = ContentAlignment.MiddleCenter;

            }
            else if (!isAuthenticated && registerBtn.Text != "Реєстрація")
            {
                registerBtn.Click -= AttemtsBtn_Click;
                registerBtn.Click += RegisterBtn_Click;
                registerBtn.Text = "Реєстрація";
            }
        }


        private void StartGameBtn_Click(object sender, EventArgs e)
        {
            if (!GlobalUser.IsAuthenticated)
            {
                MessageBox.Show("Будь ласка, авторизуйтесь перед початком гри.", "Авторизація");
                return;
            }

            this.Hide();
            using (Form1 gameForm = new Form1())
            {
                gameForm.ShowDialog();
            }
            this.Show();
        }


        private void LoginBtn_Click(object sender, EventArgs e)
        {
            if (GlobalUser.IsAuthenticated)
            {
                GlobalUser.Username = null;
                MessageBox.Show("Ви вийшли з акаунту.", "Вихід");
                this.Activate();
                
                return;

            }

            LoginForm loginForm = new LoginForm();
            loginForm.ShowDialog();
            this.Activate();
        }

        private void AttemtsBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (RecordForm recordForm = new RecordForm())
            {
                recordForm.ShowDialog();
            }
            this.Show();
        }
    }

}
