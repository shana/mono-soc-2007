using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Microsoft.ApplicationBlocks.Data;
using umbraco.BasePages;
using umbraco.presentation.cache;
using umbraco.uicontrols;

namespace umbraco.cms.presentation.developer
{
	/// <summary>
	/// Summary description for editMacro.
	/// </summary>
	public partial class editMacro : UmbracoEnsuredPage
	{
		protected PlaceHolder buttons;

		public TabPage InfoTabPage;
		protected Table macroElements;
		public TabPage Parameters;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				int macroID = Convert.ToInt32(Request.QueryString["macroID"]);

				// Load base data from macro
				using (SqlDataReader macro = SqlHelper.ExecuteReader(GlobalSettings.DbDSN,
				                                                     CommandType.Text, "select * from cmsMacro where id = @id",
				                                                     new SqlParameter("@id", macroID)))
				{
					if (macro.Read())
					{
						string tempMacroAssembly = string.Empty;
						string tempMacroType = string.Empty;
						string tmpStr;
						int tmpInt;
						bool tmpBool;

						if (TryGetColumnString(macro, "macroName", out tmpStr))
							macroName.Text = tmpStr;
						if (TryGetColumnString(macro, "macroAlias", out tmpStr))
							macroAlias.Text = tmpStr;
						if (TryGetColumnString(macro, "macroScriptAssembly", out tmpStr))
							tempMacroAssembly = tmpStr;
						if (TryGetColumnString(macro, "macroScriptType", out tmpStr))
							tempMacroType = tmpStr;
						if (TryGetColumnString(macro, "macroXSLT", out tmpStr))
							macroXslt.Text = tmpStr;
						if (TryGetColumnString(macro, "macroPython", out tmpStr))
							macroPython.Text = tmpStr;
						if (TryGetColumnInt32(macro, "macroRefreshRate", out tmpInt))
							cachePeriod.Text = tmpInt.ToString();
						if (TryGetColumnBool(macro, "macroUseInEditor", out tmpBool))
							macroEditor.Checked = tmpBool;
						if (TryGetColumnBool(macro, "macroDontRender", out tmpBool))
							macroRenderContent.Checked = tmpBool;
						else
							macroRenderContent.Checked = false;
						if (TryGetColumnBool(macro, "macroCacheByPage", out tmpBool))
							cacheByPage.Checked = tmpBool;
						if (TryGetColumnBool(macro, "macroCachePersonalized", out tmpBool))
							cachePersonalized.Checked = tmpBool;

						// Populate either usercontrol or custom control
						if (tempMacroType != string.Empty && tempMacroAssembly != string.Empty)
						{
							macroAssembly.Text = tempMacroAssembly;
							macroType.Text = tempMacroType;
						}
						else
						{
							macroUserControl.Text = tempMacroType;
						}

						// Check for assemblyBrowser
						if (tempMacroType.IndexOf(".ascx") > 0)
							assemblyBrowserUserControl.Controls.Add(
								new LiteralControl("<br/><button onClick=\"window.open('assemblyBrowser.aspx?fileName=" + macroUserControl.Text +
								                   "&macroID=" + Request.QueryString["macroID"] +
								                   "', 'nytVindue', 'width=500,height=475,scrollbars=auto'); return false;\" class=\"guiInputButton\"><img src=\"../images/editor/propertiesNew.gif\" align=\"absmiddle\" style=\"width: 18px; height: 17px; padding-right: 5px;\"/> Browse properties</button>"));
						else if (tempMacroType != string.Empty && tempMacroAssembly != string.Empty)
							assemblyBrowser.Controls.Add(
								new LiteralControl("<br/><button onClick=\"window.open('assemblyBrowser.aspx?fileName=" + macroAssembly.Text +
								                   "&macroID=" + Request.QueryString["macroID"] + "&type=" + macroType.Text +
								                   "', 'nytVindue', 'width=500,height=475,scrollbars=auto'); return false\" class=\"guiInputButton\"><img src=\"../images/editor/propertiesNew.gif\" align=\"absmiddle\" style=\"width: 18px; height: 17px; padding-right: 5px;\"/> Browse properties</button>"));
					}
				}

				// Load elements from macro
				macroPropertyBind();

				// Load xslt files from default dir
				populateXsltFiles();

				// Load python files from default dir
				populatePythonFiles();

