using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace Umbraco.BusinessLogic.Utils
{
	/// <summary>
	/// This class implements type resolution functionalities
	/// </summary>
	[Serializable]
	public class TypeResolver : MarshalByRefObject
	{
		/// <summary>
		/// Gets the type of the assignables from.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="path">The path.</param>
		/// <param name="filePattern">The file pattern.</param>
		/// <returns></returns>
		public static string[] GetAssignablesFromType<T>(string path, string filePattern)
		{
			FileInfo[] fis = Array.ConvertAll<string, FileInfo>(
				Directory.GetFiles(path, filePattern),
				delegate(string s) { return new FileInfo(s); });
			string[] absoluteFiles = Array.ConvertAll<FileInfo, string>(
				fis, delegate(FileInfo fi) { return fi.FullName; });
			return GetAssignablesFromType<T>(absoluteFiles);
		}

		/// <summary>
		/// Gets the type of the assignables from.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="files">The files.</param>
		/// <returns></returns>
		public static string[] GetAssignablesFromType<T>(string[] files)
		{
			AppDomainSetup domainSetup = new AppDomainSetup();
			domainSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
			domainSetup.ApplicationName = "Umbraco_Sandbox_" + Guid.NewGuid();
			domainSetup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
			domainSetup.DynamicBase = AppDomain.CurrentDomain.SetupInformation.DynamicBase;
			domainSetup.LicenseFile = AppDomain.CurrentDomain.SetupInformation.LicenseFile;
			domainSetup.LoaderOptimization = AppDomain.CurrentDomain.SetupInformation.LoaderOptimization;
			domainSetup.PrivateBinPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
			domainSetup.PrivateBinPathProbe = AppDomain.CurrentDomain.SetupInformation.PrivateBinPathProbe;
			domainSetup.ShadowCopyFiles = "false";

			AppDomain sandbox = AppDomain.CreateDomain("Sandbox",
				AppDomain.CurrentDomain.Evidence, domainSetup);
			try
			{
				TypeResolver typeResolver = (TypeResolver)sandbox.CreateInstanceAndUnwrap(
					typeof(TypeResolver).Assembly.GetName().Name,
					typeof(TypeResolver).FullName);

				return typeResolver.GetTypes(typeof(T), files);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
			finally
			{
				AppDomain.Unload(sandbox);
			}
			return new string[0];
		}

		/// <summary>
		/// Gets the types.
		/// </summary>
		/// <param name="assignTypeFrom">The assign type from.</param>
		/// <param name="assemblyFiles">The assembly files.</param>
		/// <returns></returns>
		public string[] GetTypes(Type assignTypeFrom, string[] assemblyFiles)
		{
			List<string> result = new List<string>();
			foreach(string fileName in assemblyFiles)
			{
				if(!File.Exists(fileName))
					continue;
				try
				{
					Assembly assembly = Assembly.LoadFile(fileName);
					foreach(Type t in assembly.GetTypes()) 
					{
					    if(!t.IsInterface && assignTypeFrom.IsAssignableFrom(t))
					        result.Add(t.AssemblyQualifiedName);
					} 
				}
				catch(Exception e)
				{
					Debug.WriteLine(string.Format("Error loading assembly: {0}\n{1}", fileName, e));
			    }
			}
			return result.ToArray();
		}
	}
}
