using System.Collections;
using System.ComponentModel;
using System.Windows.Markup;
#if Implementation
using System;
using System.Windows;
using System.Windows.Controls;
namespace Mono.System.Windows.Controls {
#else
namespace System.Windows.Controls {
#endif
	[Localizability(LocalizationCategory.None, Readability=Readability.Unreadable)] 
	[ContentProperty("Content")] 
	public class ContentControl : Control, IAddChild {
		#region Public Fields
		#region Dependency Properties
		public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(ContentControl), new FrameworkPropertyMetadata(null, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ContentControl i = (ContentControl)d;
			i.should_serialize_content = true;
			i.OnContentChanged(e.OldValue, e.NewValue);
		}));
		public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(ContentControl), new FrameworkPropertyMetadata(null, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ContentControl)d).OnContentTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
		}));
		public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyProperty.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(ContentControl), new FrameworkPropertyMetadata(null, delegate(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ContentControl)d).OnContentTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue);
		}));
		static readonly DependencyPropertyKey HasContentPropertyKey = DependencyProperty.RegisterReadOnly("HasContent", typeof(bool), typeof(ContentControl), new FrameworkPropertyMetadata());
		public static readonly DependencyProperty HasContentProperty = HasContentPropertyKey.DependencyProperty;
		#endregion
		#endregion

		#region Private Fields
		bool should_serialize_content;
		#endregion

		#region Static Constructor
		static ContentControl() {
#if Implementation
			Theme.Load();
#endif
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ContentControl), new FrameworkPropertyMetadata(typeof(ContentControl)));
		}
		#endregion

		#region Public Constructors
		public ContentControl() {
		}
		#endregion

		#region Public Properties
		#region Dependency Properties
		[Bindable(true)]
		public object Content {
			get { return GetValue(ContentProperty); }
			set {
				//HACK: It should not probably be done this way.
				//FIXME: It does not solve the problem if SetValue is called directly.
				should_serialize_content = true;
				SetValue(ContentProperty, value); 
			}
		}

		[Bindable(true)]
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}

		[Bindable(true)]
		public DataTemplateSelector ContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
			set { SetValue(ContentTemplateSelectorProperty, value); }
		}

		public bool HasContent {
			get { return (bool)GetValue(HasContentProperty); }
			private set { SetValue(HasContentPropertyKey, value); }
		}
		#endregion
		#endregion

		#region Protected Properties
		protected override IEnumerator LogicalChildren {
			get {
				object content = Content;
				return (content == null ? new object[] { } : new object[] { content }).GetEnumerator();
			}
		}
		#endregion

		#region Public Methods
		public virtual bool ShouldSerializeContent() {
			return should_serialize_content;
		}
		#endregion

		#region Protected Methods
		protected virtual void AddChild(object value) {
			CheckContent();
			Content = value;
		}

		protected virtual void AddText(string text) {
			CheckContent();
			Content = text;
		}

		protected virtual void OnContentChanged(object oldContent, object newContent) {
		}

		protected virtual void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate) {
		}

		protected virtual void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector) {
		}
		#endregion
		
		#region Explicit Interface Implementations
		#region IAddChild
		void IAddChild.AddChild(object value) {
			AddChild(value);
		}

		void IAddChild.AddText(string text) {
			AddText(text);
		}
		#endregion
		#endregion

		#region Private Methods
		void CheckContent() {
			if (Content != null)
				throw new InvalidOperationException("Content of a ContentControl must be a single element.");
		}
		#endregion
	}
}