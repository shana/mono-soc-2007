using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using umbraco.BasePages;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.media;
using umbraco.cms.businesslogic.propertytype;
using umbraco.cms.businesslogic.web;
using umbraco.presentation.channels.businesslogic;
using umbraco.uicontrols;

namespace umbraco.cms.presentation.user
{
    /// <summary>
    /// Summary description for EditUser.
    /// </summary>
    public partial class EditUser : UmbracoEnsuredPage
    {
        protected HtmlTable macroProperties;
        protected TextBox uname = new TextBox();
        protected TextBox lname = new TextBox();
        protected PlaceHolder passw = new PlaceHolder();
        protected ListBox lapps = new ListBox();
        protected TextBox email = new TextBox();
        protected DropDownList userType = new DropDownList();
        protected DropDownList userLanguage = new DropDownList();
        protected CheckBox NoConsole = new CheckBox();
        protected CheckBox Disabled = new CheckBox();
        protected HtmlInputHidden mediaStartNode = new HtmlInputHidden();
        protected HtmlInputHidden startNode = new HtmlInputHidden();
        protected LiteralControl mediaTitleControl = new LiteralControl();
        protected LiteralControl contentTitleControl = new LiteralControl();

        protected TextBox cName = new TextBox();
        protected HtmlInputHidden cStartNode = new HtmlInputHidden();
        protected LiteralControl cContentTitleControl = new LiteralControl();
        protected CheckBox cFulltree = new CheckBox();
        protected DropDownList cDocumentType = new DropDownList();
        protected DropDownList cDescription = new DropDownList();
        protected DropDownList cCategories = new DropDownList();
        protected DropDownList cExcerpt = new DropDownList();
        protected HtmlInputHidden cMediaStartNode = new HtmlInputHidden();
        protected LiteralControl cMediaTitleControl = new LiteralControl();


        protected Pane pp = new Pane();

        private User u;


        protected void Page_Load(object sender, EventArgs e)
        {
            
            int UID = int.Parse(Request.QueryString["id"]);
            u = BusinessLogic.User.GetUser(UID);

            mediaStartNode.ID = "mediaStartNode";
            startNode.ID = "startNode";

            // Populate usertype list
            foreach (UserType ut in UserType.getAll)
            {
                ListItem li = new ListItem(ui.Text("user", ut.Name.ToLower(), base.getUser()), ut.Id.ToString());
                if (ut.Id == u.UserType.Id)
                    li.Selected = true;

                userType.Items.Add(li);
            }

            // Populate ui language lsit
            foreach (
                string f in
                    Directory.GetFiles(HttpContext.Current.Server.MapPath(GlobalSettings.Path + "/config/lang"), "*.xml")
                )
            {
                XmlDocument x = new XmlDocument();
                x.Load(f);
                ListItem li =
                    new ListItem(x.DocumentElement.Attributes.GetNamedItem("intName").Value,
                                 x.DocumentElement.Attributes.GetNamedItem("alias").Value);
                if (x.DocumentElement.Attributes.GetNamedItem("alias").Value == u.Language)
                    li.Selected = true;

                userLanguage.Items.Add(li);
            }

            // Console access and disabling
            if (u.NoConsole)
                NoConsole.Checked = true;
            else
                NoConsole.Checked = false;
            if (u.Disabled)
                Disabled.Checked = true;
            else
                Disabled.Checked = false;

            PlaceHolder medias = new PlaceHolder();
            if (u.StartMediaId > 0)
            {
                try
                {
                    mediaTitleControl.Text = new Media(u.StartMediaId).Text;
                }
                catch
                {
                }
            }
            else if (u.StartMediaId == -1)
                mediaTitleControl.Text = ui.Text("media", base.getUser());

            medias.Controls.Add(mediaStartNode);
            medias.Controls.Add(new LiteralControl("<span id=\"mediaStartNodeTitle\">"));
            medias.Controls.Add(mediaTitleControl);
            medias.Controls.Add(
                new LiteralControl("</span> <a href=\"javascript:updateMediaId('media')\">" + ui.Text("choose", base.getUser()) +
                                   "...</a>"));

            PlaceHolder content = new PlaceHolder();
            if (u.StartNodeId > 0)
            {
                try
                {
                    contentTitleControl.Text = new Document(u.StartNodeId).Text;
                }
                catch
                {
                }
            }
            else if (u.StartNodeId == -1)
                contentTitleControl.Text = ui.Text("content", base.getUser());

            content.Controls.Add(startNode);
            content.Controls.Add(new LiteralControl("<span id=\"startNodeTitle\">"));
            content.Controls.Add(contentTitleControl);
            content.Controls.Add(
                new LiteralControl("</span> <a href=\"javascript:updateContentId('')\">" +
                                   ui.Text("choose", base.getUser()) + "...</a>"));


            // Add password changer
            passw.Controls.Add(new UserControl().LoadControl(GlobalSettings.Path + "/controls/passwordChanger.ascx"));
            
            pp.addProperty(ui.Text("user", "username", base.getUser()), uname);
            pp.addProperty(ui.Text("user", "loginname", base.getUser()), lname);
            pp.addProperty(ui.Text("user", "password", base.getUser()), passw);
            pp.addProperty(ui.Text("email", base.getUser()), email);
            pp.addProperty(ui.Text("user", "usertype", base.getUser()), userType);
            pp.addProperty(ui.Text("user", "language", base.getUser()), userLanguage);

            Pane ppNodes = new Pane();

            ppNodes.addProperty(ui.Text("user", "startnode", base.getUser()), content);
            ppNodes.addProperty(ui.Text("user", "mediastartnode", base.getUser()), medias);

            Pane ppAccess = new Pane();
            ppAccess.addProperty(ui.Text("user", "noConsole", base.getUser()), NoConsole);
            ppAccess.addProperty(ui.Text("user", "disabled", base.getUser()), Disabled);

            Pane ppModules = new Pane();
            ppModules.addProperty(ui.Text("user", "modules", base.getUser()), lapps);

            TabPage userInfo = UserTabs.NewTabPage(u.Name);

            userInfo.Controls.Add(pp);
            userInfo.Controls.Add(ppNodes);
            userInfo.Controls.Add(ppAccess);
            userInfo.Controls.Add(ppModules);
            userInfo.Style.Add("text-align", "center");

            userInfo.HasMenu = true;
            ImageButton save = userInfo.Menu.NewImageButton();
            save.ImageUrl = GlobalSettings.Path + "/images/editor/save.gif";
            save.Click += new ImageClickEventHandler(saveUser_Click);

            setupForm();
            setupChannel();
        }

