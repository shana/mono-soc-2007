using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace umbraco.cms.businesslogic.member
{
	/// MemberType
	/// 
	/// Due to the inheritance of the ContentType class, it enables definition of generic datafields on a Members.
	/// 
	public class MemberType : ContentType
	{
		private static Guid _objectType = new Guid("9b5416fb-e72f-45a9-a07b-5a9a2709ce43");

		/// <summary>
		/// Initializes a new instance of the MemberType class.
		/// </summary>
		/// <param name="id">MemberType id</param>
		public MemberType(int id) : base(id)
		{
		}
		/// <summary>
		/// Initializes a new instance of the MemberType class.
		/// </summary>
		/// <param name="id">MemberType id</param>
		public MemberType(Guid id) : base(id)
		{
		}


        /// <summary>
        /// Used to persist object changes to the database. In v3.0 it's just a stub for future compatibility
        /// </summary>
        public override void Save()
        {
        }


		/// <summary>
		/// Create a new MemberType
		/// </summary>
		/// <param name="Text">The name of the MemberType</param>
		/// <param name="u">Creator of the MemberType</param>
		public static MemberType MakeNew( BusinessLogic.User u,string Text) 
		{
		
			int ParentId= -1;
			int level = 1;
			Guid uniqueId = Guid.NewGuid();
			CMSNode n = CMSNode.MakeNew(ParentId, _objectType, u.Id, level,Text, uniqueId);

			ContentType.Create(n.Id, Text,"");
	
			return new MemberType(n.Id);
		}

		/// <summary>
		/// Retrieve a list of all MemberTypes
		/// </summary>
		new public static MemberType[] GetAll {
			get
			{
				Guid[] Ids = CMSNode.getAllUniquesFromObjectType(_objectType);

				MemberType[] retVal = new MemberType[Ids.Length];
				for (int i = 0; i  < Ids.Length; i++) retVal[i] = new MemberType(Ids[i]);
				return retVal;
			}
		}

		/// <summary>
		/// Get an true/false if the Member can edit the given data defined in the propertytype
		/// </summary>
		/// <param name="pt">Propertytype to edit</param>
		/// <returns>True if the Member can edit the data</returns>
		public bool MemberCanEdit(propertytype.PropertyType pt) {
			if(propertyTypeRegistered(pt)) {
				return (bool.Parse(SqlHelper.ExecuteScalar(_ConnString,CommandType.Text,"Select memberCanEdit from cmsMemberType where NodeId = " + this.Id +" And propertytypeId = "+pt.Id).ToString()));				   
			}
			return false;
		}	

		/// <summary>
		/// Get a MemberType by it's alias
		/// </summary>
		/// <param name="Alias">The alias of the MemberType</param>
		/// <returns>The MeberType with the given Alias</returns>
		public static new MemberType GetByAlias(string Alias) 
		{
			return new MemberType(int.Parse(SqlHelper.ExecuteScalar(_ConnString, CommandType.Text, "select nodeid from cmsContentType where alias = '" + sqlHelper.safeString(Alias) + "'").ToString()));
		}


		/// <summary>
		/// Get an true/false if the given data defined in the propertytype, should be visible on the members profile page
		/// </summary>
		/// <param name="pt">Propertytype</param>
		/// <returns>True if the data should be displayed on the profilepage</returns>
		public bool ViewOnProfile(propertytype.PropertyType pt) 
		{
			if(propertyTypeRegistered(pt)) 
			{
				return (bool.Parse(SqlHelper.ExecuteScalar(_ConnString,CommandType.Text,"Select viewOnProfile from cmsMemberType where NodeId = " + this.Id +" And propertytypeId = "+pt.Id).ToString()));				   
			}
			return false;
		}
		
		/// <summary>
		/// Set if the member should be able to edit the data defined by its propertytype
		/// </summary>
		/// <param name="pt">PropertyType</param>
		/// <param name="value">True/False if Members of the type shoúld be able to edit the data</param>
		public void setMemberCanEdit(propertytype.PropertyType pt, bool value) {
			int tmpval = 0;
			if (value) tmpval = 1;
			if (propertyTypeRegistered(pt))
				SqlHelper.ExecuteNonQuery(_ConnString,CommandType.Text,"Update cmsMemberType set memberCanEdit = " + tmpval + " where NodeId = " + this.Id +" And propertytypeId = "+pt.Id);
			else
				SqlHelper.ExecuteNonQuery(_ConnString,CommandType.Text,"insert into cmsMemberType (NodeId, propertytypeid, memberCanEdit,viewOnProfile) values ("+this.Id+","+pt.Id+", "+tmpval+",0)");

		}

		private bool propertyTypeRegistered(propertytype.PropertyType pt) {
				return (int.Parse(SqlHelper.ExecuteScalar(_ConnString,CommandType.Text,"Select count(pk) as tmp from cmsMemberType where NodeId = " + this.Id +" And propertytypeId = "+pt.Id).ToString()) > 0);
		}


		/// <summary>
		/// Set if the data should be displayed on members of this type's profilepage
		/// </summary>
		/// <param name="pt">PropertyType</param>
		/// <param name="value">True/False if the data should be displayed</param>
        public void setMemberViewOnProfile(propertytype.PropertyType pt, bool value) 
		{
			int tmpval = 0;
			if (value) tmpval = 1;
			if (propertyTypeRegistered(pt))
				SqlHelper.ExecuteNonQuery(_ConnString,CommandType.Text,"Update cmsMemberType set viewOnProfile = " + tmpval + " where NodeId = " + this.Id +" And propertytypeId = "+pt.Id);
			else
				SqlHelper.ExecuteNonQuery(_ConnString,CommandType.Text,"insert into cmsMemberType (NodeId, propertytypeid, viewOnProfile) values ("+this.Id+","+pt.Id+", "+tmpval+")");
		}

		/// <summary>
		/// Delete the current MemberType.
		/// 
		/// Deletes all Members of the type
		/// 
		/// Use with care
		/// </summary>
		new public void delete() 
		{
			// delete all documents of this type
			Member.DeleteFromType(this);
			// delete membertype specific data
			SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,"Delete from cmsMemberType where nodeId = " + this.Id);
			
			// Delete contentType
			base.delete();
           }


	}
}