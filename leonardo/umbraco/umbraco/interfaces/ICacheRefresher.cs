using System;

namespace umbraco.interfaces
{
	/// <summary>
	/// Summary description for IDataFieldWithButtons.
	/// </summary>
	public interface ICacheRefresher
	{
		Guid UniqueIdentifier {get;}
		string Name {get;}
		void RefreshAll();
		void Refresh(int Id);
		void Refresh(Guid Id);
	}
}
