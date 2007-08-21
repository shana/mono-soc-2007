using System;
using System.Collections;
using System.Data;
using System.Text;

using Mono.Cecil;
using Mono.Cecil.Cil;

using FieldStat.DataCollection;
using FieldStat.CodeModel;

namespace FieldStat.Plugins
{
    public class DesignFragmentPlugin : AbstractPlugin
    {
        private DataTable m_fragments;
        public DesignFragmentPlugin() : base( false ){}
        public override void ComputeResults(Results results, ICollection files, Hashtable htBin)
        {
            Visit scan = new Visit();
            scan.Collectors.Register("DesignFragment", new DesignFragmentCollector());
            scan.DoScan( files, htBin);
            DesignFragmentCollector frags = (DesignFragmentCollector)scan.Collectors["DesignFragment"];

            MakeDataTable();

            foreach (ArrayList seq in frags.Values)
            {
                //LCS
            }
            {
            //    m_fragments.Rows.Add(frag.sequence, frag.count);
            }

            results.PluginTables.Add(m_fragments);
        }

        private void MakeDataTable()
        {
            m_fragments = new DataTable("DesignFragments");
            m_fragments.Columns.Add("Sequence", typeof(string));
            m_fragments.Columns.Add("Occurrences", typeof(int));
        }
    }

    public class DesignFragmentCollector : AbstractCollector
    {
        public override ICollection Values
        {
            get
            {
                return sequences;
            }
        }

        ArrayList sequences = new ArrayList();

        public override void OnMethodBody(MethodBody body)
        {
            ArrayList seq = GetSystemCallSequences(body);
            if (seq.Count > 3)
            {
                sequences.Add(seq);
            }
        }

        private ArrayList GetSystemCallSequences(MethodBody body)
        {
            ArrayList list = new ArrayList();
            foreach (Instruction i in body.Instructions)
            {
                if (CodeProperties.IsMethodCall(i))
                {
                    MethodReference rf = (MethodReference)i.Operand;
                    if (CodeProperties.IsSystem(rf))
                    {
                        list.Add(CodeProperties.GetFullName(rf));
                    }
                }
            }
            return list;
        }
    }
}
