using System;
using System.Collections;
using System.Data;
using System.Text;

using FieldStat.CodeModel;

namespace FieldStat.DataCollection
{
    public class Results
    {
        DataTable m_dtAssemblies;
        ArrayList m_Filters = new ArrayList();
        DataTable m_dtResults;
        DataTable m_dtCoverage;
        DataTable m_dtAppStats;
        DataTable m_dtCodeRank;

        public Results()
        {
            MakeTables();
        }

        #region Properties
        public DataTable Assemblies
        {
            get { return m_dtAssemblies;  }
        }
        public ArrayList Filters
        {
            get { return m_Filters; }
        }
        public DataTable AnalysisResults
        {
            get { return m_dtResults; }
        }
        public DataTable Coverage
        {
            get { return m_dtCoverage; }
        }
        public DataTable ApplicationStats
        {
            get { return m_dtAppStats; }
        }
        public DataTable CodeRank
        {
            get { return m_dtCodeRank; }
        }

        ArrayList m_PluginTables = new ArrayList();
        public ArrayList PluginTables
        {
            get { return m_PluginTables;  }
        }

        #endregion

        private void MakeTables()
        {
            m_dtAssemblies = new DataTable("Assemblies");
            m_dtAssemblies.Columns.Add("AssemblyName", typeof(string));
            m_dtAssemblies.Columns.Add("Stats", typeof(string));
            m_dtAssemblies.Columns.Add("Assembly", typeof(object));

            m_dtCoverage = new DataTable("Coverage");
            m_dtCoverage.Columns.Add("Type", typeof(string));
            m_dtCoverage.Columns.Add("Method", typeof(string));
            m_dtCoverage.Columns.Add("Coverage", typeof(float));
            m_dtCoverage.Columns.Add("Length", typeof(int));

            m_dtAppStats = new DataTable("ApplicationStats");
            m_dtAppStats.Columns.Add("App", typeof(string));
            m_dtAppStats.Columns.Add("SystemCount", typeof(int));
            m_dtAppStats.PrimaryKey = new DataColumn[1] { m_dtAppStats.Columns["App"] };

            m_dtCodeRank = new DataTable("ApplicationCodeRank");
            m_dtCodeRank.Columns.Add("Application", typeof(string));
            m_dtCodeRank.Columns.Add("Type", typeof(string));
            m_dtCodeRank.Columns.Add("Method", typeof(string));
            m_dtCodeRank.Columns.Add("Rank", typeof(double));
            m_dtCodeRank.PrimaryKey = new DataColumn[3] { m_dtCodeRank.Columns["Application"], m_dtCodeRank.Columns["Type"], m_dtCodeRank.Columns["Method"] };

            m_dtResults = new DataTable("Results");
            m_dtResults.Columns.Add("Type", typeof(string));
            m_dtResults.Columns.Add("Method", typeof(string));
            m_dtResults.Columns.Add("Length", typeof(int));
            m_dtResults.Columns.Add("Frequency", typeof(int));
            m_dtResults.Columns.Add("RankedFrequency", typeof(double));
            m_dtResults.Columns.Add("AppFrequency", typeof(int));
            m_dtResults.Columns.Add("Coverage", typeof(float));
            m_dtResults.PrimaryKey = new DataColumn[2] { m_dtResults.Columns["Type"], m_dtResults.Columns["Method"] };
        }

        public void ComputeResults(ICollection m_Files, Hashtable htAppBin, ICollection filters )
        {
            // First Pass
            Visit scan = new Visit();

            scan.Collectors.Register("App", new AppCollector());
            scan.Collectors.Register("CallGraph", new CallGraphCollector());

            scan.DoScan (m_Files, htAppBin, filters);

            AppCollector appresult = (AppCollector)scan.Collectors["App"];
            CallGraphCollector callgraph = (CallGraphCollector)scan.Collectors["CallGraph"];

            // Code Rank
            foreach (NodeGraph nodeGraph in callgraph.Values)
            {
                CodeRank rank = new CodeRank();
                rank.CalculateRank(nodeGraph);

                foreach (Node node in nodeGraph.Nodes)
                {
                    try
                    {
                        m_dtCodeRank.Rows.Add(nodeGraph.Application, node.TypeName, node.MethodName, node.Rank);
                    }
                    catch (ConstraintException constraint)
                    {
                        Console.WriteLine(constraint.Message);
                    }
                }
            }

            // App Usage
            foreach (AppResult app in appresult.Values)
            {
                m_dtAppStats.Rows.Add(app.AppName, app.SystemCalls.Count);
            }

            // Second Pass
            Visit scantype = new Visit();
            scantype.Collectors.Register("Type", new TypeCollector( m_dtCodeRank ));
            scantype.DoScan( m_Files, htAppBin, filters);
            TypeCollector typeresult = (TypeCollector)scantype.Collectors["Type"];

            // Type Usage + Coverage
            foreach (TypeResult tr in typeresult.Values)
            {
                foreach (MethodResult mr in tr.Methods.Values)
                {
                    // systemType, systemMethod, methodLength, TotalUse, RankedUse, TotalAppsThatUse, Coverage
                    m_dtResults.Rows.Add(tr.Name, mr.Name, -1, mr.MethodFrequency, mr.MethodRankedFrequency, mr.TotalAppCount, -1);
                }
            }
            // Join Coverage with the Usage data.
            JoinCoverage();

            // Run Plugin Collectors.
            Plugins.ComputeResults(this, m_Files, htAppBin);
        }

        private void JoinCoverage()
        {
            foreach (DataRow row in m_dtCoverage.Select("", "", DataViewRowState.CurrentRows))
            {
                DataRow resultDr = m_dtResults.Rows.Find(new object[] { row["Type"], row["Method"] });
                if (resultDr != null)
                {
                    resultDr["Coverage"] = row["Coverage"];
                    resultDr["Length"] = row["Length"];
                }
            }
        }
    }
}