using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Microsoft.ApplicationBlocks.Data;

namespace umbraco.cms.businesslogic.datatype
{
    public class PreValues
    {
        public static SortedList GetPreValues(int DataTypeId)
        {
            SortedList retval = new SortedList();
            SqlDataReader dr = SqlHelper.ExecuteReader(
                umbraco.GlobalSettings.DbDSN,
                CommandType.Text,
                "Select id, sortorder, [value] from cmsDataTypeprevalues where DataTypeNodeId = @dataTypeId order by sortorder",
                new SqlParameter("@dataTypeId", DataTypeId));

            int counter = 0;
            while (dr.Read())
            {
                retval.Add(counter, new PreValue(int.Parse(dr["id"].ToString()), int.Parse(dr["sortorder"].ToString()), dr["value"].ToString()));
                counter++;
            }
            dr.Close();
            return retval;
        }

    }

    public class PreValue
    {
        public PreValue()
        {
            
        }

        public void Save()
        {
            // Check for new
            if (Id == 0)
            {
                // Update sortOrder
                int tempSortOrder =
                    int.Parse(SqlHelper.ExecuteScalar(
                                  GlobalSettings.DbDSN,
                                  CommandType.Text,
                                  "select max(sortorder) from cmsDataTypePreValues where datatypenodeid = @dataTypeId",
                                  new SqlParameter("@dataTypeId", DataTypeId)).ToString());
                SortOrder = tempSortOrder+1;

                SqlParameter[] SqlParams = new SqlParameter[] {
								new SqlParameter("@value",Value),
								new SqlParameter("@dtdefid",DataTypeId)};
                _id = int.Parse(SqlHelper.ExecuteScalar(umbraco.GlobalSettings.DbDSN, CommandType.Text, "SET NOCOUNT OFF insert into cmsDataTypePrevalues (datatypenodeid,[value],sortorder,alias) values (@dtdefid,@value,0,'') select @@identity SET NOCOUNT ON", SqlParams).ToString());
            }

            SqlHelper.ExecuteNonQuery(
                GlobalSettings.DbDSN,
                CommandType.Text,
                "update cmsDataTypePrevalues set sortorder = @sortOrder, [value] = @value where id = @id",
                new SqlParameter("@sortOrder", SortOrder),
                new SqlParameter("@value", Value),
                new SqlParameter("@id", Id));
        }

        public PreValue(int Id, int SortOrder, string Value)
        {
            _id = Id;
            _sortOrder = SortOrder;
            _value = Value;
        }

        public PreValue(int Id)
        {
            _id = Id;
            initialize();
        }

        public PreValue(int DataTypeId, string Value)
        {
            object id = SqlHelper.ExecuteScalar(
                umbraco.GlobalSettings.DbDSN,
                CommandType.Text,
                "Select id from cmsDataTypeprevalues where [Value] = @value and DataTypeNodeId = @dataTypeId",
                new SqlParameter("@dataTypeId", DataTypeId),
                new SqlParameter("@value", Value));
            if (id != null)
                _id = int.Parse(id.ToString());
            
            initialize();
        }

        private void initialize()
        {
            SqlDataReader dr = SqlHelper.ExecuteReader(
                 umbraco.GlobalSettings.DbDSN,
                 CommandType.Text,
                 "Select id, sortorder, [value] from cmsDataTypeprevalues where id = @id order by sortorder",
                 new SqlParameter("@id", _id));
            if (dr.Read())
            {
                _sortOrder = int.Parse(dr["sortorder"].ToString());
                _value = dr["value"].ToString();
            }
            dr.Close();
        }

        private int  _dataTypeId;

        public int  DataTypeId
        {
            get { return _dataTypeId; }
            set { _dataTypeId = value; }
        }
	

        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
	

        private string  _value;

        public string  Value
        {
            get { return _value; }
            set { _value = value; }
        }
	

        private int _sortOrder;

        public int SortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; }
        }
	
    }
}
