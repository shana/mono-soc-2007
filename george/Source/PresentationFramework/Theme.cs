//
// Theme.cs
//
// Author:
//   George Giolfan (georgegiolfan@yahoo.com)
//
// Copyright (C) 2007 George Giolfan
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
#if Implementation
using System;
using System.IO;
using System.Reflection;
using System.Windows;
static class Theme
{
	static bool loaded;

	/// <summary>
	/// Makes sure that the theme works.
	/// </summary>
	/// <remarks>
	/// Now the assemblies are not placed in the GAC, so when a Visual Studio project only references this one, WPF will not automatically find the theme assembly since it will look in the directory where Visual Studio builds the project. To work, projects will need to have their build directory in a subdirectory of the directory containing the Debug directory in which both this and the theme assembly are built.
	/// </remarks>
	static public void Load ()
	{
		if (loaded)
			return;
		const string ThemeAssemblyName = "Mono.PresentationFramework.Luna.dll";
		string assembly_location = new FileInfo (Assembly.GetExecutingAssembly ().Location).DirectoryName;
		string theme_assembly_file = Path.Combine (assembly_location, ThemeAssemblyName);
		if (!File.Exists (theme_assembly_file)) {
			while (true) {
				DirectoryInfo parent_directory = Directory.GetParent (assembly_location);
				assembly_location = parent_directory.FullName;
				string file_name = Path.Combine (assembly_location, "Debug" + Path.DirectorySeparatorChar + ThemeAssemblyName);
				if (File.Exists (file_name)) {
					File.Copy (file_name, theme_assembly_file);
					break;
				}
			}
		}
		const int WindowsVistaMajorVersion = 6;
		if (Environment.OSVersion.Version.Major == WindowsVistaMajorVersion) {
			if (Application.Current == null)
				new Application ();
			Application.Current.Resources.MergedDictionaries.Add (Application.LoadComponent (new Uri ("PresentationFramework.Luna;V3.0.0.0;31bf3856ad364e35;component\\themes/luna.normalcolor.xaml", UriKind.Relative)) as ResourceDictionary);
			Application.Current.Resources.MergedDictionaries.Add (Application.LoadComponent (new Uri ("Mono.PresentationFramework.Luna;V3.0.0.0;;component\\themes/luna.normalcolor.xaml", UriKind.Relative)) as ResourceDictionary);
		}
		loaded = true;
	}
}
#endif