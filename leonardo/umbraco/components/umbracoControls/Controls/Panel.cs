using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;

namespace Umbraco.uicontrols
{
	[Designer(typeof(UmbracoPanelDesigner)), PersistChildren(true), ParseChildren(false),
	 ToolboxData("<{0}:UmbracoPanel runat=server></{0}:UmbracoPanel>")]
	public class UmbracoPanel : Panel
	{
		private bool _hasMenu = false;
		private ScrollingMenu _menu = new ScrollingMenu();
		private string _StatusBarText = "";
		private string _text;

		public UmbracoPanel()
		{
		}


		public bool hasMenu
		{
			get { return _hasMenu; }
			set { _hasMenu = value; }
		}

		public string Text
		{
			get
			{
				if (_text == "")
				{
					_text = "[UNDEFINED VALUE TEXT]";
				}
				return _text;
			}
			set { _text = value; }
		}

		public string StatusBarText
		{
			get { return _StatusBarText; }
			set { _StatusBarText = value; }
		}

		public ScrollingMenu Menu
		{
			get { return _menu; }
		}

		protected override void OnInit(EventArgs e)
		{
			setupMenu();
			Page.ClientScript.RegisterClientScriptBlock(GetType(), "PanelStyles",
														"<link rel='stylesheet' href='/Umbraco_client/panel/style.css' />");
			Page.ClientScript.RegisterClientScriptBlock(GetType(), "PanelScript",
														"<script language='javascript' src='/Umbraco_client/panel/javascript.js'></script>");
		}

		protected override void OnLoad(EventArgs EventArguments)
		{
		}

		internal void setupMenu()
		{
			_menu.ID = ID + "_menu";
			if (Width.Value < 20) Width = Unit.Pixel(24);
			_menu.Width = Unit.Pixel((int)Width.Value - 20);
			Controls.Add(_menu);
		}


		protected override void Render(HtmlTextWriter writer)
		{
			base.CreateChildControls();
			try
			{
				if (HttpContext.Current == null)
				{
					writer.WriteLine("Number of child controls : " + Controls.Count);
				}
				writer.WriteLine("<div id=\"" + ClientID + "\" class=\"panel\" style=\"height:" + Height.Value + "px;width:" +
								 Width.Value + "px;\">");
				writer.WriteLine("<div class=\"boxhead\">");
				writer.WriteLine("<h2 id=\"" + ClientID + "Label\">" + Text + "</h2>");
				writer.WriteLine("</div>");
				writer.WriteLine("<div class=\"boxbody\">");
				if (hasMenu)
				{
					writer.WriteLine("<div id='" + ClientID + "_menubackground' class=\"menubar_panel\" style=\"margin-left:2px;width:" +
									 (Width.Value - 7) + "px;display:block;\">");
					_menu.RenderControl(writer);
					writer.WriteLine("</div>");
				}

				int upHeight = (int)Height.Value - 46;
				int upWidth = (int)Width.Value - 5;

				if (hasMenu) upHeight = upHeight - 34;
				writer.WriteLine("<div id=\"" + ClientID + "_content\" class=\"content\" style=\"height:" + (upHeight) + "px;width:" +
								 (upWidth) + "px;\">");

				string styleString = "";

				foreach (string key in Style.Keys)
				{
					styleString += key + ":" + Style[key] + ";";
				}


				writer.WriteLine("<div style='" + styleString + "'>");
				foreach (Control c in Controls)
				{
					if (!(c.ID == _menu.ID))
					{
						c.RenderControl(writer);
					}
				}
				writer.WriteLine("</div>");
				writer.WriteLine("</div>");
				writer.WriteLine("</div>");
				writer.WriteLine("<div class=\"boxfooter\"></div>");
				writer.WriteLine("</div>");
			}
			catch (Exception ex)
			{
				Page.Trace.Warn("Error rendering umbracopanel control" + ex.ToString());
			}
		}
	}

	internal class UmbracoPanelDesigner : ControlDesigner
	{
		public UmbracoPanelDesigner()
		{
		}

		public string GetDesignTimeHTML()
		{
			try
			{
				int CTRLHeight = 10;
				int CTRLWidth = 10;

				try
				{
					Panel PanelCtrl = (Panel)Component;
					CTRLHeight = (int)PanelCtrl.Height.Value;
					CTRLWidth = (int)PanelCtrl.Width.Value;
				}
				catch
				{
				}
				return
					("<link rel='stylesheet' href='/Umbraco_client/panel/style.css' /><div style='height:" + CTRLHeight + ";width:" +
					 CTRLWidth + ";border:1px #ccc;'>" + base.GetDesignTimeHtml() + "</div>");
			}
			catch (Exception ex)
			{
				return ("<div style='height:200px;width:200px;'> Error loading control!! </div>");
			}
		}
	}
}