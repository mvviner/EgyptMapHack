using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EgyptMapHack {
    public partial class Form1 : Form {
        private FileInfo CurrentFile = null;
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
            try
            {
                bool update = false;
                var latestFile = new DirectoryInfo(DirectoryPath).GetFiles("*.json").Where(x => x.FullName.IndexOf("save_map") == -1).OrderByDescending(x => x.LastWriteTimeUtc).First();
                if (CurrentFile == null || CurrentFile.LastWriteTimeUtc < latestFile.LastWriteTimeUtc)
                {
                    CurrentFile = latestFile;
                    update = true;
                }
                if (!update) return;
                string json = File.ReadAllText(CurrentFile.FullName);
                CurrentSave = JsonConvert.DeserializeObject<EgyptSave>(json);
                Refresh();
            } catch (IOException) {
                StartTimer();
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
            float sizev = Math.Min(ClientSize.Width - 20, (ClientSize.Height - 180) / 3);
            float sizeh = Math.Min((ClientSize.Width - 30) / 2, (ClientSize.Height - 20) / 2);
            bool vertical = sizev > sizeh;
            float size = Math.Max(Math.Max(sizeh, sizev), 160);
            RectangleF rect1 = new RectangleF(10, 160, size, size);
            RectangleF rect2 = new RectangleF(vertical ? 10 : 20 + size, vertical ? rect1.Bottom + 10 : Math.Max(10, 80 - size / 2), size, size * 2);
            int TotalTurnsToExplore = 0;
            int WorkersScouting = 0;
            DrawMap(e.Graphics, rect1, CurrentSave.City, ref TotalTurnsToExplore, ref WorkersScouting);
            DrawMap(e.Graphics, rect2, CurrentSave.Egypt, ref TotalTurnsToExplore, ref WorkersScouting);
            TotalTurnsToExploreLabel.Text = "Total turns to explore: " + TotalTurnsToExplore;
            WorkersScoutingLabel.Text = "Workers scouting: " + WorkersScouting;
            TurnLabel.Text = "Turn: " + CurrentSave.Turn;
            int cnt = 0;
            foreach (Event ev in Schedule.EventList.Where(ev => ev.Turn > CurrentSave.Turn)) {
                Label l = cnt == 0 ? Event1Label : cnt == 1 ? Event2Label : Event3Label;
                l.Text = ev.Turn.ToString() + " - " + ev.Name;
                l.ForeColor = ev.Turn == CurrentSave.Turn + 1 ? Color.Red : Color.Black;
                if (++cnt == 3) break;
            }
            if (cnt == 0) Event1Label.Text = "";
            if (cnt <= 1) Event2Label.Text = "";
            if (cnt <= 2) Event3Label.Text = "";
        }
        private void DrawMap(Graphics g, RectangleF rect, Map Map, ref int TotalTurnsToExplore, ref int WorkersScouting) {
            PointF center = new PointF(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            float coef = rect.Width / 26;
            g.DrawRectangle(Pens.Black, rect.Left, rect.Top, rect.Width, rect.Height);
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
                bool CannotRaidWarning = raiding && TurnsToWait > 0 && cell.Tasks[0].Complexity - cell.Tasks[0].Progress == 1;
                bool ShouldRaidWarning = raiding && TurnsToWait <= 0;
                bool ScoutWarning = exploring && TurnsToExplore <= 0;
                if (CannotRaidWarning || ScoutWarning)
                    g.DrawEllipse(new Pen(Color.Red, 2), center.X + cell.X * coef - coef, center.Y - cell.Y * coef - coef, coef * 2, coef * 2);
                if (ShouldRaidWarning)
                    g.DrawEllipse(new Pen(Color.Blue, 2), center.X + cell.X * coef - coef, center.Y - cell.Y * coef - coef, coef * 2, coef * 2);
                g.DrawString((TurnsToExplore <= 0 ? ressymbol : TurnsToExplore.ToString() + (ressymbol == "-" ? "" : ressymbol)) + (TurnsToWait <= 0 ? "" : TurnsToWait.ToString()),
                    Font, Brushes.Black, center.X + cell.X * coef - size.Width / 2, center.Y - cell.Y * coef - size.Height / 2);
                TotalTurnsToExplore += TurnsToExplore;
                WorkersScouting += exploring ? cell.WorkersCount : 0;
            }
        }
        private void AcceleratedExploration_CheckedChanged(object sender, EventArgs e) {
            Refresh();
        }
    }
    public class Event {
        public int Turn { get; set; }
        public string Name { get; set; }
        public Event(int Turn, string Name) { this.Turn = Turn; this.Name = Name; }
    }
    public static class Schedule {
        private static string ScheduleString = @"
11 - complete the mission ""Survival"" (growth up to 3 people). +5 food
13 - random event
16 - tribe event (attack - before turn 25 you should grow up to 5 people)
22 - random event
32 - ""The Sanctuary of the Falcon"" trial
34 - random event (flood)
35 - pass the trial. +30 food, +40 hammers
36 - tribe event
44 - tribe event
47 - random event
52 - tribe event
57 - random event
61 - ""The Exodus from the Sahara"" trial
63 - Desertification of the Sahara (-2 food, -2 hammers)
64 - Refugees (+2 workers)
64 - pass the trial. +160 culture, +40 prestige
66 - random event
68 - the First sand storm (2 districts damaged)
72 - contact with the Nubians (+ hammers)
74 - tribe event
78 - random event
81 - tribe event
84 - random event
86 - incorporation of Eileithyias (+2 food, +2 hammers, +300 culture)
86 - complete the mission ""The Formation of the nobility"" (train 500 warriors). +100 warriors
87 - ""The Hierakonpolis chiefdom"" trial. Pass it immediately. +1 food, +2 hammers, +2 authority.
89 - Zoo
92 - random event
96 - tribe event
99 - random event
101 - the Caravan from Mesopotamia
103 - damage of the workshop (1 district)
105 - tribe event
108 - random event
109 - tribe event
112 - Best days of Hierakonpolis (+ culture)
115 - random event
118 - the Trade blockade, the mission to join Thinis begins
119 - tribe event
121 - ""The Great drought"" trial. Pass it immediately
123 - complete the mission to join Thinis. +300 warriors
124 - random event
125 - complete the mission to join other tribes
126 - complete the mission to conquer the tribe of Seth. +10% strength, +20% trade
127 - tribe event
132 - Immigration (worker price +20%, effects of events changed)
133 - random event
137 - tribe event
140 - tribe event
142 - random event
145 - Necropolis at Abydos (+ authority)
147 - tribe event
150 - random event
151 - ""The Ombos Invasion"" trial. The trial is omitted. +30 strength each turn
155 - Governors (+5 food or +12 strength or +5 prestige each turn)
165 - tribe event
168 - random event
171 - King Scorpion. Build a tomb not later than the turn 178
174 - random event
177 - tribe event
181 - ""The Civil War"" trial
183 - pass the trial. +400 authority
183 - random event
186 - King Iri-Horus. Build a tomb not later than the turn 192
189 - tribe event
192 - King Ka. Build a tomb not later than the turn 199
194 - random event
197 - tribe event
201 - King Narmer. Build a tomb immediately (finish not later than the turn 204).
202 - random event
204 - ""The Unification Of Egypt"" trial";
        public static List<Event> EventList = new List<Event>();
        static Schedule() {
            string[] strings = ScheduleString.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in strings) {
                int pos = s.IndexOf(" - ");
                if (pos > 0) {
                    int turn;
                    if (!int.TryParse(s.Substring(0, pos), out turn)) continue;
                    EventList.Add(new Event(turn, s.Substring(pos + 3)));
                }
            }
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
