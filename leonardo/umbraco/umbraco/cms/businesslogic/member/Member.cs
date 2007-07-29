using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web;
using System.Web.Caching;
using System.Xml;
using Microsoft.ApplicationBlocks.Data;
using Umbraco.BusinessLogic;
using Umbraco.BusinessLogic;

namespace Umbraco.Cms.BusinessLogic.Member
{
	/// <summary>
	/// The Member class represents a member of the public website (not to be confused with Umbraco users)
	/// 
	/// Members are used when creating communities and collaborative applications using Umbraco, or if there are a 
	/// need for identifying or authentifying the visitor. (extranets, protected/private areas of the public website)
	/// 
	/// Inherits generic datafields from it's baseclass content.
	/// </summary>
	public class Member : Content
	{
		private static readonly Guid _objectType = new Guid("39eb0f98-b348-42a1-8662-e7eb18487560");
		private static readonly Cache _memberCache = HttpRuntime.Cache;

		private static readonly string UmbracoMemberIdCookieKey = "umbracoMemberId";
		private static readonly string UmbracoMemberGuidCookieKey = "umbracoMemberGuid";
		private static readonly string UmbracoMemberLoginCookieKey = "umbracoMemberLogin";
		private string _text;

		private Hashtable _groups = null;

		/// <summary>
		/// Initializes a new instance of the Member class.
		/// </summary>
		/// <param Name="id">Identifier</param>
		public Member(int id) : base(id)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Member class.
		/// </summary>
		/// <param Name="id">Identifier</param>
		public Member(Guid id) : base(id)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Member class, with an option to only initialize 
		/// the data used by the tree in the Umbraco console.
		/// 
		/// Performace
		/// </summary>
		/// <param Name="id">Identifier</param>
		/// <param Name="noSetup"></param>
		public Member(int id, bool noSetup) : base(id, noSetup)
		{
		}

		/// <summary>
		/// The Name of the member
		/// </summary>
		public new string Text
		{
			get
			{
				// TODO: SQL
				if (string.IsNullOrEmpty(_text))
					_text = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text,
					                                "select text from umbracoNode where id = @id",
					                                new SqlParameter("@id", Id)).ToString();
				return _text;
			}
			set
			{
				_text = value;
				base.Text = value;
			}
		}

		/// <summary>
		/// A list of all members in the current Umbraco install
		/// 
		/// Note: is ressource intensive, use with care.
		/// </summary>
		public static Member[] GetAll
		{
			get
			{
				Guid[] tmp = getAllUniquesFromObjectType(_objectType);

				return Array.ConvertAll<Guid, Member>(tmp, delegate(Guid g) { return new Member(g); });
			}
		}

