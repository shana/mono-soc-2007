using System;

using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using System.Collections;

namespace Umbraco.Cms.BusinessLogic.index
{
	/// <summary>
	/// Indexer contains methods for populating data to the internal search of the Umbraco console
	/// 
	/// 
	/// </summary>
	public class Indexer
	{

		private static string _indexDirectory = "";
		public static string RelativeIndexDir = Umbraco.GlobalSettings.StorageDirectory + "/_systemUmbracoIndexDontDelete";

		/// <summary>
		/// The physical path to the folder where Umbraco stores the files used by the Lucene searchcomponent
		/// </summary>
		public static string IndexDirectory 
		{
			get 
			{
				try 
				{
					if (_indexDirectory == "")
						_indexDirectory = System.Web.HttpContext.Current.Server.MapPath(RelativeIndexDir);
					return _indexDirectory;
				} 
				catch 
				{
					return "";
				}
			}
			set 
			{
                _indexDirectory = value;
			}
		}

	

		/// <summary>
		/// Method for accesing the Lucene indexwriter in the internal search
		/// </summary>
		/// <param Name="ForceRecreation">If set to true, a new index is created</param>
		/// <returns>The lucene indexwriter</returns>
		public static IndexWriter ContentIndex(bool ForceRecreation) 
		{
			if (!ForceRecreation && System.IO.Directory.Exists(IndexDirectory) &&
				new System.IO.DirectoryInfo(IndexDirectory).GetFiles().Length > 1)
				return new IndexWriter(IndexDirectory, new StandardAnalyzer(), false);
			else 
			{
				IndexWriter iw = new IndexWriter(IndexDirectory, new StandardAnalyzer(), true);
				return iw;
			}
		}

		/// <summary>
		/// Method for accessing the Lucene indexreader
		/// </summary>
		/// <returns></returns>
		public static IndexReader ContentIndexReader() 
		{
			return IndexReader.Open(IndexDirectory);
		}

		/// <summary>
		/// Method for reindexing data in all documents of Umbraco, this is a performaceheavy invocation and should be
		/// used with care!
		/// </summary>
		public static void ReIndex() 
		{
			// Create new index
			IndexWriter w = ContentIndex(true);
			w.Close();

			Guid[] documents = Cms.BusinessLogic.web.Document.getAllUniquesFromObjectType(Cms.BusinessLogic.web.Document._objectType);
			
			System.Web.HttpContext.Current.Application["umbIndexerTotal"] = documents.Length;
			System.Web.HttpContext.Current.Application["umbIndexerCount"] = 0;
			System.Web.HttpContext.Current.Application["umbIndexerInfo"] = "";

			foreach(Guid g in documents) 
			{
				Cms.BusinessLogic.web.Document d = 
				new Cms.BusinessLogic.web.Document(g);
				d.Index(true);
				System.Web.HttpContext.Current.Application.Lock();
				System.Web.HttpContext.Current.Application["umbIndexerInfo"] = d.Text;
				System.Web.HttpContext.Current.Application["umbIndexerCount"] = 
					((int) System.Web.HttpContext.Current.Application["umbIndexerCount"])+1;
				System.Web.HttpContext.Current.Application.UnLock();
			}
		}

		public static void IndexNode(Guid ObjectType, int Id, string Text, string UserName, DateTime CreateDate, Hashtable Fields, bool Optimize) 
		{
			// remove node if exists
			RemoveNode(Id);

			// Add node
			Document d = new Document(); // Lucene document, not Umbraco Document
			d.Add(new Field("Id", Id.ToString(), true, true, true));
			d.Add(new Field("Text", Text, true, true, true));
			d.Add(new Field("ObjectType", ObjectType.ToString(), true, true, true));
			d.Add(new Field("User", UserName, true, true, false));
			d.Add(new Field("CreateDate", convertDate(CreateDate), true, true, false));

			// Sort key
			d.Add(new Field("SortText", Text, true, true, false));

			System.Text.StringBuilder total = new System.Text.StringBuilder();
			total.Append(Text + " ");

			// Add all fields
			if (Fields != null) 
			{
				IDictionaryEnumerator ide = Fields.GetEnumerator();
				while (ide.MoveNext()) 
				{
					d.Add(new Field("field_" + ide.Key.ToString(), ide.Value.ToString(), true, true, true));
					total.Append(ide.Value.ToString());
					total.Append(" ");
				}
			}

			IndexWriter writer = ContentIndex(false);
			try 
			{
				d.Add(new Field("Content", total.ToString(), true, true, true));
				writer.AddDocument(d);
				writer.Optimize();
				writer.Close();
			} 
			catch (Exception ee)
			{
				BusinessLogic.Log.Add(BusinessLogic.LogTypes.Error, BusinessLogic.User.GetUser(0), Id, "Error indexing node: (" + ee.ToString() + ")");
			}
			finally 
			{
				writer.Close();
			}

		}

		public static void RemoveNode(int Id) 
		{
			try 
			{
				IndexReader ir = ContentIndexReader();
				ir.Delete(new Term("id", Id.ToString()));
				ir.Close();
			} 
			catch (Exception ee)
			{
				BusinessLogic.Log.Add(
					BusinessLogic.LogTypes.Error,
					BusinessLogic.User.GetUser(0),
					Id,
					"Error removing node from Umbraco index: '" + ee.ToString()  + "'");
			}
		}

		private static string convertDate(DateTime Date) 
		{
			try 
			{
				string thisYear = Date.Year.ToString();
				if (thisYear.Length == 1)
					thisYear = "0" + thisYear;
				string thisMonth = Date.Month.ToString();
				if (thisMonth.Length == 1) 
					thisMonth = "0" + thisMonth;
				string thisDay = Date.Day.ToString();
				if (thisDay.Length == 1)
					thisDay = "0" + thisDay;
		        
				return thisYear + thisMonth + thisDay;
			} 
			catch 
			{
				return "";
			}
		}
	}

}
