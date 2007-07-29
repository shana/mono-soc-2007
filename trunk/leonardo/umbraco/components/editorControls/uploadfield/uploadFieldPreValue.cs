using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace umbraco.editorControls.uploadfield
{
    class uploadFieldPreValue : System.Web.UI.WebControls.PlaceHolder, interfaces.IDataPrevalue
	{
	
		// UI controls
        private TextBox _textboxThumbnails;
		private DropDownList _dropdownlist;
				
		// referenced datatype
		private Cms.BusinessLogic.datatype.BaseDataType _datatype;

        public uploadFieldPreValue(Cms.BusinessLogic.datatype.BaseDataType DataType) 
		{
			// state it knows its datatypedefinitionid
			_datatype = DataType;
			setupChildControls();

		}
		
		private void setupChildControls() 
		{
			_dropdownlist = new DropDownList();
			_dropdownlist.ID = "dbtype";
			_dropdownlist.Items.Add(DBTypes.Date.ToString());
			_dropdownlist.Items.Add(DBTypes.Integer.ToString());
			_dropdownlist.Items.Add(DBTypes.Ntext.ToString());
			_dropdownlist.Items.Add(DBTypes.Nvarchar.ToString());


			_textboxThumbnails = new TextBox();
            _textboxThumbnails.ID = "thumbNailSizes";

			// put the childcontrols in context - ensuring that
			// the viewstate is persisted etc.
			this.Controls.Add(_dropdownlist);
            this.Controls.Add(_textboxThumbnails);

		}
		
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			if (!Page.IsPostBack)
			{
				string[] config = Configuration.Split("|".ToCharArray());
				if (config.Length > 0) 
				{
                    _textboxThumbnails.Text = config[0];
				}
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
            _datatype.DBType = (umbraco.cms.businesslogic.datatype.DBTypes)Enum.Parse(typeof(umbraco.cms.businesslogic.datatype.DBTypes), _dropdownlist.SelectedValue, true);

			// Generate data-string
            string data = _textboxThumbnails.Text;
			// If the add new prevalue textbox is filled out - add the value to the collection.
			SqlParameter[] SqlParams = new SqlParameter[] {
																new SqlParameter("@value",data),
																new SqlParameter("@dtdefid",_datatype.DataTypeDefinitionId)};
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(umbraco.GlobalSettings.DbDSN,CommandType.Text,"delete from cmsDataTypePrevalues where datatypenodeid = @dtdefid",SqlParams);
			Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(umbraco.GlobalSettings.DbDSN,CommandType.Text,"insert into cmsDataTypePrevalues (datatypenodeid,[value],sortorder,alias) values (@dtdefid,@value,0,'')",SqlParams);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.WriteLine("<table>");
			writer.WriteLine("<tr><th>Database datatype</th><td>");
			_dropdownlist.RenderControl(writer);
			writer.Write("</td></tr>");
			writer.Write("<tr><th>Thumbnail sizes (max width/height, semicolon separated for multiples):</th><td>");
            _textboxThumbnails.RenderControl(writer);
			writer.Write("</td></tr>");
			writer.Write("</table>");
		}

		public string Configuration 
		{
			get 
			{
                object configVal = Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text, "select value from cmsDataTypePrevalues where datatypenodeid = @datatypenodeid", new SqlParameter("@datatypenodeid", _datatype.DataTypeDefinitionId));
                if (configVal != null)
                    return configVal.ToString();
                else
                    return "";
			}
		}

	}
}
