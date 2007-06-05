//
// CProjectConfiguration.cs: Configuration for C/C++ projects
//
// Authors:
//   Marcos David Marin Amador <MarcosMarin@gmail.com>
//
// Copyright (C) 2007 Marcos David Marin Amador
//
//
// This source code is licenced under The MIT License:
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

using MonoDevelop.Projects;
using MonoDevelop.Projects.Serialization;

namespace CBinding
{
	public enum CompileTarget {
		Bin,
		StaticLibrary,
		SharedLibrary
	};
	
	public class CProjectConfiguration : AbstractProjectConfiguration
	{
		[ItemProperty("Output/output")]
		string output = string.Empty;
		
		[ItemProperty("Build/target")]
		CBinding.CompileTarget target = CBinding.CompileTarget.Bin;
		
		[ItemProperty ("Includes")]
		[ItemProperty ("Include", Scope = 1, ValueType = typeof(string))]
    	private ArrayList includes = new ArrayList ();
		
		[ItemProperty ("LibPaths")]
		[ItemProperty ("LibPath", Scope = 1, ValueType = typeof(string))]
    	private ArrayList libpaths = new ArrayList ();
		
		[ItemProperty ("Libs")]
		[ItemProperty ("Lib", Scope = 1, ValueType = typeof(string))]
    	private ArrayList libs = new ArrayList ();
		
		[ItemProperty("CodeGeneration",
		              FallbackType = typeof(UnknownCompilationParameters))]
		ICloneable compilationParameters;
		
		private string source_directory_path;
		
		public string Output {
			get { return output; }
			set { output = value; }
		}
		
		public CompileTarget CompileTarget {
			get { return target; }
			set { target = value; }
		}

		public ICloneable CompilationParameters {
			get { return compilationParameters; }
			set { compilationParameters = value; }
		}
		
		public string CompiledOutputName {
			get {
				string ext;
				string suf;
				
				if (Output.EndsWith (".so")) {
					ext = string.Empty;
					suf = string.Empty;
				}
				else if (Output.EndsWith (".a") && Output.StartsWith ("lib")) {
					ext = string.Empty;
					suf = string.Empty;
				}
				else {
					switch (CompileTarget)
					{
					case CBinding.CompileTarget.Bin:
						ext = string.Empty;
						suf = string.Empty;
						break;
					case CBinding.CompileTarget.SharedLibrary:
						ext = ".so";
						suf = string.Empty;
						break;
					case CBinding.CompileTarget.StaticLibrary:
						ext = ".a";
						suf = "lib";
						break;
					default:
						ext = string.Empty;
						suf = string.Empty;
						break;
					}
				}	
				
				return string.Format("{0}{1}{2}", suf, Output, ext);
			}
		}
		
		public string SourceDirectory {
			get { return source_directory_path; }
			set { source_directory_path = value; }
		}
		
		public ArrayList Includes {
			get { return includes; }
			set { includes = value; }
		}
		
		public ArrayList LibPaths {
			get { return libpaths; }
			set { libpaths = value; }
		}
		
		public ArrayList Libs {
			get { return libs; }
			set { libs = value; }
		}
		
		public override void CopyFrom (IConfiguration configuration)
		{
			base.CopyFrom(configuration);
			CProjectConfiguration conf = (CProjectConfiguration)configuration;
			
			output = conf.output;
			target = conf.target;
			source_directory_path = conf.source_directory_path;
			
			if (conf.CompilationParameters == null) {
				compilationParameters = null;
			} else {
				compilationParameters = (ICloneable)conf.Clone ();
			}
		}
	}
	
	public class UnknownCompilationParameters : ICloneable, IExtendedDataItem
	{
		Hashtable table = new Hashtable ();
		
		public IDictionary ExtendedProperties {
			get { return table; }
		}
		
		public object Clone ()
		{
			return MemberwiseClone ();
		}
	}
}
