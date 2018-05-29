using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Device.Location;

using static MineStudio.frmMain;
using System.Runtime.InteropServices;

namespace MineStudio
{
    #region vPoll Struct
    public enum ErrState
    {
        ERR_NO_ERROR,
        ERR_NO_DATA,
        ERR_DATAPATH,
        ERR_TOMANY_CONNECTIONS,
        ERR_UNKNOWN_CMD,
        ERR_UNKNOWN_UID,
        ERR_FILE_NOTDELETED,
        ERR_FILE_NOTFOUND,
        ERR_LINENO_TOBIG,
        ERR_NO_CARDREADER,
        ERR_CMD_FORMAT,
        ERR_COMPORT_NO,
        ERR_CARD_DATA,
        ERR_CARDDATA_CRC,
        ERR_CARD_POWER,
        ERR_CARD_ERASE,
        ERR_CARD_WRITE,
        ERR_CARD_TIMEOUT,
        ERR_MSGLEN_OVERFLOW,
        ERR_CRC,
        ERR_TIMEOUT,
        ERR_LAN_CONNECT,
        ERR_COMPORT_OPEN,
        MAX_ERRORS
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct accHeaderStruct
    {
        public byte command;
        public byte cardType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public byte[] name;     // 13
        public byte expYear;
        public byte expMonth;
        public byte expDay;
        public byte usageDays;         // D0 sunday, D1 monday, ....
        public byte startHour;
        public byte startMin;
        public byte endHour;
        public byte endMin;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] group; //8
        public byte spare;

        public accHeaderStruct(int init)
        {
            this.command = 0;
            this.cardType = 0;
            name = new byte[13];
            this.expYear = 0;
            this.expMonth = 0;
            this.expDay = 0;
            this.usageDays = 0;
            this.startHour = 0;
            this.startMin = 0;
            this.endHour = 0;
            this.endMin = 0;
            this.group = new byte[8];
            this.spare = 0;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct dateStruct
    {
        public Int16 year;
        public byte month;
        public byte day;

        public dateStruct(int init)
        {
            this.year = 0;
            this.month = 0;
            this.day = 0;
        }
    }
    #endregion
    
    #region DELEGATES
    public delegate void formClosedCallback(object sender, Form_Closed_EventArgs e);
    public delegate void updateTextboxCallback(TextBox textBox, string text);
    public delegate void updateCheckboxCallback(CheckBox checkbox, bool checkedState);
    public delegate void updateGroupboxCallback(GroupBox gbox, string text);
    public delegate void updateRadiobuttonCallback(RadioButton radiobutton, bool checkedState);
    public delegate void updateListViewCallback(ListView listView, flashHeaderStruct block);
    #endregion

    #region ENUMS
    public enum COMMS_STATUS
    {
        OKAY,
        TIMED_OUT,
        CRC_ERROR,
        PORT_ERROR,
        FRAME_ERROR,
        COMMAND_ERROR
    }
    public enum TRIP_RECORDS
    {
        BRAKE_EVENT = 0x81,
        ACCEL_EVENT = 0x82,
        FUEL_EVENT = 0x83,
        INPUT_EVENT = 0x85,
        USER_EVENT = 0x86,
        IDLE_EVENT = 0x87,
        POWER_EVENT = 0x88,
        TRIP_DATA = 0x89,
        OVERSPEED_EVENT = 0x8A,
        OLD_BRAKE_EVENT = 0x99,
        OLD_ACCEL_EVENT = 0x77,
        OLD_FUEL_EVENT = 0x33,
        OLD_INPUT_EVENT = 0x44,
        OLD_USER_EVENT = 0x22,
        OLD_IDLE_EVENT = 0x87,
        OLD_TRIP_DATA = 0x10,

    }
    public enum FileType
    {
        Trip,
        Log,
        GPS,
        LEACH
    }
    public enum RECORD_SIZE
    {
        BRAKE_SIZE = 80,
        ACCEL_SIZE = 80,
        FUEL_SIZE = 48,
        INPUT_SIZE = 48,
        USER_SIZE = 60,
        IDLE_SIZE = 44,
        POWER_SIZE = 44,
        TRIP_SIZE = 104,
        SPEED_SIZE = 52
    }
    #endregion

    public partial class frmMain : Form
    {
        #region vPoll Properties
        public FormClosedDelegate FormClosedCallabck;
        public FormLoadedDelegate FormLoadedCallback;
        public CardCmds cardCmds;

        public delegate void FormClosedDelegate(object parms);
        public delegate void FormLoadedDelegate(object parms);

        private string removeNullCharaters(string s)
        {
            return s.Replace("\0", "");
        }
        private void jdToDate(UInt32 jd, ref dateStruct newDate)
        {
            UInt32 l, n, i, j;

            l = jd + 68569;
            n = (4 * l) / 146097;
            l = l - (146097 * n + 3) / 4;
            i = (4000 * (l + 1)) / 1461001; //     (that's 1,461,001)
            l = l - (1461 * i) / 4 + 31;
            j = (80 * l) / 2447;
            newDate.day = (byte)(l - (2447L * j) / 80);
            l = j / 11;
            newDate.month = (byte)(j + 2 - (12 * l));
            newDate.year = (Int16)(100 * (n - 49) + i + l);
        }
        private byte[] structToByteArray(object anyStruct)
        {
            Type type = anyStruct.GetType();

            if (!type.IsLayoutSequential)
            {
                throw new Exception("Struct layout must be Sequential!");
            }
            if (type.StructLayoutAttribute.Pack != 1)
            {
                throw new Exception("Struct Pack must be 1 !");
            }

            int rawsize = Marshal.SizeOf(anyStruct);
            byte[] rawdata = new byte[rawsize];
            GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr(anyStruct, handle.AddrOfPinnedObject(), false);
            }
            catch (Exception)
            {

            }
            finally
            {
                handle.Free();
            }

            return rawdata;

        }
        private T arrayToStruct<T>(byte[] data, int offset)
        {
            Type type = typeof(T);
            if (!type.IsLayoutSequential)
            {
                throw new Exception("Struct layout must be Sequential!");
            }
            if (type.StructLayoutAttribute.Pack != 1)
            {
                throw new Exception("Struct Pack must be 1 !");
            }

            T output;
            GCHandle pinnedPacket = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                if (offset == 0)
                {
                    output = (T)Marshal.PtrToStructure(pinnedPacket.AddrOfPinnedObject(), type);
                }
                else
                {
                    output = (T)Marshal.PtrToStructure(Marshal.UnsafeAddrOfPinnedArrayElement(data, offset), type);
                }
                return output;
            }
            finally
            {
                pinnedPacket.Free();
            }
        }
        #endregion

        #region PROPERTIES
        private RS232_Comms rs232Comms = new RS232_Comms();
        private RS232_Comms comms = new RS232_Comms();
        private VMUCFG_Events vmucfg_Events = new VMUCFG_Events();
        private SerialPort currentPort = new SerialPort();
        private VMU vmu = new VMU();
        private profileStruct currentProfile = new profileStruct(0);
        private VMUCFG_Events vmucfg_events = new VMUCFG_Events();
        private GeoCoordinateWatcher watcher = null;
        
        private COMMS_STATUS status;
        public FileType fileType;

        private int RxLen = 0;
        private int tempDistCnt = 0;
        private double tempDist = 0.0;

        public string TripFilesFilter = null;
        public string LogFilesFilter = null;
        public string GPSFilsFilter = null;
        public string LeachFilter = null;
        public string filter = null;
        public string filename;
        public string shortName;

        public byte[] readBuf = new byte[32 * 3639];
        private byte[] RxBuf = new byte[256];
        public static byte MAX_POINTS = 60;
        public static byte VEHICLE_ID_SIZE = 11;
        public static byte DRIVER_ID_SIZE = 13;
        public static byte JOB_NUMBER_SIZE = 12;
        #endregion

