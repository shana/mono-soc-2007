using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.ApplicationBlocks.Data;
using System.Data.SqlClient;
using System.Data;

namespace Umbraco.Cms.BusinessLogic
{
    public class RecycleBin : CMSNode
    {
        private Guid _nodeObjectType;

        public RecycleBin(Guid NodeObjectType) : base(-20) {
            _nodeObjectType = NodeObjectType;
        }

        /// <summary>
        /// If I smell, I'm not empty 
        /// </summary>
        public bool Smells()
        {
            return
                (int.Parse(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN,
                CommandType.Text,
                "select count(id) from umbracoNode where nodeObjectType = @nodeObjectType and parentId = @parentId",
                new SqlParameter("@parentId", this.Id),
                new SqlParameter("@nodeObjectType", _nodeObjectType)).ToString()) > 0);

        }

        public override Umbraco.BusinessLogic.Console.IIcon[] Children
        {
            get
            {
                System.Collections.ArrayList tmp = new System.Collections.ArrayList();
                SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_ConnString, CommandType.Text, "select id from umbracoNode where ParentID = " + this.Id + " And NodeObjectType = '" + _nodeObjectType.ToString() + "' order by sortOrder");

                while (dr.Read())
                    tmp.Add(dr["Id"]);

                dr.Close();

                CMSNode[] retval = new CMSNode[tmp.Count];

                for (int i = 0; i < tmp.Count; i++)
                    retval[i] = new CMSNode((int)tmp[i]);

                return retval;
            }
        }

        /// <summary>
        /// Get the number of items in the Recycle Bin
        /// </summary>
        /// <returns>The number of all items in the Recycle Bin</returns>
        public static int Count()
        {
            return
                int.Parse(
                    Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text,
                                            "select count(id) from umbracoNode where path like '%,-20,%'").ToString());
        }

    
    }
}
