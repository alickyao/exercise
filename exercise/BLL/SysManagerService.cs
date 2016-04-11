using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cyclonestyle.DataBase;
using cyclonestyle.Models;
using Newtonsoft.Json;

namespace cyclonestyle.BLL
{
    /// <summary>
    /// 后台角色、权限、模块服务
    /// </summary>
    public class SysManagerService
    {

        #region -- 角色
        /// <summary>
        /// 获取后台系统角色列表
        /// </summary>
        /// <returns></returns>
        internal static ReplaySysUserRoleListModel GetSysRoleList(RequestBase condtion)
        {
            ReplaySysUserRoleListModel result = new ReplaySysUserRoleListModel();
            try
            {
                result = BaseSysTemDataBaseManager.SysGetSysRoleList(condtion);
                if (result.ReturnCode == EnumErrorCode.EmptyDate) {
                    result.RowList = new List<SysUserRoleModel>();
                }
            }
            catch (Exception e) {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysSaveErrorLogMsg(e.ToString(), condtion);
            }
            return result;
        }

        /// <summary>
        /// 获取后台全部角色信息
        /// </summary>
        /// <returns></returns>
        internal static List<SysUserRoleListModel> GetSysRoleAllRowsList()
        {
            try
            {
                List<SysUserRoleListModel> result = BaseSysTemDataBaseManager.SysGetSysRoleAllRowsList();
                return result;
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString());
                return new List<SysUserRoleListModel>();
            }
        }

