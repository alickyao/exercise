using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using cyclonestyle.Models;
using cyclonestyle.BLL;
using System.Web.Security;
using System.Data;

namespace cyclonestyle.Controllers
{
    /// <summary>
    /// 会员接口-注册、登陆、详情、找回密码等功能
    /// </summary>
    public class ApiMembersController : ApiController
    {

        /// <summary>
        /// 会员用户登录
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        public RegisterMembersReplayModel LogOnMembers(RequestLogOnMembersModel condtion)
        {
            MembersService ms = new MembersService();
            RegisterMembersReplayModel result = ms.CheckMemberLoginNameandPwd(condtion);
            if (result.ReturnCode == EnumErrorCode.Success) { 
                //保存登陆信息
                FormsAuthentication.SetAuthCookie(result.UserInfo.UserId, true);
                //保存日志
                SysManagerService.CreateSysUserLog(new SysUserLogModel()
                {
                    SysUserId = result.UserInfo.UserId,
                    Describe = "用户登录,IP地址：" + System.Web.HttpContext.Current.Request.UserHostAddress+"["+ condtion.DeviceInfo +"]"
                });
            }
            return result;
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns>返回文本 Success</returns>
        [HttpGet]
        [Authorize]
        public string LogOutMembers() {
            FormsAuthentication.SignOut();

            //保存日志
            SysManagerService.CreateSysUserLog(new SysUserLogModel()
            {
                SysUserId = User.Identity.Name,
                Describe = "用户退出登录,IP地址：" + System.Web.HttpContext.Current.Request.UserHostAddress
            });

            return "Success";
        }

        /// <summary>
        /// 检查当前回话是否已验证了用户
        /// </summary>
        /// <returns>布尔值，true表示已登录，false表示未登陆</returns>
        [HttpGet]
        public bool CheckisLogOn() {
            return User.Identity.IsAuthenticated;
        }

        /// <summary>
        /// 检查登录名出现的次数
        /// </summary>
        /// <param name="LoginName">登录名</param>
        /// <returns>返回出现的次数，如果大于0则表示该登录名已使用</returns>
        [HttpGet]
        public int CheckLoginName(string LoginName) {
            return SysManagerService.CheckLoginNameisExist(LoginName);
        }

        /// <summary>
        /// 会员用户注册
        /// </summary>
        /// <param name="condtion">此注册方法中登录名请限制为用户手机号码</param>
        /// <returns></returns>
        [HttpPost]
        public RegisterMembersReplayModel RegisterMembers(RegisterMembersRequestModel condtion) {
            condtion.IpAddress = System.Web.HttpContext.Current.Request.UserHostAddress;
            MembersService ms = new MembersService();
            RegisterMembersReplayModel result = ms.RegisterMembers(condtion);
            if (result.ReturnCode == EnumErrorCode.Success) { 
                //记录到日志
                SysManagerService.CreateSysUserLog(new SysUserLogModel()
                {
                    SysUserId = result.UserInfo.UserId,
                    Describe = "用户注册，渠道" + condtion.RegisterWay + "，注册时客户端IP地址" + condtion.IpAddress
                });
            }
            return result;
        }

        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="condtion">参数中的orgId会影响用户所属部门的值，如果传入orgId则用户的部门只会是该组织下的部门</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public MembersInfoModel GetMemberInfo(GetMembersInfoRequestModel condtion) {
            MembersService ms = new MembersService();
            ms.GetMemberInfo(condtion);
            return ms.memberInfo;
        }

        /// <summary>
        /// 编辑用户基础信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ReplayBase EditMemberBaseInfo(MembersBaseInfoModel condtion) {
            MembersService ms = new MembersService();
            ReplayBase result = ms.EditMemberBaseInfo(condtion);
            return result;
        }

        /// <summary>
        /// 会员检索
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public SearchMembersreplayModel SearchMembersList(SearchMembersRequestModel condtion) {
            SearchMembersreplayModel result = MembersService.SearchMembersList(condtion);
            return result;
        }
        


        /// <summary>
        /// 会员检索（下拉搜索数据）
        /// </summary>
        /// <param name="q">关键字,登录名,姓名，手机，昵称</param>
        /// <param name="orgid">用户所在组织的ID，选填，前台如要检索某一个组织下的用户则必填该项</param>
        /// <param name="userid">用户的ID</param>
        /// <returns>返回用户Id+姓名/昵称[登录名]格式数据</returns>
        [HttpGet]
        [Authorize]
        public List<Combobox> SearchMembersListForcombobox(string q = null, string orgid = null, string userid = null)
        {
            List<Combobox> result = new List<Combobox>();
            SearchMembersRequestModel condtion = new SearchMembersRequestModel()
            {
                keyWords = q,
                orgId = orgid,
                userId = userid,
                Page = 1,
                PageSize = 10,
                ordertype = EnumSortOrderType.按标题降序
            };
            SearchMembersreplayModel l = MembersService.SearchMembersList(condtion);
            if (l.rows.Count > 0)
            {
                foreach (MembersBaseInfoModel u in l.rows)
                {
                    result.Add(new Combobox()
                    {
                        id = u.UserId,
                        text = (string.IsNullOrEmpty(u.FullName) ? u.NickName : u.FullName) + "[" + u.LoginName + "]"
                    });
                }
            }
            return result;
        }

        /// <summary>
        /// 根据用户的ID，登录名或者部门获取用户列表（不翻页，可用于发送短信，推送，批量设置用户等操作）
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns>如果没有匹配的用户则返回一个长度为空的集合</returns>
        [HttpPost]
        [Authorize]
        public List<MembersBaseInfoModel> GetMembersList(GetMembersListRequstModel condtion) {
            List<MembersBaseInfoModel> result = MembersService.GetMembersList(condtion);
            return result;
        }
    }
}
