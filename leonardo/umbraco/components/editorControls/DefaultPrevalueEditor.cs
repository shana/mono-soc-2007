using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ApplicationBlocks.Data;
using umbraco.interfaces;

namespace umbraco.editorControls
{
    public class DefaultPrevalueEditor : PlaceHolder, IDataPrevalue
    {
        // UI controls
        private TextBox _textbox;
        private DropDownList _dropdownlist;

        // referenced datatype
        private Cms.BusinessLogic.datatype.BaseDataType _datatype;
        private BaseDataType _datatypeOld;

        private bool _isEnsured = false;
        private string _prevalue;
        private bool _displayTextBox;

        /// <summary>
        /// The default editor for editing the build-in pre values in Umbraco
        /// </summary>
        /// <param Name="DataType">The DataType to be parsed</param>
        /// <param Name="DisplayTextBox">Whether to use the default text box</param>
        public DefaultPrevalueEditor(Cms.BusinessLogic.datatype.BaseDataType DataType, bool DisplayTextBox)
        {
            // state it knows its datatypedefinitionid
            _displayTextBox = DisplayTextBox;
            _datatype = DataType;
            setupChildControls();
        }

        /// <summary>
        /// For backwards compatibility, should be replaced in your extension with the constructor that
        /// uses the BaseDataType from the cms.Umbraco.Cms.BusinessLogic.datatype namespace
        /// </summary>
        /// <param Name="DataType">The DataType to be parsed (note: the BaseDataType from editorControls is obsolete)</param>
        /// <param Name="DisplayTextBox">Whether to use the default text box</param>
        public DefaultPrevalueEditor(BaseDataType DataType, bool DisplayTextBox)
        {
            // state it knows its datatypedefinitionid
            _displayTextBox = DisplayTextBox;
            _datatypeOld = DataType;
            setupChildControls();
        }

        private void setupChildControls()
        {
            _dropdownlist = new DropDownList();
            _dropdownlist.ID = "dbtype";

            _textbox = new TextBox();
            _textbox.ID = "prevalues";
            _textbox.Visible = _displayTextBox;

            // put the childcontrols in context - ensuring that
            // the viewstate is persisted etc.
            Controls.Add(_textbox);
            Controls.Add(_dropdownlist);

            _dropdownlist.Items.Add(DBTypes.Date.ToString());
            _dropdownlist.Items.Add(DBTypes.Integer.ToString());
            _dropdownlist.Items.Add(DBTypes.Ntext.ToString());
            _dropdownlist.Items.Add(DBTypes.Nvarchar.ToString());
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                if (_datatype != null)
                    _dropdownlist.SelectedValue = _datatype.DBType.ToString();
                else
                    _dropdownlist.SelectedValue = _datatypeOld.DBType.ToString();
                _textbox.Text = Prevalue;
            }
        }

        public string Prevalue
        {
            get
            {
                ensurePrevalue();
                if (_prevalue == null) {
                    int defId;
                    if (_datatype != null)
                        defId = _datatype.DataTypeDefinitionId;
                    else if (_datatypeOld != null)
                        defId = _datatypeOld.DataTypeDefinitionId;
                    else
                        throw new ArgumentException("Datatype is not initialized");

                    _prevalue =
                        Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text,
                                                "Select [value] from cmsDataTypeprevalues where DataTypeNodeId = " +
                                                defId).ToString();
                }
                return _prevalue;
            }
            set
            {
                int defId;
                if (_datatype != null)
                    defId = _datatype.DataTypeDefinitionId;
                else if (_datatypeOld != null)
                    defId = _datatypeOld.DataTypeDefinitionId;
                else
                    throw new ArgumentException("Datatype is not initialized");

                _prevalue = value;
                ensurePrevalue();
                SqlParameter[] SqlParams = new SqlParameter[]
                    {
                        new SqlParameter("@value", _textbox.Text),
                        new SqlParameter("@dtdefid", defId)
                    };
                // update prevalue
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text,
                                          "update cmsDataTypePrevalues set [value] = @value where datatypeNodeId = @dtdefid",
                                          SqlParams);
            }
        }

        private void ensurePrevalue()
        {
            if (!_isEnsured)
            {

                int defId;
                if (_datatype != null)
                    defId = _datatype.DataTypeDefinitionId;
                else if (_datatypeOld != null)
                    defId = _datatypeOld.DataTypeDefinitionId;
                else
                    throw new ArgumentException("Datatype is not initialized");


                bool hasPrevalue =
                    int.Parse(
                        Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text,
                                                "select count(id) from cmsDataTypePrevalues where dataTypeNodeId = " +
                                               defId).ToString()) > 0;
                SqlParameter[] SqlParams = new SqlParameter[]
                    {
                        new SqlParameter("@value", _textbox.Text),
                        new SqlParameter("@dtdefid", defId)
                    };
                if (!hasPrevalue)
                {
                    Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text,
                                              "insert into cmsDataTypePrevalues (datatypenodeid,[value],sortorder,alias) values (@dtdefid,@value,0,'')",
                                              SqlParams);
                }
                _isEnsured = true;
            }
        }

        public Control Editor
        {
            get { return this; }
        }

        public void Save()
        {
            // save the prevalue data and get on with you life ;)
            if (_datatype != null)
                _datatype.DBType =
                    (Cms.BusinessLogic.datatype.DBTypes)
                    Enum.Parse(typeof (Cms.BusinessLogic.datatype.DBTypes), _dropdownlist.SelectedValue, true);
            else if (_datatypeOld != null)
                _datatypeOld.DBType =
                    (DBTypes)
                    Enum.Parse(typeof (DBTypes), _dropdownlist.SelectedValue, true);


            if (_displayTextBox)
            {
                // If the prevalue editor has an prevalue textbox - save the textbox value as the prevalue
                Prevalue = _textbox.Text;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.WriteLine("<table>");
            writer.WriteLine("<tr><td>Database datatype</td><td>");
            _dropdownlist.RenderControl(writer);
            writer.Write("</td></tr>");
            if (_displayTextBox)
                writer.WriteLine("<tr><td>Prevalue: </td><td>");
            _textbox.RenderControl(writer);
            writer.WriteLine("</td></tr>");
            writer.Write("</table>");
        }

        public static string GetPrevalueFromId(int Id)
        {
            return
                Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text,
                                        "Select [value] from cmsDataTypeprevalues where id = @id",
                                        new SqlParameter("@id", Id)).ToString();
        }
    }
}