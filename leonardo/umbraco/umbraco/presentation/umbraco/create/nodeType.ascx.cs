namespace umbraco.cms.presentation.create.controls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for nodeType.
	/// </summary>
	public partial class nodeType : System.Web.UI.UserControl
	{


		protected void Page_Load(object sender, System.EventArgs e)
		{
			sbmt.Text = ui.Text("create");
		}

		protected void sbmt_Click(object sender, System.EventArgs e)
		{
			if (Page.IsValid) 
			{
				int createTemplateVal = 0;
				int nodeId = -1;
				if (createTemplate.Checked)
					createTemplateVal = 1;

				string returnUrl = umbraco.presentation.create.dialogHandler_temp.Create(
					umbraco.helper.Request("nodeType"),
					createTemplateVal,
					rename.Text);

				if (returnUrl == "") 
				{
					if (nodeId > 0)
						Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "refresh", "<script>\nwindow.opener.refreshTree(true, true);\nwindow.close();</script>");
					else
						Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "refresh", "<script>\nwindow.opener.refreshTree(false, true);\nwindow.close();</script>");
				}
				else
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "refresh", "<script>\nwindow.opener.right.location.href = '" + returnUrl + "';\nwindow.close();</script>");
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}
