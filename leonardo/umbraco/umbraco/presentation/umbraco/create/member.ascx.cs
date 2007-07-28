namespace umbraco.cms.presentation.create.controls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for member.
	/// </summary>
	public partial class member : System.Web.UI.UserControl
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			sbmt.Text = ui.Text("create");
			foreach(cms.businesslogic.member.MemberType dt in cms.businesslogic.member.MemberType.GetAll) 
			{
				ListItem li = new ListItem();
				li.Text = dt.Text;
				li.Value = dt.Id.ToString();
				nodeType.Items.Add(li);
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

		protected void sbmt_Click(object sender, System.EventArgs e)
		{
			if (Page.IsValid) 
			{
				string returnUrl = umbraco.presentation.create.dialogHandler_temp.Create(
					umbraco.helper.Request("nodeType"),
					int.Parse(nodeType.SelectedValue),
					-1,
					rename.Text);

				if (returnUrl == "")
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "refresh", "<script>\nwindow.close();</script>");
				else
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "refresh", "<script>\nwindow.opener.right.location.href = '" + returnUrl + "';\nwindow.close();</script>");
			}
		
		}
	}
}
