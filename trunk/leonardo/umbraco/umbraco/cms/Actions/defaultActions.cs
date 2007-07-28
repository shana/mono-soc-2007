using System;

namespace umbraco.BusinessLogic.Actions
{
	/// <summary>
	/// This action is invoked upon creation of a document
	/// </summary>
    public class ActionNew : interfaces.IAction
    {
        #region IAction Members

        public char Letter
        {
            get
            {
                return 'C';
            }
        }

        public string JsFunctionName
        {
            get
            {
                // TODO:  Add ActionNew.JsFunctionName getter implementation
                return "parent.createNew()";
            }
        }

        public string JsSource
        {
            get
            {
                // TODO:  Add ActionNew.JsSource getter implementation
                return null;
            }
        }

        public string Alias
        {
            get
            {
                // TODO:  Add ActionNew.Alias getter implementation
                return "create";
            }
        }

        public string Icon
        {
            get
            {
                // TODO:  Add ActionNew.Icon getter implementation
                return "new.gif";
            }
        }

        public bool ShowInNotifier
        {
            get
            {
                // TODO:  Add ActionNew.ShowInNotifier getter implementation
                return true;
            }
        }
        public bool CanBePermissionAssigned
        {
            get
            {
                // TODO:  Add ActionNew.ShowInNotifier getter implementation
                return true;
            }
        }

        #endregion
    }
    public class ActionSendToTranslate : interfaces.IAction
    {
        #region IAction Members

        public char Letter
        {
            get
            {
                return '5';
            }
        }

        public string JsFunctionName
        {
            get
            {
                return "parent.translateThis()";
            }
        }

        public string JsSource
        {
            get
            {
                return null;
            }
        }

        public string Alias
        {
            get
            {
                return "sendToTranslate";
            }
        }

        public string Icon
        {
            get
            {
                return "sendToTranslate.png";
            }
        }

        public bool ShowInNotifier
        {
            get
            {
                return true;
            }
        }
        public bool CanBePermissionAssigned
        {
            get
            {
                return true;
            }
        }

        #endregion
    }

    public class ActionEmptyTranscan : interfaces.IAction
    {
        #region IAction Members

        public char Letter
        {
            get
            {
                return 'N';
            }
        }

        public string JsFunctionName
        {
            get
            {
                return "parent.emptyTrashcan()";
            }
        }

        public string JsSource
        {
            get
            {
                return null;
            }
        }

        public string Alias
        {
            get
            {
                return "emptyTrashcan";
            }
        }

        public string Icon
        {
            get
            {
                return "tree/bin_empty.png";
            }
        }

        public bool ShowInNotifier
        {
            get
            {
                return false;
            }
        }
        public bool CanBePermissionAssigned
        {
            get
            {
                return false;
            }
        }

        #endregion
    }
    public class ActionTranslate : interfaces.IAction
    {
        #region IAction Members

        public char Letter
        {
            get
            {
                return '4';
            }
        }

        public string JsFunctionName
        {
            get
            {
                // TODO:  Add ActionNew.JsFunctionName getter implementation
                return "";
            }
        }

        public string JsSource
        {
            get
            {
                // TODO:  Add ActionNew.JsSource getter implementation
                return null;
            }
        }

        public string Alias
        {
            get
            {
                // TODO:  Add ActionNew.Alias getter implementation
                return "translate";
            }
        }

        public string Icon
        {
            get
            {
                // TODO:  Add ActionNew.Icon getter implementation
                return "translate.png";
            }
        }

        public bool ShowInNotifier
        {
            get
            {
                // TODO:  Add ActionNew.ShowInNotifier getter implementation
                return true;
            }
        }
        public bool CanBePermissionAssigned
        {
            get
            {
                // TODO:  Add ActionNew.ShowInNotifier getter implementation
                return true;
            }
        }

        #endregion
    }

