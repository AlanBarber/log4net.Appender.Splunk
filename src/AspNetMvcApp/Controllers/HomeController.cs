using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspNetMvcApp.Controllers
{
    public class HomeController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(HomeController));


        public ActionResult Index()
        {
            // with batch setting of 10, it should submit 3 batches which we can observe in Fiddler
            for (var i = 0; i < 22; i++)
            {
                log.Info($"Action Index has been fired.i={i}, time=" + DateTime.Now.ToString("hh:mm:ss.fff tt"));
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}