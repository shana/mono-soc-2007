    using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Reflection;
using Microsoft.ApplicationBlocks.Data;
using System.Data.SqlClient;

namespace umbraco.cms.presentation
{
	public partial class editContent : BasePages.UmbracoEnsuredPage
	{
		protected uicontrols.TabView TabView1;
		protected System.Web.UI.WebControls.TextBox documentName;
		private cms.businesslogic.web.Document _document;
		protected System.Web.UI.WebControls.Literal jsIds;
		

		private controls.datePicker dp = new controls.datePicker();
		private controls.datePicker dpRelease = new controls.datePicker();
		private controls.datePicker dpExpire = new controls.datePicker();

		controls.ContentControl tmp;

		DropDownList ddlDefaultTemplate = new DropDownList();

		uicontrols.Pane publishProps = new uicontrols.Pane();
		uicontrols.Pane linkProps = new uicontrols.Pane();

		Button UnPublish = new Button();
		private Literal littPublishStatus = new Literal();

		private Literal l = new Literal();
		private Literal domainText = new Literal();

		protected System.Web.UI.WebControls.Literal SyncPath;


		private controls.ContentControl.publishModes _canPublish = controls.ContentControl.publishModes.Publish;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            // Validate permissions
            if (!base.ValidateUserApp("content"))
                throw new ArgumentException("The current user doesn't have access to this application. Please contact the system administrator.");
            if (!base.ValidateUserNodeTreePermissions(_document.Path, "A"))
                throw new ArgumentException("The current user doesn't have permissions to edit this document. Please contact the system administrator.");

			if (helper.Request("frontEdit") != "")
				syncScript.Visible = false;

			if (!IsPostBack) 
			{
				SyncPath.Text = _document.Path;
				BusinessLogic.Log.Add(BusinessLogic.LogTypes.Open, base.getUser(), _document.Id, "");
			}

			jsIds.Text = "var umbPageId = " + _document.Id.ToString() + ";\nvar umbVersionId = '" + _document.Version.ToString() + "';\n";

			// Shortcut keys
			//Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "umbShortcutKeys", "<script src=\"js/umbracoCheckKeys.js\" type=\"text/javascript\"></script>");
			
		}

		protected void Save(object sender, System.EventArgs e) 
		{

			// error handling test
			if (!Page.IsValid) 
			{
				foreach(uicontrols.TabPage tp in tmp.GetPanels()) 
				{
					ValidationSummary vs = new ValidationSummary();
					vs.ShowSummary = true;
					vs.ID = Guid.NewGuid().ToString();

					// Add error pane
					Control tempErr = new Control();
					tempErr.Controls.Add(new LiteralControl("<div id=\"errorPane_" + tp.ClientID + "\" style=\"display: none; text-align: left; color: red;width: 100%; border: 1px solid red; padding: 5px; background-color: #F2E2E2\"><div><div style=\"float: right;\"><a href=\"#\" onClick=\"javascript:document.getElementById('errorPane_" + tp.ClientID + "').style.display = 'none'; return false;\"><img src=\"images/speechbubble_close.gif\" style=\"border: none; width: 18px; height: 18px;\" alt=\"" + ui.Text("close") + "\"/></a></div><b>" + ui.Text("errorHandling", "errorHeader") + "</b><br/>"));
					tempErr.Controls.Add(vs);
					tempErr.Controls.Add(new LiteralControl("</div></div>"));
					tp.Controls.AddAt(0,tempErr);
					tp.Controls.Add(new LiteralControl("<script>\n setTimeout(\"new Effect2.Appear('errorPane_" + tp.ClientID + "');\", 500);\n</script>"));
				}
			} 


			BusinessLogic.Log.Add(BusinessLogic.LogTypes.Save, base.getUser(), _document.Id, "");

			// Update name
			_document.Text = tmp.NameTxt.Text;

			// Update the update date
			_document.UpdateDate = dp.DateTime;
			if (dpRelease.DateTime > new DateTime(1753, 1, 1) && dpRelease.DateTime < new DateTime(9999, 12, 31))
				_document.ReleaseDate = dpRelease.DateTime;
			else
				_document.ReleaseDate = new DateTime(1,1,1,0,0,0);
			if (dpExpire.DateTime > new DateTime(1753, 1, 1) && dpExpire.DateTime < new DateTime(9999, 12, 31))
				_document.ExpireDate = dpExpire.DateTime;
			else
				_document.ExpireDate = new DateTime(1,1,1,0,0,0);

			// Update default template
			if (ddlDefaultTemplate.SelectedIndex > 0) 
			{
				_document.Template = int.Parse(ddlDefaultTemplate.SelectedValue);
			}

			// Run Handler				
			BusinessLogic.Actions.Action.RunActionHandlers(_document, new BusinessLogic.Actions.ActionUpdate());

			if (!tmp.DoesPublish)
				this.speechBubble(speechBubbleIcon.save, ui.Text("speechBubbles", "editContentSavedHeader", null), ui.Text("speechBubbles", "editContentSavedText", null)); 

		}

