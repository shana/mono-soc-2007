//
// Logger.cs: Logs server events.
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Copyright (C) 2007 Brian Nickel
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
using System.IO;
using System.Text;
using System.Globalization;

namespace FastCgi
{
	[Flags]
	public enum LogLevel
	{
		None    = 0x00,
		Error   = 0x01,
		Warning = 0x02,
		Notice  = 0x04,
		All     = 0xFF
	}
	
	/// <summary>
	///   Logs contents to a specified location.
	/// </summary>
	public static class Logger
	{
		private static StreamWriter writer;
		private static LogLevel     level;
		private static object       write_lock = new object ();
		
		public static LogLevel Level {
			get {return level;}
			set {level = value;}
		}
		
		public static void Open (string path)
		{
			lock (write_lock) {
				Close ();
				FileInfo info = new FileInfo (path);
				Stream stream = info.OpenWrite ();
				stream.Seek (0, SeekOrigin.End);
				writer = new StreamWriter (stream);
			}
		}
		
		public static void Write (LogLevel level, IFormatProvider provider, string format, params object [] args)
		{
			Write (level, string.Format (provider, format, args));
		}
				
		public static void Write (LogLevel level, string format, params object [] args)
		{
			Write (level, CultureInfo.CurrentCulture, format, args);
		}
		
		public static void Write (LogLevel level, string message)
		{
			if (writer == null)
				throw new InvalidOperationException
					("Cannot write to closed streams.");
			
			if ((Level & level) == LogLevel.None)
				return;
			
			string text = string.Format (CultureInfo.CurrentCulture,
				"[{0:u}] {1} {2}",
				DateTime.Now,
				level,
				message);
			
			lock (write_lock) {
				writer.WriteLine (text);
				Console.WriteLine (text);
				writer.Flush ();
			}
		}
		
		public static void Close ()
		{
			lock (write_lock) {
				if (writer == null)
					return;
				
				writer.Close ();
				writer = null;
			}
		}
	}
}
