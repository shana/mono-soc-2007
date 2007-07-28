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

using umbraco.cms.businesslogic.member;
using umbraco.cms.businesslogic.web;

namespace umbraco.presentation.umbraco.dialogs
{
	/// <summary>
	/// Summary description for protectPage.
	/// </summary>
	public partial class protectPage : BasePages.UmbracoEnsuredPage
	{
		protected System.Web.UI.WebControls.Literal jsShowWindow;
		protected controls.DualSelectbox _memberGroups = new controls.DualSelectbox();
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Check for editing
			int documentId = int.Parse(helper.Request("nodeId"));
			cms.businesslogic.web.Document documentObject = new cms.businesslogic.web.Document(documentId);
			jsShowWindow.Text = "";

			if (!IsPostBack) 
			{
				if (Access.IsProtected(documentId, documentObject.Path)) 
				{
					buttonRemoveProtection.Visible = true;
					buttonRemoveProtection.Attributes.Add("onClick", "return confirm('" + ui.Text("areyousure") + "')");

					// Get login and error pages
					int errorPage = Access.GetErrorPage(documentObject.Path);
					int loginPage = Access.GetLoginPage(documentObject.Path);
                    try
                    {
                        Document loginPageObj = new Document(loginPage);
                        if (loginPageObj != null)
                        {
                            loginId.Value = loginPage.ToString();
                            loginTitle.Text = "<b>" + new Document(loginPage).Text + "</b>";
                        }
                        Document errorPageObj = new Document(errorPage);
                        if (errorPageObj != null)
                        {
                            errorId.Value = errorPage.ToString();
                            errorTitle.Text = "<b>" + new Document(errorPage).Text + "</b>";
                        }
                    }
                    catch
                    {
                    }

					if (Access.GetProtectionType(documentId) == ProtectionType.Simple) 
					{
						Member m = Access.GetAccessingMember(documentId);
						jsShowWindow.Text = "toggleSimple();\n";
						simpleLogin.Text = m.LoginName;
						simplePassword.Text = m.Password;
						
					}
					else if (Access.GetProtectionType(documentId) == ProtectionType.Advanced)
						jsShowWindow.Text = "toggleAdvanced();\n";
				}
			}

			// Load up membergrouops
			_memberGroups.ID = "Membergroups";
			_memberGroups.Width = 175;
			string selectedGroups = "";
			foreach(MemberGroup mg in MemberGroup.GetAll) 
			{
				ListItem li = new ListItem(mg.Text, mg.Id.ToString());
				if (!IsPostBack) 
				{
					if (cms.businesslogic.web.Access.IsProtectedByGroup(int.Parse(helper.Request("nodeid")), mg.Id))
						selectedGroups += mg.Id + ",";
				}
				_memberGroups.Items.Add(li);
			}
			_memberGroups.Value = selectedGroups;
			groupsSelector.Controls.Add(_memberGroups);


			protectSimple.Text = ui.Text("update");
			protectAdvanced.Text = ui.Text("update");
			buttonRemoveProtection.Text = ui.Text("paRemoveProtection");

			// Put user code to initialize the page here
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

		protected void protectSimple_Click(object sender, System.EventArgs e)
		{
			if (Page.IsValid) 
			{
				int pageId = int.Parse(helper.Request("nodeId"));
				cms.businesslogic.member.Member m = cms.businesslogic.member.Member.GetMemberFromLoginNameAndPassword(simpleLogin.Text, simplePassword.Text);

				if (m == null) 
				{
					try 
					{
						MemberType.GetByAlias("_umbracoSystemDefaultProtectType");
					} 
					catch 
					{
						MemberType.MakeNew(BusinessLogic.User.GetUser(0), "_umbracoSystemDefaultProtectType");
					}
					m = cms.businesslogic.member.Member.MakeNew(simpleLogin.Text, "", cms.businesslogic.member.MemberType.GetByAlias("_umbracoSystemDefaultProtectType"), base.getUser());
					m.Password = simplePassword.Text;
				}

				// Create or find a memberGroup
				MemberGroup mg = MemberGroup.GetByName(simpleLogin.Text);
				if (mg == null)
					mg = MemberGroup.MakeNew(simpleLogin.Text, BusinessLogic.User.GetUser(0));

				m.AddGroup(mg.Id);

				Access.ProtectPage(true, pageId, int.Parse(loginId.Value), int.Parse(errorId.Value));
				Access.AddMemberGroupToDocument(pageId, mg.Id);
				Access.AddMemberToDocument(pageId, m.Id);
				Wizard.Visible = false;
				buttonRemoveProtection.Visible = false;
				FeedBackMessage.Text = "<div class=\"feedbackCreate\">" + ui.Text("publicAccess", "paIsProtected", new cms.businesslogic.CMSNode(pageId).Text, null) + "</div>";

			}
		}

		protected void protectAdvanced_Click(object sender, System.EventArgs e)
		{
			if (Page.IsValid) 
			{
				int pageId = int.Parse(helper.Request("nodeId"));

				cms.businesslogic.web.Access.ProtectPage(false, pageId, int.Parse(loginId.Value), int.Parse(errorId.Value));
				foreach (ListItem li in _memberGroups.Items)
					if ((","+_memberGroups.Value+",").IndexOf(","+li.Value+",") > -1)
						cms.businesslogic.web.Access.AddMemberGroupToDocument(pageId, int.Parse(li.Value));
					else
						cms.businesslogic.web.Access.RemoveMemberGroupFromDocument(pageId, int.Parse(li.Value));
				Wizard.Visible = false;
				buttonRemoveProtection.Visible = false;
				FeedBackMessage.Text = "<div class=\"feedbackCreate\">" + ui.Text("publicAccess", "paIsProtected", new cms.businesslogic.CMSNode(pageId).Text, null) + "</div>";
			}
		}

		protected void buttonRemoveProtection_Click(object sender, System.EventArgs e)
		{
			int pageId = int.Parse(helper.Request("nodeId"));
			Access.RemoveProtection(pageId);
			Wizard.Visible = false;
			buttonRemoveProtection.Visible = false;
			FeedBackMessage.Text = "<div class=\"feedbackCreate\">" + ui.Text("publicAccess", "paIsRemoved", new cms.businesslogic.CMSNode(pageId).Text, null) + "</div>";
		}
	}
}
