using System;

namespace Umbraco.interfaces
{
	/// <summary>
	/// Summary description for IMacroGuiRendering.
	/// </summary>
	public interface IMacroGuiRendering
	{
		string Value {set; get;}
		bool ShowCaption {get;}
	}
}
