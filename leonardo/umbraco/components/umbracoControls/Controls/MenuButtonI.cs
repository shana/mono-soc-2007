using System;
using System.Web.UI.WebControls;

namespace Umbraco.uicontrols
{
	public class MenuImageButton : ImageButton, MenuIconI
	{
		private string _OnClickCommand = "";

		#region MenuIconI Members

		string MenuIconI.ID
		{
			get { return base.ID; }
			set { base.ID = value; }
		}

		string MenuIconI.AltText
		{
			get { return AlternateText; }
			set
			{
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
			get { return base.ImageUrl; }
			set { base.ImageUrl = value; }
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

			Width = Unit.Pixel(22);
			Height = Unit.Pixel(23);
			Style.Add("border", "0px");
			Attributes.Add("class", "editorIcon");
			Attributes.Add("onMouseover", "this.className='editorIconOver'");
			Attributes.Add("onMouseout", "this.className='editorIcon'");
			Attributes.Add("onMouseup", "this.className='editorIconOver'");
			Attributes.Add("onMouseDown", "this.className='editorIconDown'");

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