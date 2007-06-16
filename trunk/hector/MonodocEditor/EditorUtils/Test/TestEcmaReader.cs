//
// TestEcmaReader.cs: Test unit for EcmaReader class.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M

using System;
using System.IO;
using NUnit.Framework;

namespace Monodoc.EcmaUtils {
[TestFixture()]
public class TestEcmaReader {
	
	private string pathTest;
	private Type type;
	
	[SetUp()]
	public void Initialize ()
	{
		string filePath;
		
		filePath = Test.PathOfTestFiles ();
		pathTest = Path.Combine (filePath, "Examples");
		type = Type.GetType ("Monodoc.EcmaUtils.EcmaReader");
	}
	
	[Test()]
	public void ConstructorInvalid ()
	{
		EcmaReader reader = null;
		
		try {
			reader = new EcmaReader (String.Empty);
		} catch {
		}
		
		Assert.IsNotInstanceOfType (type, reader, "CI01");
		Assert.IsNull (reader, "CI02");
	}
	
	[Test()]
	public void ConstructorValid ()
	{
		string fileName;
		
		fileName = Path.Combine (pathTest, "Accel.xml");
		EcmaReader reader = new EcmaReader (fileName);
		Assert.IsInstanceOfType (type, reader, "CV02");
		Assert.IsNotNull (reader, "CV02");
	}
}
}