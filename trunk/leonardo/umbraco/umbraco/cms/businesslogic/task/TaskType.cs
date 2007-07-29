using System;
using System.Collections.Generic;
using System.Text;

using Umbraco.BusinessLogic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace Umbraco.Cms.BusinessLogic.task
{
    public class TaskType
    {
        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _alias;

        public string Alias
        {
            get { return _alias; }
            set { _alias = value; }
        }

        public TaskType()
        {
        }

        public TaskType(string TypeAlias)
        {
            Id = int.Parse(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(
                GlobalSettings.DbDSN,
                CommandType.Text,
                "select id from cmsTaskType where alias = @alias",
                new SqlParameter("@alias", TypeAlias)).ToString());
            setup();
        }

        public TaskType(int TaskTypeId)
        {
            Id = TaskTypeId;
            setup();
        }

        private void setup()
        {
			using (SqlDataReader dr =
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(
				GlobalSettings.DbDSN,
				CommandType.Text,
				"select alias from cmsTaskType where id = @id",
				new SqlParameter("@id", Id)))
			{
				if(dr.Read())
				{
					_id = Id;
					Alias = dr["alias"].ToString();
				}
			}
        }

        public void Save()
        {
            if (Id == 0)
            {
                Id = int.Parse(
                    Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(
                    GlobalSettings.DbDSN,
                    CommandType.Text,
                    "SET NOCOUNT ON insert into cmsTaskType (alias) values (@alias) select @@IDENTITY SET NOCOUNT OFF",
                    new SqlParameter("@alias", Alias)).ToString());
            }
            else
            {
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(
                    GlobalSettings.DbDSN,
                    CommandType.Text,
                    "update cmsTaskType set alias = @alias where id = @id",
                    new SqlParameter("@alias", Alias),
                    new SqlParameter("@id", Id));
            }
        }
	
    }
}
