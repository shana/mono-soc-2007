using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Mono.Cecil;
using Mono.Cecil.Cil;

using Cecil.FlowAnalysis;
using Cecil.FlowAnalysis.ControlFlow;
using Cecil.FlowAnalysis.ActionFlow;
using Cecil.FlowAnalysis.CodeStructure;

using FieldStat.CodeModel;

namespace FieldStat.DataCollection
{

    public class Visit
    {
        Collectors m_collectors = new Collectors();

        public Collectors Collectors
        {
            get { return m_collectors;  }
        }

        public void DoScan(ICollection files, Hashtable htBin)
        {
            if (files != null)
            {
                this.ScanAssemblies(files, null);
            }
            else
            {
                this.ScanApps(htBin, null);
            }
        }

        public void ScanAssemblies(ICollection files, ICollection filters )
        {
            foreach (string file in files)
            {
                Collectors.OnApplication(file);
                ScanFile(file, file, filters);
            }
        }

        public void ScanApps(Hashtable ht, ICollection filters)
        {
            foreach (string app in ht.Keys)
            {
                Collectors.OnApplication(app);
                foreach (string file in (ArrayList)ht[app])
                {
                    ScanFile(app, file, filters);
                }            
            }
        }

        public void ScanFile(string app, string fileName, ICollection filters)
        {
            AssemblyDefinition targetAssembly = null;
            try
            {
                targetAssembly = AssemblyFactory.GetAssembly(fileName);
            }
            catch
            {
                return;
            }
            foreach (ModuleDefinition md in targetAssembly.Modules)
            {
                foreach (TypeDefinition td in md.Types)
                {
                    ScanTypes(app, td);
                }
            }
        }

        public void ScanTypes(string app, TypeDefinition td)
        {
            foreach (TypeDefinition inner in td.NestedTypes)
            {
                ScanTypes(app, inner);
            }
            foreach (MethodDefinition methd in td.Methods)
            {
                ScanMethodBody(app, methd);
            }
        }

        public void ScanMethodBody(string app, MethodDefinition methd)
        {
            if (methd.Body == null)
            {
                return;
            }

            Collectors.OnMethodBody(methd.Body);

            foreach (Mono.Cecil.Cil.Instruction i in methd.Body.Instructions)
            {
                if (CodeProperties.IsMethodCall(i))
                {
                    //if (i.Operand is Mono.Cecil.CallSite || i.Operand is Mono.Cecil.FieldReference)
                    //    continue;

                    MethodReference rf = (MethodReference)i.Operand;
                    string typeName = CodeProperties.GetClassName(rf.DeclaringType);
                    string methName = CodeProperties.GetMethodNameWithParameters(((MethodReference)i.Operand));

                    bool isSystem   = CodeProperties.IsSystem(rf);
                    bool isExternal = CodeProperties.IsExternalCall(rf);

                    Collectors.OnMethodCall(methd, rf, typeName, methName, isSystem, isExternal);
                    if (isSystem)
                    {
                        Collectors.OnSystemMethodCall(methd, rf, typeName, methName);
                    }
                }
            }
        }
    }
}
