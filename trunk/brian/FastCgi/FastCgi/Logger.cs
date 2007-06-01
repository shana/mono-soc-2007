/***********************************************************************
 *                                                                     *
 *  Copyright (c) 2007 Brian Nickel <brian.nickel@gmail.com            *
 *                                                                     *
 *  Permission is hereby granted, free of charge, to any person        *
 *  obtaining a copy of this software and associated documentation     *
 *  files (the "Software"), to deal in the Software without            *
 *  restriction, including without limitation the rights to use,       *
 *  copy, modify, merge, publish, distribute, sublicense, and/or sell  *
 *  copies of the Software, and to permit persons to whom the          *
 *  Software is furnished to do so, subject to the following           *
 *  conditions:                                                        *
 *                                                                     *
 *  The above copyright notice and this permission notice shall be     *
 *  included in all copies or substantial portions of the Software.    *
 *                                                                     *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,    *
 *  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES    *
 *  OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND           *
 *  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT        *
 *  HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,       *
 *  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING       *
 *  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR      *
 *  OTHER DEALINGS IN THE SOFTWARE.                                    *
 *                                                                     *
 ***********************************************************************/

using System;
using System.IO;

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
		private static StreamWriter _writer;
		private static LogLevel     _level;
		private const  string       _lock = "LOCK";
		
		public static LogLevel Level
		{
			get {return _level;}
			set {_level = value;}
		}
		
		public static void Open (string path)
		{
			lock (_lock)
			{
				if (_writer != null)
				{
					_writer.Close ();
				}
			
				FileInfo info = new FileInfo (path);
				Stream stream = info.OpenWrite ();
				stream.Seek (0, SeekOrigin.End);
				_writer = new StreamWriter (stream);
			}
		}
		
		public static void Write (LogLevel level, string message)
		{
			lock (_lock)
			{
				if (_writer == null)
				{
					throw new InvalidOperationException ("Cannot write to closed streams.");
				}
				
				if ((_level & level) == LogLevel.None)
				{
					return;
				}
				
				_writer.WriteLine ("[" + DateTime.Now.ToString () + "] " + level.ToString () + "\t" + message);
				Console.WriteLine ("[" + DateTime.Now.ToString () + "] " + level.ToString () + "\t" + message);
				_writer.Flush ();
			}
		}
		
		public static void Close ()
		{
			lock (_lock)
			{
				if (_writer == null)
				{
					return;
				}
				
				_writer.Close ();
				_writer = null;
			}
		}
	}
}
