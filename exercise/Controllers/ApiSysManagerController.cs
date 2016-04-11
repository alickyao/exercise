using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using cyclonestyle.Models;
using cyclonestyle.BLL;
using Newtonsoft.Json;
using System.Web.Security;

namespace cyclonestyle.Controllers
{
    /// <summary>
    /// 后台系统-用户、角色，权限，日志，模块接口
    /// </summary>
    public class ApiSysManagerController : ApiController
    {
        #region -- 系统用户

        /// <summary>
        /// 添加/编辑系统用户
        /// </summary>
        /// <param name="sysuserinfo">用户信息</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ReplayBase EditSysUser(SysUserModel sysuserinfo)
        {
            SysManagerService sms = new SysManagerService();
            sms.SysUserInfo = sysuserinfo;
            ReplayBase result = sms.SaveUser();

            if (result.ReturnCode == EnumErrorCode.Success)
            {
                //发起者
                SysUserLogModel log = new SysUserLogModel()
                {
                    Describe = "添加/编辑用户:" + sms.SysUserInfo.FullName,
                    SysUserId = User.Identity.Name
                };
                SysManagerService.CreateSysUserLog(log);

                SysManagerService sms1 = new SysManagerService();//操作员
                sms1.GetSysUser(User.Identity.Name);
                //被记录
                SysUserLogModel log1 = new SysUserLogModel()
                {
                    SysUserId = result.ReturnMessage,
                    Describe = "用户被创建By" + sms1.SysUserInfo.FullName
                };
                SysManagerService.CreateSysUserLog(log1);
            }

            return result;
        }

        /// <summary>
        /// 查询系统用户
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Users")]
        public ReplaySysUserListModel SearchSysUserList(RequestSysUserListModel condtion)
        {
            ReplaySysUserListModel result = SysManagerService.SearchSysUserList(condtion);
            return result;
        }
        /// <summary>
        /// 删除系统用户（标记删除）
        /// </summary>
        /// <param name="Id">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ReplayBase DelSysUser(string Id)
        {
            SysManagerService sms = new SysManagerService();
            sms.GetSysUser(Id);
            ReplayBase result = sms.DelSysUser();

            if (result.ReturnCode == EnumErrorCode.Success)
            {
                //发起者
                SysUserLogModel log = new SysUserLogModel()
                {
                    Describe = "删除用户:" + sms.SysUserInfo.FullName+"["+ sms.SysUserInfo.LoginName +"]",
                    SysUserId = User.Identity.Name
                };
                SysManagerService.CreateSysUserLog(log);

                SysManagerService sms1 = new SysManagerService();//操作员
                sms1.GetSysUser(User.Identity.Name);
                //被记录
                SysUserLogModel log1 = new SysUserLogModel()
                {
                    SysUserId = Id,
                    Describe = "用户被删除By:" + sms1.SysUserInfo.FullName
                };
                SysManagerService.CreateSysUserLog(log1);
            }

            return result;
        }

        /// <summary>
        /// 重置用户密码（管理员重置其他后台系统用户的密码）
        /// </summary>
        /// <param name="Id">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ReplayBase ReSetUserPwd(string Id)
        {
            SysManagerService sms = new SysManagerService();
            sms.GetSysUser(Id);
            ReplayBase result = sms.ReSetUserPwd();

            if (result.ReturnCode == EnumErrorCode.Success)
            {
                //发起者
                SysUserLogModel log = new SysUserLogModel()
                {
                    Describe = "重置用户密码:" + sms.SysUserInfo.FullName,
                    SysUserId = User.Identity.Name
                };
                SysManagerService.CreateSysUserLog(log);

                SysManagerService sms1 = new SysManagerService();//操作员
                sms1.GetSysUser(User.Identity.Name);
                //被记录
                SysUserLogModel log1 = new SysUserLogModel()
                {
                    SysUserId = Id,
                    Describe = "用户密码被重置By:" + sms1.SysUserInfo.FullName
                };
                SysManagerService.CreateSysUserLog(log1);
            }

            return result;
        }

