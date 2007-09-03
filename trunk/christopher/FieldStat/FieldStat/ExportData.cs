using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;


using FieldStat.DataCollection;

namespace FieldStat
{
    
    public class ExportData
    {
        Results m_results;
        string m_outputDirectory;

        public ExportData(Results results, string outputDirectory )
        {
            m_results = results;
            m_outputDirectory = outputDirectory;
        }

        public void ExportResults()
        {
            //m_results.AnalysisResults.TableName = "Results";
            if (!Directory.Exists(m_outputDirectory))
            {
                Directory.CreateDirectory(m_outputDirectory);
            }
            m_results.AnalysisResults.WriteXml(m_outputDirectory + "\\Results.xml", XmlWriteMode.WriteSchema);

            foreach (DataTable dtPlugin in m_results.PluginTables)
            {
                dtPlugin.WriteXml(m_outputDirectory + "\\"+ dtPlugin.TableName+".xml", XmlWriteMode.WriteSchema);
            }
        }
    }
}