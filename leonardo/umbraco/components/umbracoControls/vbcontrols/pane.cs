using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
[Designer(typeof(PanelDesigner)), PersistChildren(true), ParseChildren(false), ToolboxData("<{0}:Pane runat=server></{0}:Pane>")]
public class Pane : System.Web.UI.WebControls.Panel
{

	private TableRow tr = new TableRow();
	private TableCell td = new TableCell();
	private Table tbl = new Table();

	public Pane()
	{

	}

	public void addProperty(string Caption, Control C)
	{
		tr = new TableRow();
		td = new TableHeaderCell();
		td.Text = Caption;
		td.Attributes.Add("width", "15%");
		td.Attributes.Add("valign", "top");
		tr.Cells.Add(td);

		td = new TableCell();
		td.Controls.Add(C);
		tr.Cells.Add(td);

		tbl.Rows.Add(tr);
		if (!this.Controls.Contains(tbl))
		{
			this.Controls.Add(tbl);
		}
	}

	public void addProperty(Control ctrl)
	{
		tr = new TableRow();
		td = new TableHeaderCell();
		td.ColumnSpan = 2;
		td.Attributes.Add("width", "100%");
		td.Attributes.Add("valign", "top");
		td.Controls.Add(ctrl);
		tr.Cells.Add(td);
		tbl.Rows.Add(tr);
		if (!this.Controls.Contains(tbl))
		{
			this.Controls.Add(tbl);
		}
	}


	protected override void OnLoad(System.EventArgs EventArguments)
	{
		this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "propertypanestyles", "<link rel='stylesheet' href='/umbraco_client/propertypane/style.css' />");
		tbl.Style.Add("width", "100%");
	}

	protected override void Render(System.Web.UI.HtmlTextWriter writer)
	{

		this.CreateChildControls();
		string styleString = "";

		foreach (string key in this.Style.Keys) {
			styleString += key + ":" + this.Style.Item(key) + ";";
		}

		writer.WriteLine("<div class=\"propertypane\" style='" + styleString + "'>");
		writer.WriteLine("<div>");
		try {
			this.RenderChildren(writer);
		}

		catch (Exception ex) {
			writer.WriteLine("Error creating control <br />");
			writer.WriteLine(ex.ToString());
		}
		writer.WriteLine("</div>");
		writer.WriteLine("</div>");

	}

}

public class PanelDesigner : System.Web.UI.Design.ControlDesigner
{

	public override string GetDesignTimeHTML()
	{
		//Return ("<div style='height:200px;width:100%;'> UMBRACO PROPERTYPANE UPDATE - SOON WITH DESIGNERSUPPORT! </div>")
		try {
			int CTRLHeight = 10;
			int CTRLWidth = 10;
			try {
				Pane PanelCtrl = (Pane)this.Component;
				CTRLHeight = (int)PanelCtrl.Height.Value;
				CTRLWidth = (int)PanelCtrl.Width.Value;
			}
			catch {
			}
			return ("<div style='width: 98%;background: #FFF url(http://localhost/umbraco_client/propertypane/images/propertyBackground.gif) top repeat-x;padding: 5;margin: 0;margin-top:5px;border: 1px solid #BABABA;text-align:left;'>" + base.GetDesignTimeHtml + "</div>");
		}
		catch (Exception ex) {
			return ("<div style='height:200px;width:200px;'> Error loading control!! </div>");
		}

	}
}