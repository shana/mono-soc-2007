using System;
using System.Collections;
using System.Text;

namespace FieldStat.CodeModel
{
    public class CodeEntity
    {
        public string Name;
        public CodeEntity( string name )
        {
            this.Name = name;
        }
        public CodeEntity() {}
    }

    public class TypeResult : CodeEntity
    {
        public TypeResult() {}
        public TypeResult(string name) : base(name) { }
        
        public Hashtable Methods = new Hashtable();
        public int TypeFrequency;
        public Hashtable AppOccurrence = new Hashtable();
        // histogram, in distinct classes or  (distribution).
    }

    public class MethodResult : CodeEntity
    {
        public MethodResult() {}
        public MethodResult(string name) : base(name) {}

        public int MethodFrequency;
        public double MethodRankedFrequency;
        public Hashtable AppOccurrence = new Hashtable();
        // histogram, in distinct classes or  (distribution).
        public int TotalAppCount
        {
            get
            {
                return AppOccurrence.Keys.Count;
            }
        }
    }

    public class AppResult
    {
        public string AppName;
        public Hashtable SystemCalls = new Hashtable();
        public AppResult(string app)
        {
            this.AppName = app;
        }
    }

    public class NodeGraph
    {
        public NodeGraph(string application)
        {
            this.Application = application;
        }
        public string Application;
        private Hashtable m_nodesTable = new Hashtable();
        public ICollection Nodes
        {
            get { return m_nodesTable.Values; }
        }
        public void Add(string callingContextType, string callingContextMethod, string callType, string methodCall)
        {
            Node context = GetNodeOrCreate(callingContextType, callingContextMethod);
            Node call = GetNodeOrCreate(callType, methodCall);
            context.AddChild(call);
        }
        private Node GetNodeOrCreate(string type, string method)
        {
            string full = type + "." + method;
            if (!m_nodesTable.Contains(full))
            {
                m_nodesTable[full] = new Node(type, method);
            }
            return (Node)m_nodesTable[full];
        }
    }
    public class Node
    {
        public Node(string type, string method)
        {
            this.FullName = type + "." + method;
            this.MethodName = method;
            this.TypeName = type;
        }
        public string FullName;
        public string MethodName;
        public string TypeName;
        public double Rank;
        public double TempRank;
        Hashtable m_children = new Hashtable();
        public ICollection Children
        {
            get { return m_children.Values; }
        }
        public void AddChild(Node node)
        {
            m_children[node.FullName] = node;
        }

        public bool IsLeaf
        {
            get { return this.Children.Count == 0; }
        }
    }
}
