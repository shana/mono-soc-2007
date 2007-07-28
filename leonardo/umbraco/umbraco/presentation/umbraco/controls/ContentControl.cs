using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;
using umbraco.BasePages;

namespace umbraco.controls
{
	using uicontrols;
	/// <summary>
	/// Summary description for ContentControl.
	/// </summary>
	public class ContentControl : uicontrols.TabView
	{
		private cms.businesslogic.Content _content;
		private ArrayList _dataFields = new ArrayList();
		private BasePages.UmbracoEnsuredPage prntpage;
		public event System.EventHandler SaveAndPublish;
		public event System.EventHandler SaveToPublish;
		public event System.EventHandler Save;
		private publishModes CanPublish = publishModes.NoPublish;
		public uicontrols.TabPage tpProp;
		public bool DoesPublish = false;
		Hashtable inTab = new Hashtable();
		public TextBox NameTxt = new TextBox();
		private static string _UmbracoPath = GlobalSettings.Path;
		public uicontrols.Pane PropertiesPane = new uicontrols.Pane();

		// Error messages
		private string _errorMessage = "";

		public string ErrorMessage 
		{
			set {_errorMessage = value;}
		}

		protected void standardSaveAndPublishHandler(object sender, System.EventArgs e) 
		{
		
		}

		public ContentControl(cms.businesslogic.Content c, publishModes CanPublish,string Id)
         
		{
			this.ID = Id;
			this.CanPublish = CanPublish;
			_content = c;
		
			this.Width = 350;
			this.Height = 350;
			this.SaveAndPublish += new System.EventHandler(standardSaveAndPublishHandler);
			this.Save += new System.EventHandler(standardSaveAndPublishHandler);
			prntpage = (BasePages.UmbracoEnsuredPage) this.Page;
			
			foreach (cms.businesslogic.ContentType.TabI t in _content.ContentType.getVirtualTabs) 
			{
				uicontrols.TabPage tp = this.NewTabPage(t.Caption);
				addSaveAndPublishButtons(ref tp);
			
				tp.Style.Add("text-align","center");


				// Iterate through the property types and add them to the tab
				foreach (cms.businesslogic.propertytype.PropertyType pt in t.PropertyTypes) 
				{
					// table.Rows.Add(addControl(_content.getProperty(pt.Alias), tp));
					addControlNew(_content.getProperty(pt),tp,t.Caption);
					inTab.Add(pt.Id.ToString(), true);
				}
			}


			// Add property pane
			tpProp = this.NewTabPage(ui.Text("general","properties",null));
			addSaveAndPublishButtons(ref tpProp);
			tpProp.Controls.Add(new LiteralControl("<div id=\"errorPane_" + tpProp.ClientID + "\" style=\"display: none; text-align: left; color: red;width: 100%; border: 1px solid red; background-color: #FCDEDE\"><div><b>There were errors - data has not been saved!</b><br/></div></div>"));
		}


		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			// Add extras for the property tabpage. .
			
			NameTxt.ID = "NameTxt";
			if (!Page.IsPostBack) 
			{
				NameTxt.Text = _content.Text;
			}
			
			PropertiesPane.addProperty(ui.Text("general","name",null), NameTxt);
			
			Literal ltt = new Literal();
			ltt.Text =  _content.User.Name;
			PropertiesPane.addProperty(ui.Text("content","createBy",null),ltt);

			ltt = new Literal();
			ltt.Text = _content.CreateDateTime.ToString();
			PropertiesPane.addProperty(ui.Text("content", "createDate", null),ltt);

			tpProp.Controls.AddAt(0, PropertiesPane);
			tpProp.Style.Add("text-align","center");
			tpProp.Style.Add("padding","10px");
						
		}

		protected override void OnLoad(EventArgs e)
		{
			foreach (cms.businesslogic.property.Property p in _content.getProperties) 
			{
				if (inTab[p.PropertyType.Id.ToString()] == null) 
					addControlNew(p,tpProp, ui.Text("general","properties",null));
				
			}
			base.OnLoad (e);
		}


