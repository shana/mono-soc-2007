using System;

namespace Umbraco.BusinessLogic.Console
{
	/// <summary>
	/// Summary description for IIcon.
	/// </summary>
	public interface IIcon
	{
		Guid UniqueId{get;}
		int Id{get;}
		IMenuItem[] MenuItems {get;}
		IIcon[] Children {get;}
		string DefaultEditorURL{get;}
		string Text{get;set;}
		string OpenImage {get;}
		string Image {get;}
	}
}