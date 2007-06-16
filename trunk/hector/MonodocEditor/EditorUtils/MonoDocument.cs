//
// MonoDocument.cs: Class that reads an Monodoc XML Documentation.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M

using System;
using System.Xml;

namespace Monodoc.EditorUtils {
public class MonoDocument {
	private XmlDocument document;
	private XmlNode root_element;
	private bool valid;
	
	public MonoDocument (string filePath)
	{
		EcmaReader reader = new EcmaReader (filePath);
		document = reader.Document;
		root_element = reader.Document.DocumentElement;
		valid = reader.Valid;
	}
	
	public bool Valid {
		get {
			return valid;
		}
	}
	
	public void Convert ()
	{
		Console.WriteLine (ConvertRoot ());
	}
	
	private string ConvertRoot () 
	{
		string result = "";
		string temp;
		
		XmlElement root = root_element as XmlElement;
		temp = "\nBegins Document:\n" + root_element.Name + "\n";
		
		if (root.HasAttributes) {
			foreach (XmlNode node in root.Attributes) {
				temp += "\tAttribute " + node.Name + ": " + node.InnerText + "\n";
			}
		}
		
		result = temp + ConvertRootChild (root.ChildNodes) ;
		
		return result;
	}
	
	private string ConvertRootChild (XmlNodeList childNodes)
	{
		string result = "";
		
		foreach (XmlNode node in childNodes) {
			switch (node.Name) {
				case "TypeSignature":
					result += ConvertSignature (node);
					break;
				default:
					break;
			}
		}
		
		return result;
	}
	
	private string ConvertSignature (XmlNode node)
	{
		string result = "";
		return result;
	}
}
}