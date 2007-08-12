using System.Windows;
using System.Windows.Controls;
namespace Mono.WindowsPresentationFoundation {
	/// <summary>
	/// An element that displays a plus/minus sign and represents expanding/collapsing content.
	/// </summary>
	public class PlusMinusElement : ThreeDElement {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty ShowsMinusProperty = DependencyProperty.Register("ShowsMinus", typeof(bool), typeof(PlusMinusElement), new PropertyMetadata(delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PlusMinusElement)d).SetSign();
		}));
		#endregion
		#endregion

		#region Private Fields
		TextBlock sign_block = new TextBlock();
		#endregion

		#region Public Constructors
		public PlusMinusElement() {
			Width = Height = 20;
			sign_block.HorizontalAlignment = HorizontalAlignment.Center;
			sign_block.VerticalAlignment = VerticalAlignment.Center;
			Child = sign_block;
			SetSign();
		}
		#endregion

		#region Public Fields
		#region Dependency Properties
		public bool ShowsMinus {
			get { return (bool)GetValue(ShowsMinusProperty); }
			set { SetValue(ShowsMinusProperty, value); }
		}
		#endregion
		#endregion

		#region Private Methods
		void SetSign() {
			sign_block.Text = ShowsMinus ? "-" : "+";
		}
		#endregion
	}
}