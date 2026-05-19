using System;
using System.Drawing;
using System.Windows.Forms;
using MyLib;

namespace Sudoky
{
    public partial class Form1 : Form
    {
        private TextBox[,] cells = new TextBox[9, 9];
        private int[,] board = new int[9, 9];
        private int[,] solution;
        private Button newGameBtn;
        private Button menuBtn;
        private Button checkBtn;
        private Button changeComplicationBtn;
        private Label labelComplication;
        private Label labelTimer;
        private Timer gameTimer;
        private enum Difficulty { Dev = 1, Easy = 20, Medium = 40, Hard = 75 }
        private int complication = 1;
        private string complicationString = "Dev";
        private TimeSpan elapsedTime;
        private Panel numberPad;
        private TextBox selectedCell;

        public Form1()
        {
            InitializeComponent();
            InitializeGrid();
            InitializeTimer();
            this.Paint += Form1_Paint;
            GeneratePuzzle();
        }

        private void InitializeTimer()
        {
            labelTimer = new Label
            {
                Text = "Час: 00:00",
                Location = new Point(725 , 175),
                Width = 200,
                Height = 40,
                Font = new Font("Arial", 24, FontStyle.Bold)
            };
            this.Controls.Add(labelTimer);
            gameTimer = new Timer();
            gameTimer.Interval = 1000;
            gameTimer.Tick += GameTimer_Tick;
            elapsedTime = TimeSpan.Zero;
            gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            labelTimer.Text = $"Час: {elapsedTime.Minutes:D2}:{elapsedTime.Seconds:D2}";
        }

        private void InitializeGrid()
        {
            int size = 70;
            int gridSize = size * 9;
            int marginGridTop = 50;
            int marginGridLeft = 50;
            int fontSizeBtn = 28;
            int gapBtn = 90;
            int leftMarg = 250;

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    var tb = new TextBox();
                    tb.Width = tb.Height = size;
                    tb.Multiline = true;
                    tb.MaxLength = 1;
                    tb.Font = new Font("Arial", 36);
                    tb.TextAlign = HorizontalAlignment.Center;
                    tb.BorderStyle = BorderStyle.FixedSingle;
                    tb.Location = new Point(col * size + marginGridLeft, row * size + marginGridTop);
                    tb.Tag = new Point(row, col);
                    tb.KeyPress += Cell_KeyPress;
                    tb.Enter += Cell_Enter;
                    tb.TextChanged += Cell_TextChanged;
                    tb.Click += (s, e) => selectedCell = (TextBox)s;
                    this.Controls.Add(tb);
                    cells[row, col] = tb;
                }
            }

            checkBtn = new Button {  Location = new Point(leftMarg + gridSize, marginGridTop), Width = 90, Height = 90 };
            checkBtn.Name = "checkBtn";
            checkBtn.Click += CheckSolution;
            //checkBtn.Text = "Перевірити";

            //checkBtn.Font = new Font("Arial", fontSizeBtn, FontStyle.Bold);
            //checkBtn.TextAlign = ContentAlignment.MiddleRight;

            checkBtn.Image = new Bitmap(Image.FromFile("Assets\\mag_glass.png"), new Size(74, 74));
            //checkBtn.FlatStyle = FlatStyle.Standard; 
            checkBtn.Padding = new Padding(20,20, 20, 20);
            //checkBtn.ImageAlign = ContentAlignment.MiddleLeft;
            
             
            this.Controls.Add(checkBtn);

            newGameBtn = new Button { Location = new Point(leftMarg + gridSize + checkBtn.Width + gapBtn, marginGridTop), Width = 90, Height = 90 };
            newGameBtn.Name = "newGameBtn";
            newGameBtn.Click += (s, e) => { GeneratePuzzle(); ResetTimer(); };
            
            newGameBtn.Image = new Bitmap(Image.FromFile("Assets\\plus.png"), new Size(74, 74));
            
            this.Controls.Add(newGameBtn);

