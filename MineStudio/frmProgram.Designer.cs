namespace MineStudio
{
    partial class frmProgram
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProgram));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnProgram = new System.Windows.Forms.Button();
            this.checkBoxVerify = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtLockBits = new System.Windows.Forms.TextBox();
            this.txtFuse = new System.Windows.Forms.TextBox();
            this.txtEeprom = new System.Windows.Forms.TextBox();
            this.txtFlash = new System.Windows.Forms.TextBox();
            this.txtBootloader = new System.Windows.Forms.TextBox();
            this.cmbProduct = new System.Windows.Forms.ComboBox();
            this.txtProcessor = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnChange = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.cmbProgrammer = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStripProgramming = new System.Windows.Forms.ToolStrip();
            this.btnExit = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.backgroundWorkerProgram = new System.ComponentModel.BackgroundWorker();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.statusStripProgramming.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.ForeColor = System.Drawing.Color.Maroon;
            this.btnCancel.Location = new System.Drawing.Point(241, 372);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "C&ancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnProgram
            // 
            this.btnProgram.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnProgram.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnProgram.ForeColor = System.Drawing.Color.Maroon;
            this.btnProgram.Location = new System.Drawing.Point(140, 372);
            this.btnProgram.Name = "btnProgram";
            this.btnProgram.Size = new System.Drawing.Size(75, 23);
            this.btnProgram.TabIndex = 2;
            this.btnProgram.Text = "&Program";
            this.btnProgram.UseVisualStyleBackColor = false;
            this.btnProgram.Click += new System.EventHandler(this.btnProgram_Click);
            // 
            // checkBoxVerify
            // 
            this.checkBoxVerify.AutoSize = true;
            this.checkBoxVerify.Checked = true;
            this.checkBoxVerify.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxVerify.Location = new System.Drawing.Point(39, 256);
            this.checkBoxVerify.Name = "checkBoxVerify";
            this.checkBoxVerify.Size = new System.Drawing.Size(139, 17);
            this.checkBoxVerify.TabIndex = 10;
            this.checkBoxVerify.Text = "Verfiy after programming";
            this.checkBoxVerify.UseVisualStyleBackColor = true;
            this.checkBoxVerify.CheckedChanged += new System.EventHandler(this.checkBoxVerify_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtLockBits);
            this.groupBox1.Controls.Add(this.txtFuse);
            this.groupBox1.Controls.Add(this.txtEeprom);
            this.groupBox1.Controls.Add(this.txtFlash);
            this.groupBox1.Controls.Add(this.checkBoxVerify);
            this.groupBox1.Controls.Add(this.txtBootloader);
            this.groupBox1.Controls.Add(this.cmbProduct);
            this.groupBox1.Controls.Add(this.txtProcessor);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.btnChange);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(12, 69);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(331, 283);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "DEVICE SETTINGS";
            // 
            // txtLockBits
            // 
            this.txtLockBits.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtLockBits.ForeColor = System.Drawing.Color.Black;
            this.txtLockBits.Location = new System.Drawing.Point(128, 167);
            this.txtLockBits.Name = "txtLockBits";
            this.txtLockBits.Size = new System.Drawing.Size(176, 20);
            this.txtLockBits.TabIndex = 6;
            this.txtLockBits.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtFuse
            // 
            this.txtFuse.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtFuse.ForeColor = System.Drawing.Color.Black;
            this.txtFuse.Location = new System.Drawing.Point(128, 142);
            this.txtFuse.Name = "txtFuse";
            this.txtFuse.Size = new System.Drawing.Size(176, 20);
            this.txtFuse.TabIndex = 5;
            this.txtFuse.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtEeprom
            // 
            this.txtEeprom.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtEeprom.ForeColor = System.Drawing.Color.Black;
            this.txtEeprom.Location = new System.Drawing.Point(128, 117);
            this.txtEeprom.Name = "txtEeprom";
            this.txtEeprom.Size = new System.Drawing.Size(176, 20);
            this.txtEeprom.TabIndex = 4;
            this.txtEeprom.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtFlash
            // 
            this.txtFlash.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtFlash.ForeColor = System.Drawing.Color.Black;
            this.txtFlash.Location = new System.Drawing.Point(128, 192);
            this.txtFlash.Name = "txtFlash";
            this.txtFlash.Size = new System.Drawing.Size(176, 20);
            this.txtFlash.TabIndex = 7;
            this.txtFlash.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBootloader
            // 
            this.txtBootloader.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtBootloader.ForeColor = System.Drawing.Color.Black;
            this.txtBootloader.Location = new System.Drawing.Point(128, 92);
            this.txtBootloader.Name = "txtBootloader";
            this.txtBootloader.Size = new System.Drawing.Size(176, 20);
            this.txtBootloader.TabIndex = 3;
            this.txtBootloader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cmbProduct
            // 
            this.cmbProduct.BackColor = System.Drawing.SystemColors.ControlDark;
            this.cmbProduct.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmbProduct.ForeColor = System.Drawing.Color.Black;
            this.cmbProduct.FormattingEnabled = true;
            this.cmbProduct.Location = new System.Drawing.Point(128, 32);
            this.cmbProduct.Name = "cmbProduct";
            this.cmbProduct.Size = new System.Drawing.Size(176, 21);
            this.cmbProduct.TabIndex = 0;
            this.cmbProduct.SelectedIndexChanged += new System.EventHandler(this.cmbProduct_SelectedIndexChanged);
            // 
            // txtProcessor
            // 
            this.txtProcessor.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtProcessor.ForeColor = System.Drawing.Color.Black;
            this.txtProcessor.Location = new System.Drawing.Point(128, 67);
            this.txtProcessor.Name = "txtProcessor";
            this.txtProcessor.Size = new System.Drawing.Size(176, 20);
            this.txtProcessor.TabIndex = 2;
            this.txtProcessor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 195);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Bootloader";
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.ForeColor = System.Drawing.Color.Maroon;
            this.btnSave.Location = new System.Drawing.Point(229, 227);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnChange
            // 
            this.btnChange.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnChange.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnChange.ForeColor = System.Drawing.Color.Maroon;
            this.btnChange.Location = new System.Drawing.Point(128, 227);
            this.btnChange.Name = "btnChange";
            this.btnChange.Size = new System.Drawing.Size(75, 23);
            this.btnChange.TabIndex = 1;
            this.btnChange.Text = "&Change";
            this.btnChange.UseVisualStyleBackColor = false;
            this.btnChange.Click += new System.EventHandler(this.btnChange_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(45, 170);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Lock Bits";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(66, 145);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Fuse";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(53, 120);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Eeprom";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(64, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Flash";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(42, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Processor";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "PRODUCT";
            // 
            // richTextBox
            // 
            this.richTextBox.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox.ForeColor = System.Drawing.Color.White;
            this.richTextBox.Location = new System.Drawing.Point(355, 12);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(498, 546);
            this.richTextBox.TabIndex = 8;
            this.richTextBox.Text = "";
            // 
            // cmbProgrammer
            // 
            this.cmbProgrammer.BackColor = System.Drawing.SystemColors.ControlDark;
            this.cmbProgrammer.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmbProgrammer.ForeColor = System.Drawing.Color.Black;
            this.cmbProgrammer.FormattingEnabled = true;
            this.cmbProgrammer.Location = new System.Drawing.Point(140, 26);
            this.cmbProgrammer.Name = "cmbProgrammer";
            this.cmbProgrammer.Size = new System.Drawing.Size(176, 21);
            this.cmbProgrammer.TabIndex = 0;
            this.cmbProgrammer.SelectedIndexChanged += new System.EventHandler(this.cmbProgrammer_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "PROGRAMMER";
            // 
            // statusStripProgramming
            // 
            this.statusStripProgramming.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.statusStripProgramming.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusStripProgramming.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.statusStripProgramming.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1,
            this.toolStripButton1,
            this.btnExit});
            this.statusStripProgramming.Location = new System.Drawing.Point(0, 569);
            this.statusStripProgramming.Name = "statusStripProgramming";
            this.statusStripProgramming.Size = new System.Drawing.Size(865, 25);
            this.statusStripProgramming.TabIndex = 13;
            this.statusStripProgramming.Text = "toolStrip1";
            // 
            // btnExit
            // 
            this.btnExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExit.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
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
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 22);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.Maroon;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(39, 22);
            this.toolStripStatusLabel1.Text = "Status";
            // 
            // backgroundWorkerProgram
            // 
            this.backgroundWorkerProgram.WorkerReportsProgress = true;
            this.backgroundWorkerProgram.WorkerSupportsCancellation = true;
            this.backgroundWorkerProgram.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerProgram_DoWork);
            this.backgroundWorkerProgram.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerProgram_ProgressChanged);
            this.backgroundWorkerProgram.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerProgram_RunWorkerCompleted);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(20, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(293, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "In order to Program devices, please make sure you install the";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(20, 42);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(123, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "following 2 AVR Studios:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(35, 117);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(61, 13);
            this.label12.TabIndex = 14;
            this.label12.Text = "Filename:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(103, 117);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(142, 13);
            this.label13.TabIndex = 14;
            this.label13.Text = "AStudio61sp1_1net (806Mb)";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(12, 409);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(331, 149);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ADDITIONAL SOFTWARE";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(103, 68);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(71, 13);
            this.label18.TabIndex = 14;
            this.label18.Text = "AVR Studio 4";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(103, 104);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(75, 13);
            this.label17.TabIndex = 14;
            this.label17.Text = "\'AVR Studio 6\'";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(103, 81);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(76, 13);
            this.label15.TabIndex = 14;
            this.label15.Text = "aStudio4b-589";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(20, 68);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(76, 13);
            this.label14.TabIndex = 14;
            this.label14.Text = "AVR Studio:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(20, 104);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(76, 13);
            this.label16.TabIndex = 14;
            this.label16.Text = "AVR Studio:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(35, 81);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(61, 13);
            this.label11.TabIndex = 14;
            this.label11.Text = "Filename:";
            // 
            // frmProgram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(865, 594);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.statusStripProgramming);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnProgram);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.cmbProgrammer);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "frmProgram";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Program Unit ";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmProgram_FormClosed);
            this.Load += new System.EventHandler(this.frmProgram_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmProgram_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statusStripProgramming.ResumeLayout(false);
            this.statusStripProgramming.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnProgram;
        private System.Windows.Forms.CheckBox checkBoxVerify;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtLockBits;
        private System.Windows.Forms.TextBox txtFuse;
        private System.Windows.Forms.TextBox txtEeprom;
        private System.Windows.Forms.TextBox txtFlash;
        private System.Windows.Forms.TextBox txtBootloader;
        private System.Windows.Forms.ComboBox cmbProduct;
        private System.Windows.Forms.TextBox txtProcessor;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnChange;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.ComboBox cmbProgrammer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStrip statusStripProgramming;
        private System.Windows.Forms.ToolStripLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerProgram;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolStripButton btnExit;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
    }
}