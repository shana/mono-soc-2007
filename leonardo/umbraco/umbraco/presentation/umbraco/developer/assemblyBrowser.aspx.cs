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
using System.Reflection;
using System.Collections.Specialized;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace umbraco.developer
{
	/// <summary>
	/// Summary description for assemblyBrowser.
	/// </summary>
	public partial class assemblyBrowser : BasePages.UmbracoEnsuredPage
	{
	
		private string _ConnString = GlobalSettings.DbDSN;
		protected void Page_Load(object sender, System.EventArgs e)
		{

//			if (!IsPostBack) 
//			{
				bool isUserControl = false;
				try 
				{

					Type type = null;
					if (Request.QueryString["type"] == null) 
					{
						isUserControl = true;
						string fileName = Request.QueryString["fileName"];
						UserControl oControl = (UserControl) LoadControl(@"~/"+fileName);

						type = oControl.GetType();
					} 
					else 
					{
						string currentAss = Server.MapPath( GlobalSettings.Path + "/../bin/" + Request.QueryString["fileName"]+".dll");
						Assembly asm = System.Reflection.Assembly.LoadFrom(currentAss);
						type = asm.GetType(Request.QueryString["type"]);
			}
				
					if (type != null) 
					{
						MacroProperties.Items.Clear();
						string fullControlAssemblyName;
						if (isUserControl) 
						{
							AssemblyName.Text = "Choose Properties from " + type.BaseType.Name;
							fullControlAssemblyName = type.BaseType.Namespace + "." + type.BaseType.Name;
						} 
						else 
						{
							AssemblyName.Text = "Choose Properties from " + type.Name;
							fullControlAssemblyName = type.Namespace + "." + type.Name;
						}
						foreach(PropertyInfo pi in type.GetProperties()) 
						{
							if (pi.CanWrite && fullControlAssemblyName == pi.DeclaringType.Namespace+"."+pi.DeclaringType.Name) 
							{
								MacroProperties.Items.Add(new ListItem(pi.Name + " <span style=\"color: #99CCCC\">(" + pi.PropertyType.Name + ")</span>", pi.PropertyType.Name));
						
								//						Response.Write("<li>" + pi.Name + ", " + pi.CanWrite.ToString() + ", " + pi.DeclaringType.Namespace+"."+pi.DeclaringType.Name + ", " + pi.PropertyType.Name + "</li>");
							}

							foreach (ListItem li in MacroProperties.Items)
								li.Selected = true;
						}
					} 
					else 
					{
						AssemblyName.Text = "Type '" + Request.QueryString["type"] + "' is null";
					}
				} 
				catch (Exception err) 
				{
					AssemblyName.Text = "Error reading " + Request["fileName"];
					Button1.Visible = false;
					ChooseProperties.Controls.Add(new LiteralControl("<p class=\"guiDialogNormal\" style=\"color: red;\">" + err.ToString() + "</p><p/><p class=\"guiDialogNormal\"><a href=\"javascript:window.close()\">Close window</a>"));
				}
//			}
	
			
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

		protected void Button1_Click(object sender, System.EventArgs e)
		{

			// Load all macroPropertyTypes
			Hashtable macroPropertyTypes = new Hashtable();
			Hashtable macroPropertyIds = new Hashtable();
			SqlDataReader dr = SqlHelper.ExecuteReader(_ConnString, CommandType.Text, "select id, macroPropertyTypeBaseType, macroPropertyTypeAlias from cmsMacroPropertyType");
			while (dr.Read()) 
			{
				macroPropertyIds.Add(dr[2].ToString(), dr[0].ToString());
				macroPropertyTypes.Add(dr[2].ToString(), dr[1].ToString());
			}
			dr.Close();

			foreach (ListItem li in MacroProperties.Items) 
			{
				if (li.Selected) 
				{
					string _macroPropertyTypeAlias = findMacroType(macroPropertyTypes, li.Value);
					if (_macroPropertyTypeAlias == "")
						_macroPropertyTypeAlias = "text";

					SqlHelper.ExecuteNonQuery(_ConnString, 
						CommandType.Text, @"
insert into cmsMacroProperty (      
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
)  
",
						new SqlParameter("@macroID", Convert.ToInt32(Request.QueryString["macroID"])), 
						new SqlParameter("@macroPropertyHidden", true), 
						new SqlParameter("@macroPropertyType", macroPropertyIds[_macroPropertyTypeAlias].ToString()), 
						new SqlParameter("@macroPropertyAlias", li.Text.Substring(0, li.Text.IndexOf(" "))), 
						new SqlParameter("@macroPropertyName", spaceCamelCasing(li.Text)) 
						);

				}
			}
			ChooseProperties.Visible = false;
			ConfigProperties.Visible = true;
		}

		private string spaceCamelCasing(string text) 
		{
			string _tempString = text.Substring(0,1).ToUpper();
			for (int i=1;i<text.Length;i++) 
			{
				if (text.Substring(i,1) == " ")
					break;
				if (text.Substring(i,1).ToUpper() == text.Substring(i,1))
					_tempString += " ";
				_tempString += text.Substring(i,1);
			}
			return _tempString;
		}

		private string findMacroType(Hashtable macroPropertyTypes, string baseTypeName) 
		{
			string _tempType = "";
			// Hard-code numeric values
			if (baseTypeName == "Int32")
				_tempType = "number";
			else if (baseTypeName == "String")
				_tempType = "text";
			else if (baseTypeName == "Boolean")
				_tempType = "bool";
			{

				foreach(DictionaryEntry de in macroPropertyTypes)
					if (de.Value.ToString() == baseTypeName) 
					{
						_tempType = de.Key.ToString();
						break;
					}
			}

			return _tempType;
		}

	}

}
