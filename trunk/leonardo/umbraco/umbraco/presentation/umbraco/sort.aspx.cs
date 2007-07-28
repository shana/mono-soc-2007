using System;
using System.Collections;
using System.Web.UI.WebControls;
using System.Xml;
using umbraco.BasePages;
using umbraco.BusinessLogic.Actions;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.media;
using umbraco.cms.businesslogic.web;

namespace umbraco.cms.presentation
{
    /// <summary>
    /// Summary description for sort.
    /// </summary>
    public partial class sort : UmbracoEnsuredPage
    {
        private int parentId;

        protected void Page_Load(object sender, EventArgs e)
        {
            parentId = int.Parse(Request.QueryString["id"]);


            if (Request.QueryString["save"] != null)
            {
                string tmpVal = Request.QueryString["save"];
                if (tmpVal.Trim().Length > 0)
                {
                    string[] tmp = tmpVal.Split(',');

                    for (int i = 0; i < tmp.Length; i++)
                    {
                        new CMSNode(int.Parse(tmp[i])).sortOrder = i;
                        if (helper.Request("app") == "content")
                        {
                            Document d = new Document(int.Parse(tmp[i]));
                            // refresh the xml for the sorting to work
                            if (d.Published)
                            {
                                d.refreshXmlSortOrder();
                                library.PublishSingleNode(int.Parse(tmp[i]));
                            }
                        }
                    }

                    // Republish
                    if (helper.Request("app") == "content")
                    {
                        // Re-sort
                        XmlNode n;
                        if (parentId > 0)
                            n = content.Instance.XmlContent.GetElementById(parentId.ToString());
                        else
                            n = content.Instance.XmlContent.DocumentElement;

                        content.SortNodes(ref n);

                        // Run ActionHandler
                        if (parentId > 0)
                            Action.RunActionHandlers(new Document(parentId), new ActionSort());
                    }
                }

                ClientScript.RegisterClientScriptBlock(GetType(), "close",
                                                       "<script>window.top.opener.refreshTree(false,true);window.top.close();</script>");
            }

            sortDone.Text = ui.Text("sortDone");
            help.Text = ui.Text("sortHelp");

        }

        private void populateListBox()
        {
            /*
            ListBox1.Items.Clear();
            ArrayList tmp = new ArrayList();

            if (parentId > 1)
            {
                if (helper.Request("app") == "settings")
                {
                    // stylesheets
                    foreach (StylesheetProperty p in new StyleSheet(parentId).Properties)
                    {
                        tmp.Add(new ListItem(p.Text, p.Id.ToString()));
                    }
                }
                else
                {
                    foreach (Document d in new Document(parentId).Children)
                    {
                        tmp.Add(new ListItem(d.Text, d.Id.ToString()));
                    }
                }
            }
            else
            {
                //get all topmost nodes
                if (helper.Request("app") == "media")
                {
                    foreach (Media m in  Media.GetRootMedias())
                    {
                        tmp.Add(new ListItem(m.Text, m.Id.ToString()));
                    }
                }
                else
                {
                    foreach (Document d in  Document.GetRootDocuments())
                    {
                        tmp.Add(new ListItem(d.Text, d.Id.ToString()));
                    }
                }
            }

            for (int i = 0; i < tmp.Count; i++)
            {
                ListBox1.Items.Add((ListItem) tmp[i]);
            }
             */
        }

        #region Web Form Designer generated code

        protected override void OnInit(EventArgs e)
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