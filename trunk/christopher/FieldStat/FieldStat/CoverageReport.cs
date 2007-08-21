using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FieldStat
{
    public partial class CoverageReport : Form
    {
        DataTable m_dtReport;
        public CoverageReport()
        {
            InitializeComponent();
        }

        public void CalcReport(DataTable report)
        {
            m_dtReport = report;

            txtReport.Text = "";

            foreach( string str in new string[]{
                CalcCoveragePercentage(),
                Top10FrequencyUncovered(),
                Top10Types()})
            {
                txtReport.Text += str + "\r\n";
            }
        }

        private string CalcCoveragePercentage()
        {
            int covered=0;
            int total=0;
            foreach (DataRow dr in m_dtReport.Select("", "", DataViewRowState.CurrentRows))
            {
                if ((float)dr["Coverage"] > 0.0)
                {
                    covered++;
                }
                total++;
            }
            float p = (float)covered / total;
            return "Percentage Used Methods Covered: " + p;
        }

        private bool isinterface ( string name )
        {
            int pos = name.LastIndexOf(".");
            return name.Substring(pos+1).StartsWith("I");
        }
        private bool iscontrol(string name)
        {
            return name.StartsWith("System.Windows.Forms");
        }

        private string Top10FrequencyUncovered()
        {
            string[] top10 = new string[10];
            int count = 0;
            foreach (DataRow dr in m_dtReport.Select("", "Frequency DESC", DataViewRowState.CurrentRows))
            {
                // skip interfaces or windows control hack
                if( isinterface( dr["Type"].ToString() ) ||
                    iscontrol( dr["Type"].ToString() ) )
                    continue;
                if ((float)dr["Coverage"] <= 0.0)
                {
                    top10[count] = dr["Type"]+"."+dr["Method"] + "["+ dr["Frequency"] + "]";
                    count++;

                    if (top10.Length == count)
                        break;
                }
            }
            return "Top10 Frequent Uncovered Methods: " + string.Join(",", top10);
        }

        private string Top10Types()
        {
            System.Collections.Hashtable ht = new System.Collections.Hashtable();

            foreach (DataRow dr in m_dtReport.Select("", "Frequency DESC", DataViewRowState.CurrentRows))
            {
                string type = (string)dr["Type"];
                if (!ht.Contains(dr["Type"].ToString()))
                    ht[type] = 0;
                ht[type] = (int)ht[type] + (int)dr["Frequency"];
            }

            System.Collections.ArrayList indices = range(ht.Values.Count);
            indices.Sort(  new Comparer( ht ) );
            System.Collections.ArrayList keys = new System.Collections.ArrayList(ht.Keys);
            System.Collections.ArrayList top10 = new System.Collections.ArrayList();
            for (int i = 0; i < 10; i++)
            {
                string key = (string)keys[(int)indices[i]];
                top10.Add(key + ": " + ht[key]);
            }
            string[] output = new string[top10.Count];
            for (int i = 0; i < top10.Count; i++)
            {
                output[i] = (string)top10[i];
            }
            return "Top10 Types Used " + string.Join(",", output);
        }
        System.Collections.ArrayList range(int count)
        {
            System.Collections.ArrayList list = new System.Collections.ArrayList();
            for (int i = 0; i < count; i++)
            {
                list.Add(i);
            }
            return list;
        }
        class Comparer : System.Collections.IComparer
        {
            System.Collections.Hashtable m_ht;
            System.Collections.ArrayList m_keys;
            public Comparer(System.Collections.Hashtable ht) 
            { 
                m_ht = ht;
                m_keys = new System.Collections.ArrayList(m_ht.Keys);
            }
            int  System.Collections.IComparer.Compare(object x, object y)
            {
                return -((int)m_ht[m_keys[(int)x]]).CompareTo(
                    (int)m_ht[m_keys[(int)y]]);
            }
        }

        private string Least10Types()
        {
            return "";
        }
    }
}