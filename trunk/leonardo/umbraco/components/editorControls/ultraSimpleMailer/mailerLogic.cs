using System;
using System.Data.SqlClient;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using System.IO;
using System.Text.RegularExpressions;
using DotNetOpenMail; /* http://dotnetopenmail.sourceforge.net/ */
using System.Collections;

namespace umbraco.editorControls.ultraSimpleMailer
{
	/// <summary>
	/// Summary description for mailerLogic.
	/// </summary>
	public class mailerLogic
	{
		public mailerLogic()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static int GetTotalReceiptients(umbraco.cms.businesslogic.member.MemberGroup Group) 
		{
			return int.Parse(SqlHelper.ExecuteScalar(umbraco.GlobalSettings.DbDSN, CommandType.Text, "select count(*) from cmsMember inner join cmsMember2memberGroup on cmsmember.nodeId = cmsMember2MemberGroup.member where memberGroup = @memberGroupId", new SqlParameter("@memberGroupId", Group.Id)).ToString());
		}

		public static void SendTestmail(string email, 
			umbraco.cms.businesslogic.property.Property Property, 
			string fromName, string fromEmail, bool IsHtml) 
		{
			// version
			string version = Property.VersionId.ToString();

			// Get document
			umbraco.cms.businesslogic.web.Document d = new umbraco.cms.businesslogic.web.Document(umbraco.cms.businesslogic.Content.GetContentFromVersion(Property.VersionId).Id);
			System.Web.HttpContext.Current.Items["pageID"] = d.Id;

			// Format mail
			string subject = d.Text;
			string sender = "\"" + fromName + "\" <" + fromEmail + ">";

			// Get template			
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.IO.StringWriter sw = new StringWriter(sb);
			System.Web.UI.HtmlTextWriter writer = new System.Web.UI.HtmlTextWriter(sw);
			umbraco.template t = new template(d.Template);
			t.ParseWithControls(new umbraco.page(d.Id, d.Version)).RenderControl(writer);
			
			// Embedded emails ;) added by DB, 2005-10-04

			EmailMessage message = mailerHelper.CreateEmbeddedEmail(sb.ToString(), cms.businesslogic.web.Document.GetContentFromVersion(Property.VersionId).Id);
            			
			message.FromAddress = new EmailAddress(fromEmail, fromName);
			message.ToAddresses.Add(new EmailAddress(email));
			message.Subject = subject;
			message.Send(new SmtpServer(GlobalSettings.SmtpServer));
            
		}

