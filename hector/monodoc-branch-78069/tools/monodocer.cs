// Updater program for syncing Mono's ECMA-style documentation files
// with an assembly.
// By Joshua Tauberer <tauberer@for.net>

using System;
using System.Collections;
#if !NET_1_0
using System.Collections.Generic;
#endif
using System.Globalization;
using System.Text;
using System.Reflection;
using System.Xml;

using Mono.GetOptions;

[assembly: AssemblyTitle("Monodocer - The Mono Documentation Tool")]
[assembly: AssemblyCopyright("Copyright (c) 2004 Joshua Tauberer <tauberer@for.net>\nreleased under the GPL.")]
[assembly: AssemblyDescription("A tool for creating and updating Mono XML documentation files for assemblies.")]

[assembly: Mono.UsageComplement("")]

public class Stub {
	
	static string srcPath;
	static Assembly[] assemblies;
	
	static bool nooverrides = true, delete = false, ignoremembers = false;
	static bool pretty = false;
	static bool ignore_extra_docs = false;
	
	static int additions = 0, deletions = 0;

	static string name;
	static XmlDocument slashdocs;
	static XmlDocument ecmadocs;

	static string since;

	static MemberFormatter csharpFullFormatter  = new CSharpFullMemberFormatter ();
	static MemberFormatter csharpFormatter      = new CSharpMemberFormatter ();
	static MemberFormatter docTypeFullFormatter = new DocTypeFullMemberFormatter ();
	static MemberFormatter docTypeFormatter     = new DocTypeMemberFormatter ();
	static MemberFormatter slashdocFormatter    = new SlashDocMemberFormatter ();
	static MemberFormatter filenameFormatter    = new FileNameMemberFormatter ();
	
	private class Opts : Options {
		[Option("The root {directory} of an assembly's documentation files.")]
		public string path = null;

		[Option("When updating documentation, write the updated files to this {path}.")]
		public string updateto = null;

		[Option(-1, "The assembly to document.  Specify a {file} path or the name of a GAC'd assembly.")]
		public string[] assembly = null;

		[Option("Document only the {type name}d by this argument.")]
		public string type = null;

		[Option("Update only the types in this {namespace}.")]
		public string @namespace = null;

		[Option("Allow monodocer to delete members from files.")]
		public bool delete = false;

		[Option("Include overridden methods in documentation.")]
		public bool overrides = false;

		[Option("Don't update members.")]
		public bool ignoremembers = false;

		[Option("Don't rename documentation XML files for missing types.")]
		public bool ignore_extra_docs = false;

		[Option("The {name} of the project this documentation is for.")]
		public string name;

		[Option("An XML documemntation {file} made by the /doc option of mcs/csc the contents of which will be imported.")]
		public string importslashdoc;
		
		[Option("An ECMA or monodoc-generated XML documemntation {file} to import.")]
		public string importecmadoc;

		[Option("Indent the XML files nicely.")]
		public bool pretty = false;
		
		[Option("Create a <since/> element for added types/members with the value {since}.")]
		public string since;
	}
	
	private static int n(object o) {
		return o == null ? 0 : 1;
	}
	
	public static void Main(string[] args) {
		Opts opts = new Opts();
		opts.ProcessArgs(args);

		if (args.Length == 0) {
			opts.DoHelp();
			return;
		}
		
		nooverrides = !opts.overrides;
		delete = opts.delete;
		ignoremembers = opts.ignoremembers;
		name = opts.name;
		pretty = opts.pretty;
		since = opts.since;
		ignore_extra_docs = opts.ignore_extra_docs;

		try {
			// PARSE BASIC OPTIONS AND LOAD THE ASSEMBLY TO DOCUMENT
			
			if (opts.path == null)
				throw new InvalidOperationException("The path option is required.");
			
			srcPath = opts.path;

			if (n(opts.type) + n(opts.@namespace) > 1)
				throw new InvalidOperationException("You cannot specify both 'type' and 'namespace'.");
			
			if (opts.assembly == null)
				throw new InvalidOperationException("The assembly option is required.");
				
			assemblies = new Assembly [opts.assembly.Length];
			for (int i = 0; i < opts.assembly.Length; i++)
				assemblies [i] = LoadAssembly (opts.assembly [i]);
				
			// IMPORT FROM /DOC?
			
			if (opts.importslashdoc != null) {
				try {
					slashdocs = new XmlDocument();
					slashdocs.Load(opts.importslashdoc);
				} catch (Exception e) {
					Console.Error.WriteLine("Could not load /doc file: " + e.Message);
					Environment.ExitCode = 1;
					return;
				}
			}
			
			if (opts.importecmadoc != null) {
				try {
					ecmadocs = new XmlDocument();
					ecmadocs.Load(opts.importecmadoc);
				} catch (Exception e) {
					Console.Error.WriteLine("Could not load XML file: " + e.Message);
					Environment.ExitCode = 1;
					return;
				}
			}
			
			// PERFORM THE UPDATES
			
			if (opts.type != null)
				DoUpdateType(opts.path, opts.type, opts.updateto);
			else if (opts.@namespace != null)
				DoUpdateNS(opts.@namespace, opts.path + "/" + opts.@namespace, opts.updateto == null ? opts.path + "/" + opts.@namespace : opts.updateto + "/" + opts.@namespace);
			else
				DoUpdateAssemblies(opts.path, opts.updateto == null ? opts.path : opts.updateto);
		
		} catch (InvalidOperationException error) {
			Console.Error.WriteLine(error.Message);
			Environment.ExitCode = 1;
			return;
			
		} catch (System.IO.IOException error) {
			Console.Error.WriteLine(error.Message);
			Environment.ExitCode = 1;
			return;

		} catch (Exception error) {
			Console.Error.WriteLine(error);
			Environment.ExitCode = 1;
		}

		Console.Error.WriteLine("Members Added: {0}, Members Deleted: {1}", additions, deletions);
	}
	
	private static Assembly LoadAssembly (string name)
	{
		Assembly assembly = null;
		try {
			assembly = Assembly.LoadFile (name);
		} catch (System.IO.FileNotFoundException) { }

		if (assembly == null) {
			try {
				assembly = Assembly.LoadWithPartialName (name);
			} catch (Exception) { }
		}
			
		if (assembly == null)
			throw new InvalidOperationException("Assembly " + name + " not found.");

		return assembly;
	}

	private static void WriteXml(XmlElement element, System.IO.TextWriter output) {
		OrderTypeAttributes (element);
		XmlTextWriter writer = new XmlTextWriter(output);
		writer.Formatting = Formatting.Indented;
		writer.Indentation = 2;
		writer.IndentChar = ' ';
		element.WriteTo(writer);
		output.WriteLine();	
	}

	private static void OrderTypeAttributes (XmlElement e)
	{
		foreach (XmlElement type in e.SelectNodes ("//Type")) {
			OrderTypeAttributes (type.Attributes);
		}
	}

	static readonly string[] TypeAttributeOrder = {
		"Name", "FullName", "FullNameSP", "Maintainer"
	};

	private static void OrderTypeAttributes (XmlAttributeCollection c)
	{
		XmlAttribute[] attrs = new XmlAttribute [TypeAttributeOrder.Length];
		for (int i = 0; i < c.Count; ++i) {
			XmlAttribute a = c [i];
			for (int j = 0; j < TypeAttributeOrder.Length; ++j) {
				if (a.Name == TypeAttributeOrder [j]) {
					attrs [j] = a;
					break;
				}
			}
		}
		for (int i = attrs.Length-1; i >= 0; --i) {
			XmlAttribute n = attrs [i];
			if (n == null)
				continue;
			XmlAttribute r = null;
			for (int j = i+1; j < attrs.Length; ++j) {
				if (attrs [j] != null) {
					r = attrs [j];
					break;
				}
			}
			if (r == null)
				continue;
			c.Remove (n);
			c.InsertBefore (n, r);
		}
	}
	
	private static XmlDocument CreateIndexStub() {
		XmlDocument index = new XmlDocument();

		XmlElement index_root = index.CreateElement("Overview");
		index.AppendChild(index_root);

		if (assemblies.Length == 0)
			throw new Exception ("No assembly");

		XmlElement index_assemblies = index.CreateElement("Assemblies");
		index_root.AppendChild(index_assemblies);

		XmlElement index_remarks = index.CreateElement("Remarks");
		index_remarks.InnerText = "To be added.";
		index_root.AppendChild(index_remarks);

		XmlElement index_copyright = index.CreateElement("Copyright");
		index_copyright.InnerText = "To be added.";
		index_root.AppendChild(index_copyright);

		XmlElement index_types = index.CreateElement("Types");
		index_root.AppendChild(index_types);
		
		return index;
	}
	
	private static void WriteNamespaceStub(string ns, string outdir) {
		XmlDocument index = new XmlDocument();

		XmlElement index_root = index.CreateElement("Namespace");
		index.AppendChild(index_root);
		
		index_root.SetAttribute("Name", ns);

		XmlElement index_docs = index.CreateElement("Docs");
		index_root.AppendChild(index_docs);

		XmlElement index_summary = index.CreateElement("summary");
		index_summary.InnerText = "To be added.";
		index_docs.AppendChild(index_summary);

		XmlElement index_remarks = index.CreateElement("remarks");
		index_remarks.InnerText = "To be added.";
		index_docs.AppendChild(index_remarks);

		using (System.IO.TextWriter writer
		= new System.IO.StreamWriter(new System.IO.FileStream(outdir + "/" + ns + ".xml", System.IO.FileMode.CreateNew))) {
			WriteXml(index.DocumentElement, writer);
		}
	}
	
	public static void DoUpdateType(string basepath, string typename, string dest) {
		Type type = null;
		foreach (Assembly assembly in assemblies) {
			type = assembly.GetType(typename, false);
			if (type != null)
				break;
		}
		if (type == null) throw new InvalidOperationException("Type not found: " + typename);
		if (type.Namespace == null) throw new InvalidOperationException("Types in the root namespace cannot be documented: " + typename);
		string typename2 = GetTypeFileName (type);

		XmlDocument basefile = new XmlDocument();
		if (!pretty) basefile.PreserveWhitespace = true;
		string typefile = basepath + "/" + type.Namespace + "/" + typename2 + ".xml";
		try {
			basefile.Load(typefile);
		} catch (Exception e) {
			throw new InvalidOperationException("Error loading " + typefile + ": " + e.Message, e);
		}
		
		if (dest == null) {
			DoUpdateType2(basefile, type, typefile);
		} else if (dest == "-") {
			DoUpdateType2(basefile, type, null);
		} else {
			DoUpdateType2(basefile, type, dest + "/" + type.Namespace + "/" + typename2 + ".xml");
		}	
	}

	
	public static void DoUpdateNS(string ns, string nspath, string outpath) {
		Hashtable seenTypes = new Hashtable();
		Assembly assembly = assemblies [0];
		
		foreach (System.IO.FileInfo file in new System.IO.DirectoryInfo(nspath).GetFiles("*.xml")) {
			XmlDocument basefile = new XmlDocument();
			if (!pretty) basefile.PreserveWhitespace = true;
			string typefile = nspath + "/" + file.Name;
			try {
				basefile.Load(typefile);
			} catch (Exception e) {
				throw new InvalidOperationException("Error loading " + typefile + ": " + e.Message, e);
			}

			string typename = 
				GetTypeFileName (basefile.SelectSingleNode("Type/@FullName").InnerText);
			Type type = assembly.GetType(typename, false);
			if (type == null) {
				Console.Error.WriteLine(typename + " is no longer in the assembly.");
				continue;
			}			

			seenTypes[type] = seenTypes;
			DoUpdateType2(basefile, type, outpath + "/" + file.Name);
		}
		
		// Stub types not in the directory
		foreach (Type type in assembly.GetTypes()) {
			if (type.Namespace == null) continue;
			if (type.Namespace != ns || seenTypes.ContainsKey(type))
				continue;

			XmlElement td = StubType(type);
			if (td == null) continue;
			
			Console.Error.WriteLine("New Type: " + type.FullName);

			using (System.IO.TextWriter writer
			= new System.IO.StreamWriter(new System.IO.FileStream(outpath + "/" + GetTypeFileName(type) + ".xml", System.IO.FileMode.CreateNew))) {
				WriteXml(td, writer);
			}
			
		}
	}
	
