//
// MonoDocument.cs: Class that represents an Monodoc XML Documentation.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M

using System;
using System.Xml;

namespace Monodoc.Editor.Utils {
public class MonoDocument {
	private XmlDocument document;
	private bool valid;
	
	public MonoDocument (string filePath)
	{
		EcmaReader reader = new EcmaReader (filePath);
		document = reader.Document;
		valid = reader.Valid;
	}
	
	public string Text {
		get {
			return document.OuterXml;
		}
	}
		
	public bool Valid {
		get {
			return valid;
		}
	}
}
}