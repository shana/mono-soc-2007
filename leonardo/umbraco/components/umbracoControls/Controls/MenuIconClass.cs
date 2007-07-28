namespace umbraco.uicontrols
{
	public class MenuIconClass : MenuIconI
	{
		private string _AltText;
		private int _height;

		private string _id;
		private string _imageURL;
		private string _onClickCommand;
		private int _width;

		#region MenuIconI Members

		string MenuIconI.ID
		{
			get { return _id; }
			set { _id = value; }
		}

		string MenuIconI.AltText
		{
			get { return _AltText; }
			set { _AltText = value; }
		}

		int MenuIconI.IconWidth
		{
			get { return _width; }
			set { _width = value; }
		}

		int MenuIconI.IconHeight
		{
			get { return _height; }
			set { _height = value; }
		}

		string MenuIconI.ImageURL
		{
			get { return _imageURL; }
			set { _imageURL = value; }
		}

		string MenuIconI.OnClickCommand
		{
			get { return _onClickCommand; }
			set { _onClickCommand = value; }
		}

		#endregion
	}
}