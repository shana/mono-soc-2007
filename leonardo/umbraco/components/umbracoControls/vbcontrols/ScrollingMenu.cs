using System.ComponentModel;
using System.Web.UI;

[ToolboxData("<{0}:ScrollingMenu runat=server></{0}:ScrollingMenu>")]
public class ScrollingMenu : System.Web.UI.WebControls.WebControl
{
	private ArrayList Icons = new ArrayList();
	private string iconIds;
	private int extraMenuWidth = 0;
	private string _ClientFilesPath = "/umbraco_client/scrollingmenu/";

	public MenuIconI NewIcon(int Index)
	{
		MenuIcon Icon = new MenuIcon();
		Icons.Insert(Index, icon);
		return Icon;
	}

	public MenuIconI NewIcon()
	{
		MenuIcon icon = new MenuIcon();
		Icons.Add(icon);
		return icon;
	}

	public MenuImageButton NewImageButton()
	{
		MenuImageButton icon = new MenuImageButton();
		Icons.Add(icon);
		return icon;
	}

	public MenuImageButton NewImageButton(int Index)
	{
		MenuImageButton icon = new MenuImageButton();
		Icons.Insert(Index, icon);
		return icon;
	}

	public WebControls.DropDownList NewDropDownList()
	{
		WebControls.DropDownList icon = new WebControls.DropDownList();
		Icons.Add(icon);
		return icon;
	}
	public void NewElement(string ElementName, string ElementId, string ElementClass, int ExtraWidth)
	{
		Icons.Add(new LiteralControl("<" + ElementName + " class=\"" + ElementClass + "\" id=\"" + ElementId + "\"></" + ElementName + ">"));
		extraMenuWidth = extraMenuWidth + ExtraWidth;
	}

	public void InsertSplitter()
	{
		Splitter icon = new Splitter();
		Icons.Add(icon);
	}
	public void InsertSplitter(int Index)
	{
		Splitter icon = new Splitter();
		Icons.Insert(Index, icon);
	}


	protected override void OnLoad(System.EventArgs EventArguments)
	{
		SetupMenu();
		SetupClientScript();
	}

	private void SetupClientScript()
	{
		this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SCROLLINGMENUCSS", "<link rel='stylesheet' href='/umbraco_client/scrollingmenu/style.css' />");
		this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SCROLLINGMENUJS", "<script language='javascript' src='/umbraco_client/scrollingmenu/javascript.js'></script>");
	}

	private WebControls.Image scrollImage()
	{
		WebControls.Image functionReturnValue = null;
		functionReturnValue = new WebControls.Image();
		functionReturnValue.Width = WebControls.Unit.Pixel(7);
		functionReturnValue.Height = WebControls.Unit.Pixel(20);
		functionReturnValue.BorderWidth = WebControls.Unit.Pixel(0);
		functionReturnValue.Attributes.Add("align", "absMiddle");
		functionReturnValue.CssClass = "editorArrow";
		functionReturnValue.Attributes.Add("onMouseOut", "this.className = 'editorArrow'; scrollStop();");
		return functionReturnValue;
	}

	private void SetupMenu()
	{
		// Calculate innerlayer max width 32 pixel per icon
		int ScrollingLayerWidth = Icons.Count * 26 + extraMenuWidth;

		WebControls.Table Container = new WebControls.Table();
		WebControls.TableRow tr = new WebControls.TableRow();
		Container.Rows.Add(tr);

		// // scroll-left image
		WebControls.TableCell td = new WebControls.TableCell();
		WebControls.Image scrollL = scrollImage();
		scrollL.ImageUrl = _ClientFilesPath + "images/arrawBack.gif";
		scrollL.Attributes.Add("onMouseOver", "this.className = 'editorArrowOver'; scrollR('" + this.ClientID + "_sl','" + this.ClientID + "_slh'," + ScrollingLayerWidth + ");");
		td.Controls.Add(scrollL);
		tr.Cells.Add(td);

		// // Menulayers
		td = new WebControls.TableCell();

		HtmlControls.HtmlGenericControl outerLayer = new HtmlControls.HtmlGenericControl();
		outerLayer.TagName = "div";
		outerLayer.ID = this.ID + "_slh";
		outerLayer.Attributes.Add("class", "slh");
		outerLayer.Style.Add("height", "26px");
		string tmp = this.Width.ToString;

		outerLayer.Style.Add("width", (this.Width.Value - 18).ToString + "px");
		td.Controls.Add(outerLayer);

		HtmlControls.HtmlGenericControl menuLayer = new HtmlControls.HtmlGenericControl();
		menuLayer.TagName = "div";
		menuLayer.ID = this.ID + "_sl";
		menuLayer.Style.Add("top", "0px");
		menuLayer.Style.Add("left", "0px");
		menuLayer.Attributes.Add("class", "sl");
		menuLayer.Style.Add("height", "26px");
		menuLayer.Style.Add("width", ScrollingLayerWidth + "px");

		HtmlControls.HtmlGenericControl nobr = new HtmlControls.HtmlGenericControl();
		nobr.TagName = "nobr";
		menuLayer.Controls.Add(nobr);

		// // add all icons to the menu layer
		foreach (Control item in Icons) {
			menuLayer.Controls.Add(item);

			if (item.ID != "")
			{
				iconIds = iconIds + item.ID + ",";
			}
		}

		outerLayer.Controls.Add(new LiteralControl("<script>" + Constants.vbCrLf + "RegisterScrollingMenuButtons('" + this.ClientID + "', '" + iconIds + "');" + Constants.vbCrLf + "</script>"));

		outerLayer.Controls.Add(menuLayer);

		tr.Cells.Add(td);

		// // scroll-right image
		td = new WebControls.TableCell();
		WebControls.Image scrollR = scrollImage();
		scrollR.ImageUrl = _ClientFilesPath + "images/arrowForward.gif";
		scrollR.Attributes.Add("onMouseOver", "this.className = 'editorArrowOver'; scrollL('" + this.ClientID + "_sl','" + this.ClientID + "_slh'," + ScrollingLayerWidth + ");");
		td.Controls.Add(scrollR);
		tr.Cells.Add(td);

		this.Controls.Add(Container);
	}

}