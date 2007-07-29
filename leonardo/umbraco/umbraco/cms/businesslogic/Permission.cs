using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace Umbraco.BusinessLogic
{
	/// <summary>
	/// Summary description for Permission.
	/// </summary>
	public class Permission
	{
		public Permission() 
		{
		}

		public static void MakeNew(BusinessLogic.User User, Cms.BusinessLogic.CMSNode Node, char PermissionKey) 
		{
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "if not exists(select userId from umbracoUser2nodePermission where userId = @userId and nodeId = @nodeId and permission = @permission) insert into umbracoUser2nodePermission (userId, nodeId, permission) values (@userId, @nodeId, @permission)", new SqlParameter("@userId", User.Id), new SqlParameter("@nodeId", Node.Id), new SqlParameter("@permission", PermissionKey.ToString()));
		}

		public static void UpdateCruds(BusinessLogic.User User, Cms.BusinessLogic.CMSNode Node, string Permissions) 
		{
			// delete all settings on the node for this user
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "delete from umbracoUser2NodePermission where userId = @userId and nodeId = @nodeId", new SqlParameter("@userId", User.Id), new SqlParameter("@nodeId", Node.Id)); 

			// Loop through the permissions and create them
			foreach (char c in Permissions.ToCharArray())
				MakeNew(User, Node, c);
		}
	}
}