	private static string GetTypeFileName(Type type) {
		return filenameFormatter.GetName (type);
	}

	public static string GetTypeFileName (string typename)
	{
		StringBuilder filename = new StringBuilder (typename.Length);
		int numArgs = 0;
		int numLt = 0;
		bool copy = true;
		for (int i = 0; i < typename.Length; ++i) {
			char c = typename [i];
			switch (c) {
				case '<':
					copy = false;
					++numLt;
					break;
				case '>':
					--numLt;
					if (numLt == 0) {
						filename.Append ('`').Append ((numArgs+1).ToString());
						numArgs = 0;
						copy = true;
					}
					break;
				case ',':
					if (numLt == 1)
						++numArgs;
					break;
				default:
					if (copy)
						filename.Append (c);
					break;
			}
		}
		return filename.ToString ();
	}

	
	private static void AddIndexAssembly (Assembly assembly, XmlElement parent)
	{
		XmlElement index_assembly = parent.OwnerDocument.CreateElement("Assembly");
		index_assembly.SetAttribute("Name", assembly.GetName().Name);
		index_assembly.SetAttribute("Version", assembly.GetName().Version.ToString());
		MakeAttributes(index_assembly, assembly, true);
		parent.AppendChild(index_assembly);
	}

	private static void DoUpdateAssemblies (string source, string dest) 
	{
		string indexfile = dest + "/index.xml";
		XmlDocument index;
		if (System.IO.File.Exists(indexfile)) {
			index = new XmlDocument();
			index.Load(indexfile);

			// Format change
			ClearElement(index.DocumentElement, "Assembly");
			ClearElement(index.DocumentElement, "Attributes");
		} else {
			index = CreateIndexStub();
		}
		
		if (name == null) {
			string defaultTitle = "Untitled";
			if (assemblies.Length == 1)
				defaultTitle = assemblies[0].GetName().Name;
			WriteElementInitialText(index.DocumentElement, "Title", defaultTitle);
		} else {
			WriteElementText(index.DocumentElement, "Title", name);
		}
		
		XmlElement index_types = WriteElement(index.DocumentElement, "Types");
		XmlElement index_assemblies = WriteElement(index.DocumentElement, "Assemblies");
		index_assemblies.RemoveAll ();
		
		Hashtable goodfiles = new Hashtable();

		foreach (Assembly assm in assemblies) {
			AddIndexAssembly (assm, index_assemblies);
			DoUpdateAssembly (assm, index_types, source, dest, goodfiles);
		}
		
		CleanupFiles (dest, goodfiles);
		CleanupIndexTypes (index_types, goodfiles);

		using (System.IO.TextWriter writer
			= new System.IO.StreamWriter(new System.IO.FileStream(indexfile, System.IO.FileMode.Create)))
			WriteXml(index.DocumentElement, writer);
	
	}
		
	private static void DoUpdateAssembly (Assembly assembly, XmlElement index_types, string source, string dest, Hashtable goodfiles) 
	{
		foreach (Type type in assembly.GetTypes()) {
			if (type.Namespace == null) continue;
			if (!IsPublic (type)) continue;
			
			// Must get the A+B form of the type name.
			string typename = GetTypeFileName(type);
			
			string typefile = source + "/" + type.Namespace + "/" + typename + ".xml";
			System.IO.FileInfo file = new System.IO.FileInfo(typefile);
			if (file.Exists) {
				// Update
				XmlDocument basefile = new XmlDocument();
				if (!pretty) basefile.PreserveWhitespace = true;
				try {
					basefile.Load(typefile);
				} catch (Exception e) {
					throw new InvalidOperationException("Error loading " + typefile + ": " + e.Message, e);
				}
				
				DoUpdateType2(basefile, type, dest + "/" + type.Namespace + "/" + typename + ".xml");
			} else {
				// Stub
				XmlElement td = StubType(type);
				if (td == null) continue;
				
				System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(dest + "/" + type.Namespace);
				if (!dir.Exists) {
					dir.Create();
					Console.Error.WriteLine("Namespace Directory Created: " + type.Namespace);
				}

				Console.Error.WriteLine("New Type: " + type.FullName);

				using (System.IO.TextWriter writer
				= new System.IO.StreamWriter(new System.IO.FileStream(dest + "/" + type.Namespace + "/" + typename + ".xml", System.IO.FileMode.CreateNew))) {
					WriteXml(td, writer);
				}
			}
			
			// Add namespace and type nodes into the index file as needed
			XmlElement nsnode = (XmlElement)index_types.SelectSingleNode("Namespace[@Name='" + type.Namespace + "']");
			if (nsnode == null) {
				nsnode = index_types.OwnerDocument.CreateElement("Namespace");
				nsnode.SetAttribute("Name", type.Namespace);
				index_types.AppendChild(nsnode);
			}
			string doc_typename = GetDocTypeName (type);
			XmlElement typenode = (XmlElement)nsnode.SelectSingleNode("Type[@Name='" + typename + "']");
			if (typenode == null) {
				typenode = index_types.OwnerDocument.CreateElement("Type");
				typenode.SetAttribute("Name", typename);
				nsnode.AppendChild(typenode);
			}
			if (typename != doc_typename)
				typenode.SetAttribute("DisplayName", doc_typename);
			else
				typenode.RemoveAttribute("DisplayName");
				
			// Ensure the namespace index file exists
			if (!new System.IO.FileInfo(dest + "/" + type.Namespace + ".xml").Exists) {
				Console.Error.WriteLine("New Namespace File: " + type.Namespace);
				WriteNamespaceStub(type.Namespace, dest);
			}

			// mark the file as corresponding to a type
			goodfiles[type.Namespace + "/" + typename + ".xml"] = goodfiles;
		}
	}

	private static bool IsPublic (Type type)
	{
		Type decl = type;
		while (decl != null) {
			if (!(decl.IsPublic || decl.IsNestedPublic)) {
				return false;
			}
			decl = decl.DeclaringType;
		}
		return true;
	}

	private static void CleanupFiles (string dest, Hashtable goodfiles)
	{
		// Look for files that no longer correspond to types
		if (ignore_extra_docs)
			return;
		foreach (System.IO.DirectoryInfo nsdir in new System.IO.DirectoryInfo(dest).GetDirectories("*")) {
			foreach (System.IO.FileInfo typefile in nsdir.GetFiles("*.xml")) {
				if (!goodfiles.ContainsKey(nsdir.Name + "/" + typefile.Name)) {
					string newname = typefile.FullName + ".remove";
					try { System.IO.File.Delete(newname); } catch (Exception) { }
					try { typefile.MoveTo(newname); } catch (Exception) { }					
					Console.Error.WriteLine("Class no longer present; file renamed: " + nsdir.Name + "/" + typefile.Name);
				}
			}
		}
	}
		
	private static void CleanupIndexTypes (XmlElement index_types, Hashtable goodfiles)
	{
		// Look for type nodes that no longer correspond to types
		foreach (XmlElement typenode in index_types.SelectNodes("Namespace/Type")) {
			string fulltypename = ((XmlElement)typenode.ParentNode).GetAttribute("Name") + "/" + typenode.GetAttribute("Name") + ".xml";
			if (!goodfiles.ContainsKey(fulltypename))
				typenode.ParentNode.RemoveChild(typenode);
		}
	}
		
	public static void DoUpdateType2(XmlDocument basefile, Type type, string output) {
		Console.Error.WriteLine("Updating: " + type.FullName);
		
		Hashtable seenmembers = new Hashtable();

		// Update type metadata
		UpdateType(basefile.DocumentElement, type, false);

		// Update existing members.  Delete member nodes that no longer should be there,
		// and remember what members are already documented so we don't add them again.
		if (!ignoremembers) {
			ArrayList todelete = new ArrayList ();
			foreach (XmlElement oldmember in basefile.SelectNodes("Type/Members/Member")) {
				MemberInfo oldmember2 = GetMember(type, oldmember);
	 			string sig = oldmember2 != null ? MakeMemberSignature(oldmember2) : null;
				
				// Interface implementations and overrides are deleted from the docs
				// unless the overrides option is given.
				if (oldmember2 != null && (!IsNew(oldmember2) || sig == null))
					oldmember2 = null;
				
				// Deleted (or signature changed)
				if (oldmember2 == null) {
					if (ignore_extra_docs && !delete)
						continue;
					Console.Error.WriteLine("Member Deleted: " + oldmember.SelectSingleNode("MemberSignature[@Language='C#']/@Value").InnerText);
					if (!delete && MemberDocsHaveUserContent(oldmember)) {
						Console.Error.WriteLine("Member deletions must be enabled with the --delete option.");
					} else {
						todelete.Add (oldmember);
						deletions++;
					}
					continue;
				}
				
				// Duplicated
				if (seenmembers.ContainsKey(oldmember2)) {
					Console.Error.WriteLine("Duplicate member: " + oldmember.SelectSingleNode("MemberSignature[@Language='C#']/@Value").InnerText);
					todelete.Add (oldmember);
					deletions++;
					continue;
				}
				
				// Update signature information
				UpdateMember(oldmember, oldmember2);
				
				seenmembers[sig] = 1;
			}
			foreach (XmlElement oldmember in todelete)
				oldmember.ParentNode.RemoveChild (oldmember);
		}
		
		if (!IsDelegate(type) && !ignoremembers) {
			XmlNode members = basefile.SelectSingleNode("Type/Members");
			foreach (MemberInfo m in Sort (type.GetMembers(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static|BindingFlags.Instance|BindingFlags.DeclaredOnly))) {
				if (m is Type) continue;
				
				string sig = MakeMemberSignature(m);
				if (sig == null) continue;
				if (seenmembers.ContainsKey(sig)) continue;
				
				// To be nice on diffs, members/properties/events that are overrides or are interface implementations
				// are not added in.
				if (!IsNew(m)) continue;
				
				XmlElement mm = MakeMember(basefile, m);
				if (mm == null) continue;
				members.AppendChild( mm );
	
				Console.Error.WriteLine("Member Added: " + mm.SelectSingleNode("MemberSignature/@Value").InnerText);
				additions++;
			}
		}
		
		// Import code snippets from files
		foreach (XmlNode code in basefile.GetElementsByTagName("code")) {
			if (!(code is XmlElement)) continue;
			string file = ((XmlElement)code).GetAttribute("src");
			if (file != "") {
				try {
					using (System.IO.StreamReader reader = new System.IO.StreamReader(srcPath + "/" + file)) {
						code.InnerText = reader.ReadToEnd();
					}
				} catch (Exception e) {
					Console.Error.WriteLine("Could not load code file '" + srcPath + "/" + file + "': " + e.Message);
				}
			}
		}

		System.IO.TextWriter writer;
		if (output == null)
			writer = Console.Out;
		else
			writer = new System.IO.StreamWriter(new System.IO.FileStream(output, System.IO.FileMode.Create));
		WriteXml(basefile.DocumentElement, writer);
		writer.Close();
	}
	
