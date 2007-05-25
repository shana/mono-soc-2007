#define UseMicrosoftTheme
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
static class Theme {
	static ResourceDictionary resource_dictionary;
    static public void Load() {
		if (Application.Current == null) {
			new Application();
		}
		if (resource_dictionary != null)
			if (Application.Current.Resources.MergedDictionaries.Contains(resource_dictionary))
				return;
			else {
				Application.Current.Resources.MergedDictionaries.Add(resource_dictionary);
				return;
			}
#if UseMicrosoftTheme
#if Implementation
		const string ThemeAssemblyName = "Mono.PresentationFramework.Luna.dll";
#else
		const string ThemeAssemblyName = "PresentationFramework.Luna.dll";
#endif
		string assembly_location = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
		string theme_assembly_file = Path.Combine(assembly_location, ThemeAssemblyName);
#if Implementation
		//HACK:
		while (!File.Exists(theme_assembly_file)) {
			DirectoryInfo parent_directory = Directory.GetParent(assembly_location);
			assembly_location = parent_directory.FullName;
			theme_assembly_file = Path.Combine(assembly_location, "Debug" + Path.DirectorySeparatorChar + ThemeAssemblyName);
		}
#endif
#endif
		Assembly assembly;
		string theme_xaml_file_resource_name;
#if UseMicrosoftTheme
		assembly = Assembly.LoadFile(theme_assembly_file);
		theme_xaml_file_resource_name = "themes.luna.normalcolor.xaml";
#else
		assembly = Assembly.GetExecutingAssembly();
		theme_xaml_file_resource_name = "Mono.PresentationFramework.Theme.xaml";
#endif
		resource_dictionary = (ResourceDictionary)XamlReader.Load(assembly.GetManifestResourceStream(theme_xaml_file_resource_name));
		Application.Current.Resources.MergedDictionaries.Add(resource_dictionary);
    }
}