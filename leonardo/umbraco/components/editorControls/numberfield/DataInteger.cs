using System;

namespace umbraco.editorControls.numberfield
{
	/// <summary>
	/// Summary description for DataInteger.
	/// </summary>
	public class DataInteger : Cms.BusinessLogic.datatype.DefaultData
	{
		public DataInteger(Cms.BusinessLogic.datatype.BaseDataType DataType) : base(DataType) {}

		public override void MakeNew(int PropertyId) {
			this.PropertyId = PropertyId;
		    string defaultValue = ((DefaultPrevalueEditor) _dataType.PrevalueEditor).Prevalue;
            if (defaultValue.Trim() != "")
			    this.Value = defaultValue;
		}
	} 
}
