//
// TypeResolutionService
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

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;

using Mono.Design;

namespace WinFormsAddin
{
	public class TypeResolutionService : ITypeResolutionService
	{

		public Assembly GetAssembly (AssemblyName name)
		{
			throw new NotImplementedException ();
		}

		public Assembly GetAssembly (AssemblyName name, bool throwOnError)
		{
			throw new NotImplementedException ();
		}

		public string GetPathOfAssembly (AssemblyName name)
		{
			throw new NotImplementedException ();
		}

		public Type GetType (string name)
		{
			return this.GetType (name, false);
		}

		public Type GetType (string name, bool throwOnError)
		{
			return this.GetType (name, throwOnError, false);
		}

		public Type GetType (string name, bool throwOnError, bool ignoreCase)
		{
			if (name == null)
				throw new ArgumentNullException ("name");

			Type result = Type.GetType (name);
			if (result == null && name.StartsWith ("Form"))
				result = typeof (System.Windows.Forms.Form);
			if (result == null) {
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies ();
				foreach (Assembly a in assemblies) {
					result = a.GetType (name);
					if (result != null)
						break;
				}
			}
			return result;
		}

		public void ReferenceAssembly (AssemblyName name)
		{
			throw new NotImplementedException ();
		}
	}

}
