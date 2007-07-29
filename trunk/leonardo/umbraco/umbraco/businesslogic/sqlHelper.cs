using System;

namespace Umbraco
{
	/// <summary>
	/// This class implements common SQL related functions
	/// </summary>
	public static class SqlHelper
	{
		/// <summary>
		/// Safes the string.
		/// </summary>
		/// <param Name="text">The text.</param>
		/// <returns></returns>
		[Obsolete("Use parameterized queries instead")]
		public static string SafeString(string text) 
		{
			return text.Replace("'", "''");
		}
	}
}