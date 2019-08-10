﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace FortuneSystem.App_Start
{
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public class SessionExpireFilterAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
		{
			var context = filterContext.HttpContext;
			if (context.Session != null)
			{
				if (context.Session.IsNewSession)
				{
					string sessionCookie = context.Request.Headers["Cookie"];
					if ((sessionCookie != null) && (sessionCookie.IndexOf("ASP.NET&#95;SessionId") >= 0))
					{
						FormsAuthentication.SignOut();
						string redirectTo = "~/Login/Login";
						if (!string.IsNullOrEmpty(context.Request.RawUrl))
						{
							redirectTo = string.Format("~/Login/Login?ReturnUrl={0}", HttpUtility.UrlEncode(context.Request.RawUrl));
						}
						filterContext.HttpContext.Response.Redirect(redirectTo, true);
					}
				}
			}
			base.OnActionExecuting(filterContext);
		}



	}
}