				// Load usercontrols
				populateUserControls(Server.MapPath("/usercontrols"));
				userControlList.Items.Insert(0, new ListItem("Browse usercontrols on server...", string.Empty));
				userControlList.Attributes.Add("onChange",
				                               "document.getElementById('macroUserControl').value = this[this.selectedIndex].value;");
			}
			else
			{
				int macroID = Convert.ToInt32(Request.QueryString["macroID"]);
				string tempMacroAssembly = macroAssembly.Text;
				string tempMacroType = macroType.Text;
				string tempCachePeriod = cachePeriod.Text;
				if (tempCachePeriod == string.Empty)
					tempCachePeriod = "0";
				bool dontRender = false;
				if (!macroRenderContent.Checked)
					dontRender = true;

				if (tempMacroAssembly == string.Empty && macroUserControl.Text != string.Empty)
					tempMacroType = macroUserControl.Text;

				// Save macro
				SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN,
				                          CommandType.Text,
				                          @"update cmsMacro set          
 macroUseInEditor = @macroUseInEditor,      
 macroDontRender = @macroDontRender,      
 macroCacheByPage = @macroCacheByPage,      
 macroCachePersonalized = @macroCachePersonalized,      
 macroRefreshRate = @macroRefreshRate,      
 macroAlias = @macroAlias,          
 macroName = @macroName,          
 macroScriptAssembly = @macroScriptAssembly,          
 macroScriptType = @macroScriptType,          
 macroXSLT = @macroXSLT,
 macroPython = @macroPython           
where          
 id = @macroID            
",
				                          new SqlParameter("@macroID", Convert.ToInt32(macroID)),
				                          new SqlParameter("@macroUseInEditor", macroEditor.Checked),
				                          new SqlParameter("@macroDontRender", dontRender),
				                          new SqlParameter("@macroCacheByPage", cacheByPage.Checked),
				                          new SqlParameter("@macroCachePersonalized", cachePersonalized.Checked),
				                          new SqlParameter("@macroRefreshRate", Convert.ToInt32(tempCachePeriod)),
				                          new SqlParameter("@macroAlias", macroAlias.Text),
				                          new SqlParameter("@macroName", macroName.Text),
				                          new SqlParameter("@macroScriptAssembly", tempMacroAssembly),
				                          new SqlParameter("@macroScriptType", tempMacroType),
				                          new SqlParameter("@macroXSLT", macroXslt.Text),
				                          new SqlParameter("@macroPython", macroPython.Text)
					);