		public static void SendMail(umbraco.cms.businesslogic.member.MemberGroup Group, umbraco.cms.businesslogic.property.Property Property, string fromName, string fromEmail, bool IsHtml) 
		{
            // Create ArrayList with emails who've received the e-mail
            ArrayList sent = new ArrayList();

			// set timeout
			System.Web.HttpContext.Current.Server.ScriptTimeout = 43200; // 12 hours
			// version
			string version = Property.VersionId.ToString();

			// Get document
			umbraco.cms.businesslogic.web.Document d = new umbraco.cms.businesslogic.web.Document(umbraco.cms.businesslogic.Content.GetContentFromVersion(Property.VersionId).Id);
			int id = d.Id;
			System.Web.HttpContext.Current.Items["pageID"] = id;

			// Format mail
			string subject = d.Text;
			string sender = "\"" + fromName + "\" <" + fromEmail + ">";

			// Get template			
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.IO.StringWriter sw = new StringWriter(sb);
			System.Web.UI.HtmlTextWriter writer = new System.Web.UI.HtmlTextWriter(sw);
			umbraco.template t = new template(d.Template);
			t.ParseWithControls(new umbraco.page(d.Id, d.Version)).RenderControl(writer);


			EmailMessage message = mailerHelper.CreateEmbeddedEmail(sb.ToString(), cms.businesslogic.web.Document.GetContentFromVersion(Property.VersionId).Id);
            			
			message.FromAddress = new EmailAddress(fromEmail, fromName);
			message.Subject = subject;
			SmtpServer smtpServer = new SmtpServer(GlobalSettings.SmtpServer);

			// Bulk send mails
			int counter = 0;
			System.Text.StringBuilder sbStatus = new System.Text.StringBuilder();
			sbStatus.Append("The Newsletter '" + d.Text + "' was starting at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "\n");
			using(SqlDataReader dr = SqlHelper.ExecuteReader(umbraco.GlobalSettings.DbDSN, CommandType.Text, "select text, email from cmsMember inner join umbracoNode node on node.id = cmsMember.nodeId inner join cmsMember2memberGroup on cmsmember.nodeId = cmsMember2MemberGroup.member where memberGroup = @memberGroupId", new SqlParameter("@memberGroupId", Group.Id)))
			{
				while(dr.Read())
				{
					try
					{
                        if (!sent.Contains(dr["email"].ToString()))
                        {
                            message.ToAddresses.Clear();
                            message.ToAddresses.Add(new EmailAddress(dr["email"].ToString(), dr["text"].ToString()));
                            message.Send(smtpServer);

                            // add to arraylist of receiptients
                            sent.Add(dr["email"].ToString());

                            // Append to status
                            sbStatus.Append("Sent to " + dr["text"].ToString() + " <" + dr["email"].ToString() + "> \n\r");
                        }
                        else
                        {
                            // Append to status
                            sbStatus.Append("E-mail has already been sent to email '" + dr["email"].ToString() + ". You have a duplicate member: " + dr["text"].ToString() + " <" + dr["email"].ToString() + "> \n\r");
                        }
					}
					catch(Exception ee)
					{
						sbStatus.Append("Error sending to " + dr["text"].ToString() + " <" + dr["email"].ToString() + ">: " +
						                ee.ToString() + " \n");
					}

					// For progress bar
					counter++;

					if(counter % 5 == 0)
					{
						System.Web.HttpContext.Current.Application.Lock();
						System.Web.HttpContext.Current.Application["ultraSimpleMailerProgress" + id.ToString() + "Done"] = counter;
						System.Web.HttpContext.Current.Application.UnLock();
					}
				}
			}

			sbStatus.Append("Finished at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "\n");

			// Send status mail
			library.SendMail(fromEmail, fromEmail, "Newsletter status", sbStatus.ToString(), false);
		}

		private static string updateLocalUris(string body, int newsletterId) 
		{
			string currentDomain = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
			string pattern = "href=\"?([^\\\"' >]+)|src=\\\"?([^\\\"' >]+)";
			string appendNewsletter = "umbNl=" + newsletterId.ToString();
			MatchCollection tags = Regex.Matches(body, pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
			foreach (Match tag in tags) 
				if (tag.Groups.Count > 0) 
				{

					if (tag.Groups[1].Value.ToLower().IndexOf("http://") == -1 &&
						tag.Groups[2].Value.ToLower().IndexOf("http://") == -1 &&
						tag.Groups[1].Value.ToLower().IndexOf("mailto:") == -1 &&
						tag.Groups[2].Value.ToLower().IndexOf("mailto:") == -1) 
					{
						// links
						if (tag.Groups[1].Value != "") 
						{
							// Special case for root link ("/")
							if (tag.Groups[0].Value.ToLower() == "href=\"/\"") 
							{
								if (tag.Groups[1].Value.IndexOf("?") == -1)
									body = body.Replace(tag.Groups[0].Value, "href=\"" + currentDomain + tag.Groups[1].Value + "?" + appendNewsletter+ "\"");
								else
									body = body.Replace(tag.Groups[0].Value, "href=\"" + currentDomain + tag.Groups[1].Value + "&" + appendNewsletter + "\"");
							}
							else 
							{
								if (tag.Groups[1].Value.IndexOf("?") == -1)
									body = body.Replace(tag.Groups[1].Value, currentDomain + tag.Groups[1].Value + "?" + appendNewsletter);
								else
									body = body.Replace(tag.Groups[1].Value, currentDomain + tag.Groups[1].Value + "&" + appendNewsletter);
							}

						}
							// src
						else
							body = body.Replace(tag.Groups[2].Value, currentDomain + tag.Groups[2].Value);
					}
				}

			return body;
		}
	}
}
