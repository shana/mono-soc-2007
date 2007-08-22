using System;
using System.Collections.Generic;
using System.Text;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace FieldStat.CodeModel
{
    public static class CodeProperties
    {
        public static bool IsMethodCall(Mono.Cecil.Cil.Instruction i)
        {
            if (i.Operand is Mono.Cecil.CallSite || i.Operand is Mono.Cecil.FieldReference)
                return false;

            return i.OpCode == Mono.Cecil.Cil.OpCodes.Call ||
                   i.OpCode == Mono.Cecil.Cil.OpCodes.Calli ||
                   i.OpCode == Mono.Cecil.Cil.OpCodes.Callvirt ||
                // Constructor
                   i.OpCode == Mono.Cecil.Cil.OpCodes.Newobj;
                    
        }

        public static string GetFullName(MethodReference rf)
        {
            return GetClassName(rf.DeclaringType) + "." + GetMethodNameWithParameters(rf);
        }

        public static string GetClassName(TypeReference type)
        {
            // Trim off Generic Parameters to classes.  e.g. System.Collections.Generic.List`1<MyFirstClass> =>
            // System.Collections.Generic.List
            if (type.FullName.Contains("`"))
            {
                return type.FullName.Remove(type.FullName.IndexOf("`"));
            }
            return type.FullName;
        }

        public static string GetMethodNameWithParameters(MethodReference method)
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
            return method.Name + " (" + string.Join(",", paramList) + ")";
        }

        // format uses full name for non-value types
        // for types with alias, the alias is used: int32 -> int
        // TODO: Fix for ref.
        public static string GetParameterName(ParameterDefinition param)
        {
            if (param.ParameterType == null)
                return param.ToString();

            if (param.ParameterType.IsValueType ||
                param.ParameterType.FullName == "System.Object" ||
                param.ParameterType.FullName == "System.String")
            {
                string name = param.ParameterType.Name.ToLower();
                if (name.IndexOf("16") > -1 ||
                    name.IndexOf("32") > -1 ||
                    name.IndexOf("64") > -1)
                {
                    name = name.Replace("16", "");
                    name = name.Replace("32", "");
                    name = name.Replace("64", "");
                }
                return name;
            }
            else
                return param.ParameterType.FullName;
        }

        public static bool IsSystem(MethodReference rf)
        {
            if (IsExternalCall(rf))
            {
                Mono.Cecil.AssemblyNameReference assemRef = (Mono.Cecil.AssemblyNameReference)rf.DeclaringType.Scope;

                if (CodeProperties.IsSystemSigned(assemRef.PublicKeyToken))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsExternalCall(MethodReference rf)
        {
            return rf.DeclaringType.Scope is Mono.Cecil.AssemblyNameReference;
        }

        public static bool IsSystemSigned(byte[] bytes)
        {
            if (bytes.Length == 0)
                return false;
            
            //b77a5c561934e089
            if (bytes[0] == 0xb7 &&
                bytes[1] == 0x7a &&
                bytes[2] == 0x5c &&
                bytes[3] == 0x56 &&
                bytes[4] == 0x19 &&
                bytes[5] == 0x34 &&
                bytes[6] == 0xe0 &&
                bytes[7] == 0x89)
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
