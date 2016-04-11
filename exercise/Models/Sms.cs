using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cyclonestyle.Models
{
    /// <summary>
    /// 发送短信基础请求
    /// </summary>
    public class SendSmsBaseRequestModel
    {
        private string _createdBy = System.Configuration.ConfigurationManager.AppSettings["AdminId"].ToString();
        /// <summary>
        /// 发送者[可为空，为空则默认为管理员]
        /// </summary>
        internal string createdBy {
            get { return _createdBy; }
            set { _createdBy = value; }
        }
        /// <summary>
        /// 手机号码，必填(支持10000个手机号,建议少于5000)多个英文逗号隔开
        /// </summary>
        public string mobilePhone { get; set; }
        /// <summary>
        /// 短信内容，必填（最多200个字符，汉子或字母数字 加起来不超过200个）
        /// </summary>
        public string content { get; set; }
    }
    /// <summary>
    /// 检索历史发送短信请求
    /// </summary>
    public class SearchSmsInfoListRequestModel :RequestBase{
        /// <summary>
        /// 接收的手机号码，模糊匹配
        /// </summary>
        public string mobilePhones { get; set; }
        /// <summary>
        /// 短信内容，模糊匹配
        /// </summary>
        public string contentMsg { get; set; }
    }
    /// <summary>
    /// 检索历史发送短信返回对象
    /// </summary>
    public class SearchSmsInfoListReplayModel {
        /// <summary>
        /// 记录的总数，用于翻页
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 行列表
        /// </summary>
        public List<SentSmsInfoModel> rows { get; set; }
    }
    /// <summary>
    /// 已发送的短信历史信息
    /// </summary>
    public class SentSmsInfoModel {
        /// <summary>
        /// 记录的ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime createdOn { get; set; }
        /// <summary>
        /// 发送者
        /// </summary>
        public SysUserModel createdBy { get; set; }
        /// <summary>
        /// 接收的手机号码
        /// </summary>
        public string mobilePhones { get; set; }
        /// <summary>
        /// 短信内容
        /// </summary>
        public string contentMsg { get; set; }
        /// <summary>
        /// 发送状态
        /// </summary>
        public bool sendState { get; set; }
        /// <summary>
        /// 短信接口返回的响应字串【远程ID】
        /// </summary>
        public string returnMsg { get; set; }
    }
}