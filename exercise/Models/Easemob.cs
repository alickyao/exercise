using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 环信即时通信
/// </summary>
namespace cyclonestyle.Models
{
    /// <summary>
    /// 环信注册请求
    /// </summary>
    public class RegisterUserEasemobRequestModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }
    }

    /// <summary>
    /// 文本消息
    /// </summary>
    public class EasemobTextMesModel {
        /// <summary>
        /// 消息类型
        /// </summary>
        public string type {
            get { return "txt"; }
        }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string msg { get; set; }
    }
    /// <summary>
    /// 图片消息
    /// </summary>
    public class EasemobPicMesModel
    {

    }
    /// <summary>
    /// 声音消息
    /// </summary>
    public class EasemobSoundMesModel
    {

    }

    /// <summary>
    /// 发送环信消息请求
    /// </summary>
    public class SendMessageRequestModel {
        private string _target_type = "users";//默认为发送给用户
        private string[] _target = { "7181210@qq.com" };//默认为联系客服
        /// <summary>
        /// users 给用户发消息, chatgroups 给群发消息, chatrooms 给聊天室发消息
        /// 默认 users
        /// </summary>
        public string target_type {
            get { return _target_type; }
            set { _target_type = value; }
        }
        /// <summary>
        /// 接收的用户，默认为客服
        /// </summary>
        public string[] target {
            get { return _target; }
            set { _target = value; }
        }
        /// <summary>
        /// 文本消息内容
        /// </summary>
        public EasemobTextMesModel msg { get; set; }
        /// <summary>
        /// 发送者
        /// </summary>
        public string from { get; set; }
    }
}