        private void setupChannel()
        {
            Channel userChannel;
            try
            {
                userChannel =
                    new Channel(u.Id);
            }
            catch
            {
                userChannel = new Channel();
            }

            // Populate dropdowns
            foreach (DocumentType dt in DocumentType.GetAll)
                cDocumentType.Items.Add(
                    new ListItem(dt.Text, dt.Alias)
                    );

            // populate fields
            ArrayList fields = new ArrayList();
            cDescription.ID = "cDescription";
            cCategories.ID = "cCategories";
            cExcerpt.ID = "cExcerpt";
            cDescription.Items.Add(new ListItem(ui.Text("choose"), ""));
            cCategories.Items.Add(new ListItem(ui.Text("choose"), ""));
            cExcerpt.Items.Add(new ListItem(ui.Text("choose"), ""));

            foreach (PropertyType pt in PropertyType.GetAll())
            {
                if (!fields.Contains(pt.Alias))
                {
                    cDescription.Items.Add(new ListItem(string.Format("{0} ({1})", pt.Name, pt.Alias), pt.Alias));
                    cCategories.Items.Add(new ListItem(string.Format("{0} ({1})", pt.Name, pt.Alias), pt.Alias));
                    cExcerpt.Items.Add(new ListItem(string.Format("{0} ({1})", pt.Name, pt.Alias), pt.Alias));
                    fields.Add(pt.Alias);
                }
            }

            // Handle content and media pickers
            cMediaStartNode.ID = "cmediaStartNode";
            cStartNode.ID = "cstartNode";
            PlaceHolder medias = new PlaceHolder();
            if (userChannel.MediaFolder > 0)
            {
                try
                {
                    cMediaTitleControl.Text = new Media(userChannel.MediaFolder).Text;
                }
                catch
                {
                }
            }
            else if (userChannel.MediaFolder == -1)
                cMediaTitleControl.Text = ui.Text("media", base.getUser());

            medias.Controls.Add(cMediaStartNode);
            medias.Controls.Add(new LiteralControl("<span id=\"cmediaStartNodeTitle\">"));
            medias.Controls.Add(cMediaTitleControl);
            medias.Controls.Add(
                new LiteralControl("</span> <a href=\"javascript:updateMediaId('cmedia')\">" +
                                   ui.Text("choose", base.getUser()) + "...</a>"));

            PlaceHolder content = new PlaceHolder();
            if (userChannel.StartNode > 0)
            {
                try
                {
                    cContentTitleControl.Text = new Document(userChannel.StartNode).Text;
                }
                catch
                {
                }
            }
            else if (userChannel.StartNode == -1)
                cContentTitleControl.Text = ui.Text("content", base.getUser());

            content.Controls.Add(cStartNode);
            content.Controls.Add(new LiteralControl("<span id=\"cstartNodeTitle\">"));
            content.Controls.Add(cContentTitleControl);
            content.Controls.Add(
                new LiteralControl("</span> <a href=\"javascript:updateContentId('c')\">" +
                                   ui.Text("choose", base.getUser()) + "...</a>"));


            // Setup the panes
            Pane ppInfo = new Pane();
            ppInfo.addProperty("Name", cName);
            ppInfo.addProperty(ui.Text("user", "startnode", base.getUser()), content);
            ppInfo.addProperty("Search all children", cFulltree);
            ppInfo.addProperty(ui.Text("user", "mediastartnode", base.getUser()), medias);

            Pane ppFields = new Pane();
            ppFields.addProperty("Document Type", cDocumentType);
            ppFields.addProperty("Description field", cDescription);
            ppFields.addProperty("Category Field", cCategories);
            ppFields.addProperty("Excerpt Field", cExcerpt);


            TabPage channelInfo = UserTabs.NewTabPage("Content Channel");

            channelInfo.Controls.Add(ppInfo);
            channelInfo.Controls.Add(ppFields);

            channelInfo.HasMenu = true;
            ImageButton save = channelInfo.Menu.NewImageButton();
            save.ImageUrl = GlobalSettings.Path + "/images/editor/save.gif";
            save.Click += new ImageClickEventHandler(saveUser_Click);

            if (!IsPostBack)
            {
                cName.Text = userChannel.Name;
                cDescription.SelectedValue = userChannel.FieldDescriptionAlias;
                cCategories.SelectedValue = userChannel.FieldCategoriesAlias;
                cExcerpt.SelectedValue = userChannel.FieldExcerptAlias;
                cDocumentType.SelectedValue = userChannel.DocumentTypeAlias;
                cStartNode.Value = userChannel.StartNode.ToString();
                cMediaStartNode.Value = userChannel.MediaFolder.ToString();
                cFulltree.Checked = userChannel.FullTree;
            }
        }

