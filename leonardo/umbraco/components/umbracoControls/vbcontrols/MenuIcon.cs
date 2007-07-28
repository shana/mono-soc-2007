using System.ComponentModel;
using System.Web.UI;

internal class MenuIcon : System.Web.UI.WebControls.Image, MenuIconI
{
	private string _OnClickCommand = "";
	private string _AltText = "init";


	public string MenuIconI.ID1 {
		get { return this.ID; }
		set { this.ID = value; }
	}

	public string MenuIconI.AltText {
		get { return this.AlternateText; }
		set {
			_AltText = value;
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

	public string MenuIconI.ImageURL1 {
		get { return this.ImageUrl(); }
		set { this.ImageUrl = value; }
	}

	public string MenuIconI.OnClickCommand {
		get { return _OnClickCommand; }
		set { _OnClickCommand = value; }
	}

	protected override void OnLoad(System.EventArgs EventArguments)
	{
		SetupClientScript();

		// NH 17-01-2007. Trying to avoid inline styling soup 
		//        Me.Width = WebControls.Unit.Pixel(22)
		//       Me.Height = WebControls.Unit.Pixel(23)
		//Me.Style.Add("border", "0px")
		this.Attributes.Add("class", "editorIcon");
		this.Attributes.Add("onMouseover", "this.className='editorIconOver'");
		string holder;
		if (this.ID != "")
		{
			holder = this.ID.Substring(0, this.ID.LastIndexOf("_")) + "_menu";
			this.Attributes.Add("onMouseout", "hoverIconOut('" + holder + "','" + this.ID + "');");
			this.Attributes.Add("onMouseup", "hoverIconOut('" + holder + "','" + this.ID + "');");
		}
		else
		{
			this.Attributes.Add("onMouseout", "this.className='editorIcon'");
			this.Attributes.Add("onMouseup", "this.className='editorIcon'");
		}
		this.Attributes.Add("onMouseDown", "this.className='editorIconDown'");
		this.AlternateText = _AltText;

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