using System;

namespace Umbraco.BusinessLogic.Console
{
	/// <summary>
	/// Rhis interface apparently indicates that a class is a menu item
	/// </summary>
	public interface IMenuItem
	{
		/// <summary>
		/// Gets the behavior.
		/// </summary>
		/// <value>The behavior.</value>
		EditorBehavior Behavior {get;}
		/// <summary>
		/// Gets the editor URL.
		/// </summary>
		/// <value>The editor URL.</value>
		string EditorURL {get;}
		/// <summary>
		/// Gets the text.
		/// </summary>
		/// <value>The text.</value>
		string Text {get;}
	}
	
	public enum EditorBehavior {
		Modal, 
		External, 
		Standard, 
		Command	
	}
}
