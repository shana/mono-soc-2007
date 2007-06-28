#if Implementation
using System;
using System.IO;
using System.Reflection;
using System.Windows;
static class Theme {
	static bool loaded;

	/// <summary>
	/// Makes sure that the theme works.
	/// </summary>
	/// <remarks>
	/// Now the assemblies are not placed in the GAC, so when a Visual Studio project only references this one, WPF will not automatically find the theme assembly since it will look in the directory where Visual Studio builds the project. To work, projects will need to have their build directory in a subdirectory of the directory containing the Debug directory in which both this and the theme assembly are built.
	/// </remarks>
	static public void Load() {
		if (loaded)
			return;
		const string ThemeAssemblyName = "Mono.PresentationFramework.Luna.dll";
		string assembly_location = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
		string theme_assembly_file = Path.Combine(assembly_location, ThemeAssemblyName);
		if (!File.Exists(theme_assembly_file)) {
			while (true) {
				DirectoryInfo parent_directory = Directory.GetParent(assembly_location);
				assembly_location = parent_directory.FullName;
				string file_name = Path.Combine(assembly_location, "Debug" + Path.DirectorySeparatorChar + ThemeAssemblyName);
				if (File.Exists(file_name)) {
					File.Copy(file_name, theme_assembly_file);
					break;
				}
			}
		}
		const int WindowsVistaMajorVersion = 6;
		if (Environment.OSVersion.Version.Major == WindowsVistaMajorVersion) {
			if (Application.Current == null)
				new Application();
			Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(new Uri("PresentationFramework.Luna;V3.0.0.0;31bf3856ad364e35;component\\themes/luna.normalcolor.xaml", UriKind.Relative)) as ResourceDictionary);
			Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(new Uri("Mono.PresentationFramework.Luna;V3.0.0.0;;component\\themes/luna.normalcolor.xaml", UriKind.Relative)) as ResourceDictionary);
		}
		loaded = true;
	}
}
#endif