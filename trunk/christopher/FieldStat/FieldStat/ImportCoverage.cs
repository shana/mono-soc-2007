using System;
using System.Collections;
using System.Xml;
using System.Text;
using System.Data;

namespace FieldStat
{
    public class ImportCoverage
    {
        public void Import(string file, DataTable m_dtCoverage)
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
    }
}
