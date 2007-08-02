//
// TestDocumentTagTable.cs: Test unit for the DocumentTagTable class.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M

using Gtk;
using System;
using NUnit.Framework;

namespace Monodoc.Editor.Gui {
[TestFixture()]
public class TestDocumentTagTable {
	[SetUp()]
	public void Initialize ()
	{
		Application.Init ();
	}
	
	[TearDown()]
	public void Dispose ()
	{
		Application.Quit ();
	}
	
	[Test()]
	public void TestCreateDynamicTag ()
	{
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		TextTag actualTag = tagTable.CreateDynamicTag ("format#0");
		
		TextIter insertIter = buffer.StartIter;
		buffer.InsertWithTags (ref insertIter, "Example Region", actualTag);
		
		TextTag expectedTag = tagTable.Lookup ("format#0");
		Assert.AreEqual (expectedTag, actualTag, "CDT01");
	}
}
}