	private static bool MemberDocsHaveUserContent(XmlElement e) {
		e = (XmlElement)e.SelectSingleNode("Docs");
		if (e == null) return false;
		foreach (XmlElement d in e.SelectNodes("*"))
			if (d.InnerText != "" && !d.InnerText.StartsWith("To be added"))
				return true;
		return false;
	}
	
	private static bool IsNew(MemberInfo m) {
		if (!nooverrides) return true;
		if (m is MethodInfo && !IsNew((MethodInfo)m)) return false;
		if (m is PropertyInfo && !IsNew(((PropertyInfo)m).GetGetMethod())) return false;
		if (m is PropertyInfo && !IsNew(((PropertyInfo)m).GetSetMethod())) return false;
		if (m is EventInfo && !IsNew(((EventInfo)m).GetAddMethod())) return false;
		if (m is EventInfo && !IsNew(((EventInfo)m).GetRaiseMethod())) return false;
		if (m is EventInfo && !IsNew(((EventInfo)m).GetRemoveMethod())) return false;
		return true;
	}
	
	private static bool IsNew(MethodInfo m) {
		if (m == null) return true;
		MethodInfo b = m.GetBaseDefinition();
		if (b == null || b == m) return true;
		return false;
	}
	
	// UPDATE HELPER FUNCTIONS	
	
	private static XmlElement FindMatchingMember(Type type, XmlElement newfile, XmlElement oldmember) {
		MemberInfo oldmember2 = GetMember(type, oldmember);
		if (oldmember2 == null) return null;
		
		string membername = oldmember.GetAttribute("MemberName");
		foreach (XmlElement newmember in newfile.SelectNodes("Members/Member[@MemberName='" + membername + "']")) {
			if (GetMember(type, newmember) == oldmember2) return newmember;
		}
		
		return null;
	}
	
