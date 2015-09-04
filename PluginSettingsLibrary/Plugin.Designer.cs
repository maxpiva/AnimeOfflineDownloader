namespace PluginSettingsLibrary
{
    partial class Plugin
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.panel = new System.Windows.Forms.Panel();
            this.tableAuth = new System.Windows.Forms.TableLayoutPanel();
            this.labStatus = new System.Windows.Forms.Label();
            this.butTest = new System.Windows.Forms.Button();
            this.linkRegister = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbPlugin = new System.Windows.Forms.ComboBox();
            this.groupBox4.SuspendLayout();
            this.tableAuth.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.panel);
            this.groupBox4.Controls.Add(this.tableAuth);
            this.groupBox4.Controls.Add(this.tableLayoutPanel4);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.groupBox4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox4.Location = new System.Drawing.Point(0, 0);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(10);
            this.groupBox4.Size = new System.Drawing.Size(585, 213);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Plugin Settings";
            // 
            // panel
            // 
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.panel.Location = new System.Drawing.Point(10, 62);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(565, 51);
            this.panel.TabIndex = 7;
            // 
            // tableAuth
            // 
            this.tableAuth.AutoScroll = true;
            this.tableAuth.AutoSize = true;
            this.tableAuth.ColumnCount = 2;
            this.tableAuth.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableAuth.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableAuth.Controls.Add(this.labStatus, 0, 1);
            this.tableAuth.Controls.Add(this.butTest, 1, 2);
            this.tableAuth.Controls.Add(this.linkRegister, 1, 0);
            this.tableAuth.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableAuth.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tableAuth.Location = new System.Drawing.Point(10, 113);
            this.tableAuth.Name = "tableAuth";
            this.tableAuth.RowCount = 3;
            this.tableAuth.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableAuth.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableAuth.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableAuth.Size = new System.Drawing.Size(565, 90);
            this.tableAuth.TabIndex = 6;
            // 
            // labStatus
            // 
            this.labStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableAuth.SetColumnSpan(this.labStatus, 2);
            this.labStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labStatus.Location = new System.Drawing.Point(3, 33);
            this.labStatus.Name = "labStatus";
            this.labStatus.Size = new System.Drawing.Size(559, 23);
            this.labStatus.TabIndex = 5;
            this.labStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // butTest
            // 
            this.butTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.butTest.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butTest.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(255)))));
            this.butTest.Location = new System.Drawing.Point(172, 63);
            this.butTest.Name = "butTest";
            this.butTest.Size = new System.Drawing.Size(390, 23);
            this.butTest.TabIndex = 4;
            this.butTest.Text = "Test";
            this.butTest.UseVisualStyleBackColor = true;
            // 
            // linkRegister
            // 
            this.linkRegister.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.linkRegister.AutoSize = true;
            this.linkRegister.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.linkRegister.Location = new System.Drawing.Point(172, 5);
            this.linkRegister.Name = "linkRegister";
            this.linkRegister.Size = new System.Drawing.Size(390, 19);
            this.linkRegister.TabIndex = 6;
            this.linkRegister.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel4.Controls.Add(this.label11, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.cmbPlugin, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel4.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(10, 32);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(565, 30);
            this.tableLayoutPanel4.TabIndex = 3;
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label11.Location = new System.Drawing.Point(119, 7);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(47, 15);
            this.label11.TabIndex = 0;
            this.label11.Text = "Plugin :";
            // 
            // cmbPlugin
            // 
            this.cmbPlugin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPlugin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPlugin.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmbPlugin.FormattingEnabled = true;
            this.cmbPlugin.Location = new System.Drawing.Point(172, 4);
            this.cmbPlugin.Name = "cmbPlugin";
            this.cmbPlugin.Size = new System.Drawing.Size(390, 23);
            this.cmbPlugin.TabIndex = 6;
            // 
            // Plugin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.groupBox4);
            this.Name = "Plugin";
            this.Size = new System.Drawing.Size(585, 213);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tableAuth.ResumeLayout(false);
            this.tableAuth.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbPlugin;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.TableLayoutPanel tableAuth;
        private System.Windows.Forms.Label labStatus;
        private System.Windows.Forms.Button butTest;
        private System.Windows.Forms.LinkLabel linkRegister;
    }
}
