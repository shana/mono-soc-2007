using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using umbraco.BasePages;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.workflow;

namespace umbraco.dialogs
{
    /// <summary>
    /// Summary description for cruds.
    /// </summary>
    public partial class notifications : UmbracoEnsuredPage
    {
        private ArrayList actions = new ArrayList();
        private CMSNode node;


        protected void Page_Load(object sender, EventArgs e)
        {
            Button1.Text = ui.Text("update");
            Header.Text = ui.Text("notifications");
            pageName.Text = ui.Text("notifications", "editNotifications", node.Text, base.getUser());

            // Put user code to initialize the page here
        }

        #region Web Form Designer generated code

        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);

            node = new cms.businesslogic.CMSNode(int.Parse(helper.Request("id")));

            HtmlTable ht = new HtmlTable();
            ht.CellPadding = 4;

            HtmlTableRow captions = new HtmlTableRow();
            HtmlTableRow checkboxes = new HtmlTableRow();
            ArrayList actionList = BusinessLogic.Actions.Action.GetAll();
            foreach (interfaces.IAction a in actionList)
            {
                if (a.ShowInNotifier)
                {
                    HtmlTableCell hc = new HtmlTableCell();
                    hc.Attributes.Add("class", "guiDialogTinyMark");
                    hc.Controls.Add(new LiteralControl(ui.Text("actions", a.Alias)));
                    captions.Cells.Add(hc);

                    // Checkbox
                    HtmlTableCell hcc = new HtmlTableCell();
                    CheckBox c = new CheckBox();
                    c.ID = a.Letter.ToString();
                    if (base.getUser().GetNotifications(node.Path).IndexOf(a.Letter) > -1)
                        c.Checked = true;

                    hcc.Controls.Add(c);
                    actions.Add(c);
                    checkboxes.Cells.Add(hcc);
                }
            }
            ht.Rows.Add(captions);
            ht.Rows.Add(checkboxes);
            PlaceHolder1.Controls.Add(ht);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        #endregion

        protected void Button1_Click(object sender, EventArgs e)
        {
            string notifications = "";

            // First off - load all users
            foreach (CheckBox c in actions)
            {
                // Update the user with the new permission
                if (c.Checked)
                    notifications += c.ID;
            }
            Notification.UpdateNotifications(base.getUser(), node, notifications);
            getUser().resetNotificationCache();
            base.getUser().initNotifications();


            // Update feedback message
            FeedBackMessage.Text = "<div class=\"feedbackCreate\">" + ui.Text("notifications") + " " + ui.Text("ok") +
                                   "</div>";
            TheForm.Visible = false;
        }
    }
}