            menuBtn = new Button { Location = new Point(leftMarg + gridSize + newGameBtn.Width + gapBtn * 2 + checkBtn.Width, marginGridTop), Width = 90, Height = 90 };
            menuBtn.Name = "menuBtn";
            menuBtn.Click += MenuBtn_Click;
            
            menuBtn.Image = new Bitmap(Image.FromFile("Assets\\menu.png"), new Size(74, 74));
            
            this.Controls.Add(menuBtn);

            labelComplication = new Label
            {
                Text = "Зараз : Dev",
                Location = new Point(100 + gridSize, marginGridTop + 100 + checkBtn.Height),
                Width = 600 + gapBtn * 2,
                Height = 50
            };
            labelComplication.Font = new Font("Arial", fontSizeBtn, FontStyle.Bold);
            this.Controls.Add(labelComplication);

            changeComplicationBtn = new Button
            {
                Text = "Змінити складність",    
                Location = new Point(100 + gridSize, marginGridTop + 100 + checkBtn.Height + labelComplication.Height),
                Width = 600 + gapBtn * 2,
                Height = 90
            };
            changeComplicationBtn.Name = "changeComplicationBtn";
            changeComplicationBtn.Click += Change_Complication;
            changeComplicationBtn.Font = new Font("Arial", fontSizeBtn, FontStyle.Bold);
            this.Controls.Add(changeComplicationBtn);

            int numPadBtnSize = 50;

            numberPad = new Panel { Width = numPadBtnSize * 3*2, Height = numPadBtnSize * 3 * 2, Location = new Point(gridSize + 335, 400) };
            for (int i = 1; i <= 9; i++)
            {
                var btn = new Button
                {
                    Text = i.ToString(),
                    Width = numPadBtnSize * 2,
                    Height = numPadBtnSize * 2,
                    Font = new Font("Arial", 28, FontStyle.Bold),
                    Location = new Point(((i - 1) % 3) * numPadBtnSize * 2, ((i - 1) / 3) * numPadBtnSize * 2)
                };
                btn.Click += (s, e) =>
                {
                    if (selectedCell != null && !selectedCell.ReadOnly)
                        selectedCell.Text = ((Button)s).Text;
                };
                numberPad.Controls.Add(btn);
            }
            this.Controls.Add(numberPad);

