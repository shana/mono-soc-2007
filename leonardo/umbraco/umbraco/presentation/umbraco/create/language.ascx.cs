using System;
using System.Collections;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.presentation.create;

namespace umbraco.cms.presentation.create.controls
{
    /// <summary>
    ///		Summary description for language.
    /// </summary>
    public partial class language : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            sbmt.Text = ui.Text("create");
            SortedList sortedCultures = new SortedList();
            Cultures.Items.Clear();
            Cultures.Items.Add(new ListItem(ui.Text("choose") + "...", ""));
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures))
                sortedCultures.Add(ci.DisplayName + "|||" + Guid.NewGuid().ToString(), ci.Name);

            IDictionaryEnumerator ide = sortedCultures.GetEnumerator();
            while (ide.MoveNext())
            {
                ListItem li = new ListItem(ide.Key.ToString().Substring(0, ide.Key.ToString().IndexOf("|||")), ide.Value.ToString());
                Cultures.Items.Add(li);
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
        }

        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        #endregion

        protected void sbmt_Click(object sender, EventArgs e)
        {
            dialogHandler_temp.Create(
                helper.Request("nodeType"),
                -1,
                Cultures.SelectedValue);

            Page.ClientScript.RegisterClientScriptBlock(GetType(), "refresh",
                                                        "<script>\nwindow.opener.refreshTree(false, true);\nwindow.close();</script>");
        }
    }
}