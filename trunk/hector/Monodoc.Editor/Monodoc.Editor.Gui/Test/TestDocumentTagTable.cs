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
	public void TestTagTableSize ()
	{
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		
		Assert.AreEqual (115, tagTable.Size, "TTS01");
	}
	
	public void TestIsDynamicValid ()
	{
	}
	
	[Test()]
	public void TestCreateDynamicTag ()
	{
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
		
		TextTag expectedTag = tagTable.Lookup ("format#0");
		Assert.IsNull (expectedTag, "CDT01");
		TextTag actualTag = tagTable.CreateDynamicTag ("format#0");
		expectedTag = tagTable.Lookup ("format#0");
		
		TextIter insertIter = buffer.StartIter;
		buffer.InsertWithTags (ref insertIter, "Example Region", actualTag);
		
		Assert.AreEqual (expectedTag, actualTag, "CDT02");
	}
}
}