		protected void SendToPublish(object sender, System.EventArgs e) 
		{
			if (Page.IsValid) 
			{
				this.speechBubble(speechBubbleIcon.save, ui.Text("speechBubbles", "editContentSendToPublish", base.getUser()), ui.Text("speechBubbles", "editContentSendToPublishText", base.getUser())); 
				BusinessLogic.Log.Add(BusinessLogic.LogTypes.SendToPublish, base.getUser(), _document.Id, "");

				// Run Handler				
				BusinessLogic.Actions.Action.RunActionHandlers(_document, new BusinessLogic.Actions.ActionToPublish());
			}
		}

		protected void Publish(object sender, System.EventArgs e) 
		{
			if (Page.IsValid) 
			{
				if (_document.Level == 1 || new cms.businesslogic.web.Document(_document.Parent.Id).Published) 
				{
                    Trace.Warn("before d.publish");

					_document.Publish(base.getUser());
                    Trace.Warn("after d.publish");
					this.speechBubble(speechBubbleIcon.save, ui.Text("speechBubbles", "editContentPublishedHeader", null), ui.Text("speechBubbles", "editContentPublishedText", null));
                    Trace.Warn("Before library publish single node");
                    //library.PublishSingleNode(_document.Id);
                    content.Instance.PublishNode(_document);
                    Trace.Warn("After library publish single node");


					BusinessLogic.Log.Add(BusinessLogic.LogTypes.Publish, base.getUser(), _document.Id, "");
					this.reloadParentNode();
					littPublishStatus.Text = ui.Text("content", "lastPublished", base.getUser()) + ": " + _document.VersionDate.ToString() + "<br/>";
					if (base.getUser().GetPermissions(_document.Path).IndexOf("U") > -1)
						UnPublish.Visible = true;
					updateLinks();
				} 
				else
					this.speechBubble(speechBubbleIcon.error, ui.Text("error"), ui.Text("speechBubbles", "editContentPublishedFailedByParent"));



				// page cache disabled...
				//			cms.businesslogic.cache.Cache.ClearCacheObjectTypes("umbraco.page");


				// Update links
			}
		}

		protected void UnPublishDo(object sender, System.EventArgs e) 
		{
			_document.UnPublish();
			littPublishStatus.Text = ui.Text("content", "itemNotPublished", base.getUser());
			UnPublish.Visible = false;

			library.UnPublishSingleNode(_document.Id);
		}

