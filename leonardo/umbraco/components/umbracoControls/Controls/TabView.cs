using System;
using System.Collections;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

//[Designer(typeof (TabViewDesigner)), ToolboxData("<{0}:TabView runat=server></{0}:TabView>")]
namespace Umbraco.uicontrols
{
	public class TabView : WebControl
	{
		private ArrayList Panels = new ArrayList();
		private ArrayList Tabs = new ArrayList();
		private HtmlInputHidden tb = new HtmlInputHidden();

		private string ActiveTabId
		{
			get
			{
				if (Parent.Page.IsPostBack)
				{
					return Parent.Page.Request.Form[ClientID + "_activetab"];
				}
				return ClientID + "_tab01";
			}
		}

		public ArrayList GetPanels()
		{
			return Panels;
		}

		public TabPage NewTabPage(string text)
		{
			Tabs.Add(text);
			TabPage tp = new TabPage();
			tp.Width = Width;
			tp.ID = ID + "_tab0" + (Panels.Count + 1) + "layer";
			Panels.Add(tp);
			Controls.Add(tp);
			return tp;
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			SetupClientScript();
		}

		private void SetupClientScript()
		{
			Page.ClientScript.RegisterClientScriptBlock(GetType(), "TABVIEWCSS",
														"<link rel='stylesheet' href='/Umbraco_client/tabview/style.css' />");
			Page.ClientScript.RegisterClientScriptBlock(GetType(), "TABVIEWJS",
														"<script language='javascript' src='/Umbraco_client/tabview/javascript.js'></script>");

			string strTmp = "";
			for (int i = 1; i <= Tabs.Count; i++)
			{
				if (i > 1) strTmp += ",";
				strTmp += "\"" + ClientID + "_tab0" + i + "\"";
			}
			Page.ClientScript.RegisterStartupScript(GetType(), ClientID + "TABCOLLECTION",
													"<script language='javascript'>var " + ClientID + "_tabs = new Array(" +
													strTmp + ");</script>");
			Page.ClientScript.RegisterStartupScript(GetType(), ClientID + "TABVIEWSTARTUP",
													"<script language='javascript'>setActiveTab('" + ClientID + "','" +
													ActiveTabId + "'," + ClientID + "_tabs);</script>");
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.WriteLine("<div id='" + ClientID + "' style='height:" + Height.Value + "px;width:" + Width.Value + "px;'>");
			writer.WriteLine("  <div class='header'>");
			writer.WriteLine("      <ul>");
			for (int i = 0; i <= Tabs.Count - 1; i++)
			{
				string TabPageCaption = (string)Tabs[i];
				string TabId = ClientID + "_tab0" + (i + 1);
				writer.WriteLine("          <li id='" + TabId + "' class='tabOff' onclick=\"setActiveTab('" + ClientID + "','" +
								 TabId + "'," + ClientID + "_tabs)\">");
				writer.WriteLine("              <a id='" + TabId + "a' href='#'>");
				writer.WriteLine("                  <span><nobr>" + TabPageCaption + "</nobr></span>");
				writer.WriteLine("              </a>");
				writer.WriteLine("          </li>");
			}
			writer.WriteLine("      </ul>");
			writer.WriteLine("  </div>");
			writer.WriteLine("  <div id='' class='tabpagecontainer'>");
			RenderChildren(writer);
			writer.WriteLine("\t</div>");
			writer.WriteLine("</div>");
			writer.WriteLine("<input type='hidden' name='" + ClientID + "_activetab' id='" + ClientID + "_activetab' value='" +
							 ActiveTabId + "'/>");
		}
	}
}