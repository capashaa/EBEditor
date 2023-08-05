﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using PlayerIOClient;
using System.Text;
using System.Linq;

namespace EEditor
{
    public partial class Accounts : Form
    {
        private int accountOption = 0;
        public static string lastSelected = "guest";
        public static string accss = @"\accounts.json";
        public static bool admin_ = false;
        public static bool moderator_ = false;
        public static Dictionary<string, accounts> accs = new Dictionary<string, accounts>();
        public static Client client_;
        static ToolStripProgressBar toolsprog;
        static ToolStrip tools;
        static ListBox listb;
        public Accounts()
        {
            InitializeComponent();
        }

        private void Accounts_Load(object sender, EventArgs e)
        {
            toolsprog = toolStripProgressBar1;
            tools = statusStrip1;
            listb = accountListBox;
            bool admin = false;
            foreach (KeyValuePair<string, accounts> acco in MainForm.accs)
            {
                accountListBox.Items.Add(acco.Key);
                accountListBox.SelectedIndex = 0; //Select topmost account (guest) in account list and load it's data to details
                accEverybodyEdits.Checked = true; //Selects EE because guest is an EE account
                instructionsField.Rtf = everybodyEditsRTF;
            }

            this.BackColor = MainForm.themecolors.background;
            foreach (Control value0 in this.Controls)
            {
                if (value0.GetType() == typeof(StatusStrip))
                {
                    value0.ForeColor = MainForm.themecolors.foreground;
                    value0.BackColor = MainForm.themecolors.accent;
                }
                if (value0.GetType() == typeof(GroupBox))
                {
                    value0.ForeColor = MainForm.themecolors.foreground;
                }
                foreach (Control value1 in value0.Controls)
                {
                    Control ctrl = value1;
                    if (ctrl.GetType() == typeof(TextBox))
                    {
                        ((TextBox)ctrl).BorderStyle = BorderStyle.FixedSingle;
                        ctrl.BackColor = MainForm.themecolors.accent;
                        ctrl.ForeColor = MainForm.themecolors.foreground;
                    }
                    if (ctrl.GetType() == typeof(RichTextBox))
                    {
                        ctrl.BackColor = MainForm.themecolors.accent;
                        ctrl.ForeColor = MainForm.themecolors.foreground;
                    }

                    if (ctrl.GetType() == typeof(Label))
                    {
                        ctrl.ForeColor = MainForm.themecolors.foreground;
                    }
                    if (ctrl.GetType() == typeof(ListBox))
                    {
                        ctrl.BackColor = MainForm.themecolors.accent;
                        ctrl.ForeColor = MainForm.themecolors.foreground;
                    }
                    if (ctrl.GetType() == typeof(Button))
                    {
                        ctrl.BackColor = MainForm.themecolors.accent;
                        ctrl.ForeColor = MainForm.themecolors.foreground;
                        ((Button)ctrl).FlatStyle = FlatStyle.Flat;
                        if (((Button)ctrl).Image != null)
                        {
                            Bitmap bmpa = (Bitmap)((Button)ctrl).Image;
                            Bitmap bmpa1 = new Bitmap(((Button)ctrl).Image.Width, ((Button)ctrl).Image.Height);
                            for (int x = 0; x < ((Button)ctrl).Image.Width; x++)
                            {
                                for (int y = 0; y < ((Button)ctrl).Image.Height; y++)
                                {
                                    if (bmpa.GetPixel(x, y).A > 80)
                                    {
                                        bmpa1.SetPixel(x, y, MainForm.themecolors.imageColors);
                                    }
                                    else
                                    {
                                        bmpa1.SetPixel(x, y, MainForm.themecolors.accent);
                                    }
                                }
                            }
                        ((Button)ctrl).Image = bmpa1;
                        }
                    }
                }
            }
            #region Tooltips
            ToolTip tp = new ToolTip();

            tp.SetToolTip(addAccount, "Add an account");
            tp.SetToolTip(removeAccount, "Remove selected account");
            tp.SetToolTip(saveAccount, "Save account values");
            tp.SetToolTip(reloadPacks, "Reload the list of item packs purchased with the account");
            tp.SetToolTip(accEverybodyEdits, "Use Everybody Edits Account");
            tp.SetToolTip(accEverybodyEditsTransfer, "Use transfered Accounts");
            tp.SetToolTip(accKongregate, "Use Kongregate Account");
            tp.SetToolTip(accArmorGames, "Use ArmorGames Account");
            tp.SetToolTip(accFacebook, "Use Facebook Account");
            #endregion
        }

