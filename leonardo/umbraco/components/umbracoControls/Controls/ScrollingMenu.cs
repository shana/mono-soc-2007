using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Umbraco.uicontrols
{
	[ToolboxData("<{0}:ScrollingMenu runat=server></{0}:ScrollingMenu>")]
	public class ScrollingMenu : WebControl
	{
		private string _ClientFilesPath = "/Umbraco_client/scrollingmenu/";
		private int extraMenuWidth = 0;
		private string iconIds;
		private ArrayList Icons = new ArrayList();

		public MenuIconI NewIcon(int Index)
		{
			MenuIcon Icon = new MenuIcon();
			Icons.Insert(Index, Icon);
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

		public DropDownList NewDropDownList()
		{
			DropDownList icon = new DropDownList();
			Icons.Add(icon);
			return icon;
		}

		public void NewElement(string ElementName, string ElementId, string ElementClass, int ExtraWidth)
		{
			Icons.Add(
				new LiteralControl("<" + ElementName + " class=\"" + ElementClass + "\" id=\"" + ElementId + "\"></" + ElementName +
								   ">"));
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


		protected override void OnLoad(EventArgs EventArguments)
		{
			SetupMenu();
			SetupClientScript();
		}

		private void SetupClientScript()
		{
			Page.ClientScript.RegisterClientScriptBlock(GetType(), "SCROLLINGMENUCSS",
														"<link rel='stylesheet' href='/Umbraco_client/scrollingmenu/style.css' />");
			Page.ClientScript.RegisterClientScriptBlock(GetType(), "SCROLLINGMENUJS",
														"<script language='javascript' src='/Umbraco_client/scrollingmenu/javascript.js'></script>");
		}

		private Image scrollImage()
		{
			Image functionReturnValue = null;
			functionReturnValue = new Image();
			functionReturnValue.Width = Unit.Pixel(7);
			functionReturnValue.Height = Unit.Pixel(20);
			functionReturnValue.BorderWidth = Unit.Pixel(0);
			functionReturnValue.Attributes.Add("align", "absMiddle");
			functionReturnValue.CssClass = "editorArrow";
			functionReturnValue.Attributes.Add("onMouseOut", "this.className = 'editorArrow'; scrollStop();");
			return functionReturnValue;
		}

		private void SetupMenu()
		{
			// Calculate innerlayer max width 32 pixel per icon
			int ScrollingLayerWidth = Icons.Count * 26 + extraMenuWidth;

			Table Container = new Table();
			TableRow tr = new TableRow();
			Container.Rows.Add(tr);

			// // scroll-left image
			TableCell td = new TableCell();
			Image scrollL = scrollImage();
			scrollL.ImageUrl = _ClientFilesPath + "images/arrawBack.gif";
			scrollL.Attributes.Add("onMouseOver",
								   "this.className = 'editorArrowOver'; scrollR('" + ClientID + "_sl','" + ClientID + "_slh'," +
								   ScrollingLayerWidth + ");");
			td.Controls.Add(scrollL);
			tr.Cells.Add(td);

			// // Menulayers
			td = new TableCell();

			HtmlGenericControl outerLayer = new HtmlGenericControl();
			outerLayer.TagName = "div";
			outerLayer.ID = ID + "_slh";
			outerLayer.Attributes.Add("class", "slh");
			outerLayer.Style.Add("height", "26px");
			string tmp = Width.ToString();

			outerLayer.Style.Add("width", (Width.Value - 18).ToString() + "px");
			td.Controls.Add(outerLayer);

			HtmlGenericControl menuLayer = new HtmlGenericControl();
			menuLayer.TagName = "div";
			menuLayer.ID = ID + "_sl";
			menuLayer.Style.Add("top", "0px");
			menuLayer.Style.Add("left", "0px");
			menuLayer.Attributes.Add("class", "sl");
			menuLayer.Style.Add("height", "26px");
			menuLayer.Style.Add("width", ScrollingLayerWidth + "px");

			HtmlGenericControl nobr = new HtmlGenericControl();
			nobr.TagName = "nobr";
			menuLayer.Controls.Add(nobr);

			// // add all icons to the menu layer
			foreach (Control item in Icons)
			{
				menuLayer.Controls.Add(item);

				if (item.ID != "")
				{
					iconIds = iconIds + item.ID + ",";
				}
			}

			outerLayer.Controls.Add(
				new LiteralControl("<script>" + Environment.NewLine + "RegisterScrollingMenuButtons('" + ClientID + "', '" + iconIds +
								   "');" + Environment.NewLine + "</script>"));

			outerLayer.Controls.Add(menuLayer);

			tr.Cells.Add(td);

			// // scroll-right image
			td = new TableCell();
			Image scrollR = scrollImage();
			scrollR.ImageUrl = _ClientFilesPath + "images/arrowForward.gif";
			scrollR.Attributes.Add("onMouseOver",
								   "this.className = 'editorArrowOver'; scrollL('" + ClientID + "_sl','" + ClientID + "_slh'," +
								   ScrollingLayerWidth + ");");
			td.Controls.Add(scrollR);
			tr.Cells.Add(td);

			Controls.Add(Container);
		}
	}
}