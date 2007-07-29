using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Text;
using System.Web;
using Microsoft.ApplicationBlocks.Data;
using Umbraco.BusinessLogic;
using Umbraco.Cms.BusinessLogic.property;
using Umbraco.Cms.BusinessLogic.web;
using Umbraco.interfaces;

namespace Umbraco.Cms.BusinessLogic.workflow
{
    /// <summary>
    /// Summary description for Notification.
    /// </summary>
    public class Notification
    {
        public static void GetNotifications(CMSNode Node, User user, IAction Action)
        {
            User[] allUsers = User.getAll();
            foreach (User u in allUsers)
            {
                try
                {
                    if (!u.Disabled && u.GetNotifications(Node.Path).IndexOf(Action.Letter.ToString()) > -1)
                    {
                        Log.Add(LogTypes.Notify, User.GetUser(0), Node.Id,
                                "Notification about " + ui.Text(Action.Alias, u) + " sent to " + u.Name + " (" + u.Email +
                                ")");
                        sendNotification(user, u, (Document) Node, Action);
                    }
                }
                catch (Exception notifyExp)
                {
                    Log.Add(LogTypes.Error, u, Node.Id, "Error in notification: " + notifyExp);
                }
            }
        }

        // TODO: Include update with html mail notification and document contents
        private static void sendNotification(User performingUser, User mailingUser, Document documentObject,
                                             IAction Action)
        {
            // build summary
            StringBuilder summary = new StringBuilder();
            foreach (Property p in documentObject.getProperties)
            {
                summary.Append("<tr>");
                summary.Append("<th>" + p.PropertyType.Name + "</th>");
                summary.Append("<td>" + p.Value.ToString() + "</td>");
                summary.Append("</tr>");
                summary.Append("<tr><td colspan=\"2\" class=\"splitter\">&nbsp;</td></tr>");
            }
            string[] subjectVars = {
                                       HttpContext.Current.Request.ServerVariables["SERVER_NAME"], ui.Text(Action.Alias)
                                       ,
                                       documentObject.Text
                                   };
            string[] bodyVars = {
                                    mailingUser.Name, ui.Text(Action.Alias), documentObject.Text, performingUser.Name,
                                    HttpContext.Current.Request.ServerVariables["SERVER_NAME"],
                                    documentObject.Id.ToString(), summary.ToString()
                                };

            // create the mail message 
            MailMessage mail = new MailMessage(UmbracoSettings.NotificationEmailSender, mailingUser.Email);

            // populate the message
            mail.Subject = ui.Text("notifications", "mailSubject", subjectVars, mailingUser);
            if (UmbracoSettings.NotificationDisableHtmlEmail)
            {
                mail.IsBodyHtml = false;
                mail.Body = ui.Text("notifications", "mailBody", bodyVars, mailingUser);
            } else
            {
                mail.IsBodyHtml = true;
                mail.Body =
                    @"<html><head><style>
body {
    font-family: Trebuchet MS, arial, sans-serif;
    font-color: black;
}


.buttons {
    margin: 8px 0;
    padding: 8px;
    display: block;
}

.buttons a, .buttons a:visited, .buttons a:active {
    color: white;
}


.buttonPublish {
    color: white;
    font-weight: bold;
    background-color: #66cc66;
    text-decoration : none;
    margin-right: 20px;
    border: 8px solid #66cc66;
    width: 150px;
}

.buttonEdit {
    color: white;
    font-weight: bold;
    background-color: #5372c3;
    text-decoration : none;    
    margin-right: 20px;
    border: 8px solid #5372c3;
    width: 150px;
}

.buttonDelete {
    color: white;
    font-weight: bold;
    background-color: #ca4a4a;
    text-decoration : none;
    border: 8px solid #ca4a4a;
    width: 150px;
}

.updateSummary {
    width: 100%;
}

.splitter {
    border-bottom: 1px solid #CCC;
    font-size: 2px;
}

.updateSummary th {
    text-align: left;
    vertical-align: top;
    width: 25%;
}

.updateSummary td {
    text-align: left;
    vertical-align: top;
}
</style>
</head>
<body>
" +
                    ui.Text("notifications", "mailBodyHtml", bodyVars, mailingUser) + "</body></html>";
            }

            // send it
            SmtpClient sender = new SmtpClient(GlobalSettings.SmtpServer);
            sender.Send(mail);
        }

        public static void MakeNew(User User, CMSNode Node, char ActionLetter)
        {
            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text,
                                      "if not exists(select userId from umbracoUser2nodeNotify where userId = @userId and nodeId = @nodeId and action = @action) insert into umbracoUser2nodeNotify (userId, nodeId, action) values (@userId, @nodeId, @action)",
                                      new SqlParameter("@userId", User.Id), new SqlParameter("@nodeId", Node.Id),
                                      new SqlParameter("@action", ActionLetter.ToString()));
        }

        public static void UpdateNotifications(User User, CMSNode Node, string Notifications)
        {
            // delete all settings on the node for this user
            Microsoft.ApplicationBlocks.Data.SqlHelper.ExecuteNonQuery(GlobalSettings.DbDSN, CommandType.Text,
                                      "delete from umbracoUser2NodeNotify where userId = @userId and nodeId = @nodeId",
                                      new SqlParameter("@userId", User.Id), new SqlParameter("@nodeId", Node.Id));

            // Loop through the permissions and create them
            foreach (char c in Notifications.ToCharArray())
                MakeNew(User, Node, c);
        }
    }
}