        private void Accounts_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainForm.editArea.MainForm.cb.Focus();
            MainForm.editArea.MainForm.cb.Items.Clear();
            foreach (KeyValuePair<string, accounts> accd in MainForm.accs)
            {
                MainForm.editArea.MainForm.cb.Items.Add(accd.Key);
            }
            MainForm.editArea.MainForm.cb.Items.Add("---------");
            MainForm.editArea.MainForm.cb.Items.Add("Edit accounts...");
            MainForm.editArea.MainForm.cb.SelectedIndex = 0;
        }

        #region RTF tutorial
        //Everybody Edits rtf line
        private string everybodyEditsRTF = @"{\rtf1\pc Use the e-mail and password you signed up with.";
        private string everybodyEditsRTF2 = @"{\rtf1\pc Use the e-mail and password you got when transfered your account from facebook or armorgames.";
        //Kongregate rtf line
        private string kongregateRTF = @"{\rtf1\pc 1. Go to http://pastebin.com/raw/SMFAdPH9 and drag the text to bookmarks bar\par
2. Go to Everybody Edits in Kongregate and click the bookmark\par
3. Copy shown values to EEditor";

        //Armor Games rtf line
        /*private string armorgamesRTF = @"{\rtf1\pc 1. Go to http://pastebin.com/raw/Ars6y8GN and drag the text to bookmarks bar\par
2. Go to Everybody Edits in ArmorGames and click the bookmark\par
3. Copy shown values to EEditor";*/

