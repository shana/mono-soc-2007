using System;

namespace Umbraco.Cms.BusinessLogic
{
	/// <summary>
	/// Summary description for ËnsureDataLayer.
	/// </summary>
	[Obsolete("This class has no implementation at all")]
	public class DataLayer
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataLayer"/> class.
		/// </summary>
		private DataLayer() 
		{
		}

		/// <summary>
		/// Ensures the table.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="fields">The fields.</param>
		/// <param name="fieldDefinitions">The field definitions.</param>
		[Obsolete("This method does nothing")]
		public static void EnsureTable(string tableName, string[] fields, string[] fieldDefinitions) 
		{
		}
	}
}