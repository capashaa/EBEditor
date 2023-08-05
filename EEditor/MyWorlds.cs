﻿using System;
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
        private Client client_, cl;
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
                //Console.WriteLine(selectedworld);

                switch (MainForm.accs[MainForm.userdata.username].loginMethod)
                {
                    default:
                    case 0:
                        PlayerIO.QuickConnect.SimpleConnect(bdata.gameID, MainForm.accs[MainForm.userdata.username].login, MainForm.accs[MainForm.userdata.username].password, null, (Client client) => { executeMinimap(client, selectedworld); }, (PlayerIOError error) => { Errorhandler1(error); });
                        break;
                    case 2:
                        PlayerIO.QuickConnect.KongregateConnect(bdata.gameID, MainForm.accs[MainForm.userdata.username].login, MainForm.accs[MainForm.userdata.username].password, null, (Client client) => { executeMinimap(client, selectedworld); }, (PlayerIOError error) => { Errorhandler1(error); });
                        break;
                    case 4:
                        PlayerIO.QuickConnect.SimpleConnect(bdata.gameID, MainForm.accs[MainForm.userdata.username].login, MainForm.accs[MainForm.userdata.username].password, null, (Client cli) =>
                        {
                            cli.Multiplayer.CreateJoinRoom("$service-room", "AuthRoom", true, null, new Dictionary<string, string>() { { "type", "Link" } }, (Connection con) =>
                            {
                                con.OnMessage += (object sender1, PlayerIOClient.Message m) =>
                                {
                                    if (m.Type == "auth") PlayerIO.Authenticate(bdata.gameID, "connected", new Dictionary<string, string>() { { "userId", m.GetString(0) }, { "auth", m.GetString(1) } }, null, (Client client) => { executeMinimap(client, selectedworld); }, (PlayerIOError error) => { Errorhandler1(error); });
                                };
                            },
                            (PlayerIOError error) => { Errorhandler1(error); });
                        }, (PlayerIOError error) => { Errorhandler1(error); });
                        break;
                }
                //if (worlds.Count() > 0) File.WriteAllText(Directory.GetCurrentDirectory() + "\\" + MainForm.userdata.username + ".myworlds.json", JsonConvert.SerializeObject(worlds, Newtonsoft.Json.Formatting.Indented));
                //this.Close();
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
        private void MyWorlds_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (worlds.Count() > 0) File.WriteAllText($"{Directory.GetCurrentDirectory()}\\{MainForm.userdata.username}.myworlds.json", JsonConvert.SerializeObject(worlds, Newtonsoft.Json.Formatting.Indented));
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            rooms.Clear();
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
                    switch (MainForm.accs[MainForm.userdata.username].loginMethod)
                    {
                        default:
                        case 0:
                            PlayerIO.QuickConnect.SimpleConnect(bdata.gameID, MainForm.accs[MainForm.userdata.username].login, MainForm.accs[MainForm.userdata.username].password, null, loginToWorlds, Errorhandler);
                            break;
                        case 2:
                            PlayerIO.QuickConnect.KongregateConnect(bdata.gameID, MainForm.accs[MainForm.userdata.username].login, MainForm.accs[MainForm.userdata.username].password, null, loginToWorlds, Errorhandler);
                            break;
                        case 4:
                            PlayerIO.QuickConnect.SimpleConnect(bdata.gameID, MainForm.accs[MainForm.userdata.username].login, MainForm.accs[MainForm.userdata.username].password, null, (Client cli) =>
                            {
                                cli.Multiplayer.CreateJoinRoom("$service-room", "AuthRoom", true, null, new Dictionary<string, string>() { { "type", "Link" } }, (Connection con) =>
                                {
                                    con.OnMessage += (object sender1, PlayerIOClient.Message m) =>
                                    {
                                        if (m.Type == "auth") PlayerIO.Authenticate(bdata.gameID, "connected", new Dictionary<string, string>() { { "userId", m.GetString(0) }, { "auth", m.GetString(1) } }, null, loginToWorlds, Errorhandler);
                                    };
                                },
                                (PlayerIOError error) => { Errorhandler(error); });
                            }, (PlayerIOError error) => { Errorhandler(error); });
                            break;
                    }

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
            tryLobbyConnect(string.Format("{0}_{1}", client.ConnectUserId, RandomString(5)));
        }
        private void tryLobbyConnect(string id)
        {
            if (client_ != null)
            {
                client_.Multiplayer.CreateJoinRoom(id, $"Lobby{bdata.version}", true, null, null, lobbyConnected, (PlayerIOError error) => { tryLobbyConnect(id); });

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
                        con.Send("getMySimplePlayerObject");
                        break;
                    case "getMySimplePlayerObject":
                        string owner = "Unknown";
                        int incr = 0, incr1 = 0, total1 = 0;
                        int total = bdata.extractPlayerObjectsMessage(m) + 1;
                        owner = m[(UInt32)total].ToString();
                        total += 14;
                        if (!string.IsNullOrEmpty(m[(UInt32)total].ToString())) rooms.Add(m[(UInt32)total].ToString());
                        total++;
                        if (!string.IsNullOrEmpty(m[(UInt32)total].ToString())) rooms.Add(m[(UInt32)total].ToString());
                        total++;
                        if (!string.IsNullOrEmpty(m[(UInt32)total].ToString())) rooms.Add(m[(UInt32)total].ToString());
                        total++;
                        total++;
                        if (m[(UInt32)total].ToString().Contains((char)0x1399))
                        {
                            string[] worlds = m[(UInt32)total].ToString().Split(new char[] { (char)0x1399 });
                            rooms.AddRange(worlds);
                        }
                        else
                        {
                            if (m[(UInt32)total].ToString() != null) rooms.Add(m[(UInt32)total].ToString());
                        }
                        if (rooms.Count > 0)
                        {
                            total1 = rooms.Count;
                            for (int i = 0; i < rooms.Count; i++)
                            {
                                var world = new World(InputType.BigDB, rooms[i], client_);
                                this.Invoke((MethodInvoker)delegate
                                {
                                    ListViewItem lvi = new ListViewItem();
                                    string names = null;
                                    string wh = null;
                                    //Console.WriteLine(world.Width);
                                    if (world.Width.ToString() == null && world.Height.ToString() == null)
                                    {
                                        if (world.Type.ToString() != null)
                                        {
                                            wh = world.Type.ToString();
                                        }
                                        else
                                        {
                                            wh = "?x?";
                                        }
                                    }
                                    else
                                    {
                                        wh = $"{world.Width}x{world.Height}";
                                    }
                                    progressBar1.Value = (incr1 * 100) / total1;
                                    if (!worlds.ContainsKey(rooms[i]))
                                    {
                                        names = world.Title;
                                        lvi.Text = names;
                                        lvi.SubItems.Add(wh);
                                        lvi.SubItems.Add(rooms[i]);
                                        listView1.Items.Add(lvi);
                                        worlds.Add(rooms[i], new myWorlds() { name = names, size = wh });
                                    }
                                    incr1++;
                                    if (incr1 >= total1)
                                    {
                                        s2.Release();
                                    }

                                });
                            }
                        }
                        else
                        {
                            s2.Release();
                        }


                        s2.WaitOne();
                        if (listView1.InvokeRequired)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                listView1.Enabled = true;
                                listView1.EndUpdate();
                            });
                        }
                        if (!listView1.InvokeRequired)
                        {
                            listView1.Enabled = true;
                            listView1.EndUpdate();
                        }
                        if (progressBar1.InvokeRequired)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                progressBar1.Value = 100;
                            });
                        }
                        if (!progressBar1.InvokeRequired)
                        {
                            progressBar1.Value = 100;
                        }



                        break;
                    /*
                   case "theReceivedData":
                           Here do you get the received data that you requested for. (Sent message)
                        break;
                    */
                    case "linked":
                        client_.Multiplayer.CreateJoinRoom("$service-room", "AuthRoom", true, null, new Dictionary<string, string>() { { "type", "Link" } }, (Connection conn) =>
                        {
                            conn.OnMessage += (object sender1, PlayerIOClient.Message mm) =>
                            {
                                if (mm.Type == "auth")
                                {
                                    PlayerIO.Authenticate("everybody-edits-su9rn58o40itdbnw69plyw", "connected", new Dictionary<string, string>() { { "userId", mm.GetString(0) }, { "auth", mm.GetString(1) } }, null, (Client client1) =>
                                    {
                                        cl = client1;
                                        con.Disconnect();
                                        tryLobbyConnect(string.Format("{0}_{1}", cl.ConnectUserId, RandomString(5)));
                                    }, (PlayerIOError error) =>
                                    {
                                    });
                                }
                            };
                        },
                        (PlayerIOError error) =>
                        {
                        });
                        break;
                }
            };
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