            this.Width = 1280;
            this.Height = 720;
            this.WindowState = FormWindowState.Maximized;
        }


        private void ResetTimer()
        {
            elapsedTime = TimeSpan.Zero;
            labelTimer.Text = "Час: 00:00";
            gameTimer.Start();
        }

        private void Change_Complication(object sender, EventArgs e)
        {
            
            if (complication == 1) { complication = 20; labelComplication.Text = "Зараз : Легка"; complicationString = "Легка"; }
            else if (complication == 20) { complication = 40; labelComplication.Text = "Зараз : Середня"; complicationString = "Середня"; }
            else if (complication == 40) { complication = 75; labelComplication.Text = "Зараз : Важка"; complicationString = "Важка"; }
            else if (complication == 75) { complication = 1; labelComplication.Text = "Зараз : Dev"; complicationString = "Dev"; }
            changeComplicationBtn.Text = "Loading...";
            changeComplicationBtn.Enabled = false;
            gameTimer.Stop();
            var bw = new System.ComponentModel.BackgroundWorker();
            bw.DoWork += (s, ev) => GeneratePuzzle();
            bw.RunWorkerCompleted += (s, ev) => { changeComplicationBtn.Text = "Змінити складність"; changeComplicationBtn.Enabled = true; ResetTimer(); };
            bw.RunWorkerAsync();
         
            gameTimer.Stop();
            gameTimer.Start();
        }
        private void GeneratePuzzle()
        {
            var generator = new SudokuGenerator();
            board = generator.Generate(complication, out solution);
            FillGrid();
            ResetTimer();
        }

        private void Cell_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (complication == 40 || complication == 75)
                return; 
            if (!char.IsControl(e.KeyChar) && (e.KeyChar < '1' || e.KeyChar > '9')) e.Handled = true;
        }

        private void FillGrid()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    cells[row, col].Text = board[row, col] != 0 ? board[row, col].ToString() : "";
                    cells[row, col].ReadOnly = board[row, col] != 0;
                    cells[row, col].BackColor = board[row, col] != 0 ? Color.LightGray : Color.White;
                }
            }
        }

        private void Cell_Enter(object sender, EventArgs e)
        {
            ResetAllBackgrounds();
            var current = sender as TextBox;
            if (current?.Tag is Point pt)
            {
                int row = pt.X, col = pt.Y;
                for (int i = 0; i < 9; i++)
                {
                    cells[row, i].BackColor = Color.LightBlue;
                    cells[i, col].BackColor = Color.LightBlue;
                }
                cells[row, col].BackColor = Color.CornflowerBlue;
            }
        }

        private void Cell_TextChanged(object sender, EventArgs e)
        {
            if (complication == 40 || complication == 75)
            {
                ResetAllBackgrounds(); 
                return;
            }
            var current = sender as TextBox;
            if (current == null || !(current.Tag is Point pt)) return;
            int row = pt.X, col = pt.Y;
            string value = current.Text.Trim();
            ResetAllBackgrounds();
            if (value == "" || !int.TryParse(value, out int num) || num < 1 || num > 9) return;
            int blockRow = (row / 3) * 3;
            int blockCol = (col / 3) * 3;
            bool conflict = false;
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (r == row && c == col) continue;
                    if (cells[r, c].Text == value &&
                        (r == row || c == col ||
                         (r >= blockRow && r < blockRow + 3 && c >= blockCol && c < blockCol + 3)))
                    {
                        cells[r, c].BackColor = Color.MistyRose;
                        conflict = true;
                    }
                }
            }
            if (conflict) current.BackColor = Color.MistyRose;
        }

        private void ResetAllBackgrounds()
        {
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    cells[r, c].BackColor = cells[r, c].ReadOnly ? Color.LightGray : Color.White;
                }
            }
        }

        private void CheckSolution(object sender, EventArgs e)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (cells[row, col].Text == "" || cells[row, col].Text != solution[row, col].ToString())
                    {
                        MessageBox.Show("Рішення неправильне або не завершене.", "Перевірка");
                        return;
                    }
                }
            }
            gameTimer.Stop();
            try
            {
                using (var db = new DB())
                {
                    var openResult = db.Open();
                    if (openResult != null)
                    {
                        MessageBox.Show("Помилка з'єднання з БД: " + openResult.Message, "База даних");
                        return;
                    }
                    string formattedTime = $"{elapsedTime.Minutes:D2}:{elapsedTime.Seconds:D2}";
                    string insertSql = $"INSERT INTO attempts (time, userId, username, complication) VALUES ('{formattedTime}', {GlobalUser.UserID},'{GlobalUser.Username}','{complicationString}')";
                    db.ExecuteNonQuery(insertSql);
                    db.Close();
                    MessageBox.Show($"Вітаємо! Ви вирішили судоку за {formattedTime}!", "Успіх");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при збереженні результату: " + ex.Message, "Помилка БД");
            }
            gameTimer.Start();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            using (var pen = new Pen(Color.Black, 3))
            {
                int size = 70;
                int offsetX = 50;
                int offsetY = 50;
                int gridSize = size * 9;
                for (int i = 0; i <= 3; i++)
                {
                    int x = offsetX + i * size * 3;
                    g.DrawLine(pen, x, offsetY, x, offsetY + gridSize);
                }
                for (int i = 0; i <= 3; i++)
                {
                    int y = offsetY + i * size * 3;
                    g.DrawLine(pen, offsetX, y, offsetX + gridSize, y);
                }
            }
        }

        private void MenuBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
