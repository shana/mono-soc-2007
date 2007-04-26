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
