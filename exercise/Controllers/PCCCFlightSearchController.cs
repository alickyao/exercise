using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cyclonestyle.Models;

namespace cyclonestyle.Controllers
{
    /// <summary>
    /// 航班查询
    /// </summary>
    public class PCCCFlightSearchController : Controller
    {
        // GET: PCCCFlightSearch

        /// <summary>
        /// 航班查询
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult FlightList(SearchFlightInfoListRequestModel condtion)
        {
            ViewBag.condtion = condtion;
            ViewBag.PageId = Guid.NewGuid().ToString();
            return View();
        }
    }
}