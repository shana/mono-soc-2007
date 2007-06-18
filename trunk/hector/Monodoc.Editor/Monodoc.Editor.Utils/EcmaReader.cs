//
// EcmaReader.cs: Class that reads an Monodoc XML Documentation.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M

using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;

namespace Monodoc.Editor.Utils {
public class EcmaReader {
	private XmlDocument document;
	private bool val_success = true;
	
	public EcmaReader (string filePath)
	{
		if (!File.Exists (filePath))
			throw new ArgumentException ("Error: An invalid path was given.");
		
		document = new XmlDocument ();
		
		try {
			XmlTextReader textReader = new XmlTextReader (filePath);
			document.Load (GetValidatingReader (textReader));
			
			#if DEBUG
			Console.WriteLine ("DEBUG: Validando estilo. La validacion fue {0}", 
			(val_success == true ? "exitosa!" : "no exitosa."));
			#endif
			
		} catch (FileNotFoundException e) {
			Console.WriteLine ("Error: {0} not found.", e.FileName);
			Environment.Exit (1);
		}
	}
	
	public XmlDocument Document {
		get {
			return document;
		}
	}
	
	public bool Valid {
		get {
			return val_success;
		}
	}
	
	private XmlValidatingReader GetValidatingReader (XmlReader reader)
	{
		XmlValidatingReader valReader;
		XmlSchema schema;
		Stream xsdStream;
		Type type;
		
		valReader = new XmlValidatingReader (reader);
		valReader.ValidationType = ValidationType.Schema;
		
		// Set the validation event handler
		valReader.ValidationEventHandler += new ValidationEventHandler (ValidationCallBack);
		
		type = Type.GetType ("Monodoc.Editor.Utils.EcmaReader");
		xsdStream = Assembly.GetAssembly (type).GetManifestResourceStream ("monodoc-ecma.xsd");
		
		schema = XmlSchema.Read (xsdStream, ValidationCallBack);
		schema.Compile (null);
		valReader.Schemas.Add (schema);
		
		return valReader;
	}
	
	private void ValidationCallBack (object sender, ValidationEventArgs args)
  	{
  		val_success = false;
  		Console.WriteLine("\r\n\tValidation error: " + args.Message );
  	}
}
}