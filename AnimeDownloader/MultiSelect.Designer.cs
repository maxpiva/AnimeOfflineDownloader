namespace AnimeDownloader
{
    partial class MultiSelect
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
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbQuality = new System.Windows.Forms.ComboBox();
            this.butOk = new System.Windows.Forms.Button();
            this.chkListBox = new System.Windows.Forms.CheckedListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.objEpisodes = new BrightIdeasSoftware.ObjectListView();
            this.olvFile = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tableLayoutPanel3.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objEpisodes)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label4, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.cmbQuality, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.butOk, 2, 2);
            this.tableLayoutPanel3.Controls.Add(this.chkListBox, 3, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 281);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.Padding = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(543, 99);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(110, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 15);
            this.label3.TabIndex = 15;
            this.label3.Text = "Quality :";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(375, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 15);
            this.label4.TabIndex = 16;
            this.label4.Text = "Format :";
            // 
            // cmbQuality
            // 
            this.cmbQuality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQuality.FormattingEnabled = true;
            this.cmbQuality.Location = new System.Drawing.Point(167, 8);
            this.cmbQuality.Name = "cmbQuality";
            this.cmbQuality.Size = new System.Drawing.Size(100, 23);
            this.cmbQuality.TabIndex = 17;
            // 
            // butOk
            // 
            this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.SetColumnSpan(this.butOk, 2);
            this.butOk.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butOk.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(255)))));
            this.butOk.Location = new System.Drawing.Point(273, 67);
            this.butOk.Name = "butOk";
            this.butOk.Size = new System.Drawing.Size(262, 24);
            this.butOk.TabIndex = 21;
            this.butOk.Text = "Download";
            this.butOk.UseVisualStyleBackColor = true;
            // 
            // chkListBox
            // 
            this.chkListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkListBox.BackColor = System.Drawing.SystemColors.Control;
            this.chkListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chkListBox.FormattingEnabled = true;
            this.chkListBox.Location = new System.Drawing.Point(432, 10);
            this.chkListBox.Name = "chkListBox";
            this.chkListBox.Size = new System.Drawing.Size(103, 18);
            this.chkListBox.TabIndex = 24;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.objEpisodes);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(543, 281);
            this.panel1.TabIndex = 3;
            // 
            // objEpisodes
            // 
            this.objEpisodes.AllColumns.Add(this.olvFile);
            this.objEpisodes.AlternateRowBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(200)))));
            this.objEpisodes.CellEditUseWholeCell = false;
            this.objEpisodes.CheckBoxes = true;
            this.objEpisodes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvFile});
            this.objEpisodes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objEpisodes.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.objEpisodes.FullRowSelect = true;
            this.objEpisodes.GridLines = true;
            this.objEpisodes.HideSelection = false;
            this.objEpisodes.Location = new System.Drawing.Point(5, 5);
            this.objEpisodes.MultiSelect = false;
            this.objEpisodes.Name = "objEpisodes";
            this.objEpisodes.ShowImagesOnSubItems = true;
            this.objEpisodes.ShowItemCountOnGroups = true;
            this.objEpisodes.Size = new System.Drawing.Size(533, 271);
            this.objEpisodes.SortGroupItemsByPrimaryColumn = false;
            this.objEpisodes.TabIndex = 6;
            this.objEpisodes.UseAlternatingBackColors = true;
            this.objEpisodes.UseCompatibleStateImageBehavior = false;
            this.objEpisodes.View = System.Windows.Forms.View.Details;
            // 
            // olvFile
            // 
            this.olvFile.AspectName = "Name";
            this.olvFile.FillsFreeSpace = true;
            this.olvFile.Groupable = false;
            this.olvFile.HeaderFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.olvFile.MinimumWidth = 50;
            this.olvFile.Text = "Name";
            this.olvFile.Width = 150;
            // 
            // MultiSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(543, 380);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tableLayoutPanel3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MultiSelect";
            this.ShowInTaskbar = false;
            this.Text = "Select Episodes to download";
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.objEpisodes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbQuality;
        private System.Windows.Forms.Button butOk;
        private System.Windows.Forms.Panel panel1;
        private BrightIdeasSoftware.ObjectListView objEpisodes;
        private BrightIdeasSoftware.OLVColumn olvFile;
        private System.Windows.Forms.CheckedListBox chkListBox;
    }
}