        private void instructionsField_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            // Makes RTF links work
            System.Diagnostics.Process.Start(e.LinkText);
        }
        #endregion

        #region Login method and fields
        private void radiobuttons_CheckedChanged(object sender, EventArgs e)
        {
            var value = (RadioButton)sender;
            if (value.Name == "accEverybodyEdits")
            {
                loginLabel1.Text = "Email:";
                loginField1.Width = 146;
                loginField1.Location = new Point(48, 190);
                loginField1.Visible = true;

                loginLabel2.Text = "Password:";
                loginField2.Width = 125;
                loginField2.Location = new Point(69, 219);

                instructionsField.Rtf = everybodyEditsRTF;
            }
            if (value.Name == "accEverybodyEditsTransfer")
            {
                loginLabel1.Text = "Email:";
                loginField1.Width = 146;
                loginField1.Location = new Point(48, 190);
                loginField1.Visible = true;

                loginLabel2.Text = "Password:";
                loginField2.Width = 125;
                loginField2.Location = new Point(69, 219);

                instructionsField.Rtf = everybodyEditsRTF2;
            }
            /* Facebook disabled
             * else if (value.Text == "Facebook")
            {
                loginLabel1.Text = "Note: Token will expire in 1-2 hours";
                loginField1.Visible = false;

                loginLabel2.Text = "AccessToken:";
                loginField2.Width = 112;
                loginField2.Location = new Point(82, 219);

                instructionsField.Rtf = facebookRTF;
            }*/
            else if (value.Name == "accKongregate")
            {
                loginLabel1.Text = "UserID:";
                loginField1.Width = 138;
                loginField1.Location = new Point(56, 190);
                loginField1.Visible = true;

                loginLabel2.Text = "GameAuthToken:";
                loginField2.Width = 98;
                loginField2.Location = new Point(96, 219);

                instructionsField.Rtf = kongregateRTF;
            }
            /*else if (value.Name == "accArmorGames")
            {
                loginLabel1.Text = "UserID:";
                loginField1.Width = 138;
                loginField1.Location = new Point(56, 190);
                loginField1.Visible = true;

                loginLabel2.Text = "AuthToken:";
                loginField2.Width = 125;
                loginField2.Location = new Point(69, 219);

                instructionsField.Rtf = armorgamesRTF;
            }*/
        }

        public static void CheckAccounts(MainForm mf)
        {

            var acccs = $"{Directory.GetCurrentDirectory()}\\accounts.json";
            if (File.Exists(acccs))
            {

                var output = JObject.Parse(File.ReadAllText(acccs));
                foreach (var property in output)
                {
                    if (!accs.ContainsKey(property.Key))
                    {
                        if (property.Value["payvault"] != null)
                        {
                            if (property.Value["login"].ToString() != "guest" && property.Value["password"].ToString() != "guest" && property.Value["login"] != null && property.Value["password"] != null && property.Value["loginMethod"] != null)
                            {
                                try
                                {
                                    Dictionary<string, int> values = JsonConvert.DeserializeObject<Dictionary<string, int>>(property.Value["payvault"].ToString());
                                    MainForm.accs.Add(property.Key, new accounts() { login = property.Value["login"].ToString(), password = property.Value["password"].ToString(), loginMethod = (int)property.Value["loginMethod"],admin = (bool)property.Value["admin"], moderator = (bool)property.Value["moderator"], payvault = values });
                                    mf.cb.Items.Add(property.Key);
                                }
                                catch (Exception)
                                {
                                    switch (Convert.ToInt32(property.Value["loginMethod"]))
                                    {
                                        case 0:
                                            PlayerIO.QuickConnect.SimpleConnect(bdata.gameID, property.Value["login"].ToString(), property.Value["password"].ToString(), null, (Client client) => successLogin2(client, property.Value["login"].ToString().ToString(), property.Value["password"].ToString(), Convert.ToInt32(property.Value["loginMethod"])), failLogin1);
                                            break;
                                        case 2:
                                            PlayerIO.QuickConnect.KongregateConnect(bdata.gameID, property.Value["login"].ToString(), property.Value["password"].ToString(), null, (Client client) => successLogin2(client, property.Value["login"].ToString().ToString(), property.Value["password"].ToString(), Convert.ToInt32(property.Value["loginMethod"])), failLogin1);
                                            break;
                                        case 3:
                                            PlayerIO.Authenticate(bdata.gameID, "secure", new Dictionary<string, string> { { "userId", property.Value["login"].ToString() }, { "authToken", property.Value["password"].ToString() } }, null, (Client client) => successLogin2(client, property.Value["login"].ToString(), property.Value["password"].ToString(), Convert.ToInt32(property.Value["loginMethod"])), failLogin1);
                                            break;
                                        case 4:
                                            PlayerIO.QuickConnect.SimpleConnect(bdata.gameID, property.Value["login"].ToString(), property.Value["password"].ToString(), null, (Client cli) =>
                                            {
                                                cli.Multiplayer.CreateJoinRoom("$service-room", "AuthRoom", true, null, new Dictionary<string, string>() { { "type", "Link" } }, (Connection con) =>
                                                {
                                                    con.OnMessage += (object sender1, PlayerIOClient.Message m) =>
                                                    {
                                                        if (m.Type == "auth") PlayerIO.Authenticate(bdata.gameID, "connected", new Dictionary<string, string>() { { "userId", m.GetString(0) }, { "auth", m.GetString(1) } }, null, (Client client) => successLogin2(client, property.Value["login"].ToString(), property.Value["password"].ToString(), Convert.ToInt32(property.Value["loginMethod"])), failLogin1);
                                                    };
                                                },
                                                (PlayerIOError error) => MessageBox.Show(error.Message, "Error"));
                                            }, failLogin1);
                                            break;
                                    }
                                    mf.cb.Items.Add(property.Key);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("derp");
                        }
                    }
                }
                if (!accs.ContainsKey("guest"))
                {
                    MainForm.accs.Add("guest", new accounts() { login = "guest", password = "guest", loginMethod = 0, payvault = new Dictionary<string, int>() });
                    mf.cb.Items.Add("guest");
                    File.WriteAllText(acccs, JsonConvert.SerializeObject(MainForm.accs, Formatting.Indented));
                }
            }
            else
            {
                if (!accs.ContainsKey("guest"))
                {
                    MainForm.accs.Add("guest", new accounts() { login = "guest", password = "guest", loginMethod = 0, payvault = new Dictionary<string, int>() });
                    mf.cb.Items.Add("guest");
                    File.WriteAllText(acccs, JsonConvert.SerializeObject(MainForm.accs, Formatting.Indented));
                }
            }
            mf.cb.Items.Add("---------");
            mf.cb.Items.Add("Edit accounts...");
        }
        private void saveAccount_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(loginField1.Text))
            {
                if (!string.IsNullOrWhiteSpace(loginField2.Text))
                {
                    if (accEverybodyEdits.Checked)
                    {
                        PlayerIO.QuickConnect.SimpleConnect(bdata.gameID, loginField1.Text, loginField2.Text, null, (Client client) => successLogin(client, loginField1.Text, loginField2.Text, 0), failLogin);
                        accountOption = 0;
                    }

                    if (accEverybodyEditsTransfer.Checked)
                    {
                        PlayerIO.QuickConnect.SimpleConnect(bdata.gameID, loginField1.Text, loginField2.Text, null, (Client cli) =>
                        {
                            cli.Multiplayer.CreateJoinRoom("$service-room", "AuthRoom", true, null, new Dictionary<string, string>() { { "type", "Link" } }, (Connection con) =>
                            {
                                con.OnMessage += (object sender1, PlayerIOClient.Message m) =>
                                {
                                    if (m.Type == "auth") PlayerIO.Authenticate("everybody-edits-su9rn58o40itdbnw69plyw", "connected", new Dictionary<string, string>() { { "userId", m.GetString(0) }, { "auth", m.GetString(1) } }, null, (Client client) => successLogin(client, loginField1.Text, loginField2.Text, 4), failLogin);
                                };
                            },
                            (PlayerIOError error) => MessageBox.Show(error.Message, "Error"));
                        },
                        failLogin);
                        accountOption = 4;
                    }
                    /* Facebook disabled
                     * else if (accFacebook.Checked)
                    {
                        PlayerIO.Authenticate("everybody-edits-su9rn58o40itdbnw69plyw", "secure", new Dictionary<string, string>() { { "accessToken", loginField1.Text } }, null, successLogin, failLogin);
                        //PlayerIO.QuickConnect.FacebookOAuthConnect("everybody-edits-su9rn58o40itdbnw69plyw", loginField1.Text, null, null, successLogin, failLogin);
                        accountOption = 1;
                    }
                    */
                    else if (accKongregate.Checked)
                    {
                        PlayerIO.QuickConnect.KongregateConnect(bdata.gameID, loginField1.Text, loginField2.Text, null, (Client client) => successLogin(client, loginField1.Text, loginField2.Text, 2), failLogin);
                        accountOption = 2;
                    }
                    /*else if (accArmorGames.Checked)
                    {
                        PlayerIO.Authenticate(bdata.gameID, "secure", new Dictionary<string, string> { { "userId", loginField1.Text }, { "authToken", loginField2.Text } }, null, successLogin, failLogin);
                        accountOption = 3;
                    }*/
                }
                else { MessageBox.Show("Your login details aren't added", "Login error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            else { MessageBox.Show("Your login details aren't added", "Login error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void reloadPacks_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(loginField1.Text))
            {
                if (!string.IsNullOrWhiteSpace(loginField2.Text))
                {
                    if (accEverybodyEdits.Checked)
                    {
                        PlayerIO.QuickConnect.SimpleConnect(bdata.gameID, loginField1.Text, loginField2.Text, null, (Client client) => successLogin1(client, loginField1.Text, loginField2.Text, 0), failLogin);
                        accountOption = 0;
                    }
                    if (accEverybodyEditsTransfer.Checked)
                    {
                        PlayerIO.QuickConnect.SimpleConnect(bdata.gameID, loginField1.Text, loginField2.Text, null, (Client cli) =>
                        {
                            cli.Multiplayer.CreateJoinRoom("$service-room", "AuthRoom", true, null, new Dictionary<string, string>() { { "type", "Link" } }, (Connection con) =>
                            {
                                con.OnMessage += (object sender1, PlayerIOClient.Message m) =>
                                {
                                    if (m.Type == "auth") PlayerIO.Authenticate("everybody-edits-su9rn58o40itdbnw69plyw", "connected", new Dictionary<string, string>() { { "userId", m.GetString(0) }, { "auth", m.GetString(1) } }, null, (Client client) => successLogin1(client, loginField1.Text, loginField2.Text, 4), failLogin);
                                };
                            },
                            (PlayerIOError error) => MessageBox.Show(error.Message, "Error"));
                        },
                        failLogin);
                        accountOption = 4;
                    }
                    /* Facebook disabled
                    else if (accFacebook.Checked)
                    {
                        PlayerIO.QuickConnect.FacebookOAuthConnect("everybody-edits-su9rn58o40itdbnw69plyw", loginField1.Text, null, null, successLogin1, failLogin);
                    }
                    */
                    else if (accKongregate.Checked)
                    {
                        PlayerIO.QuickConnect.KongregateConnect(bdata.gameID, loginField1.Text, loginField2.Text, null, (Client client) => successLogin1(client, loginField1.Text, loginField2.Text, 0), failLogin);
                        accountOption = 2;
                    }
                    /*else if (accArmorGames.Checked)
                    {
                        PlayerIO.Authenticate(bdata.gameID, "secure", new Dictionary<string, string> { { "userId", loginField1.Text }, { "authToken", loginField2.Text } }, null, successLogin1, failLogin);
                    }*/
                }
                else { MessageBox.Show("Your login details aren't added", "Login error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            else { MessageBox.Show("Your login details aren't added", "Login error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void failLogin(PlayerIOError error)
        {
            MessageBox.Show(error.Message, "Login error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void failLogin1(PlayerIOError error)
        {
            MessageBox.Show(error.Message, "Login error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static string RandomString(int length)
        {
            const string chars = "abcdefghijlmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[new Random().Next(s.Length)]).ToArray());
        }

        public static void tryLobbyConnect(string id, Client client, string login, string password, int loginmethod)
        {
            if (client != null)
            {
                client.Multiplayer.CreateJoinRoom(id, $"Lobby{bdata.version}", true, null, null,
                    (Connection con) =>
                    {
                        lobbyConnected(con, client, login, password, loginmethod);
                    },
                    (PlayerIOError error) =>
                    {
                        tryLobbyConnect(id, client, login, password, loginmethod);
                    });

            }
        }
        public static void lobbyConnected(Connection con, Client client, string login, string pass, int loginmethod)
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
                        tryLobbyConnect(m.GetString(0), client, login, pass, loginmethod);
                        break;
                    case "connectioncomplete":
                        con.Send("getMySimplePlayerObject");
                        break;
                    case "getMySimplePlayerObject":
                        Dictionary<string, int> pv = new Dictionary<string, int>();
                        int total = bdata.extractPlayerObjectsMessage(m) + 1;
                        string nickname = m[(uint)total].ToString();
                        int goldmember = total + 9;
                        int isadmin = total + 7;
                        int ismod = total + 8;
                        admin_ = m.GetBoolean((uint)isadmin);
                        moderator_ = m.GetBoolean((uint)ismod);
                        if (m.GetBoolean((uint)goldmember)) pv.Add("goldmember", 0);
                        client.PayVault.Refresh(() =>
                        {
                            int totalpv = client.PayVault.Items.Length;
                            int incr = 0;
                            if (client.PayVault.Has("pro") && !pv.ContainsKey("beta")) pv.Add("beta", 0);
                            if (client.PayVault.Items.Length > 0)
                            {
                                for (int a = 0; a < client.PayVault.Items.Length; a++)
                                {
                                    incr += 1;
                                    var brick = client.PayVault.Items[a].ItemKey;
                                    if (brick.StartsWith("brick") || brick.StartsWith("npc") || brick.StartsWith("block") || brick == "mixednewyear2010")
                                    {
                                        if (!pv.ContainsKey(brick))
                                        {
                                            if (brick.StartsWith("npc")) pv.Add(brick, 1);
                                            else pv.Add(brick, 0);
                                        }
                                        else
                                        {
                                            pv[brick] += 1;
                                        }
                                    }
                                    if (tools.InvokeRequired)
                                    {
                                        tools.Invoke((MethodInvoker)delegate
                                        {
                                            Accounts.toolsprog.Value = Convert.ToInt32((double)incr / totalpv * 100);
                                        });
                                    }
                                }



                                MainForm.userdata.username = nickname;
                                if (!MainForm.accs.ContainsKey(nickname))
                                {
                                    MainForm.accs.Add(nickname, new accounts() { login = login, password = pass, loginMethod = loginmethod, admin = admin_, moderator = moderator_, payvault = pv });
                                    if (Accounts.listb.InvokeRequired)
                                    {
                                        Accounts.listb.Invoke((MethodInvoker)delegate
                                        {
                                            Accounts.listb.Items.Remove("(new account)");
                                            Accounts.listb.Items.Add(nickname);
                                        });
                                    }
                                    File.WriteAllText(Directory.GetCurrentDirectory() + accss, JsonConvert.SerializeObject(MainForm.accs, Formatting.Indented));
                                }
                                else if (MainForm.accs.ContainsKey(nickname))
                                {
                                    MainForm.accs[nickname] = new accounts() { login = login, password = pass, loginMethod = loginmethod, admin = admin_, moderator = moderator_,payvault = pv };
                                    File.WriteAllText(Directory.GetCurrentDirectory() + accss, JsonConvert.SerializeObject(MainForm.accs, Formatting.Indented));
                                }
                                if (Accounts.listb.InvokeRequired)
                                {
                                    Accounts.listb.Invoke((MethodInvoker)delegate
                                    {
                                        Accounts.listb.SelectedItem = nickname;
                                    });
                                }


                            }
                            else
                            {

                                if (!MainForm.accs.ContainsKey(nickname))
                                {
                                    MainForm.userdata.username = nickname;
                                    MainForm.accs.Add(nickname, new accounts() { login = login, password = pass, loginMethod = loginmethod, admin = admin_, moderator = moderator_, payvault = pv });
                                    if (Accounts.listb.InvokeRequired)
                                    {
                                        Accounts.listb.Invoke((MethodInvoker)delegate
                                        {
                                            Accounts.listb.Items.Remove("(new account)");
                                            Accounts.listb.Items.Add(nickname);
                                        });
                                    }

                                    File.WriteAllText(Directory.GetCurrentDirectory() + accss, JsonConvert.SerializeObject(MainForm.accs, Formatting.Indented));
                                    if (Accounts.listb.InvokeRequired)
                                    {
                                        Accounts.listb.Invoke((MethodInvoker)delegate
                                        {
                                            Accounts.listb.SelectedItem = nickname;
                                        });
                                    }
                                     if (tools.InvokeRequired)
                                    {
                                        tools.Invoke((MethodInvoker)delegate
                                        {
                                            Accounts.toolsprog.Value = 100;
                                        });
                                    }
                                }

                            }
                        });

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
                                        Client clie = client1;
                                        con.Disconnect();
                                        tryLobbyConnect(string.Format("{0}_{1}", clie.ConnectUserId, RandomString(5)), client, login, pass, loginmethod);
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
        public static void successLogin2(Client client, string login, string password, int loginmethod) //login for save
        {
            client_ = client;
            tryLobbyConnect($"{client.ConnectUserId}_{RandomString(5)}", client, login, password, loginmethod);

        }
        private void successLogin(Client client, string login, string password, int loginmethod) //login for save
        {
            client_ = client;
            tryLobbyConnect($"{client.ConnectUserId}_{RandomString(5)}", client, login, password, loginmethod);
        }

        private void successLogin1(Client client, string login, string password, int loginmethod) //login for reload
        {
            client_ = client;
            tryLobbyConnect($"{client.ConnectUserId}_{RandomString(5)}", client, login, password, loginmethod);
        }


        #endregion

        #region Multiaccount
        private void addAccount_Click(object sender, EventArgs e)
        {
            if (!accountListBox.Items.Contains("(new account)"))
            {
                accountListBox.Items.Add("(new account)"); //Add a placeholder if doesn't exist already 
            }
            accountListBox.SelectedIndex = accountListBox.Items.Count - 1; //Select placeholder
            loginField1.Focus(); //Focus first login field
        }

        private void removeAccount_Click(object sender, EventArgs e)
        {
            if (accountListBox.SelectedIndices.Count > 0)
            {
                if (MainForm.accs.ContainsKey(lastSelected) && lastSelected == "(new account)") //Removes placeholder entry from sight
                {
                    accountListBox.Items.Remove("(new account)");
                }
                else //Remove entry and file part if it's a real account
                {
                    MainForm.accs.Remove(lastSelected);
                    accountListBox.Items.Remove(lastSelected);
                    File.WriteAllText(Directory.GetCurrentDirectory() + accss, JsonConvert.SerializeObject(MainForm.accs, Formatting.Indented));
                    //loginField1.Clear(); //no need to clear if you just select another item
                    //loginField2.Clear();
                }
            }
            accountListBox.SelectedIndex = accountListBox.Items.Count - 1; //Select last account after done removing
        }

        private void accountListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (accountListBox.SelectedIndices.Count > 0)
            {
                lastSelected = accountListBox.Items[accountListBox.SelectedIndex].ToString();
                if (MainForm.accs.ContainsKey(lastSelected))
                {
                    var lm = MainForm.accs[lastSelected].loginMethod;
                    switch (lm)
                    {
                        case 0:
                            accEverybodyEdits.Checked = true;
                            break;
                        /* Facebook disabled
                         *  case 1:
                            accFacebook.Checked = true;
                            break;
                        */
                        case 2:
                            accKongregate.Checked = true;
                            break;
                        /*case 3: Armorgames removed
                            accArmorGames.Checked = true;
                            break;*/
                        case 4:
                            accEverybodyEditsTransfer.Checked = true;
                            break;
                    }
                    loginField1.Text = MainForm.accs[lastSelected].login;
                    loginField2.Text = MainForm.accs[lastSelected].login == "guest" ? "guest" : MainForm.accs[lastSelected].password;
                }
                else
                {
                    loginField1.Clear();
                    loginField2.Clear();
                }

                if (lastSelected == "guest") //Gray out removal button and account data when guest is selected
                {
                    addAccount.Enabled = true;
                    removeAccount.Enabled = false;
                    saveAccount.Enabled = false;
                    reloadPacks.Enabled = true;
                    groupBox2.Enabled = false;
                }
                else if (lastSelected == "(new account)") //Gray out addition button when placeholder is selected
                {
                    addAccount.Enabled = false;
                    removeAccount.Enabled = true;
                    saveAccount.Enabled = true;
                    reloadPacks.Enabled = false;
                    groupBox2.Enabled = true;
                }
                else
                {
                    addAccount.Enabled = true;
                    removeAccount.Enabled = true;
                    saveAccount.Enabled = true;
                    reloadPacks.Enabled = true;
                    groupBox2.Enabled = true;
                }
            }
        }
        #endregion
    }
    public class SpecialFunctions
    {
        public static void successLogin(Client client, string login, string password, int loginmethod)
        {
            Accounts.tryLobbyConnect($"{client.ConnectUserId}_{Accounts.RandomString(5)}", client, login, password, loginmethod);
        }
    }
}
