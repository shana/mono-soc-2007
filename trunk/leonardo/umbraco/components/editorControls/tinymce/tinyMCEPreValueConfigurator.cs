using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace umbraco.editorControls.tinymce
{
    class tinyMCEPreValueConfigurator : System.Web.UI.WebControls.PlaceHolder, interfaces.IDataPrevalue
    {
		// UI controls
		private CheckBoxList _editorButtons;
        private CheckBox _enableRightClick;
        private DropDownList _dropdownlist;
        private CheckBoxList _advancedUsersList;
        private CheckBoxList _stylesheetList;
        private TextBox _width = new TextBox();
        private TextBox _height = new TextBox();
        private CheckBox _fullWidth = new CheckBox();
        private RegularExpressionValidator _widthValidator = new RegularExpressionValidator();
        private RegularExpressionValidator _heightValidator = new RegularExpressionValidator();
				
		// referenced datatype
		private cms.businesslogic.datatype.BaseDataType _datatype;
        private string _selectedButtons = "";
        private string _advancedUsers = "";
        private string _stylesheets = "";

        public tinyMCEPreValueConfigurator(cms.businesslogic.datatype.BaseDataType DataType) 
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

            _editorButtons = new CheckBoxList();
            _editorButtons.ID = "editorButtons";
            _editorButtons.RepeatColumns = 4;
            _editorButtons.CellPadding = 3;

            _enableRightClick = new CheckBox();
		    _enableRightClick.ID = "enableRightClick";

            _advancedUsersList = new CheckBoxList();
            _advancedUsersList.ID = "advancedUsersList";

            _stylesheetList = new CheckBoxList();
            _stylesheetList.ID = "stylesheetList";


            // put the childcontrols in context - ensuring that
			// the viewstate is persisted etc.
            Controls.Add(_dropdownlist);
            Controls.Add(_enableRightClick);
            Controls.Add(_editorButtons);
            Controls.Add(_advancedUsersList);
            Controls.Add(_stylesheetList);
            Controls.Add(_width);
		    Controls.Add(_widthValidator);
            Controls.Add(_height);
            Controls.Add(_heightValidator);
            //            Controls.Add(_fullWidth);

		}
		
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
            // add ids to controls
            _width.ID = "width";
            _height.ID = "height";


            // initialize validators
            _widthValidator.ValidationExpression = "0*[1-9][0-9]*";
            _widthValidator.ErrorMessage = "Width must be an integer";
		    _widthValidator.Display = ValidatorDisplay.Dynamic;
            _widthValidator.ControlToValidate = _width.ID;
            _heightValidator.ValidationExpression = "0*[1-9][0-9]*";
            _heightValidator.ErrorMessage = "Height must be an integer";
            _heightValidator.ControlToValidate = _height.ID;
            _heightValidator.Display = ValidatorDisplay.Dynamic;
            
            if (!Page.IsPostBack)
			{
				string[] config = Configuration.Split("|".ToCharArray());
				if (config.Length > 0) 
				{
                    _selectedButtons = config[0];

                    if (config.Length > 1)
                        if (config[1] == "1")
                            _enableRightClick.Checked = true;

                    if (config.Length > 2)
                        _advancedUsers = config[2];

                    if (config.Length > 4 && config[4].Split(',').Length > 1)
                    {
//                        if (config[3] == "1")
//                            _fullWidth.Checked = true;
//                        else
//                        {
                            _width.Text = config[4].Split(',')[0];
                            _height.Text = config[4].Split(',')[1];
//                        }
                    }

                    // if width and height are empty or lower than 0 then set default sizes:
				    int tempWidth, tempHeight;
                    int.TryParse(_width.Text, out tempWidth);
                    int.TryParse(_height.Text, out tempHeight);
                    if (_width.Text.Trim() == "" || tempWidth < 1)
                        _width.Text = "500";
                    if (_height.Text.Trim() == "" || tempHeight < 1)
                        _height.Text = "400";

                    if (config.Length > 4)
                        _stylesheets = config[5];
                }
              
                // add editor buttons
                IDictionaryEnumerator ide = tinyMCEConfiguration.SortedCommands.GetEnumerator();
                while (ide.MoveNext()) 
                {
                    tinyMCECommand cmd = (tinyMCECommand) ide.Value;
                    ListItem li = new ListItem(string.Format("<img src=\"{0}\" class=\"tinymceIcon\" alt=\"{1}\" />&nbsp;", cmd.Icon, cmd.Alias), cmd.Alias);
                    if (_selectedButtons.IndexOf(cmd.Alias) > -1)
                        li.Selected = true;

                    _editorButtons.Items.Add(li);
                }

                // add users
                foreach (BusinessLogic.UserType ut in BusinessLogic.UserType.getAll)
                {
                    ListItem li = new ListItem(ut.Name, ut.Id.ToString());
                    if (("," + _advancedUsers + ",").IndexOf("," + ut.Id.ToString() + ",") > -1)
                        li.Selected = true;

                    _advancedUsersList.Items.Add(li);
                }

                // add stylesheets
                foreach (cms.businesslogic.web.StyleSheet st in cms.businesslogic.web.StyleSheet.GetAll())
                {
                    ListItem li = new ListItem(st.Text, st.Id.ToString());
                    if (("," + _stylesheets + ",").IndexOf("," + st.Id.ToString() + ",") > -1)
                        li.Selected = true;

                    _stylesheetList.Items.Add(li);
                }
                

                // Mark the current db type
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
            _datatype.DBType = (cms.businesslogic.datatype.DBTypes)Enum.Parse(typeof(cms.businesslogic.datatype.DBTypes), _dropdownlist.SelectedValue, true);

			// Generate data-string
            string data = ",";

            foreach (ListItem li in _editorButtons.Items)
                if (li.Selected)
                    data += li.Value + ",";

            data += "|";

            if (_enableRightClick.Checked)
                data += "1";
            else
                data += "0";

            data += "|";

            foreach (ListItem li in _advancedUsersList.Items)
                if (li.Selected)
                    data += li.Value + ",";

            data += "|";

            // full width currenctly not supported
		    data += "0|";
            /*
            if (_fullWidth.Checked)
                data += "1|";
            else
                data += "0|";
            */

            data += _width.Text + "," + _height.Text + "|";

            foreach (ListItem li in _stylesheetList.Items)
                if (li.Selected)
                    data += li.Value + ",";
		    data += "|";

//			string data = _textboxSender.Text + "|"+ _textboxEmail.Text + "|" + _dropdownlistMG.SelectedValue;

            // If the add new prevalue textbox is filled out - add the value to the collection.
			SqlParameter[] SqlParams = new SqlParameter[] {
																new SqlParameter("@value",data),
																new SqlParameter("@dtdefid",_datatype.DataTypeDefinitionId)};
			SqlHelper.ExecuteNonQuery(umbraco.GlobalSettings.DbDSN,CommandType.Text,"delete from cmsDataTypePrevalues where datatypenodeid = @dtdefid",SqlParams);
			SqlHelper.ExecuteNonQuery(umbraco.GlobalSettings.DbDSN,CommandType.Text,"insert into cmsDataTypePrevalues (datatypenodeid,[value],sortorder,alias) values (@dtdefid,@value,0,'')",SqlParams);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.WriteLine("<table>");
			writer.WriteLine("<tr><th>Database datatype</th><td>");
			_dropdownlist.RenderControl(writer);
			writer.Write("</td></tr>");
            writer.Write("<tr><th>Buttons:</th><td>");
            _editorButtons.RenderControl(writer);
            writer.Write("</td></tr>");
            writer.Write("<tr><th>Related stylesheets:</th><td>");
            _stylesheetList.RenderControl(writer);
            writer.Write("</td></tr>");
            writer.Write("<tr><th>Enable Contextmenu:</th><td>");
            _enableRightClick.RenderControl(writer);
            writer.Write("</td></tr>");
            writer.Write("<tr><th>Enable advanced settings for:</th><td>");
            _advancedUsersList.RenderControl(writer);
            writer.Write("</td></tr>");
            writer.Write("<tr><th>"); //"Size:</th><td>Maximum width and height: ");
//            _fullWidth.RenderControl(writer);
            writer.Write("Width and height:</th><td>");
            _width.RenderControl(writer);
            _widthValidator.RenderControl(writer);
            writer.Write(" x ");
            _height.RenderControl(writer);
            _heightValidator.RenderControl(writer);
            writer.Write("</td></tr>");
            writer.Write("</table>");
		}

		public string Configuration 
		{
			get 
			{
                try
                {
                    return SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text, "select value from cmsDataTypePrevalues where datatypenodeid = @datatypenodeid", new SqlParameter("@datatypenodeid", _datatype.DataTypeDefinitionId)).ToString();
                }
                catch
                {
                    return "";
                }
			}
		}

	}
}
