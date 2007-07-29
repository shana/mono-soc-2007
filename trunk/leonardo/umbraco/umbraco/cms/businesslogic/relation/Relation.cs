using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace Umbraco.Cms.BusinessLogic.relation
{
	/// <summary>
	/// Summary description for Relation.
	/// </summary>
	public class Relation
	{
		private int _id;
		private CMSNode _parentNode;
		private CMSNode _childNode;
		private string _comment;
		private DateTime _datetime;
		private RelationType _relType;

		public CMSNode Parent 
		{
			get {return _parentNode;}
			set 
			{
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "update umbracoRelation set parentId = @parent where id = " + this.Id, new SqlParameter("@parent", value.Id));
				_parentNode = value;
			}
		}

		public CMSNode Child 
		{
			get {return _childNode;}
			set 
			{
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "update umbracoRelation set childId = @child where id = " + this.Id, new SqlParameter("@child", value.Id));
				_childNode = value;
			}
		}

		public string Comment 
		{
			get {return _comment;}
			set 
			{
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "update umbracoRelation set comment = @comment where id = " + this.Id, new SqlParameter("@comment", value));
				_comment = value;
			}
		}

		public DateTime CreateDate 
		{
			get {return _datetime;}
		}

		public RelationType RelType
		{
			get {return _relType;}
			set 
			{
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "update umbracoRelation set relType = @relType where id = " + this.Id, new SqlParameter("@relType", value.Id));
				_relType = value;
			}
		}

		public int Id 
		{
			get {return _id;}
		}

		public Relation(int Id)
		{
			using (SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text, "select * from umbracoRelation where id = @id", new SqlParameter("@id", Id)))
			{
				if(dr.Read())
				{
					this._id = int.Parse(dr["id"].ToString());
					this._parentNode = new CMSNode(int.Parse(dr["parentId"].ToString()));
					this._childNode = new CMSNode(int.Parse(dr["childId"].ToString()));
					this._relType = RelationType.GetById(int.Parse(dr["relType"].ToString()));
					this._comment = dr["comment"].ToString();
					this._datetime = DateTime.Parse(dr["datetime"].ToString());
				}
			}
		}

        /// <summary>
        /// Used to persist object changes to the database. In v3.0 it's just a stub for future compatibility
        /// </summary>
        public virtual void Save()
        {
        }


		public void Delete() 
		{
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text, "delete from umbracoRelation where id = @id", new SqlParameter("@id", this.Id));
		}

		public static Relation MakeNew(int ParentId, int ChildId, RelationType RelType, string Comment) 
		{
			return new Relation(int.Parse(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text, "set nocount on insert into umbracoRelation (childId, parentId, relType, comment) values (@childId, @parentId, @relType, @comment) select @@identity as Id", new SqlParameter("@childId", ChildId), new SqlParameter("@parentId", ParentId), new SqlParameter("@relType", RelType.Id), new SqlParameter("@comment", Comment)).ToString()));
		}

		public static Relation[] GetRelations(int NodeId) 
		{
			System.Collections.ArrayList tmp = new System.Collections.ArrayList();
			using (SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text, "select umbracoRelation.id from umbracoRelation inner join umbracoRelationType on umbracoRelationType.id = umbracoRelation.relType where umbracoRelation.parentId = @id or (umbracoRelation.childId = @id and umbracoRelationType.dual = 1)", new SqlParameter("@id", NodeId)))
			{
				while(dr.Read())
				{
					tmp.Add(int.Parse(dr["id"].ToString()));
				}
			}

			Relation[] retval = new Relation[tmp.Count];
			
			for(int i = 0; i < tmp.Count; i++) retval[i] = new Relation((int) tmp[i]);
			return retval;

		}

        public static Relation[] GetRelations(int NodeId, RelationType Filter)
        {
            System.Collections.ArrayList tmp = new System.Collections.ArrayList();
			using (SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text, "select umbracoRelation.id from umbracoRelation inner join umbracoRelationType on umbracoRelationType.id = umbracoRelation.relType and umbracoRelationType.id = @relTypeId where umbracoRelation.parentId = @id or (umbracoRelation.childId = @id and umbracoRelationType.dual = 1)", new SqlParameter("@id", NodeId), new SqlParameter("@relTypeId", Filter.Id)))
			{
				while(dr.Read())
				{
					tmp.Add(int.Parse(dr["id"].ToString()));
				}
			}

            Relation[] retval = new Relation[tmp.Count];

            for (int i = 0; i < tmp.Count; i++) retval[i] = new Relation((int)tmp[i]);
            return retval;

        }

	}
}
