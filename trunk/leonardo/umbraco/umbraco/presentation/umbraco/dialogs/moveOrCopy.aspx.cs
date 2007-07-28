using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace umbraco.dialogs
{
	/// <summary>
	/// Summary description for moveOrCopy.
	/// </summary>
	public partial class moveOrCopy : BasePages.UmbracoEnsuredPage
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			if (!IsPostBack) 
			{
				// Caption and properies on BUTTON
				ok.Text = ui.Text("general", "ok", this.getUser());
				ok.Attributes.Add("style", "width: 60px");
				ok.Attributes.Add("disabled", "true");

				// Captions 
				pageName.Text = "<i>" + ui.Text("moveOrCopy", "choose", this.getUser()) + "</i>";

				string currentPath = "";
				cms.businesslogic.web.Document d = new cms.businesslogic.web.Document(int.Parse(helper.Request("id")));
				foreach(string s in d.Path.Split(',')) {
					if (int.Parse(s) > 0)
						currentPath += "/" + new cms.businesslogic.web.Document(int.Parse(s)).Text;
				}

				if (helper.Request("mode") == "cut") 
				{
					Title.Text = ui.Text("general", "move", this.getUser());
					Header.Text = ui.Text("general", "move", this.getUser());
					SubHeader.Text = ui.Text("general", "move", this.getUser()) + ": " + currentPath;
					moveOrCopyTo.Text = ui.Text("moveOrCopy", "moveTo", this.getUser());
				} 
				else 
				{
					Title.Text = ui.Text("general", "copy", this.getUser());
					Header.Text = ui.Text("general", "copy", this.getUser());
					SubHeader.Text = ui.Text("general", "copy", this.getUser()) + ": " + currentPath;
					moveOrCopyTo.Text = ui.Text("moveOrCopy", "copyTo", this.getUser());

                    rememberHistory.Visible = true;
				}
			}
			
		}

		public void HandleMoveOrCopy(object sender, System.EventArgs e) 
		{
			if (helper.Request("copyTo") != "" && helper.Request("id") != "") 
			{
				// Check if the current node is allowed at new position
				bool nodeAllowed = false;

				cms.businesslogic.Content currentNode = new cms.businesslogic.Content(int.Parse(helper.Request("id")));
				cms.businesslogic.Content newNode = new cms.businesslogic.Content(int.Parse(helper.Request("copyTo")));

				// Check on contenttypes
				if (int.Parse(helper.Request("copyTo")) == -1)
					nodeAllowed = true;
				else 
				{
					foreach (int i in newNode.ContentType.AllowedChildContentTypeIDs)
						if (i == currentNode.ContentType.Id) 
						{
							nodeAllowed = true;
							break;
						}
					if (!nodeAllowed)
						FeedBackMessage.Text = "<div class=\"feedbackDelete\">" + ui.Text("moveOrCopy", "notAllowedByContentType", base.getUser()) + "</div>";
					else 
					{
						// Check on paths
						if (((string) (","+newNode.Path+",")).IndexOf(","+currentNode.Id+",") > -1) 
						{
							nodeAllowed = false;
							FeedBackMessage.Text = "<div class=\"feedbackDelete\">" + ui.Text("moveOrCopy", "notAllowedByPath", base.getUser()) + "</div>";
						}
					}
				}


				if (nodeAllowed) 
				{
					TheForm.Visible = false;
					string[] nodes = {currentNode.Text, newNode.Text};

					if (helper.Request("mode") == "cut") 
					{
						cms.businesslogic.CMSNode c = new cms.businesslogic.CMSNode(int.Parse(helper.Request("id")));
						c.Move(int.Parse(helper.Request("copyTo")));
						// library.RePublishNodesDotNet(-1, false);
                        content.Instance.RefreshContentFromDatabaseAsync();
						FeedBackMessage.Text = "<div class=\"feedbackCreate\">" + ui.Text("moveOrCopy", "moveDone", nodes, base.getUser()) + "</div>";
					} 
					else 
					{
						cms.businesslogic.web.Document d = new cms.businesslogic.web.Document(int.Parse(helper.Request("id")));
						d.Copy(int.Parse(helper.Request("copyTo")), this.getUser(), RelateDocuments.Checked);
						FeedBackMessage.Text = "<div class=\"feedbackCreate\">" + ui.Text("moveOrCopy", "copyDone", nodes, base.getUser()) + "</div>";
					}
				} 
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}
