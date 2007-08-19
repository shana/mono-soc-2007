//
// DefinitionBase.cs
//
// Author:
//   George Giolfan (georgegiolfan@yahoo.com)
//
// Copyright (C) 2007 George Giolfan
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

#if Implementation
using System.Windows;
namespace Mono.System.Windows.Controls
{
#else
namespace System.Windows.Controls {
#endif
	[Localizability (LocalizationCategory.Ignore)]
	public abstract class DefinitionBase : FrameworkContentElement
	{
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty SharedSizeGroupProperty = DependencyProperty.Register ("SharedSizeGroup", typeof (string), typeof (DefinitionBase), new FrameworkPropertyMetadata (null, FrameworkPropertyMetadataOptions.Inherits), delegate (object value)
		{
			if (value == null)
				return true;
			string string_value = (string)value;
			if (string_value.Length == 0)
				return false;
			char character = string_value [0];
			if (!(char.IsLetter (character) || character == '_'))
				return false;

			for (int character_index = 1; character_index < string_value.Length; character_index++) {
				character = string_value [character_index];
				if (!(char.IsLetterOrDigit (character) || character == '_'))
					return false;
			}
			return true;
		});
		#endregion
		#endregion

		#region Internal Constructors
		internal DefinitionBase ()
		{
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public string SharedSizeGroup {
			get { return (string)GetValue (SharedSizeGroupProperty); }
			set { SetValue (SharedSizeGroupProperty, value); }
		}
		#endregion
		#endregion
	}
}