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
using System.Xml;

namespace umbraco.developer
{
	/// <summary>
	/// Summary description for xsltChooseExtension.
	/// </summary>
	public partial class xsltChooseExtension : System.Web.UI.Page
	{


		protected void Page_Load(object sender, System.EventArgs e)
		{
            SortedList ht = getXsltAssemblies();
			if (!IsPostBack) 
			{
				assemblies.Attributes.Add("onChange", "document.forms[0].submit()");
				foreach(string s in ht.Keys)
					assemblies.Items.Add(new ListItem(s));

				// Select the umbraco extensions as default
				assemblies.Items[0].Selected = true;
			}

			string selectedMethod = "";
			if (methods.SelectedValue != "") 
			{
				selectedMethod = methods.SelectedValue;
				PlaceHolderParamters.Controls.Clear();
				PlaceHolderParamters.Controls.Add(new LiteralControl("<strong>" + methods.SelectedValue.Substring(0, methods.SelectedValue.IndexOf("(")) + "</strong> "));
				PlaceHolderParamters.Controls.Add(new LiteralControl("<input type=\"hidden\" name=\"selectedMethod\" value=\"" + methods.SelectedValue.Substring(0, methods.SelectedValue.IndexOf("(")) + "\"/>"));

				int counter = 0;
				foreach (string s in methods.SelectedValue.Substring(methods.SelectedValue.IndexOf("(")+1, methods.SelectedValue.IndexOf(")")-methods.SelectedValue.IndexOf("(")-1).Split(',')) 
				{
					if (s.Trim() != "") 
					{
						counter++;
						TextBox t = new TextBox();
						t.ID = "param" + Guid.NewGuid().ToString();
						t.Text = s.Trim();
						t.TabIndex = (short) counter;
						t.Attributes.Add("onFocus", "if (this.value == '" + s.Trim() + "') this.value = '';");
						t.Attributes.Add("onBlur", "if (this.value == '') this.value = '" + s.Trim() + "'");
						t.Attributes.Add("style", "font-size:XX-Small;width:80px;");
						PlaceHolderParamters.Controls.Add(t);
					}
				}

				counter++;
				PlaceHolderParamters.Controls.Add(new LiteralControl(" <input type=\"button\" style=\"font-size: XX-Small\" tabIndex=\"" + counter.ToString() + "\" value=\"Insert\" onClick=\"returnResult();\"/>"));

			} 
			else
				PlaceHolderParamters.Controls.Clear();


			if (assemblies.SelectedValue != "") 
			{
				methods.Items.Clear();
				methods.Items.Add(new ListItem("Choose method", ""));
				methods.Attributes.Add("onChange", "document.forms[0].submit()");
				SortedList methodList = (SortedList) ht[assemblies.SelectedValue];
				IDictionaryEnumerator ide = methodList.GetEnumerator();
				while (ide.MoveNext())
				{
					ListItem li = new ListItem(ide.Value.ToString());
					if (ide.Value.ToString() == selectedMethod)
						li.Selected = true;
					methods.Items.Add(li);
				}
			}
		}

        private SortedList getXsltAssemblies() 
		{
            SortedList _tempAssemblies = new SortedList();
            _tempAssemblies.Add("umbraco.library", getStaticMethods("/bin/umbraco", "umbraco.library"));
            _tempAssemblies.Add("Exslt.ExsltCommon", getStaticMethods("/bin/umbraco", "umbraco.presentation.xslt.Exslt.ExsltCommon"));
            _tempAssemblies.Add("Exslt.ExsltDatesAndTimes", getStaticMethods("/bin/umbraco", "umbraco.presentation.xslt.Exslt.ExsltDatesAndTimes"));
            _tempAssemblies.Add("Exslt.ExsltMath", getStaticMethods("/bin/umbraco", "umbraco.presentation.xslt.Exslt.ExsltMath"));
            _tempAssemblies.Add("Exslt.ExsltRegularExpressions", getStaticMethods("/bin/umbraco", "umbraco.presentation.xslt.Exslt.ExsltRegularExpressions"));
            _tempAssemblies.Add("Exslt.ExsltStrings", getStaticMethods("/bin/umbraco", "umbraco.presentation.xslt.Exslt.ExsltStrings"));
            _tempAssemblies.Add("Exslt.ExsltSets", getStaticMethods("/bin/umbraco", "umbraco.presentation.xslt.Exslt.ExsltSets"));
            XmlDocument xsltExt = new XmlDocument();
			xsltExt.Load(System.Web.HttpContext.Current.Server.MapPath(GlobalSettings.Path + "/../config/xsltExtensions.config"));
			foreach (XmlNode xsltEx in xsltExt.SelectSingleNode("/XsltExtensions")) 
			{
				_tempAssemblies.Add(xsltEx.Attributes["alias"].Value, getStaticMethods(xsltEx.Attributes["assembly"].Value, xsltEx.Attributes["type"].Value));
				
			}
			return _tempAssemblies;

		}

		private SortedList getStaticMethods(string currentAss, string currentType) 
		{
			SortedList tempMethods = new SortedList();
			currentAss = System.Web.HttpContext.Current.Server.MapPath( GlobalSettings.Path + "/.." + currentAss +".dll");
			Assembly asm = System.Reflection.Assembly.LoadFrom(currentAss);
			Type type = asm.GetType(currentType);				
			foreach(MethodInfo mi in type.GetMethods())
				if (mi.IsStatic) 
				{
					string _temp = mi.Name + "(";
					foreach (ParameterInfo pi in mi.GetParameters()) 
					{
						_temp += pi.ParameterType.Name.ToString() + " " + pi.Name + ", ";
					}
					if (_temp.Substring(_temp.Length-1, 1) == " ")
						_temp = _temp.Substring(0, _temp.Length-2);
					tempMethods.Add(_temp + ")", _temp + ")");
				}
			return tempMethods;
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
