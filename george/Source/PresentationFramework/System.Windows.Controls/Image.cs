using System.Windows.Automation.Peers;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
	public class Image : FrameworkElement, IUriContext {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(Image), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
		public static readonly DependencyProperty StretchDirectionProperty = DependencyProperty.Register("StretchDirection", typeof(StretchDirection), typeof(Image), new FrameworkPropertyMetadata(StretchDirection.Both, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(Image), new FrameworkPropertyMetadata(Stretch.Uniform, FrameworkPropertyMetadataOptions.AffectsMeasure));
		#endregion

		#region Routed Events
		public static readonly RoutedEvent ImageFailedEvent = EventManager.RegisterRoutedEvent("ImageFailed", RoutingStrategy.Bubble, typeof(EventHandler), typeof(Image));
		#endregion
		#endregion

		#region Private Fields
		Uri base_uri;
		#endregion

		#region Public Constructors
		public Image() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		public ImageSource Source {
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		public Stretch Stretch {
			get { return (Stretch)GetValue(StretchProperty); }
			set { SetValue(StretchProperty, value); }
		}

		public StretchDirection StretchDirection {
			get { return (StretchDirection)GetValue(StretchDirectionProperty); }
			set { SetValue(StretchDirectionProperty, value); }
		}
		#endregion
		#endregion

		#region Protected Properties
		protected virtual Uri BaseUri {
			get { return base_uri; }
			set { base_uri = value; }
		}
		#endregion

		#region Protected Methods
		protected override Size ArrangeOverride(Size finalSize) {
			//WDTDH
			return base.ArrangeOverride(finalSize);
		}

		protected override Size MeasureOverride(Size availableSize) {
			//WDTDH
			return base.MeasureOverride(availableSize);
		}

		protected override AutomationPeer OnCreateAutomationPeer() {
#if Implementation
			return null;
#else
			return new ImageAutomationPeer(this);
#endif
		}

		protected override void OnRender(DrawingContext drawingContext) {
			base.OnRender(drawingContext);
			ImageSource source = Source;
			if (source == null)
				return;
			//FIXME:
			if (source is BitmapImage)
				return;
			double x;
			double y;
			double width;
			double height;
			switch (Stretch) {
			case Stretch.None:
				x = 0;
				y = 0;
				width = source.Width;
				height = source.Height;
				break;
			case Stretch.Fill:
				x = 0;
				y = 0;
				width = ActualWidth;
				height = ActualHeight;
				break;
			default:
				x = 0;
				y = 0;
				width = ActualWidth;
				height = ActualHeight;
				break;
			}
			drawingContext.DrawImage(Source, new Rect(x, y, width, height));
		}
		#endregion

		#region Public Events
		public event EventHandler ImageFailed {
			add { AddHandler(ImageFailedEvent, value); }
			remove { RemoveHandler(ImageFailedEvent, value); }
		}
		#endregion

		#region Explicit Interface Implementations
		Uri IUriContext.BaseUri {
			get { return BaseUri; }
			set { BaseUri = value; }
		}
		#endregion
	}
}