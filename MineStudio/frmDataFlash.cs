using MineStudio.Properties;
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
using static MineStudio.frmMain;

namespace MineStudio
{
    public partial class frmDataFlash : Form
    {
        #region Properties
        private List<int> selectedBlocks = new List<int>();
        private RS232_Comms rs232comms = new RS232_Comms();
        private VMU vmu = new VMU();
        private VMUCFG_Events vmucfg_events = new VMUCFG_Events();
        private SerialPort currentPort = new SerialPort();
        private COMMS_STATUS status;
        private bool highDownloadSpeed = false;
        private int tripDefaultVal = 6;
        private int sbsDefaultVal = 13;
        private int gpsDefaultVal = 13;
        private int currentTripVal;
        private int currentSbsVal;
        private int currentGpsVal;

        private int maxBlocks = 32;
        private int frameSize = 128;
        private int framesPerBlock = 512;
        private byte[] blockData = new byte[65536];
        #endregion

        #region Constructors
        public frmDataFlash()
        {
            InitializeComponent();
        }
        public frmDataFlash(RS232_Comms comms, VMU vmu, VMUCFG_Events e)
        {
            InitializeComponent();
            this.rs232comms = comms;
            this.vmu = vmu;
            this.vmucfg_events = e;
            this.currentPort = this.rs232comms.getConnectedVMUPort(0);
        }
        private void frmDataFlash_Load(object sender, EventArgs e)
        {
            //groupBoxMessage.Visible = false;
            //toolStripLabelRestart.Visible = true;
            this.listView1.Items.Clear();
            this.listView1.CheckBoxes = true;
            setProgressBarParms(ProgressBarStyle.Blocks, true);
            setEnable(true);

            this.tripsTextBox.Text = this.tripDefaultVal.ToString();
            this.sbsTextBox.Text = this.sbsDefaultVal.ToString();
            this.gpsTextBox.Text = this.gpsDefaultVal.ToString();
        }
        private void frmDataFlash_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.flashConfigBackgroundWorker.IsBusy)
            {
                this.flashConfigBackgroundWorker.CancelAsync();
                MessageBox.Show("Cancelling...", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (this.downloadBackgroundWorker.IsBusy)
            {
                MessageBox.Show("Cancelling...", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.downloadBackgroundWorker.CancelAsync();
            }
            
            if (this.highDownloadSpeed)
            {
                this.rs232comms.setVMUBaudrate(this.currentPort, 9600);
                this.highDownloadSpeed = false;
            }
        }
        private void frmDataFlash_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.vmucfg_events.fireFormClosedEvent(this.Name);
        }
        private void frmDataFlash_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.W)
            {
                e.SuppressKeyPress = true;
                btnWriteData.PerformClick();
            }
            if (e.Control && e.KeyCode == Keys.R)
            {
                e.SuppressKeyPress = true;
                btnReadDataFlash.PerformClick();
            }
            if (e.Control && e.KeyCode == Keys.D)
            {
                e.SuppressKeyPress = true;
                btnDownload.PerformClick();
            }
            if (e.Control && e.KeyCode == Keys.C)
            {
                e.SuppressKeyPress = true;
                btnClearData.PerformClick();
            }
            if (e.Control && e.KeyCode == Keys.A)
            {
                e.SuppressKeyPress = true;
                btnCancel.PerformClick();
            }
            if (e.Control && e.KeyCode == Keys.X)
            {
                e.SuppressKeyPress = true;
                btnExit.PerformClick();
            }
        }
        #endregion

        #region Background Worker
        private void flashConfigBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if (worker.CancellationPending) { e.Cancel = true; return; }

            this.vmu.flashConfig = new flashHeaderStruct[VMU.FLASH_BLOCKS];
            int maxBlocks = VMU.FLASH_BLOCKS;
            double p;
            int percentage;

            for (int i = 0; i < maxBlocks; i++)
            {
                if (worker.CancellationPending) { e.Cancel = true; return; }

                worker.ReportProgress(this.toolStripProgressBar1.Value,
                    string.Format("Reading Block {0:G} Configuration", i));
                this.status = this.rs232comms.getVMUFlashConfig(this.currentPort, i, ref this.vmu.flashConfig[i]);

                p = ((double)i) / maxBlocks;
                percentage = (int)(p * 100);
                worker.ReportProgress(percentage,
                string.Format("Reading Block {0:G} Configuration", i));

                if (this.status == COMMS_STATUS.OKAY)
                {
                    updateListViewItems(this.listView1, this.vmu.flashConfig[i]);
                }
            }
        }
        private void flashConfigBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.toolStripProgressBar1.Value = e.ProgressPercentage;
            this.toolStripStatusLabel1.Text = (string)e.UserState;
        }
        private void flashConfigBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.toolStripStatusLabel1.Text = "Done";
            setProgressBarParms(ProgressBarStyle.Blocks, false);
            if (e.Cancelled) { this.listView1.Items.Clear(); }
            this.listView1.Enabled = true;
            setEnable(true);
        }

        private void downloadBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            string path = (string)e.Argument;
            string fname = "";

            if (worker.CancellationPending) { e.Cancel = true; return; }
            byte[] temp = new byte[this.frameSize];

            int totalBlocks = this.selectedBlocks.Count;
            int totalFramesToRead = this.framesPerBlock * totalBlocks;
            double p = 0.0;

            for (int i = 0; i < totalBlocks; i++)
            {
                fname = this.vmu.vmuConst.ID.TrimEnd(new char[] { '\0' }) + getFileType(this.vmu.flashConfig[this.selectedBlocks[i]]);

                if (worker.CancellationPending) { e.Cancel = true; return; }

                for (int j = 0; j < this.framesPerBlock; j++)
                {
                    if (worker.CancellationPending) { e.Cancel = true; return; }
                    p = (((double)((i * this.framesPerBlock) + (j + 1))) / totalFramesToRead) * 100;
                    worker.ReportProgress((int)p, string.Format("Downloading Block{0:G} to {1:G}    {2:G}% Complete",
                       this.selectedBlocks[i], fname, (int)p));
                    this.status = this.rs232comms.getVMUFlashData(this.currentPort, this.selectedBlocks[i], j * this.frameSize, frameSize, temp);
                    if (this.status == COMMS_STATUS.OKAY)
                    {
                        Array.Copy(temp, 0, this.blockData, j * this.frameSize, this.frameSize);

                    }
                }

                FileInfo fi = new FileInfo(path + "\\" + fname);
                FileStream fs = fi.OpenWrite();
                fs.Write(this.blockData, 0, this.blockData.Length);
                fs.Close();
            }
        }
        private void downloadBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.toolStripProgressBar1.Value = e.ProgressPercentage;
            this.toolStripStatusLabel1.Text = (string)e.UserState;
        }
        private void downloadBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.highDownloadSpeed)
            {
                this.status = this.rs232comms.setVMUBaudrate(this.currentPort, 9600);
                if (this.status == COMMS_STATUS.OKAY) { this.highDownloadSpeed = false; }
            }
            this.toolStripProgressBar1.Value = 0;
            setProgressBarParms(ProgressBarStyle.Blocks, false);
            this.toolStripStatusLabel1.Text = "Done";
            this.listView1.Enabled = true;
            setEnable(true);
            foreach (ListViewItem item in this.listView1.Items)
            {
                if (item.Checked) { item.Checked = false; }
            }
            if (e.Cancelled)
            {
                MessageBox.Show("Download Cancelled!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show("Download Complete", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region Button Click Event
        private void btnWriteData_Click(object sender, EventArgs e)
        {
            if (!readFlashBackgroundWorker.IsBusy)
            {
                readFlashBackgroundWorker.RunWorkerAsync();
            }

            readFlashBackgroundWorker.ReportProgress(0);
        }
        private void btnReadDataFlash_Click(object sender, EventArgs e)
        {
            this.listView1.Items.Clear();
            setProgressBarParms(ProgressBarStyle.Blocks, true);
            this.listView1.Enabled = false;
            setEnable(false);
            if (!this.flashConfigBackgroundWorker.IsBusy)
            {
                this.flashConfigBackgroundWorker.RunWorkerAsync();
            }
        }
        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (this.selectedBlocks.Count > 0)
            {
                DialogResult res = MessageBox.Show("Switch to Higher Download Speed", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (res == DialogResult.Yes)
                {
                    this.status = this.rs232comms.setVMUBaudrate(this.currentPort, 56000);
                    if (this.status == COMMS_STATUS.OKAY) { this.highDownloadSpeed = true; }
                }


                this.folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Desktop;

                res = this.folderBrowserDialog1.ShowDialog(this);

                if (res == DialogResult.OK)
                {
                    this.status = this.rs232comms.getVMUConstants(this.currentPort, ref this.vmu.vmuConst);
                    if (this.status == COMMS_STATUS.OKAY)
                    {
                        this.listView1.Enabled = false;
                        setEnable(false);
                        this.selectedBlocks.Sort();
                        setProgressBarParms(ProgressBarStyle.Blocks, true);
                        Settings.Default.downloadFolder = this.folderBrowserDialog1.SelectedPath;
                        Settings.Default.Save();
                        string path = Settings.Default.downloadFolder;
                        if (!this.downloadBackgroundWorker.IsBusy) this.downloadBackgroundWorker.RunWorkerAsync(path);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please Tick the Blocks to download from!", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnClearData_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("All the Data will be erased!\nDo you want to proceed?", "",
               MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res == DialogResult.No) return;
            if (res == DialogResult.Yes)
            {
                this.status = this.rs232comms.eraseFlash(this.currentPort);
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (this.flashConfigBackgroundWorker.IsBusy) { this.flashConfigBackgroundWorker.CancelAsync(); }
            if (this.downloadBackgroundWorker.IsBusy) { this.downloadBackgroundWorker.CancelAsync(); }
            //this.status = this.rs232comms.setVMUBaudrate(this.currentPort, 56000);
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            if (this.flashConfigBackgroundWorker.IsBusy) { this.flashConfigBackgroundWorker.CancelAsync(); }
            if (this.downloadBackgroundWorker.IsBusy) { this.downloadBackgroundWorker.CancelAsync(); }
            this.Close();
        }
        #endregion

        private void updateListViewItems(ListView listView, flashHeaderStruct block)
        {
            if (listView.InvokeRequired)
            {
                updateListViewCallback d = new updateListViewCallback(updateListViewItems);
                this.Invoke(d, new object[] { listView, block });
            }
            else
            {
                ListViewItem item = new ListViewItem(block.blockNum.ToString());
                ListViewItem.ListViewSubItem type = new ListViewItem.ListViewSubItem(item, getDataFormatName(block.blockType));
                ListViewItem.ListViewSubItem seq = new ListViewItem.ListViewSubItem(item, string.Format("{0:G}", block.seqNo));
                ListViewItem.ListViewSubItem eraseCnt = new ListViewItem.ListViewSubItem(item, string.Format("{0:G}", block.eraseCnt));
                ListViewItem.ListViewSubItem date = new ListViewItem.ListViewSubItem(item, string.Format("{0:0000}/{1:00}/{2:00}",
                    block.lastEraseDate.year, block.lastEraseDate.month, block.lastEraseDate.day));
                ListViewItem.ListViewSubItem time = new ListViewItem.ListViewSubItem(item, string.Format("{0:00}:{1:00}:{2:00}",
                    block.lastEraseTime.hour, block.lastEraseTime.minute, block.lastEraseTime.second));
                ListViewItem.ListViewSubItem size = new ListViewItem.ListViewSubItem(item, string.Format("{0:G}", block.recordSize));
                ListViewItem.ListViewSubItem used = new ListViewItem.ListViewSubItem(item, (string.Format("{0:X}", block.used)).PadLeft(4, '0'));
                ListViewItem.ListViewSubItem saved = new ListViewItem.ListViewSubItem(item, (string.Format("{0:X}", block.saved)).PadLeft(4, '0'));
                item.SubItems.Insert(1, type);
                item.SubItems.Insert(2, seq);
                item.SubItems.Insert(3, eraseCnt);
                item.SubItems.Insert(4, date);
                item.SubItems.Insert(5, time);
                item.SubItems.Insert(6, size);
                item.SubItems.Insert(7, used);
                item.SubItems.Insert(8, saved);
                listView.Items.Add(item);
            }
        }
        private void setProgressBarParms(ProgressBarStyle style, bool visible)
        {
            this.toolStripProgressBar1.Style = style;
            this.toolStripProgressBar1.Visible = visible;
            this.toolStripProgressBar1.Value = 0;
        }
        private void validateNumericText(TextBox textBox)
        {
            foreach (char c in textBox.Text)
            {
                if (!char.IsDigit(c))
                {
                    MessageBox.Show("Incorrect Value Format", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox.Text = "1";
                    return;
                }
            }
        }
        private void setEnable(bool enable)
        {
            this.btnReadDataFlash.Enabled = enable;
            this.btnDownload.Enabled = enable;
            this.btnClearData.Enabled = enable;
            this.btnCancel.Enabled = !enable;
        }
        private string getDataFormatName(byte type)
        {
            if ((type & 0x80) == 0x80) return "TRIP";
            else if ((type & 0x20) == 0x20) return "SBS";
            else if ((type & 0x10) == 0x10) return "GPS";
            else return "NONE";
        }
        private string getFileType(flashHeaderStruct block)
        {
            if (getDataFormatName(block.blockType) == "TRIP")
            {
                return string.Format(".t{0:00}", block.blockNum);
            }
            else if (getDataFormatName(block.blockType) == "SBS")
            {
                return string.Format(".s{0:00}", block.blockNum);
            }
            else if (getDataFormatName(block.blockType) == "GPS")
            {
                return string.Format(".g{0:00}", block.blockNum);
            }
            else
            {
                return "";
            }
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item.Checked)
            {
                if (!this.selectedBlocks.Contains(int.Parse(e.Item.Text))) this.selectedBlocks.Add(int.Parse(e.Item.Text));
            }
            else
            {
                this.selectedBlocks.Remove(int.Parse(e.Item.Text));
            }
        }
        private void tripsTextBox_TextChanged(object sender, EventArgs e)
        {
            int t, temp;
            validateNumericText(this.tripsTextBox);
            temp = this.currentTripVal;

            if (this.tripsTextBox.Text != "")
            {
                this.currentTripVal = int.Parse(this.tripsTextBox.Text);

                if (this.currentTripVal <= 0)
                {
                    MessageBox.Show("Error Number of Trip Blocks Cannot be less than 1", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.tripsTextBox.Text = "1";
                    return;
                }

                if (this.currentTripVal > 30) { this.currentTripVal = 30; this.currentGpsVal = 0; this.currentSbsVal = 2; }
                else
                {
                    t = this.maxBlocks - this.currentGpsVal;
                    this.currentSbsVal = t - this.currentTripVal;
                    if (currentSbsVal <= 0)
                    {
                        MessageBox.Show("Error Reduce the Number of Trip Blocks", "",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.tripsTextBox.Text = temp.ToString();
                        return;
                    }
                }
                this.tripsTextBox.Text = this.currentTripVal.ToString();
                this.sbsTextBox.Text = this.currentSbsVal.ToString();
                this.gpsTextBox.Text = this.currentGpsVal.ToString();
            }
        }
        private void gpsTextBox_TextChanged(object sender, EventArgs e)
        {
            validateNumericText(this.gpsTextBox);
            this.currentGpsVal = int.Parse(this.gpsTextBox.Text);
        }
        private void sbsTextBox_TextChanged(object sender, EventArgs e)
        {
            int t, t2;
            validateNumericText(this.sbsTextBox);
            t = this.currentSbsVal + this.currentGpsVal;

            if (this.sbsTextBox.Text != "")
            {
                this.currentSbsVal = int.Parse(this.sbsTextBox.Text);
                if (this.currentSbsVal <= 0)
                {
                    MessageBox.Show("Number of Sbs Blocks Cannot be less than 1", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.currentSbsVal = 1;
                }
                if (this.currentSbsVal > t) this.currentSbsVal = t;
                this.sbsTextBox.Text = this.currentSbsVal.ToString();
                t2 = t - this.currentSbsVal;
                this.gpsTextBox.Text = t2.ToString();
            }
        }

        private void tripsTextBox_Leave(object sender, EventArgs e)
        {
            if (this.tripsTextBox.Text != "")
            {
                int val = int.Parse(this.tripsTextBox.Text);
                if (val < 1)
                {
                    MessageBox.Show("Number of Trip Blocks Cannot be less than 1", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.tripsTextBox.Text = "1";
                }
                if (val > 30)
                {
                    MessageBox.Show("Number of Trip Blocks cannot be greater than 30", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.tripsTextBox.Text = "30";
                }
            }
            else
            {
                MessageBox.Show("Trip Blocks cannot be Blank", "", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.tripsTextBox.Text = this.tripDefaultVal.ToString();
            }
        }
        private void sbsTextBox_Leave(object sender, EventArgs e)
        {
            if (this.sbsTextBox.Text != "")
            {
                int val = int.Parse(this.sbsTextBox.Text);
                if (val < 1)
                {
                    MessageBox.Show("Number of Sbs Blocks Cannot be less than 1", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.sbsTextBox.Text = "1";
                }
                if (val > 31)
                {
                    MessageBox.Show("Number of Sbs Blocks Cannot be greater than 31", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.sbsTextBox.Text = "31";
                }
            }
            else
            {
                MessageBox.Show("Error Sbs Field Cannot be Blank", "", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.sbsTextBox.Text = "1";
            }
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            int sbsVal = int.Parse(this.sbsTextBox.Text);
            int gpsVal = int.Parse(this.gpsTextBox.Text);
            int t = sbsVal + gpsVal;
            int t2;

            if (e.Type == ScrollEventType.SmallDecrement)
            {
                sbsVal++;
                if (sbsVal > t) sbsVal = t;
                this.sbsTextBox.Text = sbsVal.ToString();

            }
            else if (e.Type == ScrollEventType.SmallIncrement)
            {
                sbsVal--;
                if (sbsVal < 1) sbsVal = 1;
                this.sbsTextBox.Text = sbsVal.ToString();
            }
            t2 = t - sbsVal;
            this.gpsTextBox.Text = this.gpsTextBox.Text = t2.ToString();
        }

        #region Background Worker
        private void readFlashBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int tripBlocks, sbsBlocks, gpsBlocks;

            tripBlocks = int.Parse(this.tripsTextBox.Text);
            sbsBlocks = int.Parse(this.sbsTextBox.Text);
            gpsBlocks = int.Parse(this.gpsTextBox.Text);

            DialogResult res = MessageBox.Show(string.Format(
                "TRIP Blocks = {0:G}\nSBS Blocks = {1:G}\nGPS Blocks = {2:G}\n\nAll Data will be ERASED!!\n\n\nDo you wish to proceed?"
                , tripBlocks, sbsBlocks, gpsBlocks)
                , "WARNING!"
                , MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (res == DialogResult.Yes)
            {
                for (int i = 0; i < VMU.FLASH_BLOCKS; i++)
                {
                    this.vmu.newFlashConfig[i].blockNum = i;

                    if (i < tripBlocks)
                    {
                        this.vmu.newFlashConfig[i].type = 0x80;
                        this.vmu.newFlashConfig[i].recordSize = 0;
                    }
                    if ((i >= tripBlocks) && (i < tripBlocks + sbsBlocks))
                    {
                        this.vmu.newFlashConfig[i].type = 0x21;
                        this.vmu.newFlashConfig[i].recordSize = 8;
                    }
                    if ((i >= tripBlocks + sbsBlocks) && (i < tripBlocks + sbsBlocks + gpsBlocks))
                    {
                        this.vmu.newFlashConfig[i].type = 0x10;
                        this.vmu.newFlashConfig[i].recordSize = 18;
                    }
                }

                this.status = this.rs232comms.setVMUFlashConfig(this.currentPort, this.vmu.newFlashConfig, VMU.FLASH_BLOCKS);

                //this.currentPort.Close();

                if (this.status == COMMS_STATUS.OKAY)
                {
                    MessageBox.Show("Flash Memory Successfully Configured!!\nYour Device will restart shortly...", "CONFIRMATION!",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                //this.currentPort.Open();

                for (int i = 0; i <= 80; i++)
                {
                    Thread.Sleep(210);
                    readFlashBackgroundWorker.ReportProgress(i);

                    if (readFlashBackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        readFlashBackgroundWorker.ReportProgress(0);
                        return;
                    }
                }

                toolStripLabelRestart.Text = "Restarting Device!";

                for (int i = 81; i <= 94; i++)
                {
                    Thread.Sleep(210);
                    readFlashBackgroundWorker.ReportProgress(i);

                    if (readFlashBackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        readFlashBackgroundWorker.ReportProgress(0);
                        return;
                    }
                }

                toolStripLabelRestart.Text = "";

                for (int i = 95; i <= 100; i++)
                {
                    Thread.Sleep(210);
                    readFlashBackgroundWorker.ReportProgress(i);

                    if (readFlashBackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        readFlashBackgroundWorker.ReportProgress(0);
                        return;
                    }
                }

                readFlashBackgroundWorker.ReportProgress(0);
            }
        }
        private void readFlashBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            lblPercentage.Text = e.ProgressPercentage.ToString() + "%";
        }
        private void readFlashBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripLabelRestart.Text = "";
        }
        #endregion

        
    }
}
