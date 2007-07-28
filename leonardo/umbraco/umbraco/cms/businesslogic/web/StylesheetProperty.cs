using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace umbraco.cms.businesslogic.web
{
	public class StylesheetProperty : CMSNode
	{
		private string _alias;
		private string _value;

		private static Guid moduleObjectType = new Guid("5555da4f-a123-42b2-4488-dcdfb25e4111");
		// internal static moduleId = 

		public StylesheetProperty(int id) : base(id)
		{
			initProperty();
		}

		public StylesheetProperty(Guid id) : base(id)
		{
			initProperty();
		}
		private  void initProperty() {
			SqlDataReader dr = SqlHelper.ExecuteReader(_ConnString, CommandType.Text,"Select stylesheetPropertyAlias,stylesheetPropertyValue from cmsStylesheetProperty where nodeId = " + this.Id.ToString());
			if (dr.Read())
			{
				_alias = dr["stylesheetPropertyAlias"].ToString();
				_value = dr["stylesheetPropertyValue"].ToString();
			} 
			else
				throw new ArgumentException("NO DATA EXSISTS");
			dr.Close();
		}


		public StyleSheet StyleSheet() {
			return new StyleSheet(this.Parent.Id);
		}


		public string Alias {
			get{return _alias;}
			set {
				SqlHelper.ExecuteNonQuery(_ConnString,CommandType.Text,"update cmsStylesheetProperty set stylesheetPropertyAlias = '"+ value.Replace("'","''")+"' where nodeId = " + this.Id);
				_alias=value;
			}
		}
	
		public string value {
			get {return _value;}
			set {
				SqlHelper.ExecuteNonQuery(_ConnString,CommandType.Text,"update cmsStylesheetProperty set stylesheetPropertyValue = '"+ value.Replace("'","''")+"' where nodeId = " + this.Id);
				_value=value;

			}
		}

		public static StylesheetProperty MakeNew(string Text, StyleSheet sheet, BusinessLogic.User user) {
			CMSNode newNode = CMSNode.MakeNew(sheet.Id, moduleObjectType, user.Id, 2, Text, Guid.NewGuid());
			SqlHelper.ExecuteNonQuery(_ConnString,CommandType.Text,"Insert into cmsStylesheetProperty (nodeId,stylesheetPropertyAlias,stylesheetPropertyValue) values ('"+ newNode.Id +"','" + Text+ "','')");
			return new StylesheetProperty(newNode.Id);
			
		}

		new public void delete() 
		{
			SqlHelper.ExecuteNonQuery(_ConnString,CommandType.Text,"delete from cmsStylesheetProperty where nodeId = @nodeId", new SqlParameter("@nodeId", this.Id));
			base.delete();
		}

		public override string ToString()
		{
			return this.Alias +" {\n"+ this.value+"\n}\n";
		}

	}
}
