//
// ApplicationServer.cs
//
// Authors:
//      Marek Habersack (mhabersack@novell.com)
//
// Documentation:
//	Brian Nickel
//
// Copyright (c) Copyright 2007 Novell, Inc
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
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

namespace Mono.WebServer
{
	// FIXME: Add remarks on conditional and how this works.
	
	/// <summary>
	///    This sealed class provides methods for providing additional
	///    information in the diagnostic trace.
	/// </summary>
	public sealed class WebTrace
	{
		/// <summary>
		///    Gets the name of a method contained in a specified stack
		///    frame.
		/// </summary>
		/// <param name="sf">
		///    A <see cref="StackFrame" /> object to get the name of.
		/// </param>
		/// <returns>
		///    A <see cref="string" /> containing the name of the
		///    method in the specified stack frame.
		/// </returns>
		static string GetMethodName (StackFrame sf)
		{
			MethodBase mi = sf.GetMethod ();

			if (mi != null)
				return String.Format ("{0}.{1}(): ", mi.ReflectedType, mi.Name);
			return null;
		}

		/// <summary>
		///    Gets extra info about the current process and a specified
		///    stack frame.
		/// </summary>
		/// <param name="sf">
		///    A <see cref="StackFrame" /> object to get information
		///    about.
		/// </param>
		/// <returns>
		///    A <see cref="string" /> containing the tread ID,
		///    app-domain ID, file path, and file line number, of the
		///    current stack.
		/// </returns>
		static string GetExtraInfo (StackFrame sf)
		{
#if NET_2_0
			string threadid = String.Format ("thread_id: {0}", Thread.CurrentThread.ManagedThreadId.ToString ("x"));
			string domainid = String.Format ("appdomain_id: {0}", AppDomain.CurrentDomain.Id.ToString ("x"));
#else
			string threadid = String.Format ("thread_hash: {0}", Thread.CurrentThread.GetHashCode ().ToString ("x"));
			string domainid = String.Format ("appdomain_hash: {0}", AppDomain.CurrentDomain.GetHashCode ().ToString ("x"));
#endif
			
			string filepath = sf != null ? sf.GetFileName () : null;
			if (filepath != null && filepath.Length > 0)
				return String.Format (" [{0}, {1}, in {2}:{3}]", domainid, threadid, filepath, sf.GetFileLineNumber ());
			else
				return String.Format (" [{0}, {1}]", domainid, threadid);
		}

		/// <summary>
		///    Gets extra info about the process.
		/// </summary>
		/// <returns>
		///    A <see cref="string" /> containing the thread and
		///    app-domain ID's of the current stack.
		/// </returns>
		static string GetExtraInfo ()
		{
			return GetExtraInfo (null);
		}

		/// <summary>
		///    Enters a method in the diagnostic trace with a custom
		///    message.
		/// </summary>
		/// <param name="format">
		///    A <see cref="string" /> containing a formattable string
		///    to be written to the diagnostic trace.
		/// </param>
		/// <param name="sf">
		///    A <see cref="StackFrame" /> object to display information
		///    about.
		/// </param>
		/// <param name="parms">
		///    A <see cref="object[]" /> containing values to use when
		///    formatting the message.
		/// </param>
		/// <remarks>
		///    See <see cref="string.Format(string,object[])" /> for
		///    more details on formatting.
		/// </remarks>
		static void Enter (string format, StackFrame sf, params object[] parms)
		{
			StringBuilder sb = new StringBuilder ("Enter: ");
					
			string methodName = GetMethodName (sf);
			if (methodName != null)
				sb.Append (methodName);
			if (format != null)
				sb.AppendFormat (format, parms);
			sb.Append (GetExtraInfo (sf));
			
			Trace.WriteLine (sb.ToString ());
			Trace.Indent ();
		}
		
		/// <summary>
		///    Enters a method in the diagnostic trace with a custom
		///    message.
		/// </summary>
		/// <param name="format">
		///    A <see cref="string" /> containing a formattable string
		///    to be written to the diagnostic trace.
		/// </param>
		/// <param name="parms">
		///    A <see cref="object[]" /> containing values to use when
		///    formatting the message.
		/// </param>
		/// <remarks>
		///    See <see cref="string.Format(string,object[])" /> for
		///    more details on formatting.
		/// </remarks>
		[Conditional ("WEBTRACE")]
		public static void Enter (string format, params object[] parms)
		{
			Enter (format, new StackFrame (1), parms);
		}

