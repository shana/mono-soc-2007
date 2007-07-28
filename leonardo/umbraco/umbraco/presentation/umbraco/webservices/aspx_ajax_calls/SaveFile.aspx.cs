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
using System.Xml;
using System.Xml.Xsl;
using System.Text.RegularExpressions;

namespace umbraco.presentation.umbraco.webservices.aspx_ajax_calls
{
    public partial class SaveFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (BasePages.BasePage.ValidateUserContextID(BasePages.BasePage.umbracoUserContextID))
            {
                string fileName = Request["fileName"];
                string fileAlias = Request["fileAlias"];
                string fileContents = Request["fileContents"];
                string fileType = Request["fileType"];
                int fileID = 0; 
                int.TryParse(Request["fileID"], out fileID);
                
                int masterID = 0; 
                int.TryParse(Request["masterID"], out masterID);


                bool ignoreDebug = false;
                bool.TryParse(Request["ignoreDebug"], out ignoreDebug);

                
                switch (fileType)
                {
                    case "xslt":
                        Response.Write(saveXslt(fileName, fileContents, ignoreDebug));
                        break;
                    case "python":
                        Response.Write("true");
                        break;
                    case "css":
                        Response.Write(saveCss(fileName, fileContents, fileID));
                        break;
                    case "script":
                        Response.Write(saveScript(fileName, fileContents));
                        break;
                    case "template":
                        Response.Write(saveTemplate(fileName, fileAlias, fileContents, fileID, masterID));  
                        break;
                }
                
            }
        }

        private string saveCss(string fileName, string fileContents, int fileID)
        {
            string returnValue = "false";
            cms.businesslogic.web.StyleSheet stylesheet = new cms.businesslogic.web.StyleSheet(fileID);
            
            if (stylesheet != null)
            {
                stylesheet.Content = fileContents;
                stylesheet.Text = fileName;
                try
                {
                    stylesheet.saveCssToFile();
                    returnValue = "true";
                }
                catch
                {
                    Trace.Warn("Couldnt save to file");
                }

                //this.speechBubble(speechBubbleIcon.save, ui.Text("speechBubbles", "editStylesheetSaved", base.getUser()), "");
            }
            return returnValue;
        }

        private string saveXslt(string fileName, string fileContents, bool ignoreDebugging)
        {
                StreamWriter SW;
				string tempFileName = Server.MapPath(GlobalSettings.Path + "/../xslt/" + System.DateTime.Now.Ticks + "_temp.xslt");
				SW=File.CreateText(tempFileName);
				SW.Write(fileContents);
				SW.Close();

                // Test the xslt
                string errorMessage = "";
                if (!ignoreDebugging)
                {
                    try
                    {

                        // Check if there's any documents yet
                        if (content.Instance.XmlContent.SelectNodes("/root/node").Count > 0)
                        {
                            XmlDocument macroXML = new XmlDocument();
                            macroXML.LoadXml("<macro/>");

                            XslTransform macroXSLT = new XslTransform();
                            page umbPage = new page(content.Instance.XmlContent.SelectSingleNode("//node [@parentID = -1]"));

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
                            //base.speechBubble(speechBubbleIcon.info, ui.Text("errors", "xsltErrorHeader", base.getUser()), "Unable to validate xslt as no published content nodes exist.");
                        }

                    }
                    catch (Exception errorXslt)
                    {
                        //base.speechBubble(speechBubbleIcon.error, ui.Text("errors", "xsltErrorHeader", base.getUser()), ui.Text("errors", "xsltErrorText", base.getUser()));

                        //errorHolder.Visible = true;
                        //closeErrorMessage.Visible = true;
                        //errorHolder.Attributes.Add("style", "height: 250px; overflow: auto; border: 1px solid CCC; padding: 5px;");

                        errorMessage = errorXslt.ToString();

                        // Full error message
                        errorMessage = errorMessage.Replace("\n", "<br/>\n");
                        //closeErrorMessage.Visible = true;

                        string[] errorLine;
                        // Find error
                        MatchCollection m = Regex.Matches(errorMessage, @"\d*[^,],\d[^\)]", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                        foreach (Match mm in m)
                        {
                            errorLine = mm.Value.Split(',');

                            if (errorLine.Length > 0)
                            {
                                int theErrorLine = int.Parse(errorLine[0]);
                                int theErrorChar = int.Parse(errorLine[1]);

                                errorMessage = "Error in XSLT at line " + errorLine[0] + ", char " + errorLine[1] + "<br/>";
                                errorMessage += "<span style=\"font-family: courier; font-size: 11px;\">";
                                
                                string[] xsltText = fileContents.Split("\n".ToCharArray());
                                for (int i = 0; i < xsltText.Length; i++)
                                {
                                    if (i >= theErrorLine - 3 && i <= theErrorLine + 1)
                                        if (i + 1 == theErrorLine)
                                        {
                                            errorMessage += "<b>" + (i + 1) + ": &gt;&gt;&gt;&nbsp;&nbsp;" + Server.HtmlEncode(xsltText[i].Substring(0, theErrorChar));
                                            errorMessage += "<span style=\"text-decoration: underline; border-bottom: 1px solid red\">" + Server.HtmlEncode(xsltText[i].Substring(theErrorChar, xsltText[i].Length - theErrorChar)).Trim() + "</span>";
                                            errorMessage += " &lt;&lt;&lt;</b><br/>";
                                        }
                                        else
                                            errorMessage += (i + 1) + ": &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + Server.HtmlEncode(xsltText[i]) + "<br/>";
                                }
                                errorMessage += "</span>";

                            }
                        }
                    }

                }

                
                if (errorMessage == "" && fileName.ToLower().EndsWith(".xslt"))
                {
                    //Hardcoded security-check... only allow saving files in xslt directory... 
                    string savePath = Server.MapPath(GlobalSettings.Path + "/../xslt/" + fileName);

                    if(savePath.StartsWith(Server.MapPath(GlobalSettings.Path + "/../xslt/")))
                    {
                    SW = File.CreateText(savePath);
                    SW.Write(fileContents);
                    SW.Close();
                    errorMessage = "true"; 
                    }
                    else
                    {
                    errorMessage = "Illegal path"; 
                    }
            }

                System.IO.File.Delete(tempFileName);
            

            return errorMessage;
        }

        private string savePython(string filename, string contents)
        {


            return "true";
        }

        private string saveScript(string filename, string contents)
        {
            string val = contents;
            string returnValue = "false";
            try
            {
                string savePath = Server.MapPath(UmbracoSettings.ScriptFolderPath + "/" + filename);

                //Directory check.. only allow files in script dir and below to be edited
                if (savePath.StartsWith(Server.MapPath(UmbracoSettings.ScriptFolderPath + "/")))
                {
                    StreamWriter SW;
                    SW = File.CreateText(Server.MapPath(UmbracoSettings.ScriptFolderPath + "/" + filename));
                    SW.Write(val);
                    SW.Close();
                    returnValue = "true";
                 }
                else
                {
                    Trace.Warn("Couldnt save to file - Illegal path");
                    returnValue = "illegalPath";
                }
            }
            catch (Exception ex)
            {
                Trace.Warn("saveFile", "Couldnt save to file", ex);
                returnValue = "false";
            }


            return returnValue;
        }

        private string saveTemplate(string templateName, string templateAlias, string templateContents, int templateID, int masterTemplateID)
        {
            
            cms.businesslogic.template.Template _template = new global::umbraco.cms.businesslogic.template.Template(templateID);
            string retVal = "false";

            if (_template != null)
            {
                _template.Text = templateName;
                _template.Alias = templateAlias;
                _template.MasterTemplate = masterTemplateID;
                _template.Design = templateContents;

                retVal = "true";

                // Clear cache in rutime
                if (UmbracoSettings.UseDistributedCalls)
                    cache.dispatcher.Refresh(
                        new Guid("dd12b6a0-14b9-46e8-8800-c154f74047c8"),
                        _template.Id);
                else
                    template.ClearCachedTemplate(_template.Id);
            }
            else
                return "false";


            return retVal;
        }
    }
}
