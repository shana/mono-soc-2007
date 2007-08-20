using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace mwf_designer
{
    public partial class PropertyGrid : UserControl
    {

        public PropertyGrid()
        {
            InitializeComponent();
        }

		private void OnPrimarySelectionChanged (object sender, EventArgs args)
		{
			ISelectionService selectionService = this.GetService (typeof (ISelectionService)) as ISelectionService;
			if (selectionService != null)
				this.ActiveComponent = (IComponent) selectionService.PrimarySelection;
		}

		private void OnComponentsCombo_SelectedIndexChanged (object sender, EventArgs args)
		{
			ISelectionService selectionService = this.GetService (typeof (ISelectionService)) as ISelectionService;
			if (selectionService != null && _componentsCombo.SelectedIndex != -1) {
				string selectedComponentName = (string) _componentsCombo.Items[_componentsCombo.SelectedIndex];
				IComponent selectedComponent = this.ActiveComponent.Site.Container.Components[selectedComponentName];
				selectionService.SetSelectedComponents (new IComponent[] { selectedComponent });
			}
		}

		public IComponent ActiveComponent {
			get { return _propertyGrid.SelectedObject as IComponent; }
			set {
				if (_propertyGrid.SelectedObject != null) // detach events from the prev active component
					DisableNotification ((IComponent)_propertyGrid.SelectedObject);

				PopulateComponents (value);
				_componentsCombo.SelectedIndex = _componentsCombo.Items.IndexOf (value.Site.Name);
				_propertyGrid.SelectedObject = value; 
//              ShowEventsTab (); // MWF's PropertyGrid doesn't support EventsTab
				EnableNotification (value);
			}
		}

		// MWF's PropertyGrid doesn't support EventsTab
		//
		private void ShowEventsTab ()
		{
			_propertyGrid.PropertyTabs.AddTabType (typeof (System.Windows.Forms.Design.EventsTab));
			_propertyGrid.Site = ActiveComponent.Site;
			_propertyGrid.GetType ().InvokeMember ("ShowEventsButton", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
							   null, _propertyGrid, new object [] { true });
		}

		private void EnableNotification (IComponent component)
		{
			IComponentChangeService changeService = this.GetService (typeof (IComponentChangeService)) as IComponentChangeService;
			if (changeService != null) {
				changeService.ComponentAdded += new ComponentEventHandler (OnComponentAdded);
				changeService.ComponentRemoving += new ComponentEventHandler (OnComponentRemoving);
				changeService.ComponentRename += new ComponentRenameEventHandler (OnComponentRename);
			}
			ISelectionService selectionService = this.GetService (typeof (ISelectionService)) as ISelectionService;
			if (selectionService != null)
				selectionService.SelectionChanged += new EventHandler (OnPrimarySelectionChanged);
		}

		private void DisableNotification (IComponent component)
		{
			IComponentChangeService changeService = this.GetService (typeof (IComponentChangeService)) as IComponentChangeService;
			if (changeService != null) {
				changeService.ComponentAdded -= new ComponentEventHandler (OnComponentAdded);
				changeService.ComponentRemoving -= new ComponentEventHandler (OnComponentRemoving);
				changeService.ComponentRename -= new ComponentRenameEventHandler (OnComponentRename);
			}
			ISelectionService selectionService = this.GetService (typeof (ISelectionService)) as ISelectionService;
			if (selectionService != null)
				selectionService.SelectionChanged -= new EventHandler (OnPrimarySelectionChanged);
		}


		private void OnComponentAdded (object sender, ComponentEventArgs args)
		{
			_componentsCombo.Items.Add (args.Component.Site.Name);
		}

		private void OnComponentRemoving (object sender, ComponentEventArgs args)
		{
			_componentsCombo.Items.Remove (args.Component.Site.Name);
		}

		private void OnComponentRename (object sender, ComponentRenameEventArgs args)
		{
			_componentsCombo.Items.Remove (args.OldName);
			_componentsCombo.SelectedIndex = _componentsCombo.Items.Add (args.NewName);
		}

		private void PopulateComponents (IComponent component)
		{
			ComponentCollection components = component.Site.Container.Components;
			_componentsCombo.Items.Clear ();
			foreach (IComponent c in components) {
				_componentsCombo.Items.Add (c.Site.Name);
			}
		}

		protected override object GetService (Type type)
		{
			if (this.ActiveComponent != null && this.ActiveComponent.Site != null)
				return this.ActiveComponent.Site.GetService (type);
			else
				return base.GetService (type);
		}
    }
}
