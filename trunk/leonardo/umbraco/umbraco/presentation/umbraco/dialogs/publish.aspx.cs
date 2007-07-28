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

using System.Threading;

namespace umbraco.dialogs
{
	/// <summary>
	/// Summary description for publish.
	/// </summary>
	public partial class publish : BasePages.UmbracoEnsuredPage
	{
		protected System.Web.UI.WebControls.Literal total;

		private int nodeId;
		private int nodesPublished = 0;


		protected void Page_Load(object sender, System.EventArgs e)
		{
			nodeId = int.Parse(helper.Request("id"));

			int TotalNodesToPublish = cms.businesslogic.CMSNode.CountSubs(nodeId);
			Application.Lock();
			Application["publishTotal" + nodeId.ToString()] = TotalNodesToPublish.ToString();
			Application["publishDone" + nodeId.ToString()] = "0";
			Application.UnLock();
			total.Text = TotalNodesToPublish.ToString();

			// Put user code to initialize the page here
			if (!IsPostBack) 
			{
				// Add caption to checkbox
				PublishAll.Text = ui.Text("publish", "publishAll", base.getUser());
				ok.Text = ui.Text("general", "ok", base.getUser());
				ok.Attributes.Add("style", "width: 60px");
				ok.Attributes.Add("onClick", "startPublication();");

				// Add progressBar
				controls.progressBar pb = new controls.progressBar();
				pb.ID = "publishStatus";
				pb.Width = 200;
				progressBar.Controls.Add(pb);
				
			} 
			else 
			{
				cms.businesslogic.web.Document d = new cms.businesslogic.web.Document(nodeId);
				if (PublishAll.Checked) 
				{
					nodesPublished = 0;

                    umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Debug, d.Id, "before documentpublish");
					doPublishSubs(d);
                    umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Debug, d.Id, "After documentpublish");

                    umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Debug, d.Id, "before publishing to xmlcache");
					
                    content.Instance.PublishNode(documents);
                    umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Debug, d.Id, "after publishing to xmlcache");
                    Application.Lock();
                    Application["publishTotal" + nodeId.ToString()] = 0;
                    Application.UnLock();

                    FeedBackMessage.Text = "<div class=\"feedbackCreate\">" + ui.Text("publish", "nodePublishAll", d.Text, base.getUser()) + "</div>";
					Application.Lock();

                    

					Application["publishTotal" + nodeId.ToString()] = null;
					Application["publishDone" + nodeId.ToString()] = null;
					Application.UnLock();
				} 
				else 
				{
					d.Publish(base.getUser());
					library.PublishSingleNode(d.Id);
                    FeedBackMessage.Text = "<div class=\"feedbackCreate\">" + ui.Text("publish", "nodePublish", d.Text, base.getUser()) + "</div>";
				}
				TheForm.Visible = false;
			}
		}
        private System.Collections.Generic.List<cms.businesslogic.web.Document> documents = new System.Collections.Generic.List<umbraco.cms.businesslogic.web.Document>();

		private void doPublishSubs(cms.businesslogic.web.Document d) 
		{

			d.Publish(base.getUser());
			//library.PublishSingleNode(d.Id);
            documents.Add(d);

			nodesPublished++;
			Application.Lock();
			Application["publishDone" + nodeId.ToString()] = nodesPublished.ToString();
			Application.UnLock();
			foreach (cms.businesslogic.web.Document dc in d.Children) 
			{
				doPublishSubs(dc);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
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
