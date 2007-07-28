using System.ComponentModel;
using System.Web.UI;

internal class Splitter : System.Web.UI.WebControls.Image
{

	protected override void OnLoad(System.EventArgs EventArguments)
	{
		this.Height = WebControls.Unit.Pixel(21);
		this.Style.Add("border", "0px");
		this.Attributes.Add("class", "editorIconSplit");
		this.ImageUrl = "/umbraco_client/menuicon/images/split.gif";
	}
}