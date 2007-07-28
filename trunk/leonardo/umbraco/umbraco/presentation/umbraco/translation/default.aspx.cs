using System;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using umbraco.BasePages;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.propertytype;
using umbraco.cms.businesslogic.task;
using umbraco.cms.businesslogic.translation;
//using umbraco.cms.businesslogic.utilities;
using umbraco.cms.businesslogic.web;

using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.GZip;


namespace umbraco.presentation.translation
{
    public partial class _default : UmbracoEnsuredPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Panel2.Text = "Translation tasks for " + base.getUser().Name;

            if (Request["view"] == "details")
            {
                taskView.Visible = false;
                details.Visible = true;

                int translationId = int.Parse(Request["id"]);
                Task t = new Task(translationId);
                Document page = new Document(t.Node.Id);
                currentPage.Text = page.Text;

                // Page Details
                DataTable translationTable = new DataTable();
                translationTable.Columns.Add("Id");
                translationTable.Columns.Add("Date");
                translationTable.Columns.Add("NodeId");
                translationTable.Columns.Add("NodeName");
                translationTable.Columns.Add("ReferingUser");
                translationTable.Columns.Add("Comments");
                translationTable.Columns.Add("Words");

                DataRow task = translationTable.NewRow();
                task["Id"] = t.Id;
                task["Date"] = t.Date;
                task["NodeId"] = t.Node.Id;
                task["NodeName"] = t.Node.Text;
                task["ReferingUser"] = t.ParentUser.Name;
                task["Comments"] = t.Comment;
                task["Words"] = Translation.CountWords(t.Node.Id);
                translationTable.Rows.Add(task);
                translationDetails.DataSource = translationTable;
                translationDetails.DataBind();

                DataTable pageTable = new DataTable();

                pageTable.Columns.Add("PageName");
                foreach (PropertyType pt in page.ContentType.PropertyTypes)
                    pageTable.Columns.Add(pt.Name);

                DataRow pageRow = pageTable.NewRow();
                pageRow["PageName"] = page.Text;
                foreach (PropertyType pt in page.ContentType.PropertyTypes)
                    pageRow[pt.Name] = page.getProperty(pt.Alias).Value;
                pageTable.Rows.Add(pageRow);

                pageDetails.DataSource = pageTable;
                pageDetails.DataBind();
            }
            else
            {
                DataTable tasks = new DataTable();
                tasks.Columns.Add("Id");
                tasks.Columns.Add("Date");
                tasks.Columns.Add("NodeId");
                tasks.Columns.Add("NodeName");
                tasks.Columns.Add("ReferingUser");
                tasks.Columns.Add("Language");

                foreach (Task t in Task.GetTasks(base.getUser(), false))
                {
                    if (t.Type.Alias == "toTranslate")
                    {
                        DataRow task = tasks.NewRow();
                        task["Id"] = t.Id;
                        task["Date"] = t.Date;
                        task["NodeId"] = t.Node.Id;
                        task["NodeName"] = t.Node.Text;
                        task["ReferingUser"] = t.ParentUser.Name;
                        tasks.Rows.Add(task);
                    }
                }
                taskList.DataSource = tasks;
                taskList.DataBind();
            }
        }

        protected void uploadFile_Click(object sender, EventArgs e)
        {
            // Save temp file
            if (translationFile.PostedFile != null)
            {
                string tempFileName;
                if (translationFile.PostedFile.FileName.ToLower().Contains(".zip"))
                    tempFileName =
                        Server.MapPath(GlobalSettings.Path + Path.DirectorySeparatorChar + ".." +
                                       Path.DirectorySeparatorChar + "data" +
                                       Path.DirectorySeparatorChar + "translationFile_" + Guid.NewGuid().ToString() +
                                       ".zip");
                else
                    tempFileName =
                        Server.MapPath(GlobalSettings.Path + Path.DirectorySeparatorChar + ".." +
                                       Path.DirectorySeparatorChar + "data" +
                                       Path.DirectorySeparatorChar + "translationFile_" + Guid.NewGuid().ToString() +
                                       ".xml");

                translationFile.PostedFile.SaveAs(tempFileName);

                // xml or zip file
                if (new FileInfo(tempFileName).Extension.ToLower() == ".zip")
                {
                    // Zip Directory
                    string tempPath =
                        Server.MapPath(GlobalSettings.Path + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "data" +
                                       Path.DirectorySeparatorChar + "translationFiles_" + Guid.NewGuid().ToString());


                    // Add the path to the zipfile to viewstate
                    ViewState.Add("zipFile", tempPath);

                    // Unpack the zip file

                    cms.businesslogic.utilities.Zip.UnPack(tempFileName, tempPath, true);

                    // Test the number of xml files
                    try
                    {
                        zipContents.Text = new DirectoryInfo(tempPath).GetFiles().Length.ToString();
                    }
                    catch (Exception ex) {
                        zipContents.Text = ex.ToString();
                    }

                    // Toggle panels
                    panelZipFile.Visible = true;
                    uploadStatus.Visible = false;
                    taskView.Visible = false;
                    details.Visible = false;
                }
                else
                {
                    Task t = importTranslatationFile(tempFileName);

                    // Update UI
                    translationFileId.Text = t.Node.Id.ToString();
                    translationFilePage.Text = t.Node.Text;
                    translationFilePreviewLink.Text = "<a href=\"preview.aspx?id=" + t.Id + "\">";
                    translationFilePreviewLinkEnd.Text = "</a>";

                    // Toggle panels
                    uploadStatus.Visible = true;
                    taskView.Visible = false;
                    details.Visible = false;
                }

                // clean up
                File.Delete(tempFileName);
            }
        }

        private Task importTranslatationFile(string tempFileName)
        {
            try
            {
                // open file
                XmlDocument tf = new XmlDocument();
                tf.XmlResolver = null;
                tf.Load(tempFileName);

                // Get task xml node
                XmlNode taskXml = tf.SelectSingleNode("/task");
                XmlNode taskNode = taskXml.SelectSingleNode("node");

                // validate file
                Task t = new Task(int.Parse(taskXml.Attributes.GetNamedItem("Id").Value));
                if (t != null)
                {
                    if (t.Node.Id == int.Parse(taskNode.Attributes.GetNamedItem("id").Value))
                    {
                        // update node contents
                        Document d = new Document(t.Node.Id);
                        d.Text = taskNode.Attributes.GetNamedItem("nodeName").Value.Trim();

                        // update data elements
                        foreach (XmlNode data in taskNode.SelectNodes("data"))
                            if (data.FirstChild != null)
                                d.getProperty(data.Attributes.GetNamedItem("alias").Value).Value =
                                    data.FirstChild.Value;
                            else
                                d.getProperty(data.Attributes.GetNamedItem("alias").Value).Value =
                                    "";


                        // add log entry
                        Log.Add(LogTypes.Save, getUser(), t.Node.Id, "Updated via translation file");

                        // Close task?
                        if (closeTask.Checked)
                        {
                            t.Closed = true;
                            t.Save();
                        }

                        return t;
                    }
                }
                else
                    throw new Exception("Task with id: '" + taskXml.Attributes.GetNamedItem("Id").Value + "' not found");
            }
            catch (Exception ee)
            {
                throw new Exception("Error importing translation file '" + tempFileName + "': " + ee.ToString());
            }

            return null;
        }

        protected void zipYes_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (FileInfo translationFileXml in new DirectoryInfo(ViewState["zipFile"].ToString()).GetFiles("*.xml"))
            {
                try
                {
                    Task translation =
                        importTranslatationFile(translationFileXml.FullName);
                    sb.Append("<li>" + translation.Node.Text + " <a href=\"preview.aspx?id=" + translation.Id +
                              "\">Preview</a></li>");
                }
                catch (Exception ee)
                {
                    sb.Append("<li style=\"color: red;\">" + ee.ToString() + "</li>");
                }
            }

            zipStatus.Text = sb.ToString();

            // Toggle panels
            panelZipSuccess.Visible = true;
            panelZipFile.Visible = false;
            uploadStatus.Visible = false;
            taskView.Visible = false;
            details.Visible = false;
        }
    }
}