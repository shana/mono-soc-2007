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
		DocumentTag tag;
		
		tag = new DocumentTag ("Type");
		tag.IsElement = true;
		tag.Foreground = "blue";
		Add (tag);
		
		tag = new DocumentTag ("Type:Attributes");
		tag.Foreground = "green";
		Add (tag);
		
		tag = new DocumentTag ("Type:Name");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("Type:FullName");
		tag.IsAttribute = true;
		tag.Scale = Pango.Scale.XXLarge;
		tag.PixelsBelowLines = 10;
		Add (tag);
		
		tag = new DocumentTag ("Type:FullNameSP");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("Type:Maintainer");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("TypeSignature");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("TypeSignature:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("TypeSignature:Language");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("TypeSignature:Value");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("TypeSignature:Maintainer");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("AssemblyInfo");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("AssemblyName");
		tag.IsElement = true;
		tag.HasText = true;
		Add (tag);
		
		tag = new DocumentTag ("AssemblyName:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("AssemblyPublicKey");
		tag.IsElement = true;
		tag.HasText = true;
		Add (tag);
		
		tag = new DocumentTag ("AssemblyPublicKey:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("AssemblyVersion");
		tag.IsElement = true;
		tag.HasText = true;
		Add (tag);
		
		tag = new DocumentTag ("AssemblyVersion:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("ThreadSafetyStatement");
		tag.IsElement = true;
		tag.HasText = true;
		Add (tag);
		
		tag = new DocumentTag ("ThreadSafetyStatement:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("Base");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("BaseTypeName");
		tag.IsElement = true;
		tag.HasText = true;
		Add (tag);
		
		tag = new DocumentTag ("BaseTypeName:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("Attributes");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("Attribute");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("AttributeName");
		tag.IsElement = true;
		tag.HasText = true;
		Add (tag);
		
		tag = new DocumentTag ("AttributeName:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("Members");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("Member");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("Member:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("Member:MemberName");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("Member:Deprecated");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("MemberSignature");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("MemberSignature:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("MemberSignature:Language");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("MemberSignature:Value");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("MemberType");
		tag.IsElement = true;
		tag.HasText = true;
		Add (tag);
		
		tag = new DocumentTag ("MemberType:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("ReturnValue");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("ReturnType");
		tag.IsElement = true;
		tag.HasText = true;
		Add (tag);
		
		tag = new DocumentTag ("ReturnType:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("Parameters");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("Docs");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("summary");
		tag.IsElement = true;
		tag.HasText = true;
		Add (tag);
		
		tag = new DocumentTag ("summary:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("remarks");
		tag.IsElement = true;
		tag.HasText = true;
		Add (tag);
		
		tag = new DocumentTag ("remarks:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("para");
		tag.IsElement = true;
		tag.HasText = true;
		Add (tag);
		
		tag = new DocumentTag ("para:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("see");
		tag.IsElement = true;
		tag.Foreground = "red";
		Add (tag);
		
		tag = new DocumentTag ("see:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("see:cref");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("see:langword");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("see:qualify");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("link");
		tag.IsElement = true;
		tag.HasText = true;
		tag.Underline = Pango.Underline.Single;
		tag.Foreground = "#204a87";
		Add (tag);
		
		tag = new DocumentTag ("link:Attributes");
		tag.Foreground = "purple";
		tag.Underline = Pango.Underline.None;
		Add (tag);
		
		tag = new DocumentTag ("link:location");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("link:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("since");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("since:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("since:version");
		tag.IsAttribute = true;
		tag.Foreground = "orange";
		Add (tag);
		
		tag = new DocumentTag ("ignore");
		tag.IsSerializable = false;
		tag.IsText = true;
		Add (tag);
	}
}
}