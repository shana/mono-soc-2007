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

using System.Xml;

namespace umbraco.dialogs
{
	/// <summary>
	/// Summary description for uploadImage.
	/// </summary>
	public partial class uploadImage : BasePages.UmbracoEnsuredPage
	{

		protected interfaces.IDataType uploadField = new cms.businesslogic.datatype.controls.Factory().GetNewObject(new Guid("5032a6e6-69e3-491d-bb28-cd31cd11086c"));
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			//uploadField.Alias = "umbracoFile";
			((HtmlInputFile) uploadField.DataEditor).Attributes.Add("onChange", "validateImage()");
			((HtmlInputFile) uploadField.DataEditor).ID = "uploadFile";
			if (!IsPostBack) 
			{
				Button1.Text = ui.Text("save");
				LiteralTitle.Text = ui.Text("name");
				LiteralUpload.Text = ui.Text("choose");
				folderid.Value = this.getUser().StartMediaId.ToString();
				if (int.Parse(folderid.Value) > 0)
					FolderName.Text = new cms.businesslogic.media.Media(int.Parse(folderid.Value)).Text;
				else
					FolderName.Text = ui.Text("media");
			} 
			else 
			{
				
				cms.businesslogic.media.Media m = cms.businesslogic.media.Media.MakeNew(TextBoxTitle.Text, cms.businesslogic.media.MediaType.GetByAlias("image"), this.getUser(), int.Parse(folderid.Value));
				foreach (cms.businesslogic.property.Property p in m.getProperties) {
					if (p.PropertyType.DataTypeDefinition.DataType.Id == uploadField.Id) 
					{
						uploadField.DataTypeDefinitionId = p.PropertyType.DataTypeDefinition.Id;
						uploadField.Data.PropertyId = p.Id;
					}
				}
				uploadField.DataEditor.Save();

				// Generate xml on image
				m.XmlGenerate(new XmlDocument());
				Panel1.Visible = false;
				Panel2.Visible = true;
				string imagename = m.getProperty("umbracoFile").Value.ToString();
				
				string extension = imagename.Substring(imagename.Length-4,4);
				imagename = imagename.Remove(imagename.Length-4,4) + "_thumb.jpg";
				thumbnailtext.Text = umbraco.ui.Text("thumbnailimageclickfororiginal");		
				uploadedimage.Src = umbraco.GlobalSettings.Path + "/../" + imagename;
				uploadedimage.Attributes.Add("onclick","document.location.href='" + umbraco.GlobalSettings.Path + "/.." + m.getProperty("umbracoFile").Value.ToString() + "'");
				Panel2.Controls.Add(new LiteralControl("<script>\n parent.updateImageSource('" + umbraco.GlobalSettings.Path + "/.." + m.getProperty("umbracoFile").Value.ToString() + "', '" + TextBoxTitle.Text + "', " + m.getProperty("umbracoWidth").Value.ToString() + ", " + m.getProperty("umbracoHeight").Value.ToString() + ")\n</script>"));

			}
			// Put user code to initialize the page here
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);

			// Get upload field from datafield factory
			PlaceHolder1.Controls.Add((Control) uploadField.DataEditor);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

		protected void Button1_Click(object sender, System.EventArgs e)
		{
		
		}
	}
}
