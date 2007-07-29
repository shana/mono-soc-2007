using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using System.Xml;
using SqlHelper=Umbraco.SqlHelper;

namespace Umbraco.Cms.BusinessLogic
{
	/// <summary>
	/// CMSNode class serves as the baseclass for many of the other components in the cms.Umbraco.Cms.BusinessLogic.xx namespaces.
	/// Providing the basic hierarchical datastructure and properties Text (Name), Creator, Createdate, updatedate etc.
	/// which are shared by most Umbraco objects.
	/// 
	/// The childclass'es are required to implement an identifier (Guid) which is used as the objecttype identifier, for 
	/// distinguishing the different types of CMSNodes (ex. Documents/Medias/Stylesheets/documenttypes and so forth).
	/// </summary>
	public class CMSNode : BusinessLogic.console.IconI
	{
		private string _text;
		private int _id = 0;
		private Guid _uniqueid;
		protected static readonly string _ConnString = GlobalSettings.DbDSN;
		private int _parentid;
		private Guid _nodeobjecttype;
		private int _level;
		private string _path;
		private bool _hasChildren;
		private int _sortOrder;
		private int _userId;
		private DateTime _createDate;
	    private bool _hasChildrenInitialized;
				
		/// <summary>
		/// 
		/// </summary>
		/// <param Name="Id">Identifier</param>
		public CMSNode(int Id) 
		{
			_id = Id;
			setupNode();
		}

		/// <summary>
		/// </summary>
		/// <param Name="id">Identifier</param>
		/// <param Name="noSetup">Not implemented</param>
		public CMSNode(int id, bool noSetup) 
		{
			_id = id;
		}

		/// <summary>
		/// </summary>
		/// <param Name="uniqueId">Identifier</param>
		public CMSNode(Guid uniqueId) {
			_id = int.Parse(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString,CommandType.Text,"Select id from umbracoNode where uniqueID = '" + uniqueId + "'").ToString());
			setupNode();
		}

        /// <summary>
        /// Used to persist object changes to the database. In v3.0 it's just a stub for future compatibility
        /// </summary>
        public virtual void Save()
        {
        }


		/// <summary>
		/// Sets up the internal data of the CMSNode, used by the various contructors
		/// </summary>
		protected void setupNode() {
			SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(
				_ConnString,
				CommandType.Text ,
				"Select createDate, trashed, parentId, nodeObjectType, nodeUser, level, path, sortOrder, uniqueId, text from UmbracoNode where id = " + this.Id
				);

            if (dr.Read())
            {
                // testing purposes only > original Umbraco data hasnt any unique values ;)
                // And we need to have a parent in order to create a new node ..
                // Should automaticly add an unique value if no exists (or throw a decent exception)
                if (dr["uniqueId"] == DBNull.Value) _uniqueid = Guid.NewGuid();
                else _uniqueid = new Guid(dr["uniqueId"].ToString());

                _nodeobjecttype = new Guid(dr["nodeObjectType"].ToString());
                _level = int.Parse(dr["level"].ToString());
                _path = dr["path"].ToString();
                _parentid = int.Parse(dr["parentId"].ToString());
                _text = dr["text"].ToString();
                _sortOrder = int.Parse(dr["sortOrder"].ToString());
                _userId = int.Parse(dr["nodeUser"].ToString());
                _createDate = DateTime.Parse(dr["createDate"].ToString());
            }
            else
                throw new ArgumentException(string.Format("No node exists with id '{0}'", Id));

			dr.Close();
		}

        protected void SetupNodeForTree(Guid UniqueId, Guid NodeObjectType, int Level, int ParentId, int UserId, string Path, string Text,
            DateTime CreateDate, bool hasChildren)
        {
            _uniqueid = UniqueId;
            _nodeobjecttype = NodeObjectType;
            _level = Level;
            _parentid = ParentId;
            _userId = UserId;
            _path = Path;
            _text = Text;
            _createDate = CreateDate;
            HasChildren = hasChildren;
        }

