//
// A provider to display man pages
//
// Authors:
//   Johannes Roith <johannes@roith.de>

namespace Monodoc { 
using System;
using System.IO;
using System.Xml;

//
// The simple provider generates the information source
//
public class ManProvider : Provider {
	string[] tocFiles;
	
	public  ManProvider (string[] handbookTocFiles)
	{

		tocFiles = handbookTocFiles;

		// huh...
		if (!File.Exists (tocFiles[0]))
			throw new FileNotFoundException (String.Format ("The table of contents, `{0}' does not exist", tocFiles[0]));
		
	}

	public override void PopulateTree (Tree tree)
	{

		foreach(string TocFile in tocFiles) {

			XmlDocument doc = new XmlDocument();
			doc.Load(TocFile);

			XmlNodeList nodeList = doc.GetElementsByTagName("manpage");
			Node nodeToAddChildrenTo = tree;

			foreach(XmlNode node in nodeList) {

			XmlAttribute name = node.Attributes["name"];
			XmlAttribute page = node.Attributes["page"];

				if (name == null || page == null) continue;

				string target = "man:" + page.Value;
				nodeToAddChildrenTo.CreateNode (name.Value, target);

					if (File.Exists(page.Value))
						nodeToAddChildrenTo.tree.HelpSource.PackFile (page.Value, page.Value);
				}
		}
	}


	public override void CloseTree (HelpSource hs, Tree tree)
	{
	}
}

//
// The HelpSource is used during the rendering phase.
//

public class ManHelpSource : HelpSource {
	
	public ManHelpSource (string base_file, bool create) : base (base_file, create) {}
	protected const string MAN_PREFIX = "man:";
	
	public override string GetText (string url, out Node match_node)
	{
		match_node = null;
		if (url.IndexOf (MAN_PREFIX) > -1)
			return GetTextFromUrl (url);

		return null;
	}
	
	protected string GetTextFromUrl (string url)
	{
		// Remove "man:" prefix including any help-source id on the front.
		int prefixStart = url.IndexOf(MAN_PREFIX);
		if (prefixStart > -1)
			url = url.Substring (prefixStart + 4);


		if (url == null)
		{
			Console.WriteLine("Warning, NULL url!");
			return "<html>url was null</html>";
		}

		string output = "";
		Stream s = GetHelpStream (url);
	        StreamReader file;
		file = new StreamReader(s);
                bool previoustp = false;
		bool headingdisplayed = false;

                while (file.Peek() != -1)
                {
                        string line = file.ReadLine();
			try {

                        string tmp = line.Substring(0,3);

                        switch(tmp.ToLower()) {
                                                                                                                                                 
                        case ".pp":
				line = "<br /> <br />" + line.Substring(3, line.Length - 3);
                        break;
                                                                                                                                                
                        case ".tp":
				line = "<br />"; //"<br /> <br />" + line.Substring(3, line.Length - 3);
                        break;
                                                  
                             
                        case ".th":


				line = "<table width=\"100%\"><tr bgcolor=\"#b0c4da\"><td>Manual Pages<h3>" + line.Substring(3, line.Length - 3) + "" + "</h3></td></tr></table>";


			headingdisplayed = true;
                        break;
                      
                        case ".sh":
				line = "<br /></blockquote><h2>" + line.Substring(3, line.Length - 3) + "</h2><blockquote>";
                        break;
                                                   
                        case ".br":
				line = line.Substring(3, line.Length - 3) + "<br />";
                        break;      

                        case ".ss":
                                line = "<br /><b>" + line.Substring(3, line.Length - 3) + "</b><br />";
                        break;

                        case ".\\\"":
				line = "";
                        break;

                        case ".de":
				line = "";
                        break;
                         
                                                                                
                        case ".nf":
                                line = "<blockquote><pre>";
                        break;
                                                                                
                        case ".fi":
				line = "</blockquote></pre>";
			break;

                        case ".sp":
                                line = "<br />";
                        break;

                        case ".ns":
                                line = "";
                        break;
                                                                                
                        case ".ig":
                                line = "";
                        break;
                                                                                
                        case ".ie":
                                line = "";
                        break;
                                               
                        case ".el":
                                line = "";
                        break;
                                                                                
                        case ".if":
				line = "";
                        break;
                                             
                        }

                        string tmp2 = line.Substring(0,2);

                        switch(tmp2.ToLower()) {
                                                                                                  
                        case "..":
				line = "";
                        break;         
                                                                                                        
                        case ".i":
/*				if (previoustp) {
				line = "<br /> <u>" + line.Substring(2, line.Length - 2) + "</u><br /><br /> ";

				}
				else {
*/					line = " <u>" + line.Substring(2, line.Length - 2) + "</u> ";
//				}
                        break;
                                                                         
                        case ".b":
				line = "<b>" + line.Substring(2, line.Length - 2) + "</b> ";
                        break;
                        }
			if (previoustp) {
				line = "<br /><br />" + line + "<br /><br />";
			}
                                if (tmp.ToLower() == ".tp") {
                                                previoustp = true;
                                }
                                else {
                                        previoustp = false;
                               }
                                                                                

			}
			catch {}

			// This needs to be improved

                        line = line.Replace("\\fI", "<u>");
			line = line.Replace("\\fB", "<b>");
			line = line.Replace("\\-", " - ");
                        line = line.Replace("\\//", "/");
			line = line.Replace("\n", "<br />");
			// fR does not really close>, 
			// never twice, just a fix.
			// Hopefully most browsers will render this.

                        line = line.Replace("\\fP", "</u>&nbsp;");
			line = line.Replace("\\fR", "</b></b>&nbsp;");

			line = line.Replace("\\'", "'");

			// must be last one.
			line = line.Replace("\\\\", "\\");

                        if (!headingdisplayed)
                                line = "";

		output += line + "\n";

			}

			return output;
		}

	}
}
