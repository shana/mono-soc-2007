//
// DocumentTag.cs: TextTag based class that represent the tags applied to a buffer of Monodoc documentation..
//
// Author:
//   Hector E. Gomez M (hectorgm@ciencias.unam.mx)
//
// (C) 2007 Hector E. Gomez M
//

using System;
using Gtk;

namespace Monodoc.Editor.Gui {
public class DocumentTag : TextTag {
	[Flags]
	enum TagFlags {
		IsSerializable = 0x0001,
		IsElement = 0x0002,
		IsAttribute = 0x0004,
		IsText = 0x0008,
		IsDynamic = 0x0010,
		IsEditable = 0x0020,
		IsSerializableText = (IsSerializable | IsText)
	};
	
	TagFlags flags;
	
	public DocumentTag (string tagName) : base (tagName)
	{
		Initialize ();
	}
	
	public void Initialize () 
	{
		this.Editable = false;
		flags = TagFlags.IsSerializable;
	}
	
	public bool IsSerializable {
		get {
			return (flags & TagFlags.IsSerializable) == TagFlags.IsSerializable;
		}
		
		set {
			if (value)
				flags |= TagFlags.IsSerializable;
			else
				flags &= ~TagFlags.IsSerializable;
		}
	}
	
	public bool IsAttribute {
		get {
			return (flags & TagFlags.IsAttribute) == TagFlags.IsAttribute;
		}
		
		set {
			if (value)
				flags |= TagFlags.IsAttribute;
			else
				flags &= ~TagFlags.IsAttribute;
		}
	}
	
	public bool IsElement {
		get {
			return (flags & TagFlags.IsElement) == TagFlags.IsElement;
		}
		
		set {
			if (value)
				flags |= TagFlags.IsElement;
			else
				flags &= ~TagFlags.IsElement;
		}
	}
	
	public bool IsText {
		get {
			return (flags & TagFlags.IsText) == TagFlags.IsText;
		}
		
		set {
			if (value)
				flags |= TagFlags.IsText;
			else
				flags &= ~TagFlags.IsText;
		}
	}
	
	public bool IsDynamic {
		get {
			return (flags & TagFlags.IsDynamic) == TagFlags.IsDynamic;
		}
		
		set {
			if (value)
				flags |= TagFlags.IsDynamic;
			else
				flags &= ~TagFlags.IsDynamic;
		}
	}
	
	public bool IsSerializableText {
		get {
			return (flags & (TagFlags.IsSerializableText)) == TagFlags.IsSerializableText;
		}
		
		set {
			if (value)
				flags |= TagFlags.IsSerializableText;
			else
				flags &= ~TagFlags.IsSerializableText;
		}
	}
	
	public bool IsEditable {
		get {
			return (flags & (TagFlags.IsEditable)) == TagFlags.IsEditable;
		}
		
		set {
			if (value)
				flags |= TagFlags.IsEditable;
			else
				flags &= ~TagFlags.IsEditable;
		}
	}
}
}