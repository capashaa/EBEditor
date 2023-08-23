using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PlayerIOClient;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using static World;
using Newtonsoft.Json.Linq;
using EEditor;
using System.Xml.Linq;

namespace EEditor
{
    public partial class MyWorlds : Form
    {
        private Semaphore s1 = new Semaphore(0, 1);
        private Semaphore s2 = new Semaphore(0, 1);
        private List<string> rooms = new List<string>();
        public static myWorlds myworlds = new myWorlds();
        public Frame MapFrame { get; private set; }
        public string selectedworld = null;
        public bool loaddb = false;
        private Client client_, cl;
        private int inccon = 0;
        public Dictionary<string, myWorlds> worlds = new Dictionary<string, myWorlds>();
        private ListViewColumnSorter listviewsorter;

        public MyWorlds()
        {
            InitializeComponent();
            listviewsorter = new ListViewColumnSorter();
            listView1.ListViewItemSorter = listviewsorter;
            listView1.Sort();
        }

        private void MyWorlds_Load(object sender, EventArgs e)
        {
            this.BackColor = MainForm.themecolors.background;
            this.ForeColor = MainForm.themecolors.foreground;
            listView1.ForeColor = MainForm.themecolors.foreground;
            listView1.BackColor = MainForm.themecolors.accent;
            panelBg.BackColor = MainForm.themecolors.accent;
            LoadWorldButton.ForeColor = MainForm.themecolors.foreground;
            LoadWorldButton.BackColor = MainForm.themecolors.accent;
            LoadWorldButton.FlatStyle = FlatStyle.Flat;
            ResetButton.ForeColor = MainForm.themecolors.foreground;
            ResetButton.BackColor = MainForm.themecolors.accent;
            ResetButton.FlatStyle = FlatStyle.Flat;
            listView1.Items.Clear();
            panelBg.AutoScroll = true;
            panelBg.Controls.Add(pictureBox1);
            rooms.Clear();
            loadWorlds(false);
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (((ListView)sender).SelectedIndices.Count == 1)
            {
                selectedworld = ((ListView)sender).SelectedItems[0].SubItems[2].Text;
                if (cbShowMinimap.Checked)
                {

                    PlayerIO.QuickConnect.SimpleConnect(bdata.gameID, MainForm.accs[MainForm.userdata.username].login, MainForm.accs[MainForm.userdata.username].password, null, (Client client) =>
                    {
                        executeMinimap(client, selectedworld);
                    },
                    (PlayerIOError error) =>
                    {
                        Errorhandler1(error);
                    });
                }
            }
        }

        private void executeMinimap(Client client, string world)
        {
            System.Threading.Thread runner = new System.Threading.Thread(delegate () { GetMinimap(client, world); });
            runner.Start();
        }
        private void Errorhandler1(PlayerIOError error)
        {

        }

