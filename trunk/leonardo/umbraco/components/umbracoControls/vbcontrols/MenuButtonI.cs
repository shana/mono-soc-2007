using System.ComponentModel;
using System.Web.UI;

public class MenuImageButton : System.Web.UI.WebControls.ImageButton, MenuIconI
{
	private string _OnClickCommand = "";


	public override string MenuIconI.ID {
		get { return base.ID; }
		set { base.ID = value; }
	}

	public string MenuIconI.AltText {
		get { return this.AlternateText; }
		set {
			this.AlternateText = value;
			this.Attributes.Add("title", value);
		}
	}
	public int MenuIconI.IconWidth {
		get { return this.Width.Value; }
		set { this.Width = value; }
	}
	public int MenuIconI.IconHeight {
		get { return this.Height.Value; }
		set { this.Height = value; }
	}


	public override string MenuIconI.ImageURL {
		get { return base.ImageUrl(); }
		set { base.ImageUrl = value; }
	}

	public string MenuIconI.OnClickCommand {
		get { return _OnClickCommand; }
		set { _OnClickCommand = value; }
	}

	protected override void OnLoad(System.EventArgs EventArguments)
	{
		SetupClientScript();

		this.Width = WebControls.Unit.Pixel(22);
		this.Height = WebControls.Unit.Pixel(23);
		this.Style.Add("border", "0px");
		this.Attributes.Add("class", "editorIcon");
		this.Attributes.Add("onMouseover", "this.className='editorIconOver'");
		this.Attributes.Add("onMouseout", "this.className='editorIcon'");
		this.Attributes.Add("onMouseup", "this.className='editorIconOver'");
		this.Attributes.Add("onMouseDown", "this.className='editorIconDown'");

		if (_OnClickCommand != "")
		{
			this.Attributes.Add("onClick", OnClickCommand);
		}
	}

	private void SetupClientScript()
	{
		this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MENUICONCSS", "<link rel='stylesheet' href='/umbraco_client/menuicon/style.css' />");
	}
}