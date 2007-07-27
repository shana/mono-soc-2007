using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Mono.Cecil;

namespace FieldStat
{
    public class TypeResult
    {
        public TypeResult(string name)
        {
            this.Name = name;
        }
        public string Name;
        public Hashtable Methods = new Hashtable();
        public int TypeFrequency;
        public Hashtable AppOccurrence = new Hashtable();
        // histogram, in distinct classes or  (distribution).
    }
    public class MethodResult
    {
        public MethodResult(string name)
        {
            this.Name = name;
        }
        public string Name;
        public int MethodFrequency;
        public Hashtable AppOccurrence = new Hashtable();
        // histogram, in distinct classes or  (distribution).
        public int TotalAppCount
        {
            get
            {
                //int sum = 0;
                //foreach (string app in AppOccurrence.Keys)
                //{
                //    sum += (int)AppOccurrence[app];
                //}
                //return sum;
                return AppOccurrence.Keys.Count;
            }
        }
    }

    public class Scan
    {
        Hashtable m_htTypeFreq = new Hashtable();

        public Hashtable ScanAssemblies(ICollection files, ICollection filters )
        {
            foreach (string file in files)
            {
                ScanFile(file, file, filters);
            }
            return m_htTypeFreq;
        }

        public Hashtable ScanApps(Hashtable ht, ICollection filters)
        {
            foreach (string app in ht.Keys)
            {
                foreach (string file in (ArrayList)ht[app])
                {
                    ScanFile(app, file, filters);
                }
            }
            return m_htTypeFreq;
        }

        public void ScanFile( string app, string fileName, ICollection filters )
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
                    foreach (MethodDefinition methd in td.Methods)
                    {
                        ScanMethodBody(app, methd);
                    }
                }
            }
        }

        public void ScanMethodBody(string app, MethodDefinition methd)
        {
            if (methd.Body == null)
            {
                return;
            }
            foreach (Mono.Cecil.Cil.Instruction i in methd.Body.Instructions)
            {
                if (i.OpCode == Mono.Cecil.Cil.OpCodes.Call ||
                    i.OpCode == Mono.Cecil.Cil.OpCodes.Calli ||
                    i.OpCode == Mono.Cecil.Cil.OpCodes.Callvirt)
                {
                    if (i.Operand is Mono.Cecil.CallSite || i.Operand is Mono.Cecil.FieldReference )
                        continue;
                    MethodReference rf = (MethodReference)i.Operand;

                    if (rf.DeclaringType.Scope is Mono.Cecil.IAssemblyNameReference)
                    {
                        Mono.Cecil.IAssemblyNameReference assemRef = (Mono.Cecil.IAssemblyNameReference)rf.DeclaringType.Scope;
                        
                        if ( IsSystem( assemRef.PublicKeyToken ) )
                        {
                            string typeName = GetClassName(rf.DeclaringType);
                            //string methName = methd.Name;  //calling context
                            string methName = GetMethodNameWithParameters(((MethodReference)i.Operand));
                            
                            if (!m_htTypeFreq.Contains(typeName))
                            {
                                m_htTypeFreq[typeName] = new TypeResult(typeName);
                            }
                            TypeResult tr = (TypeResult)m_htTypeFreq[typeName];
                            tr.TypeFrequency++;
                            if (!tr.AppOccurrence.Contains(app))
                            {
                                tr.AppOccurrence[app] = 0;
                            }
                            tr.AppOccurrence[app] = (int)tr.AppOccurrence[app] + 1;
                            if (!tr.Methods.Contains(methName))
                            {
                                tr.Methods[methName] = new MethodResult(methName);
                            }
                            MethodResult mr = (MethodResult)tr.Methods[methName];
                            mr.MethodFrequency++;
                            if (!mr.AppOccurrence.Contains(app))
                            {
                                mr.AppOccurrence[app] = 0;
                            }
                            mr.AppOccurrence[app] = (int)mr.AppOccurrence[app] + 1;
                        }
                    }
                }
            }
        }
        class Special<T>
        {
            public void Meth ()
            {
                System.Collections.Generic.LinkedList<Special<Scan>> test = new System.Collections.Generic.LinkedList<Special<Scan>>();
                System.Collections.Generic.LinkedList<Scan> test2 = new System.Collections.Generic.LinkedList<Scan>();
                System.Collections.Generic.LinkedList<ImportCoverage> test3 = new System.Collections.Generic.LinkedList<ImportCoverage>();
                test.Clear();
                test2.Clear();
                test3.Clear();
            }
        }
        
        private string GetClassName(TypeReference type)
        {
            // Trim off Generic Parameters to classes.  e.g. System.Collections.Generic.List`1<MyFirstClass> =>
            // System.Collections.Generic.List
            if (type.FullName.Contains("`"))
            {
                return type.FullName.Remove( type.FullName.IndexOf("`"));
            }
            return type.FullName;
        }

        private string GetMethodNameWithParameters(MethodReference method)
        {
            if (method.Parameters.Count == 0)
                return method.Name + " ()";
            // Monocov output for overloaded methods look like:
            // MethodName (string,byte[],System.Data.DataTable)
            // MethodName (string,int)
            string[] paramList = new string[method.Parameters.Count];
            int i = 0;
            foreach (ParameterDefinition param in method.Parameters)
            {
                paramList[i] = GetParameterName(param);
                i++;
            }
            return method.Name + " (" + string.Join(",", paramList ) +")";
        }

        // format uses full name for non-value types
        // for types with alias, the alias is used: int32 -> int
        private string GetParameterName(ParameterDefinition param)
        {
            if (param.ParameterType.IsValueType || 
                param.ParameterType.FullName == "System.Object" ||
                param.ParameterType.FullName == "System.String")
            {
                string name = param.ParameterType.Name.ToLower();
                if (name.IndexOf("16") > -1 ||
                    name.IndexOf("32") > -1 ||
                    name.IndexOf("64") > -1 )
                {
                    //name = name.Substring(0, name.Length - 2);
                    name = name.Replace("16", "");
                    name = name.Replace("32", "");
                    name = name.Replace("64", "");
                }
                return name;
            }
            else
                return param.ParameterType.FullName;
        }

        private bool IsSystem( byte[] bytes )
        {
            if (bytes.Length == 0)
                return false;
            //b77a5c561934e089
            
            if( bytes[0] == 0xb7 &&
                bytes[1] == 0x7a &&
                bytes[2] == 0x5c &&
                bytes[3] == 0x56 &&
                bytes[4] == 0x19 &&
                bytes[5] == 0x34 &&
                bytes[6] == 0xe0 &&
                bytes[7] == 0x89 )
                return true;
            //b03f5f7f11d50a3a
            if (bytes[0] == 0xb0 &&
                bytes[1] == 0x3f &&
                bytes[2] == 0x5f &&
                bytes[3] == 0x7f &&
                bytes[4] == 0x11 &&
                bytes[5] == 0xd5 &&
                bytes[6] == 0x0a &&
                bytes[7] == 0x3a)
                return true;
            return false;
        }
    }
}
