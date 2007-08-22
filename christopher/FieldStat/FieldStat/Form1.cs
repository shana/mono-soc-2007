using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using FieldStat.DataCollection;

namespace FieldStat
{
    public partial class Form1 : Form
    {
        ParameterData m_parameterData;
        ExportData m_export;

        public Form1()
        {
            InitializeComponent();
            m_parameterData = new ParameterData();
            m_export = new ExportData(m_parameterData.Results, Application.StartupPath +  "\\output");

            DoDataBinding();

            //m_parameterData.Results.AnalysisResults.DefaultView.s
            //m_dgResults.RowPrePaint += new DataGridViewRowPrePaintEventHandler(m_dgResults_RowPrePaint);
            m_dgResults.CellFormatting += new DataGridViewCellFormattingEventHandler(m_dgResults_CellFormatting);
            //m_dgResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        void m_dgResults_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewRow view = m_dgResults.Rows[e.RowIndex];
            ColorRow(view);
        }

        private void DoDataBinding()
        {
            m_parameterData.Results.Assemblies.DefaultView.AllowNew = false;
            m_dgAssemblies.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            m_dgAssemblies.DataSource = m_parameterData.Results.Assemblies;

            m_parameterData.Results.Filters.DefaultView.AllowNew = false;
            m_dgTypeFilters.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            m_dgTypeFilters.DataSource = m_parameterData.Results.Filters;

            m_parameterData.Results.AnalysisResults.DefaultView.AllowNew = false;
            m_dgResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            m_dgResults.DataSource = m_parameterData.Results.AnalysisResults;

            m_parameterData.Results.Coverage.DefaultView.AllowNew = false;
            m_dgCoverage.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            m_dgCoverage.DataSource = m_parameterData.Results.Coverage;

            m_parameterData.Results.ApplicationStats.DefaultView.AllowNew = false;
            m_dgAppStats.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            m_dgAppStats.DataSource = m_parameterData.Results.ApplicationStats;

            m_parameterData.Results.CodeRank.DefaultView.AllowNew = false;
            m_dgCodeRank.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            m_dgCodeRank.DataSource = m_parameterData.Results.CodeRank;
        }

        #region Events
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                m_parameterData.LoadAssemblies(ofd.FileNames);
            }
        }

        private void btnAddFilter_Click(object sender, EventArgs e)
        {
            if( txtAddFilter.Text != "" )
                m_parameterData.LoadFilters( new string[]{ txtAddFilter.Text } );
        }

        private void btnRemoveFilter_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in m_dgTypeFilters.SelectedRows)
            {
                m_parameterData.Results.Filters.Rows.Remove(((DataRowView)row.DataBoundItem).Row);
            }
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            m_parameterData.ComputeResults();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            m_export.ExportResults();
        }

        private void btnCoverageLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                m_parameterData.LoadCoverage(ofd.FileNames);
            }
        }

        private void btnScanRepos_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                m_parameterData.LoadApplicationRepository(fbd.SelectedPath);
            }
        }

        private void btnImportResults_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                m_parameterData.ImportCachedResults(ofd.FileName);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            CoverageReport report = new CoverageReport();
            report.CalcReport(m_parameterData.Results.AnalysisResults);
            report.Show();
        }
        #endregion

        void ColorRows()
        {
            foreach( DataGridViewRow view in m_dgResults.Rows )   
            {
                ColorRow(view);
            }
        }
        void ColorRow( DataGridViewRow view )
        {
            DataRow dr = ((DataRowView)view.DataBoundItem).Row;
            float coverage = (float)dr["Coverage"];
            if (coverage == -1)
            {
                view.DefaultCellStyle.BackColor = Color.Pink;
            }
            else if (coverage == 0)
            {
                view.DefaultCellStyle.BackColor = Color.Red;
            }
            else
            {
                view.DefaultCellStyle.BackColor = Color.FromArgb(0, (int)(coverage * 255), 0);
            }
        }

        private void btnPluginImportResults_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                DataTable dt = new DataTable();
                dt.ReadXml(ofd.FileName);
                m_dgPluginResults.DataSource = dt;
            }
        }

        int m_currentIndex = -1;
        private void btnNextPluginResult_Click(object sender, EventArgs e)
        {
            if (m_parameterData.Results.PluginTables.Count == 0)
                return;
            m_currentIndex += 1;
            if( m_currentIndex >= m_parameterData.Results.PluginTables.Count )
            {
                m_currentIndex = 0;
            }
            DataTable dt = (DataTable)m_parameterData.Results.PluginTables[m_currentIndex];
            m_dgPluginResults.DataSource = dt;
        }
    }
}