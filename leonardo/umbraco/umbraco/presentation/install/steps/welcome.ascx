<%@ Control Language="c#" AutoEventWireup="True" Codebehind="welcome.ascx.cs" Inherits="presentation.install.steps.welcome" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<p style="font-size: 80%; float: right;"><a href="#" onClick="javascript:document.getElementById('detailsPane').style.display = 'block';"><span style="color: #000; text-decoration: none"><b>Where's my site?</b></span></a><br />
</p>
<h1>Thank you for choosing umbraco!</h1>
<div id="detailsPane" style="DISPLAY: none">
	<p style="border: 2px solid green; padding: 5px; background: #FFF">
	This wizard will guide you through setting up umbraco successfully.<br/>
<br/>
If you have successfully upgraded from a previous version of umbraco, you can skip this wizard by editing the “web.config” file by inserting a line of code (shown below) between the “appSettings” tags: <br/>
<add key="umbracoConfigurationDone" value="true"/> <br />
	&lt;add key="umbracoConfigurationDone" value="211"/&gt;
	<br/><br/></p>
	</div>
<p>
You're only a few minutes away from being up and running the web development platform that makes ASP.NET content management enjoyable.<br/>
<br/>
This wizard will guide you through the process of configuring <strong>umbraco <%=umbraco.GlobalSettings.CurrentVersion%></strong> for a fresh install or upgrading from 2.0, 2.1 Release Candidates or 2.1 Final.<br/>
<br/>
To complete this wizard you must know some information regarding your database server ("connection string"). Please contact your ISP if necessary. If you're installing on a local machine or server you might need information from your System Admin.<br/>
<br/>
The wizard will start by examining the current installation to check whether you're upgrading or running umbraco for the first time.<br/>
</p><p>Press <b>"Next"</b> to start the wizard. </p>
