using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Xsl;
using umbraco.BasePages;
using umbraco.uicontrols;

namespace umbraco.cms.presentation.developer
{
    /// <summary>
    /// Summary description for editXslt.
    /// </summary>
    public partial class editXslt : UmbracoEnsuredPage
    {
        protected PlaceHolder buttons;
        protected CodeArea CodeArea1;


        protected void Page_Load(object sender, EventArgs e)
        {
            // Put user code to initialize the page here
            xsltError.Text = "";
            UmbracoPanel1.hasMenu = true;
            UmbracoPanel1.Style.Add("text-align", "center");
            if (IsPostBack)
            {
                // Save to a temporary file and validate!
                StreamWriter SW;
                string tempFileName =
                    Server.MapPath(GlobalSettings.Path + "/../xslt/" + DateTime.Now.Ticks + "_temp.xslt");
                SW = File.CreateText(tempFileName);
                SW.Write(editorSource.Text);
                SW.Close();

                // Test the xslt
                string errorMessage = "";
                if (!SkipTesting.Checked)
                {
                    try
                    {
                        // Check if there's any documents yet
                        if (content.Instance.XmlContent.SelectNodes("/root/node").Count > 0)
                        {
                            XmlDocument macroXML = new XmlDocument();
                            macroXML.LoadXml("<macro/>");

                            XslTransform macroXSLT = new XslTransform();
                            page umbPage =
                                new page(content.Instance.XmlContent.SelectSingleNode("//node [@parentID = -1]"));

                            XsltArgumentList xslArgs;
                            xslArgs = macro.AddMacroXsltExtensions();
                            library lib = new library(umbPage);
                            xslArgs.AddExtensionObject("urn:umbraco.library", lib);
                            HttpContext.Current.Trace.Write("umbracoMacro", "After adding extensions");

                            // Add the current node
                            xslArgs.AddParam("currentPage", "", library.GetXmlNodeById(umbPage.PageID.ToString()));
                            HttpContext.Current.Trace.Write("umbracoMacro", "Before performing transformation");

                            macroXSLT.Load(tempFileName);
                            HtmlTextWriter macroResult = new HtmlTextWriter(new StringWriter());
                            macroXSLT.Transform(macroXML, xslArgs, macroResult, null);
                            macroResult.Close();
                        }
                        else
                        {
                            errorMessage = "stub";
                            base.speechBubble(speechBubbleIcon.info,
                                              ui.Text("errors", "xsltErrorHeader", base.getUser()),
                                              "Unable to validate xslt as no published content nodes exist.");
                        }
                    }
                    catch (Exception errorXslt)
                    {
                        base.speechBubble(speechBubbleIcon.error, ui.Text("errors", "xsltErrorHeader", base.getUser()),
                                          ui.Text("errors", "xsltErrorText", base.getUser()));

                        errorHolder.Visible = true;
                        closeErrorMessage.Visible = true;
                        errorHolder.Attributes.Add("style",
                                                   "height: 250px; overflow: auto; border: 1px solid CCC; padding: 5px;");

                        errorMessage = errorXslt.ToString();

                        // Full error message
                        xsltError.Text = errorMessage.Replace("\n", "<br/>\n");
                        closeErrorMessage.Visible = true;

                        string[] errorLine;
                        // Find error
                        MatchCollection m =
                            Regex.Matches(errorMessage, @"\d*[^,],\d[^\)]",
                                          RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                        foreach (Match mm in m)
                        {
                            errorLine = mm.Value.Split(',');

                            if (errorLine.Length > 0)
                            {
                                int theErrorLine = int.Parse(errorLine[0]);
                                int theErrorChar = int.Parse(errorLine[1]);

                                xsltError.Text = "Error in XSLT at line " + errorLine[0] + ", char " + errorLine[1] +
                                                 "<br/>";
                                xsltError.Text += "<span style=\"font-family: courier; font-size: 11px;\">";
                                string[] xsltText = editorSource.Text.Split("\n".ToCharArray());
                                for (int i = 0; i < xsltText.Length; i++)
                                {
                                    if (i >= theErrorLine - 3 && i <= theErrorLine + 1)
                                        if (i + 1 == theErrorLine)
                                        {
                                            xsltError.Text += "<b>" + (i + 1) + ": &gt;&gt;&gt;&nbsp;&nbsp;" +
                                                              Server.HtmlEncode(xsltText[i].Substring(0, theErrorChar));
                                            xsltError.Text +=
                                                "<span style=\"text-decoration: underline; border-bottom: 1px solid red\">" +
                                                Server.HtmlEncode(
                                                    xsltText[i].Substring(theErrorChar,
                                                                          xsltText[i].Length - theErrorChar)).Trim() +
                                                "</span>";
                                            xsltError.Text += " &lt;&lt;&lt;</b><br/>";
                                        }
                                        else
                                            xsltError.Text += (i + 1) + ": &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                                                              Server.HtmlEncode(xsltText[i]) + "<br/>";
                                }
                                xsltError.Text += "</span>";
                            }
                        }
                    }
                }


                if (errorMessage == "" && xsltFileName.Text.ToLower().EndsWith(".xslt"))
                {
                    //Hardcoded security-check... only allow saving files in xslt directory... 
                    string savePath = Server.MapPath(GlobalSettings.Path + "/../xslt/" + xsltFileName.Text);

                    if (savePath.StartsWith(Server.MapPath(GlobalSettings.Path + "/../xslt/")))
                    {
                        SW = File.CreateText(savePath);
                        SW.Write(editorSource.Text);
                        SW.Close();
                        base.speechBubble(speechBubbleIcon.save,
                                          ui.Text("speechBubbles", "xsltSavedHeader", base.getUser()),
                                          ui.Text("speechBubbles", "xsltSavedText", base.getUser()));
                    }
                    else
                    {
                        base.speechBubble(speechBubbleIcon.error, ui.Text("errors", "xsltErrorHeader", base.getUser()),
                                          ui.Text("errors", "xsltErrorText", base.getUser()));
                    }
                }

                File.Delete(tempFileName);
            }
            else
            {
                // editor source
                if (UmbracoSettings.ScriptDisableEditor)
                    editorJs.Text = "<script src=\"../js/textareaEditor.js\" type=\"text/javascript\"></script>";
                else
                    editorJs.Text = "<script src=\"../js/codepress/codepress.js\" type=\"text/javascript\"></script>";
            }
        }

        #region Web Form Designer generated code

        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);


