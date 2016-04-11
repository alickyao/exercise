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
    /// 公共资源管理，公用页面
    /// </summary>
    public class PCCCPublicResourceController : Controller
    {
        //
        // GET: /PCCCPublicResource/

        /// <summary>
        /// 用户输入项维护
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult UserInputRuleSet(SearchResourceUserInputRuleRequestModel condtion)
        {
            ViewBag.PageId = Guid.NewGuid().ToString();
            ViewBag.condtion = condtion;
            return View();
        }

        /// <summary>
        /// 分类树维护
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult CatTreeSet(SearchCatInfoRequest condtion)
        {
            ViewBag.PageId = Guid.NewGuid().ToString();
            ViewBag.condtion = condtion;
            return View();
        }


        /// <summary>
        /// 省市区联动选择框
        /// </summary>
        /// <param name="condtion">省市区信息</param>
        /// <param name="callback">回调</param>
        ///  <param name="PageId">guid</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult selectcity(LocationInfoModel condtion, string PageId, string callback = null) {
            ViewBag.PageId = PageId;
            ViewBag.condtion = condtion;
            return View();
        }
    }
}
