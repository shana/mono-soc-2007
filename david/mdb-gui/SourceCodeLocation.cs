using System;

using Mono.Debugger;

namespace Mono.Debugger.Frontend
{
	[Serializable]
	public class SourceCodeLocation
	{
		string filename;
		int line;
		
		public string Filename {
			get { return filename; }
		}
		
		public int Line {
			get { return line; }
		}
		
		public SourceCodeLocation(string filename, int line)
		{
			this.filename = filename;
			this.line = line;
		}
		
		public SourceCodeLocation(SourceLocation location)
		{
			this.filename = location.FileName;
			this.line = location.Line;
		}
		
		public override string ToString()
		{
			return string.Format("[SourceCodeLocation {0}:{1}]", this.filename, this.line);
		}
	}
}
