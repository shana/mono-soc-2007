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
		
		tag = new DocumentTag ("MemberOfLibrary");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("MemberOfLibrary:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("AssemblyInfo");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("AssemblyName");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("AssemblyName:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("AssemblyPublicKey");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("AssemblyPublicKey:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("AssemblyVersion");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("AssemblyVersion:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("AssemblyCulture");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("AssemblyCulture:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("TypeParameters");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("TypeParameter");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("TypeParameter:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("ThreadingSafetyStatement");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("ThreadingSafetyStatement:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("ThreadSafetyStatement");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("ThreadSafetyStatement:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("Base");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("BaseTypeName");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("BaseTypeName:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("BaseTypeArguments");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("BaseTypeArgument");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("BaseTypeArgument:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("BaseTypeArgument:TypeParamName");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("BaseTypeArgument:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("ExcludedBaseTypeName");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("ExcludedBaseTypeName:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("Interfaces");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("Interface");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("InterfaceName");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("InterfaceName:Text");
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
		Add (tag);
		
		tag = new DocumentTag ("MemberType:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("ReturnValue");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("ReturnType");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("ReturnType:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("MemberValue");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("MemberValue:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("ExcludedLibrary");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("ExcludedLibrary:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("Parameters");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("Parameter");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("Parameter:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("Parameter:Name");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("Parameter:Type");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("Parameter:RefType");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("TypeExcluded");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("TypeExcluded:Text");
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
		Add (tag);
		
		tag = new DocumentTag ("AttributeName:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("Excluded");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("Excluded:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("ExcludedTypeName");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("ExcludedTypeName:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("ExcludedLibraryName");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("ExcludedLibraryName:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("Docs");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("summary");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("summary:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("param");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("param:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("param:name");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("param:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("exception");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("exception:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("exception:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("returns");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("returns:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("remarks");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("remarks:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("value");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("value:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("example");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("example:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("permission");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("permission:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("permission:cref");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("permission:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("altmember");
		tag.IsElement = true;
		Add(tag);
		
		tag = new DocumentTag ("altmember:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("altmember:cref");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("altmember:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("altcompliant");
		tag.IsElement = true;
		Add(tag);
		
		tag = new DocumentTag ("altcompliant:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("altcompliant:cref");
		tag.IsAttribute = true;
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
		
		tag = new DocumentTag ("threadsafe");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("typeparam");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("typeparam:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("typeparam:name");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("typeparam:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("para");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("para:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("block");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("block:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("block:subset");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("block:type");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("block:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("list");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("list:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("list:type");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("listheader");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("item");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("term");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("term:Text");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("description");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("description:Text");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("ul");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("li");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("li:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("c");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("c:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("SPAN");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("SPAN:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("SPAN:version");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("code");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("code:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("code:lang");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("code:language");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("code:source");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("code:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("see");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("see:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("see:cref");
		tag.IsAttribute = true;
		tag.Foreground = "red";
		Add (tag);
		
		tag = new DocumentTag ("see:langword");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("see:qualify");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("paramref");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("paramref:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("paramref:name");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("typeparamref");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("typeparamref:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("typeparamref:name");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("link");
		tag.IsElement = true;
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
		
		tag = new DocumentTag ("onequarter");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("sub");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("sub:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("sup");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("sup:Text");
		tag.IsText = true;
		Add (tag);
		
		tag = new DocumentTag ("subscript");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("subscript:Attributes");
		Add (tag);
		
		tag = new DocumentTag ("subscript:term");
		tag.IsAttribute = true;
		Add (tag);
		
		tag = new DocumentTag ("ignore");
		tag.IsSerializable = false;
		tag.IsText = true;
		Add (tag);
	}
}
}