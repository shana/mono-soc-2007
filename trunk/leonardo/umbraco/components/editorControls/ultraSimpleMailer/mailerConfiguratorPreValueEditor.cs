using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace umbraco.editorControls.ultraSimpleMailer
{
	/// <summary>
	/// Summary description for mailerConfiguratorPreValueEditor.
	/// </summary>
	public class mailerConfiguratorPreValueEditor : PlaceHolder, interfaces.IDataPrevalue
	{
	
		// UI controls
		private TextBox _textboxEmail;
		private TextBox _textboxSender;
		private DropDownList _dropdownlist;
		private DropDownList _dropdownlistMG;
				
		// referenced datatype
		private cms.businesslogic.datatype.BaseDataType _datatype;

		public mailerConfiguratorPreValueEditor(cms.businesslogic.datatype.BaseDataType DataType) 
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
			
			_dropdownlistMG = new DropDownList();
			_dropdownlistMG.ID = "memberGroup";

			_textboxSender = new TextBox();
			_textboxSender.ID = "SenderName";
			_textboxEmail = new TextBox();
			_textboxEmail.ID = "SenderEmail";

			// put the childcontrols in context - ensuring that
			// the viewstate is persisted etc.
			Controls.Add(_dropdownlist);
			Controls.Add(_dropdownlistMG);
			Controls.Add(_textboxSender);
			Controls.Add(_textboxEmail);


			// Get all membergroups
			foreach(cms.businesslogic.member.MemberGroup mg in cms.businesslogic.member.MemberGroup.GetAll)
				_dropdownlistMG.Items.Add(new ListItem(mg.Text, mg.Id.ToString()));
		}
		
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			if (!Page.IsPostBack)
			{
				string[] config = Configuration.Split("|".ToCharArray());
				if (config.Length > 1) 
				{
					_textboxSender.Text = config[0];
					_textboxEmail.Text = config[1];
					_dropdownlistMG.SelectedValue = config[2];
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
			string data = _textboxSender.Text + "|"+ _textboxEmail.Text + "|" + _dropdownlistMG.SelectedValue;
			// If the add new prevalue textbox is filled out - add the value to the collection.
			SqlParameter[] SqlParams = new SqlParameter[] {
																new SqlParameter("@value",data),
																new SqlParameter("@dtdefid",_datatype.DataTypeDefinitionId)};
			SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN,CommandType.Text,"delete from cmsDataTypePrevalues where datatypenodeid = @dtdefid",SqlParams);
			SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN,CommandType.Text,"insert into cmsDataTypePrevalues (datatypenodeid,[value],sortorder,alias) values (@dtdefid,@value,0,'')",SqlParams);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.WriteLine("<table>");
			writer.WriteLine("<tr><th>Database datatype</th><td>");
			_dropdownlist.RenderControl(writer);
			writer.Write("</td></tr>");
			writer.Write("<tr><th>Sender name:</th><td>");
			_textboxSender.RenderControl(writer);
			writer.Write("</td></tr>");
			writer.Write("<tr><th>Sender email:</th><td>");
			_textboxEmail.RenderControl(writer);
			writer.Write("</td></tr>");
			writer.Write("<tr><th>Membergroup to recieve mail:</th><td>");
			_dropdownlistMG.RenderControl(writer);
			writer.Write("</td></tr>");
			writer.Write("</table>");
		}

		public string Configuration 
		{
			get 
			{
			    object conf =
			        SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text,
			                                "select value from cmsDataTypePrevalues where datatypenodeid = @datatypenodeid",
			                                new SqlParameter("@datatypenodeid", _datatype.DataTypeDefinitionId));
                if (conf != null)
                    return conf.ToString();
                else
                    return "";

			}
		}

	}
}
