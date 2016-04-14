using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cyclonestyle.Models
{
    /// <summary>
    /// 会员用户登录请求
    /// </summary>
    public class RequestLogOnMembersModel {
        /// <summary>
        /// 设备信息
        /// </summary>
        public string DeviceInfo { get; set; }

        /// <summary>
        /// 设备ID（与下方手机号码 二选一必填）
        /// </summary>
        public string deviceUUid { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string mobilePhone { get; set; }

        private string _pwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(System.Configuration.ConfigurationManager.AppSettings["DefaultSysUserPassWord"].ToString(), "MD5").ToLower();

        /// <summary>
        /// 密码（默认为系统初始密码）
        /// </summary>
        public string UserPassWord
        {
            get { return _pwd; }
            set { _pwd = value; }
        }
    }

    /// <summary>
    /// 会员用户注册请求
    /// </summary>
    public class RegisterMembersRequestModel: RequestLogOnMembersModel
    {
        /// <summary>
        /// 注册时的IP地址
        /// </summary>
        internal string IpAddress { get; set; }

        /// <summary>
        /// 注册渠道
        /// </summary>
        public string RegisterWay { get; set; }

        /// <summary>
        /// 电子邮件地址,用于找回密码
        /// </summary>
        public string eMail { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        internal long roleId { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        internal string loginName { get; set; }
    }

    /// <summary>
    /// 会员用户注册返回对象
    /// </summary>
    public class RegisterMembersReplayModel : ReplayBase {
        /// <summary>
        /// 注册成功后返回用户基本信息
        /// </summary>
        public MembersBaseInfoModel UserInfo { get; set; }
    }

    /// <summary>
    /// 获取用户详情请求参数
    /// </summary>
    public class GetMembersInfoRequestModel {
        /// <summary>
        /// 用户ID（必填）
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// 组织ID（后台非必填，前台必填）
        /// </summary>
        public string orgId { get; set; }
    }
    /// <summary>
    /// 检索会员请求
    /// </summary>
    public class SearchMembersRequestModel : RequestBase {
        /// <summary>
        /// 默认按时间降序排序
        /// </summary>
        protected EnumSortOrderType _ordertype = EnumSortOrderType.按时间降序;
        /// <summary>
        /// 排序方式
        /// </summary>
        public EnumSortOrderType ordertype {
            get { return _ordertype; }
            set { _ordertype = value; }
        }
        /// <summary>
        /// 登录名,姓名,电话,昵称
        /// </summary>
        public string keyWords { get; set; }

        private List<long> _roleids = new List<long>() { 1009, 1008 };

        /// <summary>
        /// 角色选择
        /// </summary>
        public List<long> roleids {
            get { return _roleids; }
            set { _roleids = value; }
        }

        /// <summary>
        /// 用户的ID
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// 用户根组织限定设置
        /// </summary>
        public string orgId { get; set; }
        /// <summary>
        /// 是否获取该用户在其他组织的信息 默认为false
        /// </summary>
        protected bool _getOtherOrgDepartmentInfo = false;

        /// <summary>
        /// 是否获取该用户在其他组织的信息，若要此值生效orgId必传，默认为false，用户端请设置为false，后台请设置为true，
        /// 此属性涉及用户所属组织的相关返回值
        /// </summary>
        public bool getOtherOrgDepartmentInfo {
            get { return _getOtherOrgDepartmentInfo; }
            set { _getOtherOrgDepartmentInfo = value; }
        }
        /// <summary>
        /// 部门限定
        /// </summary>
        public List<string> departmentIds { get; set; }
    }

    /// <summary>
    /// 获取会员列表请求（不翻页可按多个用户ID、登录名、部门ID获取用户信息列表）
    /// </summary>
    public class GetMembersListRequstModel {
        private List<string> _userIds = new List<string>();
        private List<string> _loginNames = new List<string>();
        private List<string> _depIds = new List<string>();
        /// <summary>
        /// 用户ID集合
        /// </summary>
        public List<string> userIds {
            get { return _userIds; }
            set { _userIds = value; }
        }
        /// <summary>
        /// 登录名集合
        /// </summary>
        public List<string> loginNames {
            get { return _loginNames; }
            set { _loginNames = value; }
        }
        /// <summary>
        /// 部门ID集合
        /// </summary>
        public List<string> depIds {
            get { return _depIds; }
            set { _depIds = value; }
        }
        /// <summary>
        /// 是否获子部门的用户
        /// </summary>
        public bool getChilds { get; set; }
    }

    /// <summary>
    /// 检索会员返回对象
    /// </summary>
    public class SearchMembersreplayModel {
        /// <summary>
        /// 总数
        /// </summary>
        public long total { get; set; }
        /// <summary>
        /// 翻页
        /// </summary>
        public List<MembersBaseInfoModel> rows { get; set; }
    }

    /// <summary>
    /// 会员用户基本信息
    /// </summary>
    public class MembersBaseInfoModel {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 创建/注册时间
        /// </summary>
        public DateTime createdon { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool isDisabled { get; set; }

        /// <summary>
        /// 用户所在部门信息
        /// </summary>
        public List<UserDepartmentInfoList> userDepartmentList { get; set; }

        /// <summary>
        /// 用户所属公司/部门的数量信息
        /// </summary>
        public MemberNumOfDepModel numOfDep {
            get;set;
        }
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// 注册时的设备ID
        /// </summary>
        public string uuId { get; set; }

        /// <summary>
        /// 所属角色
        /// </summary>
        public string role { get; set; }

        /// <summary>
        /// Email地址
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public EnumSex Sex { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        protected EnumUserIdType _idtype = EnumUserIdType.IdCard;

        /// <summary>
        /// 证件类型，默认为身份证
        /// </summary>
        public EnumUserIdType IdType {
            get { return _idtype; }
            set { _idtype = value; }
        }

        /// <summary>
        /// 证件号
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        
        /// <summary>
        /// 头像地址
        /// </summary>
        public string UserFace { get; set; }
        /// <summary>
        /// 【留作备用，推送系统使用个推平台】百度推送设置，如果用户没有该信息则返回null值
        /// </summary>
        internal BaiduPushSetModel baiduPushSet { get; set; }
        /// <summary>
        /// 用户的个推推送设置，如果用户没有该信息则返回null值
        /// </summary>
        public GeTuiSetModel getuiPushSet { get; set; }
    }

    /// <summary>
    /// 用户完整信息
    /// </summary>
    public class MembersInfoModel {
        /// <summary>
        /// 用户基础信息
        /// </summary>
        public MembersBaseInfoModel baseInfo { get; set; }
        /// <summary>
        /// 扩展信息列表
        /// </summary>
        public List<InfoExInfoModel> exList { get; set; }
    }
    

    /// <summary>
    /// 用户所属公司/组织的数量
    /// </summary>
    public class MemberNumOfDepModel {
        /// <summary>
        /// 所属公司的数量（如果该值大于0，则表示该用户为企业用户）
        /// </summary>
        public int numofcompany { get; set; }
        /// <summary>
        /// 所在部门的数量
        /// </summary>
        public int numofdepartments { get; set; }
    }
}