		private void saveClick(object Sender, System.Web.UI.ImageClickEventArgs e) 
		{
			foreach (interfaces.IDataEditor df in _dataFields) 
			{
				df.Save();
			}
			_content.Text = NameTxt.Text;
			Save(this,new System.EventArgs());
		}

		private void savePublish(object Sender, System.Web.UI.ImageClickEventArgs e) 
		{
			DoesPublish = true;
			saveClick(Sender, e);
			SaveAndPublish(this,new System.EventArgs());
		}

		private void saveToPublish(object Sender, System.Web.UI.ImageClickEventArgs e) 
		{
			saveClick(Sender, e);
			SaveToPublish(this,new System.EventArgs());
		}

		private void addSaveAndPublishButtons(ref uicontrols.TabPage tp) 
		{
			MenuImageButton menuSave = tp.Menu.NewImageButton();
			menuSave.ID = tp.ID + "_save";
			menuSave.ImageUrl = _UmbracoPath + "/images/editor/save.gif";
			menuSave.Click += new System.Web.UI.ImageClickEventHandler(saveClick);
			((MenuIconI)menuSave).OnClickCommand = "invokeSaveHandlers();";
			((MenuIconI)menuSave).AltText = ui.Text("buttons", "save", null);
			if (CanPublish == publishModes.Publish) 
			{
				MenuImageButton menuPublish = tp.Menu.NewImageButton();
				menuPublish.ID = tp.ID + "_publish";
				menuPublish.ImageUrl = _UmbracoPath + "/images/editor/saveAndPublish.gif";
				((MenuIconI)menuPublish).OnClickCommand = "invokeSaveHandlers();";
				menuPublish.Click += new System.Web.UI.ImageClickEventHandler(savePublish);
				((MenuIconI)menuPublish).AltText = ui.Text("buttons", "saveAndPublish", null);
			} 
			else if (CanPublish == publishModes.SendToPublish) 
			{
				MenuImageButton menuToPublish = tp.Menu.NewImageButton();
				menuToPublish.ID = tp.ID + "_topublish";
				menuToPublish.ImageUrl = _UmbracoPath + "/images/editor/saveToPublish.gif";
				((MenuIconI)menuToPublish).OnClickCommand = "invokeSaveHandlers();";
				menuToPublish.Click += new System.Web.UI.ImageClickEventHandler(saveToPublish);
				((MenuIconI)menuToPublish).AltText = ui.Text("buttons", "saveToPublish", null);
			}
		}


