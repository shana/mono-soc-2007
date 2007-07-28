using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

[Designer(typeof(UmbracoPanelDesigner)), PersistChildren(true), ParseChildren(false), ToolboxData("<{0}:UmbracoPanel runat=server></{0}:UmbracoPanel>")]
public class UmbracoPanel : System.Web.UI.WebControls.Panel
{
	private ScrollingMenu _menu = new ScrollingMenu();

	public UmbracoPanel()
	{

	}

	protected override void OnInit(EventArgs e)
	{
		setupMenu();
		this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "PanelStyles", "<link rel='stylesheet' href='/umbraco_client/panel/style.css' />");
		this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "PanelScript", "<script language='javascript' src='/umbraco_client/panel/javascript.js'></script>");

	}

	protected override void OnLoad(System.EventArgs EventArguments)
	{
	}

	private bool _hasMenu = false;
	private string _StatusBarText = "";
	private string _text;


	public bool hasMenu {
		get { return _hasMenu; }
		set { _hasMenu = value; }
	}

	public string Text {
		get {
			if (_text == "")
			{
				_text = "[UNDEFINED VALUE TEXT]";
			}
			return _text;

		}
		set { _text = value; }
	}

	public string StatusBarText {
		get { return _StatusBarText; }
		set { _StatusBarText = value; }
	}

	public ScrollingMenu Menu {
		get { return _menu; }
	}

	internal void setupMenu()
	{
		_menu.ID = this.ID + "_menu";
		if (this.Width.Value < 20) this.Width = Unit.Pixel(24); 
		_menu.Width = Unit.Pixel(this.Width.Value - 20);
		this.Controls.Add(_menu);
	}



	protected override void Render(System.web.UI.HtmlTextWriter writer)
	{
		base.CreateChildControls();
		try {
			if (System.Web.HttpContext.Current == null)
			{
				writer.WriteLine("Number of child controls : " + this.Controls.Count);
			}
			writer.WriteLine("<div id=\"" + this.ClientID + "\" class=\"panel\" style=\"height:" + this.Height.Value + "px;width:" + this.Width.Value + "px;\">");
			writer.WriteLine("<div class=\"boxhead\">");
			writer.WriteLine("<h2 id=\"" + this.ClientID + "Label\">" + this.Text + "</h2>");
			writer.WriteLine("</div>");
			writer.WriteLine("<div class=\"boxbody\">");
			if (this.hasMenu)
			{
				writer.WriteLine("<div id='" + this.ClientID + "_menubackground' class=\"menubar_panel\" style=\"margin-left:2px;width:" + (this.Width.Value - 7) + "px;display:block;\">");
				_menu.RenderControl(writer);
				writer.WriteLine("</div>");
			}

			int upHeight = this.Height.Value - 46;
			int upWidth = this.Width.Value - 5;

			if (this.hasMenu) upHeight = upHeight - 34; 
			writer.WriteLine("<div id=\"" + this.ClientID + "_content\" class=\"content\" style=\"height:" + (upHeight) + "px;width:" + (upWidth) + "px;\">");

			string styleString = "";

			foreach (string key in this.Style.Keys) {
				styleString += key + ":" + this.Style.Item(key) + ";";
			}


			writer.WriteLine("<div style='" + styleString + "'>");
			foreach (Control c in this.Controls) {
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
		catch (Exception ex) {
			this.Page.Trace.Warn("Error rendering umbracopanel control" + ex.ToString);
		}
	}
}

class UmbracoPanelDesigner : System.Web.UI.Design.ControlDesigner
{

	public UmbracoPanelDesigner()
	{

	}
	public override string GetDesignTimeHTML()
	{
		try {
			int CTRLHeight = 10;
			int CTRLWidth = 10;

			try {
				Panel PanelCtrl = (Panel)this.Component;
				CTRLHeight = (int)PanelCtrl.Height.Value;
				CTRLWidth = (int)PanelCtrl.Width.Value;
			}
			catch {
			}
			return ("<link rel='stylesheet' href='/umbraco_client/panel/style.css' /><div style='height:" + CTRLHeight + ";width:" + CTRLWidth + ";border:1px #ccc;'>" + base.GetDesignTimeHtml + "</div>");
		}
		catch (Exception ex) {
			return ("<div style='height:200px;width:200px;'> Error loading control!! </div>");
		}
	}

}