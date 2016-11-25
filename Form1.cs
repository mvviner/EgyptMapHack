using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing.Drawing2D;

namespace EgyptMapHack {
    public partial class Form1 : Form {
        private string CurrentFilename = null;
        private EgyptSave CurrentSave = null;
        private string DirectoryPath;
        private Timer Timer = new Timer();
        public Form1() {
            InitializeComponent();
            DirectoryPath = Environment.CurrentDirectory;
            if (!File.Exists(Path.Combine(DirectoryPath, "save_map.json")))
                DirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"..\LocalLow\Clarus Victoria\Predynastic Egypt\saves");
            try {
                ShowLatestFile();
                Timer.Tick += (sender, e) => ShowLatestFile();
                FileSystemWatcher fsw = new FileSystemWatcher(DirectoryPath, "*.json") { SynchronizingObject = this };
                fsw.Created += (sender, e) => StartTimer();
                fsw.Changed += (sender, e) => StartTimer();
                fsw.EnableRaisingEvents = true;
            } catch (Exception ex) {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }
        private void StartTimer() {
            Timer.Interval = 1000;
            Timer.Enabled = true;
        }
        private void ShowLatestFile() {
            try {
                bool update = false;
                foreach (string s in Directory.GetFiles(DirectoryPath, "*.json")) {
                    string s1 = Path.GetFileNameWithoutExtension(s);
                    s1 = s1.IndexOf("autosave", StringComparison.CurrentCultureIgnoreCase) >= 0 ? Directory.GetLastWriteTime(s).ToString("yyyyMMddHHmmss") + s1 : s1;
                    if (s1.IndexOf("save_map", StringComparison.CurrentCultureIgnoreCase) < 0 && (CurrentFilename == null || CurrentFilename.CompareTo(s1) < 0)) {
                        CurrentFilename = s1;
                        update = true;
                    }
                }
                if (!update) return;
                string Filename = Path.Combine(DirectoryPath, (CurrentFilename.IndexOf("autosave", StringComparison.CurrentCultureIgnoreCase) >= 0 ? CurrentFilename.Substring(14) : CurrentFilename) + ".json");
                string json = File.ReadAllText(Filename);
                CurrentSave = JsonConvert.DeserializeObject<EgyptSave>(json);
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
            int TotalTurnsToExplore = 0;
            int WorkersScouting = 0;
            DrawMap(e.Graphics, center, coef, 1, CurrentSave.City, ref TotalTurnsToExplore, ref WorkersScouting);
            DrawMap(e.Graphics, new PointF(center.X * 3, center.Y), coef, 2, CurrentSave.Egypt, ref TotalTurnsToExplore, ref WorkersScouting);
            TotalTurnsToExploreLabel.Text = "Total turns to explore: " + TotalTurnsToExplore;
            WorkersScoutingLabel.Text = "Workers scouting: " + WorkersScouting;
        }
        private void DrawMap(Graphics g, PointF center, float coef, float ratio, Map Map, ref int TotalTurnsToExplore, ref int WorkersScouting) {
            g.DrawRectangle(Pens.Black, center.X - 13 * coef, center.Y - 13 * coef * ratio, coef * 26, coef * 26 * ratio);
            foreach (var cell in Map.Cells) {
                string cultres = cell.CultResourceId;
                string ressymbol = (from res in CurrentSave.CultResources where res.ID == cultres select res.IncomeResourceId.Substring(0, 1)).FirstOrDefault() ?? "-";
                bool exploring = cell.Tasks.Count > 0 && cell.Tasks[0].FinalEventId == "!explore_manager";
                bool raiding = cell.Tasks.Count > 0 && cell.Tasks[0].FinalEventId == "tribe_raid_final";
                int PlunderTurnNumber;
                int TurnsToWait = CurrentSave.AllEvents.map.globals.Plundered.TryGetValue(cell.TribeNumber, out PlunderTurnNumber)
                    ? PlunderTurnNumber + 10 - CurrentSave.Turn - (!raiding ? 5 : cell.Tasks[0].Complexity - cell.Tasks[0].Progress) : 0;
                int TurnsToExplore = !exploring ? 0 : (int)Math.Floor(cell.Tasks[0].Complexity * (AcceleratedExploration.Checked && CurrentSave.AllEvents.map.globals.explorationComplexityMod > 0.5f ? 0.5f : 1)) - cell.Tasks[0].Progress;
                var Borders = cell.Borders.Select(p => new PointF(center.X + (cell.X + p.x) * coef, center.Y - (cell.Y + p.y) * coef)).ToArray();
                var size = g.MeasureString(ressymbol, Font);
                if (cell.IsExplored)
                    g.FillEllipse(Brushes.LightGreen, center.X + cell.X * coef - coef, center.Y - cell.Y * coef - coef, coef * 2, coef * 2);
                if (!cell.IsExplored && cell.Tasks.Count > 0 && cell.Tasks[0].FinalEventId == "!explore_manager")
                    g.FillEllipse(Brushes.LightCyan, center.X + cell.X * coef - coef, center.Y - cell.Y * coef - coef, coef * 2, coef * 2);
                //if (cell.CellTypeId == "e_sea" || cell.CellTypeId == "n_river")
                //    g.DrawEllipse(Pens.Blue, center.X + cell.X * coef - coef, center.Y - cell.Y * coef - coef, coef * 2, coef * 2);
                //if (cell.CellTypeId == "e_cities")
                //    g.DrawEllipse(Pens.Brown, center.X + cell.X * coef - coef, center.Y - cell.Y * coef - coef, coef * 2, coef * 2);
                bool CannotRaidWarning = raiding && TurnsToWait > 0 && cell.WorkersCount > 0 && cell.Tasks[0].Complexity - cell.Tasks[0].Progress == 1;
                bool ShouldRaidWarning = raiding && TurnsToWait == 0 && cell.WorkersCount == 0;
                bool ScoutWarning = exploring && TurnsToExplore <= 0 && cell.WorkersCount > 0;
                if (CannotRaidWarning || ScoutWarning)
                    g.DrawEllipse(new Pen(Color.Red, 2), center.X + cell.X * coef - coef, center.Y - cell.Y * coef - coef, coef * 2, coef * 2);
                if (ShouldRaidWarning)
                    g.DrawEllipse(new Pen(Color.Blue, 2), center.X + cell.X * coef - coef, center.Y - cell.Y * coef - coef, coef * 2, coef * 2);
                g.DrawString((TurnsToExplore <= 0 ? ressymbol : TurnsToExplore.ToString() + (ressymbol == "-" ? "" : "/" + ressymbol)) + (TurnsToWait <= 0 ? "" : TurnsToWait.ToString()),
                    Font, Brushes.Black, center.X + cell.X * coef - size.Width / 2, center.Y - cell.Y * coef - size.Height / 2);
                TotalTurnsToExplore += TurnsToExplore;
                WorkersScouting += exploring ? cell.WorkersCount : 0;
            }
        }
        private void AcceleratedExploration_CheckedChanged(object sender, EventArgs e) {
            Refresh();
        }
    }
    public class Cell {
        public string CellTypeId { get; set; }
        public string CultResourceId { get; set; }
        public bool IsExplored { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int TribeNumber { get; set; }
        public int WorkersCount { get; set; }
        public List<CellTask> Tasks { get; set; }
        public List<BorderPoint> Borders { get; set; }
        public override string ToString() => $"{TribeNumber}";
    }
    public class AllEventsNode {
        public AllEventsMapNode map { get; set; }
    }
    public class AllEventsMapNode {
        public AllEventsMapGlobalsNode globals { get; set; }
    }
    public class AllEventsMapGlobalsNode {
        public JObject Globals { get; set; }
        public Dictionary<int, int> Plundered { get; set; } = new Dictionary<int, int>();
        public float explorationComplexityMod { get; set; } = 1;
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            foreach (var prop in Globals.Properties())
                if (prop.Name.StartsWith("plundered_")) {
                    int key = int.Parse(prop.Name.Substring(10));
                    int value = (int)(double)((JValue)prop.Value["value"]).Value;
                    Plundered.Add(key, value);
                } else if (prop.Name == "explorationComplexityMod")
                    explorationComplexityMod = (float)(double)((JValue)prop.Value["value"]).Value;
        }
    }
    public class CellTask {
        public int Capacity { get; set; }
        public int Complexity { get; set; }
        public int Progress { get; set; }
        public string FinalEventId { get; set; } // "!explore_manager", "tribe_base_final", "tribe_raid_final"
    }
    public class BorderPoint {
        public float x { get; set; }
        public float y { get; set; }
        public override string ToString() => $"{x}, {y}";
    }
    public class CultResource {
        public string ID { get; set; }
        public string IncomeResourceId { get; set; }
    }
    public class Map {
        public List<Cell> Cells { get; set; }
    }
    public class EgyptSave {
        public int Turn { get; set; }
        public Map City { get; set; }
        public Map Egypt { get; set; }
        public List<CultResource> CultResources { get; set; }
        public AllEventsNode AllEvents { get; set; }
    }
}
