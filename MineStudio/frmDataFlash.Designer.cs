namespace MineStudio
{
    partial class frmDataFlash
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDataFlash));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnWriteData = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
            this.btnReadDataFlash = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
            this.btnDownload = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton8 = new System.Windows.Forms.ToolStripButton();
            this.btnClearData = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton9 = new System.Windows.Forms.ToolStripButton();
            this.btnCancel = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton10 = new System.Windows.Forms.ToolStripButton();
            this.btnExit = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.lblPercentage = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabelRestart = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.listView1 = new System.Windows.Forms.ListView();
            this.item = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.seq = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.eraseCnt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.date = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.time = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.size = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.used = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.saved = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.label1 = new System.Windows.Forms.Label();
            this.tripsTextBox = new System.Windows.Forms.TextBox();
            this.gpsTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.sbsTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.flashConfigBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.downloadBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.readFlashBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnWriteData,
            this.toolStripButton6,
            this.btnReadDataFlash,
            this.toolStripButton7,
            this.btnDownload,
            this.toolStripButton8,
            this.btnClearData,
            this.toolStripButton9,
            this.btnCancel,
            this.toolStripButton10,
            this.btnExit,
            this.toolStripButton1,
            this.toolStripButton4,
            this.toolStripButton3,
            this.progressBar,
            this.lblPercentage,
            this.toolStripLabelRestart,
            this.toolStripButton2,
            this.toolStripButton5});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(659, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnWriteData
            // 
            this.btnWriteData.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnWriteData.ForeColor = System.Drawing.Color.IndianRed;
            this.btnWriteData.Image = ((System.Drawing.Image)(resources.GetObject("btnWriteData.Image")));
            this.btnWriteData.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnWriteData.Name = "btnWriteData";
            this.btnWriteData.Size = new System.Drawing.Size(39, 22);
            this.btnWriteData.Text = "&Write";
            this.btnWriteData.ToolTipText = "Ctrl + W";
            this.btnWriteData.Click += new System.EventHandler(this.btnWriteData_Click);
            // 
            // toolStripButton6
            // 
            this.toolStripButton6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton6.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton6.Image")));
            this.toolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton6.Name = "toolStripButton6";
            this.toolStripButton6.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton6.Text = "l";
            // 
            // btnReadDataFlash
            // 
            this.btnReadDataFlash.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnReadDataFlash.ForeColor = System.Drawing.Color.IndianRed;
            this.btnReadDataFlash.Image = ((System.Drawing.Image)(resources.GetObject("btnReadDataFlash.Image")));
            this.btnReadDataFlash.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReadDataFlash.Name = "btnReadDataFlash";
            this.btnReadDataFlash.Size = new System.Drawing.Size(37, 22);
            this.btnReadDataFlash.Text = "&Read";
            this.btnReadDataFlash.ToolTipText = "Ctrl + R";
            this.btnReadDataFlash.Click += new System.EventHandler(this.btnReadDataFlash_Click);
            // 
            // toolStripButton7
            // 
            this.toolStripButton7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton7.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton7.Image")));
            this.toolStripButton7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton7.Name = "toolStripButton7";
            this.toolStripButton7.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton7.Text = "l";
            // 
            // btnDownload
            // 
            this.btnDownload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDownload.ForeColor = System.Drawing.Color.IndianRed;
            this.btnDownload.Image = ((System.Drawing.Image)(resources.GetObject("btnDownload.Image")));
            this.btnDownload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(65, 22);
            this.btnDownload.Text = "&Download";
            this.btnDownload.ToolTipText = "Ctrl + D";
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // toolStripButton8
            // 
            this.toolStripButton8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton8.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton8.Image")));
            this.toolStripButton8.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton8.Name = "toolStripButton8";
            this.toolStripButton8.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton8.Text = "l";
            // 
            // btnClearData
            // 
            this.btnClearData.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnClearData.ForeColor = System.Drawing.Color.IndianRed;
            this.btnClearData.Image = ((System.Drawing.Image)(resources.GetObject("btnClearData.Image")));
            this.btnClearData.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClearData.Name = "btnClearData";
            this.btnClearData.Size = new System.Drawing.Size(38, 22);
            this.btnClearData.Text = "&Clear";
            this.btnClearData.ToolTipText = "Ctrl + C";
            this.btnClearData.Click += new System.EventHandler(this.btnClearData_Click);
            // 
            // toolStripButton9
            // 
            this.toolStripButton9.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton9.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton9.Image")));
            this.toolStripButton9.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton9.Name = "toolStripButton9";
            this.toolStripButton9.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton9.Text = "l";
            // 
            // btnCancel
            // 
            this.btnCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCancel.ForeColor = System.Drawing.Color.IndianRed;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(47, 22);
            this.btnCancel.Text = "C&ancel";
            this.btnCancel.ToolTipText = "Ctrl + A";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // toolStripButton10
            // 
            this.toolStripButton10.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton10.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton10.Image")));
            this.toolStripButton10.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton10.Name = "toolStripButton10";
            this.toolStripButton10.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton10.Text = "l";
            // 
            // btnExit
            // 
            this.btnExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExit.ForeColor = System.Drawing.Color.IndianRed;
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(29, 22);
            this.btnExit.Text = "E&xit";
            this.btnExit.ToolTipText = "Ctrl + X";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.toolStripButton1.Enabled = false;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.toolStripButton4.Enabled = false;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton4.Text = "toolStripButton4";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.toolStripButton3.Enabled = false;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton3.Text = "toolStripButton3";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 22);
            // 
            // lblPercentage
            // 
            this.lblPercentage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblPercentage.Name = "lblPercentage";
            this.lblPercentage.Size = new System.Drawing.Size(17, 22);
            this.lblPercentage.Text = "%";
            // 
            // toolStripLabelRestart
            // 
            this.toolStripLabelRestart.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.toolStripLabelRestart.Name = "toolStripLabelRestart";
            this.toolStripLabelRestart.Size = new System.Drawing.Size(0, 22);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.toolStripButton2.Enabled = false;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "toolStripButton2";
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.toolStripButton5.Enabled = false;
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton5.Text = "toolStripButton5";
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.item,
            this.type,
            this.seq,
            this.eraseCnt,
            this.date,
            this.time,
            this.size,
            this.used,
            this.saved});
            this.listView1.ForeColor = System.Drawing.Color.White;
            this.listView1.Location = new System.Drawing.Point(12, 119);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(636, 623);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView1_ItemChecked);
            // 
            // item
            // 
            this.item.Text = "Block";
            this.item.Width = 50;
            // 
            // type
            // 
            this.type.Text = "Type";
            this.type.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // seq
            // 
            this.seq.Text = "Seq";
            this.seq.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // eraseCnt
            // 
            this.eraseCnt.Text = "Erase Count";
            this.eraseCnt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.eraseCnt.Width = 100;
            // 
            // date
            // 
            this.date.Text = "Date";
            this.date.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.date.Width = 100;
            // 
            // time
            // 
            this.time.Text = "Time";
            this.time.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.time.Width = 71;
            // 
            // size
            // 
            this.size.Text = "Size";
            this.size.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.size.Width = 65;
            // 
            // used
            // 
            this.used.Text = "Used";
            this.used.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.used.Width = 65;
            // 
            // saved
            // 
            this.saved.Text = "Saved";
            this.saved.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.saved.Width = 65;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.vScrollBar1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tripsTextBox);
            this.groupBox1.Controls.Add(this.gpsTextBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.sbsTextBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 36);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(636, 72);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "WRITE DATA CONFIGURATION";
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Location = new System.Drawing.Point(390, 28);
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(17, 20);
            this.vScrollBar1.TabIndex = 11;
            this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "# of Trip Blocks";
            // 
            // tripsTextBox
            // 
            this.tripsTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.tripsTextBox.Location = new System.Drawing.Point(94, 28);
            this.tripsTextBox.Name = "tripsTextBox";
            this.tripsTextBox.Size = new System.Drawing.Size(83, 20);
            this.tripsTextBox.TabIndex = 9;
            this.tripsTextBox.TextChanged += new System.EventHandler(this.tripsTextBox_TextChanged);
            this.tripsTextBox.Leave += new System.EventHandler(this.tripsTextBox_Leave);
            // 
            // gpsTextBox
            // 
            this.gpsTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.gpsTextBox.Location = new System.Drawing.Point(547, 28);
            this.gpsTextBox.Name = "gpsTextBox";
            this.gpsTextBox.Size = new System.Drawing.Size(83, 20);
            this.gpsTextBox.TabIndex = 7;
            this.gpsTextBox.TextChanged += new System.EventHandler(this.gpsTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(213, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "# of SBS Blocks";
            // 
            // sbsTextBox
            // 
            this.sbsTextBox.BackColor = System.Drawing.SystemColors.ControlDark;
            this.sbsTextBox.Location = new System.Drawing.Point(304, 28);
            this.sbsTextBox.Name = "sbsTextBox";
            this.sbsTextBox.Size = new System.Drawing.Size(83, 20);
            this.sbsTextBox.TabIndex = 8;
            this.sbsTextBox.TextChanged += new System.EventHandler(this.sbsTextBox_TextChanged);
            this.sbsTextBox.Leave += new System.EventHandler(this.sbsTextBox_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(455, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "# of GPS Blocks";
            // 
            // flashConfigBackgroundWorker
            // 
            this.flashConfigBackgroundWorker.WorkerReportsProgress = true;
            this.flashConfigBackgroundWorker.WorkerSupportsCancellation = true;
            this.flashConfigBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.flashConfigBackgroundWorker_DoWork);
            this.flashConfigBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.flashConfigBackgroundWorker_ProgressChanged);
            this.flashConfigBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.flashConfigBackgroundWorker_RunWorkerCompleted);
            // 
            // toolStrip2
            // 
            this.toolStrip2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.toolStrip2.Location = new System.Drawing.Point(0, 745);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(659, 25);
            this.toolStrip2.TabIndex = 3;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.Maroon;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(39, 22);
            this.toolStripStatusLabel1.Text = "Status";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 22);
            // 
            // downloadBackgroundWorker
            // 
            this.downloadBackgroundWorker.WorkerReportsProgress = true;
            this.downloadBackgroundWorker.WorkerSupportsCancellation = true;
            this.downloadBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.downloadBackgroundWorker_DoWork);
            this.downloadBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.downloadBackgroundWorker_ProgressChanged);
            this.downloadBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.downloadBackgroundWorker_RunWorkerCompleted);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.SelectedPath = "C:\\Users\\faf78\\Desktop";
            // 
            // readFlashBackgroundWorker
            // 
            this.readFlashBackgroundWorker.WorkerReportsProgress = true;
            this.readFlashBackgroundWorker.WorkerSupportsCancellation = true;
            this.readFlashBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.readFlashBackgroundWorker_DoWork);
            this.readFlashBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.readFlashBackgroundWorker_ProgressChanged);
            this.readFlashBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.readFlashBackgroundWorker_RunWorkerCompleted);
            // 
            // frmDataFlash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(659, 770);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.toolStrip1);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximumSize = new System.Drawing.Size(675, 809);
            this.MinimumSize = new System.Drawing.Size(675, 809);
            this.Name = "frmDataFlash";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data Flash";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDataFlash_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmDataFlash_FormClosed);
            this.Load += new System.EventHandler(this.frmDataFlash_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmDataFlash_KeyDown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnReadDataFlash;
        private System.Windows.Forms.ToolStripButton btnExit;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tripsTextBox;
        private System.Windows.Forms.TextBox gpsTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox sbsTextBox;
        private System.Windows.Forms.Label label3;
        private System.ComponentModel.BackgroundWorker flashConfigBackgroundWorker;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.ComponentModel.BackgroundWorker downloadBackgroundWorker;
        private System.Windows.Forms.ToolStripButton btnDownload;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolStripButton btnClearData;
        private System.Windows.Forms.ToolStripButton btnCancel;
        private System.Windows.Forms.ColumnHeader item;
        private System.Windows.Forms.ColumnHeader type;
        private System.Windows.Forms.ColumnHeader seq;
        private System.Windows.Forms.ColumnHeader eraseCnt;
        private System.Windows.Forms.ColumnHeader date;
        private System.Windows.Forms.ColumnHeader time;
        private System.Windows.Forms.ColumnHeader size;
        private System.Windows.Forms.ColumnHeader used;
        private System.Windows.Forms.ColumnHeader saved;
        private System.ComponentModel.BackgroundWorker readFlashBackgroundWorker;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton btnWriteData;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripLabel lblPercentage;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripLabel toolStripLabelRestart;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
        private System.Windows.Forms.ToolStripButton toolStripButton6;
        private System.Windows.Forms.ToolStripButton toolStripButton7;
        private System.Windows.Forms.ToolStripButton toolStripButton8;
        private System.Windows.Forms.ToolStripButton toolStripButton9;
        private System.Windows.Forms.ToolStripButton toolStripButton10;
    }
}