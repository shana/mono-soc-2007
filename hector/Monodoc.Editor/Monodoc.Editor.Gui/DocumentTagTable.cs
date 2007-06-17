
//
// DocumentTagTable.cs: TextTagTable based class that represent the table of tags for a buffer.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using System;
using Gtk;

namespace Monodoc.Editor.Gui {
public class DocumentTagTable : TextTagTable {
	
	private static DocumentTagTable instance;
	
	public static DocumentTagTable Instance {
		get {
			if (instance == null)
				instance = new DocumentTagTable ();
			return instance;
		}
	}
	
	public DocumentTagTable () : base ()
	{
		InitCommonTags ();
	}
	
	private void InitCommonTags ()
	{
		TextTag tag;
		
		tag = new TextTag ("Type");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("Type:Attributes");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("TypeSignature");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("TypeSignature:Attributes");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("AssemblyInfo");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("AssemblyName");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("AssemblyPublicKey");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("AssemblyVersion");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("ThreadSafetyStatement");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("Base");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("BaseTypeName");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("Attributes");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("Attribute");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("AttributeName");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("Members");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("Member");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("Member:Attributes");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("MemberSignature");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("MemberSignature:Attributes");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("MemberType");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("ReturnValue");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("ReturnType");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("Parameters");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("Docs");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("summary");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("remarks");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("para");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("see");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("link");
		tag.Editable = false;
		Add (tag);
		
		tag = new TextTag ("since");
		tag.Editable = false;
		Add (tag);
	}
}
}