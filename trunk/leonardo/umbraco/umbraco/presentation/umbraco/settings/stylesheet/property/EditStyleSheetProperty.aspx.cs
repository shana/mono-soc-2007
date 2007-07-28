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

namespace umbraco.cms.presentation.settings.stylesheet
{
	/// <summary>
	/// Summary description for EditStyleSheetProperty.
	/// </summary>
	public partial class EditStyleSheetProperty : BasePages.UmbracoEnsuredPage
	{
		protected TextBox NameTxt = new TextBox();
		protected TextBox AliasTxt = new TextBox();
		protected TextBox Content = new TextBox();
		private cms.businesslogic.web.StylesheetProperty stylesheetproperty;
		private DropDownList ddl = new DropDownList();
		private Literal lttPreView = new Literal();
        uicontrols.Pane pp = new uicontrols.Pane();

		protected void Page_Load(object sender, System.EventArgs e)
		{
			stylesheetproperty = new cms.businesslogic.web.StylesheetProperty(int.Parse(Request.QueryString["id"]));
			
			if (!IsPostBack) 
			{
				NameTxt.Text = stylesheetproperty.Text;
				Content.Text = stylesheetproperty.value;
				AliasTxt.Text = stylesheetproperty.Alias;
			}

			
			Panel1.Controls.Add(pp);

			pp.addProperty(ui.Text("name", base.getUser()),NameTxt);
			pp.addProperty(ui.Text("alias", base.getUser()),AliasTxt);
			Panel1.Text = ui.Text("editstylesheet", base.getUser());
			
			pp.addProperty(Content);
			
		
			ImageButton bt = Panel1.Menu.NewImageButton();
			bt.Click += new System.Web.UI.ImageClickEventHandler(save_click);
			bt.ImageUrl = UmbracoPath +"/images/editor/save.gif";
			bt.AlternateText = ui.Text("save");
			setupPreView();
		}

		protected override void OnPreRender(EventArgs e)
		{
			//

			//
			lttPreView.Text = "<div style='font-family:helvetica, arial,Lucida Grande;overflow:auto;border:1px #ccc solid;font-weight:normal;width:100%;padding:10px;height:150px;scroll:auto;'><span style='"+ Content.Text +"'>a b c d e f g h i j k l m n o p q r s t u v w x t z <br/>A B C D E F G H I J K L M N O P Q R S T U V W X Y Z<br/> 1 2 3 4 5 6 7 8 9 0 $%&(.,;:'\"!?)<br/><br/>Just keep examining every bid quoted for zinc etchings.<br/></style></div>";
			pp.addProperty(lttPreView);
			base.OnPreRender (e);
		}

		private void setupPreView() 
		{
			lttPreView.Text = stylesheetproperty.ToString();
		}
		private void save_click(object sender, System.Web.UI.ImageClickEventArgs e) 
		{
			stylesheetproperty.value = Content.Text;
			stylesheetproperty.Text = NameTxt.Text;
			stylesheetproperty.Alias = AliasTxt.Text;
			try 
			{
				stylesheetproperty.StyleSheet().saveCssToFile();
			} 
			catch {}
			this.speechBubble(speechBubbleIcon.save,ui.Text("speechBubbles", "editStylesheetPropertySaved", base.getUser()),"");
			setupPreView();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
			Content.TextMode = TextBoxMode.MultiLine;
			Content.Height = 250;
			Content.Width = 300;
			
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
