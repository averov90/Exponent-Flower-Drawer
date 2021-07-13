namespace ExponentFlowerDrawer {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.view = new System.Windows.Forms.PictureBox();
            this.cmd_log = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.cmd = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.view)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // view
            // 
            this.view.BackColor = System.Drawing.SystemColors.Control;
            this.view.Dock = System.Windows.Forms.DockStyle.Fill;
            this.view.Location = new System.Drawing.Point(0, 0);
            this.view.Name = "view";
            this.view.Size = new System.Drawing.Size(507, 411);
            this.view.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.view.TabIndex = 0;
            this.view.TabStop = false;
            // 
            // cmd_log
            // 
            this.cmd_log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmd_log.FormattingEnabled = true;
            this.cmd_log.HorizontalScrollbar = true;
            this.cmd_log.ItemHeight = 17;
            this.cmd_log.Items.AddRange(new object[] {
            "> help",
            "alpha {true|false} - prop - backgroung color mode",
            "autoinit {true|false} - prop - initiate pic on prop change",
            "autoregen {true|false} - prop - regenerate flower on prop change",
            "chide {true|false} - prop - draw dots on connections",
            "clean - func - clean the log",
            "eyl {float 0<} - prop - exponent generator y limit",
            "example - func - draw default flower",
            "gen - func - generate flower with current props",
            "help - func - show help info",
            "init - func - init pic",
            "lb {e|pi|float 1<} - prop - logoriphm basis",
            "pc {int 3+} - prop - count of petals",
            "pwp {float 0-100} - prop - pen width percent",
            "quality {medium|high|int 100+} - prop - set pic quality",
            "save - func - save image into file",
            "savep {path} - func - save image into specefied file",
            "sfp - func - show flower props",
            "spp - func - show program props"});
            this.cmd_log.Location = new System.Drawing.Point(2, 24);
            this.cmd_log.Name = "cmd_log";
            this.cmd_log.Size = new System.Drawing.Size(249, 327);
            this.cmd_log.TabIndex = 2;
            this.cmd_log.DoubleClick += new System.EventHandler(this.cmd_log_DoubleClick);
            this.cmd_log.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmd_log_KeyUp);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.button1.Location = new System.Drawing.Point(216, 375);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(36, 33);
            this.button1.TabIndex = 2;
            this.button1.TabStop = false;
            this.button1.Text = "↳";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cmd
            // 
            this.cmd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmd.Location = new System.Drawing.Point(3, 382);
            this.cmd.Name = "cmd";
            this.cmd.Size = new System.Drawing.Size(207, 25);
            this.cmd.TabIndex = 1;
            this.cmd.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmd_KeyUp);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.view);
            this.splitContainer1.Panel1MinSize = 500;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.cmd_log);
            this.splitContainer1.Panel2.Controls.Add(this.button1);
            this.splitContainer1.Panel2.Controls.Add(this.cmd);
            this.splitContainer1.Panel2MinSize = 160;
            this.splitContainer1.Size = new System.Drawing.Size(766, 411);
            this.splitContainer1.SplitterDistance = 507;
            this.splitContainer1.TabIndex = 3;
            this.splitContainer1.TabStop = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 362);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "cmd:";
            this.label2.UseMnemonic = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "cmd history & output:";
            this.label1.UseMnemonic = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(766, 411);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(720, 450);
            this.Name = "MainForm";
            this.Text = "Exponent Flower Drawer";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.view)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox view;
        private System.Windows.Forms.ListBox cmd_log;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox cmd;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}