        #region Constructors
        public frmMain()
        {
            InitializeComponent();
        }
        public frmMain(RS232_Comms comms)
        {
            InitializeComponent();
            this.comms = comms;
        }
        public frmMain(RS232_Comms comms, VMU vmu, VMUCFG_Events e)
        {
            InitializeComponent();
            this.rs232Comms = comms;
            this.vmu = vmu;
            this.vmucfg_Events = e;
            this.currentPort = this.rs232Comms.getConnectedVMUPort(0);

            //CheckBox c = getCheckBox("checkBox1");
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            this.cardCmds = new CardCmds();
            //FormLoadedCallback(this.Name);

            timerSystem.Start();
            latitudeTextBox.Enabled = false;
            longitudeTextBox.Enabled = false;

            this.toolStripProgressBar1.Visible = true;
            this.versionNumberLabel.Visible = false;
            this.connectionStatusLabel.Visible = false;

            search();

            /*if(!backgroundWorkerCOMMS.IsBusy)
            {
                backgroundWorkerCOMMS.RunWorkerAsync();
            }*/
        }
        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.vmucfg_events.fireFormClosedEvent(this.Name);
        }
        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            #region Toolstrip
            if (e.Control && e.KeyCode == Keys.P)
            {
                e.SuppressKeyPress = true;
                btnProgramming.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.M)
            {
                e.SuppressKeyPress = true;
                btnMemoryFlash.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.X)
            {
                e.SuppressKeyPress = true;
                btnExit.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.A)
            {
                e.SuppressKeyPress = true;
                btnInstructions.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.S)
            {
                e.SuppressKeyPress = true;
                btnShortcuts.PerformClick();
            }
            #endregion

            #region Dallas
            else if (e.Control && e.KeyCode == Keys.H)
            {
                e.SuppressKeyPress = true;
                btnReadHeader.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.Z)
            {
                e.SuppressKeyPress = true;
                btnBuzzer.PerformClick();
            }
            #endregion

            #region Counts
            else if (e.Control && e.KeyCode == Keys.V)
            {
                e.SuppressKeyPress = true;
                btnVmuRefresh.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.N)
            {
                e.SuppressKeyPress = true;
                btnLedON.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.F)
            {
                e.SuppressKeyPress = true;
                btnLedOFF.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.G)
            {
                e.SuppressKeyPress = true;
                btnGPS.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                e.SuppressKeyPress = true;
                btnReconnect.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.D)
            {
                e.SuppressKeyPress = true;
                btnDisconnect.PerformClick();
            }
            #endregion

            #region Time
            else if (e.Control && e.KeyCode == Keys.T)
            {
                e.SuppressKeyPress = true;
                btnSetTime.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.R)
            {
                e.SuppressKeyPress = true;
                btnReadTime.PerformClick();
            }
            #endregion

