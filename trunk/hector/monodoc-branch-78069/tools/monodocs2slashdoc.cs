using System;
using System.Collections;
using System.IO;
using System.Xml;

public class Monodocs2SlashDoc {
	
	public static void Main(string[] args) {
		if (args.Length != 1) {
			Console.WriteLine("Usage:  monodocs2slashdoc.exe docsdir");
			Console.WriteLine();
			Console.WriteLine("Converts Monodoc-style API document to /doc format for use with");
			Console.WriteLine("tools such as NDoc.");
			Console.WriteLine();
			Console.WriteLine("A NDoc-style XML file will be created for each assembly documented");
			Console.WriteLine("in docsdir.  The new files will be created in the current directory");
			Console.WriteLine("and will be named after the assemblies.  Additionally, a");
			Console.WriteLine("NamespaceSummaries.xml file will be written to the current directory.");
			return;
		}
		
		string basepath = args[0];
		
		if (System.Environment.CurrentDirectory == System.IO.Path.GetFullPath(basepath)) {
			Console.WriteLine("Don't run this tool from your documentation directory, since some files could be accidentally overwritten.");
			return;
		}
		
		Hashtable outputfiles = new Hashtable(); // map assembly name to members node of appropriate document
		
		XmlDocument index_doc = new XmlDocument();
		index_doc.Load(Path.Combine(basepath, "index.xml"));
		XmlElement index = index_doc.DocumentElement;
		
		foreach (XmlElement assmbly in index.SelectNodes("Assemblies/Assembly")) {
			XmlDocument output = new XmlDocument();
			XmlElement output_root = output.CreateElement("doc");
			output.AppendChild(output_root);

			XmlElement output_assembly = output.CreateElement("assembly");
			output_root.AppendChild(output_assembly);
			XmlElement output_assembly_name = output.CreateElement("name");
			output_assembly.AppendChild(output_assembly_name);
			string assemblyName = assmbly.GetAttribute("Name");
			output_assembly_name.InnerText = assemblyName;
		
			XmlElement members = output.CreateElement("members");
			output_root.AppendChild(members);
			
			outputfiles[assemblyName] = members;
		}
			
		foreach (XmlElement nsnode in index.SelectNodes("Types/Namespace")) {
			string ns = nsnode.GetAttribute("Name");
			foreach (XmlElement typedoc in nsnode.SelectNodes("Type")) {
				string typename = typedoc.GetAttribute("Name");
				XmlDocument type = new XmlDocument();
				type.Load(Path.Combine(Path.Combine(basepath, ns), typename) + ".xml");
				
				string assemblyname = type.SelectSingleNode("Type/AssemblyInfo/AssemblyName").InnerText;
				XmlElement members = (XmlElement)outputfiles[assemblyname];
				if (members == null) continue; // assembly is strangely not listed in the index
				
				CreateMember("T:" + type.SelectSingleNode("Type/@FullName").InnerText,
					type.DocumentElement, members);
					
				foreach (XmlElement memberdoc in type.SelectNodes("Type/Members/Member")) {
					string name = type.SelectSingleNode("Type/@FullName").InnerText;
					switch (memberdoc.SelectSingleNode("MemberType").InnerText) {
						case "Constructor":
							name = "C:" + name + ".#ctor";
							break;
						case "Method":
							name = "M:" + name + "." + memberdoc.GetAttribute("MemberName") + MakeArgs(memberdoc);
							if (memberdoc.GetAttribute("MemberName") == "op_Implicit" || memberdoc.GetAttribute("MemberName") == "op_Explicit")
								name += "~" + memberdoc.SelectSingleNode("ReturnValue/ReturnType").InnerText;
							break;
						case "Property":
							name = "P:" + name + "." + memberdoc.GetAttribute("MemberName") + MakeArgs(memberdoc);
							break;
						case "Field":
							name = "F:" + name + "." + memberdoc.GetAttribute("MemberName");
							break;
						case "Event":
							name = "E:" + name + "." + memberdoc.GetAttribute("MemberName");
							break;
					}
					
					CreateMember(name, memberdoc, members);
				}
			}
		}
		
		// Write out each of the assembly documents
		foreach (string assemblyName in outputfiles.Keys) {
			XmlElement members = (XmlElement)outputfiles[assemblyName];
			Console.WriteLine(assemblyName + ".xml");
			using(StreamWriter sw = new StreamWriter(assemblyName + ".xml")) {
				WriteXml(members.OwnerDocument.DocumentElement, sw);
			}
		}
	
		// Write out a namespace summaries file.
		XmlDocument nsSummaries = new XmlDocument();
		nsSummaries.LoadXml("<namespaces/>");
		foreach (XmlElement nsnode in index.SelectNodes("Types/Namespace")) {
			AddNamespaceSummary(nsSummaries, basepath, nsnode.GetAttribute("Name"));
		}
		Console.WriteLine("NamespaceSummaries.xml");
		using(StreamWriter writer = new StreamWriter("NamespaceSummaries.xml")) {
			WriteXml(nsSummaries.DocumentElement, writer);
		}
	}
	
	private static void AddNamespaceSummary(XmlDocument nsSummaries, string basepath, string currentNs) {
		string filename = Path.Combine(basepath, currentNs + ".xml");
		if (File.Exists(filename)) 	{
			XmlDocument nsSummary = new XmlDocument();
			nsSummary.Load(filename);
			XmlElement ns = nsSummaries.CreateElement("namespace");
			nsSummaries.DocumentElement.AppendChild(ns);
			ns.SetAttribute("name", currentNs);
			ns.InnerText = nsSummary.SelectSingleNode("/Namespace/Docs/summary").InnerText;
		}
	}
	
	private static void CreateMember(string name, XmlElement input, XmlElement output) {
		XmlElement member = output.OwnerDocument.CreateElement("member");
		output.AppendChild(member);
		
		member.SetAttribute("name", name);
		
		foreach (XmlNode docnode in input.SelectSingleNode("Docs"))
			member.AppendChild(output.OwnerDocument.ImportNode(docnode, true));
	}
	
	private static string MakeArgs(XmlElement member) {
		string ret = "";
		foreach (XmlElement p in member.SelectNodes("Parameters/Parameter")) {
			if (ret != "") ret += ",";
			ret += p.GetAttribute("Type").Replace("&", "@");
		}
		if (ret != "")
			ret = "(" + ret + ")";
		return ret;
	}

	private static void WriteXml(XmlElement element, System.IO.TextWriter output) {
		XmlTextWriter writer = new XmlTextWriter(output);
		writer.Formatting = Formatting.Indented;
		writer.Indentation = 2;
		writer.IndentChar = ' ';
		element.WriteTo(writer);
		output.WriteLine();	
	}
	
}
