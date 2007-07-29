using System;

namespace umbraco.editorControls
{
	/// <summary>
	/// Summary description for mediaChooser.
	/// </summary>
	public class mediaChooser : System.Web.UI.WebControls.WebControl, interfaces.IDataEditor
	{
		interfaces.IData _data;
		public mediaChooser(interfaces.IData Data)
		{
			_data = Data;
		}

		public System.Web.UI.Control Editor {get{return this;}}
		#region IDataField Members

		private string _text;

		public virtual bool TreatAsRichTextEditor 
		{
			get {return false;}
		}

		public bool ShowLabel
		{
			get
			{
				// TODO:  Add mediaChooser.ShowLabel getter implementation
				return true;
			}
		}

		
		public string Text
		{
			get
			{
				if (!Page.IsPostBack) 
				{
					_text = _data.Value.ToString();
				}
				return _text;
			}
			set {
			_text = value;
			}
		}

		public void Save()
		{
            _text = helper.Request(this.ClientID);
            if (Text.Trim() != "")
                _data.Value = _text;
            else
                _data.Value = null;
        }

		protected override void OnInit(EventArgs e)
		{
			base.OnInit (e);
			base.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ajax", "<script type=\"text/javascript\" src=\"" + GlobalSettings.Path + "/webservices/ajax.js\"></script>");
			base.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ajax1", "<script type=\"text/javascript\" src=\"" + GlobalSettings.Path + "/js/xmlextras.js\"></script><script type=\"text/javascript\" src=\"" + GlobalSettings.Path + "/js/xmlRequest.js\"></script>");
            base.Page.ClientScript.RegisterClientScriptBlock(GetType(), "subModal", "<script type=\"text/javascript\" src=\"" + GlobalSettings.Path + "/js/submodal/common.js\"></script><script type=\"text/javascript\" src=\"" + GlobalSettings.Path + "/js/submodal/subModal.js\"></script><link href=\"" + GlobalSettings.Path + "/js/submodal/subModal.css\" type=\"text/css\" rel=\"stylesheet\"></link>");
            base.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "webserviceCMSNode", "<script type=\"text/javascript\" src=\"" + GlobalSettings.Path + "/webservices/GetJavaScriptProxy.aspx?service=CMSNode.asmx\"></script>");
        }


		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{

			string tempTitle = "";
			string deleteLink = " &nbsp; <a href=\"javascript:" + this.ClientID + "_clear();\" style=\"color: red\">" + UI.Text("delete") + "</a> &nbsp; ";
			try 
			{
				if (this.Text != "") 
				{
					tempTitle = new Cms.BusinessLogic.CMSNode(int.Parse(this.Text)).Text;
				}
			} 
			catch {}

            writer.WriteLine("<script language=\"javascript\">\nfunction " + this.ClientID + "_chooseId() {" +
                "\nshowPopWin('" + GlobalSettings.Path + "/dialogs/treePicker.aspx?useSubModal=true&appAlias=media', 350, 300, " + ClientID + "_saveId)" +
                //				"\nvar treePicker = window.showModalDialog(, 'treePicker', 'dialogWidth=350px;dialogHeight=300px;scrollbars=no;center=yes;border=thin;help=no;status=no')			" +
                "\n}" +
                "\nfunction " + ClientID + "_saveId(treePicker) {" +
                "\nsetTimeout('" + ClientID + "_saveIdDo(' + treePicker + ')', 200);" +
                "\n}" +
                "\nfunction " + ClientID + "_saveIdDo(treePicker) {" +
                "\nif (treePicker != undefined) {" +
					"\ndocument.getElementById(\"" + this.ClientID + "\").value = treePicker;" +
					"\nif (treePicker > 0) {"+
                    "\nproxies.CMSNode.GetNodeName.func = " + this.ClientID + "_updateContentTitle;" + 
			        "\nproxies.CMSNode.GetNodeName('" + BasePages.BasePage.UmbracoUserContextID + "', treePicker);" +
					"\n}				" +
				"\n}"+
			"\n}			" + 
				"\nfunction " + this.ClientID + "_updateContentTitle(retVal) {"+
                "\ndocument.getElementById(\"" + this.ClientID + "_title\").innerHTML = \"<strong>\" + retVal + \"</strong>" + deleteLink.Replace("\"", "\\\"") + "\";" +
				"\n}"+
				"\nfunction " + this.ClientID + "_clear() {"+
				"\ndocument.getElementById(\"" + this.ClientID + "_title\").innerHTML = \"\";"+
				"\ndocument.getElementById(\"" + this.ClientID + "\").value = \"\";"+
				"\n}"+
				"\n</script>");

			// Clear remove link if text if empty
			if (this.Text == "")
				deleteLink = "";
			writer.WriteLine("<span id=\"" + this.ClientID + "_title\"><b>" + tempTitle + "</b>" + deleteLink + "</span><a href=\"javascript:" + this.ClientID + "_chooseId()\">" + UI.Text("choose") + "...</a> &nbsp; <input type=\"hidden\" id=\"" + this.ClientID + "\" Name=\"" + this.ClientID + "\" value=\"" + this.Text + "\">");
			base.Render (writer);
		}
		#endregion
	}
}
