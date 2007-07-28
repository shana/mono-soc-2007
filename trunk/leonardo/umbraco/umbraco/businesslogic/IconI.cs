using System;

namespace umbraco.BusinessLogic.console
{
	/// <summary>
	/// Summary description for IconI.
	/// </summary>
	public interface IconI
	{
		Guid UniqueId{get;}
		int Id{get;}
		MenuItemI[] MenuItems {get;}
		IconI[] Children {get;}
		string DefaultEditorURL{get;}
		string Text{get;set;}
		string OpenImage {get;}
		string Image {get;}
	}
}