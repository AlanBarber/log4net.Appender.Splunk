using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AspNetWebApp
{
    public partial class _Default : Page
    {
        private static log4net.ILog Log { get; set; } = log4net.LogManager.GetLogger(typeof(_Default));
        protected void Page_Load(object sender, EventArgs e)
        {
            Log.Debug("Default page loaded!");
        }
    }
}