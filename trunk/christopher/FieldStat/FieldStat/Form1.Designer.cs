namespace FieldStat
{
    partial class Form1
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.btnScanRepos = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.m_dgAssemblies = new System.Windows.Forms.DataGridView();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.btnCoverageLoad = new System.Windows.Forms.Button();
            this.m_dgCoverage = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnRemoveFilter = new System.Windows.Forms.Button();
            this.btnAddFilter = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl1 = new System.Windows.Forms.Label();
            this.txtAddFilter = new System.Windows.Forms.TextBox();
            this.m_dgTypeFilters = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btnScan = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.btnImportResults = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.m_dgResults = new System.Windows.Forms.DataGridView();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dgAssemblies)).BeginInit();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dgCoverage)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dgTypeFilters)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dgResults)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(0, 1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(802, 287);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.btnScanRepos);
            this.tabPage1.Controls.Add(this.btnLoad);
            this.tabPage1.Controls.Add(this.m_dgAssemblies);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(794, 261);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Assemblies";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(255, 222);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(185, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Scans directory structure for dlls, exes";
            // 
            // btnScanRepos
            // 
            this.btnScanRepos.Location = new System.Drawing.Point(127, 217);
            this.btnScanRepos.Name = "btnScanRepos";
            this.btnScanRepos.Size = new System.Drawing.Size(122, 23);
            this.btnScanRepos.TabIndex = 2;
            this.btnScanRepos.Text = "Scan App Repository";
            this.btnScanRepos.UseVisualStyleBackColor = true;
            this.btnScanRepos.Click += new System.EventHandler(this.btnScanRepos_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(31, 217);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load File(s)";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // m_dgAssemblies
            // 
            this.m_dgAssemblies.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_dgAssemblies.Location = new System.Drawing.Point(31, 15);
            this.m_dgAssemblies.Name = "m_dgAssemblies";
            this.m_dgAssemblies.Size = new System.Drawing.Size(406, 196);
            this.m_dgAssemblies.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.btnCoverageLoad);
            this.tabPage5.Controls.Add(this.m_dgCoverage);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(794, 261);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Coverage";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // btnCoverageLoad
            // 
            this.btnCoverageLoad.Location = new System.Drawing.Point(30, 220);
            this.btnCoverageLoad.Name = "btnCoverageLoad";
            this.btnCoverageLoad.Size = new System.Drawing.Size(75, 23);
            this.btnCoverageLoad.TabIndex = 3;
            this.btnCoverageLoad.Text = "Load File(s)";
            this.btnCoverageLoad.UseVisualStyleBackColor = true;
            this.btnCoverageLoad.Click += new System.EventHandler(this.btnCoverageLoad_Click);
            // 
            // m_dgCoverage
            // 
            this.m_dgCoverage.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_dgCoverage.Location = new System.Drawing.Point(30, 18);
            this.m_dgCoverage.Name = "m_dgCoverage";
            this.m_dgCoverage.Size = new System.Drawing.Size(406, 196);
            this.m_dgCoverage.TabIndex = 2;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnRemoveFilter);
            this.tabPage2.Controls.Add(this.btnAddFilter);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.lbl1);
            this.tabPage2.Controls.Add(this.txtAddFilter);
            this.tabPage2.Controls.Add(this.m_dgTypeFilters);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(794, 261);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Type Params";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnRemoveFilter
            // 
            this.btnRemoveFilter.Location = new System.Drawing.Point(288, 171);
            this.btnRemoveFilter.Name = "btnRemoveFilter";
            this.btnRemoveFilter.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveFilter.TabIndex = 6;
            this.btnRemoveFilter.Text = "Remove";
            this.btnRemoveFilter.UseVisualStyleBackColor = true;
            this.btnRemoveFilter.Click += new System.EventHandler(this.btnRemoveFilter_Click);
            // 
            // btnAddFilter
            // 
            this.btnAddFilter.Location = new System.Drawing.Point(207, 171);
            this.btnAddFilter.Name = "btnAddFilter";
            this.btnAddFilter.Size = new System.Drawing.Size(75, 23);
            this.btnAddFilter.TabIndex = 5;
            this.btnAddFilter.Text = "Add";
            this.btnAddFilter.UseVisualStyleBackColor = true;
            this.btnAddFilter.Click += new System.EventHandler(this.btnAddFilter_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(89, 197);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "(e.g. System.IO.*)";
            // 
            // lbl1
            // 
            this.lbl1.AutoSize = true;
            this.lbl1.Location = new System.Drawing.Point(32, 177);
            this.lbl1.Name = "lbl1";
            this.lbl1.Size = new System.Drawing.Size(29, 13);
            this.lbl1.TabIndex = 3;
            this.lbl1.Text = "Filter";
            // 
            // txtAddFilter
            // 
            this.txtAddFilter.Location = new System.Drawing.Point(67, 174);
            this.txtAddFilter.Name = "txtAddFilter";
            this.txtAddFilter.Size = new System.Drawing.Size(125, 20);
            this.txtAddFilter.TabIndex = 2;
            // 
            // m_dgTypeFilters
            // 
            this.m_dgTypeFilters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_dgTypeFilters.Location = new System.Drawing.Point(35, 25);
            this.m_dgTypeFilters.Name = "m_dgTypeFilters";
            this.m_dgTypeFilters.Size = new System.Drawing.Size(340, 131);
            this.m_dgTypeFilters.TabIndex = 1;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btnScan);
            this.tabPage3.Controls.Add(this.checkBox1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(794, 261);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Analysis";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(412, 203);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(75, 23);
            this.btnScan.TabIndex = 2;
            this.btnScan.Text = "Start";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(32, 37);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(108, 17);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Use Code Rank?";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.btnImportResults);
            this.tabPage4.Controls.Add(this.button6);
            this.tabPage4.Controls.Add(this.button5);
            this.tabPage4.Controls.Add(this.btnExport);
            this.tabPage4.Controls.Add(this.m_dgResults);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(794, 261);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Results";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // btnImportResults
            // 
            this.btnImportResults.Location = new System.Drawing.Point(359, 221);
            this.btnImportResults.Name = "btnImportResults";
            this.btnImportResults.Size = new System.Drawing.Size(91, 23);
            this.btnImportResults.TabIndex = 5;
            this.btnImportResults.Text = "Import Results";
            this.btnImportResults.UseVisualStyleBackColor = true;
            this.btnImportResults.Click += new System.EventHandler(this.btnImportResults_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(235, 221);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(108, 23);
            this.button6.TabIndex = 4;
            this.button6.Text = "Coverage Report";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(124, 221);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(93, 23);
            this.button5.TabIndex = 3;
            this.button5.Text = "Visual Report";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(31, 221);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // m_dgResults
            // 
            this.m_dgResults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_dgResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_dgResults.Location = new System.Drawing.Point(31, 15);
            this.m_dgResults.Name = "m_dgResults";
            this.m_dgResults.Size = new System.Drawing.Size(654, 196);
            this.m_dgResults.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 286);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "FieldStat";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dgAssemblies)).EndInit();
            this.tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_dgCoverage)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_dgTypeFilters)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_dgResults)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.DataGridView m_dgAssemblies;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button btnRemoveFilter;
        private System.Windows.Forms.Button btnAddFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl1;
        private System.Windows.Forms.TextBox txtAddFilter;
        private System.Windows.Forms.DataGridView m_dgTypeFilters;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.DataGridView m_dgResults;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Button btnCoverageLoad;
        private System.Windows.Forms.DataGridView m_dgCoverage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnScanRepos;
        private System.Windows.Forms.Button btnImportResults;

    }
}

