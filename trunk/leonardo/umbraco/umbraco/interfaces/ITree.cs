using System;
using System.Xml;

namespace Umbraco.interfaces
{
	/// <summary>
	/// Summary description for ITree.
	/// </summary>
	public interface ITree
	{
		int id{set;}
		String app {set;}
		void Render(ref XmlDocument Tree);
		void RenderJS(ref System.Text.StringBuilder Javascript);
	}
}
