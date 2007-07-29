using System;

namespace Umbraco.Cms.BusinessLogic.media
{
	/// <summary>
	/// A media represents a physical file and metadata on the file.
	///  
	/// By inheriting the Content class it has a generic datafields which enables custumization
	/// </summary>
	public class Media : Content
	{
		/// <summary>
		/// Contructs a media object given the Id
		/// </summary>
		/// <param Name="id">Identifier</param>
		public Media(int id) : base(id)
		{
		}

		/// <summary>
		/// Contructs a media object given the Id
		/// </summary>
		/// <param Name="id">Identifier</param>
		public Media(Guid id) : base(id)
		{}


        /// <summary>
        /// Used to persist object changes to the database. In v3.0 it's just a stub for future compatibility
        /// </summary>
        public override void Save()
        {
        }


		/// <summary>
		/// -
		/// </summary>
		public static Guid _objectType = new Guid("b796f64c-1f99-4ffb-b886-4bf4bc011a9c");

		/// <summary>
		/// Creates a new Media
		/// </summary>
		/// <param Name="Name">The Name of the media</param>
		/// <param Name="dct">The type of the media</param>
		/// <param Name="u">The user creating the media</param>
		/// <param Name="ParentId">The id of the folder under which the media is created</param>
		/// <returns></returns>
		public static Media MakeNew(string Name, MediaType dct, BusinessLogic.User u, int ParentId) 
		{			
			Guid newId = Guid.NewGuid();
			// Updated to match level from base node
			CMSNode n = new CMSNode(ParentId);
			int newLevel = n.Level;
			newLevel++;
			CMSNode.MakeNew(ParentId,_objectType, u.Id, newLevel,  Name, newId);
			Media tmp = new Media(newId);
			tmp.CreateContent(dct);
			return tmp;
		}

		/// <summary>
		/// Retrieve a list of all toplevel medias and folders
		/// </summary>
		/// <returns></returns>
		public static Media[] GetRootMedias() 
		{
			Guid[] topNodeIds = CMSNode.TopMostNodeIds(_objectType);
			
			Media[] retval = new Media[topNodeIds.Length];
			for (int i = 0;i < topNodeIds.Length;i++) 
			{
				Media d = new Media(topNodeIds[i]);
				retval[i] = d;
			}
			return retval;
		}


		/// <summary>
		/// Retrieve a list of all medias underneath the current
		/// </summary>
		new public Media[] Children 
		{
			get
			{
				BusinessLogic.console.IconI[] tmp = base.Children;
				Media[] retval = new Media[tmp.Length];
				for (int i = 0; i < tmp.Length; i++) retval[i] = new Media(tmp[i].UniqueId);
				return retval;
			}
		}

		/// <summary>
		/// Deletes all medias of the given type, used when deleting a mediatype
		/// 
		/// Use with care.
		/// </summary>
		/// <param Name="dt"></param>
		public static void DeleteFromType(MediaType dt) 
		{
			foreach (Content c in Media.getContentOfContentType(dt)) 
			{
				// due to recursive structure document might already been deleted..
				if (CMSNode.IsNode(c.UniqueId)) 
				{
					Media tmp = new Media(c.UniqueId);
					tmp.delete();
				}
			}
		}
		/// <summary>
		/// Deletes the current media and all children.
		/// </summary>
		new public void delete() 
		{
			foreach (Media d in this.Children) 
			{
				d.delete();
			}
			base.delete();
		}
	}
}
