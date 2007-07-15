/* vi:set ts=4 sw=4: */
//
// DesignerHost
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
using System.ComponentModel.Design.Serialization;
using System.Reflection;

namespace Mono.Design.Test
{

	internal sealed class DesignerHost : Container, IDesignerHost
	{

		private Hashtable _designers;
		private TypeDescriptorFilterService _filterServ;

		public DesignerHost ()
		{
			_designers = new Hashtable ();
			_filterServ = new TypeDescriptorFilterService (this);
			if (LoadComplete != null)
			LoadComplete (this, EventArgs.Empty);
		}

		
#region IContainer

		public override void Add (IComponent component, string name)
		{
			IDesigner designer;
			
			base.Add (component, name);
			
			if (_rootComponent == null) {
				_rootComponent = component;
				designer = this.CreateDesigner (component, true);
			}
			else {
				designer = this.CreateDesigner (component, false);
			}
			
			if (designer != null) {	
				_designers[component] = designer;
				designer.Initialize (component);
			}

			if (component is IExtenderProvider) {
				IExtenderProviderService service = this.GetService (typeof (IExtenderProviderService)) as IExtenderProviderService;
				if (service != null)
					service.AddExtenderProvider ((IExtenderProvider) component);
			}
		}

		public override void Remove (IComponent component)
		{
			IDesigner designer = _designers[component] as IDesigner;
			if (designer != null)
				designer.Dispose ();
			
			_designers.Remove (component);
			
			if (component == _rootComponent)
				_rootComponent = null;

			if (component is IExtenderProvider) {
				IExtenderProviderService service = GetService (typeof (IExtenderProviderService)) as IExtenderProviderService;
				if (service != null)
					service.RemoveExtenderProvider ((IExtenderProvider) component);
			}
			
			base.Remove (component);
		}

		protected override ISite CreateSite (IComponent component, string name)
		{
			return (ISite) new DesignerSite (component, name, this, this);
		}
#endregion

		
#region IDesignerHost

		private IComponent _rootComponent;
		
		public IContainer Container {
			get { return this; }
		}

		public bool InTransaction {
			get { return false; }
		}

		// indicates if the host (designerLoader) is currently loading a document
		// 
		public bool Loading {
			get {
				// FIXME: this should check with the loader probably!!!!
				return false;
			}
		}

		public IComponent RootComponent {
			get { return _rootComponent; }
		}

		public string RootComponentClassName {
			get {
				if (_rootComponent != null)
					return ((object)_rootComponent).GetType ().AssemblyQualifiedName;

				return null;
			}
		}

		public string TransactionDescription {
			get { return null; }
		}

		public void Activate ()
		{
			if (Activated != null)
				Activated (this, EventArgs.Empty);
		}
		
		public IComponent CreateComponent (Type componentClass)
		{
			return CreateComponent (componentClass, null);
		}

		public IComponent CreateComponent (Type componentClass, string name)
		{
			if (componentClass == null)
				throw new ArgumentNullException ("componentClass");
				
			else if (!typeof(IComponent).IsAssignableFrom(componentClass))
				throw new ArgumentException ("componentClass");

			IComponent component = this.CreateInstance (componentClass) as IComponent;
			this.Add (component, name);
			
			return component;
		}

		internal object CreateInstance (Type type)
		{
			if (type == null)
				throw new System.ArgumentNullException ("type");
			
			return Activator.CreateInstance (type, BindingFlags.CreateInstance | BindingFlags.Public
							 | BindingFlags.Instance, null,  null, null);
		}

		internal IDesigner CreateDesigner (IComponent component, bool rootDesigner)
		{
			if (component == null)
				throw new System.ArgumentNullException ("component");
			
			if (rootDesigner)
				return TypeDescriptor.CreateDesigner (component, typeof (IRootDesigner));
			else
				return TypeDescriptor.CreateDesigner (component, typeof (IDesigner));
		}

		public void DestroyComponent (IComponent component)
		{
				this.Remove (component); // takes care of the designer as well
				component.Dispose ();
		}

		public IDesigner GetDesigner (IComponent component)
		{
			if (component == null)
				throw new ArgumentNullException ("component");
			
			return _designers[component] as IDesigner;
		}

		public DesignerTransaction CreateTransaction ()
		{
			return CreateTransaction (null);
		}
		
		public DesignerTransaction CreateTransaction (string description)
		{
			return null;
		}

		public Type GetType (string typeName)
		{
			Type result;
			ITypeResolutionService s = GetService (typeof (ITypeResolutionService)) as ITypeResolutionService;
			
			if (s != null)
				result = s.GetType (typeName);
			else
				result = Type.GetType (typeName);
			
			return result;
		}

