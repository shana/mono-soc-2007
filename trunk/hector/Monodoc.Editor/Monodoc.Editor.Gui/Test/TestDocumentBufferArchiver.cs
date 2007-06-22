//
// TestDocumentBufferArchiver.cs: Test unit for DocumentBufferArchiver class.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M

using Gtk;
using System;
using System.IO;
using NUnit.Framework;
using Monodoc.Editor.Utils;

namespace Monodoc.Editor.Gui {
[TestFixture()]
public class TestDocumentBufferArchiver {
	
	private string pathTest;
	private TextBuffer buffer;
	
	[SetUp()]
	public void Initialize ()
	{
		string filePath;
		
		filePath = Test.PathOfTestFiles ();
		pathTest = Path.Combine (filePath, "Examples");
		Application.Init ();
		DocumentEditor editor = new DocumentEditor ();
		buffer = editor.Buffer;
	}
	
	[TearDown()]
	public void Dispose ()
	{
		Application.Quit ();
	}
	
	[Test()]
	public void Serialize()
	{
		string originalText, newText, fileName;
		
		fileName = Path.Combine (pathTest, "WrapMode.xml");
		MonoDocument document = new MonoDocument (fileName);
		originalText = document.Text;
		
		DocumentBufferArchiver.Deserialize (buffer, originalText);
		newText = DocumentBufferArchiver.Serialize (buffer);
		
		Assert.AreEqual (originalText, newText, "SR01");
	}
	
}
}