		/// <summary>
		/// The members password, used when logging in on the public website
		/// </summary>
		public string Password
		{
			get
			{
				// TODO: SQL
				return Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
				                               "select Password from cmsMember where nodeId = @id",
				                               new SqlParameter("@id", Id)).ToString();
			}
			set
			{
				// TODO: SQL
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
				                          "update cmsMember set Password = @password where nodeId = @id",
				                          new SqlParameter("@password", value),
				                          new SqlParameter("@id", Id));
			}
		}

		/// <summary>
		/// The loginname of the member, used when logging in
		/// </summary>
		public string LoginName
		{
			get
			{
				// TODO: SQL
				return Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
				                               "select LoginName from cmsMember where nodeId = @id",
				                               new SqlParameter("@id", Id)).ToString();
			}
			set
			{
				// TODO: SQL
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
				                          "update cmsMember set LoginName = @loginName where nodeId =  @id",
				                          new SqlParameter("@loginName", value),
				                          new SqlParameter("@id", Id));
			}
		}

		/// <summary>
		/// A list of groups the member are member of
		/// </summary>
		public Hashtable Groups
		{
			get
			{
				if (_groups == null)
					populateGroups();
				return _groups;
			}
		}

		/// <summary>
		/// The members email
		/// </summary>
		public string Email
		{
			get
			{
				// TODO: SQL
				return Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
				                               "select Email from cmsMember where nodeId = @id",
				                               new SqlParameter("@id", Id)).ToString();
			}
			set
			{
				// TODO: SQL
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
				                          "update cmsMember set Email = @email where nodeId = @id",
				                          new SqlParameter("@id", Id), new SqlParameter("@email", value));
			}
		}

		/// <summary>
		/// Used to persist object changes to the database. In v3.0 it's just a stub for future compatibility
		/// </summary>
		public override void Save()
		{
		}

		/// <summary>
		/// Retrieves a list of members thats not start with a-z
		/// </summary>
		/// <returns>array of members</returns>
		public static Member[] getAllOtherMembers()
		{
			// TODO: SQL
			string query =
				"SELECT id, text FROM umbracoNode WHERE (nodeObjectType = @nodeObjectType) AND (ASCII(SUBSTRING(text, 1, 1)) NOT BETWEEN ASCII('a') AND ASCII('z')) AND (ASCII(SUBSTRING(text, 1, 1)) NOT BETWEEN ASCII('A') AND ASCII('Z'))";
			List<Member> m = new List<Member>();
			using (SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_ConnString, CommandType.Text, query,
			                                                  new SqlParameter("@nodeObjectType", _objectType)))
			{
				while (dr.Read())
				{
					Member newMember = new Member(int.Parse(dr["id"].ToString()), true);
					newMember._text = dr["text"].ToString();
					m.Add(new Member(newMember.Id));
				}
			}

			return m.ToArray();
		}

		/// <summary>
		/// Retrieves a list of members by the first letter in their Name.
		/// </summary>
		/// <param Name="letter">The first letter</param>
		/// <returns></returns>
		public static Member[] getMemberFromFirstLetter(char letter)
		{
			// TODO: SQL
			string query =
				"Select id, text from umbracoNode where NodeObjectType = @objectType and text like @letter order by text";
			List<Member> m = new List<Member>();
			using (SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_ConnString, CommandType.Text, query,
			                                                  new SqlParameter("@objectType", _objectType),
			                                                  new SqlParameter("@letter", letter + "%")))
			{
				while (dr.Read())
				{
					Member newMember = new Member(int.Parse(dr["id"].ToString()), true);
					newMember._text = dr["text"].ToString();
					m.Add(new Member(newMember.Id));
				}
			}
			return m.ToArray();
		}

		/// <summary>
		/// Creates a new member
		/// </summary>
		/// <param Name="Name">Membername</param>
		/// <param Name="mbt">Member type</param>
		/// <param Name="u">The Umbraco usercontext</param>
		/// <returns>The new member</returns>
		public static Member MakeNew(string Name, MemberType mbt, User u)
		{
			Guid newId = Guid.NewGuid();
			MakeNew(-1, _objectType, u.Id, 1, Name, newId);

			Member tmp = new Member(newId);

			tmp.CreateContent(mbt);
			// Create member specific data ..

			// TODO: SQL
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
			                          "insert into cmsMember (nodeId,Email,LoginName,Password) values (@id,'',@text,'')",
			                          new SqlParameter("@id", tmp.Id),
			                          new SqlParameter("@text", tmp.Text));
			return tmp;
		}

		/// <summary>
		/// Creates a new member
		/// </summary>
		/// <param Name="Name">Membername</param>
		/// <param Name="mbt">Member type</param>
		/// <param Name="u">The Umbraco usercontext</param>
		/// <param Name="Email">The email of the user</param>
		/// <returns>The new member</returns>
		public static Member MakeNew(string Name, string Email, MemberType mbt, User u)
		{
			Guid newId = Guid.NewGuid();
			MakeNew(-1, _objectType, u.Id, 1, Name, newId);

			Member tmp = new Member(newId);

			tmp.CreateContent(mbt);
			// Create member specific data ..
			// TODO: SQL
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
			                          "insert into cmsMember (nodeId,Email,LoginName,Password) values (@id,@email,@text,'')",
			                          new SqlParameter("@id", tmp.Id),
			                          new SqlParameter("@text", tmp.Text),
			                          new SqlParameter("@email", Email));
			return tmp;
		}

		/// <summary>
		/// Generates the xmlrepresentation of a member
		/// </summary>
		/// <param Name="xd"></param>
		public override void XmlGenerate(XmlDocument xd)
		{
			XmlNode x = xd.CreateNode(XmlNodeType.Element, "node", "");
			XmlPopulate(xd, ref x, false);
			x.Attributes.Append(XmlHelper.AddAttribute(xd, "loginName", LoginName));
			x.Attributes.Append(XmlHelper.AddAttribute(xd, "email", Email));

			// Save to db
			// TODO: SQL
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text,
			                          @"if exists(select nodeId from cmsContentXml where nodeId = @nodeId) " +
			                          @"update cmsContentXml set xml = @xml where nodeId = @nodeId " +
			                          @"else " +
			                          @"insert into cmsContentXml(nodeId, xml) values (@nodeId, @xml)",
			                          new SqlParameter("@nodeId", Id), new SqlParameter("@xml", x.OuterXml));
		}

		/// <summary>
		/// Xmlrepresentation of a member
		/// </summary>
		/// <param Name="xd">The xmldocument context</param>
		/// <param Name="Deep">Recursive - should always be set to false</param>
		/// <returns>A the xmlrepresentation of the current member</returns>
		public override XmlNode ToXml(XmlDocument xd, bool Deep)
		{
			XmlNode x = base.ToXml(xd, Deep);
			x.Attributes.Append(XmlHelper.AddAttribute(xd, "loginName", LoginName));
			x.Attributes.Append(XmlHelper.AddAttribute(xd, "email", Email));
			return x;
		}

		/// <summary>
		/// Deltes the current member
		/// </summary>
		public new void delete()
		{
			/*
			// delete all content created by this member!
			foreach (Umbraco.Cms.BusinessLogic.console.IIcon d in this.ChildrenOfAllObjectTypes) 
			{
				new cms.Umbraco.Cms.BusinessLogic.contentitem.ContentItem(d.UniqueId).delete();
			}
			*/
			// delete memeberspecific data!
			// TODO: SQL
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text, "Delete from cmsMember where nodeId = @id",
			                          new SqlParameter("@id", Id));

			// Delete all content and cmsnode specific data!
			base.delete();
		}

		/// <summary>
		/// Deletes all members of the membertype specified
		/// 
		/// Used when a membertype is deleted
		/// 
		/// Use with care
		/// </summary>
		/// <param Name="dt">The membertype which are being deleted</param>
		public static void DeleteFromType(MemberType dt)
		{
			foreach (Content c in getContentOfContentType(dt))
			{
				// due to recursive structure document might already been deleted..
				if (IsNode(c.UniqueId))
				{
					Member tmp = new Member(c.UniqueId);
					tmp.delete();
				}
			}
		}

		/// <summary>
		/// Adds the member to group with the specified id
		/// </summary>
		/// <param Name="GroupId">The id of the group which the member is being added to</param>
		public void AddGroup(int GroupId)
		{
			// TODO: SQL
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
			                          "if not exists(select member from cmsMember2MemberGroup where member = @id and memberGroup = @groupId) insert into cmsMember2MemberGroup (member, memberGroup) values (@id, @groupId)",
			                          new SqlParameter("@id", Id), new SqlParameter("@groupId", GroupId));
			populateGroups();
		}

		/// <summary>
		/// Removes the member from the MemberGroup specified
		/// </summary>
		/// <param Name="GroupId">The MemberGroup from which the Member is removed</param>
		public void RemoveGroup(int GroupId)
		{
			// TODO: SQL
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text,
			                          "delete from cmsMember2MemberGroup where member = @id and Membergroup = @groupId",
			                          new SqlParameter("@id", Id), new SqlParameter("@groupId", GroupId));
			populateGroups();
		}

		private void populateGroups()
		{
			Hashtable temp = new Hashtable();
			// TODO: SQL
			using (SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_ConnString, CommandType.Text,
			                                                  "select memberGroup from cmsMember2MemberGroup where member = @id",
			                                                  new SqlParameter("@id", Id)))
			{
				while (dr.Read())
					temp.Add(int.Parse(dr["memberGroup"].ToString()),
					         new MemberGroup(int.Parse(dr["memberGroup"].ToString())));
			}
			_groups = temp;
		}

		/// <summary>
		/// Retrieve a member given the loginname
		/// 
		/// Used when authentifying the Member
		/// </summary>
		/// <param Name="loginName">The unique Loginname</param>
		/// <returns>The member with the specified loginname - null if no Member with the login exists</returns>
		public static Member GetMemberFromLoginName(string loginName)
		{
			// TODO: SQL
			if (IsMember(loginName))
			{
				object o = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
				                                   "select nodeID from cmsMember where LoginName = @loginName",
				                                   new SqlParameter("@loginName", loginName));

				if (o == null)
					return null;

				int tmpId;
				if (!int.TryParse(o.ToString(), out tmpId))
					return null;

				return new Member(tmpId);
			}
			else
				HttpContext.Current.Trace.Warn("No member with loginname: " + loginName + " Exists");

			return null;
		}

		/// <summary>
		/// Retrieve a Member given an email
		/// 
		/// Used when authentifying the Member
		/// </summary>
		/// <param Name="email">The email of the member</param>
		/// <returns>The member with the specified email - null if no Member with the email exists</returns>
		public static Member GetMemberFromEmail(string email)
		{
			// TODO: SQL
			object o = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
			                                   "select nodeID from cmsMember where Email = @email",
			                                   new SqlParameter("@email", email));

			if (o == null)
				return null;

			int tmpId;
			if (!int.TryParse(o.ToString(), out tmpId))
				return null;

			return new Member(tmpId);
		}

		/// <summary>
		/// Retrieve a Member given the credentials
		/// 
		/// Used when authentifying the member
		/// </summary>
		/// <param Name="loginName">Member login</param>
		/// <param Name="password">Member password</param>
		/// <returns>The member with the credentials - null if none exists</returns>
		public static Member GetMemberFromLoginNameAndPassword(string loginName, string password)
		{
			if (IsMember(loginName))
			{
				// TODO: SQL
				object o = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
				                                   "select nodeID from cmsMember where LoginName = @loginName and Password = @password",
				                                   new SqlParameter("loginName", loginName),
				                                   new SqlParameter("@password", password));

				if (o == null)
					return null;

				int tmpId;
				if (!int.TryParse(o.ToString(), out tmpId))
					return null;

				return new Member(tmpId);
			}
			else
			{
				HttpContext.Current.Trace.Warn("No member with loginname: " + loginName + " Exists");
				//				throw new ArgumentException("No member with Loginname: " + LoginName + " exists");
				return null;
			}
		}

		/// <summary>
		/// Helper method - checks if a Member with the LoginName exists
		/// </summary>
		/// <param Name="loginName">Member login</param>
		/// <returns>True if the member exists</returns>
		public static bool IsMember(string loginName)
		{
			// TODO: SQL
			Debug.Assert(loginName != null, "loginName cannot be null");
			object o = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(_ConnString, CommandType.Text,
			                                   "select count(nodeID) as tmp from cmsMember where LoginName = @loginName",
			                                   new SqlParameter("@loginName", loginName));
			if (o == null)
				return false;
			int count;
			if (!int.TryParse(o.ToString(), out count))
				return false;
			return count > 0;
		}

		/*
		public contentitem.ContentItem[] CreatedContent() {
			return new contentitem.ContentItem[0];
		}
		*/

		#region MemberHandle functions

		/// <summary>
		/// Method is used when logging a member in.
		/// 
		/// Adds the member to the cache of logged in members
		/// 
		/// Uses cookiebased recognition
		/// 
		/// Can be used in the runtime
		/// </summary>
		/// <param Name="m">The member to log in</param>
		public static void AddMemberToCache(Member m)
		{
			if (m != null)
			{
				// TODO: Hard coded
				Hashtable umbracoMembers = CachedMembers();

				// Check if member already exists
				if (umbracoMembers[m.Id] == null)
					umbracoMembers.Add(m.Id, m);

				removeCookie("umbracoMemberId");

				// Add cookie with member-id, guid and loginname
				addCookie("umbracoMemberId", m.Id.ToString(), 365);
				addCookie("umbracoMemberGuid", m.UniqueId.ToString(), 365);
				addCookie("umbracoMemberLogin", m.LoginName, 365);

				// Debug information
				HttpContext.Current.Trace.Write("member",
				                                "Member added to cache: " + m.Text + "/" + m.LoginName + " (" +
				                                m.Id + ")");

				_memberCache["umbracoMembers"] = umbracoMembers;
			}
		}

		#region cookieHelperMethods

		private static void removeCookie(string Name)
		{
			HttpCookie c = HttpContext.Current.Request.Cookies[Name];
			if (c != null)
			{
				c.Expires = DateTime.Now.AddDays(-1);
				HttpContext.Current.Response.Cookies.Add(c);
			}
		}

		private static void addCookie(string Name, object Value, int NumberOfDaysToLast)
		{
			HttpCookie c = new HttpCookie(Name, Value.ToString());
			c.Value = Value.ToString();
			c.Expires = DateTime.Now.AddDays(NumberOfDaysToLast);
			HttpContext.Current.Response.Cookies.Add(c);
		}

		private static void addCookie(string Name, object Value, TimeSpan timeout)
		{
			HttpCookie c = new HttpCookie(Name, Value.ToString());
			c.Value = Value.ToString();
			c.Expires = DateTime.Now.Add(timeout);
			HttpContext.Current.Response.Cookies.Add(c);
		}

		private static string getCookieValue(string Name)
		{
			string tempValue = "";

			if (HttpContext.Current.Session[Name] != null)
				if (HttpContext.Current.Session[Name].ToString() != "0")
					tempValue = HttpContext.Current.Session[Name].ToString();

			if (tempValue == "")
			{
				if (Array.IndexOf(HttpContext.Current.Response.Cookies.AllKeys, Name) == -1)
				{
					if (HttpContext.Current.Request.Cookies[Name] != null)
						if (HttpContext.Current.Request.Cookies[Name].Value != "")
						{
							tempValue = HttpContext.Current.Request.Cookies[Name].Value;
						}
				}
				else
				{
					tempValue = HttpContext.Current.Response.Cookies[Name].Value;
				}
			}

			return tempValue;
		}

		#endregion

		/// <summary>
		/// Method is used when logging a member in.
		/// 
		/// Adds the member to the cache of logged in members
		/// 
		/// Uses cookie or session based recognition
		/// 
		/// Can be used in the runtime
		/// </summary>
		/// <param Name="m">The member to log in</param>
		/// <param Name="UseSession">Use sessionbased recognition</param>
		/// <param Name="TimespanForCookie">The live time of the cookie</param>
		public static void AddMemberToCache(Member m, bool UseSession, TimeSpan TimespanForCookie)
		{
			if (m != null)
			{
				Hashtable umbracoMembers = CachedMembers();

				// Check if member already exists
				if (umbracoMembers[m.Id] == null)
					umbracoMembers.Add(m.Id, m);

				if (!UseSession)
				{
					removeCookie("umbracoMemberId");

					// Add cookie with member-id
					addCookie("umbracoMemberId", m.Id.ToString(), TimespanForCookie);
					addCookie("umbracoMemberGuid", m.UniqueId.ToString(), TimespanForCookie);
					addCookie("umbracoMemberLogin", m.LoginName, TimespanForCookie);
				}
				else
				{
					HttpContext.Current.Session["umbracoMemberId"] = m.Id.ToString();
					HttpContext.Current.Session["umbracoMemberGuid"] = m.UniqueId.ToString();
					HttpContext.Current.Session["umbracoMemberLogin"] = m.LoginName;
				}

				// Debug information
				HttpContext.Current.Trace.Write("member",
				                                string.Format("Member added to cache: {0}/{1} ({2})",
				                                              m.Text, m.LoginName, m.Id));

				_memberCache["umbracoMembers"] = umbracoMembers;
			}
		}

		/// <summary>
		/// Removes the member from the cache
		/// 
		/// Can be used in the public website
		/// </summary>
		/// <param Name="m">Member to remove</param>
		public static void RemoveMemberFromCache(Member m)
		{
			Hashtable umbracoMembers = CachedMembers();
			if (umbracoMembers.ContainsKey(m.Id))
				umbracoMembers.Remove(m.Id);

			_memberCache["umbracoMembers"] = umbracoMembers;
		}

		/// <summary>
		/// Deletes the member cookie from the browser 
		/// 
		/// Can be used in the public website
		/// </summary>
		/// <param Name="m">Member</param>
		public static void ClearMemberFromClient(Member m)
		{
			removeCookie("umbracoMemberId");
			removeCookie("umbracoMemberGuid");
			removeCookie("umbracoMemberLogin");

			RemoveMemberFromCache(m);
			HttpContext.Current.Trace.Write("member", "Member removed from client");
		}

		/// <summary>
		/// Retrieve a collection of members in the cache
		/// 
		/// Can be used from the public website
		/// </summary>
		/// <returns>A collection of cached members</returns>
		public static Hashtable CachedMembers()
		{
			Hashtable umbracoMembers;

			// Check for member hashtable in cache
			if (_memberCache["umbracoMembers"] == null)
				umbracoMembers = new Hashtable();
			else
				umbracoMembers = (Hashtable) _memberCache["umbracoMembers"];

			return umbracoMembers;
		}

		/// <summary>
		/// Retrieve a member from the cache
		/// 
		/// Can be used from the public website
		/// </summary>
		/// <param Name="id">Id of the member</param>
		/// <returns>If the member is cached it returns the member - else null</returns>
		public static Member GetMemberFromCache(int id)
		{
			Hashtable members = CachedMembers();
			if (members.ContainsKey(id))
				return (Member) members[id];
			else
				return null;
		}

		/// <summary>
		/// An indication if the current visitor is logged in
		/// 
		/// Can be used from the public website
		/// </summary>
		/// <returns>True if the the current visitor is logged in</returns>
		public static bool IsLoggedOn()
		{
			bool _isMember = false;
			if (CurrentMemberId() != 0)
				_isMember = true;

			return _isMember;
		}

		/// <summary>
		/// Gets the current visitors memberid
		/// </summary>
		/// <returns>The current visitors members id, if the visitor is not logged in it returns 0</returns>
		public static int CurrentMemberId()
		{
			int _currentMemberId = 0;

			if (StateHelper.HasCookieValue(UmbracoMemberIdCookieKey) &&
			    StateHelper.HasCookieValue(UmbracoMemberGuidCookieKey) &&
			    StateHelper.HasCookieValue(UmbracoMemberLoginCookieKey))
			{
				int.TryParse(StateHelper.GetCookieValue(UmbracoMemberIdCookieKey), out _currentMemberId);
			}

			return _currentMemberId;
		}

		/// <summary>
		/// Get the current member
		/// </summary>
		/// <returns>Returns the member, if visitor is not logged in: null</returns>
		public static Member GetCurrentMember()
		{
			try
			{
				int _currentMemberId = CurrentMemberId();
				if (_currentMemberId != 0)
				{
					// return member from cache
					Member m = GetMemberFromCache(_currentMemberId);
					if (m == null)
						m = new Member(_currentMemberId);

					if (m.UniqueId == new Guid(getCookieValue("umbracoMemberGuid")) &&
					    m.LoginName == getCookieValue("umbracoMemberLogin"))
						return m;

					return null;
				}
				else
					return null;
			}
			catch
			{
				return null;
			}
		}

		#endregion
	}
}