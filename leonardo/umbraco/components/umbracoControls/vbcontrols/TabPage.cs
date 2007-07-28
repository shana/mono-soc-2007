public class TabPage : System.Web.UI.WebControls.WebControl
{
	// Ensure that a TabPage cannot be instatiated outside 
	// this assembly -> New instances of a tabpage can only be retrieved through the tabview
	private bool _hasMenu = true;
	private ScrollingMenu _Menu = new ScrollingMenu();

	internal TabPage()
	{

	}

	protected override void OnLoad(EventArgs e)
	{
		Menu.Width = System.Web.UI.WebControls.Unit.Pixel(this.Width.Value - 12);
		_Menu.ID = this.ID + "_menu";
		this.Controls.Add(_Menu);
	}
	public ScrollingMenu Menu {
		get { return _Menu; }
	}

	public bool HasMenu {
		get { return _hasMenu; }
		set { _hasMenu = value; }
	}

	protected override void Render(System.web.UI.HtmlTextWriter writer)
	{
		CreateChildControls();
		writer.WriteLine("<div id='" + this.ClientID + "' class='tabpage'>");
		if (HasMenu)
		{
			writer.WriteLine("<div class='menubar'>");
			Menu.Width = this.Width;
			Menu.RenderControl(writer);
			writer.WriteLine("</div>");
		}
		int ScrollingLayerHeight = ((System.Web.UI.WebControls.WebControl)this.Parent).Height.Value - 22;
		int ScrollingLayerWidth = ((System.Web.UI.WebControls.WebControl)this.Parent).Width.Value;
		if (HasMenu) ScrollingLayerHeight = ScrollingLayerHeight - 28; 
		writer.WriteLine("<div class='tabpagescrollinglayer' id='" + this.ClientID + "_contentlayer' style='height:" + ScrollingLayerHeight + "px;width:" + ScrollingLayerWidth + "px'>");

		string styleString = "";
		foreach (string key in this.Style.Keys) {
			styleString += key + ":" + this.Style.Item(key) + ";";
		}

		writer.WriteLine("<div style='" + styleString + "'>");
		foreach (System.Web.UI.Control C in this.Controls) {
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