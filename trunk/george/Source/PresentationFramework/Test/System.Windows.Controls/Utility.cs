namespace System.Windows.Controls
{
	static class Utility
	{
		static double empty_button_size = -1;
		public static double GetEmptyButtonSize ()
		{
			if (empty_button_size == -1) {
				Window window = new Window ();
				Canvas canvas = new Canvas ();
				window.Content = canvas;
				Button button = new Button ();
				canvas.Children.Add (button);
				window.Show ();
				empty_button_size = button.DesiredSize.Width;
				window.Close ();
			}
			return empty_button_size;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value1"></param>
		/// <param name="value2"></param>
		/// <returns></returns>
		/// <remarks>Designed for pixel values.</remarks>
		public static bool AreCloseEnough (double value1, double value2)
		{
			return Math.Abs (value1 - value2) < 0.1;
		}

		public static FrameworkPropertyMetadataOptions GetOptions (FrameworkPropertyMetadata value)
		{
			FrameworkPropertyMetadataOptions result = FrameworkPropertyMetadataOptions.None;
			if (value.AffectsArrange)
				result |= FrameworkPropertyMetadataOptions.AffectsArrange;
			if (value.AffectsMeasure)
				result |= FrameworkPropertyMetadataOptions.AffectsMeasure;
			if (value.AffectsParentArrange)
				result |= FrameworkPropertyMetadataOptions.AffectsParentArrange;
			if (value.AffectsParentMeasure)
				result |= FrameworkPropertyMetadataOptions.AffectsParentMeasure;
			if (value.AffectsRender)
				result |= FrameworkPropertyMetadataOptions.AffectsRender;
			if (value.BindsTwoWayByDefault)
				result |= FrameworkPropertyMetadataOptions.BindsTwoWayByDefault;
			if (value.Inherits)
				result |= FrameworkPropertyMetadataOptions.Inherits;
			if (value.Journal)
				result |= FrameworkPropertyMetadataOptions.Journal;
			if (value.IsNotDataBindable)
				result |= FrameworkPropertyMetadataOptions.NotDataBindable;
			if (value.OverridesInheritanceBehavior)
				result |= FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior;
			if (value.SubPropertiesDoNotAffectRender)
				result |= FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender;
			return result;
		}
	}
}