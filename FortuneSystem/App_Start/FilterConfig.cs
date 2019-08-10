﻿using FortuneSystem.App_Start;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ErrorHandler.AiHandleErrorAttribute());
			filters.Add(new SessionExpireFilterAttribute());
		}
    }
}
