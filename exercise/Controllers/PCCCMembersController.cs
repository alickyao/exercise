using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cyclonestyle.Models;
using cyclonestyle.BLL;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;

namespace cyclonestyle.Controllers
{
    /// <summary>
    /// 后台，会员用户与组织相关
    /// </summary>
    public class PCCCMembersController : Controller
    {
        // GET: PCCCMembers

        static string fkid = "";
        /// <summary>
        /// 会员用户列表
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult userlist(SearchMembersRequestModel condtion)
        {
            if (condtion == null)
            {
                condtion = new SearchMembersRequestModel();
            }
            condtion.getOtherOrgDepartmentInfo = true;
            if (condtion.departmentIds == null)
            {
                condtion.departmentIds = new List<string>();
            };
            ViewBag.condtion = condtion;
            ViewBag.PageId = Guid.NewGuid().ToString();
            return View();
        }

        /// <summary>
        /// 会员列表用于选择用户
        /// </summary>
        /// <param name="userids">用户ID，多个用,隔开</param>
        /// <param name="loginname">用户登录名，多个用,隔开</param>
        /// <param name="depid">部门ID，多个用,隔开</param>
        /// <param name="getchild">是否获取部门下子集的用户</param>
        /// <param name="PageId">传入的PageId</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult selecteduserlist(string userids = null, string loginname = null, string depid = null, bool getchild = false, string PageId = null)
        {
            GetMembersListRequstModel condtion = new GetMembersListRequstModel();
            if (!string.IsNullOrEmpty(userids))
            {
                condtion.userIds = userids.Split(',').ToList();
            }
            if (!string.IsNullOrEmpty(loginname))
            {
                condtion.loginNames = loginname.Split(',').ToList();
            }
            if (!string.IsNullOrEmpty(depid))
            {
                condtion.depIds = depid.Split(',').ToList();
            }
            condtion.getChilds = getchild;
            ViewBag.condtion = condtion;
            ViewBag.PageId = PageId == null ? Guid.NewGuid().ToString() : PageId;
            return View();
        }

        /// <summary>
        /// 用户详情
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult userdetail(GetMembersInfoRequestModel condtion)
        {
            MembersService ms = new MembersService();
            ms.GetMemberInfo(condtion);
            ViewBag.Info = ms.memberInfo;
            ViewBag.PageId = Guid.NewGuid().ToString();
            return View();
        }

        /// <summary>
        /// 会员用户组织列表
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult orglist()
        {
            ViewBag.PageId = Guid.NewGuid().ToString();
            ViewBag.condtion = new SearchMemberRootDepartmentRequest()
            {
                page = new RequestBase(),
                showdisabled = true,
                showontheui = false,
                ordertype = EnumSortOrderType.按时间降序
            };
            return View();
        }

        /// <summary>
        /// 组织详情
        /// </summary>
        /// <param name="Id">组织的ID</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult orgdetail(string Id)
        {
            ViewBag.PageId = Guid.NewGuid().ToString();
            ViewBag.orgId = Id;
            MembersService ms = new MembersService();
            ms.GetOrgBaseInfo(Id);
            ViewBag.orginfo = ms.orgInfo;
            return View();
        }

        /// <summary>
        /// 部门列表
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult deplist(SearchMemberDepartmentRequst condtion)
        {
            if (condtion == null)
            {
                condtion = new SearchMemberDepartmentRequst();
            }
            condtion.ordertype = EnumSortOrderType.按标题升序;
            condtion.showdisabled = true;
            condtion.getchild = true;
            condtion.showontheui = false;
            ViewBag.PageId = Guid.NewGuid().ToString();
            ViewBag.condtion = condtion;
            return View();
        }

        /// <summary>
        /// 部门详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Users")]
        public ActionResult depdetail(string Id)
        {
            ViewBag.PageId = Guid.NewGuid().ToString();
            ViewBag.Id = Id;
            MembersService ms = new MembersService();
            ms.GetDepBaseInfo(new GetDepInfoRequestModel()
            {
                depmentid = Id
            });
            ViewBag.depInfo = ms.depInfo;
            return View();
        }

        /// <summary>
        /// 常旅客网格
        /// </summary>
        /// <param name="condtion">请求参数</param>
        /// <param name="callback">网格选择按钮回调界面的ID</param>
        /// <returns></returns>
        public ActionResult traveller(GetUserExInfoListRequest condtion, string callback = null)
        {
            ViewBag.PageId = Guid.NewGuid().ToString();
            ViewBag.condtion = condtion;
            fkid = condtion.fkId;
            ViewBag.callback = callback;
            return View();
        }
  

        /// <summary>
        /// 常用地址网格
        /// </summary>
        /// <param name="condtion">请求参数</param>
        /// <param name="callback">网格选择按钮回调界面的ID</param>
        /// <returns></returns>
        public ActionResult address(GetUserExInfoListRequest condtion, string callback = null)
        {
            ViewBag.PageId = Guid.NewGuid().ToString(); 
            ViewBag.condtion = condtion;
            ViewBag.callback = callback;
            return View();
        }
    }
}