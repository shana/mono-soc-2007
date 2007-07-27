using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Xml;

namespace MonoCov {

public class EasyExporter {


	public string DestinationDir;

	public string StyleSheet;


	private XmlTextWriter writer;

	private CoverageModel model;

	private static string DefaultStyleSheet = "style.xsl";

	//private int itemCount;

	//private int itemsProcessed;
 
	public void Export (CoverageModel model) {

		this.model = model;

		if (model.hit + model.missed == 0)
			return;

		if (StyleSheet == null) {
			// Use default stylesheet
			using (StreamReader sr = new StreamReader (typeof (XmlExporter).Assembly.GetManifestResourceStream ("style.xsl"))) {
				using (StreamWriter sw = new StreamWriter (Path.Combine (DestinationDir, "style.xsl"))) {
					string line;
					while ((line = sr.ReadLine ()) != null)
						sw.WriteLine (line);
				}
			}
			using (Stream s = typeof (XmlExporter).Assembly.GetManifestResourceStream ("trans.gif")) {
				using (FileStream fs = new FileStream (Path.Combine (DestinationDir, "trans.gif"), FileMode.Create)) {
					byte[] buf = new byte[1024];
					int len = s.Read (buf, 0, buf.Length);
					fs.Write (buf, 0, len);
				}
			}

			StyleSheet = DefaultStyleSheet;
		}

		// Count items
		//itemCount = 1 + model.Classes.Count + model.Namespaces.Count;
		//itemsProcessed = 0;

		//WriteProject ();
		//WriteNamespaces ();
		WriteClasses ();
	}

	private void WriteStyleSheet () {
		// The standard says text/xml, while IE6 only understands text/xsl
		writer.WriteProcessingInstruction ("xml-stylesheet", "href=\"" + StyleSheet + "\" type=\"text/xsl\"");
	}

	private void WriteClasses () {
		string fileName = Path.Combine (DestinationDir, "output.xml");
		writer = new XmlTextWriter (fileName, Encoding.ASCII);
		writer.Formatting = Formatting.Indented;
		writer.WriteStartDocument ();
		WriteStyleSheet ();
		
		writer.WriteStartElement ("classes");
		foreach (ClassCoverageItem item in model.Classes.Values) 
		{
			if (!item.filtered || !(item.hit + item.missed == 0))
			{
				WriteClass (item);
			}
		}
		writer.WriteEndElement();

		writer.WriteEndDocument ();
		writer.WriteRaw ("\n");
		writer.Close ();
	}

	private void WriteClass (ClassCoverageItem item) {
		if (item.filtered)
			return;

		// start class
		writer.WriteStartElement ("class");
		writer.WriteAttributeString ("name", item.name);
		writer.WriteAttributeString ("fullname", item.FullName.Replace('/', '.'));
		writer.WriteAttributeString ("namespace", item.Namespace);
		WriteCoverage (item);
		
		//		start source
		writer.WriteStartElement ("source");

		if (item.sourceFile != null) 
		{
			writer.WriteAttributeString ("sourceFile", item.sourceFile.sourceFile);

			int[] coverage = item.sourceFile.Coverage;
			
			foreach( MethodCoverageItem method in item.Methods )
			{
				int sum = 0;
				int length = method.endLine - method.startLine;
				for( int index = method.startLine; index < method.endLine; index++ )
				{
					if( coverage[index] > 0 )
						sum++;
				}
				if( length == 0 )
					continue;
				
				writer.WriteStartElement ("Method");
				writer.WriteAttributeString ("type", item.FullName);
				writer.WriteAttributeString ("method", method.Name);
				writer.WriteAttributeString ("coverage", "" + ((float)sum)/length );
				writer.WriteAttributeString ("length", "" + length );
				writer.WriteEndElement ();
			}
		}
		else
		{
		}
		// end source
		writer.WriteEndElement ();
		// end class
		writer.WriteEndElement ();
	}

	private void WriteCoverage (CoverageItem item) {

		double coverage;
		if (item.hit + item.missed == 0)
			coverage = 1.0;
		else
			coverage = (double)item.hit / (item.hit + item.missed);

		string coveragePercent 
			= String.Format ("{0:###0}", coverage * 100);

		writer.WriteStartElement ("coverage");
		writer.WriteAttributeString ("hit", item.hit.ToString ());
		writer.WriteAttributeString ("missed", item.missed.ToString ());
		writer.WriteAttributeString ("coverage", coveragePercent);
		writer.WriteEndElement ();
	}
}
}
