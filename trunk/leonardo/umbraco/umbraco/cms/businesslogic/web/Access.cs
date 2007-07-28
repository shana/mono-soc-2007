using System;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.IO;

namespace umbraco.cms.businesslogic.web
{
	/// <summary>
	/// Summary description for Access.
	/// </summary>
	public class Access
	{
		public Access()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		static private Hashtable _checkedPages = new Hashtable();

		static private XmlDocument _accessXmlContent;
		static private string _accessXmlSource = System.Web.HttpContext.Current.Server.MapPath(GlobalSettings.StorageDirectory + "/access.xml");

		private static void clearCheckPages() 
		{
			_checkedPages.Clear();
		}

		public static XmlDocument AccessXml 
		{
			get {
				if (_accessXmlContent == null) 
				{
					_accessXmlContent = new XmlDocument();

					if (!System.IO.File.Exists(_accessXmlSource)) 
					{
						System.IO.FileStream f = System.IO.File.Open(_accessXmlSource, FileMode.Create);
						System.IO.StreamWriter sw = new StreamWriter(f);
						sw.WriteLine("<access/>");
						sw.Close();
						f.Close();
					}
					_accessXmlContent.Load(_accessXmlSource);
				}
				return _accessXmlContent;
			}
		}

		public static void AddMemberGroupToDocument(int DocumentId, int MemberGroupId) 
		{
			XmlElement x = (XmlElement) getPage(DocumentId);
			
			if (x == null)
				throw new Exception("Document is not protected!");
			else 
			{
				if (x.SelectSingleNode("group [@id = '" + MemberGroupId.ToString() + "']") == null) 
				{
					XmlElement groupXml = (XmlElement) AccessXml.CreateNode(XmlNodeType.Element, "group", "");
					groupXml.SetAttribute("id", MemberGroupId.ToString());
					x.AppendChild(groupXml);
					save();
				}
			}
		}

		public static void AddMemberToDocument(int DocumentId, int MemberId) 
		{
			XmlElement x = (XmlElement) getPage(DocumentId);
			
			if (x == null)
				throw new Exception("Document is not protected!");
			else 
			{
				if (x.Attributes.GetNamedItem("memberId") != null)
					x.Attributes.GetNamedItem("memberId").Value = MemberId.ToString();
				else
					x.SetAttribute("memberId", MemberId.ToString());
				save();
			}
		}

		public static void RemoveMemberGroupFromDocument(int DocumentId, int MemberGroupId) 
		{
			XmlElement x = (XmlElement) getPage(DocumentId);
			
			if (x == null)
				throw new Exception("Document is not protected!");
			else 
			{
				XmlNode xGroup = x.SelectSingleNode("group [@id = '" + MemberGroupId.ToString() + "']");
				if (xGroup != null) 
				{
					x.RemoveChild(xGroup);
					save();
				}
			}
		}

		public static void ProtectPage(bool Simple, int DocumentId, int LoginDocumentId, int ErrorDocumentId) 
		{
			XmlElement x = (XmlElement) getPage(DocumentId);
			if (x == null) 
			{
				x = (XmlElement) _accessXmlContent.CreateNode(XmlNodeType.Element, "page", "");
				AccessXml.DocumentElement.AppendChild(x);
			}
			// if using simple mode, make sure that all existing groups are removed
			else if (Simple) 
			{
				x.RemoveAll();
			}
			x.SetAttribute("id", DocumentId.ToString());
			x.SetAttribute("loginPage", LoginDocumentId.ToString());
			x.SetAttribute("noRightsPage", ErrorDocumentId.ToString());
			x.SetAttribute("simple", Simple.ToString());
			save();
			clearCheckPages();
		}

		public static void RemoveProtection(int DocumentId) 
		{
			XmlElement x = (XmlElement) getPage(DocumentId);
			if (x != null) 
			{
				x.ParentNode.RemoveChild(x);
				save();
				clearCheckPages();
			}
		}

		private static void save() 
		{
			System.IO.FileStream f = System.IO.File.Open(_accessXmlSource, FileMode.Create);
			AccessXml.Save(f);
			f.Close();
		}

		public static bool IsProtectedByGroup(int DocumentId, int GroupId) 
		{
			bool isProtected = false;

			cms.businesslogic.web.Document d = new Document(DocumentId);

			if (!IsProtected(DocumentId, d.Path))
				isProtected = false;
			else 
			{
				XmlNode currentNode = getPage(getProtectedPage(d.Path));
				if (currentNode.SelectSingleNode("./group [@id=" + GroupId.ToString() + "]") != null) 
				{
					isProtected = true;
				}
			}

			return isProtected;
		}