        /// <summary>
        /// 修改系统用户登录密码
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Users")]
        public ReplayBase ChangeSysUserPwd(RequestChangeUserPwdModel condtion)
        {
            condtion.UserId = User.Identity.Name;
            ReplayBase result = SysManagerService.ChangeSysUserPwd(condtion);

            if (result.ReturnCode == EnumErrorCode.Success)
            {
                SysUserLogModel log = new SysUserLogModel()
                {
                    Describe = "用户修改登陆密码",
                    SysUserId = User.Identity.Name
                };
                SysManagerService.CreateSysUserLog(log);
            }

            return result;
        }

        /// <summary>
        /// 用户登录验证
        /// </summary>
        /// <param name="condtion">登陆请求</param>
        /// <returns></returns>
        [HttpPost]
        public ReplayBase SysUserLogon(RequestUserLogoModel condtion)
        {
            ReplayBase result = SysManagerService.SysUserLogon(condtion);
            if (result.ReturnCode == EnumErrorCode.Success)
            {
                //保存登陆信息
                FormsAuthentication.SetAuthCookie(result.ReturnMessage, true);

                SysUserLogModel log = new SysUserLogModel()
                {
                    Describe = "用户登录到后台系统，IP地址："+System.Web.HttpContext.Current.Request.UserHostAddress,
                    SysUserId = result.ReturnMessage
                };
                SysManagerService.CreateSysUserLog(log);
            }
            return result;
        }

        #endregion

        #region -- 角色
        /// <summary>
        /// 获取后台系统角色列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ReplaySysUserRoleListModel GetSysRoleList(RequestBase condtion)
        {
            return SysManagerService.GetSysRoleList(condtion);
        }

        /// <summary>
        /// 获取后台系统全部角色信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Users")]
        public List<SysUserRoleListModel> GetSysRoleAllRowList() {
            return SysManagerService.GetSysRoleAllRowsList();
        }

        /// <summary>
        /// 添加或编辑系统角色
        /// </summary>
        /// <param name="SysRole"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ReplayBase EditSysRole(SysUserRoleModel SysRole)
        {
            SysManagerService sms = new SysManagerService();
            sms.UserRole = SysRole;
            ReplayBase result = sms.SaveRole();

            if (result.ReturnCode == EnumErrorCode.Success)
            {
                SysUserLogModel log = new SysUserLogModel()
                {
                    Describe = "添加/编辑角色:" + SysRole.RoleName,
                    SysUserId = User.Identity.Name
                };
                SysManagerService.CreateSysUserLog(log);
            }

            return result;
        }

        /// <summary>
        /// 删除角色信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ReplayBase DelSysRole(long Id) {
            SysManagerService sms = new SysManagerService();
            sms.GetRole(Id);
            ReplayBase result = sms.DelRole();
            if (result.ReturnCode == EnumErrorCode.Success)
            {
                SysUserLogModel log = new SysUserLogModel()
                {
                    Describe = "删除角色:" + sms.UserRole.RoleName,
                    SysUserId = User.Identity.Name
                };
                SysManagerService.CreateSysUserLog(log);
            }

            return result;
        }

        #endregion

        #region -- 菜单

        /// <summary>
        /// 保存或新增系统菜单项
        /// </summary>
        /// <param name="SysMenu">系统菜单</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ReplayBase EditSysMenu(SysMenuModel SysMenu) {
            SysManagerService sms = new SysManagerService();
            sms.SysMenu = SysMenu;
            ReplayBase result = sms.SaveMenu();
            if (result.ReturnCode == EnumErrorCode.Success)
            {
                SysUserLogModel log = new SysUserLogModel()
                {
                    Describe = "添加/编辑菜单:" + SysMenu.Name,
                    SysUserId = User.Identity.Name
                };
                SysManagerService.CreateSysUserLog(log);
            }

