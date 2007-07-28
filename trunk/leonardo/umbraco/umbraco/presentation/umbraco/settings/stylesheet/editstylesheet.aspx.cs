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
using umbraco.BusinessLogic;

namespace umbraco.cms.presentation.settings.stylesheet
{
	/// <summary>
	/// Summary description for editstylesheet.
	/// </summary>
	public partial class editstylesheet : BasePages.UmbracoEnsuredPage
	{
		private cms.businesslogic.web.StyleSheet stylesheet;

		protected void Page_Load(object sender, System.EventArgs e)
		{

            if(!IsPostBack)
            {
                // editor source
                if (UmbracoSettings.ScriptDisableEditor)
                    editorJs.Text = "<script src=\"../../js/textareaEditor.js\" type=\"text/javascript\"></script>";
                else
                    editorJs.Text = "<script src=\"../../js/codepress/codepress.js\" type=\"text/javascript\"></script>";
                
            }

             uicontrols.MenuIconI save = Panel1.Menu.NewIcon();
             save.ImageURL = GlobalSettings.Path + "/images/editor/save.gif";
             save.OnClickCommand = "doSubmit()";
             save.AltText = "Save stylesheet";

            Panel1.Text = ui.Text("editstylesheet", base.getUser());
 

			stylesheet = new cms.businesslogic.web.StyleSheet(int.Parse(Request.QueryString["id"]));
			string appPath = Request.ApplicationPath;
			if (appPath == "/") 
				appPath = "";
			lttPath.Text = "(" + appPath + "/css/"+ stylesheet.Text + ".css)";
			Panel1.Style.Add("text-align","center");
			if (!IsPostBack) 
			{
				NameTxt.Text = stylesheet.Text;
                editorSource.Text = stylesheet.Content;
			}
		}

		private void save_click(object sender, System.Web.UI.ImageClickEventArgs e) {
            string val = editorSource.Text;

			if (stylesheet == null) throw new ArgumentException("hest");

            stylesheet.Content = editorSource.Text;
			stylesheet.Text = NameTxt.Text;
			try 
			{
				stylesheet.saveCssToFile();
                this.speechBubble(speechBubbleIcon.save, ui.Text("speechBubbles", "editStylesheetSaved", base.getUser()), "");
            } 
			catch (Exception ex) {
                this.speechBubble(speechBubbleIcon.error, "Error saving stylesheet", "Probably due missing permissions. Look in log for full error message.");
                Log.Add(LogTypes.Error, getUser(), -1, string.Format("Error saving stylesheet: {0}", ex.ToString()));
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