	private static MemberInfo GetMember(Type type, XmlElement member) {
		string membertype = member.SelectSingleNode("MemberType").InnerText;
		
		string returntype = null;
		XmlNode returntypenode = member.SelectSingleNode("ReturnValue/ReturnType");
		if (returntypenode != null) returntype = returntypenode.InnerText;
		
		// Get list of parameter types for member
		ArrayList memberparams = new ArrayList();
		foreach (XmlElement param in member.SelectNodes("Parameters/Parameter"))
			memberparams.Add(param.GetAttribute("Type"));

		string memberName = member.GetAttribute ("MemberName");
		do {
			int idx = memberName.IndexOf ("<");
			if (idx != -1)
				memberName = memberName.Substring (0, idx);
		} while (false);
		
		// Loop through all members in this type with the same name
		MemberInfo[] mis = type.GetMember(memberName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
		foreach (MemberInfo mi in mis) {
			if (mi is Type) continue;
			if (GetMemberType(mi) != membertype) continue;

			string sig = MakeMemberSignature(mi);
			if (sig == null) continue; // not publicly visible

			if (mi is MethodInfo) {
				// Casting operators can overload based on return type.
				if (returntype != GetDocTypeFullName (((MethodInfo)mi).ReturnType)) 
					continue;
			}

			ParameterInfo[] pis = null;
			if (mi is MethodInfo || mi is ConstructorInfo)
				pis = ((MethodBase)mi).GetParameters();
			else if (mi is PropertyInfo)
				pis = ((PropertyInfo)mi).GetIndexParameters();
			
			if (pis == null)
				pis = new ParameterInfo[0];
				
			if (pis.Length != memberparams.Count) continue;			
			
			bool good = true;
			for (int i = 0; i < pis.Length; i++)
				if (GetDocParameterType (pis[i].ParameterType) != (string)memberparams[i]) {
					good = false;
					break;
				}
			if (!good) continue;

			return mi;
		}
		
		return null;
	}
	
	// CREATE A STUB DOCUMENTATION FILE	

	public static XmlElement StubType(Type type) {
		string typesig = MakeTypeSignature(type);
		if (typesig == null) return null; // not publicly visible
		
		XmlDocument doc = new XmlDocument();
		XmlElement root = doc.CreateElement("Type");
		
		UpdateType(root, type, true);

		if (since != null) {
			XmlNode docs = root.SelectSingleNode("Docs");
			docs.AppendChild (CreateSinceNode (doc));
		}
		
		return root;
	}

	private static XmlElement CreateSinceNode (XmlDocument doc)
	{
		XmlElement s = doc.CreateElement ("since");
		s.SetAttribute ("version", since);
		return s;
	}
	
	// STUBBING/UPDATING FUNCTIONS
	
	public static void UpdateType(XmlElement root, Type type, bool addmembers) {
		root.SetAttribute("Name", GetDocTypeName (type));
		root.SetAttribute("FullName", GetDocTypeFullName (type));

		WriteElementAttribute(root, "TypeSignature[@Language='C#']", "Language", "C#");
		WriteElementAttribute(root, "TypeSignature[@Language='C#']", "Value", MakeTypeSignature(type));
		
		XmlElement ass = WriteElement(root, "AssemblyInfo");
		WriteElementText(ass, "AssemblyName", type.Assembly.GetName().Name);
		WriteElementText(ass, "AssemblyVersion", type.Assembly.GetName().Version.ToString());
		if (type.Assembly.GetName().CultureInfo.Name != "")
			WriteElementText(ass, "AssemblyCulture", type.Assembly.GetName().CultureInfo.Name);
		else
			ClearElement(ass, "AssemblyCulture");
		
		// Why-oh-why do we put assembly attributes in each type file?
		// Neither monodoc nor monodocs2html use them, so I'm deleting them
		// since they're outdated in current docs, and a waste of space.
		//MakeAttributes(ass, type.Assembly, true);
		XmlNode assattrs = ass.SelectSingleNode("Attributes");
		if (assattrs != null)
			ass.RemoveChild(assattrs);
		
		NormalizeWhitespace(ass);
		
		if (DocUtils.IsGenericType (type)) {
			XmlElement typeargs = WriteElement(root, "TypeParameters");
			ClearElementChildren(typeargs);
			foreach (Type arg in DocUtils.GetGenericArguments (type)) {
				XmlElement argnode = typeargs.OwnerDocument.CreateElement("TypeParameter");
				typeargs.AppendChild(argnode);
				argnode.InnerText = arg.Name;
			}
		} else {
			ClearElement(root, "TypeParameters");
		}
		
		if (type.BaseType != null) {
			XmlElement basenode = WriteElement(root, "Base");
			
			string basetypename = GetDocTypeFullName (type.BaseType);
			if (basetypename == "System.MulticastDelegate") basetypename = "System.Delegate";
			WriteElementText(root, "Base/BaseTypeName", basetypename);
			
			// Document how this type instantiates the generic parameters of its base type
			if (DocUtils.IsGenericType (type.BaseType)) {
				ClearElement(basenode, "BaseTypeArguments");
				Type[] baseGenArgs     = DocUtils.GetGenericArguments (type.BaseType);
				Type genericDefinition = DocUtils.GetGenericTypeDefinition (type.BaseType);
				Type[] genTypeDefArgs  = DocUtils.GetGenericArguments (genericDefinition);
				for (int i = 0; i < baseGenArgs.Length; i++) {
					Type typearg   = baseGenArgs [i];
					Type typeparam = genTypeDefArgs [i];
					
					XmlElement bta = WriteElement(basenode, "BaseTypeArguments");
					XmlElement arg = bta.OwnerDocument.CreateElement("BaseTypeArgument");
					bta.AppendChild(arg);
					arg.SetAttribute("TypeParamName", typeparam.Name);
					arg.InnerText = GetDocTypeFullName (typearg);
				}
			}
		} else {
			ClearElement(root, "Base");
		}

		if (!IsDelegate(type) && !type.IsEnum) {
			// Get a sorted list of interface implementations.  Don't include
			// interfaces that are implemented by a base type or another interface
			// because they go on the base type or base interface's signature.
			ArrayList interface_names = new ArrayList();
			foreach (Type i in type.GetInterfaces())
				if ((type.BaseType == null || Array.IndexOf(type.BaseType.GetInterfaces(), i) == -1) && InterfaceNotFromAnother(i, type.GetInterfaces()))
					interface_names.Add(GetDocTypeFullName (i));
			interface_names.Sort();

			XmlElement interfaces = WriteElement(root, "Interfaces");
			interfaces.RemoveAll();
			foreach (string iname in interface_names) {
				XmlElement iface = root.OwnerDocument.CreateElement("Interface");
				interfaces.AppendChild(iface);
				WriteElementText(iface, "InterfaceName", iname);
			}
		} else {
			ClearElement(root, "Interfaces");
		}

		MakeAttributes(root, type, false);
		
		if (IsDelegate(type)) {
			MakeParameters(root, type.GetMethod("Invoke").GetParameters());
			MakeReturnValue(root, type.GetMethod("Invoke"));
		}
		
		if (!IsDelegate(type) && addmembers) {
			XmlElement members = WriteElement(root, "Members");
			
			foreach (MemberInfo m in Sort (type.GetMembers(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static|BindingFlags.Instance|BindingFlags.DeclaredOnly))) {
				if (m is Type) continue;
				if (!IsNew(m)) continue;
				
				XmlElement mm = MakeMember(root.OwnerDocument, m);
				if (mm == null) continue;
				members.AppendChild( mm );
			}
		}
			
		XmlElement docs = WriteElement(root, "Docs");
		MakeDocNode (new DocsNodeInfo (docs, type));
		
		NormalizeWhitespace(root);
	}

	class MemberInfoComparer : IComparer
#if !NET_1_0
														 , IComparer<MemberInfo>
#endif
	{
		public int Compare (MemberInfo x, MemberInfo y)
		{
			string xs = slashdocFormatter.GetName (x);
			string ys = slashdocFormatter.GetName (y);
			// return String.Compare (xs, ys, StringComparison.OrdinalIgnoreCase);
			return string.Compare (xs, ys, true, CultureInfo.InvariantCulture);
		}

		public int Compare (object x, object y)
		{
			return Compare ((MemberInfo) x, (MemberInfo) y);
		}
	}

	static MemberInfoComparer memberInfoComparer = new MemberInfoComparer ();

	private static MemberInfo[] Sort (MemberInfo[] members)
	{
#if NET_1_0
		ArrayList l = new ArrayList ();
		l.AddRange (members);
		l.Sort (memberInfoComparer);
		return (MemberInfo[]) l.ToArray (typeof(MemberInfo));
#else
		Array.Sort (members, memberInfoComparer);
		return members;
#endif
	}
	
	private static void UpdateMember(XmlElement me, MemberInfo mi) {	
		WriteElementAttribute(me, "MemberSignature[@Language='C#']", "Language", "C#");
		WriteElementAttribute(me, "MemberSignature[@Language='C#']", "Value", MakeMemberSignature(mi));

		WriteElementText(me, "MemberType", GetMemberType(mi));
		
		MakeAttributes(me, mi, false);
		MakeReturnValue(me, mi);
		MakeParameters(me, mi);
		
		string fieldValue;
		if (mi is FieldInfo && GetFieldConstValue((FieldInfo)mi, out fieldValue))
			WriteElementText(me, "MemberValue", fieldValue);
		
		MakeDocNode (new DocsNodeInfo (WriteElement (me, "Docs"), mi));
	}
	
	private static bool GetFieldConstValue(FieldInfo field, out string value) {
		value = null;
		if (field.DeclaringType.IsEnum) return false;
		if (field.IsLiteral || (field.IsStatic && field.IsInitOnly)) {
			object val = field.GetValue(null);
			if (val == null) value = "null";
			else if (val is Enum) value = val.ToString();
			else if (val is IFormattable) {
				value = ((IFormattable)val).ToString();
				if (val is string)
					value = "\"" + value + "\"";
			}
			if (value != null && value != "")
				return true;
		}
		return false;
	}
	
	// XML HELPER FUNCTIONS
	
	private static XmlElement WriteElement(XmlElement parent, string element) {
		XmlElement ret = (XmlElement)parent.SelectSingleNode(element);
		if (ret == null) {
			string[] path = element.Split('/');
			foreach (string p in path) {
				ret = (XmlElement)parent.SelectSingleNode(p);
				if (ret == null) {
					string ename = p;
					if (ename.IndexOf('[') >= 0) // strip off XPath predicate
						ename = ename.Substring(0, ename.IndexOf('['));
					ret = parent.OwnerDocument.CreateElement(ename);
					parent.AppendChild(ret);
					parent = ret;
				} else {
					parent = ret;
				}
			}
		}
		return ret;
	}
	private static void WriteElementText(XmlElement parent, string element, string value) {
		XmlElement node = WriteElement(parent, element);
		node.InnerText = value;
	}

	private static void CopyNode (XmlNode source, XmlNode dest)
	{
		XmlNode copy = dest.OwnerDocument.ImportNode (source, true);
		dest.AppendChild (copy);
	}

	private static void WriteElementInitialText(XmlElement parent, string element, string value) {
		XmlElement node = (XmlElement)parent.SelectSingleNode(element);
		if (node != null)
			return;
		node = WriteElement(parent, element);
		node.InnerText = value;
	}
	private static void WriteElementAttribute(XmlElement parent, string element, string attribute, string value) {
		XmlElement node = WriteElement(parent, element);
		if (node.GetAttribute(attribute) == value) return;
		node.SetAttribute(attribute, value);
	}
	private static void ClearElement(XmlElement parent, string name) {
		XmlElement node = (XmlElement)parent.SelectSingleNode(name);
		if (node != null)
			parent.RemoveChild(node);
	}
	private static void ClearElementChildren(XmlElement parent) {
		parent.RemoveAll();
	}
	
	// DOCUMENTATION HELPER FUNCTIONS
	
	private static void MakeDocNode (DocsNodeInfo info)
	{
		Type[] genericParams        = info.GenericParameters;
		ParameterInfo[] parameters  = info.Parameters;
		Type returntype             = info.ReturnType;
		bool returnisreturn         = info.ReturnIsReturn;
		XmlElement e                = info.Node;
		bool addremarks             = info.AddRemarks;

		WriteElementInitialText(e, "summary", "To be added.");
		
		if (parameters != null) {
			string[] values = new string [parameters.Length];
			for (int i = 0; i < values.Length; ++i)
				values [i] = parameters [i].Name;
			UpdateParameters (e, "param", values);
		}

		if (genericParams != null) {
			string[] values = new string [genericParams.Length];
			for (int i = 0; i < values.Length; ++i)
				values [i] = genericParams [i].Name;
			UpdateParameters (e, "typeparam", values);
		}

		string retnodename = null;
		if (returntype != null && returntype != typeof(void)) {
			retnodename = returnisreturn ? "returns" : "value";
			string retnodename_other = !returnisreturn ? "returns" : "value";
			
			// If it has a returns node instead of a value node, change its name.
			XmlElement retother = (XmlElement)e.SelectSingleNode(retnodename_other);
			if (retother != null) {
				XmlElement retnode = e.OwnerDocument.CreateElement(retnodename);
				foreach (XmlNode node in retother)
					retnode.AppendChild(node.CloneNode(true));
				e.ReplaceChild(retnode, retother);
			} else {
				WriteElementInitialText(e, retnodename, "To be added.");
			}
		} else {
			ClearElement(e, "returns");
			ClearElement(e, "value");
		}

		if (addremarks)
			WriteElementInitialText(e, "remarks", "To be added.");
		
		if (info.EcmaDocs != null) {
			foreach (XmlNode child in info.EcmaDocs.ChildNodes) {
				switch (child.Name) {
					case "param":
					case "typeparam": {
						XmlNode doc = e.SelectSingleNode (
								child.Name + "[@name='" + child.Attributes ["name"].Value + "']");
						if (doc != null)
							doc.InnerXml = child.InnerXml.Replace ("\r", "");
						break;
					}
					case "altmember":
					case "exception":
					case "permission":
					case "seealso": {
						XmlNode doc = e.SelectSingleNode (
								child.Name + "[@cref='" + child.Attributes ["cref"].Value + "']");
						if (doc != null)
							doc.InnerXml = child.InnerXml.Replace ("\r", "");
						break;
					}
					default: {
						XmlNode doc = e [child.Name];
						if (doc != null)
							doc.InnerXml = child.InnerXml.Replace ("\r", "");
						else
							CopyNode (child, e);
						break;
					}
				}
			}
		}
		if (info.SlashDocs != null) {
			XmlNode elem = info.SlashDocs;
			if (elem != null) {
				if (elem.SelectSingleNode("summary") != null)
					ClearElement(e, "summary");
				if (elem.SelectSingleNode("remarks") != null)
					ClearElement(e, "remarks");
				if (retnodename != null && elem.SelectSingleNode(retnodename) != null)
					ClearElement(e, retnodename);

				foreach (XmlNode child in elem.ChildNodes) {
					if (child.Name != "param" && child.Name != "typeparam" && child.Name != "seealso")
						CopyNode(child, e);
				}

				foreach (XmlElement sa in elem.SelectNodes("seealso")) {
					XmlElement a = (XmlElement)e.SelectSingleNode("altmember[@cref='" + sa.GetAttribute("cref") + "']");
					if (a == null) {
						a = e.OwnerDocument.CreateElement ("altmember");
						a.SetAttribute ("cref", sa.GetAttribute("cref"));
						e.AppendChild(a);
					}
				}

				foreach (XmlElement p in elem.SelectNodes("param")) {
					XmlElement p2 = (XmlElement)e.SelectSingleNode("param[@name='" + p.GetAttribute("name") + "']");
					if (p2 != null)
						p2.InnerXml = p.InnerXml;
				}

				foreach (XmlElement p in elem.SelectNodes("typeparam")) {
					XmlElement p2 = (XmlElement)e.SelectSingleNode("typeparam[@name='" + p.GetAttribute("name") + "']");
					if (p2 != null)
						p2.InnerXml = p.InnerXml;
				}
			}
		}
		
		NormalizeWhitespace(e);
	}

	private static void UpdateParameters (XmlElement e, string element, string[] values)
	{	
		if (values != null) {
			XmlNode[] paramnodes = new XmlNode[values.Length];
			
			// Some documentation had param nodes with leading spaces.
			foreach (XmlElement paramnode in e.SelectNodes(element)){
				paramnode.SetAttribute("name", paramnode.GetAttribute("name").Trim());
			}
			
			// If a member has only one parameter, we can track changes to
			// the name of the parameter easily.
			if (values.Length == 1 && e.SelectNodes(element).Count == 1) {
				UpdateParameterName (e, (XmlElement) e.SelectSingleNode(element), values [0]);
			}

			bool reinsert = false;

			// Pick out existing and still-valid param nodes, and
			// create nodes for parameters not in the file.
			Hashtable seenParams = new Hashtable();
			for (int pi = 0; pi < values.Length; pi++) {
				string p = values [pi];
				seenParams[p] = pi;
				
				paramnodes[pi] = e.SelectSingleNode(element + "[@name='" + p + "']");
				if (paramnodes[pi] != null) continue;
				
				XmlElement pe = e.OwnerDocument.CreateElement(element);
				pe.SetAttribute("name", p);
				pe.InnerText = "To be added.";
				paramnodes[pi] = pe;
				reinsert = true;
			}

			// Remove parameters that no longer exist and check all params are in the right order.
			int idx = 0;
			foreach (XmlElement paramnode in e.SelectNodes(element)) {
				string name = paramnode.GetAttribute("name");
				if (!seenParams.ContainsKey(name)) {
					if (!delete && !paramnode.InnerText.StartsWith("To be added")) {
						Console.Error.Write("The following param node can only be deleted if the --delete option is given: ");
						Console.Error.WriteLine(paramnode.OuterXml);
					} else {
						paramnode.ParentNode.RemoveChild(paramnode);
					}
					continue;
				}

				if ((int)seenParams[name] != idx)
					reinsert = true;
				
				idx++;
			}
			
			// Re-insert the parameter nodes at the top of the doc section.
			if (reinsert)
				for (int pi = values.Length-1; pi >= 0; pi--)
					e.PrependChild(paramnodes[pi]);
		} else {
			// Clear all existing param nodes
			foreach (XmlNode paramnode in e.SelectNodes(element)) {
				if (!delete && !paramnode.InnerText.StartsWith("To be added")) {
					Console.Error.WriteLine("The following param node can only be deleted if the --delete option is given:");
					Console.Error.WriteLine(paramnode.OuterXml);
				} else {
					paramnode.ParentNode.RemoveChild(paramnode);
				}
			}
		}
	}

	private static void UpdateParameterName (XmlElement docs, XmlElement pe, string newName)
	{
		string existingName = pe.GetAttribute ("name");
		pe.SetAttribute ("name", newName);
		if (existingName == newName)
			return;
		foreach (XmlElement paramref in docs.SelectNodes (".//paramref"))
			if (paramref.GetAttribute ("name").Trim () == existingName)
				paramref.SetAttribute ("name", newName);
	}

	private static void NormalizeWhitespace(XmlElement e) {
		// Remove all text and whitespace nodes from the element so it
		// is outputted with nice indentation and no blank lines.
		ArrayList deleteNodes = new ArrayList();
		foreach (XmlNode n in e)
			if (n is XmlText || n is XmlWhitespace || n is XmlSignificantWhitespace)
				deleteNodes.Add(n);
		foreach (XmlNode n in deleteNodes)
				n.ParentNode.RemoveChild(n);
	}
	
	private static void MakeAttributes(XmlElement root, object attributes, bool assemblyAttributes) {
		int len;
#if NET_1_0
		object[] at = ((ICustomAttributeProvider) attributes).GetCustomAttributes (false);
		len = at.Length;
#else
		System.Collections.Generic.IList<CustomAttributeData> at;
		if (attributes is Assembly)
			at = CustomAttributeData.GetCustomAttributes((Assembly)attributes);
		else if (attributes is MemberInfo)
			at = CustomAttributeData.GetCustomAttributes((MemberInfo)attributes);
		else if (attributes is Module)
			at = CustomAttributeData.GetCustomAttributes((Module)attributes);
		else if (attributes is ParameterInfo)
			at = CustomAttributeData.GetCustomAttributes((ParameterInfo)attributes);
		else
			throw new ArgumentException("unsupported type: " + attributes.GetType().ToString());
		len = at.Count;
#endif
	
		if (len == 0) {
			ClearElement(root, "Attributes");
			return;
		}

		bool b = false;
		XmlElement e = (XmlElement)root.SelectSingleNode("Attributes");
		if (e != null)
			e.RemoveAll();
		else
			e = root.OwnerDocument.CreateElement("Attributes");
		
#if !NET_1_0
		foreach (CustomAttributeData a in at) {
			if (!IsPublic (a.Constructor.DeclaringType))
				continue;
			if (slashdocFormatter.GetName (a.Constructor.DeclaringType) == null)
				continue;
			
			if (a.Constructor.DeclaringType == typeof(System.Reflection.AssemblyKeyFileAttribute) || a.Constructor.DeclaringType == typeof(System.Reflection.AssemblyDelaySignAttribute)) continue; // hide security-related attributes
			if (a.Constructor.DeclaringType == typeof(System.Runtime.InteropServices.OutAttribute)) continue; // hide this because it is given in RefType attribute
			
			b = true;
			
			ArrayList fields = new ArrayList();

			foreach (CustomAttributeTypedArgument f in a.ConstructorArguments) {
				fields.Add(MakeAttributesValueString(f.Value));
			}
			foreach (CustomAttributeNamedArgument f in a.NamedArguments) {
				fields.Add(f.MemberInfo.Name + "=" + MakeAttributesValueString(f.TypedValue.Value));
			}

			string a2 = String.Join(", ", (string[])fields.ToArray(typeof(string)));
			if (a2 != "") a2 = "(" + a2 + ")";
			
			XmlElement ae = root.OwnerDocument.CreateElement("Attribute");
			e.AppendChild(ae);
			
			string name = a.Constructor.DeclaringType.FullName;
			if (name.EndsWith("Attribute")) name = name.Substring(0, name.Length-"Attribute".Length);
			WriteElementText(ae, "AttributeName", name + a2);
		}
#else
		foreach (Attribute a in at) {
			if (!IsPublic (a.GetType ()))
				continue;
			if (slashdocFormatter.GetName (a.GetType ()) == null) continue; // hide non-visible attributes
			//if (assemblyAttributes && a.GetType().FullName.StartsWith("System.Reflection.")) continue;
			if (a.GetType().FullName == "System.Reflection.AssemblyKeyFileAttribute" || a.GetType().FullName == "System.Reflection.AssemblyDelaySignAttribute") continue; // hide security-related attributes

 			b = true;
 			
			// There's no way to reconstruct how the attribute's constructor was called,
			// so as a substitute, just list the value of all of the attribute's public fields.
			
 			ArrayList fields = new ArrayList();
			foreach (PropertyInfo f in a.GetType().GetProperties(BindingFlags.Public|BindingFlags.Instance)) {
				if (f.Name == "TypeId") continue;
				
				object v = f.GetValue(a, null);
				if (v == null) v = "null";
				else if (v is string) v = "\"" + v + "\"";
				else if (v is Type) v = "typeof(" + GetCSharpFullName ((Type)v) + ")";
				else if (v is Enum) v = v.GetType().FullName + "." + v.ToString().Replace(", ", "|");
					
				fields.Add(f.Name + "=" + v);
			}
 			string a2 = String.Join(", ", (string[])fields.ToArray(typeof(string)));
 			if (a2 != "") a2 = "(" + a2 + ")";
 			
 			XmlElement ae = root.OwnerDocument.CreateElement("Attribute");
 			e.AppendChild(ae);
 			
			string name = a.GetType().FullName;
 			if (name.EndsWith("Attribute")) name = name.Substring(0, name.Length-"Attribute".Length);
 			WriteElementText(ae, "AttributeName", name + a2);
 		}
#endif
		
		if (b && e.ParentNode == null)
			root.AppendChild(e);
		else if (!b)
			ClearElement(root, "Attributes");
		
		NormalizeWhitespace(e);
	}
	
	private static string MakeAttributesValueString(object v) {
		if (v == null) return "null";
		else if (v is string) return "\"" + v + "\"";
		else if (v is bool) return (bool)v ? "true" : "false";
		else if (v is Type) return "typeof(" + GetCSharpFullName ((Type)v) + ")";
		else if (v is Enum) {
			string typename = v.GetType ().FullName;
			return typename + "." + v.ToString().Replace(", ", " | " + typename + ".");
		}
		else return v.ToString();
	}
	
	private static void MakeParameters(XmlElement root, ParameterInfo[] parameters) {
		XmlElement e = WriteElement(root, "Parameters");
		e.RemoveAll();
		foreach (ParameterInfo p in parameters) {
			XmlElement pe = root.OwnerDocument.CreateElement("Parameter");
			e.AppendChild(pe);
			pe.SetAttribute("Name", p.Name);
			pe.SetAttribute("Type", GetDocParameterType (p.ParameterType));
			if (p.ParameterType.IsByRef) {
				if (p.IsOut) pe.SetAttribute("RefType", "out");
				else pe.SetAttribute("RefType", "ref");
			}
			MakeAttributes(pe, p, false);
		}
	}

	private static void MakeParameters(XmlElement root, MemberInfo mi) {
		if (mi is ConstructorInfo) MakeParameters(root, ((ConstructorInfo)mi).GetParameters());
		else if (mi is MethodInfo) MakeParameters(root, ((MethodInfo)mi).GetParameters());
		else if (mi is PropertyInfo) {
			ParameterInfo[] parameters = ((PropertyInfo)mi).GetIndexParameters();
			if (parameters.Length > 0)
				MakeParameters(root, parameters);
			else
				return;
		}
		else if (mi is FieldInfo) return;
		else if (mi is EventInfo) return;
		else throw new ArgumentException();
	}

	private static string GetDocParameterType (Type type)
	{
		return GetDocTypeFullName (type).Replace ("@", "&");
	}

	private static void MakeReturnValue(XmlElement root, Type type, ICustomAttributeProvider attributes) {
		XmlElement e = WriteElement(root, "ReturnValue");
		e.RemoveAll();
		WriteElementText(e, "ReturnType", GetDocTypeFullName (type));
		if (attributes != null)
			MakeAttributes(root, attributes, false);
	}
	
	private static void MakeReturnValue(XmlElement root, MemberInfo mi) {
		if (mi is ConstructorInfo) return;
		else if (mi is MethodInfo) MakeReturnValue(root, ((MethodInfo)mi).ReturnType, ((MethodInfo)mi).ReturnTypeCustomAttributes);
		else if (mi is PropertyInfo) MakeReturnValue(root, ((PropertyInfo)mi).PropertyType, null);
		else if (mi is FieldInfo) MakeReturnValue(root, ((FieldInfo)mi).FieldType, null);
		else if (mi is EventInfo) MakeReturnValue(root, ((EventInfo)mi).EventHandlerType, null);
		else throw new ArgumentException(mi + " is a " + mi.GetType().FullName);
	}
	
	private static MemberInfo GetInterfaceDefinition(MethodInfo mi) {
		if (mi.DeclaringType.IsInterface) return null;

		foreach (Type i in mi.DeclaringType.GetInterfaces()) {
			System.Reflection.InterfaceMapping map = mi.DeclaringType.GetInterfaceMap(i);
			for (int idx = 0; idx < map.InterfaceMethods.Length; idx++) {
				if (map.TargetMethods[idx] == mi)
					return map.InterfaceMethods[idx];
			}
		}
		
		return null;
	}

	private static XmlElement MakeMember(XmlDocument doc, MemberInfo mi) {
		if (mi is Type) return null;
		
		string sigs = MakeMemberSignature(mi);
		if (sigs == null) return null; // not publicly visible
		
		// no documentation for property/event accessors.  Is there a better way of doing this?
		if (mi.Name.StartsWith("get_")) return null;
		if (mi.Name.StartsWith("set_")) return null;
		if (mi.Name.StartsWith("add_")) return null;
		if (mi.Name.StartsWith("remove_")) return null;
		if (mi.Name.StartsWith("raise_")) return null;
		
		XmlElement me = doc.CreateElement("Member");
		me.SetAttribute("MemberName", GetMemberName (mi));
		
		UpdateMember(me, mi);

		if (since != null) {
			XmlNode docs = me.SelectSingleNode("Docs");
			docs.AppendChild (CreateSinceNode (doc));
		}
		
		return me;
	}

	private static string GetMemberName (MemberInfo mi)
	{
		MethodBase mb = mi as MethodBase;
		if (mb == null)
			return mi.Name;
		try {
			StringBuilder sb = new StringBuilder ();
			sb.Append (mi.Name);
			Type[] typeParams = DocUtils.GetGenericArguments (mb);
			if (typeParams.Length > 0) {
				sb.Append ("<");
				sb.Append (typeParams [0].Name);
				for (int i = 1; i < typeParams.Length; ++i)
					sb.Append (",").Append (typeParams [i].Name);
				sb.Append (">");
			}
			return sb.ToString ();
		}
		catch (NotSupportedException) {
			return mi.Name;
		}
	}
	
	static bool IsDelegate(Type type) {
		return typeof(System.Delegate).IsAssignableFrom (type) && !type.IsAbstract;
	}
	
	/// SIGNATURE GENERATION FUNCTIONS
	
	private static bool InterfaceNotFromAnother(Type i, Type[] i2) {
		foreach (Type t in i2)
			if (i != t && Array.IndexOf(t.GetInterfaces(), i) != -1)
				return false;
		return true;
	}
	
	static string MakeTypeSignature (Type type) {
		return csharpFormatter.GetDeclaration (type);
	}

	static string MakeMemberSignature(MemberInfo mi) {
		return csharpFullFormatter.GetDeclaration (mi);
	}

	static string GetMemberType(MemberInfo mi) {
		if (mi is ConstructorInfo) return "Constructor";
		if (mi is MethodInfo) return "Method";
		if (mi is PropertyInfo) return "Property";
		if (mi is FieldInfo) return "Field";
		if (mi is EventInfo) return "Event";
		throw new ArgumentException();
	}

	private static string GetDocTypeName (Type type)
	{
		return docTypeFormatter.GetName (type);
	}

	private static string GetDocTypeFullName (Type type)
	{
		return docTypeFullFormatter.GetName (type);
	}

	private static string GetCSharpFullName (Type type)
	{
		return docTypeFullFormatter.GetName (type);
	}

	class DocsNodeInfo {
		public DocsNodeInfo (XmlElement node)
		{
			this.Node = node;
		}

		public DocsNodeInfo (XmlElement node, Type type)
			: this (node)
		{
			GenericParameters = DocUtils.GetGenericArguments (type);
			if (type.DeclaringType != null) {
				Type[] declGenParams = DocUtils.GetGenericArguments (type.DeclaringType);
				if (declGenParams != null && GenericParameters.Length == declGenParams.Length) {
					GenericParameters = null;
				}
				else if (declGenParams != null) {
					Type[] nestedParams = new Type [GenericParameters.Length - declGenParams.Length];
					for (int i = 0; i < nestedParams.Length; ++i) {
						nestedParams [i] = GenericParameters [i+declGenParams.Length];
					}
					GenericParameters = nestedParams;
				}
			}
			if (IsDelegate(type)) {
				Parameters = type.GetMethod("Invoke").GetParameters();
				ReturnType = type.GetMethod("Invoke").ReturnType;
			}
			SetSlashDocs (type);
			SetEcmaDocs (type);
		}

		public DocsNodeInfo (XmlElement node, MemberInfo member)
			: this (node)
		{
			ReturnIsReturn = true;
			AddRemarks = true;
			
			if (member is MethodInfo || member is ConstructorInfo) {
				Parameters = ((MethodBase) member).GetParameters ();
				try {
					GenericParameters = DocUtils.GetGenericArguments ((MethodBase) member);
				}
				catch (NotSupportedException) {
					/* ignore */
				}
			}
			else if (member is PropertyInfo) {
				Parameters = ((PropertyInfo) member).GetIndexParameters ();
			}
				
			if (member is MethodInfo) {
				ReturnType = ((MethodInfo) member).ReturnType;
			} else if (member is PropertyInfo) {
				ReturnType = ((PropertyInfo) member).PropertyType;
				ReturnIsReturn = false;
			}

			// no remarks section for enum members
			if (member.DeclaringType != null && member.DeclaringType.IsEnum)
				AddRemarks = false;
			SetSlashDocs (member);
			SetEcmaDocs (member);
		}

		private void SetSlashDocs (MemberInfo member)
		{
			string slashdocsig = slashdocFormatter.GetDeclaration (member);
			if (slashdocs != null && slashdocsig != null)
				SlashDocs = slashdocs.SelectSingleNode ("doc/members/member[@name='" + slashdocsig + "']");
		}

		private void SetEcmaDocs (MemberInfo member)
		{
			if (ecmadocs == null)
				return;

			string xpath;

			if (member is Type) {
				xpath = "//Type[@FullName=\"" + GetDocTypeFullName ((Type) member)+ "\"]/Docs";
			}
			else {
				// Create e.g.:
				// //Members/Member[@MemberName="CopyTo"]/Parameters[count(Parameter) = 2 
				//   and Parameter[1]/@Type="System.Array" and 
				//   Parameter[2]/@Type="System.Int64"]/../Docs'
				StringBuilder sb = new StringBuilder ();
				sb.Append ("//Type[@FullName=\"")
					.Append (GetDocTypeFullName (member.DeclaringType))
					.Append ("\"]");
				sb.Append ("/Members/Member[@MemberName=\"");
				if (member is ConstructorInfo)
					sb.Append (".ctor");
				else
					sb.Append (GetMemberName (member));
				sb.Append ("\"]");
				if (member is MethodBase) {
					sb.Append ("/Parameters[");
					ParameterInfo[] parameters = ((MethodBase) member).GetParameters ();
					sb.Append ("count(Parameter) = ").Append (parameters.Length);
					for (int i = 0; i < parameters.Length; ++i) {
						sb.Append (" and Parameter[").Append (i+1).Append ("]/@Type=\"")
							.Append (GetDocTypeFullName (parameters [i].ParameterType))
							.Append ("\"");
					}
					sb.Append ("]/..");
				}
				sb.Append ("/Docs");
				xpath = sb.ToString ();
			}
			EcmaDocs = ecmadocs.SelectSingleNode (xpath);
		}

		public Type ReturnType;
		public Type[] GenericParameters;
		public ParameterInfo[] Parameters;
		public bool ReturnIsReturn;
		public XmlElement Node;
		public bool AddRemarks = true;
		public XmlNode SlashDocs;
		public XmlNode EcmaDocs;
	}
}

static class DocUtils {
	public static Type[] GetGenericArguments (Type type)
	{
#if NET_1_0
		return new Type [0];
#else
		return type.GetGenericArguments ();
#endif
	}

