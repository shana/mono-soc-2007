//
// TestUtils.cs: Test unit for Utils helpers classes.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M

using Gtk;
using System;
using NUnit.Framework;
using Monodoc.Editor.Gui;

namespace Monodoc.Editor.Gui {
[TestFixture()]
public class TestUtils {
	
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
	public void TagEndsHereSimple ()
	{
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		TextIter insertIter = buffer.StartIter;
		buffer.InsertWithTagsByName (ref insertIter, "Example Region", "Type");
		
		TextIter endIter = buffer.GetIterAtOffset (insertIter.Offset - 1);
		TextIter nextIter = buffer.GetIterAtOffset (insertIter.Offset);
		bool endsTag = DocumentUtils.TagEndsHere (buffer.TagTable.Lookup ("Type"), endIter, nextIter);
		Assert.IsTrue (endsTag, "TEH01");
	}
	
	[Test()]
	public void TagEndsHereOverlaping ()
	{
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		TextIter insertIter = buffer.StartIter;
		buffer.InsertWithTagsByName (ref insertIter, "Example Region", "Type");
		int firstEndOffset = insertIter.Offset;
		buffer.InsertWithTagsByName (ref insertIter, "Second Region", "Type");
		
		TextIter firstEndIter = buffer.GetIterAtOffset (firstEndOffset - 1);
		TextIter firstNextIter = buffer.GetIterAtOffset (firstEndOffset);
		TextIter secondEndIter = buffer.GetIterAtOffset (insertIter.Offset - 1);
		TextIter secondNextIter = buffer.GetIterAtOffset (insertIter.Offset);
		
		bool firstEndsTag = DocumentUtils.TagEndsHere (buffer.TagTable.Lookup ("Type"), firstEndIter, firstNextIter);
		bool secondEndsTag = DocumentUtils.TagEndsHere (buffer.TagTable.Lookup ("Type"), secondEndIter, secondNextIter);
		Assert.IsFalse (firstEndsTag, "TEH01");
		Assert.IsTrue (secondEndsTag, "TEH02");
	}
	
	[Test()]
	public void GetLastTag ()
	{
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		TextIter insertIter = buffer.StartIter;
		buffer.Insert (ref insertIter, "Start Extern Region ");
		buffer.InsertWithTagsByName (ref insertIter, "Intern Region", "Type:Attributes");
		int index = insertIter.Offset - 1;
		buffer.Insert (ref insertIter, "End Extern Region");
		buffer.ApplyTag ("Type", buffer.StartIter, insertIter);
		
		TextTag expectedTag = buffer.TagTable.Lookup ("Type:Attributes");
		TextTag actualTag = DocumentUtils.GetLastTag (buffer.GetIterAtOffset (index));
		Assert.AreEqual (expectedTag, actualTag, "GLT01");
	}
	
	[Test()]
	public void AddStringInt ()
	{
		int initialOffset, endOffset, nextOffset;
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		initialOffset = 0;
		nextOffset = DocumentUtils.AddString (buffer, initialOffset, "Inserting format Region", "#0");
		endOffset = nextOffset - 1;
		
		TextTag expectedTag = buffer.TagTable.Lookup ("format#0");
		bool beginsFormat = buffer.GetIterAtOffset (initialOffset).BeginsTag (expectedTag);
		bool endsFormat = DocumentUtils.TagEndsHere  (expectedTag, buffer.GetIterAtOffset (endOffset), buffer.GetIterAtOffset (nextOffset));
		Assert.IsTrue (beginsFormat, "ASI01");
		Assert.IsTrue (endsFormat, "ASI02");
	}
	
	[Test()]
	public void AddStringVoid ()
	{
		int initialOffset, endOffset;
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		initialOffset = 0;
		TextIter insertIter = buffer.StartIter;
		DocumentUtils.AddString (buffer, ref insertIter, "Inserting format Region", "#0");
		endOffset = insertIter.Offset - 1;
		
		TextTag expectedTag = buffer.TagTable.Lookup ("format#0");
		bool beginsFormat = buffer.GetIterAtOffset (initialOffset).BeginsTag (expectedTag);
		bool endsFormat = DocumentUtils.TagEndsHere  (expectedTag, buffer.GetIterAtOffset (endOffset), insertIter);
		Assert.IsTrue (beginsFormat, "ASI01");
		Assert.IsTrue (endsFormat, "ASI02");
	}
}
}
