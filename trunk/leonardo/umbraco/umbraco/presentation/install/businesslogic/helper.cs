using System;

namespace umbraco.presentation.install.businesslogic
{
	/// <summary>
	/// Summary description for helper.
	/// </summary>
	public class helper
	{
		public helper()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static string loadTextFile(string name) 
		{
			try 
			{
				System.IO.StreamReader re = System.IO.File.OpenText(name);				
				string design = re.ReadToEnd();
				re.Close();
				return design;
				
			} 
			catch 
			{
				return "";
			}

		}

	}
}
