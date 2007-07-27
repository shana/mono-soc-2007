using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FieldStat
{
    public partial class Form1 : Form
    {
        DataTable m_dtAssemblies;
        ArrayList m_Files;
        DataTable m_dtFilters;
        DataTable m_dtResults;
        DataTable m_dtCoverage;

        public Form1()
        {
            InitializeComponent();

            // Make Tables
            m_dtAssemblies = new DataTable();
            m_dtAssemblies.Columns.Add("AssemblyName", typeof(string) );
            m_dtAssemblies.Columns.Add("Stats", typeof(string));
            m_dtAssemblies.Columns.Add("Assembly", typeof(object));

            m_dtFilters = new DataTable();
            m_dtFilters.Columns.Add("Filter", typeof(string));

            m_dtCoverage = new DataTable();
            m_dtCoverage.Columns.Add("Type", typeof(string));
            m_dtCoverage.Columns.Add("Method", typeof(string));
            m_dtCoverage.Columns.Add("Coverage", typeof(float));
            m_dtCoverage.Columns.Add("Length", typeof(int));

            m_dtResults = new DataTable();
            m_dtResults.Columns.Add("Type", typeof(string));
            m_dtResults.Columns.Add("Method", typeof(string));
            m_dtResults.Columns.Add("Length", typeof(int));
            m_dtResults.Columns.Add("Frequency", typeof(int));
            m_dtResults.Columns.Add("AppFrequency", typeof(int));
            m_dtResults.Columns.Add("Coverage", typeof(float));
            m_dtResults.PrimaryKey = new DataColumn[2] { m_dtResults.Columns["Type"], m_dtResults.Columns["Method"]};

            // Data Binding
            m_dtAssemblies.DefaultView.AllowNew = false;
            m_dgAssemblies.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            m_dgAssemblies.DataSource = m_dtAssemblies;

            m_dtFilters.DefaultView.AllowNew = false;
            m_dgTypeFilters.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            m_dgTypeFilters.DataSource = m_dtFilters;

            m_dtResults.DefaultView.AllowNew = false;
            m_dgResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            m_dgResults.DataSource = m_dtResults;

            m_dtCoverage.DefaultView.AllowNew = false;
            m_dgCoverage.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            m_dgCoverage.DataSource = m_dtCoverage;
        }
        
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in ofd.FileNames)
                {
                    string name = System.IO.Path.GetFileNameWithoutExtension(fileName);
                    m_dtAssemblies.Rows.Add(name, "0", null);
                }
                m_Files = new ArrayList(ofd.FileNames);
            }
        }

        private void btnAddFilter_Click(object sender, EventArgs e)
        {
            if( txtAddFilter.Text != "" )
                m_dtFilters.Rows.Add(txtAddFilter.Text);
        }

        private void btnRemoveFilter_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in m_dgTypeFilters.SelectedRows)
            {
                m_dtFilters.Rows.Remove(((DataRowView)row.DataBoundItem).Row);
            }
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            Scan scan = new Scan();
            Hashtable result = null;
            if (m_Files != null)
            {
                result = scan.ScanAssemblies(m_Files, null);
            }
            else
            {
                result = scan.ScanApps(htAppBin, null);
            }
            foreach (TypeResult tr in result.Values)
            {
                foreach( MethodResult mr in tr.Methods.Values )
                {
                    m_dtResults.Rows.Add(tr.Name, mr.Name, -1, mr.MethodFrequency, mr.TotalAppCount, -1);
                }
            }
            JoinCoverage();
        }

        private void JoinCoverage()
        {
            foreach (DataRow row in m_dtCoverage.Select("","",DataViewRowState.CurrentRows) )
            {
                DataRow resultDr = m_dtResults.Rows.Find(new object[] { row["Type"], row["Method"] });
                if (resultDr != null)
                {
                    resultDr["Coverage"] = row["Coverage"];
                    resultDr["Length"] = row["Length"];
                }
            }
        }


        private void btnExport_Click(object sender, EventArgs e)
        {
            m_dtResults.TableName = "Results";
            m_dtResults.WriteXml( Application.StartupPath + "\\Results.xml", XmlWriteMode.WriteSchema);
        }

        private void btnCoverageLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in ofd.FileNames)
                {
                    ImportCoverage imp = new ImportCoverage();
                    imp.Import(fileName, m_dtCoverage);  
                }
            }
        }

        private void btnScanRepos_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string baseDirectory = fbd.SelectedPath;
                DirectoryInfo app_repos = new DirectoryInfo(baseDirectory);
                foreach (DirectoryInfo app in app_repos.GetDirectories())
                {
                    ScanBins(app.Name, app);
                }
            }
        }
        Hashtable htGlobalBin = new Hashtable();
        Hashtable htAppBin = new Hashtable();
        private void ScanBins(string parent, DirectoryInfo app)
        {
            foreach (FileInfo fi in app.GetFiles() )
            {
                if (fi.Extension != ".exe" &&
                    fi.Extension != ".dll")
                    continue;
                htGlobalBin[fi.Name] = fi.Name;
                if (!htAppBin.Contains(parent))
                    htAppBin[parent] = new ArrayList();
                ArrayList list = (ArrayList)htAppBin[parent];
                list.Add(fi.FullName);
            }
            foreach (DirectoryInfo child in app.GetDirectories())
            {
                ScanBins(parent, child);
            }
        }

        private void btnImportResults_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                m_dtResults.Reset();
                m_dtResults.ReadXml(ofd.FileName);
                // Databinding isn't notified of readxml.  This hack notifies by resorting.
                m_dtResults.DefaultView.Sort = "AppFrequency DESC";
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            CoverageReport report = new CoverageReport();
            report.CalcReport(m_dtResults);
            report.Show();
        }
    }
}