		/// <summary>
		///    Enters a method in the diagnostic trace.
		/// </summary>
		[Conditional ("WEBTRACE")]
		public static void Enter ()
		{
			Enter (null, new StackFrame (1), null);
		}

		/// <summary>
		///    Leaves a method in the diagnostic trace with a custom
		///    message.
		/// </summary>
		/// <param name="format">
		///    A <see cref="string" /> containing a formattable string
		///    to be written to the diagnostic trace.
		/// </param>
		/// <param name="sf">
		///    A <see cref="StackFrame" /> object to display information
		///    about.
		/// </param>
		/// <param name="parms">
		///    A <see cref="object[]" /> containing values to use when
		///    formatting the message.
		/// </param>
		/// <remarks>
		///    See <see cref="string.Format(string,object[])" /> for
		///    more details on formatting.
		/// </remarks>
		static void Leave (string format, StackFrame sf, params object[] parms)
		{
			StringBuilder sb = new StringBuilder ("Leave: ");

			string methodName = GetMethodName (sf);
			if (methodName != null)
				sb.Append (methodName);
			if (format != null)
				sb.AppendFormat (format, parms);
			sb.Append (GetExtraInfo (sf));
			
			Trace.Unindent ();
			Trace.WriteLine (sb.ToString ());
		}
		
		/// <summary>
		///    Leaves a method in the diagnostic trace with a custom
		///    message.
		/// </summary>
		/// <param name="format">
		///    A <see cref="string" /> containing a formattable string
		///    to be written to the diagnostic trace.
		/// </param>
		/// <param name="parms">
		///    A <see cref="object[]" /> containing values to use when
		///    formatting the message.
		/// </param>
		/// <remarks>
		///    See <see cref="string.Format(string,object[])" /> for
		///    more details on formatting.
		/// </remarks>
		[Conditional ("WEBTRACE")]
		public static void Leave (string format, params object[] parms)
		{
			Leave (format, new StackFrame (1), parms);
		}

		/// <summary>
		///    Leaves a method in the diagnostic trace.
		/// </summary>
		[Conditional ("WEBTRACE")]
		public static void Leave ()
		{
			Leave (null, new StackFrame (1), null);
		}

		/// <summary>
		///    Writes a formatted line of text to the diagnostic trace.
		/// </summary>
		/// <param name="format">
		///    A <see cref="string" /> containing a formattable string
		///    to be written to the diagnostic trace.
		/// </param>
		/// <param name="parms">
		///    A <see cref="object[]" /> containing values to use when
		///    formatting the message.
		/// </param>
		/// <remarks>
		///    See <see cref="string.Format(string,object[])" /> for
		///    more details on formatting.
		/// </remarks>
		[Conditional ("WEBTRACE")]
		public static void WriteLine (string format, params object[] parms)
		{
			if (format == null)
				return;
			
			StringBuilder sb = new StringBuilder ();
			sb.AppendFormat (format, parms);
			sb.Append (GetExtraInfo ());
			Trace.WriteLine (sb.ToString ());
		}

		/// <summary>
		///    Conditionally writes a formatted line of text to the
		///    diagnostic trace.
		/// </summary>
		/// <param name="cond">
		///    A <see cref="bool" /> specifying whether or not to write
		///    the line.
		/// </param>
		/// <param name="format">
		///    A <see cref="string" /> containing a formattable string
		///    to be written to the diagnostic trace.
		/// </param>
		/// <param name="parms">
		///    A <see cref="object[]" /> containing values to use when
		///    formatting the message.
		/// </param>
		/// <remarks>
		///    See <see cref="string.Format(string,object[])" /> for
		///    more details on formatting.
		/// </remarks>
		[Conditional ("WEBTRACE")]
		public static void WriteLineIf (bool cond, string format, params object[] parms)
		{
			if (!cond)
				return;
			
			if (format == null)
				return;
			
			StringBuilder sb = new StringBuilder ();
			sb.AppendFormat (format, parms);
			sb.Append (GetExtraInfo ());
			Trace.WriteLine (sb.ToString ());
		}
	}
}
