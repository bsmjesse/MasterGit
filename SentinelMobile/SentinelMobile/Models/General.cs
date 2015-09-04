using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SentinelMobile.Models
{
    public class LeftMenu
    {
        public string RefPage { get; set; }

        public LeftMenu(string refpage)
        {
            this.RefPage = refpage;
        }
    }
}