        private Color UIntToColor(uint color)
        {
            byte a = (byte)(color >> 24);
            byte r = (byte)(color >> 16);
            byte g = (byte)(color >> 8);
            byte b = (byte)(color >> 0);
            return Color.FromArgb(a, r, g, b);
        }
        private void GetMinimap(Client client, string worldid)
        {
            if (cbDB.Checked)
            {
                var world = new World(InputType.BigDB, worldid, client);
                Bitmap fg = new Bitmap(world.Width, world.Height);
                Bitmap bg = new Bitmap(world.Width, world.Height);
                Bitmap bmp = new Bitmap(world.Width, world.Height);
                var value = world.Blocks.ToArray();

                foreach (var val in value)
                {
                    Color color = UIntToColor(Minimap.Colors[Convert.ToInt32(val.Type)]);
                    foreach (var vale in val.Locations)
                    {
                        if (val.Layer == 1 && Convert.ToInt32(val.Type) != 0)
                        {
                            bg.SetPixel(vale.X, vale.Y, color);
                        }
                        else
                        {
                            fg.SetPixel(vale.X, vale.Y, color);


                        }
                    }
                }
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    gr.Clear(world.BackgroundColor);
                    gr.DrawImage(bg, new Point(0, 0));
                    gr.DrawImage(fg, new Point(0, 0));
                }
                if (pictureBox1.InvokeRequired) this.Invoke((MethodInvoker)delegate
                {
                    pictureBox1.Width = world.Width;
                    pictureBox1.Height = world.Height;
                    pictureBox1.Image = bmp;
                });
            }
        }
        private void MyWorlds_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (worlds.Count() > 0) File.WriteAllText($"{Directory.GetCurrentDirectory()}\\{MainForm.userdata.username}.myworlds.json", JsonConvert.SerializeObject(worlds, Newtonsoft.Json.Formatting.Indented));
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            worlds.Clear();
            loadWorlds(true);
        }
        private void loadWorlds(bool reset)
        {
            int incr = 0, total = 0;
            listView1.Enabled = false;
            listView1.BeginUpdate();
            if (MainForm.userdata.username != "guest")
            {
            retry:
                if (File.Exists($"{Directory.GetCurrentDirectory()}\\{MainForm.userdata.username}.myworlds.json"))
                {
                    if (reset)
                    {
                        File.Delete($"{Directory.GetCurrentDirectory()}\\{MainForm.userdata.username}.myworlds.json");
                        goto retry;
                    }
                    var output = JObject.Parse(File.ReadAllText($"{Directory.GetCurrentDirectory()}\\{MainForm.userdata.username}.myworlds.json"));
                    total = output.Count;
                    foreach (var property in output)
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = property.Value["name"].ToString() == "" ? "Untitled World" : property.Value["name"].ToString();
                        lvi.SubItems.Add(property.Value["size"].ToString());
                        lvi.SubItems.Add(property.Key);
                        listView1.Items.Add(lvi);
                        progressBar1.Value = (incr * 100) / total;
                        incr++;
                    }
                    progressBar1.Value = 100;
                    listView1.Enabled = true;
                    listView1.EndUpdate();
                }
                else
                {
                    PlayerIO.QuickConnect.SimpleConnect(bdata.gameID, MainForm.accs[MainForm.userdata.username].login, MainForm.accs[MainForm.userdata.username].password, null, loginToWorlds, Errorhandler);

                }
            }
        }
        private void Errorhandler(PlayerIOError error)
        {
            MessageBox.Show($"Error: {error.Message}");
        }
        private void loginToWorlds(Client client)
        {
            client_ = client;
            inccon = 0;
            tryLobbyConnect(string.Format("{0}_{1}", client.ConnectUserId, RandomString(5)));
        }
        private void tryLobbyConnect(string id)
        {
            if (client_ != null)
            {
                int version = bdata.forceversion ? bdata.version : Convert.ToInt32(client_.BigDB.Load("config", "config")["version"]);
                client_.Multiplayer.CreateJoinRoom(id, $"Lobby{version}", true, null, null, lobbyConnected, (PlayerIOError error) => { tryLobbyConnect(id); });

            }
        }

        private void lobbyConnected(Connection con)
        {
            
            con.OnMessage += (s, m) =>
            {

                switch (m.Type)
                {
                    case "LobbyTo":
                        /*
                           Message 1 is the current id that get generated for you.
                           It will look like UserID_RandomString.
                           You need to join the lobby 1 more time with the generated id for you.
                           Before you can send connection messages to lobby.
                        */
                        tryLobbyConnect(m.GetString(0));
                        break;

                    //When connected to lobby you get this message.
                    
                    case "connectioncomplete":
                        if (inccon == 1) con.Send("getMySimplePlayerObject");
                        inccon += 1;
                        break;
                    case "getMySimplePlayerObject":
                        string owner = "Unknown";
                        int total = bdata.extractPlayerObjectsMessage(m) + 1;
                        owner = m[(UInt32)total].ToString();
                        total += 8 + 13;
                        if (!string.IsNullOrEmpty(m[(UInt32)total].ToString())) worlds.Add(m[(UInt32)total].ToString(), new myWorlds() { size = "world3x" });
                        total++;
                        if (!string.IsNullOrEmpty(m[(UInt32)total].ToString())) worlds.Add(m[(UInt32)total].ToString(), new myWorlds() { size = "world3x" });
                        total++;
                        total++;
                        if (m[(UInt32)total].ToString().Contains((char)0x1399) && m[(UInt32)total + 1].ToString().Contains((char)0x1399) && m[(UInt32)total + 2].ToString().Contains((char)0x1399))
                        {
                            string[] sizes = m[(UInt32)total].ToString().Split(new char[] { (char)0x1399 });
                            string[] worlds_ = m[(UInt32)total + 1].ToString().Split(new char[] { (char)0x1399 });
                            string[] title = m[(UInt32)total + 2].ToString().Split(new char[] { (char)0x1399 });
                            string title_ = "Untitled World";
                            for (int i = 0; i < worlds_.Length; i++)
                            {
                                if (!worlds.ContainsKey(worlds_[i]))
                                {
                                    if (string.IsNullOrEmpty(title[i])) title_ = "Untitled World";
                                    else title_ = title[i];
                                    worlds.Add(worlds_[i].ToString(), new myWorlds() { name = title_, size = sizes[i] });
                                }
                            }
                        }

                        else
                        {
                            if (m[(UInt32)total].ToString() != null) worlds.Add(m[(UInt32)total].ToString(), new myWorlds() { name = "Untitled World", size = "world3x" });
                        }
                        s1.Release();
                        LoadWorld();
                        break;
                }
            };
        }
        private void LoadWorld()
        {
            s1.WaitOne();
            int incr = 0;
            if (worlds.Count > 0)
            {
                int w = 200;
                int h = 200;
                foreach (KeyValuePair<string, myWorlds> kvp in worlds)
                {

                    if (kvp.Value.size.Contains("x"))
                    {
                        switch (kvp.Value.size.Split('x')[0])
                        {
                            case "world0":
                                w = 25;
                                h = 25;
                                break;
                            case "world1":
                                w = 50;
                                h = 50;
                                break;
                            case "world2":
                                w = 100;
                                h = 100;
                                break;
                            default:
                            case "world3":
                                w = 200;
                                h = 200;
                                break;
                            case "world4":
                                w = 400;
                                h = 50;
                                break;
                            case "world5":
                                w = 400;
                                h = 200;
                                break;
                            case "world6":
                                w = 100;
                                h = 400;
                                break;
                            case "world7":
                                w = 636;
                                h = 50;
                                break;
                            case "world8":
                                w = 110;
                                h = 110;
                                break;
                            case "world11":
                                w = 300;
                                h = 300;
                                break;
                            case "world12":
                                w = 250;
                                h = 150;
                                break;
                            case "world13":
                                w = 150;
                                h = 150;
                                break;
                        };
                    }
                    else
                    {
                        if (kvp.Value.size == "worldhome")
                        {
                            w = 40;
                            h = 30;
                        }
                    }
                    worlds[kvp.Key].size = $"{w}x{h}";
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (incr >= 100)
                        {
                            progressBar1.Value = 100;
                        }
                        else
                        {
                            progressBar1.Value = (incr * 100) / worlds.Count;
                        }
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = worlds[kvp.Key].name;
                        lvi.SubItems.Add($"{w}x{h}");
                        lvi.SubItems.Add(kvp.Key);
                        listView1.Items.Add(lvi);
                        listView1.Enabled = true;

                    });
                    incr++;
                }
                this.Invoke((MethodInvoker)delegate
                {
                    if (incr >= 100)
                    {
                        progressBar1.Value = 100;
                    }
                    else
                    {
                        progressBar1.Value = (incr * 100) / worlds.Count;
                    }
                    listView1.EndUpdate();
                });
            }
        }
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {

            if (e.Column == listviewsorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (listviewsorter.Order == SortOrder.Ascending)
                {
                    listviewsorter.Order = SortOrder.Descending;
                }
                else
                {
                    listviewsorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                listviewsorter.SortColumn = e.Column;
                listviewsorter.Order = SortOrder.Ascending;
            }
            listView1.Sort();
        }


        private void LoadWorldButton_Click(object sender, EventArgs e)
        {
            if (selectedworld != null)
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
        }

        private string RandomString(int length)
        {
            const string chars = "abcdefghijlmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[new Random().Next(s.Length)]).ToArray());
        }
    }
    public class myWorlds
    {
        public string name { get; set; }
        public string size { get; set; }
    }

}
