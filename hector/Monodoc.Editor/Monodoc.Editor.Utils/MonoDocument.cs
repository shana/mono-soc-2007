//
// MonoDocument.cs: Class that represents an Monodoc XML Documentation.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M

using System;
using System.Xml;
using System.IO;

namespace Monodoc.Editor.Utils {
public class MonoDocument {
	private bool valid_document;
	private string xml_content;
	private string filename;
	
	public MonoDocument (string filePath)
	{
		EcmaReader reader = new EcmaReader (filePath);
		valid_document = reader.Valid;
		
		if (!valid_document)
			throw new ArgumentException ("Error: Invalid XML document.", "filePath");
			
		filename = Path.GetFileName (filePath);
		using (FileStream filestream= new FileStream (filePath, FileMode.Open)) {
			using (StreamReader stream = new StreamReader (filestream)) {
				xml_content = stream.ReadToEnd ();
			}
		}
	}
	
	public string Xml {
		get {
			return xml_content;
		}
	}
		
	public bool IsValid {
		get {
			return valid_document;
		}
	}
	
	public string Filename {
		get {
			return filename;
		}
	}
}
}