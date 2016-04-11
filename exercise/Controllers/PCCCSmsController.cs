using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cyclonestyle.Models;

namespace cyclonestyle.Controllers
{
    /// <summary>
    /// 手机短消息
    /// </summary>
    public class PCCCSmsController : Controller
    {
        // GET: PCCCSms

        /// <summary>
        /// 发送短消息[通过号码表单界面]
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult sendsms(SendSmsBaseRequestModel condtion)
        {
            ViewBag.condtion = condtion;
            ViewBag.PageId = Guid.NewGuid().ToString();
            return View();
        }

        /// <summary>
        /// 短信发送历史
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult smshistory(SearchSmsInfoListRequestModel condtion) {
            ViewBag.condtion = condtion;
            ViewBag.PageId = Guid.NewGuid().ToString();
            return View();
        }

        /// <summary>
        /// 推送历史
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult pushhistory(SearchSentPushListRequestModel condtion) {
            ViewBag.condtion = condtion;
            ViewBag.PageId = Guid.NewGuid().ToString();
            return View();
        }
        /// <summary>
        /// 基于用户发送短信
        /// </summary>
        /// <param name="userids">用户ID，多个用,隔开</param>
        /// <param name="loginname">登录名，多个用,隔开</param>
        /// <param name="depid">部门ID，多个用,隔开</param>
        /// <param name="getchild">是否获取子集</param>
        /// <param name="msg">短消息文本</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult sendsmsbyusers(string userids = null, string loginname = null, string depid = null, bool getchild = false,string msg = null) {
            ViewBag.PageId = Guid.NewGuid().ToString();
            ViewBag.userids = userids;
            ViewBag.loginname = loginname;
            ViewBag.depid = depid;
            ViewBag.getchild = getchild;
            ViewBag.msg = msg;
            return View();
        }

        /// <summary>
        /// 基于用户发送推送
        /// </summary>
        /// <param name="userids">用户ID，多个用,隔开</param>
        /// <param name="loginname">登录名，多个用,隔开<</param>
        /// <param name="depid">部门ID，多个用,隔开</param>
        /// <param name="getchild">是否获取子集</param>
        /// <param name="title">推送标题</param>
        /// <param name="msg">推送文本</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult sendpushbyusers(string userids = null, string loginname = null, string depid = null, bool getchild = false,string title=null, string msg = null)
        {
            ViewBag.PageId = Guid.NewGuid().ToString();
            ViewBag.userids = userids;
            ViewBag.loginname = loginname;
            ViewBag.depid = depid;
            ViewBag.getchild = getchild;
            ViewBag.title = title;
            ViewBag.msg = msg;
            return View();
        }
    }
}