            uicontrols.MenuIconI save = UmbracoPanel1.Menu.NewIcon();
            save.ImageURL = GlobalSettings.Path + "/images/editor/save.gif";
            save.OnClickCommand = "doSubmit()";
            save.AltText = "Save Xslt File";


            UmbracoPanel1.Menu.InsertSplitter();


            uicontrols.MenuIconI tmp = UmbracoPanel1.Menu.NewIcon();
            tmp.ImageURL = GlobalSettings.Path + "/images/editor/insField.GIF";
            tmp.OnClickCommand =
                "umbracoInsertField(document.getElementById('editorSource'), 'xsltInsertValueOf', '','felt', 750, 230, '');";
            tmp.AltText = "Insert xslt:value-of";

            UmbracoPanel1.Menu.InsertSplitter();

            tmp = UmbracoPanel1.Menu.NewIcon();
            tmp.ImageURL = GlobalSettings.Path + "/images/editor/insMemberItem.GIF";
            tmp.OnClickCommand =
                "insertAtCaretAndMove(document.getElementById('editorSource'), '<xsl:variable name=\"\" select=\"\"/>\\n',-14);";
            tmp.AltText = "Insert xsl:variable";

            UmbracoPanel1.Menu.InsertSplitter();

            tmp = UmbracoPanel1.Menu.NewIcon();
            tmp.ImageURL = GlobalSettings.Path + "/images/editor/insChildTemplateNew.GIF";
            tmp.OnClickCommand =
                "insertAtCaretAndMove(document.getElementById('editorSource'), '<xsl:apply-templates select=\"\"/>\\n', -4);";
            tmp.AltText = "Insert apply-templates without sorting";

            tmp = UmbracoPanel1.Menu.NewIcon();
            tmp.ImageURL = GlobalSettings.Path + "/images/editor/insChildTemplateNew.GIF";
            tmp.OnClickCommand =
                "insertAtCaretAndMove(document.getElementById('editorSource'), '<xsl:apply-templates select=\"\">\\n\t<xsl:sort select=\"\" order=\"\"/>\\n</xsl:apply-templates>\\n',-58);";
            tmp.AltText = "Insert apply-template with sorting";

            UmbracoPanel1.Menu.InsertSplitter();

            tmp = UmbracoPanel1.Menu.NewIcon();
            tmp.ImageURL = GlobalSettings.Path + "/images/editor/insFieldByLevel.GIF";
            tmp.OnClickCommand =
                "insertAtCaretAndMove(document.getElementById('editorSource'), '<xsl:template match=\"/\">\\n</xsl:template>\\n', -16);";
            tmp.AltText = "Insert xsl:template with select";

            tmp = UmbracoPanel1.Menu.NewIcon();
            tmp.ImageURL = GlobalSettings.Path + "/images/editor/insFieldByTree.GIF";
            tmp.OnClickCommand =
                "insertAtCaretAndMove(document.getElementById('editorSource'), '<xsl:template name=\"\">\\n\\t<xsl:param name=\"\"/>\\n</xsl:template>\\n',-41);";
            tmp.AltText = "Insert xsl:template with match";


            // Add source and filename
            String file = Request.QueryString["file"];

            //Hardcoded Fix/Hack, form can only open and edit xslt files.. PPH
            if (file.ToLower().EndsWith(".xslt"))
            {
                xsltFileName.Text = file;

                StreamReader SR;
                string S;
                SR = File.OpenText(Server.MapPath(GlobalSettings.Path + "/../xslt/" + file));

                S = SR.ReadToEnd();
                SR.Close();

                editorSource.Text = S;
                editorSource.Attributes.Add("onKeyDown", "AllowTabCharacter();");
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