            #region Limits
            else if (e.Control && e.KeyCode == Keys.W)
            {
                e.SuppressKeyPress = true;
                btnWriteVMU.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.E)
            {
                e.SuppressKeyPress = true;
                btnReadVMU.PerformClick();
            }
            #endregion
        }
        private void timerSystem_Tick(object sender, EventArgs e)
        {
            timerSystem.Start();
            //lblTime.ForeColor = Color.CornflowerBlue;
            lblTime.Text = DateTime.Now.ToString("yyyy/MM/dd     HH:mm:ss");
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Stop();
            updateCountsData();
            updateTripData();
            this.timer1.Start();
        }
        #endregion

        #region Button & Textbox Events
        private void btnProgramming_Click(object sender, EventArgs e)
        {
            frmProgram frmProgram = new frmProgram();
            frmProgram.Show();
        }
        private void btnMemoryFlash_Click(object sender, EventArgs e)
        {
            if(connectionStatusLabel.Text == "No VMU Found" || connectionStatusLabel.Text == "No Dallas Found")
            {
                MessageBox.Show("Cannot read the Data Flash if there is no VMU connected!", "WARNING!");
                return;
            }

            frmDataFlash frmDataFlash = new frmDataFlash(this.rs232Comms, this.vmu, this.vmucfg_Events);
            frmDataFlash.Show();
        }
        private void btnInstructions_Click(object sender, EventArgs e)
        {
            try
            {
                string file_name = "ReadMe.txt";
                file_name = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\GJS Technologies\\GJS Studio\\" + file_name;
                //file_name = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "GJS Technologies" + file_name;
                StreamReader sr = new StreamReader(file_name);
                if (File.Exists(file_name))
                {
                    System.Diagnostics.Process.Start(file_name);
                }

                else
                {
                    MessageBox.Show("Could not find the ReadMe.txt file in your folder!", "File Error!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error finding file!");
            }
        }
        private void btnShortcuts_Click(object sender, EventArgs e)
        {
            try
            {
                string file_name = "Shortcuts.txt";
                file_name = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\GJS Technologies\\GJS Studio\\" + file_name;
                //file_name = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "GJS Technologies" + file_name;
                StreamReader sr = new StreamReader(file_name);
                if (File.Exists(file_name))
                {
                    System.Diagnostics.Process.Start(file_name);
                }

                else
                {
                    MessageBox.Show("Could not find the Shortcuts.txt file in your folder!", "File Error!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error finding file!");
            }
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnReadHeader_Click(object sender, EventArgs e)
        {
            if (!backgroundWorkerDallas.IsBusy)
            {
                backgroundWorkerDallas.RunWorkerAsync();
            }
        }
        private void btnBuzzer_Click(object sender, EventArgs e)
        {
            this.cardCmds.openPort();

            int ret = this.cardCmds.buzzer(1);
            if (ret > 0)
            {
                MessageBox.Show("No Comms with reader!", "Card Reader Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            Thread.Sleep(400);
            this.cardCmds.buzzer(0);
            this.cardCmds.closePort();
        }

        private void btnVmuRefresh_Click(object sender, EventArgs e)
        {
            if(connectionStatusLabel.Text == "No Dallas Found" || connectionStatusLabel.Text == "No VMU Found")
            {
                MessageBox.Show("Make sure the VMU is connected to the PC\nand that it is powered on, then:\n\n" +
                    "- Click RECONNECT again to search for the VMU!", "WARNING!");
            }

            else
            {
                if (!this.readVMUBackgroundWorker.IsBusy)
                {
                    this.readVMUBackgroundWorker.RunWorkerAsync();
                    this.timer1.Enabled = true;
                    timer1.Start();
                }
            }
        }
        private void btnLedON_Click(object sender, EventArgs e)
        {
            this.rs232Comms.turnONOutputs(this.currentPort);
        }
        private void btnLedOFF_Click(object sender, EventArgs e)
        {
            this.rs232Comms.turnOFFOutputs(this.currentPort);
        }
        private void btnGPS_Click(object sender, EventArgs e)
        {
            latitudeTextBox.Enabled = true;
            longitudeTextBox.Enabled = true;
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
            watcher.PositionChanged += watcher_PositionChanged;
            watcher.Start();
        }
        private void btnReconnect_Click(object sender, EventArgs e)
        {
            if (connectionStatusLabel.Text != "No VMU Found")
            {
                Application.Restart();
                Environment.Exit(0);
            }

            this.search();
        }
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            clearText(true);
            this.rs232Comms.closePorts();
            timer1.Stop();
            setEnable(false);
            connectionStatusLabel.Text = "Disconnected!";
            connectionStatusLabel.ForeColor = Color.Black;
        }

        private void btnSetTime_Click(object sender, EventArgs e)
        {
            if(connectionStatusLabel.Text == "No VMU Found")
            {
                MessageBox.Show("No VMU connected!", "Error!");
                return;
            }
            else
            {
                this.status = this.rs232Comms.setVMUTime(this.currentPort);
                btnVmuRefresh.PerformClick();
                btnReadTime.PerformClick();
            }
        }
        private void btnReadTime_Click(object sender, EventArgs e)
        {
            if(connectionStatusLabel.Text == "No VMU Found")
            {
                MessageBox.Show("No VMU connected!", "Error!");
                return;
            }
            if (!this.readVMUBackgroundWorker.IsBusy)
            {
                this.toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
                this.toolStripProgressBar1.Visible = true;
                setEnable(false);
                this.readVMUBackgroundWorker.RunWorkerAsync();
            }
        }

        private void btnReadVMU_Click(object sender, EventArgs e)
        {
            if (!this.readVMUBackgroundWorker.IsBusy)
            {
                this.toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
                this.toolStripProgressBar1.Visible = true;
                this.readVMUBackgroundWorker.RunWorkerAsync();
            }
        }
        private void btnWriteVMU_Click(object sender, EventArgs e)
        {
            loadProfileData(getProfileData(), ref this.currentProfile);
            if (!this.writeBackgroundWorker.IsBusy)
            {
                this.toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
                this.toolStripProgressBar1.Visible = true;
                this.writeBackgroundWorker.RunWorkerAsync();
                setEnable(false);
            }
        }
        
        private void latitudeTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.latitudeTextBox.Text == "Waiting for GPS Lock") this.latitudeTextBox.ForeColor = Color.Maroon;
            else this.latitudeTextBox.ForeColor = Color.Black;
        }
        private void longitudeTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.longitudeTextBox.Text == "Waiting for GPS Lock") this.longitudeTextBox.ForeColor = Color.Maroon;
            else this.longitudeTextBox.ForeColor = Color.Black;
        }

        private void calRPMTextBox_TextChanged(object sender, EventArgs e)
        {
            this.validateNumericText(this.calRPMTextBox);
        }
        private void calDistTextBox_TextChanged(object sender, EventArgs e)
        {
            this.validateNumericText(this.calDistTextBox);
        }

        private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            latitudeTextBox.Text = e.Position.Location.Latitude.ToString();
            longitudeTextBox.Text = e.Position.Location.Longitude.ToString();
        }
        #endregion

        #region Dallas Backgournd Worker
        private void backgroundWorkerDallas_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker dallasWorker = sender as BackgroundWorker;

            ErrState state;
            byte cardType = 0;
            byte[] snr = new byte[16];
            byte[] buf = new byte[200];
            int bytesRead = 0;
            string[] days = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

            this.cardCmds.openPort();

            state = this.cardCmds.getCardType(snr, ref cardType); // 0-no button, 1-DS1977, 2-DS1996, 3-DS1992

            if (state != ErrState.ERR_NO_ERROR)
            {
                MessageBox.Show("No Comms with reader!", "Card Reader Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.cardCmds.closePort();
                return;
            }
            dallasWorker.ReportProgress(50);
            if (cardType == 3)
            {
                state = this.cardCmds.readCardData(0, 32, buf, ref bytesRead);
                if (bytesRead == 32)
                {
                    StringBuilder sb = new StringBuilder();
                    byte[] name = new byte[16];
                    accHeaderStruct accHdr = arrayToStruct<accHeaderStruct>(buf, 0);
                    int i;

                    //sb.AppendFormat("Command   : {0:X2}H\n", accHdr.command);
                    //sb.AppendFormat("Card Type : {0:X2}H\n", accHdr.cardType);
                    for (i = 12; i > 0; i--)
                    {
                        if (accHdr.name[i] == 0xFF) accHdr.name[i] = 0x00;
                        name[i] = accHdr.name[i];
                    }
                    name[i] = accHdr.name[i];   // must copy name[0]
                    sb.AppendFormat("Name      : {0:G}\n", removeNullCharaters(Encoding.ASCII.GetString(name)));

                    sb.AppendFormat("Exp date  : {0:0000}/{1:00}/{2:00}\n", accHdr.expYear + 2000,
                     accHdr.expMonth, accHdr.expDay);

                    //sb.AppendFormat("Usage days: {0:X2}H\n", accHdr.usageDays);

                    sb.AppendFormat("Time      : from {0:00}H{1:00} to {2:00}H{3:00}\n",
                            accHdr.startHour, accHdr.startMin, accHdr.endHour, accHdr.endMin);

                    //sb.AppendFormat("Groups 00 to 07 : {0:X2}H\n", accHdr.group[0]);
                    //sb.AppendFormat("Groups 08 to 15 : {0:X2}H\n", accHdr.group[1]);
                    //sb.AppendFormat("Groups 16 to 23 : {0:X2}H\n", accHdr.group[2]);
                    //sb.AppendFormat("Groups 24 to 31 : {0:X2}H\n", accHdr.group[3]);
                    //sb.AppendFormat("Groups 32 to 39 : {0:X2}H\n", accHdr.group[4]);
                    //sb.AppendFormat("Groups 40 to 47 : {0:X2}H\n", accHdr.group[5]);
                    //sb.AppendFormat("Groups 48 to 55 : {0:X2}H\n", accHdr.group[6]);
                    //sb.AppendFormat("Groups 56 to 63 : {0:X2}H\n", accHdr.group[7]);

                    // show the serial number, added Ver1.1s  GJS
                    //sb.AppendFormat("Serial Number   : {0:X2} {0:X2} {0:X2} {0:X2} {0:X2} {0:X2}\n",
                    //     snr[0], snr[1], snr[2], snr[3], snr[4], snr[5]);

                    string s = sb.ToString();
                    //richTextBox1.Text = s;

                    //MessageBox.Show(Application.OpenForms.OfType<frmMain>().First<frmMain>(),
                    //   s, "Card Reader Header ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    { MessageBox.Show("No Comms with reader!", "Card Reader Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }

                this.cardCmds.closePort();
                return;
            }

            dallasWorker.ReportProgress(100);
            Array.Clear(buf, 0, buf.Length);
            state = this.cardCmds.readCardData(0, 200, buf, ref bytesRead);         //(0, buf, 200);
            if (state != ErrState.ERR_NO_ERROR)
            {
                MessageBox.Show("No Comms with reader!", "Card Reader Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.cardCmds.closePort();
                return;
            }

            //dallasWorker.ReportProgress(100);
            if (bytesRead == 200)
            {
                StringBuilder sb = new StringBuilder();
                sb.Capacity = Int16.MaxValue;
                //sb.AppendFormat("Command    : {0:X2}H\n", buf[0]);
                //sb.AppendFormat("Card Type  : {0:X2}H\n", buf[1]);
                //if ((buf[1] & 0x01) > 0) sb.Append("            Don't save data to card\n");
                //else sb.Append("            Save data to card\n");
                //if ((buf[1] & 0x02) > 0) sb.Append("            Don't save vehicle ID to card\n");
                //else sb.Append("            Save vehicle ID to card\n");
                //if ((buf[1] & 0x04) > 0) sb.Append("            Use extended group\n");
                //else sb.Append("            Don't use extended groups\n");
                sb.AppendFormat("Driver ID  : {0:G}\n", removeNullCharaters(Encoding.ASCII.GetString(buf, 2, 13)));
                sb.AppendFormat("Expire date : {0:0000}/{1:00}/{2:00}\n", buf[15] + 2000, buf[16], buf[17]);

                sb.AppendFormat("Start Time : {0:00}H{1:00}\n", buf[32], buf[33]);
                sb.AppendFormat("End Time : {0:00}H{1:00}\n", buf[34], buf[35]);


                /*sb.Append("Usage Days : ");
                UInt16 mask = 0x01;
                for (int i = 0; i < 7; i++)
                {

                    if ((mask & buf[31]) > 0)
                    {
                        sb.AppendFormat("{0:G},", days[i]);
                    }
                    mask = (UInt16)(mask << 1);
                }

                UInt16 group = (UInt16)(buf[18] * 256 + buf[19]);
                //sb.AppendFormat("\nVehicle Types : {0:X4}H\n            ", group);
                mask = 0x0001;
                for (int i = 0; i < 16; i++)
                {
                    if (i > 0)
                    {
                        if ((group & mask) > 0) sb.AppendFormat(" {0:G},", i);
                    }
                    else
                    { // do specific group
                        if ((group & mask) > 0) sb.Append("Specific,");
                    }
                    mask = (UInt16)(mask << 1);

                }*/

                /*group = (UInt16)(buf[108] * 256 + buf[109]);
                sb.AppendFormat("\nExt. Types 1 : {0:X4}H\n            ", group);
                mask = 0x0001;
                for (int i = 0; i < 16; i++)
                {
                    if ((group & mask) > 0) sb.AppendFormat(" {0:G},", i + 16);
                    mask = (UInt16)(mask << 1);
                }

                group = (UInt16)(buf[110] * 256 + buf[111]);
                sb.AppendFormat("\nExt. Types 2 : {0:X4}H\n            ", group);
                mask = 0x0001;
                for (int i = 0; i < 16; i++)
                {
                    if ((group & mask) > 0) sb.AppendFormat(" {0:G},", i + 32);
                    mask = (UInt16)(mask << 1);
                }

                group = (UInt16)(buf[174] * 256 + buf[175]);
                sb.AppendFormat("\nExt. Types 3 : {0:X4}H\n            ", group);
                mask = 0x0001;
                for (int i = 0; i < 16; i++)
                {
                    if ((group & mask) > 0) sb.AppendFormat(" {0:G},", i + 48);
                    mask = (UInt16)(mask << 1);
                }

                group = (UInt16)(buf[176] * 256 + buf[177]);
                sb.AppendFormat("\nExt. Types 4 : {0:X4}H\n            ", group);
                mask = 0x0001;
                for (int i = 0; i < 16; i++)
                {
                    if ((group & mask) > 0) sb.AppendFormat(" {0:G},", i + 64);
                    mask = (UInt16)(mask << 1);
                }

                group = (UInt16)(buf[178] * 256 + buf[179]);
                sb.AppendFormat("\nExt. Types 5 : {0:X4}H\n            ", group);
                mask = 0x0001;
                for (int i = 0; i < 16; i++)
                {
                    if ((group & mask) > 0) sb.AppendFormat(" {0:G},", i + 80);
                    mask = (UInt16)(mask << 1);
                }*/

                /*sb.AppendFormat("\nSpec. Vehicle : {0:G}\n", removeNullCharaters(Encoding.ASCII.GetString(buf, 20, 11)));

                sb.AppendFormat("Card Status : {0:X2}H\n", buf[180]);

                sb.AppendFormat("Card Empty Ptr : {0:X2}{1:X2}H\n", buf[184], buf[185]);

                sb.AppendFormat("Card Command Ptr : {0:X2}{1:X2}H\n", buf[186], buf[187]);

                byte[] cStr = new byte[32];
                Array.Copy(buf, 188, cStr, 0, 11);
                for (int i = 13; i > 0; i--)
                {
                    if (cStr[i - 1] == 0xFF) { cStr[i - 1] = 0x00; }
                }
                sb.AppendFormat("Last Vehicle : {0:G}\n", removeNullCharaters(Encoding.ASCII.GetString(cStr, 0, 11)));

                for (int i = 0; i < 6; i++)               // do the 6 Job numbers
                {
                    Array.Clear(cStr, 0, 32);
                    Array.Copy(buf, (36 + (i * 12)), cStr, 0, 12);
                    for (int j = 14; j > 0; j--)
                    {
                        if (cStr[j - 1] == 0xFF) { cStr[j - 1] = 0x00; }
                    }
                    sb.AppendFormat("Job #{0:G} : {1:G}\n", i + 1, removeNullCharaters(Encoding.ASCII.GetString(cStr, 0, 12)));
                }


                for (int i = 0; i < 12; i++)          // do the 12 auto expire dates
                {
                    if ((buf[112 + (i * 3)] == 0xFF) || (buf[112 + (i * 3)] == 0x00))
                    {
                        sb.AppendFormat("Auto Expire #{0:G} : \n", i + 1);
                        continue;
                    }
                    dateStruct d = new dateStruct(0);
                    UInt32 mjd = (UInt32)(2400000 + (buf[113 + (i * 3)] * 256) + buf[114 + (i * 3)]);
                    jdToDate(mjd, ref d);
                    sb.AppendFormat("Auto Expire #{0:G} : Group {1:00} - {2:0000}/{3:00}/{4:00}\n",
                    i + 1, buf[112 + (i * 3)], d.year, d.month, d.day);
                }*/

                if (cardType > 0)
                {
                    if (cardType == 1) sb.Append("Dallse type : DS1977\n");
                    else if (cardType == 2) sb.Append("Dallse type : DS1996\n");
                    else sb.Append("Dallse type : unknown\n");
                    sb.AppendFormat("Dallas snr  : {0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}\n",
                                snr[0], snr[1], snr[2], snr[3], snr[4], snr[5]);
                }

                string s = sb.ToString();

                this.Invoke((MethodInvoker)delegate
                {
                    richTextBox1.Text = s;
                });

                //MessageBox.Show(Application.OpenForms.OfType<frmMain>().First<frmMain>(),
                //        s, "Card Reader Header ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


            this.cardCmds.closePort();
        }
        private void backgroundWorkerDallas_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar2.Value = e.ProgressPercentage;
        }
        private void backgroundWorkerDallas_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Thread.Sleep(3000);
            richTextBox1.Text = "";
            toolStripProgressBar2.Value = 0;
        }
        #endregion

        #region VMU Background Worker
        private void searchForVMUBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker searchWorker = sender as BackgroundWorker;

            if (searchWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            e.Result = (object)this.rs232Comms.searchForVMUs();
        }
        private void searchForVMUBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.toolStripStatusLabel.Text = "Done";
            this.toolStripProgressBar1.Style = ProgressBarStyle.Blocks;
            this.toolStripProgressBar1.Visible = true;
            this.connectionStatusLabel.Visible = true;

            if (!e.Cancelled)
            {
                if ((int)e.Result == 0)
                {
                    this.currentPort = this.rs232Comms.getConnectedVMUPort(0);
                    this.status = this.rs232Comms.getVMUVersion(this.currentPort, ref vmu.vmuConst);
                    if (this.status == COMMS_STATUS.OKAY)
                    {
                        this.versionNumberLabel.Text = string.Format("{0,-5:G}: {1:G}", "Version", vmu.vmuConst.version);
                        this.versionNumberLabel.Visible = true;
                        setEnable(true);
                    }
                    
                    this.connectionStatusLabel.Text = string.Format("connected on {0:G}", this.currentPort.PortName);

                    this.status = this.rs232Comms.getVMUSerialNum(this.currentPort, ref vmu.vmuConst);
                    
                    setEnable(true);
                    btnVmuRefresh.PerformClick();
                }
                else
                {
                    this.versionNumberLabel.Text = "";
                    this.versionNumberLabel.Visible = false;
                    this.connectionStatusLabel.Text = "No VMU Found";
                    setEnable(false);
                }
            }
            else
            {
                this.versionNumberLabel.Text = "";
                this.versionNumberLabel.Visible = false;
                this.connectionStatusLabel.Text = "No VMU Found";
                setEnable(false);
            }
        }

        private void writeBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker writeWorker = sender as BackgroundWorker;
            if (writeWorker.CancellationPending) { e.Cancel = true; return; }

            writeWorker.ReportProgress(0, "Writing VMU Calibration");
            this.status = this.rs232Comms.setVMUCalibration(this.currentPort, this.currentProfile);
            if (this.status != COMMS_STATUS.OKAY)
            {
                MessageBox.Show("Failed to Write VMU Calibration", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else { Thread.Sleep(2000); }

            writeWorker.ReportProgress(0, "Writing VMU Limits");
            this.status = this.rs232Comms.setVMULimits(this.currentPort, this.currentProfile);
            if (this.status != COMMS_STATUS.OKAY)
            {
                MessageBox.Show("Failed to Write VMU Limits", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else { Thread.Sleep(2000); }

            writeWorker.ReportProgress(0, "Writing Idle Time Limit");
            this.status = this.rs232Comms.setVMUIdleLimit(this.currentPort, this.currentProfile);
            if (this.status != COMMS_STATUS.OKAY)
            {
                MessageBox.Show("Failed to Write VMU Idle Time Limit", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else { Thread.Sleep(2000); }

            writeWorker.ReportProgress(0, "Writing VMU Vehicle Constants");
            this.status = this.rs232Comms.setVMUConstatnts(this.currentPort, this.currentProfile);
            if (this.status != COMMS_STATUS.OKAY)
            {
                MessageBox.Show("Failed to Write VMU Vehicle Constants", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else { Thread.Sleep(2000); }

            writeWorker.ReportProgress(0, "Writing VMU Extended Groups");
            this.status = this.rs232Comms.setVMUExtendedGroups(this.currentPort, this.currentProfile);
            if (this.status != COMMS_STATUS.OKAY)
            {
                MessageBox.Show("Failed to Write VMU Extended Groups", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void writeBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.toolStripStatusLabel.Text = "Done";
            //setEnable(true);
            this.toolStripProgressBar1.Style = ProgressBarStyle.Blocks;
            this.toolStripProgressBar1.Visible = false;
        }

        private void readVMUBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker readWorker = sender as BackgroundWorker;

            if (readWorker.CancellationPending) { e.Cancel = true; return; }

            this.vmu.vmuConst = new VMUConstStruct(0);

            //calibrattion====================================================================
            readWorker.ReportProgress(33, "Reading Calibration");
            
            status = this.rs232Comms.getVMUCalibration(this.currentPort, ref this.vmu.vmuConst);
            if (status == COMMS_STATUS.OKAY)
            {
                updateTextbox(this.calRPMTextBox, this.vmu.vmuConst.calRPM.ToString());
                updateTextbox(this.calDistTextBox, this.vmu.vmuConst.calDistance.ToString());
            }
            else
            {
                updateTextbox(this.calRPMTextBox, "");
                updateTextbox(this.calDistTextBox, "");

            }
            if (readWorker.CancellationPending) { e.Cancel = true; return; }
            //Limits====================================================================
            readWorker.ReportProgress(66, "Reading Limits");
            
            status = this.rs232Comms.getVMULimits(this.currentPort, ref this.vmu.vmuConst);
            if (status == COMMS_STATUS.OKAY)
            {
                updateTextbox(this.limRPMTextBox, this.vmu.vmuConst.maxRPM.ToString());
                updateTextbox(this.limSpeedTextBox, this.vmu.vmuConst.maxSpeed.ToString());
                updateTextbox(this.limBrakeTextBox, this.vmu.vmuConst.maxBrake.ToString());
                updateTextbox(this.limAccelTextBox, this.vmu.vmuConst.maxAccel.ToString());
            }
            else
            {
                updateTextbox(this.limRPMTextBox, "");
                updateTextbox(this.limSpeedTextBox, "");
                updateTextbox(this.limBrakeTextBox, "");
                updateTextbox(this.limAccelTextBox, "");

            }
            if (readWorker.CancellationPending) { e.Cancel = true; return; }
            //Time====================================================================
            readWorker.ReportProgress(100, "Reading Time");
            
            status = this.rs232Comms.getVMUTime(this.currentPort, ref this.vmu.currentDate, ref this.vmu.currentTime);
            DateTime pcTime = DateTime.Now;
            if (status == COMMS_STATUS.OKAY)
            {
                string t = string.Format("{0:00}:{1:00}:{2:00}",
                    this.vmu.currentTime.hour, this.vmu.currentTime.minute, this.vmu.currentTime.second);
                string d = string.Format("{0:0000}/{1:00}/{2:00}",
                    this.vmu.currentDate.year, this.vmu.currentDate.month, this.vmu.currentDate.day);
                updateTextbox(this.vmuTimeTextBox, d + "    " + t);
            }
            else
            {
                updateTextbox(this.vmuTimeTextBox, "");
            }
            updateTextbox(this.pcTimeTextBox, string.Format("{0:0000}/{1:00}/{2:00}    {3:00}:{4:00}:{5:00}",
                pcTime.Year, pcTime.Month, pcTime.Day,
                pcTime.Hour, pcTime.Minute, pcTime.Second));
        }
        private void readVMUBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.toolStripStatusLabel.Text = (string)e.UserState;
            toolStripProgressBar1.Value = e.ProgressPercentage;
        }
        private void readVMUBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.toolStripStatusLabel.Text = "Done";
            setEnable(true);
            this.toolStripProgressBar1.Visible = false;
        }
        #endregion

        #region Read VMU Methods
        private void search()
        {
            if (!this.searchForVMUBackgroundWorker.IsBusy)
            {
                this.toolStripStatusLabel.Text = "Searching for VMU Connected to this machine";
                this.toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
                this.toolStripProgressBar1.Visible = true;
                this.searchForVMUBackgroundWorker.RunWorkerAsync();
            }
        }
        private void updateTextbox(TextBox textBox, string text)
        {
            if (textBox.InvokeRequired)
            {
                updateTextboxCallback d = new updateTextboxCallback(updateTextbox);
                this.Invoke(d, new object[] { textBox, text });
            }
            else
            {
                textBox.Text = text;
            }
            //MessageBox.Show("Update Textbox!");
            

        }
        private void updateTripData()
        {
            this.status = this.rs232Comms.getVMUCurrentTripStruct(this.currentPort, ref this.vmu.currentTrip);

            if (this.status == COMMS_STATUS.OKAY)
            {
                this.status = this.rs232Comms.getVMU_RSTDO_data(this.currentPort, ref this.vmu.rstdoData);

                if (this.status == COMMS_STATUS.OKAY)
                {
                    updateTextbox(this.odometerTextBox, string.Format("{0:0.00}", (((double)this.vmu.rstdoData.oddoPlusCurDist) / 10)));
                    updateTextbox(this.latitudeTextBox, (this.vmu.currentTrip.gpsLat == 0.0) ? "Waiting for GPS Lock" : this.vmu.currentTrip.gpsLat.ToString());
                    updateTextbox(this.longitudeTextBox, (this.vmu.currentTrip.gpsLong == 0.0) ? "Waiting for GPS Lock" : this.vmu.currentTrip.gpsLong.ToString());
                    #region Unused Code
                    /*updateTextbox(this.driverIDTextBox, this.vmu.currentTrip.driverID);
                    updateTextbox(this.jobNumberTextBox, this.vmu.currentTrip.jobNumber);
                    updateTextbox(this.tripNumberTextBox, this.vmu.vmuConst.tripNumber.ToString());
                    updateTextbox(this.vehicleTextBox, this.vmu.currentTrip.vehicleID);
                    updateTextbox(this.currentRPMTextBox, this.vmu.countsData.currentRPM.ToString());
                    updateTextbox(this.maxRPMTextBox, this.vmu.currentTrip.maxRPM.ToString());
                    updateTextbox(this.currentSpeedTextBox, this.vmu.countsData.currentSpeed.ToString());
                    updateTextbox(this.maxSpeedTextBox, this.vmu.currentTrip.maxSpeed.ToString());
                    updateTextbox(this.tripDistTextBox, string.Format("{0:0.00}", (((double)this.vmu.countsData.currentDistance / 10))));
                    int hr = (this.vmu.countsData.totalTime) / 3600;
                    int min = ((this.vmu.countsData.totalTime) / 60) % 60;
                    int sec = (this.vmu.countsData.totalTime) % 60;
                    updateTextbox(this.tTripTimeTextBox, string.Format("{0:00}Hrs {1:00}Mins {2:00}Sec"
                        , hr, min, sec));
                    updateTextbox(this.startTimeTextBox, string.Format("{0:0000}/{1:00}/{2:00}  {3:00}:{4:00}:{5:00}",
                        this.vmu.currentTrip.startDate.year, this.vmu.currentTrip.startDate.month,
                        this.vmu.currentTrip.startDate.day,
                        this.vmu.currentTrip.startTime.hour, this.vmu.currentTrip.startTime.minute,
                        this.vmu.currentTrip.startTime.second));

                    updateTextbox(this.currentTimeTextBox, string.Format("{0:0000}/{1:00}/{2:00}  {3:00}:{4:00}:{5:00}",
                        this.vmu.rstdoData.currentDate.year, this.vmu.rstdoData.currentDate.month,
                        this.vmu.rstdoData.currentDate.day,
                        this.vmu.rstdoData.currentTime.hour, this.vmu.rstdoData.currentTime.minute,
                        this.vmu.rstdoData.currentTime.second));
                    updateTextbox(this.inputCountsTextBox, this.vmu.currentTrip.eventCnt.ToString());
                    updateTextbox(this.timerCountsTextBox, this.vmu.currentTrip.timerCnt.ToString());
                    updateTextbox(this.accelBrakesTextBox, string.Format("{0:G}/{1:G}",
                        this.vmu.currentTrip.harshAccel, this.vmu.currentTrip.harshBrake));*/
                    #endregion
                }
            }
            //MessageBox.Show("Update Trip Data!");
            DateTime t1 = DateTime.Now;
            DateTime t2 = Convert.ToDateTime(txtCurrentTimeCounts.Text);
            TimeSpan ts = t1.Subtract(t2);
            var tss = int.Parse(ts.ToString(@"ss"));
            
            if (tss>=3)
            {
                MessageBox.Show("Connect VMU, then hit OK!", "VMU DISCONNECTED!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                btnReconnect.PerformClick();
            }
        }
        private void updateCountsData()
        {
            
            this.status = this.rs232Comms.getVMUCountData(this.currentPort, ref this.vmu.countsData);
            
            if (this.status == COMMS_STATUS.OKAY)
            {
                this.status = this.rs232Comms.getVMUCalibration(this.currentPort, ref this.vmu.vmuConst);
                if (this.status == COMMS_STATUS.OKAY)
                {
                    updateTextbox(this.calRPMTextBox, this.vmu.vmuConst.calRPM.ToString());
                    updateTextbox(this.calDistTextBox, this.vmu.vmuConst.calDistance.ToString());
                }
                else
                {

                }
                
                updateTextbox(this.rpmTextBox, this.vmu.countsData.currentRPM.ToString());
                updateTextbox(this.speedTextBox, this.vmu.countsData.currentSpeed.ToString());
                this.tempDistCnt += this.vmu.countsData.speedCount;
                updateTextbox(this.distCountTextBox, this.tempDistCnt.ToString());
                try
                {
                    this.tempDist += ((double)(this.vmu.countsData.speedCount * 100)) / this.vmu.vmuConst.calDistance;
                    updateTextbox(this.distTextBox, string.Format("{0:0.000}", (this.tempDist / 1000)));
                }
                catch (Exception)
                {
                    updateTextbox(this.distTextBox, "0");
                }

                int hr = (this.vmu.countsData.totalTime) / 3600;
                int min = ((this.vmu.countsData.totalTime) / 60) % 60;
                int sec = (this.vmu.countsData.totalTime) % 60;
                updateTextbox(this.tripTimeTextBox, string.Format("{0:00} Hrs   {1:00} Mins   {2:00} Sec"
                    , hr, min, sec));
                updateTextbox(this.rpmTextBox, this.vmu.countsData.RPMCount.ToString());
                updateTextbox(this.speedTextBox, this.vmu.countsData.speedCount.ToString());
                txtCurrentTimeCounts.Text = DateTime.Now.ToString("yyyy/MM/dd     HH:mm:ss");
                //MessageBox.Show("Update Counts Data!");
                /*if(!backgroundWorkerCOMMS.IsBusy)
                {
                    backgroundWorkerCOMMS.RunWorkerAsync();
                }*/
            }
        }
        private void updateCheckbox(CheckBox checkbox, bool checkedState)
        {
            if (checkbox.InvokeRequired)
            {
                updateCheckboxCallback d = new updateCheckboxCallback(updateCheckbox);
                this.Invoke(d, new object[] { checkbox, checkedState });
            }
            else
            {
                checkbox.Checked = checkedState;
            }
        }
        private void updateRadiobutton(RadioButton radiobutton, bool checkedState)
        {
            if (radiobutton.InvokeRequired)
            {
                updateRadiobuttonCallback d = new updateRadiobuttonCallback(updateRadiobutton);
                this.Invoke(d, new object[] { radiobutton, checkedState });
            }
            else
            {
                radiobutton.Checked = checkedState;
            }
        }
        private void updateGroupbox(GroupBox gbox, string text)
        {
            if (gbox.InvokeRequired)
            {
                updateGroupboxCallback d = new updateGroupboxCallback(updateGroupbox);
                this.Invoke(d, new object[] { gbox, text });
            }
            else
            {
                gbox.Text = text;
            }
        }
        private void validateNumericText(TextBox textBox)
        {
            foreach (char c in textBox.Text)
            {
                if (!char.IsDigit(c))
                {
                    MessageBox.Show("Incorrect Value Format", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox.Text = "";
                    return;
                }
            }
        }
        private void makeFilters()
        {
            string trip = "Trip Files(.txx)|*.dat;";
            string log = "Log Files(.sxx)|";
            string gps = "GPS Files(.gxx)|";
            string leach = "Leach Files(.lxx)|";

            for (int i = 0; i < 32; i++)
            {
                if (i == 31)
                {
                    trip = trip + "*.t" + (i.ToString()).PadLeft(2, '0');
                    log = log + "*.s" + (i.ToString()).PadLeft(2, '0');
                    gps = gps + "*.g" + (i.ToString()).PadLeft(2, '0');
                    leach = leach + "*.l" + (i.ToString()).PadLeft(2, '0');
                }
                else
                {
                    trip = trip + "*.t" + (i.ToString()).PadLeft(2, '0') + ";";
                    log = log + "*.s" + (i.ToString()).PadLeft(2, '0') + ";";
                    gps = gps + "*.g" + (i.ToString()).PadLeft(2, '0') + ";";
                    leach = leach + "*.l" + (i.ToString()).PadLeft(2, '0') + ";";
                }
            }

            this.TripFilesFilter = trip;
            this.LogFilesFilter = log;
            this.GPSFilsFilter = gps;
            this.LeachFilter = leach;
        }
        private void showFileOpenDialog()
        {
            makeFilters();
            this.filter = this.TripFilesFilter + "|" + this.LogFilesFilter + "|" + this.GPSFilsFilter + "|" + this.LeachFilter;

            this.openFileDialog1.AutoUpgradeEnabled = true;
            this.openFileDialog1.Title = "Open VMU File";
            this.openFileDialog1.Filter = this.filter;

            DialogResult result = this.openFileDialog1.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                this.openFileDialog1.Dispose();
            }

        }
        private void loadProfileData(string profString, ref profileStruct profile)
        {
            string[] s = profString.Split(new char[] { ',' });
            if (s.Length < 22)
            {
                MessageBox.Show("Incorrect Profile File Format", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            UInt16.TryParse(s[0], out profile.calRPM);
            UInt16.TryParse(s[1], out profile.calDist);
            UInt16.TryParse(s[2], out profile.maxRPM);
            UInt16.TryParse(s[3], out profile.maxSpeed);
            UInt16.TryParse(s[4], out profile.maxBrake);
            UInt16.TryParse(s[5], out profile.maxAccel);
            byte.TryParse(s[6], out profile.IdleLimit);
            profile.serialNum = s[7].Replace("\"", "");
            profile.versionNum = s[8].Replace("\"", "");
            profile.vehicleID = s[9].Replace("\"", "");
            double.TryParse(s[10], out profile.odometer);
            UInt16.TryParse(s[11], out profile.tripNumber);
            Int16.TryParse(s[12], out profile.greenBandUpper);
            Int16.TryParse(s[13], out profile.greenBandLower);
            byte.TryParse(s[14], out profile.tripStartDist);
            byte.TryParse(s[15], out profile.bell);
            UInt16.TryParse(s[16], out profile.type);
            UInt16.TryParse(s[17], out profile.type2);
            UInt16.TryParse(s[18], out profile.type3);
            byte.TryParse(s[19], out profile.options);
            byte.TryParse(s[20], out profile.inputOptions);
            byte.TryParse(s[21], out profile.promptOptions);
        }
        private void setEnable(bool enable)
        {
            this.btnVmuRefresh.Enabled = enable;
            this.btnLedON.Enabled = enable;
            this.btnLedOFF.Enabled = enable;
            this.btnGPS.Enabled = enable;

            //this.btnWriteVMU.Enabled = enable;
            //this.btnReadVMU.Enabled = enable;

            this.btnSetTime.Enabled = enable;
            this.btnReadTime.Enabled = enable;

            this.btnMemoryFlash.Enabled = enable;
        }
        private void clearText(bool clear)
        {
            rpmTextBox.Text = "";
            speedTextBox.Text = "";
            distTextBox.Text = "";
            //rpmCalTextBox.Text = "";
            latitudeTextBox.Text = "";
            longitudeTextBox.Text = "";
            odometerTextBox.Text = "";
            distCountTextBox.Text = "";
            tripTimeTextBox.Text = "";
            //speedCalTextBox.Text = "";
            versionNumberLabel.Text = "";
            connectionStatusLabel.Text = "";

            pcTimeTextBox.Text = "";
            vmuTimeTextBox.Text = "";

            limRPMTextBox.Text = "";
            limSpeedTextBox.Text = "";
            limBrakeTextBox.Text = "";
            limAccelTextBox.Text = "";
        }
        private bool readFile(ref int byteCount)
        {
            if (this.filename != null)
            {
                FileInfo fi = new FileInfo(this.filename);
                if (fi.Exists)
                {
                    try
                    {
                        FileStream fs = fi.OpenRead();
                        byteCount = fs.Read(this.readBuf, 0, (int)fs.Length);
                        fs.Close();
                        return true;
                    }
                    catch (Exception fe)
                    {
                        MessageBox.Show(fe.Message, fe.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                else { return false; }
            }
            else
            {
                return false;
            }

        }
        private byte getBeepOnValue()
        {
            byte val = 0;

            if (this.rpmCheckBox.Checked) { val = (byte)(val | 0x01); }
            else { val = (byte)(val & ~(0x01)); }
            if (this.speedingCheckBox.Checked) { val = (byte)(val | 0x02); }
            else { val = (byte)(val & ~(0x02)); }
            if (this.brakeCheckBox.Checked) { val = (byte)(val | 0x04); }
            else { val = (byte)(val & ~(0x04)); }
            if (this.accelCheckBox.Checked) { val = (byte)(val | 0x08); }
            else { val = (byte)(val & ~(0x08)); }

            return val;
        }
        private string getProfileData()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.calRPMTextBox.Text + ",");
            sb.Append(this.calDistTextBox.Text + ",");
            sb.Append(this.limRPMTextBox.Text + ",");
            sb.Append(this.limSpeedTextBox.Text + ",");
            sb.Append(this.limBrakeTextBox.Text + ",");
            sb.Append(this.limAccelTextBox.Text + ",");
            sb.Append(this.odometerTextBox.Text + ",");
            sb.Append(getBeepOnValue().ToString() + ",");
            sb.Append("7,");
            return sb.ToString();
        }
        #endregion

        #region STRUCTS
        public struct VMUConstStruct
        {
            public Int16 calDistance;
            public Int16 calRPM;
            public Int16 maxSpeed;
            public byte idleTimeLimit;
            public Int16 maxRPM;
            public Int16 maxBrake;
            public Int16 maxAccel;
            public UInt32 oddo;
            public string ID;
            public string version;
            public byte bell;
            public UInt16 type;
            public string serialNumber;
            public UInt16 tripNumber;
            public Int16 greenBandUpper;
            public Int16 greenBandLower;
            public byte promptOptions;
            public byte inputOptions;
            public byte tripStartDistance;
            public byte options;
            public byte gpsTimeDiff;
            public byte gpsOptions;
            public UInt16 gpsLogInterval;
            public UInt16 type2;
            public UInt16 type3;
            public byte altMaxSpeed;
            public byte bypass;
            public UInt16 crc;

            public VMUConstStruct(int i)
            {
                this.calDistance = 0;
                this.calRPM = 0;
                this.maxSpeed = 0;
                this.idleTimeLimit = 0;
                this.maxRPM = 0;
                this.maxBrake = 0;
                this.maxAccel = 0;
                this.oddo = 0;
                this.ID = "";
                this.bell = 0;
                this.type = 0;
                this.serialNumber = "";
                this.tripNumber = 0;
                this.version = "";
                this.greenBandUpper = 0;
                this.greenBandLower = 0;
                this.promptOptions = 0;
                this.inputOptions = 0;
                this.tripStartDistance = 0;
                this.options = 0;
                this.gpsTimeDiff = 0;
                this.gpsOptions = 0;
                this.gpsLogInterval = 0;
                this.type2 = 0;
                this.type3 = 0;
                this.altMaxSpeed = 0;
                this.bypass = 0;
                this.crc = 0;
            }

        }
        public struct TimeStruct
        {
            public int hour;
            public int minute;
            public int second;
            public TimeStruct(int i)
            {
                this.hour = 0;
                this.minute = 0;
                this.second = 0;
            }
        }
        public struct DateStruct
        {
            public int year;
            public int month;
            public int day;
            public int dow;

            public DateStruct(int i)
            {
                this.year = 0;
                this.month = 0;
                this.day = 0;
                this.dow = 0;
            }
        }
        public struct TripDataStruct
        {
            public byte header;
            public byte maxSpeed;
            public UInt16 serialNumber;
            public string driverID;
            public string vehicleID;
            public string jobNumber;
            public DateStruct startDate;
            public DateStruct endDate;
            public TimeStruct startTime;
            public TimeStruct endTime;
            public UInt32 startOdo;
            public UInt32 endOdo;
            public UInt16 maxRPM;
            public TimeStruct idleTime;
            public TimeStruct greenTime;
            public TimeStruct speedingTime;
            public TimeStruct overRPMTime;
            public UInt16 fuel;
            public UInt32 fuelPrice;
            public UInt16 timerCnt;
            public UInt16 eventCnt;
            public byte harshAccel;
            public byte harshBrake;
            public double gpsLat;
            public double gpsLong;
            public byte mode;
            public byte[] spare;
            public byte status;
            public UInt16 crc;
            public byte endOfRec;

            public TripDataStruct(int i)
            {
                this.header = 0;
                this.maxSpeed = 0;
                this.serialNumber = 0;
                this.driverID = "";
                this.vehicleID = "";
                this.jobNumber = "";
                this.startDate = new DateStruct(0);
                this.endDate = new DateStruct(0);
                this.startTime = new TimeStruct(0);
                this.endTime = new TimeStruct(0);
                this.startOdo = 0;
                this.endOdo = 0;
                this.maxRPM = 0;
                this.idleTime = new TimeStruct(0);
                this.greenTime = new TimeStruct(0);
                this.speedingTime = new TimeStruct(0);
                this.overRPMTime = new TimeStruct(0);
                this.fuel = 0;
                this.fuelPrice = 0;
                this.timerCnt = 0;
                this.eventCnt = 0;
                this.harshAccel = 0;
                this.harshBrake = 0;
                this.gpsLat = 0;
                this.gpsLong = 0;
                this.mode = 0;
                this.spare = new byte[3];
                this.status = 0;
                this.crc = 0;
                this.endOfRec = 0;
            }
        }
        public struct CountsStruct
        {
            public UInt16 currentRPM;
            public UInt16 currentSpeed;
            public UInt16 currentDistance;
            public UInt16 RPMCount;
            public UInt16 speedCount;
            public UInt16 DistCount;
            public UInt16 totalTime;

            public CountsStruct(int i)
            {
                this.currentRPM = 0;
                this.currentSpeed = 0;
                this.speedCount = 0;
                this.currentDistance = 0;
                this.RPMCount = 0;
                this.DistCount = 0;
                this.totalTime = 0;
            }
        }
        public struct RSTDO_Struct
        {
            public UInt16 currentRPM;
            public UInt16 currentSpeed;
            public UInt16 currentDistance;
            public UInt32 oddoPlusCurDist;
            public UInt16 totalTime;
            public DateStruct currentDate;
            public TimeStruct currentTime;

            public RSTDO_Struct(int i)
            {
                this.currentRPM = 0;
                this.currentSpeed = 0;
                this.currentDistance = 0;
                this.oddoPlusCurDist = 0;
                this.totalTime = 0;
                this.currentDate = new DateStruct(0);
                this.currentTime = new TimeStruct(0);
            }
        }
        public struct flashHeaderStruct
        {
            public int blockNum;
            public byte blockType;
            public byte headSize;
            public UInt32 eraseCnt;
            public Int16 recordSize;
            public UInt32 seqNo;
            public DateStruct lastEraseDate;
            public UInt16 used;
            public UInt16 saved;
            public TimeStruct lastEraseTime;
            public byte[] spare;

            public flashHeaderStruct(int i)
            {
                this.blockNum = 0;
                this.blockType = 0;
                this.headSize = 0;
                this.eraseCnt = 0;
                this.recordSize = 0;
                this.seqNo = 0;
                this.lastEraseDate = new DateStruct(0);
                this.used = 0;
                this.saved = 0;
                this.lastEraseTime = new TimeStruct(0);
                this.spare = new byte[9];
            }
        }
        public struct blockConfigStruct
        {
            public int blockNum;
            public int type;
            public int recordSize;

            public blockConfigStruct(int i)
            {
                this.blockNum = 0;
                this.type = 0;
                this.recordSize = 0;
            }
        }
        public struct profileStruct
        {
            public UInt16 calRPM;
            public UInt16 calDist;
            public UInt16 maxRPM;
            public UInt16 maxSpeed;
            public UInt16 maxBrake;
            public UInt16 maxAccel;
            public byte IdleLimit;
            public string serialNum;
            public string versionNum;
            public string vehicleID;
            public double odometer;
            public UInt16 tripNumber;
            public Int16 greenBandUpper;
            public Int16 greenBandLower;
            public byte tripStartDist;
            public byte bell;
            public UInt16 type;
            public UInt16 type2;
            public UInt16 type3;
            public byte options;
            public byte inputOptions;
            public byte promptOptions;

            public profileStruct(int i)
            {
                this.calRPM = 0;
                this.calDist = 0;
                this.maxRPM = 0;
                this.maxSpeed = 0;
                this.maxBrake = 0;
                this.maxAccel = 0;
                this.IdleLimit = 0;
                this.serialNum = "";
                this.versionNum = "";
                this.vehicleID = "";
                this.odometer = 0.0;
                this.tripNumber = 0;
                this.greenBandUpper = 0;
                this.greenBandLower = 0;
                this.tripStartDist = 0;
                this.bell = 0;
                this.type = 0;
                this.type2 = 0;
                this.type3 = 0;
                this.options = 0;
                this.inputOptions = 0;
                this.promptOptions = 0;
            }
        }

        public struct brakeEventStruct
        {
            public byte header;
            public TimeStruct time;
            public UInt16 serialNum;
            public byte[] speed;
            public UInt32 odo;
            public DateStruct date;
            public byte[] spare;
            public UInt16 crc;
            public byte status;
            public byte endOfRec;

            public brakeEventStruct(int init)
            {
                header = 0;
                time = new TimeStruct(0);
                serialNum = 0;
                speed = new byte[frmMain.MAX_POINTS];
                odo = 0;
                date = new DateStruct(0);
                spare = new byte[2];
                crc = 0;
                status = 0;
                endOfRec = 0;
            }
        }
        public struct accelEventStruct
        {
            public byte header;
            public TimeStruct time;
            public UInt16 serialNum;
            public byte[] speed;
            public UInt32 odo;
            public DateStruct date;
            public byte[] spare;
            public UInt16 crc;
            public byte status;
            public byte endOfRec;

            public accelEventStruct(int init)
            {
                header = 0;
                time = new TimeStruct(0);
                serialNum = 0;
                speed = new byte[frmMain.MAX_POINTS];
                odo = 0;
                date = new DateStruct(0);
                spare = new byte[2];
                crc = 0;
                status = 0;
                endOfRec = 0;
            }
        }
        public struct fuelEventStruct
        {
            public byte header;
            public TimeStruct time;
            public string vehicleID;
            public string driverID;
            public UInt16 pumpID;
            public DateStruct date;
            public UInt16 fuel;
            public UInt32 fuelPrice;
            public UInt16 index;
            public byte[] spare;
            public UInt16 crc;
            public byte status;
            public byte endOfRec;

            public fuelEventStruct(int init)
            {
                header = 0;
                time = new TimeStruct(0);
                vehicleID = "";
                driverID = "";
                pumpID = 0;
                date = new DateStruct(0);
                fuel = 0;
                fuelPrice = 0;
                index = 0;
                spare = new byte[2];
                crc = 0;
                status = 0;
                endOfRec = 0;
            }
        }
        public struct inputEventStruct
        {
            public byte header;
            public byte type;
            public DateStruct onDate;
            public TimeStruct onTime;
            public TimeStruct offTime;
            public string id;
            public string driver;
            public UInt16 distance;
            public UInt32 odo;
            public byte[] spare;
            public UInt16 crc;
            public byte status;
            public byte endOfRec;

            public inputEventStruct(int init)
            {
                header = 0;
                type = 0;
                onDate = new DateStruct(0);
                onTime = new TimeStruct(0);
                offTime = new TimeStruct(0);
                id = "";
                driver = "";
                distance = 0;
                odo = 0;
                spare = new byte[2];
                crc = 0;
                status = 0;
                endOfRec = 0;
            }
        }
        public struct userEventStruct
        {
            public byte header;
            public TimeStruct time;
            public DateStruct date;
            public UInt16 tripNo;
            public UInt32 odometer;
            public string id;
            public string driver;
            public string input;
            public byte type;
            public byte[] spare;
            public UInt16 crc;
            public byte status;
            public byte endOfRec;

            public userEventStruct(int init)
            {
                header = 0;
                time = new TimeStruct(0);
                date = new DateStruct(0);
                tripNo = 0;
                odometer = 0;
                id = "";
                driver = "";
                input = "";
                type = 0;
                spare = new byte[1];
                crc = 0;
                status = 0;
                endOfRec = 0;
            }
        }
        public struct idleEventStruct
        {
            public byte header;
            public TimeStruct time;
            public DateStruct date;
            public UInt16 tripNum;
            public UInt32 odoMeter;
            public UInt16 duration;
            public string id;
            public string driver;
            public UInt16 crc;
            public byte status;
            public byte endOfRec;

            public idleEventStruct(int init)
            {
                header = 0;
                time = new TimeStruct(0);
                date = new DateStruct(0);
                tripNum = 0;
                odoMeter = 0;
                duration = 0;
                id = "";
                driver = "";
                crc = 0;
                status = 0;
                endOfRec = 0;
            }
        }
        public struct powerEventStruct
        {
            public byte header;
            public TimeStruct time;
            public DateStruct date;
            public UInt16 tripNo;
            public UInt32 odoMeter;
            public UInt16 spare;
            public string id;
            public string driver;
            public UInt16 crc;
            public byte status;
            public byte endOfRec;

            public powerEventStruct(int init)
            {
                header = 0;
                time = new TimeStruct(0);
                date = new DateStruct(0);
                tripNo = 0;
                odoMeter = 0;
                spare = 0;
                id = "";
                driver = "";
                crc = 0;
                status = 0;
                endOfRec = 0;
            }
        }
        public struct speedEventStruct
        {
            public byte header;
            public TimeStruct time;
            public DateStruct date;
            public UInt16 tripNo;
            public UInt32 odometer;
            public string id;
            public string driver;
            public byte limit;
            public byte spdAvg;
            public UInt16 tolTotal;
            public UInt16 tol10;
            public UInt16 tol20;
            public UInt16 tol30;
            public UInt16 crc;
            public byte status;
            public byte endOfRec;

            public speedEventStruct(int init)
            {
                header = 0;
                time = new TimeStruct(0);
                date = new DateStruct(0);
                tripNo = 0;
                odometer = 0;
                id = "";
                driver = "";
                limit = 0;
                spdAvg = 0;
                tolTotal = 0;
                tol10 = 0;
                tol20 = 0;
                tol30 = 0;
                crc = 0;
                status = 0;
                endOfRec = 0;
            }
        }

        public struct logStruct
        {
            public TimeStruct time;
            public DateStruct date;
            public int speed;
            public int RPM;
            public byte status;

            public logStruct(int init)
            {
                this.time = new TimeStruct(0);
                this.date = new DateStruct(0);
                this.speed = 0;
                this.RPM = 0;
                this.status = 0;
            }
        }
        public struct wgs84Struct
        {
            public double latitude;             // in desimal degrees
            public double longitude;
            public double altitude;            // altitude in feet
            public wgs84Struct(double lat, double longi, double alt)
            {
                this.latitude = lat;
                this.longitude = longi;
                this.altitude = alt;
            }

        }
        public struct gpsDataStruct : IComparable<gpsDataStruct>
        {
            public wgs84Struct gps;
            public int RPM;
            public int speed;              // speed in knots
            public int year;
            public byte month;
            public byte day;
            public byte hour;
            public byte minute;
            public byte second;
            public byte eInputs;            // Bit3=A8 Bit2=A6 Bit1=A5 Bit0=A5
            public byte dop;                // 0 no fix, >0 OK
            public byte status;

            public gpsDataStruct(int init)
            {
                gps.latitude = 0.0;
                gps.longitude = 0.0;
                gps.altitude = 0.0;
                RPM = 0;
                speed = 0;
                year = 0;
                month = 0;
                day = 0;
                hour = 0;
                minute = 0;
                second = 0;
                eInputs = 0;
                dop = 0;
                status = 0;
            }
            public int CompareTo(gpsDataStruct other)
            {
                DateTime time = new DateTime(year, month, day, hour, minute, second);
                DateTime otherTime = new DateTime(other.year, other.month, other.day, other.hour, other.minute, other.second);

                return time.CompareTo(otherTime);
            }

        }
        public struct gpsConfig
        {
            public int logInterval;
            public byte timeDiff;
            public byte gpsOptions;

            public gpsConfig(int init)
            {
                this.logInterval = 0;
                this.timeDiff = 0;
                this.gpsOptions = 0;
            }
        }








        #endregion

        private void backgroundWorkerCOMMS_DoWork(object sender, DoWorkEventArgs e)
        {
            while(currentPort.IsOpen)
            {
                Thread.Sleep(100);
            }
            //this.Invoke((MethodInvoker)delegate
            //{
            MessageBox.Show("Background Worker!");
            btnDisconnect.PerformClick();
            //});
        }
    }
}