		private void updateLinks() 
		{
			if (_document.Published) 
			{
				l.Text = "<a href=\"" + library.NiceUrl(_document.Id) + "\" target=\"_blank\">" + library.NiceUrl(_document.Id) + "</a>";

				string currentLink = library.NiceUrl(_document.Id);

				// domains
				domainText.Text = "";
				foreach (string s in _document.Path.Split(','))
				{
					if (int.Parse(s) > -1) 
					{
						cms.businesslogic.web.Document dChild = new cms.businesslogic.web.Document(int.Parse(s));
						if (dChild.Published) 
						{
							cms.businesslogic.web.Domain[] domains = cms.businesslogic.web.Domain.GetDomainsById(int.Parse(s));
							if (domains.Length > 0) 
							{
								for (int i=0; i<domains.Length;i++) 
								{
									string tempLink = "";
                                    if (library.NiceUrl(int.Parse(s)) == "")
                                        tempLink = "<em>N/A</em>";
                                    else if (int.Parse(s) != _document.Id)
                                        tempLink = "http://" + domains[i].Name + currentLink.Replace(library.NiceUrl(int.Parse(s)).Replace(".aspx", ""), "");
                                    else
                                        tempLink = "http://" + domains[i].Name;

									domainText.Text += "<a href=\"" + tempLink + "\" target=\"_blank\">" + tempLink + "</a><br/>";
								}
							}
						} 
						else
							l.Text = "<i>" + ui.Text("content", "parentNotPublished", dChild.Text, base.getUser()) + "</i>";
					}
				}

			}
			else
				l.Text = "<i>" + ui.Text("content", "itemNotPublished", base.getUser()) + "</i>";
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			base.OverrideClientTarget = false;

			InitializeComponent();
			base.OnInit(e);	

			_document = new cms.businesslogic.web.Document(int.Parse(Request.QueryString["id"]));

			// Check publishing permissions
			if (base.getUser().UserType.Name.ToLower() == "writer") 
				_canPublish = controls.ContentControl.publishModes.SendToPublish;
			tmp = new controls.ContentControl(_document, _canPublish, "TabView1");
			tmp.ID = "TabView1";
			
			tmp.Width = Unit.Pixel(666);
			tmp.Height = Unit.Pixel(666);

			// Add preview button

			foreach(uicontrols.TabPage tp in tmp.GetPanels()) 
			{
				tp.Menu.InsertSplitter(2);
				uicontrols.MenuIconI menuItem = tp.Menu.NewIcon(3);
				menuItem.AltText = ui.Text("buttons", "showPage", this.getUser());
				menuItem.OnClickCommand = "window.open('../" + Request["id"] + ".aspx?umbVersion=" + _document.Version.ToString() + "','umbPreview')";
				menuItem.ImageURL = GlobalSettings.Path + "/images/editor/vis.gif";
//				tp.Menu.InsertSplitter(4);
			}
			
			plc.Controls.Add(tmp);


			System.Web.UI.WebControls.PlaceHolder publishStatus = new PlaceHolder();
			if (_document.Published) 
			{
				littPublishStatus.Text = ui.Text("content", "lastPublished", base.getUser()) + ": " + _document.VersionDate.ToString() + "<br/>";
				publishStatus.Controls.Add(littPublishStatus);
				if (base.getUser().GetPermissions(_document.Path).IndexOf("U") > -1) 
					UnPublish.Visible = true;
				else
					UnPublish.Visible = false;
			} 
			else 
			{
				littPublishStatus.Text = ui.Text("content", "itemNotPublished", base.getUser());
				publishStatus.Controls.Add(littPublishStatus);
				UnPublish.Visible = false;
			}
			
			UnPublish.Text = ui.Text("content", "unPublish", base.getUser());
			UnPublish.ID = "UnPublishButton";
			UnPublish.Attributes.Add("onClick", "if (!confirm('" + ui.Text("defaultdialogs", "confirmSure", base.getUser()) + "')) return false; ");
			publishStatus.Controls.Add(UnPublish);

			publishProps.addProperty(ui.Text("content", "publishStatus", base.getUser()), publishStatus);

			// Template
			PlaceHolder template = new PlaceHolder();
			cms.businesslogic.web.DocumentType DocumentType = new cms.businesslogic.web.DocumentType(_document.ContentType.Id);
			tmp.PropertiesPane.addProperty(ui.Text("documentType"), new LiteralControl(DocumentType.Text));

			// stat (disabled in v3)
            //if (GlobalSettings.EnableStat) 
            //{
            //    string lastVisit = "";
            //    string numberOfVisits = "";
            //    try 
            //    {
            //        DateTime lastVisitDt = 
            //            DateTime.Parse(SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text, "select top 1 entryTime from umbracoStatSession inner join umbracoStatEntry on sessionId = umbracoStatSession.id where isHuman = 1 and nodeID = @docId order by entrytime desc", new SqlParameter("@docId", _document.Id)).ToString());
            //        lastVisit = ui.Text("lastVisit") + ": " + lastVisitDt.ToLongDateString() + " " + lastVisitDt.ToShortTimeString();

            //        numberOfVisits =
            //            ui.Text("visitsLastMonth") + ": ";

            //        if (Cache["statCacheNumberOfVisits" + _document.Id.ToString()] != null && Cache["statCacheNumberOfVisits" + _document.Id.ToString()].ToString() != "")
            //            numberOfVisits += Cache["statCacheNumberOfVisits" + _document.Id.ToString()].ToString();
            //        else {
            //            string tempVisits =  
            //             SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text, "select count(id) from umbracoStatSession inner join umbracoStatEntry on sessionId = umbracoStatSession.id where ishuman = 1 and datediff(mm, entrytime, getdate()) <= 1 and nodeID = @docId", new SqlParameter("@docId", _document.Id)).ToString();
            //            numberOfVisits += tempVisits;

            //        }
                    
            //        numberOfVisits += "<br/>";
            //    } 
            //    catch {}
            //    tmp.PropertiesPane.addProperty(ui.Text("stat"), new LiteralControl(numberOfVisits + lastVisit));
            //}

			int defaultTemplate;
			if (_document.Template != 0)
				defaultTemplate = _document.Template;
			else
				defaultTemplate = DocumentType.DefaultTemplate;

			if (this.getUser().UserType.Name == "writer") 
			{
				if (defaultTemplate != 0)
					template.Controls.Add(new LiteralControl(new cms.businesslogic.template.Template(defaultTemplate).Text));
				else
					template.Controls.Add(new LiteralControl(ui.Text("content", "noDefaultTemplate")));
			} 
			else 
			{
				ddlDefaultTemplate.Items.Add(new ListItem(ui.Text("choose") + "...", ""));
				foreach (cms.businesslogic.template.Template t in DocumentType.allowedTemplates) 
				{
					ListItem tTemp = new ListItem(t.Text, t.Id.ToString());
					if (t.Id == defaultTemplate)
						tTemp.Selected = true;
					ddlDefaultTemplate.Items.Add(tTemp);
				}
				template.Controls.Add(ddlDefaultTemplate);
			}
			publishProps.addProperty(ui.Text("template"), template);
			
			// Editable update date, release date and expire date added by NH 13.12.04
			dp.ID = "updateDate";
			dp.DateTime = DateTime.Now;
			dp.ShowTime = true;
			publishProps.addProperty(ui.Text("content", "updateDate", base.getUser()),dp);

			dpRelease.ID = "releaseDate";
			dpRelease.DateTime = _document.ReleaseDate;
			dpRelease.ShowTime = true;
			publishProps.addProperty(ui.Text("content", "releaseDate", base.getUser()),dpRelease);

			dpExpire.ID = "expireDate";
			dpExpire.DateTime = _document.ExpireDate;
			dpExpire.ShowTime = true;
			publishProps.addProperty(ui.Text("content", "expireDate", base.getUser()),dpExpire);
		
			// url's
			updateLinks();
			linkProps.addProperty(ui.Text("content", "urls", base.getUser()), l);
			
			if (domainText.Text != "")
				linkProps.addProperty(ui.Text("content", "alternativeUrls", base.getUser()), domainText);			

			tmp.Save += new System.EventHandler(Save);
			tmp.SaveAndPublish += new System.EventHandler(Publish);
			tmp.SaveToPublish += new System.EventHandler(SendToPublish);

			// Add panes to property page...
			tmp.tpProp.Controls.Add(publishProps);
			tmp.tpProp.Controls.Add(linkProps);

			
		}
	
		private void InitializeComponent()
		{    
			this.UnPublish.Click += new System.EventHandler(this.UnPublishDo);
		}
		#endregion
	}
}