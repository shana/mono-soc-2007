using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using System.Xml;

namespace umbraco.cms.businesslogic.property
{
	/// <summary>
	/// Property class encapsulates property factory, ensuring that the work
	/// with umbraco generic properties stays nice and easy..
	/// </summary>
	public class Property
	{
		private static string  _connstring = GlobalSettings.DbDSN;
		propertytype.PropertyType _pt;
		interfaces.IData _data;
		private int _id;
	
		public Property(int Id, propertytype.PropertyType pt) 
		{
			
			_pt = pt;
			_id = Id;
			_data = _pt.DataTypeDefinition.DataType.Data;
			_data.PropertyId = Id;
		}

		public Property(int Id) 
		{
			_id = Id;
			_pt = umbraco.cms.businesslogic.propertytype.PropertyType.GetPropertyType(int.Parse(
				SqlHelper.ExecuteScalar(GlobalSettings.DbDSN, CommandType.Text, "select propertytypeid from cmsPropertyData where id = @id", new SqlParameter("@id", Id)).ToString()));
			_data = _pt.DataTypeDefinition.DataType.Data;
			_data.PropertyId = Id;
		}

		public Guid VersionId 
		{
			get 
			{
				return (Guid) SqlHelper.ExecuteScalar(_connstring, CommandType.Text,"Select versionId from cmsPropertyData where id = " + _id.ToString());
			}
		}
		public int Id {
			get {return _id;}
		}
		public propertytype.PropertyType PropertyType {
			get{return _pt;}
		}

		public object Value {
			get {
				return _data.Value;
			}
			set {
				_data.Value = value;
			}
		}

		public void delete() {
			int contentId = int.Parse(SqlHelper.ExecuteScalar(_connstring,CommandType.Text,"Select contentNodeId from cmsPropertyData where Id = " + _id).ToString());
            SqlHelper.ExecuteNonQuery(_connstring, CommandType.Text, "Delete from cmsPropertyData where PropertyTypeId =" + _pt.Id + " And contentNodeId = "+ contentId);
			_data.Delete();
		}

		public XmlNode ToXml(XmlDocument xd) 
		{
			XmlNode x = xd.CreateNode(XmlNodeType.Element, "data", "");

			// Version not necessary after sql publishing has been removed

			// Alias
			XmlAttribute alias = xd.CreateAttribute("alias");
			alias.Value = this.PropertyType.Alias;
			x.Attributes.Append(alias);

			// Check for cdata section
			// x.AppendChild(xd.CreateCDataSection(this.Value.ToString()));
			x.AppendChild(_data.ToXMl(xd));
			
			return x;
		}
		public static Property MakeNew(propertytype.PropertyType pt, Content c, Guid VersionId) 
		{
			int newPropertyId = int.Parse(SqlHelper.ExecuteScalar(_connstring, CommandType.Text,"Insert into cmsPropertyData (contentNodeId, versionId, propertyTypeId) values ("+ c.Id.ToString()+ ", '" + VersionId.ToString() + "',"+ pt.Id.ToString()+") select @@identity").ToString());
			interfaces.IData d = pt.DataTypeDefinition.DataType.Data;
			d.MakeNew(newPropertyId);
			return new Property(newPropertyId, pt);
		}
	}

    public class Properties : List<property.Property>
    {

    }

}