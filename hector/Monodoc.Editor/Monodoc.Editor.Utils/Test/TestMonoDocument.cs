//
// TestMonoDocument.cs: Test unit for MonoDocument class.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M

using System;
using System.IO;
using NUnit.Framework;

namespace Monodoc.Editor.Utils {
[TestFixture()]
public class TestMonoDocument {
	
	private string pathTest;
	private Type type;
	
	[SetUp()]
	public void Initialize ()
	{
		string filePath;
		
		filePath = Test.PathOfTestFiles ();
		pathTest = Path.Combine (filePath, "Examples");
		type = Type.GetType ("Monodoc.Editor.Utils.MonoDocument");
	}
	
	[Test()]
	public void ConstructorInvalid ()
	{
		MonoDocument document = null;
		
		try {
			document = new MonoDocument (String.Empty);
		} catch {
		}
		
		Assert.IsNotInstanceOfType (type, document, "CI01");
		Assert.IsNull (document, "CI02");
	}
	
	[Test()]
	public void ConstructorValid ()
	{
		string fileName;
		
		fileName = Path.Combine (pathTest, "Accel.xml");
		MonoDocument document = new MonoDocument (fileName);
		Assert.IsInstanceOfType (type, document, "CV02");
		Assert.IsNotNull (document, "CV02");
	}
}
}
