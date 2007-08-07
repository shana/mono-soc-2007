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
	public void GetAssociatedTextTag ()
	{
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		TextTag summaryTag = buffer.TagTable.Lookup ("summary");
		Assert.IsNotNull (summaryTag);
		DocumentTag actualTag = (DocumentTag) DocumentUtils.GetAssociatedTextTag (buffer, summaryTag);
		Assert.AreEqual ("summary:Text", actualTag.Name);
		Assert.IsTrue (actualTag.IsText);
	}
	
	[Test()]
	public void AddPaddingIntOffset ()
	{
		int initialOffset = 0;
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		int nextOffset = DocumentUtils.AddPadding (buffer, initialOffset, "#0");
		Assert.AreEqual (1, nextOffset, "APIO");
	}
	
	[Test()]
	public void AddPaddingIntValidRegion ()
	{
		int initialOffset, endOffset, nextOffset;
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		initialOffset = 0;
		nextOffset = DocumentUtils.AddPadding (buffer, initialOffset, "#0");
		endOffset = nextOffset - 1;
		
		TextTag expectedTag = buffer.TagTable.Lookup ("padding#0");
		bool beginsPadding = buffer.GetIterAtOffset (initialOffset).BeginsTag (expectedTag);
		bool endsPadding = DocumentUtils.TagEndsHere  (expectedTag, buffer.GetIterAtOffset (endOffset), buffer.GetIterAtOffset (nextOffset));
		Assert.IsTrue (beginsPadding, "APIVR01");
		Assert.IsTrue (endsPadding, "APIVR02");
	}
	
	[Test()]
	public void AddPaddingVoidOffset ()
	{
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		TextIter insertIter = buffer.StartIter;
		DocumentUtils.AddPadding (buffer, ref insertIter, "#0");
		
		Assert.AreEqual (1, insertIter.Offset, "APVO");
	}
	
	[Test()]
	public void AddPaddingVoidValidRegion ()
	{
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		TextIter insertIter = buffer.StartIter;
		DocumentUtils.AddPadding (buffer, ref insertIter, "#0");
		
		TextTag expectedTag = buffer.TagTable.Lookup ("padding#0");
		bool beginsPadding = buffer.StartIter.BeginsTag (expectedTag);
		bool endsPadding = DocumentUtils.TagEndsHere  (expectedTag, buffer.GetIterAtOffset (insertIter.Offset - 1), insertIter);
		Assert.IsTrue (beginsPadding, "APVR01");
		Assert.IsTrue (endsPadding, "APVR02");
	}
	
	[Test()]
	public void AddPaddingEmptyIntOffset ()
	{
		int initialOffset = 0;
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		int nextOffset = DocumentUtils.AddPaddingEmpty (buffer, initialOffset, "#0");
		Assert.AreEqual (1, nextOffset, "APEIO");
	}
	
	[Test()]
	public void AddPaddingEmptyIntValidRegion ()
	{
		int initialOffset, endOffset, nextOffset;
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		initialOffset = 0;
		nextOffset = DocumentUtils.AddPaddingEmpty (buffer, initialOffset, "#0");
		endOffset = nextOffset - 1;
		
		TextTag expectedTag = buffer.TagTable.Lookup ("padding-empty#0");
		bool beginsPadding = buffer.GetIterAtOffset (initialOffset).BeginsTag (expectedTag);
		bool endsPadding = DocumentUtils.TagEndsHere  (expectedTag, buffer.GetIterAtOffset (endOffset), buffer.GetIterAtOffset (nextOffset));
		Assert.IsTrue (beginsPadding, "APEIVR01");
		Assert.IsTrue (endsPadding, "APEIVR02");
	}
	
	[Test()]
	public void AddPaddingEmptyVoidOffset ()
	{
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		TextIter insertIter = buffer.StartIter;
		DocumentUtils.AddPaddingEmpty (buffer, ref insertIter, "#0");
		
		Assert.AreEqual (1, insertIter.Offset, "APEVO");
	}
	
	[Test()]
	public void AddPaddingEmptyVoidValidRegion ()
	{
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		TextIter insertIter = buffer.StartIter;
		DocumentUtils.AddPaddingEmpty (buffer, ref insertIter, "#0");
		
		TextTag expectedTag = buffer.TagTable.Lookup ("padding-empty#0");
		bool beginsPadding = buffer.StartIter.BeginsTag (expectedTag);
		bool endsPadding = DocumentUtils.TagEndsHere  (expectedTag, buffer.GetIterAtOffset (insertIter.Offset - 1), insertIter);
		Assert.IsTrue (beginsPadding, "APEVR01");
		Assert.IsTrue (endsPadding, "APEVR02");
	}
	
