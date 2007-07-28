using System;

using umbraco.cms.businesslogic.web;

namespace umbraco.ActionHandlers
{
	/// <summary>
	/// Summary description for umbEnsureUniqueName.
	/// </summary>
	public class umbEnsureUniqueName : umbraco.BusinessLogic.Actions.IActionHandler
	{
		public umbEnsureUniqueName()
		{
		}
		#region IActionHandler Members

		public string HandlerName()
		{
			return "umbEnsureUniqueName";
		}

public bool Execute(umbraco.cms.businesslogic.web.Document documentObject, interfaces.IAction action)
{
	if (UmbracoSettings.EnsureUniqueNaming) 
	{
		string currentName = documentObject.Text;
		int uniqueNumber = 1;

		// Check for all items underneath the parent to see if they match
		// as any new created documents are stored in the bottom, we can just
		// keep checking for other documents with a uniquenumber from 
		foreach(umbraco.BusinessLogic.console.IconI d in documentObject.Parent.Children) 
		{
			if (d.Id != documentObject.Id && d.Text.ToLower() == currentName.ToLower()) 
			{
				currentName = documentObject.Text + " (" + uniqueNumber.ToString() + ")";
				uniqueNumber++;
			}
		}

		// if name has been changed, update the documentobject
		if (currentName != documentObject.Text) 
		{
			// add name change to the log
			umbraco.BusinessLogic.Log.Add(umbraco.BusinessLogic.LogTypes.Debug, umbraco.BusinessLogic.User.GetUser(0), documentObject.Id, "Title changed from '" + documentObject.Text + "' to '" + currentName + "'");

			documentObject.Text = currentName;
		
			return true;
		}
	}

	return false;
}

		public interfaces.IAction[] ReturnActions()
		{
			interfaces.IAction[] _retVal = {new umbraco.BusinessLogic.Actions.ActionNew()};
			return _retVal;
		}

		#endregion
	}
}
