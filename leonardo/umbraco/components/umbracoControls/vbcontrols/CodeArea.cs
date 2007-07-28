using System.ComponentModel;
using System.Web.UI;


[Designer(typeof(UmbracoCodeAreaDesigner)), ToolboxData("<{0}:CodeArea runat=server></{0}:CodeArea>")]
public class CodeArea : System.Web.UI.WebControls.TextBox
{

	public CodeArea()
	{
		this.Attributes.Add("class", "guiInputCode");
		this.Attributes.Add("onSelect", "storeCaret(this);");
		this.Attributes.Add("onClick", "storeCaret(this);");
		this.Attributes.Add("onKeyUp", "storeCaret(this);");
		this.Attributes.Add("onKeyDown", "AllowTabCharacter();");
		this.Attributes.Add("wrap", "off");
		this.TextMode = WebControls.TextBoxMode.MultiLine;
	}

	protected override void OnInit(EventArgs e)
	{

		this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "CodeAreaStyles", "<link rel='stylesheet' href='/umbraco_client/CodeArea/style.css' />");
		this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "CodeAreaJavaScript", "<script language='javascript' src='/umbraco_client/CodeArea/javascript.js'></script>");
	}
}


class UmbracoCodeAreaDesigner : System.Web.UI.Design.ControlDesigner
{

	public UmbracoCodeAreaDesigner()
	{

	}
	public override string GetDesignTimeHTML()
	{
		try {
			int CTRLHeight = 10;
			int CTRLWidth = 10;

			try {
				CodeArea PanelCtrl = (CodeArea)this.Component;
				CTRLHeight = (int)PanelCtrl.Height.Value;
				CTRLWidth = (int)PanelCtrl.Width.Value;
				return ("<textarea style='height:" + CTRLHeight + ";width:" + CTRLWidth + ";border:1px #ccc;'>" + PanelCtrl.Text + "</textarea>");
			}
			catch {
			}
			return ("<textarea style='height:" + CTRLHeight + ";width:" + CTRLWidth + ";border:1px #ccc;'> No width and height defined</textarea>");
		}
		catch (Exception ex) {
			return ("<div style='height:200px;width:200px;'> Error loading control!! </div>");
		}
	}

}