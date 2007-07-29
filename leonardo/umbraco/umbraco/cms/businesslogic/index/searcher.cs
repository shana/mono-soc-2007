using System;

using System.Collections;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;

namespace Umbraco.Cms.BusinessLogic.index
{
	/// <summary>
	/// Searcher is used in the internal search (autosuggestion) in the Umbraco administration console
	/// </summary>
	public class searcher
	{
		
		/// <summary>
		/// Method for retrieving a list of documents where the keyword is present
		/// </summary>
		/// <param Name="ObjectType">[not implemented] search only available for documents</param>
		/// <param Name="Keyword">The word being searched for</param>
		/// <param Name="Max">The maximum limit on results returned</param>
		/// <returns>A list of documentnames indexed by the id of the document</returns>
		public static Hashtable Search(Guid ObjectType, string Keyword, int Max) 
		{
			Hashtable results = new Hashtable();
			IndexSearcher searcher = new IndexSearcher(index.Indexer.IndexDirectory);
			Query query = QueryParser.Parse(Keyword, "Content", new StandardAnalyzer());
			Hits hits;
			
			// Sorting
			SortField[] sf = {new SortField("SortText")};
			try 
			{
				hits = searcher.Search(query, new Sort(sf));
				if (hits.Length() < Max)
					Max = hits.Length();

				for (int i=0;i<Max;i++) 
				{
					try 
					{
						results.Add(
							hits.Doc(i).Get("Id"), 
							hits.Doc(i).Get("Text"));
					} 
					catch 
					{
					}
				}

				searcher.Close();
			} 
			catch (Exception ee)
			{
				searcher.Close();
                throw ee;
			}

			return results;
			
		}
	}
}
