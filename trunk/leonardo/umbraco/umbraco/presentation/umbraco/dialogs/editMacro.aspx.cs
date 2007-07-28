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
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using System.Reflection;
using System.Text;
using System.IO;


namespace umbraco.dialogs
{
	/// <summary>
	/// Summary description for insertMacro.
	/// </summary>
	public partial class editMacro : BasePages.UmbracoEnsuredPage
	{
		protected System.Web.UI.WebControls.Button Button1;

		private cms.businesslogic.macro.Macro m;

		protected void Page_Load(object sender, System.EventArgs e)
		{

			string reqMacroID = helper.Request("umb_macroID");
			string reqMacroAlias = helper.Request("umb_macroAlias");

			if (reqMacroID != "" || reqMacroAlias != "") 
			{
				// Put user code to initialize the page here
				if (reqMacroID != "") 
				{
					m = new umbraco.cms.businesslogic.macro.Macro(int.Parse(reqMacroID));
				}
				else 
				{
					m = cms.businesslogic.macro.Macro.GetByAlias(reqMacroAlias);
				}

				String macroAssembly = "";
				String macroType = "";

				macroName.Text = m.Name;

				foreach (cms.businesslogic.macro.MacroProperty mp in m.Properties) {
		
					macroAssembly = mp.Type.Assembly;
					macroType = mp.Type.Type;
					try 
					{
					
						Assembly assembly = Assembly.LoadFrom(Server.MapPath(GlobalSettings.Path + "/../bin/"+macroAssembly+".dll"));

						Type type = assembly.GetType(macroAssembly+"."+macroType);
						interfaces.IMacroGuiRendering typeInstance = Activator.CreateInstance(type) as interfaces.IMacroGuiRendering;
						if (typeInstance != null) 
						{
							Control control = Activator.CreateInstance(type) as Control;	
							control.ID = mp.Alias;
							if (!IsPostBack) 
							{
								if (Request["umb_" + mp.Alias] != null) 
								{
									if (Request["umb_" + mp.Alias] != "") 
									{
										type.GetProperty("Value").SetValue(control, Convert.ChangeType(Request["umb_" + mp.Alias], type.GetProperty("Value").PropertyType), null);
									}
								}
							}

							// register alias
							macroProperties.Controls.Add(new LiteralControl("<script>\nregisterAlias('" + control.ID + "');\n</script>"));
							macroProperties.Controls.Add(new LiteralControl("<tr><th class=\"propertyHeader\" width=\"30%\">" + mp.Name + "</td><td class=\"propertyContent\">"));
							macroProperties.Controls.Add(control);
							macroProperties.Controls.Add(new LiteralControl("</td></tr>"));
						} 
						else 
						{						
							Trace.Warn("umbEditContent", "Type doesn't exist or is not umbraco.interfaces.DataFieldI ('" + macroAssembly + "." + macroType + "')");
						}

					} 
					catch (Exception fieldException)
					{
						Trace.Warn("umbEditContent", "Error creating type '" + macroAssembly + "." + macroType + "'", fieldException);
					}
				}
			} 

			else 
			{
				SqlDataReader macroRenderings;
				if (helper.Request("editor") != "")
					macroRenderings = SqlHelper.ExecuteReader(GlobalSettings.DbDSN, 
						CommandType.Text, "select macroAlias, macroName from cmsMacro where macroUseInEditor = 1 order by macroName");
				else
					macroRenderings = SqlHelper.ExecuteReader(GlobalSettings.DbDSN, 
						CommandType.Text, "select macroAlias, macroName from cmsMacro order by macroName");
				
				umb_macroAlias.DataSource = macroRenderings;
				umb_macroAlias.DataValueField = "macroAlias";
				umb_macroAlias.DataTextField = "macroName";
				umb_macroAlias.DataBind();
				macroRenderings.Close();

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
			renderMacro.Click +=new EventHandler(renderMacro_Click);

		}
		#endregion

		private void renderMacro_Click(object sender, EventArgs e)
		{
			int pageID = int.Parse(helper.Request("umbPageId"));
			string macroAttributes = "macroAlias=\"" + m.Alias + "\"";
			Guid pageVersion = new Guid(helper.Request("umbVersionId"));

			Hashtable attributes = new Hashtable();
			attributes.Add("macroAlias", m.Alias);
			umbraco.macro mRender = new macro(m.Id);
			foreach(Control c in macroProperties.Controls) 
			{
				try 
				{
					interfaces.IMacroGuiRendering ic = (interfaces.IMacroGuiRendering) c;
					attributes.Add(c.ID, ic.Value);
					macroAttributes += " " + c.ID + "=\"" + ic.Value + "\"";
				} 
				catch {}
			}

			// document this, for gods sake!
			System.Web.HttpContext.Current.Items["macrosAdded"] = 0;
			System.Web.HttpContext.Current.Items["pageID"] = pageID.ToString();


			umbraco.page p = new page(pageID, pageVersion);

			string div = macro.renderMacroStartTag(attributes, pageID, pageVersion);

			string macroContent = macro.MacroContentByHttp(pageID, pageVersion, attributes).Replace("\\", "\\\\").Replace("'", "\\'").Replace("/", "\\/");
			if (macroContent.Length > 0 && macroContent.ToLower().IndexOf("<script") > -1)
				macroContent = "<b>Macro rendering contains script code</b><br/>This macro won\\'t be rendered in the editor because it contains script code. It will render correct during runtime.";
			div += macroContent;
			div += macro.renderMacroEndTag();

			ClientScript.RegisterStartupScript(this.GetType(), "postbackScript", "<script>\n parent.opener.umbracoEditMacroDo('" + macroAttributes + "', '" + m.Name.Replace("'", "\'") + "', '" + div + "');\n</script>");
            ClientScript.RegisterStartupScript(this.GetType(), "postbackScriptWindowClose", "<script>\n setTimeout('window.close()',300);\n</script>");
			theForm.Visible = false;

		}
	}
}
