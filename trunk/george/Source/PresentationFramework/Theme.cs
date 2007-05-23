#define UseMicrosoftTheme
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
static class Theme {
    static ResourceDictionary ResourceDictionary;

    static Theme() {
#if UseMicrosoftTheme
#if Implementation
		const string ThemeAssemblyName = "Mono.PresentationFramework.Luna.dll";
#else
		const string ThemeAssemblyName = "PresentationFramework.Luna.dll";
#endif
		string theme_assembly_file = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, ThemeAssemblyName);
#if Implementation
        //HACK: NUnit copies the assemblies to another directory.
        if (!File.Exists(theme_assembly_file))
            theme_assembly_file = @"E:\George Giolfan\Development\Others\Mono\SOC\Private test\bin\" + ThemeAssemblyName;
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
		ResourceDictionary = (ResourceDictionary)XamlReader.Load(assembly.GetManifestResourceStream(theme_xaml_file_resource_name));
    }

	static public Style GetStyle(object key) {
		return (Style)ResourceDictionary[key];
	}
}