using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
[TestFixture]
public class Tests {
	[Test]
	public void PropertyMetadataForPresentationFrameworkDependencyPropertiesIsFrameworkPropertyMetadata() {
		List<string> excluded = new List<string>(new string[] {
			"System.Windows.Controls.ContextMenu.CustomPopupPlacementCallbackProperty",
			"System.Windows.Controls.ContextMenu.StaysOpenProperty",
			"System.Windows.Controls.HeaderedItemsControl.HasHeaderProperty",
			"System.Windows.Controls.FlowDocumentScrollViewer.ZoomIncrementProperty",
			"System.Windows.Controls.ContextMenuService.ContextMenuProperty",
			"System.Windows.Controls.ContextMenuService.HorizontalOffsetProperty",
			"System.Windows.Controls.ContextMenuService.VerticalOffsetProperty",
			"System.Windows.Controls.ContextMenuService.HasDropShadowProperty",
			"System.Windows.Controls.ContextMenuService.PlacementTargetProperty",
			"System.Windows.Controls.ContextMenuService.PlacementRectangleProperty",
			"System.Windows.Controls.ContextMenuService.PlacementProperty",
			"System.Windows.Controls.ContextMenuService.ShowOnDisabledProperty",
			"System.Windows.Controls.ContextMenuService.IsEnabledProperty",
			"System.Windows.Controls.FlowDocumentReader.ZoomIncrementProperty",
			"System.Windows.Controls.Frame.CanGoBackProperty",
			"System.Windows.Controls.Frame.CanGoForwardProperty",
			"System.Windows.Controls.Frame.BackStackProperty",
			"System.Windows.Controls.Frame.ForwardStackProperty",
			"System.Windows.Controls.Frame.NavigationUIVisibilityProperty",
			"System.Windows.Controls.GridView.ColumnCollectionProperty",
			"System.Windows.Controls.GridView.ColumnHeaderContainerStyleProperty",
			"System.Windows.Controls.GridView.ColumnHeaderContextMenuProperty",
			"System.Windows.Controls.GridView.ColumnHeaderToolTipProperty",
			"System.Windows.Controls.GridViewColumn.CellTemplateProperty",
			"System.Windows.Controls.GridViewColumn.CellTemplateSelectorProperty",
			"System.Windows.Controls.GridViewColumn.WidthProperty",
			"System.Windows.Controls.GridViewColumnHeader.ColumnProperty",
			"System.Windows.Controls.GridViewHeaderRowPresenter.AllowsColumnReorderProperty",
			"System.Windows.Controls.ListView.ViewProperty",
			"System.Windows.Controls.Page.KeepAliveProperty",
			"System.Windows.Controls.PasswordBox.MaxLengthProperty",
			"System.Windows.Controls.SpellCheck.IsEnabledProperty",
			"System.Windows.Controls.SpellCheck.SpellingReformProperty",
			"System.Windows.Controls.ToolTip.CustomPopupPlacementCallbackProperty",
			"System.Windows.Controls.ToolTip.StaysOpenProperty",
			"System.Windows.Controls.ToolTipService.ToolTipProperty",
			"System.Windows.Controls.ToolTipService.HorizontalOffsetProperty",
			"System.Windows.Controls.ToolTipService.VerticalOffsetProperty",
			"System.Windows.Controls.ToolTipService.HasDropShadowProperty",
			"System.Windows.Controls.ToolTipService.PlacementTargetProperty",
			"System.Windows.Controls.ToolTipService.PlacementRectangleProperty",
			"System.Windows.Controls.ToolTipService.PlacementProperty",
			"System.Windows.Controls.ToolTipService.ShowOnDisabledProperty",
			"System.Windows.Controls.ToolTipService.IsOpenProperty",
			"System.Windows.Controls.ToolTipService.IsEnabledProperty",
			"System.Windows.Controls.ToolTipService.ShowDurationProperty",
			"System.Windows.Controls.ToolTipService.InitialShowDelayProperty",
			"System.Windows.Controls.ToolTipService.BetweenShowDelayProperty",
			"System.Windows.Controls.Validation.ErrorsProperty",
			"System.Windows.Controls.Validation.HasErrorProperty",
			"System.Windows.Controls.Validation.ErrorTemplateProperty",
		});
		Assembly presentation_framework_assembly = typeof(
#if Implementation
		Mono.
#endif
		System.Windows.Controls.Button).Assembly;
		foreach(Type type in presentation_framework_assembly.GetExportedTypes()) {
			string namespace_name = type.Namespace;
#if Implementation
			namespace_name = namespace_name.Remove(0, "Mono.".Length);
#endif
			if (namespace_name == "System.Windows.Controls" || namespace_name == "System.Windows.Controls.Primitives")
				foreach(FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
					if (field.Name.EndsWith("Property") && field.FieldType == typeof(DependencyProperty)) {
						string field_name = type.FullName + "." + field.Name;
#if Implementation
						field_name = field_name.Remove(0, "Mono.".Length);
#endif
						if (!excluded.Contains(field_name))
							Assert.IsTrue(((DependencyProperty)field.GetValue(null)).GetMetadata(type).GetType() == typeof(FrameworkPropertyMetadata), field_name);
					}
		}
	}
}