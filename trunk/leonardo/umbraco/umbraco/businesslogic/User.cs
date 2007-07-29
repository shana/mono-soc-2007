using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using Microsoft.ApplicationBlocks.Data;

namespace Umbraco.BusinessLogic
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

		/// <summary>
		/// Initializes a new instance of the <see cref="User"/> class.
		/// </summary>
		/// <param name="ID">The ID.</param>
		public User(int ID)
		{
			SetupUser(ID);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="User"/> class.
		/// </summary>
		/// <param name="ID">The ID.</param>
		/// <param name="noSetup">if set to <c>true</c> [no setup].</param>
        public User(int ID, bool noSetup)
        {
            _id = ID;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="User"/> class.
		/// </summary>
		/// <param name="Login">The login.</param>
		/// <param name="Password">The password.</param>
	    public User(string Login, string Password)
		{			
			SetupUser(GetUserId(Login, Password));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="User"/> class.
		/// </summary>
		/// <param name="Login">The login.</param>
		public User(string Login) 
		{
			SetupUser(GetUserId(Login));
		}

		/// <summary>
		/// Setups the user.
		/// </summary>
		/// <param name="ID">The ID.</param>
		private void SetupUser(int ID)
		{
			_id = ID;

			using (SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(
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

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name 
		{
			get
			{
                if (!_isInitialized)
                    SetupUser(_id);
			    return _name;
			}
			set 
			{
				_name = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text,"Update umbracoUser set UserName = @userName where id = @id", new SqlParameter("@userName", value), new SqlParameter("@id", Id));
                FlushFromCache();
            }
		}

		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>The email.</value>
		public string Email
		{
			get
			{
                if (!_isInitialized)
                    SetupUser(_id);
                return _email;
			}
			set 
			{
				_email = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text,"Update umbracoUser set UserEmail = @email where id = @id", new SqlParameter("@id", this.Id), new SqlParameter("@email", value));
                FlushFromCache();
            }
		}

		/// <summary>
		/// Gets or sets the language.
		/// </summary>
		/// <value>The language.</value>
		public string Language 
		{
			get
			{
                if (!_isInitialized)
                    SetupUser(_id);
                return _language;
			}
			set {
				_language = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text,"Update umbracoUser set userLanguage = @language where id = @id", new SqlParameter("@language", value), new SqlParameter("@id", Id));
                FlushFromCache();
            }
	}

	/// <summary>
	/// Gets or sets the password.
	/// </summary>
	/// <value>The password.</value>
		public string Password 
		{
			get
			{
                if (!_isInitialized)
                    SetupUser(_id);
                return _password;
			}
			set {
				_password = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text,"Update umbracoUser set UserPassword = @pw where id = @id", new SqlParameter("@pw", value), new SqlParameter("@id", Id));
                FlushFromCache();
            }
		}
		static string  _connstring = GlobalSettings.DbDSN;

		/// <summary>
		/// Gets the applications.
		/// </summary>
		/// <value>The applications.</value>
		public Application[] Applications 
		{
			get{
                if (!_isInitialized)
                    SetupUser(_id);
                ArrayList al = new ArrayList();

				using (SqlDataReader appIcons = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(_connstring,
					CommandType.Text, "select appAlias, appIcon, appname from umbracoApp app join umbracoUser2app u2a on u2a.app = app.appAlias and u2a.[user] = @userID order by app.sortOrder", new SqlParameter("@userID", this.Id)))
				{
					while(appIcons.Read())
					{
						Application tmp = new Application();
						tmp.Name = appIcons.GetString(appIcons.GetOrdinal("appName"));
						tmp.Icon = appIcons.GetString(appIcons.GetOrdinal("appIcon"));
						tmp.Alias = appIcons.GetString(appIcons.GetOrdinal("appAlias"));
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

		/// <summary>
		/// Gets or sets the name of the login.
		/// </summary>
		/// <value>The name of the login.</value>
		public string LoginName {
			get
			{
                if (!_isInitialized)
                    SetupUser(_id);
                return _loginname;
			}
			set {
				_loginname = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text,"Update umbracoUser set UserLogin = @login where id = @id", new SqlParameter("@login", value), new SqlParameter("@id", Id));
                FlushFromCache();
            }
		}

		/// <summary>
		/// Validates the credentials.
		/// </summary>
		/// <param name="lname">The lname.</param>
		/// <param name="passw">The passw.</param>
		/// <returns></returns>
		public static bool ValidateCredentials(string lname, string passw) {
            return ValidateCredentials(lname, passw, true);
		}

		/// <summary>
		/// Validates the credentials.
		/// </summary>
		/// <param name="lname">The lname.</param>
		/// <param name="passw">The passw.</param>
		/// <param name="checkForUmbracoConsoleAccess">if set to <c>true</c> [check for umbraco console access].</param>
		/// <returns></returns>
        public static bool ValidateCredentials(string lname, string passw, bool checkForUmbracoConsoleAccess)
        {
            string consoleCheckSql = "";
            if (checkForUmbracoConsoleAccess)
                consoleCheckSql = "and userNoConsole = 0 ";

            SqlConnection conn = new SqlConnection(GlobalSettings.DbDSN);
            object tmp = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(
                conn,
                CommandType.Text,
                "select id from umbracoUser where userDisabled = 0 " + consoleCheckSql + " and userLogin = @login and userPassword = @pw", new SqlParameter("@login", lname), new SqlParameter("@pw", passw)
                );

            // Logging
            if (tmp == null)
                BusinessLogic.Log.Add(BusinessLogic.LogTypes.LoginFailure, BusinessLogic.User.GetUser(0), -1, "Login: '" + lname + "' failed, from IP: " + System.Web.HttpContext.Current.Request.UserHostAddress);
            return (tmp != null);
        }

		/// <summary>
		/// Gets or sets the type of the user.
		/// </summary>
		/// <value>The type of the user.</value>
		public UserType UserType {
			get
			{
                if (!_isInitialized)
                    SetupUser(_id);
                return _usertype;
			}
			set {
				_usertype = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text,
					@"Update umbracoUser set userType = @type where id = @id", 
					new SqlParameter("@type", value.Id), 
					new SqlParameter("@id", Id));
                FlushFromCache();
            }
		}


		// statics
		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns></returns>
		// TODO: Find out what it gets
		public static User[] GetAll() {
			System.Collections.ArrayList tmpContainer = new System.Collections.ArrayList();

			SqlConnection conn = new SqlConnection(GlobalSettings.DbDSN);
			SqlDataReader dr;
			dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(
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

		/// <summary>
		/// Makes the new.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="lname">The lname.</param>
		/// <param name="passw">The passw.</param>
		/// <param name="ut">The ut.</param>
		public static void MakeNew(string name, string lname, string passw, UserType ut) {
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text, @"
				insert into umbracoUser 
				(UserType,startStructureId,startMediaId, UserName, userLogin, userPassword, userEmail,userLanguage) 
				values (@type,-1,-1,@Name,@lname,@pw,'',@lang)", 
				new SqlParameter("@lang", GlobalSettings.DefaultUILanguage), 
				new SqlParameter("@Name", name), 
				new SqlParameter("@lname", lname), 
				new SqlParameter("@type", ut.Id), 
				new SqlParameter("@pw", passw));
		}

		/// <summary>
		/// Gets the user id.
		/// </summary>
		/// <param name="lname">The lname.</param>
		/// <param name="passw">The passw.</param>
		/// <returns></returns>
		private static int GetUserId(string lname, string passw) 
		{
			int retVal;
			SqlConnection conn = new SqlConnection(GlobalSettings.DbDSN);
			retVal = int.Parse(
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(
				conn,
				CommandType.Text,
				"select id from umbracoUser where userDisabled = 0 and userNoConsole = 0 and userLogin = @login and userPassword = @pw",
				new SqlParameter("@login", lname),
				new SqlParameter("@pw", passw)
				).ToString()
				);
			return retVal;
		}

		/// <summary>
		/// Gets the user id.
		/// </summary>
		/// <param name="lname">The lname.</param>
		/// <returns></returns>
		private static int GetUserId(string lname) 
		{
			int retVal;
			SqlConnection conn = new SqlConnection(GlobalSettings.DbDSN);
			retVal = int.Parse(
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(
				conn,
				CommandType.Text,
				"select id from umbracoUser where userLogin = @login",
				new SqlParameter("@login", lname)
				).ToString()
				);
			return retVal;
		}

		/// <summary>
		/// Deletes this instance.
		/// </summary>
		public void Delete() {
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text,"delete from umbracoUser where id = @id", new SqlParameter("@id", Id));
            FlushFromCache();
        }

		/// <summary>
		/// Gets the permissions.
		/// </summary>
		/// <param name="Path">The path.</param>
		/// <returns></returns>
		public string GetPermissions(string Path) 
		{
            if (!_isInitialized)
                SetupUser(_id);
            string cruds = UserType.DefaultPermissions;

			if (!_crudsInitialized)
				InitializeCruds();

			foreach(string nodeId in Path.Split(',')) 
			{
				if (_cruds.ContainsKey(int.Parse(nodeId)))
					cruds = _cruds[int.Parse(nodeId)].ToString();
			}

			return cruds;
		}

		/// <summary>
		/// Initializes the cruds.
		/// </summary>
		public void InitializeCruds() 
		{
            if (!_isInitialized)
                SetupUser(_id);

			using(SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text, "select * from umbracoUser2NodePermission where userId = @userId order by nodeId", new SqlParameter("@userId", this.Id)))
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

		/// <summary>
		/// Gets the notifications.
		/// </summary>
		/// <param name="Path">The path.</param>
		/// <returns></returns>
		public string GetNotifications(string Path) 
		{
            string notifications = "";

			if (!_notificationsInitialized)
				InitializeNotifications();

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

		/// <summary>
		/// Inits the notifications.
		/// </summary>
		public void InitializeNotifications() 
		{
            if (!_isInitialized)
                SetupUser(_id);

			using (SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text, "select * from umbracoUser2NodeNotify where userId = @userId order by nodeId", new SqlParameter("@userId", this.Id)))
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

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>The id.</value>
		public int Id 
		{
			get {return _id;}
		}

		/// <summary>
		/// Clears the applications.
		/// </summary>
		public void ClearApplications() {
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text, "delete from umbracoUser2App where [user] = @id", new SqlParameter("@id", this.Id));            		
		}

		/// <summary>
		/// Adds the application.
		/// </summary>
		/// <param name="AppAlias">The app alias.</param>
		public void AddApplication(string AppAlias) {
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text, "insert into umbracoUser2App ([user],app) values (@id, @app)", new SqlParameter("@id", this.Id), new SqlParameter("@app", AppAlias));
		}

		/// <summary>
		/// Gets or sets a value indicating whether [no console].
		/// </summary>
		/// <value><c>true</c> if [no console]; otherwise, <c>false</c>.</value>
		public bool NoConsole 
		{
            get
            {
                if (!_isInitialized)
                    SetupUser(_id);
                return _userNoConsole;
            }
			set 
			{
				_userNoConsole = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text, "update umbracoUser set userNoConsole = @userNoConsole where id = @id", new SqlParameter("@id", this.Id), new SqlParameter("@userNoConsole", _userNoConsole));
                FlushFromCache();
            }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="User"/> is disabled.
		/// </summary>
		/// <value><c>true</c> if disabled; otherwise, <c>false</c>.</value>
		public bool Disabled 
		{
            get
            {
                if (!_isInitialized)
                    SetupUser(_id);
                return _userDisabled;
            }
			set 
			{
				_userDisabled = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text, "update umbracoUser set userDisabled = @userDisabled where id = @id", new SqlParameter("@id", this.Id), new SqlParameter("@userDisabled", _userDisabled));
                FlushFromCache();
            }
		}

		/// <summary>
		/// Gets or sets the start node id.
		/// </summary>
		/// <value>The start node id.</value>
		public int StartNodeId 
		{
            get
            {
                if (!_isInitialized)
                    SetupUser(_id);
                return _startnodeid;
            }
			set 
			{
			
				_startnodeid = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text, "update umbracoUser set  startStructureId = @start where id = @id", new SqlParameter("@start", value), new SqlParameter("@id", this.Id));
                FlushFromCache();
            }
		}
		/// <summary>
		/// Gets or sets the start media id.
		/// </summary>
		/// <value>The start media id.</value>
		public int StartMediaId 
		{
            get
            {
                if (!_isInitialized)
                    SetupUser(_id);
                return _startmediaid;
            }
			set 
			{
			
				_startmediaid = value;
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(_connstring,CommandType.Text, "update umbracoUser set  startMediaId = @start where id = @id", new SqlParameter("@start", value), new SqlParameter("@id", this.Id));
			    FlushFromCache();
			}
		}

		/// <summary>
		/// Flushes from cache.
		/// </summary>
        protected void FlushFromCache()
        {
            if (System.Web.HttpRuntime.Cache[string.Format("UmbracoUser{0}", Id.ToString())] != null)
                System.Web.HttpRuntime.Cache.Remove(string.Format("UmbracoUser{0}", Id.ToString()));
        }

		/// <summary>
		/// Gets the user.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <returns></returns>
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
