using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cyclonestyle.Models
{

    #region -- 用户

    /// <summary>
    /// 系统用户信息
    /// </summary>
    public class SysUserModel {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string nickName { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public string JobTitle { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string MobilePhone { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// 角色信息
        /// </summary>
        public SysUserRoleListModel SysRole { get; set; }

        protected bool _isdisabled = false;
        /// <summary>
        /// 是否禁用(默认为FALSE)
        /// </summary>
        public bool IsDisabled { get { return _isdisabled; } set { _isdisabled = value; } }
    }

    /// <summary>
    /// 查询系统用户集请求对象
    /// </summary>
    public class RequestSysUserListModel : RequestBase
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWords { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public List<long> RoleId { get; set; }
    }
    

    /// <summary>
    /// 查询系统用户集返回对象
    /// </summary>
    public class ReplaySysUserListModel {
        /// <summary>
        /// 总数
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 列表
        /// </summary>
        public List<SysUserModel> UserList { get; set; }
    }

    #endregion

    #region - 用户日志

    /// <summary>
    /// 系统用户日志事件
    /// </summary>
    public class SysUserLogModel
    {
        /// <summary>
        /// 事件ID
        /// </summary>
        public long LogId { get; set; }
        /// <summary>
        /// 对应用户ID（新增时必填）
        /// </summary>
        public string SysUserId { get; set; }
        /// <summary>
        /// 对应用户信息
        /// </summary>
        public SysUserModel SysUserInfo { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// 事件描述（必填）
        /// </summary>
        public string Describe { get; set; }
        /// <summary>
        /// 外键字段（可能是订单ID，或其他）
        /// </summary>
        public string FkId { get; set; }
    }

    /// <summary>
    /// 查询系统用户日期请求
    /// </summary>
    public class RequestSearchSysUserLog : RequestBase
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWords { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string SysUserId { get; set; }

        /// <summary>
        /// 开始时间--可为空
        /// </summary>
        public Nullable<DateTime> BeginDate { get; set; }
        /// <summary>
        /// 结束时间--可为空
        /// </summary>
        public Nullable<DateTime> EndDate { get; set; }
    }
    /// <summary>
    /// 查询系统用户日志返回对象
    /// </summary>
    public class ReplaySearchSysUserLog
    {
        /// <summary>
        /// 总数
        /// </summary>
        public long total { get; set; }
        /// <summary>
        /// 日志列表
        /// </summary>
        public List<SysUserLogModel> rows { get; set; }
    }

    #endregion

    #region -- 角色
    /// <summary>
    /// 后台系统角色
    /// </summary>
    public class SysUserRoleModel : SysUserRoleListModel
    {
        
        /// <summary>
        /// 角色描述
        /// </summary>
        public string RoleDescribe { get; set; }
    }

    /// <summary>
    /// 后台角色列表模型
    /// </summary>
    public class SysUserRoleListModel {
        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleId { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
    }

    /// <summary>
    /// 查询后台系统角色返回对象
    /// </summary>
    public class ReplaySysUserRoleListModel : ReplayBase {
        /// <summary>
        /// 记录总数
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 列表
        /// </summary>
        public List<SysUserRoleModel> RowList { get; set; }
    }

    #endregion

    #region -- 菜单
    /// <summary>
    /// 系统菜单对象
    /// </summary>
    public class SysMenuModel {
        /// <summary>
        /// 菜单ID
        /// </summary>
        public long MenuId { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public Nullable<int> Sort { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public Nullable<long> _parentId { get; set; }
        /// <summary>
        /// 指向路径
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public string Roles { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 创建/修改时间
        /// </summary>
        public DateTime ModifiedOn { get; set; }
    }

    /// <summary>
    /// 调整系统菜单对象父节点ID请求
    /// </summary>
    public class RequestChangeSysMenuPidModel {
        /// <summary>
        /// 节点ID
        /// </summary>
        public long MenuId { get; set; }
        /// <summary>
        /// 父节点ID，可为空
        /// </summary>
        public Nullable<long> ParentId { get; set; }
    }

    /// <summary>
    /// 查询系统菜单请求
    /// </summary>
    public class RequestSearchSysMenuModel {
        /// <summary>
        /// 角色ID
        /// </summary>
        public Nullable<long> RoleId { get; set; }
        /// <summary>
        /// 查询深度-为空则不限制深度
        /// </summary>
        public Nullable<int> Dep { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public Nullable<long> Pid { get; set; }
    }

    /// <summary>
    /// 系统菜单树
    /// </summary>
    public class SysMenuTreeModel {
        /// <summary>
        /// ID
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 显示的文本
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string iconCls { get; set; }
        /// <summary>
        /// 属性
        /// </summary>
        public SysMenuTreeAttrModel attributes { get; set; }
        /// <summary>
        /// 子节点
        /// </summary>
        public List<SysMenuTreeModel> children { get; set; }
    }
    /// <summary>
    /// 系统菜单树属性节点
    /// </summary>
    public class SysMenuTreeAttrModel{
        /// <summary>
        /// 对应的URL
        /// </summary>
        public string Url { get; set; }
    }

    #endregion

    #region -- 系统更新日志

    /// <summary>
    /// 新增系统更新日志请求
    /// </summary>
    public class EditSysUpdateLogRequestModel {

        /// <summary>
        /// 创建用户ID
        /// </summary>
        internal string SysUserId { get; set; }

        /// <summary>
        /// 日志ID(新增时填0)
        /// </summary>
        public long LogId { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        public EnumUpdateLogType LogType { get; set; }

        /// <summary>
        /// 日志描述
        /// </summary>
        public string LogDescribe { get; set; }
    }

    /// <summary>
    /// 系统更新日志记录
    /// </summary>
    public class SysUpdateLogModel : EditSysUpdateLogRequestModel
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string CreatedBy { get; set; }
    }

    /// <summary>
    /// 系统更新日志查询请求
    /// </summary>
    public class SearchSysUpdateLogRequstModel : RequestBase {
        /// <summary>
        /// 更新类型
        /// </summary>
        public Nullable<EnumUpdateLogType> LogType { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWords { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public Nullable<DateTime> BeginDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public Nullable<DateTime> EndDate { get; set; }
    }
    /// <summary>
    /// 查询系统更新日志返回对象
    /// </summary>
    public class SearchSysUpdateLogReplayModel {
        /// <summary>
        /// 总数
        /// </summary>
        public long total { get; set; }
        /// <summary>
        /// 日志列表
        /// </summary>
        public List<SysUpdateLogModel> rowslist { get; set; }
    }

    /// <summary>
    /// 系统更新日志类型
    /// </summary>
    public enum EnumUpdateLogType { 
        /// <summary>
        /// 服务器
        /// </summary>
        Svr,
        /// <summary>
        /// 安卓设备
        /// </summary>
        Android,
        /// <summary>
        /// IOS设备
        /// </summary>
        IOS
    }

    #endregion

    #region -- 系统错误日志


    /// <summary>
    /// 系统日志类型
    /// </summary>
    public enum EnumSysLogType
    {
        /// <summary>
        /// 系统在运行时遇到的错误
        /// </summary>
        错误,
        /// <summary>
        /// 一些需要记录的警告
        /// </summary>
        警告,
        /// <summary>
        /// 来自远程服务器的通知（支付宝，机票出票通知等）
        /// </summary>
        通知
    }

    /// <summary>
    /// 查询系统日志请求
    /// </summary>
    public class GetSysLogRequestModel : RequestBase
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public Nullable<DateTime> BeginDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public Nullable<DateTime> EndDate { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        public Nullable<EnumSysLogType> LogType { get; set; }
    }

    /// <summary>
    /// 查询系统运行日志返回
    /// </summary>
    public class GetSysErrorLogReplayModel {
        /// <summary>
        /// 总数
        /// </summary>
        public long total { get; set; }
        /// <summary>
        /// 列表行
        /// </summary>
        public List<SysErrorLogModel> rows { get; set; }
    }

    /// <summary>
    /// 删除系统运行日志请求
    /// </summary>
    public class DelSysLogRequestModel {
        /// <summary>
        /// 列表行
        /// </summary>
        public List<SysErrorLogModel> rows { get; set; }
    }

    /// <summary>
    /// 系统运行日志
    /// </summary>
    public class SysErrorLogModel {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 日志类型
        /// </summary>
        public EnumSysLogType LogType { get; set; }

        /// <summary>
        /// 日志类型（文本）
        /// </summary>
        public string LogTypeText { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// 请求参数
        /// </summary>
        public string Condtion { get; set; }
        /// <summary>
        /// 错误内容
        /// </summary>
        public string Errormsg { get; set; }
    }

    #endregion
}