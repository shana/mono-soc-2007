<%@ Page Language="C#" Debug="true" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Xml.Xsl" %>

<!-- 
// GetJavaScriptProxy.aspx
// Javascript proxy generator for SOAP based web services
// Copyright by Matthias Hertel, http://www.mathertel.de
// This work is licensed under a Creative Commons Attribution 2.0 Germany License.
// See http://creativecommons.org/licenses/by/2.0/de/
// More information on: http://ajaxaspects.blogspot.com/ and http://ajaxaspekte.blogspot.com/
// 19.07.2005 white space removed
// 20.07.2005 more datatypes and XML Documents
// 04.09.2005 XslCompiledTransform
// 24.09.2006 direct use of wsdl files enabled
 -->
 
<script runat="server">
  private string FetchWsdl(string url) {
    if ((url != null) && (url.StartsWith("~/")))
      url = Request.ApplicationPath + url.Substring(1);
    if (url.EndsWith(".asmx", StringComparison.InvariantCultureIgnoreCase))
      url = url + "?WSDL";
    Uri uri = new Uri(Request.Url, url);
    
    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
    req.Credentials = CredentialCache.DefaultCredentials;
    // req.Proxy = WebRequest.DefaultWebProxy; // running on the same server !
    req.Timeout = 6 * 1000; // 6 seconds

    WebResponse res = req.GetResponse();
#if DOTNET11
    XmlDocument data = new XmlDocument();
    data.Load(res.GetResponseStream());

    XslTransform xsl = new XslTransform();
    xsl.Load(Server.MapPath("wsdl.xslt"));

    System.IO.StringWriter sOut = new System.IO.StringWriter();
    xsl.Transform(data, null, sOut, null);
#else
    XmlReader data = XmlReader.Create(res.GetResponseStream());

    XslCompiledTransform xsl = new XslCompiledTransform();
    xsl.Load(Server.MapPath("wsdl.xslt"));

    System.IO.StringWriter sOut = new System.IO.StringWriter();
    xsl.Transform(data, null, sOut);
#endif
    return (sOut.ToString());
  } // FetchWsdl
</script>

<%
  string asText = Request.QueryString["html"];

  Response.Clear();
  if (asText != null) {
    Response.ContentType = "text/html";
    Response.Write("<pre>");
  } else {
    Response.ContentType = "text/text";
  } // if

  string fileName = Request.QueryString["service"];
  if (fileName == null)
    fileName = "CalcService";

  // get good filenames only (local folder)
  if ((fileName.IndexOf('$') >= 0) || (Regex.IsMatch(fileName, @"\b(COM\d|LPT\d|CON|PRN|AUX|NUL)\b", RegexOptions.IgnoreCase)))
    throw new ApplicationException("Error in filename.");

  if (! Server.MapPath(fileName).StartsWith(Request.PhysicalApplicationPath, StringComparison.InvariantCultureIgnoreCase))
    throw new ApplicationException("Can show local files only.");

  string ret = FetchWsdl(fileName);
  ret = Regex.Replace(ret, @"\n *", "\n");
  ret = Regex.Replace(ret, @"\r\n *""", "\"");
  ret = Regex.Replace(ret, @"\r\n, *""", ",\"");
  ret = Regex.Replace(ret, @"\r\n\]", "]");
  ret = Regex.Replace(ret, @"\r\n; *", ";");
  Response.Write(ret);
%>