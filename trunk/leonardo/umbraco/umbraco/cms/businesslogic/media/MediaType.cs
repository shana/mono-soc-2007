using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace umbraco.cms.businesslogic.media
{
	/// <summary>
	/// The Mediatype
	/// 
	/// Due to the inheritance of the ContentType class,it enables definition of generic datafields on a Media.
	/// </summary>
	public class MediaType : ContentType
	{

		/// <summary>
		/// Constructs a MediaTypeobject given the id
		/// </summary>
		/// <param name="id">Id of the mediatype</param>
		public MediaType(int id) : base(id)
		{
		}

		/// <summary>
		/// Constructs a MediaTypeobject given the id
		/// </summary>
		/// <param name="id">Id of the mediatype</param>
		public MediaType(Guid id) : base(id)
		{
		}

        /// <summary>
        /// Used to persist object changes to the database. In v3.0 it's just a stub for future compatibility
        /// </summary>
        public override void Save()
        {
        }


		/// <summary>
		/// Retrieve a MediaType by it's alias
		/// </summary>
		/// <param name="Alias">The alias of the MediaType</param>
		/// <returns>The MediaType with the alias</returns>
		public static new MediaType GetByAlias(string Alias) 
		{
			return new MediaType(int.Parse(SqlHelper.ExecuteScalar(_ConnString, CommandType.Text, "select nodeid from cmsContentType where alias = '" + sqlHelper.safeString(Alias) + "'").ToString()));
		}


		private static Guid _objectType = new Guid("4ea4382b-2f5a-4c2b-9587-ae9b3cf3602e");

		/// <summary>
		/// Retrieve all MediaTypes in the umbraco installation
		/// </summary>
		new public static MediaType[] GetAll 
		{
			get
			{
				Guid[] Ids = CMSNode.getAllUniquesFromObjectType(_objectType);
				MediaType[] retVal = new MediaType[Ids.Length];
				for (int i = 0; i  < Ids.Length; i++) retVal[i] = new MediaType(Ids[i]);
				return retVal;
			}
		}

		/// <summary>
		/// Create a new Mediatype
		/// </summary>
		/// <param name="u">The Umbraco user context</param>
		/// <param name="Text">The name of the MediaType</param>
		/// <returns>The new MediaType</returns>
		public static MediaType MakeNew( BusinessLogic.User u,string Text) 
		{
		
			int ParentId= -1;
			int level = 1;
			Guid uniqueId = Guid.NewGuid();
			CMSNode n = CMSNode.MakeNew(ParentId, _objectType, u.Id, level,Text, uniqueId);

			ContentType.Create(n.Id, Text,"");

			return new MediaType(n.Id);
		}


		/// <summary>
		/// Deletes the current MediaType and all created Medias of the type.
		/// </summary>
		new public void delete() 
		{
			// delete all documents of this type
			Media.DeleteFromType(this);
			// Delete contentType
			base.delete();
		}
	}
}