        private void setupForm()
        {

            if (!IsPostBack)
            {
                uname.Text = u.Name;
                lname.Text = u.LoginName;
//                passw.Text = u.Password;
                email.Text = u.Email;
                startNode.Value = u.StartNodeId.ToString();
                mediaStartNode.Value = u.StartMediaId.ToString();

            }

            Application[] uapps = u.Applications;
            if (!Page.IsPostBack)
                foreach (Application app in BusinessLogic.Application.getAll())
                {
                    ListItem li = new ListItem(ui.Text("sections", app.alias), app.alias);
                    if (!IsPostBack) foreach (Application tmp in uapps) if (app.alias == tmp.alias) li.Selected = true;
                    lapps.Items.Add(li);
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lapps.SelectionMode = ListSelectionMode.Multiple;
            lapps.Width = 350;
            lapps.Height = 200;
        }

        #endregion

        private void saveUser_Click(object sender, ImageClickEventArgs e)
        {
            u.Name = uname.Text;
            u.LoginName = lname.Text;

            // Check if password should be changed
            string tempPassword = ((controls.passwordChanger) passw.Controls[0]).Password;
            if (tempPassword.Trim() != "")
                u.Password = tempPassword;

            u.Email = email.Text;
            u.StartNodeId = int.Parse(startNode.Value);
            u.Language = userLanguage.SelectedValue;
            u.UserType = UserType.GetUserType(int.Parse(userType.SelectedValue));
            u.Disabled = Disabled.Checked;
            u.NoConsole = NoConsole.Checked;
            u.StartMediaId = int.Parse(mediaStartNode.Value);
            u.clearApplications();

            foreach (ListItem li in lapps.Items)
            {
                if (li.Selected) u.addApplication(li.Value);
            }

            // Update page titles
            if (u.StartNodeId > 0)
                try
                {
                    contentTitleControl.Text = new Document(u.StartNodeId).Text;
                }
                catch
                {
                }
            else if (u.StartNodeId == -1)
                contentTitleControl.Text = ui.Text("content", base.getUser());

            if (u.StartMediaId > 0)
                try
                {
                    mediaTitleControl.Text = new Media(u.StartMediaId).Text;
                }
                catch
                {
                }
            else if (u.StartMediaId == -1)
                mediaTitleControl.Text = ui.Text("media", base.getUser());

            // save data
            if (cName.Text != "")
            {
                Channel c;
                try
                {
                    c = new Channel(u.Id);
                } catch
                {
                    c = new Channel();
                    c.User = u;
                }

                c.Name = cName.Text;
                c.FullTree = cFulltree.Checked;
                c.StartNode = int.Parse(cStartNode.Value);
                c.MediaFolder = int.Parse(cMediaStartNode.Value);
                c.FieldCategoriesAlias = cCategories.SelectedValue;
                c.FieldDescriptionAlias = cDescription.SelectedValue;
                c.FieldExcerptAlias = cExcerpt.SelectedValue;
                c.DocumentTypeAlias = cDocumentType.SelectedValue;

                //
                c.MediaTypeAlias = "image";
                c.MediaTypeFileProperty = "umbracoFile";
                c.ImageSupport = true;

                c.Save();

                // Update page titles
                if (c.StartNode > 0)
                    try
                    {
                        cContentTitleControl.Text = new Document(c.StartNode).Text;
                    }
                    catch
                    {
                    }
                else if (c.StartNode == -1)
                    cContentTitleControl.Text = ui.Text("content", base.getUser());

                if (c.MediaFolder > 0)
                    try
                    {
                        cMediaTitleControl.Text = new Media(c.MediaFolder).Text;
                    }
                    catch
                    {
                    }
                else if (c.MediaFolder == -1)
                    cMediaTitleControl.Text = ui.Text("media", base.getUser());

            }

            speechBubble(speechBubbleIcon.save, ui.Text("speechBubbles", "editUserSaved", base.getUser()), "");
        }
    }
}