	public static Type[] GetGenericArguments (MethodBase mb)
	{
#if NET_1_0
		return new Type [0];
#else
		return mb.GetGenericArguments ();
#endif
	}

	public static Type GetGenericTypeDefinition (Type type)
	{
#if NET_1_0
		return null;
#else
		return type.GetGenericTypeDefinition ();
#endif
	}

	public static bool IsGenericType (Type type)
	{
#if NET_1_0
		return false;
#else
		return type.IsGenericType;
#endif
	}

	public static bool IsGenericParameter (Type type)
	{
#if NET_1_0
		return false;
#else
		return type.IsGenericParameter;
#endif
	}
}

public abstract class MemberFormatter {
	public string GetName (MemberInfo member)
	{
		Type type = member as Type;
		if (type != null)
			return GetTypeName (type);
		ConstructorInfo ctor = member as ConstructorInfo;
		if (ctor != null)
			return GetConstructorName (ctor);
		MethodInfo method = member as MethodInfo;
		if (method != null)
			return GetMethodName (method);
		PropertyInfo prop = member as PropertyInfo;
		if (prop != null)
			return GetPropertyName (prop);
		FieldInfo field = member as FieldInfo;
		if (field != null)
			return GetFieldName (field);
		EventInfo e = member as EventInfo;
		if (e != null)
			return GetEventName (e);
		throw new NotSupportedException ("Can't handle: " + member.GetType().ToString());
	}

