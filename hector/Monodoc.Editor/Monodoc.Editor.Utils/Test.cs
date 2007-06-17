//
// EcmaReader.cs: Class that reads an Monodoc XML Documentation.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M

using System;
using System.IO;

namespace Monodoc.Editor.Utils {
public class Test {
		
	public static string PathOfTestFiles ()
	{
		// FIXME: Este es un hack para correr los casos que depende de la
		// locacion cuando se corre el test.
		string path;
		path = Environment.CurrentDirectory;
		path = path.Replace ("bin" + Path.DirectorySeparatorChar + "Debug", String.Empty);
		path = path.Replace ("bin" + Path.DirectorySeparatorChar + "Release", String.Empty);
		path = Path.Combine (path, "Test");
		
		return path;
	}
}
}
