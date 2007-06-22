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
	private XmlDocument document;
	private bool valid;
	private string text;
	
	public MonoDocument (string filePath)
	{
		EcmaReader reader = new EcmaReader (filePath);
		document = reader.Document;
		valid = reader.Valid;
		
		using (FileStream filestream= new FileStream (filePath, FileMode.Open)) {
			using (StreamReader stream = new StreamReader (filestream)) {
				text = stream.ReadToEnd ();
			}
		}
	}
	
	public string Text {
		get {
			return text;
		}
	}
		
	public bool Valid {
		get {
			return valid;
		}
	}
}
}