	protected virtual string GetTypeName (Type type)
	{
		if (type == null)
			throw new ArgumentNullException ("type");
		return _AppendTypeName (new StringBuilder (type.Name.Length), type).ToString ();
	}

	protected virtual char[] ArrayDelimeters {
		get {return new char[]{'[', ']'};}
	}

	protected StringBuilder _AppendTypeName (StringBuilder buf, Type type)
	{
		if (type.IsArray) {
			_AppendTypeName (buf, type.GetElementType ()).Append (ArrayDelimeters [0]);
			int rank = type.GetArrayRank ();
			if (rank > 1)
				buf.Append (new string (',', rank-1));
			return buf.Append (ArrayDelimeters [1]);
		}
		if (type.IsByRef) {
			return AppendRefTypeName (buf, type);
		}
		if (type.IsPointer) {
			return AppendPointerTypeName (buf, type);
		}
		AppendNamespace (buf, type);
		if (DocUtils.IsGenericParameter (type)) {
			return AppendTypeName (buf, type);
		}
		if (!DocUtils.IsGenericType (type)) {
			if (type.DeclaringType != null)
				AppendTypeName (buf, type.DeclaringType).Append (NestedTypeSeparator);
			return AppendTypeName (buf, type);
		}
		return AppendGenericType (buf, type);
	}

	protected virtual StringBuilder AppendNamespace (StringBuilder buf, Type type)
	{
		if (type.Namespace != null)
			buf.Append (type.Namespace).Append ('.');
		return buf;
	}

	protected virtual StringBuilder AppendTypeName (StringBuilder buf, Type type)
	{
		return AppendTypeName (buf, type.Name);
	}

	protected virtual StringBuilder AppendTypeName (StringBuilder buf, string typename)
	{
		int n = typename.IndexOf ("`");
		if (n >= 0)
			return buf.Append (typename.Substring (0, n));
		return buf.Append (typename);
	}

	protected virtual string RefTypeModifier {
		get {return "@";}
	}

	protected virtual StringBuilder AppendRefTypeName (StringBuilder buf, Type type)
	{
		return _AppendTypeName (buf, type.GetElementType ()).Append (RefTypeModifier);
	}

	protected virtual string PointerModifier {
		get {return "*";}
	}

	protected virtual StringBuilder AppendPointerTypeName (StringBuilder buf, Type type)
	{
		return _AppendTypeName (buf, type.GetElementType ()).Append (PointerModifier);
	}

	protected virtual char[] GenericTypeContainer {
		get {return new char[]{'<', '>'};}
	}

	protected virtual char NestedTypeSeparator {
		get {return '.';}
	}

	protected virtual StringBuilder AppendGenericType (StringBuilder buf, Type type)
	{
		Type[] genArgs = DocUtils.GetGenericArguments (type);
		int genArg = 0;
		if (type.DeclaringType != null) {
			AppendTypeName (buf, type.DeclaringType);
			if (DocUtils.IsGenericType (type.DeclaringType)) {
				buf.Append (GenericTypeContainer [0]);
				int max = DocUtils.GetGenericArguments (type.DeclaringType).Length;
				_AppendTypeName (buf, genArgs [genArg++]);
				while (genArg < max) {
					buf.Append (",");
					_AppendTypeName (buf, genArgs [genArg++]);
				}
				buf.Append (GenericTypeContainer [1]);
			}
			buf.Append (NestedTypeSeparator);
		}
		AppendTypeName (buf, type);
		if (genArg < genArgs.Length) {
			buf.Append (GenericTypeContainer [0]);
			_AppendTypeName (buf, genArgs [genArg++]);
			while (genArg < genArgs.Length) {
				buf.Append (",");
				_AppendTypeName (buf, genArgs [genArg++]);
			}
			buf.Append (GenericTypeContainer [1]);
		}
		return buf;
	}

	protected virtual string GetConstructorName (ConstructorInfo constructor)
	{
		return constructor.Name;
	}

	protected virtual string GetMethodName (MethodInfo method)
	{
		return method.Name;
	}

	protected virtual string GetPropertyName (PropertyInfo property)
	{
		return property.Name;
	}

	protected virtual string GetFieldName (FieldInfo field)
	{
		return field.Name;
	}

	protected virtual string GetEventName (EventInfo e)
	{
		return e.Name;
	}

	public string GetDeclaration (MemberInfo member)
	{
		Type type = member as Type;
		if (type != null)
			return GetTypeDeclaration (type);
		ConstructorInfo ctor = member as ConstructorInfo;
		if (ctor != null)
			return GetConstructorDeclaration (ctor);
		MethodInfo method = member as MethodInfo;
		if (method != null)
			return GetMethodDeclaration (method);
		PropertyInfo prop = member as PropertyInfo;
		if (prop != null)
			return GetPropertyDeclaration (prop);
		FieldInfo field = member as FieldInfo;
		if (field != null)
			return GetFieldDeclaration (field);
		EventInfo e = member as EventInfo;
		if (e != null)
			return GetEventDeclaration (e);
		throw new NotSupportedException ("Can't handle: " + member.GetType().ToString());
	}

