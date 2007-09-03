using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

using FieldStat.DataCollection;

namespace FieldStat
{
    public class ParameterData
    {
        private Results m_results = new Results();
        private Hashtable htGlobalBin = new Hashtable();
        private Hashtable htAppBin = new Hashtable();
        private ArrayList m_Files;

        public Results Results
        {
            get { return m_results; }
        }

        public void LoadCoverage(string directory)
        {
            DirectoryInfo dir = new DirectoryInfo(directory);
            foreach (FileInfo file in dir.GetFiles())
            {
                if (file.Extension.ToLower() == ".xml")
                {
                    Import(file.FullName, m_results.Coverage);
                }
            }
        }

        public void LoadCoverage(string[] files)
        {
            foreach (string fileName in files)
            {
                Import(fileName, m_results.Coverage);
            }
        }

        public void LoadAssemblies(string[] files)
        {
            foreach (string fileName in files)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(fileName);
                m_results.Assemblies.Rows.Add(name, "0", null);
            }
            m_Files = new ArrayList(files);
        }

        public void LoadFilters(string[] filters)
        {
            m_results.Filters.AddRange(filters);
        }

        public void LoadApplicationRepository(string baseDirectory)
        {
            DirectoryInfo app_repos = new DirectoryInfo(baseDirectory);
            foreach (DirectoryInfo app in app_repos.GetDirectories())
            {
                ScanBins(app.Name, app);
            }
        }

        public void ComputeResults()
        {
            m_results.ComputeResults(m_Files, htAppBin, m_results.Filters);
        }

        public void ImportCachedResults(string file)
        {
            m_results.AnalysisResults.Reset();
            m_results.AnalysisResults.ReadXml(file);
            // Databinding isn't notified of readxml.  This hack notifies by resorting.
            m_results.AnalysisResults.DefaultView.Sort = "AppFrequency DESC";
        }

        private void Import(string file, DataTable m_dtCoverage)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(file);
            }
            catch
            {
                return;
            }

            foreach (XmlElement klass in xml.GetElementsByTagName("class"))
            {
                foreach (XmlElement method in klass.GetElementsByTagName("Method"))
                {
                    string type = method.Attributes["type"].Value;
                    string meth = method.Attributes["method"].Value;
                    float cov = float.Parse(method.Attributes["coverage"].Value);
                    int length = int.Parse(method.Attributes["length"].Value);
                    m_dtCoverage.Rows.Add(type, meth, cov, length);
                }
            }
        }

        private void ScanBins(string parent, DirectoryInfo app)
        {
            foreach (FileInfo fi in app.GetFiles())
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
    }
}
