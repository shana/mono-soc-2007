using System;

using Mono.Cecil;
using Gendarme.Framework;

namespace Gendarme.Rules.Naming {
	public class AttributeEndsWithAttributeSuffixRule : ITypeRule {
		private bool InheritsFromAttribute (TypeDefinition typeDefinition)
		{
			TypeDefinition current =  typeDefinition;
			while (current.FullName != "System.Object") {
				if (current.BaseType.FullName == "System.Attribute")
					return true;
				current = (TypeDefinition)current.BaseType;
			}
			return false;
		}
		
		public MessageCollection CheckType (TypeDefinition typeDefinition, Runner runner)
		{
			MessageCollection messageCollection = new MessageCollection ();
			if (InheritsFromAttribute (typeDefinition)) {
				if (!typeDefinition.Name.EndsWith ("Attribute")) {
					Location location = new Location (typeDefinition.FullName, typeDefinition.Name, 0);
					Message message = new Message ("The class name doesn't end with Attribute Suffix", location, MessageType.Warning);
					messageCollection.Add (message);                        
				}
			}
			if (messageCollection.Count == 0)
				return null;
			return messageCollection;
		}
	}
}