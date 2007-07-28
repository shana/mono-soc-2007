using System;

namespace umbraco.presentation.cache
{
	/// <summary>
	/// Summary description for pageRefresher.
	/// </summary>
	public class pageRefresher : interfaces.ICacheRefresher
	{
		public pageRefresher()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#region ICacheRefresher Members

		public Guid UniqueIdentifier
		{
			get
			{
				// TODO:  Add pageRefresher.uniqueIdentifier getter implementation
				return new Guid("27AB3022-3DFA-47b6-9119-5945BC88FD66");
			}
		}

		public string Name 
		{
			get {return "Page Refresher (umbraco.library wrapper)";}
		}

		public void RefreshAll()
		{
			// library.RePublishNodesDotNet(-1, true);
            content.Instance.RefreshContentFromDatabaseAsync();
		}

		public void Refresh(Guid Id)
		{
			// Not used when pages
		}

		public void Refresh(int Id)
		{
			library.PublishSingleNodeDo(Id);
		}

		#endregion
	}
}