	/// <summary>
	/// This action is invoked upon saving of a document, media, member
	/// </summary>
	public class ActionSave : interfaces.IAction
	{

		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return '0';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "";
			}
		}

		public string JsSource
		{
			get
			{
				// TODO:  Add ActionNew.JsSource getter implementation
				return null;
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "save";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "save.png";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}

		#endregion
	}

    public class ActionImport : interfaces.IAction
    {

        #region IAction Members

        public char Letter
        {
            get
            {
                // TODO:  Add ActionNew.Letter getter implementation
                return '8';
            }
        }

        public string JsFunctionName
        {
            get
            {
                // TODO:  Add ActionNew.JsFunctionName getter implementation
                return "parent.importDocumentType()";
            }
        }

        public string JsSource
        {
            get
            {
                // TODO:  Add ActionNew.JsSource getter implementation
                return null;
            }
        }

        public string Alias
        {
            get
            {
                // TODO:  Add ActionNew.Alias getter implementation
                return "importDocumentType";
            }
        }

        public string Icon
        {
            get
            {
                // TODO:  Add ActionNew.Icon getter implementation
                return "importDocumentType.png";
            }
        }

        public bool ShowInNotifier
        {
            get
            {
                // TODO:  Add ActionNew.ShowInNotifier getter implementation
                return false;
            }
        }
        public bool CanBePermissionAssigned
        {
            get
            {
                // TODO:  Add ActionNew.ShowInNotifier getter implementation
                return false;
            }
        }

        #endregion
    }

    public class ActionExport : interfaces.IAction
    {

        #region IAction Members

        public char Letter
        {
            get
            {
                // TODO:  Add ActionNew.Letter getter implementation
                return '9';
            }
        }

        public string JsFunctionName
        {
            get
            {
                // TODO:  Add ActionNew.JsFunctionName getter implementation
                return "parent.exportDocumentType()";
            }
        }

        public string JsSource
        {
            get
            {
                // TODO:  Add ActionNew.JsSource getter implementation
                return null;
            }
        }

        public string Alias
        {
            get
            {
                // TODO:  Add ActionNew.Alias getter implementation
                return "exportDocumentType";
            }
        }

        public string Icon
        {
            get
            {
                // TODO:  Add ActionNew.Icon getter implementation
                return "exportDocumentType.png";
            }
        }

        public bool ShowInNotifier
        {
            get
            {
                // TODO:  Add ActionNew.ShowInNotifier getter implementation
                return false;
            }
        }
        public bool CanBePermissionAssigned
        {
            get
            {
                // TODO:  Add ActionNew.ShowInNotifier getter implementation
                return false;
            }
        }

        #endregion
    }


	/// <summary>
	/// This action is invoked upon viewing audittrailing on a document
	/// </summary>
	public class ActionAudit : interfaces.IAction
	{
		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return 'Z';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "parent.viewAuditTrail()";
			}
		}

		public string JsSource
		{
			get
			{
				// TODO:  Add ActionNew.JsSource getter implementation
				return null;
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "auditTrail";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "audit.png";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}

		#endregion
	}

	/// <summary>
	/// This action is invoked upon creation of a document, media, member
	/// </summary>
	public class ActionPackage : interfaces.IAction
	{
		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return 'X';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "importPackage()";
			}
		}

		public string JsSource
		{
			get
			{
				// TODO:  Add ActionNew.JsSource getter implementation
				return	"";
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "importPackage";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "package2.png";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}

		#endregion
	}
	
	/// <summary>
	/// This action is invoked upon creation of a document, media, member
	/// </summary>
	public class ActionPackageCreate : interfaces.IAction
	{
		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return 'Y';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "createPackage()";
			}
		}

		public string JsSource
		{
			get
			{
				// TODO:  Add ActionNew.JsSource getter implementation
				return null;
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "createPackage";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "package2.png";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}

		#endregion
	}
	/// <summary>
	/// This action is invoked when a document, media, member is deleted
	/// </summary>
	public class ActionDelete : interfaces.IAction
	{
		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return 'D';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "parent.deleteThis()";
			}
		}

		public string JsSource
		{
			get
			{
				// TODO:  Add ActionNew.JsSource getter implementation
				return null;
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "delete";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "delete.small.png";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}
		#endregion
	}

	/// <summary>
	/// This action is invoked when a document is disabled.
	/// </summary>
	public class ActionDisable : interfaces.IAction
	{
		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return 'E';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "parent.disableThis()";
			}
		}

		public string JsSource
		{
			get
			{
				// TODO:  Add ActionNew.JsSource getter implementation
				return null;
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "disable";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "delete.small.png";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}
		#endregion
	}
	/// <summary>
	/// This action is invoked upon creation of a document, media, member
	/// </summary>
	public class ActionMove : interfaces.IAction
	{

		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return 'M';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "parent.moveThis()";
			}
		}

		public string JsSource
		{
			get
			{
				// TODO:  Add ActionNew.JsSource getter implementation
				return null;
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "move";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "cut.small.png";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}
		#endregion
	}

	/// <summary>
	/// This action is invoked when copying a document, media, member 
	/// </summary>
	public class ActionCopy : interfaces.IAction
	{

		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return 'O';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "parent.copyThis()";
			}
		}

		public string JsSource
		{
			get
			{
				// TODO:  Add ActionNew.JsSource getter implementation
				return null;
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "copy";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "copy.small.png";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}
		#endregion
	}
	
	/// <summary>
	/// This action is invoked when children to a document, media, member is being sorted
	/// </summary>
	public class ActionSort : interfaces.IAction
	{
		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return 'S';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "parent.sortThis()";
			}
		}

		public string JsSource
		{
			get
			{
				// TODO:  Add ActionNew.JsSource getter implementation
				return null;
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "sort";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "sort.png";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}
		#endregion
	}

	/// <summary>
	/// This action is invoked when rights are changed on a document
	/// </summary>
	public class ActionRights : interfaces.IAction
	{
		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return 'R';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "parent.rightsThis()";
			}
		}

		public string JsSource
		{
			get
			{
				// TODO:  Add ActionNew.JsSource getter implementation
				return null;
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "rights";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "permission.png";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}
		#endregion
	}

	/// <summary>
	/// This action is invoked when a document is protected or unprotected
	/// </summary>
	public class ActionProtect : interfaces.IAction
	{

		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return 'P';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "parent.protectThis()";
			}
		}

		public string JsSource
		{
			get
			{
				// TODO:  Add ActionNew.JsSource getter implementation
				return null;
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "protect";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "protect.png";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}
		#endregion
	}

	/// <summary>
	/// This action is invoked when copying a document is being rolled back
	/// </summary>
	public class ActionRollback : interfaces.IAction
	{

		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return 'K';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "parent.rollback()";
			}
		}

		public string JsSource
		{
			get
			{
				return "";
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "rollback";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "rollback.png";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}
		#endregion
	}

	/// <summary>
	/// This action is invoked when a node reloads its children
	/// Concerns only the tree itself and thus you should not handle
	/// this action from without umbraco.
	/// </summary>
	public class ActionRefresh : interfaces.IAction
	{
		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return 'L';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "parent.refreshNode()";
			}
		}

		public string JsSource
		{
			get
			{
				// TODO:  Add ActionNew.JsSource getter implementation
				return null;
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "refreshNode";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "refresh.png";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}
		#endregion
	}

	/// <summary>
	/// This action is invoked when a notification is sent 
	/// </summary>
	public class ActionNotify : interfaces.IAction
	{
		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return 'T';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "parent.notifyThis()";
			}
		}

		public string JsSource
		{
			get
			{
				// TODO:  Add ActionNew.JsSource getter implementation
				return null;
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "notify";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "notify.gif";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}
		#endregion
	}

	/// <summary>
	/// This action is invoked when copying a document or media 
	/// </summary>
	public class ActionUpdate : interfaces.IAction
	{
		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return 'A';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "parent.updateThis()";
			}
		}

		public string JsSource
		{
			get
			{
				// TODO:  Add ActionNew.JsSource getter implementation
				return null;
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "update";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "update.png";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}
		#endregion
	}

	/// <summary>
	/// This action is invoked when a document is being published
	/// </summary>
	public class ActionPublish : interfaces.IAction
	{
		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return 'U';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "parent.publish()";
			}
		}

		public string JsSource
		{
			get
			{
				// TODO:  Add ActionNew.JsSource getter implementation
				return null;
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "publish";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "publish.gif";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}
		#endregion
	}

	/// <summary>
	/// This action is invoked when children to a document is being sent to published (by an editor without publishrights)
	/// </summary>
	public class ActionToPublish : interfaces.IAction
	{
		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return 'H';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "parent.toPublish()";
			}
		}

		public string JsSource
		{
			get
			{
				// TODO:  Add ActionNew.JsSource getter implementation
				return null;
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "sendtopublish";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "toPublish.png";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}
		#endregion
	}

	/// <summary>
	/// This action is invoked when a user logs out
	/// </summary>
	public class ActionQuit : interfaces.IAction
	{
		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return 'Q';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "parent.closeUmbraco()";
			}
		}

		public string JsSource
		{
			get
			{
				// TODO:  Add ActionNew.JsSource getter implementation
				return null;
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "logout";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "logout.png";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}
		#endregion
	}

	/// <summary>
	/// This action is invoked when all documents are being republished
	/// </summary>
	public class ActionRePublish : interfaces.IAction
	{
		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return 'B';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "parent.republish()";
			}
		}

		public string JsSource
		{
			get
			{
				// TODO:  Add ActionNew.JsSource getter implementation
				return null;
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "republish";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "publish.gif";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}
		#endregion
	}

	/// <summary>
	/// This action is invoked when a domain is being assigned to a document
	/// </summary>
	public class ActionAssignDomain : interfaces.IAction
	{
		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return 'I';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "parent.assignDomain()";
			}
		}

		public string JsSource
		{
			get
			{
				// TODO:  Add ActionNew.JsSource getter implementation
				return null;
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "assignDomain";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "domain.gif";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return true;
			}
		}
		#endregion
	}

	/// <summary>
	/// This action is invoked when a document is being unpublished
	/// </summary>
	public class ActionUnPublish : interfaces.IAction
	{
		#region IAction Members

		public char Letter
		{
			get
			{
				// TODO:  Add ActionNew.Letter getter implementation
				return 'Z';
			}
		}

		public string JsFunctionName
		{
			get
			{
				// TODO:  Add ActionNew.JsFunctionName getter implementation
				return "";
			}
		}

		public string JsSource
		{
			get
			{
				// TODO:  Add ActionNew.JsSource getter implementation
				return null;
			}
		}

		public string Alias
		{
			get
			{
				// TODO:  Add ActionNew.Alias getter implementation
				return "unpublish";
			}
		}

		public string Icon
		{
			get
			{
				// TODO:  Add ActionNew.Icon getter implementation
				return "delete.gif";
			}
		}

		public bool ShowInNotifier
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}
		public bool CanBePermissionAssigned
		{
			get
			{
				// TODO:  Add ActionNew.ShowInNotifier getter implementation
				return false;
			}
		}
		#endregion
	}

}
