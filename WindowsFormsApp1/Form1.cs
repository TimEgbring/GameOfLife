using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Configurations;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        const int BUTTONRANGE = 30;             //Constants
        readonly Color dead_white = Color.White;
        readonly Color full_black = Color.FromArgb(0, 0, 0);
        readonly Color mygrey1 = Color.FromArgb(192, 192, 192);
        readonly Color mygrey2 = Color.FromArgb(128, 128, 128);
        readonly Color mygrey3 = Color.FromArgb(64, 64, 64);


        enum Compass { N, E, S, W, NE, SE, SW, NW };


        private int[,] neighbors = new int[900, 8];         //Constant after Init
        BorderlessButton[] buttons = new BorderlessButton[900];


        int generation_count = 0;

        public delegate void ProcDelegate();

        public ProcDelegate gamemode;
        public delegate void ConstantChangesTick(object sender, EventArgs eventArgs);
        public ConstantChangesTick tickdelegate;
        
        public ProcDelegate designer_delegate;


        bool isrunning = false;         //Game State
        byte version = 0;
        bool gameexists = false;
        bool game_has_started = false;

        DateTime time_start;        //Statistics
        int stat_alivecount = 0;
        int stat_gradientsum_all = 0;

        int stat_alivegradient_sum = 0;

        Color[] colorgrid = new Color[900];         //Handles Gridlogic
        byte[] bytegrid = new byte[900];

        byte[] bytegrid_new = new byte[900];
        bool[] bytegrid_haschanged = new bool[900];

        bool[] designer_bytegrid_haschanged = new bool[900];
        ///<summary>
        ///Mthd handling Designer addition in rel. to black neighbors
        ///</summary>
        byte[] designer_bytegrid_plus = new byte[900];


        bool[] hasaliveneighbors = new bool[900];
        byte[] aliveneighbors_count = new byte[900];
        ///<summary>
        ///Sum of gradients of neighbors from the correspondend bnumber
        ///</summary>
        byte[] neighbors_gradient_sum = new byte[900];
        bool[] isalive = new bool[900];

        public struct GameState
        { // Doppelt gemoppelt, weil ansonsten unnötiger Zeitaufwand. Wird für Speicherung der Daten genutzt.

            public int generation_count;

            public ProcDelegate gamemode;

            public ConstantChangesTick tickdelegate;
            public ProcDelegate designer_delegate;
            public bool isrunning;         //Game State
            public byte version;
            public bool gameexists;
            public DateTime time_start;        //Statistics
            public int stat_alivecount;
            public int stat_alivegradient_sum;
            public int stat_gradientsum_all;
            public Color[] colorgrid;         //Handles Gridlogic
            public byte[] bytegrid;
            public byte[] bytegrid_new;
            public bool[] bytegrid_haschanged;

            public bool[] designer_bytegrid_haschanged;

            public byte[] designer_bytegrid_plus;

            public bool[] hasaliveneighbors;
            public byte[] aliveneighbors_count;
            public byte[] neighbors_gradient_sum;
            public bool[] isalive;

        }
        GameState gamestate;
        


        public ChartValues<MeasureModel> ChartValues { get; set; }      //ConstantChanges Graph

        public Form1()
        {
        
            InitializeComponent();
            
            ConstantChanges();
            gamemode = RuleSetOriginal;
            tickdelegate = TimerOnTick;
            designer_delegate = UpdateColorAll;


            //string fileName = "Spielablauf" + ".txt";
            //string fullPath = Path.GetFullPath(fileName);
            //string directoryName = Path.GetDirectoryName(fullPath);
            //File.AppendAllText(fileName, "3");
            // File.
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            InitButtonArray();
            InitNeighbors();
            InitColorgrid();
            InitVorlagenToolstrip();
            gamestate = GetGameState();
            button_submit_template.Hide();
            label1.Hide();


            
    }
        //Graph Start
        ///<summary>
        ///Main Part of Copied Code Handling Graph
        ///</summary>
        private void ConstantChanges()
        {
            //ConstantChangesStart
            var mapper = Mappers.Xy<MeasureModel>()
               .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
               .Y(model => model.Value);           //use the value property as Y
            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);
            //the ChartValues property will store our values array
            ChartValues = new ChartValues<MeasureModel>();
            cartesianChart1.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Values = ChartValues,
                    PointGeometrySize = 3,
                    StrokeThickness = 3
                },
            };
            cartesianChart1.AxisX.Add(new Axis
            {
                DisableAnimations = true,
                LabelFormatter = value => new DateTime((long)value).ToString("mm:ss"),
                Separator = new Separator
                {
                    Step = TimeSpan.FromSeconds(1).Ticks
                }
            });
            SetAxisLimits(DateTime.Now);
            //ConstantChangesEnds
        }
        ///<summary>
        ///Part of Copied Code Handling Graph
        ///</summary>
        private void SetAxisLimits(DateTime now)
        {
            cartesianChart1.AxisX[0].MaxValue = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 100ms ahead
            cartesianChart1.AxisX[0].MinValue = now.Ticks - TimeSpan.FromSeconds(8).Ticks; //we only care about the last 8 seconds
        }
        ///<summary>
        ///Part of Copied Code Handling Graph
        ///</summary>
        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            var now = DateTime.Now;
            ChartValues.Add(new MeasureModel
            {
                DateTime = now,
                Value = stat_alivecount
            });
            SetAxisLimits(now);
            //lets only use the last 50 values
            if (ChartValues.Count > 100
                ) ChartValues.RemoveAt(0);
        }
        ///<summary>
        ///Part of Copied Code Handling Graph
        ///</summary>
        private void ModifiedTimerOnTick(object sender, EventArgs eventArgs)
        {
            var now = DateTime.Now;
            ChartValues.Add(new MeasureModel
            {
                DateTime = now,
                Value = stat_alivegradient_sum
            });
            SetAxisLimits(now);
            //lets only use the last 50 values
            if (ChartValues.Count > 70
                ) ChartValues.RemoveAt(0);
        }
        //Graph End

        private void InitVorlagenToolstrip() 
        {
            string path = Directory.GetCurrentDirectory() + "\\Vorlagen";
            string path_klass;
            string path_modi;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path_klass = path + "\\Klassisch";
            path_modi = path + "\\Modifiziert";
            if (!Directory.Exists(path_modi))
                Directory.CreateDirectory(path_modi);
            if (!Directory.Exists(path_klass))
                Directory.CreateDirectory(path_klass);

            foreach (string file in Directory.GetFiles((Directory.GetCurrentDirectory() + "\\Vorlagen\\Klassisch") /*,".txt"*/))
            {
                string filename = Path.GetFileNameWithoutExtension(file);
              

                System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
                testToolStripMenuItem = new ToolStripMenuItem();
                klassischToolStripMenuItem2.DropDownItems.AddRange(new ToolStripItem[] {
                testToolStripMenuItem
                });

                testToolStripMenuItem.Name = filename;
                testToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
                testToolStripMenuItem.Text = filename;
                testToolStripMenuItem.Click += new System.EventHandler(LoadVorlage_Click);

            }
            foreach (string file in Directory.GetFiles((Directory.GetCurrentDirectory() + "\\Vorlagen\\Modifiziert") /*,".txt"*/))
            {
                string filename = Path.GetFileNameWithoutExtension(file);
           

                System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
                testToolStripMenuItem = new ToolStripMenuItem();
                modifiziertToolStripMenuItem2.DropDownItems.AddRange(new ToolStripItem[] {
                testToolStripMenuItem
                });
                
                testToolStripMenuItem.Name = filename;
                testToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
                testToolStripMenuItem.Text = filename;
                testToolStripMenuItem.Click += new System.EventHandler(LoadVorlage_Click);

            }

        }

        

        //private void SetGridPointFromZero(int bnumber, byte gradient)
        //{
        //    bytegrid[bnumber] = gradient;
        //    for(int i = 0; i<8; i++)
        //    {

        //    }
        //    switch (gradient)
        //    {
        //        case 0:
        //            break;
        //        case 1:
        //            bgrid = 1;
        //            for(int i = 0; i<8; i++)
        //            {
        //                b
        //            }
        //            break;
        //        case 2:
        //            bgrid = 2;
        //            break;
        //        case 3:
        //            bgrid = 3;
        //            break;
        //        case 4:
        //            bgrid = 4;
        //            break;
        //        default:
        //            break;
        //    }

        //}

        private void LoadVorlage_Click(object sender, EventArgs e)
        {
            byte[] loadable_grid;
            string cd = Directory.GetCurrentDirectory(); // \Debug
            string vcd;
            if (Directory.Exists(cd + "\\Vorlagen")) // Existiert
            {
                //string vcdtest = File.E
                if ((((ToolStripMenuItem)sender).OwnerItem.Text) == "Klassisch")
                    vcd = cd + @"\Vorlagen\Klassisch\";
                else
                    vcd = cd + @"\Vorlagen\Modifiziert\";
                loadable_grid = File.ReadAllBytes(vcd + (((ToolStripMenuItem)sender).Name) + ".txt");

                if (gameexists && !ResetGame())
                    return;
                ByteToDelegateChangeGamemode(loadable_grid[902])(sender, e);
                if (loadable_grid[901] == 1)
                {
                    for (int i = 0; i < 900; i++)
                    {
                        for (int j = 0; j < loadable_grid[i]; j++)
                        {
                            BorderlessButton_Click_All_Method(buttons[i]);
                        }
                    }
                }
                else if(loadable_grid[901] == 0)
                {
                   
                    for (int i = 0; i < 900; i++)
                    {
                        if(loadable_grid[i] == 4)
                            BorderlessButton_Click_All_Method(buttons[i]);
                    }
                }
                
                for(int i = 0; i<900; i++)
                {
                    buttons[i].BackColor = ByteToColor(bytegrid[i]);
                }
            }
            
            else MessageBox.Show("Es wurde kein Ordner mit dem Namen \"Vorlagen\" gefunden.");
            

        }
        
        ///<summary>
        ///Initiates the Colorgrid. all deadwhite
        ///</summary>
        public void InitColorgrid()
        {
            for (int i = 0; i < 900; i++)
            {
                colorgrid[i] = dead_white;
            }
        }
        ///<summary>
        ///Returns the corresponding color of the byte, 1 = white, 2=...
        ///</summary>
        private Color ByteToColor(byte i)   //returns black when higher than 3;
        {
            switch (i)
            {
                case 0: return dead_white;
                case 1: return mygrey1;
                case 2: return mygrey2;
                case 3: return mygrey3;
                default: return full_black;
            };
        }
        ///<summary>
        ///Initiates the neighbors array
        ///</summary>
        private void InitNeighbors()
        {
            for (int i = 0; i < 900; i++)
            {

                if ((i + 1) % BUTTONRANGE == 0)     // Handles EastBorder Overflow
                {
                    neighbors[i, (int)Compass.E] = i - BUTTONRANGE + 1;
                }
                else                // Handles EastBorder
                {
                    neighbors[i, (int)Compass.E] = i + 1;
                }
                if (i < BUTTONRANGE)         //Handles NorthBorder Overflow
                {
                    neighbors[i, (int)Compass.N] = i + 870;  // 870 = 30*30 - 30
                }
                else                // Handles NorthBorder
                {
                    neighbors[i, (int)Compass.N] = i - BUTTONRANGE;
                }
                if (i % BUTTONRANGE == 0)        //Handles WestBorder Overflow
                {
                    neighbors[i, (int)Compass.W] = i + BUTTONRANGE - 1;
                }
                else              //Handles WestBorder
                {
                    neighbors[i, (int)Compass.W] = i - 1;
                }
                if (i >= 900 - BUTTONRANGE)      //Handles SouthBorder Overflow
                {
                    neighbors[i, (int)Compass.S] = i - 870;
                }
                else                //Handles SouthBorder
                {
                    neighbors[i, (int)Compass.S] = i + BUTTONRANGE;
                }


                if ((i + 1) % BUTTONRANGE == 0)        //Handles NorthEast
                {
                    if (i == 29)
                    {
                        neighbors[i, (int)Compass.NE] = 870;
                    }
                    else neighbors[i, (int)Compass.NE] = i - 30 - 29;
                }
                else if (i < BUTTONRANGE)
                {
                    neighbors[i, (int)Compass.NE] = i + 870 + 1;
                }
                else neighbors[i, (int)Compass.NE] = i - 30 + 1;

                if ((i + 1) % BUTTONRANGE == 0)        //Handles SouthEast
                {
                    if (i == 899)
                    {
                        neighbors[i, (int)Compass.SE] = 0;
                    }
                    else neighbors[i, (int)Compass.SE] = i + 1;
                }
                else if (i >= 900 - BUTTONRANGE)
                {
                    neighbors[i, (int)Compass.SE] = i - 870 + 1;
                }
                else neighbors[i, (int)Compass.SE] = i + 30 + 1;

                if (i % BUTTONRANGE == 0)        //Handles SouthWest
                {
                    if (i == 870)
                    {
                        neighbors[i, (int)Compass.SW] = 29;
                    }
                    else neighbors[i, (int)Compass.SW] = i + 30 + 29;
                }
                else if (i >= 900 - BUTTONRANGE)
                {
                    neighbors[i, (int)Compass.SW] = i - 870 - 1;
                }
                else neighbors[i, (int)Compass.SW] = i + 30 - 1;

                if (i < BUTTONRANGE)        //Handles NorthWest
                {
                    if (i == 0)
                    {
                        neighbors[i, (int)Compass.NW] = 899;
                    }
                    else neighbors[i, (int)Compass.NW] = i + 870 - 1;
                }
                else if (i % BUTTONRANGE == 0)
                {
                    neighbors[i, (int)Compass.NW] = i - 1;
                }
                else neighbors[i, (int)Compass.NW] = i - 30 - 1;
            }
        }

        private GameState GetGameState()
        {
            GameState gamestate_tmp = new GameState
            { //INFO: Call by reference(?)
                generation_count = generation_count,
                gamemode = gamemode,

                tickdelegate = tickdelegate,
                designer_delegate = designer_delegate,
                isrunning = isrunning,
                version = version,
                gameexists = gameexists,
                time_start = time_start,
                stat_alivecount = DeepCopy(stat_alivecount),
                stat_alivegradient_sum = DeepCopy(stat_alivegradient_sum),
                stat_gradientsum_all = stat_gradientsum_all,
                colorgrid = DeepCopy(colorgrid),
                bytegrid = DeepCopy(bytegrid),
                bytegrid_new = DeepCopy(bytegrid_new),
                bytegrid_haschanged = DeepCopy(bytegrid_haschanged),

                designer_bytegrid_haschanged = DeepCopy(designer_bytegrid_haschanged),

                designer_bytegrid_plus = DeepCopy(designer_bytegrid_plus),

                hasaliveneighbors = DeepCopy(hasaliveneighbors),
                aliveneighbors_count = DeepCopy(aliveneighbors_count),
                neighbors_gradient_sum = DeepCopy(neighbors_gradient_sum),
                isalive = DeepCopy(isalive)
              };
            return gamestate_tmp;
        }

        

        public static T DeepCopy<T>(T obj)
        {

            if (!typeof(T).IsSerializable)

            {

                throw new Exception("The source object must be serializable");

            }

            if (Object.ReferenceEquals(obj, null))

            {

                throw new Exception("The source object must not be null");

            }

            T result = default(T);

            using (var memoryStream = new MemoryStream())

            {

                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                formatter.Serialize(memoryStream, obj);

                memoryStream.Seek(0, SeekOrigin.Begin);

                result = (T)formatter.Deserialize(memoryStream);

                memoryStream.Close();

            }

            return result;

        }

        private void SetGameGameState(GameState gstate)
        {
            generation_count = gstate.generation_count;
            gamemode = gstate.gamemode;

                tickdelegate = gstate.tickdelegate;
            designer_delegate = gstate.designer_delegate;
            isrunning = gstate.isrunning;
            version = gstate.version;
            gameexists = gstate.gameexists;
            time_start = gstate.time_start;
            stat_alivecount = DeepCopy(gstate.stat_alivecount);
            stat_alivegradient_sum = DeepCopy(gstate.stat_alivegradient_sum);
            stat_gradientsum_all = gstate.stat_gradientsum_all;
            colorgrid = DeepCopy(gstate.colorgrid);
            bytegrid = DeepCopy(gstate.bytegrid);
            bytegrid_new = DeepCopy(gstate.bytegrid_new);
            bytegrid_haschanged = DeepCopy(gstate.bytegrid_haschanged);

            designer_bytegrid_haschanged = DeepCopy(gstate.designer_bytegrid_haschanged);

            designer_bytegrid_plus = DeepCopy(gstate.designer_bytegrid_plus);

            hasaliveneighbors = DeepCopy(gstate.hasaliveneighbors);
            aliveneighbors_count = DeepCopy(gstate.aliveneighbors_count);
            neighbors_gradient_sum = DeepCopy(gstate.neighbors_gradient_sum);
            isalive = DeepCopy(gstate.isalive);
            
            game_has_started = true;
        }
        ///<summary>
        ///sets isalive[] true and stat_alivecount[]++
        ///</summary>
        private void IsAliveAndStatInc(int bnumber)
        {
                isalive[bnumber] = true;
                stat_alivecount++;
        }
        ///<summary>
        ///isalive[i] = false; stat_alivecount--
        ///</summary>
        private void IsNotAliveAndStatDec(int bnumber)
        {
            isalive[bnumber] = false;
            stat_alivecount--;
        }
        private void CDEToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
                      
        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        
        private void ShowRules(object sender, EventArgs e)
        {
            MessageBox.Show("Der Zustand einer Zelle (lebendig oder tot) in der Folgegeneration hängt nur vom aktuellen Zustand der Zelle selbst und den aktuellen Zuständen ihrer acht Nachbarzellen ab. " +
                "\n\n- Eine tote Zelle mit genau drei lebenden Nachbarn wird in der Folgegeneration neu geboren. \n- Lebende Zellen mit weniger als zwei lebenden Nachbarn sterben in der Folgegeneration an Einsamkeit. " +
                "\n- Eine lebende Zelle mit zwei oder drei lebenden Nachbarn bleibt in der Folgegeneration am Leben." +
                "\n- Lebende Zellen mit mehr als drei lebenden Nachbarn sterben in der Folgegeneration an Überbevölkerung.", "Regeln", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BeendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void NewGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ResetGame())
                game_has_started = false;
        }

        ///<summary>
        ///Inc aliveneighbors_count[] and sets hasalivenbors[]=true
        ///</summary>
        private void AliveNeighborsIncBool(int buttonnumber)
        {
            
            for (int i = 0; i < 8; i++)
            {
                hasaliveneighbors[neighbors[buttonnumber, i]] = true;
                aliveneighbors_count[neighbors[buttonnumber, i]]++;
            }
        }
        ///<summary>
        ///Dec aliveneighbors_count[] & if count == 0 -> hasalivenbors[] = false
        ///</summary>
        private void AliveNeighborsDecBool(int buttonnumber)
        {

            for (int i = 0; i < 8; i++)
            {

                aliveneighbors_count[neighbors[buttonnumber, i]]--;
                if(aliveneighbors_count[neighbors[buttonnumber, i]] == 0)
                    hasaliveneighbors[neighbors[buttonnumber, i]] = false;
            }
        }
        ///<summary>
        ///inc stat_alivegradientsum[] and for all 8 nbors: nbgradsum[]++; hasAlv_nbors[] =true;
        ///</summary>
        private void NeighborGradientSumInc(int buttonnumber)
        {
            stat_alivegradient_sum++;
                for(int i = 0; i<8; i++)
                {
                    hasaliveneighbors[neighbors[buttonnumber, i]] = true;
                    neighbors_gradient_sum[neighbors[buttonnumber, i]]++;
                
                }
        }
        ///<summary>
        /// <para>dec stat_alivegradientsum[] and for all 8 nbors: nbgradsum[nbor]--; </para>
        /// <para>if(ngradsum[] = 0)hasAlv_nbors[] =false;</para>
        ///</summary>
        private void NeighborGradientSumDec(int buttonnumber)
        {
            stat_alivegradient_sum--;
            for (int i = 0; i < 8; i++)
            {
                neighbors_gradient_sum[neighbors[buttonnumber, i]]--;
                if (neighbors_gradient_sum[neighbors[buttonnumber, i]] == 0)
                    hasaliveneighbors[neighbors[buttonnumber, i]] = false;
            }
        }

        private void BorderlessButton_Click_All(object sender, MouseEventArgs e)
        {
            BorderlessButton_Click_All_Method(sender);
        }

        private void BorderlessButton_Click_All_Method(object sender)
        {
            int bnumber = Convert.ToInt32(Regex.Replace((((BorderlessButton)sender).Name), "[^0-9.]", "")) - 1;

            if (version == 0) //Original Version
            {
                if (((BorderlessButton)sender).BackColor == dead_white)
                {
                    ((BorderlessButton)sender).BackColor = full_black;
                    bytegrid[bnumber] = 4;
                    IsAliveAndStatInc(bnumber);
                    AliveNeighborsIncBool(bnumber);

                }
                else
                {
                    ((BorderlessButton)sender).BackColor = dead_white;
                    bytegrid[bnumber] = 0;
                    AliveNeighborsDecBool(bnumber);
                    IsNotAliveAndStatDec(bnumber);
                }
            }
            else if (((BorderlessButton)sender).BackColor == dead_white) //Modified
            {
                ((BorderlessButton)sender).BackColor = mygrey1;
                bytegrid[bnumber] = 1;
                ModifiedButtonIncGeneralized(bnumber);
            }
            else if (((BorderlessButton)sender).BackColor == mygrey1)
            {
                ((BorderlessButton)sender).BackColor = mygrey2;
                bytegrid[bnumber] = 2;

                ModifiedButtonIncGeneralized(bnumber);

            }
            else if (((BorderlessButton)sender).BackColor == mygrey2)
            {
                ((BorderlessButton)sender).BackColor = mygrey3;
                bytegrid[bnumber] = 3;
                DesignerNeighborsInc(bnumber);
                ModifiedButtonIncGeneralized(bnumber);

            }
            else if (((BorderlessButton)sender).BackColor == mygrey3)
            {
                ((BorderlessButton)sender).BackColor = full_black;
                bytegrid[bnumber] = 4;
                DesignerNeighborsInc(bnumber);
                ModifiedButtonIncGeneralized(bnumber);

            }
            else if (((BorderlessButton)sender).BackColor == full_black)
            {
                ((BorderlessButton)sender).BackColor = dead_white;
                bytegrid[bnumber] = 0;
                DesignerNeighborsDec(bnumber);
                AliveNeighborsDecBool(bnumber);
                IsNotAliveAndStatDec(bnumber);
                for (int i = 0; i < 4; i++)
                    NeighborGradientSumDec(bnumber);
            }
        }
        /// <summary>
        /// Should be called if a button/grid increases its value. Doesnt Handle Designerpart
        /// 
        /// <para>Shouldnt be called by purely setting functions, like Random() and RandomSymm()</para>
        /// </summary>
       
        private void ModifiedButtonIncGeneralized(int bnumber)
        {
            if (!isalive[bnumber])
            {
                IsAliveAndStatInc(bnumber);
                AliveNeighborsIncBool(bnumber);
            }
            NeighborGradientSumInc(bnumber);
            

        }

        ///<summary>
        ///Classic Version: If bytegrid_haschanged[] : black or white, depending on isalive[]
        ///</summary>
        public void BytegridToColorReduced()
        {
            for (int i = 0; i < 900; i++)
            {
                if (bytegrid_haschanged[i])
                {
                    if (!isalive[i])
                        colorgrid[i] = dead_white;

                    if (isalive[i])
                        colorgrid[i] = full_black;
                }
            }
        }
        ///<summary>
        ///<para>Handles Classic Version Color Update</para>
        ///<para>BgridToColorReduced(); If bgrid_haschanged[] Update Backcolor</para>
        ///</summary>
        private void ReducedUpdateColorAll()
        {
            BytegridToColorReduced();

            for (int i = 0; i < 900; i++)
            {
                if(bytegrid_haschanged[i])
                    buttons[i].BackColor = colorgrid[i];
            }
        }

        /// <summary>
        /// if (btgrid_haschanged[]) translates bytegrid to colorgrid
        /// </summary>
        public void BytegridToColor()
        {
            for (int i = 0; i < 900; i++)
            {
                if (bytegrid_haschanged[i])
                {
                    if (bytegrid[i] == 0)
                        colorgrid[i] = dead_white;
                    else if (bytegrid[i] == 1)
                        colorgrid[i] = mygrey1;
                    else if (bytegrid[i] == 2)
                        colorgrid[i] = mygrey2;
                    else if (bytegrid[i] == 3)
                        colorgrid[i] = mygrey3;
                    else
                        colorgrid[i] = full_black;
                }
            }
        }
        /// <summary>
        /// <para>Handles Designer View: if dsgn_bgrid_hscnged[] or bgid_hscnged[]</para>
        /// <para>colorgrid[] = bytegrid[] + designer_bytegrid_plus[]</para>
        /// </summary>
        public void BytegridToColorDesigner()
        {
            
            for (int i = 0; i < 900; i++)
            {
                if (designer_bytegrid_haschanged[i] || bytegrid_haschanged[i])
                {
                   
                    if (bytegrid[i] + designer_bytegrid_plus[i] >= 4)
                        colorgrid[i] = full_black;
                    else if (bytegrid[i] + designer_bytegrid_plus[i] == 0)
                        colorgrid[i] = dead_white;
                    else if (bytegrid[i] + designer_bytegrid_plus[i] == 1)
                        colorgrid[i] = mygrey1;
                    else if (bytegrid[i] + designer_bytegrid_plus[i] == 2)
                        colorgrid[i] = mygrey2;
                    else if (bytegrid[i] + designer_bytegrid_plus[i] == 3)
                        colorgrid[i] = mygrey3;
                   
                }
            }
           
        }
        /// <summary>
        /// BgridToClr(); for all 900 buttons where bgrid_haschanged[]: Updates color from Colorgrid
        /// </summary>
        private void UpdateColorAll()
        {
            BytegridToColor();

            for (int i = 0; i < 900; i++)
            {
                if (bytegrid_haschanged[i])
                    buttons[i].BackColor = colorgrid[i];
            }
        }
        /// <summary>
        /// BgridToClrDesigner(); for all 900 buttons where dsgn-/ ||_bgrid_hscnged[]: Updates buttons from Colorgrid
        /// </summary>
        private void UpdateColorAllDesigner()
        {
            BytegridToColorDesigner();

            for (int i = 0; i < 900; i++)
            {
                if (designer_bytegrid_haschanged[i] || bytegrid_haschanged[i] )
                    buttons[i].BackColor = colorgrid[i];
            }
        }
        /// <summary>
        /// Designer: For all nbors: dsgn_bgrid_hscnged[nbor] = true; dsng_brig_plus[nbor]++
        /// </summary>
        /// <param name="bnumber">number of relevant button</param>
        private void DesignerNeighborsInc(int bnumber)
        {
            
            for (int i = 0; i < 8; i++)
            {
                int n_bors = neighbors[bnumber, i];
                designer_bytegrid_haschanged[n_bors] = true;
                designer_bytegrid_plus[n_bors]++;
            }
            
        }
        /// <summary>
         /// Designer: For all nbors: dsgn_bgrid_hscnged[nbor] = true; dsng_brig_plus[nbor]--
         /// </summary>
         /// <param name="bnumber">number of relevant button</param>
        private void DesignerNeighborsDec(int bnumber)
        {
            
            for (int i = 0; i < 8; i++)
            {
                int n_bors = neighbors[bnumber, i];
                designer_bytegrid_haschanged[n_bors] = true;
                designer_bytegrid_plus[n_bors]--;
            }
            
        }
        /// <summary>
        /// <para>for all 900: bytegrid_new[] in rel. to  bgrid[].  sets bgrid_haschanged[] accordingly</para>
        /// <para>changes stat_alivecount accordingly and AlivenborsInc/DecBool(bnum)</para>
        /// </summary>
        private void ReducedBytegridChangeAction()
        {
            for(int i = 0; i<900; i++)
            {
                if(bytegrid[i] > bytegrid_new[i])
                {
                    bytegrid_haschanged[i] = true;
                  
                    AliveNeighborsDecBool(i);
                    stat_alivecount--;

                }
                else if(bytegrid[i] < bytegrid_new[i])
                {
                    bytegrid_haschanged[i] = true;
                    AliveNeighborsIncBool(i);
                    stat_alivecount++;

                }
                
                else bytegrid_haschanged[i] = false;
            }
        }
        /// <summary>
        /// <para>for all 900: bytegrid_new[] in rel. to  bgrid[].  sets bgrid_haschanged[] accordingly</para>
        /// <para>changes isalive[] and NeighborGradientSumInc/Dec(bnum). Also DesignerNborInc/Dec(bnum)</para>
        /// </summary>
        private void BytegridChangeAction()
        {
            for (int i = 0; i < 900; i++)
            {
                if (bytegrid_new[i] < bytegrid[i])
                {
                    bytegrid_haschanged[i] = true;
                    NeighborGradientSumDec(i);

                    if (bytegrid_new[i] == 0)
                    {
                        isalive[i] = false;
                    }
                    if (bytegrid_new[i] == 2)
                        DesignerNeighborsDec(i);
                    

                }
                else if (bytegrid_new[i] > bytegrid[i])
                {
                    bytegrid_haschanged[i] = true;
                    NeighborGradientSumInc(i);
                    isalive[i] = true;

                    if(bytegrid_new[i] == 3)
                        DesignerNeighborsInc(i);
                    
                }
                else
                {
                  
                    bytegrid_haschanged[i] = false;
                }
            }
        }
        /// <summary>
        /// calls classic: TimerOnTick(); Mod: ModifiedTimerOnTick()
        ///<para>calls Set RuleSet </para>
        ///<para>IncGencount every call</para>
        /// </summary>
        private void Timer1_Tick(object sender, EventArgs e)
        {
            tickdelegate(sender, e);
            
            gamemode();
            
            generation_count++;
            Generation_Counter_label.Text = generation_count.ToString();
           
        }
        /// <summary>
        /// Pause the game/ Timer(s) and change according displays. Sets isrunning
        /// </summary>
        private void PauseGame()
        {
                EnableAllBorderlessButtons();
                StartPause_button.Text = "Start";
                isrunning = false;
                generationTimEr.Stop();

                TimeSpan diff = DateTime.Now - time_start;

                ControlTextBox.Text = "Vergangene Zeit: " + diff.TotalSeconds.ToString();
        }
        private void StartGame()
        {
            DisableAllBorderlessButtons();
            isrunning = true;
            StartPause_button.Text = "Pause";
            ControlTextBox.Text = "Gestartet";
            for(int i = 0; i<900; i++)
            {
                if (bytegrid[i] != 0) gameexists = true;
            }
            
            time_start = DateTime.Now;
            generationTimEr.Start();
        }

        private void Button1_Click(object sender, EventArgs e) // Start/Pause Button
        {
            if (!isrunning)
            {
                
                for (int i = 0; i < 900; i++)
                {
                    bytegrid_new[i] = bytegrid[i];
                }
                if (gameexists && !game_has_started)
                {
                    
                    game_has_started = true;
                    gamestate = GetGameState();
                }
                StartGame();

            }
            else
            {
                PauseGame();
            }
        }

        private void Randomize_Click(object sender, EventArgs e)
        {
            if (gameexists && !ResetGame())
                return;
            DisableAllBorderlessButtons();
            RandomizeButtons();
            gameexists = true;
            EnableAllBorderlessButtons();
            game_has_started = false;
        }

        private void DisableAllBorderlessButtons()
        {
            foreach (BorderlessButton button in buttons)
            {
                button.Enabled = false;
            }
        }
        /// <summary>
        /// Pauses and Resets the current game. Doesnt Change gamemode. Returns true if user accepted the reset, false if not
        /// </summary>
        /// <returns></returns>
        private bool ResetGame()
        {
            DisableAllBorderlessButtons();
            PauseGame();


            DialogResult yesorno = MessageBox.Show("Das bestehende Spiel wird unwiederruflich gelöscht.\nMöchten Sie wirklich ein neues Spiel beginnen?", "Neues Spiel starten", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (yesorno == DialogResult.Yes)
            {

                
                foreach (BorderlessButton button in buttons)
                {

                    button.BackColor = dead_white;

                }
                isrunning = false;
                gameexists = false;
                Generation_Counter_label.Text = "0";
                generation_count = 0;
                stat_alivecount = 0;
                stat_alivegradient_sum = 0;
              
                for (int i = 0; i < 900; i++)
                {
                    bytegrid[i] = 0;
                    bytegrid_new[i] = 0;
                    bytegrid_haschanged[i] = false;
                    hasaliveneighbors[i] = false;
                    designer_bytegrid_haschanged[i] = false;
                    designer_bytegrid_plus[i] = 0;
                    colorgrid[i] = dead_white;
                    aliveneighbors_count[i] = 0;
                    isalive[i] = false;
                    neighbors_gradient_sum[i] = 0;
                }
                EnableAllBorderlessButtons();
                return true;
            }

            EnableAllBorderlessButtons();
            return false;
        }

        private void EnableAllBorderlessButtons()
        {
            foreach (BorderlessButton button in buttons)
            {
                button.Enabled = true;
            }
        }

        private void Tick_Click(object sender, EventArgs e) //Manual Click
        {
            if (!gameexists)
            {
                for (int i = 0; i < 900; i++)
                {
                    if (bytegrid[i] != 0) gameexists = true;
                }

                
            }
            for (int i = 0; i < 900; i++)
            {
                bytegrid_new[i] = bytegrid[i];
            }
            if (gameexists && !game_has_started)
            {
                game_has_started = true;
                gamestate = GetGameState();
                
            }
            
            Timer1_Tick(sender, e);
            
        }

        

        

        private void ModifiedGameSettingsChange()
        {
            version = 1;
            tickdelegate = ModifiedTimerOnTick;
        }
        /// <summary>
        /// Randomizes all Buttons according to current gamemode. Doesnt Ask. Has to be handled in the method calling
        /// </summary>
        private void RandomizeButtons()
        {

            Random rnd = new Random();

         

            if (version == 1)
            {
                for (int i = 0; i < 900; i++)
                {
                    switch (rnd.Next(0, 5))
                    {
                        case 0:
                            buttons[i].BackColor = dead_white;
                            bytegrid[i] = 0;
                            isalive[i] = false;

                            break;
                        case 1:
                            buttons[i].BackColor = mygrey1;
                            bytegrid[i] = 1;
                            AliveNeighborsIncBool(i);
                            IsAliveAndStatInc(i);

                            NeighborGradientSumInc(i);


                            break;
                        case 2:
                            buttons[i].BackColor = mygrey2;
                            bytegrid[i] = 2;
                            AliveNeighborsIncBool(i);
                            IsAliveAndStatInc(i);

                            for (int j = 0; j < 2; j++)
                            {
                                NeighborGradientSumInc(i);
                            }
                            break;
                        case 3:
                            buttons[i].BackColor = mygrey3;
                            bytegrid[i] = 3;
                            AliveNeighborsIncBool(i);
                            IsAliveAndStatInc(i);
                            DesignerNeighborsInc(i);
                            for (int j = 0; j < 3; j++)
                            {
                                NeighborGradientSumInc(i);
                            }
                            break;
                        case 4:
                            buttons[i].BackColor = full_black;
                            bytegrid[i] = 4;
                            AliveNeighborsIncBool(i);
                            IsAliveAndStatInc(i);
                            DesignerNeighborsInc(i);

                            for (int j = 0; j < 4; j++)
                            {
                                NeighborGradientSumInc(i);
                            }
                            break;

                        default:
                            break;
                    }

                }
            }
            else if (version == 0)
            {
                for (int i = 0; i < 900; i++)
                {
                    switch (rnd.Next(0, 2))
                    {
                        case 0:
                            buttons[i].BackColor = dead_white;
                            bytegrid[i] = 0;
                            isalive[i] = false;
                            break;
                        case 1:
                            buttons[i].BackColor = full_black;
                            bytegrid[i] = 4;
                            AliveNeighborsIncBool(i);
                            IsAliveAndStatInc(i);
                            break;
                        default: break;
                    }
                }
            }



        }
        /// <summary>
        /// Generates Random Symmetric (along y Axis). Doesnt ask for permission. Had to be handled by calling f 
        /// </summary>
        private void GenerateSymmetricModified()
        {
            Random rnd = new Random();
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    int n1 = 30 * j + i;
                    int n2 = 30 * j + 29 - i;
                    if (version == 1) {
                        switch (rnd.Next(0, 5))
                        {
                            case 0:
                                buttons[n1].BackColor = dead_white;
                                bytegrid[n1] = 0;
                                isalive[n1] = false;

                                buttons[n2].BackColor = dead_white;
                                bytegrid[n2] = 0;
                                isalive[n2] = false;

                                break;
                            case 1:
                                buttons[n1].BackColor = mygrey1;
                                bytegrid[n1] = 1;
                                AliveNeighborsIncBool(n1);
                                IsAliveAndStatInc(n1);
                                NeighborGradientSumInc(n1);

                                buttons[n2].BackColor = mygrey1;
                                bytegrid[n2] = 1;
                                AliveNeighborsIncBool(n2);
                                IsAliveAndStatInc(n2);
                                NeighborGradientSumInc(n2);


                                break;
                            case 2:
                                buttons[n1].BackColor = mygrey2;
                                bytegrid[n1] = 2;
                                AliveNeighborsIncBool(n1);
                                IsAliveAndStatInc(n1);
                                for (int k = 0; k < 2; k++)
                                {
                                    NeighborGradientSumInc(n1);
                                }

                                buttons[n2].BackColor = mygrey2;
                                bytegrid[n2] = 2;
                                AliveNeighborsIncBool(n2);
                                IsAliveAndStatInc(n2);
                                for (int k = 0; k < 2; k++)
                                {
                                    NeighborGradientSumInc(n2);
                                }
                                break;
                            case 3:
                                buttons[n1].BackColor = mygrey3;
                                bytegrid[n1] = 3;
                                AliveNeighborsIncBool(n1);
                                IsAliveAndStatInc(n1);
                                DesignerNeighborsInc(n1); //Temp if u want
                                for (int k = 0; k < 3; k++)
                                {
                                    NeighborGradientSumInc(n1);
                                }

                                buttons[n2].BackColor = mygrey3;
                                bytegrid[n2] = 3;
                                AliveNeighborsIncBool(n2);
                                IsAliveAndStatInc(n2);
                                DesignerNeighborsInc(n2); //same
                                for (int k = 0; k < 3; k++)
                                {
                                    NeighborGradientSumInc(n2);
                                }
                                break;
                            case 4:
                                buttons[n1].BackColor = full_black;
                                bytegrid[n1] = 4;
                                DesignerNeighborsInc(n1);
                                AliveNeighborsIncBool(n1);
                                IsAliveAndStatInc(n1);
                                for (int k = 0; k < 4; k++)
                                {
                                    NeighborGradientSumInc(n1);
                                }

                                buttons[n2].BackColor = full_black;
                                bytegrid[n2] = 4;
                                DesignerNeighborsInc(n2);
                                AliveNeighborsIncBool(n2);
                                IsAliveAndStatInc(n2);
                                for (int k = 0; k < 4; k++)
                                {
                                    NeighborGradientSumInc(n2);
                                }
                                break;

                            default:
                                break;
                        }
                    }
                    else if (version == 0)
                    {
                        switch (rnd.Next(0, 2))
                        {
                            case 0:
                                buttons[n1].BackColor = dead_white;
                                bytegrid[n1] = 0;
                                isalive[n1] = false;

                                buttons[n2].BackColor = dead_white;
                                bytegrid[n2] = 0;
                                isalive[n2] = false;

                                break;
                            case 1:
                                buttons[n1].BackColor = full_black;
                                bytegrid[n1] = 4;
                                AliveNeighborsIncBool(n1);
                                IsAliveAndStatInc(n1);
                                NeighborGradientSumInc(n1);

                                buttons[n2].BackColor = full_black;
                                bytegrid[n2] = 4;
                                AliveNeighborsIncBool(n2);
                                IsAliveAndStatInc(n2);
                                NeighborGradientSumInc(n2);
                                break;
                           
                        }
                    }
                }

            }
        }


        
        private void Random_Symm_button_Click(object sender, EventArgs e)
        {

            if (gameexists && !ResetGame())
                return;

            GenerateSymmetricModified();
            gameexists = true;
            game_has_started = false;
        }

        private void FloaterToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void DesignerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PauseGame();

            designer_delegate = UpdateColorAllDesigner;
            for (int i = 0; i < 900; i++)
            {
                buttons[i].BackColor = ByteToColor(bytegrid[i]);
            }
            StartGame();
        }

        private void GenerateSymmetricSmallXY()
        {
            byte range = 3;
            Random rnd = new Random();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    int n1 = 435 - (30 + 1) * range + 30 * j + i + 1 + 30;
                    int n2 = 436 - (30 - 1) * range + 30 * j - i - 1 + 30;
                    int n3 = 465 + (30 - 1) * range - 30 * j + i + 1 - 30;
                    int n4 = 466 + (30 + 1) * range - 30 * j - i - 1 - 30;
                    if (version == 1)
                    {
                        switch (rnd.Next(0, 5))
                        {
                            case 0:
                                buttons[n1].BackColor = dead_white;
                                bytegrid[n1] = 0;
                                isalive[n1] = false;

                                buttons[n2].BackColor = dead_white;
                                bytegrid[n2] = 0;
                                isalive[n2] = false;

                                buttons[n3].BackColor = dead_white;
                                bytegrid[n3] = 0;
                                isalive[n3] = false;

                                buttons[n4].BackColor = dead_white;
                                bytegrid[n4] = 0;
                                isalive[n4] = false;

                                break;
                            case 1:
                                buttons[n1].BackColor = mygrey1;
                                bytegrid[n1] = 1;
                                AliveNeighborsIncBool(n1);
                                IsAliveAndStatInc(n1);
                                NeighborGradientSumInc(n1);

                                buttons[n2].BackColor = mygrey1;
                                bytegrid[n2] = 1;
                                AliveNeighborsIncBool(n2);
                                IsAliveAndStatInc(n2);
                                NeighborGradientSumInc(n2);

                                buttons[n3].BackColor = mygrey1;
                                bytegrid[n3] = 1;
                                AliveNeighborsIncBool(n3);
                                IsAliveAndStatInc(n3);
                                NeighborGradientSumInc(n3);

                                buttons[n4].BackColor = mygrey1;
                                bytegrid[n4] = 1;
                                AliveNeighborsIncBool(n4);
                                IsAliveAndStatInc(n4);
                                NeighborGradientSumInc(n4);

                                break;
                            case 2:
                                buttons[n1].BackColor = mygrey2;
                                bytegrid[n1] = 2;
                                AliveNeighborsIncBool(n1);
                                IsAliveAndStatInc(n1);
                                for (int k = 0; k < 2; k++)
                                {
                                    NeighborGradientSumInc(n1);
                                }

                                buttons[n2].BackColor = mygrey2;
                                bytegrid[n2] = 2;
                                AliveNeighborsIncBool(n2);
                                IsAliveAndStatInc(n2);
                                for (int k = 0; k < 2; k++)
                                {
                                    NeighborGradientSumInc(n2);
                                }

                                buttons[n3].BackColor = mygrey2;
                                bytegrid[n3] = 2;
                                AliveNeighborsIncBool(n3);
                                IsAliveAndStatInc(n3);
                                for (int k = 0; k < 2; k++)
                                {
                                    NeighborGradientSumInc(n3);
                                }

                                buttons[n4].BackColor = mygrey2;
                                bytegrid[n4] = 2;
                                AliveNeighborsIncBool(n4);
                                IsAliveAndStatInc(n4);
                                for (int k = 0; k < 2; k++)
                                {
                                    NeighborGradientSumInc(n4);
                                }
                                break;
                            case 3:
                                buttons[n1].BackColor = mygrey3;
                                bytegrid[n1] = 3;
                                AliveNeighborsIncBool(n1);
                                IsAliveAndStatInc(n1);
                                DesignerNeighborsInc(n1); //Temp if u want
                                for (int k = 0; k < 3; k++)
                                {
                                    NeighborGradientSumInc(n1);
                                }

                                buttons[n2].BackColor = mygrey3;
                                bytegrid[n2] = 3;
                                AliveNeighborsIncBool(n2);
                                IsAliveAndStatInc(n2);
                                DesignerNeighborsInc(n2); //same
                                for (int k = 0; k < 3; k++)
                                {
                                    NeighborGradientSumInc(n2);
                                }

                                buttons[n3].BackColor = mygrey3;
                                bytegrid[n3] = 3;
                                AliveNeighborsIncBool(n3);
                                IsAliveAndStatInc(n3);
                                DesignerNeighborsInc(n3); //Temp if u want
                                for (int k = 0; k < 3; k++)
                                {
                                    NeighborGradientSumInc(n3);
                                }

                                buttons[n4].BackColor = mygrey3;
                                bytegrid[n4] = 3;
                                AliveNeighborsIncBool(n4);
                                IsAliveAndStatInc(n4);
                                DesignerNeighborsInc(n4); //same
                                for (int k = 0; k < 3; k++)
                                {
                                    NeighborGradientSumInc(n4);
                                }
                                break;
                            case 4:
                                buttons[n1].BackColor = full_black;
                                bytegrid[n1] = 4;
                                DesignerNeighborsInc(n1);
                                AliveNeighborsIncBool(n1);
                                IsAliveAndStatInc(n1);
                                for (int k = 0; k < 4; k++)
                                {
                                    NeighborGradientSumInc(n1);
                                }

                                buttons[n2].BackColor = full_black;
                                bytegrid[n2] = 4;
                                DesignerNeighborsInc(n2);
                                AliveNeighborsIncBool(n2);
                                IsAliveAndStatInc(n2);
                                for (int k = 0; k < 4; k++)
                                {
                                    NeighborGradientSumInc(n2);
                                }

                                buttons[n3].BackColor = full_black;
                                bytegrid[n3] = 4;
                                DesignerNeighborsInc(n3);
                                AliveNeighborsIncBool(n3);
                                IsAliveAndStatInc(n3);
                                for (int k = 0; k < 4; k++)
                                {
                                    NeighborGradientSumInc(n3);
                                }

                                buttons[n4].BackColor = full_black;
                                bytegrid[n4] = 4;
                                DesignerNeighborsInc(n4);
                                AliveNeighborsIncBool(n4);
                                IsAliveAndStatInc(n4);
                                for (int k = 0; k < 4; k++)
                                {
                                    NeighborGradientSumInc(n4);
                                }
                                break;

                            default:
                                break;
                        }
                    }
                    else if (version == 0)
                    {
                        switch (rnd.Next(0, 2))
                        {
                            case 0:
                                buttons[n1].BackColor = dead_white;
                                bytegrid[n1] = 0;
                                isalive[n1] = false;

                                buttons[n2].BackColor = dead_white;
                                bytegrid[n2] = 0;
                                isalive[n2] = false;

                                buttons[n3].BackColor = dead_white;
                                bytegrid[n3] = 0;
                                isalive[n3] = false;

                                buttons[n4].BackColor = dead_white;
                                bytegrid[n4] = 0;
                                isalive[n4] = false;

                                break;
                            case 1:
                                buttons[n1].BackColor = full_black;
                                bytegrid[n1] = 4;
                                AliveNeighborsIncBool(n1);
                                IsAliveAndStatInc(n1);
                                NeighborGradientSumInc(n1);

                                buttons[n2].BackColor = full_black;
                                bytegrid[n2] = 4;
                                AliveNeighborsIncBool(n2);
                                IsAliveAndStatInc(n2);
                                NeighborGradientSumInc(n2);

                                buttons[n3].BackColor = full_black;
                                bytegrid[n3] = 4;
                                AliveNeighborsIncBool(n3);
                                IsAliveAndStatInc(n3);
                                NeighborGradientSumInc(n3);

                                buttons[n4].BackColor = full_black;
                                bytegrid[n4] = 4;
                                AliveNeighborsIncBool(n4);
                                IsAliveAndStatInc(n4);
                                NeighborGradientSumInc(n4);
                                break;

                        }
                    }
                }

            }
        }
        private void Randomxysymmsmall_Click(object sender, EventArgs e)
        {
            if (gameexists && !ResetGame())
                return;
            GenerateSymmetricSmallXY();
            gameexists = true;
            game_has_started = false;
        }

        private void GenerateSymmetricXY()
        {
            Random rnd = new Random();
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    int n1 = 30 * j + i;
                    int n2 = 30 * j + 29 - i;
                    int n3 = -30 * j + 870 + i;
                    int n4 = -30 * j + 870 + 29 - i;
                    if (version == 1)
                    {
                        switch (rnd.Next(0, 5))
                        {
                            case 0:
                                buttons[n1].BackColor = dead_white;
                                bytegrid[n1] = 0;
                                isalive[n1] = false;

                                buttons[n2].BackColor = dead_white;
                                bytegrid[n2] = 0;
                                isalive[n2] = false;

                                buttons[n3].BackColor = dead_white;
                                bytegrid[n3] = 0;
                                isalive[n3] = false;

                                buttons[n4].BackColor = dead_white;
                                bytegrid[n4] = 0;
                                isalive[n4] = false;

                                break;
                            case 1:
                                buttons[n1].BackColor = mygrey1;
                                bytegrid[n1] = 1;
                                AliveNeighborsIncBool(n1);
                                IsAliveAndStatInc(n1);
                                NeighborGradientSumInc(n1);

                                buttons[n2].BackColor = mygrey1;
                                bytegrid[n2] = 1;
                                AliveNeighborsIncBool(n2);
                                IsAliveAndStatInc(n2);
                                NeighborGradientSumInc(n2);

                                buttons[n3].BackColor = mygrey1;
                                bytegrid[n3] = 1;
                                AliveNeighborsIncBool(n3);
                                IsAliveAndStatInc(n3);
                                NeighborGradientSumInc(n3);

                                buttons[n4].BackColor = mygrey1;
                                bytegrid[n4] = 1;
                                AliveNeighborsIncBool(n4);
                                IsAliveAndStatInc(n4);
                                NeighborGradientSumInc(n4);

                                break;
                            case 2:
                                buttons[n1].BackColor = mygrey2;
                                bytegrid[n1] = 2;
                                AliveNeighborsIncBool(n1);
                                IsAliveAndStatInc(n1);
                                for (int k = 0; k < 2; k++)
                                {
                                    NeighborGradientSumInc(n1);
                                }

                                buttons[n2].BackColor = mygrey2;
                                bytegrid[n2] = 2;
                                AliveNeighborsIncBool(n2);
                                IsAliveAndStatInc(n2);
                                for (int k = 0; k < 2; k++)
                                {
                                    NeighborGradientSumInc(n2);
                                }

                                buttons[n3].BackColor = mygrey2;
                                bytegrid[n3] = 2;
                                AliveNeighborsIncBool(n3);
                                IsAliveAndStatInc(n3);
                                for (int k = 0; k < 2; k++)
                                {
                                    NeighborGradientSumInc(n3);
                                }

                                buttons[n4].BackColor = mygrey2;
                                bytegrid[n4] = 2;
                                AliveNeighborsIncBool(n4);
                                IsAliveAndStatInc(n4);
                                for (int k = 0; k < 2; k++)
                                {
                                    NeighborGradientSumInc(n4);
                                }
                                break;
                            case 3:
                                buttons[n1].BackColor = mygrey3;
                                bytegrid[n1] = 3;
                                AliveNeighborsIncBool(n1);
                                IsAliveAndStatInc(n1);
                                DesignerNeighborsInc(n1); //Temp if u want
                                for (int k = 0; k < 3; k++)
                                {
                                    NeighborGradientSumInc(n1);
                                }

                                buttons[n2].BackColor = mygrey3;
                                bytegrid[n2] = 3;
                                AliveNeighborsIncBool(n2);
                                IsAliveAndStatInc(n2);
                                DesignerNeighborsInc(n2); //same
                                for (int k = 0; k < 3; k++)
                                {
                                    NeighborGradientSumInc(n2);
                                }

                                buttons[n3].BackColor = mygrey3;
                                bytegrid[n3] = 3;
                                AliveNeighborsIncBool(n3);
                                IsAliveAndStatInc(n3);
                                DesignerNeighborsInc(n3); //Temp if u want
                                for (int k = 0; k < 3; k++)
                                {
                                    NeighborGradientSumInc(n3);
                                }

                                buttons[n4].BackColor = mygrey3;
                                bytegrid[n4] = 3;
                                AliveNeighborsIncBool(n4);
                                IsAliveAndStatInc(n4);
                                DesignerNeighborsInc(n4); //same
                                for (int k = 0; k < 3; k++)
                                {
                                    NeighborGradientSumInc(n4);
                                }
                                break;
                            case 4:
                                buttons[n1].BackColor = full_black;
                                bytegrid[n1] = 4;
                                DesignerNeighborsInc(n1);
                                AliveNeighborsIncBool(n1);
                                IsAliveAndStatInc(n1);
                                for (int k = 0; k < 4; k++)
                                {
                                    NeighborGradientSumInc(n1);
                                }

                                buttons[n2].BackColor = full_black;
                                bytegrid[n2] = 4;
                                DesignerNeighborsInc(n2);
                                AliveNeighborsIncBool(n2);
                                IsAliveAndStatInc(n2);
                                for (int k = 0; k < 4; k++)
                                {
                                    NeighborGradientSumInc(n2);
                                }

                                buttons[n3].BackColor = full_black;
                                bytegrid[n3] = 4;
                                DesignerNeighborsInc(n3);
                                AliveNeighborsIncBool(n3);
                                IsAliveAndStatInc(n3);
                                for (int k = 0; k < 4; k++)
                                {
                                    NeighborGradientSumInc(n3);
                                }

                                buttons[n4].BackColor = full_black;
                                bytegrid[n4] = 4;
                                DesignerNeighborsInc(n4);
                                AliveNeighborsIncBool(n4);
                                IsAliveAndStatInc(n4);
                                for (int k = 0; k < 4; k++)
                                {
                                    NeighborGradientSumInc(n4);
                                }
                                break;

                            default:
                                break;
                        }
                    }
                    else if (version == 0)
                    {
                        switch (rnd.Next(0, 2))
                        {
                            case 0:
                                buttons[n1].BackColor = dead_white;
                                bytegrid[n1] = 0;
                                isalive[n1] = false;

                                buttons[n2].BackColor = dead_white;
                                bytegrid[n2] = 0;
                                isalive[n2] = false;

                                buttons[n3].BackColor = dead_white;
                                bytegrid[n3] = 0;
                                isalive[n3] = false;

                                buttons[n4].BackColor = dead_white;
                                bytegrid[n4] = 0;
                                isalive[n4] = false;

                                break;
                            case 1:
                                buttons[n1].BackColor = full_black;
                                bytegrid[n1] = 4;
                                AliveNeighborsIncBool(n1);
                                IsAliveAndStatInc(n1);
                                NeighborGradientSumInc(n1);

                                buttons[n2].BackColor = full_black;
                                bytegrid[n2] = 4;
                                AliveNeighborsIncBool(n2);
                                IsAliveAndStatInc(n2);
                                NeighborGradientSumInc(n2);

                                buttons[n3].BackColor = full_black;
                                bytegrid[n3] = 4;
                                AliveNeighborsIncBool(n3);
                                IsAliveAndStatInc(n3);
                                NeighborGradientSumInc(n3);

                                buttons[n4].BackColor = full_black;
                                bytegrid[n4] = 4;
                                AliveNeighborsIncBool(n4);
                                IsAliveAndStatInc(n4);
                                NeighborGradientSumInc(n4);
                                break;

                        }
                    }
                }

            }
        }

        private void Randomize_xy_Symm_button_Click(object sender, EventArgs e)
        {
            if (gameexists && !ResetGame())
                return;
            GenerateSymmetricXY();
            gameexists = true;
            game_has_started = false;
        }

        

        private void WasIstDasGameOfLifeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Das Spiel des Lebens(engl.Conway’s Game of Life) ist ein vom Mathematiker John Horton Conway (gestorben 11.04.2020) 1970 entworfenes Spiel, basierend auf einem zweidimensionalen zellulären Automaten.Es ist eine einfache und bis heute populäre Umsetzung der Automaten - Theorie von Stanisław Marcin Ulam. ", "Was ist Game of Life?", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void anfangszustandWiederherstellenToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (gameexists)
            {
                if (!ResetGame())
                    return;
                gameexists = true;
                DisableAllBorderlessButtons();
                SetGameGameState(gamestate);
                for (int i = 0; i < 900; i++)
                {
                    buttons[i].BackColor = ByteToColor(bytegrid[i]);
                }
                EnableAllBorderlessButtons();
            }

            else MessageBox.Show("Es wurde noch kein Spiel gestartet");
        }

        private void checkpunktSetzenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gamestate = GetGameState();
        }

        private void alsVorlageSpeichernToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ControlTextBox.Text = "";
            label1.Show();
            button_submit_template.Show();

            

            

        }

        private void button_submit_template_Click(object sender, EventArgs e)
        {
            
                string tmp_fileName = ControlTextBox.Text+ ".txt";
            string fullPath = Path.GetFullPath(tmp_fileName);
            string directoryName = Path.GetDirectoryName(fullPath);
            string vorlagenDirectoryName;
            string vorlagenDirectoryName_tmp = directoryName + "\\Vorlagen";
            if (version == 0)
                vorlagenDirectoryName = vorlagenDirectoryName_tmp + "\\Klassisch";
            else
                vorlagenDirectoryName = vorlagenDirectoryName_tmp + "\\Modifiziert";
            if (!Directory.Exists(vorlagenDirectoryName))
            {
                Directory.CreateDirectory(vorlagenDirectoryName);
            }
            string fileName = vorlagenDirectoryName + "\\" + tmp_fileName;
            if (File.Exists(fileName))
                MessageBox.Show("Fehler: Eine Vorlage mit diesem Namen besteht bereits");
            else
            {
                byte[] brid_and_info = new byte[910];
                for(int i = 0; i<900; i++)
                {
                    brid_and_info[i] = bytegrid[i];
                    
                    
                }
                if (version == 0)
                    brid_and_info[901] = 0;
                else if (version == 1)
                {
                    brid_and_info[901] = 1;
                    brid_and_info[902] = ModifiedGamemodeToByte(gamemode);
                }
                
                File.WriteAllBytes(fileName, brid_and_info);
                //Start
                    ToolStripMenuItem testToolStripMenuItem;
                    testToolStripMenuItem = new ToolStripMenuItem();
                if(version == 0)
                    klassischToolStripMenuItem2.DropDownItems.AddRange(new ToolStripItem[] {
                    testToolStripMenuItem
                    });
                else if(version == 1)
                    modifiziertToolStripMenuItem2.DropDownItems.AddRange(new ToolStripItem[] {
                    testToolStripMenuItem
                    });
                testToolStripMenuItem.Name = Path.GetFileNameWithoutExtension(fileName);
                testToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
                testToolStripMenuItem.Text = Path.GetFileNameWithoutExtension(fileName);
                testToolStripMenuItem.Click += new System.EventHandler(LoadVorlage_Click);

                

                MessageBox.Show("Vorlage " + Path.GetFileNameWithoutExtension(fileName) + " wurde erfolgreich gespeichert.");
            }

            label1.Hide();
            button_submit_template.Hide();
            ControlTextBox.Text = "";
        }

        private byte ModifiedGamemodeToByte(ProcDelegate gmode)
        {
            if (gmode == RuleSetModifiedShelter)
                return 1;
            if (gmode == RuleSetModifiedMain)
                return 2;
            if (gmode == RuleSetModifiedLowHunger)
                return 3;
            if (gmode == RuleSetModifiedHardToBurst)
                return 4;
            if (gmode == RuleSetModifiedHungry)
                return 5;
            else return 0;

        }
        private ConstantChangesTick ByteToDelegateChangeGamemode(byte gmode) //returns func with obj and eventargs
        {
            if (gmode == 0)
                return KlassischToolStripMenuItem_Click;
            if (gmode == 1)
                return ShelterToolStripMenuItem_Click;
            if (gmode == 2)
                return ModifiziertMainToolStripMenuItem_Click;
            if (gmode == 3)
                return LowHungerToolStripMenuItem_Click;
            if (gmode == 4)
                return ÜberpopulierenMöglichToolStripMenuItem_Click;
            if (gmode == 5)
                return HungryToolStripMenuItem_Click;
            else return KlassischToolStripMenuItem_Click;

        }

        
    }
}
