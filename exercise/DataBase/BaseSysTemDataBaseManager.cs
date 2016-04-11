using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Data.Linq.SqlClient;
using cyclonestyle.Models;
using System.Web.Security;

namespace cyclonestyle.DataBase
{
    /// <summary>
    /// 数据库操作 底层框架与资源相关
    /// </summary>
    public class BaseSysTemDataBaseManager
    {
        /// <summary>
        /// 数据连接字串
        /// </summary>
        public static string SqlConnection;
        static BaseSysTemDataBaseManager()
        {
            SqlConnection = System.Configuration.ConfigurationManager.AppSettings["UsDataBaseSql"].ToString();
        }


        #region -- 管理后台 Sys

        #region -- 系统角色
        /// <summary>
        /// 获取系统角色列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplaySysUserRoleListModel SysGetSysRoleList(RequestBase condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                ReplaySysUserRoleListModel result = new ReplaySysUserRoleListModel();
                var count = (from c in context.Us_SysRole select c.Us_SysRoleId).Count();
                if (count > 0)
                {
                    result.total = count;
                    result.RowList = (from c in context.Us_SysRole
                                      select new SysUserRoleModel
                                      {
                                          RoleId = c.Us_SysRoleId,
                                          RoleDescribe = c.RoleDescribe,
                                          RoleName = c.RoleName
                                      }).Skip((condtion.Page - 1) * condtion.PageSize).Take(condtion.PageSize).ToList();
                }
                else {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = "暂无数据";
                }
                return result;
            }
        }

        


        /// <summary>
        /// 获取系统全部角色信息
        /// </summary>
        /// <returns></returns>
        internal static List<SysUserRoleListModel> SysGetSysRoleAllRowsList()
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                List<SysUserRoleListModel> result = (from c in context.Us_SysRole
                                                     orderby c.Us_SysRoleId ascending
                                                     select new SysUserRoleListModel { 
                                                        RoleId = c.Us_SysRoleId,
                                                        RoleName = c.RoleName
                                                     }).ToList();
                return result;
            }
        }




        /// <summary>
        /// 获取系统角色信息ByRoleId
        /// </summary>
        /// <param name="RoleId">角色ID</param>
        /// <returns></returns>
        internal static SysUserRoleListModel SysGetSysRoleListModelById(long RoleId) {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                SysUserRoleListModel result = new SysUserRoleListModel();
                Us_SysRole dbrole = context.Us_SysRole.SingleOrDefault(p => p.Us_SysRoleId == RoleId);
                if (dbrole != null)
                {
                    result = new SysUserRoleListModel()
                    {
                        RoleId = dbrole.Us_SysRoleId,
                        RoleName = dbrole.RoleName
                    };
                }
                return result;
            }
        }



        /// <summary>
        /// 保存/新增一个系统角色
        /// </summary>
        /// <param name="sysUserRoleModel"></param>
        /// <returns></returns>
        internal static ReplayBase SysSaveSysRole(SysUserRoleModel sysUserRole)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                ReplayBase result = new ReplayBase();
                if (sysUserRole.RoleId == 0)
                {
                    Us_SysRole dbrole = new Us_SysRole()
                    {
                        RoleDescribe = sysUserRole.RoleDescribe,
                        RoleName = sysUserRole.RoleName
                    };
                    context.Us_SysRole.InsertOnSubmit(dbrole);
                }
                else {
                    Us_SysRole dbrole = context.Us_SysRole.Single(p => p.Us_SysRoleId == sysUserRole.RoleId);
                    dbrole.RoleName = sysUserRole.RoleName;
                    dbrole.RoleDescribe = sysUserRole.RoleDescribe;
                }
                context.SubmitChanges();
                return result;
            }
        }


        /// <summary>
        /// 获取系统角色信息ByRoleId
        /// </summary>
        /// <param name="RoleId">角色ID</param>
        /// <returns></returns>
        internal static SysUserRoleModel SysGetSysRoleById(long RoleId)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                Us_SysRole dbrole = context.Us_SysRole.Single(p => p.Us_SysRoleId == RoleId);
                SysUserRoleModel Role = new SysUserRoleModel() { 
                    RoleId = dbrole.Us_SysRoleId,
                    RoleDescribe = dbrole.RoleDescribe,
                    RoleName = dbrole.RoleName
                };
                return Role;
            }
        }



        /// <summary>
        /// 删除系统角色
        /// </summary>
        /// <param name="sysUserRole"></param>
        /// <returns></returns>
        internal static ReplayBase SysDelSysRole(SysUserRoleModel sysUserRole)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                Us_SysRole dbrole = context.Us_SysRole.Single(p => p.Us_SysRoleId == sysUserRole.RoleId);
                context.Us_SysRole.DeleteOnSubmit(dbrole);
                context.SubmitChanges();
                return new ReplayBase() { };
            }
        }


        #endregion

        #region -- 后台系统菜单模块

        /// <summary>
        /// 保存或新增系统菜单
        /// </summary>
        /// <param name="sysMenuModel"></param>
        /// <returns></returns>
        internal static ReplayBase SysSaveSysMenu(SysMenuModel sysMenuModel)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                if (sysMenuModel.MenuId == 0)
                {
                    //新增
                    Us_SysMenu dbmenu = new Us_SysMenu() { 
                        ModifiedOn = DateTime.Now,
                        Name = sysMenuModel.Name,
                        Pid = sysMenuModel._parentId,
                        Sort = sysMenuModel.Sort,
                        Url = sysMenuModel.Url,
                        Roles = sysMenuModel.Roles,
                        Icon = sysMenuModel.Icon
                    };
                    context.Us_SysMenu.InsertOnSubmit(dbmenu);
                }
                else { 
                    //编辑
                    Us_SysMenu dbmenu = context.Us_SysMenu.Single(p => p.Us_SysMenuId == sysMenuModel.MenuId);
                    dbmenu.ModifiedOn = DateTime.Now;
                    dbmenu.Name = sysMenuModel.Name;
                    dbmenu.Pid = sysMenuModel._parentId;
                    dbmenu.Sort = sysMenuModel.Sort;
                    dbmenu.Url = sysMenuModel.Url;
                    dbmenu.Roles = sysMenuModel.Roles;
                    dbmenu.Icon = sysMenuModel.Icon;
                }
                context.SubmitChanges();
                return new ReplayBase() { };
            }
        }


        /// <summary>
        /// 修改系统菜单PID
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase SysChangeSysMenuPid(RequestChangeSysMenuPidModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                Us_SysMenu dbm = context.Us_SysMenu.Single(p => p.Us_SysMenuId == condtion.MenuId);
                dbm.Pid = condtion.ParentId;
                context.SubmitChanges();
                return new ReplayBase() { };
            }
        }

        

        /// <summary>
        /// 获取系统菜单项列表
        /// </summary>
        /// <returns></returns>
        internal static List<SysMenuModel> SysGetSysMenuList()
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                List<SysMenuModel> result = (from c in context.Us_SysMenu
                                             orderby c.Sort ascending
                                             select new SysMenuModel { 
                                                MenuId = c.Us_SysMenuId,
                                                ModifiedOn = c.ModifiedOn,
                                                Name = c.Name,
                                                _parentId = c.Pid,
                                                Sort = c.Sort,
                                                Url = c.Url,
                                                Roles = c.Roles,
                                                Icon = c.Icon
                                             }).ToList();
                return result;
            }
        }

        /// <summary>
        /// 获取系统菜单项列表(树)
        /// </summary>
        /// <param name="PId">父ID</param>
        /// <param name="RoleId">角色ID</param>
        /// <returns></returns>
        internal static List<SysMenuModel> SysSearchSysMenuTreeList(Nullable<long> PId, Nullable<long> RoleId) {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                List<SysMenuModel> result = (from c in context.Us_SysMenu
                                             where (PId == null ? c.Pid == null : c.Pid == PId)
                                             && (((c.Roles==null || c.Roles=="") || RoleId == null) ? true : (from x in context.f_split(c.Roles, ",") select x.a).ToList().Contains(RoleId.ToString()))
                                             orderby c.Sort ascending
                                             select new SysMenuModel
                                             {
                                                 MenuId = c.Us_SysMenuId,
                                                 ModifiedOn = c.ModifiedOn,
                                                 Name = c.Name,
                                                 _parentId = c.Pid,
                                                 Sort = c.Sort,
                                                 Url = c.Url,
                                                 Roles = c.Roles,
                                                 Icon = c.Icon
                                             }).ToList();
                return result;
            }
        }

        /// <summary>
        /// 删除系统菜单项
        /// </summary>
        /// <param name="condtion">IdList</param>
        /// <returns></returns>
        internal static ReplayBase SysDelSysMenusByIds(List<long> condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                List<Us_SysMenu> DelList = (from c in context.Us_SysMenu
                                            where condtion.Contains(c.Us_SysMenuId)
                                            select c).ToList();
                context.Us_SysMenu.DeleteAllOnSubmit(DelList);
                context.SubmitChanges();
                return new ReplayBase() { };
            }
        }



        #endregion

        #region -- 后台系统用户

        /// <summary>
        /// 保存系统用户
        /// </summary>
        /// <param name="SysUserInfo"></param>
        /// <returns></returns>
        internal static ReplayBase SysSaveSysUser(SysUserModel SysUserInfo)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                string DefaultPwd = System.Configuration.ConfigurationManager.AppSettings["DefaultSysUserPassWord"].ToString();
                DefaultPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(DefaultPwd, "MD5");

                string NewUserId = Guid.NewGuid().ToString();
                if (string.IsNullOrEmpty(SysUserInfo.UserId))
                {
                    //新增
                    Us_SysUser dbuser = new Us_SysUser()
                    {
                        CreatedOn = DateTime.Now,
                        FullName = SysUserInfo.FullName,
                        JobTitle = SysUserInfo.JobTitle,
                        LoginName = SysUserInfo.LoginName,
                        MobilePhone = SysUserInfo.MobilePhone,
                        PassWords = DefaultPwd.ToLower(),
                        Us_SysRoleId = SysUserInfo.SysRole.RoleId,
                        Us_SysUserId = NewUserId,
                        IsDeleted = false,
                        IsDisabled = false
                    };
                    context.Us_SysUser.InsertOnSubmit(dbuser);
                }
                else
                {
                    //编辑
                    NewUserId = SysUserInfo.UserId;
                    Us_SysUser dbuser = context.Us_SysUser.Single(p => p.Us_SysUserId == SysUserInfo.UserId);
                    dbuser.FullName = SysUserInfo.FullName;
                    dbuser.JobTitle = SysUserInfo.JobTitle;
                    dbuser.LoginName = SysUserInfo.LoginName;
                    dbuser.MobilePhone = SysUserInfo.MobilePhone;
                    dbuser.Us_SysRoleId = SysUserInfo.SysRole.RoleId;
                    dbuser.IsDisabled = SysUserInfo.IsDisabled;
                }
                context.SubmitChanges();
                return new ReplayBase()
                {
                    ReturnMessage = NewUserId
                };
            }
        }

        /// <summary>
        /// 查询系统用户
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplaySysUserListModel SysSearchSysUserList(RequestSysUserListModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                List<long> RoleId = new List<long>();
                if (condtion.RoleId != null) {
                    if (condtion.RoleId.Count > 0) {
                        RoleId = condtion.RoleId;
                    }
                }

                ReplaySysUserListModel result = new ReplaySysUserListModel();
                int count = (from c in context.Us_SysUser
                             join r in context.Us_SysRole on c.Us_SysRoleId equals r.Us_SysRoleId into pro
                             from x in pro.DefaultIfEmpty()
                             where (c.IsDeleted == false)
                             && (string.IsNullOrEmpty(condtion.KeyWords) ? true : (SqlMethods.Like(c.FullName, string.Format("%{0}%", condtion.KeyWords)) || SqlMethods.Like(c.LoginName, string.Format("%{0}%", condtion.KeyWords)) || SqlMethods.Like(c.MobilePhone, string.Format("%{0}%", condtion.KeyWords))))
                             && (condtion.RoleId == null ? true : RoleId.Contains(c.Us_SysRoleId))
                             select c.Us_SysUserId).Count();
                result.total = count;
                if (result.total > 0)
                {
                    result.UserList = (from c in context.Us_SysUser
                                       join r in context.Us_SysRole on c.Us_SysRoleId equals r.Us_SysRoleId into pro
                                       from x in pro.DefaultIfEmpty()
                                       where (c.IsDeleted == false)
                                       && (string.IsNullOrEmpty(condtion.KeyWords) ? true : (SqlMethods.Like(c.FullName, string.Format("%{0}%", condtion.KeyWords)) || SqlMethods.Like(c.LoginName, string.Format("%{0}%", condtion.KeyWords)) || SqlMethods.Like(c.MobilePhone, string.Format("%{0}%", condtion.KeyWords))))
                                       && (condtion.RoleId == null ? true : RoleId.Contains(c.Us_SysRoleId))
                                       orderby c.CreatedOn descending
                                       select new SysUserModel
                                       {
                                           CreatedOn = c.CreatedOn,
                                           FullName = c.FullName,
                                           IsDisabled = c.IsDisabled,
                                           JobTitle = c.JobTitle,
                                           LoginName = c.LoginName,
                                           MobilePhone = c.MobilePhone,
                                           SysRole = new SysUserRoleListModel()
                                           {
                                               RoleId = x.Us_SysRoleId,
                                               RoleName = x.RoleName
                                           },
                                           UserId = c.Us_SysUserId
                                       }).Skip((condtion.Page - 1) * condtion.PageSize).Take(condtion.PageSize).ToList();
                }
                return result;
            }
        }




        /// <summary>
        /// 获取系统用户信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        internal static SysUserModel SysGetSysUserById(string Id)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                Us_SysUser c = context.Us_SysUser.Single(p => p.Us_SysUserId == Id);
                SysUserModel UserInfo = new SysUserModel()
                {
                    CreatedOn = c.CreatedOn,
                    FullName = c.FullName,
                    IsDisabled = c.IsDisabled,
                    JobTitle = c.JobTitle,
                    LoginName = c.LoginName,
                    MobilePhone = c.MobilePhone,
                    SysRole = SysGetSysRoleListModelById(c.Us_SysRoleId),
                    UserId = c.Us_SysUserId,
                    nickName = c.NickName
                };
                return UserInfo;
            }
        }
        /// <summary>
        /// 删除用户信息-标记删除
        /// </summary>
        /// <param name="SysUserInfo"></param>
        /// <returns></returns>
        internal static ReplayBase SysDelSysUser(SysUserModel SysUserInfo)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                Us_SysUser c = context.Us_SysUser.Single(p => p.Us_SysUserId == SysUserInfo.UserId);
                c.IsDeleted = true;
                context.SubmitChanges();
                return new ReplayBase() { };
            }
        }

        

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="SysUserInfo"></param>
        /// <returns></returns>
        internal static ReplayBase SysReSetUserPwd(SysUserModel SysUserInfo)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                string DefaultPwd = System.Configuration.ConfigurationManager.AppSettings["DefaultSysUserPassWord"].ToString();
                DefaultPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(DefaultPwd, "MD5");

                Us_SysUser c = context.Us_SysUser.Single(p => p.Us_SysUserId == SysUserInfo.UserId);
                c.PassWords = DefaultPwd.ToLower();
                context.SubmitChanges();
                return new ReplayBase() { };
            }
        }

        /// <summary>
        /// 系统用户登录验证
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase SysSysUserLogon(RequestUserLogoModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                ReplayBase result = new ReplayBase();

                //允许登陆后台的用户验证角色为Admin和Users
                List<long> roles = new List<long>() { 
                    1001,1002
                };

                Us_SysUser dbuser = context.Us_SysUser.SingleOrDefault(p => p.LoginName == condtion.UserName && p.PassWords.ToLower() == condtion.UserPassWord.ToLower() && p.IsDeleted==false && p.IsDisabled == false && roles.Contains(p.Us_SysRoleId));
                if (dbuser != null)
                {
                    result.ReturnCode = EnumErrorCode.Success;
                    result.ReturnMessage = dbuser.Us_SysUserId;
                }
                else
                {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = "用户名或密码错误";
                }
                return result;
            }
        }


        /// <summary>
        /// 修改系统用户密码
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase SysChangeSysUserPwd(RequestChangeUserPwdModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                ReplayBase result = new ReplayBase();
                Us_SysUser dbuser = context.Us_SysUser.Single(p => p.Us_SysUserId == condtion.UserId);
                if (dbuser.PassWords == condtion.OldPassword)
                {
                    dbuser.PassWords = condtion.NewPassword;
                    context.SubmitChanges();
                }
                else
                {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = "当前密码验证错误";
                }
                return result;
            }
        }

        /// <summary>
        /// 差价系统用户登录名在数据库中出现的次数
        /// </summary>
        /// <param name="LoginName">登录名</param>
        /// <param name="SysUserId">需要排除的用户ID</param>
        /// <returns></returns>
        internal static int SysCheckSysUserLoginNameisExist(string LoginName, string SysUserId = null)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                return (from c in context.Us_SysUser
                        where (c.LoginName.ToLower() == LoginName.ToLower())
                        && (string.IsNullOrEmpty(SysUserId) ? true : c.Us_SysUserId != SysUserId)
                        select c.Us_SysUserId).Count();
            }
        }

        #endregion

        #region-- 后台系统用户日志

        /// <summary>
        /// 创建后台系统用户日志
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase SysCreateSysUserLog(SysUserLogModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                Us_SysUserLog dblog = new Us_SysUserLog() { 
                    CreatedOn = DateTime.Now,
                    Describe = condtion.Describe,
                    FkId = condtion.FkId,
                    Us_SysUserId = condtion.SysUserId
                };
                context.Us_SysUserLog.InsertOnSubmit(dblog);
                context.SubmitChanges();
                return new ReplayBase() { };
            }
        }
        /// <summary>
        /// 查询后台系统用户日志
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplaySearchSysUserLog SysSearchSysUserLog(RequestSearchSysUserLog condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                ReplaySearchSysUserLog result = new ReplaySearchSysUserLog();
                result.total = (from c in context.Us_SysUserLog
                                join u in context.Us_SysUser on c.Us_SysUserId equals u.Us_SysUserId
                                join r in context.Us_SysRole on u.Us_SysRoleId equals r.Us_SysRoleId
                                where (1 == 1)
                                && (condtion.BeginDate == null ? true : c.CreatedOn >= condtion.BeginDate)
                                && (condtion.EndDate == null ? true : c.CreatedOn <= condtion.EndDate)
                                && (string.IsNullOrEmpty(condtion.KeyWords) ? true : (SqlMethods.Like(c.Describe, string.Format("%{0}%", condtion.KeyWords)) || SqlMethods.Like(u.FullName, string.Format("%{0}%", condtion.KeyWords))))
                                && (string.IsNullOrEmpty(condtion.SysUserId) ? true : c.Us_SysUserId == condtion.SysUserId)
                                select c.Us_SysUserLogId).Count();
                if (result.total > 0)
                {
                    result.rows = (from c in context.Us_SysUserLog
                                   join u in context.Us_SysUser on c.Us_SysUserId equals u.Us_SysUserId
                                   join r in context.Us_SysRole on u.Us_SysRoleId equals r.Us_SysRoleId
                                   where (1 == 1)
                                   && (condtion.BeginDate == null ? true : c.CreatedOn >= condtion.BeginDate)
                                   && (condtion.EndDate == null ? true : c.CreatedOn <= condtion.EndDate)
                                   && (string.IsNullOrEmpty(condtion.KeyWords) ? true : (SqlMethods.Like(c.Describe, string.Format("%{0}%", condtion.KeyWords)) || SqlMethods.Like(u.FullName, string.Format("%{0}%", condtion.KeyWords))))
                                   && (string.IsNullOrEmpty(condtion.SysUserId) ? true : c.Us_SysUserId == condtion.SysUserId)
                                   orderby c.Us_SysUserLogId descending
                                   select new SysUserLogModel
                                   {
                                       CreatedOn = c.CreatedOn,
                                       Describe = c.Describe,
                                       FkId = c.FkId,
                                       LogId = c.Us_SysUserLogId,
                                       SysUserId = c.Us_SysUserId,
                                       SysUserInfo = new SysUserModel()
                                       {
                                           FullName = u.FullName,
                                           MobilePhone = u.MobilePhone,
                                           LoginName = u.LoginName,
                                           SysRole = new SysUserRoleListModel()
                                           {
                                               RoleName = r.RoleName,
                                               RoleId = r.Us_SysRoleId
                                           }
                                       }
                                   }).Skip((condtion.Page - 1) * condtion.PageSize).Take(condtion.PageSize).ToList();
                }
                else {
                    result.rows = new List<SysUserLogModel>();
                }
                return result;
            }
        }

        #endregion

        #region -- 后台系统更新日志

        /// <summary>
        /// 保存系统更新日志
        /// </summary>
        /// <param name="sysUpdateLogModel"></param>
        /// <returns></returns>
        internal static ReplayBase SysSaveSysUpdateLog(SysUpdateLogModel sysUpdateLogModel)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                if (sysUpdateLogModel.LogId == 0)
                {
                    //新增
                    Us_SysUpdateLog dblog = new Us_SysUpdateLog()
                    {
                        CreatedOn = DateTime.Now,
                        LogDescribe = sysUpdateLogModel.LogDescribe,
                        LogType = (byte)sysUpdateLogModel.LogType,
                        SysUserId = sysUpdateLogModel.SysUserId
                    };
                    context.Us_SysUpdateLog.InsertOnSubmit(dblog);
                }
                else
                {
                    //编辑
                    Us_SysUpdateLog dblog = context.Us_SysUpdateLog.Single(p => p.Us_SysUpdateLogId == sysUpdateLogModel.LogId);
                    dblog.LogType = (byte)sysUpdateLogModel.LogType;
                    dblog.LogDescribe = sysUpdateLogModel.LogDescribe;
                }
                context.SubmitChanges();
                return new ReplayBase() { };
            }
        }
        /// <summary>
        /// 查询系统更新日志列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static SearchSysUpdateLogReplayModel SysSearchSysUpdateLog(SearchSysUpdateLogRequstModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                SearchSysUpdateLogReplayModel result = new SearchSysUpdateLogReplayModel();
                long count = (from c in context.Us_SysUpdateLog
                             where (1 == 1)
                             && (string.IsNullOrEmpty(condtion.KeyWords) ? true : SqlMethods.Like(c.LogDescribe, string.Format("%{0}%", condtion.KeyWords)))
                             && (condtion.LogType == null ? true : c.LogType == condtion.LogType.GetHashCode())
                             && (condtion.BeginDate == null ? true : c.CreatedOn >= condtion.BeginDate)
                             && (condtion.EndDate == null ? true : c.CreatedOn <= condtion.EndDate)
                             select c.Us_SysUpdateLogId).Count();
                if (count > 0)
                {
                    result.total = count;
                    result.rowslist = (from c in context.Us_SysUpdateLog
                                       join s in context.Us_SysUser on c.SysUserId equals s.Us_SysUserId
                                       where (1 == 1)
                                       && (string.IsNullOrEmpty(condtion.KeyWords) ? true : SqlMethods.Like(c.LogDescribe, string.Format("%{0}%", condtion.KeyWords)))
                                       && (condtion.LogType == null ? true : c.LogType == condtion.LogType.GetHashCode())
                                       && (condtion.BeginDate == null ? true : c.CreatedOn >= condtion.BeginDate)
                                       && (condtion.EndDate == null ? true : c.CreatedOn <= condtion.EndDate)
                                       orderby c.CreatedOn descending
                                       select new SysUpdateLogModel
                                       {
                                           CreatedBy = s.FullName,
                                           CreatedOn = c.CreatedOn,
                                           LogDescribe = c.LogDescribe,
                                           LogId = c.Us_SysUpdateLogId,
                                           LogType = (EnumUpdateLogType)c.LogType,
                                           SysUserId = c.SysUserId
                                       }).Skip((condtion.Page - 1) * condtion.PageSize).Take(condtion.PageSize).ToList();
                }
                else
                {
                    result.rowslist = new List<SysUpdateLogModel>();
                }
                return result;
            }
        }
        /// <summary>
        /// 删除系统更新日志
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        internal static ReplayBase SysDelSysUpdateLogById(long Id)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                Us_SysUpdateLog dblog = context.Us_SysUpdateLog.SingleOrDefault(p => p.Us_SysUpdateLogId == Id);
                if (dblog != null)
                {
                    context.Us_SysUpdateLog.DeleteOnSubmit(dblog);
                    context.SubmitChanges();
                }
                return new ReplayBase() { };
            }
        }

        #endregion

        #region -- 系统错误日志

        /// <summary>
        /// 保存系统运行日志
        /// </summary>
        /// <param name="dblog"></param>
        internal static void SysSaveErrorLogMsg(Us_SysLog dblog)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                context.Us_SysLog.InsertOnSubmit(dblog);
                context.SubmitChanges();
            }
        }

        /// <summary>
        /// 查询系统运行日志
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static GetSysErrorLogReplayModel SysSearchSysLog(GetSysLogRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                GetSysErrorLogReplayModel result = new GetSysErrorLogReplayModel();
                long count = (from c in context.Us_SysLog
                              where (1 == 1)
                              && (condtion.LogType == null ? true : c.type == (byte)condtion.LogType.GetHashCode())
                              && (condtion.BeginDate == null ? true : c.CreatedOn >= condtion.BeginDate)
                              && (condtion.EndDate == null ? true : c.CreatedOn <= condtion.EndDate)
                              select c.Us_SysLogId).Count();
                if (count > 0)
                {
                    result.total = count;
                    result.rows = (from c in context.Us_SysLog
                                      where (1 == 1)
                                      && (condtion.LogType == null ? true : c.type == (byte)condtion.LogType.GetHashCode())
                                      && (condtion.BeginDate == null ? true : c.CreatedOn >= condtion.BeginDate)
                                      && (condtion.EndDate == null ? true : c.CreatedOn <= condtion.EndDate)
                                      orderby c.CreatedOn descending
                                      select new SysErrorLogModel { 
                                        Condtion = c.condtion,
                                        CreatedOn = c.CreatedOn,
                                        Errormsg = c.errormsg,
                                        Id = c.Us_SysLogId,
                                        LogType = (EnumSysLogType)c.type,
                                      }).Skip((condtion.Page - 1) * condtion.PageSize).Take(condtion.PageSize).ToList();
                    foreach (SysErrorLogModel info in result.rows) {
                        info.LogTypeText = info.LogType.ToString();
                    }
                }
                else {
                    result.rows = new List<SysErrorLogModel>();
                }
                return result;
            }
        }
        /// <summary>
        /// 批量删除系统运行日志
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase SysDelSysLog(DelSysLogRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                List<string> Ids = new List<string>();
                foreach (SysErrorLogModel row in condtion.rows) {
                    Ids.Add(row.Id);
                }
                var dellist = (from c in context.Us_SysLog
                         where Ids.Contains(c.Us_SysLogId)
                         select c).ToList();
                if (dellist.Count > 0) {
                    context.Us_SysLog.DeleteAllOnSubmit(dellist);
                    context.SubmitChanges();
                }
                return new ReplayBase()
                {
                    ReturnCode = EnumErrorCode.Success,
                    ReturnMessage = "有" + Ids.Count.ToString() + "条记录被删除"
                };
            }
        }

        #endregion

        #endregion

        #region -- 资源管理 Rs

        #region -- 文档资源管理

        /// <summary>
        /// 新增/编辑文档资源
        /// </summary>
        /// <param name="documentsResourceModel"></param>
        /// <returns></returns>
        internal static ReplayBase RsSaveDocumentResource(DocumentsResourceModel documentsResourceModel)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                string NewId = cyclonestyle.BLL.Helps.GetTimeId();

                if (string.IsNullOrEmpty(documentsResourceModel.DocumentId))
                {
                    Us_RsDocumentResource dbdoc = new Us_RsDocumentResource()
                    {
                        Us_RsDocumentResourceId = NewId,
                        Caption = documentsResourceModel.Caption,
                        Content = documentsResourceModel.Content,
                        GroupName = documentsResourceModel.GroupName,
                        ModifiedOn = DateTime.Now,
                        Sort = documentsResourceModel.Sort,
                        IsDisabled = documentsResourceModel.IsDisabled
                    };
                    context.Us_RsDocumentResource.InsertOnSubmit(dbdoc);
                }
                else {
                    NewId = documentsResourceModel.DocumentId;
                    Us_RsDocumentResource dbdoc = context.Us_RsDocumentResource.Single(p => p.Us_RsDocumentResourceId == documentsResourceModel.DocumentId);
                    dbdoc.Caption = documentsResourceModel.Caption;
                    dbdoc.Content = documentsResourceModel.Content;
                    dbdoc.GroupName = documentsResourceModel.GroupName;
                    dbdoc.ModifiedOn = DateTime.Now;
                    dbdoc.Sort = documentsResourceModel.Sort;
                    dbdoc.IsDisabled = documentsResourceModel.IsDisabled;
                }
                context.SubmitChanges();
                return new ReplayBase()
                {
                    ReturnCode = EnumErrorCode.Success,
                    ReturnMessage = NewId
                };
            }
        }

        /// <summary>
        /// 查询文档资源
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static SearchDocumentsResourceListReplayModel RsSearchDocumentResource(SearchDocumentsResourceListRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                SearchDocumentsResourceListReplayModel result = new SearchDocumentsResourceListReplayModel();
                List<string> GroupName = new List<string>();
                if (condtion.GroupName != null) {
                    if (condtion.GroupName.Count > 0) {
                        GroupName = condtion.GroupName;
                    }
                }

                string OrderString = string.Empty;
                switch (condtion.OrderType) { 
                    case EnumSortOrderType.按时间降序:
                        OrderString = "ModifiedOn desc";
                        break;
                    case EnumSortOrderType.按时间升序:
                        OrderString = "ModifiedOn asc";
                        break;
                    case EnumSortOrderType.按排序号降序:
                        OrderString = "Sort desc";
                        break;
                    case EnumSortOrderType.按排序号升序:
                        OrderString = "Sort asc";
                        break;
                }

                int count = (from c in context.Us_RsDocumentResource
                             where (1 == 1)
                             && (string.IsNullOrEmpty(condtion.DocumentId) ? true : c.Us_RsDocumentResourceId == condtion.DocumentId)
                             && (GroupName.Count == 0 ? true : GroupName.Contains(c.GroupName))
                             && (string.IsNullOrEmpty(condtion.KeyWords) ? true : (SqlMethods.Like(c.Caption, string.Format("%{0}%", condtion.KeyWords)) || SqlMethods.Like(c.Content, string.Format("%{0}%", condtion.KeyWords))))
                             && (condtion.GetDisabled ? true : c.IsDisabled == false)
                             select c.Us_RsDocumentResourceId).Count();
                result.total = count;
                if (result.total > 0)
                {
                    result.rowlist = (from c in context.Us_RsDocumentResource
                                      where (1 == 1)
                                      && (string.IsNullOrEmpty(condtion.DocumentId) ? true : c.Us_RsDocumentResourceId == condtion.DocumentId)
                                      && (GroupName.Count == 0 ? true : GroupName.Contains(c.GroupName))
                                      && (string.IsNullOrEmpty(condtion.KeyWords) ? true : (SqlMethods.Like(c.Caption, string.Format("%{0}%", condtion.KeyWords)) || SqlMethods.Like(c.Content, string.Format("%{0}%", condtion.KeyWords))))
                                      && (condtion.GetDisabled ? true : c.IsDisabled == false)
                                      select new DocumentsResourceModel
                                      {
                                          Caption = c.Caption,
                                          DocumentId = c.Us_RsDocumentResourceId,
                                          Content = c.Content,
                                          GroupName = c.GroupName,
                                          ModifiedOn = c.ModifiedOn,
                                          Sort = c.Sort,
                                          IsDisabled = c.IsDisabled
                                      }).OrderBy(OrderString).Skip((condtion.Page - 1) * condtion.PageSize).Take(condtion.PageSize).ToList();
                    
                }
                else {
                    result.rowlist = new List<DocumentsResourceModel>();
                }
                return result;
            }
        }

        /// <summary>
        /// 删除文档资源
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        internal static ReplayBase RsDelDocumentResource(string Id)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                Us_RsDocumentResource dbdoc = context.Us_RsDocumentResource.Single(p=>p.Us_RsDocumentResourceId==Id);
                context.Us_RsDocumentResource.DeleteOnSubmit(dbdoc);
                context.SubmitChanges();
                return new ReplayBase()
                {
                    ReturnCode = EnumErrorCode.Success
                };
            }
        }

        /// <summary>
        /// 批量排序及禁用文档资源
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase RsSortDocumentsResource(SortDocumentsRequest condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                if (condtion.request.Count > 0) {
                    foreach (DocumentsResourceModel row in condtion.request) {
                        Us_RsDocumentResource dbdoc = context.Us_RsDocumentResource.Single(p => p.Us_RsDocumentResourceId == row.DocumentId);
                        dbdoc.Sort = row.Sort;
                        dbdoc.IsDisabled = row.IsDisabled;
                    }
                    context.SubmitChanges();
                }
                return new ReplayBase();
            }
        }

        /// <summary>
        /// 获取所有已存在的分组信息
        /// </summary>
        /// <returns></returns>
        internal static List<Combobox> RsGetAllDocumentsResourceGroupInfoList()
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                List<Combobox> result = (from c in context.Us_RsDocumentResource
                                         group c.GroupName by c.GroupName into x
                                         where ((x.Key == null || x.Key == "") ? false : true)
                                         select new Combobox
                                         {
                                             id = x.Key,
                                             text = x.Key
                                         }).ToList();
                return result;
            }
        }

        #endregion

        #region-- 用户录入资源规则管理

        /// <summary>
        /// 新增/保存用户录入资源项目
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase RsSaveUserInputRule(ResourceUserInputRuleModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                string NewId = cyclonestyle.BLL.Helps.GetTimeId();

                if (string.IsNullOrEmpty(condtion.RuleId))
                {
                    Us_RsUserInputRule dbrule = new Us_RsUserInputRule()
                    {
                        Caption = condtion.Caption,
                        InputType = condtion.InputType,
                        Options = condtion.OptionsString,
                        Us_RsUserInputRuleId = NewId,
                        ModifiedOn = DateTime.Now,
                        Sort = condtion.Sort
                    };
                    context.Us_RsUserInputRule.InsertOnSubmit(dbrule);
                }
                else
                {
                    NewId = condtion.RuleId;
                    Us_RsUserInputRule dbrule = context.Us_RsUserInputRule.Single(p => p.Us_RsUserInputRuleId == condtion.RuleId);
                    dbrule.Caption = condtion.Caption;
                    dbrule.InputType = condtion.InputType;
                    dbrule.Options = condtion.OptionsString;
                    dbrule.Sort = condtion.Sort;
                    dbrule.ModifiedOn = DateTime.Now;
                }
                context.SubmitChanges();

                return new ReplayBase()
                {
                    ReturnCode = EnumErrorCode.Success,
                    ReturnMessage = NewId
                };
            }
        }

        /// <summary>
        /// 获取用户输入规范项目列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static SearchResourceUserInputRuleReplayModel RsSearchUserInputRuleList(SearchResourceUserInputRuleRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                SearchResourceUserInputRuleReplayModel result = new SearchResourceUserInputRuleReplayModel();
                List<string> ids = new List<string>();
                if (!string.IsNullOrEmpty(condtion.Ids))
                {
                    ids = condtion.Ids.Split(',').ToList();
                }

                string OrderString = "ModifiedOn desc";
                switch (condtion.ordertype)
                {
                    case EnumSortOrderType.按时间升序:
                        OrderString = "ModifiedOn asc";
                        break;
                    case EnumSortOrderType.按排序号降序:
                        OrderString = "Sort desc";
                        break;
                    case EnumSortOrderType.按排序号升序:
                        OrderString = "Sort asc";
                        break;
                }

                result.total = (from c in context.Us_RsUserInputRule
                                where (1 == 1)
                                && (ids.Count == 0 ? true : ids.Contains(c.Us_RsUserInputRuleId))
                                && (c.IsDeleted == false)
                                select c.Us_RsUserInputRuleId).Count();
                if (result.total > 0)
                {
                    result.rows = (from c in context.Us_RsUserInputRule
                                   where (1 == 1)
                                   && (ids.Count == 0 ? true : ids.Contains(c.Us_RsUserInputRuleId))
                                   && (c.IsDeleted == false)
                                   select new ResourceUserInputRuleModel
                                   {
                                       Caption = c.Caption,
                                       InputType = c.InputType,
                                       OptionsString = c.Options,
                                       RuleId = c.Us_RsUserInputRuleId,
                                       Sort = c.Sort,
                                       ModifiedOn = c.ModifiedOn
                                   }).OrderBy(OrderString).Skip((condtion.Page - 1) * condtion.PageSize).Take(condtion.PageSize).ToList();
                }
                else
                {
                    result.rows = new List<ResourceUserInputRuleModel>();
                }

                return result;
            }
        }

        /// <summary>
        /// 批量删除用户资源集合
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        internal static ReplayBase RsDelUserInputRules(List<string> Ids)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                var result = (from c in context.Us_RsUserInputRule
                              where Ids.Contains(c.Us_RsUserInputRuleId)
                              select c).ToList();
                if (result.Count > 0)
                {
                    foreach (var d in result)
                    {
                        d.IsDeleted = true;
                    }
                    context.SubmitChanges();
                    return new ReplayBase()
                    {
                        ReturnCode = EnumErrorCode.Success,
                        ReturnMessage = string.Format("已成功删除{0}条数据", result.Count())
                    };
                }
                else
                {
                    return new ReplayBase()
                    {
                        ReturnCode = EnumErrorCode.EmptyDate,
                        ReturnMessage = "没有数据被删除"
                    };
                }
            }
        }

        /// <summary>
        /// 获取已有的输入类型集合
        /// </summary>
        /// <returns></returns>
        internal static List<Combobox> RsGetUserInputRuleTypes()
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                List<Combobox> result = (from c in context.Us_RsUserInputRule
                                         where (c.IsDeleted == false)
                                         group c.InputType by c.InputType into x
                                         select new Combobox
                                         {
                                             id = x.Key,
                                             text = x.Key
                                         }).ToList();
                return result;
            }
        }

        #endregion

        #region  -- 分类树

        /// <summary>
        /// 新增/保存分类树
        /// </summary>
        /// <param name="catInfoModel"></param>
        /// <returns></returns>
        internal static ReplayBase RsSaveCatInfo(CatInfoModel catInfoModel)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                string NewId = cyclonestyle.BLL.Helps.GetTimeId();
                if (string.IsNullOrEmpty(catInfoModel.id))
                {
                    //新增
                    Us_RsCatInfo dbcat = new Us_RsCatInfo()
                    {
                        Caption = catInfoModel.caption,
                        ModifiedOn = DateTime.Now,
                        parentid = string.IsNullOrEmpty(catInfoModel._parentId) ? null : catInfoModel._parentId,
                        Sort = catInfoModel.sort,
                        Us_RsCatInfoId = NewId,
                        IsDeleted = false
                    };
                    context.Us_RsCatInfo.InsertOnSubmit(dbcat);
                }
                else
                {
                    //编辑
                    Us_RsCatInfo dbcar = context.Us_RsCatInfo.Single(p => p.Us_RsCatInfoId == catInfoModel.id);
                    dbcar.ModifiedOn = DateTime.Now;
                    dbcar.Sort = catInfoModel.sort;
                    dbcar.Caption = catInfoModel.caption;
                    dbcar.parentid = string.IsNullOrEmpty(catInfoModel._parentId) ? null : catInfoModel._parentId;
                }
                context.SubmitChanges();
                return new ReplayBase()
                {
                    ReturnCode = EnumErrorCode.Success,
                    ReturnMessage = NewId
                };
            }
        }
        /// <summary>
        /// 获取分类树节点信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static CatInfoModel RsGetCatInfoById(string id)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                Us_RsCatInfo dbcat = context.Us_RsCatInfo.Single(p => p.Us_RsCatInfoId == id);
                return new CatInfoModel()
                {
                    _parentId = dbcat.parentid,
                    caption = dbcat.Caption,
                    id = dbcat.Us_RsCatInfoId,
                    ModifiedOn = dbcat.ModifiedOn,
                    sort = dbcat.Sort
                };
            }
        }

        /// <summary>
        /// 查询分类树节点
        /// </summary>
        /// <param name="searchCatInfoRequest"></param>
        /// <returns></returns>
        internal static List<CatInfoModel> RsSearchCatInfoList(SearchCatInfoRequest searchCatInfoRequest)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                List<CatInfoModel> result = (from c in context.Us_RsCatInfo
                                             where (c.IsDeleted == false)
                                             && (string.IsNullOrEmpty(searchCatInfoRequest._parentId) ? (string.IsNullOrEmpty(searchCatInfoRequest.id) ? (string.IsNullOrEmpty(searchCatInfoRequest.KeyWords) ? c.parentid == null : true) : true) : c.parentid == searchCatInfoRequest._parentId)
                                             && (string.IsNullOrEmpty(searchCatInfoRequest.id) ? true : c.Us_RsCatInfoId == searchCatInfoRequest.id)
                                             && (string.IsNullOrEmpty(searchCatInfoRequest.KeyWords) ? true : SqlMethods.Like(c.Caption, string.Format("%{0}%", searchCatInfoRequest.KeyWords)))
                                             orderby c.Sort descending, c.ModifiedOn descending
                                             select new CatInfoModel
                                             {
                                                 _parentId = c.parentid,
                                                 caption = c.Caption,
                                                 id = c.Us_RsCatInfoId,
                                                 sort = c.Sort,
                                                 ModifiedOn = c.ModifiedOn
                                             }).ToList();
                return result;
            }
        }

        /// <summary>
        /// 标记删除分类树节点
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        internal static ReplayBase RsDelCatTree(List<string> Ids)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                var rows = (from c in context.Us_RsCatInfo
                            where Ids.Contains(c.Us_RsCatInfoId)
                            select c).ToList();
                foreach (var row in rows)
                {
                    row.IsDeleted = true;
                    row.ModifiedOn = DateTime.Now;
                }
                context.SubmitChanges();
                return new ReplayBase()
                {
                    ReturnCode = EnumErrorCode.Success,
                    ReturnMessage = string.Format("成功删除{0}条数据", rows.Count)
                };
            }
        }

        #endregion

        #region -- 扩展资源信息


        /// <summary>
        /// 获取扩展信息集
        /// </summary>
        /// <param name="fkId">关联的ID</param>
        /// <returns></returns>
        internal static List<InfoExInfoModel> RsGetExInfoList(string fkId)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                List<InfoExInfoModel> result = (from c in context.Us_RsExinfo
                                                   join x in context.Us_RsCatInfo on c.catId equals x.Us_RsCatInfoId
                                                   where c.fkId == fkId
                                                   && c.isDelete == false
                                                   orderby x.Sort descending
                                                   select new InfoExInfoModel
                                                   {
                                                       detail = c.detail,
                                                       exInfoId = c.Us_RsExinfoId,
                                                       cat = new CatInfoModel()
                                                       {
                                                           caption = x.Caption,
                                                           id = x.Us_RsCatInfoId,
                                                           ModifiedOn = x.ModifiedOn,
                                                           sort = x.Sort,
                                                           _parentId = x.parentid
                                                       },
                                                       modifiedOn = c.modifiedOn,
                                                       modifyedBy = SysGetSysUserById(c.modifiedBy)
                                                   }).ToList();
                return result;
            }
        }
        /// <summary>
        /// 新增/编辑扩展信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase RsEditExInfo(EditInfoExInfoRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                string newId = condtion.exInfoId;
                if (string.IsNullOrEmpty(newId))
                {
                    newId = Guid.NewGuid().ToString();
                    //新增
                    Us_RsExinfo d = new Us_RsExinfo()
                    {
                        catId = condtion.cat.id,
                        detail = condtion.detail,
                        modifiedBy = condtion.modifiedBy,
                        modifiedOn = DateTime.Now,
                        Us_RsExinfoId = newId,
                        fkId = condtion.fkId,
                        isDelete = false
                    };
                    context.Us_RsExinfo.InsertOnSubmit(d);
                }
                else {
                    //编辑
                    Us_RsExinfo d = context.Us_RsExinfo.Single(p => p.Us_RsExinfoId == newId);
                    d.modifiedBy = condtion.modifiedBy;
                    d.modifiedOn = DateTime.Now;
                    d.catId = condtion.cat.id;
                    d.detail = condtion.detail;
                }
                context.SubmitChanges();
                return new ReplayBase()
                {
                    ReturnMessage = newId
                };
            }
        }

        /// <summary>
        /// 标记删除扩展信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase RsDelExInfo(EditInfoExInfoRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                Us_RsExinfo d = context.Us_RsExinfo.Single(p => p.Us_RsExinfoId == condtion.exInfoId);
                d.isDelete = true;
                d.modifiedBy = condtion.modifiedBy;
                d.modifiedOn = DateTime.Now;
                context.SubmitChanges();
                return new ReplayBase();
            }
        }

        #endregion

        #region -- 地理位置
        /// <summary>
        /// 根据城市名称获取城市ID，如果没有匹配则返回字符串空值
        /// </summary>
        /// <param name="citynane"></param>
        /// <returns></returns>
        internal static string RsGetCityIdByCityName(string citynane)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                string cityid = string.Empty;
                ElongHotel_Geos dbg = context.ElongHotel_Geos.SingleOrDefault(p => p.CityName == citynane);
                if (dbg != null)
                {
                    cityid = dbg.CityCode;
                }
                return cityid;
            }
        }

        /// <summary>
        /// 获取省份信息列表
        /// </summary>
        /// <returns></returns>
        internal static List<Combobox> RsGetProvniceList()
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                List<Combobox> result = new List<Combobox>();
                List<Combobox> rows = (from c in context.ElongHotel_Geos
                                         group c by new { c.Country, c.ProvinceId, c.ProvinceName } into g
                                         orderby g.Key.ProvinceId
                                         select new Combobox
                                         {
                                             id = g.Key.ProvinceId,
                                             text = g.Key.ProvinceName
                                         }).ToList();
                foreach (Combobox row in rows) {
                    //排除港澳台以及“首尔特别市”
                    if (int.Parse(row.id) < 3200) {
                        result.Add(row);
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 根据省ID获取城市列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static List<GeoCityInfoModel> RsGetCityListByProvniceId(string id)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                List<GeoCityInfoModel> result = (from c in context.View_ElongHotelGeos
                                                 where c.ProvinceId == id
                                                 && !SqlMethods.Like(c.CityName, string.Format("%{0}%", "（"))//排除名字中有括号的
                                                 orderby c.CityCode
                                                 select new GeoCityInfoModel
                                                 {
                                                     CityName = c.CityName,
                                                     CityCode = c.CityCode,
                                                     PY = c.PY,
                                                     PinYin = c.PinYin
                                                 }).ToList();
                return result;
            }
        }

        /// <summary>
        /// 根据市ID检索区域信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static SearchAreaInfoListReplayModel RsGetAreaInfoListByCityCode(SearchAreaInfoListRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                SearchAreaInfoListReplayModel result = new SearchAreaInfoListReplayModel();
                if (string.IsNullOrEmpty(condtion.q)) {
                    condtion.q = "";
                }
                result.total = (from c in context.View_ElongHotelGeosLocations
                                where c.PatendId == condtion.CityCode
                                && (string.IsNullOrEmpty(condtion.q) ? true : ((SqlMethods.Like(c.Name, string.Format("%{0}%", condtion.q))) || (SqlMethods.Like(c.PY.ToLower(), string.Format("%{0}%", condtion.q.ToLower()))) || (SqlMethods.Like(c.PinYin.ToLower(), string.Format("%{0}%", condtion.q.ToLower())))))
                                && (condtion.T == EnumAreaType.A ? true : condtion.T.ToString() == c.T)
                                select new
                                {
                                    c.Id
                                }).Count();
                if (result.total > 0)
                {
                    if (!condtion.UsePageSize)
                    {
                        result.rows = (from c in context.View_ElongHotelGeosLocations
                                       where c.PatendId == condtion.CityCode
                                       && (string.IsNullOrEmpty(condtion.q) ? true : ((SqlMethods.Like(c.Name, string.Format("%{0}%", condtion.q))) || (SqlMethods.Like(c.PY.ToLower(), string.Format("%{0}%", condtion.q.ToLower()))) || (SqlMethods.Like(c.PinYin.ToLower(), string.Format("%{0}%", condtion.q.ToLower())))))
                                       && (condtion.T == EnumAreaType.A ? true : condtion.T.ToString() == c.T)
                                       orderby c.Id ascending
                                       select new GeoAreaInfoModel
                                       {
                                           id = c.Id,
                                           text = c.Name,
                                           T = (EnumAreaType)Enum.Parse(typeof(EnumAreaType), c.T)
                                       }).ToList();
                    }
                    else {
                        result.rows = (from c in context.View_ElongHotelGeosLocations
                                       where c.PatendId == condtion.CityCode
                                       && (string.IsNullOrEmpty(condtion.q) ? true : ((SqlMethods.Like(c.Name, string.Format("%{0}%", condtion.q))) || (SqlMethods.Like(c.PY.ToLower(), string.Format("%{0}%", condtion.q.ToLower()))) || (SqlMethods.Like(c.PinYin.ToLower(), string.Format("%{0}%", condtion.q.ToLower())))))
                                       && (condtion.T == EnumAreaType.A ? true : condtion.T.ToString() == c.T)
                                       orderby c.Id ascending
                                       select new GeoAreaInfoModel
                                       {
                                           id = c.Id,
                                           text = c.Name,
                                           T = (EnumAreaType)Enum.Parse(typeof(EnumAreaType), c.T)
                                       }).Skip((condtion.Page - 1) * condtion.PageSize).Take(condtion.PageSize).ToList();
                    }
                }
                else {
                    result.rows = new List<GeoAreaInfoModel>();
                }
                return result;
            }
        }

        /// <summary>
        /// 获取全国热门城市列表，按省排列，没省取前三城市
        /// </summary>
        /// <returns></returns>
        internal static List<GeoCityInfoModel> RsGetHotCityList()
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                List<GeoCityInfoModel> result = new List<GeoCityInfoModel>();
                List<Combobox> plist = RsGetProvniceList();
                foreach (Combobox p in plist) {
                    List<GeoCityInfoModel> clist = (from c in context.View_ElongHotelGeos
                                                    where c.ProvinceId == p.id
                                                    && (!SqlMethods.Like(c.CityName, string.Format("%{0}%", "（")))//排除名字中有括号的
                                                    orderby c.CityCode
                                                    select new GeoCityInfoModel
                                                    {
                                                        CityName = c.CityName,
                                                        CityCode = c.CityCode,
                                                        PY = c.PY,
                                                        PinYin = c.PinYin,
                                                        ProvniceInfo = p
                                                    }).Skip(0).Take(3).ToList();
                    result.AddRange(clist);
                }
                return result;
            }
        }
        /// <summary>
        /// 检索城市
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        internal static List<GeoCityInfoModel> RsSearchCityByKeyWords(string q)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                List<GeoCityInfoModel> result = (from c in context.View_ElongHotelGeos
                                                 where SqlMethods.Like(c.CityName, string.Format("%{0}%", q)) || SqlMethods.Like(c.PY.ToLower(), string.Format("%{0}%", q.ToLower())) || SqlMethods.Like(c.PinYin.ToLower(), string.Format("%{0}%", q.ToLower()))
                                                 orderby c.CityCode
                                                 select new GeoCityInfoModel
                                                 {
                                                     CityName = c.CityName,
                                                     CityCode = c.CityCode,
                                                     PinYin = c.PinYin,
                                                     PY = c.PY,
                                                     ProvniceInfo = new Combobox()
                                                     {
                                                         id = c.ProvinceId,
                                                         text = c.ProvinceName
                                                     }
                                                 }).Skip(0).Take(20).ToList();
                return result;
            }
        }

        /// <summary>
        /// 获取详情的城市，行政区信息
        /// </summary>
        /// <param name="locationInfo"></param>
        /// <returns></returns>
        internal static LocationInfoModel RsGetLocaionInfo(LocationInfoModel locationInfo) {
            FullLocationInfoModel r = new FullLocationInfoModel() {
                cityInfo = new GeoCityInfoModel() {
                    CityCode = locationInfo.cityInfo.CityCode
                },
                DistrictId = locationInfo.DistrictId
            };
            r = RsGetLocaionInfo(r);
            return new LocationInfoModel()
            {
                cityInfo = r.cityInfo,
                District = r.District,
                DistrictId = r.DistrictId
            };
        }

        /// <summary>
        /// 获取详细的城市，行政区以及商圈信息
        /// </summary>
        /// <param name="locationInfo"></param>
        /// <returns></returns>
        internal static FullLocationInfoModel RsGetLocaionInfo(FullLocationInfoModel locationInfo)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                if (locationInfo.cityInfo != null) {
                    if (!string.IsNullOrEmpty(locationInfo.cityInfo.CityCode)) {
                        string citycode = locationInfo.cityInfo.CityCode;
                        ElongHotel_Geos cityinfo = context.ElongHotel_Geos.SingleOrDefault(p => p.CityCode == citycode);
                        if (cityinfo != null) {
                            locationInfo.cityInfo = new GeoCityInfoModel()
                            {
                                CityCode = citycode,
                                CityName = cityinfo.CityName,
                                ProvniceInfo = new Combobox()
                                {
                                    id = cityinfo.ProvinceId,
                                    text = cityinfo.ProvinceName
                                }
                            };
                            if (!string.IsNullOrEmpty(locationInfo.DistrictId))
                            {
                                ElongHotel_GeoLocations DbDisTrice = context.ElongHotel_GeoLocations.SingleOrDefault(p => p.PatendId == citycode && p.Id == locationInfo.DistrictId && p.T == "D");
                                if (DbDisTrice != null)
                                {
                                    locationInfo.District = DbDisTrice.Name;
                                }

                            }
                            if (!string.IsNullOrEmpty(locationInfo.BusinessZoneId))
                            {
                                ElongHotel_GeoLocations dbZone = context.ElongHotel_GeoLocations.SingleOrDefault(p => p.PatendId == citycode && p.Id == locationInfo.BusinessZoneId && p.T == "C");
                                if (dbZone != null)
                                {
                                    locationInfo.BusinessZone = dbZone.Name;
                                }
                            }
                        }
                    }
                }
                return locationInfo;
            }
        }

        #endregion

        #endregion

        #region -- 会员 Member

        /// <summary>
        /// 会员自助注册
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static RegisterMembersReplayModel MemberRegisterMembers(RegisterMembersRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                string NewUserId = Guid.NewGuid().ToString();
                Us_SysUser dbuser = new Us_SysUser() { 
                    CreatedOn = DateTime.Now,
                    IsDeleted = false,
                    IsDisabled = false,
                    LoginName = condtion.LoginName,
                    PassWords = condtion.PassWord,
                    MobilePhone = condtion.LoginName,
                    Us_SysUserId = NewUserId,
                    Us_SysRoleId = 1009
                };
                context.Us_SysUser.InsertOnSubmit(dbuser);
                context.SubmitChanges();
                return new RegisterMembersReplayModel()
                {
                    ReturnCode = EnumErrorCode.Success,
                    UserInfo = new MembersBaseInfoModel()
                    { 
                        UserId = NewUserId
                    }
                };
            }
        }
        
        /// <summary>
        /// 验证登录名及密码
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static RegisterMembersReplayModel MemberCheckMemberLoginNameandPwd(RequestUserLogoModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                RegisterMembersReplayModel result = new RegisterMembersReplayModel();
                //允许登陆后台的用户验证角色为Members
                List<long> roles = new List<long>() { 
                    1009
                };

                Us_SysUser dbuser = context.Us_SysUser.SingleOrDefault(p => p.LoginName == condtion.UserName && p.PassWords.ToLower() == condtion.UserPassWord.ToLower() && p.IsDeleted == false && p.IsDisabled == false && roles.Contains(p.Us_SysRoleId));
                if (dbuser != null)
                {
                    result.ReturnCode = EnumErrorCode.Success;
                    result.UserInfo = new MembersBaseInfoModel()
                    {
                        UserId = dbuser.Us_SysUserId
                    };
                }
                else
                {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = "用户名或密码错误";
                }
                return result;
            }
        }

        /// <summary>
        /// 取会员基础数据
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static MembersBaseInfoModel MemberGetMemberBaseInfo(GetMembersInfoRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                Us_SysUser dbuser = context.Us_SysUser.Single(p => p.Us_SysUserId == condtion.userId);

                MembersBaseInfoModel Info = new MembersBaseInfoModel()
                {
                    LoginName = dbuser.LoginName,
                    MobilePhone = dbuser.MobilePhone,



                    UserId = dbuser.Us_SysUserId,
                    createdon = dbuser.CreatedOn,
                    FullName = dbuser.FullName,
                    IdCard = dbuser.IdCard,
                    IdType = (EnumUserIdType)(dbuser.IdType == null ? 0 : dbuser.IdType),
                    isDisabled = dbuser.IsDisabled,
                    NickName = dbuser.NickName,
                    Sex = (EnumSex)(dbuser.Sex == null ? 0 : dbuser.Sex),
                    UserFace = dbuser.UserFace,
                    Email = dbuser.Email,
                    userDepartmentList = (from d in dbuser.Us_MemberDepartment
                                          where (1 == 1)
                                          && ((!string.IsNullOrEmpty(condtion.orgId)) ? d.Us_DepDepartMentRootId == condtion.orgId : true)//获取用户的部门信息条件设置，当orgid不为空并且不获取其他组织的部门的时候，过滤掉其他组织的部门
                                          orderby d.Us_DepDepartMentRootId ascending //按根部门排序
                                          select new UserDepartmentInfoList
                                          {
                                              path = d.path,
                                              pathText = d.pathText,
                                              orgId = d.Us_DepDepartMentRootId,
                                              departmentId = d.Us_DepDepartMentId,
                                              isCaption = d.isCaption
                                          }).ToList(),
                    baiduPushSet = SysSmsDataBaseManager.MemberGetMemberBaiduPushSetInfo(dbuser),
                    getuiPushSet = SysSmsDataBaseManager.MemberGetMemberGetuiPushSetInfo(dbuser)
                };
                return Info;
            }
        }

        /// <summary>
        /// 编辑会员基础数据
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase MemberEditMemberBaseInfo(MembersBaseInfoModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                Us_SysUser dbuser = context.Us_SysUser.Single(p => p.Us_SysUserId == condtion.UserId);
                dbuser.MobilePhone = condtion.MobilePhone;
                dbuser.Email = condtion.Email;
                dbuser.FullName = condtion.FullName;
                dbuser.NickName = condtion.NickName;
                dbuser.Sex = condtion.Sex.GetHashCode();
                dbuser.IdType = condtion.IdType.GetHashCode();
                dbuser.IdCard = condtion.IdCard;
                context.SubmitChanges();
                return new ReplayBase() { };
            }
        }


        /// <summary>
        /// 会员检索
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static SearchMembersreplayModel MemberSearchMembersList(SearchMembersRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                SearchMembersreplayModel result = new SearchMembersreplayModel();

                string ordertype = "createdon desc";
                switch (condtion.ordertype) {
                    case EnumSortOrderType.按标题升序:
                        ordertype = "FullName asc";
                        break;
                    case EnumSortOrderType.按标题降序:
                        ordertype = "FullName desc";
                        break;
                }

                List<string> userId = new List<string>();
                if (!string.IsNullOrEmpty(condtion.orgId)) {
                    //如果有设定根部门，则获取该组织/部门下的所有用户的ID集合
                    userId = DepGetUserIdListBydepartment(condtion);
                }

                result.total = (from c in context.Us_SysUser
                                where (c.IsDeleted == false)
                                && (c.Us_SysRoleId == 1009)//角色为会员的用户
                                && (string.IsNullOrEmpty(condtion.keyWords) ? true : SqlMethods.Like(c.FullName, string.Format("%{0}%", condtion.keyWords)) || SqlMethods.Like(c.MobilePhone, string.Format("%{0}%", condtion.keyWords)) || SqlMethods.Like(c.NickName, string.Format("%{0}%", condtion.keyWords)) || SqlMethods.Like(c.LoginName, string.Format("%{0}%", condtion.keyWords)))
                                && (string.IsNullOrEmpty(condtion.userId) ? true : c.Us_SysUserId == condtion.userId)
                                && (string.IsNullOrEmpty(condtion.orgId) ? true : userId.Contains(c.Us_SysUserId))
                                select c.Us_SysUserId).Count();
                if (result.total > 0)
                {
                    result.rows = (from c in context.Us_SysUser
                                   where (c.IsDeleted == false)
                                   && (c.Us_SysRoleId == 1009)//角色为会员的用户
                                   && (string.IsNullOrEmpty(condtion.keyWords) ? true : SqlMethods.Like(c.FullName, string.Format("%{0}%", condtion.keyWords)) || SqlMethods.Like(c.MobilePhone, string.Format("%{0}%", condtion.keyWords)) || SqlMethods.Like(c.NickName, string.Format("%{0}%", condtion.keyWords)) || SqlMethods.Like(c.LoginName, string.Format("%{0}%", condtion.keyWords)))
                                   && (string.IsNullOrEmpty(condtion.userId) ? true : c.Us_SysUserId == condtion.userId)
                                   && (string.IsNullOrEmpty(condtion.orgId) ? true : userId.Contains(c.Us_SysUserId))//组织/部门限定
                                   select new MembersBaseInfoModel
                                   {
                                       createdon = c.CreatedOn,
                                       isDisabled = c.IsDisabled,
                                       Email = c.Email,
                                       FullName = c.FullName,
                                       IdCard = c.IdCard,
                                       IdType = (EnumUserIdType)(c.IdType == null ? 0 : c.IdType),
                                       LoginName = c.LoginName,
                                       MobilePhone = c.MobilePhone,
                                       NickName = c.NickName,
                                       Sex = (EnumSex)(c.Sex == null ? 0 : c.Sex),
                                       UserFace = c.UserFace,
                                       UserId = c.Us_SysUserId,
                                       userDepartmentList = (from d in c.Us_MemberDepartment
                                                             where (1 == 1)
                                                             && ((!string.IsNullOrEmpty(condtion.orgId) && !condtion.getOtherOrgDepartmentInfo) ? d.Us_DepDepartMentRootId == condtion.orgId : true)//获取用户的部门信息条件设置，当orgid不为空并且不获取其他组织的部门的时候，过滤掉其他组织的部门
                                                             orderby d.Us_DepDepartMentRootId ascending //按根部门排序
                                                             select new UserDepartmentInfoList
                                                             {
                                                                 path = d.path,
                                                                 pathText = d.pathText,
                                                                 orgId = d.Us_DepDepartMentRootId,
                                                                 departmentId = d.Us_DepDepartMentId,
                                                                 isCaption = d.isCaption
                                                             }
                                                             ).ToList(),
                                       baiduPushSet = SysSmsDataBaseManager.MemberGetMemberBaiduPushSetInfo(c),
                                       getuiPushSet = SysSmsDataBaseManager.MemberGetMemberGetuiPushSetInfo(c)
                                   }).OrderBy(ordertype).Skip((condtion.Page - 1) * condtion.PageSize).Take(condtion.PageSize).ToList();
                }
                else {
                    result.rows = new List<MembersBaseInfoModel>();
                }
                return result;
            }
        }
        /// <summary>
        /// 根据用户的Id,或者LoginName获取用户基础信息列表，不翻页
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static List<MembersBaseInfoModel> MemberGetMemberListByIdsOrLoginName(GetMembersListRequstModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                List<MembersBaseInfoModel> result = (from c in context.Us_SysUser
                                                     where (condtion.userIds.Contains(c.Us_SysUserId) || condtion.loginNames.Contains(c.LoginName))
                                                     && (c.Us_SysRoleId == 1009)//角色为会员的用户
                                                     && (c.IsDeleted == false)
                                                     orderby c.CreatedOn descending
                                                     select new MembersBaseInfoModel {
                                                         createdon = c.CreatedOn,
                                                         isDisabled = c.IsDisabled,
                                                         Email = c.Email,
                                                         FullName = c.FullName,
                                                         IdCard = c.IdCard,
                                                         IdType = (EnumUserIdType)(c.IdType == null ? 0 : c.IdType),
                                                         LoginName = c.LoginName,
                                                         MobilePhone = c.MobilePhone,
                                                         NickName = c.NickName,
                                                         Sex = (EnumSex)(c.Sex == null ? 0 : c.Sex),
                                                         UserFace = c.UserFace,
                                                         UserId = c.Us_SysUserId,
                                                         baiduPushSet = SysSmsDataBaseManager.MemberGetMemberBaiduPushSetInfo(c),
                                                         getuiPushSet = SysSmsDataBaseManager.MemberGetMemberGetuiPushSetInfo(c)
                                                     }).ToList();
                return result;
            }
        }
        /// <summary>
        /// 根据部门信息获取该部门/组织下所有用户Id
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static List<string> DepGetUserIdListBydepartment(SearchMembersRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                if (condtion.departmentIds == null) {
                    condtion.departmentIds = new List<string>();
                }
                List<string> userid = (from c in context.Us_SysUser
                                       join p in context.Us_MemberDepartment on c.Us_SysUserId equals p.Us_SysUserId
                                       where (c.IsDeleted == false)
                                       && (string.IsNullOrEmpty(condtion.orgId)? true : p.Us_DepDepartMentRootId==condtion.orgId)
                                       && (condtion.departmentIds.Count==0? true :condtion.departmentIds.Contains(p.Us_DepDepartMentId))
                                       group c by new { c.Us_SysUserId } into g
                                       select g.Key.Us_SysUserId
                                       ).ToList();
                return userid;
            }
        }

        #endregion

        #region-- 用户组织 Dep

        /// <summary>
        /// 检查组织名称在跟节点中出现的次数
        /// </summary>
        /// <param name="caption">部门名称</param>
        /// <param name="departmentid">如果是编辑状态时，可排除当前被编辑的部门ID</param>
        /// <returns></returns>
        internal static int DepCheckDepartmentCationNumInRootList(string caption, string departmentid)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                int result = (from c in context.Us_DepDepartMent
                              where caption == c.Caption
                              && (c.Pid == null || c.Pid == "")
                              && (string.IsNullOrEmpty(departmentid) ? true : c.Us_DepDepartMentId != departmentid)
                              select c.Us_DepDepartMentId).Count();
                return result;
            }
        }

        /// <summary>
        /// 新增一个组织/部门
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase DepCreateDepartment(CreateMembersDepartmentReqeustModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                string Newid = Guid.NewGuid().ToString();
                Us_DepDepartMent dbp = new Us_DepDepartMent() {
                    Caption = condtion.caption,
                    IsDeleted = false,
                    IsDisabled = false,
                    IsShow = false,
                    LinkUserId = condtion.createdby,
                    ModifiedBy = condtion.createdby,
                    ModifiedOn = DateTime.Now,
                    Pid = condtion.pid,
                    Us_DepDepartMentId = Newid
                };
                context.Us_DepDepartMent.InsertOnSubmit(dbp);
                context.SubmitChanges();
                return new ReplayBase() {
                    ReturnCode = EnumErrorCode.Success,
                    ReturnMessage = Newid
                };
            }
        }

        /// <summary>
        /// 编辑组织基础信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase DepEditOrgBaseInfo(EditOrgInfoRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                Us_DepDepartMent dbp = context.Us_DepDepartMent.Single(p => p.Us_DepDepartMentId == condtion.depmentid);
                dbp.Caption = condtion.caption;
                dbp.IsDisabled = condtion.isdisabled;
                dbp.IsShow = condtion.isshow;
                dbp.LinkUserId = condtion.linkUserId;
                context.SubmitChanges();
                return new ReplayBase()
                {
                    ReturnMessage = condtion.depmentid
                };
            }
        }

        /// <summary>
        /// 编辑部门基础信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase DepEditDepBaseInfo(EditDepInfoRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                Us_DepDepartMent dbm = context.Us_DepDepartMent.Single(p => p.Us_DepDepartMentId == condtion.depmentid);
                dbm.Caption = condtion.caption;
                dbm.IsDisabled = condtion.isdisabled;
                context.SubmitChanges();
                return new ReplayBase()
                {
                    ReturnMessage = condtion.depmentid
                };
            }
        }

        /// <summary>
        /// 标记删除组织/部门
        /// </summary>
        /// <param name="id">组织或部门的ID数组</param>
        /// <returns></returns>
        internal static ReplayBase DepDelDepartmentById(string[] id)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                ReplayBase result = new ReplayBase();
                var d = (from c in context.Us_DepDepartMent
                         where id.Contains(c.Us_DepDepartMentId)
                         select c).ToList();
                if (d.Count > 0)
                {
                    foreach (var l in d) {
                        l.IsDeleted = true;
                    }
                    context.SubmitChanges();
                    result.ReturnCode = EnumErrorCode.Success;
                    result.ReturnMessage = string.Format("删除成功，共有{0}条数据被删除", d.Count.ToString());
                }
                else {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = "参数错误，没有部门被删除";
                }
                return result;
            }
        }

        /// <summary>
        /// 检索部门
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static List<MemberDepartmentBaseInfoModel> DepSearchDepartments(SearchMemberDepartmentRequst condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                string orderby = "modifiedon desc";
                switch (condtion.ordertype) {
                    case EnumSortOrderType.按标题升序:
                        orderby = "caption asc";
                        break;
                    case EnumSortOrderType.按标题降序:
                        orderby = "caption desc";
                        break;
                }
                List<MemberDepartmentBaseInfoModel> result = (from c in context.Us_DepDepartMent
                                                              where (c.IsDeleted == false)
                                                              && (string.IsNullOrEmpty(condtion._parentId) ? (c.Pid == null || c.Pid == "") : (c.Pid == condtion._parentId))
                                                              && (string.IsNullOrEmpty(condtion.caption) ? true : SqlMethods.Like(c.Caption, string.Format("%{0}%", condtion.caption)))
                                                              && (string.IsNullOrEmpty(condtion.departmentId) ? true : c.Us_DepDepartMentId == condtion.departmentId)
                                                              && (condtion.showdisabled ? true : (c.IsDisabled == false))
                                                              && (condtion.showontheui ? (c.IsShow) : true)
                                                              select new MemberDepartmentBaseInfoModel
                                                              {
                                                                  caption = c.Caption,
                                                                  depmentid = c.Us_DepDepartMentId,
                                                                  isdisabled = c.IsDisabled,
                                                                  isshow = c.IsShow,
                                                                  linkuserid = c.LinkUserId,
                                                                  modifiedby = c.ModifiedBy,
                                                                  modifiedon = c.ModifiedOn,
                                                                  _parentId = c.Pid,
                                                                  memberNum = (from m in context.Us_SysUser
                                                                               join d in context.Us_MemberDepartment on m.Us_SysUserId equals d.Us_SysUserId
                                                                               where d.Us_DepDepartMentId == c.Us_DepDepartMentId
                                                                               && (m.IsDeleted == false)
                                                                               select m.Us_SysUserId).Count(),
                                                                  memberNumCount = (from m in context.Us_SysUser
                                                                                    join d in context.Us_MemberDepartment on m.Us_SysUserId equals d.Us_SysUserId
                                                                                    where (SqlMethods.Like(d.path, "%" + c.Us_DepDepartMentId + "%"))
                                                                                    && (m.IsDeleted == false)
                                                                                    group m by new { m.Us_SysUserId } into g
                                                                                    select g.Key.Us_SysUserId).Count()
                                                              }
                                                              ).OrderBy(orderby).ToList();
                return result;
            }
        }
        /// <summary>
        /// 检索根节点，可翻页用于后台列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static SearchMemberRootDepartMentReplay DepSearchRootDepartments(SearchMemberRootDepartmentRequest condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection))
            {
                string orderby = "modifiedon desc";
                switch (condtion.ordertype)
                {
                    case EnumSortOrderType.按标题升序:
                        orderby = "caption asc";
                        break;
                    case EnumSortOrderType.按标题降序:
                        orderby = "caption desc";
                        break;
                }
                SearchMemberRootDepartMentReplay result = new SearchMemberRootDepartMentReplay();
                result.total = (from c in context.Us_DepDepartMent
                                where (c.IsDeleted == false)
                                && (c.Pid == null || c.Pid == "")
                                && (string.IsNullOrEmpty(condtion.departmentId) ? true : c.Us_DepDepartMentId == condtion.departmentId)
                                && (string.IsNullOrEmpty(condtion.caption) ? true : SqlMethods.Like(c.Caption, string.Format("%{0}%", condtion.caption)))
                                && (condtion.showdisabled ? true : (c.IsDisabled == false))
                                && (condtion.showontheui ? (c.IsShow) : true)
                                select c.Us_DepDepartMentId).LongCount();
                if (result.total > 0)
                {
                    result.rows = (from c in context.Us_DepDepartMent
                                   where (c.IsDeleted == false)
                                   && (c.Pid == null || c.Pid == "")
                                   && (string.IsNullOrEmpty(condtion.departmentId) ? true : c.Us_DepDepartMentId == condtion.departmentId)
                                   && (string.IsNullOrEmpty(condtion.caption) ? true : SqlMethods.Like(c.Caption, string.Format("%{0}%", condtion.caption)))
                                   && (condtion.showdisabled ? true : (c.IsDisabled == false))
                                   && (condtion.showontheui ? (c.IsShow) : true)
                                   select new MemberDepartmentInfoModel
                                   {
                                       caption = c.Caption,
                                       depmentid = c.Us_DepDepartMentId,
                                       isdisabled = c.IsDisabled,
                                       isshow = c.IsShow,
                                       linkuserid = c.LinkUserId,
                                       modifiedby = c.ModifiedBy,
                                       modifiedon = c.ModifiedOn,
                                       _parentId = c.Pid,
                                       memberNumCount = (from m in context.Us_SysUser
                                                    join d in context.Us_MemberDepartment on m.Us_SysUserId equals d.Us_SysUserId
                                                    where d.Us_DepDepartMentRootId == c.Us_DepDepartMentId
                                                    && (m.IsDeleted == false)
                                                    group m by new { m.Us_SysUserId } into g
                                                    select g.Key.Us_SysUserId).Count()
                                   }).OrderBy(orderby).Skip((condtion.page.Page - 1) * condtion.page.PageSize).Take(condtion.page.PageSize).ToList();
                }
                else {
                    result.rows = new List<MemberDepartmentInfoModel>();
                }
                return result;
            }
        }

        /// <summary>
        /// 获取根节点基础信息
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        internal static MemberDepartmentInfoModel DepGetOrgBaseInfoById(string orgId)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                Us_DepDepartMent dbp = context.Us_DepDepartMent.Single(p => p.Us_DepDepartMentId == orgId);
                MemberDepartmentInfoModel result = new MemberDepartmentInfoModel() {
                    caption = dbp.Caption,
                    depmentid = dbp.Us_DepDepartMentId,
                    isdisabled = dbp.IsDisabled,
                    isshow = dbp.IsShow,
                    linkuserid = dbp.LinkUserId,
                    modifiedon = dbp.ModifiedOn,
                    modifiedby = dbp.ModifiedBy,
                    memberNumCount = (from m in context.Us_SysUser
                                      join d in context.Us_MemberDepartment on m.Us_SysUserId equals d.Us_SysUserId
                                      where d.Us_DepDepartMentRootId == dbp.Us_DepDepartMentId
                                      && (m.IsDeleted == false)
                                      group m by new { m.Us_SysUserId } into g
                                      select g.Key.Us_SysUserId).Count(),
                    linkUserinfo = SysGetSysUserById(dbp.LinkUserId)
                };
                return result;
            }
        }

        /// <summary>
        /// 获取部门详情
        /// </summary>
        /// <param name="depmentid">部门的ID</param>
        /// <returns>返回部门基础信息与所在组织的ID</returns>
        internal static DepInfoModel DepGetDepBaseInfoById(string depmentid)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                Us_DepDepartMent dbp = context.Us_DepDepartMent.Single(p => p.Us_DepDepartMentId == depmentid);
                DepInfoModel info = new DepInfoModel() {
                    //基础信息
                    baseinfo = new MemberDepartmentBaseInfoModel() {
                        caption = dbp.Caption,
                        depmentid = dbp.Us_DepDepartMentId,
                        isdisabled = dbp.IsDisabled,
                        modifiedby = dbp.ModifiedBy,
                        modifiedon = dbp.ModifiedOn,
                        _parentId = dbp.Pid,
                        memberNum = (from m in context.Us_SysUser
                                     join d in context.Us_MemberDepartment on m.Us_SysUserId equals d.Us_SysUserId
                                     where d.Us_DepDepartMentId == dbp.Us_DepDepartMentId
                                     && (m.IsDeleted == false)
                                     select m.Us_SysUserId).Count(),
                        memberNumCount = (from m in context.Us_SysUser
                                          join d in context.Us_MemberDepartment on m.Us_SysUserId equals d.Us_SysUserId
                                          where (SqlMethods.Like(d.path, "%" + dbp.Us_DepDepartMentId + "%"))
                                          && (m.IsDeleted == false)
                                          group m by new { m.Us_SysUserId } into g
                                          select g.Key.Us_SysUserId).Count()
                    }
                };
                //根节点信息
                DepartmentsrootParentInfoModel orginfo = DepGetRootDepartmentInfoBydepId(info.baseinfo.depmentid, true);
                info.orginfo = new MemberDepartmentInfoModel()
                {
                    depmentid = orginfo.orgId,
                };
                info.orginfo = DepGetOrgBaseInfoById(info.orginfo.depmentid);
                //子集
                info.children = DepSearchDepartments(new SearchMemberDepartmentRequst() {
                    _parentId = info.baseinfo.depmentid,
                    ordertype = EnumSortOrderType.按标题升序,
                    showdisabled = true,
                    showontheui = false
                });
                return info;
            }
        }

        /// <summary>
        /// 设置用户的部门
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase DepSetMemberDepartments(SetMemberDepartmentsRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                Us_SysUser dbuser = context.Us_SysUser.Single(p => p.Us_SysUserId == condtion.userId);
                var del = (from c in dbuser.Us_MemberDepartment
                           where (c.Us_SysUserId == condtion.userId)
                           && (string.IsNullOrEmpty(condtion.rootDepartmentId) ? true : c.Us_DepDepartMentRootId == condtion.rootDepartmentId)
                           select c
                           ).ToList();
                //删除用户原有的部门
                if (del.Count > 0) {
                    context.Us_MemberDepartment.DeleteAllOnSubmit(del);
                    context.SubmitChanges();
                }
                //插入新增的部门
                if (condtion.departments != null) {
                    if (condtion.departments.Count > 0) {
                        List<Us_MemberDepartment> newdepartments = new List<Us_MemberDepartment>();
                        foreach (var d in condtion.departments)
                        {
                            depRootdepartmentinfo = new DepartmentsrootParentInfoModel();
                            DepartmentsrootParentInfoModel rootDepartmentsInfo = DepGetRootDepartmentInfoBydepId(d.departmentId);//获取根节点
                            Us_MemberDepartment dbd = new Us_MemberDepartment()
                            {
                                createdOn = DateTime.Now,
                                isCaption = d.isCaption,
                                Us_DepDepartMentRootId = rootDepartmentsInfo.orgId,
                                path = rootDepartmentsInfo.path,
                                pathText = rootDepartmentsInfo.pathText,
                                Us_DepDepartMentId = d.departmentId,
                                Us_SysUserId = condtion.userId
                            };
                            newdepartments.Add(dbd);
                        }
                        context.Us_MemberDepartment.InsertAllOnSubmit(newdepartments);
                        context.SubmitChanges();
                    }
                }
                return new ReplayBase() { };
            }
        }

        /// <summary>
        /// 增加部门的用户
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase DepSetDepartmentMembers(SetDepartmentMembersRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {

                var rows = (from g in (from c in condtion.users
                                       select new {
                                           userid = c.userId,
                                           isCaption = c.isCaption,
                                           deplist = (from md in context.Us_MemberDepartment where md.Us_SysUserId == c.userId select md.Us_DepDepartMentId)
                                       })
                            where (!g.deplist.Contains(condtion.departmentId))
                            select new {
                                createdOn = DateTime.Now,
                                isCaption = g.isCaption,
                                Us_DepDepartMentId = condtion.departmentId,
                                Us_SysUserId = g.userid,
                                depRootdepartmentinfo = DepGetRootDepartmentInfoBydepId(condtion.departmentId, true),
                            }).ToList();
                List<Us_MemberDepartment> newrows = new List<Us_MemberDepartment>();
                if (rows.Count > 0) {
                    foreach (var n in rows) {
                        newrows.Add(new Us_MemberDepartment() {
                            createdOn = DateTime.Now,
                            isCaption = n.isCaption,
                            path = n.depRootdepartmentinfo.path,
                            pathText = n.depRootdepartmentinfo.pathText,
                            Us_DepDepartMentId = n.Us_DepDepartMentId,
                            Us_DepDepartMentRootId = n.depRootdepartmentinfo.orgId,
                            Us_SysUserId = n.Us_SysUserId
                        });
                    }
                    context.Us_MemberDepartment.InsertAllOnSubmit(newrows);
                    context.SubmitChanges();
                }
                return new ReplayBase()
                {
                    ReturnCode = (rows.Count > 0) ? EnumErrorCode.Success : EnumErrorCode.EmptyDate,
                    ReturnMessage = (rows.Count > 0) ? string.Format("成功设置{0}个用户", rows.Count.ToString()) : "操作失败，您可以选择了重复的用户"
                };
            }
        }


        /// <summary>
        /// 编辑用户在部门中的信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase DepEditDepartmentMembers(SetDepartmentMembersRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                foreach (var user in condtion.users) {
                    Us_MemberDepartment dbm = context.Us_MemberDepartment.Single(p => p.Us_DepDepartMentId == condtion.departmentId && p.Us_SysUserId == user.userId);
                    dbm.isCaption = user.isCaption;
                    context.SubmitChanges();
                }
                return new ReplayBase() { };
            }
        }

        /// <summary>
        /// 移除部门的用户
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        internal static ReplayBase DepDelDepartmentMembers(SetDepartmentMembersRequestModel condtion)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                ReplayBase result = new ReplayBase();
                List<string> deluserlist = new List<string>();
                foreach (DepartmentMembersSetRequestInfoModel user in condtion.users) {
                    deluserlist.Add(user.userId);
                }
                List<Us_MemberDepartment> dellist = (from c in context.Us_MemberDepartment
                                                     where c.Us_DepDepartMentId == condtion.departmentId
                                                     && deluserlist.Contains(c.Us_SysUserId)
                                                     select c).ToList();
                if (dellist.Count > 0)
                {
                    context.Us_MemberDepartment.DeleteAllOnSubmit(dellist);
                    context.SubmitChanges();
                    result.ReturnMessage = string.Format("成功移除{0}个用户", dellist.Count().ToString());
                }
                else {
                    result.ReturnCode = EnumErrorCode.EmptyDate;
                    result.ReturnMessage = "没有用户被移除";
                }
                return result;
            }
        }

        /// <summary>
        /// 根部门节点的信息
        /// </summary>
        protected static DepartmentsrootParentInfoModel depRootdepartmentinfo = new DepartmentsrootParentInfoModel();

        /// <summary>
        /// 批量更新用户所在部门路径信息
        /// </summary>
        /// <param name="depmentids">部门的ID</param>
        internal static void DepEditUserDepmentPath(List<string> depmentids)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {

                var r = (from c in context.Us_MemberDepartment
                         where depmentids.Contains(c.Us_DepDepartMentId)
                         select c
                         ).ToList();
                if (r.Count > 0) {
                    foreach (var m in r) {
                        DepartmentsrootParentInfoModel info = DepGetRootDepartmentInfoBydepId(m.Us_DepDepartMentId, true);
                        m.path = info.path;
                        m.pathText = info.pathText;
                    }
                    context.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// 根据部门的ID获取根节点的信息与当前部门的路径
        /// </summary>
        /// <param name="departmentId">部门ID</param>
        /// <param name="clearresuts">是否清空递归参数，默认为false</param>
        /// <returns></returns>
        private static DepartmentsrootParentInfoModel DepGetRootDepartmentInfoBydepId(string departmentId,bool clearresuts =false)
        {
            using (SysTemDataBaseDataContext context = new SysTemDataBaseDataContext(SqlConnection)) {
                if (clearresuts) {
                    depRootdepartmentinfo = new DepartmentsrootParentInfoModel();
                }
                Us_DepDepartMent dbd = context.Us_DepDepartMent.SingleOrDefault(p => p.Us_DepDepartMentId == departmentId);
                if (dbd != null)
                {
                    depRootdepartmentinfo.orgId = dbd.Us_DepDepartMentId;
                    depRootdepartmentinfo.path = "/" + dbd.Us_DepDepartMentId + depRootdepartmentinfo.path;
                    depRootdepartmentinfo.pathText = "/" + dbd.Caption + depRootdepartmentinfo.pathText;
                    if (!string.IsNullOrEmpty(dbd.Pid))
                    {
                        DepGetRootDepartmentInfoBydepId(dbd.Pid);
                    }
                }
                return depRootdepartmentinfo;
            }
        }

        #endregion
    }
}