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

        delegate T2 MapFunc<T,T2>(T obj);

        static Ret[] Map<Input, Ret>(MapFunc<Input, Ret> fn, Input[] a)
        {
            Ret[] list = new Ret[a.Length];
            for (int i = 0; i < a.Length; ++i)
                list[i] = fn(a[i]);
            return list;
        }

        class HashableArray 
        {
            public int[] me;
            public HashableArray(int[] a)
            {
                me = a;
            }
            public override bool Equals(object obj)
            {
                HashableArray them = (HashableArray)obj;
                if (them.me.Length != me.Length)
                    return false;
                for (int i = 0; i < me.Length; i++)
                {
                    if (them.me[i] != me[i])
                        return false;
                }
                return true;
            }

            public override int GetHashCode()
            {
                int sum =0;
                for( int i = 0; i < me.Length; i++ )
                {
                    sum += me[i];
                }
                return sum + me.Length;
            }
        }

        public override void ComputeResults(Results results, ICollection files, Hashtable htBin)
        {
            Visit scan = new Visit();
            scan.Collectors.Register("DesignFragment", new DesignFragmentCollector());
            scan.DoScan( files, htBin);
            DesignFragmentCollector seqs = (DesignFragmentCollector)scan.Collectors["DesignFragment"];

            MakeDataTable();

            ArrayList inspectSeqs = (ArrayList)seqs.Values;
            Hashtable methodMap = (Hashtable)seqs.Value;

            Hashtable frags = new Hashtable();

            for (int i = 0; i < inspectSeqs.Count; i++)
            {
                for (int j = i+1; j < inspectSeqs.Count; j++)
                {
                    int[] a = (int[])inspectSeqs[i];
                    int[] b = (int[])inspectSeqs[j];
                    int[] sequence;
                    int matchLength = LongestCommonSubstring(a, b, out sequence);
                    if (matchLength > 3)
                    {
                        HashableArray s = new HashableArray(sequence);
                        if (!frags.Contains(s))
                        {
                            frags[s] = 0;
                        }
                        frags[s] = (int)frags[s] + 1;
                    }
                }
            }

            foreach( HashableArray sequence in frags.Keys )
            {
                string[] list = Map<int,string>( delegate( int x) { return (string)methodMap[x]; }, sequence.me);
                m_fragments.Rows.Add(string.Join(";", list), frags[sequence], sequence.me.Length);
            }

            results.PluginTables.Add(m_fragments);
        }

        private void MakeDataTable()
        {
            m_fragments = new DataTable("DesignFragments");
            m_fragments.Columns.Add("Sequence", typeof(string));
            m_fragments.Columns.Add("Occurrences", typeof(int));
            m_fragments.Columns.Add("Length", typeof(int));
        }

        public int LongestCommonSubstring(int[] str1, int[] str2, out int[] sequence)
        {
            sequence = null;
            if (str1.Length == 0  || str2.Length == 0)
                return 0;

            int[,] num = new int[str1.Length, str2.Length];
            int maxlen = 0;
            int lastSubsBegin = 0;
            ArrayList sequenceBuilder = new ArrayList();

            for (int i = 0; i < str1.Length; i++)
            {
                for (int j = 0; j < str2.Length; j++)
                {
                    if (str1[i] != str2[j])
                        num[i, j] = 0;
                    else
                    {
                        if ((i == 0) || (j == 0))
                            num[i, j] = 1;
                        else
                            num[i, j] = 1 + num[i - 1, j - 1];

                        if (num[i, j] > maxlen)
                        {
                            maxlen = num[i, j];
                            int thisSubsBegin = i - num[i, j] + 1;
                            if (lastSubsBegin == thisSubsBegin)
                            {//if the current LCS is the same as the last time this block ran
                                sequenceBuilder.Add(str1[i]);
                            }
                            else //this block resets the string builder if a different LCS is found
                            {
                                lastSubsBegin = thisSubsBegin;
                                sequenceBuilder.Clear();
                                int[] sub = SubArray(str1, lastSubsBegin, i + 1);
                                sequenceBuilder.AddRange( sub );
                            }
                        }
                    }
                }
            }
            sequence = new int[sequenceBuilder.Count];
            sequenceBuilder.CopyTo( sequence );
            return maxlen;
        }

        private int[] SubArray(int[] array, int start, int end)
        {
            int[] sub = new int[end - start + 1];
            for (int i = start; i <= end && i < array.Length; i++)
            {
                sub[i - start] = array[i];
            }
            return sub;
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

        public override object Value
        {
            get
            {
                return ReverseMethodMap;
            }
        }

        ArrayList sequences = new ArrayList();

        public override void OnMethodBody(MethodBody body)
        {
            ArrayList seq = GetSystemCallSequences(body);
            if (seq.Count > 3)
            {
                sequences.Add(EncodeSequenceCalls( seq ));
            }
        }

        Hashtable MethodMap = new Hashtable();
        Hashtable ReverseMethodMap = new Hashtable();
        int m_counter = 0;
        private int[] EncodeSequenceCalls( ArrayList calls )
        {
            int[] codedCalls = new int[calls.Count];
            int i = 0;
            foreach( string call in calls )
            {
                if (!MethodMap.Contains(call))
                {
                    MethodMap[call] = m_counter;
                    ReverseMethodMap[m_counter] = call;
                    m_counter++;
                }
                codedCalls[ i ] = (int)MethodMap[call];
                i++;
            }
            return codedCalls;
        }

        private ArrayList GetSystemCallSequences(MethodBody body)
        {
            ArrayList list = new ArrayList();
            string last = "";
            foreach (Instruction i in body.Instructions)
            {
                if (CodeProperties.IsMethodCall(i))
                {
                    MethodReference rf = (MethodReference)i.Operand;
                    if (CodeProperties.IsSystem(rf)  )
                    {
                        string full = CodeProperties.GetFullName(rf);

                        if( /*last != full &&*/!Stop(full))
                            list.Add(full);
                        //last = full;                    
                    }
                }
            }
            return list;
        }

        bool Stop(string name)
        {
            if (name.StartsWith("System.Collections"))
                return true;
            if (name.StartsWith("System.Windows.Forms.Control.set_"))
                return true;
            return false;
        }
    }
}
