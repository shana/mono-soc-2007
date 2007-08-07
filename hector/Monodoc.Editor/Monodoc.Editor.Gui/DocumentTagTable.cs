//
// DocumentTagTable.cs: TextTagTable based class that represent the table of tags for a buffer.
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using System;
using System.Collections;
using Gtk;
using Pango;

namespace Monodoc.Editor.Gui {
public class DocumentTagTable : TextTagTable {
	private static Hashtable dynamic_tags;
	
	public DocumentTagTable () : base ()
	{
		InitNormalTags ();
		InitDynamicTags ();
	}
	
	public static bool IsDynamic (string name) 
	{
		InitDynamicTags ();
		return dynamic_tags [name] != null;
	}
	
	public DocumentTag CreateDynamicTag (string fullTagName)
	{
		string tagName = fullTagName.Split (':', '#') [0];
		if (!IsDynamic (tagName))
			throw new ArgumentException ("Error -> The tag \"" + tagName + "\" is not a Dynamic Tag");
		
		DocumentTag tag;
		tag = new DocumentTag (fullTagName);
		tag.IsDynamic = true;
		InitializeDynamicTag (tag);
		Add (tag);
		
		return tag;
	}
	
	private void InitNormalTags ()
	{
		DocumentTag tag;
		
		tag = new DocumentTag ("Type");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("Type:Attributes");
		tag.Foreground = "#204a87";
		Add (tag);
		
		// Type Element Attributes: Name and FullName are required.
		tag = new DocumentTag ("Type:Name");
		tag.IsAttribute = true;
		tag.Invisible = true;
		Add (tag);
		
		tag = new DocumentTag ("Type:FullName");
		tag.IsAttribute = true;
		tag.Scale = Pango.Scale.XXLarge;
		Add (tag);
		
		tag = new DocumentTag ("Type:FullNameSP");
		tag.IsAttribute = true;
		tag.Invisible = true;
		Add (tag);
		
		tag = new DocumentTag ("Type:Maintainer");
		tag.IsAttribute = true;
		tag.Invisible = true;
		Add (tag);
		
		tag = new DocumentTag ("TypeSignature");
		tag.IsElement = true;
		tag.Scale = Pango.Scale.Large;
		Add (tag);
		
		// TypeSignature Element Attributes: Language and Value are required.
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
		tag.Scale = Pango.Scale.Medium;
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
		tag.Invisible = true;
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
		tag.IsEditable = true;
		Add (tag);
		
		tag = new DocumentTag ("summary:Text");
		tag.IsText = true;
		tag.Editable = true;
		tag.Background =  "#A5C0E6";
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
		
		tag = new DocumentTag ("exception:cref");
		tag.IsAttribute = true;
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
		tag.IsEditable = true;
		Add (tag);
		
		tag = new DocumentTag ("remarks:Text");
		tag.IsText = true;
		tag.Editable = true;
		tag.Background =  "#A5C0E6";
		Add (tag);
		
		tag = new DocumentTag ("value");
		tag.IsElement = true;
		Add (tag);
		
		tag = new DocumentTag ("value:Text");
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
	}
	
	private static void InitDynamicTags ()
	{
		if (dynamic_tags == null)
			dynamic_tags = new Hashtable ();
		
		// Schema defined elements.
		dynamic_tags ["block"] = true;
		dynamic_tags ["code"] = true;
		dynamic_tags ["c"] = true;
		dynamic_tags ["description"] = true;
		dynamic_tags ["example"] = true;
		dynamic_tags ["item"] = true;
		dynamic_tags ["li"] = true;
		dynamic_tags ["link"] = true;
		dynamic_tags ["list"] = true;
		dynamic_tags ["listheader"] = true;
		dynamic_tags ["onequarter"] = true;
		dynamic_tags ["para"] = true;
		dynamic_tags ["paramref"] = true;
		dynamic_tags ["see"] = true;
		dynamic_tags ["SPAN"] = true;
		dynamic_tags ["sub"] = true;
		dynamic_tags ["subscript"] = true;
		dynamic_tags ["sup"] = true;
		dynamic_tags ["term"] = true;
		dynamic_tags ["typeparamref"] = true;
		dynamic_tags ["ul"] = true;
		
		// Editor defined tags to format the documentation
		dynamic_tags ["padding"] = true;
		dynamic_tags ["padding-empty"] = true;
		dynamic_tags ["newline"] = true;
		dynamic_tags ["format"] = true;
		dynamic_tags ["format-end"] = true;
		dynamic_tags ["significant-whitespace"] = true;
		dynamic_tags ["stub"] = true;
	}
	