		public static cms.businesslogic.member.MemberGroup[] GetAccessingGroups(int DocumentId) 
		{
			cms.businesslogic.web.Document d = new Document(DocumentId);

			if (!IsProtected(DocumentId, d.Path))
				return null;
			else 
			{
                XmlNode currentNode = getPage(getProtectedPage(d.Path));
				cms.businesslogic.member.MemberGroup[] mg = new umbraco.cms.businesslogic.member.MemberGroup[currentNode.SelectNodes("./group").Count];
				int count = 0;
				foreach (XmlNode n in currentNode.SelectNodes("./group"))
				{
					mg[count] = new cms.businesslogic.member.MemberGroup(int.Parse(n.Attributes.GetNamedItem("id").Value));
					count++;
				}
				return mg;
			}

		}

		public static cms.businesslogic.member.Member GetAccessingMember(int DocumentId) 
		{
			cms.businesslogic.web.Document d = new Document(DocumentId);

			if (!IsProtected(DocumentId, d.Path))
				return null;
			else if (GetProtectionType(DocumentId) != ProtectionType.Simple)
				throw new Exception("Document isn't protected using Simple mechanism. Use GetAccessingMemberGroups instead");
			else 
			{
				XmlNode currentNode = getPage(getProtectedPage(d.Path));
				if (currentNode.Attributes.GetNamedItem("memberId") != null)
					return new cms.businesslogic.member.Member(int.Parse(
						currentNode.Attributes.GetNamedItem("memberId").Value));
				else
					throw new Exception("Document doesn't contain a memberId. This might be caused if document is protected using umbraco RC1 or older.");

			}

		}


		public static bool HasAccess(int DocumentId, cms.businesslogic.member.Member Member) 
		{
			bool hasAccess = false;

			cms.businesslogic.web.Document d = new Document(DocumentId);

			if (!IsProtected(DocumentId, d.Path))
				hasAccess = true;
			else 
			{
				XmlNode currentNode = getPage(getProtectedPage(d.Path));
				if (Member != null) 
				{
					IDictionaryEnumerator ide = Member.Groups.GetEnumerator();
					while(ide.MoveNext())
					{
						cms.businesslogic.member.MemberGroup mg = (cms.businesslogic.member.MemberGroup) ide.Value;
						if (currentNode.SelectSingleNode("./group [@id=" + mg.Id.ToString() + "]") != null) 
						{
							hasAccess = true;
							break;
						}
					}
				}
			}

			return hasAccess;
		}


		public static bool HasAccess(int DocumentId, string Path, cms.businesslogic.member.Member Member) 
		{
			bool hasAccess = false;

			if (!IsProtected(DocumentId, Path))
				hasAccess = true;
			else 
			{
				XmlNode currentNode = getPage(getProtectedPage(Path));
				if (Member != null) 
				{
					IDictionaryEnumerator ide = Member.Groups.GetEnumerator();
					while(ide.MoveNext())
					{
						cms.businesslogic.member.MemberGroup mg = (cms.businesslogic.member.MemberGroup) ide.Value;
						if (currentNode.SelectSingleNode("./group [@id=" + mg.Id.ToString() + "]") != null) 
						{
							hasAccess = true;
							break;
						}
					}
				}
			}

			return hasAccess;
		}
		public static ProtectionType GetProtectionType(int DocumentId) 
		{
			XmlNode x = getPage(DocumentId);
			try 
			{
				if (bool.Parse(x.Attributes.GetNamedItem("simple").Value))
					return ProtectionType.Simple;
				else
					return ProtectionType.Advanced;
			} 
			catch 
			{
				return ProtectionType.NotProtected;
			}

		}

		public static bool IsProtected(int DocumentId, string Path) 
		{
			bool isProtected = false;

			if (!_checkedPages.ContainsKey(DocumentId)) 
			{
				foreach(string id in Path.Split(',')) 
				{
					if (getPage(int.Parse(id)) != null) 
					{
						isProtected = true;
						break;
					}
				}

                // Add thread safe updating to the hashtable
                System.Web.HttpContext.Current.Application.Lock();
                if (!_checkedPages.ContainsKey(DocumentId))
				    _checkedPages.Add(DocumentId, isProtected);
                System.Web.HttpContext.Current.Application.UnLock();
            }
            else
				isProtected = (bool) _checkedPages[DocumentId];
			
			return isProtected;
		}

		public static int GetErrorPage(string Path) 
		{
			return int.Parse(getPage(getProtectedPage(Path)).Attributes.GetNamedItem("noRightsPage").Value);
		}

		public static int GetLoginPage(string Path) 
		{
			return int.Parse(getPage(getProtectedPage(Path)).Attributes.GetNamedItem("loginPage").Value);
		}

		private static int getProtectedPage(string Path) 
		{
			int protectedPage = 0;

			foreach(string id in Path.Split(',')) 
				if (getPage(int.Parse(id)) != null) 
					protectedPage = int.Parse(id);

            return protectedPage;
		}

		private static XmlNode getPage(int documentId) 
		{
			XmlNode x = AccessXml.SelectSingleNode("/access/page [@id=" + documentId.ToString() + "]");
			return x;
		}
	}

	public enum ProtectionType 
	{
		NotProtected,
		Simple,
		Advanced
	}
}
