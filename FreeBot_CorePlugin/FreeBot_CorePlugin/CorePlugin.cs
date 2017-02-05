using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SilkroadSecurityApi;
using SilkroadInformationAPI;
using SilkroadInformationAPI.Client;
using SilkroadInformationAPI.Client.Network;
using SilkroadInformationAPI.PluginInterface;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace FreeBot_CorePlugin
{
    public class CorePlugin : IPluginInterface
    {
        //FIX MOVEMENTS X, Y + Cave movements
        //FIX EXP TAKEN /HOUR /MINUTEa
        //FIX MULTIPLE CONNECTIONS WHEN CLIENT IS OPENED
        //FIX BOT ONLY START WHEN CONNECTED
        //FIX CLIENTLESS
        public string PluginName
        {
            get
            {
                return "CorePlugin";
            }
        }

        public System.Windows.Forms.TabPage Initialize(Dictionary<string, int> SharedVariables)
        {
            try
            {
                return InitializeComponent();
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
            return null;
        }



        #region IMPORTS

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, ShowWindowOption nCmdShow);

        [DllImport("user32.dll")]
        static extern int SetWindowText(IntPtr hWnd, string text);

        private enum ShowWindowOption : uint
        {
            Hide = 0,
            ShowNormal = 1,
            ShowMinimized = 2,
            ShowMaximized = 3,
            Maximize = 3,
            ShowNormalNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActivate = 7,
            ShowNoActivate = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimized = 11
        }

        #endregion

        private string GamePath = "";
        private ushort GameLocalPort = 19000; //TODO: Load from settings.
        private SroClient GameClient;
        private Process GameProcess;
        private bool WindowShown = true;
        private bool Disconnected = false;
        private bool FirstTimeToConnect = true;
        private Packet LoginPacket;
        private bool LoginClientless = false;
        private ClientlessConnection Clientless = new ClientlessConnection();
        private bool ClientConnected = false;
        private Dictionary<string, int> SharedVariables = new Dictionary<string, int>();

        #region MOVEMENT
        private int mov_currentX, mov_currentY;
        private int mov_stepX, mom_stepY;
        private double mom_totalTime;
        private byte mov_currentTime;
        private int mov_destX, mov_destY;
        private System.Timers.Timer mov_timer;
        #endregion

        #region DESGINER
        private TabPage tabPage2;
        private GroupBox groupBox1;
        private CheckBox checkBox1;
        private Label label3;
        private Button button1;
        private Label label2;
        private Label label1;
        private TextBox textBox2;
        private TextBox textBox1;
        private GroupBox groupBox3;
        private Button btn_logOff;
        private Button btn_killClient;
        private Button btn_reloadClient;
        private Button btn_ShowHide;
        private Button btn_startClient;
        private Button btn_chooseFolder;
        private Label label9;
        private Label label8;
        private ComboBox cmbBox_loginServer;
        private ComboBox cmbBox_divison;
        private GroupBox groupBox2;
        private Label label7;
        private TextBox textBox6;
        private Label label4;
        private TextBox textBox5;
        private CheckBox checkBox4;
        private CheckBox checkBox3;
        private CheckBox checkBox2;
        private Label label5;
        private Label label6;
        private TextBox textBox3;
        private TextBox textBox4;
        private GroupBox groupBox4;
        private Label label10;
        private TextBox txtBox_reloginCharName;
        private TextBox txtBox_reloginWaitingTime;
        private CheckBox chkBox_autoHide;
        private CheckBox chkBox_reloginWaitingTime;
        private CheckBox chkBox_autoRelogin;
        private TextBox txtBox_captchaCode;
        private CheckBox chkBox_captchaCode;
        private CheckBox chkBox_loginClientlessly;
        private CheckBox chkBox_autoStartTraining;
        private CheckBox chkBox_useReturn;
        private Button button2;
        private GroupBox groupBox5;
        private GroupBox groupBox6;
        private RichTextBox richTextBox1;
        private Label lbl_charEXP;
        private Label label21;
        private ProgressBar pBar_charEXP;
        private Label label19;
        private Label label18;
        private Label lbl_charMP;
        private Label lbl_charHP;
        private Label label15;
        private ProgressBar pBar_charMP;
        private Label label14;
        private ProgressBar pBar_charHP;
        private Label label13;
        private Label label12;
        private Label label11;
        private Label lbl_charSP;
        private Label lbl_charLevel;
        private Label lbl_charName;
        private Label lbl_charPosition;
        private Label lbl_charLocation;
        private Label lbl_charSpeed;
        private Label label17;

        public TabPage InitializeComponent()
        {
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lbl_charSpeed = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lbl_charLocation = new System.Windows.Forms.Label();
            this.lbl_charPosition = new System.Windows.Forms.Label();
            this.lbl_charSP = new System.Windows.Forms.Label();
            this.lbl_charLevel = new System.Windows.Forms.Label();
            this.lbl_charName = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.lbl_charEXP = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.pBar_charEXP = new System.Windows.Forms.ProgressBar();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.lbl_charMP = new System.Windows.Forms.Label();
            this.lbl_charHP = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.pBar_charMP = new System.Windows.Forms.ProgressBar();
            this.label14 = new System.Windows.Forms.Label();
            this.pBar_charHP = new System.Windows.Forms.ProgressBar();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.chkBox_captchaCode = new System.Windows.Forms.CheckBox();
            this.chkBox_loginClientlessly = new System.Windows.Forms.CheckBox();
            this.chkBox_autoStartTraining = new System.Windows.Forms.CheckBox();
            this.chkBox_useReturn = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.txtBox_reloginCharName = new System.Windows.Forms.TextBox();
            this.txtBox_reloginWaitingTime = new System.Windows.Forms.TextBox();
            this.chkBox_autoHide = new System.Windows.Forms.CheckBox();
            this.chkBox_reloginWaitingTime = new System.Windows.Forms.CheckBox();
            this.chkBox_autoRelogin = new System.Windows.Forms.CheckBox();
            this.txtBox_captchaCode = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btn_logOff = new System.Windows.Forms.Button();
            this.btn_killClient = new System.Windows.Forms.Button();
            this.btn_reloadClient = new System.Windows.Forms.Button();
            this.btn_ShowHide = new System.Windows.Forms.Button();
            this.btn_startClient = new System.Windows.Forms.Button();
            this.btn_chooseFolder = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbBox_loginServer = new System.Windows.Forms.ComboBox();
            this.cmbBox_divison = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabPage2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.tabPage2.Controls.Add(this.groupBox5);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage1";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(690, 577);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Main";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.lbl_charSpeed);
            this.groupBox5.Controls.Add(this.label17);
            this.groupBox5.Controls.Add(this.lbl_charLocation);
            this.groupBox5.Controls.Add(this.lbl_charPosition);
            this.groupBox5.Controls.Add(this.lbl_charSP);
            this.groupBox5.Controls.Add(this.lbl_charLevel);
            this.groupBox5.Controls.Add(this.lbl_charName);
            this.groupBox5.Controls.Add(this.groupBox6);
            this.groupBox5.Controls.Add(this.lbl_charEXP);
            this.groupBox5.Controls.Add(this.label21);
            this.groupBox5.Controls.Add(this.pBar_charEXP);
            this.groupBox5.Controls.Add(this.label19);
            this.groupBox5.Controls.Add(this.label18);
            this.groupBox5.Controls.Add(this.lbl_charMP);
            this.groupBox5.Controls.Add(this.lbl_charHP);
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Controls.Add(this.pBar_charMP);
            this.groupBox5.Controls.Add(this.label14);
            this.groupBox5.Controls.Add(this.pBar_charHP);
            this.groupBox5.Controls.Add(this.label13);
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Location = new System.Drawing.Point(8, 313);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(666, 256);
            this.groupBox5.TabIndex = 14;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Character";
            // 
            // lbl_charSpeed
            // 
            this.lbl_charSpeed.AutoSize = true;
            this.lbl_charSpeed.Location = new System.Drawing.Point(95, 234);
            this.lbl_charSpeed.Name = "lbl_charSpeed";
            this.lbl_charSpeed.Size = new System.Drawing.Size(13, 13);
            this.lbl_charSpeed.TabIndex = 34;
            this.lbl_charSpeed.Text = "0";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(11, 234);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(69, 13);
            this.label17.TabIndex = 33;
            this.label17.Text = "Move speed:";
            // 
            // lbl_charLocation
            // 
            this.lbl_charLocation.AutoSize = true;
            this.lbl_charLocation.Location = new System.Drawing.Point(81, 209);
            this.lbl_charLocation.Name = "lbl_charLocation";
            this.lbl_charLocation.Size = new System.Drawing.Size(53, 13);
            this.lbl_charLocation.TabIndex = 32;
            this.lbl_charLocation.Text = "Unknown";
            // 
            // lbl_charPosition
            // 
            this.lbl_charPosition.AutoSize = true;
            this.lbl_charPosition.Location = new System.Drawing.Point(81, 186);
            this.lbl_charPosition.Name = "lbl_charPosition";
            this.lbl_charPosition.Size = new System.Drawing.Size(24, 13);
            this.lbl_charPosition.TabIndex = 31;
            this.lbl_charPosition.Text = "0/0";
            // 
            // lbl_charSP
            // 
            this.lbl_charSP.AutoSize = true;
            this.lbl_charSP.Location = new System.Drawing.Point(57, 73);
            this.lbl_charSP.Name = "lbl_charSP";
            this.lbl_charSP.Size = new System.Drawing.Size(13, 13);
            this.lbl_charSP.TabIndex = 30;
            this.lbl_charSP.Text = "0";
            // 
            // lbl_charLevel
            // 
            this.lbl_charLevel.AutoSize = true;
            this.lbl_charLevel.Location = new System.Drawing.Point(57, 49);
            this.lbl_charLevel.Name = "lbl_charLevel";
            this.lbl_charLevel.Size = new System.Drawing.Size(13, 13);
            this.lbl_charLevel.TabIndex = 29;
            this.lbl_charLevel.Text = "0";
            // 
            // lbl_charName
            // 
            this.lbl_charName.AutoSize = true;
            this.lbl_charName.Location = new System.Drawing.Point(57, 25);
            this.lbl_charName.Name = "lbl_charName";
            this.lbl_charName.Size = new System.Drawing.Size(19, 13);
            this.lbl_charName.TabIndex = 28;
            this.lbl_charName.Text = "....";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.richTextBox1);
            this.groupBox6.Location = new System.Drawing.Point(322, 10);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(333, 240);
            this.groupBox6.TabIndex = 27;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Quests";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(3, 16);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(327, 221);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // lbl_charEXP
            // 
            this.lbl_charEXP.AutoSize = true;
            this.lbl_charEXP.Location = new System.Drawing.Point(214, 157);
            this.lbl_charEXP.Name = "lbl_charEXP";
            this.lbl_charEXP.Size = new System.Drawing.Size(21, 13);
            this.lbl_charEXP.TabIndex = 26;
            this.lbl_charEXP.Text = "0%";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(10, 157);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(28, 13);
            this.label21.TabIndex = 25;
            this.label21.Text = "EXP";
            // 
            // pBar_charEXP
            // 
            this.pBar_charEXP.Location = new System.Drawing.Point(40, 153);
            this.pBar_charEXP.Name = "pBar_charEXP";
            this.pBar_charEXP.Size = new System.Drawing.Size(164, 22);
            this.pBar_charEXP.TabIndex = 24;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(10, 209);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(51, 13);
            this.label19.TabIndex = 23;
            this.label19.Text = "Location:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(10, 185);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(47, 13);
            this.label18.TabIndex = 22;
            this.label18.Text = "Position:";
            // 
            // lbl_charMP
            // 
            this.lbl_charMP.AutoSize = true;
            this.lbl_charMP.Location = new System.Drawing.Point(214, 129);
            this.lbl_charMP.Name = "lbl_charMP";
            this.lbl_charMP.Size = new System.Drawing.Size(24, 13);
            this.lbl_charMP.TabIndex = 21;
            this.lbl_charMP.Text = "0/0";
            // 
            // lbl_charHP
            // 
            this.lbl_charHP.AutoSize = true;
            this.lbl_charHP.Location = new System.Drawing.Point(214, 101);
            this.lbl_charHP.Name = "lbl_charHP";
            this.lbl_charHP.Size = new System.Drawing.Size(24, 13);
            this.lbl_charHP.TabIndex = 20;
            this.lbl_charHP.Text = "0/0";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(10, 129);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(23, 13);
            this.label15.TabIndex = 19;
            this.label15.Text = "MP";
            // 
            // pBar_charMP
            // 
            this.pBar_charMP.Location = new System.Drawing.Point(40, 125);
            this.pBar_charMP.Name = "pBar_charMP";
            this.pBar_charMP.Size = new System.Drawing.Size(164, 22);
            this.pBar_charMP.TabIndex = 18;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(10, 101);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(25, 13);
            this.label14.TabIndex = 17;
            this.label14.Text = "HP:";
            // 
            // pBar_charHP
            // 
            this.pBar_charHP.Location = new System.Drawing.Point(40, 97);
            this.pBar_charHP.Name = "pBar_charHP";
            this.pBar_charHP.Size = new System.Drawing.Size(164, 22);
            this.pBar_charHP.TabIndex = 16;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 73);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(24, 13);
            this.label13.TabIndex = 15;
            this.label13.Text = "SP:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(10, 49);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(36, 13);
            this.label12.TabIndex = 14;
            this.label12.Text = "Level:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 25);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(41, 13);
            this.label11.TabIndex = 13;
            this.label11.Text = "Name: ";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.chkBox_captchaCode);
            this.groupBox4.Controls.Add(this.chkBox_loginClientlessly);
            this.groupBox4.Controls.Add(this.chkBox_autoStartTraining);
            this.groupBox4.Controls.Add(this.chkBox_useReturn);
            this.groupBox4.Controls.Add(this.button2);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.txtBox_reloginCharName);
            this.groupBox4.Controls.Add(this.txtBox_reloginWaitingTime);
            this.groupBox4.Controls.Add(this.chkBox_autoHide);
            this.groupBox4.Controls.Add(this.chkBox_reloginWaitingTime);
            this.groupBox4.Controls.Add(this.chkBox_autoRelogin);
            this.groupBox4.Controls.Add(this.txtBox_captchaCode);
            this.groupBox4.Location = new System.Drawing.Point(330, 117);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(351, 190);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Automatic relogin";
            // 
            // chkBox_captchaCode
            // 
            this.chkBox_captchaCode.AutoSize = true;
            this.chkBox_captchaCode.Location = new System.Drawing.Point(16, 157);
            this.chkBox_captchaCode.Name = "chkBox_captchaCode";
            this.chkBox_captchaCode.Size = new System.Drawing.Size(120, 17);
            this.chkBox_captchaCode.TabIndex = 16;
            this.chkBox_captchaCode.Text = "Enter captcha code";
            this.chkBox_captchaCode.UseVisualStyleBackColor = true;
            // 
            // chkBox_loginClientlessly
            // 
            this.chkBox_loginClientlessly.AutoSize = true;
            this.chkBox_loginClientlessly.Enabled = false;
            this.chkBox_loginClientlessly.Location = new System.Drawing.Point(16, 138);
            this.chkBox_loginClientlessly.Name = "chkBox_loginClientlessly";
            this.chkBox_loginClientlessly.Size = new System.Drawing.Size(105, 17);
            this.chkBox_loginClientlessly.TabIndex = 15;
            this.chkBox_loginClientlessly.Text = "Login clientlessly";
            this.chkBox_loginClientlessly.UseVisualStyleBackColor = true;
            // 
            // chkBox_autoStartTraining
            // 
            this.chkBox_autoStartTraining.AutoSize = true;
            this.chkBox_autoStartTraining.Location = new System.Drawing.Point(16, 120);
            this.chkBox_autoStartTraining.Name = "chkBox_autoStartTraining";
            this.chkBox_autoStartTraining.Size = new System.Drawing.Size(143, 17);
            this.chkBox_autoStartTraining.TabIndex = 14;
            this.chkBox_autoStartTraining.Text = "Start training after relogin";
            this.chkBox_autoStartTraining.UseVisualStyleBackColor = true;
            // 
            // chkBox_useReturn
            // 
            this.chkBox_useReturn.AutoSize = true;
            this.chkBox_useReturn.Location = new System.Drawing.Point(16, 102);
            this.chkBox_useReturn.Name = "chkBox_useReturn";
            this.chkBox_useReturn.Size = new System.Drawing.Size(195, 17);
            this.chkBox_useReturn.TabIndex = 13;
            this.chkBox_useReturn.Text = "Use return scroll if character in town";
            this.chkBox_useReturn.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(269, 40);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Get current";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(13, 44);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(90, 13);
            this.label10.TabIndex = 12;
            this.label10.Text = "Character to login";
            // 
            // txtBox_reloginCharName
            // 
            this.txtBox_reloginCharName.Location = new System.Drawing.Point(112, 42);
            this.txtBox_reloginCharName.Name = "txtBox_reloginCharName";
            this.txtBox_reloginCharName.Size = new System.Drawing.Size(145, 20);
            this.txtBox_reloginCharName.TabIndex = 11;
            // 
            // txtBox_reloginWaitingTime
            // 
            this.txtBox_reloginWaitingTime.Location = new System.Drawing.Point(170, 66);
            this.txtBox_reloginWaitingTime.Name = "txtBox_reloginWaitingTime";
            this.txtBox_reloginWaitingTime.Size = new System.Drawing.Size(87, 20);
            this.txtBox_reloginWaitingTime.TabIndex = 9;
            // 
            // chkBox_autoHide
            // 
            this.chkBox_autoHide.AutoSize = true;
            this.chkBox_autoHide.Location = new System.Drawing.Point(16, 84);
            this.chkBox_autoHide.Name = "chkBox_autoHide";
            this.chkBox_autoHide.Size = new System.Drawing.Size(134, 17);
            this.chkBox_autoHide.TabIndex = 8;
            this.chkBox_autoHide.Text = "Hide client after relogin";
            this.chkBox_autoHide.UseVisualStyleBackColor = true;
            // 
            // chkBox_reloginWaitingTime
            // 
            this.chkBox_reloginWaitingTime.AutoSize = true;
            this.chkBox_reloginWaitingTime.Location = new System.Drawing.Point(16, 67);
            this.chkBox_reloginWaitingTime.Name = "chkBox_reloginWaitingTime";
            this.chkBox_reloginWaitingTime.Size = new System.Drawing.Size(151, 17);
            this.chkBox_reloginWaitingTime.TabIndex = 7;
            this.chkBox_reloginWaitingTime.Text = "Waiting time before relogin";
            this.chkBox_reloginWaitingTime.UseVisualStyleBackColor = true;
            // 
            // chkBox_autoRelogin
            // 
            this.chkBox_autoRelogin.AutoSize = true;
            this.chkBox_autoRelogin.Location = new System.Drawing.Point(16, 23);
            this.chkBox_autoRelogin.Name = "chkBox_autoRelogin";
            this.chkBox_autoRelogin.Size = new System.Drawing.Size(148, 17);
            this.chkBox_autoRelogin.TabIndex = 6;
            this.chkBox_autoRelogin.Text = "Activate automatic relogin";
            this.chkBox_autoRelogin.UseVisualStyleBackColor = true;
            // 
            // txtBox_captchaCode
            // 
            this.txtBox_captchaCode.Location = new System.Drawing.Point(139, 155);
            this.txtBox_captchaCode.Name = "txtBox_captchaCode";
            this.txtBox_captchaCode.Size = new System.Drawing.Size(96, 20);
            this.txtBox_captchaCode.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btn_logOff);
            this.groupBox3.Controls.Add(this.btn_killClient);
            this.groupBox3.Controls.Add(this.btn_reloadClient);
            this.groupBox3.Controls.Add(this.btn_ShowHide);
            this.groupBox3.Controls.Add(this.btn_startClient);
            this.groupBox3.Controls.Add(this.btn_chooseFolder);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.cmbBox_loginServer);
            this.groupBox3.Controls.Add(this.cmbBox_divison);
            this.groupBox3.Location = new System.Drawing.Point(8, 117);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(314, 190);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Client control";
            // 
            // btn_logOff
            // 
            this.btn_logOff.Location = new System.Drawing.Point(190, 138);
            this.btn_logOff.Name = "btn_logOff";
            this.btn_logOff.Size = new System.Drawing.Size(115, 23);
            this.btn_logOff.TabIndex = 13;
            this.btn_logOff.Tag = "";
            this.btn_logOff.Text = "Log off";
            this.btn_logOff.UseVisualStyleBackColor = true;
            this.btn_logOff.Click += new System.EventHandler(this.btn_logOff_Click);
            // 
            // btn_killClient
            // 
            this.btn_killClient.Location = new System.Drawing.Point(190, 109);
            this.btn_killClient.Name = "btn_killClient";
            this.btn_killClient.Size = new System.Drawing.Size(115, 23);
            this.btn_killClient.TabIndex = 12;
            this.btn_killClient.Tag = "";
            this.btn_killClient.Text = "Kill Client";
            this.btn_killClient.UseVisualStyleBackColor = true;
            this.btn_killClient.Click += new System.EventHandler(this.btn_killClient_Click);
            // 
            // btn_reloadClient
            // 
            this.btn_reloadClient.Location = new System.Drawing.Point(190, 80);
            this.btn_reloadClient.Name = "btn_reloadClient";
            this.btn_reloadClient.Size = new System.Drawing.Size(115, 23);
            this.btn_reloadClient.TabIndex = 11;
            this.btn_reloadClient.Tag = "";
            this.btn_reloadClient.Text = "Re-load Client";
            this.btn_reloadClient.UseVisualStyleBackColor = true;
            this.btn_reloadClient.Click += new System.EventHandler(this.btn_reloadClient_Click);
            // 
            // btn_ShowHide
            // 
            this.btn_ShowHide.Location = new System.Drawing.Point(13, 138);
            this.btn_ShowHide.Name = "btn_ShowHide";
            this.btn_ShowHide.Size = new System.Drawing.Size(115, 23);
            this.btn_ShowHide.TabIndex = 10;
            this.btn_ShowHide.Tag = "";
            this.btn_ShowHide.Text = "Show/Hide Client";
            this.btn_ShowHide.UseVisualStyleBackColor = true;
            this.btn_ShowHide.Click += new System.EventHandler(this.btn_ShowHide_Click);
            // 
            // btn_startClient
            // 
            this.btn_startClient.Location = new System.Drawing.Point(13, 109);
            this.btn_startClient.Name = "btn_startClient";
            this.btn_startClient.Size = new System.Drawing.Size(115, 23);
            this.btn_startClient.TabIndex = 9;
            this.btn_startClient.Tag = "";
            this.btn_startClient.Text = "Start Client";
            this.btn_startClient.UseVisualStyleBackColor = true;
            this.btn_startClient.Click += new System.EventHandler(this.btn_startClient_Click);
            // 
            // btn_chooseFolder
            // 
            this.btn_chooseFolder.Location = new System.Drawing.Point(13, 80);
            this.btn_chooseFolder.Name = "btn_chooseFolder";
            this.btn_chooseFolder.Size = new System.Drawing.Size(115, 23);
            this.btn_chooseFolder.TabIndex = 7;
            this.btn_chooseFolder.Text = "Choose SRO Client";
            this.btn_chooseFolder.UseVisualStyleBackColor = true;
            this.btn_chooseFolder.Click += new System.EventHandler(this.btn_chooseFolder_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(111, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Login server";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 23);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Division";
            // 
            // cmbBox_loginServer
            // 
            this.cmbBox_loginServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBox_loginServer.FormattingEnabled = true;
            this.cmbBox_loginServer.Location = new System.Drawing.Point(114, 42);
            this.cmbBox_loginServer.Name = "cmbBox_loginServer";
            this.cmbBox_loginServer.Size = new System.Drawing.Size(191, 21);
            this.cmbBox_loginServer.TabIndex = 1;
            // 
            // cmbBox_divison
            // 
            this.cmbBox_divison.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBox_divison.FormattingEnabled = true;
            this.cmbBox_divison.Location = new System.Drawing.Point(9, 42);
            this.cmbBox_divison.Name = "cmbBox_divison";
            this.cmbBox_divison.Size = new System.Drawing.Size(99, 21);
            this.cmbBox_divison.TabIndex = 0;
            this.cmbBox_divison.SelectedIndexChanged += new System.EventHandler(this.cmbBox_divison_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.textBox6);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBox5);
            this.groupBox2.Controls.Add(this.checkBox4);
            this.groupBox2.Controls.Add(this.checkBox3);
            this.groupBox2.Controls.Add(this.checkBox2);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.textBox3);
            this.groupBox2.Controls.Add(this.textBox4);
            this.groupBox2.Enabled = false;
            this.groupBox2.Location = new System.Drawing.Point(330, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(351, 105);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Proxy";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(180, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Proxy Addr";
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(244, 20);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(100, 20);
            this.textBox6.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(180, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Proxy Port";
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(244, 47);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(100, 20);
            this.textBox5.TabIndex = 9;
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(16, 56);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(114, 17);
            this.checkBox4.TabIndex = 8;
            this.checkBox4.Text = "Authenticate proxy";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(16, 38);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(121, 17);
            this.checkBox3.TabIndex = 7;
            this.checkBox3.Text = "Use Socks v5 proxy";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(16, 21);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(122, 17);
            this.checkBox2.TabIndex = 6;
            this.checkBox2.Text = "Use Socks v4 Proxy";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(180, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Proxy pass";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 77);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Proxy user";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(244, 74);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 20);
            this.textBox3.TabIndex = 1;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(78, 74);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(96, 20);
            this.textBox4.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(8, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(314, 105);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Bot server Login";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(11, 69);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(226, 17);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "Save login details and autologin on startup";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Not logged in.";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(230, 31);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Login";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Bot password";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Bot username";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(84, 43);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(140, 20);
            this.textBox2.TabIndex = 1;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(84, 20);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(140, 20);
            this.textBox1.TabIndex = 0;

            this.tabPage2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            //this.ResumeLayout(false);
            return tabPage2;
        }
        #endregion


        #region PLUGIN
        private void cmbBox_divison_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbBox_loginServer.Items.Clear();
            foreach (var loginServer in SilkroadInformationAPI.Media.Data.ServerInfo.LoginDivisons.First(x => x.Name == (string)cmbBox_divison.SelectedItem).IPs)
            {
                cmbBox_loginServer.Items.Add(loginServer);
            }
            cmbBox_loginServer.SelectedIndex = 0;
        }

        private void LoadSettings()
        {
            if (GamePath != "")
            {
                GameClient = new SroClient(GamePath);
                GameClient.LoadData();

                foreach (var division in SilkroadInformationAPI.Media.Data.ServerInfo.LoginDivisons)
                {
                    cmbBox_divison.Items.Add(division.Name);
                }
                cmbBox_divison.SelectedIndex = 0;

                Initialize();
            }
            else
            {
                throw new Exception("Error occured during settings loading.");
            }
        }

        private void btn_chooseFolder_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "|sro_client.exe";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                GamePath = ofd.FileName.Replace("\\" + ofd.SafeFileName, "");
                if (File.Exists(GamePath + "\\Media.pk2"))
                {
                    LoadSettings();
                }
                else
                {
                    throw new Exception("Couldn't find Media.pk2 within that folder.");
                }
            }
        }

        private void Initialize()
        {
            //Relogin.
            SilkroadInformationAPI.Client.Packets.CharacterSelection.CharacterListResponse.OnCharacterListReceive += CharacterListResponse_OnCharacterListReceive;
            SilkroadInformationAPI.Client.Network.ProxyClient.OnProxyDisconnect += ProxyClient_OnDisconnect;
            SilkroadInformationAPI.Client.Network.ClientlessConnection.OnClientlessDisconnect += ProxyClient_OnDisconnect;
            SilkroadInformationAPI.Client.Packets.Gateway.ShardResponse.OnGatewayShardResponse += ShardResponse_OnGatewayShardResponse;
            SilkroadInformationAPI.Client.Packets.Gateway.CaptchaReceived.OnGatewayCaptchaReceived += CaptchaReceived_OnGatewayCaptchaReceived;

            //Save the relogin packet.
            SilkroadInformationAPI.Client.Network.ProxyClient.OnProxyClientPacketSent += ProxyClient_OnClientPacketSent;

            //Main information updated.
            SilkroadInformationAPI.Client.Packets.Character.CharacterData.OnCharacterTeleport += CharacterData_InfoUpdated;
            SilkroadInformationAPI.Client.Packets.Character.StatsUpdate.OnStatsUpdated += CharacterData_InfoUpdated;
            SilkroadInformationAPI.Client.Packets.Entity.HPMPUpdate.OnClientHPUpdate += CharacterData_InfoUpdated;
            SilkroadInformationAPI.Client.Packets.Entity.HPMPUpdate.OnClientMPUpdate += CharacterData_InfoUpdated;
            SilkroadInformationAPI.Client.Packets.Entity.HPMPUpdate.OnClientHPMPUpdate += CharacterData_InfoUpdated;
            SilkroadInformationAPI.Client.Packets.Character.ExpSpUpdate.OnClientLevelUp += CharacterData_InfoUpdated;
            SilkroadInformationAPI.Client.Packets.Character.ExpSpUpdate.OnExpGained += ExpSpUpdate_OnExpGained;

            //Movement
            SilkroadInformationAPI.Client.Packets.Entity.PositionUpdate.OnClientPositionUpdate += PositionUpdate_OnClientPositionUpdate;

            //Window title, reset settings.
            SilkroadInformationAPI.Client.Packets.CharacterSelection.CharacterJoinResponse.OnCharacterSuccessfullyJoined += CharacterJoinResponse_OnCharacterSuccessfullyJoined;

            //Clientless
            SilkroadInformationAPI.Client.Packets.Gateway.PatchResponse.OnPatchResponseSuccess += PatchResponse_OnPatchResponseSuccess;
            SilkroadInformationAPI.Client.Packets.Gateway.LoginResponse.OnGatewayLoginResponseSuccess += LoginResponse_OnGatewayLoginResponseSuccess;
            SilkroadInformationAPI.Client.Packets.Gateway.AgentAuthResponse.OnAgentAuthResponseSuccess += AgentAuthResponse_OnAgentAuthResponseSuccess;
            SilkroadInformationAPI.Client.Network.ProxyClient.OnProxyClientlessStart += ProxyClient_OnProxyClientlessStart;

        }

        private void ExpSpUpdate_OnExpGained(ulong obj)
        {
            try
            {
                pBar_charEXP.Value = (int)((float)Client.Info.CurrentExp / (float)Client.Info.MaxEXP * 100);
            }
            catch
            {
                pBar_charEXP.Value = 0;
            }
        }

        #region Position
        private void PositionUpdate_OnClientPositionUpdate()
        {
            //Stops the timer if it's already running
            mov_timer?.Stop();

            //Get current X and current Y
            mov_currentX = Int32.Parse(lbl_charPosition.Text.Split('/')[0]);
            mov_currentY = Int32.Parse(lbl_charPosition.Text.Split('/')[1]);

            //Get the X, Y difference
            mov_stepX = Client.Position.GetRealX() - mov_currentX;
            mom_stepY = Client.Position.GetRealY() - mov_currentY;

            //Calculate the total distance -> Square root of ((newX - oldX)^2 + (newY - oldY)^2)
            double distance = Math.Sqrt(Math.Pow(mov_stepX, 2) + Math.Pow(mom_stepY, 2));

            //Calculate the time it takes to travel the distance.
            mom_totalTime = (int)(distance / (Client.State.RunSpeed / 10.0));
            mom_totalTime *= 4;
            mov_currentTime = 0;
            if (mom_totalTime == 0)
            {
                lbl_charPosition.Text = mov_destX + " / " + mov_destY;
            }
            else
            {

                //Get X Step per second, and Y Step per quarter second
                mov_stepX /= (int)mom_totalTime;
                mom_stepY /= (int)mom_totalTime;

                mov_destX = Client.Position.GetRealX();
                mov_destY = Client.Position.GetRealY();



                //Starts the timer to change X, Y every quarter second to the new ones.
                mov_timer = new System.Timers.Timer(250);
                mov_timer.Elapsed += M_timer_Elapsed;
                mov_timer.Start();
            }
            //lbl_charPosition.Text = Client.Position.GetRealX() + " / " + Client.Position.GetRealY();
        }

        private void M_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (mov_currentTime == mom_totalTime)
            {
                lbl_charPosition.Text = mov_destX + " / " + mov_destY;
                mov_timer.Stop();
            }
            else
            {
                mov_currentTime++;
                lbl_charPosition.Text = (mov_currentX + (mov_currentTime * mov_stepX)) + " / " + (mov_currentY + (mov_currentTime * mom_stepY));
            }
        }
        #endregion

        private void ProxyClient_OnProxyClientlessStart()
        {
            Console.WriteLine("Switching to clientless!");
        }

        #region Clientless Login
        private void AgentAuthResponse_OnAgentAuthResponseSuccess()
        {
            if (LoginClientless)
            {
                Console.WriteLine("Connected to AgentServer, requesting character list!");
                SilkroadInformationAPI.Client.Packets.CharacterSelection.CharacterListRequest.Send();
            }
        }

        private void LoginResponse_OnGatewayLoginResponseSuccess(SilkroadInformationAPI.Client.Packets.Gateway.LoginResponseSuccessEventArgs AgentInfo)
        {
            if (LoginClientless)
            {
                Console.WriteLine("Starting agent thread.");
                GameClient.ConfigureClientlessSettings(AgentInfo.SessionID, "3dprogrammer", "555555", SilkroadInformationAPI.Media.Data.ServerInfo.Locale, SilkroadInformationAPI.Media.Data.ServerInfo.Version);
                GameClient.StartClientlessConnection(AgentInfo.AgentIP, AgentInfo.AgentPort);
            }
        }

        private void PatchResponse_OnPatchResponseSuccess()
        {
            if (LoginClientless)
            {
                SilkroadInformationAPI.Client.Packets.Gateway.ShardRequest.Send();
                Console.WriteLine("Version is correct, requesting server list.");
            }
        }
        #endregion

        private void CharacterJoinResponse_OnCharacterSuccessfullyJoined()
        {
            Console.WriteLine("Successfully logged in!");

            if (Disconnected && chkBox_autoHide.Checked && WindowShown)
                btn_ShowHide.PerformClick();

            Disconnected = false;
            LoginClientless = false;
        }

        private void CaptchaReceived_OnGatewayCaptchaReceived(uint[] obj)
        {
            if (Disconnected && chkBox_autoRelogin.Checked && chkBox_captchaCode.Checked)
            {
                SilkroadInformationAPI.Client.Packets.Gateway.CaptchaCodeRequest.Send(txtBox_captchaCode.Text);
            }
        }

        private void CharacterListResponse_OnCharacterListReceive(SilkroadInformationAPI.Client.Packets.CharacterSelection.CharacterLsitEventArgs e)
        {
            Console.WriteLine("Received char list.");
            if (e.Characters.Where(x => x.Name == txtBox_reloginCharName.Text).Count() > 0)
            {
                SilkroadInformationAPI.Client.Packets.CharacterSelection.CharacterJoinRequest.Send(
                    e.Characters.First(x => x.Name == txtBox_reloginCharName.Text).Name);
                Console.WriteLine("Selecting character.");
            }
        }

        private void ShardResponse_OnGatewayShardResponse(SilkroadInformationAPI.Client.Packets.Gateway.ShardResponseEventArgs ServerShards)
        {
            if ((LoginPacket != null && chkBox_autoRelogin.Checked) || LoginClientless)
            {
                SroClient.RemoteSecurity.Send(LoginPacket);
                Console.WriteLine("Sending login packet!");
            }
        }

        private void ProxyClient_OnClientPacketSent(Packet obj)
        {
            if (obj.Opcode == 0x6102)
            {
                LoginPacket = new Packet(obj);
            }
        }

        private void ProxyClient_OnDisconnect()
        {
            ClientConnected = false;

            //Auto relogins
            if (chkBox_autoRelogin.Checked == true && chkBox_loginClientlessly.Checked == false && LoginPacket != null)
            {
                //If disconnected before reconnecting again.
                if (Disconnected == true)
                {
                    Console.WriteLine("Disconnected before reconnect, waiting 3minutes!");
                    Task.Delay(3 * 60 * 1000).ContinueWith(x => AutoStartClient());
                }
                else
                {
                    Disconnected = true;
                    AutoStartClient();
                }
            }
            else if (chkBox_autoRelogin.Checked == true && chkBox_loginClientlessly.Checked == true && LoginPacket != null)
            {
                LoginClientless = true;
                GameClient.ConfigureClientlessSettings(0, "3dprogrammer", "555555", SilkroadInformationAPI.Media.Data.ServerInfo.Locale, SilkroadInformationAPI.Media.Data.ServerInfo.Version);
                GameClient.StartClientlessConnection(
                    SilkroadInformationAPI.Media.Data.ServerInfo.LoginDivisons[cmbBox_divison.SelectedIndex].IPs[cmbBox_loginServer.SelectedIndex],
                    SilkroadInformationAPI.Media.Data.ServerInfo.Port);
                Console.WriteLine("Starting clientless connection!");
            }

            //this.Text = Client.Info.CharacterName + " [Free bot] [Disconnected]";

        }

        private void AutoStartClient()
        {
            if (chkBox_reloginWaitingTime.Checked)
            {
                int waitingTime;
                if (int.TryParse(txtBox_reloginWaitingTime.Text, out waitingTime))
                {
                    Task.Delay(waitingTime).ContinueWith(_ => { btn_startClient.PerformClick(); });
                }
            }
            else
            {
                btn_startClient.PerformClick();
            }
        }

        private void CharacterData_InfoUpdated()
        {
            if (FirstTimeToConnect)
            {
                SetWindowText(GameProcess.MainWindowHandle, Client.Info.CharacterName + " [SRO_Client]");
                FirstTimeToConnect = false;
            }

            lbl_charName.Text = Client.Info.CharacterName;
            lbl_charLevel.Text = Client.Info.Level.ToString();
            lbl_charSP.Text = Client.Info.SP.ToString();
            lbl_charPosition.Text = Client.Position.GetRealX() + " / " + Client.Position.GetRealY();
            lbl_charHP.Text = Client.Info.CurrentHP + " / " + Client.Info.MaxHP;
            lbl_charMP.Text = Client.Info.CurrentMP + " / " + Client.Info.MaxMP;
            lbl_charEXP.Text = Client.Info.CurrentExp / Client.Info.MaxEXP * 100 + "%";
            lbl_charSpeed.Text = Client.State.RunSpeed.ToString();

            try
            {
                lbl_charLocation.Text = SilkroadInformationAPI.Media.Data.TextZoneName[Client.Position.RegionID];
            }
            catch
            {
                lbl_charLocation.Text = "DEPTH OF HELL!!";
            }

            try
            {
                pBar_charHP.Value = (int)((float)Client.Info.CurrentHP / (float)Client.Info.MaxHP * 100);
                pBar_charMP.Value = (int)((float)Client.Info.CurrentMP / (float)Client.Info.MaxMP * 100);
            }
            catch
            {
                pBar_charHP.Value = 0;
                pBar_charMP.Value = 0;
            }
        }

        private void btn_startClient_Click(object sender, EventArgs e)
        {
            if (GamePath != "" && GameClient != null && ClientConnected == false)
            {
                GameClient.StartProxyConnection(SilkroadInformationAPI.Media.Data.ServerInfo.LoginDivisons[cmbBox_divison.SelectedIndex].IPs[cmbBox_loginServer.SelectedIndex], GameLocalPort); //TODO: Add different ports
                GameProcess = GameClient.StartClient(GameLocalPort);
                ClientConnected = true;
            }
        }

        private void btn_killClient_Click(object sender, EventArgs e)
        {
            if (GameProcess != null && GameProcess.HasExited == false)
            {
                GameProcess.Kill();
                GameProcess = null;
            }
        }

        private void btn_ShowHide_Click(object sender, EventArgs e)
        {
            if (WindowShown)
            {
                ShowWindow(GameProcess.MainWindowHandle, ShowWindowOption.Hide);
            }
            else
            {
                ShowWindow(GameProcess.MainWindowHandle, ShowWindowOption.ShowNoActivate);
            }
            WindowShown = !WindowShown;
        }

        private void btn_logOff_Click(object sender, EventArgs e)
        {
            try
            {
                SilkroadInformationAPI.Client.Packets.Character.LogOut.Send();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SharedVariables["bot_status"] = 1;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SharedVariables["bot_status"] = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtBox_reloginCharName.Text = Client.Info.CharacterName;
        }

        private void btn_reloadClient_Click(object sender, EventArgs e)
        {
            MessageBox.Show("TODO!");
        }
        #endregion
    }
}
