using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using System.Collections;


namespace umbraco.cms.businesslogic.datatype
{
	/// <summary>
	/// Datatypedefinitions is the basic buildingblocks of umbraco's documents/medias/members generic datastructure 
	/// 
	/// A datatypedefinition encapsulates an object which implements the interface IDataType, and are used when defining
	/// the properties of a document in the documenttype. This extra layer between IDataType and a documenttypes propertytype
	/// are used amongst other for enabling shared prevalues.
	/// 
	/// </summary>
	public class DataTypeDefinition : CMSNode
	{
		private Guid _controlId;
		
		private static Guid _objectType = new Guid("30a2a501-1978-4ddb-a57b-f7efed43ba3c");


		/// <summary>
		/// Initialization of the datatypedefinition
		/// </summary>
		/// <param name="id">Datattypedefininition id</param>
		public DataTypeDefinition(int id) : base(id)
		{
			setupDataTypeDefinition();
		}

        public void Delete()
        {
            delete();
            cache.Cache.ClearCacheItem(string.Format("UmbracoDataTypeDefinition{0}", Id));
        }

	    /// <summary>
		/// Initialization of the datatypedefinition
		/// </summary>
		/// <param name="id">Datattypedefininition id</param>
		public DataTypeDefinition(Guid id) : base(id)
		{
			setupDataTypeDefinition();
		}


        /// <summary>
        /// Used to persist object changes to the database. In v3.0 it's just a stub for future compatibility
        /// </summary>
        public override void Save()
        {
        }



		private void setupDataTypeDefinition() {
			SqlDataReader dr = SqlHelper.ExecuteReader(_ConnString, CommandType.Text, "select dbType, controlId from cmsDataType where nodeId = '" + this.Id.ToString() + "'");
			if (dr.Read()) 
			{
				_controlId = dr.GetGuid(dr.GetOrdinal("controlId"));
			} 
			else
				throw new ArgumentException("No dataType with id = " + this.Id.ToString() + " found");
			dr.Close();
		}


		/// <summary>
		/// The associated datatype, which delivers the methods for editing data, editing prevalues see: umbraco.interfaces.IDataType
		/// </summary>
		public interfaces.IDataType DataType
		{
			get {
				cms.businesslogic.datatype.controls.Factory f = new cms.businesslogic.datatype.controls.Factory();
				interfaces.IDataType dt = f.DataType(_controlId);
				dt.DataTypeDefinitionId = Id;
				
				return dt;
				}
			set 
			{
				SqlHelper.ExecuteNonQuery(_ConnString, CommandType.Text, "update cmsDataType set controlId = '" + value.Id + "' where nodeID = " + this.Id.ToString());
				_controlId = value.Id;
			}
		}

		/*
		public SortedList PreValues {
			get {
				SortedList retVal = new SortedList();
				SqlDataReader dr = SqlHelper.ExecuteReader(GlobalSettings.DbDSN, CommandType.Text, "select id, value from cmsDataTypePreValues where dataTypeNodeId = @nodeId order by sortOrder", new SqlParameter("@nodeId", this.Id));
				while (dr.Read()) 
				{
					retVal.Add(dr["id"].ToString(), dr["value"].ToString());
				}
				dr.Close();

				return retVal;
				}
		}
		*/

		/// <summary>
		/// Retrieves a list of all datatypedefinitions
		/// </summary>
		/// <returns>A list of all datatypedefinitions</returns>
		public static DataTypeDefinition[] GetAll() 
		{
			SortedList retvalSort = new SortedList();
            Guid[] tmp = CMSNode.getAllUniquesFromObjectType(_objectType);
			DataTypeDefinition[] retval = new DataTypeDefinition[tmp.Length];
			for(int i = 0; i < tmp.Length; i++) {
				DataTypeDefinition dt = DataTypeDefinition.GetDataTypeDefinition(tmp[i]);
				retvalSort.Add(dt.Text + "|||" + Guid.NewGuid().ToString(), dt);
			}

			IDictionaryEnumerator ide = retvalSort.GetEnumerator();
			int counter = 0;
			while (ide.MoveNext()) 
			{
				retval[counter] = (DataTypeDefinition) ide.Value;
				counter++;
			}
			return retval;
		}

		/// <summary>
		/// Creates a new datatypedefinition given its name and the user which creates it.
		/// </summary>
		/// <param name="u">The user who creates the datatypedefinition</param>
		/// <param name="Text">The name of the DataTypeDefinition</param>
		/// <returns></returns>
		public static DataTypeDefinition MakeNew(BusinessLogic.User u, string Text) 
		{
			int newId = CMSNode.MakeNew(-1, _objectType,u.Id,1, Text, Guid.NewGuid()).Id;
			cms.businesslogic.datatype.controls.Factory f = new cms.businesslogic.datatype.controls.Factory();
			Guid FirstControlId = f.GetAll()[0].Id;
			SqlHelper.ExecuteNonQuery(_ConnString,CommandType.Text, "Insert into cmsDataType (nodeId, controlId, dbType) values (" + newId.ToString() +",'" + FirstControlId.ToString() + "','Ntext')");
			return new DataTypeDefinition(newId);
		}

		/// <summary>
		/// Retrieve a list of datatypedefinitions which share the same IDataType datatype
		/// </summary>
		/// <param name="DataTypeId">The unique id of the IDataType</param>
		/// <returns>A list of datatypedefinitions which are based on the IDataType specified</returns>
		public static DataTypeDefinition GetByDataTypeId(Guid DataTypeId) 
		{
			int dfId = 0;
			foreach(DataTypeDefinition df in DataTypeDefinition.GetAll())
				if (df.DataType.Id == DataTypeId) 
				{
					dfId = df.Id;
					break;
				}

			if (dfId == 0)
				return null;
			else
				return new DataTypeDefinition(dfId);
		}

        /// <summary>
        /// Analyzes an object to see if its basetype is umbraco.editorControls.DefaultData
        /// </summary>
        /// <param name="Data">The Data object to analyze</param>
        /// <returns>True if the basetype is the DefaultData class</returns>
        public static bool IsDefaultData(object Data)
        {
            Type typeOfData = Data.GetType();

            while (typeOfData.BaseType != new Object().GetType())
                typeOfData = typeOfData.BaseType;

            return (typeOfData.FullName == "umbraco.cms.businesslogic.datatype.DefaultData");
        }

        public static DataTypeDefinition GetDataTypeDefinition(int id)
        {
            if (System.Web.HttpRuntime.Cache[string.Format("UmbracoDataTypeDefinition{0}", id.ToString())] == null)
            {
                DataTypeDefinition dt = new DataTypeDefinition(id);
                System.Web.HttpRuntime.Cache.Insert(string.Format("UmbracoDataTypeDefinition{0}", id.ToString()), dt);
            }
            return (DataTypeDefinition)System.Web.HttpRuntime.Cache[string.Format("UmbracoDataTypeDefinition{0}", id.ToString())];
        }

        public static DataTypeDefinition GetDataTypeDefinition(Guid id)
        {
            if (System.Web.HttpRuntime.Cache[string.Format("UmbracoDataTypeDefinition{0}", id.ToString())] == null)
            {
                DataTypeDefinition dt = new DataTypeDefinition(id);
                System.Web.HttpRuntime.Cache.Insert(string.Format("UmbracoDataTypeDefinition{0}", id.ToString()), dt);
            }
            return (DataTypeDefinition)System.Web.HttpRuntime.Cache[string.Format("UmbracoDataTypeDefinition{0}", id.ToString())];
        }

    }
}