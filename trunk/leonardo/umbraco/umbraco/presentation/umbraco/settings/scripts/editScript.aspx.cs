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
using System.IO;

namespace umbraco.cms.presentation.settings.scripts
{
    public partial class editScript : BasePages.UmbracoEnsuredPage
    {
        protected System.Web.UI.HtmlControls.HtmlForm Form1;
        protected uicontrols.UmbracoPanel Panel1;
        protected System.Web.UI.WebControls.TextBox NameTxt;
        protected uicontrols.Pane Pane7;
        protected System.Web.UI.WebControls.TextBox Content;
        protected System.Web.UI.WebControls.Literal lttPath;
        protected System.Web.UI.WebControls.Literal editorJs;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack)
            {
                // editor source
                if (UmbracoSettings.ScriptDisableEditor)
                    editorJs.Text = "<script src=\"../../js/textareaEditor.js\" type=\"text/javascript\"></script>";
                else
                    editorJs.Text = "<script src=\"../../js/codepress/codepress.js\" type=\"text/javascript\"></script>";
                
            }

            String file = Request.QueryString["file"];
            NameTxt.Text = file;

            string appPath = Request.ApplicationPath;
            if (appPath == "/")
                appPath = "";
            lttPath.Text = "(" + appPath + UmbracoSettings.ScriptFolderPath + "/" + Request.QueryString["file"] + ")";

            string openPath = Server.MapPath(UmbracoSettings.ScriptFolderPath + "/" + file);

            //security check... only allow
            if (openPath.StartsWith(Server.MapPath(UmbracoSettings.ScriptFolderPath + "/")))
            {
                StreamReader SR;
                string S;
                SR = File.OpenText(Server.MapPath(UmbracoSettings.ScriptFolderPath + "/" + file));
                S = SR.ReadToEnd();
                SR.Close();
                Content.Text = S;
                Content.CssClass = "codepress " + getCodepressType(file);
            }

            Panel1.Text = ui.Text("editscript", base.getUser());

            Panel1.Style.Add("text-align", "center");
        }

        private void save_click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            string val = Content.Text;

            try
            {
                string savePath = Server.MapPath(UmbracoSettings.ScriptFolderPath + "/" + NameTxt.Text);

                //Directory check.. only allow files in script dir and below to be edited
                if ( savePath.StartsWith( Server.MapPath(UmbracoSettings.ScriptFolderPath + "/")) )
                {
                    StreamWriter SW;
                    SW = File.CreateText(Server.MapPath(UmbracoSettings.ScriptFolderPath + "/" + NameTxt.Text));
                    SW.Write(val);
                    SW.Close();
                    this.speechBubble(speechBubbleIcon.save, ui.Text("speechBubbles", "editScriptSaved", base.getUser()), "");
                }
                else
                {
                    Trace.Warn("Couldnt save to file - Illegal path");
                    BusinessLogic.Log.Add(BusinessLogic.LogTypes.Error, base.getUser(), 0, "Saving scriptfile failed - Illegal path");
                    this.speechBubble(speechBubbleIcon.save, ui.Text("speechBubbles", "editScriptFailedIllegalPath", base.getUser()), "");
                }
            }
            catch(Exception ex)
            {
                Trace.Warn("Couldnt save to file");
                BusinessLogic.Log.Add(BusinessLogic.LogTypes.Error, base.getUser(), 0, "Saving scriptfile failed - " + ex.Message );
                this.speechBubble(speechBubbleIcon.save, ui.Text("speechBubbles", "editScriptFailed", base.getUser()), "");
            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            uicontrols.MenuIconI save = Panel1.Menu.NewIcon();
            save.ImageURL = GlobalSettings.Path + "/images/editor/save.gif";
            save.OnClickCommand = "doSubmit()";
            save.AltText = "Save File";
            

            this.Load += new System.EventHandler(Page_Load);
            InitializeComponent();
            base.OnInit(e);

        }

        private static string getCodepressType(string file)
        {
            string extension = file.Substring(file.LastIndexOf('.')).Trim('.').ToLower();

            switch (extension)
            {
                case "html":
                    return "html";
                case "htm":
                    return "html";
                case "pl":
                    return "perl";
                case "php":
                    return "php";
                case "sql":
                    return "sql";
                case "txt":
                    return "text";
                case "xslt":
                    return "xslt";
                case "js":
                    return "javascript";
                case "xml":
                    return "xml";
                case "rb":
                    return "ruby";
                default:
                    return "generic";    
            }

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
