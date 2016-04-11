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
    /// 酒店相关
    /// </summary>
    public class PCCCHotelController : Controller
    {
        // GET: PCCCHotel

        /// <summary>
        /// 艺龙酒店查询列表
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult elongHotelList(SearchHotelListRequestModel condtion)
        {
            ViewBag.condtion = condtion;
            ViewBag.PageId = Guid.NewGuid().ToString();
            return View();
        }

        /// <summary>
        /// 艺龙酒店详情
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult elongHotelDetail(GetElongHotelInfoRequestModel condtion) {
            ElongHotelService ehs = new ElongHotelService();
            GetElongHotelInfoReponseModel result = ehs.GetHotelInfo(condtion);
            ViewBag.result = result;
            ViewBag.PageId = Guid.NewGuid().ToString();
            return View();
        }

        /// <summary>
        /// 创建艺龙酒店订单
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public ActionResult elongHotelCreateOrder(GetElongHotelInfoRequestModel condtion) {
            ElongHotelService ehs = new ElongHotelService();
            GetElongHotelInfoReponseModel result = ehs.GetHotelInfo(condtion);
            ViewBag.result = result;
            ViewBag.PageId = Guid.NewGuid().ToString();
            return View();
        }
    }
}