        /// <summary>
        /// 系统角色
        /// </summary>
        public SysUserRoleModel UserRole { get; set; }
        /// <summary>
        /// 根据ID获取角色信息
        /// </summary>
        /// <param name="RoleId"></param>
        public void GetRole(long RoleId)
        {
            try
            {
                this.UserRole = BaseSysTemDataBaseManager.SysGetSysRoleById(RoleId);
            }
            catch (Exception e) {
                SysSaveErrorLogMsg(e.ToString(), RoleId);
            }
        }
        /// <summary>
        /// 保存角色信息
        /// </summary>
        /// <returns></returns>
        public ReplayBase SaveRole() {
            ReplayBase result  = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.SysSaveSysRole(this.UserRole);
            }
            catch (Exception e) {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysSaveErrorLogMsg(e.ToString(), this.UserRole);
            }
            return result;
        }

        /// <summary>
        /// 删除角色信息
        /// </summary>
        /// <returns></returns>
        public ReplayBase DelRole() {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.SysDelSysRole(this.UserRole);
            }
            catch (Exception e)
            {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysSaveErrorLogMsg(e.ToString(), this.UserRole);
            }
            return result;
        }
        #endregion

        #region -- 菜单（模块）

        /// <summary>
        /// 系统菜单对象
        /// </summary>
        public SysMenuModel SysMenu { get; set; }

        /// <summary>
        /// 调整系统菜单对象父节点ID
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase ChangeSysMenuPid(RequestChangeSysMenuPidModel condtion)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.SysChangeSysMenuPid(condtion);
            }
            catch (Exception e)
            {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysSaveErrorLogMsg(e.ToString(), condtion);
            }
            return result;
        }

        /// <summary>
        /// 获取系统菜单列表
        /// </summary>
        /// <returns></returns>
        public static List<SysMenuModel> GetSysMenuList() {
            try
            {
                List<SysMenuModel> result = BaseSysTemDataBaseManager.SysGetSysMenuList();
                return result;
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString());
                return new List<SysMenuModel>();
            }
        }

        /// <summary>
        /// 批量删除系统菜单项
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        internal static ReplayBase DelSysMenusByIds(string Ids)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                List<long> condtion = new List<long>();
                if (!string.IsNullOrEmpty(Ids)) {
                    string[] idstring = Ids.Split(',');
                    foreach (string id in idstring) {
                        condtion.Add(long.Parse(id));
                    }
                }
                result = BaseSysTemDataBaseManager.SysDelSysMenusByIds(condtion);
            }
            catch (Exception e)
            {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysSaveErrorLogMsg(e.ToString(), Ids);
            }
            return result;
        }

        /// <summary>
        /// 保存或新增系统菜单
        /// </summary>
        /// <returns></returns>
        public ReplayBase SaveMenu() {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.SysSaveSysMenu(this.SysMenu);
            }
            catch (Exception e)
            {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysSaveErrorLogMsg(e.ToString(), this.SysMenu);
            }
            return result;
        }

        /// <summary>
        /// 菜单查询请求参数
        /// </summary>
        public RequestSearchSysMenuModel SearchMenuCondtion { get; set; }

        /// <summary>
        /// 当前深度
        /// </summary>
        protected int dep = 0;

        /// <summary>
        /// 查询树模型--递归入口，需先设置SearchMenuCondtion项
        /// </summary>
        /// <returns></returns>
        internal List<SysMenuTreeModel> GetSysMenuTreeModel()
        {
            List<SysMenuTreeModel> SearchMenuModelResultList = new List<SysMenuTreeModel>();
            try
            {
                //获取根节点
                List<SysMenuModel> rootlist = BaseSysTemDataBaseManager.SysSearchSysMenuTreeList(SearchMenuCondtion.Pid, SearchMenuCondtion.RoleId);
                dep++;
                if (rootlist.Count > 0 && (SearchMenuCondtion.Dep == null ? true : dep <= SearchMenuCondtion.Dep))
                {
                    foreach (SysMenuModel menu in rootlist)
                    {
                        SysMenuTreeModel treeModel = new SysMenuTreeModel()
                        {
                            id = menu.MenuId,
                            text = menu.Name,
                            iconCls = menu.Icon,
                            attributes = new SysMenuTreeAttrModel()
                            {
                                Url = menu.Url
                            }
                        };
                        treeModel.children = GetChildMenuModel(treeModel);
                        SearchMenuModelResultList.Add(treeModel);
                    }
                }
            }
            catch (Exception e) {
                SysSaveErrorLogMsg(e.ToString(), this.SearchMenuCondtion);
            }
            return SearchMenuModelResultList;
        }
        /// <summary>
        /// 递归获取菜单子节点
        /// </summary>
        /// <param name="treeModel"></param>
        /// <returns></returns>
        private List<SysMenuTreeModel> GetChildMenuModel(SysMenuTreeModel treeModel)
        {
            List<SysMenuTreeModel> resultlist = new List<SysMenuTreeModel>();

            List<SysMenuModel> childlist = BaseSysTemDataBaseManager.SysSearchSysMenuTreeList(treeModel.id, SearchMenuCondtion.RoleId);
            dep++;
            if (childlist.Count > 0 && (SearchMenuCondtion.Dep == null ? true : dep <= SearchMenuCondtion.Dep))
            {
                foreach (SysMenuModel menu in childlist)
                {
                    SysMenuTreeModel ChildtreeModel = new SysMenuTreeModel()
                    {
                        id = menu.MenuId,
                        text = menu.Name,
                        iconCls = menu.Icon,
                        attributes = new SysMenuTreeAttrModel()
                        {
                            Url = menu.Url
                        },
                    };
                    ChildtreeModel.children = GetChildMenuModel(ChildtreeModel);
                    resultlist.Add(ChildtreeModel);
                }
            }
            return resultlist;
        }

        #endregion

        #region -- 系统用户
        /// <summary>
        /// 系统用户信息
        /// </summary>
        public SysUserModel SysUserInfo { get; set; }
        /// <summary>
        /// 保存用户
        /// </summary>
        /// <returns></returns>
        internal ReplayBase SaveUser()
        {
            ReplayBase result = new ReplayBase();
            try
            {
                //验证登录名
                int count = CheckLoginNameisExist(this.SysUserInfo.LoginName, this.SysUserInfo.UserId);
                if (count == 0)
                {
                    result = BaseSysTemDataBaseManager.SysSaveSysUser(SysUserInfo);
                }
                else {
                    result = new ReplayBase() { 
                        ReturnCode = EnumErrorCode.EmptyDate,
                        ReturnMessage = "当前登录名已存在"
                    };
                }
            }
            catch (Exception e)
            {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysSaveErrorLogMsg(e.ToString(), this.SysUserInfo);
            }
            return result;
        }
        /// <summary>
        /// 查询系统用户
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplaySysUserListModel SearchSysUserList(RequestSysUserListModel condtion)
        {
            try
            {
                ReplaySysUserListModel result = BaseSysTemDataBaseManager.SysSearchSysUserList(condtion);
                return result;
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
                return new ReplaySysUserListModel();
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="Id">用户Id</param>
        internal void GetSysUser(string Id)
        {
            try
            {
                this.SysUserInfo = BaseSysTemDataBaseManager.SysGetSysUserById(Id);
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), Id);
            }
        }


        /// <summary>
        /// 删除用户（标记删除）
        /// </summary>
        /// <returns></returns>
        internal ReplayBase DelSysUser()
        {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.SysDelSysUser(SysUserInfo);
            }
            catch (Exception e)
            {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysSaveErrorLogMsg(e.ToString(), this.SysUserInfo);
            }
            return result;
        }
        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <returns></returns>
        internal ReplayBase ReSetUserPwd()
        {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.SysReSetUserPwd(SysUserInfo);
            }
            catch (Exception e)
            {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysSaveErrorLogMsg(e.ToString(), SysUserInfo);
            }
            return result;
        }

        /// <summary>
        /// 系统用户登录验证
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase SysUserLogon(RequestUserLogoModel condtion)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.SysSysUserLogon(condtion);
            }
            catch (Exception e)
            {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysSaveErrorLogMsg(e.ToString(), condtion);
            }
            return result;
        }

        /// <summary>
        /// 修改系统用户密码
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase ChangeSysUserPwd(RequestChangeUserPwdModel condtion) {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.SysChangeSysUserPwd(condtion);
            }
            catch (Exception e)
            {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysSaveErrorLogMsg(e.ToString(), condtion);
            }
            return result;
        }

        /// <summary>
        /// 检查登录名在数据库中出现的次数
        /// </summary>
        /// <param name="LoginName">登录名</param>
        /// <param name="SysUserId">需要排除的用户ID（编辑情况时用户自己）</param>
        /// <returns></returns>
        internal static int CheckLoginNameisExist(string LoginName,string SysUserId = null)
        {
            try
            {
                return BaseSysTemDataBaseManager.SysCheckSysUserLoginNameisExist(LoginName, SysUserId);
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), LoginName + "[" + SysUserId + "]");
                return 1;
            }
        }

        #endregion

        #region --用户日志

        /// <summary>
        /// 创建系统用户日志
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase CreateSysUserLog(SysUserLogModel condtion) {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.SysCreateSysUserLog(condtion);
            }
            catch (Exception e) {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = "服务器错误 500";
                SysSaveErrorLogMsg(e.ToString(), condtion);
            }
            return result;
        }

        /// <summary>
        /// 查询系统用户日志
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplaySearchSysUserLog SearchSysUserLog(RequestSearchSysUserLog condtion)
        {
            try
            {
                ReplaySearchSysUserLog result = BaseSysTemDataBaseManager.SysSearchSysUserLog(condtion);
                return result;
            }
            catch (Exception e) {
                SysManagerService.SysSaveErrorLogMsg(e.ToString(), condtion);
                return new ReplaySearchSysUserLog();
            }
        }

        #endregion

        #region -- 系统更新日志

        /// <summary>
        /// 系统更新日志
        /// </summary>
        public SysUpdateLogModel SysUpdateLog { get; set; }

        /// <summary>
        /// 保存系统更新日志
        /// </summary>
        /// <returns></returns>
        internal ReplayBase SaveSysUpdateLog()
        {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.SysSaveSysUpdateLog(this.SysUpdateLog);
            }
            catch (Exception e)
            {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = e.ToString();
            }
            return result;
        }
        /// <summary>
        /// 查询系统更新日志列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static SearchSysUpdateLogReplayModel SearchSysUpdateLog(SearchSysUpdateLogRequstModel condtion)
        {
            SearchSysUpdateLogReplayModel result = BaseSysTemDataBaseManager.SysSearchSysUpdateLog(condtion);
            return result;
        }

        /// <summary>
        /// 删除系统更新日志
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        internal static ReplayBase DelSysUpdateLogById(long Id)
        {
            ReplayBase result = new ReplayBase();
            try
            {
                result = BaseSysTemDataBaseManager.SysDelSysUpdateLogById(Id);
            }
            catch (Exception e)
            {
                result.ReturnCode = EnumErrorCode.ServiceError;
                result.ReturnMessage = e.ToString();
            }
            return result;
        }

        #endregion

        #region -- 系统日志

        /// <summary>
        /// 将信息记录到错误日志
        /// </summary>
        /// <param name="errormsg">错误描述</param>
        /// <param name="condtion">当前方法的请求参数对象</param>
        public static void SysSaveErrorLogMsg(string errormsg, object condtion = null) {
            string condtions = null;
            if (condtion != null) {
                condtions = JsonConvert.SerializeObject(condtion);
            }
            Us_SysLog dblog = new Us_SysLog() { 
                Us_SysLogId = Helps.GetTimeId(),
                condtion = condtions,
                CreatedOn = DateTime.Now,
                errormsg = errormsg,
                type = 0
            };
            BaseSysTemDataBaseManager.SysSaveErrorLogMsg(dblog);
        }

        /// <summary>
        /// 将信息记录到系统日志
        /// </summary>
        /// <param name="Info"></param>
        /// <param name="logtype"></param>
        public static void SysSaveSysLog(string Info, EnumSysLogType logtype) {
            Us_SysLog dblog = new Us_SysLog()
            {
                Us_SysLogId = Helps.GetTimeId(),
                CreatedOn = DateTime.Now,
                errormsg = Info,
                type = byte.Parse(logtype.GetHashCode().ToString())
            };
            BaseSysTemDataBaseManager.SysSaveErrorLogMsg(dblog);
        }

        /// <summary>
        /// 查询系统日志
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static GetSysErrorLogReplayModel SearchSysLog(GetSysLogRequestModel condtion)
        {
            GetSysErrorLogReplayModel result = BaseSysTemDataBaseManager.SysSearchSysLog(condtion);
            return result;
        }
        /// <summary>
        /// 批量删除系统日志
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase DelSysLog(DelSysLogRequestModel condtion)
        {
            ReplayBase result = BaseSysTemDataBaseManager.SysDelSysLog(condtion);
            return result;
        }

        #endregion

        
    }
}