		private void addControlNew(cms.businesslogic.property.Property p, uicontrols.TabPage tp, string Caption) {
			interfaces.IDataType dt = p.PropertyType.DataTypeDefinition.DataType;
			dt.DataEditor.Editor.ID = p.PropertyType.Alias;
			dt.Data.PropertyId = p.Id;
			
            // check for element additions
		    interfaces.IMenuElement menuElement = dt.DataEditor.Editor as interfaces.IMenuElement;
            if (menuElement != null)
            {
                // add separator
                tp.Menu.InsertSplitter();

                // add the element
                tp.Menu.NewElement(menuElement.ElementName, menuElement.ElementIdPreFix + p.Id.ToString(), menuElement.ElementClass, menuElement.ExtraMenuWidth);
            }

            // check for buttons
			interfaces.IDataFieldWithButtons df1 = dt.DataEditor.Editor as interfaces.IDataFieldWithButtons;
			if (df1 != null)
			{
				// df1.Alias = p.PropertyType.Alias;
				/*
				// df1.Version = _content.Version;
				dt.Data.PropertyId = p.Id;
				*/
				((Control)df1).ID= p.PropertyType.Alias;
				

				if (df1.MenuIcons.Length > 0)
					tp.Menu.InsertSplitter();
					

				// Add buttons
				int c = 0;
				bool atEditHtml = false;
				bool atSplitter = false;
				foreach (object o in df1.MenuIcons) 
				{
					try 
					{
						uicontrols.MenuIconI m = (uicontrols.MenuIconI) o;
						uicontrols.MenuIconI mi = tp.Menu.NewIcon();
						mi.ImageURL = m.ImageURL;
						mi.OnClickCommand = m.OnClickCommand;
						mi.AltText = m.AltText;
						mi.ID = tp.ID + "_" + m.ID;

						if (m.ID == "html")
							atEditHtml = true;
						else
							atEditHtml = false;

						atSplitter = false;
					} 
					catch 
					{
						tp.Menu.InsertSplitter();
						atSplitter = true;
					}

					// Testing custom styles in editor
					if (atSplitter && atEditHtml && dt.DataEditor.TreatAsRichTextEditor) 
					{
						DropDownList ddl = tp.Menu.NewDropDownList();
						
						ddl.Style.Add("margin-bottom","5px");
						ddl.Items.Add(ui.Text("buttons", "styleChoose", null));
						ddl.ID = tp.ID + "_editorStyle";
						if (cms.businesslogic.web.StyleSheet.GetAll().Length > 0) 
						{
							foreach (cms.businesslogic.web.StyleSheet s in cms.businesslogic.web.StyleSheet.GetAll()) 
							{

								foreach (cms.businesslogic.web.StylesheetProperty sp in s.Properties) 
								{
									ddl.Items.Add(new ListItem(sp.Text,sp.Alias));																							
								}
							}
						}
						ddl.Attributes.Add("onChange","addStyle(this, '"+ p.PropertyType.Alias +"');");
						atEditHtml = false;
					}
					c++;
				}
			} 

			// df.Alias = p.PropertyType.Alias;
			// ((Control) df).ID = p.PropertyType.Alias;
			// df.Text = p.Value.ToString();

			_dataFields.Add(dt.DataEditor.Editor);


			uicontrols.Pane pp = new uicontrols.Pane();
			System.Web.UI.Control holder = new Control();
			holder.Controls.Add(dt.DataEditor.Editor);
			if (p.PropertyType.DataTypeDefinition.DataType.DataEditor.ShowLabel) 
				pp.addProperty(p.PropertyType.Name,holder);
			else 
				pp.addProperty(holder);

			// Validation
			if (p.PropertyType.Mandatory) 
			{
				try 
				{
					System.Web.UI.WebControls.RequiredFieldValidator rq = new RequiredFieldValidator();
					rq.ControlToValidate = dt.DataEditor.Editor.ID;
					rq.EnableClientScript = false;
					rq.Display = System.Web.UI.WebControls.ValidatorDisplay.Dynamic;
					string[] errorVars = {p.PropertyType.Name, Caption};
					rq.ErrorMessage = ui.Text("errorHandling", "errorMandatory", errorVars, null) + "<br/>";
					holder.Controls.AddAt(0,  rq);
				} 
				catch (Exception valE)
				{
					System.Web.HttpContext.Current.Trace.Warn("contentControl", "EditorControl (" + dt.DataTypeName + ") does not support validation", valE);
				}
			}

			// RegExp Validation
			if (p.PropertyType.ValidationRegExp != "") 
			{
				try {
				System.Web.UI.WebControls.RegularExpressionValidator rv =new RegularExpressionValidator();
				rv.ControlToValidate = dt.DataEditor.Editor.ID;
				rv.ValidationExpression = p.PropertyType.ValidationRegExp;
				rv.EnableClientScript = false;
				rv.Display = System.Web.UI.WebControls.ValidatorDisplay.Dynamic;
                string[] errorVars = {p.PropertyType.Name, Caption};
				rv.ErrorMessage = ui.Text("errorHandling", "errorRegExp", errorVars, null) + "<br/>";
				holder.Controls.AddAt(0,  rv);
				} 
				catch (Exception valE)
				{
					System.Web.HttpContext.Current.Trace.Warn("contentControl", "EditorControl (" + dt.DataTypeName + ") does not support validation", valE);
				}

			}
			
			// This is once again a nasty nasty hack to fix gui when rendering wysiwygeditor
			if (dt.DataEditor.TreatAsRichTextEditor) 
				{
					tp.Controls.Add(dt.DataEditor.Editor);
				}
			else {
				Panel ph = new Panel();
				ph.Attributes.Add("style", "padding: 10px 10px 0px 10px");
				ph.Controls.Add(pp);
				
				tp.Controls.Add(ph);
			}
		}

		public enum publishModes 
		{
			Publish,
			SendToPublish,
			NoPublish
		}
	}
}
