using System;
using System.Web.UI.WebControls;

namespace umbraco.uicontrols
{
	internal class Splitter : Image
	{
		protected override void OnLoad(EventArgs EventArguments)
		{
			Height = Unit.Pixel(21);
			Style.Add("border", "0px");
			Attributes.Add("class", "editorIconSplit");
			ImageUrl = "/umbraco_client/menuicon/images/split.gif";
		}
	}
}