	protected virtual string GetTypeDeclaration (Type type)
	{
		return GetTypeName (type);
	}

	protected virtual string GetConstructorDeclaration (ConstructorInfo constructor)
	{
		return GetConstructorName (constructor);
	}

	protected virtual string GetMethodDeclaration (MethodInfo method)
	{
		// Special signature for destructors.
		if (method.Name == "Finalize" && method.GetParameters().Length == 0)
			return GetFinalizerName (method);

		StringBuilder buf = new StringBuilder ();

		AppendVisibility (buf, method);
		if (buf.Length == 0)
			return null;

		AppendModifiers (buf, method);

		buf.Append (" ").Append (GetName (method.ReturnType)).Append (" ");

		buf.Append (method.Name);
		AppendGenericMethod (buf, method).Append (" ");
		AppendParameters (buf, method.GetParameters ());
		return buf.ToString ();
	}

	protected virtual string GetFinalizerName (MethodInfo method)
	{
		return "Finalize";
	}

	protected virtual StringBuilder AppendVisibility (StringBuilder buf, MethodBase method)
	{
		return buf;
	}

	protected virtual StringBuilder AppendModifiers (StringBuilder buf, MethodInfo method)
	{
		return buf;
	}

	protected virtual StringBuilder AppendGenericMethod (StringBuilder buf, MethodInfo method)
	{
		return buf;
	}

	protected virtual StringBuilder AppendParameters (StringBuilder buf, ParameterInfo[] parameters)
	{
		return buf;
	}

	protected virtual string GetPropertyDeclaration (PropertyInfo property)
	{
		return GetPropertyName (property);
	}

	protected virtual string GetFieldDeclaration (FieldInfo field)
	{
		return GetFieldName (field);
	}

	protected virtual string GetEventDeclaration (EventInfo e)
	{
		return GetEventName (e);
	}
}

class CSharpFullMemberFormatter : MemberFormatter {

	protected override StringBuilder AppendNamespace (StringBuilder buf, Type type)
	{
		if (GetCSharpType (type.FullName) == null && type.Namespace != null && type.Namespace != "System")
			buf.Append (type.Namespace).Append ('.');
		return buf;
	}

	private string GetCSharpType (string t)
	{
		switch (t) {
		case "System.Byte":    return "byte";
		case "System.SByte":   return "sbyte";
		case "System.Int16":   return "short";
		case "System.Int32":   return "int";
		case "System.Int64":   return "long";

		case "System.UInt16":  return "ushort";
		case "System.UInt32":  return "uint";
		case "System.UInt64":  return "ulong";

		case "System.Single":  return "float";
		case "System.Double":  return "double";
		case "System.Decimal": return "decimal";
		case "System.Boolean": return "bool";
		case "System.Char":    return "char";
		case "System.Void":    return "void";
		case "System.String":  return "string";
		case "System.Object":  return "object";
		}
		return null;
	}

	protected override StringBuilder AppendTypeName (StringBuilder buf, Type type)
	{
		if (DocUtils.IsGenericParameter (type))
			return buf.Append (type.Name);
		string t = type.FullName;
		if (!t.StartsWith ("System.")) {
			return base.AppendTypeName (buf, type);
		}

		string s = GetCSharpType (t);
		if (s != null)
			return buf.Append (s);
		
		return base.AppendTypeName (buf, type);
	}

	protected override string GetTypeDeclaration (Type type)
	{
		string visibility = GetTypeVisibility (type.Attributes);
		if (visibility == null)
			return null;

		StringBuilder buf = new StringBuilder ();
		
		buf.Append (visibility);
		buf.Append (" ");

		MemberFormatter full = new CSharpFullMemberFormatter ();

		if (IsDelegate(type)) {
			buf.Append("delegate ");
			MethodInfo invoke = type.GetMethod ("Invoke");
			buf.Append (full.GetName (invoke.ReturnType)).Append (" ");
			buf.Append (GetName (type));
			AppendParameters (buf, invoke.GetParameters ()).Append (";");

			return buf.ToString();
		}
		
		if (type.IsAbstract && !type.IsInterface)
			buf.Append("abstract ");
		if (type.IsSealed && !IsDelegate(type) && !type.IsValueType)
			buf.Append("sealed ");
		buf.Replace ("abstract sealed", "static");

		buf.Append (GetTypeKind (type));
		buf.Append (" ");
		buf.Append (GetCSharpType (type.FullName) == null 
				? GetName (type) 
				: type.Name);

		if (!type.IsEnum) {
			Type basetype = type.BaseType;
			if (basetype == typeof(object) || type.IsValueType) // don't show this in signatures
				basetype = null;
			
			ArrayList interface_names = new ArrayList ();
			foreach (Type i in type.GetInterfaces ())
				if ((type.BaseType == null || Array.IndexOf (type.BaseType.GetInterfaces (), i) == -1) && 
						InterfaceNotFromAnother (i, type.GetInterfaces ()))
					interface_names.Add (full.GetName (i));
			interface_names.Sort ();
			
			if (basetype != null || interface_names.Count > 0)
				buf.Append (" : ");
			
			if (basetype != null) {
				buf.Append (full.GetName (basetype));
				if (interface_names.Count > 0)
					buf.Append (", ");
			}
			
			for (int i = 0; i < interface_names.Count; i++){
				if (i != 0)
					buf.Append (", ");
				buf.Append (interface_names [i]);
			}
		}

		return buf.ToString ();
	}

	static string GetTypeKind (Type t)
	{
		if (t.IsEnum)
			return "enum";
		if (t.IsClass || t == typeof(System.Enum))
			return "class";
		if (t.IsInterface)
			return "interface";
		if (t.IsValueType)
			return "struct";
		throw new ArgumentException(t.FullName);
	}

	static string GetTypeVisibility (TypeAttributes ta)
	{
		switch (ta & TypeAttributes.VisibilityMask) {
		case TypeAttributes.Public:
		case TypeAttributes.NestedPublic:
			return "public";

		case TypeAttributes.NestedFamily:
		case TypeAttributes.NestedFamORAssem:
			return "protected";

		default:
			return null;
		}
	}

	static bool IsDelegate(Type type)
	{
		return typeof (System.Delegate).IsAssignableFrom (type) && !type.IsAbstract;
	}
	
	private static bool InterfaceNotFromAnother(Type i, Type[] i2)
	{
		foreach (Type t in i2)
			if (i != t && Array.IndexOf (t.GetInterfaces(), i) != -1)
				return false;
		return true;
	}

	protected override string GetConstructorDeclaration (ConstructorInfo constructor)
	{
		StringBuilder buf = new StringBuilder ();
		AppendVisibility (buf, constructor);
		if (buf.Length == 0)
			return null;

		buf.Append (' ');
		base.AppendTypeName (buf, constructor.DeclaringType.Name).Append (' ');
		AppendParameters (buf, constructor.GetParameters ());
		buf.Append (';');

		return buf.ToString ();
	}
	
	protected override string GetMethodDeclaration (MethodInfo method)
	{
		string decl = base.GetMethodDeclaration (method);
		if (decl != null)
			return decl + ";";
		return null;
	}

	protected override string RefTypeModifier {
		get {return "";}
	}

	protected override string GetFinalizerName (MethodInfo method)
	{
		return "~" + method.DeclaringType.Name + " ()";	
	}

	protected override StringBuilder AppendVisibility (StringBuilder buf, MethodBase method)
	{
		if (method == null)
			return buf;
		if (method.IsPublic)
			return buf.Append ("public");
		if (method.IsFamily || method.IsFamilyOrAssembly)
			return buf.Append ("protected");
		return buf;
	}

	protected override StringBuilder AppendModifiers (StringBuilder buf, MethodInfo method)
	{
		string modifiers = String.Empty;
		if (method.IsStatic) modifiers += " static";
		if (method.IsVirtual && !method.IsAbstract) {
			if ((method.Attributes & MethodAttributes.NewSlot) != 0) modifiers += " virtual";
			else modifiers += " override";
		}
		if (method.IsAbstract && !method.DeclaringType.IsInterface) modifiers += " abstract";
		if (method.IsFinal) modifiers += " sealed";
		if (modifiers == " virtual sealed") modifiers = "";

		return buf.Append (modifiers);
	}

	protected override StringBuilder AppendGenericMethod (StringBuilder buf, MethodInfo method)
	{
		try {
			Type[] args = DocUtils.GetGenericArguments (method);
			if (args.Length > 0) {
				buf.Append ("<");
				buf.Append (args [0].Name);
				for (int i = 1; i < args.Length; ++i)
					buf.Append (",").Append (args [i].Name);
				buf.Append (">");
			}
		}
		catch (NotSupportedException) {
			/* ignore */
		}
		return buf;
	}

	protected override StringBuilder AppendParameters (StringBuilder buf, ParameterInfo[] parameters)
	{
		return AppendParameters (buf, parameters, '(', ')');
	}

	private StringBuilder AppendParameters (StringBuilder buf, ParameterInfo[] parameters, char begin, char end)
	{
		buf.Append (begin);

		if (parameters.Length > 0) {
			AppendParameter (buf, parameters [0]);
			for (int i = 1; i < parameters.Length; ++i) {
				buf.Append (", ");
				AppendParameter (buf, parameters [i]);
			}
		}

		return buf.Append (end);
	}

	private StringBuilder AppendParameter (StringBuilder buf, ParameterInfo parameter)
	{
		if (parameter.ParameterType.IsByRef) {
			if (parameter.IsOut)
				buf.Append ("out ");
			else
				buf.Append ("ref ");
		}
		buf.Append (GetName (parameter.ParameterType)).Append (" ");
		return buf.Append (parameter.Name);
	}

