#if Implementation
using System.Windows;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[Localizability(LocalizationCategory.Ignore)]
	public abstract class DefinitionBase : FrameworkContentElement {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty SharedSizeGroupProperty = DependencyProperty.Register("SharedSizeGroup", typeof(string), typeof(DefinitionBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits), delegate(object value) {
			if (value == null)
				return true;
			string string_value = (string)value;
			if (string_value.Length == 0)
				return false;
			char character = string_value[0];
			if (!(char.IsLetter(character) || character == '_'))
				return false;

			for (int character_index = 1; character_index < string_value.Length; character_index++) {
				character = string_value[character_index];
				if (!(char.IsLetterOrDigit(character) || character == '_'))
					return false;
			}
			return true;
		});
		#endregion
		#endregion

		#region Internal Constructors
		internal DefinitionBase() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public string SharedSizeGroup {
			get { return (string)GetValue(SharedSizeGroupProperty); }
			set { SetValue(SharedSizeGroupProperty, value); }
		}
		#endregion
		#endregion
	}
}