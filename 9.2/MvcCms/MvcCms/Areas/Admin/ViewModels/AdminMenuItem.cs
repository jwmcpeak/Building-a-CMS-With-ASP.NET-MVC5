using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcCms.Areas.Admin.ViewModels
{
    public class AdminMenuItem
    {
        public string Text { get; set; }
        public string Action { get; set; }
        public object RouteInfo { get; set; }
    }
}