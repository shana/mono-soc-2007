using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;

using umbraco.BasePages;
using umbraco.BusinessLogic.Utils;
using umbraco.cms.businesslogic.web;
using umbraco.cms.businesslogic.workflow;
using umbraco.interfaces;

namespace umbraco.BusinessLogic.Actions
{
	/// <summary>
	/// Actions and Actionhandlers are a key concept to umbraco and a developer whom wish to apply
	/// businessrules whenever data is changed within umbraco, by implementing the IActionHandler
	/// interface it's possible to invoke methods (foreign to umbraco) - this can be used whenever
	/// there is a specific rule which needs to be applied to content.
	///
	/// The Action class itself has responsibility for registering actions and actionhandlers,
	/// and contains methods which will be invoked whenever a change is made to ex. a document, media or member
	/// 
	/// An action/actionhandler will automatically be registered, using reflection 
	/// which is enabling thirdparty developers to extend the core functionality of
	/// umbraco without changing the codebase.
	/// </summary>
	public class Action
	{
		private static readonly List<IAction> _actions = new List<IAction>();
		private static readonly Dictionary<string, string> _actionJs = new Dictionary<string, string>();
		private static readonly List<IActionHandler> _actionHandlers = new List<IActionHandler>();

		static Action()
		{
			registerActionHandlers();
		}

		/// <summary>
		/// Initialization of all ActionHandlers
		/// 
		/// By using reflection, this method scans the /bin folder for components which implements the 
		/// IActionHandler interface.
		/// </summary>
		private static void registerActionHandlers()
		{
			string _pluginFolder = GlobalSettings.Path + "/../bin";
			HttpContext.Current.Trace.Write("action.factory", "Adding actions from directory: " + HttpContext.Current.Server.MapPath(_pluginFolder));

			string[] types = TypeResolver.GetAssignablesFromType<IActionHandler>(
				HttpContext.Current.Server.MapPath(_pluginFolder), "*.dll");
			foreach(string type in types)
			{
				Type t = Type.GetType(type);
				IActionHandler typeInstance = Activator.CreateInstance(t) as IActionHandler;
				if(typeInstance == null)
					continue;
				_actionHandlers.Add(typeInstance);
				if(HttpContext.Current != null)
					HttpContext.Current.Trace.Write("FindActionHandlers", " + Adding actionhandler '" + typeInstance.HandlerName());
			}
		}

		/// <summary>
		/// Whenever an action is performed upon a document/media/member, this method is executed, ensuring that 
		/// all registered handlers will have an oppotunity to handle the action.
		/// </summary>
		/// <param name="d">The document being operated on</param>
		/// <param name="action">The action triggered</param>
		public static void RunActionHandlers(Document d, IAction action)
		{
			foreach(IActionHandler ia in _actionHandlers)
			{
				try
				{
					foreach(IAction a in ia.ReturnActions())
					{
						if(a.Alias == action.Alias)
						{
							// Uncommented for auto publish support
							//								System.Web.HttpContext.Current.Trace.Write("BusinessLogic.Action.RunActionHandlers", "Running " + ia.HandlerName() + " (matching action: " + a.Alias + ")");
							ia.Execute(d, action);
						}
					}
				}
				catch(Exception iaExp)
				{
					Log.Add(LogTypes.Error, User.GetUser(0), -1, string.Format("Error loading actionhandler '{0}': {1}",
						ia.HandlerName(), iaExp));
				}
			}

			// Run notification
			// Find current user
			User u;
			try
			{
				u = new UmbracoEnsuredPage().getUser();
			}
			catch
			{
				u = User.GetUser(0);
			}
			Notification.GetNotifications(d, u, action);
		}

		/// <summary>
		/// Jacascript for the contextmenu
		/// Suggestion: this method should be moved to the presentation layer.
		/// </summary>
		/// <param name="language"></param>
		/// <returns>String representation</returns>
		public string ReturnJavascript(string language)
		{
			return findActions(language);
		}

		/// <summary>
		/// Javascript menuitems - tree contextmenu
		/// Umbraco console
		/// 
		/// Suggestion: this method should be moved to the presentation layer.
		/// </summary>
		/// <param name="language"></param>
		/// <returns></returns>
		private static string findActions(string language)
		{
			if(!_actionJs.ContainsKey(language))
			{
				string _actionJsList = "";
				string _pluginFolder = GlobalSettings.Path + "/../bin";

				HttpContext.Current.Trace.Write("datatype.factory", "Adding actions from directory: " + HttpContext.Current.Server.MapPath(_pluginFolder));

				string[] types = TypeResolver.GetAssignablesFromType<IAction>(
					HttpContext.Current.Server.MapPath(_pluginFolder), "*.dll");
				foreach(string type in types)
				{
					Type t = Type.GetType(type);
					IAction typeInstance = Activator.CreateInstance(t) as IAction;
					if(typeInstance == null)
						continue;

					// Add to language JsList
					_actionJsList += string.Format(",\n\tmenuItem(\"{0}\", \"{1}\", \"{2}\", \"{3}\")",
						typeInstance.Letter, typeInstance.Icon, ui.GetText("actions", typeInstance.Alias, language), typeInstance.JsFunctionName);

                    // Test if current action is in the list
				    IAction _tempAction = _actions.Find(delegate(IAction action) { return action.Alias == typeInstance.Alias; });
					if(_tempAction == null)
						_actions.Add(typeInstance);
				}

				if(_actionJsList.Length > 0)
					_actionJsList = _actionJsList.Substring(2, _actionJsList.Length - 2);

				_actionJsList = "\nvar menuMethods = new Array(\n" + _actionJsList + "\n)\n";
				_actionJs.Add(language, _actionJsList);
			}

			return _actionJs[language];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns>An arraylist containing all javascript variables for the contextmenu in the tree</returns>
		public static ArrayList GetAll()
		{
			findActions(GlobalSettings.DefaultUILanguage);
			return new ArrayList(_actions);
		}
	}

	/// <summary>
	/// Implement the IActionHandler interface in order to automatically get code
	/// run whenever a document, member or media changed, deleted, created etc.
	/// The Clases implementing IActionHandler are loaded at runtime which means
	/// that there are no other setup when creating a custom actionhandler.
	/// </summary>
	/// <example>
	/// 
	/// </example>
	public interface IActionHandler
	{
		bool Execute(Document documentObject, IAction action);
		IAction[] ReturnActions();
		string HandlerName();
	}
}
