using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace umbraco.controls
{
	/// <summary>
	/// Summary description for dualSelectbox.
	/// </summary>
	public class DualSelectbox : System.Web.UI.WebControls.WebControl, System.Web.UI.INamingContainer
	{
		private ListItemCollection _items = new ListItemCollection();

		private ListBox _possibleValues = new ListBox();
		private ListBox _selectedValues = new ListBox();
		private HtmlInputHidden _value = new HtmlInputHidden();
		private int _rows = 8;

		public ListItemCollection Items 
		{
			get 
			{
				EnsureChildControls();
				return _items;
			}
		}

		public new int Width 
		{
			set 
			{
				_possibleValues.Width = new Unit(value);
				_selectedValues.Width = new Unit(value);
			}
		}


		public string Value 
		{
			get 
			{
				EnsureChildControls();
				return _value.Value;
			}
			
			set 
			{
				EnsureChildControls();
				_value.Value = value;
				foreach(ListItem li in _items) 
				{
					if (((string) (","+_value.Value+",")).IndexOf(","+li.Value+",") > -1) 
					{
						_selectedValues.Items.Add(li);
						_possibleValues.Items.Remove(li);
					}
																				
				}
			}
		}

		public int Rows 
		{
			set 
			{
				EnsureChildControls();
				_rows = value;
			}
		}

		public DualSelectbox()
		{
		}

		public override ControlCollection Controls
		{
			get
			{
				EnsureChildControls();
				return base.Controls;
			}
		}
		protected override void CreateChildControls()
		{
			Controls.Clear();

			_possibleValues.ID = "posVals";
			_selectedValues.ID = "selVals";
			_possibleValues.SelectionMode = ListSelectionMode.Multiple;
			_selectedValues.SelectionMode = ListSelectionMode.Multiple;
			_possibleValues.CssClass = "guiInputTextStandard";
			_selectedValues.CssClass = "guiInputTextStandard";
			_possibleValues.Rows = _rows;
			_selectedValues.Rows = _rows;

			_value.ID = "theValue";
			if (helper.Request(_value.ClientID) != "")
				_value.Value = helper.Request(_value.ClientID);

			HtmlTable table = new HtmlTable();
			table.CellPadding = 5;
			table.CellSpacing = 0;
			table.Border = 0;
			HtmlTableRow row = new HtmlTableRow();
			table.Controls.Add(row);
			HtmlTableCell cFirst = new HtmlTableCell();
			cFirst.Controls.Add(_possibleValues);
			row.Controls.Add(cFirst);
			HtmlTableCell cButtons = new HtmlTableCell();
			cButtons.Controls.Add(new LiteralControl("<input type=\"button\" class=\"guiInputButton\" onClick=\"dualSelectBoxShift('" + this.ClientID + "_" + _possibleValues.ClientID + "','" + this.ClientID + "_" + _selectedValues.ClientID + "','" + this.ClientID + "_" + _value.ClientID + "');\" value=\"&gt;&gt;\"/><br/><br/><input type=\"button\" class=\"guiInputButton\" onClick=\"dualSelectBoxShift('" + this.ClientID + "_" + _possibleValues.ClientID + "','" + this.ClientID + "_" + _selectedValues.ClientID + "','" + this.ClientID + "_" + _value.ClientID + "');\" value=\"&lt;&lt;\"/>"));
			row.Controls.Add(cButtons);
			HtmlTableCell cSecond = new HtmlTableCell();
			cSecond.Controls.Add(_selectedValues);
			row.Controls.Add(cSecond);
			this.Controls.Add(table);
			this.Controls.Add(_value);

		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender (e);

			_selectedValues.Items.Clear();
			_possibleValues.Items.Clear();

			foreach(ListItem li in _items) 
			{
				if (((string) (","+ this.Value +",")).IndexOf(","+li.Value+",") > -1) 
					_selectedValues.Items.Add(li);
				else
					_possibleValues.Items.Add(li);											
			}

		}

		protected override void OnInit(EventArgs e)
		{
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "dualSelectBox", "<script language='javascript' src='" + GlobalSettings.Path + "/js/dualSelectBox.js'></script>");
			base.OnInit (e);

		}


	}
}
