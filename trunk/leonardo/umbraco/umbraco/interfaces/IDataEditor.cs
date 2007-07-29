using System;

namespace Umbraco.interfaces
{

	public interface IDataEditor 
	{
		void Save();
		bool ShowLabel {get;}
		bool TreatAsRichTextEditor {get;}
		System.Web.UI.Control Editor{get;}
	}
}
