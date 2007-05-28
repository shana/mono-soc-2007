using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;

using Mono.GetOptions;

[assembly: AssemblyTitle("Monodocs-to-HTML")]
[assembly: AssemblyCopyright("Copyright (c) 2004 Joshua Tauberer <tauberer@for.net>, released under the GPL.")]
[assembly: AssemblyDescription("Convert Monodoc XML documentation to static HTML.")]
 
[assembly: Mono.UsageComplement("")]

public class Monodocs2HTML {
	
	private class Opts : Options {
		[Option("The base directory of the XML documentation")]
		public string source;
		
		[Option("The output directory of the XML documentation")]
		public string dest;

		[Option("The file extension for generated files (default 'html')")]
		public string ext = "html";

		[Option("Only update the given type")]
		public string onlytype;

		[Option("Use the given page template")]
		public string template;

		[Option("Dump the default page template to standard out so you may copy it to make a new template for the --template option.")]
		public bool dumptemplate;
	}

	static Opts opts = new Opts();
	
	public static void Main(string[] args) {
		try {
			opts.ProcessArgs(args);		
			Main2();
		} catch (ApplicationException appex) {
			Exception e = appex;
			do {
				Console.Error.WriteLine(e.Message);
				e = e.InnerException;
			} while (e != null);
		}
	}
	
	private static void Main2() {
		if (opts.dumptemplate) {
			DumpTemplate();
			return;
		}
		
		if (opts.source == null || opts.source == "" || opts.dest == null || opts.dest == "")
			throw new ApplicationException("The source and dest options must be specified.");
		
		Directory.CreateDirectory(opts.dest);
		
		// Load the stylesheets, overview.xml, and resolver
		
		XslTransform overviewxsl = LoadTransform("overview.xsl");
		XslTransform stylesheet = LoadTransform("stylesheet.xsl");
		XslTransform template;
		if (opts.template == null) {
			template = LoadTransform("defaulttemplate.xsl");
		} else {
			try {
				XmlDocument templatexsl = new XmlDocument();
				templatexsl.Load(opts.template);
				template = new XslTransform();
				template.Load(templatexsl);
			} catch (Exception e) {
				throw new ApplicationException("There was an error loading " + opts.template, e);
			}
		}
		
		XmlDocument overview = new XmlDocument();
		overview.Load(opts.source + "/index.xml");
		
		// Create the master page
		XsltArgumentList overviewargs = new XsltArgumentList();
		overviewargs.AddParam("ext", "", opts.ext);
		overviewargs.AddParam("basepath", "", "./");
		Generate(overview, overviewxsl, overviewargs, opts.dest + "/index." + opts.ext, template);
		overviewargs.RemoveParam("basepath", "");
		overviewargs.AddParam("basepath", "", "../");
		
		// Create the namespace & type pages
		
		XsltArgumentList typeargs = new XsltArgumentList();
		typeargs.AddParam("ext", "", opts.ext);
		typeargs.AddParam("basepath", "", "../");
		
		foreach (XmlElement ns in overview.SelectNodes("Overview/Types/Namespace")) {
			string nsname = ns.GetAttribute("Name");

			if (opts.onlytype != null && !opts.onlytype.StartsWith(nsname + "."))
				continue;
				
			System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(opts.dest + "/" + nsname);
			if (!d.Exists) d.Create();
			
			// Create the NS page
			overviewargs.AddParam("namespace", "", nsname);
			Generate(overview, overviewxsl, overviewargs, opts.dest + "/" + nsname + "/index." + opts.ext, template);
			overviewargs.RemoveParam("namespace", "");
			
			foreach (XmlElement ty in ns.SelectNodes("Type")) {
				string typefilebase = ty.GetAttribute("Name");
				string typename = ty.GetAttribute("DisplayName");
				if (typename.Length == 0)
					typename = typefilebase;
				
				if (opts.onlytype != null && !(nsname + "." + typename).StartsWith(opts.onlytype))
					continue;

				string typefile = opts.source + "/" + nsname + "/" + typefilebase + ".xml";
				if (!File.Exists(typefile)) continue;

				XmlDocument typexml = new XmlDocument();
				typexml.Load(typefile);
				
				Console.WriteLine(nsname + "." + typename);
				
				Generate(typexml, stylesheet, typeargs, opts.dest + "/" + nsname + "/" + typefilebase + "." + opts.ext, template);
			}
		}
	}
	
	private static void DumpTemplate() {
		XmlDocument dumptemplate = new XmlDocument();
		dumptemplate.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("defaulttemplate.xsl"));
		XmlTextWriter writer = new XmlTextWriter(Console.Out);
		writer.Formatting = Formatting.Indented;
		writer.Indentation = 2;
		writer.IndentChar = ' ';
		dumptemplate.WriteTo(writer);
	}
	
	private static void Generate(XmlDocument source, XslTransform transform, XsltArgumentList args, string output, XslTransform template) {
		using (TextWriter textwriter = new StreamWriter(new FileStream(output, FileMode.Create))) {
			XmlTextWriter writer = new XmlTextWriter(textwriter);
			writer.Formatting = Formatting.Indented;
			writer.Indentation = 2;
			writer.IndentChar = ' ';
			
			try {
				XmlDocument intermediate = new XmlDocument();
				intermediate.PreserveWhitespace = true;
				intermediate.Load(transform.Transform(source, args, new MyXmlResolver()));
				template.Transform(intermediate, new XsltArgumentList(), writer, null);
			} catch (Exception e) {
				throw new ApplicationException("An error occured while generating " + output, e);
			}
		}
	}
	
	private static XslTransform LoadTransform(string name) {
		try {
			XmlDocument xsl = new XmlDocument();
			xsl.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream(name));
			
			if (name == "overview.xsl") {
				// bit of a hack.  overview needs the templates in stylesheet
				// for doc formatting, and rather than write a resolver, I'll
				// just do the import for it.
				
				XmlNode importnode = xsl.DocumentElement.SelectSingleNode("*[name()='xsl:include']");
				xsl.DocumentElement.RemoveChild(importnode);
				
				XmlDocument xsl2 = new XmlDocument();
				xsl2.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("stylesheet.xsl"));
				foreach (XmlNode node in xsl2.DocumentElement.ChildNodes)
					xsl.DocumentElement.AppendChild(xsl.ImportNode(node, true));
			}
			
			XslTransform t = new XslTransform();
			t.Load(xsl);
			
			return t;
		} catch (Exception e) {
			throw new ApplicationException("Error loading " + name + " from internal resource", e);
		}
	}
	
	private class MyXmlResolver : XmlUrlResolver {
		/*public override Uri ResolveUri(Uri baseUri, string relativeUri) {
			return base.ResolveUri(new Uri("file://" + new DirectoryInfo(opts.source).FullName + "/"), relativeUri);
		}*/
	}
}
