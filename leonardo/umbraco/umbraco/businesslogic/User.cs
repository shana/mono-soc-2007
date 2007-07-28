using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using Microsoft.ApplicationBlocks.Data;

namespace umbraco.BusinessLogic
{
	/// <summary>
	/// Summary description for User.
	/// </summary>
	public class User
	{
		// private SqlConnection _conn = new SqlConnection(GlobalSettings.DbDSN);


		private int _id;
	    private bool _isInitialized;
		private string _name;
		private string _loginname;
		private string _password;
		private int _startnodeid;
		private int _startmediaid;
		private string _email;
		private string _language = "";
		private UserType _usertype;
		private bool _userNoConsole;
		private bool _userDisabled;

		private Hashtable _cruds = new Hashtable();
		private bool _crudsInitialized = false;

		private Hashtable _notifications = new Hashtable();
		private bool _notificationsInitialized = false;

		public User(int ID)
		{
			setupUser(ID);
		}

        public User(int ID, bool noSetup)
        {
            _id = ID;
        }

	    public User(string Login, string Password)
		{			
			setupUser(getUserId(Login, Password));
		}

		public User(string Login) 
		{
			setupUser(getUserId(Login));
		}

		private void setupUser(int ID)
		{
			_id = ID;

			using (SqlDataReader dr = SqlHelper.ExecuteReader(
				GlobalSettings.DbDSN, CommandType.Text,
				"Select userNoConsole, userDisabled, userType,startStructureID, startMediaId, userName,userLogin,userPassword,userEmail,userDefaultPermissions, userLanguage from umbracoUser where id = @id",
				new SqlParameter("@id", ID)))
			{
				if(dr.Read())
				{
					_userNoConsole = bool.Parse(dr["usernoconsole"].ToString());
					_userDisabled = bool.Parse(dr["userDisabled"].ToString());
					_name = dr["userName"].ToString();
					_loginname = dr["userLogin"].ToString();
					_password = dr["userPassword"].ToString();
					_email = dr["userEmail"].ToString();
					_language = dr["userLanguage"].ToString();
					_startnodeid = int.Parse(dr["startStructureID"].ToString());
					if(!dr.IsDBNull(dr.GetOrdinal("startMediaId")))
						_startmediaid = int.Parse(dr["startMediaID"].ToString());
					_usertype = UserType.GetUserType(int.Parse(dr["UserType"].ToString()));
				}
			}
			_isInitialized = true;
		}

        /// <summary>
        /// Used to persist object changes to the database. In v3.0 it's just a stub for future compatibility
        /// </summary>
        public void Save()
        {
        }

		public string Name 
		{
			get
			{
                if (!_isInitialized)
                    setupUser(_id);
			    return _name;
			}
			set 
			{
				_name = value;
				SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text,"Update umbracoUser set UserName = @userName where id = @id", new SqlParameter("@userName", value), new SqlParameter("@id", Id));
                FlushFromCache();
            }
		}

