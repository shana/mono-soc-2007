//
// System.ComponentModel.Design.Serialization.RootCodeDomSerializer
//
// Authors:
//	  Ivan N. Zlatev (contact i-nZ.net)
//
// (C) 2007 Ivan N. Zlatev

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#if NET_2_0

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.CodeDom;

namespace Mono.Design
{
	internal class RootCodeDomSerializer : CodeDomSerializer
	{

		internal class CodeMap
		{

			private CodeTypeDeclaration _class;
			private List<CodeMemberField> _fields;
			private CodeStatementCollection _preInit;
			private CodeStatementCollection _init;
			private CodeStatementCollection _postInit;

			public CodeMap (Type documentType, string documentName)
			{
				if (documentName == null)
					throw new ArgumentNullException ("documentName");
				if (documentType == null)
					throw new ArgumentNullException ("documentType");

				_class = new CodeTypeDeclaration (documentName);
				_class.BaseTypes.Add (documentType.BaseType);
			}

			public void AddField (CodeMemberField field)
			{
				if (_fields == null)
					_fields = new List<CodeMemberField> ();
				_fields.Add (field);
			}

			public void AddPreInitStatement (CodeStatement statement)
			{
				if (_preInit == null)
					_preInit = new CodeStatementCollection ();
				_preInit.Add (statement);
			}

			public void AddInitStatement (CodeStatement statement)
			{
				if (_init == null)
					_init = new CodeStatementCollection ();
				_init.Add (statement);
			}

			public void AddInitStatements (CodeStatementCollection statements)
			{
				if (_init == null)
					_init = new CodeStatementCollection ();
				_init.AddRange (statements);
			}

			public void AddPostInitStatement (CodeStatement statement)
			{
				if (_postInit == null)
					_postInit = new CodeStatementCollection ();
				_postInit.Add (statement);
			}

			/*
				class Type : BaseType
				{
					#region Windows Form Designer generated code

					private void InitializeComponent ()
					{
						preInit;
						init;
						postInit;
					}

					private field1;
					private field2;

					#endregion
				}
            */

			public CodeTypeDeclaration GenerateClass ()
			{
				CodeTypeDeclaration clas = _class;

				clas.StartDirectives.Add (new CodeRegionDirective (CodeRegionMode.Start, "Windows Form Designer generated code"));

				CodeMemberMethod initialize = new CodeMemberMethod ();
				initialize.Name = "InitializeComponent";
				initialize.ReturnType = new CodeTypeReference (typeof (void));
				initialize.Attributes = MemberAttributes.Private;

				initialize.Statements.AddRange (_preInit);
				initialize.Statements.AddRange (_init);
				initialize.Statements.AddRange (_postInit);

				clas.Members.Add (initialize);

				foreach (CodeMemberField field in _fields)
					clas.Members.Add (field);

				clas.EndDirectives.Add (new CodeRegionDirective (CodeRegionMode.End, null));

				return clas;
			}

			public void Clear ()
			{
				_preInit.Clear ();
				_init.Clear ();
				_postInit.Clear ();
				_fields.Clear ();
			}
		}


		private CodeMap _codeMap;
		private CodeDomSerializationProvider _provider;

		public RootCodeDomSerializer ()
		{
			_provider = new CodeDomSerializationProvider ();
		}

		public override object Serialize (IDesignerSerializationManager manager, object value)
		{
			if (manager == null)
				throw new ArgumentNullException ("manager");
			if (value == null)
				throw new ArgumentNullException ("value");

			// Add the provider to supply the ComponentCodeSerializer, Primitives..., etc
			manager.AddSerializationProvider (_provider);
			RootContext rootContext = new RootContext (new CodeThisReferenceExpression (), value);
			manager.Context.Push (rootContext);

			// Initialize code map
			if (_codeMap != null)
				_codeMap = new CodeMap (value.GetType (), manager.GetName (value));
			_codeMap.Clear ();
			CodeStatementCollection statements = null;
			CodeDomSerializer serializer = null;

			foreach (object component in ((IComponent) value).Site.Container.Components) {
				if (!Object.ReferenceEquals (component, value)) {
					serializer = base.GetSerializer (manager, component) as CodeDomSerializer; // ComponentCodeDomSerializer
					if (serializer != null) {
						// add an expressioncontext to inform the component that we want it fully serialized (it is in context)
						ExpressionContext context = new ExpressionContext (null, null, null, component);
						manager.Context.Push (context);

						_codeMap.AddField (new CodeMemberField (value.GetType (), manager.GetName (value)));
						statements = (CodeStatementCollection) serializer.Serialize (manager, component);

						manager.Context.Pop ();
						// XXX: what if there are more than one objects constructed by the serializer?
						// this will only add the first one on the statements list.
						CodeStatement ctorStatement = ExtractCtorStatement (statements);
						if (ctorStatement != null)
							_codeMap.AddPreInitStatement (ctorStatement);
						_codeMap.AddInitStatements (statements);
					}
				}
			}

			// Serializer root component
			// 
			statements = new CodeStatementCollection ();
			base.SerializeProperties (manager, statements, value, new Attribute[0]);
			base.SerializeEvents (manager, statements, value, new Attribute[0]);
			_codeMap.AddInitStatements (statements);

			manager.Context.Pop ();
			return _codeMap.GenerateClass ();
		}

		internal CodeMap Code {
			get { return _codeMap; }
		}

		private CodeStatement ExtractCtorStatement (CodeStatementCollection statements)
		{
			CodeStatement result = null;
			CodeAssignStatement assignment = null;
			int toRemove = -1;

			for (int i=0; i < statements.Count; i++) {
				assignment = statements[i] as CodeAssignStatement;
				if (assignment != null && assignment.Right is CodeObjectCreateExpression) {
					result = assignment;
					toRemove = i;
				}
			}

			if (toRemove != -1)
				statements.RemoveAt (toRemove);

			return result;
		}

		// TODO
		public override object Deserialize (IDesignerSerializationManager manager, object codeObject) 
		{
			throw new NotImplementedException ();
		}
	}
}
#endif
