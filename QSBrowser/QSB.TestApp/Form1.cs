/*
Copyright 2009-2011 Joe Lukacovic

This file is part of QSBrowser.

QSBrowser is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

QSBrowser is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with QSBrowser.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QSB.Server;
using QSB.Data;
using QSB.Common;
using System.Collections;
using QSB.Data.TableObject;
using System.Drawing.Imaging;
using System.Timers;
using QSB.GameServerInterface;

namespace QSB.TestApp
{
    public partial class Form1 : Form
    {
        const int QUERY_INTERVAL_SECONDS = 5;


        /// <summary>
        /// Event raised when Timer has elapsed a predefined period
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryDispatchTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!_Querying && sm != null)
            {
                _Querying = true;
                //sm.DoQueries();
                _Querying = false;
            }
        }

        public Form1()
        {
            InitializeComponent();
            // Old Way:
            //DataBridge.InitializeDatabase(Config.DatabasePath, DataBridge.DatabaseEngine.SQLite);

            _dataSessionFactory = DataBridge.InitializeDatabase(@"Server=127.0.0.1;Database=Servers;Uid=root;Pwd=graphics;", DataBridge.DatabaseEngine.MySql);

            sm = new ServerManager(_dataSessionFactory);
            _fakeServerInterface = new FakeServerInterface();
            _queryController = new ServerQueryController(_dataSessionFactory, _fakeServerInterface);

            _QueryDispatchTimer = new System.Timers.Timer(QUERY_INTERVAL_SECONDS * 1000);
            _QueryDispatchTimer.Elapsed += new ElapsedEventHandler(QueryDispatchTimer_Elapsed);

            lbSnapshots.Click += new EventHandler(lbDetailClick);
            lbGhosts.Click += new EventHandler(lbDetailClick);
            lbActivities.Click += new EventHandler(lbDetailClick);

            foreach (Enum enumValue in Enum.GetValues(typeof(QSB.Common.Game)))
            {
                cbxGame.Items.Add(enumValue);
            }
            cbxGame.DropDownStyle = ComboBoxStyle.DropDownList;

            txt11.Text = "1.0";
            txt12.Text = "0.0";
            txt13.Text = "0.0";
            txt14.Text = "0.0";
            txt15.Text = "0.0";
            txt21.Text = "0.0";
            txt22.Text = "1.0";
            txt23.Text = "0.0";
            txt24.Text = "0.0";
            txt25.Text = "0.0";
            txt31.Text = "0.0";
            txt32.Text = "0.0";
            txt33.Text = "1.0";
            txt34.Text = "0.0";
            txt35.Text = "0.0";
            txt41.Text = "0.0";
            txt42.Text = "0.0";
            txt43.Text = "0.0";
            txt44.Text = "1.0";
            txt45.Text = "0.0";
            txt51.Text = "0.0";
            txt52.Text = "0.0";
            txt53.Text = "0.0";
            txt54.Text = "0.0";
            txt55.Text = "1.0";
        }

        void lbDetailClick(object sender, EventArgs e)
        {
            if (sender is ListBox)
                PutListBoxItemDetail((sender as ListBox).SelectedItem as ListBoxItem);
        }
        ServerManager sm;

        private static bool _Querying;
        System.Timers.Timer _QueryDispatchTimer;
        ServerQueryController _queryController;
        FakeServerInterface _fakeServerInterface;
        IDataSessionFactory _dataSessionFactory;

        private void button1_Click(object sender, EventArgs e)
        {
            var store = _dataSessionFactory.GetDataSession();

            GameServer gameServer = store.GetServerByDNS(txtIpAddress.Text, int.Parse(txtPort.Text));

            if (gameServer == null)
                gameServer = new GameServer();

            gameServer.Region = string.Empty;
            gameServer.GameId = int.Parse(cbxGame.SelectedItem.ToString());
            gameServer.DNS = txtIpAddress.Text;
            gameServer.Location = string.Empty;
            gameServer.Port = int.Parse(txtPort.Text);
            gameServer.ModificationCode = string.Empty;
            gameServer.QueryInterval = 15;

            store.AddUpdateServer(gameServer);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StartQueryTimer();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            StopQueryTimer();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
        }

        private void btnBulkLoad_Click(object sender, EventArgs e)
        {
            string parser = "quake.crmod.com:26001 	CRMOD 26001 DM	CRMOD	10/16	dm3\r\n"
                + "speaknow.quakeone.com 	Chicago CAx	Clan Arena	01/12	dm3\r\n"
                + "fear.quakeone.com 	Fear RQP+	Rune Quake Plus	01/12	start\r\n"
                + "ctf.clan-rum.org 	RuM ThreeWave Pub CTF	Capture The Flag	01/16	e2m4\r\n"
                + "quake.xoc.net:26005 	XOC Pub Headhunters	Headhunters	00/06	dm3\r\n"
                + "quake.xoc.net:26002 	XOC IHOC 26002	IHOC	00/15	dm4\r\n"
                + "quake.xoc.net:26001 	XOC IHOC 26001	IHOC	00/08	start\r\n"
                + "quake.xoc.net:26004 	XOC Rocket Arena	Rocket Arena	00/04	arenarg2\r\n"
                + "rage.quakeone.com 	Rage Clan Arena	Clan Arena	00/16	e1m1\r\n"
                + "ra.clanhdz.com 	Rocket Arena HDZ	Rocket Arena	00/10	start\r\n"
                + "rage.quakeone.com:26002 	Rage CAx 26002	Clan Arena	00/00	 \r\n"
                + "rage.quakeone.com:26001 	Rage CAx 26001	Clan Arena	00/00	 \r\n"
                + "quake.xoc.net:26008 	XOC Artifact-RJS	Artifact-RJS	00/09	e1m2\r\n"
                + "quake.xoc.net:26007 	XOC Lab	IHOC	00/15	start\r\n"
                + "quake.xoc.net:26666 	IHOC Vs. Bots	IHOC	00/16	zed2\r\n"
                + "quake.msmcs.net 	MSM Rune Quake	Rune Quake	00/00	 \r\n"
                + "quake.fquick.com 	Fquick Clan Arena	Clan Arena	00/16	dm3\r\n"
                + "quake.nac.net 	Miny Mine	 	00/00	 \r\n"
                + "quake.intertex.net 	Intertex Pub CTF	Capture The Flag	00/00	 \r\n"
                + "quake.intertex.net:26666 	Intertex ThreeWave CTF	Capture The Flag	00/00	 \r\n"
                + "quake.trinicom.com 	Trinicom Pub CTF	Capture The Flag	00/00	 \r\n"
                + "quake.shmack.net:26003 	HeadHunters 03	Head Hunters	00/16	dm3\r\n"
                + "quake.xoc.net 	XOC IHOC	IHOC	00/12	e1m5\r\n"
                + "quake.shmack.net:26001 	RQ Rocket Arena	Rocket Arena	00/08	mayan1\r\n"
                + "quake.shmack.net 	Shmack Rune Quake	Rune Quake	00/16	e3m2\r\n"
                + "quake.shmack.net:26002 	Shmack Practice Mode	Practice Mode	00/16	dm6\r\n"
                + "xmd.quake1.net:26004 	XMD Pub CTF	Capture The Flag	00/10	start\r\n"
                + "rum.quakeone.com 	RuM Clan Arena	Clan Arena	00/16	dm3\r\n"
                + "roots.quakeone.com 	Roots Chicago CAx	DM	00/00	 \r\n"
                + "rune.ihoc.net 	IHOC RuneHOC	IHOC	00/06	dm4\r\n"
                + "rage.quakeone.com:26004 	Rage CAx 26004	Clan Arena	00/00	 \r\n"
                + "rage.quakeone.com:26003 	Rage CAx 26003	Clan Arena	00/00	 \r\n"
                + "rage.quakeone.com:26005 	Rage CAx 26005	Clan Arena	00/00	 \r\n"
                + "xmd.quake1.net:26002 	XMD Artifact-RJS	Artifact-RJS	00/10	e4m5\r\n"
                + "sctf.dyndns.org:26167 	Super CTF	Super CTF	00/00	 \r\n"
                + "whitehot.quakeone.com 	Whitehot NewDM	NewDM	00/00	 \r\n"
                + "dz.net-tyme.com 	DeadZone Server	DeadZone Mod	00/00	 \r\n"
                + "dredd.quakeone.com 	Dredd Clan Arena	Clan Arena	00/00	 \r\n"
                + "dm.clanhdz.com 	Clan HDZ DM	CRMOD	00/16	dm6\r\n"
                + "dm.clan-rum.org 	RuM CRMOD	CRMOD	00/16	dm6\r\n"
                + "dmx.quake1.net 	DMX IHOC	IHOC	00/10	start\r\n"
                + "flanders.servegame.org:26001 	Flanders Coop(EU)	RQuake Team Coop	00/00	 \r\n"
                + "ffa.quakeone.com 	FFA Deathmatch	DM	00/00	 \r\n"
                + "esx.kicks-ass.net 	eSx IHOC	IHOC	00/00	 \r\n"
                + "bogo.quakeone.com 	Bogo Clan Arena	Clan Arena	00/00	 \r\n"
                + "arena.clan-rum.org 	RuM Rocket Arena	RocketArena	00/10	xarena3\r\n"
                + "banana.essentrix.net 	ZopMod	ZopMod	00/00	 \r\n"
                + "crctf.clan-rum.org 	RuM ThreeWave CTF	Capture The Flag	00/16	ctf1\r\n"
                + "coop.runequake.com:26003 	RQuake Team Cooperative	RQuake Team Coop	00/12	intro\r\n"
                + "ctf.quakeone.com 	QuakeOne CTF	Capture The Flag	00/16	e3m1\r\n"
                + "ca.quakezone.net 	QuakeZone CAx	Clan Arena	00/00	 \r\n"
                + "chicago.pulsegate.net 	Pulsegate CRCTF	CRCTF	00/00	 \r\n"
                + "nj1.suroot.com:26001 	NJ1 Suroot DM	CRMOD	00/00	 \r\n"
                + "nct.gta.igs.net 	SpaceBall CTF	Capture The Flag	00/00	 \r\n"
                + "quake.dod.net 	Capture The Force	Capture The Flag	00/00	 \r\n"
                + "quake.ihoc.net 	IHOC Rocketwar	IHOC	00/08	dm1\r\n"
                + "ihoc.clan-rum.org 	RuM IHOC	IHOC	00/16	e1m1\r\n"
                + "hub.quakeone.com 	QuakeOne Hub	 	00/00	 \r\n"
                + "lab.ihoc.net:26001 	IHOC HPB Only	IHOC	00/08	e1m1\r\n"
                + "fvf.servequake.com 	Future Vs. Fantasy	Future Vs. Fantasy	00/12	e3m4\r\n"
                + "gooland.gotdns.com 	Gooland Artifact	Rune Quake	00/00	 \r\n"
                + "lab.ihoc.net:26008 	IHOC Slide Hoverboard	Slide Hoverboard	00/08	slstart\r\n"
                + "lab.ihoc.net:26006 	IHOC Quake Chat	 	00/08	start\r\n"
                + "lab.ihoc.net:26009 	IHOC AirQuake	Air Quake	00/04	air2\r\n"
                + "lab.ihoc.net:26004 	IHOC Rocket Arena	RocketArena	00/08	arenax\r\n"
                + "lab.ihoc.net:26002 	IHOC Match	IHOC	00/08	start\r\n"
                + "lab.ihoc.net:26005 	IHOC HeadHunters	HeadHunters	00/08	e1m2\r\n"
                + "quake.intertex.net:26002 	Intertex Q1DM	Q1 Deathmatch	00/00	 \r\n"
                + "xctf.clan-rum.org 	RuM Shareware xCTF	xCTF	00/16	e1m5\r\n"
                + "quake.crmod.com 	Quake CRMOD	CRMOD	02/16	dm6\r\n"
                + "wi1.suroot.com:26001 	Wisconsin OFQSP	CRMOD	00/00	 \r\n"
                + "virginia.suroot.com:26001 	Virginia OFQSP	CRMOD	00/00	 \r\n"
                + "edm1.suroot.com:26001 	Edmonton OFQSP	CRMOD	00/16	dm3\r\n"
                + "ca.clanhdz.com 	Clan HDZ CA+	Clan Arena	00/00	 \r\n"
                + "az1.suroot.com:26001 	Arizona OFQSP	CRMOD	00/00	 \r\n"
                + "chi1.suroot.com 	Chi1 Suroot DM	CRMOD	00/00	 \r\n"
                + "phi1.suroot.com:26001 	Philadelphia OFQSP	CRMOD	00/00	 \r\n"
                + "ont1.suroot.com:26001 	Ontario OFQSP	CRMOD	00/00	 \r\n"
                + "por1.suroot.com:26001 	Portland OFQSP	CRMOD	00/00	 \r\n"
                + "ny.ecliptiq.com 	CQL CA+	Clan Arena	00/00	 \r\n"
                + "fmt1.suroot.com:26001 	Fremont OFQSP	CRMOD	00/00\r\n";

            AddBulkFromQuakeOneDotCom(parser, QSB.Common.Region.UnitedStates);

            string europe = "flanders.servegame.org:26002 	Flanders MOTD	Slide	00/00	 \r\n"
                + "flanders.servegame.org:26003 	Flanders NewDM	NewDM	00/00	 \r\n"
                + "qrf.mine.nu:26001 	QRF Rocket Arena	RocketArena	00/12	barena1\r\n"
                + "q1.sydorko.com:26001 	Sydorko 26001	CRMOD	00/00	 \r\n"
                + "bigfoot.quake1.net 	EuroQuake	Rune Quake	00/16	blizz2\r\n"
                + "qrf.mine.nu 	QRF FFA	CRMOD	00/12	dm4\r\n"
                + "flanders.servegame.org 	Flanders DM(EU)	CRMOD	00/00	 \r\n";

            AddBulkFromQuakeOneDotCom(europe, QSB.Common.Region.Europe);

            string brazil = "q1.terra.com.br:26030 	Terra Rocket Arena	Rocket Arena	00/00	 \r\n"
                    + "q1.terra.com.br:26020 	Terra Clan Arena	Clan Arena	00/00	 \r\n"
                    + "q1.terra.com.br:26050 	Terra Total Destruction II	Total Destruction II	00/12	e2m3\r\n"
                    + "q1.terra.com.br:26040 	Terra CRCTF	Capture The Flag	00/12	e1m1\r\n"
                    + "q1.terra.com.br:26010 	Terra CRMOD	CRMOD	00/12	e1m2\r\n"
                    + "q1.terra.com.br:26011 	Terra CRMOD	CRMOD	00/12	dm6\r\n"
                    + "q1.jogos.uol.com.br:26004 	Jogos CRMOD	CRMOD	00/00	 \r\n"
                    + "q1.jogos.uol.com.br:26003 	Jogos Total Destruction II	Total Destruction II	00/00	 \r\n"
                    + "q1.jogos.uol.com.br:26005 	Jogos CRMOD	CRMOD	00/00	 \r\n"
                    + "q1.jogos.uol.com.br:26001 	Jogos DM	 	00/00	 \r\n"
                    + "q1.jogos.uol.com.br 	Jogos DM	 	00/00	 \r\n"
                    + "q1.jogos.uol.com.br:26002 \r\n";

            AddBulkFromQuakeOneDotCom(brazil, QSB.Common.Region.Brazil);

                
        }

        private void AddBulkFromQuakeOneDotCom(string pString, QSB.Common.Region pRegion)
        {
            var store = _dataSessionFactory.GetDataSession();

            foreach (string str in pString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] str2 = str.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                int port = 0;
                string dns = string.Empty;
                string mod = "DM";
                if (str2[0].Contains(':'))
                {
                    string[] str3 = str2[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    port = int.Parse(str3[1]);
                    dns = str3[0].Trim();
                }
                else
                {
                    port = 26000;
                    dns = str2[0].Trim();
                }
                if (str2.Length > 1 &&!str2[2].Contains('/'))
                {
                    mod = str2[2].Trim();
                }



                GameServer gameServer = store.GetServerByDNS(txtIpAddress.Text, int.Parse(txtPort.Text));

                if (gameServer == null)
                    gameServer = new GameServer();

                gameServer.Region = pRegion.ToString();
                gameServer.GameId = (int)Game.NetQuake;
                gameServer.DNS = dns;
                gameServer.Location = string.Empty;
                gameServer.Port = port;
                gameServer.ModificationCode =mod;
                gameServer.QueryInterval = 15;

                store.AddUpdateServer(gameServer);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //IList<GameServer> servers = sm._Store.GetServersByLastQueried(DateTime.UtcNow);
            //label3.Text = servers.Count.ToString();
        }

        /// <summary>
        /// Starts the Query Timer
        /// </summary>
        public void StartQueryTimer()
        {
            _QueryDispatchTimer.Start();
        }

        /// <summary>
        /// Stops the Query Timer
        /// </summary>
        public void StopQueryTimer()
        {
            _QueryDispatchTimer.Stop();
        }

        private void process_Click(object sender, EventArgs e)
        {
            Image file = Image.FromFile(@"C:\temp\try8815.png");

            ColorMatrix cm = new ColorMatrix();
            cm.Matrix00 = float.Parse(txt11.Text);
            cm.Matrix01 = float.Parse(txt12.Text);
            cm.Matrix02 = float.Parse(txt13.Text);
            cm.Matrix03 = float.Parse(txt14.Text);
            cm.Matrix04 = float.Parse(txt15.Text);
            cm.Matrix10 = float.Parse(txt21.Text);
            cm.Matrix11 = float.Parse(txt22.Text);
            cm.Matrix12 = float.Parse(txt23.Text);
            cm.Matrix13 = float.Parse(txt24.Text);
            cm.Matrix14 = float.Parse(txt25.Text);
            cm.Matrix20 = float.Parse(txt31.Text);
            cm.Matrix21 = float.Parse(txt32.Text);
            cm.Matrix22 = float.Parse(txt33.Text);
            cm.Matrix23 = float.Parse(txt34.Text);
            cm.Matrix24 = float.Parse(txt35.Text);
            cm.Matrix30 = float.Parse(txt41.Text);
            cm.Matrix31 = float.Parse(txt42.Text);
            cm.Matrix32 = float.Parse(txt43.Text);
            cm.Matrix33 = float.Parse(txt44.Text);
            cm.Matrix34 = float.Parse(txt45.Text);
            cm.Matrix40 = float.Parse(txt51.Text);
            cm.Matrix41 = float.Parse(txt52.Text);
            cm.Matrix42 = float.Parse(txt53.Text);
            cm.Matrix43 = float.Parse(txt54.Text);
            cm.Matrix44 = float.Parse(txt55.Text);

            Bitmap bmp = new Bitmap(file);

            ImageAttributes ia = new ImageAttributes();
            ia.SetColorMatrix(cm);

            Graphics g = Graphics.FromImage(bmp);

            g.DrawImage(file, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, ia);
            
            pictureBox1.Image = bmp;

            //using (System.IO.FileStream filen = new System.IO.FileStream("C:\\temp\\try" + (DateTime.Now.Ticks % 25435).ToString() + ".png", System.IO.FileMode.OpenOrCreate))
            //{

            //    bmp.Save(filen, System.Drawing.Imaging.ImageFormat.Png);
            //    filen.Flush();
            //    filen.Close();
            //}
        }


        // System Testing

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == gridPlayerInput.Columns["colDepart"].Index)
            {
                gridPlayerInput.Rows.Remove(gridPlayerInput.Rows[e.RowIndex]);
            }
        }

        /// <summary>
        /// AddPlayer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPlayerName.Text)
                && !string.IsNullOrEmpty(txtPlayerIpAddress.Text))
            {
                gridPlayerInput.Rows.Add(txtPlayerName.Text, txtPlayerIpAddress.Text, 0, "Remove");
            }
        }

        private void btnGetSnapshot_Click(object sender, EventArgs e)
        {
            var store = _dataSessionFactory.GetDataSession();
            txtStatus.Text = "Querying...";
            Application.DoEvents();

            lbActivities.Items.Clear();
            lbGhosts.Items.Clear();
            lbSnapshots.Items.Clear();

            // Collect inputs
            
            ServerSnapshot snapshot = new ServerSnapshot();
            snapshot.CurrentMap = txtServerMap.Text;
            snapshot.IpAddress = "10.0.0.3";
            snapshot.Mod = "DM";
            foreach (DataGridViewRow row in gridPlayerInput.Rows)
            {
                snapshot.Players.Add(new PlayerSnapshot( Encoding.UTF8.GetBytes(row.Cells["colName"].Value as string),
                    row.Cells["colIpAddress"].Value as string,
                    row.Cells["colName"].Value as string,
                    0)
                    {
                        Frags = Convert.ToInt32(row.Cells["colFrags"].Value as string)
                        
                    }
                );
            }

            snapshot.ServerName = "FakeServer";
            snapshot.Status = ServerStatus.Running;
            GameServer server = store.GetServer(231);

            _fakeServerInterface.CurrentServerState = snapshot;
            _queryController.DoQuery(server);

            // Get Outputs

            ServerActivity activity = ServerCache.Cache[231];

            foreach (PlayerActivity playerActivity in activity.PlayerActivities)
            {
                lbActivities.Items.Add(new ListBoxItem("session", playerActivity, "Activity: " +playerActivity.PlayerSnap.PlayerName +" Session:" + playerActivity.Session.SessionId));
            }

            foreach (PlayerSnapshot playerSnapshot in activity.ServerSnapshot.Players)
            {
                lbSnapshots.Items.Add(new ListBoxItem("snapshot", playerSnapshot, playerSnapshot.PlayerName));
            }

            foreach (PlayerActivity playerGhost in activity.PlayerMatchGhosts)
            {
                lbGhosts.Items.Add(new ListBoxItem("ghost", playerGhost, "Ghost: " + playerGhost.PlayerSnap.PlayerName + " Session:" + playerGhost.Session.SessionId));
            }

            txtStatus.Text = "";
        }

        public void PutListBoxItemDetail(ListBoxItem pListBoxItem)
        {
            if (pListBoxItem == null)
                return;

            txtDetail1.Text = "";
            txtDetail2.Text = "";
            StringBuilder sb;
            object playerObject = pListBoxItem.Value;
            switch (pListBoxItem.ValueKind)
            {
                case "session":
                    PlayerActivity session = playerObject as PlayerActivity;
                    if (session == null)
                    {
                        txtStatus.Text = "Session is null";
                        return;
                    }
                    lblDetail1.Text = "Activity of: " + session.PlayerSnap.PlayerName + " SessionId: " + session.Session.SessionId.ToString();
                    sb = new StringBuilder();
                    sb.AppendLine("SessionStart: " + session.Session.SessionStart.Value.ToString("MM/dd HH:mm:ss") + " SessionEnd: " + session.Session.SessionEnd == null ? session.Session.SessionEnd.Value.ToString("MM/dd HH:mm:ss") : "NULL");
                    sb.AppendLine("Session.FragCount: " + session.Session.FragCount + " Obj.TotalFrags: " + session.TotalFrags + " MatchStart: " + session.CurrentMatch.PlayerMatchStart.Value.ToString("MM/dd HH:mm:ss"));
                    sb.AppendLine("IsScoreReset: " + session.IsScoreReset.ToString());
                    txtDetail1.Text = sb.ToString();
                    break;
                case "snapshot":
                    PlayerSnapshot snapshot = playerObject as PlayerSnapshot;
                    if (snapshot == null)
                    {
                        txtStatus.Text = "Snapshot is null";
                        return;
                    } 
                    sb = new StringBuilder();
                    sb.AppendLine("Player Name: " + snapshot.PlayerName + " PlayerFrags: " + snapshot.Frags);
                    txtDetail1.Text = sb.ToString();
                    break;
                case "ghost":
                    PlayerActivity ghost = playerObject as PlayerActivity;
                    if (ghost == null)
                    {
                        txtStatus.Text = "Ghost is null";
                        return;
                    }
                    lblDetail1.Text = "Ghost of: " + ghost.PlayerSnap.PlayerName + " SessionId: " + ghost.Session.SessionId.ToString();
                    sb = new StringBuilder();
                    sb.AppendLine("SessionStart: " + ghost.Session.SessionStart.Value.ToString("MM/dd HH:mm:ss") + " SessionEnd: " + ghost.Session.SessionEnd == null ? ghost.Session.SessionEnd.Value.ToString("MM/dd HH:mm:ss") : "NULL");
                    sb.AppendLine("Session.FragCount: " + ghost.Session.FragCount + " Obj.TotalFrags: " + ghost.TotalFrags + " MatchStart: " + ghost.CurrentMatch.PlayerMatchStart.Value.ToString("MM/dd HH:mm:ss"));
                    sb.AppendLine("IsScoreReset: " + ghost.IsScoreReset.ToString());
                    txtDetail1.Text = sb.ToString();
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

    public class ListBoxItem
    {
        public string ValueKind { get; private set; }
        public object Value { get; private set; }
        public string DisplayValue { get; private set; }

        public ListBoxItem(string pValueKind, object pValue, string pDisplayValue)
        {
            ValueKind = pValueKind;
            Value = pValue;
            DisplayValue = pDisplayValue;
        }

        public override string ToString()
        {
            return DisplayValue;
        }
    }
}
