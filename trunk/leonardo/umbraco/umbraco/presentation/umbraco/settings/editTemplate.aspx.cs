using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace umbraco.cms.presentation.settings
{
	/// <summary>
	/// Summary description for editTemplate.
	/// </summary>
	public partial class editTemplate : BasePages.UmbracoEnsuredPage
	{
		private cms.businesslogic.template.Template _template;
	

		protected void Page_Load(object sender, System.EventArgs e)
		{
			_template = new cms.businesslogic.template.Template(int.Parse(Request.QueryString["templateID"]));
			
			if (!IsPostBack) {
				MasterTemplate.Items.Add(new ListItem(ui.Text("none"),"0"));
				foreach (cms.businesslogic.template.Template t in cms.businesslogic.template.Template.getAll()) 
				{
					if (t.Id != _template.Id) 
					{
						ListItem li = new ListItem(t.Text,t.Id.ToString());
						if (t.Id == _template.MasterTemplate) 
						{
							try 
							{
								li.Selected = true;
							}
							catch {}
						}
						MasterTemplate.Items.Add(li);
					}
				}
				NameTxt.Text = _template.GetRawText();
				AliasTxt.Text = _template.Alias;
                editorSource.Text = _template.Design;

                // editor source
                if (UmbracoSettings.ScriptDisableEditor)
                    editorJs.Text = "<script src=\"../js/textareaEditor.js\" type=\"text/javascript\"></script>";
                else
                    editorJs.Text = "<script src=\"../js/codepress/codepress.js\" type=\"text/javascript\"></script>";

              }			
		}

		private void save_click(object sender, System.Web.UI.ImageClickEventArgs e) {
			_template.Text = NameTxt.Text;
			_template.Alias = AliasTxt.Text;
			_template.MasterTemplate = int.Parse(MasterTemplate.SelectedValue);
            _template.Design = editorSource.Text;

			this.speechBubble(BasePages.BasePage.speechBubbleIcon.save,ui.Text("speechBubbles", "editTemplateSaved", base.getUser()),"");

			// Clear cache in rutime
			if (UmbracoSettings.UseDistributedCalls)
				umbraco.presentation.cache.dispatcher.Refresh(
					new Guid("dd12b6a0-14b9-46e8-8800-c154f74047c8"), 
					_template.Id);
			else
				template.ClearCachedTemplate(_template.Id);

		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
			Panel1.hasMenu = true;

            uicontrols.MenuIconI save = Panel1.Menu.NewIcon();
            save.ImageURL = GlobalSettings.Path + "/images/editor/save.gif";
            save.OnClickCommand = "doSubmit()";
            save.AltText = "Save File";
	
			Panel1.Text = ui.Text("edittemplate");

			// Editing buttons
			Panel1.Menu.InsertSplitter();
			uicontrols.MenuIconI umbField = Panel1.Menu.NewIcon();
			umbField.ImageURL = UmbracoPath + "/images/editor/insField.gif";
			umbField.OnClickCommand = "umbracoInsertField(document.forms[0].TemplateBody, 'umbracoField', 'UMBRACOGETDATA','felt', 640, 650, '../dialogs/');";
			umbField.AltText = "Insert umbraco page field";
			uicontrols.MenuIconI umbMacro = Panel1.Menu.NewIcon();
			umbMacro.ImageURL = UmbracoPath + "/images/editor/insMacro.gif";
			umbMacro.AltText = "Insert umbraco macro";
			umbMacro.OnClickCommand = "umbracoTemplateInsertMacro();";

			// Help
			Panel1.Menu.InsertSplitter();
			uicontrols.MenuIconI helpIcon = Panel1.Menu.NewIcon();
			helpIcon.OnClickCommand = "window.open('modals/showumbracotags.aspx','helpwindow','height=400,width=500,channelmode=0,dependent=0,directories=0,fullscreen=0,location=0,menubar=0,resizable=1,scrollbars=1,status=0,toolbar=0');";
			helpIcon.ImageURL = UmbracoPath + "/images/editor/help.png";
			helpIcon.AltText = "Help (see all template tags)";


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
