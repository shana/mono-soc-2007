using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace umbraco.editorControls
{
	public class noEdit : System.Web.UI.WebControls.Label, interfaces.IDataEditor
	{
		private interfaces.IData _data;
		
		public noEdit(interfaces.IData Data) {
			_data = Data;
		}
		public virtual bool TreatAsRichTextEditor 
		{
			get {return false;}
		}
		public bool ShowLabel {get {return true;}}

		public void Save() {}

		public Control Editor {get {return this;}}

		public override string Text {
			get {return _data.Value.ToString();}
		}
	}
}