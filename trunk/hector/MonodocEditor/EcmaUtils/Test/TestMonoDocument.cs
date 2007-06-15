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

namespace Monodoc.EcmaUtils {
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
		type = Type.GetType ("Monodoc.EcmaUtils.MonoDocument");
	}
	
	[Test()]
	public void ConstructorInvalid ()
	{
	}
}
}
