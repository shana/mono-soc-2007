public class MenuIconClass : MenuIconI
{

	private string _id;
	private string _imageURL;
	private string _onClickCommand;
	private string _AltText;
	private int _width;
	private int _height;


	public string MenuIconI.ID {
		get { return _id; }
		set { _id = value; }
	}

	public string MenuIconI.AltText {
		get { return _AltText; }
		set { _AltText = value; }
	}
	public int MenuIconI.IconWidth {
		get { return _width; }
		set { _width = value; }
	}
	public int MenuIconI.IconHeight {
		get { return _height; }
		set { _height = value; }
	}
	public string MenuIconI.ImageURL {
		get { return _imageURL; }
		set { _imageURL = value; }
	}

	public string MenuIconI.OnClickCommand {
		get { return _onClickCommand; }
		set { _onClickCommand = value; }
	}
}