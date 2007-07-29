using System;
using System.Web.UI.WebControls;

namespace Umbraco.uicontrols
{
	internal class MenuIcon : Image, MenuIconI
	{
		private string _AltText = "init";
		private string _OnClickCommand = "";

		#region MenuIconI Members

		string MenuIconI.ID
		{
			get { return ID; }
			set { ID = value; }
		}

		string MenuIconI.AltText
		{
			get { return AlternateText; }
			set
			{
				_AltText = value;
				AlternateText = value;
				Attributes.Add("title", value);
			}
		}

		int MenuIconI.IconWidth
		{
			get { return (int)Width.Value; }
			set { Width = value; }
		}

		int MenuIconI.IconHeight
		{
			get { return (int)Height.Value; }
			set { Height = value; }
		}

		string MenuIconI.ImageURL
		{
			get { return this.ImageUrl; }
			set { ImageUrl = value; }
		}

		string MenuIconI.OnClickCommand
		{
			get { return _OnClickCommand; }
			set { _OnClickCommand = value; }
		}

		#endregion

		protected override void OnLoad(EventArgs EventArguments)
		{
			SetupClientScript();

			// NH 17-01-2007. Trying to avoid inline styling soup 
			//        Me.Width = WebControls.Unit.Pixel(22)
			//       Me.Height = WebControls.Unit.Pixel(23)
			//Me.Style.Add("border", "0px")
			Attributes.Add("class", "editorIcon");
			Attributes.Add("onMouseover", "this.className='editorIconOver'");
			string holder;
			if (ID != "")
			{
				holder = ID.Substring(0, ID.LastIndexOf("_")) + "_menu";
				Attributes.Add("onMouseout", "hoverIconOut('" + holder + "','" + ID + "');");
				Attributes.Add("onMouseup", "hoverIconOut('" + holder + "','" + ID + "');");
			}
			else
			{
				Attributes.Add("onMouseout", "this.className='editorIcon'");
				Attributes.Add("onMouseup", "this.className='editorIcon'");
			}
			Attributes.Add("onMouseDown", "this.className='editorIconDown'");
			AlternateText = _AltText;

			if (_OnClickCommand != "")
			{
				Attributes.Add("onClick", ((MenuIconI)this).OnClickCommand);
			}
		}

		private void SetupClientScript()
		{
			Page.ClientScript.RegisterClientScriptBlock(GetType(), "MENUICONCSS",
														"<link rel='stylesheet' href='/Umbraco_client/menuicon/style.css' />");
		}
	}
}