		public string Email
		{
			get
			{
                if (!_isInitialized)
                    setupUser(_id);
                return _email;
			}
			set 
			{
				_email = value;
				SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text,"Update umbracoUser set UserEmail = @email where id = @id", new SqlParameter("@id", this.Id), new SqlParameter("@email", value));
                FlushFromCache();
            }
		}

		public string Language 
		{
			get
			{
                if (!_isInitialized)
                    setupUser(_id);
                return _language;
			}
			set {
				_language = value;
				SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text,"Update umbracoUser set userLanguage = @language where id = @id", new SqlParameter("@language", value), new SqlParameter("@id", Id));
                FlushFromCache();
            }
	}

		public string Password 
		{
			get
			{
                if (!_isInitialized)
                    setupUser(_id);
                return _password;
			}
			set {
				_password = value;
				SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text,"Update umbracoUser set UserPassword = @pw where id = @id", new SqlParameter("@pw", value), new SqlParameter("@id", Id));
                FlushFromCache();
            }
		}
		static string  _connstring = GlobalSettings.DbDSN;
		
		public Application[] Applications 
		{
			get{
                if (!_isInitialized)
                    setupUser(_id);
                ArrayList al = new ArrayList();

				using (SqlDataReader appIcons = SqlHelper.ExecuteReader(_connstring,
					CommandType.Text, "select appAlias, appIcon, appname from umbracoApp app join umbracoUser2app u2a on u2a.app = app.appAlias and u2a.[user] = @userID order by app.sortOrder", new SqlParameter("@userID", this.Id)))
				{
					while(appIcons.Read())
					{
						Application tmp = new Application();
						tmp.name = appIcons.GetString(appIcons.GetOrdinal("appName"));
						tmp.icon = appIcons.GetString(appIcons.GetOrdinal("appIcon"));
						tmp.alias = appIcons.GetString(appIcons.GetOrdinal("appAlias"));
						al.Add(tmp);
					}
				}
				
				Application[] retVal = new Application[al.Count];

				for (int i = 0;i < al.Count;i++) {
					retVal[i] = (Application) al[i];
                  }
				return retVal;
			}
		}

		public string LoginName {
			get
			{
                if (!_isInitialized)
                    setupUser(_id);
                return _loginname;
			}
			set {
				_loginname = value;
				SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text,"Update umbracoUser set UserLogin = @login where id = @id", new SqlParameter("@login", value), new SqlParameter("@id", Id));
                FlushFromCache();
            }
		}

		public static bool validateCredentials(string lname, string passw) {
            return validateCredentials(lname, passw, true);
		}

        public static bool validateCredentials(string lname, string passw, bool checkForUmbracoConsoleAccess)
        {
            string consoleCheckSql = "";
            if (checkForUmbracoConsoleAccess)
                consoleCheckSql = "and userNoConsole = 0 ";

            SqlConnection conn = new SqlConnection(GlobalSettings.DbDSN);
            object tmp = SqlHelper.ExecuteScalar(
                conn,
                CommandType.Text,
                "select id from umbracoUser where userDisabled = 0 " + consoleCheckSql + " and userLogin = @login and userPassword = @pw", new SqlParameter("@login", lname), new SqlParameter("@pw", passw)
                );

            // Logging
            if (tmp == null)
                BusinessLogic.Log.Add(BusinessLogic.LogTypes.LoginFailure, BusinessLogic.User.GetUser(0), -1, "Login: '" + lname + "' failed, from IP: " + System.Web.HttpContext.Current.Request.UserHostAddress);
            return (tmp != null);
        }

		public UserType UserType {
			get
			{
                if (!_isInitialized)
                    setupUser(_id);
                return _usertype;
			}
			set {
				_usertype = value;
				SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text,
					@"Update umbracoUser set userType = @type where id = @id", 
					new SqlParameter("@type", value.Id), 
					new SqlParameter("@id", Id));
                FlushFromCache();
            }
		}


		// statics
		public static User[] getAll() {
			System.Collections.ArrayList tmpContainer = new System.Collections.ArrayList();

			SqlConnection conn = new SqlConnection(GlobalSettings.DbDSN);
			SqlDataReader dr;
			dr = SqlHelper.ExecuteReader(
				conn,
				CommandType.Text,
				"Select id from umbracoUser"
				);
            
			while (dr.Read()) {
                tmpContainer.Add(BusinessLogic.User.GetUser(int.Parse(dr["id"].ToString())));
			}
			dr.Close();
			User[] retVal = new User[tmpContainer.Count];
			
			int c = 0;
			foreach (User u in tmpContainer) {
				retVal[c] = u;
				c++;
			}
			return retVal;
		}
		
		public static void MakeNew(string name, string lname, string passw, UserType ut) {
			SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text, @"
				insert into umbracoUser 
				(UserType,startStructureId,startMediaId, UserName, userLogin, userPassword, userEmail,userLanguage) 
				values (@type,-1,-1,@name,@lname,@pw,'',@lang)", 
				new SqlParameter("@lang", GlobalSettings.DefaultUILanguage), 
				new SqlParameter("@name", name), 
				new SqlParameter("@lname", lname), 
				new SqlParameter("@type", ut.Id), 
				new SqlParameter("@pw", passw));
		}
			
		private static int getUserId(string lname, string passw) 
		{
			int retVal;
			SqlConnection conn = new SqlConnection(GlobalSettings.DbDSN);
			retVal = int.Parse(
				SqlHelper.ExecuteScalar(
				conn,
				CommandType.Text,
				"select id from umbracoUser where userDisabled = 0 and userNoConsole = 0 and userLogin = @login and userPassword = @pw",
				new SqlParameter("@login", lname),
				new SqlParameter("@pw", passw)
				).ToString()
				);
			return retVal;
		}
					
		private static int getUserId(string lname) 
		{
			int retVal;
			SqlConnection conn = new SqlConnection(GlobalSettings.DbDSN);
			retVal = int.Parse(
				SqlHelper.ExecuteScalar(
				conn,
				CommandType.Text,
				"select id from umbracoUser where userLogin = @login",
				new SqlParameter("@login", lname)
				).ToString()
				);
			return retVal;
		}

		public void delete() {
			SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text,"delete from umbracoUser where id = @id", new SqlParameter("@id", Id));
            FlushFromCache();
        }

		/*
		public string GetPermissions(int NodeId) 
		{
			return GetPermissions(new cms.businesslogic.web.Document(NodeId).Path);
		}
		*/

		public string GetPermissions(string Path) 
		{
            if (!_isInitialized)
                setupUser(_id);
            string cruds = UserType.DefaultPermissions;

			if (!_crudsInitialized)
				initCruds();

			foreach(string nodeId in Path.Split(',')) 
			{
				if (_cruds.ContainsKey(int.Parse(nodeId)))
					cruds = _cruds[int.Parse(nodeId)].ToString();
			}

			return cruds;
		}

		public void initCruds() 
		{
            if (!_isInitialized)
                setupUser(_id);

			using(SqlDataReader dr = SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text, "select * from umbracoUser2NodePermission where userId = @userId order by nodeId", new SqlParameter("@userId", this.Id)))
			{
				//	int currentId = -1;
				while(dr.Read())
				{
					if(!_cruds.ContainsKey(int.Parse(dr["nodeId"].ToString())))
						_cruds.Add(int.Parse(dr["nodeId"].ToString()), "");

					_cruds[int.Parse(dr["nodeId"].ToString())] += dr["permission"].ToString();
				}
			}
			_crudsInitialized = true;
		}

		public string GetNotifications(string Path) 
		{
            string notifications = "";

			if (!_notificationsInitialized)
				initNotifications();

			foreach(string nodeId in Path.Split(',')) 
			{
				if (_notifications.ContainsKey(int.Parse(nodeId)))
					notifications = _notifications[int.Parse(nodeId)].ToString();
			}

			return notifications;
		}

        /// <summary>
        /// Clears the internal hashtable containing cached information about notifications for the user
        /// </summary>
        public void resetNotificationCache()
        {
            _notificationsInitialized = false;
            _notifications.Clear();
        }

		public void initNotifications() 
		{
            if (!_isInitialized)
                setupUser(_id);

			using (SqlDataReader dr = SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text, "select * from umbracoUser2NodeNotify where userId = @userId order by nodeId", new SqlParameter("@userId", this.Id)))
			{
				while(dr.Read())
				{
					int nodeId = dr.GetInt32(dr.GetOrdinal("nodeId"));
					if(!_notifications.ContainsKey(nodeId))
						_notifications.Add(nodeId, "");

					_notifications[nodeId] += dr["action"].ToString();
				}
			}
			_notificationsInitialized = true;
		}

		public int Id 
		{
			get {return _id;}
		}

		public void clearApplications() {
			SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text, "delete from umbracoUser2App where [user] = @id", new SqlParameter("@id", this.Id));            		
		}

		public void addApplication(string AppAlias) {
			SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text, "insert into umbracoUser2App ([user],app) values (@id, @app)", new SqlParameter("@id", this.Id), new SqlParameter("@app", AppAlias));
		}

		public bool NoConsole 
		{
            get
            {
                if (!_isInitialized)
                    setupUser(_id);
                return _userNoConsole;
            }
			set 
			{
				_userNoConsole = value;
				SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text, "update umbracoUser set userNoConsole = @userNoConsole where id = @id", new SqlParameter("@id", this.Id), new SqlParameter("@userNoConsole", _userNoConsole));
                FlushFromCache();
            }
		}

		public bool Disabled 
		{
            get
            {
                if (!_isInitialized)
                    setupUser(_id);
                return _userDisabled;
            }
			set 
			{
				_userDisabled = value;
				SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text, "update umbracoUser set userDisabled = @userDisabled where id = @id", new SqlParameter("@id", this.Id), new SqlParameter("@userDisabled", _userDisabled));
                FlushFromCache();
            }
		}

		public int StartNodeId 
		{
            get
            {
                if (!_isInitialized)
                    setupUser(_id);
                return _startnodeid;
            }
			set 
			{
			
				_startnodeid = value;
				SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text, "update umbracoUser set  startStructureId = @start where id = @id", new SqlParameter("@start", value), new SqlParameter("@id", this.Id));
                FlushFromCache();
            }
		}
		public int StartMediaId 
		{
            get
            {
                if (!_isInitialized)
                    setupUser(_id);
                return _startmediaid;
            }
			set 
			{
			
				_startmediaid = value;
				SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text, "update umbracoUser set  startMediaId = @start where id = @id", new SqlParameter("@start", value), new SqlParameter("@id", this.Id));
			    FlushFromCache();
			}
		}

        protected void FlushFromCache()
        {
            if (System.Web.HttpRuntime.Cache[string.Format("UmbracoUser{0}", Id.ToString())] != null)
                System.Web.HttpRuntime.Cache.Remove(string.Format("UmbracoUser{0}", Id.ToString()));
        }

        public static User GetUser(int id)
        {
            if (System.Web.HttpRuntime.Cache[string.Format("UmbracoUser{0}", id.ToString())] == null)
            {
                User u = new User(id);
                System.Web.HttpRuntime.Cache.Insert(string.Format("UmbracoUser{0}", id.ToString()), u);
            }
            return (User)System.Web.HttpRuntime.Cache[string.Format("UmbracoUser{0}", id.ToString())];
        }

	}
}
