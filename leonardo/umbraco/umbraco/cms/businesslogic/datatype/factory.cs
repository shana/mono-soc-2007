using System;
using System.Collections;
using System.Web;

using Umbraco.BusinessLogic.Utils;
using Umbraco.interfaces;
using System.Collections.Generic;

namespace Umbraco.Cms.BusinessLogic.datatype.controls
{
	/// <summary>
	/// IDataType factory, handles the registering and retrieval of IDatatypes.
	/// 
	/// Then registering is done using reflection.
	/// </summary>
	public class Factory
	{
		#region Declarations

		private static string _pluginFolder = "";
		private static readonly Dictionary<Guid, Type> _controls = new Dictionary<Guid, Type>();

		#endregion

		#region Constructors

		static Factory()
		{
			Initialize();
		}

		#endregion

		/// <summary>
		/// Retrieves the IDataType specified by it's unique ID
		/// </summary>
		/// <param Name="DataTypeId">The IDataType id</param>
		/// <returns></returns>
		public IDataType DataType(Guid DataTypeId)
		{
			return GetNewObject(DataTypeId);
		}

		/// <summary>
		/// Retrieves the IDataType specified by it's unique ID
		/// </summary>
		/// <param Name="DataEditorId">The IDataType id</param>
		/// <returns></returns>
		public IDataType GetNewObject(Guid DataEditorId)
		{
			IDataType newObject = Activator.CreateInstance(_controls[DataEditorId]) as IDataType;
			return newObject;
		}

		/// <summary>
		/// Retrieve a complete list of all registered IDataType's
		/// </summary>
		/// <returns>A list of IDataType's</returns>
		public IDataType[] GetAll()
		{
			IDataType[] retVal = new IDataType[_controls.Count];
			int c = 0;

			foreach(Guid id in _controls.Keys)
			{
				retVal[c] = GetNewObject(id);
				c++;
			}

			return retVal;
		}

		private static void Initialize()
		{
			// Updated to use reflection  26-08-2004, NH
			// Add'ed plugin-folder setting key in web.config
			_pluginFolder = GlobalSettings.Path + "/../bin";

			HttpContext.Current.Trace.Write("datatype.factory", "Adding datatypes from directory: " + _pluginFolder);

			string[] types = TypeResolver.GetAssignablesFromType<IDataType>(
				HttpContext.Current.Server.MapPath(_pluginFolder), "*.dll");
			foreach(string type in types)
			{
				Type t = Type.GetType(type);
				IDataType typeInstance = Activator.CreateInstance(t) as IDataType;
				if(typeInstance == null)
					continue;

				_controls.Add(typeInstance.Id, t);

				if(HttpContext.Current != null)
					HttpContext.Current.Trace.Write("datatype.factory", " + Adding datatype '" + typeInstance.DataTypeName);
			}
		}
	}
}