//
// TestTextRange.cs: Test unit for TextRange helper class.
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
public class TestTextRange {

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
	public void TestText ()
	{
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		TextTag summaryTag = buffer.TagTable.Lookup ("summary");
		TextTag textTag = buffer.TagTable.Lookup ("summary:Text");
		
		TextIter insertAt = buffer.StartIter;
		buffer.InsertWithTags (ref insertAt, "Test: ", summaryTag);
		
		int startIndex = insertAt.Offset;
		buffer.InsertWithTags (ref insertAt, "Testing TextRange", textTag);

		TextRange textRange = new TextRange (buffer.GetIterAtOffset (startIndex), insertAt);
		Assert.AreEqual ("Testing TextRange", textRange.Text, "TRT");
	}
	
	[Test()]
	public void TestErase ()
	{
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		TextTag summaryTag = buffer.TagTable.Lookup ("summary");
		TextTag textTag = buffer.TagTable.Lookup ("summary:Text");
		
		TextIter insertAt = buffer.StartIter;
		buffer.InsertWithTags (ref insertAt, "Test: ", summaryTag);
		
		int startIndex = insertAt.Offset;
		buffer.InsertWithTags (ref insertAt, "Testing TextRange", textTag);

		TextRange textRange = new TextRange (buffer.GetIterAtOffset (startIndex), insertAt);
		Assert.AreEqual ("Test: Testing TextRange", buffer.Text, "TRE01");
		textRange.Erase ();
		Assert.AreEqual ("Test: ", buffer.Text, "TRE", "TRE02");
	}
	
	[Test()]
	public void TestRemoveTag ()
	{
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		TextTag summaryTag = buffer.TagTable.Lookup ("summary");
		TextTag textTag = buffer.TagTable.Lookup ("summary:Text");
		
		TextIter insertAt = buffer.StartIter;
		buffer.InsertWithTags (ref insertAt, "Test: ", summaryTag);
		
		int startIndex = insertAt.Offset;
		buffer.InsertWithTags (ref insertAt, "Testing TextRange", textTag);

		TextRange textRange = new TextRange (buffer.GetIterAtOffset (startIndex), insertAt);
		Assert.AreEqual (1, textRange.Start.Tags.Length, "TRRT01");
		textRange.RemoveTag (textTag);
		Assert.AreEqual (0, textRange.Start.Tags.Length, "TRRT02");
	}
}
}
