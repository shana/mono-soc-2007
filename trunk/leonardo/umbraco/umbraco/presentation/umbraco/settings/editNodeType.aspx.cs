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

namespace umbraco.cms.presentation.settings
{
	/// <summary>
	/// Summary description for editXslt.
	/// </summary>
	public partial class editNodeType : BasePages.UmbracoEnsuredPage
	{
		private cms.businesslogic.web.DocumentType dt;
		DropDownList TemplateDDL = new DropDownList();

		protected void Page_Load(object sender, System.EventArgs e)
		{
			dt = new cms.businesslogic.web.DocumentType(int.Parse(Request.QueryString["id"]));
			controls.ContentTypeControl tmp = new controls.ContentTypeControl(dt,"TabView1");
			
			// handle save click ..
			tmp.OnSave +=new EventHandler(tmp_OnSave);
	
			// Fix template on documenttype
			TemplateDDL.Items.Add(new ListItem("Ingen template","0"));

			foreach (cms.businesslogic.template.Template t in cms.businesslogic.template.Template.getAll()) 
			{
				ListItem li = new ListItem();
				li.Value = t.Id.ToString();
				li.Text = t.Text;
				TemplateDDL.Items.Add(li);
			}			
			if (dt.HasTemplate()) {
//				int templateId = dt.Template.Id;
//				TemplateDDL.SelectedValue = templateId.ToString();
			}
			uicontrols.Pane pp = new uicontrols.Pane();
			pp.addProperty("Skabelon",TemplateDDL);
			tmp.addPropertyPaneToGeneralTab(pp);

			plc.Controls.Add(tmp);
		}

		protected void tmp_OnSave(object sender, System.EventArgs e) {
//			if (int.Parse(TemplateDDL.SelectedValue) != 0) 
//				dt.Template = new cms.businesslogic.template.Template(int.Parse(TemplateDDL.SelectedValue));
//			else
//				dt.clearTemplates();
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