	protected override string GetPropertyDeclaration (PropertyInfo property)
	{
		MethodBase method;

		string get_visible = null;
		if ((method = property.GetGetMethod (true)) != null && !method.IsPrivate && !method.IsAssembly && !method.IsFamilyAndAssembly)
			get_visible = AppendVisibility (new StringBuilder (), method).ToString ();
		string set_visible = null;
		if ((method = property.GetSetMethod (true)) != null  && !method.IsPrivate && !method.IsAssembly && !method.IsFamilyAndAssembly)
			set_visible = AppendVisibility (new StringBuilder (), method).ToString ();

		if ((set_visible == null || set_visible.Length  == 0) && 
				(get_visible == null || get_visible.Length == 0))
			return null;

		string visibility;
		StringBuilder buf = new StringBuilder ();
		if (get_visible != null && (set_visible == null || (set_visible != null && get_visible == set_visible)))
			buf.Append (visibility = get_visible);
		else if (set_visible != null && get_visible == null)
			buf.Append (visibility = set_visible);
		else
			buf.Append (visibility = "public");

		// Pick an accessor to use for static/virtual/override/etc. checks.
		method = property.GetSetMethod (true);
		if (method == null)
			method = property.GetGetMethod (true);
	
		string modifiers = String.Empty;
		if (method.IsStatic) modifiers += " static";
		if (method.IsVirtual && !method.IsAbstract) {
				if ((method.Attributes & MethodAttributes.NewSlot) != 0)
					modifiers += " virtual";
				else
					modifiers += " override";
		}
		if (method.IsAbstract && !method.DeclaringType.IsInterface)
			modifiers += " abstract";
		if (method.IsFinal)
			modifiers += " sealed";
		if (modifiers == " virtual sealed")
			modifiers = "";
		buf.Append (modifiers).Append (' ');

		buf.Append (GetName (property.PropertyType)).Append (' ');
	
		MemberInfo[] defs = property.DeclaringType.GetDefaultMembers ();
		string name = property.Name;
		foreach (MemberInfo mi in defs) {
			if (mi == property) {
				name = "this";
				break;
			}
		}
		buf.Append (name);
	
		if (property.GetIndexParameters ().Length != 0) {
			AppendParameters (buf, property.GetIndexParameters (), '[', ']');
		}

		buf.Append (" {");
		if (set_visible != null) {
			if (set_visible != visibility)
				buf.Append (' ').Append (set_visible);
			buf.Append (" set;");
		}
		if (get_visible != null) {
			if (get_visible != visibility)
				buf.Append (' ').Append (get_visible);
			buf.Append (" get;");
		}
		buf.Append (" };");
	
		return buf.ToString ();
	}

	protected override string GetFieldDeclaration (FieldInfo field)
	{
		if (field.DeclaringType.IsEnum && field.Name == "value__")
			return null; // This member of enums aren't documented.

		StringBuilder buf = new StringBuilder ();
		AppendFieldVisibility (buf, field);
		if (buf.Length == 0)
			return null;

		if (field.DeclaringType.IsEnum)
			return field.Name;

		if (field.IsStatic && !field.IsLiteral)
			buf.Append (" static");
		if (field.IsInitOnly)
			buf.Append (" readonly");
		if (field.IsLiteral)
			buf.Append (" const");

		buf.Append (' ').Append (GetName (field.FieldType)).Append (' ');
		buf.Append (field.Name);
		AppendFieldValue (buf, field);
		buf.Append (';');

		return buf.ToString ();
	}

	static StringBuilder AppendFieldVisibility (StringBuilder buf, FieldInfo field)
	{
		if (field.IsPublic)
			return buf.Append ("public");
		if (field.IsFamily || field.IsFamilyOrAssembly)
			return buf.Append ("protected");
		return buf;
	}

	static StringBuilder AppendFieldValue (StringBuilder buf, FieldInfo field)
	{
		if (field.DeclaringType.IsEnum)
			return buf;
		if (field.IsLiteral || (field.IsStatic && field.IsInitOnly)) {
			object val = field.GetValue (null);
			if (val == null)
				buf.Append (" = ").Append ("null");
			else if (val is Enum)
				buf.Append (" = ").Append (val.ToString ());
			else if (val is IFormattable) {
				string value = ((IFormattable)val).ToString();
				if (val is string)
					value = "\"" + value + "\"";
				buf.Append (" = ").Append (value);
			}
		}
		return buf;
	}

	protected override string GetEventDeclaration (EventInfo e)
	{
		StringBuilder buf = new StringBuilder ();
		if (AppendVisibility (buf, e.GetAddMethod ()).Length == 0)
			return null;

		AppendModifiers (buf, e.GetAddMethod ());

		buf.Append (" event ");
		buf.Append (GetName (e.EventHandlerType)).Append (' ');
		buf.Append (e.Name).Append (';');

		return buf.ToString ();
	}
}

class CSharpMemberFormatter : CSharpFullMemberFormatter {
	protected override StringBuilder AppendNamespace (StringBuilder buf, Type type)
	{
		return buf;
	}
}

class DocTypeFullMemberFormatter : MemberFormatter {
	protected override char NestedTypeSeparator {
		get {return '+';}
	}
}

class DocTypeMemberFormatter : DocTypeFullMemberFormatter {
	protected override StringBuilder AppendNamespace (StringBuilder buf, Type type)
	{
		return buf;
	}
}

class SlashDocMemberFormatter : MemberFormatter {

	protected override char[] GenericTypeContainer {
		get {return new char[]{'{', '}'};}
	}

	private bool AddTypeCount = true;

	protected override string GetTypeName (Type type)
	{
		return base.GetTypeName (type);
	}

	private Type genDeclType;
	private MethodBase genDeclMethod;

	protected override StringBuilder AppendTypeName (StringBuilder buf, Type type)
	{
		if (DocUtils.IsGenericParameter (type)) {
			int l = buf.Length;
			if (genDeclType != null) {
				Type[] genArgs = DocUtils.GetGenericArguments (genDeclType);
				for (int i = 0; i < genArgs.Length; ++i) {
					if (genArgs [i].Name == type.Name) {
						buf.Append ('`').Append (i);
						break;
					}
				}
			}
			if (genDeclMethod != null) {
				Type[] genArgs = null;
				try {
					genArgs = DocUtils.GetGenericArguments (genDeclMethod);
				}
				catch (NotSupportedException) {
					genArgs = new Type [0];
				}
				for (int i = 0; i < genArgs.Length; ++i) {
					if (genArgs [i].Name == type.Name) {
						buf.Append ("``").Append (i);
						break;
					}
				}
			}
			if (buf.Length == l) {
				throw new Exception (string.Format (
						"Unable to translate generic parameter {0}; genDeclType={1}, genDeclMethod={2}", 
						type.Name, genDeclType, genDeclMethod));
			}
		}
		else {
			base.AppendTypeName (buf, type);
			if (AddTypeCount) {
				int numArgs = DocUtils.GetGenericArguments (type).Length;
				if (type.DeclaringType != null)
					numArgs -= DocUtils.GetGenericArguments (type).Length;
				if (numArgs > 0) {
					buf.Append ('`').Append (numArgs);
				}
			}
		}
		return buf;
	}

	protected override StringBuilder AppendGenericType (StringBuilder buf, Type type)
	{
		if (!AddTypeCount)
			base.AppendGenericType (buf, type);
		else
			AppendType (buf, type);
		return buf;
	}

	private StringBuilder AppendType (StringBuilder buf, Type type)
	{
		int numArgs = DocUtils.GetGenericArguments (type).Length;
		if (type.DeclaringType != null) {
			AppendType (buf, type.DeclaringType).Append (NestedTypeSeparator);
			numArgs -= DocUtils.GetGenericArguments (type.DeclaringType).Length;
		}
		base.AppendTypeName (buf, type);
		if (numArgs > 0) {
			buf.Append ('`').Append (numArgs);
		}
		return buf;
	}

	protected override string GetConstructorName (ConstructorInfo constructor)
	{
		return GetMethodBaseName (constructor, "#ctor");
	}

	protected override string GetMethodName (MethodInfo method)
	{
		return GetMethodBaseName (method, method.Name);
	}

	private string GetMethodBaseName (MethodBase method, string name)
	{
		StringBuilder buf = new StringBuilder ();
		buf.Append (GetTypeName (method.DeclaringType));
		buf.Append ('.');
		buf.Append (name);
		try {
			Type[] genArgs = DocUtils.GetGenericArguments (method);
			if (genArgs.Length > 0)
				buf.Append ("``").Append (genArgs.Length);
		}
		catch (NotSupportedException) {
			/* ignore */
		}
		ParameterInfo[] parameters = method.GetParameters ();
		genDeclType = method.DeclaringType;
		genDeclMethod = method;
		AppendParameters (buf, DocUtils.GetGenericArguments (method.DeclaringType), parameters);
		genDeclType = null;
		genDeclMethod = null;
		return buf.ToString ();
	}

	private StringBuilder AppendParameters (StringBuilder buf, Type[] genArgs, ParameterInfo[] parameters)
	{
		if (parameters.Length == 0)
			return buf;

		buf.Append ('(');

		AppendParameter (buf, genArgs, parameters [0]);
		for (int i = 1; i < parameters.Length; ++i) {
			buf.Append (',');
			AppendParameter (buf, genArgs, parameters [i]);
		}

		return buf.Append (')');
	}

	private StringBuilder AppendParameter (StringBuilder buf, Type[] genArgs, ParameterInfo parameter)
	{
		AddTypeCount = false;
		buf.Append (GetTypeName (parameter.ParameterType));
		AddTypeCount = true;
		return buf;
	}

	protected override string GetPropertyName (PropertyInfo property)
	{
		StringBuilder buf = new StringBuilder ();
		buf.Append (GetName (property.DeclaringType));
		buf.Append ('.');
		buf.Append (property.Name);
		ParameterInfo[] parameters = property.GetIndexParameters ();
		if (parameters.Length > 0) {
			genDeclType = property.DeclaringType;
			buf.Append ('(');
			Type[] genArgs = DocUtils.GetGenericArguments (property.DeclaringType);
			AppendParameter (buf, genArgs, parameters [0]);
			for (int i = 1; i < parameters.Length; ++i) {
				 buf.Append (',');
				 AppendParameter (buf, genArgs, parameters [i]);
			}
			buf.Append (')');
			genDeclType = null;
		}
		return buf.ToString ();
	}

	protected override string GetFieldName (FieldInfo field)
	{
		return string.Format ("{0}.{1}",
			GetName (field.DeclaringType), field.Name);
	}

	protected override string GetEventName (EventInfo e)
	{
		return string.Format ("{0}.{1}",
			GetName (e.DeclaringType), e.Name);
	}

	protected override string GetTypeDeclaration (Type type)
	{
		string name = GetName (type);
		if (type == null)
			return null;
		return "T:" + name;
	}

	protected override string GetConstructorDeclaration (ConstructorInfo constructor)
	{
		string name = GetName (constructor);
		if (name == null)
			return null;
		return "M:" + name;
	}

	protected override string GetMethodDeclaration (MethodInfo method)
	{
		string name = GetName (method);
		if (name == null)
			return null;
		if (method.Name == "op_Implicit" || method.Name == "op_Explicit") {
			genDeclType = method.DeclaringType;
			genDeclMethod = method;
			name += "~" + GetName (method.ReturnType);
			genDeclType = null;
			genDeclMethod = null;
		}
		return "M:" + name;
	}

	protected override string GetPropertyDeclaration (PropertyInfo property)
	{
		string name = GetName (property);
		if (name == null)
			return null;
		return "P:" + name;
	}

	protected override string GetFieldDeclaration (FieldInfo field)
	{
		string name = GetName (field);
		if (name == null)
			return null;
		return "F:" + name;
	}

	protected override string GetEventDeclaration (EventInfo e)
	{
		string name = GetName (e);
		if (name == null)
			return null;
		return "E:" + name;
	}
}

class FileNameMemberFormatter : SlashDocMemberFormatter {
	protected override StringBuilder AppendNamespace (StringBuilder buf, Type type)
	{
		return buf;
	}

	protected override char NestedTypeSeparator {
		get {return '+';}
	}
}

