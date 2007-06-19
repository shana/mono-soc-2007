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
using Pango;

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
		tag.Foreground = "blue";
		Add (tag);
		
		tag = new TextTag ("Type:Name");
		tag.Invisible = false;
		Add (tag);
		
		tag = new TextTag ("Type:FullName");
		tag.Scale = Pango.Scale.XXLarge;
		tag.PixelsBelowLines = 10;
		Add (tag);
		
		tag = new TextTag ("Type:FullNameSP");
		Add (tag);
		
		tag = new TextTag ("Type:Maintainer");
		Add (tag);
		
		tag = new TextTag ("TypeSignature");
		Add (tag);
		
		tag = new TextTag ("TypeSignature:Language");
		Add (tag);
		
		tag = new TextTag ("TypeSignature:Value");
		Add (tag);
		
		tag = new TextTag ("TypeSignature:Maintainer");
		Add (tag);
		
		tag = new TextTag ("AssemblyInfo");
		Add (tag);
		
		tag = new TextTag ("AssemblyName");
		Add (tag);
		
		tag = new TextTag ("AssemblyPublicKey");
		Add (tag);
		
		tag = new TextTag ("AssemblyVersion");
		Add (tag);
		
		tag = new TextTag ("ThreadSafetyStatement");
		Add (tag);
		
		tag = new TextTag ("Base");
		Add (tag);
		
		tag = new TextTag ("BaseTypeName");
		Add (tag);
		
		tag = new TextTag ("Attributes");
		Add (tag);
		
		tag = new TextTag ("Attribute");
		Add (tag);
		
		tag = new TextTag ("AttributeName");
		Add (tag);
		
		tag = new TextTag ("Members");
		Add (tag);
		
		tag = new TextTag ("Member");
		Add (tag);
		
		tag = new TextTag ("Member:MemberName");
		Add (tag);
		
		tag = new TextTag ("Member:Deprecated");
		Add (tag);
		
		tag = new TextTag ("Member:Attributes");
		Add (tag);
		
		tag = new TextTag ("MemberSignature");
		Add (tag);
		
		tag = new TextTag ("MemberSignature:Language");
		Add (tag);
		
		tag = new TextTag ("MemberSignature:Value");
		Add (tag);
		
		tag = new TextTag ("MemberType");
		Add (tag);
		
		tag = new TextTag ("ReturnValue");
		Add (tag);
		
		tag = new TextTag ("ReturnType");
		Add (tag);
		
		tag = new TextTag ("Parameters");
		Add (tag);
		
		tag = new TextTag ("Docs");
		Add (tag);
		
		tag = new TextTag ("summary");
		Add (tag);
		
		tag = new TextTag ("remarks");
		Add (tag);
		
		tag = new TextTag ("para");
		Add (tag);
		
		tag = new TextTag ("see");
		tag.Foreground = "red";
		Add (tag);
		
		tag = new TextTag ("see:cref");
		Add (tag);
		
		tag = new TextTag ("see:langword");
		Add (tag);
		
		tag = new TextTag ("see:qualify");
		Add (tag);
		
		tag = new TextTag ("link");
		tag.Underline = Pango.Underline.Single;
		tag.Foreground = "#204a87";
		Add (tag);
		
		tag = new TextTag ("link:location");
		Add (tag);
		
		tag = new TextTag ("since");
		Add (tag);
		
		tag = new TextTag ("since:version");
		tag.Foreground = "orange";
		Add (tag);
		
		tag = new TextTag ("ignore");
		Add (tag);
	}
}
}