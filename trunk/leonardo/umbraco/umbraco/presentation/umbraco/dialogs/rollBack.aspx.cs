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

using umbraco.cms.businesslogic.web;
using umbraco.cms.businesslogic.property;

namespace umbraco.presentation.dialogs
{
	/// <summary>
	/// Summary description for rollBack.
	/// </summary>
	public partial class rollBack : BasePages.UmbracoEnsuredPage
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Document d = new Document(int.Parse(helper.Request("nodeId")));
			currentVersionTitle.Text = d.Text;
			currentVersionDetails.Text = "Created by: " + d.User.Name + " at " + d.VersionDate.ToLongDateString() + " " + d.VersionDate.ToLongTimeString();
			foreach(Property p in d.getProperties) 
			{
				string thevalue = p.Value.ToString();
				if (CheckBoxHtml.Checked)
					thevalue = Server.HtmlEncode(thevalue);
				currentVersionContent.Controls.Add(new LiteralControl("<div style=\"margin-top: 4px; border: 1px solid #DEDEDE; padding: 4px;\"><p style=\"padding: 0px; margin: 0px;\" class=\"guiDialogNormal\"><b>" + p.PropertyType.Name + "</b><br/>" + thevalue + "</p></div>"));
			}

			if (allVersions.SelectedValue != "") 
			{
				Document rollback = new Document(d.Id, new Guid(allVersions.SelectedValue));
				previewVersionTitle.Text = rollback.Text;
				previewVersionDetails.Text = "Created by: " + rollback.User.Name + " at " + rollback.VersionDate.ToLongDateString() + " " + rollback.VersionDate.ToLongTimeString();
				foreach(Property p in rollback.getProperties) 
				{
					try 
					{
						previewVersionContent.Controls.Add(new LiteralControl("<div style=\"margin-top: 4px; border: 1px solid #DEDEDE; padding: 4px;\"><p style=\"padding: 0px; margin: 0px;\" class=\"guiDialogNormal\"><b>" + p.PropertyType.Name + "</b><br/>"));
						if (p.Value != null) 
						{
							string thevalue = p.Value.ToString();
							if (CheckBoxHtml.Checked)
								thevalue = Server.HtmlEncode(thevalue);
							previewVersionContent.Controls.Add(new LiteralControl(thevalue));
						}
						previewVersionContent.Controls.Add(new LiteralControl("</p></div>"));
					} 
					catch {}
				}
				doRollback.Enabled = true;
				doRollback.Attributes.Add("onClick", "return confirm('" + ui.Text("areyousure") + "');");
			} 
			else 
			{
				doRollback.Enabled = false;
				previewVersionTitle.Text = "No version selected...";
			}

			if (!IsPostBack) 
			{
				allVersions.Items.Add(new ListItem("Select version...", ""));
				foreach(DocumentVersionList dl in d.GetVersions()) 
				{
                    allVersions.Items.Add(new ListItem(dl.Text + " (Written by: " + dl.User.Name + " at " + dl.Date.ToShortDateString() + " " + dl.Date.ToShortTimeString() + ")", dl.Version.ToString()));
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

		protected void doRollback_Click(object sender, System.EventArgs e)
		{
			Document d = new Document(int.Parse(helper.Request("nodeId")));
			d.RollBack(new Guid(allVersions.SelectedValue), base.getUser());
			BusinessLogic.Log.Add(BusinessLogic.LogTypes.RollBack, base.getUser(), d.Id, "Version rolled back to revision '" + allVersions.SelectedValue + "'");
			Document rollback = new Document(d.Id, new Guid(allVersions.SelectedValue));
			FeedBackMessage.Text = "<div class=\"feedbackCreate\">Document rolled back to '" + rollback.Text + "', from " + rollback.VersionDate.ToLongDateString() + " " + rollback.VersionDate.ToShortDateString() + "</div>";
			theForm.Visible = false;
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "RollBack", "<script>\nwindow.opener.right.location.href = '../editContent.aspx?Id=" + d.Id.ToString() + "';\n</script>\n");

		}
	}
}
