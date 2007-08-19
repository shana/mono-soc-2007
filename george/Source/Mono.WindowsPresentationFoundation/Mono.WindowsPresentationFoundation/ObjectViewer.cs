using System;
using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
namespace Mono.WindowsPresentationFoundation
{
	class ObjectViewer : TreeView
	{
		public object Object
		{
			set
			{
				Items.Clear ();
				if (value == null)
					return;
				Items.Add (new ObjectTreeViewItem ("Visual", value, value.GetType ()));
				ContextMenu = new ContextMenu ();
				MenuItem copy_menu_item = new MenuItem ();
				copy_menu_item.Header = "_Copy";
				copy_menu_item.Click += delegate (object sender, RoutedEventArgs e)
				{
					if (SelectedItem != null) {
						object object_ = ((ObjectTreeViewItem)SelectedItem).Object;
						if (object_ != null)
							try {
								Clipboard.SetText (object_.ToString ());
							} catch {
								MessageBox.Show ("There was an error.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
							}
					}
				};
				ContextMenu.Items.Add (copy_menu_item);
			}
		}

		class ObjectTreeViewItem : TreeViewItem
		{
			object object_;
			Type type;
			bool populated;

			public ObjectTreeViewItem (string name, object object_, Type type)
			{
				this.object_ = object_;
				this.type = type;
				Header = name + (object_ == null ? " = null" : (object_.GetType () == type ? " = " + object_ + " (" + object_.GetType ().Name + ")" : null));
				if (object_ == null)
					return;
				Type base_type = object_.GetType ().BaseType;
				if (base_type == typeof (ValueType))
					return;
				Items.Add (new TreeViewItem ());
			}

			protected override void OnExpanded (RoutedEventArgs e)
			{
				base.OnExpanded (e);
				if (populated)
					return;
				Items.Clear ();
				Type base_type = type.BaseType;
				if (base_type != null && base_type != typeof (object))
					Items.Add (new ObjectTreeViewItem ("(" + base_type.Name + ")", object_, base_type));
				foreach (PropertyInfo property in type.GetProperties (BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)) {
					if (property.GetIndexParameters ().GetLength (0) == 0) {
						object property_value = property.GetValue (object_, null);
						Items.Add (new ObjectTreeViewItem (property.Name, property_value, property_value == null ? null : property_value.GetType ()));
					}
				}
				if (typeof (IEnumerable).IsAssignableFrom (type)) {
					int counter = 0;
					foreach (object item in (IEnumerable)object_) {
						Items.Add (new ObjectTreeViewItem ("Item " + counter, item, item == null ? null : item.GetType ()));
						counter++;
					}
				}
				populated = true;
			}

			public object Object {
				get { return object_; }
			}
		}
	}
}