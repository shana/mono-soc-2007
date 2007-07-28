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

namespace umbraco.cms.presentation.members
{
	/// <summary>
	/// Summary description for EditMember.
	/// </summary>
	public partial class EditMember : BasePages.UmbracoEnsuredPage
	{
		protected uicontrols.TabView TabView1;
		protected System.Web.UI.WebControls.TextBox documentName;
		private cms.businesslogic.member.Member _document;
		controls.ContentControl tmp;

		protected TextBox MemberLoginNameTxt = new TextBox();
		protected PlaceHolder MemberPasswordTxt = new PlaceHolder();
		protected TextBox MemberEmail = new TextBox();
		protected controls.DualSelectbox _memberGroups = new controls.DualSelectbox();


		protected void Page_Load(object sender, System.EventArgs e)
		{

            // Add password changer
            MemberPasswordTxt.Controls.Add(new UserControl().LoadControl(GlobalSettings.Path + "/controls/passwordChanger.ascx"));
            
            _document = new cms.businesslogic.member.Member(int.Parse(Request.QueryString["id"]));
			tmp = new controls.ContentControl(_document,controls.ContentControl.publishModes.NoPublish, "TabView1");
			tmp.Width = Unit.Pixel(666);
			tmp.Height = Unit.Pixel(666);
			plc.Controls.Add(tmp);

			tmp.PropertiesPane.addProperty(ui.Text("login"), MemberLoginNameTxt);
			tmp.PropertiesPane.addProperty(ui.Text("password"), MemberPasswordTxt);
			tmp.PropertiesPane.addProperty("Email", MemberEmail);

			// Groups
			umbraco.uicontrols.Pane p = new umbraco.uicontrols.Pane();
			_memberGroups.ID = "Membergroups";
			_memberGroups.Width = 175;
			string selectedMembers = "";
			foreach(MemberGroup mg in MemberGroup.GetAll) 
			{
				ListItem li = new ListItem(mg.Text, mg.Id.ToString());
				if (!IsPostBack) 
				{
					if (_document.Groups.ContainsKey(mg.Id))
						selectedMembers += mg.Id + ",";
				}
				_memberGroups.Items.Add(li);
			}
			_memberGroups.Value = selectedMembers;
			p.addProperty(ui.Text("memberGroups"), _memberGroups);
			tmp.tpProp.Controls.Add(p);


			if (!IsPostBack) {
				MemberLoginNameTxt.Text = _document.LoginName;
            	MemberEmail.Text = _document.Email;

			}

			tmp.Save += new System.EventHandler(tmp_save);
		}
		protected void tmp_save(object sender, System.EventArgs e) {
			_document.LoginName = MemberLoginNameTxt.Text;
			_document.Email = MemberEmail.Text;

            // Check if password should be changed
            string tempPassword = ((controls.passwordChanger)MemberPasswordTxt.Controls[0]).Password;
            if (tempPassword.Trim() != "")
                _document.Password = tempPassword;

			// Groups
			foreach (ListItem li in _memberGroups.Items)
				if ((","+_memberGroups.Value+",").IndexOf(","+li.Value+",") > -1)
					_document.AddGroup(int.Parse(li.Value));
				else
					_document.RemoveGroup(int.Parse(li.Value));

			// refresh cache
			_document.XmlGenerate(new System.Xml.XmlDocument());

			this.speechBubble(BasePages.BasePage.speechBubbleIcon.save,ui.Text("speechBubbles", "editMemberSaved", base.getUser()),"");
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
