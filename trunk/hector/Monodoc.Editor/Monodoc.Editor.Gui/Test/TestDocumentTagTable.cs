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
		DocumentTagTable tagTable = new DocumentTagTable ();
		Assert.AreEqual (115, tagTable.Size, "TTS");
	}
	
	[Test()]
	public void TestIsDynamicValid ()
	{
		Assert.IsTrue (DocumentTagTable.IsDynamic ("para"), "TIDV");
	}
	
	[Test()]
	public void TestIsDynamicInvalid ()
	{
		Assert.IsFalse (DocumentTagTable.IsDynamic ("test"), "TIDI");
	}
	
	[Test()]
	public void TestCreateDynamicTag ()
	{
		DocumentTagTable tagTable = new DocumentTagTable ();
		
		TextTag expectedTag = tagTable.Lookup ("format#0");
		Assert.IsNull (expectedTag, "CDT01");
		
		TextTag actualTag = tagTable.CreateDynamicTag ("format#0");
		expectedTag = tagTable.Lookup ("format#0");
		Assert.AreEqual (expectedTag, actualTag, "CDT02");
	}
	
	[Test()]
	public void TestCreateDynamicTagInvalid ()
	{
		DocumentTagTable tagTable = new DocumentTagTable ();
		TextTag tag = null;
		
		try {
			tag = tagTable.CreateDynamicTag ("foo-tag");
		} catch (ArgumentException exception) {
			Assert.AreEqual ("Error -> The tag \"foo-tag\" is not a Dynamic Tag", exception.Message, "TCDTI01");
		}
		
		Assert.IsNull (tag);
	}
}
}
