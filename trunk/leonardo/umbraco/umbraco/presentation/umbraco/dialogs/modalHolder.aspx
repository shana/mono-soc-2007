<%@ Page language="c#" Codebehind="modalHolder.aspx.cs" AutoEventWireup="True" Inherits="umbraco.dialogs.modalHolder" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
  <head>
    <title>modalHolder</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
  </head>
  <body MS_POSITIONING="GridLayout" style="margin:0px;padding:0px;" onload="this.focus()">
  <iframe frameborder="0" src="<%=umbraco.helper.Request("url")%>?<%=umbraco.helper.Request("params").Replace("|", "&")%>" scrolling="auto" width="100%" height="100%"></iframe> 
  </body>
</html>
