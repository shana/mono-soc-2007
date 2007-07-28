using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using umbraco.cms.businesslogic.web;
using umbraco.cms.businesslogic;
using umbraco.BusinessLogic;

namespace umbraco.presentation.dialogs
{
    public partial class sendToTranslation : BasePages.UmbracoEnsuredPage
    {
        private CMSNode _currentPage;

        protected void Page_Load(object sender, EventArgs e)
        {
            _currentPage = new cms.businesslogic.CMSNode(int.Parse(helper.Request("id")));

            if (!IsPostBack)
            {
                nodeName.Text = _currentPage.Text;


                // default language
                int selectedLanguage = 0;

                Domain[] domains = library.GetCurrentDomains(_currentPage.Id);
                if (domains != null)
                {
                    selectedLanguage = domains[0].Language.id;
                    defaultLanguage.Text = ui.Text("defaultLanguageIs", base.getUser()) + " " + domains[0].Language.FriendlyName;
                }
                else
                {
                    defaultLanguage.Text = ui.Text("defaultLanguageIsNotAssigned", base.getUser());
                }


                // languages
                language.Items.Add(new ListItem(ui.Text("general", "choose", base.getUser()), ""));
                foreach (cms.businesslogic.language.Language l in cms.businesslogic.language.Language.getAll)
                {
                    ListItem li = new ListItem();
                    li.Text = l.FriendlyName;
                    li.Value = l.id.ToString();
                    if (selectedLanguage == l.id)
                        li.Selected = true;
                    language.Items.Add(li);
                }

                // Subpages
                if (_currentPage.Children.Length == 0)
                    includeSubpages.Enabled = false;

                // Translators
                foreach (User u in BusinessLogic.User.getAll())
                    if (u.UserType.Alias.ToLower() == "translator")
                        translator.Items.Add(new ListItem(u.Name, u.Id.ToString()));

                // send button
                doTranslation.Text = ui.Text("general", "ok", base.getUser());
            }
        }

        protected void doTranslation_Click(object sender, EventArgs e)
        {
            // testing translate
            cms.businesslogic.translation.Translation.MakeNew(
                _currentPage,
                getUser(),
                BusinessLogic.User.GetUser(int.Parse(translator.SelectedValue)),
                new cms.businesslogic.language.Language(int.Parse(language.SelectedValue)),
                comment.Text, includeSubpages.Checked,
                true);

            TheForm.Visible = false;
            FeedBackMessage.Text = "<div class=\"feedbackCreate\">" + ui.Text("pageHasBeenSendToTranslation") + "</div>";
        }
    }
}
