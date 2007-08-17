//
// Copyright (c) Eli Yukelzon - reflog@gmail.com
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Collections;
using Gendarme.Framework;

using Mono.Cecil;

namespace MonoDevelop
{
       
    public class GendarmeRunner : Runner
    {
        
        public GendarmeRunner()
        {
        }
        public event ProgressChangedHandler ProgressChanged;                                                                                                     
        public delegate void ProgressChangedHandler(int progress);                                                                                      
        public void OnProgressChanged(int progress){                                                                                                    
            if(ProgressChanged != null)                                                                                                                        
                ProgressChanged(progress);                                                                                                                       
        }        
        int _progress = 0;
        int current_progress {
            get { return _progress; }
            set { 
                _progress = value;
                OnProgressChanged(value);
                }
        }
        public void ProcessWithProgress (AssemblyDefinition assembly){
           _CheckAssembly (assembly);                                                                                                            
           current_progress = 0;  
           int module_step = 0;
           if (assembly.Modules.Count>0)
             module_step = (100 / assembly.Modules.Count);
           foreach (ModuleDefinition module in assembly.Modules) {                                                                              
                 _CheckModule (module);                                                                                                        
                 int type_step = 0;
                 if(module.Types.Count>0) 
                    type_step = (module_step / module.Types.Count);
                 else   
                    current_progress += module_step;
                 foreach (TypeDefinition type in module.Types){
                         current_progress += type_step; 
                         _CheckType (type);             
                 }
           }            
           current_progress = 100;
        }
        
          void _CheckAssembly (AssemblyDefinition assembly)                                                                                             
                {                                                                                                                                            
                        foreach (IAssemblyRule rule in Rules.Assembly)                                                                                       
                                _ProcessMessages (rule.CheckAssembly (assembly, this), rule, assembly);                                                       
                }                                                                                                                                            
                                                                                                                                                             
                void _CheckModule (ModuleDefinition module)                                                                                                   
                {                                                                                                                                            
                        foreach (IModuleRule rule in Rules.Module)                                                                                           
                                _ProcessMessages (rule.CheckModule (module, this), rule, module);                                                             
                }                                                                                                                                            
       void _ProcessMessages (MessageCollection messages, IRule rule, object target)                                                                 
                {                                                                                                                                            
                        if (messages == RuleSuccess)                                                                                                         
                                return;                                                                                                                      
                                                                                                                                                             
                        Violations.Add (rule, target, messages);                                                                                             
                }              
                                                                                                                                          
                                                                                                                                                             
                void _CheckType (TypeDefinition type)                                                                                                         
                {                                                                                                                                            
                        foreach (ITypeRule rule in Rules.Type)                                                                                               
                                _ProcessMessages (rule.CheckType (type, this), rule, type);                                                                   
                                                                                                                                                             
                        _CheckMethods (type);                                                                                                                 
                }                                                                                                                                            
                                                                                                                                                             
                void _CheckMethods (TypeDefinition type)                                                                                                      
                {                                                                                                                                            
                        _CheckMethods (type, type.Constructors);                                                                                              
                        _CheckMethods (type, type.Methods);                                                                                                   
                }                                                                                                                                            
                                                                                                                                                             
                void _CheckMethods (TypeDefinition type, ICollection methods)                                                                                 
                {                                                                                                                                            
                        foreach (MethodDefinition method in methods)                                                                                         
                                foreach (IMethodRule rule in Rules.Method)                                                                                   
                                        _ProcessMessages (rule.CheckMethod (method, this), rule, method);                                                     
                }                                   
    }
}
