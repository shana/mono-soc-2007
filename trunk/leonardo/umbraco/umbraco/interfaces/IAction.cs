using System;

namespace umbraco.interfaces
{
	/// <summary>
	/// Summary description for ActionI.
	/// </summary>
	public interface IAction
	{
		char Letter {get;}
		bool ShowInNotifier {get;}
		bool CanBePermissionAssigned {get;}
		string Icon {get;}
		string Alias {get;}
		string JsFunctionName {get;}
		string JsSource {get;}
	}
}
