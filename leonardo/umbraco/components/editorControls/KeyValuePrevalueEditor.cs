using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace umbraco.editorControls
{
	/// <summary>
	/// Summary description for KeyValuePrevalueEditor.
	/// </summary>
	public class KeyValuePrevalueEditor : System.Web.UI.WebControls.PlaceHolder, interfaces.IDataPrevalue
	{
	
		// UI controls
		private System.Web.UI.WebControls.DropDownList _dropdownlist;
		private TextBox _textbox;
				
		private Hashtable DeleteButtons = new Hashtable();

		// referenced datatype
		private Cms.BusinessLogic.datatype.BaseDataType _datatype;

		public KeyValuePrevalueEditor(Cms.BusinessLogic.datatype.BaseDataType DataType) 
		{
			// state it knows its datatypedefinitionid
			_datatype = DataType;
			setupChildControls();

			// Bootstrap delete.
			if (System.Web.HttpContext.Current.Request["delete"] != null) {
				DeletePrevalue(int.Parse(System.Web.HttpContext.Current.Request["delete"]));
			}
			
		}
		
		private void DeletePrevalue(int id) {
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(umbraco.GlobalSettings.DbDSN,CommandType.Text,"delete from cmsDataTypePrevalues where id = " + id);
		}

		private void setupChildControls() 
		{
			_dropdownlist = new DropDownList();
			_dropdownlist.ID = "dbtype";
			
			_textbox = new TextBox();
			_textbox.ID = "AddValue";

			// put the childcontrols in context - ensuring that
			// the viewstate is persisted etc.
			this.Controls.Add(_dropdownlist);
			this.Controls.Add(_textbox);

			_dropdownlist.Items.Add(DBTypes.Date.ToString());
			_dropdownlist.Items.Add(DBTypes.Integer.ToString());
			_dropdownlist.Items.Add(DBTypes.Ntext.ToString());
			_dropdownlist.Items.Add(DBTypes.Nvarchar.ToString());
			
		}
		
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			if (!Page.IsPostBack)
			{
				_dropdownlist.SelectedValue = _datatype.DBType.ToString();

			}
		}
		
		public Control Editor 
		{
			get
			{
				return this;
			}
		}

		public void Save() 
		{
            _datatype.DBType = (Cms.BusinessLogic.datatype.DBTypes)Enum.Parse(typeof(Cms.BusinessLogic.datatype.DBTypes), _dropdownlist.SelectedValue, true);
			// If the add new prevalue textbox is filled out - add the value to the collection.
			if (_textbox.Text != "") 
			{
				SqlParameter[] SqlParams = new SqlParameter[] {
								new SqlParameter("@value",_textbox.Text),
								new SqlParameter("@dtdefid",_datatype.DataTypeDefinitionId)};
				Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(umbraco.GlobalSettings.DbDSN,CommandType.Text,"insert into cmsDataTypePrevalues (datatypenodeid,[value],sortorder,alias) values (@dtdefid,@value,0,'')",SqlParams);
				_textbox.Text = "";
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.WriteLine("<table>");
			writer.WriteLine("<tr><td colspan='2'>Database datatype</td><td>");
			_dropdownlist.RenderControl(writer);
			writer.Write("</td></tr>");
			writer.Write("<tr><th>Value</th><th colspan='2'>Text</th></tr>");
			SortedList _prevalues = Prevalues;
			foreach (string key in _prevalues.Keys) 
			{
				writer.Write("<tr><td> " + key + "</td><td>" +_prevalues[key].ToString() + "</td><td><a href='?id=" + _datatype.DataTypeDefinitionId + "&delete=" + key.ToString() +"'>" + UI.Text("delete") + "</a></td></tr>");
			}
			writer.Write("<tr><th>Add prevalue</th><td colspan='2'>");
			_textbox.RenderControl(writer);
			writer.WriteLine("</td></tr>");
			writer.Write("</table>");
		}

		public SortedList Prevalues {
			get {
				SortedList retval = new SortedList();
				SqlDataReader dr = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteReader(
					umbraco.GlobalSettings.DbDSN,
					CommandType.Text,
					"Select id, [value] from cmsDataTypeprevalues where DataTypeNodeId = "
					+ _datatype.DataTypeDefinitionId + " order by sortorder");
				
				while (dr.Read()) {
					retval.Add(dr["id"].ToString(),dr["value"].ToString());
				}
				dr.Close();
				return retval;
			}
		}
	}
}
