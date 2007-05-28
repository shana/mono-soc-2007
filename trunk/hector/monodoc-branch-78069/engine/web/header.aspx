<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
<head>
  <meta http-equiv="content-type" content="text/html; charset=ISO-8859-1">
  <title>MonoDoc Header</title>
  <meta name="description" content="Mono Documentation header">
  <link rel="stylesheet" type="text/css" href="common.css" media="all"
  title='Default Style'/>
  <style>
img.toolbarsep {
        border: 0px;
        margin-bottom: 1px;
        margin-top: 1px;
        padding-top: 3px;
        padding-bottom: 3px;
        vertical-align: middle;
}

img.toolbar {
        border: 0px;
        margin: 1px;
        padding: 3px;
        vertical-align: middle;
}

img.toolbar:hover {
        border-left: 1px solid white;
        border-right: 1px solid #B1A598;
        border-top: 1px solid white;
        border-bottom: 1px solid #B1A598;
        margin: 0px;
}

img.toolbar:active {
        border-right: 1px solid white;
        border-left: 1px solid #B1A598;
        border-bottom: 1px solid white;
        border-top: 1px solid #B1A598;
}


#login {
        position: fixed;
        top: 0px;
        right: 0px;
        float: right;
        padding: 5px;
}

#rightSide {
        position: fixed;
        bottom: 0px;
        right: 0px;
        float: right;
        padding: 5px;
}
  </style>

  <script language="C#" runat=server>
        void Page_Load (object sender, EventArgs e)
        {
		if (User.Identity.IsAuthenticated){
			login.NavigateUrl = "logout.aspx";
			login.Text = "Logged in as " + User.Identity.Name;
		} else {
			login.NavigateUrl = "javascript:parent.content.login (parent.content.window.location)";
			//login.Text = "Sign in / create account"; 
		}
        }
  </script>

</head>

<body
style="color: rgb(255, 255, 255); background-color: #c0dda2;"
link="#ffffff" alink="#ffffff" vlink="#ffffff">

<a href="index.aspx" class="headerlink" target="_top"><img
 src="images/mono-logo.png" alt="Mono"
 style="border: 0px solid ; height:75;" /></a>

<div id="dlogin">
   <asp:HyperLink id="login" runat="server" target="_top"/>
</div>

<div id="rightSide">
   <a href="javascript:parent.content.print();">
     <img class="toolbar" src="images/print.png" alt="Print"
     title="Print this document"/>
   </a>
   <a target="_top" id="pageLink" href="/">
     <img class="toolbar" src="images/link.png" alt="Link to this
     document" title="Link to this document"/>
   </a>
</div>


</body>
</html>
