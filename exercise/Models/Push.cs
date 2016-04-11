using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cyclonestyle.Models
{

    /// <summary>
    /// 推送消息接收设备类型
    /// </summary>
    public enum EnumUserDeviceType
    {
        /// <summary>
        /// 安卓手机
        /// </summary>
        Android,
        /// <summary>
        /// IOS正式版本
        /// </summary>
        IOSProduction,
        /// <summary>
        /// IOS测试版本
        /// </summary>
        IOSDeveloper
    }

    /// <summary>
    /// 推送类型
    /// </summary>
    public enum EnumPushMessagesType
    {
        /// <summary>
        /// 手机通知栏的通知
        /// </summary>
        通知,
        /// <summary>
        /// 发送一个通知到客户端手机，对用户完全透明，无振动无响铃 界面无任何变化
        /// </summary>
        透传消息
    }

    #region -- 个推
    /// <summary>
    /// 个推用户设置
    /// </summary>
    public class GeTuiSetModel {
        /// <summary>
        /// 安卓设备为 SDK中的 clientId ， IOS设备为 SDK中的 DeviceToken
        /// </summary>
        public string clientId { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        public EnumUserDeviceType deviceType { get; set; }
        /// <summary>
        /// 用户的ID
        /// </summary>
        internal string userId { get; set; }
    }

    /// <summary>
    /// 发送个推请求[基于个推的设置]
    /// </summary>
    public class SendGeTuiPushBySetRequestModel : SendGeTuiPushBaseRequestModel {
        private List<GeTuiSetModel> _pushSets = new List<GeTuiSetModel>();
        /// <summary>
        /// 接收者信息
        /// </summary>
        public List<GeTuiSetModel> pushSets {
            get { return _pushSets; }
            set { _pushSets = value; }
        }
    }
    /// <summary>
    /// 发送个推请求[基于用户列表]
    /// </summary>
    public class SendGeTuiPushByUsersIdRequestModel : SendGeTuiPushBaseRequestModel {
        private List<string> _userIds = new List<string>();
        /// <summary>
        /// 接收用户列表
        /// </summary>
        public List<string> userIds {
            get { return _userIds; }
            set { _userIds = value; }
        }
    }

    /// <summary>
    /// 发送个推请求参数
    /// </summary>
    public class SendGeTuiPushBaseRequestModel {
        private List<GeTuiSetModel> _sets = new List<GeTuiSetModel>();
        /// <summary>
        /// 接收者信息
        /// </summary>
        internal List<GeTuiSetModel> sets {
            get { return _sets; }
            set { _sets = value; }
        }
        /// <summary>
        /// 消息类型
        /// </summary>
        public EnumPushMessagesType messageType { get; set; }

        private EnumPushCustomInfo _custominfo = new EnumPushCustomInfo();
        /// <summary>
        /// 自定义的内容
        /// </summary>
        public EnumPushCustomInfo customInfo {
            get { return _custominfo; }
            set { _custominfo = value; }
        }

        private string _createdBy = System.Configuration.ConfigurationManager.AppSettings["AdminId"].ToString();
        /// <summary>
        /// 发送者，默认为系统管理员
        /// </summary>
        internal string createdBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }

        /// <summary>
        /// 默认从配置文件读取项目标题
        /// </summary>
        private string _title = System.Configuration.ConfigurationManager.AppSettings["ProjectName"].ToString();
        /// <summary>
        /// 推送的标题[推荐设置，默认为配置文件中的项目标题，U上商侣]
        /// </summary>
        public string title {
            get { return _title; }
            set { _title = value; }
        }
        /// <summary>
        /// 详细内容[必填]
        /// </summary>
        public string msg { get; set; }
    }

    /// <summary>
    /// 发送个推返回参数
    /// </summary>
    public class SendGeTuiPushReplay : ReplayBase {
        /// <summary>
        /// 发送推送远程返回字串[安卓]
        /// </summary>
        public string sentResultAndroid { get; set; }
        /// <summary>
        /// 发送推送远程返回字串[IOS]
        /// </summary>
        public string sentResultIos { get; set; }
    }

    #endregion

    #region -- 推送历史
    /// <summary>
    /// 推送历史查询请求参数
    /// </summary>
    public class SearchSentPushListRequestModel : RequestBase
    {
        /// <summary>
        /// 查询开始时间
        /// </summary>
        public Nullable<DateTime> beginDate { get; set; }
        /// <summary>
        /// 查询结束时间
        /// </summary>
        public Nullable<DateTime> endDate { get; set; }
        /// <summary>
        /// 标题/内容
        /// </summary>
        public string keyWords { get; set; }
    }
    /// <summary>
    /// 推送历史查询返回对象
    /// </summary>
    public class SearchSentPushListReplayModel {
        /// <summary>
        /// 总数
        /// </summary>
        public int total { get; set; }
        private List<SentPushInfoModel> _rows = new List<SentPushInfoModel>();
        /// <summary>
        /// 行
        /// </summary>
        public List<SentPushInfoModel> rows {
            get { return _rows; }
            set { _rows = value; }
        }
    }

    /// <summary>
    /// 历史推送信息
    /// </summary>
    public class SentPushInfoModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createdOn { get; set; }
        /// <summary>
        /// 创建者信息
        /// </summary>
        public SysUserModel createdBy { get; set; }
        /// <summary>
        /// 信息类型
        /// </summary>
        public EnumPushMessagesType messageType { get; set; }
        /// <summary>
        /// 消息标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 推送中自定义的内容
        /// </summary>
        public string customInfo { get; set; }
        /// <summary>
        /// 接收者用户的数量
        /// </summary>
        public int sentUserNum { get; set; }
        /// <summary>
        /// 发送状态
        /// </summary>
        public bool sentStats { get; set; }
        /// <summary>
        /// 远程接口返回结果字串[安卓]
        /// </summary>
        public string sentResultAndroid { get; set; }
        /// <summary>
        /// 远程接口返回结果字串[IOS]
        /// </summary>
        public string sentResultIos { get; set; }
        /// <summary>
        /// 接收者详情
        /// </summary>
        public List<SentPushInfoDetailInfoModel> sentUserList { get; set; }
    }
    /// <summary>
    /// 历史推送推送接收者详情
    /// </summary>
    public class SentPushInfoDetailInfoModel
    {
        /// <summary>
        /// 接收者信息，如果采用的是根据设备信息进行的推送那么该值可能为空
        /// </summary>
        public SysUserModel sentUserInfo { get; set; }
        /// <summary>
        /// 接收设备的推送参数设置
        /// </summary>
        public GeTuiSetModel sentDeviceInfo { get; set; }
    }
    #endregion

    #region -- 百度推送
    /// <summary>
    /// 百度推送设置
    /// </summary>
    public class BaiduPushSetModel
    {
        /// <summary>
        /// 用户ID-来自百度推送SDK
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// 频道ID-来自百度推送SDK
        /// </summary>
        public string channelId { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public EnumUserDeviceType deviceType { get; set; }
    }


    /// <summary>
    /// 基于百度推送设置的发送推送请求模型
    /// </summary>
    public class SendBaiduPushbySetsRequstModel : SendBaiduPushBaseRequestModel {
        private List<BaiduPushSetModel> _sets = new List<BaiduPushSetModel>();
        /// <summary>
        /// 百度推送设置【必填，可多个】
        /// </summary>
        public List<BaiduPushSetModel> sets {
            get { return _sets; }
            set { _sets = value; }
        }
    }
    /// <summary>
    /// 基于用户来发送推送请求模型
    /// </summary>
    public class SendBaiduPushByUsersRequestModle : SendBaiduPushBaseRequestModel {
        private List<string> _userIds = new List<string>();
        /// <summary>
        /// 接收者的用户ID【必填，可多个】
        /// </summary>
        public List<string> userIds {
            get { return _userIds; }
            set { _userIds = value; }
        }
    }
    /// <summary>
    /// 发送基础对象推送
    /// </summary>
    public class SendBaiduPushBaseRequestModel {
        /// <summary>
        /// 推送接收者的设置
        /// </summary>
        internal BaiduPushSetModel set { get; set; }

        private string _createdBy = System.Configuration.ConfigurationManager.AppSettings["AdminId"].ToString();
        /// <summary>
        /// 发送者，默认为系统管理员
        /// </summary>
        internal string createdBy {
            get { return _createdBy; }
            set { _createdBy = value; }
        }
        
        /// <summary>
        /// 默认从配置文件读取项目标题
        /// </summary>
        private string _title = System.Configuration.ConfigurationManager.AppSettings["ProjectName"].ToString();
        /// <summary>
        /// 推送的标题[推荐设置，默认为配置文件中的项目标题，U上商侣]
        /// </summary>
        public string title {
            get { return _title; }
            set { _title = value; }
        }
        /// <summary>
        /// 详细内容[必填]
        /// </summary>
        public string msg { get; set; }

        private EnumPushMessagesType _messageClass = EnumPushMessagesType.通知;
        /// <summary>
        /// 推送类型(默认为通知)
        /// </summary>
        public EnumPushMessagesType messageType
        {
            get { return _messageClass; }
            set { _messageClass = value; }
        }
        private int _messagestype = 5;
        /// <summary>
        /// 通知样式（0 无声不能清除，1 无声可以清除 4有声音可清除 5 有声音可清除） 默认为5
        /// 【当推送类型为通知时有效】
        /// </summary>
        public int messagesStyle
        {
            get { return _messagestype; }
            set { _messagestype = value; }
        }
        private EnumPushCustomInfo _customInfo = new EnumPushCustomInfo();
        /// <summary>
        /// 自定义内容
        /// </summary>
        public EnumPushCustomInfo customInfo
        {
            get { return _customInfo; }
            set { _customInfo = value; }
        }
    }

    
    /// <summary>
    /// 百度推送自定义信息
    /// </summary>
    public class EnumPushCustomInfo
    {
        /// <summary>
        /// 对应打开的模块
        /// </summary>
        public EnumPushCustomOpenType openType { get; set; }
        /// <summary>
        /// 对应的详情信息ID
        /// </summary>
        public string id { get; set; }
    }
    /// <summary>
    /// 百度推送自定义的打开方式
    /// </summary>
    public enum EnumPushCustomOpenType {
        /// <summary>
        /// Default
        /// </summary>
        默认打开应用程序
    }
    #endregion
}