using System;
using System.Reflection;
using System.Web;
using System.Web.UI;

namespace umbraco.BusinessLogic
{
	public class StateHelper
	{
		#region Session Helpers

		public static T GetSessionValue<T>(string key)
		{
			return GetSessionValue<T>(HttpContext.Current, key);
		}

		public static T GetSessionValue<T>(HttpContext context, string key)
		{
			if (context == null)
				return default(T);
			object o = context.Session[key];
			if (o == null)
				return default(T);
			return (T)o;
		}

		public static void SetSessionValue(string key, object value)
		{
			SetSessionValue(HttpContext.Current, key, value);
		}

		public static void SetSessionValue(HttpContext context, string key, object value)
		{
			if (context == null)
				return;
			context.Session[key] = value;
		}

		#endregion

		#region Context Helpers

		public static T GetContextValue<T>(string key)
		{
			return GetContextValue<T>(HttpContext.Current, key);
		}

		public static T GetContextValue<T>(HttpContext context, string key)
		{
			if (context == null)
				return default(T);
			object o = context.Items[key];
			if (o == null)
				return default(T);
			return (T)o;
		}

		public static void SetContextValue(string key, object value)
		{
			SetContextValue(HttpContext.Current, key, value);
		}

		public static void SetContextValue(HttpContext context, string key, object value)
		{
			if (context == null)
				return;
			context.Items[key] = value;
		}

		#endregion

		#region ViewState Helpers

		private static StateBag GetStateBag()
		{
			if (HttpContext.Current == null)
				return null;

			Page page = HttpContext.Current.CurrentHandler as Page;
			if (page == null)
				return null;

			Type pageType = typeof(Page);
			PropertyInfo viewState = pageType.GetProperty("ViewState", BindingFlags.GetProperty | BindingFlags.Instance);
			if (viewState == null)
				return null;

			return viewState.GetValue(page, null) as StateBag;
		}

		public static T GetViewStateValue<T>(string key)
		{
			return GetViewStateValue<T>(GetStateBag(), key);
		}

		public static T GetViewStateValue<T>(StateBag bag, string key)
		{
			if (bag == null)
				return default(T);
			object o = bag[key];
			if (o == null)
				return default(T);
			return (T)o;
		}

		public static void SetViewStateValue(string key, object value)
		{
			SetViewStateValue(GetStateBag(), key, value);
		}

		public static void SetViewStateValue(StateBag bag, string key, object value)
		{
			if (bag != null)
				bag[key] = value;
		}

		#endregion

		#region Cookie Helpers

		public static bool HasCookieValue(string key)
		{
			return !string.IsNullOrEmpty(GetCookieValue(HttpContext.Current, key));
		}

		public static string GetCookieValue(string key)
		{
			return GetCookieValue(HttpContext.Current, key);
		}

		public static string GetCookieValue(HttpContext context, string key)
		{
            // Updated by NH to check against session values as well, which is an optional switch used by members
		    string tempValue = null;
			if (context == null || context.Request == null)
				return null;
			
            HttpCookie cookie = context.Request.Cookies[key];
            if (cookie == null) {
                // Check for session
                if (context.Session[key] != null)
                    if (context.Session[key].ToString() != "0")
                        tempValue = context.Session[key].ToString();
            }
            else
                tempValue = cookie.Value;

			return tempValue;
		}

		public static void SetCookieValue(string key, string value)
		{
			SetCookieValue(HttpContext.Current, key, value);
		}

		public static void SetCookieValue(HttpContext context, string key, string value)
		{
			if (context == null || context.Request == null)
				return;
			HttpCookie cookie = context.Request.Cookies[key];
			if (cookie == null)
				cookie = new HttpCookie(key);

			cookie.Value = value;

            // add default exp on a month
		    cookie.Expires = DateTime.Now.AddMonths(1);

            // if cookie exists, remove
            context.Response.Cookies.Add(cookie);
		}

		#endregion
	}
}
