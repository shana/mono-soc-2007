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
using System.Text.RegularExpressions;

namespace umbraco.cms.presentation
{
	/// <summary>
	/// Summary description for richTextHolder.
	/// </summary>
	public partial class richTextHolder : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Literal customStyles;
		protected System.Web.UI.WebControls.Literal myAlias;
		
		protected void Page_Load(object sender, System.EventArgs e)
		{

			// Check for xhtml mode
			if (GlobalSettings.EditXhtmlMode == "true") 
			{
				LabelDoctype.Text = 
					"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n\n<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">\n<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\">";
			} 
			else
			{
				LabelDoctype.Text = 
					"<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" >";
			}

			int nodeId = int.Parse(umbraco.helper.Request("nodeId"));
			Guid versionId = new Guid(umbraco.helper.Request("versionId"));
			int propertyId = int.Parse(umbraco.helper.Request("propertyId"));

			cms.businesslogic.property.Property prop = new cms.businesslogic.property.Property(propertyId);
			string content = "," + prop.Value.ToString() + ",";
			myAlias.Text = prop.PropertyType.Alias;

	
			string pattern = @"(<\?UMBRACO_MACRO\W*[^>]*/>)";
			MatchCollection tags = Regex.Matches(content, pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

			// Page for macro rendering
			umbraco.page p = new page(nodeId, versionId);
			System.Web.HttpContext.Current.Items["macrosAdded"] = 0;
			System.Web.HttpContext.Current.Items["pageID"] = nodeId.ToString();

			foreach (Match tag in tags) 
			{
				// Create div
				Hashtable attributes = helper.ReturnAttributes(tag.Groups[1].Value);
				string div = macro.renderMacroStartTag(attributes, nodeId, versionId);
                
                // Insert macro contents here...
				umbraco.macro m;
				if (helper.FindAttribute(attributes, "macroID") != "")
					m = new macro(int.Parse(helper.FindAttribute(attributes, "macroID")));
				else
					m = new macro(cms.businesslogic.macro.Macro.GetByAlias(helper.FindAttribute(attributes, "macroAlias")).Id);

				if (helper.FindAttribute(attributes, "macroAlias") == "")
					attributes.Add("macroAlias", m.Alias);


				try 
				{
					div += umbraco.macro.MacroContentByHttp(nodeId, versionId, attributes);
				} 
				catch 
				{
					div += "<span style=\"color: green\">No macro content available for WYSIWYG editing</span>";
				}


				div += macro.renderMacroEndTag();

				content = content.Replace(tag.Groups[1].Value, div);
			}

			content = formatMedia(content).Trim();
			content = content.Substring(1, content.Length-2);
			if (content == "")
				content = "<p></p>";
			contentHolder.Controls.Add(new System.Web.UI.LiteralControl(content));

			setCustomStyles();
		}

		private string formatMedia(string html) 
		{
			// Local media path
			string localMediaPath = getLocalMediaPath();

			// Find all media images
			string pattern = "<img [^>]*src=\"(?<mediaString>/media[^\"]*)\" [^>]*>";

			// A string containing the javascript events that should be appended to the image
			string imgResizeParams = "onresizeend=\"defaultStatus = ''; umbracoImageResize(this);\" onresizestart=umbracoImageResizeStart(this); onresize=umbracoImageResizeUpdateSize()";

			MatchCollection tags = Regex.Matches(html, pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
			foreach (Match tag in tags) 
				if (tag.Groups.Count > 0) 
				{
					// Replace /> to ensure we're in old-school html mode
					string tempTag = tag.Value.Replace("/>", ">");
					string orgSrc = tag.Groups["mediaString"].Value;

					// gather all attributes
					// TODO: This should be replaced with a general helper method - but for now we'll wanna leave umbraco.dll alone for this patch
					Hashtable ht = new Hashtable();
					MatchCollection m = Regex.Matches(tag.Value.Replace(">", " >"), "(?<attributeName>\\S*)=\"(?<attributeValue>[^\"]*)\"|(?<attributeName>\\S*)=(?<attributeValue>[^\"|\\s]*)\\s",  RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
					foreach (System.Text.RegularExpressions.Match attributeSet in m) 
					{
						ht.Add(attributeSet.Groups["attributeName"].Value.ToString(), attributeSet.Groups["attributeValue"].Value.ToString());
					}

					// Find the original filename, by removing the might added width and height
					orgSrc = orgSrc.Replace("_" + helper.FindAttribute(ht, "width") + "x" + helper.FindAttribute(ht, "height"), "").Replace("%20", " ");
                    
					// Check for either id or guid from media
					string mediaId = getIdFromSource(orgSrc, localMediaPath);

					cms.businesslogic.media.Media imageMedia = null;

					try 
					{
						int mId = int.Parse(mediaId);
						cms.businesslogic.property.Property p = new umbraco.cms.businesslogic.property.Property(mId);
						imageMedia = new umbraco.cms.businesslogic.media.Media(cms.businesslogic.Content.GetContentFromVersion(p.VersionId).Id);
					} 
					catch 
					{
						try 
						{
							imageMedia = new umbraco.cms.businesslogic.media.Media(cms.businesslogic.Content.GetContentFromVersion(new Guid(mediaId)).Id);
						} 
						catch {}
					}

					// Check with the database if any media matches this url
					if (imageMedia != null)
					{
						try 
						{
							// Format the tag
							tempTag = tempTag.Substring(0, tempTag.Length-1) + " " + imgResizeParams + " umbracoOrgWidth=\"" + imageMedia.getProperty("umbracoWidth").Value.ToString() + "\" umbracoOrgHeight=\"" + imageMedia.getProperty("umbracoHeight").Value.ToString() + "\" umbracoOrgFileName=\"" + orgSrc + "\"";
							if (bool.Parse(GlobalSettings.EditXhtmlMode))
								tempTag += "/";
							tempTag += ">";
			
							// Replace the tag
							html = html.Replace(tag.Value, tempTag);
						} 
						catch (Exception ee)
						{
							BusinessLogic.Log.Add(BusinessLogic.LogTypes.Error, BusinessLogic.User.GetUser(0), -1, "Error reading size data from media: " + imageMedia.Id.ToString() + ", " + ee.ToString());
						}
					}
					else
						BusinessLogic.Log.Add(BusinessLogic.LogTypes.Error, BusinessLogic.User.GetUser(0), -1, "Error reading size data from media (not found): " + orgSrc);
						
                    

				}
			return html;
		}

		private string getIdFromSource(string src, string localMediaPath) 
		{
			// important - remove out the umbraco path + media!
			src = src.Replace(localMediaPath, "");

			string _id = "";

			// Check for directory id naming 
			if (src.Length-src.Replace("/","").Length > 0) 
			{
				string[] dirSplit = src.Split("/".ToCharArray());
				string tempId = dirSplit[0];
				try 
				{
					// id
					_id = int.Parse(tempId).ToString();
				} 
				catch 
				{
					// guid
					_id = tempId;
				}
			}
			else 
			{
				string[] fileSplit = src.Replace("/media/","").Split("-".ToCharArray());

				// guid or id
				if (fileSplit.Length > 3) 
				{
					for(int i=0;i<5;i++)
						_id += fileSplit[i] + "-";
					_id = _id.Substring(0, _id.Length-1);
				}
				else
					_id = fileSplit[0];
			}

			return _id;
		}

		private string getLocalMediaPath() 
		{
			string[] umbracoPathSplit = umbraco.GlobalSettings.Path.Split("/".ToCharArray());
			string umbracoPath = "";
			for(int i=0;i<umbracoPathSplit.Length-1;i++)
				umbracoPath += umbracoPathSplit[i] + "/";
			return umbracoPath + "media/";
		}

		private void setCustomStyles()
		{
		
			foreach (cms.businesslogic.web.StyleSheet s in cms.businesslogic.web.StyleSheet.GetAll()) 
			{
				customStyles.Text += s.Content + "\n";
				foreach (cms.businesslogic.web.StylesheetProperty p in s.Properties)
					customStyles.Text += p.ToString(); 
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

		}
		#endregion
	}
}