		/// <summary>
		/// CMSNode sortorder key
		/// </summary>
		public int sortOrder {
			get{return _sortOrder;}
			set{_sortOrder = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text, "update umbracoNode set sortOrder = '" + value + "' where id = " + this.Id.ToString());
				}
		}

		/// <summary>
		/// Create DateTime
		/// </summary>
		public DateTime CreateDateTime {
			get{return _createDate;}
			set {
				_createDate = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text, "update umbracoNode set createDate = @createDate where id = " + this.Id.ToString(), new SqlParameter("@createDate", _createDate));
			}
		}

		/// <summary>
		/// The creator
		/// </summary>
		public BusinessLogic.User User {
			get {
				return BusinessLogic.User.GetUser(_userId);
			}
		}

		/// <summary>
		/// Method for checking if a CMSNode exits with the given Guid
		/// </summary>
		/// <param Name="UniqueId">Identifier</param>
		/// <returns>True if there is a CMSNode with the given Guid</returns>
		static public bool IsNode(Guid UniqueId) {
			return (int.Parse(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text, "select count(id) from umbracoNode where uniqueid = '" + UniqueId + "'").ToString()) > 0);
		}

		/// <summary>
		/// Method for checking if a CMSNode exits with the given id
		/// </summary>
		/// <param Name="Id">Identifier</param>
		/// <returns>True if there is a CMSNode with the given id</returns>

		static public bool IsNode(int Id) 
		{
			return (int.Parse(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text, "select count(id) from umbracoNode where id = '" + Id + "'").ToString()) > 0);
		}


		/// <summary>
		/// Identifier
		/// </summary>
        public int Id 
		{
			get{return _id;}
		}

		/// <summary>
		/// Given the hierarchical tree structure a CMSNode has only one parent but can have many children 
		/// </summary>
		public CMSNode Parent {
			get{
				if (Level == 1) throw new ArgumentException("No parentnode");
				return new CMSNode(_parentid);
			}
			set 
			{
				_parentid = value.Id;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text, "update umbracoNode set parentId = " + value.Id.ToString() + " where id = " + this.Id.ToString());
			}
		}

		#region IIcon members

		// Unique identifier of the given node
		/// <summary>
		/// Unique identifier of the CMSNode, used when locating data.
		/// </summary>
		public Guid UniqueId 
		{
			get {return _uniqueid;}
		}

		/// <summary>
		/// Humanreadable Name/label
		/// </summary>
		public virtual string Text {
			get {return _text;}
			set {
				_text = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString,CommandType.Text,"Update umbracoNode set text = N'" + Umbraco.SqlHelper.SafeString(value) + "' where id = " + this.Id.ToString());
			}
		}

		/// <summary>
		/// The menuitems used in the treeview
		/// </summary>
		public virtual BusinessLogic.console.MenuItemI[] MenuItems {
			get {return new BusinessLogic.console.MenuItemI[0];}
		}

		/// <summary>
		/// Not implemented, always returns "about:blank"
		/// </summary>
		public virtual string DefaultEditorURL {
			get{return "about:blank";}
		}

		/// <summary>
		/// The icon in the tree
		/// </summary>
		public virtual string Image {
			get{return "";}

		}

		/// <summary>
		/// The "open/active" icon in the tree
		/// </summary>
		public virtual string OpenImage  {
			get{return "";}
		}

		#endregion


		/// <summary>
		/// An comma separeted string consisting of integer node id's
		/// that indicates the path from the topmost node to the given node
		/// </summary>
		public string Path 
		{
			get {return _path;}
			set {
				_path = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text, "update umbracoNode set path = '" + _path + "' where id = " + this.Id.ToString());
				}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param Name="Path"></param>
		protected void UpdateTempPathForTree(string Path) 
		{
			this._path = Path;
		}

		

		/// <summary>
		/// Moves the CMSNode from the current position in the hierarchicy to the target
		/// </summary>
		/// <param Name="NewParentId">Target CMSNode id</param>
		public void Move(int NewParentId)
		{
			int maxSortOrder = int.Parse(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(
				GlobalSettings.DbDSN, 
				CommandType.Text, 
				"select isnull(max(sortOrder),0) from umbracoNode where parentid = @parentId",
				new SqlParameter("@parentId", NewParentId)).ToString());

			CMSNode n = new CMSNode(NewParentId);
			this.Parent = n;
			this.Level = n.Level+1;
			this.Path = n.Path + "," + this.Id.ToString();

			this.sortOrder = maxSortOrder+1;
			
		
			// Nasty hack - this will be fixed when the
			// application server part is done in the Umbraco.Umbraco.Cms.BusinessLogic
            // project.
			if (n.nodeObjectType == web.Document._objectType)
				new Umbraco.Cms.BusinessLogic.web.Document(n.Id).XmlGenerate(new XmlDocument());
			else if (n.nodeObjectType == media.Media._objectType) 
				new Umbraco.Cms.BusinessLogic.media.Media(n.Id).XmlGenerate(new XmlDocument());

			foreach (CMSNode c in this.Children)
				c.Move(this.Id);
		}

		

		/// <summary>
		/// Returns an integer value that indicates in which level of the
		/// treestructure the given node is
		/// </summary>
		public int Level 
		{
			get {return _level;}
			set 
			{
				_level = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text, "update umbracoNode set level = " + _level.ToString() + " where id = " + this.Id.ToString());
			}
		}

		/// <summary>
		/// All CMSNodes has an objecttype ie. Webpage, StyleSheet etc., used to distinuish between the different
		/// objecttypes for for fast loading children to the tree.
		/// </summary>
		protected Guid nodeObjectType 
		{
			get{return _nodeobjecttype;}
		}

		/// <summary>
		/// Besides the hierarchy it's possible to relate one CMSNode to another, use this for alternative
		/// nonstrict hierarchy
		/// </summary>
		public relation.Relation[] Relations 
		{
			get {return relation.Relation.GetRelations(this.Id);}
		}

		/// <summary>
		/// Does the current CMSNode have any childnodes.
		/// </summary>
		public virtual bool HasChildren {
			get
			{
			    if (!_hasChildrenInitialized)
			    {
                    int tmpChildrenCount = int.Parse(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text, "select count(id) from umbracoNode where ParentId = " + _id).ToString());
                    HasChildren = (tmpChildrenCount > 0);
                }
			    return _hasChildren;
			}
			set
			{
			    _hasChildrenInitialized = true;
			    _hasChildren = value;
			}
		}

		/// <summary>
		/// The basic recursive tree pattern
		/// </summary>
		public virtual BusinessLogic.console.IconI[] Children {
				get {
					System.Collections.ArrayList tmp = new System.Collections.ArrayList();
					SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_ConnString,CommandType.Text,"select id from umbracoNode where ParentID = " + this.Id + " And NodeObjectType = '" + this.nodeObjectType.ToString() + "' order by sortOrder");
					
					while(dr.Read()) 
						tmp.Add(dr["Id"]);
					
					dr.Close();

					CMSNode[] retval = new CMSNode[tmp.Count];
					
					for (int i = 0; i < tmp.Count; i ++) 
						retval[i] = new CMSNode((int) tmp[i]);
				
					return retval;}
		}

		/// <summary>
		/// Retrieve all CMSNodes in the Umbraco installation
		/// 
		/// Use with care.
		/// </summary>
		public BusinessLogic.console.IconI[] ChildrenOfAllObjectTypes
		{
			get 
			{
				System.Collections.ArrayList tmp = new System.Collections.ArrayList();
				SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_ConnString,CommandType.Text,"select id from umbracoNode where ParentID = " + this.Id + " order by sortOrder");
					
				while(dr.Read()) 
					tmp.Add(dr["Id"]);
					
				dr.Close();

				CMSNode[] retval = new CMSNode[tmp.Count];
					
				for (int i = 0; i < tmp.Count; i ++) 
					retval[i] = new CMSNode((int) tmp[i]);
				
				return retval;}
		}

		/// <summary>
		/// Retrieves the toplevel nodes in the hierarchy
		/// </summary>
		/// <param Name="ObjectType">The Guid identifier of the type of objects</param>
		/// <returns>A list of all toplevel nodes given the objecttype</returns>
		protected static Guid[] TopMostNodeIds(Guid ObjectType) {
			SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_ConnString,CommandType.Text,"Select uniqueId from umbracoNode where NodeObjectType = '"+ ObjectType +"' And parentId = -1 order by sortOrder");
			System.Collections.ArrayList tmp = new System.Collections.ArrayList();

			while(dr.Read()) tmp.Add(new Guid(dr["uniqueId"].ToString()));
			dr.Close();

			Guid[] retval = new Guid[tmp.Count];
			for(int i= 0; i < tmp.Count; i++) retval[i] = (Guid) tmp[i];
			return retval;
		}

		/// <summary>
		///  Given the protected modifier the CMSNode.MakeNew method can only be accessed by
		//	 derrived classes > who by definition knows of its own objectType.
		/// </summary>
		/// <param Name="parentId">The parent CMSNode id</param>
		/// <param Name="objectType">The objecttype identifier</param>
		/// <param Name="userId">Creator</param>
		/// <param Name="level">The level in the tree hieararchy</param>
		/// <param Name="text">The Name of the CMSNode</param>
		/// <param Name="uniqueId">The unique identifier</param>
		/// <returns></returns>
		protected static CMSNode MakeNew(int parentId, Guid objectType, int userId, int level, string text, Guid uniqueId) 
		{
			CMSNode parent;
			string path = "";
			int sortOrder = 0;

			if (level > 0) 
			{
				parent = new CMSNode(parentId);
				sortOrder = parent.Children.Length+1;
				path = parent.Path;
			} else
				path = "-1";

			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(
				_ConnString,
				 CommandType.Text ,
				"Insert into UmbracoNode " +
				"(trashed, parentID, nodeObjectType, nodeUser, level, path, sortOrder, uniqueId, text) values" +
				"(0, " + parentId + ", '"+ objectType +"', " + userId  + "," + (level++).ToString() + ", '" + path + "'," + sortOrder + ",'" + uniqueId.ToString() + "', N'" + SqlHelper.SafeString(text) + "')"
                );

			CMSNode retVal = new CMSNode(uniqueId);
			retVal.Path = path + "," + retVal.Id.ToString();
			return retVal;
		}

		/// <summary>
		/// Retrieve a list of the unique id's of all CMSNodes given the objecttype
		/// </summary>
		/// <param Name="objectType">The objecttype identifier</param>
		/// <returns>A list of all unique identifiers which each are associated to a CMSNode</returns>
		public static Guid[] getAllUniquesFromObjectType(Guid objectType) {
			SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_ConnString,CommandType.Text,"Select uniqueId from umbracoNode where NodeObjectType = '"+ objectType +"'");
			System.Collections.ArrayList tmp = new System.Collections.ArrayList();

			while(dr.Read()) tmp.Add(new Guid(dr["uniqueId"].ToString()));
			dr.Close();

			Guid[] retval = new Guid[tmp.Count];
			for(int i= 0; i < tmp.Count; i++) retval[i] = (Guid) tmp[i];
			return retval;
			}
		

		/// <summary>
		/// Retrieve a list of the id's of all CMSNodes given the objecttype and the firstletter of the Name.
		/// </summary>
		/// <param Name="objectType">The objecttype identifier</param>
		/// <param Name="letter">Firstletter</param>
		/// <returns>A list of all CMSNodes which has the objecttype and a Name that starts with the given letter</returns>
		protected static int[] getUniquesFromObjectTypeAndFirstLetter(Guid objectType, char letter) {
			using (SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_ConnString, CommandType.Text, "Select id from umbracoNode where NodeObjectType = @objectType AND text like @letter", new SqlParameter("@objectType", objectType), new SqlParameter("@letter", letter.ToString() + "%")))
			{
				List<int> tmp = new List<int>();
				while(dr.Read()) tmp.Add((int)dr[0]);
				return tmp.ToArray();
			}
		}
		
		/// <summary>
		/// Deletes the current CMSNode
		/// </summary>
		public void delete() 
		{
			index.Indexer.RemoveNode(this.Id);
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text ,"Delete from umbracoNode where uniqueID = '" + this.UniqueId + "'");
		}

		/// <summary>
		/// Get a count on all CMSNodes given the objecttype
		/// </summary>
		/// <param Name="objectType">The objecttype identifier</param>
		/// <returns>The number of CMSNodes of the given objecttype</returns>
		public static int CountByObjectType(Guid objectType) 
		{
			return int.Parse(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text, "select count(*) from umbracoNode where nodeObjectType = '" + objectType + "'").ToString());
		}

		/// <summary>
		/// Number of children of the current CMSNode
		/// </summary>
		/// <param Name="Id">The CMSNode Id</param>
		/// <returns>The number of children from the given CMSNode</returns>
		public static int CountSubs(int Id) 
		{
			return int.Parse(Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text, "select count(*) from umbracoNOde where ','+path+',' like '%," + Id.ToString() + ",%'").ToString());
		}

		/// <summary>
		/// An xmlrepresentation of the CMSNOde
		/// </summary>
		/// <param Name="xd">Xmldocument context</param>
		/// <param Name="Deep">If true the xml will append the CMSNodes child xml</param>
		/// <returns>The CMSNode Xmlrepresentation</returns>
		public virtual XmlNode ToXml(XmlDocument xd, bool Deep) 
		{
			XmlNode x = xd.CreateNode(XmlNodeType.Element, "node", "");
			XmlPopulate(xd, x, Deep);
			return x;
		}

		private void XmlPopulate(XmlDocument xd, XmlNode x, bool Deep) 
		{
			// attributes
			x.Attributes.Append(XmlHelper.addAttribute(xd, "id", this.Id.ToString()));
			if (this.Level > 1)
				x.Attributes.Append(XmlHelper.addAttribute(xd, "parentID", this.Parent.Id.ToString()));
			else
				x.Attributes.Append(XmlHelper.addAttribute(xd, "parentID", "-1"));
			x.Attributes.Append(XmlHelper.addAttribute(xd, "level", this.Level.ToString()));
			x.Attributes.Append(XmlHelper.addAttribute(xd, "writerID", this.User.Id.ToString()));
			x.Attributes.Append(XmlHelper.addAttribute(xd, "sortOrder", this.sortOrder.ToString()));
			x.Attributes.Append(XmlHelper.addAttribute(xd, "createDate", this.CreateDateTime.ToString("s")));
			x.Attributes.Append(XmlHelper.addAttribute(xd, "nodeName", this.Text));
			x.Attributes.Append(XmlHelper.addAttribute(xd, "path", this.Path));

			if (Deep) 
			{
				foreach(Content c in this.Children)
					x.AppendChild(c.ToXml(xd, true));
			}
		}
	}
}