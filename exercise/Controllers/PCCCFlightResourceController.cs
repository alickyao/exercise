using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cyclonestyle.Models;
using cyclonestyle.BLL;

namespace cyclonestyle.Controllers
{
    /// <summary>
    /// 航旅机票资源维护
    /// </summary>
    public class PCCCFlightResourceController : Controller
    {
        // GET: PCCCFlightResource

        /// <summary>
        /// 机场信息维护
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult Airport(GetFlightAirPortListRequestModel condtion)
        {
            condtion.showOtherAirport = true;
            condtion.sorttype = EnumSortOrderType.按时间降序;
            ViewBag.condtion = condtion;
            ViewBag.PageId = Guid.NewGuid().ToString();
            return View();
        }

        /// <summary>
        /// 航空公司维护
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult AirCompany(GetFlightAirCompanyListRequestModel condtion) {
            condtion.sorttype = EnumSortOrderType.按时间降序;
            ViewBag.condtion = condtion;
            ViewBag.PageId = Guid.NewGuid().ToString();
            return View();
        }
    }
}