            return result;
        }

        /// <summary>
        /// 调整系统菜单的父ID
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ReplayBase ChangeSysMenuPid(RequestChangeSysMenuPidModel condtion) {
            ReplayBase result = SysManagerService.ChangeSysMenuPid(condtion);

            if (result.ReturnCode == EnumErrorCode.Success)
            {
                SysUserLogModel log = new SysUserLogModel()
                {
                    Describe = "调整菜单节点位置:" + condtion.MenuId.ToString(),
                    SysUserId = User.Identity.Name
                };
                SysManagerService.CreateSysUserLog(log);
            }

            return result;
        }

        /// <summary>
        /// 删除系统菜单，一次可以删除多个项目
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ReplayBase DelSysMenus(string Ids) {
            ReplayBase result = SysManagerService.DelSysMenusByIds(Ids);

            if (result.ReturnCode == EnumErrorCode.Success) {
                SysUserLogModel log = new SysUserLogModel()
                {
                    Describe = "删除系统菜单节点:" + Ids,
                    SysUserId = User.Identity.Name
                };
                SysManagerService.CreateSysUserLog(log);
            }
            return result;
        }

        /// <summary>
        /// 获取后台菜单列表（ForTreeGrid）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Users")]
        public List<SysMenuModel> GetSysMenuList() {
            return SysManagerService.GetSysMenuList();
        }

        /// <summary>
        /// 获取后台系统菜单树模型(ForTreeMenu)
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Users")]
        public List<SysMenuTreeModel> GetSysMenuTreeModel(RequestSearchSysMenuModel condtion) {
            if (condtion == null)
            {
                condtion = new RequestSearchSysMenuModel();
            }
            SysManagerService sms = new SysManagerService();
            sms.GetSysUser(User.Identity.Name);
            condtion.RoleId = sms.SysUserInfo.SysRole.RoleId;
            sms.SearchMenuCondtion = condtion;
            return sms.GetSysMenuTreeModel();
        }


        #endregion

        #region -- 用户日志

        /// <summary>
        /// 获取用户日志列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ReplaySearchSysUserLog SearchSysUserLog(RequestSearchSysUserLog condtion) {
            ReplaySearchSysUserLog result = SysManagerService.SearchSysUserLog(condtion);
            return result;
        }

        #endregion

        #region -- 系统更新日志
        /*
        /// <summary>
        /// 创建/编辑系统更新日志
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ReplayBase EditSysUpdateLog(EditSysUpdateLogRequestModel condtion) {
            SysManagerService sms = new SysManagerService();
            sms.SysUpdateLog = new SysUpdateLogModel()
            {
                LogDescribe = condtion.LogDescribe,
                LogId = condtion.LogId,
                LogType = condtion.LogType,
                SysUserId = User.Identity.Name
            };
            ReplayBase result = sms.SaveSysUpdateLog();
            return result;
        }

        /// <summary>
        /// 查询系统更新日志列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        public SearchSysUpdateLogReplayModel SearchSysUpdateLog(SearchSysUpdateLogRequstModel condtion) {
            SearchSysUpdateLogReplayModel result = SysManagerService.SearchSysUpdateLog(condtion);
            return result;
        }

        /// <summary>
        /// 删除某条系统更新日志
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ReplayBase DelSysUpdateLogById(long Id) {
            ReplayBase result = SysManagerService.DelSysUpdateLogById(Id);
            return result;
        }
        */
        #endregion

        #region -- 系统日志

        /// <summary>
        /// 获取系统运行日志信息列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public GetSysErrorLogReplayModel SearchSysLog(GetSysLogRequestModel condtion) {
            GetSysErrorLogReplayModel result = SysManagerService.SearchSysLog(condtion);
            return result;
        }

        /// <summary>
        /// 删除系统运行日志信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ReplayBase DelSysLog(DelSysLogRequestModel condtion) {
            ReplayBase result = SysManagerService.DelSysLog(condtion);
            if (result.ReturnCode == EnumErrorCode.Success)
            {
                SysUserLogModel log = new SysUserLogModel()
                {
                    Describe = "删除了"+ condtion.rows.Count.ToString() +"条系统运行日志",
                    SysUserId = User.Identity.Name
                };
                SysManagerService.CreateSysUserLog(log);
            }
            return result;
        }

        #endregion
    }
}