		// Take care of disposing the designer the base.Dispose will cleanup
		// the components.
		//
		protected override void Dispose (bool disposing)
		{
			IComponent[] components = new IComponent[this.Components.Count];
			this.Components.CopyTo (components, 0);
			
			foreach (IComponent component in components)
				this.Remove (component);

			if (Deactivated != null)
				Deactivated (this, EventArgs.Empty);
							
			base.Dispose (disposing);
		}

		
		public event EventHandler Activated;
		public event EventHandler Deactivated;
		public event EventHandler LoadComplete;	 
		public event DesignerTransactionCloseEventHandler TransactionClosed;
		public event DesignerTransactionCloseEventHandler TransactionClosing;
		public event EventHandler TransactionOpened;
		public event EventHandler TransactionOpening;
		
#endregion

	
#region IServiceContainer

		public void AddService (Type serviceType, object serviceInstance)
		{
		}
		
		public void AddService (Type serviceType, object serviceInstance, bool promote)
		{
		}
		
		public void AddService (Type serviceType, ServiceCreatorCallback callback)
		{
		}

		public void AddService (Type serviceType, ServiceCreatorCallback callback, bool promote)
		{
		}

		public void RemoveService (Type serviceType)
		{
		}

		public void RemoveService (Type serviceType, bool promote)
		{
		}
			
#endregion


#region IServiceProvider

		public new object GetService (Type serviceType)
		{
			if (serviceType == typeof (IDesignerHost))
					return this;
			else if (serviceType == typeof (ITypeDescriptorFilterService))
					return _filterServ;
			return null;
		}
		
#endregion
	  
	}

	internal class DesignerSite : ISite, IServiceProvider
	{

		private IServiceProvider _serviceProvider;
		private IComponent _component;
		private IContainer _container;
		private string _componentName;
		

		public DesignerSite (IComponent component, string name, IContainer container, IServiceProvider serviceProvider)
		{
			_component = component;
			_container = container;
			_componentName = name;
			_serviceProvider = serviceProvider;
		}

		public IComponent Component {
			get { return _component; }
		}

		public IContainer Container {
			get { return _container; }
		}

		public bool DesignMode {
			get { return true; }
		}

		public string Name {
			get { return _componentName; }
			set { _componentName = value; }
		}
		
#region IServiceProvider

		public virtual object GetService (Type service)
		{
			return _serviceProvider.GetService (service);
		}
#endregion

	}

	internal sealed class TypeDescriptorFilterService : ITypeDescriptorFilterService, IDisposable
	{

		IServiceProvider _serviceProvider;
		
		public TypeDescriptorFilterService (IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
				throw new ArgumentNullException ("serviceProvider");

			_serviceProvider = serviceProvider;
		}

		// Return values are:
		// true if the set of filtered attributes is to be cached; false if the filter service must query again.
		//	  
		public bool FilterAttributes (IComponent component, IDictionary attributes)
		{
			if (_serviceProvider == null)
				throw new ObjectDisposedException ("TypeDescriptorFilterService");
			if (component == null)
				throw new ArgumentNullException ("component");
			
			IDesignerHost designerHost = _serviceProvider.GetService (typeof (IDesignerHost)) as IDesignerHost;
			if (designerHost != null) {
				IDesigner designer = designerHost.GetDesigner (component);
				if (designer is IDesignerFilter) {
					((IDesignerFilter) designer).PreFilterAttributes (attributes);
					((IDesignerFilter) designer).PostFilterAttributes (attributes);
				}
			}

			return true;
		}
	
		public bool FilterEvents (IComponent component, IDictionary events)
		{
			if (_serviceProvider == null)
				throw new ObjectDisposedException ("TypeDescriptorFilterService");
			if (component == null)
				throw new ArgumentNullException ("component");
			
			IDesignerHost designerHost = _serviceProvider.GetService (typeof (IDesignerHost)) as IDesignerHost;
			if (designerHost != null) {
				IDesigner designer = designerHost.GetDesigner (component);
				if (designer is IDesignerFilter) {
					((IDesignerFilter) designer).PreFilterEvents (events);
					((IDesignerFilter) designer).PostFilterEvents (events);
				}
			}
			
			return true;
		}
	
		public bool FilterProperties (IComponent component, IDictionary properties)
		{
			if (_serviceProvider == null)
				throw new ObjectDisposedException ("TypeDescriptorFilterService");
			if (component == null)
				throw new ArgumentNullException ("component");

			IDesignerHost designerHost = _serviceProvider.GetService (typeof (IDesignerHost)) as IDesignerHost;
			if (designerHost != null) {
				IDesigner designer = designerHost.GetDesigner (component);
				if (designer is IDesignerFilter) {
					((IDesignerFilter) designer).PreFilterProperties (properties);
					((IDesignerFilter) designer).PostFilterProperties (properties);
				}
			}
			
			return true;
		}

		public void Dispose ()
		{
			_serviceProvider = null;
		}
	}
}

