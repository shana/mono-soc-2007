//
// DesignSurfaceDisplayBinding.cs
//
// Authors:
//   Ivan N. Zlatev <contact@i-nz.net>
//
// Copyright (C) 2007 Ivan N. Zlatev
//
//
// This source code is licenced under The MIT License:
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

using System;
using System.IO;

using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Codons;
using MonoDevelop.Projects;

using Mono.Design;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.CSharp;
using ICSharpCode.NRefactory.Visitors;

using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace WinFormsAddin
{
	
	public class MDDesignerLoader : CodeDomDesignerLoader
	{
		string _designerFile;

		public MDDesignerLoader (string designerFile)
		{
			if (designerFile == null)
				throw new ArgumentNullException ("designerFile");

			_designerFile = designerFile;
		}

		protected override CodeDomProvider CodeDomProvider {
			get { return CodeProviderFactory.CreateCodeProvider (_designerFile); }
		}

		protected override ITypeResolutionService TypeResolutionService { 
			get { return base.LoaderHost.GetService (typeof (ITypeResolutionService)) as ITypeResolutionService; }
		}

		protected override CodeCompileUnit Parse ()
		{
			ICSharpCode.NRefactory.IParser parser = ICSharpCode.NRefactory.ParserFactory.CreateParser (_designerFile);
			parser.Parse ();
			CodeDomVisitor visitor = new CodeDomVisitor ();
			visitor.VisitCompilationUnit (parser.CompilationUnit, null);
			return visitor.codeCompileUnit;
		}

		protected override void Write (CodeCompileUnit unit)
		{
			StreamWriter writer = new StreamWriter (_designerFile, false /* append */);

			CodeGeneratorOptions options = new CodeGeneratorOptions ();
			options.BracingStyle = "C";
			options.BlankLinesBetweenMembers = false;
			options.VerbatimOrder = true;
			this.CodeDomProvider.GenerateCodeFromCompileUnit (unit, writer, options);

			writer.Close();
			writer.Dispose ();
		}
	}
}
