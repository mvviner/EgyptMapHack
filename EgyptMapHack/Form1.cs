using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace EgyptMapHack {
    public partial class Form1 : Form {
        private string CurrentFilename = null;
        private EgyptSave CurrentSave = null;
        private string DirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\..\LocalLow\Clarus Victoria\Pre-Civilization Egypt\saves";
        public Form1() {
            InitializeComponent();
            ShowLatestFile();
            try {
                FileSystemWatcher fsw = new FileSystemWatcher(DirectoryPath, "*.json");
                fsw.SynchronizingObject = this;
                fsw.Created += File_Created;
                fsw.EnableRaisingEvents = true;
            } catch (Exception ex) {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }
        private void File_Created(object sender, FileSystemEventArgs e) {
            if (e.Name != "save_map.json") ShowLatestFile();
        }
        private void ShowLatestFile() {
            try {
                string name = null;
                foreach (string s in Directory.GetFiles(DirectoryPath, "*.json"))
                    if (!s.Contains("save_map") && !s.Contains("autosave") && (name == null || name.CompareTo(s) < 0)) name = s;
                if (name == null || name == CurrentFilename) return;
                CurrentFilename = name;
                string json = File.ReadAllText(name);
                CurrentSave = Newtonsoft.Json.JsonConvert.DeserializeObject<EgyptSave>(json);
                Refresh();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            Refresh();
        }
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            if (CurrentSave == null) return;
            PointF center = new PointF(ClientSize.Width / 4, ClientSize.Height / 2);
            float coef = Math.Min(ClientSize.Width / 2, ClientSize.Height / 2) * 0.038f;
            DrawMap(e.Graphics, center, coef, 1, CurrentSave.City);
            DrawMap(e.Graphics, new PointF(center.X * 3, center.Y), coef, 2, CurrentSave.Egypt);
        }
        private void DrawMap(Graphics g, PointF center, float coef, float ratio, Map Map) {
            g.DrawRectangle(Pens.Black, center.X - 12.5f * coef, center.Y - 12.5f * coef * ratio, coef * 25, coef * 25 * ratio);
            foreach (var cell in Map.Cells) {
                string cultres = cell.CultResourceId;
                string ressymbol = "-";
                foreach (var res in CurrentSave.CultResources)
                    if (res.ID == cultres)
                        ressymbol = (res.IncomeResourceId).Substring(0, 1);
                g.DrawString(ressymbol, Font, cell.IsExplored ? Brushes.DarkCyan : Brushes.Black, center.X + cell.X * coef, center.Y - cell.Y * coef);
            }
        }
    }
    public class Cell {
        public string CultResourceId { get; set; }
        public bool IsExplored { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
    public class CultResource {
        public string ID { get; set; }
        public string IncomeResourceId { get; set; }
    }
    public class Map {
        public List<Cell> Cells { get; set; }
    }
    public class EgyptSave {
        public Map City { get; set; }
        public Map Egypt { get; set; }
        public List<CultResource> CultResources { get; set; }
    }
}
