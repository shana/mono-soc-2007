using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace umbraco.cms.businesslogic.member
{
	/// <summary>
	/// Membergroups are used for grouping Umbraco Members
	/// 
	/// A Member can exist in multiple groups.
	/// 
	/// It's possible to protect webpages/documents by membergroup.
	/// </summary>
	public class MemberGroup : CMSNode
	{
		private static Guid _objectType = new Guid("366e63b9-880f-4e13-a61c-98069b029728");

		/// <summary>
		/// Initialize a new object of the MemberGroup class
		/// </summary>
		/// <param name="id">Membergroup id</param>
		public MemberGroup(int id): base(id)
		{

		}

		/// <summary>
		/// Initialize a new object of the MemberGroup class
		/// </summary>
		/// <param name="id">Membergroup id</param>
		public MemberGroup(Guid id) : base(id)
		{
			
		}

        /// <summary>
        /// Deltes the current membergroup
        /// </summary>
        public new void delete()
        {
            // delete member specific data!
            SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text, "Delete from cmsMember2MemberGroup where memberGroup = @id",
                new SqlParameter("@id", Id));

            // Delete all content and cmsnode specific data!
            base.delete();
        }


        /// <summary>
        /// Used to persist object changes to the database. In v3.0 it's just a stub for future compatibility
        /// </summary>
        public override void Save()
        {
        }

		/// <summary>
		/// Retrieve a list of all existing MemberGroups
		/// </summary>
		public static MemberGroup[] GetAll 
		{
			get
			{
				Guid[] tmp = getAllUniquesFromObjectType(_objectType);
				MemberGroup[] retval = new MemberGroup[tmp.Length];

				int i = 0;
				foreach(Guid g in tmp)
				{
					retval[i]= new MemberGroup(g);
					i++;
				}
				return retval;
			}
		}

		/// <summary>
		/// Get a membergroup by it's name
		/// </summary>
		/// <param name="Name">Name of the membergroup</param>
		/// <returns>If a MemberGroup with the given name exists, it will return this, else: null</returns>
		public static MemberGroup GetByName(string Name) 
		{
			try 
			{
				return
					new MemberGroup(
						int.Parse(
							SqlHelper.ExecuteScalar(
								GlobalSettings.DbDSN,
								CommandType.Text,
								"select id from umbracoNode where Text = @text and NodeObjectType = @objectType",
								new SqlParameter("@text", Name),
								new SqlParameter("@objectType", _objectType)).ToString()));
			} 
			catch 
			{
				return null;
			}
		}

		/// <summary>
		/// Create a new MemberGroup
		/// </summary>
		/// <param name="Name">The name of the MemberGroup</param>
		/// <param name="u">The creator of the MemberGroup</param>
		/// <returns>The new MemberGroup</returns>
		public static MemberGroup MakeNew(string Name, BusinessLogic.User u) 
		{
			Guid newId = Guid.NewGuid();
			CMSNode.MakeNew(-1,_objectType, u.Id, 1,  Name, newId);
			
			return new MemberGroup(newId);
		}
	}
}