				// Save elements
				foreach (RepeaterItem item in macroProperties.Items)
				{
					HtmlInputHidden macroPropertyID = (HtmlInputHidden) item.FindControl("macroPropertyID");
					TextBox macroElementName = (TextBox) item.FindControl("macroPropertyName");
					TextBox macroElementAlias = (TextBox) item.FindControl("macroPropertyAlias");
					CheckBox macroElementShow = (CheckBox) item.FindControl("macroPropertyHidden");
					DropDownList macroElementType = (DropDownList) item.FindControl("macroPropertyType");

					SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN,
					                          CommandType.Text,
					                          @"update cmsMacroProperty set      
 macroPropertyHidden = @macroPropertyHidden,      
 macroPropertyType = @macroPropertyType,      
 macroPropertyAlias = @macroPropertyAlias,      
 macroPropertyName = @macroPropertyName      
where      
 id = @macroPropertyID 
",
					                          new SqlParameter("@macroPropertyID", macroPropertyID.Value),
					                          new SqlParameter("@macroPropertyHidden", macroElementShow.Checked),
					                          new SqlParameter("@macroPropertyType", macroElementType.SelectedValue),
					                          new SqlParameter("@macroPropertyAlias", macroElementAlias.Text),
					                          new SqlParameter("@macroPropertyName", macroElementName.Text)
						);
				}

				// Flush macro from cache!
				if (UmbracoSettings.UseDistributedCalls)
					dispatcher.Refresh(
						new Guid("7B1E683C-5F34-43dd-803D-9699EA1E98CA"),
						macroID);
				else
					new macro(macroID).removeFromCache();

				// Check for assemblyBrowser
				if (tempMacroType.IndexOf(".ascx") > 0)
					assemblyBrowserUserControl.Controls.Add(
						new LiteralControl("<br/><button onClick=\"window.open('assemblyBrowser.aspx?fileName=" + macroUserControl.Text +
						                   "&macroID=" + Request.QueryString["macroID"] +
						                   "', 'nytVindue', 'width=500,height=475,scrollbars=auto'); return false;\" class=\"guiInputButton\"><img src=\"../images/editor/propertiesNew.gif\" align=\"absmiddle\" style=\"width: 18px; height: 17px; padding-right: 5px;\"/> Browse properties</button>"));
				else if (tempMacroType != string.Empty && tempMacroAssembly != string.Empty)
					assemblyBrowser.Controls.Add(
						new LiteralControl("<br/><button onClick=\"window.open('assemblyBrowser.aspx?fileName=" + macroAssembly.Text +
						                   "&macroID=" + Request.QueryString["macroID"] + "&type=" + macroType.Text +
						                   "', 'nytVindue', 'width=500,height=475,scrollbars=auto'); return false\" class=\"guiInputButton\"><img src=\"../images/editor/propertiesNew.gif\" align=\"absmiddle\" style=\"width: 18px; height: 17px; padding-right: 5px;\"/> Browse properties</button>"));
			}
		}

		private bool TryGetColumnString(SqlDataReader reader, string columnName, out string value)
		{
			value = string.Empty;
			// First check if column actually exists
			DataTable table = reader.GetSchemaTable();
			foreach (DataRow row in table.Rows)
			{
				if ((string) row["ColumnName"] == columnName)
				{
					int colIndex = (int) row["ColumnOrdinal"];
					if (!reader.IsDBNull(colIndex))
					{
						value = reader.GetString(colIndex);
						return true;
					}
				}
			}
			return false;
		}

		private bool TryGetColumnInt32(SqlDataReader reader, string columnName, out int value)
		{
			value = -1;
			// First check if column actually exists
			foreach (DataRow row in reader.GetSchemaTable().Rows)
			{
				if ((string) row["ColumnName"] == columnName)
				{
					int colIndex = (int) row["ColumnOrdinal"];
					if (!reader.IsDBNull(colIndex))
					{
						value = reader.GetInt32(colIndex);
						return true;
					}
				}
			}
			return false;
		}

		private bool TryGetColumnBool(SqlDataReader reader, string columnName, out bool value)
		{
			value = false;
			// First check if column actually exists
			foreach (DataRow row in reader.GetSchemaTable().Rows)
			{
				if ((string) row["ColumnName"] == columnName)
				{
					int colIndex = (int) row["ColumnOrdinal"];
					if (!reader.IsDBNull(colIndex))
					{
						value = reader.GetBoolean(colIndex);
						return true;
					}
				}
			}
			return false;
		}

		private void getXsltFilesFromDir(string orgPath, string path, ArrayList files)
		{
			DirectoryInfo dirInfo = new DirectoryInfo(path);

			FileInfo[] fileInfo = dirInfo.GetFiles("*.xsl*");
			foreach (FileInfo file in fileInfo)
				files.Add(path.Replace(orgPath, string.Empty) + file.Name);

			// Populate subdirectories
			DirectoryInfo[] dirInfos = dirInfo.GetDirectories();
			foreach (DirectoryInfo dir in dirInfos)
				getXsltFilesFromDir(orgPath, path + "/" + dir.Name, files);
		}

		private void populateXsltFiles()
		{
			ArrayList xslts = new ArrayList();
			string xsltDir = HttpContext.Current.Server.MapPath(GlobalSettings.Path + "/../xslt/");
			getXsltFilesFromDir(xsltDir, xsltDir, xslts);
			xsltFiles.DataSource = xslts;
			xsltFiles.DataBind();
			xsltFiles.Items.Insert(0, new ListItem("Browse xslt files on server...", string.Empty));
			xsltFiles.Attributes.Add("onChange",
			                         "document.getElementById('macroXslt').value = this[this.selectedIndex].value; document.getElementById('macroPython').value =''");
		}

		private void getPythonFilesFromDir(string orgPath, string path, ArrayList files)
		{
			DirectoryInfo dirInfo = new DirectoryInfo(path);
			if (!dirInfo.Exists)
				return;

			FileInfo[] fileInfo = dirInfo.GetFiles("*.py");
			foreach (FileInfo file in fileInfo)
				files.Add(path.Replace(orgPath, string.Empty) + file.Name);

			// Populate subdirectories
			DirectoryInfo[] dirInfos = dirInfo.GetDirectories();
			foreach (DirectoryInfo dir in dirInfos)
				getPythonFilesFromDir(orgPath, path + "/" + dir.Name, files);
		}

		private void populatePythonFiles()
		{
			ArrayList pythons = new ArrayList();
			string pythonDir = HttpContext.Current.Server.MapPath(GlobalSettings.Path + "/../python/");
			getPythonFilesFromDir(pythonDir, pythonDir, pythons);
			pythonFiles.DataSource = pythons;
			pythonFiles.DataBind();
			pythonFiles.Items.Insert(0, new ListItem("Browse python files on server...", string.Empty));
			pythonFiles.Attributes.Add("onChange",
			                           "document.getElementById('macroPython').value = this[this.selectedIndex].value; document.getElementById('macroXslt').value = ''");
		}

		public void deleteMacroProperty(object sender, EventArgs e)
		{
			HtmlInputHidden macroPropertyID = (HtmlInputHidden) ((Control) sender).Parent.FindControl("macroPropertyID");
			SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN,
			                          CommandType.Text, "delete from cmsMacroProperty where id = @id",
			                          new SqlParameter("@id", macroPropertyID.Value));

			macroPropertyBind();
		}

		public void macroPropertyBind()
		{
			using (SqlDataReader macroPropertiesSource = SqlHelper.ExecuteReader(GlobalSettings.DbDSN,
			                                                                     CommandType.Text,
			                                                                     @"select       
 id, macroPropertyHidden, macroPropertyType, macroPropertyAlias, macroPropertyName       
from      
 cmsMacroProperty      
where      
 macro = @macroID      
  
",
			                                                                     new SqlParameter("@macroID",
			                                                                                      Convert.ToInt32(
			                                                                                      	Request.QueryString["macroID"])))
				)
			{
				macroProperties.DataSource = macroPropertiesSource;
				macroProperties.DataBind();
			}
		}

		public object CheckNull(object test)
		{
			if (Convert.IsDBNull(test))
				return 0;
			else
				return test;
		}

		public SqlDataReader GetMacroPropertyTypes()
		{
			// Load dataChildTypes	
			return SqlHelper.ExecuteReader(GlobalSettings.DbDSN,
			                               CommandType.Text,
			                               "select id, macroPropertyTypeAlias from cmsMacroPropertyType order by macroPropertyTypeAlias");
		}

		public void macroPropertyCreate(object sender, EventArgs e)
		{
			CheckBox macroPropertyHiddenNew = (CheckBox) ((Control) sender).Parent.FindControl("macroPropertyHiddenNew");
			TextBox macroPropertyAliasNew = (TextBox) ((Control) sender).Parent.FindControl("macroPropertyAliasNew");
			TextBox macroPropertyNameNew = (TextBox) ((Control) sender).Parent.FindControl("macroPropertyNameNew");
			DropDownList macroPropertyTypeNew = (DropDownList) ((Control) sender).Parent.FindControl("macroPropertyTypeNew");
			if (macroPropertyAliasNew.Text !=
			    ui.Text("general", "new", getUser()) + " " + ui.Text("general", "alias", getUser()))
			{
				SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN,
				                          CommandType.Text,
				                          @"insert into cmsMacroProperty (      
 macro,      
 macroPropertyHidden,      
 macroPropertyType,      
 macroPropertyAlias,      
 macroPropertyName      
)      
values (      
 @macroID,      
 @macroPropertyHidden,      
 @macroPropertyType,      
 @macroPropertyAlias,      
 @macroPropertyName      
)",
				                          new SqlParameter("@macroID", Convert.ToInt32(Request.QueryString["macroID"])),
				                          new SqlParameter("@macroPropertyHidden", macroPropertyHiddenNew.Checked),
				                          new SqlParameter("@macroPropertyType", macroPropertyTypeNew.SelectedValue),
				                          new SqlParameter("@macroPropertyAlias", macroPropertyAliasNew.Text),
				                          new SqlParameter("@macroPropertyName", macroPropertyNameNew.Text)
					);

				macroPropertyBind();
			}
		}

		public bool macroIsVisible(object IsChecked)
		{
			if (Convert.ToBoolean(IsChecked))
				return true;
			else
				return false;
		}

		public int SetMacroPropertyTypesIndex(string macroPropertyType)
		{
			SqlDataReader macroPropertyTypeSource = SqlHelper.ExecuteReader(GlobalSettings.DbDSN,
			                                                                CommandType.Text,
			                                                                "select id, macroPropertyTypeAlias from cmsMacroPropertyType order by macroPropertyTypeAlias");
			int i = 0;
			while (macroPropertyTypeSource.Read())
			{
				if (macroPropertyTypeSource.GetInt16(macroPropertyTypeSource.GetOrdinal("id")).ToString() == macroPropertyType)
				{
					break;
				}
				i++;
			}
			macroPropertyTypeSource.Close();

			return i;
		}

		public void AddChooseList(Object Sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				DropDownList dropDown = (DropDownList) Sender;
				dropDown.Items.Insert(0, new ListItem("Choose...", string.Empty));
			}
		}

		private void populateUserControls(string path)
		{
			DirectoryInfo di = new DirectoryInfo(path);
			foreach (FileInfo uc in di.GetFiles("*.ascx"))
			{
				userControlList.Items.Add(
					new ListItem(
						uc.FullName.Substring(uc.FullName.IndexOf("\\usercontrols"),
						                      uc.FullName.Length - uc.FullName.IndexOf("\\usercontrols")).Replace("\\", "/")));
			}
			foreach (DirectoryInfo dir in di.GetDirectories())
				populateUserControls(dir.FullName);
		}

		#region Web Form Designer generated code

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();

			// Tab setup
			InfoTabPage = TabView1.NewTabPage("Macro Properties");
			InfoTabPage.Controls.Add(Panel1);
			Parameters = TabView1.NewTabPage("Parameters");
			Parameters.Controls.Add(Panel2);

			ImageButton save = InfoTabPage.Menu.NewImageButton();
			save.ImageUrl = GlobalSettings.Path + "/images/editor/save.gif";
			ImageButton save2 = Parameters.Menu.NewImageButton();
			save2.ImageUrl = GlobalSettings.Path + "/images/editor/save.gif";

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

	public class macroElement : TableRow
	{
		private string _alias;
		private int _id;
		private string _name;
		private bool _show;
		private int _type;

		public int MacroElementID
		{
			get { return _id; }
			set { _id = value; }
		}

		public int Type
		{
			get { return _type; }
			set { _type = value; }
		}

		public bool Show
		{
			get { return _show; }
			set { _show = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Alias
		{
			get { return _alias; }
			set { _alias = value; }
		}

		protected override void OnInit(EventArgs e)
		{
			/*
			// Visibility
			System.Web.UI.WebControls.CheckBox macroElementShow = new CheckBox();
			macroElementShow.Checked = _show;
			macroElementShow.ID = _id + "_show";
			System.Web.UI.WebControls.TableCell cellShow = new TableCell();
			cellShow.Controls.Add(macroElementShow);
			cellShow.CssClass = "propertyContent";
			this.Cells.Add(cellShow);

			// Alias
			System.Web.UI.WebControls.TextBox macroElementAlias = new TextBox();
			macroElementAlias.Text = _alias;
			macroElementAlias.ID = _id + "_alias";
			System.Web.UI.WebControls.TableCell cellAlias = new TableCell();
			cellAlias.Controls.Add(macroElementAlias);
			cellAlias.CssClass = "propertyContent";
			this.Cells.Add(cellAlias);

			// Name
			System.Web.UI.WebControls.TextBox macroElementName = new TextBox();
			macroElementName.Text = _name;
			macroElementName.ID = _id + "_name";
			System.Web.UI.WebControls.TableCell cellName = new TableCell();
			cellName.Controls.Add(macroElementName);
			cellName.CssClass = "propertyContent";
			this.Cells.Add(cellName);

			// Type
			SqlDataReader macroPropertyTypes = SqlHelper.ExecuteReader(GlobalSettings.DbDSN, 
				CommandType.StoredProcedure, "umbracoAppMacroGetPropertyTypes");
			System.Web.UI.WebControls.ListBox macroElementType = new ListBox();
			macroElementType.ID = _id + "_type";
			macroElementType.Rows = 1;
			macroElementType.SelectionMode = ListSelectionMode.Single;
			macroElementType.DataTextField = "macroPropertyTypeName";
			macroElementType.DataValueField = "id";
			macroElementType.DataSource = macroPropertyTypes;
			macroElementType.DataBind();
			macroPropertyTypes.Close();

			// Check for match
			if (_type > 0)
				try 
				{
					macroElementType.SelectedValue = _type.ToString();
				} 
				catch {}

			System.Web.UI.WebControls.TableCell cellType = new TableCell();
			cellType.Controls.Add(macroElementType);
			cellType.CssClass = "propertyContent";
			this.Cells.Add(cellType);

			// Delete or save button
			System.Web.UI.WebControls.Button macroElementButton = new Button();
			if (_id > -1)
				macroElementButton.Text = "Delete";
			else
				macroElementButton.Text = "Save";

			System.Web.UI.WebControls.TableCell cellButton = new TableCell();
			cellButton.Controls.Add(macroElementButton);
			cellButton.CssClass = "propertyContent";
			this.Cells.Add(cellButton);
			*/

			base.OnInit(e);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			base.Render(writer);
			writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
			writer.AddAttribute(HtmlTextWriterAttribute.Value, _id.ToString());
			writer.AddAttribute(HtmlTextWriterAttribute.Name, "macroElement");
			writer.RenderBeginTag(HtmlTextWriterTag.Input);
			writer.RenderEndTag();
		}
	}
}