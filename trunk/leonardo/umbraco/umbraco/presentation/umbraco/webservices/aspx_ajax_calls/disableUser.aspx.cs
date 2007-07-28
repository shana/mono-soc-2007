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

namespace umbraco.presentation.webservices.aspx_ajax_calls
{
    public partial class disableUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (BasePages.BasePage.ValidateUserContextID(BasePages.BasePage.umbracoUserContextID))
                BusinessLogic.User.GetUser(int.Parse(helper.Request("id"))).Disabled = true;
        }
    }
}
