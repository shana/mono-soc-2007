namespace umbraco.presentation.install.steps
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for upgradeIndex.
	/// </summary>
	public partial class upgradeIndex : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Literal total;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			((Button) Page.FindControl("next")).Visible = false;
			Button1.Attributes.Add("onClick", "doAjax()");
			total.Text = cms.businesslogic.web.Document.getAllUniquesFromObjectType(new Guid("c66ba18e-eaf3-4cff-8a22-41b16d66a972")).Length.ToString();

			// Add progressBar
			controls.progressBar pb = new controls.progressBar();
			pb.ID = "upgradeStatus";
			pb.Width = 200;
			progressBar.Controls.Add(pb);
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		protected void Button1_Click(object sender, System.EventArgs e)
		{
			int count = 0;
			System.Xml.XmlDocument xd = new	System.Xml.XmlDocument();
			foreach(Guid g in cms.businesslogic.web.Document.getAllUniquesFromObjectType(new Guid("c66ba18e-eaf3-4cff-8a22-41b16d66a972"))) 
			{
				cms.businesslogic.web.Document d = new cms.businesslogic.web.Document(g);
				if (d.Published)
					d.XmlGenerate(xd);
				Application.Lock();
				Application["cmsXmlDone"] = count++;
				Application.UnLock();
			}

            form.Visible = false;
			result.Visible = true;
			((Button) Page.FindControl("next")).Visible = true;
		}
	}
}