	private void InitializeDynamicTag (DocumentTag tag)
	{
		string tagName = tag.Name.Split ('#')[0];
		switch (tagName) {
		// Schema defined elements.
		case "block":
			tag.IsElement = true;
			break;
		case "block:Attributes":
			break;
		case "block:subset":
			tag.IsAttribute = true;
			break;
		case "block:type":
			tag.IsAttribute = true;
			break;
		case "block:Text":
			tag.IsText = true;
			break;
		case "c":
			tag.IsElement = true;
			break;
		case "c:Text":
			tag.IsText = true;	
			break;
		case "code":
			tag.IsElement = true;
			break;
		case "code:Attributes":
			break;
		case "code:lang":
			tag.IsAttribute = true;
			break;
		case "code:language":
			tag.IsAttribute = true;
		break;
		case "code:source":
			tag.IsAttribute = true;
			break;
		case "code:Text":
			tag.IsText = true;
			break;
		case "description":
			tag.IsElement = true;
			break;
		case "description:Text":
			tag.IsText = true;
			break;
		case "example":
			tag.IsElement = true;
			break;
		case "example:Text":
			tag.IsText = true;
			break;
		case "item":
			tag.IsElement = true;
			break;
		case "li":	
			tag.IsElement = true;
			break;
		case "li:Text":
			tag.IsText = true;
			break;
		case "link":
			tag.IsElement = true;
			tag.Underline = Pango.Underline.Single;
			tag.Foreground = "#204a87";
			break;
		case "link:Attributes":
			tag.Foreground = "purple";
			tag.Underline = Pango.Underline.None;
			break;
		case "link:location":
			tag.IsAttribute = true;
			tag.Invisible = true;
			break;
		case "link:Text":
			tag.IsText = true;
			break;
		case "list":
			tag.IsElement = true;
			break;
		case "list:Attributes":
			break;
		case "list:type":
			tag.IsAttribute = true;
			break;
		case "listheader":
			tag.IsElement = true;
			break;
		case "onequarter":
			tag.IsElement = true;
			break;
		case "para":
			tag.IsElement = true;
			break;
		case "para:Text":
			tag.IsText = true;
			tag.Editable = true;
			tag.Background =  "#A5C0E6";
			break;
		case "paramref":
			tag.IsElement = true;
			break;
		case "paramref:Attributes":
			break;
		case "paramref:name":
			tag.IsAttribute = true;	
			break;
		case "see":
			tag.IsElement = true;
			break;
		case "see:Attributes":
			break;
		case "see:cref":
			tag.IsAttribute = true;
			tag.Foreground = "red";
			break;
		case "see:langword":
			tag.IsAttribute = true;
			tag.Foreground = "yellow";
			break;
		case "see:qualify":
			tag.IsAttribute = true;
			break;
		case "SPAN":
			tag.IsElement = true;
			break;
		case "SPAN:Attributes":
			break;
		case "SPAN:version":
			tag.IsAttribute = true;
			break;
		case "sub":
			tag.IsElement = true;
			break;
		case "sub:Text":
			tag.IsText = true;
			break;
		case "subscript":
			tag.IsElement = true;
			break;
		case "subscript:Attributes":
			break;
		case "subscript:term":
			tag.IsAttribute = true;
			break;
		case "sup":
			tag.IsElement = true;
			break;
		case "sup:Text":
			tag.IsText = true;
			break;
		case "term":
			tag.IsElement = true;
			break;
		case "term:Text":
			tag.IsText = true;
			break;
		case "typeparamref":
			tag.IsElement = true;
			break;
		case "typeparamref:Attributes":
			break;
		case "typeparamref:name":
			tag.IsAttribute = true;
			break;
		case "ul":
			tag.IsElement = true;
			break;
		// Editor defined tags.
		case "padding":
			tag.IsText = true;
			tag.IsSerializable = false;
			tag.Invisible = true;
			break;
		case "padding-empty":
			tag.IsText = true;
			tag.IsSerializable = false;
			tag.Invisible = true;
			break;
		case "newline":
			tag.IsText = true;
			tag.IsSerializable = false;
			tag.Invisible = false;
			break;
		case "format":
			tag.IsText = true;
			tag.IsSerializable = false;
			tag.Invisible = false;
			break;
		case "format-end":
			tag.IsText = true;
			tag.IsSerializable = false;
			tag.Invisible = true;
			break;
		case "significant-whitespace":
			tag.IsText = true;
			tag.Invisible = false;
			tag.Invisible = true;
			break;
		case "stub":
			tag.IsText = true;
			tag.IsSerializable = false;
			tag.Invisible = false;
			tag.Editable = true;
			tag.Background =  "#A5C0E6";
			tag.Weight = Pango.Weight.Bold;
			tag.Style = Pango.Style.Italic;
			break;
		default:
			break;
		}
	}
}
}