//	[Test()]
//	public void AddText ()
//	{
//		int initialOffset, endOffset;
//		DocumentEditor editor = new DocumentEditor ();
//		TextBuffer buffer = editor.Buffer;
//		DocumentTagTable tagTable = (DocumentTagTable) buffer.TagTable;
//		TextTag textTag = tagTable.Lookup ("summary:Text");
//		
//		initialOffset = 0;
//		TextIter insertIter = buffer.StartIter;
//		DocumentUtils.AddText (buffer, ref insertIter, "\n    Test Region\n    ", "#0", textTag);
//		endOffset = insertIter.Offset - 1;
//		
//		TextTag expectedTag = buffer.TagTable.Lookup ("significant-whitespace#0#0");
//		bool beginsSpace = buffer.GetIterAtOffset (initialOffset).BeginsTag (expectedTag);
//		bool endsSpace = DocumentUtils.TagEndsHere  (expectedTag, buffer.GetIterAtOffset (4), buffer.GetIterAtOffset (5));
//		Assert.IsTrue (beginsSpace, "AT01");
//		Assert.IsTrue (endsSpace, "AT02");
//		
//		bool beginsText = buffer.GetIterAtOffset (5).BeginsTag (textTag);
//		bool endsText = DocumentUtils.TagEndsHere  (textTag, buffer.GetIterAtOffset (15), buffer.GetIterAtOffset (16));
//		Assert.IsTrue (beginsText, "AT03");
//		Assert.IsTrue (endsText, "AT04");
//		
//		beginsSpace = buffer.GetIterAtOffset (16).BeginsTag (expectedTag);
//		endsSpace = DocumentUtils.TagEndsHere  (expectedTag, buffer.GetIterAtOffset (endOffset), buffer.GetIterAtOffset (insertIter.Offset));
//		Assert.IsTrue (beginsSpace, "AT05");
//		Assert.IsTrue (endsSpace, "AT06");
//	}
	
	[Test()]
	public void AddStringIntOffset ()
	{
		int initialOffset, nextOffset;
		string data = "Inserting format Region";
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		initialOffset = 0;
		
		nextOffset = DocumentUtils.AddString (buffer, initialOffset, data, "#0");
		Assert.AreEqual (data.Length + 1, nextOffset, "ASIO");
	}
	
	[Test ()]
	public void AddStringIntValidRegion ()
	{
		int initialOffset, endOffset, nextOffset;
		string data = "Inserting format Region";
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		initialOffset = 0;
		nextOffset = DocumentUtils.AddString (buffer, initialOffset, data, "#0");
		endOffset = nextOffset - 2;
		
		TextTag expectedTag = buffer.TagTable.Lookup ("format#0");
		bool beginsFormat = buffer.GetIterAtOffset (initialOffset).BeginsTag (expectedTag);
		bool endsFormat = DocumentUtils.TagEndsHere  (expectedTag, buffer.GetIterAtOffset (endOffset), buffer.GetIterAtOffset (nextOffset - 1));
		Assert.IsTrue (beginsFormat, "ASIVR01");
		Assert.IsTrue (endsFormat, "ASIVR02");
	}
	
	[Test()]
	public void AddStringVoidOffset ()
	{
		string data = "Inserting format Region";
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		TextIter insertIter = buffer.StartIter;
		DocumentUtils.AddString (buffer, ref insertIter, data, "#0");
		
		Assert.AreEqual (data.Length + 1, insertIter.Offset, "ASVO");
	}
	
	[Test()]
	public void AddStringVoidValidRegion ()
	{
		string data = "Inserting format Region";
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		TextIter insertIter = buffer.StartIter;
		DocumentUtils.AddString (buffer, ref insertIter, data, "#0");
		
		TextTag expectedTag = buffer.TagTable.Lookup ("format#0");
		bool beginsFormat = buffer.StartIter.BeginsTag (expectedTag);
		bool endsFormat = DocumentUtils.TagEndsHere  (expectedTag, buffer.GetIterAtOffset (insertIter.Offset - 2), buffer.GetIterAtOffset (insertIter.Offset -1 ));
		Assert.IsTrue (beginsFormat, "ASVR01");
		Assert.IsTrue (endsFormat, "ASVR02");
	}
	
	[Test()]
	public void AddNewLineInt ()
	{
		int initialOffset, endOffset, nextOffset;
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		initialOffset = 0;
		nextOffset = DocumentUtils.AddNewLine (buffer, initialOffset, "#0");
		endOffset = nextOffset - 1;
		
		TextTag expectedTag = buffer.TagTable.Lookup ("newline#0");
		bool beginsNewLine = buffer.GetIterAtOffset (initialOffset).BeginsTag (expectedTag);
		bool endsNewLine = DocumentUtils.TagEndsHere  (expectedTag, buffer.GetIterAtOffset (endOffset), buffer.GetIterAtOffset (nextOffset));
		Assert.IsTrue (beginsNewLine, "ANLI01");
		Assert.IsTrue (endsNewLine, "ANLI02");
	}
	
	[Test()]
	public void AddNewLineVoid ()
	{
		int initialOffset, endOffset;
		DocumentEditor editor = new DocumentEditor ();
		TextBuffer buffer = editor.Buffer;
		
		initialOffset = 0;
		TextIter insertIter = buffer.StartIter;
		DocumentUtils.AddNewLine (buffer, ref insertIter, "#0");
		endOffset = insertIter.Offset - 1;
		
		TextTag expectedTag = buffer.TagTable.Lookup ("newline#0");
		bool beginsNewLine = buffer.GetIterAtOffset (initialOffset).BeginsTag (expectedTag);
		bool endsNewLine = DocumentUtils.TagEndsHere  (expectedTag, buffer.GetIterAtOffset (endOffset), insertIter);
		Assert.IsTrue (beginsNewLine, "ANLV01");
		Assert.IsTrue (endsNewLine, "ANLV02");
	}
}
}
