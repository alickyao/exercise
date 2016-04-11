using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using cyclonestyle.Models;
using cyclonestyle.BLL;

namespace cyclonestyle.Controllers
{
    /// <summary>
    /// 呼叫中心核心
    /// </summary>
    public class PCCCMainController : Controller
    {
        //
        // GET: /PCCCMain/

        /// <summary>
        /// HelloWord
        /// </summary>
        /// <returns></returns>
        public string HelloWord() {
            return "HelloWord";
        }

        /// <summary>
        /// 呼叫中心登陆
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOn() {
            return View();
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            SysManagerService.CreateSysUserLog(new SysUserLogModel()
            {
                SysUserId = User.Identity.Name,
                Describe = "用户退出登录，IP地址：" + Request.UserHostAddress
            });
            return RedirectToAction("LogOn", "PCCCMain");
        }

        /// <summary>
        /// 呼叫中心（后台主界面）
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles="Admin,Users")]
        public ActionResult Home()
        {
            
            SysManagerService sms = new SysManagerService();
            #region -- 当前用户信息
            sms.GetSysUser(User.Identity.Name);
            ViewBag.SysUserInfo = sms.SysUserInfo;
            #endregion

            #region -- 顶部菜单
            RequestSearchSysMenuModel condtion = new RequestSearchSysMenuModel()
            {
                Dep = 1,
                RoleId = sms.SysUserInfo.SysRole.RoleId
            };
            
            sms.SearchMenuCondtion = condtion;
            List<SysMenuTreeModel> SysMenuList = sms.GetSysMenuTreeModel();
            ViewBag.SysMenuList = SysMenuList;
            #endregion

            ViewBag.baiduKey = System.Configuration.ConfigurationManager.AppSettings["Baidu_ak"].ToString();

            return View();
        }

        /// <summary>
        /// 工作台
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult MyWorkPanel() {

            return View();
        }
    }
}
