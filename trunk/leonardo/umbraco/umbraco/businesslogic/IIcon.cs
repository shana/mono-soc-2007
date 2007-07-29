using System;

namespace Umbraco.BusinessLogic.Console
{
	/// <summary>
	/// Summary description for IIcon.
	/// </summary>
	public interface IIcon
	{
		/// <summary>
		/// Gets the unique id.
		/// </summary>
		/// <value>The unique id.</value>
		Guid UniqueId{get;}
		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>The id.</value>
		int Id{get;}
		/// <summary>
		/// Gets the menu items.
		/// </summary>
		/// <value>The menu items.</value>
		IMenuItem[] MenuItems {get;}
		/// <summary>
		/// Gets the children.
		/// </summary>
		/// <value>The children.</value>
		IIcon[] Children {get;}
		/// <summary>
		/// Gets the default editor URL.
		/// </summary>
		/// <value>The default editor URL.</value>
		string DefaultEditorURL{get;}
		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		string Text{get;set;}
		/// <summary>
		/// Gets the open image.
		/// </summary>
		/// <value>The open image.</value>
		string OpenImage {get;}
		/// <summary>
		/// Gets the image.
		/// </summary>
		/// <value>The image.</value>
		string Image {get;}
	}
}