using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;

namespace Umbraco.uicontrols
{
	[Designer(typeof(UmbracoCodeAreaDesigner)), ToolboxData("<{0}:CodeArea runat=server></{0}:CodeArea>")]
	public class CodeArea : TextBox
	{
		public CodeArea()
		{
			Attributes.Add("class", "guiInputCode");
			Attributes.Add("onSelect", "storeCaret(this);");
			Attributes.Add("onClick", "storeCaret(this);");
			Attributes.Add("onKeyUp", "storeCaret(this);");
			Attributes.Add("onKeyDown", "AllowTabCharacter();");
			Attributes.Add("wrap", "off");
			TextMode = TextBoxMode.MultiLine;
		}

		protected override void OnInit(EventArgs e)
		{
			Page.ClientScript.RegisterClientScriptBlock(GetType(), "CodeAreaStyles",
														"<link rel='stylesheet' href='/Umbraco_client/CodeArea/style.css' />");
			Page.ClientScript.RegisterClientScriptBlock(GetType(), "CodeAreaJavaScript",
														"<script language='javascript' src='/Umbraco_client/CodeArea/javascript.js'></script>");
		}
	}


	internal class UmbracoCodeAreaDesigner : ControlDesigner
	{
		public UmbracoCodeAreaDesigner()
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
					CodeArea PanelCtrl = (CodeArea)Component;
					CTRLHeight = (int)PanelCtrl.Height.Value;
					CTRLWidth = (int)PanelCtrl.Width.Value;
					return
						("<textarea style='height:" + CTRLHeight + ";width:" + CTRLWidth + ";border:1px #ccc;'>" + PanelCtrl.Text +
						 "</textarea>");
				}
				catch
				{
				}
				return
					("<textarea style='height:" + CTRLHeight + ";width:" + CTRLWidth +
					 ";border:1px #ccc;'> No width and height defined</textarea>");
			}
			catch (Exception ex)
			{
				return ("<div style='height:200px;width:200px;'> Error loading control!! </div>");
			}
		}
	}
}