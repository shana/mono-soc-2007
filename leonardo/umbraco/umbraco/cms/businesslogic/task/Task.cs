using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using Umbraco.BusinessLogic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace Umbraco.Cms.BusinessLogic.task
{
    public class Task
    {

        #region Properties
        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private bool _closed;

        public bool Closed
        {
            get { return _closed; }
            set { _closed = value; }
        }
	

        private CMSNode _node;

        public CMSNode Node
        {
            get { return _node; }
            set { _node = value; }
        }

        private TaskType _type;

        public TaskType Type
        {
            get { return _type; }
            set { _type = value; }
        }
	

        private User _parentUser;

        public User ParentUser
        {
            get { return _parentUser; }
            set { _parentUser = value; }
        }

        private string _comment;

        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }
	

        private DateTime _date;

        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }
	
        private User _user;

        public User User
        {
            get { return _user; }
            set { _user = value; }
        }

        #endregion

        #region Constructors

        public Task()
        {
        }

        public Task(int TaskId)
        {
			using (SqlDataReader dr =
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(
				GlobalSettings.DbDSN,
				CommandType.Text,
				"select taskTypeId, nodeId, parentUserId, userId, DateTime, comment from cmsTask where id = @id",
				new SqlParameter("@id", TaskId)))
			{
				if(dr.Read())
				{
					_id = TaskId;
					Type = new TaskType(int.Parse(dr["taskTypeId"].ToString()));
					Node = new CMSNode(int.Parse(dr["nodeId"].ToString()));
					ParentUser = User.GetUser(int.Parse(dr["parentUserId"].ToString()));
					User = User.GetUser(int.Parse(dr["userId"].ToString()));
					Date = dr.GetDateTime(dr.GetOrdinal("DateTime"));
					Comment = dr["comment"].ToString();
				}
				else
					throw new ArgumentException("Task with id: '" + TaskId + "' not found");
			}
        }

        #endregion

        #region Public Methods


        public void Save()
        {
            if (Id == 0)
            {
                Id = int.Parse(
                    Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(
                    GlobalSettings.DbDSN,
                    CommandType.Text,
                    "SET NOCOUNT ON insert into cmsTask (closed, taskTypeId, nodeId, parentUserId, userId, comment) values (@closed, @taskTypeId, @nodeId, @parentUserId, @userId, @comment) select @@IDENTITY SET NOCOUNT OFF",
                    new SqlParameter("@closed", Closed),
                    new SqlParameter("@taskTypeId", Type.Id),
                    new SqlParameter("@nodeId", Node.Id),
                    new SqlParameter("@parentUserId", ParentUser.Id),
                    new SqlParameter("@userId", User.Id),
                    new SqlParameter("@comment", Comment)).ToString());
            }
            else
            {
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(
                    GlobalSettings.DbDSN,
                    CommandType.Text,
                    "update cmsTask set closed = @closed, taskTypeId = @taskTypeId, nodeId = @nodeId, parentUserId = @parentUserId, userId = @userId, comment = @comment where id = @id",
                    new SqlParameter("@closed", Closed),
                    new SqlParameter("@taskTypeId", Type.Id),
                    new SqlParameter("@nodeId", Node.Id),
                    new SqlParameter("@parentUserId", ParentUser.Id),
                    new SqlParameter("@userId", User.Id),
                    new SqlParameter("@comment", Comment),
                    new SqlParameter("@id", Id));
            }
        }

        #endregion

        #region static methods

        /// <summary>
        /// Retrieves a collection of open tasks assigned to the user
        /// </summary>
        /// <param Name="User">The User who have the tasks assigned</param>
        /// <param Name="IncludeClosed">If true both open and closed tasks will be returned</param>
        /// <returns>A collections of tasks</returns>
        public static Tasks GetTasks(User User, bool IncludeClosed) {
            string sql = "select id from cmsTask where userId = @userId";
            if (!IncludeClosed)
                sql += " and closed = 0";
            sql += " order by DateTime desc";

            Tasks t = new Tasks();
			using (SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(
				GlobalSettings.DbDSN,
				CommandType.Text,
				sql,
				new SqlParameter("@userId", User.Id)))
			{
				while(dr.Read())
					t.Add(new Task(int.Parse(dr["id"].ToString())));
			}

            return t;
        }

        #endregion
    }

    public class Tasks : CollectionBase
	{
		public virtual void Add(Task NewTask)
		{
			this.List.Add(NewTask);
		}

		public virtual Task this[int Index]
		{
			get { return (Task) this.List[Index]; }
		}
	}
}
