using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;

namespace Umbraco.BusinessLogic
{
	/// <summary>
	/// Summary description for Log.
	/// </summary>
	public static class Log
	{
		/// <summary>
		/// Adds the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="user">The user.</param>
		/// <param name="nodeId">The node id.</param>
		/// <param name="comment">The comment.</param>
		public static void Add(LogTypes type, User user, int nodeId, string comment)
		{
			if(GlobalSettings.DisableLogging) return;

			if(comment.Length > 3999)
				comment = comment.Substring(0, 3955) + "...";

			if (GlobalSettings.EnableAsyncLogging)
			{
				ThreadPool.QueueUserWorkItem(
					delegate { AddSynced(type, user == null ? 0 : user.Id, nodeId, comment); });
				return;
			}

			AddSynced(type, user == null ? 0 : user.Id, nodeId, comment);
		}

		/// <summary>
		/// Adds the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="nodeId">The node id.</param>
		/// <param name="comment">The comment.</param>
		public static void Add(LogTypes type, int nodeId, string comment)
		{
			Add(type, null, nodeId, comment);
		}

		/// <summary>
		/// Adds the synced.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="userId">The user id.</param>
		/// <param name="nodeId">The node id.</param>
		/// <param name="comment">The comment.</param>
		public static void AddSynced(LogTypes type, int userId, int nodeId, string comment)
		{
			try
			{
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text,
					"insert into umbracoLog (userId, nodeId, logHeader, logComment) values (@userId, @nodeId, @logHeader, @comment)",
					new SqlParameter("@userId", userId),
					new SqlParameter("@nodeId", nodeId),
					new SqlParameter("@logHeader", type.ToString()),
					new SqlParameter("@comment", comment));
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.ToString(), "Error");
				Trace.WriteLine(e.ToString());
			}
		}

		/// <summary>
		/// Gets the log.
		/// </summary>
		/// <param name="Type">The type.</param>
		/// <param name="SinceDate">The since date.</param>
		/// <returns></returns>
		public static DataSet GetLog(LogTypes Type, DateTime SinceDate)
		{
			return Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteDataset(GlobalSettings.DbDSN, CommandType.Text,
				"select userId, NodeId, DateStamp, logHeader, logComment from umbracoLog where logHeader = @logHeader and DateStamp >= @dateStamp order by dateStamp desc",
				new SqlParameter("@logHeader", Type.ToString()),
				new SqlParameter("@dateStamp", SinceDate));
		}

		/// <summary>
		/// Gets the log.
		/// </summary>
		/// <param name="Type">The type.</param>
		/// <param name="SinceDate">The since date.</param>
		/// <param name="Limit">The limit.</param>
		/// <returns></returns>
		public static DataSet GetLog(LogTypes Type, DateTime SinceDate, int Limit)
		{
			return Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteDataset(GlobalSettings.DbDSN, CommandType.Text,
				"select top " + Limit +
				" userId, NodeId, DateStamp, logHeader, logComment from umbracoLog where logHeader = @logHeader and DateStamp >= @dateStamp order by dateStamp desc",
				new SqlParameter("@logHeader", Type.ToString()),
				new SqlParameter("@dateStamp", SinceDate));
		}

		/// <summary>
		/// Gets the log.
		/// </summary>
		/// <param name="NodeId">The node id.</param>
		/// <returns></returns>
		public static DataSet GetLog(int NodeId)
		{
			return Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteDataset(GlobalSettings.DbDSN, CommandType.Text,
				"select u.userName, DateStamp, logHeader, logComment from umbracoLog inner join umbracoUser u on u.id = userId where nodeId = @id",
				new SqlParameter("@id", NodeId));
		}

		/// <summary>
		/// Gets the audit log.
		/// </summary>
		/// <param name="NodeId">The node id.</param>
		/// <returns></returns>
		public static DataSet GetAuditLog(int NodeId)
		{
			return Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteDataset(GlobalSettings.DbDSN, CommandType.Text,
				"select u.userName as [User], logHeader as Action, DateStamp as Date, logComment as Comment from umbracoLog inner join umbracoUser u on u.id = userId where nodeId = @id and logHeader not in ('open','system') order by DateStamp desc",
				new SqlParameter("@id", NodeId));
		}

		/// <summary>
		/// Gets the log.
		/// </summary>
		/// <param name="u">The u.</param>
		/// <param name="SinceDate">The since date.</param>
		/// <returns></returns>
		public static DataSet GetLog(User u, DateTime SinceDate)
		{
			return Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteDataset(GlobalSettings.DbDSN, CommandType.Text,
				"select userId, NodeId, DateStamp, logHeader, logComment from umbracoLog where UserId = @user and DateStamp >= @dateStamp order by dateStamp desc",
				new SqlParameter("@user", u.Id), new SqlParameter("@dateStamp", SinceDate));
		}

		/// <summary>
		/// Gets the log.
		/// </summary>
		/// <param name="u">The u.</param>
		/// <param name="Type">The type.</param>
		/// <param name="SinceDate">The since date.</param>
		/// <returns></returns>
		public static DataSet GetLog(User u, LogTypes Type, DateTime SinceDate)
		{
			return Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteDataset(GlobalSettings.DbDSN, CommandType.Text,
				"select userId, NodeId, DateStamp, logHeader, logComment from umbracoLog where UserId = @user and logHeader = @logHeader and DateStamp >= @dateStamp order by dateStamp desc",
				new SqlParameter("@logHeader", Type.ToString()),
				new SqlParameter("@user", u.Id), new SqlParameter("@dateStamp", SinceDate));
		}

		/// <summary>
		/// Gets the log.
		/// </summary>
		/// <param name="u">The u.</param>
		/// <param name="Type">The type.</param>
		/// <param name="SinceDate">The since date.</param>
		/// <param name="Limit">The limit.</param>
		/// <returns></returns>
		public static DataSet GetLog(User u, LogTypes Type, DateTime SinceDate, int Limit)
		{
			return Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteDataset(GlobalSettings.DbDSN, CommandType.Text,
				"select top " + Limit +
				" userId, NodeId, DateStamp, logHeader, logComment from umbracoLog where UserId = @user and logHeader = @logHeader and DateStamp >= @dateStamp order by dateStamp desc",
				new SqlParameter("@logHeader", Type.ToString()),
				new SqlParameter("@user", u.Id), new SqlParameter("@dateStamp", SinceDate));
		}
	}

	public enum LogTypes
	{
		New,
		Save,
		Open,
		Delete,
		Publish,
		SendToPublish,
		UnPublish,
		Move,
		Copy,
		AssignDomain,
		PublicAccess,
		Sort,
		Notify,
		Login,
		Logout,
		LoginFailure,
		System,
		Debug,
		Error,
		NotFound,
		RollBack,
		PackagerInstall,
		PackagerUninstall,
		Ping,
		SendToTranslate,
		ScheduledTask
	}
}