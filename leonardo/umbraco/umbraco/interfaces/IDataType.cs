using System;

namespace umbraco.interfaces
{
	/// <summary>
	/// Summary description for IDataType.
	/// </summary>
	public interface IDataType 
	{
		Guid Id {get;}
		string DataTypeName{get;}
		IDataEditor DataEditor{get;}
		IDataPrevalue PrevalueEditor {get;}
		IData Data{get;}
		int DataTypeDefinitionId {set; get;}
	}

}
