using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Umbraco.uicontrols
{
	public class TabPage : WebControl
	{
		// Ensure that a TabPage cannot be instatiated outside 
		// this assembly -> New instances of a tabpage can only be retrieved through the tabview
		private bool _hasMenu = true;
		private ScrollingMenu _Menu = new ScrollingMenu();

		internal TabPage()
		{
		}

		public ScrollingMenu Menu
		{
			get { return _Menu; }
		}

		public bool HasMenu
		{
			get { return _hasMenu; }
			set { _hasMenu = value; }
		}

		protected override void OnLoad(EventArgs e)
		{
			Menu.Width = Unit.Pixel((int)Width.Value - 12);
			_Menu.ID = ID + "_menu";
			Controls.Add(_Menu);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			CreateChildControls();
			writer.WriteLine("<div id='" + ClientID + "' class='tabpage'>");
			if (HasMenu)
			{
				writer.WriteLine("<div class='menubar'>");
				Menu.Width = Width;
				Menu.RenderControl(writer);
				writer.WriteLine("</div>");
			}
			int ScrollingLayerHeight = (int)((WebControl)Parent).Height.Value - 22;
			int ScrollingLayerWidth = (int)((WebControl)Parent).Width.Value;
			if (HasMenu) ScrollingLayerHeight = ScrollingLayerHeight - 28;
			writer.WriteLine("<div class='tabpagescrollinglayer' id='" + ClientID + "_contentlayer' style='height:" +
							 ScrollingLayerHeight + "px;width:" + ScrollingLayerWidth + "px'>");

			string styleString = "";
			foreach (string key in Style.Keys)
			{
				styleString += key + ":" + Style[key] + ";";
			}

			writer.WriteLine("<div style='" + styleString + "'>");
			foreach (Control C in Controls)
			{
				if (!(C.ClientID == _Menu.ClientID))
				{
					C.RenderControl(writer);
				}
			}
			writer.WriteLine("</div>");
			writer.WriteLine("</div>");
			writer.WriteLine("</div>");
		}
	}
}