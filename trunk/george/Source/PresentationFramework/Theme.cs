#if Implementation
using System.Reflection;
using System.IO;
static class Theme {
	/// <summary>
	/// Makes sure that the theme works.
	/// </summary>
	/// <remarks>
	/// Now the assemblies are not placed in the GAC, so when a Visual Studio project only references this one, WPF will not automatically find the theme assembly since it will look in the directory where Visual Studio builds the project. To work, projects will need to have their build directory in a subdirectory of the directory containing the Debug directory in which both this and the theme assembly are built.
	/// </remarks>
    static public void Load() {
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
    }
}
#endif