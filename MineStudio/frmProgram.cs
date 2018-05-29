using MineStudio.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MineStudio.frmMain;

namespace MineStudio
{
    public partial class frmProgram : Form
    {
        #region Properties
        private Process _process = new Process();
        private delegate void UpdateRichTexBoxDelegate(string text, bool clear);
        private delegate void UpdateToolStripTextDelegate(string text);
        private delegate void UpdateToolStripProgressBarDelegate(int value);
        private VMUCFG_Events vmucfg_events = new VMUCFG_Events();
        private string[] _deviceSettings;
        private bool _closeForm = false;
        private bool _loaded = false;
        #endregion

        #region Constructors
        public frmProgram()
        {
            InitializeComponent();

            this.btnCancel.Enabled = false;
            this.btnSave.Enabled = false;
            this._process.OutputDataReceived += new DataReceivedEventHandler(_process_OutputDataReceived);
            this._process.ErrorDataReceived += new DataReceivedEventHandler(_process_ErrorDataReceived);
        }
        private void frmProgram_Load(object sender, EventArgs e)
        {
            groupBox2.Visible = false;
            string[] programmers = Settings.Default.Programmers.Split(new char[] { ',' });
            string[] devices = Settings.Default.Devices.Split(new char[] { ',' });

            foreach (var s in programmers)
            {
                this.cmbProgrammer.Items.Add(s);
            }

            foreach (var s in devices)
            {
                this.cmbProduct.Items.Add(s);
            }

            this.cmbProgrammer.SelectedItem = Settings.Default.CurrentProgrammer;
            this.cmbProduct.SelectedItem = Settings.Default.CurrentDevice;
            this.checkBoxVerify.Checked = Settings.Default.Verify;
        }
        private void frmProgram_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.vmucfg_events.fireFormClosedEvent(this.Name);
        }
        private void frmProgram_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.P)
            {
                e.SuppressKeyPress = true;
                btnProgram.PerformClick();
            }

            if (e.Control && e.KeyCode == Keys.C)
            {
                e.SuppressKeyPress = true;
                btnChange.PerformClick();
            }

            if (e.Control && e.KeyCode == Keys.S)
            {
                e.SuppressKeyPress = true;
                btnSave.PerformClick();
            }

            if (e.Control && e.KeyCode == Keys.A)
            {
                e.SuppressKeyPress = true;
                btnCancel.PerformClick();
            }

            if (e.Control && e.KeyCode == Keys.X)
            {
                e.SuppressKeyPress = true;
                this.Close();
            }
        }
        #endregion

        #region Void
        private void _process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                UpdateRichTextBox(e.Data + "\n", false);

            }
        }
        private void _process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                UpdateRichTextBox(e.Data + "\n", false);
            }
        }
        private void UpdateRichTextBox(string text, bool clear)
        {
            if (this.richTextBox.InvokeRequired)
            {
                this.Invoke(new UpdateRichTexBoxDelegate(UpdateRichTextBox), new object[] { text, clear });
            }
            else
            {
                if (clear)
                {
                    this.richTextBox.Text = text;
                }
                else
                {
                    this.richTextBox.AppendText(text);
                }
            }
        }
        private void UpdateToolStripText(string text)
        {
            if (this.statusStripProgramming.InvokeRequired)
            {
                this.Invoke(new UpdateToolStripTextDelegate(UpdateToolStripText), new object[] { text });
            }
            else
            {
                this.toolStripStatusLabel1.Text = text;
            }
        }
        private void UpdateToolStripProgressBar(int value)
        {
            if (this.statusStripProgramming.InvokeRequired)
            {
                this.Invoke(new UpdateToolStripProgressBarDelegate(UpdateToolStripProgressBar), new object[] { value });
            }
            else
            {
                this.toolStripProgressBar1.Value = value;
            }
        }
        private void LoadDeviceSettings()
        {
            //string[] deviceSettings;

            switch ((string)this.cmbProduct.SelectedItem)
            {
                case "RAPTOR":
                    {
                        this._deviceSettings = Settings.Default.RAPTOR.Split(new char[] { ',' });
                        Settings.Default.CurrentDevice = "RAPTOR";

                        break;
                    }
                case "MINIDAL":
                    {
                        this._deviceSettings = Settings.Default.MINIDAL.Split(new char[] { ',' });
                        Settings.Default.CurrentDevice = "MINIDAL";

                        break;
                    }
                case "MS6":
                    {
                        this._deviceSettings = Settings.Default.MS6.Split(new char[] { ',' });
                        Settings.Default.CurrentDevice = "MS6";

                        break;
                    }
                case "SQSPEEDO_V2":
                    {
                        this._deviceSettings = Settings.Default.SQSPEEDO_V2.Split(new char[] { ',' });
                        Settings.Default.CurrentDevice = "SQSPEEDO_V2";

                        break;
                    }
                case "SQSPEEDO_V1":
                    {
                        this._deviceSettings = Settings.Default.SQSPEEDO_V1.Split(new char[] { ',' });
                        Settings.Default.CurrentDevice = "SQSPEEDO_V1";

                        break;
                    }
                case "LEACH":
                    {
                        this._deviceSettings = Settings.Default.LEACH.Split(new char[] { ',' });
                        Settings.Default.CurrentDevice = "LEACH";

                        break;
                    }
                case "PHAROS_TX":
                    {
                        this._deviceSettings = Settings.Default.PHAROS_TX.Split(new char[] { ',' });
                        Settings.Default.CurrentDevice = "PHAROS_TX";

                        break;
                    }
                case "PHAROS_RX":
                    {
                        this._deviceSettings = Settings.Default.PHAROS_RX.Split(new char[] { ',' });
                        Settings.Default.CurrentDevice = "PHAROS_RX";

                        break;
                    }
                case "ULOGGER":
                    {
                        this._deviceSettings = Settings.Default.ULOGGER.Split(new char[] { ',' });
                        Settings.Default.CurrentDevice = "ULOGGER";

                        break;
                    }
                case "XPORTLAN":
                    {
                        this._deviceSettings = Settings.Default.XPORTLAN.Split(new char[] { ',' });
                        Settings.Default.CurrentDevice = "XPORTLAN";

                        break;
                    }
                case "RTTEMP":
                    {
                        this._deviceSettings = Settings.Default.RTTEMP.Split(new char[] { ',' });
                        Settings.Default.CurrentDevice = "RTTEMP";

                        break;
                    }
                default:
                    {
                        return;
                    }
            }

            if (this._deviceSettings.Length != 7)
            {
                MessageBox.Show("Device Settings Error", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Settings.Default.Save();

            try
            {
                this.txtBootloader.Text = this._deviceSettings[1];
                this.txtEeprom.Text = this._deviceSettings[2];
                this.txtFlash.Text = this._deviceSettings[3];
                this.txtFuse.Text = this._deviceSettings[4];
                this.txtLockBits.Text = this._deviceSettings[5];
                this.txtProcessor.Text = this._deviceSettings[6];
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }
        private void SaveDeviceSettings()
        {
            string s = string.Format("{0:G},{1:G},{2:G},{3:G},{4:G},{5:G},{6:G}",
                this._deviceSettings[0],
                this._deviceSettings[1],
                this._deviceSettings[2],
                this._deviceSettings[3],
                this._deviceSettings[4],
                this._deviceSettings[5],
                this._deviceSettings[6]);

            switch ((string)this.cmbProduct.SelectedItem)
            {
                case "RAPTOR":
                    {
                        Settings.Default.RAPTOR = s;

                        break;
                    }
                case "MINIDAL":
                    {
                        Settings.Default.MINIDAL = s;

                        break;
                    }
                case "MS6":
                    {
                        Settings.Default.MS6 = s;

                        break;
                    }
                case "SQSPEEDO_V2":
                    {
                        Settings.Default.SQSPEEDO_V2 = s;

                        break;
                    }
                case "SQSPEEDO_V1":
                    {
                        Settings.Default.SQSPEEDO_V1 = s;

                        break;
                    }
                case "LEACH":
                    {
                        Settings.Default.LEACH = s;

                        break;
                    }
                case "PHAROS_TX":
                    {
                        Settings.Default.PHAROS_TX = s;

                        break;
                    }
                case "PHAROS_RX":
                    {
                        Settings.Default.PHAROS_RX = s;

                        break;
                    }
                case "ULOGGER":
                    {
                        Settings.Default.ULOGGER = s;

                        break;
                    }
                case "XPORTLAN":
                    {
                        Settings.Default.XPORTLAN = s;

                        break;
                    }
                case "RTTEMP":
                    {
                        Settings.Default.RTTEMP = s;

                        break;
                    }
                default:
                    {
                        return;
                    }
            }

            Settings.Default.Save();

        }
        private void SetEnable(bool enable)
        {
            this.cmbProgrammer.Enabled = enable;
            this.cmbProduct.Enabled = enable;
            this.txtProcessor.Enabled = enable;
            this.txtBootloader.Enabled = enable;
            this.txtFlash.Enabled = enable;
            this.txtEeprom.Enabled = enable;
            this.txtFuse.Enabled = enable;
            this.txtLockBits.Enabled = enable;
            this.btnChange.Enabled = enable;
            this.btnSave.Enabled = enable;
            this.btnProgram.Enabled = enable;
            this.btnCancel.Enabled = !enable;
        }
        private void SetChangeEnable(bool enable)
        {
            this.txtProcessor.ReadOnly = !enable;
            this.txtBootloader.ReadOnly = !enable;
            this.txtFlash.ReadOnly = !enable;
            this.txtEeprom.ReadOnly = !enable;
            this.txtFuse.ReadOnly = !enable;
            this.txtLockBits.ReadOnly = !enable;
            this.btnChange.Enabled = !enable;
            this.btnSave.Enabled = enable;

        }
        private void ExecuteCommandLineCommand(ProcessStartInfo pStartInfo)
        {
            try
            {
                this._process.StartInfo = pStartInfo;
                this._process.Start();
                this._process.BeginOutputReadLine();
                this._process.BeginErrorReadLine();
                this._process.WaitForExit();

                this._process.CancelOutputRead();
                this._process.CancelErrorRead();
                this._process.Close();

            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        #endregion

        #region Bool
        private bool FileExist(string path, string filename)
        {
            if (string.IsNullOrEmpty(filename)) return true;

            try
            {
                FileInfo fi = new FileInfo(path + "\\" + filename);
                return fi.Exists;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private bool IsProcessorValid(string name)
        {
            switch (name)
            {
                case "ATMEGA8":
                case "ATMEGA16":
                case "ATMEGA32":
                case "ATMEGA64":
                case "ATMEGA128":
                case "ATMEGA256": { return true; }
                default:
                    {
                        return false;
                    }
            }
        }
        private bool IsHexNumber(string number)
        {
            foreach (char c in number)
            {
                if (char.IsDigit(c) || ((c >= 'A') && (c <= 'F')) || ((c >= 'a') && (c <= 'f'))) { }
                else { return false; }
            }
            return true;
        }
        public bool FindMyText(string text)
        {
            // Initialize the return value to false by default.
            bool returnValue = false;

            // Ensure a search string has been specified.
            if (text.Length > 0)
            {
                // Obtain the location of the search string in richTextBox1.
                int textNot = richTextBox.Find("not");
                int textfailed = richTextBox.Find("failed");

                // Determine whether the text was found in richTextBox1.
                if (textNot >= 0)
                {
                    returnValue = true;
                }

                else if (textfailed >= 0)
                {
                    returnValue = true;
                }
            }

            return returnValue;
        }
        #endregion

        #region Background Worker
        private void backgroundWorkerProgram_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                BackgroundWorker worker = sender as BackgroundWorker;
                ProcessStartInfo pStartInfo = new ProcessStartInfo("Stk500\\Stk500.exe");
                string verifyStringBoot = (this.checkBoxVerify.Checked) ? " -vf " : " ";
                string verifyStringEEprom = (this.checkBoxVerify.Checked) ? " -ve " : " ";
                string verifyStringFlash = (this.checkBoxVerify.Checked) ? " -vf " : " ";
                string eraseFlash;

                pStartInfo.UseShellExecute = false;
                pStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                pStartInfo.CreateNoWindow = true;
                pStartInfo.RedirectStandardOutput = true;
                pStartInfo.RedirectStandardError = true;

                if (worker.CancellationPending) { e.Cancel = true; return; }

                if (!string.IsNullOrEmpty(this._deviceSettings[1]))
                {
                    worker.ReportProgress(0, "Programming Bootloader");
                    pStartInfo.Arguments = string.Format("-cUSB -d{0:G} -ms -e -pf{2:G}-if{1:G}", this._deviceSettings[6],
                        this._deviceSettings[0] + "\\" + this._deviceSettings[1], verifyStringBoot);
                    ExecuteCommandLineCommand(pStartInfo);
                    worker.ReportProgress(20, "");
                    eraseFlash = " ";
                }
                else
                {
                    worker.ReportProgress(20, "");
                    eraseFlash = " -e ";
                }

                if (worker.CancellationPending) { e.Cancel = true; return; }

                if (!string.IsNullOrEmpty(this._deviceSettings[2]))
                {
                    worker.ReportProgress(20, "Programming EEPROM");
                    pStartInfo.Arguments = string.Format("-cUSB -d{0:G} -ms -pe{2:G}-ie{1:G}", this._deviceSettings[6],
                        this._deviceSettings[0] + "\\" + this._deviceSettings[2], verifyStringEEprom);
                    ExecuteCommandLineCommand(pStartInfo);
                    worker.ReportProgress(40, "");
                }
                else
                {
                    worker.ReportProgress(40, "");
                }

                if (worker.CancellationPending) { e.Cancel = true; return; }

                if (!string.IsNullOrEmpty(this._deviceSettings[3]))
                {
                    worker.ReportProgress(40, "Programming Flash");
                    pStartInfo.Arguments = string.Format("-cUSB -d{0:G} -ms{3:G}-pf{2:G}-if{1:G}", this._deviceSettings[6],
                        this._deviceSettings[0] + "\\" + this._deviceSettings[3], verifyStringFlash, eraseFlash);
                    ExecuteCommandLineCommand(pStartInfo);
                    worker.ReportProgress(60, "");
                }
                else
                {
                    worker.ReportProgress(60, "");
                }

                if (worker.CancellationPending) { e.Cancel = true; return; }

                if ((!string.IsNullOrEmpty(this._deviceSettings[4])) && (!string.IsNullOrEmpty(this._deviceSettings[5])))
                {
                    worker.ReportProgress(60, "Programming FuseBits and LockBits");
                    pStartInfo.Arguments = string.Format("-cUSB -d{0:G} -ms -f{1:G} -F{1:G} -EFF -GFF -l{2:G} -L{2:G}", this._deviceSettings[6],
                        this._deviceSettings[4], this._deviceSettings[5]);
                    ExecuteCommandLineCommand(pStartInfo);
                    worker.ReportProgress(100, "");
                }
                else
                {
                    worker.ReportProgress(100, "");
                }
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateRichTextBox("", true);
            }
        }
        private void backgroundWorkerProgram_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.toolStripProgressBar1.Value = e.ProgressPercentage;
            if (!string.IsNullOrEmpty((string)e.UserState))
            {
                this.toolStripStatusLabel1.Text = (string)e.UserState;
            }
        }
        private void backgroundWorkerProgram_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.toolStripProgressBar1.Value = 0;
            this.toolStripStatusLabel1.Text = "Done";

            string s;
            if (e.Cancelled)
            {
                s = "Programming Cancelled";
            }
            else
            {
                if (FindMyText("not") == true || FindMyText("failed") == true)
                {
                    groupBox2.Visible = true;
                    s = "There was an error during programming!";
                }
                else { s = "Programming Successful!"; }
            }

            MessageBox.Show(s, "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SetEnable(true);

            if (this._closeForm)
            {
                this.Close();
            }
        }
        #endregion

        #region Button Click Events
        private void btnChange_Click(object sender, EventArgs e)
        {
            SetChangeEnable(true);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            this.btnChange.Enabled = true;

            if (!IsProcessorValid(this.txtProcessor.Text))
            {
                MessageBox.Show("Invalid Processor", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtProcessor.Text = this._deviceSettings[6];
                return;
            }

            if (!FileExist(_deviceSettings[0], this.txtBootloader.Text))
            {
                MessageBox.Show("Bootloader file does not exist", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtBootloader.Text = this._deviceSettings[1];
                return;
            }

            if (!FileExist(_deviceSettings[0], this.txtEeprom.Text))
            {
                MessageBox.Show("EEPROM file does not exist", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtEeprom.Text = this._deviceSettings[2];
                return;
            }

            if (!FileExist(_deviceSettings[0], this.txtFlash.Text))
            {
                MessageBox.Show("Flash file does not exist", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtFlash.Text = this._deviceSettings[3];
                return;
            }

            if (!IsHexNumber(this.txtFuse.Text))
            {
                MessageBox.Show("Wrong format in fuse bits", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtFuse.Text = this._deviceSettings[4];
                return;
            }

            if (!IsHexNumber(this.txtLockBits.Text))
            {
                MessageBox.Show("Wrong format in lock bits", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtLockBits.Text = this._deviceSettings[5];
                return;
            }

            this._deviceSettings[1] = this.txtBootloader.Text;
            this._deviceSettings[2] = this.txtEeprom.Text;
            this._deviceSettings[3] = this.txtFlash.Text;
            this._deviceSettings[4] = this.txtFuse.Text;
            this._deviceSettings[5] = this.txtLockBits.Text;
            this._deviceSettings[6] = this.txtProcessor.Text;

            SaveDeviceSettings();

            MessageBox.Show("Setting Saved Successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

            SetChangeEnable(false);
        }
        private void btnProgram_Click(object sender, EventArgs e)
        {
            SetEnable(false);
            this.richTextBox.Clear();
            backgroundWorkerProgram.RunWorkerAsync();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.backgroundWorkerProgram.IsBusy)
                {
                    this.backgroundWorkerProgram.CancelAsync();
                    this.btnCancel.Enabled = false;
                    this.toolStripStatusLabel1.Text = "Cancelling current operation. Please wait...";
                }
            }
            catch (Exception)
            {

            }
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region SelectedIndex_Changed
        private void cmbProgrammer_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((string)cmbProgrammer.SelectedItem)
            {
                case "AVRISP_MKII":
                    {
                        Settings.Default.CurrentProgrammer = "AVRISP_MKII";
                        break;
                    }
                default:
                    {
                        return;
                    }
            }

            Settings.Default.Save();
        }
        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDeviceSettings();
        }

        #endregion

        #region Checkbox Events
        private void checkBoxVerify_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.Verify = checkBoxVerify.Checked;
            Settings.Default.Save();
        }
        #endregion
    }
}
