using System;
using System.Collections;
using System.Data;
using System.Text;

using Mono.Cecil;
using Mono.Cecil.Cil;

using FieldStat.CodeModel;

namespace FieldStat.DataCollection
{
    public class AbstractCollector
    {
        public virtual ICollection Values
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public virtual object Value
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public virtual void OnApplication(string application) { }
        public virtual void OnAssembly(AssemblyDefinition assemb) { }
        public virtual void OnType(TypeDefinition type) { }
        public virtual void OnMethodBody(MethodBody body) { }
        public virtual void OnSystemMethodCall(MethodDefinition callingContext, MethodReference methodCall, string callTypeName, string callMethodName) { }
        public virtual void OnMethodCall(MethodDefinition callingContext, MethodReference methodCall,
            string callTypeName, string callMethodName, bool isSystem, bool isExternal) { }
        public virtual void OnInstruction(Instruction i) { }

        protected void IncrementByOne(Hashtable table, string key)
        {
            if (!table.Contains(key))
            {
                table[key] = 0;
            }
            table[key] = (int)table[key] + 1;
        }

        protected T LookupOrCreateType<T>(Hashtable table, string name) where T : CodeEntity, new()
        {
            if (!table.Contains(name))
            {
                T t = new T();
                t.Name = name;
                table[name] = t;
                return t;
            }
            return (T)table[name];
        }

    }

    public class Collectors
    {
        Hashtable m_collectionTable = new Hashtable();

        public object this[string id]
        {
            get { return m_collectionTable[id]; }
        }

        public void Register(string id, AbstractCollector collector)
        {
            m_collectionTable.Add(id, collector);
        }

        public void UnRegister(string id)
        {
            m_collectionTable.Remove(id);
        }

        public void OnApplication(string application)
        {
            foreach (AbstractCollector col in m_collectionTable.Values)
            {
                col.OnApplication(application);
            }
        }

        public void OnMethodBody(MethodBody body)
        {
            foreach (AbstractCollector col in m_collectionTable.Values)
            {
                col.OnMethodBody(body);
            }
        }

        public void OnSystemMethodCall(MethodDefinition callingContext, MethodReference methodCall, string callTypeName, string callMethodName)
        {
            foreach (AbstractCollector col in m_collectionTable.Values)
            {
                col.OnSystemMethodCall(callingContext, methodCall, callTypeName, callMethodName);
            }
        }

        public void OnMethodCall(MethodDefinition callingContext, MethodReference methodCall,
            string typeName, string methodName, bool isSystem, bool isExternal)
        {
            foreach (AbstractCollector col in m_collectionTable.Values)
            {
                col.OnMethodCall(callingContext, methodCall, typeName, methodName, isSystem, isExternal);
            }
        }

        public void OnInstruction(Mono.Cecil.Cil.Instruction i)
        {
            foreach (AbstractCollector col in m_collectionTable.Values)
            {
                col.OnInstruction(i);
            }
        }
    }

    public class CallGraphCollector : AbstractCollector
    {
        private NodeGraph m_graph = null;
        Hashtable m_graphTable = new Hashtable();
        string m_app;

        public override ICollection Values
        {
            get { return this.m_graphTable.Values; }
        }

        public override object Value
        {
            get { return this.m_graphTable; }
        }

        public override void OnApplication(string application)
        {
            this.m_app = application;
            m_graph = new NodeGraph(application);
            m_graphTable[m_app] = m_graph;
        }

        // Add edges to virtual methods or after the fact?
        public override void OnMethodCall(MethodDefinition callingContext, MethodReference methodCall,
            string callTypeName, string callMethodName, bool isSystem, bool isExternal)
        {
            // Don't include system calls.
            if (isSystem)
                return;

            string contextTypeName = CodeProperties.GetClassName(callingContext.DeclaringType);
            string contextMethName = CodeProperties.GetMethodNameWithParameters(callingContext);
            m_graph.Add(contextTypeName, contextMethName, callTypeName, callMethodName);
        }
    }

    // Need collector to use rank when weighting frequency...

    public class TypeCollector : AbstractCollector
    {
        Hashtable m_htTypeFreq = new Hashtable();
        string m_app;
        DataTable m_dtCodeRank;

        public TypeCollector(DataTable dtCodeRank)
        {
            m_dtCodeRank = dtCodeRank;
        }

        private double Rank(string app, string type, string method)
        {
            DataRow dr = m_dtCodeRank.Rows.Find(new object[] { app, type, method });
            if (dr != null)
            {
                return (double)dr["Rank"];
            }
            return 0.0;
        }

        public override ICollection Values
        {
            get { return m_htTypeFreq.Values; }
        }

        public override void OnApplication(string application)
        {
            this.m_app = application;
        }
        public override void OnSystemMethodCall(MethodDefinition callingContext, MethodReference methodCall, string callTypeName, string callMethodName)
        {
            TypeResult tr = LookupOrCreateType<TypeResult>(m_htTypeFreq, callTypeName);
            MethodResult mr = LookupOrCreateType<MethodResult>(tr.Methods, callMethodName);
            
            // Usage of Types, Usage of Type in App
            tr.TypeFrequency++;
            IncrementByOne(tr.AppOccurrence, m_app);
            
            // Usage of Methods
            mr.MethodFrequency++;
            
            // Ranked Usage of Methods
            CountRankedUsage(callingContext, mr);
            
            // Usage of Methods in App
            IncrementByOne(mr.AppOccurrence, m_app);
        }

        private void CountRankedUsage(MethodDefinition callingContext, MethodResult mr)
        {
            if (m_dtCodeRank != null)
            {
                string contextTypeName = CodeProperties.GetClassName(callingContext.DeclaringType);
                string contextMethName = CodeProperties.GetMethodNameWithParameters(callingContext);

                mr.MethodRankedFrequency += Rank(m_app, contextTypeName, contextMethName);
            }
        }
    }

    public class AppCollector : AbstractCollector
    {
        ArrayList m_AppDataList = new ArrayList();
        AppResult m_appResult;

        public override ICollection Values
        {
            get { return m_AppDataList; }
        }

        public override void OnApplication(string application)
        {
            m_appResult = new AppResult(application);
            m_AppDataList.Add(m_appResult);
        }

        public override void OnSystemMethodCall(MethodDefinition callingContext, MethodReference methodCall, string callTypeName, string callMethodName)
        {
            IncrementByOne(m_appResult.SystemCalls, callMethodName);
        }
    }
}
