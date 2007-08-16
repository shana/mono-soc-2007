//
// ApplicationManager.cs
//
// Authors:
//	Gonzalo Paniagua Javier (gonzalo@ximian.com)
//	Lluis Sanchez Gual (lluis@ximian.com)
//	Brian Nickel (brian.nickel@gmail.com)
//
// Copyright (c) Copyright 2002,2003,2004 Novell, Inc
// Copyright (c) 2007 Brian Nickel
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.Web;
using System.Web.Hosting;
#if NET_2_0
using System.Collections.Generic;
#else
using System.Collections;
#endif
using System.Text;
using System.Threading;
using System.IO;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Mono.WebServer
{
	public delegate void HostCreatedHandler (string vhost, int port,
	                                         string path,
	                                         IApplicationHost appHost);
	
	public class ApplicationManager : MarshalByRefObject
	{
		public event HostCreatedHandler HostCreated;
		
		bool auto_map;
		// TODO: Add automatic appliciation lookup.
		// bool auto_find_apps;
		bool verbose;
		Type host_type;
		WebSource web_source;

		// This is much faster than hashtable for typical cases.
		#if NET_2_0
		List<VPathToHost> vpathToHost = new List<VPathToHost> ();
		#else
		ArrayList vpathToHost = new ArrayList ();
		#endif
		
		[Obsolete]
		public ApplicationManager (Type hostType) :
			this (hostType, false, false)
		{
			host_type = hostType;
		}
		
		public ApplicationManager (Type hostType, bool autoMapPaths,
			bool autoFindApps)
		{
			host_type = hostType;
			auto_map = autoMapPaths;
		}
		
		protected ApplicationManager (WebSource source)
		{
			web_source = source;
		}
		
		public virtual Type GetApplicationHostType ()
		{
			return host_type;
		}
		
		public bool Verbose {
			get { return verbose; }
			set { verbose = value; }
		}

		public WebSource WebSource {
			get { return web_source; }
		}

		public void AddApplication (string vhost, int vport, string vpath, string fullPath)
		{
			// TODO - check for duplicates, sort, optimize, etc.
			if (verbose) {
				Console.WriteLine("Registering application:");
				Console.WriteLine("    Host:          {0}", (vhost != null) ? vhost : "any");
				Console.WriteLine("    Port:          {0}", (vport != -1) ?
					vport.ToString () : "any");
				Console.WriteLine("    Virtual path:  {0}", vpath);
				Console.WriteLine("    Physical path: {0}", fullPath);
			}

			vpathToHost.Add (new VPathToHost (vhost, vport, vpath, fullPath));
		}

 		public void AddApplicationsFromConfigDirectory (string directoryName)
 		{
			if (verbose) {
				Console.WriteLine ("Adding applications from *.webapp files in " +
						   "directory '{0}'", directoryName);
			}

			DirectoryInfo di = new DirectoryInfo (directoryName);
			if (!di.Exists) {
				Console.Error.WriteLine ("Directory {0} does not exist.", directoryName);
				return;
			}
			
			foreach (FileInfo fi in di.GetFiles ("*.webapp"))
				AddApplicationsFromConfigFile (fi.FullName);
		}

 		public void AddApplicationsFromConfigFile (string fileName)
 		{
			if (verbose) {
				Console.WriteLine ("Adding applications from config file '{0}'", fileName);
			}

			try {
				XmlDocument doc = new XmlDocument ();
				doc.Load (fileName);

				foreach (XmlElement el in doc.SelectNodes ("//web-application")) {
					AddApplicationFromElement (el);
				}
			} catch {
				Console.WriteLine ("Error loading '{0}'", fileName);
				throw;
			}
		}

		void AddApplicationFromElement (XmlElement el)
		{
			XmlNode n;

			n = el.SelectSingleNode ("enabled");
			if (n != null && n.InnerText.Trim () == "false")
				return;

			string vpath = el.SelectSingleNode ("vpath").InnerText;
			string path = el.SelectSingleNode ("path").InnerText;

			string vhost = null;
			n = el.SelectSingleNode ("vhost");
#if !MOD_MONO_SERVER
			if (n != null)
				vhost = n.InnerText;
#else
			// TODO: support vhosts in xsp.exe
			string name = el.SelectSingleNode ("name").InnerText;
			if (verbose)
				Console.WriteLine ("Ignoring vhost {0} for {1}", n.InnerText, name);
#endif

			int vport = -1;
			n = el.SelectSingleNode ("vport");
#if !MOD_MONO_SERVER
			if (n != null)
				vport = Convert.ToInt32 (n.InnerText);
#else
			// TODO: Listen on different ports
			if (verbose)
				Console.WriteLine ("Ignoring vport {0} for {1}", n.InnerText, name);
#endif

			AddApplication (vhost, vport, vpath, path);
		}

 		public void AddApplicationsFromCommandLine (string applications)
 		{
 			if (applications == null)
 				throw new ArgumentNullException ("applications");
 
 			if (applications == "")
				return;

			if (verbose) {
				Console.WriteLine("Adding applications '{0}'...", applications);
			}

 			string [] apps = applications.Split (',');

			foreach (string str in apps) {
				string [] app = str.Split (':');

				if (app.Length < 2 || app.Length > 4)
					throw new ArgumentException ("Should be something like " +
								"[[hostname:]port:]VPath:realpath");

				int vport;
				string vhost;
				string vpath;
				string realpath;
				int pos = 0;

				if (app.Length >= 3) {
					vhost = app[pos++];
				} else {
					vhost = null;
				}

				if (app.Length >= 4) {
					// FIXME: support more than one listen port.
					vport = Convert.ToInt16 (app[pos++]);
				} else {
					vport = -1;
				}

				vpath = app [pos++];
				realpath = app[pos++];

				if (!vpath.EndsWith ("/"))
					vpath += "/";
 
 				string fullPath = System.IO.Path.GetFullPath (realpath);
				AddApplication (vhost, vport, vpath, fullPath);
 			}
 		}

		public void UnloadAll ()
		{
			lock (vpathToHost) {
				foreach (VPathToHost v in vpathToHost) {
					v.UnloadHost ();
				}
			}
		}
		
		public VPathToHost GetApplicationForPath (string vhost,
		                                          int vport,
		                                          string vpath,
		                                          string realPath)
		{
			VPathToHost host = GetApplicationForPath_mapping (
				vhost, vport, vpath, realPath);
			
			return host == null ? GetApplicationForPath_existing (
				vhost, vport, vpath) : host;
		}
		
		private VPathToHost GetApplicationForPath_mapping (string vhost,
		                                                   int vport,
		                                                   string vpath,
		                                                   string realPath)
		{
			// If we can't automap, abort.
			if (!auto_map || realPath == null)
				return null;
			
			#if NET_2_0
			List<VPathToHost> hosts =
				new List<VPathToHost> (vpathToHost);
			#else
			ArrayList hosts = new ArrayList (vpathToHost);
			#endif
			
			char sep = Path.DirectorySeparatorChar;
			string [] parts = vpath.Split ('/');
			int length = parts.Length;
			DirectoryInfo dir = new DirectoryInfo (realPath);
			
			while (true) {
				
				// Remove any extra blank parts from the path.
				while (length > 0 && parts [length - 1].Length == 0)
					length --;
				
				// If we run out of directories, there is
				// nothing left to map.
				if (dir == null || length < 0)
					return null;
				
				// We can't map to a non-existent directory. If
				// we find one, move up a level and try again.
				if (!dir.Exists) {
					length --;
					dir = dir.Parent;
					continue;
				}
				
				// Get the directory name with the trailing
				// slash.
				realPath = dir.FullName;
				if (realPath [realPath.Length - 1] != sep)
					realPath += sep;
				
				// Search through existing hosts. If their real
				// path is longer than our real path, we have an
				// application we can't use. If their path is
				// the same as ours, return that host.
				for (int i = hosts.Count - 1; i > 0; i--) {
					#if NET_2_0
					VPathToHost h = hosts [i];
					#else
					VPathToHost h = hosts [i] as VPathToHost;
					#endif
					
					if (!h.realPath.StartsWith (realPath)) {
						hosts.RemoveAt (i);
					} else if (realPath == h.realPath) {
						CreateHost (h);
						return h;
					}
				}
				
				// If the paths differ, we've reached the end of
				// the map and must escape so it can be built.
				if (length == 0 || parts [length - 1].Equals (
					dir.Name))
					break;
				
				// Check that if the directory contains both a
				// "Web.Config" file and a "Bin" directory. If
				// so, it is most likely an application.
				
				bool webConfigFound = false;
				bool binFound = false;
				
				foreach (FileInfo info in dir.GetFiles ()) {
					#if NET_2_0
					if (!webConfigFound && info.Name.Equals ("web.config",
						StringComparison.InvariantCultureIgnoreCase))
						webConfigFound = true;
					else if (!binFound && info.Name.Equals ("bin",
						StringComparison.InvariantCultureIgnoreCase))
						binFound = true;
					#else
					if (!webConfigFound && info.Name.ToLower () == "web.config")
						webConfigFound = true;
					else if (!binFound && info.Name.ToLower () == "bin")
						binFound = true;
					#endif
					
					if (binFound && webConfigFound)
						break;
				}
				
				// Move up a level and try again.
				length --;
				dir = dir.Parent;
			}
			
			// We must now have a valid directory and path.
			string vPath = string.Join ("/", parts, 0, length) + "/";
			
			// For now, don't care about host or port. It is safe to
			// assume that if two hosts have an identical root
			// directory, they are the same application.
			lock (vpathToHost) {
				AddApplication (null, -1, vPath,
					realPath);
				VPathToHost host = vpathToHost [
					vpathToHost.Count - 1] as VPathToHost;
				CreateHost (host);
				return host;
			}
		}
		
		private VPathToHost GetApplicationForPath_existing (string vhost,
		                                                    int vport,
		                                                    string vpath)
		{
			VPathToHost bestMatch = null;
			int bestMatchLength = 0;
			
			// Checks for the matching vhost with the longest name.
			// For example, if the request is for
			// "/foo/bar/test.aspx", and hosts exist for "/", "/foo/",
			// "/foo/bar/", "/foo/bar/" should be used.
			for (int i = vpathToHost.Count - 1; i > -1; i--) {
				VPathToHost v = (VPathToHost) vpathToHost [i];
				int matchLength = v.vpath.Length;
				if (matchLength <= bestMatchLength ||
					!v.Match (vhost, vport, vpath))
					continue;
				
				bestMatchLength = matchLength;
				bestMatch = v;
			}
			
			// Create the host and return the best match, if found.
			if (bestMatch != null) {
				CreateHost (bestMatch);
				return bestMatch;
			}
			
			if (verbose)
				Console.WriteLine (
					"No application defined for: {0}:{1}{2}",
					vhost, vport, vpath);
			
			return null;
		}
		
		[Obsolete]
		public VPathToHost GetApplicationForPath (string vhost, int port, string path,
							       bool defaultToRoot)
		{
			return GetApplicationForPath (vhost, port, path, null);
		}
		
		private void CreateHost (VPathToHost v)
		{
			lock (v) {
				if (v.AppHost == null) {
					v.CreateHost (this, web_source);
					if (HostCreated != null)
						HostCreated (v.vhost, v.vport,
							v.vpath, v.AppHost);
				}
			}
		}

		public void DestroyHost (IApplicationHost host)
		{
			// Called when the host appdomain is being unloaded
			for (int i = vpathToHost.Count - 1; i >= 0; i--) {
				VPathToHost v = (VPathToHost) vpathToHost [i];
				if (v.TryClearHost (host))
					break;
			}
		}
		
		public bool ApplicationsSet {
			get { return vpathToHost == null; }
		}

		public override object InitializeLifetimeService ()
		{
			return null;
		}
	}

	public class VPathToHost
	{
		public readonly string vhost;
		public readonly int vport;
		public readonly string vpath;
		public string realPath;
		public readonly bool haveWildcard;

		public IApplicationHost AppHost;
		public IRequestBroker RequestBroker;

		public VPathToHost (string vhost, int vport, string vpath, string realPath)
		{
			this.vhost = (vhost != null) ? vhost.ToLower (CultureInfo.InvariantCulture) : null;
			this.vport = vport;
			this.vpath = vpath;
			if (vpath == null || vpath == "" || vpath [0] != '/')
				throw new ArgumentException ("Virtual path must begin with '/': " + vpath,
								"vpath");

			this.realPath = realPath;
			this.AppHost = null;
			if (vhost != null && this.vhost.Length != 0 && this.vhost [0] == '*') {
				haveWildcard = true;
				if (this.vhost.Length > 2 && this.vhost [1] == '.')
					this.vhost = this.vhost.Substring (2);
			}
		}


		public bool TryClearHost (IApplicationHost host)
		{
			if (this.AppHost == host) {
				this.AppHost = null;
				return true;
			}

			return false;
		}

		public void UnloadHost ()
		{
			if (AppHost != null)
				AppHost.Unload ();

			AppHost = null;
		}

		public bool Redirect (string path, out string redirect)
		{
			redirect = null;
			int plen = path.Length;
			if (plen == this.vpath.Length - 1) {
				redirect = this.vpath;
				return true;
			}

			return false;
		}

		public bool Match (string vhost, int vport, string vpath)
		{
			if (vport != -1 && this.vport != -1 && vport != this.vport)
				return false;

			if (vhost != null && this.vhost != null && this.vhost != "*") {
				int length = this.vhost.Length;
				string lwrvhost = vhost.ToLower (CultureInfo.InvariantCulture);
				if (haveWildcard) {
					if (length > vhost.Length)
						return false;

					if (length == vhost.Length && this.vhost != lwrvhost)
						return false;

					if (vhost [vhost.Length - length - 1] != '.')
						return false;

					if (!lwrvhost.EndsWith (this.vhost))
						return false;

				} else if (this.vhost != lwrvhost) {
					return false;
				}
			}

			int local = vpath.Length;
			int vlength = this.vpath.Length;
			if (vlength > local) {
				// Check for /xxx requests to be redirected to /xxx/
				if (this.vpath [vlength - 1] != '/')
					return false;

				return (vlength - 1 == local && this.vpath.Substring (0, vlength - 1) == vpath);
			}

			return (vpath.StartsWith (this.vpath));
		}

		public void CreateHost (ApplicationManager server, WebSource webSource)
		{
			string v = vpath;
			if (v != "/" && v.EndsWith ("/")) {
				v = v.Substring (0, v.Length - 1);
			}
			
			Type type = (webSource != null) ? webSource.GetApplicationHostType () : server.GetApplicationHostType ();
				

			AppHost = ApplicationHost.CreateApplicationHost (type, v, realPath) as IApplicationHost;
			AppHost.Server = server;
			
			// Link the host in the application domain with a request broker in the main domain
			if (webSource != null)
				RequestBroker = webSource.CreateRequestBroker ();
			AppHost.RequestBroker = RequestBroker;
		}
	}
	
	public class HttpErrors
	{
		static byte [] error500;
		static byte [] badRequest;

		static HttpErrors ()
		{
			string s = "HTTP/1.0 500 Server error\r\n" +
				   "Connection: close\r\n\r\n" +
				   "<html><head><title>500 Server Error</title><body><h1>Server error</h1>\r\n" +
				   "Your client sent a request that was not understood by this server.\r\n" +
				   "</body></html>\r\n";
			error500 = Encoding.ASCII.GetBytes (s);

			string br = "HTTP/1.0 400 Bad Request\r\n" + 
				"Connection: close\r\n\r\n" +
				"<html><head><title>400 Bad Request</title></head>" +
				"<body><h1>Bad Request</h1>The request was not understood" +
				"<p></body></html>";

			badRequest = Encoding.ASCII.GetBytes (br);
		}

		public static byte [] NotFound (string uri)
		{
			string s = String.Format ("HTTP/1.0 404 Not Found\r\n" + 
				"Connection: close\r\n\r\n" +
				"<html><head><title>404 Not Found</title></head>\r\n" +
				"<body><h1>Not Found</h1>The requested URL {0} was not found on this " +
				"server.<p>\r\n</body></html>\r\n", uri);

			return Encoding.ASCII.GetBytes (s);
		}

		public static byte [] BadRequest ()
		{
			return badRequest;
		}

		public static byte [] ServerError ()
		{
			return error500;
		}
	}

	public class Paths {
		private Paths ()
		{
		}

		public static void GetPathsFromUri (string uri, out string realUri, out string pathInfo)
		{
			// There's a hidden missing feature here... :)
			realUri = uri; pathInfo = "";
			string basepath = HttpRuntime.AppDomainAppPath;
			string vpath = HttpRuntime.AppDomainAppVirtualPath;
			if (vpath [vpath.Length - 1] != '/')
				vpath += '/';

			if (vpath.Length > uri.Length)
				return;

			uri = uri.Substring (vpath.Length);
			while (uri.Length > 0 && uri [0] == '/')
				uri = uri.Substring (1);

			int dot, slash;
			int lastSlash = uri.Length;
			bool windows = (Path.DirectorySeparatorChar == '\\');

			for (dot = uri.LastIndexOf ('.'); dot > 0; dot = uri.LastIndexOf ('.', dot - 1)) {
				slash = uri.IndexOf ('/', dot);
				string partial;
				if (slash == -1)
					slash = lastSlash;

				partial = uri.Substring (0, slash);
				lastSlash = slash;
				string partial_win = null;
				if (windows)
					partial_win = partial.Replace ('/', '\\');

				string path = Path.Combine (basepath, (windows ? partial_win : partial));
				if (!File.Exists (path))
					continue;
				
				realUri = vpath + uri.Substring (0, slash);
				pathInfo = uri.Substring (slash);
				break;
			}
		}
	}
}

