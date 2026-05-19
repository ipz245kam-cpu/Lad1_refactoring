using MyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Sudoky
{
    public partial class RecordForm : Form
    {
        private BackgroundWorker backgroundWorker;
        private List<Attemt> loadedRecords;

        public RecordForm()
        {
            InitializeComponent();

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        private void RecordForm_Load(object sender, EventArgs e)
        {

            loadingLabelEasy.Text = "Завантаження...";
            loadingLabelMedium.Text = "Завантаження...";
            loadingLabelHard.Text = "Завантаження...";
            loadingLabelDev.Text = "Завантаження...";

            loadingLabelEasy.Visible = true;
            loadingLabelMedium.Visible = true;
            loadingLabelHard.Visible = true;
            loadingLabelDev.Visible = true;

            recordsListViewEasy.Visible = false;
            recordsListViewMedium.Visible = false;
            recordsListViewHard.Visible = false;
            recordsListViewDev.Visible = false;

            backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            loadedRecords = new List<Attemt>();

            using (var db = new DB())
            {
                if (db.Open() != null) return;

                var reader = db.ExecuteReader("SELECT id, time, userId, userName, complication FROM attempts");
                if (reader == null) return;

                while (reader.Read())
                {
                    loadedRecords.Add(new Attemt
                    {
                        TimeRaw = reader["time"].ToString(),
                        UserId = Convert.ToInt32(reader["userId"]),
                        UserName = reader["userName"].ToString(),
                        Complication = reader["complication"].ToString()
                    });
                }

                reader.Close();
            }

            loadedRecords.Sort((a, b) =>
            {
                if (TimeSpan.TryParse(a.TimeRaw, out var timeA) && TimeSpan.TryParse(b.TimeRaw, out var timeB))
                {
                    return timeA.CompareTo(timeB);
                }
                return 0;
            });

            for (int i = 0; i < loadedRecords.Count; i++)
            {
                loadedRecords[i].Index = i + 1;
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loadingLabelEasy.Visible = false;
            loadingLabelMedium.Visible = false;
            loadingLabelHard.Visible = false;
            loadingLabelDev.Visible = false;

            recordsListViewEasy.Items.Clear();
            recordsListViewMedium.Items.Clear();
            recordsListViewHard.Items.Clear();
            recordsListViewDev.Items.Clear();

            var easyRecords = loadedRecords.Where(r => r.Complication == "Легка").ToList();
            var mediumRecords = loadedRecords.Where(r => r.Complication == "Середня").ToList();
            var hardRecords = loadedRecords.Where(r => r.Complication == "Важка").ToList();
            var devRecords = loadedRecords.Where(r => r.Complication == "Dev").ToList();

            FillListView(recordsListViewEasy, easyRecords);
            FillListView(recordsListViewMedium, mediumRecords);
            FillListView(recordsListViewHard, hardRecords);
            FillListView(recordsListViewDev, devRecords);

            recordsListViewEasy.Visible = true;
            recordsListViewMedium.Visible = true;
            recordsListViewHard.Visible = true;
            recordsListViewDev.Visible = true;
        }

        private void FillListView(ListView listView, List<Attemt> records)
        {
            foreach (var record in records)
            {
                string nameDisplay = record.UserId == GlobalUser.UserID
                    ? $"ВИ({record.UserName})"
                    : record.UserName;

                ListViewItem item = new ListViewItem(record.Index.ToString());
                item.SubItems.Add(record.TimeRaw);
                item.SubItems.Add(record.UserId.ToString());
                item.SubItems.Add(nameDisplay);
                item.SubItems.Add(record.Complication);

                if (record.UserId == GlobalUser.UserID)
                    item.SubItems[3].ForeColor = Color.Red;

                listView.Items.Add(item);
            }
        }
    }
}
